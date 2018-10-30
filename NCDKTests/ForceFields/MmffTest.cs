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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Common.Base;
using NCDK.Smiles;
using NCDK.Tools.Manipulator;

namespace NCDK.ForceFields
{
    // @author John May
    [TestClass()]
    public class MmffTest
    {
        private static SmilesParser smipar = null;
        private static Mmff mmff = null;

        static MmffTest()
        {
            smipar = CDK.SilentSmilesParser;
            mmff = new Mmff();
        }

        [ClassCleanup()]
        public static void TearDown()
        {
            smipar = null;
            mmff = null;
        }

        [TestMethod()]
        public void TetrazoleAnion()
        {
            IAtomContainer mol = LoadSmi("[N-]1N=CN=N1");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            AssertAtomTypes(mol, "N5M", "N5M", "C5", "N5M", "N5M", "HC");
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, -0.25, -0.5875, 0.525, -0.5875, -0.25, 0.15);
            AssertPartialChargeSum(mol, -1);
        }

        [TestMethod()]
        public void Tetrazole()
        {
            IAtomContainer mol = LoadSmi("N1N=CN=N1");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            AssertAtomTypes(mol, "NPYL", "N5A", "C5B", "N5B", "N5A", "HPYL", "HC");
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, 0.566, -0.7068, 0.366, -0.2272, -0.418, 0.27, 0.15);
            AssertPartialChargeSum(mol, 0);
        }

        [TestMethod()]
        public void UntypedAtom()
        {
            IAtomContainer mol = LoadSmi("[Se]C1C=CC=C1");
            Assert.IsFalse(mmff.AssignAtomTypes(mol));
            AssertAtomTypes(mol, "UNK", "CR", "C=C", "C=C", "C=C", "C=C", "HC", "HC", "HC", "HC", "HC");
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, 0.0, 0.2764, -0.2882, -0.15, -0.15, -0.2882, 0.0, 0.15, 0.15, 0.15, 0.15);
            AssertPartialChargeSum(mol, 0);
        }

        [TestMethod()]
        public void ClearProps()
        {
            IAtomContainer mol = LoadSmi("o1cccc1");
            int sizeBefore = mol.GetProperties().Count;
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            Assert.IsTrue(mmff.PartialCharges(mol));
            mmff.ClearProps(mol);
            Assert.AreEqual(sizeBefore, mol.GetProperties().Count);
        }

        [TestMethod()]
        public void NitrobenzeneCovalent()
        {
            IAtomContainer mol = LoadSmi("c1ccccc1N(=O)=O");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            AssertAtomTypes(mol, "CB", "CB", "CB", "CB", "CB", "CB", "NO2", "O2N", "O2N", "HC", "HC", "HC", "HC", "HC");
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, -0.15, -0.15, -0.15, -0.15, -0.15, 0.133, 0.907, -0.52, -0.52, 0.15, 0.15, 0.15, 0.15, 0.15);
            AssertPartialChargeSum(mol, 0);
        }

        [TestMethod()]
        public void NitrobenzeneChargeSeparated()
        {
            IAtomContainer mol = LoadSmi("c1ccccc1[N+](-[O-])=O");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            AssertAtomTypes(mol, "CB", "CB", "CB", "CB", "CB", "CB", "NO2", "O2N", "O2N", "HC", "HC", "HC", "HC", "HC");
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, -0.15, -0.15, -0.15, -0.15, -0.15, 0.133, 0.907, -0.52, -0.52, 0.15, 0.15, 0.15, 0.15, 0.15);
            AssertPartialChargeSum(mol, 0);
        }

        /// <summary>TABLE V - CH3OH</summary>
        [TestMethod()]
        public void Methanol()
        {
            IAtomContainer mol = LoadSmi("CO");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, 0.28, -0.68, 0.0, 0.0, 0.0, 0.4);
            AssertPartialChargeSum(mol, 0);
        }

        /// <summary>TABLE V - CH3NH2</summary>
        [TestMethod()]
        public void Methylamine()
        {
            IAtomContainer mol = LoadSmi("CN");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, 0.27, -0.99, 0.0, 0.0, 0.0, 0.36, 0.36);
            AssertPartialChargeSum(mol, 0);
        }

        /// <summary>TABLE V - CH3CN</summary>
        [TestMethod()]
        public void Acetonitrile()
        {
            IAtomContainer mol = LoadSmi("CC#N");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, 0.2, 0.357, -0.557, 0.0, 0.0, 0.0);
            AssertPartialChargeSum(mol, 0);
        }

        /// <summary>TABLE V - CH3OCH3</summary>
        [TestMethod()]
        public void Dimethylether()
        {
            IAtomContainer mol = LoadSmi("COC");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, 0.28, -0.56, 0.28, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
            AssertPartialChargeSum(mol, 0);
        }

        /// <summary>TABLE V - CH3SH</summary>
        [TestMethod()]
        public void Methanethiol()
        {
            IAtomContainer mol = LoadSmi("CS");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, 0.23, -0.41, 0.0, 0.0, 0.0, 0.18);
            AssertPartialChargeSum(mol, 0);
        }

        /// <summary>TABLE V - CH3Cl</summary>
        [TestMethod()]
        public void Chloromethane()
        {
            IAtomContainer mol = LoadSmi("CCl");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, 0.29, -0.29, 0.0, 0.0, 0.0);
            AssertPartialChargeSum(mol, 0);
        }

        /// <summary>TABLE V - C2H6</summary>
        [TestMethod()]
        public void Ethane()
        {
            IAtomContainer mol = LoadSmi("CC");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
            AssertPartialChargeSum(mol, 0);
        }

        /// <summary>TABLE V - CH3CONH2 (note wrong formula)</summary>
        [TestMethod()]
        public void Acetamide()
        {
            IAtomContainer mol = LoadSmi("O=C(N)C");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, -0.57, 0.569, -0.8, 0.061, 0.37, 0.37, 0.0, 0.0, 0.0);
            AssertPartialChargeSum(mol, 0);
        }

        /// <summary>TABLE V - CH3COOH</summary>
        [TestMethod()]
        public void AceticAcid()
        {
            IAtomContainer mol = LoadSmi("CC(O)=O");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, 0.061, 0.659, -0.65, -0.57, 0.0, 0.0, 0.0, 0.5);
            AssertPartialChargeSum(mol, 0);
        }

        /// <summary>TABLE V - (CH3)2CO</summary>
        [TestMethod()]
        public void Acetone()
        {
            IAtomContainer mol = LoadSmi("CC(=O)C");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, 0.061, 0.447, -0.57, 0.061, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
            AssertPartialChargeSum(mol, 0);
        }

        /// <summary>TABLE V - CH3COOCH3</summary>
        [TestMethod()]
        public void Methylacetate()
        {
            IAtomContainer mol = LoadSmi("O=C(OC)C");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, -0.57, 0.659, -0.43, 0.28, 0.061, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
            AssertPartialChargeSum(mol, 0);
        }

        /// <summary>TABLE V - C6H6</summary>
        [TestMethod()]
        public void Benzene()
        {
            IAtomContainer mol = LoadSmi("c1ccccc1");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, -0.15, -0.15, -0.15, -0.15, -0.15, -0.15, 0.15, 0.15, 0.15, 0.15, 0.15, 0.15);
            AssertPartialChargeSum(mol, 0);
        }

        /// <summary>TABLE V - C5H5N</summary>
        [TestMethod()]
        public void Pyridine()
        {
            IAtomContainer mol = LoadSmi("C1=CC=NC=C1");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, -0.15, -0.15, 0.16, -0.62, 0.16, -0.15, 0.15, 0.15, 0.15, 0.15, 0.15);
            AssertPartialChargeSum(mol, 0);
        }

        /// <summary>TABLE V - C6H5NH2</summary>
        [TestMethod()]
        public void Aniline()
        {
            IAtomContainer mol = LoadSmi("C1=CC=C(N)C=C1");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, -0.15, -0.15, -0.15, 0.1, -0.9, -0.15, -0.15, 0.15, 0.15, 0.15, 0.4, 0.4, 0.15, 0.15);
            AssertPartialChargeSum(mol, 0);
        }

        /// <summary>TABLE V - imidazole</summary>
        [TestMethod()]
        public void Imidazole()
        {
            IAtomContainer mol = LoadSmi("C=1NC=NC1");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, -0.3016, 0.0332, 0.0365, -0.5653, 0.0772, 0.15, 0.27, 0.15, 0.15);
            AssertPartialChargeSum(mol, 0);
        }

        /// <summary>TABLE V - H2O</summary>
        [TestMethod()]
        public void Water()
        {
            IAtomContainer mol = LoadSmi("O");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, -0.86, 0.43, 0.43);
            AssertPartialChargeSum(mol, 0);
        }

        /// <summary>TABLE V - CH3CO2-</summary>
        [TestMethod()]
        public void Acetate()
        {
            IAtomContainer mol = LoadSmi("CC([O-])=O");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, -0.106, 0.906, -0.9, -0.9, 0.0, 0.0, 0.0);
            AssertPartialChargeSum(mol, -1);
        }

        /// <summary>TABLE V - CH3NH3(+)</summary>
        [TestMethod()]
        public void Methanaminium()
        {
            IAtomContainer mol = LoadSmi("C[NH3+]");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, 0.503, -0.853, 0.0, 0.0, 0.0, 0.45, 0.45, 0.45);
            AssertPartialChargeSum(mol, +1);
        }

        /// <summary>TABLE V - Imidazolium(+)</summary>
        [TestMethod()]
        public void Imidazolium()
        {
            IAtomContainer mol = LoadSmi("[nH+]1c[nH]cc1");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, -0.7, 0.65, -0.7, 0.2, 0.2, 0.45, 0.15, 0.45, 0.15, 0.15);
            AssertPartialChargeSum(mol, +1);
        }

        /// <summary>TABLE V - (-)O2C(CH2)6NH3(+)</summary>
        [TestMethod()]
        public void Test_7aminoheptanoicAcid()
        {
            IAtomContainer mol = LoadSmi("[NH3+]CCCCCCC([O-])=O");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, -0.853, 0.503, 0.0, 0.0, 0.0, 0.0, -0.106, 0.906, -0.9, -0.9, 0.45, 0.45, 0.45, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
            AssertPartialChargeSum(mol, 0);
        }

        [TestMethod()]
        public void Ethoxyethane()
        {
            IAtomContainer mol = LoadSmi("CCOCC");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, 0.0, 0.28, -0.56, 0.28, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
            AssertPartialChargeSum(mol, 0);
        }

        private IAtomContainer LoadSmi(string smi)
        {
            var mol = smipar.ParseSmiles(smi);
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(mol);
            return mol;
        }

        /// <summary>[PO4]3-</summary>
        [TestMethod()]
        public void Phosphate()
        {
            IAtomContainer mol = LoadSmi("[O-]P([O-])([O-])=O");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, -1.075, 1.3, -1.075, -1.075, -1.075);
        }

        /// <summary>[HOPO3]2-</summary>
        [TestMethod()]
        public void HydrogenPhosphate()
        {
            IAtomContainer mol = LoadSmi("OP([O-])([O-])=O");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, -0.7712, 1.3712, -1.033, -1.033, -1.033, 0.5);
        }

        /// <summary>[H2OPO3]-</summary>
        [TestMethod()]
        public void DihydrogenPhosphate()
        {
            IAtomContainer mol = LoadSmi("OP([O-])(O)=O");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, -0.7712, 1.4424, -0.95, -0.7712, -0.95, 0.5, 0.5);
        }

        /// <summary>H3OPO3</summary>
        [TestMethod()]
        public void PhosphoricAcid()
        {
            IAtomContainer mol = LoadSmi("OP(O)(O)=O");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, -0.7712, 1.514, -0.7712, -0.7712, -0.7, 0.5, 0.5, 0.5);
        }

        /// <summary>SEYWUO - validation suite showing positive charge charging</summary>
        [TestMethod()]
        public void SEYWUO()
        {
            IAtomContainer mol = LoadSmi("[H]OC(=S)[N-][N+]1=C(N([H])[H])C([H])([H])N([H])C1=O");
            Assert.IsTrue(mmff.AssignAtomTypes(mol));
            AssertAtomTypes(mol, "HOCS", "OC=S", "C=S", "S=C", "NM", "NCN+", "CNN+", "NCN+", "HNN+", "HNN+", "CR", "HC", "HC", "NC=O", "HNCO", "CONN", "O=CN");
            Assert.IsTrue(mmff.EffectiveCharges(mol));
            AssertPartialCharges(mol, 0.0, 0.0, -0.25, 0.0, -0.5, 0.25, 0.0, 0.5, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
            Assert.IsTrue(mmff.PartialCharges(mol));
            AssertPartialCharges(mol, 0.4, -0.55, 0.31, -0.38, -0.179, -0.8364, 0.6038, -0.7544, 0.45, 0.45, 0.4051, 0.0, 0.0, -0.7301, 0.37, 1.011, -0.57);
        }

        private void AssertAtomTypes(IAtomContainer mol, params string[] expected)
        {
            string[] actual = new string[mol.Atoms.Count];
            for (int i = 0; i < mol.Atoms.Count; i++)
                actual[i] = mol.Atoms[i].AtomTypeName;
            Assert.IsTrue(Compares.AreDeepEqual(expected, actual));
        }

        private void AssertPartialCharges(IAtomContainer mol, params double[] expected)
        {
            double[] actual = new double[mol.Atoms.Count];
            for (int i = 0; i < mol.Atoms.Count; i++)
                actual[i] = mol.Atoms[i].Charge.Value;
            Assert.AreEqual(expected.Length, actual.Length);
            for (var x = 0; x < actual.Length; x++)
                Assert.AreEqual(expected[x], actual[x], 0.001);
        }

        private void AssertPartialChargeSum(IAtomContainer mol, double expected)
        {
            double actual = 0;
            for (int i = 0; i < mol.Atoms.Count; i++)
                actual += mol.Atoms[i].Charge.Value;
            Assert.AreEqual(expected, actual, 0.001, "Unexpected partial charge sum");
        }
    }
}
