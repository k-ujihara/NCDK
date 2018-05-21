/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.Numerics;
using System.IO;
using System.Text;

namespace NCDK.IO.CML
{
    /// <summary>
    /// Atomic tests for reading CML documents. All tested CML strings are valid CML 2,
    /// as can be determined in NCDK.IO.CML.cml/cmlTestFramework.xml.
    /// </summary>
    // @cdk.module test-io
    // @author Egon Willighagen <egonw@sci.kun.nl>
    [TestClass()]
    public class Jumbo46CMLFragmentsTest : CDKTestCase
    {
        [TestMethod()]
        public void TestAtomId()
        {
            string cmlString = "<molecule id='m1'><atomArray><atom id='a1'/></atomArray></molecule>";

            IChemFile chemFile = ParseCMLString(cmlString);
            IAtomContainer mol = CheckForSingleMoleculeFile(chemFile);

            Assert.AreEqual(1, mol.Atoms.Count);
            IAtom atom = mol.Atoms[0];
            Assert.AreEqual("a1", atom.Id);
        }

        [TestMethod()]
        public void TestAtomId3()
        {
            string cmlString = "<molecule id='m1'><atomArray atomID='a1 a2 a3'/></molecule>";

            IChemFile chemFile = ParseCMLString(cmlString);
            IAtomContainer mol = CheckForSingleMoleculeFile(chemFile);

            Assert.AreEqual(3, mol.Atoms.Count);
            IAtom atom = mol.Atoms[1];
            Assert.AreEqual("a2", atom.Id);
        }

        [TestMethod()]
        public void TestAtomElementType3()
        {
            string cmlString = "<molecule id='m1'><atomArray atomID='a1' elementType='C'/></molecule>";

            IChemFile chemFile = ParseCMLString(cmlString);
            IAtomContainer mol = CheckForSingleMoleculeFile(chemFile);

            Assert.AreEqual(1, mol.Atoms.Count);
            IAtom atom = mol.Atoms[0];
            Assert.AreEqual("C", atom.Symbol);
        }

        [TestMethod()]
        public void TestBond()
        {
            string cmlString = "<molecule id='m1'><atomArray><atom id='a1'/><atom id='a2'/></atomArray><bondArray><bond id='b1' atomRefs2='a1 a2'/></bondArray></molecule>";

            IChemFile chemFile = ParseCMLString(cmlString);
            IAtomContainer mol = CheckForSingleMoleculeFile(chemFile);

            Assert.AreEqual(2, mol.Atoms.Count);
            Assert.AreEqual(1, mol.Bonds.Count);
            IBond bond = mol.Bonds[0];
            Assert.AreEqual(2, bond.Atoms.Count);
            IAtom atom1 = bond.Begin;
            IAtom atom2 = bond.End;
            Assert.AreEqual("a1", atom1.Id);
            Assert.AreEqual("a2", atom2.Id);
        }

        [TestMethod()]
        public void TestBond4()
        {
            string cmlString = "<molecule id='m1'><atomArray atomID='a1 a2 a3'/><bondArray atomRef1='a1 a1' atomRef2='a2 a3' bondID='b1 b2'/></molecule>";

            IChemFile chemFile = ParseCMLString(cmlString);
            IAtomContainer mol = CheckForSingleMoleculeFile(chemFile);

            Assert.AreEqual(3, mol.Atoms.Count);
            Assert.AreEqual(2, mol.Bonds.Count);
            IBond bond = mol.Bonds[0];
            Assert.AreEqual(2, bond.Atoms.Count);
            IAtom atom1 = bond.Begin;
            IAtom atom2 = bond.End;
            Assert.AreEqual("a1", atom1.Id);
            Assert.AreEqual("a2", atom2.Id);
            Assert.AreEqual("b2", mol.Bonds[1].Id);
        }

