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

using NCDK.Common.Collections;
using NCDK.Smiles;
using System.Collections.Generic;
using static NCDK.Graphs.GraphUtil;
using System;
using System.Collections;
using System.Linq;
using NCDK.Graphs;
using NCDK.SGroups;
using NCDK.Tools.Manipulator;
using System.Collections.ObjectModel;
using NCDK.Isomorphisms.Matchers;
using NCDK.Isomorphisms.Matchers.SMARTS;
using static NCDK.Isomorphisms.Matchers.SMARTS.LogicalOperatorAtom;
using System.IO;
using System.Text;
using System.Reflection;

namespace NCDK.Depict
{
    /**
     * Utility class for abbreviating (sub)structures. Using either self assigned structural
     * motifs or pre-loading a common set a structure depiction can be made more concise with
     * the use of abbreviations (sometimes called superatoms). <p/>
     *
     * Basic usage:
     * <code>{@code
     * Abbreviations abrv = new Abbreviations();
     *
     * // add some abbreviations, when overlapping (e.g. Me,Et,tBu) first one wins
     * abrv.Add("[Na+].[H-] NaH");
     * abrv.Add("*c1ccccc1 Ph");
     * abrv.Add("*C(C)(C)C tBu");
     * abrv.Add("*CC Et");
     * abrv.Add("*C Me");
     *
     * // maybe we don't want 'Me' in the depiction
     * abrv.SetEnabled("Me", false);
     *
     * // assign abbreviations with some filters
     * int numAdded = abrv.Apply(mol);
     *
     * // generate all but don't assign, need to be added manually
     * // set/update the CDKConstants.CTAB_SGROUPS property of mol
     * List<Sgroup> sgroups = abrv.Generate(mol);
     * }</code>
     *
     * Predefined sets of abbreviations can be loaded, the following are
     * on the classpath.
     *
     * <code>{@code
     * // https://www.github.com/openbabel/superatoms
     * abrv.LoadFromFile("obabel_superatoms.smi");
     * }</code>
     *
     * @cdk.keyword abbreviate
     * @cdk.keyword depict
     * @cdk.keyword superatom
     * @see CDKConstants#CTAB_SGROUPS
     * @see Sgroup
     */
    public class Abbreviations : IEnumerable<string>
    {
        private const int MAX_FRAG = 50;

        private readonly IDictionary<string, string> connectedAbbreviations = new SortedDictionary<string, string>();
        private readonly IDictionary<string, string> disconnectedAbbreviations = new SortedDictionary<string, string>();
        private readonly ISet<string> labels = new LinkedHashSet<string>();
        private readonly ISet<string> disabled = new HashSet<string>();
        private readonly SmilesGenerator usmigen = SmilesGenerator.Unique();

        private readonly SmilesParser smipar = new SmilesParser(Silent.ChemObjectBuilder.Instance);

        public Abbreviations()
        {
        }

        /// <summary>
        ///  Iterate over loaded abbreviations. Both enabled and disabled abbreviations are listed.
        /// </summary>
        /// <returns>the abbreviations labels (e.g. Ph, Et, Me, OAc, etc.)</returns>
        public IEnumerator<string> GetEnumerator() => labels.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Check whether an abbreviation is enabled.
        /// </summary>
        /// <param name="label">is enabled</param>
        /// <returns>the label is enabled</returns>
        public bool IsEnabled(string label)
        {
            return labels.Contains(label) && !disabled.Contains(label);
        }

        /// <summary>
        /// Set whether an abbreviation is enabled or disabled.
        /// </summary>
        /// <param name="label">the label (e.g. Ph, Et, Me, OAc, etc.)</param>
        /// <param name="enabled">flag the label as enabled or disabled</param>
        /// <returns>the label state was modified</returns>
        public bool SetEnabled(string label, bool enabled)
        {
            return enabled ? labels.Contains(label) && disabled.Remove(label)
                           : labels.Contains(label) && disabled.Add(label);
        }

