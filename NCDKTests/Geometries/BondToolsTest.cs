/* Copyright (C) 2007  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Config;
using NCDK.Default;
using NCDK.IO;


namespace NCDK.Geometries
{
    /// <summary>
    // @cdk.module test-standard
    /// </summary>
    [TestClass()]
    public class BondToolsTest : CDKTestCase
    {

        public BondToolsTest()
            : base()
        { }

        [TestMethod()]
        public void TestIsValidDoubleBondConfiguration_IAtomContainer_IBond()
        {
            string filename = "NCDK.Data.MDL.testdoublebondconfig.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            IAtomContainer mol = chemFile[0][0].MoleculeSet[0];
            Assert.IsTrue(BondTools.IsValidDoubleBondConfiguration(mol, mol.Bonds[0]));
            Assert.IsFalse(BondTools.IsValidDoubleBondConfiguration(mol, mol.Bonds[1]));
            Assert.IsFalse(BondTools.IsValidDoubleBondConfiguration(mol, mol.Bonds[2]));
            Assert.IsFalse(BondTools.IsValidDoubleBondConfiguration(mol, mol.Bonds[3]));
            Assert.IsFalse(BondTools.IsValidDoubleBondConfiguration(mol, mol.Bonds[4]));
            Assert.IsFalse(BondTools.IsValidDoubleBondConfiguration(mol, mol.Bonds[5]));
            Assert.IsFalse(BondTools.IsValidDoubleBondConfiguration(mol, mol.Bonds[6]));
            Assert.IsFalse(BondTools.IsValidDoubleBondConfiguration(mol, mol.Bonds[7]));
        }

        [TestMethod()]
        public void TestIsCIsTrans_IAtom_IAtom_IAtom_IAtom_IAtomContainer()
        {
            string filename = "NCDK.Data.MDL.testdoublebondconfig.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            IAtomContainer mol = chemFile[0][0].MoleculeSet[0];
            Assert.IsFalse(BondTools.IsCisTrans(mol.Atoms[2], mol.Atoms[0], mol.Atoms[1], mol.Atoms[4], mol));
        }

        [TestMethod()]
        public void TestIsLeft_IAtom_IAtom_IAtom()
        {
            string filename = "NCDK.Data.MDL.testdoublebondconfig.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            IAtomContainer mol = chemFile[0][0].MoleculeSet[0];
            Assert.IsFalse(BondTools.IsLeft(mol.Atoms[1], mol.Atoms[0], mol.Atoms[2]));
        }

        [TestMethod()]
        public void TestGiveAngleBothMethods_IAtom_IAtom_IAtom_bool()
        {
            string filename = "NCDK.Data.MDL.testdoublebondconfig.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            IAtomContainer mol = chemFile[0][0].MoleculeSet[0];
            Assert.AreEqual(2.0943946986086157,
                    BondTools.GiveAngleBothMethods(mol.Atoms[0], mol.Atoms[2], mol.Atoms[3], true), 0.2);
            Assert.AreEqual(2.0943946986086157,
                    BondTools.GiveAngleBothMethods(mol.Atoms[0], mol.Atoms[2], mol.Atoms[3], false), 0.2);
        }

        /// <summary>
        /// Make sure the the rebonding is working.
        /// </summary>
        [TestMethod()]
        public void TestCloseEnoughToBond_IAtom_IAtom_Double()
        {
            string filename = "NCDK.Data.XYZ.viagra.xyz";
            var ins = ResourceLoader.GetAsStream(filename);
            XYZReader reader = new XYZReader(ins);
            AtomTypeFactory atf = AtomTypeFactory.GetInstance("NCDK.Config.Data.jmol_atomtypes.txt",
                    Silent.ChemObjectBuilder.Instance);
            ChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            IAtomContainer mol = chemFile[0][0].MoleculeSet[0];
            foreach (var atom in mol.Atoms)
            {
                atf.Configure(atom);
            }
            Assert.IsTrue(BondTools.CloseEnoughToBond(mol.Atoms[0], mol.Atoms[1], 1));
            Assert.IsFalse(BondTools.CloseEnoughToBond(mol.Atoms[0], mol.Atoms[8], 1));
        }

        [TestMethod()]
        public void TestGiveAngleBothMethods_Point2d_Point2d_Point2d_bool()
        {
            string filename = "NCDK.Data.MDL.testdoublebondconfig.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            IAtomContainer mol = chemFile[0][0].MoleculeSet[0];
            Assert.AreEqual(2.0943946986086157, BondTools.GiveAngleBothMethods(mol.Atoms[0].Point2D.Value, mol
                    .Atoms[2].Point2D.Value, mol.Atoms[3].Point2D.Value, true), 0.2);
            Assert.AreEqual(2.0943946986086157, BondTools.GiveAngleBothMethods(mol.Atoms[0].Point2D.Value, mol
                    .Atoms[2].Point2D.Value, mol.Atoms[3].Point2D.Value, false), 0.2);
        }

        [TestMethod()]
        public void TestIsTetrahedral_IAtomContainer_IAtom_bool()
        {
            string filename = "NCDK.Data.MDL.tetrahedral_1.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            IAtomContainer mol = chemFile[0][0].MoleculeSet[0];
            Assert.AreEqual(BondTools.IsTetrahedral(mol, mol.Atoms[0], true), 1);
            Assert.AreEqual(BondTools.IsTetrahedral(mol, mol.Atoms[1], true), 0);
            filename = "NCDK.Data.MDL.tetrahedral_1_lazy.mol";
            ins = ResourceLoader.GetAsStream(filename);
            reader = new MDLV2000Reader(ins);
            chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            mol = chemFile[0][0].MoleculeSet[0];
            Assert.AreEqual(BondTools.IsTetrahedral(mol, mol.Atoms[0], true), 0);
            Assert.AreEqual(BondTools.IsTetrahedral(mol, mol.Atoms[0], false), 3);
        }

        [TestMethod()]
        public void TestIsTrigonalBipyramidalOrOctahedral_IAtomContainer_IAtom()
        {
            string filename = "NCDK.Data.MDL.trigonal_bipyramidal.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            IAtomContainer mol = chemFile[0][0].MoleculeSet[0];
            Assert.AreEqual(BondTools.IsTrigonalBipyramidalOrOctahedral(mol, mol.Atoms[0]), 1);
            Assert.AreEqual(BondTools.IsTrigonalBipyramidalOrOctahedral(mol, mol.Atoms[1]), 0);
        }

        [TestMethod()]
        public void TestIsStereo_IAtomContainer_IAtom()
        {
            string filename = "NCDK.Data.MDL.trigonal_bipyramidal.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            IAtomContainer mol = chemFile[0][0].MoleculeSet[0];
            Assert.IsTrue(BondTools.IsStereo(mol, mol.Atoms[0]));
            Assert.IsFalse(BondTools.IsStereo(mol, mol.Atoms[1]));
        }

        [TestMethod()]
        public void TestIsStereo_IAtomContainer_IAtom_forinvalid()
        {
            string filename = "NCDK.Data.MDL.trigonal_bipyramidal.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            IAtomContainer mol = chemFile[0][0].MoleculeSet[0];
            for (int i = 1; i < 6; i++)
            {
                mol.Atoms[i].Symbol = "C";
            }
            Assert.IsFalse(BondTools.IsStereo(mol, mol.Atoms[0]));
            Assert.IsFalse(BondTools.IsStereo(mol, mol.Atoms[1]));
        }

        [TestMethod()]
        public void TestIsSquarePlanar_IAtomContainer_IAtom()
        {
            string filename = "NCDK.Data.MDL.squareplanar.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            IAtomContainer mol = chemFile[0][0].MoleculeSet[0];
            Assert.IsTrue(BondTools.IsSquarePlanar(mol, mol.Atoms[0]));
            Assert.IsFalse(BondTools.IsSquarePlanar(mol, mol.Atoms[1]));
        }

        [TestMethod()]
        public void TestStereosAreOpposite_IAtomContainer_IAtom()
        {
            string filename = "NCDK.Data.MDL.squareplanar.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            IAtomContainer mol = chemFile[0][0].MoleculeSet[0];
            Assert.IsFalse(BondTools.StereosAreOpposite(mol, mol.Atoms[0]));
            filename = "NCDK.Data.MDL.tetrahedral_with_four_wedges.mol";
            ins = ResourceLoader.GetAsStream(filename);
            reader = new MDLV2000Reader(ins);
            chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            mol = chemFile[0][0].MoleculeSet[0];
            Assert.IsTrue(BondTools.StereosAreOpposite(mol, mol.Atoms[0]));
        }

        [TestMethod()]
        public void TestMakeUpDownBonds_IAtomContainer()
        {
            string filename = "NCDK.Data.MDL.tetrahedral_2_lazy.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            IAtomContainer mol = chemFile[0][0].MoleculeSet[0];
            BondTools.MakeUpDownBonds(mol);
            Assert.AreEqual(BondStereo.Down, mol.Bonds[3].Stereo);
        }

        [TestMethod()]
        public void TestGiveAngle_IAtom_IAtom_IAtom()
        {
            string filename = "NCDK.Data.MDL.testdoublebondconfig.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            IAtomContainer mol = chemFile[0][0].MoleculeSet[0];
            Assert.AreEqual(2.0943946986086157, BondTools.giveAngle(mol.Atoms[0], mol.Atoms[2], mol.Atoms[3]),
                    0.2);
        }

        [TestMethod()]
        public void TestGiveAngleFromMiddle_IAtom_IAtom_IAtom()
        {
            string filename = "NCDK.Data.MDL.testdoublebondconfig.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            IAtomContainer mol = chemFile[0][0].MoleculeSet[0];
            Assert.AreEqual(2.0943946986086157,
                    BondTools.GiveAngleFromMiddle(mol.Atoms[0], mol.Atoms[2], mol.Atoms[3]), 0.2);
        }

        /// <summary>
        // @cdk.bug 2831420
        /// </summary>
        [TestMethod()]
        public void TestBug2831420()
        {
            string filename = "NCDK.Data.MDL.bug2831420.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            IAtomContainer mol = chemFile[0][0].MoleculeSet[0];
            Assert.IsTrue(BondTools.IsStereo(mol, mol.Atoms[5]));
        }
    }
}