        [TestMethod()]
        public void TestBond5()
        {
            string cmlString = "<molecule id='m1'><atomArray atomID='a1 a2 a3'/><bondArray atomRef1='a1 a1' atomRef2='a2 a3' order='1 1'/></molecule>";

            IChemFile chemFile = ParseCMLString(cmlString);
            IAtomContainer mol = CheckForSingleMoleculeFile(chemFile);

            Assert.AreEqual(3, mol.Atoms.Count);
            Assert.AreEqual(2, mol.Bonds.Count);
            IBond bond = mol.Bonds[0];
            Assert.AreEqual(2, bond.Atoms.Count);
            Assert.AreEqual(BondOrder.Single, bond.Order);
            bond = mol.Bonds[1];
            Assert.AreEqual(2, bond.Atoms.Count);
            Assert.AreEqual(BondOrder.Single, bond.Order);
        }

        [TestMethod()]
        public void TestBondAromatic()
        {
            string cmlString = "<molecule id='m1'><atomArray atomID='a1 a2'/><bondArray atomRef1='a1' atomRef2='a2' order='A'/></molecule>";
            IChemFile chemFile = ParseCMLString(cmlString);
            IAtomContainer mol = CheckForSingleMoleculeFile(chemFile);

            Assert.AreEqual(2, mol.Atoms.Count);
            Assert.AreEqual(1, mol.Bonds.Count);
            IBond bond = mol.Bonds[0];
            Assert.AreEqual(BondOrder.Single, bond.Order);
            Assert.IsTrue(bond.IsAromatic);
        }

        [TestMethod()]
        public void TestBondId()
        {
            string cmlString = "<molecule id='m1'><atomArray><atom id='a1'/><atom id='a2'/></atomArray><bondArray><bond id='b1' atomRefs2='a1 a2'/></bondArray></molecule>";

            IChemFile chemFile = ParseCMLString(cmlString);
            IAtomContainer mol = CheckForSingleMoleculeFile(chemFile);

            Assert.AreEqual(2, mol.Atoms.Count);
            Assert.AreEqual(1, mol.Bonds.Count);
            IBond bond = mol.Bonds[0];
            Assert.AreEqual("b1", bond.Id);
        }

        [TestMethod()]
        public void TestList()
        {
            string cmlString = "<list>"
                    + "<molecule id='m1'><atomArray><atom id='a1'/><atom id='a2'/></atomArray><bondArray><bond id='b1' atomRefs2='a1 a2'/></bondArray></molecule>"
                    + "<molecule id='m2'><atomArray><atom id='a1'/><atom id='a2'/></atomArray><bondArray><bond id='b1' atomRefs2='a1 a2'/></bondArray></molecule>"
                    + "</list>";

            IChemFile chemFile = ParseCMLString(cmlString);
            CheckForXMoleculeFile(chemFile, 2);
        }

        [TestMethod()]
        public void TestCoordinates2D()
        {
            string cmlString = "<molecule id='m1'><atomArray atomID='a1 a2' x2='0.0 0.1' y2='1.2 1.3'/></molecule>";

            IChemFile chemFile = ParseCMLString(cmlString);
            IAtomContainer mol = CheckForSingleMoleculeFile(chemFile);

            Assert.AreEqual(2, mol.Atoms.Count);
            Assert.IsNotNull(mol.Atoms[0].Point2D);
            Assert.IsNotNull(mol.Atoms[1].Point2D);
            Assert.IsNull(mol.Atoms[0].Point3D);
            Assert.IsNull(mol.Atoms[1].Point3D);
        }

        [TestMethod()]
        public void TestCoordinates3D()
        {
            string cmlString = "<molecule id='m1'><atomArray atomID='a1 a2' x3='0.0 0.1' y3='1.2 1.3' z3='2.1 2.5'/></molecule>";

            IChemFile chemFile = ParseCMLString(cmlString);
            IAtomContainer mol = CheckForSingleMoleculeFile(chemFile);

            Assert.AreEqual(2, mol.Atoms.Count);
            Assert.IsNull(mol.Atoms[0].Point2D);
            Assert.IsNull(mol.Atoms[1].Point2D);
            Assert.IsNotNull(mol.Atoms[0].Point3D);
            Assert.IsNotNull(mol.Atoms[1].Point3D);
        }