        private static ISet<IBond> FindCutBonds(IAtomContainer mol, EdgeToBondMap bmap, int[][] adjlist)
        {
            var cuts = new HashSet<IBond>();
            int numAtoms = mol.Atoms.Count;
            for (int i = 0; i < numAtoms; i++)
            {
                var atom = mol.Atoms[i];
                int deg = adjlist[i].Length;
                int elem = atom.AtomicNumber.Value;

                if (elem == 6 && deg <= 2 || deg < 2)
                    continue;

                foreach (var w in adjlist[i])
                {
                    IBond bond = bmap[i, w];
                    if (adjlist[w].Length >= 2 && !bond.IsInRing)
                    {
                        cuts.Add(bond);
                    }
                }
            }
            return cuts;
        }

        private const string CUT_BOND = "cutbond";

        private static List<IAtomContainer> MakeCut(IBond cut, IAtomContainer mol, Dictionary<IAtom, int> idx, int[][] adjlist)
        {
            IAtom beg = cut.Atoms[0];
            IAtom end = cut.Atoms[1];

            var bvisit = new LinkedHashSet<IAtom>();
            var evisit = new LinkedHashSet<IAtom>();
            var queue = new ArrayDeque<IAtom>();

            bvisit.Add(beg);
            evisit.Add(end);

            queue.Add(beg);
            bvisit.Add(end); // stop visits
            while (queue.Any())
            {
                IAtom atom = queue.Poll();
                bvisit.Add(atom);
                foreach (var w in adjlist[idx[atom]])
                {
                    IAtom nbr = mol.Atoms[w];
                    if (!bvisit.Contains(nbr))
                        queue.Add(nbr);
                }
            }
            bvisit.Remove(end);

            queue.Add(end);
            evisit.Add(beg); // stop visits
            while (queue.Any())
            {
                IAtom atom = queue.Poll();
                evisit.Add(atom);
                foreach (var w in adjlist[idx[atom]])
                {
                    IAtom nbr = mol.Atoms[w];
                    if (!evisit.Contains(nbr))
                        queue.Add(nbr);
                }
            }
            evisit.Remove(beg);

            IChemObjectBuilder bldr = mol.Builder;
            IAtomContainer bfrag = bldr.CreateAtomContainer();
            IAtomContainer efrag = bldr.CreateAtomContainer();

            int diff = bvisit.Count - evisit.Count;

            if (diff < -10)
                evisit.Clear();
            else if (diff > 10)
                bvisit.Clear();

            if (bvisit.Any())
            {
                bfrag.Atoms.Add(bldr.CreatePseudoAtom());
                foreach (var atom in bvisit)
                    bfrag.Atoms.Add(atom);
                bfrag.AddBond(bfrag.Atoms[0], bfrag.Atoms[1], cut.Order);
                bfrag.Bonds[0].SetProperty(CUT_BOND, cut);
            }

            if (evisit.Any())
            {
                efrag.Atoms.Add(bldr.CreatePseudoAtom());
                foreach (var atom in evisit)
                    efrag.Atoms.Add(atom);
                efrag.AddBond(efrag.Atoms[0], efrag.Atoms[1], cut.Order);
                efrag.Bonds[0].SetProperty(CUT_BOND, cut);
            }

            foreach (var bond in mol.Bonds)
            {
                IAtom a1 = bond.Atoms[0];
                IAtom a2 = bond.Atoms[1];
                if (bvisit.Contains(a1) && bvisit.Contains(a2))
                    bfrag.Bonds.Add(bond);
                else if (evisit.Contains(a1) && evisit.Contains(a2))
                    efrag.Bonds.Add(bond);
            }

            var res = new List<IAtomContainer>();
            if (!bfrag.IsEmpty)
                res.Add(bfrag);
            if (!efrag.IsEmpty)
                res.Add(efrag);
            return res;
        }

