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
using NCDK.Config;
using NCDK.Graphs;
using NCDK.Isomorphisms.Matchers;
using NCDK.Isomorphisms.Matchers.SMARTS;
using NCDK.Sgroups;
using NCDK.Smiles;
using NCDK.Tools.Manipulator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using static NCDK.Graphs.GraphUtil;
using static NCDK.Isomorphisms.Matchers.SMARTS.LogicalOperatorAtom;

namespace NCDK.Depict
{
    /// <summary>
    /// Utility class for abbreviating (sub)structures. Using either self assigned structural
    /// motifs or pre-loading a common set a structure depiction can be made more concise with
    /// the use of abbreviations (sometimes called superatoms). 
    /// </summary>
    /// <example>
    /// Basic usage:
    /// <code>
    /// Abbreviations abrv = new Abbreviations();
    ///
    /// // add some abbreviations, when overlapping (e.g. Me,Et,tBu) first one wins
    /// abrv.Add("[Na+].[H-] NaH");
    /// abrv.Add("*c1ccccc1 Ph");
    /// abrv.Add("*C(C)(C)C tBu");
    /// abrv.Add("*CC Et");
    /// abrv.Add("*C Me");
    ///
    /// // maybe we don't want 'Me' in the depiction
    /// abrv.SetEnabled("Me", false);
    ///
    /// // assign abbreviations with some filters
    /// int numAdded = abrv.Apply(mol);
    ///
    /// // generate all but don't assign, need to be added manually
    /// // set/update the CDKConstants.CTAB_SGROUPS property of mol
    /// List&lt;Sgroup&gt; sgroups = abrv.Generate(mol);
    /// </code>
    ///
    /// Predefined sets of abbreviations can be loaded, the following are
    /// on the classpath.
    ///
    /// <code>
    /// // https://www.github.com/openbabel/superatoms
    /// abrv.LoadFromFile("obabel_superatoms.smi");
    /// </code>
    /// </example>
    /// <seealso cref="CDKPropertyName.CtabSgroups"/>
    /// <seealso cref="Sgroup"/>
    // @cdk.keyword abbreviate
    // @cdk.keyword depict
    // @cdk.keyword superatom
    public class Abbreviations : IEnumerable<string>
    {
        private const int MaxFrag = 50;

        /// <summary>
        /// Symbol for joining disconnected fragments.
        /// </summary>
        private const string INTERPUNCT = "·";

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

        /// <summary>
        /// Set whether abbreviations should be further contracted when they are connected
        /// to a heteroatom, for example -NH-Boc becomes -NHBoc. By default this option
        /// is enabled.
        /// </summary>
        public bool ContractOnHetero { get; set; } = true;

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

        private const string PropertyName_CutBond = "cutbond";