        [TestMethod()]
        public void TestFractional3D()
        {
            string cmlString = "<molecule id='m1'><atomArray atomID='a1 a2' xFract='0.0 0.1' yFract='1.2 1.3' zFract='2.1 2.5'/></molecule>";

            IChemFile chemFile = ParseCMLString(cmlString);
            IAtomContainer mol = CheckForSingleMoleculeFile(chemFile);

            Assert.AreEqual(2, mol.Atoms.Count);
            Assert.IsNull(mol.Atoms[0].Point3D);
            Assert.IsNull(mol.Atoms[1].Point3D);
            Assert.IsNotNull(mol.Atoms[0].FractionalPoint3D);
            Assert.IsNotNull(mol.Atoms[1].FractionalPoint3D);
        }

        [TestMethod()]
        public void TestMissing2DCoordinates()
        {
            string cmlString = "<molecule id='m1'><atomArray><atom id='a1' xy2='0.0 0.1'/><atom id='a2'/><atom id='a3' xy2='0.1 0.0'/></atomArray></molecule>";

            IChemFile chemFile = ParseCMLString(cmlString);
            IAtomContainer mol = CheckForSingleMoleculeFile(chemFile);

            Assert.AreEqual(3, mol.Atoms.Count);
            IAtom atom1 = mol.Atoms[0];
            IAtom atom2 = mol.Atoms[1];
            IAtom atom3 = mol.Atoms[2];

            Assert.IsNotNull(atom1.Point2D);
            Assert.IsNull(atom2.Point2D);
            Assert.IsNotNull(atom3.Point2D);
        }

        [TestMethod()]
        public void TestMissing3DCoordinates()
        {
            string cmlString = "<molecule id='m1'><atomArray><atom id='a1' xyz3='0.0 0.1 0.2'/><atom id='a2'/><atom id='a3' xyz3='0.1 0.0 0.2'/></atomArray></molecule>";

            IChemFile chemFile = ParseCMLString(cmlString);
            IAtomContainer mol = CheckForSingleMoleculeFile(chemFile);

            Assert.AreEqual(3, mol.Atoms.Count);
            IAtom atom1 = mol.Atoms[0];
            IAtom atom2 = mol.Atoms[1];
            IAtom atom3 = mol.Atoms[2];

            Assert.IsNotNull(atom1.Point3D);
            Assert.IsNull(atom2.Point3D);
            Assert.IsNotNull(atom3.Point3D);
        }