        private static List<IAtomContainer> GenerateFragments(IAtomContainer mol)
        {
            EdgeToBondMap bmap = EdgeToBondMap.WithSpaceFor(mol);
            int[][] adjlist = GraphUtil.ToAdjList(mol, bmap);

            Cycles.MarkRingAtomsAndBonds(mol, adjlist, bmap);

            var cuts = FindCutBonds(mol, bmap, adjlist);

            var atmidx = new Dictionary<IAtom, int>();
            foreach (var atom in mol.Atoms)
                atmidx[atom] = atmidx.Count;

            // frags are ordered by biggest to smallest
            var frags = new List<IAtomContainer>();

            foreach (var cut in cuts)
            {
                if (frags.Count >= MAX_FRAG)
                    break;
                frags.AddRange(MakeCut(cut, mol, atmidx, adjlist));
            }

            frags.Sort(delegate (IAtomContainer a, IAtomContainer b)
                {
                    return -a.Bonds.Count.CompareTo(b.Bonds.Count);
                });

            return frags;
        }

        /// <summary>
        /// Find all enabled abbreviations in the provided molecule. They are not
        /// added to the existing Sgroups and may need filtering.
        /// </summary>
        /// <param name="mol">molecule</param>
        /// <returns>list of new abbreviation Sgroups</returns>
        public IList<Sgroup> Generate(IAtomContainer mol)
        {
            // mark which atoms have already been abbreviated or are
            // part of an existing Sgroup
            var usedAtoms = new HashSet<IAtom>();
            IList<Sgroup> sgroups = mol.GetProperty<IList<Sgroup>>(CDKPropertyName.CTAB_SGROUPS);
            if (sgroups != null)
            {
                foreach (var sgroup in sgroups)
                    foreach (var atom in sgroup.Atoms)
                        usedAtoms.Add(atom);
            }

            // disconnected abbreviations, salts, common reagents, large compounds
            if (!usedAtoms.Any())
            {
                try
                {
                    string cansmi = usmigen.Create(AtomContainerManipulator.CopyAndSuppressedHydrogens(mol));
                    string label;
                    if (disconnectedAbbreviations.TryGetValue(cansmi, out label) && !disabled.Contains(label))
                    {
                        Sgroup sgroup = new Sgroup();
                        sgroup.Type = SgroupType.CtabAbbreviation;
                        sgroup.Subscript = label;
                        foreach (var atom in mol.Atoms)
                            sgroup.Atoms.Add(atom);
                        return new[] { sgroup };
                    }
                }
                catch (CDKException)
                {
                }
            }

            var newSgroups = new List<Sgroup>();
            List<IAtomContainer> fragments = GenerateFragments(mol);

            foreach (var frag in fragments)
            {
                try
                {
                    string smi = usmigen.Create(AtomContainerManipulator.CopyAndSuppressedHydrogens(frag));
                    string label;
                    if (!connectedAbbreviations.TryGetValue(smi, out label) || disabled.Contains(label))
                        continue;

                    bool overlap = false;

                    // note: first atom is '*'
                    int numAtoms = frag.Atoms.Count;
                    int numBonds = frag.Bonds.Count;
                    for (int i = 1; i < numAtoms; i++)
                    {
                        if (usedAtoms.Contains(frag.Atoms[i]))
                        {
                            overlap = true;
                            break;
                        }
                    }

                    // overlaps with previous assignment
                    if (overlap)
                        continue;

                    // create new abbreviation SGroup
                    Sgroup sgroup = new Sgroup();
                    sgroup.Type = SgroupType.CtabAbbreviation;
                    sgroup.Subscript = label;
                    sgroup.Bonds.Add(frag.Bonds[0].GetProperty<IBond>(CUT_BOND));
                    for (int i = 1; i < numAtoms; i++)
                    {
                        IAtom atom = frag.Atoms[i];
                        usedAtoms.Add(atom);
                        sgroup.Atoms.Add(atom);
                    }

                    newSgroups.Add(sgroup);
                }
                catch (CDKException)
                {
                    // ignore
                }
            }
            return newSgroups;
        }

