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
using NCDK.Dict;
using NCDK.QSAR;
using NCDK.QSAR.Results;
using NCDK.Silent;
using NCDK.Stereo;
using System.IO;
using System.Linq;
using System.Text;

namespace NCDK.IO.CML
{
    /// <summary>
    /// Atomic tests for reading CML documents. All tested CML strings are valid CML 2.3,
    /// as can be determined in NCDK.IO.CML.cml23TestFramework.xml.
    /// </summary>
    // @cdk.module test-io
    // @author Egon Willighagen <egonw@sci.kun.nl>
    [TestClass()]
    public class CML23FragmentsTest : CDKTestCase
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
        public void TestMassNumber()
        {
            string cmlString = "<molecule id='m1'><atomArray><atom id='a1' elementType='C' isotopeNumber='12'/></atomArray></molecule>";

            IChemFile chemFile = ParseCMLString(cmlString);
            IAtomContainer mol = CheckForSingleMoleculeFile(chemFile);

            Assert.AreEqual(1, mol.Atoms.Count);
            IAtom atom = mol.Atoms[0];
            Assert.AreEqual("C", atom.Symbol);
            Assert.AreEqual(12, atom.MassNumber.Value);
        }

        [TestMethod()]
        public void TestAtomicNumber()
        {
            string cmlString = "<molecule><atomArray><atom id='a1' elementType=\"C\"><scalar dataType=\"xsd:integer\" dictRef=\"cdk:atomicNumber\">6</scalar></atom></atomArray></molecule>";

            IChemFile chemFile = ParseCMLString(cmlString);
            IAtomContainer mol = CheckForSingleMoleculeFile(chemFile);

            Assert.AreEqual(1, mol.Atoms.Count);
            IAtom atom = mol.Atoms[0];
            Assert.AreEqual("C", atom.Symbol);
            Assert.AreEqual(6, atom.AtomicNumber.Value);
        }

        [TestMethod()]
        public void TestIsotopicMass()
        {
            string cmlString = "<molecule><atomArray><atom id='a1' elementType=\"C\"><scalar dataType=\"xsd:float\" dictRef=\"cdk:isotopicMass\">12.0</scalar></atom></atomArray></molecule>";

            IChemFile chemFile = ParseCMLString(cmlString);
            IAtomContainer mol = CheckForSingleMoleculeFile(chemFile);

            Assert.AreEqual(1, mol.Atoms.Count);
            IAtom atom = mol.Atoms[0];
            Assert.AreEqual("C", atom.Symbol);
            Assert.AreEqual(12.0, atom.ExactMass.Value, 0.01);
        }