        [TestMethod()]
        public void TestCrystal()
        {
            StringBuilder cmlStringB = new StringBuilder("  <molecule id=\"m1\">\n");
            cmlStringB.Append("    <crystal z=\"4\">\n");
            cmlStringB
                    .Append("      <scalar id=\"sc1\" title=\"a\" errorValue=\"0.001\" units=\"units:angstrom\">4.500</scalar>\n");
            cmlStringB
                    .Append("      <scalar id=\"sc2\" title=\"b\" errorValue=\"0.001\" units=\"units:angstrom\">4.500</scalar>\n");
            cmlStringB
                    .Append("      <scalar id=\"sc3\" title=\"c\" errorValue=\"0.001\" units=\"units:angstrom\">4.500</scalar>\n");
            cmlStringB.Append("      <scalar id=\"sc4\" title=\"alpha\" units=\"units:degrees\">90</scalar>\n");
            cmlStringB.Append("      <scalar id=\"sc5\" title=\"beta\" units=\"units:degrees\">90</scalar>\n");
            cmlStringB.Append("      <scalar id=\"sc6\" title=\"gamma\" units=\"units:degrees\">90</scalar>\n");
            cmlStringB.Append("      <symmetry id=\"s1\" spaceGroup=\"Fm3m\"/>\n");
            cmlStringB.Append("    </crystal>\n");
            cmlStringB.Append("    <atomArray>\n");
            cmlStringB.Append("      <atom id=\"a1\" elementType=\"Na\" formalCharge=\"1\" xyzFract=\"0.0 0.0 0.0\"\n");
            cmlStringB.Append("        xy2=\"+23.1 -21.0\"></atom>\n");
            cmlStringB
                    .Append("      <atom id=\"a2\" elementType=\"Cl\" formalCharge=\"-1\" xyzFract=\"0.5 0.0 0.0\"></atom>\n");
            cmlStringB.Append("    </atomArray>\n");
            cmlStringB.Append("  </molecule>\n");

            IChemFile chemFile = ParseCMLString(cmlStringB.ToString());
            ICrystal crystal = CheckForCrystalFile(chemFile);
            Assert.AreEqual(4, crystal.Z.Value);
            Assert.AreEqual("Fm3m", crystal.SpaceGroup);
            Assert.AreEqual(2, crystal.Atoms.Count);
            Vector3 aaxis = crystal.A;
            Assert.AreEqual(4.5, aaxis.X, 0.1);
            Assert.AreEqual(0.0, aaxis.Y, 0.1);
            Assert.AreEqual(0.0, aaxis.Z, 0.1);
            Vector3 baxis = crystal.B;
            Assert.AreEqual(0.0, baxis.X, 0.1);
            Assert.AreEqual(4.5, baxis.Y, 0.1);
            Assert.AreEqual(0.0, baxis.Z, 0.1);
            Vector3 caxis = crystal.C;
            Assert.AreEqual(0.0, caxis.X, 0.1);
            Assert.AreEqual(0.0, caxis.Y, 0.1);
            Assert.AreEqual(4.5, caxis.Z, 0.1);
        }

        [TestMethod()]
        public void TestMoleculeId()
        {
            string cmlString = "<molecule id='m1'><atomArray><atom id='a1'/></atomArray></molecule>";

            IChemFile chemFile = ParseCMLString(cmlString);
            IAtomContainer mol = CheckForSingleMoleculeFile(chemFile);

            Assert.AreEqual("m1", mol.Id);
        }

        private IChemFile ParseCMLString(string cmlString)
        {
            IChemFile chemFile = null;
            CMLReader reader = new CMLReader(new MemoryStream(Encoding.UTF8.GetBytes(cmlString)));
            chemFile = (IChemFile)reader.Read(new ChemFile());
            return chemFile;
        }

        /// <summary>
        /// Tests whether the file is indeed a single molecule file
        /// </summary>
        private IAtomContainer CheckForSingleMoleculeFile(IChemFile chemFile)
        {
            return CheckForXMoleculeFile(chemFile, 1);
        }

        private IAtomContainer CheckForXMoleculeFile(IChemFile chemFile, int numberOfMolecules)
        {
            Assert.IsNotNull(chemFile);

            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);

            Assert.AreEqual(seq.Count, 1);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);

            var moleculeSet = model.MoleculeSet;
            Assert.IsNotNull(moleculeSet);

            Assert.AreEqual(moleculeSet.Count, numberOfMolecules);
            IAtomContainer mol = null;
            for (int i = 0; i < numberOfMolecules; i++)
            {
                mol = moleculeSet[i];
                Assert.IsNotNull(mol);
            }
            return mol;
        }

        private ICrystal CheckForCrystalFile(IChemFile chemFile)
        {
            Assert.IsNotNull(chemFile);

            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);

            Assert.AreEqual(seq.Count, 1);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);

            ICrystal crystal = model.Crystal;
            if (crystal != null) return crystal;

            // null crystal, try and find it in the set
            var set = model.MoleculeSet;
            Assert.IsNotNull(set);
            foreach (var container in set)
            {
                if (container is ICrystal)
                {
                    crystal = (ICrystal)container;
                    return crystal;
                }
            }

            Assert.Fail("no crystal could be found in the ChemModel");
            return crystal;
        }
    }
}