        /// <summary>
        /// Generates and assigns abbreviations to a molecule. Abbrevations are first
        /// generated with <see cref="Generate(IAtomContainer)"/> and the filtered based on
        /// the coverage. Currently only abbreviations that cover 100%, or &lt; 40% of the
        /// atoms are assigned.
        /// </summary>
        /// <param name="mol">molecule</param>
        /// <returns>number of new abbreviations</returns>
        /// <seealso cref="Generate(IAtomContainer)"/>
        public int Apply(IAtomContainer mol)
        {
            var newSgroups = Generate(mol);
            var sgroups = mol.GetProperty<IList<Sgroup>>(CDKPropertyName.CTAB_SGROUPS);

            if (sgroups == null)
                sgroups = new List<Sgroup>();
            else
                sgroups = new List<Sgroup>(sgroups);

            int prev = sgroups.Count;
            foreach (var sgroup in newSgroups)
            {
                double coverage = sgroup.Atoms.Count / (double)mol.Atoms.Count;
                // update javadoc if changed!
                if (!sgroup.Bonds.Any() || coverage < 0.4d)
                    sgroups.Add(sgroup);
            }
            mol.SetProperty(CDKPropertyName.CTAB_SGROUPS, new ReadOnlyCollection<Sgroup>(sgroups));
            return sgroups.Count - prev;
        }

        /// <summary>
        /// Make a query atom that matches atomic number, h count, valence, and
        /// connectivity. This effectively provides an exact match for that atom
        /// type.
        /// </summary>
        /// <param name="mol">molecule</param>
        /// <param name="atom">atom of molecule</param>
        /// <returns>the query atom (null if attachment point)</returns>
        private IQueryAtom matchExact(IAtomContainer mol, IAtom atom)
        {
            IChemObjectBuilder bldr = atom.Builder;

            int elem = atom.AtomicNumber.Value;

            // attach atom skipped
            if (elem == 0)
                return null;

            int hcnt = atom.ImplicitHydrogenCount.Value;
            int val = hcnt;
            int con = hcnt;

            foreach (var bond in mol.GetConnectedBonds(atom))
            {
                val += bond.Order.Numeric;
                con++;
                if (bond.GetConnectedAtom(atom).AtomicNumber == 1)
                    hcnt++;
            }

            return And(And(new AtomicNumberAtom(elem, bldr),
                           new TotalConnectionAtom(con, bldr)),
                       And(new TotalHCountAtom(hcnt, bldr),
                           new TotalValencyAtom(val, bldr)));
        }

        /// <summary>
        /// Internal - create a query atom container that exactly matches the molecule provided.
        /// Similar to {@link org.openscience.cdk.isomorphism.matchers.QueryAtomContainerCreator}
        /// but we can't access SMARTS query classes from that module (cdk-isomorphism).
        /// </summary>
        /// <param name="mol">molecule</param>
        /// <returns>query container</returns>
        /// <seealso cref="QueryAtomContainerCreator"/>
        private IQueryAtomContainer matchExact(IAtomContainer mol)
        {
            IChemObjectBuilder bldr = mol.Builder;
            IQueryAtomContainer qry = new QueryAtomContainer(mol.Builder);
            var atmmap = new Dictionary<IAtom, IAtom>();

            foreach (var atom in mol.Atoms)
            {
                IAtom qatom = matchExact(mol, atom);
                if (qatom != null)
                {
                    atmmap[atom] = qatom;
                    qry.Atoms.Add(qatom);
                }
            }

            foreach (var bond in mol.Bonds)
            {
                IAtom beg = atmmap[bond.Atoms[0]];
                IAtom end = atmmap[bond.Atoms[1]];

                // attach bond skipped
                if (beg == null || end == null)
                    continue;

                IQueryBond qbond = new AnyOrderQueryBond(bldr);
                qbond.Atoms[0] = beg;
                qbond.Atoms[1] = end;
                qry.Bonds.Add(qbond);
            }

            return qry;
        }

        private bool AddDisconnectedAbbreviation(IAtomContainer mol, string label)
        {
            try
            {
                string cansmi = SmilesGenerator.Unique().Create(mol);
                disconnectedAbbreviations[cansmi] = label;
                labels.Add(label);
                return true;
            }
            catch (CDKException)
            {
                return false;
            }
        }

