/* Copyright (C) 2009  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Smiles;
using NCDK.Tools.Manipulator;
using System.Linq;

namespace NCDK.AtomTypes
{
    /// <summary>
    /// This class tests the matching of atom types defined in the
    /// CDK atom type list, starting from SMILES strings.
    /// </summary>
    // @cdk.module test-core
    [TestClass()]
    public class CDKAtomTypeMatcherSMILESTest : AbstractCDKAtomTypeTest
    {
        private static SmilesParser smilesParser = CDK.SilentSmilesParser;
        private static IAtomTypeMatcher atomTypeMatcher = CDK.AtomTypeMatcher;

        /// <summary>
        // @cdk.bug 2826961
        /// </summary>
        [TestMethod()]
        public void TestIdenticalTypes()
        {
            var smiles1 = "CN(C)CCC1=CNC2=C1C=C(C=C2)CC1NC(=O)OC1";
            var smiles2 = "CN(C)CCC1=CNc2c1cc(cc2)CC1NC(=O)OC1";

            var mol1 = smilesParser.ParseSmiles(smiles1);
            var mol2 = smilesParser.ParseSmiles(smiles2);

            Assert.AreEqual(mol1.Atoms.Count, mol2.Atoms.Count);
            Assert.AreEqual(mol1.Bonds.Count, mol2.Bonds.Count);

            var types1 = atomTypeMatcher.FindMatchingAtomTypes(mol1).ToReadOnlyList();
            var types2 = atomTypeMatcher.FindMatchingAtomTypes(mol2).ToReadOnlyList();
            for (int i = 0; i < mol1.Atoms.Count; i++)
            {
                Assert.AreEqual(types1[i].AtomTypeName, types2[i].AtomTypeName);
            }
        }

        [TestMethod()]
        public void TestNitrogen()
        {
            string smiles1 = "c1c2cc[NH]cc2nc1";

            var mol1 = smilesParser.ParseSmiles(smiles1);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol1);

            Assert.AreEqual(9, mol1.Atoms.Count);

            var types1 = atomTypeMatcher.FindMatchingAtomTypes(mol1);
            foreach (var type in types1)
            {
                Assert.IsNotNull(type.AtomTypeName);
            }
        }

        [TestMethod()]
        public void TestNitrogen_SP2()
        {
            string smiles1 = "c1c2cc[nH]cc2nc1";
            var mol1 = smilesParser.ParseSmiles(smiles1);
            Assert.AreEqual(9, mol1.Atoms.Count);

            var types1 = atomTypeMatcher.FindMatchingAtomTypes(mol1);
            foreach (var type in types1)
            {
                Assert.IsNotNull(type.AtomTypeName);
            }
        }

        // @cdk.bug 2976054
        [TestMethod()]
        public void TestAnotherNitrogen_SP2()
        {
            var smiles1 = "c1cnc2s[cH][cH]n12";
            var mol1 = smilesParser.ParseSmiles(smiles1);

            Assert.AreEqual(8, mol1.Atoms.Count);
            var types1 = atomTypeMatcher.FindMatchingAtomTypes(mol1);
            foreach (var type in types1)
            {
                Assert.IsNotNull(type.AtomTypeName);
            }
        }

        // @cdk.bug 1294
        [TestMethod()]
        public void TestBug1294()
        {
            string smiles1 = "c2c1ccccc1c[nH]2";
            string smiles2 = "C2=C1C=CC=CC1=CN2";

            var mol1 = smilesParser.ParseSmiles(smiles1);
            var mol2 = smilesParser.ParseSmiles(smiles2);

            Assert.AreEqual(mol1.Atoms.Count, mol2.Atoms.Count);
            Assert.AreEqual(mol1.Bonds.Count, mol2.Bonds.Count);

            var types1 = atomTypeMatcher.FindMatchingAtomTypes(mol1).ToReadOnlyList();
            var types2 = atomTypeMatcher.FindMatchingAtomTypes(mol2).ToReadOnlyList();
            for (int i = 0; i < mol1.Atoms.Count; i++)
            {
                Assert.AreEqual(types1[i].AtomTypeName, types2[i].AtomTypeName);
            }
        }

        // @cdk.bug 3093644
        [TestMethod()]
        public void TestBug3093644()
        {
            var smiles1 = "[H]C5(CCC(N)=O)(C=1N=C(C=C4N=C(C(C)=C3[N-]C(C)(C2N=C(C=1(C))C(C)"
                    + "(CCC(=O)NCC(C)O)C2([H])(CC(N)=O))C(C)(CC(N)=O)C3([H])(CCC(N)=O))"
                    + "C(C)(CC(N)=O)C4([H])(CCC(N)=O))C5(C)(C)).[H][C-]([H])C3([H])(OC([H])"
                    + "(N2C=NC=1C(N)=NC=NC=12)C([H])(O)C3([H])(O)).[Co+3]";

            var mol1 = smilesParser.ParseSmiles(smiles1);
            var types1 = atomTypeMatcher.FindMatchingAtomTypes(mol1);
            foreach (var type in types1)
            {
                Assert.IsNotNull(type.AtomTypeName);
            }
        }

        [TestMethod()]
        public void TestPlatinum4()
        {
            var smiles1 = "Cl[Pt]1(Cl)(Cl)(Cl)NC2CCCCC2N1";

            var mol1 = smilesParser.ParseSmiles(smiles1);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol1);
            Assert.AreEqual(13, mol1.Atoms.Count);
            Assert.AreEqual("Pt.6", mol1.Atoms[1].AtomTypeName);
        }

        [TestMethod()]
        public void TestPlatinum6()
        {
            var smiles1 = "[Pt](Cl)(Cl)(N)N";

            var mol1 = smilesParser.ParseSmiles(smiles1);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol1);
            Assert.AreEqual(5, mol1.Atoms.Count);
            Assert.AreEqual("Pt.4", mol1.Atoms[0].AtomTypeName);
        }

        [TestMethod()]
        public void TestAmineOxide()
        {
            var smiles = "CN(C)(=O)CCC=C2c1ccccc1CCc3ccccc23";

            var mol = smilesParser.ParseSmiles(smiles);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);

            Assert.AreEqual("N.oxide", mol.Atoms[1].AtomTypeName);
        }

        [TestMethod()]
        public void TestYetAnotherNitrogen()
        {
            var smiles = "CCCN1CC(CSC)CC2C1Cc3c[nH]c4cccc2c34";

            var mol = smilesParser.ParseSmiles(smiles);
            var types = atomTypeMatcher.FindMatchingAtomTypes(mol);
            foreach (var type in types)
            {
                Assert.IsNotNull(type.AtomTypeName);
            }
        }

        [TestMethod()]
        public void Test4Sulphur()
        {
            var smiles = "Br.Br.CS(CCC(N)C#N)C[C@H]1OC([C@H](O)[C@@H]1O)N2cnc3c(N)ncnc23";

            var mol = smilesParser.ParseSmiles(smiles);
            var types = atomTypeMatcher.FindMatchingAtomTypes(mol);
            foreach (var type in types)
            {
                Assert.IsNotNull(type.AtomTypeName);
            }
        }

        [TestMethod()]
        public void TestTellaneLike()
        {
            var smiles = "Clc1cccc(N2CCN(CCCCNC(=O)C3=Cc4ccccc4[Te]3)CC2)c1Cl";
            var mol = smilesParser.ParseSmiles(smiles);
            foreach (var atom in mol.Atoms)
                Assert.AreNotSame("X", atom.AtomTypeName);
        }
    }
}