        private static List<IAtomContainer> MakeCut(IBond cut, IAtomContainer mol, Dictionary<IAtom, int> idx, int[][] adjlist)
        {
            IAtom beg = cut.Begin;
            IAtom end = cut.End;

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
            IAtomContainer bfrag = bldr.NewAtomContainer();
            IAtomContainer efrag = bldr.NewAtomContainer();

            int diff = bvisit.Count - evisit.Count;

            if (diff < -10)
                evisit.Clear();
            else if (diff > 10)
                bvisit.Clear();

            if (bvisit.Any())
            {
                bfrag.Atoms.Add(bldr.NewPseudoAtom());
                foreach (var atom in bvisit)
                    bfrag.Atoms.Add(atom);
                bfrag.AddBond(bfrag.Atoms[0], bfrag.Atoms[1], cut.Order);
                bfrag.Bonds[0].SetProperty(PropertyName_CutBond, cut);
            }

            if (evisit.Any())
            {
                efrag.Atoms.Add(bldr.NewPseudoAtom());
                foreach (var atom in evisit)
                    efrag.Atoms.Add(atom);
                efrag.AddBond(efrag.Atoms[0], efrag.Atoms[1], cut.Order);
                efrag.Bonds[0].SetProperty(PropertyName_CutBond, cut);
            }

            foreach (var bond in mol.Bonds)
            {
                IAtom a1 = bond.Begin;
                IAtom a2 = bond.End;
                if (bvisit.Contains(a1) && bvisit.Contains(a2))
                    bfrag.Bonds.Add(bond);
                else if (evisit.Contains(a1) && evisit.Contains(a2))
                    efrag.Bonds.Add(bond);
            }

            var res = new List<IAtomContainer>();
            if (!bfrag.IsEmpty())
                res.Add(bfrag);
            if (!efrag.IsEmpty())
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
                if (frags.Count >= MaxFrag)
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
            IList<Sgroup> sgroups = mol.GetProperty<IList<Sgroup>>(CDKPropertyName.CtabSgroups);
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
                    IAtomContainer copy = AtomContainerManipulator.CopyAndSuppressedHydrogens(mol);
                    string cansmi = usmigen.Create(copy);
                    if (disconnectedAbbreviations.TryGetValue(cansmi, out string label) && !disabled.Contains(label))
                    {
                        Sgroup sgroup = new Sgroup
                        {
                            Type = SgroupTypes.CtabAbbreviation,
                            Subscript = label
                        };
                        foreach (var atom in mol.Atoms)
                            sgroup.Atoms.Add(atom);
                        return new[] { sgroup };
                    }
                    else if (cansmi.Contains("."))
                    {
                        List<Sgroup> complexAbbr = new List<Sgroup>(4); // e.g. NEt3
                        List<Sgroup> simpleAbbr = new List<Sgroup>(4); // e.g. HCl
                        foreach (IAtomContainer part in ConnectivityChecker.PartitionIntoMolecules(mol))
                        {
                            if (part.Atoms.Count == 1)
                            {
                                IAtom atom = part.Atoms[0];
                                label = GetBasicElementSymbol(atom);
                                if (label != null)
                                {
                                    Sgroup sgroup = new Sgroup
                                    {
                                        Type = SgroupTypes.CtabAbbreviation,
                                        Subscript = label
                                    };
                                    sgroup.Atoms.Add(atom);
                                    simpleAbbr.Add(sgroup);
                                }
                            }
                            else
                            {
                                cansmi = usmigen.Create(part);
                                label = disconnectedAbbreviations[cansmi];
                                if (label != null && !disabled.Contains(label))
                                {
                                    foreach (IAtom atom in part.Atoms)
                                        new Sgroup
                                        {
                                            Type = SgroupTypes.CtabAbbreviation,
                                            Subscript = label
                                        }.Atoms.Add(atom);
                                    complexAbbr.Add(new Sgroup
                                    {
                                        Type = SgroupTypes.CtabAbbreviation,
                                        Subscript = label
                                    });
                                }
                            }
                        }
                        if (complexAbbr.Any())
                        {
                            // merge together the abbreviations, if there is at least
                            // one complex abbr
                            if (complexAbbr.Any() &&
                                complexAbbr.Count + simpleAbbr.Count > 1)
                            {
                                Sgroup combined = new Sgroup();
                                label = null;
                                complexAbbr.AddRange(simpleAbbr);
                                foreach (Sgroup sgroup in complexAbbr)
                                {
                                    if (label == null)
                                        label = sgroup.Subscript;
                                    else
                                        label += INTERPUNCT + sgroup.Subscript;
                                    foreach (IAtom atom in sgroup.Atoms)
                                        combined.Atoms.Add(atom);
                                }
                                combined.Subscript = label;
                                combined.Type = SgroupTypes.CtabAbbreviation;
                                complexAbbr.Clear();
                                complexAbbr.Add(combined);
                            }
                            return complexAbbr;
                        }
                    }
                }
                catch (CDKException)
                {
                }
            }

            var newSgroups = new List<Sgroup>();
            List<IAtomContainer> fragments = GenerateFragments(mol);
            MultiDictionary<IAtom, Sgroup> sgroupAdjs = new MultiDictionary<IAtom, Sgroup>();

            foreach (var frag in fragments)
            {
                try
                {
                    string smi = usmigen.Create(AtomContainerManipulator.CopyAndSuppressedHydrogens(frag));
                    if (!connectedAbbreviations.TryGetValue(smi, out string label) || disabled.Contains(label))
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

                    // create new abbreviation Sgroup
                    Sgroup sgroup = new Sgroup
                    {
                        Type = SgroupTypes.CtabAbbreviation,
                        Subscript = label
                    };

                    IBond attachBond = frag.Bonds[0].GetProperty<IBond>(PropertyName_CutBond);
                    IAtom attachAtom = null;
                    sgroup.Bonds.Add(attachBond);
                    for (int i = 1; i < numAtoms; i++)
                    {
                        IAtom atom = frag.Atoms[i];
                        usedAtoms.Add(atom);
                        sgroup.Atoms.Add(atom);
                        if (attachBond.Begin == atom)
                            attachAtom = attachBond.End;
                        else if (attachBond.End == atom)
                            attachAtom = attachBond.Begin;
                    }

                    if (attachAtom != null)
                        sgroupAdjs.Add(attachAtom, sgroup);
                    newSgroups.Add(sgroup);
                }
                catch (CDKException)
                {
                    // ignore
                }
            }