        [TestMethod()]
        public void TestAtomParity()
        {
            string cmlString = "<molecule><atomArray><atom id='a1' elementType='C'><atomParity atomRefs4='a2 a3 a5 a4'>1</atomParity></atom>"
                    + "<atom id='a2' elementType='Br'/><atom id='a3' elementType='Cl'/><atom id='a4' elementType='F'/><atom id='a5' elementType='I'/></atomArray>"
                    + "<bondArray><bond atomRefs2='a1 a2' order='1'/><bond atomRefs2='a1 a3' order='1'/><bond atomRefs2='a1 a4' order='1'/><bond atomRefs2='a1 a5' order='1'/></bondArray></molecule>";

            IChemFile chemFile = ParseCMLString(cmlString);
            IAtomContainer mol = CheckForSingleMoleculeFile(chemFile);

            Assert.AreEqual(5, mol.Atoms.Count);
            IAtom atom = mol.Atoms[0];
            Assert.AreEqual("C", atom.Symbol);
            var stereo = mol.StereoElements.First();
            Assert.IsTrue(stereo is TetrahedralChirality);
            Assert.AreEqual(((TetrahedralChirality)stereo).ChiralAtom.Id, "a1");
            var ligandAtoms = ((TetrahedralChirality)stereo).Ligands;
            Assert.AreEqual(4, ligandAtoms.Count);
            Assert.AreEqual(ligandAtoms[0].Id, "a2");
            Assert.AreEqual(ligandAtoms[1].Id, "a3");
            Assert.AreEqual(ligandAtoms[2].Id, "a5");
            Assert.AreEqual(ligandAtoms[3].Id, "a4");
            Assert.AreEqual(((TetrahedralChirality)stereo).Stereo, TetrahedralStereo.Clockwise);
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
        public void TestBondStereo()
        {
            string cmlString = "<molecule id='m1'><atomArray><atom id='a1'/><atom id='a2'/></atomArray><bondArray><bond id='b1' atomRefs2='a1 a2'><bondStereo dictRef='cml:H'/></bond></bondArray></molecule>";
            IChemFile chemFile = ParseCMLString(cmlString);
            IAtomContainer mol = CheckForSingleMoleculeFile(chemFile);

            Assert.AreEqual(2, mol.Atoms.Count);
            Assert.AreEqual(1, mol.Bonds.Count);
            IBond bond = mol.Bonds[0];
            Assert.AreEqual(BondStereo.Down, bond.Stereo);
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
        public void TestBondAromatic2()
        {
            string cmlString = "<molecule id='m1'><atomArray atomID='a1 a2'/><bondArray><bond atomRefs='a1 a2' order='2'><bondType dictRef='cdk:aromaticBond'/></bond></bondArray></molecule>";
            IChemFile chemFile = ParseCMLString(cmlString);
            IAtomContainer mol = CheckForSingleMoleculeFile(chemFile);

            Assert.AreEqual(2, mol.Atoms.Count);
            Assert.AreEqual(1, mol.Bonds.Count);
            IBond bond = mol.Bonds[0];
            Assert.AreEqual(BondOrder.Double, bond.Order);
            Assert.IsTrue(bond.IsAromatic);
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
        public void TestMoleculeId()
        {
            string cmlString = "<molecule id='m1'><atomArray><atom id='a1'/></atomArray></molecule>";

            IChemFile chemFile = ParseCMLString(cmlString);
            IAtomContainer mol = CheckForSingleMoleculeFile(chemFile);

            Assert.AreEqual("m1", mol.Id);
        }

        [TestMethod()]
        public void TestName()
        {
            string cmlString = "<molecule id='m1'><name>acetic acid</name><atomArray atomID='a1 a2 a3'/></molecule>";

            IChemFile chemFile = ParseCMLString(cmlString);
            IAtomContainer mol = CheckForSingleMoleculeFile(chemFile);

            Assert.AreEqual("acetic acid", mol.Title);
        }

        // @cdk.bug 2142400
        [TestMethod()]
        public void TestHydrogenCount1()
        {
            string cmlString = "<molecule><atomArray><atom id='a1' elementType='C' hydrogenCount='4'/></atomArray></molecule>";

            IChemFile chemFile = ParseCMLString(cmlString);
            IAtomContainer mol = CheckForSingleMoleculeFile(chemFile);

            Assert.AreEqual(1, mol.Atoms.Count);
            IAtom atom = mol.Atoms[0];
            Assert.IsNotNull(atom);
            Assert.IsNotNull(atom.ImplicitHydrogenCount);
            Assert.AreEqual(4, atom.ImplicitHydrogenCount.Value);
        }

        // @cdk.bug 2142400
        [TestMethod()]
        public void TestHydrogenCount2()
        {
            string cmlString = "<molecule><atomArray>" + "<atom id='a1' elementType='C' hydrogenCount='4'/>"
                    + "<atom id='a2' elementType='H'/>" + "<atom id='a3' elementType='H'/>"
                    + "<atom id='a4' elementType='H'/>" + "<atom id='a5' elementType='H'/>" + "</atomArray>"
                    + "<bondArray>" + "<bond id='b1' atomRefs2='a1 a2' order='S'/>"
                    + "<bond id='b2' atomRefs2='a1 a3' order='S'/>" + "<bond id='b3' atomRefs2='a1 a4' order='S'/>"
                    + "<bond id='b4' atomRefs2='a1 a5' order='S'/>" + "</bondArray></molecule>";

            IChemFile chemFile = ParseCMLString(cmlString);
            IAtomContainer mol = CheckForSingleMoleculeFile(chemFile);

            Assert.AreEqual(5, mol.Atoms.Count);
            IAtom atom = mol.Atoms[0];
            Assert.IsNotNull(atom);
            Assert.AreEqual("C", atom.Symbol);
            Assert.IsNotNull(atom.ImplicitHydrogenCount);
            Assert.AreEqual(0, atom.ImplicitHydrogenCount.Value);
        }

        // @cdk.bug 2142400
        [TestMethod()]
        public void TestHydrogenCount3()
        {
            string cmlString = "<molecule>" + "<atomArray>" + "<atom id='a1' elementType='C' hydrogenCount='4'/>"
                    + "<atom id='a2' elementType='H'/>" + "</atomArray>" + "<bondArray>"
                    + "<bond id='b1' atomRefs2='a1 a2' order='S'/>" + "</bondArray>" + "</molecule>";

            IChemFile chemFile = ParseCMLString(cmlString);
            IAtomContainer mol = CheckForSingleMoleculeFile(chemFile);

            Assert.AreEqual(2, mol.Atoms.Count);
            IAtom atom = mol.Atoms[0];
            Assert.IsNotNull(atom);
            Assert.IsNotNull(atom.ImplicitHydrogenCount);
            Assert.AreEqual(3, atom.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestInChI()
        {
            string cmlString = "<molecule id='m1'><identifier convention='iupac:inchi' value='InChI=1/CH2O2/c2-1-3/h1H,(H,2,3)'/><atomArray atomID='a1 a2 a3'/></molecule>";

            IChemFile chemFile = ParseCMLString(cmlString);
            IAtomContainer mol = CheckForSingleMoleculeFile(chemFile);

            Assert.AreEqual("InChI=1/CH2O2/c2-1-3/h1H,(H,2,3)", mol.GetProperty<string>(CDKPropertyName.InChI));
        }

        [TestMethod()]
        public void TestDictRef()
        {
            string cmlString = "<molecule id=\"alanine\" dictRef=\"pdb:aminoAcid\"><name>alanine</name><name dictRef=\"pdb:residueName\">Ala</name><name dictRef=\"pdb:oneLetterCode\">A</name><scalar dictRef=\"pdb:id\">3</scalar><atomArray><atom id=\"a1\" elementType=\"C\" x2=\"265.0\" y2=\"989.0\"/><atom id=\"a2\" elementType=\"N\" x2=\"234.0\" y2=\"972.0\" dictRef=\"pdb:nTerminus\"/><atom id=\"a3\" elementType=\"C\" x2=\"265.0\" y2=\"1025.0\"/><atom id=\"a4\" elementType=\"C\" x2=\"296.0\" y2=\"971.0\" dictRef=\"pdb:cTerminus\"/><atom id=\"a5\" elementType=\"O\" x2=\"296.0\" y2=\"935.0\"/><atom id=\"a6\" elementType=\"O\" x2=\"327.0\" y2=\"988.0\"/></atomArray><bondArray><bond id=\"b1\" atomRefs2=\"a2 a1\" order=\"S\"/><bond id=\"b2\" atomRefs2=\"a1 a3\" order=\"S\"/><bond id=\"b3\" atomRefs2=\"a1 a4\" order=\"S\"/><bond id=\"b4\" atomRefs2=\"a4 a5\" order=\"D\"/><bond id=\"b5\" atomRefs2=\"a4 a6\" order=\"S\"/></bondArray></molecule>";

            IChemFile chemFile = ParseCMLString(cmlString);
            IAtomContainer mol = CheckForSingleMoleculeFile(chemFile);

            bool foundDictRefs = false;
            foreach (var next in mol.GetProperties().Keys)
            {
                if (next is DictRef) foundDictRefs = true;
            }
            Assert.IsTrue(foundDictRefs);
        }

        [TestMethod()]
        public void TestQSAROutput()
        {
            string specificationReference = "qsar:weight";
            string implementationTitle = "NCDK.QSAR.Descriptors.Moleculars.WeightDescriptor";
            string implementationIdentifier = "$Id$";
            string implementationVendor = "The Chemistry Development Kit";

            string cmlString = "<molecule xmlns=\"http://www.xml-cml.org/schema\"><atomArray><atom id=\"a5256233\" "
                    + "elementType=\"C\" formalCharge=\"0\" hydrogenCount=\"0\" /><atom id=\"a26250401\" elementType=\"C\" "
                    + "formalCharge=\"0\" hydrogenCount=\"0\" /><atom id=\"a16821027\" elementType=\"C\" formalCharge=\"0\" "
                    + "hydrogenCount=\"0\" /><atom id=\"a14923925\" elementType=\"C\" formalCharge=\"0\" hydrogenCount=\"0\" />"
                    + "<atom id=\"a7043360\" elementType=\"C\" formalCharge=\"0\" hydrogenCount=\"0\" /><atom id=\"a31278839\" "
                    + "elementType=\"C\" formalCharge=\"0\" hydrogenCount=\"0\" /></atomArray><bondArray><bond id=\"b6175092\" "
                    + "atomRefs2=\"a5256233 a26250401\" order=\"S\" /><bond id=\"b914691\" atomRefs2=\"a26250401 a16821027\" "
                    + "order=\"D\" /><bond id=\"b5298332\" atomRefs2=\"a16821027 a14923925\" order=\"S\" /><bond id=\"b29167060\" "
                    + "atomRefs2=\"a14923925 a7043360\" order=\"D\" /><bond id=\"b14093690\" atomRefs2=\"a7043360 a31278839\" "
                    + "order=\"S\" /><bond id=\"b11924794\" atomRefs2=\"a31278839 a5256233\" order=\"D\" /></bondArray>"
                    + "<propertyList><property xmlns:qsar=\"http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/\" "
                    + "convention=\"qsar:DescriptorValue\"><metadataList><metadata dictRef=\"qsar:specificationReference\" "
                    + "content=\""
                    + specificationReference
                    + "\" /><metadata dictRef=\"qsar:implementationTitle\" content=\""
                    + implementationTitle
                    + "\" /><metadata dictRef=\"qsar:implementationIdentifier\" "
                    + "content=\""
                    + implementationIdentifier
                    + "\" /><metadata dictRef=\""
                    + "qsar:implementationVendor\" content=\""
                    + implementationVendor
                    + "\" /><metadataList title=\"qsar:"
                    + "descriptorParameters\"><metadata title=\"elementSymbol\" content=\"*\" /></metadataList></metadataList>"
                    + "<scalar dataType=\"xsd:double\" dictRef=\"qsar:weight\">72.0</scalar></property></propertyList></molecule>";

            IChemFile chemFile = ParseCMLString(cmlString);
            IAtomContainer mol = CheckForSingleMoleculeFile(chemFile);

            Assert.IsNotNull(mol);
            Assert.AreEqual(1, mol.GetProperties().Count);
            object key = mol.GetProperties().Keys.ToArray()[0];
            Assert.IsNotNull(key);
            Assert.IsTrue(key is DescriptorSpecification);
            DescriptorSpecification spec = (DescriptorSpecification)key;
            Assert.AreEqual(specificationReference, spec.SpecificationReference);
            Assert.AreEqual(implementationTitle, spec.ImplementationTitle);
            Assert.AreEqual(implementationIdentifier, spec.ImplementationIdentifier);
            Assert.AreEqual(implementationVendor, spec.ImplementationVendor);

            Assert.IsNotNull(mol.GetProperty<object>(key));
            Assert.IsTrue(mol.GetProperty<IDescriptorValue>(key) is IDescriptorValue);
            var value = mol.GetProperty<IDescriptorValue>(key);
            IDescriptorResult result = value.Value;
            Assert.IsNotNull(result);
            Assert.IsTrue(result is Result<double>);
            Assert.AreEqual(72.0, ((Result<double>)result).Value, 0.001);
        }

        private IChemFile ParseCMLString(string cmlString)
        {
            IChemFile chemFile = null;
            CMLReader reader = new CMLReader(new MemoryStream(Encoding.UTF8.GetBytes(cmlString)));
            chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();
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

        //    private ICrystal CheckForCrystalFile(IChemFile chemFile) {
        //        Assert.IsNotNull(chemFile);
        //
        //        Assert.AreEqual(chemFile.Count, 1);
        //        IChemSequence seq = chemFile[0];
        //        Assert.IsNotNull(seq);
        //
        //        Assert.AreEqual(seq.Count, 1);
        //        IChemModel model = seq[0];
        //        Assert.IsNotNull(model);
        //
        //        ICrystal crystal = model.Crystal;
        //        Assert.IsNotNull(crystal);
        //
        //        return crystal;
        //    }

        [TestMethod()]
        public void TestReaction()
        {
            string cmlString = "<reaction>" + "<reactantList><reactant><molecule id='react'/></reactant></reactantList>"
                    + "<productList><product><molecule id='product'/></product></productList>"
                    + "<substanceList><substance><molecule id='water'/></substance></substanceList>" + "</reaction>";

            IChemFile chemFile = ParseCMLString(cmlString);
            IReaction reaction = CheckForSingleReactionFile(chemFile);

            Assert.AreEqual(1, reaction.Reactants.Count);
            Assert.AreEqual(1, reaction.Products.Count);
            Assert.AreEqual(1, reaction.Agents.Count);
            Assert.AreEqual("react", reaction.Reactants[0].Id);
            Assert.AreEqual("product", reaction.Products[0].Id);
            Assert.AreEqual("water", reaction.Agents[0].Id);
        }

        /// <summary>
        /// Tests whether the file is indeed a single reaction file
        /// </summary>
        private IReaction CheckForSingleReactionFile(IChemFile chemFile)
        {
            return CheckForXReactionFile(chemFile, 1);
        }

        private IReaction CheckForXReactionFile(IChemFile chemFile, int numberOfReactions)
        {
            Assert.IsNotNull(chemFile);

            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);

            Assert.AreEqual(seq.Count, 1);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);

            IReactionSet reactionSet = model.ReactionSet;
            Assert.IsNotNull(reactionSet);

            Assert.AreEqual(reactionSet.Count, numberOfReactions);
            IReaction reaction = null;
            for (int i = 0; i < numberOfReactions; i++)
            {
                reaction = reactionSet[i];
                Assert.IsNotNull(reaction);
            }
            return reaction;
        }
    }
}
