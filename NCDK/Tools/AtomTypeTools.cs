/* Copyright (C) 2005-2007  Christian Hoppe <chhoppe@users.sf.net>
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */
using NCDK.Aromaticities;
using NCDK.Graphs;
using NCDK.Smiles;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NCDK.Tools
{
    /// <summary>
    /// AtomTypeTools is a helper class for assigning atom types to an atom.
    /// </summary>
    // @author         cho
    // @cdk.created    2005-18-07
    // @cdk.module     extra
    // @cdk.githash
    public class AtomTypeTools
    {
        public const int NotInRing = 100;
        public const int PyrroleRing = 4;
        public const int FuranRing = 6;
        public const int ThiopheneRing = 8;
        public const int PyridineRing = 10;
        public const int PyrimidineRing = 12;
        public const int BenzeneRing = 5;
        HOSECodeGenerator hcg = null;

        /// <summary>
        /// Constructor for the MMFF94AtomTypeMatcher object.
        /// </summary>
        public AtomTypeTools()
        {
            hcg = new HOSECodeGenerator();
        }

        public IRingSet AssignAtomTypePropertiesToAtom(IAtomContainer molecule)
        {
            return AssignAtomTypePropertiesToAtom(molecule, true);
        }

        /// <summary>
        ///  Method assigns certain properties to an atom. Necessary for the atom type matching
        ///  Properties:
        ///  <list type="bullet">
        ///   <item>aromaticity)</item>
        ///   <item>ChemicalGroup (CDKChemicalRingGroupConstant)</item>
        ///   <item>
        ///     <item>SSSR</item>
        ///     <item>Ring/Group, ringSize, aromaticity</item>
        ///     <item>SphericalMatcher (HoSe Code)</item>
        ///   </item>
        ///  </list>
        /// </summary>
        /// <param name="molecule"></param>
        /// <param name="aromaticity">bool true/false true if aromaticity should be calculated</param>
        /// <returns>sssrf ring set of the molecule</returns>
        public IRingSet AssignAtomTypePropertiesToAtom(IAtomContainer molecule, bool aromaticity)
        {
            SmilesGenerator sg = new SmilesGenerator();

            Debug.WriteLine("assignAtomTypePropertiesToAtom Start ...");
            string hoseCode = "";
            IRingSet ringSetMolecule = Cycles.FindSSSR(molecule).ToRingSet();
            Debug.WriteLine(ringSetMolecule);

            if (aromaticity)
            {
                try
                {
                    Aromaticity.CDKLegacy.Apply(molecule);
                }
                catch (Exception cdk1)
                {
                    //Debug.WriteLine("AROMATICITYError: Cannot determine aromaticity due to: " + cdk1.ToString());
                    Trace.TraceError("AROMATICITYError: Cannot determine aromaticity due to: " + cdk1.ToString());
                }
            }

            for (int i = 0; i < molecule.Atoms.Count; i++)
            {
                // FIXME: remove casting
                IAtom atom2 = molecule.Atoms[i];
                //Atom aromatic is set by HueckelAromaticityDetector
                //Atom in ring?
                if (ringSetMolecule.Contains(atom2))
                {
                    var ringSetA = ringSetMolecule.Builder.NewRingSet();
                    ringSetA.AddRange(ringSetMolecule.GetRings(atom2));
                    RingSetManipulator.Sort(ringSetA);
                    IRing sring = (IRing)ringSetA.Last();
                    atom2.SetProperty(CDKPropertyName.PartOfRingOfSize, sring.RingSize);
                    atom2.SetProperty(
                        CDKPropertyName.ChemicalGroupConstant,
                        RingSystemClassifier(sring, GetSubgraphSmiles(sring, molecule)));
                    atom2.IsInRing = true;
                    atom2.IsAliphatic = false;
                }
                else
                {
                    atom2.SetProperty(CDKPropertyName.ChemicalGroupConstant, NotInRing);
                    atom2.IsInRing = false;
                    atom2.IsAliphatic = true;
                }
                try
                {
                    hoseCode = hcg.GetHOSECode(molecule, atom2, 3);
                    hoseCode = RemoveAromaticityFlagsFromHoseCode(hoseCode);
                    atom2.SetProperty(CDKPropertyName.SphericalMatcher, hoseCode);
                }
                catch (CDKException ex1)
                {
                    throw new CDKException("Could not build HOSECode from atom " + i + " due to " + ex1.ToString(), ex1);
                }
            }
            return ringSetMolecule;
        }

        /// <summary>
        /// New SMILES code respects atom valency hence a ring subgraph of 'o1cccc1CCCC' is correctly
        /// written as 'o1ccc[c]1' note there is no hydrogen there since it was an external attachment.
        /// To get unique subgraph SMILES we need to adjust valencies of atoms by adding Hydrogens. We
        /// base this on the sum of bond orders removed.
        /// </summary>
        /// <param name="subgraph">subgraph (atom and bond refs in 'molecule')</param>
        /// <param name="molecule">the molecule</param>
        /// <returns>the canonical smiles of the subgraph</returns>
        /// <exception cref="CDKException">something went wrong with SMILES gen</exception>
        private static string GetSubgraphSmiles(IAtomContainer subgraph, IAtomContainer molecule)
        {
            var bonds = new HashSet<IBond>();
            foreach (var bond in subgraph.Bonds)
                bonds.Add(bond);

            int?[] hCount = new int?[subgraph.Atoms.Count];
            for (int i = 0; i < subgraph.Atoms.Count; i++)
            {
                IAtom atom = subgraph.Atoms[i];
                int removed = 0;
                foreach (var bond in molecule.GetConnectedBonds(atom))
                {
                    if (!bonds.Contains(bond))
                        removed += bond.Order.Numeric;
                }
                hCount[i] = atom.ImplicitHydrogenCount;
                atom.ImplicitHydrogenCount = (hCount[i] == null ? removed : hCount[i] + removed);
            }

            string smi = Cansmi(subgraph);

            // reset for fused rings!
            for (int i = 0; i < subgraph.Atoms.Count; i++)
            {
                subgraph.Atoms[i].ImplicitHydrogenCount = hCount[i];
            }

            return smi;
        }

        /// <summary>
        /// Canonical SMILES for the provided molecule.
        /// </summary>
        /// <param name="mol">molecule</param>
        /// <returns>the cansmi string</returns>
        /// <exception cref="CDKException">something went wrong with SMILES gen</exception>
        private static string Cansmi(IAtomContainer mol)
        {
            return SmilesGenerator.Unique().Create(mol);
        }

        private string PyrroleSmi = null;
        private string FuranSmi = null;
        private string ThiopheneSmi = null;
        private string PyridineSmi = null;
        private string PyrimidineSmi = null;
        private string BenzeneSmi = null;

        private static string Smicache(string cached, SmilesParser smipar, string input)
        {
            if (cached != null) return cached;
            return cached = Cansmi(smipar.ParseSmiles(input));
        }

        /// <summary>
        ///  Identifies ringSystem and returns a number which corresponds to
        ///  CDKChemicalRingConstant
        /// </summary>
        /// <param name="ring">Ring class with the ring system</param>
        /// <param name="smile">smile of the ring system</param>
        /// <returns>chemicalRingConstant</returns>
        private int RingSystemClassifier(IRing ring, string smile)
        {
            /* Console.Out.WriteLine("IN AtomTypeTools Smile:"+smile); */
            Debug.WriteLine("Comparing ring systems: SMILES=", smile);

            SmilesParser smipar = new SmilesParser(ring.Builder);

            if (smile.Equals(Smicache(PyrroleSmi, smipar, "c1cc[nH]c1")))
                return PyrroleRing;
            else if (smile.Equals(Smicache(FuranSmi, smipar, "o1cccc1")))
                return FuranRing;
            else if (smile.Equals(Smicache(ThiopheneSmi, smipar, "c1ccsc1")))
                return ThiopheneRing;
            else if (smile.Equals(Smicache(PyridineSmi, smipar, "c1ccncc1")))
                return PyridineRing;
            else if (smile.Equals(Smicache(PyrimidineSmi, smipar, "c1cncnc1")))
                return PyrimidineRing;
            else if (smile.Equals(Smicache(BenzeneSmi, smipar, "c1ccccc1")))
                return BenzeneRing;

            int ncount = 0;
            foreach (var atom in ring.Atoms)
            {
                if (atom.Symbol.Equals("N"))
                {
                    ncount = ncount + 1;
                }
            }

            if (ring.Atoms.Count == 6 & ncount == 1)
            {
                return 10;
            }
            else if (ring.Atoms.Count == 5 & ncount == 1)
            {
                return 4;
            }

            if (ncount == 0)
            {
                return 3;
            }
            else
            {
                return 0;
            }
        }

        private string RemoveAromaticityFlagsFromHoseCode(string hoseCode)
        {
            string hosecode = "";
            for (int i = 0; i < hoseCode.Length; i++)
            {
                if (hoseCode[i] != '*')
                {
                    hosecode = hosecode + hoseCode[i];
                }
            }
            return hosecode;
        }
    }
}