            // now collapse
            foreach (IAtom attach in mol.Atoms)
            {
                if (usedAtoms.Contains(attach))
                    continue;

                // skip charged or isotopic labelled, C or R/*, H, He
                if ((attach.FormalCharge != null && attach.FormalCharge != 0)
                    || attach.MassNumber != null
                    || attach.AtomicNumber == 6
                    || attach.AtomicNumber < 2)
                    continue;

                int hcount = attach.ImplicitHydrogenCount.Value;
                var xatoms = new HashSet<IAtom>();
                var xbonds = new HashSet<IBond>();
                var newbonds = new HashSet<IBond>();
                xatoms.Add(attach);

                List<string> nbrSymbols = new List<string>();
                var todelete = new HashSet<Sgroup>();
                foreach (Sgroup sgroup in sgroupAdjs[attach])
                {
                    if (ContainsChargeChar(sgroup.Subscript))
                        continue;
                    foreach (var b in sgroup.Bonds)
                        xbonds.Add(b);
                    foreach (var a in sgroup.Atoms)
                        xatoms.Add(a);
                    nbrSymbols.Add(sgroup.Subscript);
                    todelete.Add(sgroup);
                }
                int numSGrpNbrs = nbrSymbols.Count;
                foreach (IBond bond in mol.GetConnectedBonds(attach))
                {
                    if (!xbonds.Contains(bond))
                    {
                        IAtom nbr = bond.GetOther(attach);
                        // contract terminal bonds
                        if (mol.GetConnectedBonds(nbr).Count() == 1)
                        {
                            if (nbr.MassNumber != null ||
                                (nbr.FormalCharge != null && nbr.FormalCharge != 0))
                            {
                                newbonds.Add(bond);
                            }
                            else if (nbr.AtomicNumber == 1)
                            {
                                hcount++;
                                xatoms.Add(nbr);
                            }
                            else if (nbr.AtomicNumber > 0)
                            {
                                nbrSymbols.Add(NewSymbol(nbr.AtomicNumber.Value, nbr.ImplicitHydrogenCount.Value, false));
                                xatoms.Add(nbr);
                            }
                        }
                        else
                        {
                            newbonds.Add(bond);
                        }
                    }
                }

                // reject if no symbols
                // reject if no bonds (<1), except if all symbols are identical... (HashSet.size==1)
                // reject if more that 2 bonds
                if (!nbrSymbols.Any() ||
                    newbonds.Count < 1 && (new HashSet<string>(nbrSymbols).Count != 1) ||
                    newbonds.Count > 2)
                    continue;

                // create the symbol
                StringBuilder sb = new StringBuilder();
                sb.Append(NewSymbol(attach.AtomicNumber.Value, hcount, newbonds.Count == 0));
                string prev = null;
                int count = 0;
                nbrSymbols.Sort((o1, o2) =>
                    {
                        int cmp = o1.Length.CompareTo(o2.Length);
                        if (cmp != 0) return cmp;
                        return o1.CompareTo(o2);
                    });
                foreach (string nbrSymbol in nbrSymbols)
                {
                    if (nbrSymbol.Equals(prev))
                    {
                        count++;
                    }
                    else
                    {
                        bool useParen = count == 0 || CountUpper(prev) > 1 || (prev != null && nbrSymbol.StartsWith(prev));
                        AppendGroup(sb, prev, count, useParen);
                        prev = nbrSymbol;
                        count = 1;
                    }
                }
                AppendGroup(sb, prev, count, false);

                // remove existing
                foreach (var e in todelete)
                    newSgroups.Remove(e);

                // create new
                Sgroup newSgroup = new Sgroup
                {
                    Type = SgroupTypes.CtabAbbreviation,
                    Subscript = sb.ToString()
                };
                foreach (IBond bond in newbonds)
                    newSgroup.Bonds.Add(bond);
                foreach (IAtom atom in xatoms)
                    newSgroup.Atoms.Add(atom);

                newSgroups.Add(newSgroup);
                foreach (var a in xatoms)
                    usedAtoms.Add(a);
            }

