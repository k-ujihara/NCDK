/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
 *                    2010  Stefan Kuhn <Stefan.Kuhn@ebi.ac.uk>
 *
 * Contact: cdk-devel@slists.sourceforge.net
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
using NCDK.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.IO.Listener;
using NCDK.SGroups;
using NCDK.Templates;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the writer MDL mol files using one test file.
    ///
    // @cdk.module test-io
    ///
    // @see org.openscience.cdk.io.MDLV2000Writer
    /// </summary>
    [TestClass()]
    public class MDLV2000WriterTest : ChemObjectIOTest
    {
        protected override IChemObjectIO ChemObjectIOToTest { get; } = new MDLV2000Writer();
        private static IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;

        [TestMethod()]
        public void TestAccepts()
        {
            MDLV2000Writer reader = new MDLV2000Writer();
            Assert.IsTrue(reader.Accepts(typeof(ChemFile)));
            Assert.IsTrue(reader.Accepts(typeof(ChemModel)));
            Assert.IsTrue(reader.Accepts(typeof(AtomContainer)));
        }

        /// <summary>
        // @cdk.bug 890456
        // @cdk.bug 1524466
        /// </summary>
        [TestMethod()]
        public void TestBug890456()
        {
            StringWriter writer = new StringWriter();
            IAtomContainer molecule = new AtomContainer();
            molecule.Atoms.Add(new PseudoAtom("*"));
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms.Add(new Atom("C"));

            MDLV2000Writer mdlWriter = new MDLV2000Writer(writer);
            mdlWriter.Write(molecule);
            mdlWriter.Close();
            Assert.IsTrue(writer.ToString().IndexOf("M  END") != -1);
        }

        /// <summary>
        // @cdk.bug 1212219
        /// </summary>
        [TestMethod()]
        public void TestBug1212219()
        {
            StringWriter writer = new StringWriter();
            IAtomContainer molecule = new AtomContainer();
            Atom atom = new Atom("C");
            atom.MassNumber = 14;
            molecule.Atoms.Add(atom);

            MDLV2000Writer mdlWriter = new MDLV2000Writer(writer);
            mdlWriter.Write(molecule);
            mdlWriter.Close();
            string output = writer.ToString();
            //Debug.WriteLine("MDL output for testBug1212219: " + output);
            Assert.IsTrue(output.IndexOf("M  ISO  1   1  14") != -1);
        }

        [TestMethod()]
        public void TestWriteValence()
        {
            StringWriter writer = new StringWriter();
            IAtomContainer molecule = TestMoleculeFactory.MakeAlphaPinene();
            molecule.Atoms[0].Valency = 1;
            molecule.Atoms[1].Valency = 0;
            MDLV2000Writer mdlWriter = new MDLV2000Writer(writer);
            var customSettings = new NameValueCollection();
            customSettings["WriteQueryFormatValencies"] = "true";
            mdlWriter.Listeners.Add(new PropertiesListener(customSettings));
            mdlWriter.Write(molecule);
            mdlWriter.Close();
            string output = writer.ToString();
            Assert.IsTrue(output.IndexOf("0  0  0  0  0  1  0  0  0  0  0  0") != -1);
            Assert.IsTrue(output.IndexOf("0  0  0  0  0 15  0  0  0  0  0  0") != -1);
        }

        [TestMethod()]
        public void NonDefaultValence_fe_iii()
        {
            IAtomContainer container = new AtomContainer();
            IAtom fe1 = new Atom("Fe");
            fe1.ImplicitHydrogenCount = 3;
            container.Atoms.Add(fe1);
            StringWriter writer = new StringWriter();
            MDLV2000Writer mdlWriter = new MDLV2000Writer(writer);
            mdlWriter.Write(container);
            mdlWriter.Close();
            string output = writer.ToString();
            Assert.IsTrue(output.Contains("Fe  0  0  0  0  0  3  0  0  0  0  0  0"));
        }

        [TestMethod()]
        public void TestWriteAtomAtomMapping()
        {
            StringWriter writer = new StringWriter();
            IAtomContainer molecule = TestMoleculeFactory.MakeAlphaPinene();
            molecule.Atoms[0].SetProperty(CDKPropertyName.AtomAtomMapping, 1);
            molecule.Atoms[1].SetProperty(CDKPropertyName.AtomAtomMapping, 15);
            MDLV2000Writer mdlWriter = new MDLV2000Writer(writer);
            mdlWriter.Write(molecule);
            mdlWriter.Close();
            string output = writer.ToString();
            Assert.IsTrue(output.IndexOf("0  0  0  0  0  0  0  0  0  1  0  0") != -1);
            Assert.IsTrue(output.IndexOf("0  0  0  0  0  0  0  0  0 15  0  0") != -1);
        }

        /// <summary>
        /// Tests if string atom atom mappings are parsed correctly
        /// </summary>
        [TestMethod()]
        public void TestWriteStringAtomAtomMapping()
        {
            StringWriter writer = new StringWriter();
            IAtomContainer molecule = TestMoleculeFactory.MakeAlphaPinene();
            molecule.Atoms[0].SetProperty(CDKPropertyName.AtomAtomMapping, "1");
            molecule.Atoms[1].SetProperty(CDKPropertyName.AtomAtomMapping, "15");
            MDLV2000Writer mdlWriter = new MDLV2000Writer(writer);
            mdlWriter.Write(molecule);
            mdlWriter.Close();
            string output = writer.ToString();
            Assert.IsTrue(output.Contains("0  0  0  0  0  0  0  0  0  1  0  0"));
            Assert.IsTrue(output.Contains("0  0  0  0  0  0  0  0  0 15  0  0"));
        }

        /// <summary>
        /// Tests if non-valid atom atom mappings are ignored by the reader.
        /// </summary>
        [TestMethod()]
        public void TestWriteInvalidAtomAtomMapping()
        {
            StringWriter writer = new StringWriter();
            IAtomContainer molecule = TestMoleculeFactory.MakeAlphaPinene();
            molecule.Atoms[0].SetProperty(CDKPropertyName.AtomAtomMapping, "1a");
            molecule.Atoms[1].SetProperty(CDKPropertyName.AtomAtomMapping, "15");
            MDLV2000Writer mdlWriter = new MDLV2000Writer(writer);
            mdlWriter.Write(molecule);
            mdlWriter.Close();
            string output = writer.ToString();
            Regex p = new Regex(".*V2000.*    0.0000    0.0000    0.0000 C   0  0  0  0  0  0  0  0  0  0  "
                + "0  0.*    0.0000    0.0000    0.0000 C   0  0  0  0  0  0  0  0  0 15  0  0.*",
                RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);
            var m = p.Match(output);
            Assert.IsTrue(m.Success);
        }

        /// <summary>
        /// Test for bug #1778479 "MDLWriter writes empty PseudoAtom label string".
        /// When a molecule contains an IPseudoAtom without specifying the atom label
        /// the MDLWriter generates invalid output as it prints the zero-length atom
        /// label.
        /// This was fixed with letting PseudoAtom have a default label of '*'.
        ///
        /// Author: Andreas Schueller <a.schueller@chemie.uni-frankfurt.de>
        ///
        // @cdk.bug 1778479
        /// </summary>
        [TestMethod()]
        public void TestBug1778479()
        {
            StringWriter writer = new StringWriter();
            IAtomContainer molecule = builder.CreateAtomContainer();
            IAtom atom1 = builder.CreatePseudoAtom();
            IAtom atom2 = builder.CreateAtom("C");
            IBond bond = builder.CreateBond(atom1, atom2);
            molecule.Atoms.Add(atom1);
            molecule.Atoms.Add(atom2);
            molecule.Bonds.Add(bond);

            MDLV2000Writer mdlWriter = new MDLV2000Writer(writer);
            mdlWriter.Write(molecule);
            mdlWriter.Close();
            string output = writer.ToString();
            Assert.AreEqual(-1,
                                output.IndexOf("0.0000    0.0000    0.0000     0  0  0  0  0  0  0  0  0  0  0  0"),
                                "Test for zero length pseudo atom label in MDL file");
        }

        [TestMethod()]
        public void TestNullFormalCharge()
        {
            StringWriter writer = new StringWriter();
            IAtomContainer molecule = builder.CreateAtomContainer();
            IAtom atom = builder.CreateAtom("C");
            atom.FormalCharge = null;
            molecule.Atoms.Add(atom);

            MDLV2000Writer mdlWriter = new MDLV2000Writer(writer);
            mdlWriter.Write(molecule);
            mdlWriter.Close();
            string output = writer.ToString();
            // test ensures that the writer does not throw an exception on
            // null formal charges, so a mere assert on output being non-zero
            // length is enough
            Assert.IsNotNull(output);
            Assert.AreNotSame(0, output.Length);
        }

        [TestMethod()]
        public void TestPrefer3DCoordinateOutput()
        {
            StringWriter writer = new StringWriter();
            IAtomContainer molecule = builder.CreateAtomContainer();
            IAtom atom = builder.CreateAtom("C");
            atom.Point2D = new Vector2(1.0, 2.0);
            atom.Point3D = new Vector3(3.0, 4.0, 5.0);
            molecule.Atoms.Add(atom);

            MDLV2000Writer mdlWriter = new MDLV2000Writer(writer);
            mdlWriter.Write(molecule);
            mdlWriter.Close();
            string output = writer.ToString();
            // the current behavior is that if both 2D and 3D coordinates
            // are available, the 3D is outputed, and the 2D not
            Assert.IsTrue(output.Contains("3.0"));
            Assert.IsTrue(output.Contains("4.0"));
            Assert.IsTrue(output.Contains("5.0"));
        }

        [TestMethod()]
        public void TestForce2DCoordinates()
        {
            StringWriter writer = new StringWriter();
            IAtomContainer molecule = builder.CreateAtomContainer();
            IAtom atom = builder.CreateAtom("C");
            atom.Point2D = new Vector2(1.0, 2.0);
            atom.Point3D = new Vector3(3.0, 4.0, 5.0);
            molecule.Atoms.Add(atom);

            MDLV2000Writer mdlWriter = new MDLV2000Writer(writer);
            var prop = new NameValueCollection();
            prop["ForceWriteAs2DCoordinates"] = "true";
            PropertiesListener listener = new PropertiesListener(prop);
            mdlWriter.Listeners.Add(listener);
            mdlWriter.CustomizeJob();
            mdlWriter.Write(molecule);
            mdlWriter.Close();
            string output = writer.ToString();
            // the current behavior is that if both 2D and 3D coordinates
            // are available, the 3D is outputed, and the 2D not
            Assert.IsTrue(output.Contains("1.0"));
            Assert.IsTrue(output.Contains("2.0"));
        }

        [TestMethod()]
        public void TestUndefinedStereo()
        {
            IAtomContainer mol = TestMoleculeFactory.MakeAlphaPinene();
            mol.Bonds[0].Stereo = BondStereo.UpOrDown;
            mol.Bonds[1].Stereo = BondStereo.EOrZ;
            StringWriter writer = new StringWriter();
            MDLV2000Writer mdlWriter = new MDLV2000Writer(writer);
            mdlWriter.Write(mol);
            mdlWriter.Close();
            string output = writer.ToString();
            Assert.IsTrue(output.IndexOf("1  2  2  4  0  0  0") > -1);
            Assert.IsTrue(output.IndexOf("2  3  1  3  0  0  0") > -1);
        }

        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        public void TestUnsupportedBondOrder()
        {
            IAtomContainer molecule = new AtomContainer();
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms.Add(new Atom("C"));
            molecule.Bonds.Add(new Bond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Quadruple));
            MDLV2000Writer mdlWriter = new MDLV2000Writer(new StringWriter());
            mdlWriter.Write(molecule);
            mdlWriter.Close();
        }

        [TestMethod()]
        public void TestTwoFragmentsWithTitle()
        {
            IAtomContainer mol1 = TestMoleculeFactory.MakeAlphaPinene();
            mol1.SetProperty(CDKPropertyName.Title, "title1");
            IAtomContainer mol2 = TestMoleculeFactory.MakeAlphaPinene();
            mol2.SetProperty(CDKPropertyName.Title, "title2");
            IChemModel model = mol1.Builder.CreateChemModel();
            model.MoleculeSet = mol1.Builder.CreateAtomContainerSet();
            model.MoleculeSet.Add(mol1);
            model.MoleculeSet.Add(mol2);
            StringWriter writer = new StringWriter();
            MDLV2000Writer mdlWriter = new MDLV2000Writer(writer);
            mdlWriter.Write(model);
            mdlWriter.Close();
            string output = writer.ToString();
            Assert.IsTrue(output.Contains("title1; title2"));
        }

        /// <summary>
        /// Test correct output of R-groups, using the hash (#) and a separate RGP line.
        /// </summary>
        [TestMethod()]
        public void TestRGPLine()
        {
            StringWriter writer = new StringWriter();
            IAtomContainer molecule = builder.CreateAtomContainer();
            IPseudoAtom atom1 = builder.CreatePseudoAtom();
            atom1.Symbol = "R";
            atom1.Label = "R12";

            IAtom atom2 = builder.CreateAtom("C");
            IBond bond = builder.CreateBond(atom1, atom2);

            IPseudoAtom atom3 = builder.CreatePseudoAtom();
            atom3.Symbol = "A";
            atom3.Label = "A";
            IBond bond2 = builder.CreateBond(atom3, atom2);

            molecule.Atoms.Add(atom1);
            molecule.Atoms.Add(atom2);
            molecule.Atoms.Add(atom3);
            molecule.Bonds.Add(bond);
            molecule.Bonds.Add(bond2);

            MDLV2000Writer mdlWriter = new MDLV2000Writer(writer);
            mdlWriter.Write(molecule);
            mdlWriter.Close();
            string output = writer.ToString();

            Assert.IsTrue(-1 != output.IndexOf("R#"), "Test for R#");
            Assert.IsTrue(-1 != output.IndexOf("M  RGP  1   1  12"), "Test for RGP line");
        }

        /// <summary>
        /// Test writing of comments made on individual atoms into an Atom Value lines.
        /// </summary>
        [TestMethod()]
        public void TestAtomValueLine()
        {
            IAtom carbon = builder.CreateAtom("C");
            carbon.SetProperty(CDKPropertyName.Comment, "Carbon comment");
            IAtom oxygen = builder.CreateAtom("O");
            oxygen.SetProperty(CDKPropertyName.Comment, "Oxygen comment");
            IBond bond = builder.CreateBond(carbon, oxygen, BondOrder.Double);

            IAtomContainer molecule = new AtomContainer();
            molecule.Atoms.Add(oxygen);
            molecule.Atoms.Add(carbon);
            molecule.Bonds.Add(bond);

            StringWriter writer = new StringWriter();
            MDLV2000Writer mdlWriter = new MDLV2000Writer(writer);
            mdlWriter.Write(molecule);
            mdlWriter.Close();

            Assert.IsTrue(writer.ToString().IndexOf("V    1 Oxygen comment") != -1);
            Assert.IsTrue(writer.ToString().IndexOf("V    2 Carbon comment") != -1);

        }

        /// <summary>
        /// Test option to write aromatic bonds with bond type "4".
        /// Please note: bond type values 4 through 8 are for SSS queries only.
        // @
        /// </summary>
        [TestMethod()]
        public void TestAromaticBondType4()
        {

            IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();
            foreach (var atom in benzene.Atoms)
            {
                atom.IsAromatic = true;
            }
            foreach (var bond in benzene.Bonds)
            {
                bond.IsAromatic = true;
            }

            StringWriter writer = new StringWriter();
            MDLV2000Writer mdlWriter = new MDLV2000Writer(writer);
            mdlWriter.Write(benzene);
            mdlWriter.Close();
            Assert.IsTrue(writer.ToString().IndexOf("1  2  1  0  0  0  0") != -1);

            writer = new StringWriter();
            mdlWriter = new MDLV2000Writer(writer);
            var prop = new NameValueCollection();
            prop["WriteAromaticBondTypes"] = "true";
            PropertiesListener listener = new PropertiesListener(prop);
            mdlWriter.Listeners.Add(listener);
            mdlWriter.CustomizeJob();
            mdlWriter.Write(benzene);
            mdlWriter.Close();
            Assert.IsTrue(writer.ToString().IndexOf("1  2  4  0  0  0  0") != -1);
        }

        [TestMethod()]
        public void TestAtomParity()
        {
            Stream ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.mol_testAtomParity.mol");
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer molecule = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            molecule = reader.Read(molecule);
            reader.Close();

            StringWriter sw = new StringWriter();
            MDLV2000Writer writer = new MDLV2000Writer(sw);
            writer.Write(molecule);
            writer.Close();

            Assert.IsTrue(sw.ToString().Contains(
                    "   -1.1749    0.1436    0.0000 C   0  0  1  0  0  0  0  0  0  0  0  0"));

        }

        [TestMethod()]
        public void TestWritePseudoAtoms()
        {
            Stream ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.pseudoatoms.sdf");
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer molecule = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            molecule = reader.Read(molecule);
            reader.Close();

            StringWriter writer = new StringWriter();
            MDLV2000Writer mwriter = new MDLV2000Writer(writer);
            mwriter.Write(molecule);
            mwriter.Close();

            string output = writer.ToString();
            Assert.IsTrue(output.IndexOf("Gln") != -1);
            Assert.IsTrue(output.IndexOf("Leu") != -1);
        }

        /// <summary>
        // @cdk.bug 1263
        // @
        /// </summary>
        [TestMethod()]
        public void TestWritePseudoAtoms_LongLabel()
        {

            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer container = builder.CreateAtomContainer();

            IAtom c1 = builder.CreateAtom("C");
            IAtom tRNA = builder.CreatePseudoAtom("tRNA");

            container.Atoms.Add(c1);
            container.Atoms.Add(tRNA);

            StringWriter sw = new StringWriter();
            MDLV2000Writer writer = new MDLV2000Writer(sw);
            writer.Write(container);
            writer.Close();

            string output = sw.ToString();

            Assert.IsTrue(output.Contains("A    2"));
            Assert.IsTrue(output.Contains("tRNA"));

        }

        /// <summary>
        /// Checks that null atom labels are handled correctly.
        /// </summary>
        [TestMethod()]
        public void TestWritePseudoAtoms_nullLabel()
        {

            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer container = builder.CreateAtomContainer();

            IAtom c1 = builder.CreateAtom("C");
            IPseudoAtom nullAtom = builder.CreatePseudoAtom("");
            nullAtom.Label = null;

            container.Atoms.Add(c1);
            container.Atoms.Add(nullAtom);

            StringWriter sw = new StringWriter();
            MDLV2000Writer writer = new MDLV2000Writer(sw);
            writer.Write(container);
            writer.Close();

            string output = sw.ToString();
            Assert.IsTrue(output.Contains("R"));

        }

        /// <summary>
        /// When there are more then 16 R Groups these should be wrapped
        // @
        /// </summary>
        [TestMethod()]
        public void TestRGPLine_Multiline()
        {

            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer container = builder.CreateAtomContainer();

            for (int i = 1; i < 20; i++)
                container.Atoms.Add(builder.CreatePseudoAtom("R" + i));

            StringWriter sw = new StringWriter();
            MDLV2000Writer writer = new MDLV2000Writer(sw);
            writer.Write(container);
            writer.Close();

            string output = sw.ToString();
            Assert.IsTrue(output.Contains("M  RGP  8   1   1   2   2   3   3   4   4   5   5   6   6   7   7   8   8"));
            Assert.IsTrue(output.Contains("M  RGP  8   9   9  10  10  11  11  12  12  13  13  14  14  15  15  16  16"));
            Assert.IsTrue(output.Contains("M  RGP  3  17  17  18  18  19  19"));

        }

        [TestMethod()]
        public void TestAlias_TruncatedLabel()
        {

            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer container = builder.CreateAtomContainer();

            string label = "This is a very long label - almost too long. it should be cut here -> and the rest is truncated";

            container.Atoms.Add(builder.CreatePseudoAtom(label));

            StringWriter sw = new StringWriter();
            MDLV2000Writer writer = new MDLV2000Writer(sw);
            writer.Write(container);
            writer.Close();

            string output = sw.ToString();

            Assert.IsTrue(output.Contains("This is a very long label - almost too long. it should be cut here ->"));
            // make sure the full label wasn't output
            Assert.IsFalse(output.Contains(label));
        }

        private static string[] SplitLines(string str)
        {
            var list = new List<string>();
            using (var reader = new StringReader(str))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    list.Add(line);
            }
            return list.ToArray();
        }

        [TestMethod()]
        public void TestSingleSingletRadical()
        {

            Stream ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.singleSingletRadical.mol");
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer molecule = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            molecule = reader.Read(molecule);
            reader.Close();

            StringWriter sw = new StringWriter();
            MDLV2000Writer writer = new MDLV2000Writer(sw);
            writer.Write(molecule);
            writer.Close();

            string[] lines = SplitLines(sw.ToString());

            Assert.AreEqual(9, lines.Length, "incorrect file length");
            Assert.AreEqual("M  RAD  1   2   1", lines[7], "incorrect radical output");
        }

        [TestMethod()]
        public void TestSingleDoubletRadical()
        {

            Stream ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.singleDoubletRadical.mol");
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer molecule = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            molecule = reader.Read(molecule);
            reader.Close();

            StringWriter sw = new StringWriter();
            MDLV2000Writer writer = new MDLV2000Writer(sw);
            writer.Write(molecule);
            writer.Close();

            string[] lines = SplitLines(sw.ToString());

            Assert.AreEqual(9, lines.Length, "incorrect file length");
            Assert.AreEqual("M  RAD  1   2   2", lines[7], "incorrect radical output");

        }

        // XXX: information loss, CDK does not distinquish between divalence
        //      singlet and triplet and only stores the unpaired electrons
        [TestMethod()]
        public void TestSingleTripletRadical()
        {

            Stream ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.singleTripletRadical.mol");
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer molecule = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            molecule = reader.Read(molecule);
            reader.Close();

            StringWriter sw = new StringWriter();
            MDLV2000Writer writer = new MDLV2000Writer(sw);
            writer.Write(molecule);
            writer.Close();

            string[] lines = SplitLines(sw.ToString());

            Assert.AreEqual(9, lines.Length, "incorrect file length");
            Assert.AreEqual("M  RAD  1   2   1", lines[7], "incorrect radical output");
        }

        [TestMethod()]
        public void TestMultipleRadicals()
        {

            Stream ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.multipleRadicals.mol");
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer molecule = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            molecule = reader.Read(molecule);
            reader.Close();

            StringWriter sw = new StringWriter();
            MDLV2000Writer writer = new MDLV2000Writer(sw);
            writer.Write(molecule);
            writer.Close();

            string[] lines = SplitLines(sw.ToString());

            Assert.AreEqual(24, lines.Length, "incorrect file length");
            Assert.AreEqual("M  RAD  8   1   2   2   2   3   2   4   2   5   2   6   2   7   2   8   2",
                lines[21], "incorrect radical output on line 22");
            Assert.AreEqual("M  RAD  1   9   2", lines[22], "incorrect radical output on line 23");

        }

        [TestMethod()]
        public void TestSgroupAtomListWrapping()
        {
            IAtomContainer mol = TestMoleculeFactory.MakeEthylPropylPhenantren();

            Sgroup sgroup = new Sgroup();
            foreach (var atom in mol.Atoms)
                sgroup.Atoms.Add(atom);
            mol.SetProperty(CDKPropertyName.CtabSgroups,
                         new[] { sgroup });

            StringWriter sw = new StringWriter();
            using (MDLV2000Writer mdlw = new MDLV2000Writer(sw))
            {
                mdlw.Write(mol);
                string output = sw.ToString();
                Assert.IsTrue(output.Contains("M  SAL   1 15"));
                Assert.IsTrue(output.Contains("M  SAL   1  4"));
            }
        }

        [TestMethod()]
        public void SgroupRepeatUnitRoundTrip()
        {
            StringWriter sw = new StringWriter();
            using (MDLV2000Reader mdlr = new MDLV2000Reader(ResourceLoader.GetAsStream("NCDK.Data.MDL.sgroup-sru.mol")))
            using (MDLV2000Writer mdlw = new MDLV2000Writer(sw))
            {
                mdlw.Write(mdlr.Read(new AtomContainer()));
                string output = sw.ToString();
                Assert.IsTrue(output.Contains("M  STY  1   1 SRU"));
                Assert.IsTrue(output.Contains("M  SMT   1 n"));
                Assert.IsTrue(output.Contains("M  SCN  1   1 HT"));
            }
        }

        [TestMethod()]
        public void SgroupBracketStylesRoundTrip()
        {
            StringWriter sw = new StringWriter();
            using (MDLV2000Reader mdlr = new MDLV2000Reader(ResourceLoader.GetAsStream("NCDK.Data.MDL.sgroup-sru-bracketstyles.mol")))
            using (MDLV2000Writer mdlw = new MDLV2000Writer(sw))
            {
                mdlw.Write(mdlr.Read(new AtomContainer()));
                string output = sw.ToString();
                Assert.IsTrue(output.Contains("M  STY  2   1 SRU   2 SRU"));
                Assert.IsTrue(output.Contains("M  SBT  1   1   1"));
            }
        }

        [TestMethod()]
        public void SgroupUnorderedMixtureRoundTrip()
        {
            StringWriter sw = new StringWriter();
            using (MDLV2000Reader mdlr = new MDLV2000Reader(ResourceLoader.GetAsStream("NCDK.Data.MDL.sgroup-unord-mixture.mol")))
            using (MDLV2000Writer mdlw = new MDLV2000Writer(sw))
            {
                mdlw.Write(mdlr.Read(new AtomContainer()));
                string output = sw.ToString();
                Assert.IsTrue(output.Contains("M  STY  3   1 COM   2 COM   3 MIX"));
                Assert.IsTrue(output.Contains("M  SPL  1   1   3"));
            }
        }

        [TestMethod()]
        public void SgroupCopolymerRoundTrip()
        {
            StringWriter sw = new StringWriter();
            using (MDLV2000Reader mdlr = new MDLV2000Reader(ResourceLoader.GetAsStream("NCDK.Data.MDL.sgroup-ran-copolymer.mol")))
            using (MDLV2000Writer mdlw = new MDLV2000Writer(sw))
            {
                mdlw.Write(mdlr.Read(new AtomContainer()));
                string output = sw.ToString();
                Assert.IsTrue(output.Contains("M  SST  1   1 RAN"));
                Assert.IsTrue(output.Contains("M  STY  3   1 COP   2 SRU   3 SRU"));
            }
        }

        [TestMethod()]
        public void SgroupExpandedAbbreviationRoundTrip()
        {
            StringWriter sw = new StringWriter();
            using (MDLV2000Reader mdlr = new MDLV2000Reader(ResourceLoader.GetAsStream("NCDK.Data.MDL.triphenyl-phosphate-expanded.mol")))
            using (MDLV2000Writer mdlw = new MDLV2000Writer(sw))
            {
                mdlw.Write(mdlr.Read(new AtomContainer()));
                string output = sw.ToString();
                Assert.IsTrue(output.Contains("M  STY  3   1 SUP   2 SUP   3 SUP" + Environment.NewLine));
                Assert.IsTrue(output.Contains("M  SDS EXP  1   1"));
            }
        }

        [TestMethod()]
        public void SgroupParentAtomListRoundTrip()
        {
            StringWriter sw = new StringWriter();
            using (MDLV2000Reader mdlr = new MDLV2000Reader(ResourceLoader.GetAsStream("NCDK.Data.MDL.ChEBI_81539.mol")))
            using (MDLV2000Writer mdlw = new MDLV2000Writer(sw))
            {
                mdlw.Write(mdlr.Read(new AtomContainer()));
                string output = sw.ToString();
                Assert.IsTrue(output.Contains("M  STY  5   1 MUL   2 SRU"));
                Assert.IsTrue(output.Contains("M  SPA   1 12"));
            }
        }

        [TestMethod()]
        public void SgroupOrderedMixtureRoundTrip()
        {
            StringWriter sw = new StringWriter();
            using (MDLV2000Reader mdlr = new MDLV2000Reader(ResourceLoader.GetAsStream("NCDK.Data.MDL.sgroup-ord-mixture.mol")))
            using (MDLV2000Writer mdlw = new MDLV2000Writer(sw))
            {
                mdlw.Write(mdlr.Read(new AtomContainer()));
                string output = sw.ToString();
                Assert.IsTrue(output.Contains("M  STY  3   1 COM   2 COM   3 FOR"));
                Assert.IsTrue(output.Contains("M  SNC  1   1   1"));
                Assert.IsTrue(output.Contains("M  SNC  1   2   2"));
            }
        }

        [TestMethod()]
        public void RoundtripAtomParityExpH()
        {
            StringWriter sw = new StringWriter();
            using (MDLV2000Reader mdlr = new MDLV2000Reader(ResourceLoader.GetAsStream("NCDK.Data.MDL.tetrahedral-parity-withExpH.mol")))
            using (MDLV2000Writer mdlw = new MDLV2000Writer(sw))
            {
                mdlw.Write(mdlr.Read(new AtomContainer()));
                string output = sw.ToString();
                Assert.IsTrue(output.Contains("    0.0000    0.0000    0.0000 C   0  0  1  0  0  0  0  0  0  0  0  0" + Environment.NewLine));
            }
        }

        [TestMethod()]
        public void RoundtripAtomParityImplH()
        {
            StringWriter sw = new StringWriter();
            using (MDLV2000Reader mdlr = new MDLV2000Reader(ResourceLoader.GetAsStream("NCDK.Data.MDL.tetrahedral-parity-withImplH.mol")))
            using (MDLV2000Writer mdlw = new MDLV2000Writer(sw))
            {
                mdlw.Write(mdlr.Read(new AtomContainer()));
                string output = sw.ToString();
                Assert.IsTrue(output.Contains("    0.0000    0.0000    0.0000 C   0  0  1  0  0  0  0  0  0  0  0  0" + Environment.NewLine));
            }
        }

        [TestMethod()]
        public void RoundtripAtomParityImplModified()
        {
            StringWriter sw = new StringWriter();
            using (MDLV2000Reader mdlr = new MDLV2000Reader(ResourceLoader.GetAsStream("NCDK.Data.MDL.tetrahedral-parity-withImplH.mol")))
            using (MDLV2000Writer mdlw = new MDLV2000Writer(sw))
            {
                AtomContainer mol = mdlr.Read(new AtomContainer());
                ITetrahedralChirality tc = (ITetrahedralChirality)mol.StereoElements.First();
                tc.Stereo = tc.Stereo.Invert();
                mdlw.Write(mol);
                string output = sw.ToString();
                Assert.IsTrue(output.Contains("    0.0000    0.0000    0.0000 C   0  0  2  0  0  0  0  0  0  0  0  0" + Environment.NewLine));
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        public void AromaticBondTypes()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            mol.Atoms.Add(builder.CreateAtom("C"));
            mol.Atoms.Add(builder.CreateAtom("C"));
            IBond bond = builder.CreateBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Unset);
            bond.IsAromatic = true;
            mol.Bonds.Add(bond);
            StringWriter sw = new StringWriter();
            using (MDLV2000Writer mdlw = new MDLV2000Writer(sw))
            {
                mdlw.Write(mol);
            }
        }

        [TestMethod()]
        public void AromaticBondTypesEnabled()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            mol.Atoms.Add(builder.CreateAtom("C"));
            mol.Atoms.Add(builder.CreateAtom("C"));
            IBond bond = builder.CreateBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Unset);
            bond.IsAromatic = true;
            mol.Bonds.Add(bond);
            StringWriter sw = new StringWriter();
            using (MDLV2000Writer mdlw = new MDLV2000Writer(sw))
            {
                mdlw.SetWriteAromaticBondTypes(true);
                mdlw.Write(mol);
            }
            Assert.IsTrue(sw.ToString().Replace(Environment.NewLine, "\n").Contains("  1  2  4  0  0  0  0 \n"));
        }
    }
}