        private bool AddConnectedAbbreviation(IAtomContainer mol, string label)
        {
            try
            {
                connectedAbbreviations[usmigen.Create(mol)] = label;
                labels.Add(label);
                return true;
            }
            catch (CDKException)
            {
                return false;
            }
        }

        /// <summary>
        /// Convenience method to add an abbreviation from a SMILES string.
        /// </summary>
        /// <param name="line">the smiles to add with a title (the label)</param>
        /// <returns>the abbreviation was added, will be false if no title supplied</returns>
        /// <exception cref="InvalidSmilesException">the SMILES was not valid</exception>
        public bool Add(string line)
        {
            return Add(smipar.ParseSmiles(line), GetSmilesSuffix(line));
        }

        /// <summary>
        /// Add an abbreviation to the factory. Abbreviations can be of various flavour based
        /// on the number of attachments:
        /// <list type="bullet">
        /// <item><b>Detached</b> - zero attachments, the abbreviation covers the whole structure (e.g. THF)</item>
        /// <item><b>Terminal</b> - one attachment, covers substituents (e.g. Ph for Phenyl)</item>
        /// <item><b>Linker</b> - [NOT SUPPORTED YET] two attachments, covers long repeated chains (e.g. PEG4)</item>
        /// </list>
        /// Attachment points (if present) must be specified with zero element atoms. 
        /// <code>
        /// *c1ccccc1 Ph
        /// *OC(=O)C OAc
        /// </code>
        /// </summary>
        /// <param name="mol">the fragment to abbreviate</param>
        /// <param name="label">the label of the fragment</param>
        /// <returns>the abbreviation was added</returns>
        public bool Add(IAtomContainer mol, string label)
        {
            if (string.IsNullOrEmpty(label))
                return false;

            // required invariants and check for number of attachment points
            int numAttach = 0;
            foreach (var atom in mol.Atoms)
            {
                if (atom.ImplicitHydrogenCount == null || atom.AtomicNumber == null)
                    throw new ArgumentException("Implicit hydrogen count or atomic number is null");
                if (atom.AtomicNumber == 0)
                    numAttach++;
            }

            switch (numAttach)
            {
                case 0:
                    return AddDisconnectedAbbreviation(mol, label);
                case 1:
                    return AddConnectedAbbreviation(mol, label);
                default:
                    // not-supported yet - update JavaDoc if added
                    return false;
            }
        }

        private static string GetSmilesSuffix(string line)
        {
            int last = line.Length - 1;
            for (int i = 0; i < last; i++)
                if (line[i] == ' ' || line[i] == '\t')
                    return line.Substring(i + 1).Trim();
            return "";
        }

        private int LoadSmiles(Stream ins)
        {
            int count = 0;
            using (var brdr = new StreamReader(ins, Encoding.UTF8))
            {
                string line;
                while ((line = brdr.ReadLine()) != null)
                {
                    if (line.Length == 0 || line[0] == '#')
                        continue;
                    try
                    {
                        if (Add(line))
                            count++;
                    }
                    catch (InvalidSmilesException e)
                    {
                        Console.Error.WriteLine(e.StackTrace);
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Load a set of abbreviations from a classpath resource or file.
        /// <code>
        /// *c1ccccc1 Ph
        /// *c1ccccc1 OAc
        /// </code>
        ///
        /// Available:
        /// <code>
        /// obabel_superatoms.smi - https://www.github.com/openbabel/superatoms
        /// </code>
        /// </summary>
        /// <param name="path">classpath or filesystem path to a SMILES file</param>
        /// <returns>the number of loaded abbreviation</returns>
        /// <exception cref="IOException"></exception>
        public int LoadFromFile(string path)
        {
            Stream ins = null;
            try
            {
                ins = ResourceLoader.GetAsStream(path);
                if (ins != null)
                    return LoadSmiles(ins);
                if (File.Exists(path))
                    return LoadSmiles(new FileStream(path, FileMode.Open));
            }
            finally
            {
                if (ins != null)
                    ins.Close();
            }
            return 0;
        }
    }
}