            return newSgroups;
        }

        /// <summary>
        /// Count number of upper case chars.
        /// </summary>
        private int CountUpper(String str)
        {
            if (str == null)
                return 0;
            int num = 0;
            for (int i = 0; i < str.Length; i++)
                if (char.IsUpper(str[i]))
                    num++;
            return num;
        }

        private bool ContainsChargeChar(String str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (c == '-' || c == '+')
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Check if last char is a digit.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool DigitAtEnd(string str)
        {
            return char.IsDigit(str[str.Length - 1]);
        }

        private string NewSymbol(int atomnum, int hcount, bool prefix)
        {
            StringBuilder sb = new StringBuilder();
            Elements elem = Elements.OfNumber(atomnum);
            if (elem == Elements.Carbon && hcount == 3)
                return "Me";
            if (prefix)
            {
                if (hcount > 0)
                {
                    sb.Append('H');
                    if (hcount > 1)
                        sb.Append(hcount);
                }
                sb.Append(elem.Symbol);
            }
            else
            {
                sb.Append(elem.Symbol);
                if (hcount > 0)
                {
                    sb.Append('H');
                    if (hcount > 1)
                        sb.Append(hcount);
                }
            }
            return sb.ToString();
        }

        private void AppendGroup(StringBuilder sb, String group, int coef, bool useParen)
        {
            if (coef <= 0 || group == null || !group.Any()) return;
            if (!useParen)
                useParen = coef > 1 && (CountUpper(group) > 1 || DigitAtEnd(group));
            if (useParen)
                sb.Append('(');
            sb.Append(group);
            if (useParen)
                sb.Append(')');
            if (coef > 1)
                sb.Append(coef);
        }

        /// <summary>
        /// Generates and assigns abbreviations to a molecule. Abbreviations are first
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
            var sgroups = mol.GetProperty<IList<Sgroup>>(CDKPropertyName.CtabSgroups);

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
            mol.SetProperty(CDKPropertyName.CtabSgroups, new ReadOnlyCollection<Sgroup>(sgroups));
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
        private IQueryAtom MatchExact(IAtomContainer mol, IAtom atom)
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
                val += bond.Order.Numeric();
                con++;
                if (bond.GetOther(atom).AtomicNumber == 1)
                    hcnt++;
            }

            return And(And(new AtomicNumberAtom(elem, bldr),
                           new TotalConnectionAtom(con, bldr)),
                       And(new TotalHCountAtom(hcnt, bldr),
                           new TotalValencyAtom(val, bldr)));
        }

        /// <summary>
        /// Internal - create a query atom container that exactly matches the molecule provided.
        /// Similar to <see cref="QueryAtomContainerCreator"/>
        /// but we can't access SMARTS query classes from that module (cdk-isomorphism).
        /// </summary>
        /// <param name="mol">molecule</param>
        /// <returns>query container</returns>
        /// <seealso cref="QueryAtomContainerCreator"/>
        private IQueryAtomContainer MatchExact(IAtomContainer mol)
        {
            IChemObjectBuilder bldr = mol.Builder;
            IQueryAtomContainer qry = new QueryAtomContainer(mol.Builder);
            var atmmap = new Dictionary<IAtom, IAtom>();

            foreach (var atom in mol.Atoms)
            {
                IAtom qatom = MatchExact(mol, atom);
                if (qatom != null)
                {
                    atmmap[atom] = qatom;
                    qry.Atoms.Add(qatom);
                }
            }

            foreach (var bond in mol.Bonds)
            {
                IAtom beg = atmmap[bond.Begin];
                IAtom end = atmmap[bond.End];

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

        private static string GetBasicElementSymbol(IAtom atom)
        {
            if (atom.FormalCharge != null && atom.FormalCharge != 0)
                return null;
            if (atom.MassNumber != null && atom.MassNumber != 0)
                return null;
            if (atom.AtomicNumber == null || atom.AtomicNumber.Value < 1)
                return null;
            var hcnt = atom.ImplicitHydrogenCount;
            if (hcnt == null) return null;
            Elements elem = Elements.OfNumber(atom.AtomicNumber.Value);
            string hsym = (hcnt > 0) ? ((hcnt > 1) ? ("H" + hcnt) : "H") : "";
            // see HydrogenPosition for canonical list
            switch (elem.AtomicNumber)
            {
                case Elements.O.Oxygen:
                case Elements.O.Sulfur:
                case Elements.O.Selenium:
                case Elements.O.Tellurium:
                case Elements.O.Fluorine:
                case Elements.O.Chlorine:
                case Elements.O.Bromine:
                case Elements.O.Iodine:
                    return hsym + elem.Symbol;
                default:
                    return elem.Symbol + hsym;
            }
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
        /// Load a set of abbreviations from a resource or file.
        /// </summary>
        /// <remarks>
        /// <pre>
        /// *c1ccccc1 Ph
        /// *c1ccccc1 OAc
        /// </pre>
        /// Available:
        /// <pre>
        /// obabel_superatoms.smi - 
        /// <see href="https://www.github.com/openbabel/superatoms"/>
        /// </pre>
        /// </remarks>
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
