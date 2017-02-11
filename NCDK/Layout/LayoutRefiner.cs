/*
 * Copyright (c) 2015 John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using NCDK.Common.Mathematics;
using NCDK.Graphs;
using NCDK.Tools.Manipulator;
using System.Collections.Generic;
using NCDK.Numerics;
using System;
using NCDK.Common.Collections;
using System.Linq;
using System.Diagnostics;

namespace NCDK.Layout
{
    /**
     * An overlap resolver that tries to resolve overlaps by rotating (reflecting),
     * bending, and stretching bonds. <p/>
     * 
     * The RBS (rotate, bend, stretch) algorithm is first described by {@cdk.cite Shelley83},
     * and later in more detail by {@cdk.cite HEL99}.
     * <p/>
     * Essentially we have a measure of {@link Congestion}. From that we find 
     * un-bonded atoms that contribute significantly (i.e. overlap). To resolve
     * that overlap we try resolving the overlap by changing (acyclic) bonds in the
     * shortest path between the congested pair. Operations, from most to least 
     * favourable, are:
     * <ul>
     *     <li>Rotation (or reflection), {@link #Rotate(Collection)}</li>
     *     <li>Inversion (not described in lit), {@link #Invert(Collection)}</li>
     *     <li>Stretch, {@link #Stretch(AtomPair, IntStack, Vector2[])}</li>
     *     <li>Bend, {@link #Bend(AtomPair, IntStack, Vector2[])}</li>
     * </ul>
     */
    sealed class LayoutRefiner
    {

        /**
         * These value are constants but could be parametrised in future.
         */

        // bond length should be changeable
        private const double BOND_LENGTH = 1.5;

        // Min dist between un-bonded atoms, making the denominator smaller means
        // we want to spread atoms out more
        private const double MIN_DIST = BOND_LENGTH / 2;

        // Min score is derived from the min distance
        private const double MIN_SCORE = 1 / (MIN_DIST * MIN_DIST);

        // How much do we add to a bond when making it longer.
        private const double STRETCH_STEP = 0.32 * BOND_LENGTH;

        // How much we bend bonds by
        private readonly double BEND_STEP = Vectors.DegreeToRadian(10);

        // Ensure we don't stretch bonds too long.
        private const double MAX_BOND_LENGTH = 2 * BOND_LENGTH;

        // Only accept if improvement is >= 2%. I don't like this because
        // huge structures will have less improvement even though the overlap
        // was resolved.
        public const double IMPROVEMENT_PERC_THRESHOLD = 0.02;

        // Rotation (reflection) is always good if it improves things
        // since we're not distorting the layout. Rather than use the
        // percentage based threshold we accept an modification if
        // the improvement is this much better.
        public const int ROTATE_DELTA_THRESHOLD = 5;


        // Maximum number of iterations whilst improving
        private const int MAX_ITERATIONS = 10;

        // fast lookup structures
        private readonly IAtomContainer mol;
        private readonly IDictionary<IAtom, int> idxs;
        private readonly int[][] adjList;
        private readonly GraphUtil.EdgeToBondMap bondMap;
        private readonly IAtom[] atoms;

        // measuring and finding congestion
        private readonly Congestion congestion;
        private readonly AllPairsShortestPaths apsp;

        // buffers where we can store and restore different solutions
        private readonly Vector2[] buffer1, buffer2, backup;
        private readonly IntStack stackBackup;
        private readonly bool[] visited;

        // ring system index, allows us to quickly tell if two atoms are
        // in the same ring system
        private readonly int[] ringsystems;

        /**
         * Create a new layout refiner for the provided molecule.
         * 
         * @param mol molecule to refine
         */
        public LayoutRefiner(IAtomContainer mol)
        {
            this.mol = mol;
            this.bondMap = GraphUtil.EdgeToBondMap.WithSpaceFor(mol);
            this.adjList = GraphUtil.ToAdjList(mol, bondMap);
            this.idxs = new Dictionary<IAtom, int>();
            foreach (var atom in mol.Atoms)
                idxs[atom] = idxs.Count;
            this.atoms = AtomContainerManipulator.GetAtomArray(mol);

            // buffers for storing coordinates
            this.buffer1 = new Vector2[atoms.Length];
            this.buffer2 = new Vector2[atoms.Length];
            this.backup = new Vector2[atoms.Length];
            for (int i = 0; i < buffer1.Length; i++)
            {
                buffer1[i] = new Vector2();
                buffer2[i] = new Vector2();
                backup[i] = new Vector2();
            }
            this.stackBackup = new IntStack(atoms.Length);
            this.visited = new bool[atoms.Length];

            this.congestion = new Congestion(mol, adjList);

            // note, this is lazy so only does the shortest path when needed
            // and does |V| search at maximum
            this.apsp = new AllPairsShortestPaths(mol);

            // index ring systems, idx -> ring system number (rnum)
            int rnum = 1;
            this.ringsystems = new int[atoms.Length];
            for (int i = 0; i < atoms.Length; i++)
            {
                if (atoms[i].IsInRing && ringsystems[i] == 0)
                    TraverseRing(ringsystems, i, rnum++);
            }
        }

        /**
         * Simple method for marking ring systems with a flood-fill.
         *
         * @param ringSystem ring system vector
         * @param v          start atom
         * @param rnum       the number to mark atoms of this ring
         */
        private void TraverseRing(int[] ringSystem, int v, int rnum)
        {
            ringSystem[v] = rnum;
            foreach (var w in adjList[v])
            {
                if (ringSystem[w] == 0 && bondMap[v, w].IsInRing)
                    TraverseRing(ringSystem, w, rnum);
            }
        }

        /**
         * Find all pairs of un-bonded atoms that are congested.
         *
         * @return pairs of congested atoms
         */
        List<AtomPair> FindCongestedPairs()
        {

            List<AtomPair> pairs = new List<AtomPair>();

            // only add a single pair between each ring system, otherwise we
            // may have many pairs that are actually all part of the same
            // congestion
            var ringpairs = new HashSet<IntTuple>();

            // score at which to check for crossing bonds
            double maybeCrossed = 1 / (2 * 2);

            int numAtoms = mol.Atoms.Count;
            for (int u = 0; u < numAtoms; u++)
            {
                for (int v = u + 1; v < numAtoms; v++)
                {
                    double contribution = congestion.Contribution(u, v);
                    // <0 = bonded
                    if (contribution <= 0)
                        continue;

                    // we don't modify ring bonds with the class to when the atoms
                    // same ring systems we can't reduce the congestion
                    if (ringsystems[u] > 0 && ringsystems[u] == ringsystems[v])
                        continue;

                    // an un-bonded atom pair is congested if they're and with a certain distance
                    // or any of their bonds are crossing
                    if (contribution >= MIN_SCORE || contribution >= maybeCrossed && haveCrossingBonds(u, v))
                    {

                        int uWeight = mol.Atoms[u].GetProperty<int>(AtomPlacer.PRIORITY);
                        int vWeight = mol.Atoms[v].GetProperty<int>(AtomPlacer.PRIORITY);

                        int[] path = uWeight > vWeight ? apsp.From(u).GetPathTo(v)
                                                       : apsp.From(v).GetPathTo(u);

                        // something not right here if the len is < 3
                        int len = path.Length;
                        if (len < 3) continue;

                        // build the seqAt and bndAt lists from shortest path
                        int[] seqAt = new int[len - 2];
                        IBond[] bndAt = new IBond[len - 1];
                        MakeAtmBndQueues(path, seqAt, bndAt);

                        // we already know about this collision between these ring systems
                        // so dont add the pair
                        if (ringsystems[u] > 0 && ringsystems[v] > 0 &&
                                !ringpairs.Add(new IntTuple(ringsystems[u], ringsystems[v])))
                            continue;

                        // add to pairs to overlap
                        pairs.Add(new AtomPair(u, v, seqAt, bndAt));
                    }
                }
            }

            // sort the pairs to attempt consistent overlap resolution (order independent)
            pairs.Sort(new AtomPairComparer(this));

            return pairs;
        }

        class AtomPairComparer : IComparer<AtomPair>
        {
            LayoutRefiner parent;
            public AtomPairComparer(LayoutRefiner parent)
            {
                this.parent = parent;
            }

            public int Compare(AtomPair a, AtomPair b)
            {
                int a1 = parent.atoms[a.fst].GetProperty<int>(AtomPlacer.PRIORITY);
                int a2 = parent.atoms[a.snd].GetProperty<int>(AtomPlacer.PRIORITY);
                int b1 = parent.atoms[b.fst].GetProperty<int>(AtomPlacer.PRIORITY);
                int b2 = parent.atoms[b.snd].GetProperty<int>(AtomPlacer.PRIORITY);
                int amin, amax;
                int bmin, bmax;
                if (a1 < a2)
                {
                    amin = a1;
                    amax = a2;
                }
                else
                {
                    amin = a2;
                    amax = a1;
                }
                if (b1 < b2)
                {
                    bmin = a1;
                    bmax = a2;
                }
                else
                {
                    bmin = a2;
                    bmax = a1;
                }
                int cmp = amin.CompareTo(bmin);
                if (cmp != 0) return cmp;
                return amax.CompareTo(bmax);
            }
        }

        /**
         * Check if two bonds are crossing.
         *
         * @param beg1 first atom of first bond
         * @param end1 second atom of first bond
         * @param beg2 first atom of second bond
         * @param end2 first atom of second bond
         * @return bond is crossing
         */
        private bool IsCrossed(Vector2 beg1, Vector2 end1, Vector2 beg2, Vector2 end2)
        {
            return Vectors.LinesIntersect(beg1.X, beg1.Y, end1.X, end1.Y, beg2.X, beg2.Y, end2.X, end2.Y);
        }

        /**
         * Check if any of the bonds adjacent to u, v (not bonded) are crossing.
         *
         * @param u an atom (idx)
         * @param v another atom (idx)
         * @return there are crossing bonds
         */
        private bool haveCrossingBonds(int u, int v)
        {
            int[] us = adjList[u];
            int[] vs = adjList[v];
            foreach (var u1 in us)
            {
                foreach (var v1 in vs)
                {
                    if (u1 == v || v1 == u || u1 == v1)
                        continue;
                    if (IsCrossed(atoms[u].Point2D.Value, atoms[u1].Point2D.Value, atoms[v].Point2D.Value, atoms[v1].Point2D.Value))
                        return true;
                }
            }
            return false;
        }

        /// <summary>Set of rotatable bonds we've explored and found are probably symmetric.</summary>
        private readonly HashSet<IBond> probablySymmetric = new HashSet<IBond>();

        /**
         * Attempt to reduce congestion through rotation of flippable bonds between
         * congest pairs.
         *
         * @param pairs congested pairs of atoms
         */
        void Rotate(ICollection<AtomPair> pairs)
        {

            // bond has already been tried in this phase so
            // don't need to test again
            var tried = new HashSet<IBond>();

            Pair:
            foreach (var pair in pairs)
            {
                foreach (var bond in pair.bndAt)
                {

                    // only try each bond once per phase and skip
                    if (!tried.Add(bond))
                        continue;

                    // those we have found to probably be symmetric
                    if (probablySymmetric.Contains(bond))
                        continue;

                    // can't rotate these
                    if (bond.Order != BondOrder.Single || bond.IsInRing)
                        continue;

                    IAtom beg = bond.Atoms[0];
                    IAtom end = bond.Atoms[1];
                    int begIdx = idxs[beg];
                    int endIdx = idxs[end];

                    // terminal
                    if (adjList[begIdx].Length == 1 || adjList[endIdx].Length == 1)
                        continue;

                    int begPriority = beg.GetProperty<int>(AtomPlacer.PRIORITY);
                    int endPriority = end.GetProperty<int>(AtomPlacer.PRIORITY);

                    Arrays.Fill(visited, false);
                    if (begPriority < endPriority)
                    {
                        stackBackup.len = VisitAdj(visited, stackBackup.xs, begIdx, endIdx);
                    }
                    else
                    {
                        stackBackup.len = VisitAdj(visited, stackBackup.xs, endIdx, begIdx);
                    }

                    double min = congestion.Score();

                    backupCoords(backup, stackBackup);
                    Reflect(stackBackup, beg, end);
                    congestion.Update(visited, stackBackup.xs, stackBackup.len);

                    double delta = min - congestion.Score();

                    // keep if decent improvement or improvement and resolves this overlap
                    if (delta > ROTATE_DELTA_THRESHOLD ||
                        (delta > 1 && congestion.Contribution(pair.fst, pair.snd) < MIN_SCORE))
                    {
                        goto continue_Pair;
                    }
                    else
                    {

                        // almost no difference from flipping... bond is probably symmetric
                        // mark to avoid in future iterations
                        if (Math.Abs(delta) < 0.1)
                            probablySymmetric.Add(bond);

                        // restore
                        restoreCoords(stackBackup, backup);
                        congestion.Update(visited, stackBackup.xs, stackBackup.len);
                        congestion.score = min;
                    }
                }
            }
            continue_Pair:
            ;

        }

        /**
         * Special case congestion minimisation, rotate terminals bonds around ring
         * systems so they are inside the ring.
         *
         * @param pairs congested atom pairs
         */
        void Invert(IEnumerable<AtomPair> pairs)
        {
            foreach (var pair in pairs)
            {
                if (congestion.Contribution(pair.fst, pair.snd) < MIN_SCORE)
                    continue;
                if (FusionPointInversion(pair))
                    continue;
                if (macroCycleInversion(pair))
                    continue;
            }
        }

        // For substituents attached to macrocycles we may be able to point these in/out
        // of the ring
        private bool macroCycleInversion(AtomPair pair)
        {

            foreach (var v in pair.seqAt)
            {
                IAtom atom = mol.Atoms[v];
                if (!atom.IsInRing || adjList[v].Length == 2)
                    continue;
                if (atom.GetProperty<object>(MacroCycleLayout.MACROCYCLE_ATOM_HINT) == null)
                    continue;
                var acyclic = new List<IBond>(2);
                var cyclic = new List<IBond>(2);
                foreach (var w in adjList[v])
                {
                    IBond bond = bondMap[v, w];
                    if (bond.IsInRing)
                        cyclic.Add(bond);
                    else
                        acyclic.Add(bond);
                }
                if (cyclic.Count > 2)
                    continue;

                foreach (var bond in acyclic)
                {

                    Arrays.Fill(visited, false);
                    stackBackup.len = Visit(visited, stackBackup.xs, v, idxs[bond.GetConnectedAtom(atom)], 0);

                    Vector2 a = atom.Point2D.Value;
                    Vector2 b = bond.GetConnectedAtom(atom).Point2D.Value;

                    Vector2 perp = new Vector2(b.X - a.X, b.Y - a.Y);
                    perp = Vector2.Normalize(perp);
                    double score = congestion.Score();
                    backupCoords(backup, stackBackup);

                    Reflect(stackBackup, new Vector2(a.X - perp.Y, a.Y + perp.X), new Vector2(a.X + perp.Y, a.Y - perp.X));
                    congestion.Update(visited, stackBackup.xs, stackBackup.len);

                    if (PercDiff(score, congestion.Score()) >= IMPROVEMENT_PERC_THRESHOLD)
                    {
                        return true;
                    }

                    restoreCoords(stackBackup, backup);
                }
            }

            return false;
        }

        private bool FusionPointInversion(AtomPair pair)
        {
            // not candidates for inversion
            // > 3 bonds
            if (pair.bndAt.Count != 3)
                return false;
            // we want *!@*@*!@*
            if (!pair.bndAt[0].IsInRing || pair.bndAt[1].IsInRing || pair.bndAt[2].IsInRing)
                return false;
            // non-terminals
            if (adjList[pair.fst].Length > 1 || adjList[pair.snd].Length > 1)
                return false;

            IAtom fst = atoms[pair.fst];

            // choose which one to invert, preffering hydrogens
            stackBackup.Clear();
            if (fst.AtomicNumber == 1)
                stackBackup.Push(pair.fst);
            else
                stackBackup.Push(pair.snd);

            Reflect(stackBackup, pair.bndAt[0].Atoms[0], pair.bndAt[0].Atoms[1]);
            congestion.Update(stackBackup.xs, stackBackup.len);
            return true;
        }

        /**
         * Bend all bonds in the shortest path between a pair of atoms in an attempt
         * to resolve the overlap. The bend that produces the minimum congestion is
         * stored in the provided stack and coords with the congestion score
         * returned.
         *
         * @param pair   congested atom pair
         * @param stack  best result vertices
         * @param coords best result coords
         * @return congestion score of best result
         */
        private double Bend(AtomPair pair, IntStack stack, Vector2[] coords)
        {

            stackBackup.Clear();

            Trace.Assert(stack.len == 0);
            double score = congestion.Score();
            double min = score;

            // special case: if we have an even length path where the two
            // most central bonds are cyclic but the next two aren't we bend away
            // from each other
            if (pair.bndAt.Count > 4 && (pair.bndAtCode & 0x1F) == 0x6)
            {

                IBond bndA = pair.bndAt[2];
                IBond bndB = pair.bndAt[3];

                IAtom pivotA = GetCommon(bndA, pair.bndAt[1]);
                IAtom pivotB = GetCommon(bndB, pair.bndAt[0]);

                if (pivotA == null || pivotB == null)
                    return int.MaxValue;

                Arrays.Fill(visited, false);
                int split = Visit(visited, stack.xs, idxs[pivotA], idxs[bndA.GetConnectedAtom(pivotA)], 0);
                stack.len = Visit(visited, stack.xs, idxs[pivotB], idxs[bndB.GetConnectedAtom(pivotB)], split);

                // perform bend one way
                backupCoords(backup, stack);
                Bend(stack.xs, 0, split, pivotA, BEND_STEP);
                Bend(stack.xs, split, stack.len, pivotB, -BEND_STEP);

                congestion.Update(stack.xs, stack.len);

                if (PercDiff(score, congestion.Score()) >= IMPROVEMENT_PERC_THRESHOLD)
                {
                    backupCoords(coords, stack);
                    stackBackup.CopyFrom(stack);
                    min = congestion.Score();
                }

                // now bend the other way
                restoreCoords(stack, backup);
                Bend(stack.xs, 0, split, pivotA, -BEND_STEP);
                Bend(stack.xs, split, stack.len, pivotB, BEND_STEP);
                congestion.Update(stack.xs, stack.len);
                if (PercDiff(score, congestion.Score()) >= IMPROVEMENT_PERC_THRESHOLD && congestion.Score() < min)
                {
                    backupCoords(coords, stack);
                    stackBackup.CopyFrom(stack);
                    min = congestion.Score();
                }

                // restore original coordinates and reset score
                restoreCoords(stack, backup);
                congestion.Update(stack.xs, stack.len);
                congestion.score = score;
            }
            // general case: try bending acyclic bonds in the shortest
            // path from inside out
            else
            {

                // try bending all bonds and accept the best one
                foreach (var bond in pair.bndAt)
                {
                    if (bond.IsInRing) continue;

                    IAtom beg = bond.Atoms[0];
                    IAtom end = bond.Atoms[1];
                    int begPriority = beg.GetProperty<int>(AtomPlacer.PRIORITY);
                    int endPriority = end.GetProperty<int>(AtomPlacer.PRIORITY);

                    Arrays.Fill(visited, false);
                    if (begPriority < endPriority)
                        stack.len = Visit(visited, stack.xs, idxs[beg], idxs[end], 0);
                    else
                        stack.len = Visit(visited, stack.xs, idxs[end], idxs[beg], 0);

                    backupCoords(backup, stack);

                    // bend one way
                    if (begPriority < endPriority)
                        Bend(stack.xs, 0, stack.len, beg, pair.attempt * BEND_STEP);
                    else
                        Bend(stack.xs, 0, stack.len, end, pair.attempt * BEND_STEP);
                    congestion.Update(visited, stack.xs, stack.len);

                    if (PercDiff(score, congestion.Score()) >= IMPROVEMENT_PERC_THRESHOLD &&
                            congestion.Score() < min)
                    {
                        backupCoords(coords, stack);
                        stackBackup.CopyFrom(stack);
                        min = congestion.Score();
                    }

                    // bend other way
                    if (begPriority < endPriority)
                        Bend(stack.xs, 0, stack.len, beg, pair.attempt * -BEND_STEP);
                    else
                        Bend(stack.xs, 0, stack.len, end, pair.attempt * -BEND_STEP);
                    congestion.Update(visited, stack.xs, stack.len);

                    if (PercDiff(score, congestion.Score()) >= IMPROVEMENT_PERC_THRESHOLD && congestion.Score() < min)
                    {
                        backupCoords(coords, stack);
                        stackBackup.CopyFrom(stack);
                        min = congestion.Score();
                    }

                    restoreCoords(stack, backup);
                    congestion.Update(visited, stack.xs, stack.len);
                    congestion.score = score;
                }
            }

            stack.CopyFrom(stackBackup);

            return min;
        }

        /**
         * Stretch all bonds in the shortest path between a pair of atoms in an
         * attempt to resolve the overlap. The stretch that produces the minimum
         * congestion is stored in the provided stack and coords with the congestion
         * score returned.
         *
         * @param pair   congested atom pair
         * @param stack  best result vertices
         * @param coords best result coords
         * @return congestion score of best result
         */
        private double Stretch(AtomPair pair, IntStack stack, Vector2[] coords)
        {

            stackBackup.Clear();

            double score = congestion.Score();
            double min = score;

            foreach (var bond in pair.bndAt)
            {

                // don't stretch ring bonds
                if (bond.IsInRing)
                    continue;

                IAtom beg = bond.Atoms[0];
                IAtom end = bond.Atoms[1];
                int begIdx = idxs[beg];
                int endIdx = idxs[end];
                int begPriority = beg.GetProperty<int>(AtomPlacer.PRIORITY);
                int endPriority = end.GetProperty<int>(AtomPlacer.PRIORITY);

                Arrays.Fill(visited, false);
                if (begPriority < endPriority)
                    stack.len = Visit(visited, stack.xs, endIdx, begIdx, 0);
                else
                    stack.len = Visit(visited, stack.xs, begIdx, endIdx, 0);

                backupCoords(backup, stack);
                if (begPriority < endPriority)
                    Stretch(stack, end, beg, pair.attempt * STRETCH_STEP);
                else
                    Stretch(stack, beg, end, pair.attempt * STRETCH_STEP);

                congestion.Update(visited, stack.xs, stack.len);

                if (PercDiff(score, congestion.Score()) >= IMPROVEMENT_PERC_THRESHOLD && congestion.Score() < min)
                {
                    backupCoords(coords, stack);
                    min = congestion.Score();
                    stackBackup.CopyFrom(stack);
                }

                restoreCoords(stack, backup);
                congestion.Update(visited, stack.xs, stack.len);
                congestion.score = score;
            }

            stack.CopyFrom(stackBackup);

            return min;
        }


        /**
         * Resolves conflicts either by bending bonds or stretching bonds in the
         * shortest path between an overlapping pair. Bending and stretch are tried
         * for each pair and the best resolution is used.
         *
         * @param pairs pairs
         */
        private void BendOrStretch(IEnumerable<AtomPair> pairs)
        {

            IntStack bendStack = new IntStack(atoms.Length);
            IntStack stretchStack = new IntStack(atoms.Length);

            foreach (var pair in pairs)
            {

                double score = congestion.Score();

                // each attempt will be more aggressive/distorting
                for (pair.attempt = 1; pair.attempt <= 3; pair.attempt++)
                {

                    bendStack.Clear();
                    stretchStack.Clear();

                    // attempt both bending and stretching storing the
                    // best result in the provided buffer
                    double bendScore = Bend(pair, bendStack, buffer1);
                    double stretchScore = Stretch(pair, stretchStack, buffer2);

                    // bending is better than stretching
                    if (bendScore < stretchScore && bendScore < score)
                    {
                        restoreCoords(bendStack, buffer1);
                        congestion.Update(bendStack.xs, bendStack.len);
                        break;
                    }

                    // stretching is better than bending
                    else if (bendScore > stretchScore && stretchScore < score)
                    {
                        restoreCoords(stretchStack, buffer2);
                        congestion.Update(stretchStack.xs, stretchStack.len);
                        break;
                    }

                }
            }
        }

        /**
         * Refine the 2D coordinates of a layout to reduce overlap and congestion.
         */
        public void Refine()
        {
            for (int i = 1; i <= MAX_ITERATIONS; i++)
            {
                var pairs = FindCongestedPairs();

                if (pairs.Count == 0)
                    break;

                var min = congestion.Score();

                // rotation: flipping around sigma bonds
                Rotate(pairs);

                // rotation improved, so try more rotation, we may have caused
                // new conflicts that can be resolved through more rotations
                if (congestion.Score() < min)
                    continue;

                // inversion: terminal atoms can be placed inside rings
                // which is preferable to bending or stretching
                Invert(pairs);

                if (congestion.Score() < min)
                    continue;

                // bending or stretching: least favourable but sometimes
                // the only way. We try either and use the best
                BendOrStretch(pairs);

                if (congestion.Score() < min)
                    continue;

                break;
            }
        }

        /**
         * Backup the coordinates of atoms (idxs) in the stack to the provided
         * destination.
         *
         * @param dest  destination
         * @param stack atom indexes to backup
         */
        private void backupCoords(Vector2[] dest, IntStack stack)
        {
            for (int i = 0; i < stack.len; i++)
            {
                int v = stack.xs[i];
                dest[v] = new Vector2(atoms[v].Point2D.Value.X, atoms[v].Point2D.Value.Y);
            }
        }

        /**
         * Restore the coordinates of atoms (idxs) in the stack to the provided
         * source.
         *
         * @param stack atom indexes to backup
         * @param src   source of coordinates
         */
        private void restoreCoords(IntStack stack, Vector2[] src)
        {
            for (int i = 0; i < stack.len; i++)
            {
                int v = stack.xs[i];
                atoms[v].Point2D = new Vector2(src[v].X, src[v].Y);
            }
        }

        /**
         * Reflect all atoms (indexes) int he provided stack around the line formed
         * of the beg and end atoms.
         *
         * @param stack atom indexes to reflect
         * @param beg   beg atom of a bond
         * @param end   end atom of a bond
         */
        private void Reflect(IntStack stack, IAtom beg, IAtom end)
        {
            Vector2 begP = beg.Point2D.Value;
            Vector2 endP = end.Point2D.Value;
            Reflect(stack, begP, endP);
        }

        private void Reflect(IntStack stack, Vector2 begP, Vector2 endP)
        {
            double dx = endP.X - begP.X;
            double dy = endP.Y - begP.Y;

            double a = (dx * dx - dy * dy) / (dx * dx + dy * dy);
            double b = 2 * dx * dy / (dx * dx + dy * dy);

            for (int i = 0; i < stack.len; i++)
            {
                Reflect(atoms[stack.xs[i]], begP, a, b);
            }
        }

        /**
         * Reflect a point (p) in a line formed of 'base', 'a', and 'b'.
         *
         * @param p    point to reflect
         * @param base base of the refection source
         * @param a    a reflection coef
         * @param b    b reflection coef
         */
        private static void Reflect(IAtom ap, Vector2 base_, double a, double b)
        {
            double x = a * (ap.Point2D.Value.X - base_.X) + b * (ap.Point2D.Value.Y - base_.Y) + base_.X;
            double y = b * (ap.Point2D.Value.X - base_.X) - a * (ap.Point2D.Value.Y - base_.Y) + base_.Y;
            ap.Point2D = new Vector2(x, y);
        }


        /**
         * Bend select atoms around a provided pivot by the specified amount (r).
         *
         * @param indexes  array of atom indexes
         * @param from     start offset into the array (inclusive)
         * @param to       end offset into the array (exclusive)
         * @param pivotAtm the point about which we are pivoting
         * @param r        radians to bend by
         */
        private void Bend(int[] indexes, int from, int to, IAtom pivotAtm, double r)
        {
            double s = Math.Sin(r);
            double c = Math.Cos(r);
            Vector2 pivot = pivotAtm.Point2D.Value;
            for (int i = from; i < to; i++)
            {
                var atom = mol.Atoms[indexes[i]];
                Vector2 p = atom.Point2D.Value;
                double x = p.X - pivot.X;
                double y = p.Y - pivot.Y;
                double nx = x * c + y * s;
                double ny = -x * s + y * c;
                atom.Point2D = new Vector2(nx + pivot.X, ny + pivot.Y);
            }
        }

        /**
         * Stretch the distance between beg and end, moving all atoms provided in
         * the stack.
         *
         * @param stack  atoms to be moved
         * @param beg    begin atom of a bond
         * @param end    end atom of a bond
         * @param amount amount to try stretching by (absolute)
         */
        private void Stretch(IntStack stack, IAtom beg, IAtom end, double amount)
        {
            Vector2 begPoint = beg.Point2D.Value;
            Vector2 endPoint = end.Point2D.Value;

            if (Vector2.Distance(begPoint, endPoint) + amount > MAX_BOND_LENGTH)
                return;

            Vector2 vector = new Vector2(endPoint.X - begPoint.X, endPoint.Y - begPoint.Y);
            vector = Vector2.Normalize(vector);
            vector *= amount;

            for (int i = 0; i < stack.len; i++)
            {
                var atom = atoms[stack.xs[i]];
                atom.Point2D = atom.Point2D.Value + vector;
            }
        }


        /**
         * Internal - makes atom (seq) and bond priority queues for resolving
         * overlap. Only (acyclic - but not really) atoms and bonds in the shortest
         * path between the two atoms can resolve an overlap. We create prioritised
         * sequences of atoms/bonds where the more central in the shortest path.
         *
         * @param path  shortest path between atoms
         * @param seqAt prioritised atoms, first atom is the middle of the path
         * @param bndAt prioritised bonds, first bond is the middle of the path
         */
        private void MakeAtmBndQueues(int[] path, int[] seqAt, IBond[] bndAt)
        {
            int len = path.Length;
            int i = (len - 1) / 2;
            int j = i + 1;
            int nSeqAt = 0;
            int nBndAt = 0;
            if (IsOdd((path.Length)))
            {
                seqAt[nSeqAt++] = path[i--];
                bndAt[nBndAt++] = bondMap[path[j], path[j - 1]];
            }
            bndAt[nBndAt++] = bondMap[path[i], path[i + 1]];
            while (i > 0 && j < len - 1)
            {
                seqAt[nSeqAt++] = path[i--];
                seqAt[nSeqAt++] = path[j++];
                bndAt[nBndAt++] = bondMap[path[i], path[i + 1]];
                bndAt[nBndAt++] = bondMap[path[j], path[j - 1]];
            }
        }

        // is a number odd
        private static bool IsOdd(int len)
        {
            return (len & 0x1) != 0;
        }

        // percentage difference
        private static double PercDiff(double prev, double curr)
        {
            return (prev - curr) / prev;
        }

        /**
         * Recursively visit 'v' and all vertices adjacent to it (excluding 'p')
         * adding all except 'v' to the result array.
         *
         * @param visited visit flags array, should be cleared before search
         * @param result  visited vertices
         * @param p       previous vertex
         * @param v       start vertex
         * @return number of visited vertices
         */
        private int VisitAdj(bool[] visited, int[] result, int p, int v)
        {
            int n = 0;
            Arrays.Fill(visited, false);
            visited[v] = true;
            foreach (var w in adjList[v])
            {
                if (w != p && !visited[w])
                {
                    n = Visit(visited, result, v, w, n);
                }
            }
            visited[v] = false;
            return n;
        }

        /**
         * Recursively visit 'v' and all vertices adjacent to it (excluding 'p')
         * adding them to the result array.
         *
         * @param visited visit flags array, should be cleared before search
         * @param result  visited vertices
         * @param p       previous vertex
         * @param v       start vertex
         * @param n       current number of visited vertices
         * @return new number of visited vertices
         */
        private int Visit(bool[] visited, int[] result, int p, int v, int n)
        {
            visited[v] = true;
            result[n++] = v;
            foreach (var w in adjList[v])
            {
                if (w != p && !visited[w])
                {
                    n = Visit(visited, result, v, w, n);
                }
            }
            return n;
        }


        /**
         * Access the common atom shared by two bonds.
         *
         * @param bndA first bond
         * @param bndB second bond
         * @return common atom or null if non exists
         */
        private static IAtom GetCommon(IBond bndA, IBond bndB)
        {
            IAtom beg = bndA.Atoms[0];
            IAtom end = bndA.Atoms[1];
            if (bndB.Contains(beg))
                return beg;
            else if (bndB.Contains(end))
                return end;
            else
                return null;
        }

        /**
         * Congested pair of un-bonded atoms, described by the index of the atoms
         * (fst, snd). The atoms (seqAt) and bonds (bndAt) in the shortest path
         * between the pair are stored as well as a bndAtCode for checking special
         * case ring bond patterns.
         */
        sealed class AtomPair
        {
            internal readonly int fst, snd;
            internal readonly int[] seqAt;
            internal readonly IList<IBond> bndAt;
            internal readonly int bndAtCode;

            /**
             * Which attempt are we trying to resolve this overlap with.
             */
            public int attempt = 1;

            public AtomPair(int fst, int snd, int[] seqAt, IList<IBond> bndAt)
            {
                this.fst = fst;
                this.snd = snd;
                this.seqAt = seqAt;
                this.bndAt = bndAt;
                this.bndAtCode = bndCode(bndAt);
            }

            public override bool Equals(object o)
            {
                if (this == o) return true;
                if (o == null || GetType() != o.GetType()) return false;

                AtomPair pair = (AtomPair)o;

                return (fst == pair.fst && snd == pair.snd) || (fst == pair.snd && snd == pair.fst);
            }

            public override int GetHashCode()
            {
                return fst ^ snd;
            }

            /**
             * Create the bond code bit mask, lowest bit is whether the path is
             * odd/even then the other bits are whether the bonds are in a ring or
             * not.
             *
             * @param bonds bonds to encode
             * @return the bond code
             */
            static int bndCode(IEnumerable<IBond> enumBonds)
            {
                var bonds = enumBonds.ToList();
                int code = bonds.Count & 0x1;
                for (int i = 0; i < bonds.Count; i++)
                {
                    if (bonds[i].IsInRing)
                    {
                        code |= 0x1 << (i + 1);
                    }
                }
                return code;
            }
        }

        /**
         * Internal - fixed size integer stack.
         */
        private sealed class IntStack
        {
            internal readonly int[] xs;
            internal int len;

            public IntStack(int cap)
            {
                this.xs = new int[cap];
            }

            public void Push(int x)
            {
                xs[len++] = x;
            }

            public void Clear()
            {
                this.len = 0;
            }

            public void CopyFrom(IntStack stack)
            {
                Array.Copy(stack.xs, 0, xs, 0, stack.len);
                this.len = stack.len;
            }

            public override string ToString()
            {
                return Arrays.ToJavaString(Arrays.CopyOf(xs, len));
            }
        }

        /**
         * Internal - A hashable tuple of integers, allows to check for previously
         * seen pairs.
         */
        private sealed class IntTuple
        {
            private readonly int fst, snd;

            public IntTuple(int fst, int snd)
            {
                this.fst = fst;
                this.snd = snd;
            }

            public override bool Equals(Object o)
            {
                if (this == o) return true;
                if (o == null || GetType() != o.GetType()) return false;

                IntTuple that = (IntTuple)o;


                return (this.fst == that.fst && this.snd == that.snd) ||
                        (this.fst == that.snd && this.snd == that.fst);

            }

            public override int GetHashCode()
            {
                return fst ^ snd;
            }
        }
    }
}
