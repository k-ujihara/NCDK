/*
 * Copyright (c) 2014 European Bioinformatics Institute (EMBL-EBI)
 *                    John May <jwmay@users.sf.net>
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Common.Base;
using NCDK.Silent;
using System;
using System.IO;
using System.Text;

namespace NCDK.ForceFields
{
    /// <summary>
    /// Unit tests for MMFF symbolic atom types. This class primarily tests preconditions and some
    /// failing cases from old implementations. The atom types of the MMFF validation suite is tested by
    /// <see cref="MmffAtomTypeValidationSuiteTest"/> .
    /// </summary>
    [TestClass()]
    public class MmffAtomTypeMatcherTest
    {
        static MmffAtomTypeMatcher Instance = new MmffAtomTypeMatcher();

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void HydrogenCountMustBeDefined()
        {
            var container = Silent.ChemObjectBuilder.Instance.NewAtomContainer();
            container.Atoms.Add(new Atom("C"));
            container.Atoms.Add(new Atom("H"));
            container.Atoms.Add(new Atom("H"));
            container.Atoms.Add(new Atom("H"));
            container.Atoms.Add(new Atom("H"));
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);
            container.AddBond(container.Atoms[0], container.Atoms[2], BondOrder.Single);
            container.AddBond(container.Atoms[0], container.Atoms[3], BondOrder.Single);
            container.AddBond(container.Atoms[0], container.Atoms[4], BondOrder.Single);
            container.Atoms[0].ImplicitHydrogenCount = null;
            Instance.SymbolicTypes(container);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void HydrogenCountMustBeExplicit()
        {
            var container = Silent.ChemObjectBuilder.Instance.NewAtomContainer();
            container.Atoms.Add(new Atom("C"));
            container.Atoms[0].ImplicitHydrogenCount = 4;
            Instance.SymbolicTypes(container);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void AromaticCompoundsAreRejected()
        {
            var container = Silent.ChemObjectBuilder.Instance.NewAtomContainer();
            container.Atoms.Add(new Atom("C"));
            container.Atoms[0].ImplicitHydrogenCount = 4;
            container.Atoms[0].IsAromatic = true;
            Instance.SymbolicTypes(container);
        }

        /// <summary>
        /// This test ensures a unit from the old ForceFieldConfigurator passes. The nitrogen should be
        /// 'NC=O' and we see this is the case. SMILES: CC(C)C1CCC(CC1)C(=O)NC(Cc1ccccc1)C(=O)O
        /// </summary>
        // @cdk.bug #3523240
        [TestMethod()]
        public void Bug3523240IsResolved()
        {
            var container = Silent.ChemObjectBuilder.Instance.NewAtomContainer();
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("O", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("O", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("N", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("O", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("H", 0));
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);
            container.AddBond(container.Atoms[1], container.Atoms[2], BondOrder.Single);
            container.AddBond(container.Atoms[2], container.Atoms[3], BondOrder.Double);
            container.AddBond(container.Atoms[2], container.Atoms[4], BondOrder.Single);
            container.AddBond(container.Atoms[4], container.Atoms[5], BondOrder.Single);
            container.AddBond(container.Atoms[4], container.Atoms[6], BondOrder.Single);
            container.AddBond(container.Atoms[6], container.Atoms[7], BondOrder.Single);
            container.AddBond(container.Atoms[6], container.Atoms[8], BondOrder.Single);
            container.AddBond(container.Atoms[8], container.Atoms[9], BondOrder.Double);
            container.AddBond(container.Atoms[8], container.Atoms[10], BondOrder.Single);
            container.AddBond(container.Atoms[10], container.Atoms[11], BondOrder.Single);
            container.AddBond(container.Atoms[10], container.Atoms[12], BondOrder.Single);
            container.AddBond(container.Atoms[12], container.Atoms[13], BondOrder.Single);
            container.AddBond(container.Atoms[12], container.Atoms[14], BondOrder.Single);
            container.AddBond(container.Atoms[12], container.Atoms[15], BondOrder.Single);
            container.AddBond(container.Atoms[15], container.Atoms[16], BondOrder.Single);
            container.AddBond(container.Atoms[15], container.Atoms[17], BondOrder.Single);
            container.AddBond(container.Atoms[15], container.Atoms[18], BondOrder.Single);
            container.AddBond(container.Atoms[18], container.Atoms[19], BondOrder.Single);
            container.AddBond(container.Atoms[18], container.Atoms[20], BondOrder.Single);
            container.AddBond(container.Atoms[20], container.Atoms[21], BondOrder.Single);
            container.AddBond(container.Atoms[20], container.Atoms[22], BondOrder.Single);
            container.AddBond(container.Atoms[20], container.Atoms[23], BondOrder.Single);
            container.AddBond(container.Atoms[10], container.Atoms[23], BondOrder.Single);
            container.AddBond(container.Atoms[23], container.Atoms[24], BondOrder.Single);
            container.AddBond(container.Atoms[23], container.Atoms[25], BondOrder.Single);
            container.AddBond(container.Atoms[18], container.Atoms[26], BondOrder.Single);
            container.AddBond(container.Atoms[26], container.Atoms[27], BondOrder.Single);
            container.AddBond(container.Atoms[26], container.Atoms[28], BondOrder.Single);
            container.AddBond(container.Atoms[28], container.Atoms[29], BondOrder.Single);
            container.AddBond(container.Atoms[28], container.Atoms[30], BondOrder.Single);
            container.AddBond(container.Atoms[28], container.Atoms[31], BondOrder.Single);
            container.AddBond(container.Atoms[26], container.Atoms[32], BondOrder.Single);
            container.AddBond(container.Atoms[32], container.Atoms[33], BondOrder.Single);
            container.AddBond(container.Atoms[32], container.Atoms[34], BondOrder.Single);
            container.AddBond(container.Atoms[32], container.Atoms[35], BondOrder.Single);
            container.AddBond(container.Atoms[4], container.Atoms[36], BondOrder.Single);
            container.AddBond(container.Atoms[36], container.Atoms[37], BondOrder.Single);
            container.AddBond(container.Atoms[36], container.Atoms[38], BondOrder.Single);
            container.AddBond(container.Atoms[36], container.Atoms[39], BondOrder.Single);
            container.AddBond(container.Atoms[39], container.Atoms[40], BondOrder.Double);
            container.AddBond(container.Atoms[40], container.Atoms[41], BondOrder.Single);
            container.AddBond(container.Atoms[40], container.Atoms[42], BondOrder.Single);
            container.AddBond(container.Atoms[42], container.Atoms[43], BondOrder.Single);
            container.AddBond(container.Atoms[42], container.Atoms[44], BondOrder.Double);
            container.AddBond(container.Atoms[44], container.Atoms[45], BondOrder.Single);
            container.AddBond(container.Atoms[44], container.Atoms[46], BondOrder.Single);
            container.AddBond(container.Atoms[46], container.Atoms[47], BondOrder.Single);
            container.AddBond(container.Atoms[46], container.Atoms[48], BondOrder.Double);
            container.AddBond(container.Atoms[39], container.Atoms[48], BondOrder.Single);
            container.AddBond(container.Atoms[48], container.Atoms[49], BondOrder.Single);
            string[] expected = {"HOCO", "OC=O", "COO", "O=CO", "CR", "HC", "NC=O", "HNCO", "C=ON", "O=CN", "CR", "HC",
                "CR", "HC", "HC", "CR", "HC", "HC", "CR", "HC", "CR", "HC", "HC", "CR", "HC", "HC", "CR", "HC", "CR",
                "HC", "HC", "HC", "CR", "HC", "HC", "HC", "CR", "HC", "HC", "CB", "CB", "HC", "CB", "HC", "CB", "HC",
                "CB", "HC", "CB", "HC"};
            string[] actual = Instance.SymbolicTypes(container);
            Assert.IsTrue(Compares.AreDeepEqual(expected, actual));
        }

        /// <summary>
        /// This test ensures a unit from the old ForceFieldConfigurator passes. The nitrogen should be
        /// 'NO2', it was previously assigned 'N2OX'. SMILES: CC[N+](=O)[O-]
        /// </summary>
        // @cdk.bug #3524734
        [TestMethod()]
        public void Bug3524734IsResolved()
        {
            var container = Silent.ChemObjectBuilder.Instance.NewAtomContainer();
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("N", 0));
            container.Atoms.Add(Atom("O", 0));
            container.Atoms.Add(Atom("O", 0));
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);
            container.AddBond(container.Atoms[1], container.Atoms[2], BondOrder.Single);
            container.AddBond(container.Atoms[1], container.Atoms[3], BondOrder.Single);
            container.AddBond(container.Atoms[1], container.Atoms[4], BondOrder.Single);
            container.AddBond(container.Atoms[4], container.Atoms[5], BondOrder.Single);
            container.AddBond(container.Atoms[4], container.Atoms[6], BondOrder.Single);
            container.AddBond(container.Atoms[4], container.Atoms[7], BondOrder.Single);
            container.AddBond(container.Atoms[7], container.Atoms[8], BondOrder.Single);
            container.AddBond(container.Atoms[7], container.Atoms[9], BondOrder.Double);

            string[] expected = { "HC", "CR", "HC", "HC", "CR", "HC", "HC", "NO2", "O2N", "O2N" };
            string[] actual = Instance.SymbolicTypes(container);
            Assert.IsTrue(Compares.AreDeepEqual(expected, actual));
        }

        /// <summary>
        /// An old test from ForceFieldConfigurator. The expected atom types listed in that test are
        /// don't seem right, here 'CONN' and 'NC=O' is definitely correct. Previously the test expected
        /// N2OX but this is for nitrogen cations so '*[NH+]([O-])*', NC=O is more likely to be correct.
        /// </summary>
        [TestMethod()]
        public void Hydroxyurea()
        {
            var container = Silent.ChemObjectBuilder.Instance.NewAtomContainer();
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("O", 0));
            container.Atoms.Add(Atom("N", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("O", 0));
            container.Atoms.Add(Atom("N", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("H", 0));
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);
            container.AddBond(container.Atoms[1], container.Atoms[2], BondOrder.Single);
            container.AddBond(container.Atoms[2], container.Atoms[3], BondOrder.Single);
            container.AddBond(container.Atoms[2], container.Atoms[4], BondOrder.Single);
            container.AddBond(container.Atoms[4], container.Atoms[5], BondOrder.Double);
            container.AddBond(container.Atoms[4], container.Atoms[6], BondOrder.Single);
            container.AddBond(container.Atoms[6], container.Atoms[7], BondOrder.Single);
            container.AddBond(container.Atoms[6], container.Atoms[8], BondOrder.Single);
            string[] expected = { "HO", "-O-", "NC=O", "HNCO", "CONN", "O=CN", "NC=O", "HNCO", "HNCO" };
            string[] actual = Instance.SymbolicTypes(container);
            Assert.IsTrue(Compares.AreDeepEqual(expected, actual));
        }

        /// <summary>
        /// The MMFF articles mention H2 as a special case for assigning hydrogen types. However it is
        /// not mentioned what type they are assigned. This test simply shows molecular hydrogens don't
        /// break the assignment and are set to null.
        /// </summary>
        [TestMethod()]
        public void MolecularHydrogenDoesNotBreakAssignment()
        {
            var container = Silent.ChemObjectBuilder.Instance.NewAtomContainer();
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("H", 0));
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);
            string[] expected = { null, null };
            string[] actual = Instance.SymbolicTypes(container);
            Assert.IsTrue(Compares.AreDeepEqual(expected, actual));
        }

        /// <summary>
        /// MMFF94AtomTypeMatcherTest.testFindMatchingAtomType_IAtomContainer_IAtom_Methylamine. The
        /// nitrogen was being assigned NPYL by MMFF94AtomTypeMatcherTest. It is now assigned 'NR:
        /// NITROGEN IN ALIPHATIC AMINES'.
        /// </summary>
        [TestMethod()]
        public void Methylamine()
        {
            var container = Silent.ChemObjectBuilder.Instance.NewAtomContainer();
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("N", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("H", 0));
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);
            container.AddBond(container.Atoms[1], container.Atoms[2], BondOrder.Single);
            container.AddBond(container.Atoms[1], container.Atoms[3], BondOrder.Single);
            container.AddBond(container.Atoms[3], container.Atoms[4], BondOrder.Single);
            container.AddBond(container.Atoms[3], container.Atoms[5], BondOrder.Single);
            container.AddBond(container.Atoms[3], container.Atoms[6], BondOrder.Single);
            string[] expected = { "HNR", "NR", "HNR", "CR", "HC", "HC", "HC" };
            string[] actual = Instance.SymbolicTypes(container);
            Assert.IsTrue(Compares.AreDeepEqual(expected, actual));
        }

        /// <summary>
        /// MMFF94AtomTypeMatcherTest.testSthi would not assign STHI in thiophene. This is no longer the
        /// case.
        /// </summary>
        [TestMethod()]
        public void Thiophene()
        {
            var container = Silent.ChemObjectBuilder.Instance.NewAtomContainer();
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("S", 0));
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);
            container.AddBond(container.Atoms[1], container.Atoms[2], BondOrder.Double);
            container.AddBond(container.Atoms[2], container.Atoms[3], BondOrder.Single);
            container.AddBond(container.Atoms[2], container.Atoms[4], BondOrder.Single);
            container.AddBond(container.Atoms[4], container.Atoms[5], BondOrder.Single);
            container.AddBond(container.Atoms[4], container.Atoms[6], BondOrder.Double);
            container.AddBond(container.Atoms[6], container.Atoms[7], BondOrder.Single);
            container.AddBond(container.Atoms[6], container.Atoms[8], BondOrder.Single);
            container.AddBond(container.Atoms[1], container.Atoms[8], BondOrder.Single);
            string[] expected = { "HC", "C5A", "C5B", "HC", "C5B", "HC", "C5A", "HC", "STHI" };
            string[] actual = Instance.SymbolicTypes(container);
            Assert.IsTrue(Compares.AreDeepEqual(expected, actual));
        }

        /// <summary>
        /// MMFF94AtomTypeMatcherTest.testOar would not assign OFUR in thiophene. This is no longer the
        /// case. Note the CDK used 'Oar' instead of the actual 'OFUR' type.
        /// </summary>
        [TestMethod()]
        public void Furane()
        {
            var container = Silent.ChemObjectBuilder.Instance.NewAtomContainer();
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("O", 0));
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);
            container.AddBond(container.Atoms[1], container.Atoms[2], BondOrder.Double);
            container.AddBond(container.Atoms[2], container.Atoms[3], BondOrder.Single);
            container.AddBond(container.Atoms[2], container.Atoms[4], BondOrder.Single);
            container.AddBond(container.Atoms[4], container.Atoms[5], BondOrder.Single);
            container.AddBond(container.Atoms[4], container.Atoms[6], BondOrder.Double);
            container.AddBond(container.Atoms[6], container.Atoms[7], BondOrder.Single);
            container.AddBond(container.Atoms[6], container.Atoms[8], BondOrder.Single);
            container.AddBond(container.Atoms[1], container.Atoms[8], BondOrder.Single);
            string[] expected = { "HC", "C5A", "C5B", "HC", "C5B", "HC", "C5A", "HC", "OFUR" };
            string[] actual = Instance.SymbolicTypes(container);
            Assert.IsTrue(Compares.AreDeepEqual(expected, actual));
        }

        [TestMethod()]
        public void Methane()
        {
            var container = Silent.ChemObjectBuilder.Instance.NewAtomContainer();
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("H", 0));
            container.Atoms.Add(Atom("H", 0));
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);
            container.AddBond(container.Atoms[0], container.Atoms[2], BondOrder.Single);
            container.AddBond(container.Atoms[0], container.Atoms[3], BondOrder.Single);
            container.AddBond(container.Atoms[0], container.Atoms[4], BondOrder.Single);
            string[] expected = { "CR", "HC", "HC", "HC", "HC" };
            string[] actual = Instance.SymbolicTypes(container);
            Assert.IsTrue(Compares.AreDeepEqual(expected, actual));
        }

        [TestMethod()]
        [ExpectedException(typeof(IOException))]
        public void InvalidSmilesThrowsIOExceptionForTokenManagerError()
        {
            string row = "INVALID.SMILES X";
            using (var ins = new MemoryStream(Encoding.UTF8.GetBytes(row)))
            {
                MmffAtomTypeMatcher.LoadPatterns(ins);
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(IOException))]
        public void InvalidSmilesThrowsIOExceptionForIllegalArgument()
        {
            string row = "23 X";
            using (var ins = new MemoryStream(Encoding.UTF8.GetBytes(row)))
            {
                MmffAtomTypeMatcher.LoadPatterns(ins);
            }
        }

        static IAtom Atom(string symb, int h)
        {
            IAtom atom = Silent.ChemObjectBuilder.Instance.NewAtom(symb);
            atom.ImplicitHydrogenCount = h;
            return atom;
        }
    }
}
