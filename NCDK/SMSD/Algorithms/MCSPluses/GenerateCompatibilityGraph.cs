/*
 *
 * Copyright (C) 2006-2010  Syed Asad Rahman <asad@ebi.ebi.ac.uk>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received iIndex copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Common.Base;
using NCDK.SMSD.Algorithms.Matchers;
using NCDK.SMSD.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NCDK.SMSD.Algorithms.MCSPluses
{
    /// <summary>
    /// This class generates compatibility graph between query and target molecule.
    /// It also markes edges in the compatibility graph as c-edges or d-edges.
    /// </summary>
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    public sealed class GenerateCompatibilityGraph
    {
        private List<int> compGraphNodes = null;
        private List<int> compGraphNodesCZero = null;
        private List<int> cEdges = null;
        private List<int> dEdges = null;
        private int cEdgesSize = 0;
        private int dEdgesSize = 0;
        private IAtomContainer source = null;
        private IAtomContainer target = null;
        private bool shouldMatchBonds = false;

        /// <summary>
       /// Default constructor added
       /// </summary>
        public GenerateCompatibilityGraph()
        {
        }

        /// <summary>
        /// Generates a compatibility graph between two molecules
        /// </summary>
        /// <exception cref="System.IO.IOException"></exception>
        public GenerateCompatibilityGraph(IAtomContainer source, IAtomContainer target, bool shouldMatchBonds)
        {
            IsMatchBond = shouldMatchBonds;
            this.source = source;
            this.target = target;
            compGraphNodes = new List<int>();
            compGraphNodesCZero = new List<int>();
            cEdges = new List<int>();
            dEdges = new List<int>();
            CompatibilityGraphNodes();
            CompatibilityGraph();

            if (CEdgesSize == 0)
            {
                ClearCompGraphNodes();

                ClearCEgdes();
                ClearDEgdes();

                ReSetCEdgesSize();
                ReSetDEdgesSize();

                CompatibilityGraphNodesIfCEdgeIsZero();
                CompatibilityGraphCEdgeZero();
                ClearCompGraphNodesCZero();
            }
        }

        private List<List<int>> LabelAtoms(IAtomContainer atomCont)
        {
            List<List<int>> labelList = new List<List<int>>();

            for (int i = 0; i < atomCont.Atoms.Count; i++)
            {
                LabelContainer labelContainer = LabelContainer.Instance;
                List<int> label = new List<int>(7);
                //            label.SetSize(7);

                for (int a = 0; a < 7; a++)
                {
                    label.Insert(a, 0);
                }

                IAtom refAtom = atomCont.Atoms[i];
                string atom1Type = refAtom.Symbol;

                label[0] = labelContainer.GetLabelID(atom1Type);

                int countNeighbors = 1;
                var connAtoms = atomCont.GetConnectedAtoms(refAtom);

                foreach (var negAtom in connAtoms)
                {
                    string atom2Type = negAtom.Symbol;
                    label[countNeighbors++] = labelContainer.GetLabelID(atom2Type);
                }

                BubbleSort(label);
                labelList.Add(label);

            }
            return labelList;
        }

        private void BubbleSort(List<int> label)
        {

            bool flag = true; // set flag to 1 to begin initial pass

            int temp; // holding variable

            for (int i = 0; i < 7 && flag; i++)
            {
                flag = false;
                for (int j = 0; j < 6; j++)
                {
                    if (label[i] > label[j + 1])
                    {
                        // descending order simply changes to >
                        temp = label[i]; // swap elements

                        label[i] = label[j + 1];
                        label[j + 1] = temp;
                        flag = true; // indicates that iIndex swap occurred.
                    }
                }
            }
        }

        private List<IAtom> ReduceAtomSet(IAtomContainer atomCont)
        {

            List<IAtom> basicAtoms = new List<IAtom>();
            foreach (var atom in atomCont.Atoms)
            {
                basicAtoms.Add(atom);
            }
            return basicAtoms;
        }

        /// <summary>
        /// Generate Compatibility Graph Nodes
        /// </summary>
        /// <exception cref="System.IO.IOException"></exception>
        internal int CompatibilityGraphNodes()
        {
            compGraphNodes.Clear();
            List<IAtom> basicAtomVecA = null;
            List<IAtom> basicAtomVecB = null;
            IAtomContainer reactant = source;
            IAtomContainer product = target;

            basicAtomVecA = ReduceAtomSet(reactant);
            basicAtomVecB = ReduceAtomSet(product);

            List<List<int>> labelListMolA = LabelAtoms(reactant);
            List<List<int>> labelListMolB = LabelAtoms(product);

            int molANodes = 0;
            int countNodes = 1;

            foreach (var labelA in labelListMolA)
            {
                int molBNodes = 0;
                foreach (var labelB in labelListMolB)
                {
                    if (Compares.AreEqual(labelA, labelB))
                    {
                        compGraphNodes.Add(reactant.Atoms.IndexOf(basicAtomVecA[molANodes]));
                        compGraphNodes.Add(product.Atoms.IndexOf(basicAtomVecB[molBNodes]));
                        compGraphNodes.Add(countNodes++);
                    }
                    molBNodes++;
                }
                molANodes++;
            }
            return 0;
        }

        /// <summary>
        /// Generate Compatibility Graph Nodes Bond Insensitive
        /// </summary>
        /// <exception cref="System.IO.IOException"></exception>
        internal int CompatibilityGraph()
        {
            int compGraphNodesListSize = compGraphNodes.Count;

            cEdges = new List<int>(); //Initialize the cEdges List
            dEdges = new List<int>(); //Initialize the dEdges List

            for (int a = 0; a < compGraphNodesListSize; a += 3)
            {
                int indexA = compGraphNodes[a];
                int indexAPlus1 = compGraphNodes[a + 1];

                for (int b = a + 3; b < compGraphNodesListSize; b += 3)
                {
                    int indexB = compGraphNodes[b];
                    int indexBPlus1 = compGraphNodes[b + 1];

                    // if element atomCont !=jIndex and atoms on the adjacent sides of the bonds are not equal
                    if (a != b && indexA != indexB && indexAPlus1 != indexBPlus1)
                    {

                        IBond reactantBond = null;
                        IBond productBond = null;

                        reactantBond = source.GetBond(source.Atoms[indexA], source.Atoms[indexB]);
                        productBond = target.GetBond(target.Atoms[indexAPlus1], target.Atoms[indexBPlus1]);
                        if (reactantBond != null && productBond != null)
                        {
                            AddEdges(reactantBond, productBond, a, b);
                        }
                    }
                }
            }
            cEdgesSize = cEdges.Count;
            dEdgesSize = dEdges.Count;
            return 0;
        }

        private void AddEdges(IBond reactantBond, IBond productBond, int iIndex, int jIndex)
        {
            //if (IsMatchBond && BondMatch(ReactantBond, ProductBond)) {
            if (IsMatchFeasible(source, reactantBond, target, productBond, shouldMatchBonds))
            {
                cEdges.Add((iIndex / 3) + 1);
                cEdges.Add((jIndex / 3) + 1);
            }
            else if (reactantBond == null && productBond == null)
            {
                dEdges.Add((iIndex / 3) + 1);
                dEdges.Add((jIndex / 3) + 1);
            }
        }

        /// <summary>
        /// compGraphNodesCZero is used to build up of the edges of the compatibility graph
        /// </summary>
        /// <exception cref="System.IO.IOException"></exception>
        internal int CompatibilityGraphNodesIfCEdgeIsZero()
        {

            int countNodes = 1;
            List<string> map = new List<string>();
            compGraphNodesCZero = new List<int>(); //Initialize the compGraphNodesCZero List
            LabelContainer labelContainer = LabelContainer.Instance;
            compGraphNodes.Clear();

            for (int i = 0; i < source.Atoms.Count; i++)
            {
                for (int j = 0; j < target.Atoms.Count; j++)
                {
                    IAtom atom1 = source.Atoms[i];
                    IAtom atom2 = target.Atoms[j];

                    //You can also check object equal or charge, hydrogen count etc

                    if (string.Equals(atom1.Symbol, atom2.Symbol, StringComparison.OrdinalIgnoreCase) && (!map.Contains(i + "_" + j)))
                    {
                        compGraphNodesCZero.Add(i);
                        compGraphNodesCZero.Add(j);
                        compGraphNodesCZero.Add(labelContainer.GetLabelID(atom1.Symbol)); //i.e C is label 1
                        compGraphNodesCZero.Add(countNodes);
                        compGraphNodes.Add(i);
                        compGraphNodes.Add(j);
                        compGraphNodes.Add(countNodes++);
                        map.Add(i + "_" + j);
                    }
                }
            }
            map.Clear();
            return countNodes;
        }

        /// <summary>
        /// compatibilityGraphCEdgeZero is used to
        /// build up of the edges of the
        /// compatibility graph BIS
        /// </summary>
        /// <exception cref="System.IO.IOException"></exception>
        internal int CompatibilityGraphCEdgeZero()
        {
            int compGraphNodesCZeroListSize = compGraphNodesCZero.Count;
            cEdges = new List<int>(); //Initialize the cEdges List
            dEdges = new List<int>(); //Initialize the dEdges List

            for (int a = 0; a < compGraphNodesCZeroListSize; a += 4)
            {
                int indexA = compGraphNodesCZero[a];
                int indexAPlus1 = compGraphNodesCZero[a + 1];
                for (int b = a + 4; b < compGraphNodesCZeroListSize; b += 4)
                {
                    int indexB = compGraphNodesCZero[b];
                    int indexBPlus1 = compGraphNodesCZero[b + 1];

                    // if element atomCont !=jIndex and atoms on the adjacent sides of the bonds are not equal
                    if ((a != b) && (indexA != indexB) && (indexAPlus1 != indexBPlus1))
                    {
                        IBond reactantBond = null;
                        IBond productBond = null;

                        reactantBond = source.GetBond(source.Atoms[indexA], source.Atoms[indexB]);
                        productBond = target.GetBond(target.Atoms[indexAPlus1], target.Atoms[indexBPlus1]);

                        if (reactantBond != null && productBond != null)
                        {
                            AddCZeroEdges(reactantBond, productBond, a, b);
                        }
                    }
                }
            }

            //Size of C and D edges of the compatibility graph
            cEdgesSize = cEdges.Count;
            dEdgesSize = dEdges.Count;
            return 0;
        }

        private void AddCZeroEdges(IBond reactantBond, IBond productBond, int indexI, int indexJ)
        {
            if (IsMatchFeasible(source, reactantBond, target, productBond, shouldMatchBonds))
            {
                //BondMatch(reactantBond, productBond)
                cEdges.Add((indexI / 4) + 1);
                cEdges.Add((indexJ / 4) + 1);
            }
            if (reactantBond == null && productBond == null)
            {
                dEdges.Add((indexI / 4) + 1);
                dEdges.Add((indexJ / 4) + 1);
            }
        }

        private static bool IsMatchFeasible(IAtomContainer ac1, IBond bondA1, IAtomContainer ac2, IBond bondA2,
                bool shouldMatchBonds)
        {
            //Bond Matcher
            IBondMatcher bondMatcher = new DefaultBondMatcher(ac1, bondA1, shouldMatchBonds);
            //Atom Matcher
            IAtomMatcher atomMatcher1 = new DefaultMCSPlusAtomMatcher(ac1, bondA1.Atoms[0], shouldMatchBonds);
            //Atom Matcher
            IAtomMatcher atomMatcher2 = new DefaultMCSPlusAtomMatcher(ac1, bondA1.Atoms[1], shouldMatchBonds);

            if (DefaultMatcher.IsBondMatch(bondMatcher, ac2, bondA2, shouldMatchBonds)
                    && DefaultMatcher.IsAtomMatch(atomMatcher1, atomMatcher2, ac2, bondA2, shouldMatchBonds))
            {
                return true;
            }
            return false;
        }

        public IList<int> GetCEgdes()
        {
            return new ReadOnlyCollection<int>(cEdges);
        }

        internal IList<int> GetDEgdes()
        {
            return new ReadOnlyCollection<int>(dEdges);
        }

        internal IList<int> GetCompGraphNodes()
        {
            return new ReadOnlyCollection<int>(compGraphNodes);
        }

        internal int CEdgesSize => cEdgesSize;

        internal int DEdgesSize => dEdgesSize;

        internal List<int> GetCompGraphNodesCZero()
        {
            return compGraphNodesCZero;
        }

        internal void ClearCEgdes()
        {
            cEdges.Clear();
        }

        internal void ClearDEgdes()
        {
            dEdges.Clear();
        }

        internal void ClearCompGraphNodes()
        {
            compGraphNodes.Clear();
        }

        internal void ClearCompGraphNodesCZero()
        {
            compGraphNodesCZero.Clear();
        }

        internal void ReSetCEdgesSize()
        {
            cEdgesSize = 0;
        }

        internal void ReSetDEdgesSize()
        {
            dEdgesSize = 0;
        }

        public void Clear()
        {
            cEdges = null;
            dEdges = null;
            compGraphNodes = null;
            compGraphNodesCZero = null;
        }

        /// <summary>
        ///  the shouldMatchBonds
        /// </summary>
        public bool IsMatchBond
        {
            get
            {
                return shouldMatchBonds;
            }
            set
            {
                this.shouldMatchBonds = value;
            }
        }
    }
}
