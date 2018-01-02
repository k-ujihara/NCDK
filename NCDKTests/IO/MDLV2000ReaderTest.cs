/* 
 * Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
 *                    2014  Mark B Vine (orcid:0000-0002-7794-0426)
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
 *  */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCDK.Default;
using NCDK.Geometries;
using NCDK.IO.Listener;
using NCDK.Isomorphisms.Matchers;
using NCDK.SGroups;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the reading MDL mol files using one test file.
    /// A test case for SDF files is available as separate Class.
    /// </summary>
    /// <seealso cref="MDLV2000Reader"/>
    /// <seealso cref="SDFReaderTest"/>
    // @cdk.module test-io
    [TestClass()]
    public class MDLV2000ReaderTest : SimpleChemObjectReaderTest
    {
        protected override string TestFile => "NCDK.Data.MDL.bug682233.mol";
        protected override Type ChemObjectIOToTestType => typeof(MDLV2000Reader);

        [TestMethod()]
        public void TestAccepts()
        {
            MDLV2000Reader reader = new MDLV2000Reader(new StringReader(""));
            Assert.IsTrue(reader.Accepts(typeof(ChemFile)));
            Assert.IsTrue(reader.Accepts(typeof(ChemModel)));
            Assert.IsTrue(reader.Accepts(typeof(AtomContainer)));
        }

        // @cdk.bug 3084064
        [TestMethod()]
        public void TestBug3084064()
        {
            var filename = "NCDK.Data.MDL.weirdprops.sdf";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = reader.Read(new ChemFile());
            reader.Close();

            Assert.IsNotNull(chemFile);

            IList<IAtomContainer> mols = ChemFileManipulator.GetAllAtomContainers(chemFile).ToList();
            Assert.AreEqual(10, mols.Count);

            IAtomContainer mol = mols[0];
            var props = mol.GetProperties();
            Assert.IsNotNull(props);
            Assert.AreEqual(5, props.Count());

            string[] keys = { "DatabaseID", "cdk:Title", "PeaksExplained", "cdk:Remark", "Score" };
            foreach (var s in keys)
            {
                bool found = false;
                foreach (var key in props.Keys)
                {
                    if (s.Equals(key))
                    {
                        found = true;
                        break;
                    }
                }
                Assert.IsTrue(found, s + " was not read from the file");
            }
        }

        // @cdk.bug 682233
        [TestMethod()]
        public void TestBug682233()
        {
            var filename = "NCDK.Data.MDL.bug682233.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = reader.Read(new ChemFile());
            reader.Close();

            Assert.IsNotNull(chemFile);
            Assert.AreEqual(1, chemFile.Count);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(1, seq.Count);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);

            var som = model.MoleculeSet;
            Assert.IsNotNull(som);
            Assert.AreEqual(1, som.Count);
            IAtomContainer m = som[0];
            Assert.IsNotNull(m);
            Assert.AreEqual(4, m.Atoms.Count);
            Assert.AreEqual(2, m.Bonds.Count);

            // test reading of formal charges
            IAtom a = m.Atoms[0];
            Assert.IsNotNull(a);
            Assert.AreEqual("Na", a.Symbol);
            Assert.AreEqual(1, a.FormalCharge.Value);
            a = m.Atoms[2];
            Assert.IsNotNull(a);
            Assert.AreEqual("O", a.Symbol);
            Assert.AreEqual(-1, a.FormalCharge.Value);
        }

        [TestMethod()]
        public void TestAPinene()
        {
            string filename = "NCDK.Data.MDL.a-pinene.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = reader.Read(new ChemFile());
            reader.Close();
            Assert.IsNotNull(chemFile);
            IList<IAtomContainer> containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToList();
            Assert.AreEqual(1, containersList.Count);
            Assert.IsTrue(containersList[0].Atoms.Count > 0);
            Assert.IsTrue(containersList[0].Bonds.Count > 0);
        }

        [TestMethod()]
        public void TestReadingMISOLines()
        {
            var filename = "NCDK.Data.MDL.ChEBI_37340.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            ChemFile chemFile = reader.Read(new ChemFile());
            reader.Close();
            Assert.IsNotNull(chemFile);
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToList();
            Assert.AreEqual(1, containersList.Count);
            Assert.IsTrue(containersList[0].Atoms.Count > 0);
            Assert.AreEqual(210, containersList[0].Atoms[0].MassNumber.Value);
        }

        // @cdk.bug 2234820
        [TestMethod()]
        public void TestMassNumber()
        {
            string filename = "NCDK.Data.MDL.massnumber.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            ChemFile chemFile = reader.Read(new ChemFile());
            reader.Close();
            Assert.IsNotNull(chemFile);
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToList();
            Assert.AreEqual(1, containersList.Count);
            Assert.AreEqual(3, containersList[0].Atoms.Count);
            Assert.AreEqual(2, containersList[0].Atoms[1].MassNumber.Value);
            Assert.AreEqual(3, containersList[0].Atoms[2].MassNumber.Value);
        }

        [TestMethod()]
        public void TestAlkane()
        {
            var filename = "NCDK.Data.MDL.shortest_path_test.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = reader.Read(new ChemFile());
            reader.Close();
            Assert.IsNotNull(chemFile);
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToList();
            Assert.AreEqual(1, containersList.Count);
            IAtomContainer container = containersList[0];
            Assert.AreEqual(10, container.Atoms.Count);
            Assert.AreEqual(9, container.Bonds.Count);
            foreach (var atom in container.Atoms)
                Assert.AreEqual("C", atom.Symbol);
            foreach (var bond in container.Bonds)
                Assert.AreEqual(BondOrder.Single, bond.Order);
        }

        [TestMethod()]
        public void TestReadTitle()
        {
            var filename = "NCDK.Data.MDL.a-pinene.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer mol = reader.Read(new AtomContainer());
            reader.Close();
            Assert.AreEqual("a-pinen.mol", mol.Title);
        }

        [TestMethod()]
        public void TestFourRing()
        {
            var filename = "NCDK.Data.MDL.four-ring-5x10.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = reader.Read(new ChemFile());
            reader.Close();
            Assert.IsNotNull(chemFile);
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToList();
            Assert.AreEqual(1, containersList.Count);
            Assert.IsTrue(containersList[0].Atoms.Count > 0);
            Assert.IsTrue(containersList[0].Bonds.Count > 0);
        }

        [TestMethod()]
        public void TestHydrozyamino()
        {
            var filename = "NCDK.Data.MDL.hydroxyamino.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = reader.Read(new ChemFile());
            reader.Close();
            Assert.IsNotNull(chemFile);
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToList();
            Assert.AreEqual(1, containersList.Count);
            Assert.IsTrue(containersList[0].Atoms.Count > 0);
            Assert.IsTrue(containersList[0].Bonds.Count > 0);
        }

        [TestMethod()]
        public void TestMethylBenzol()
        {
            var filename = "NCDK.Data.MDL.methylbenzol.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = reader.Read(new ChemFile());
            reader.Close();
            Assert.IsNotNull(chemFile);
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToList();
            Assert.AreEqual(1, containersList.Count);
            Assert.IsTrue(containersList[0].Atoms.Count > 0);
            Assert.IsTrue(containersList[0].Bonds.Count > 0);
        }

        [TestMethod()]
        public void TestPolycarpol()
        {
            var filename = "NCDK.Data.MDL.polycarpol.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = reader.Read(new ChemFile());
            reader.Close();
            Assert.IsNotNull(chemFile);
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToList();
            Assert.AreEqual(1, containersList.Count);
            Assert.IsTrue(containersList[0].Atoms.Count > 0);
            Assert.IsTrue(containersList[0].Bonds.Count > 0);
        }

        [TestMethod()]
        public void TestReserpine()
        {
            var filename = "NCDK.Data.MDL.reserpine.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = reader.Read(new ChemFile());
            reader.Close();
            Assert.IsNotNull(chemFile);
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToList();
            Assert.AreEqual(1, containersList.Count);
            Assert.IsTrue(containersList[0].Atoms.Count > 0);
            Assert.IsTrue(containersList[0].Bonds.Count > 0);
        }

        [TestMethod()]
        public void TestSixRing()
        {
            var filename = "NCDK.Data.MDL.six-ring-4x4.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = reader.Read(new ChemFile());
            reader.Close();
            Assert.IsNotNull(chemFile);
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToList();
            Assert.AreEqual(1, containersList.Count);
            Assert.IsTrue(containersList[0].Atoms.Count > 0);
            Assert.IsTrue(containersList[0].Bonds.Count > 0);
        }

        [TestMethod()]
        public void TestSuperspiro()
        {
            var filename = "NCDK.Data.MDL.superspiro.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = reader.Read(new ChemFile());
            reader.Close();
            Assert.IsNotNull(chemFile);
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToList();
            Assert.AreEqual(1, containersList.Count);
            Assert.IsTrue(containersList[0].Atoms.Count > 0);
            Assert.IsTrue(containersList[0].Bonds.Count > 0);
        }

        [TestMethod()]
        public void TestGhemicalOutput()
        {
            var filename = "NCDK.Data.MDL.butanoic_acid.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = reader.Read(new ChemFile());
            reader.Close();
            Assert.IsNotNull(chemFile);
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToList();
            Assert.AreEqual(1, containersList.Count);
            Assert.IsTrue(containersList[0].Atoms.Count > 0);
            Assert.IsTrue(containersList[0].Bonds.Count > 0);
        }

        [TestMethod()]
        public void TestUsesGivenMolecule()
        {
            var filename = "NCDK.Data.MDL.superspiro.mol"; // just a random file
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer superspiro = new AtomContainer();
            superspiro.Id = "superspiro";
            IAtomContainer result = reader.Read(superspiro);
            reader.Close();
            Assert.AreEqual(superspiro.Id, result.Id);
        }

        // @cdk.bug 835571
        [TestMethod()]
        public void TestReadFromStringReader()
        {
            var mdl = "cyclopropane.mol\n" + "\n" + "\n" + "  9  9  0  0  0                 1 V2000\n"
                    + "   -0.0073   -0.5272    0.9655 C   0  0  0  0  0\n"
                    + "   -0.6776   -0.7930   -0.3498 C   0  0  0  0  0\n"
                    + "    0.2103    0.4053   -0.1891 C   0  0  0  0  0\n"
                    + "    0.8019   -1.1711    1.2970 H   0  0  0  0  0\n"
                    + "   -0.6000   -0.2021    1.8155 H   0  0  0  0  0\n"
                    + "   -1.7511   -0.6586   -0.4435 H   0  0  0  0  0\n"
                    + "   -0.3492   -1.6277   -0.9620 H   0  0  0  0  0\n"
                    + "    1.1755    0.4303   -0.6860 H   0  0  0  0  0\n"
                    + "   -0.2264    1.3994   -0.1675 H   0  0  0  0  0\n" + "  1  2  1  6  0  0\n"
                    + "  1  3  1  6  0  0\n" + "  1  4  1  0  0  0\n" + "  1  5  1  1  0  0\n" + "  2  3  1  0  0  0\n"
                    + "  2  6  1  0  0  0\n" + "  2  7  1  6  0  0\n" + "  3  8  1  6  0  0\n" + "  3  9  1  0  0  0\n"
                    + "M  END\n";
            MDLV2000Reader reader = new MDLV2000Reader(new StringReader(mdl));
            ChemFile chemFile = reader.Read(new ChemFile());
            reader.Close();
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(1, chemFile.Count);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(1, seq.Count);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);

            var som = model.MoleculeSet;
            Assert.IsNotNull(som);
            Assert.AreEqual(1, som.Count);
            IAtomContainer m = som[0];
            Assert.IsNotNull(m);
            Assert.AreEqual(9, m.Atoms.Count);
            Assert.AreEqual(9, m.Bonds.Count);
        }

        [TestMethod()]
        public void TestRGroup()
        {
            var filename = "NCDK.Data.MDL.SARGROUPTEST.sdf";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer mol = reader.Read(new AtomContainer());
            reader.Close();
            Assert.AreEqual("R2", ((IPseudoAtom)mol.Atoms[19]).Label);
        }

        [TestMethod()]
        public void TestAliasPropertyGroup()
        {
            var filename = "NCDK.Data.MDL.AliasPropertyRGroup.sdf";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer mol = reader.Read(new AtomContainer());
            reader.Close();
            IAtom atom = mol.Atoms[0];
            Assert.IsTrue(atom is IPseudoAtom);
            Assert.AreEqual("R\\1", ((IPseudoAtom)atom).Label);
        }

        // @cdk.bug 1587283
        [TestMethod()]
        public void TestBug1587283()
        {
            var filename = "NCDK.Data.MDL.bug1587283.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = reader.Read(new ChemFile());
            reader.Close();
            Assert.IsNotNull(chemFile);
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToList();
            Assert.AreEqual(1, containersList.Count);
            Assert.AreEqual(15, containersList[0].Atoms.Count);
            Assert.AreEqual(16, containersList[0].Bonds.Count);
        }

        [TestMethod()]
        public void TestReadProton()
        {
            var mdl = "proton.mol\n" + "\n" + "\n" + "  1  0  0  0  0                 1 V2000\n"
                    + "   -0.0073   -0.5272    0.9655 H   0  0  0  0  0\n" + "M  CHG  1   1   1\n" + "M  END\n";
            MDLV2000Reader reader = new MDLV2000Reader(new StringReader(mdl));
            IAtomContainer mol = reader.Read(new AtomContainer());
            reader.Close();
            Assert.IsNotNull(mol);
            Assert.AreEqual(1, mol.Atoms.Count);
            Assert.AreEqual(0, mol.Bonds.Count);
            Assert.AreEqual(1, AtomContainerManipulator.GetTotalFormalCharge(mol));
            IAtom atom = mol.Atoms[0];
            Assert.AreEqual(1, atom.FormalCharge.Value);
        }

        [TestMethod()]
        public void TestReadingCharges()
        {
            var filename = "NCDK.Data.MDL.withcharges.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IChemFile chemFile = reader.Read(new ChemFile());
            reader.Close();
            IAtomContainer container = chemFile[0][0].MoleculeSet[0];
            Assert.AreEqual(1, container.Atoms[6].FormalCharge.Value);
            Assert.AreEqual(-1, container.Atoms[8].FormalCharge.Value);
        }

        [TestMethod()]
        public void TestEmptyString()
        {
            var emptyString = "";
            MDLV2000Reader reader = new MDLV2000Reader(new StringReader(emptyString));
            IAtomContainer mol = reader.Read(new AtomContainer());
            reader.Close();
            Assert.IsNull(mol);
        }

        [TestMethod()]
        public void TestNoAtomCase()
        {
            var filename = "NCDK.Data.MDL.emptyStructure.sdf";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = reader.Read(new ChemFile());
            reader.Close();
            Assert.IsNotNull(chemFile);
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToList();
            Assert.AreEqual(1, containersList.Count);

            IAtomContainer container = containersList[0];
            Assert.IsNotNull(container);
            Assert.AreEqual(0, container.Atoms.Count);
            Assert.AreEqual(0, container.Bonds.Count);

            var props = container.GetProperties();
            var keys = props.Keys;

            Assert.IsTrue(keys.Contains("SubstanceType"));
            Assert.IsTrue(keys.Contains("TD50 Rat"));
            Assert.IsTrue(keys.Contains("ChemCount"));
        }

        // @cdk.bug 1732307
        [TestMethod()]
        public void TestZeroZCoordinates()
        {
            var filename = "NCDK.Data.MDL.nozcoord.sdf";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            var prop = new NameValueCollection();
            prop["ForceReadAs3DCoordinates"] = "true";
            PropertiesListener listener = new PropertiesListener(prop);
            reader.Listeners.Add(listener);
            reader.CustomizeJob();

            IAtomContainer mol = reader.Read(Default.ChemObjectBuilder.Instance.NewAtomContainer());
            reader.Close();
            Assert.IsNotNull(mol);
            Assert.AreEqual(5, mol.Atoms.Count);

            bool has3d = GeometryUtil.Has3DCoordinates(mol);
            Assert.IsTrue(has3d);
        }

        // @cdk.bug 1732307 
        [TestMethod()]
        public void TestZeroZCoordinates3DMarked()
        {
            string filename = "NCDK.Data.MDL.nozcoord.sdf";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer mol = reader.Read(Default.ChemObjectBuilder.Instance.NewAtomContainer());
            reader.Close();
            Assert.IsNotNull(mol);
            Assert.AreEqual(5, mol.Atoms.Count);

            bool has3d = GeometryUtil.Has3DCoordinates(mol);
            Assert.IsTrue(has3d);
        }

        // @cdk.bug 1826577
        [TestMethod()]
        public void TestHIsotopes_Strict()
        {
            string filename = "NCDK.Data.MDL.hisotopes.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            try
            {
                MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
                reader.Read(new ChemFile());
                reader.Close();
                Assert.Fail("Expected a CDKException");
            }
            catch (CDKException)
            {
                // OK, that's what's is supposed to happen
            }
            catch (IOException)
            {
                // OK, that's what's is supposed to happen
            }
        }

        // @cdk.bug 1826577
        [TestMethod()]
        public void TestHIsotopes_Relaxed()
        {
            var filename = "NCDK.Data.MDL.hisotopes.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Relaxed);
            IChemFile chemFile = reader.Read(new ChemFile());
            reader.Close();
            Assert.IsNotNull(chemFile);
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToList();
            Assert.IsNotNull(containersList[0]);
            Assert.IsFalse((containersList[0]).Atoms[1] is IPseudoAtom);
            Assert.IsFalse((containersList[0]).Atoms[2] is IPseudoAtom);
        }

        [TestMethod()]
        public void TestReadRadical()
        {
            var filename = "NCDK.Data.MDL.332727182.radical.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            ChemFile chemFile = reader.Read(new ChemFile());
            reader.Close();
            Assert.IsNotNull(chemFile);
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToList();
            Assert.AreEqual(1, containersList.Count);
            Assert.IsTrue((containersList[0]).Atoms.Count > 0);
            Assert.IsTrue((containersList[0]).Bonds.Count > 0);
            Assert.IsTrue((containersList[0]).SingleElectrons.Count > 0);
        }

        // @cdk.bug 2604888
        [TestMethod()]
        public void TestNoCoordinates()
        {
            var mdl = "cyclopropane.mol\n" + "\n" + "\n" + "  9  9  0  0  0 0 0 0 0 0 0 0 0 1 V2000\n"
                    + "    0.0000    0.0000    0.0000 C   0  0  0  0  0\n"
                    + "    0.0000    0.0000    0.0000 C   0  0  0  0  0\n"
                    + "    0.0000    0.0000    0.0000 C   0  0  0  0  0\n"
                    + "    0.0000    0.0000    0.0000 H   0  0  0  0  0\n"
                    + "    0.0000    0.0000    0.0000 H   0  0  0  0  0\n"
                    + "    0.0000    0.0000    0.0000 H   0  0  0  0  0\n"
                    + "    0.0000    0.0000    0.0000 H   0  0  0  0  0\n"
                    + "    0.0000    0.0000    0.0000 H   0  0  0  0  0\n"
                    + "    0.0000    0.0000    0.0000 H   0  0  0  0  0\n" + "  1  2  1  6  0  0\n"
                    + "  1  3  1  6  0  0\n" + "  1  4  1  0  0  0\n" + "  1  5  1  1  0  0\n" + "  2  3  1  0  0  0\n"
                    + "  2  6  1  0  0  0\n" + "  2  7  1  6  0  0\n" + "  3  8  1  6  0  0\n" + "  3  9  1  0  0  0\n"
                    + "M  END\n";
            MDLV2000Reader reader = new MDLV2000Reader(new StringReader(mdl));
            IAtomContainer molecule = reader.Read(new AtomContainer());
            reader.Close();
            Assert.IsNotNull(molecule);
            Assert.AreEqual(9, molecule.Atoms.Count);
            Assert.AreEqual(9, molecule.Bonds.Count);
            foreach (var atom in molecule.Atoms)
            {
                Assert.IsNull(atom.Point2D);
                Assert.IsNull(atom.Point2D);
            }
        }

        [TestMethod()]
        public void TestUndefinedStereo()
        {
            var filename = "NCDK.Data.MDL.ChEBI_26120.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            IAtomContainer mol = reader.Read(new AtomContainer());
            reader.Close();
            Assert.AreEqual(BondStereo.EOrZ, mol.Bonds[1].Stereo);
            Assert.AreEqual(BondStereo.EOrZ, mol.Bonds[6].Stereo);
            Assert.AreEqual(BondStereo.EOrZ, mol.Bonds[7].Stereo);
            Assert.AreEqual(BondStereo.EOrZ, mol.Bonds[11].Stereo);
        }

        [TestMethod()]
        public void TestUndefinedStereo2()
        {
            var filename = "NCDK.Data.MDL.a-pinene-with-undefined-stereo.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            IAtomContainer mol = reader.Read(new AtomContainer());
            reader.Close();
            Assert.AreEqual(BondStereo.UpOrDown, mol.Bonds[1].Stereo);
        }

        /// <summary>
        /// Tests that the '0' read from the bond block for bond stereo
        /// is read is 'no stereochemistry involved'.
        /// </summary>
        [TestMethod()]
        public void TestStereoReadZeroDefault()
        {
            var filename = "NCDK.Data.MDL.withcharges.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            ChemFile chemFile = reader.Read(new ChemFile());
            reader.Close();
            Assert.IsNotNull(chemFile);
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToList();
            Assert.AreEqual(1, containersList.Count);
            IAtomContainer container = containersList[0];
            Assert.AreEqual(BondStereo.None, container.Bonds[0].Stereo);
        }

        [TestMethod()]
        public void TestReadStereoBonds()
        {
            var mdl = "cyclopropane.mol\n" + "\n" + "\n" + "  9  9  0  0  0                 1 V2000\n"
                    + "   -0.0073   -0.5272    0.9655 C   0  0  0  0  0\n"
                    + "   -0.6776   -0.7930   -0.3498 C   0  0  0  0  0\n"
                    + "    0.2103    0.4053   -0.1891 C   0  0  0  0  0\n"
                    + "    0.8019   -1.1711    1.2970 H   0  0  0  0  0\n"
                    + "   -0.6000   -0.2021    1.8155 H   0  0  0  0  0\n"
                    + "   -1.7511   -0.6586   -0.4435 H   0  0  0  0  0\n"
                    + "   -0.3492   -1.6277   -0.9620 H   0  0  0  0  0\n"
                    + "    1.1755    0.4303   -0.6860 H   0  0  0  0  0\n"
                    + "   -0.2264    1.3994   -0.1675 H   0  0  0  0  0\n" + "  1  2  1  6  0  0\n"
                    + "  1  3  1  6  0  0\n" + "  1  4  1  0  0  0\n" + "  1  5  1  1  0  0\n" + "  2  3  1  0  0  0\n"
                    + "  2  6  1  0  0  0\n" + "  2  7  1  6  0  0\n" + "  3  8  1  6  0  0\n" + "  3  9  1  0  0  0\n"
                    + "M  END\n";
            MDLV2000Reader reader = new MDLV2000Reader(new StringReader(mdl));
            IAtomContainer mol = reader.Read(new AtomContainer());
            reader.Close();
            Assert.IsNotNull(mol);
            Assert.AreEqual(9, mol.Atoms.Count);
            Assert.AreEqual(9, mol.Bonds.Count);
            Assert.AreEqual(BondStereo.Down, mol.Bonds[0].Stereo);
            Assert.AreEqual(BondStereo.Up, mol.Bonds[3].Stereo);
        }

        [TestMethod()]
        public void TestStereoDoubleBonds()
        {
            var filename = "NCDK.Data.MDL.butadiene.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            ChemFile chemFile = reader.Read(new ChemFile());
            reader.Close();
            Assert.IsNotNull(chemFile);
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToList();
            Assert.AreEqual(1, containersList.Count);
            IAtomContainer container = containersList[0];
            Assert.AreEqual(BondStereo.EZByCoordinates, container.Bonds[0].Stereo);
            Assert.AreEqual(BondStereo.EOrZ, container.Bonds[2].Stereo);
        }

        /// <summary>
        /// Tests numbering of R# elements according to RGP line.
        /// </summary>
        [TestMethod()]
        public void TestRGroupHashNumbering()
        {
            var filename = "NCDK.Data.MDL.rgroups.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer mol = reader.Read(new AtomContainer());
            reader.Close();
            foreach (var bond in mol.Bonds)
            {
                IPseudoAtom rGroup = null;
                IAtom partner = null;
                if (bond.Atoms[0] is IPseudoAtom)
                {
                    rGroup = (IPseudoAtom)bond.Begin;
                    partner = bond.End;
                }
                else
                {
                    partner = bond.Begin;
                    rGroup = (IPseudoAtom)bond.End;
                }
                if (partner.Symbol.Equals("N"))
                {
                    Assert.AreEqual(rGroup.Label, "R4");
                }
                else if (partner.Symbol.Equals("P"))
                {
                    Assert.AreEqual(rGroup.Label, "R1");
                }
                else if (partner.Symbol.Equals("As"))
                {
                    Assert.AreEqual(rGroup.Label, "R4");
                }
                else if (partner.Symbol.Equals("Si"))
                {
                    Assert.AreEqual(rGroup.Label, "R");
                }
            }
        }

        /// <summary>
        /// Test for hard coded R-group numbers in the Atom block.
        /// Hard coding is accepted but should not be done really, instead use
        /// a hash (#) conform the CTFile spec.
        /// </summary>
        [TestMethod()]
        public void TestRGroupHardcodedNumbering()
        {
            var filename = "NCDK.Data.MDL.rgroupsNumbered.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer mol = reader.Read(Default.ChemObjectBuilder.Instance.NewAtomContainer());
            reader.Close();
            foreach (var bond in mol.Bonds)
            {
                IPseudoAtom rGroup;
                if (bond.Atoms[0] is IPseudoAtom)
                    rGroup = (IPseudoAtom)bond.Begin;
                else
                    rGroup = (IPseudoAtom)bond.End;

                if (bond.Order == BondOrder.Double)
                {
                    Assert.AreEqual(rGroup.Label, "R32");
                }
                else if (bond.Stereo == BondStereo.Down)
                {
                    Assert.AreEqual(rGroup.Label, "R2");
                }
                else if (bond.Stereo == BondStereo.Up)
                {
                    Assert.AreEqual(rGroup.Label, "R20");
                }
                else
                    Assert.AreEqual(rGroup.Label, "R5");
            }
        }

        [TestMethod()]
        public void TestReadValence()
        {
            var filename = "NCDK.Data.MDL.a-pinene-with-valence.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);

            IAtomContainer mol = reader.Read(new AtomContainer());
            reader.Close();
            Assert.IsNotNull(mol);
            Assert.AreEqual(2, mol.Atoms[0].Valency.Value);
            Assert.AreEqual(3, mol.Atoms[1].Valency.Value);
            Assert.AreNotEqual(0, mol.Atoms[2].Valency);
            Assert.AreEqual(4, mol.Atoms[2].Valency);
            Assert.AreEqual(0, mol.Atoms[3].Valency.Value);
        }

        [TestMethod()]
        public void TestShortLines()
        {
            Trace.TraceInformation("Testing short lines ChemObjectReaderModes.Relaxed");
            TestShortLinesForMode(ChemObjectReaderModes.Relaxed);
            Trace.TraceInformation("Testing short lines ChemObjectReaderModes.Strict");
            TestShortLinesForMode(ChemObjectReaderModes.Strict);
        }

        private void TestShortLinesForMode(ChemObjectReaderModes mode)
        {
            var filename = "NCDK.Data.MDL.glycine-short-lines.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins, mode);
            IAtomContainer mol = reader.Read(new AtomContainer());
            reader.Close();
            Assert.IsNotNull(mol);
            Assert.AreEqual(mol.Atoms.Count, 5);
            Assert.AreEqual(mol.Bonds.Count, 4);
        }

        [TestMethod()]
        public void TestReadAtomAtomMapping()
        {
            var filename = "NCDK.Data.MDL.a-pinene-with-atom-atom-mapping.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer mol = reader.Read(new AtomContainer());
            reader.Close();
            Assert.IsNotNull(mol);
            Assert.AreEqual(1, (int)mol.Atoms[0].GetProperty<int>(CDKPropertyName.AtomAtomMapping));
            Assert.AreEqual(15, (int)mol.Atoms[1].GetProperty<int>(CDKPropertyName.AtomAtomMapping));
            Assert.IsNull(mol.Atoms[2].GetProperty<int?>(CDKPropertyName.AtomAtomMapping));
        }

        // @cdk.bug 2936440
        [TestMethod()]
        public void TestHas2DCoordinates_With000()
        {
            var filenameMol = "NCDK.Data.MDL.with000coordinate.mol";
            var ins = ResourceLoader.GetAsStream(filenameMol);
            IAtomContainer molOne = null;
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            molOne = reader.Read(new AtomContainer());
            reader.Close();
            Assert.IsNotNull(molOne.Atoms[0].Point2D);
            Assert.IsNotNull(molOne.Atoms[0].Point3D);
        }

        [TestMethod()]
        public void TestAtomValueLines()
        {
            var filename = "NCDK.Data.MDL.atomValueLines.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer testMolecule = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            IAtomContainer result = reader.Read(testMolecule);
            reader.Close();
            IAtom oxygen = result.Atoms[0];
            Assert.IsTrue(oxygen.Symbol.Equals("O"));
            Assert.AreEqual(oxygen.GetProperty<string>(CDKPropertyName.Comment), "Oxygen comment");
        }

        [TestMethod()]
        public void TestDeuterium()
        {
            var filename = "NCDK.Data.MDL.chemblMolregno5369.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Relaxed);

            var prop = new NameValueCollection();
            prop["InterpretHydrogenIsotopes"] = "true";
            PropertiesListener listener = new PropertiesListener(prop);
            reader.Listeners.Add(listener);
            reader.CustomizeJob();

            IAtomContainer molecule = new AtomContainer();
            molecule = reader.Read(molecule);
            reader.Close();
            int deuteriumCount = 0;
            foreach (var atom in molecule.Atoms)
                if (atom.Symbol.Equals("H") && atom.MassNumber != null && atom.MassNumber == 2)
                    deuteriumCount++;
            Assert.AreEqual(3, deuteriumCount);
        }

        [TestMethod()]
        public void TestDeuteriumProperties()
        {
            var filename = "NCDK.Data.MDL.chemblMolregno5369.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Relaxed);
            IAtomContainer molecule = new AtomContainer();
            molecule = reader.Read(molecule);
            reader.Close();
            IAtom deuterium = molecule.Atoms[molecule.Atoms.Count - 1];
            Assert.IsTrue(1 == deuterium.AtomicNumber);
            Assert.IsTrue(2 == deuterium.MassNumber);
        }

        [TestMethod()]
        public void TestTritium()
        {
            var filename = "NCDK.Data.MDL.chemblMolregno7039.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer molecule = new AtomContainer();
            molecule = reader.Read(molecule);
            reader.Close();
            int tritiumCount = 0;
            foreach (var atom in molecule.Atoms)
                if (atom.Symbol.Equals("H") && atom.MassNumber != null && atom.MassNumber == 3)
                    tritiumCount++;
            Assert.AreEqual(1, tritiumCount);
        }

        [TestMethod()]
        public void TestTritiumProperties()
        {
            var filename = "NCDK.Data.MDL.chemblMolregno7039.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer molecule = new AtomContainer();
            molecule = reader.Read(molecule);
            reader.Close();
            IAtom tritium = molecule.Atoms[molecule.Atoms.Count - 1];
            Assert.IsTrue(1 == tritium.AtomicNumber);
            Assert.IsTrue(3 == tritium.MassNumber);
        }

        /// <summary>
        /// Tests a molfile with 'query' bond types (in this case bond type == 8 (any)).
        /// </summary>
        [TestMethod()]
        public void TestQueryBondType8()
        {
            var filename = "NCDK.Data.MDL.iridiumCoordination.chebi52748.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer atc = reader.Read(new AtomContainer());
            reader.Close();

            int queryBondCount = 0;
            foreach (var atom in atc.Atoms)
            {
                if (atom.Symbol.Equals("Ir"))
                {
                    foreach (var bond in atc.GetConnectedBonds(atom))
                    {
                        if (bond is CTFileQueryBond)
                        {
                            queryBondCount++;
                            Assert.IsTrue(((CTFileQueryBond)bond).Type == CTFileQueryBond.BondTypes.Any);
                            Assert.AreEqual(BondOrder.Unset, bond.Order);
                        }
                    }
                }
            }
            Assert.IsTrue(queryBondCount == 3, "Expecting three 'query' bond types to 'Ir'");
        }

        /// <summary>
        /// Tests a molfile with 'query' bond types (in this case bond type == 6).
        /// </summary>
        [TestMethod()]
        public void TestQueryBondType6()
        {
            var filename = "NCDK.Data.MDL.chebi.querybond.51736.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer atc = reader.Read(new AtomContainer());
            reader.Close();
            int queryBondCount = 0;

            foreach (var bond in atc.Bonds)
            {
                if (bond is CTFileQueryBond)
                {
                    queryBondCount++;
                    Assert.IsTrue(((CTFileQueryBond)bond).Type == CTFileQueryBond.BondTypes.SingleOrAromatic);
                    Assert.AreEqual(BondOrder.Unset, bond.Order);
                }
            }
            Assert.IsTrue(queryBondCount == 6, "Expecting six 'query' bond types");
        }

        /// <summary>
        /// Test that R-groups at higher atom numbers (>9) are read correctly
        /// </summary>
        [TestMethod()]
        public void TestRGroupHighAtomNumber()
        {
            var ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.brenda_molfile_rgroup.mol");
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer molecule = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            reader.Read(molecule);
            reader.Close();
            Assert.AreEqual("R", molecule.Atoms[55].Symbol);
        }

        [TestMethod()]
        public void TestAliasAtomNaming()
        {
            var ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.mol_testAliasAtomNaming.mol");
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer molecule = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            reader.Read(molecule);
            reader.Close();

            IAtom[] atoms = AtomContainerManipulator.GetAtomArray(molecule);

            int r1Count = 0;
            foreach (var atom in atoms)
            {
                if (atom is IPseudoAtom)
                {
                    Assert.AreEqual("R1", ((IPseudoAtom)atom).Label);
                    r1Count++;
                }
            }
            Assert.AreEqual(2, r1Count);
        }

        [TestMethod()]
        public void TestPseudoAtomLabels()
        {
            var ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.pseudoatoms.sdf");
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer molecule = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            molecule = reader.Read(molecule);
            reader.Close();
            Assert.IsTrue(molecule.Atoms[4] is IPseudoAtom);
            Assert.AreEqual("Gln", molecule.Atoms[4].Symbol);
            IPseudoAtom pa = (IPseudoAtom)molecule.Atoms[4];
            Assert.AreEqual("Gln", pa.Label);
        }

        // @cdk.bug 3485634
        [TestMethod()]
        public void TestMissingAtomProperties()
        {
            var ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.bug3485634.mol");
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer molecule = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            molecule = reader.Read(molecule);
            reader.Close();
            Assert.AreEqual(9, molecule.Atoms.Count);
        }

        [TestMethod()]
        public void TestBondOrderFour()
        {
            var ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.mdlWithBond4.mol");
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer molecule = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            molecule = reader.Read(molecule);
            reader.Close();
            Assert.AreEqual(9, molecule.Atoms.Count);
            Assert.AreEqual(BondOrder.Unset, molecule.Bonds[0].Order);
            Assert.IsTrue(molecule.Bonds[0].IsSingleOrDouble);
            Assert.AreEqual(BondOrder.Single, molecule.Bonds[1].Order);
            Assert.IsFalse(molecule.Bonds[1].IsSingleOrDouble);
        }

        [TestMethod()]
        public void TestAtomParity()
        {
            var ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.mol_testAtomParity.mol");
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer molecule = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            molecule = reader.Read(molecule);
            reader.Close();

            Assert.AreEqual(6, molecule.Atoms.Count);
            bool chiralCentre = false;
            IAtom[] atoms = AtomContainerManipulator.GetAtomArray(molecule);
            foreach (var atom in atoms)
            {
                var parity = atom.StereoParity;
                if (parity == 1)
                {
                    chiralCentre = true;
                }
            }

            Assert.IsTrue(chiralCentre);

        }

        [TestMethod()]
        public void TestSingleSingletRadical()
        {

            var ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.singleSingletRadical.mol");
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer molecule = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            molecule = reader.Read(molecule);
            reader.Close();

            Assert.AreEqual(2, molecule.GetConnectedSingleElectrons(molecule.Atoms[1]).Count());
        }

        [TestMethod()]
        public void TestSingleDoubletRadical()
        {

            var ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.singleDoubletRadical.mol");
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer molecule = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            molecule = reader.Read(molecule);
            reader.Close();

            Assert.AreEqual(1, molecule.GetConnectedSingleElectrons(molecule.Atoms[1]).Count());
        }

        [TestMethod()]
        public void TestSingleTripletRadical()
        {

            var ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.singleTripletRadical.mol");
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer molecule = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            molecule = reader.Read(molecule);
            reader.Close();
            Assert.AreEqual(2, molecule.GetConnectedSingleElectrons(molecule.Atoms[1]).Count());
        }

        [TestMethod()]
        public void TestMultipleRadicals()
        {

            var ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.multipleRadicals.mol");
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer molecule = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            molecule = reader.Read(molecule);
            reader.Close();

            Assert.AreEqual(1, molecule.GetConnectedSingleElectrons(molecule.Atoms[0]).Count());
            Assert.AreEqual(1, molecule.GetConnectedSingleElectrons(molecule.Atoms[1]).Count());
            Assert.AreEqual(1, molecule.GetConnectedSingleElectrons(molecule.Atoms[2]).Count());
            Assert.AreEqual(1, molecule.GetConnectedSingleElectrons(molecule.Atoms[3]).Count());
            Assert.AreEqual(1, molecule.GetConnectedSingleElectrons(molecule.Atoms[4]).Count());
            Assert.AreEqual(1, molecule.GetConnectedSingleElectrons(molecule.Atoms[5]).Count());
            Assert.AreEqual(1, molecule.GetConnectedSingleElectrons(molecule.Atoms[6]).Count());
            Assert.AreEqual(1, molecule.GetConnectedSingleElectrons(molecule.Atoms[7]).Count());
            Assert.AreEqual(1, molecule.GetConnectedSingleElectrons(molecule.Atoms[8]).Count());
        }

        [TestMethod()]
        public void Fe_iii_valence()
        {
            var ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.iron-iii.mol");
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer molecule = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            molecule = reader.Read(molecule);
            reader.Close();
            Assert.AreEqual(1, molecule.Atoms[0].ImplicitHydrogenCount);
            Assert.AreEqual(0, molecule.Atoms[1].ImplicitHydrogenCount);
            Assert.AreEqual(0, molecule.Atoms[2].ImplicitHydrogenCount);
        }

        [TestMethod()]
        public void Bismuth_ion_valence()
        {
            var ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.bismuth-ion.mol");
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer molecule = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            molecule = reader.Read(molecule);
            reader.Close();
            Assert.AreEqual(3, molecule.Atoms[0].ImplicitHydrogenCount);
        }

        [TestMethod()]
        public void E_butene_2D()
        {
            var ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.e_butene_2d.mol");
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer molecule = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            molecule = reader.Read(molecule);
            reader.Close();
            Assert.IsTrue(molecule.StereoElements.GetEnumerator().MoveNext());
        }

        // when there are no coordinates stereo perception should not be done
        [TestMethod()]
        public void E_butene_0D()
        {
            var ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.e_butene_0d.mol");
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer molecule = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            molecule = reader.Read(molecule);
            reader.Close();
            Assert.IsNotNull(molecule);
            Assert.IsFalse(molecule.StereoElements.GetEnumerator().MoveNext());
        }

        // forcing as 3D is problematic for stereo perception as we put 2D coordinates
        // in to 3D as we then no longer know to check wedge/hatch labels.
        [TestMethod()]
        public void E_butene_2D_force3D()
        {
            var ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.e_butene_2d.mol");
            MDLV2000Reader reader = new MDLV2000Reader(ins);

            // yuk!
            var prop = new NameValueCollection();
            prop["ForceReadAs3DCoordinates"] = "true";
            PropertiesListener listener = new PropertiesListener(prop);
            reader.Listeners.Add(listener);
            reader.CustomizeJob();

            IAtomContainer molecule = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            molecule = reader.Read(molecule);
            reader.Close();
            Assert.IsNotNull(molecule);
            Assert.IsFalse(molecule.StereoElements.GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void E_butene_3D()
        {
            var ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.e_butene_3d.mol");
            MDLV2000Reader reader = new MDLV2000Reader(ins);

            var prop = new NameValueCollection();
            prop["ForceReadAs3DCoordinates"] = "true";
            PropertiesListener listener = new PropertiesListener(prop);
            reader.Listeners.Add(listener);
            reader.CustomizeJob();

            IAtomContainer molecule = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            molecule = reader.Read(molecule);
            reader.Close();
            Assert.IsNotNull(molecule);
            Assert.IsTrue(molecule.StereoElements.GetEnumerator().MoveNext());
        }

        // turn off adding stereoelements
        [TestMethod()]
        public void E_butene_2D_optOff()
        {
            var ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.e_butene_2d.mol");
            MDLV2000Reader reader = new MDLV2000Reader(ins);

            var prop = new NameValueCollection();
            prop["AddStereoElements"] = "false";
            PropertiesListener listener = new PropertiesListener(prop);
            reader.Listeners.Add(listener);
            reader.CustomizeJob();

            IAtomContainer molecule = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            molecule = reader.Read(molecule);
            reader.Close();
            Assert.IsNotNull(molecule);
            Assert.IsFalse(molecule.StereoElements.GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void DataHeader_1()
        {
            Assert.AreEqual("DENSITY", MDLV2000Reader.DataHeader("> 29 <DENSITY> "));
        }

        [TestMethod()]
        public void DataHeader_2()
        {
            Assert.AreEqual("MELTING.POINT", MDLV2000Reader.DataHeader("> <MELTING.POINT> "));
        }

        [TestMethod()]
        public void DataHeader_3()
        {
            Assert.AreEqual("BOILING.POINT", MDLV2000Reader.DataHeader("> 55 (MD-08974) <BOILING.POINT> DT12"));
        }

        [TestMethod()]
        public void ReadNonStructuralData()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("> 29 <DENSITY>").Append('\n');
            sb.Append("0.9132 - 20.0").Append('\n');
            sb.Append('\n');
            sb.Append("> 29 <BOILING.POINT>").Append('\n');
            sb.Append("63.0 (737 MM)").Append('\n');
            sb.Append("79.0 (42 MM)").Append('\n');
            sb.Append('\n');
            sb.Append("> 29 <ALTERNATE.NAMES>").Append('\n');
            sb.Append("SYLVAN").Append('\n');
            sb.Append('\n');
            sb.Append("> 29 <DATE>").Append('\n');
            sb.Append("09-23-1980").Append('\n');
            sb.Append('\n');
            sb.Append("> 29 <CRC.NUMBER>").Append('\n');
            sb.Append("F-0213").Append('\n');
            sb.Append('\n');

            var input = new StringReader(sb.ToString());
            var mock = new Mock<IAtomContainer>();

            MDLV2000Reader.ReadNonStructuralData(input, mock.Object);

            mock.Verify(n => n.SetProperty("DENSITY", "0.9132 - 20.0"));
            mock.Verify(n => n.SetProperty("BOILING.POINT", "63.0 (737 MM)\n79.0 (42 MM)"));
            mock.Verify(n => n.SetProperty("ALTERNATE.NAMES", "SYLVAN"));
            mock.Verify(n => n.SetProperty("DATE", "09-23-1980"));
            mock.Verify(n => n.SetProperty("CRC.NUMBER", "F-0213"));
        }

        [TestMethod()]
        public void ReadNonStructuralData_emtpy()
        {
            // a single space is read as a property
            StringBuilder sb = new StringBuilder();
            sb.Append("> <ONE_SPACE>").Append('\n');
            sb.Append(" ").Append('\n');
            sb.Append('\n');
            // empty entries are read as non-null - so m.GetProperty() does not
            // return null
            sb.Append("> <EMTPY_LINES>").Append('\n');
            sb.Append('\n');
            sb.Append('\n');
            sb.Append('\n');

            var input = new StringReader(sb.ToString());
            var mock = new Mock<IAtomContainer>();

            MDLV2000Reader.ReadNonStructuralData(input, mock.Object);

            mock.Verify(n => n.SetProperty("ONE_SPACE", " "));
            mock.Verify(n => n.SetProperty("EMTPY_LINES", ""));
        }

        [TestMethod()]
        public void ReadNonStructuralData_wrap()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("> <LONG_PROPERTY>").Append('\n');
            sb.Append("This is a long property which should be wrapped when stored as field in an SDF D");
            sb.Append('\n');
            sb.Append("ata entry");
            sb.Append('\n');

            var input = new StringReader(sb.ToString());
            var mock = new Mock<IAtomContainer>();

            MDLV2000Reader.ReadNonStructuralData(input, mock.Object);

            mock.Verify(n => n.SetProperty("LONG_PROPERTY",
                    "This is a long property which should be wrapped when stored as field in an SDF Data entry"));

        }

        /// <summary>
        /// Ensure having a property with 2 new line lines will still allow 2 entries
        /// to be read - a bug from the mailing list.
        /// </summary>
        [TestMethod()]
        public void TestMultipleNewlinesInSDFProperty()
        {
            var ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.multiplenewline-property.sdf");
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IChemFile chemFile = reader.Read(new ChemFile());
            reader.Close();
            Assert.AreEqual(2, ChemFileManipulator.GetAllAtomContainers(chemFile).Count());
        }

        [TestMethod()]
        public void TestAliasAfterRgroup()
        {
            var ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.r-group-with-alias.mol");
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer container = reader.Read(new AtomContainer());
            reader.Close();
            Assert.IsInstanceOfType(container.Atoms[6], typeof(IPseudoAtom));
            Assert.AreEqual("R6", ((IPseudoAtom)container.Atoms[6]).Label);
            Assert.IsInstanceOfType(container.Atoms[7], typeof(IPseudoAtom));
            Assert.AreEqual("Protein", ((IPseudoAtom)container.Atoms[7]).Label);
        }

        [TestMethod()]
        public void V2000Version()
        {
            Assert.AreEqual(MDLV2000Reader.CTabVersion.V2000,
                MDLV2000Reader.CTabVersion.OfHeader("  5  5  0  0  0  0            999 V2000"));
            Assert.AreEqual(MDLV2000Reader.CTabVersion.V2000,
                MDLV2000Reader.CTabVersion.OfHeader("  5  5  0  0  0  0            999 v2000"));
        }

        [TestMethod()]
        public void V3000Version()
        {
            Assert.AreEqual(MDLV2000Reader.CTabVersion.V3000,
                MDLV2000Reader.CTabVersion.OfHeader("  0  0  0  0  0  0            999 V3000"));
            Assert.AreEqual(MDLV2000Reader.CTabVersion.V3000,
                MDLV2000Reader.CTabVersion.OfHeader("  0  0  0  0  0  0            999 v3000"));
        }

        [TestMethod()]
        public void UnspecVersion()
        {
            Assert.AreEqual(MDLV2000Reader.CTabVersion.Unspecified,
                MDLV2000Reader.CTabVersion.OfHeader("  5  5  0  0  0  0            999"));
            Assert.AreEqual(MDLV2000Reader.CTabVersion.Unspecified,
                MDLV2000Reader.CTabVersion.OfHeader("  5  5  0  0  0  0            999      "));
        }

        [TestMethod()]
        public void RadicalsReflectedInHydrogenCount()
        {
            IAtomContainer m;
            using (var s = ResourceLoader.GetAsStream("NCDK.IO.structure-with-radical.mol"))
            using (var r = new MDLV2000Reader(s))
            {
                m = r.Read(new AtomContainer());
            }
            Assert.AreEqual(8, m.Atoms[0].AtomicNumber);
            Assert.AreEqual(0, m.Atoms[0].ImplicitHydrogenCount);
        }

        // @cdk.bug 1326
        [TestMethod()]
        public void NonNegativeHydrogenCount()
        {
            using (var ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.ChEBI_30668.mol"))
            using (MDLV2000Reader reader = new MDLV2000Reader(ins))
            {
                var container = reader.Read(new AtomContainer());
                foreach (var atom in container.Atoms)
                {
                    Assert.IsTrue(0 <= atom.ImplicitHydrogenCount);
                    Assert.IsNotNull(atom.Valency);
                }
            }
        }

        // @cdk.bug 1343
        [TestMethod()]
        public void NonNegativeHydrogenCountOnHydrogenRadical()
        {
            var ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.ChEBI_29293.mol");
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer container = reader.Read(new AtomContainer());
            reader.Close();
            Assert.AreEqual(0, container.Atoms[0].ImplicitHydrogenCount);
            Assert.AreEqual(0, container.Atoms[1].ImplicitHydrogenCount);
        }

        /// <summary>
        /// The non-standard ACDLabs atom label property should throw a CDKException in Strict mode.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        public void TestAcdChemSketchLabel_Strict()
        {
            var filename = "NCDK.Data.MDL.chemsketch-all-labelled.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            reader.Read(new AtomContainer());
        }

        /// <summary>
        /// Test a simple ChemSketch label containing an integer.
        /// </summary>
        [TestMethod()]
        public void TestAcdChemSketchLabel()
        {
            var filename = "NCDK.Data.MDL.chemsketch-one-label.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer mol = reader.Read(new AtomContainer());
            reader.Close();

            Assert.AreEqual("6", mol.Atoms[1].GetProperty<string>(CDKPropertyName.ACDLabsAtomLabel));
        }

        /// <summary>
        /// Test ChemSketch labels containing all non-whitespace printable ASCII characters.
        /// </summary>
        [TestMethod()]
        public void TestAcdChemSketchLabel_PrintableAscii()
        {
            var filename = "NCDK.Data.MDL.chemsketch-printable-ascii.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer mol = reader.Read(new AtomContainer());
            reader.Close();

            // Printable ASCII characters, excluding whitespace. Note each string contains an atom number
            var expected = new string[]
            {
                "!\"#$%&'()*+,-./0123456789:;<=>?@[\\]^_`{|}~",
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ1abcdefghijklmnopqrstuvwxyz",
                "012345678901234567890123456789012345678901234567890"
            };
            Assert.AreEqual(expected[0], mol.Atoms[0].GetProperty<string>(CDKPropertyName.ACDLabsAtomLabel));
            Assert.AreEqual(expected[1], mol.Atoms[1].GetProperty<string>(CDKPropertyName.ACDLabsAtomLabel));
            Assert.AreEqual(expected[2], mol.Atoms[2].GetProperty<string>(CDKPropertyName.ACDLabsAtomLabel));
        }

        /// <summary>
        /// Check that multiple atom labels are all read.
        /// </summary>
        [TestMethod()]
        public void TestAcdChemSketchLabel_AllAtomsLabelled()
        {
            var filename = "NCDK.Data.MDL.chemsketch-all-labelled.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer mol = reader.Read(new AtomContainer());
            reader.Close();

            foreach (var atom in mol.Atoms)
            {
                Assert.IsNotNull(atom.GetProperty<string>(CDKPropertyName.ACDLabsAtomLabel));
            }
        }

        /// <summary>
        /// Check that leading and trailing whitespace in atom labels is preserved on reading.
        /// </summary>
        [TestMethod()]
        public void TestAcdChemSketchLabel_LeadingTrailingWhitespace()
        {
            var filename = "NCDK.Data.MDL.chemsketch-leading-trailing-space.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer mol = reader.Read(new AtomContainer());
            reader.Close();

            // Leading and trailing whitespace in both prefix and suffix
            var expected = " a 1 b ";
            Assert.AreEqual(expected, mol.Atoms[0].GetProperty<string>(CDKPropertyName.ACDLabsAtomLabel));
        }

        /// <summary>
        /// Check that embedded whitespace in atom labels is preserved on reading.
        /// </summary>
        [TestMethod()]
        public void TestAcdChemSketchLabel_EmbeddedWhitespace()
        {
            var filename = "NCDK.Data.MDL.chemsketch-embedded-space.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer mol = reader.Read(new AtomContainer());
            reader.Close();

            // Embedded whitespace in both prefix and suffix
            var expected = "a b1c d";
            Assert.AreEqual(expected, mol.Atoms[0].GetProperty<string>(CDKPropertyName.ACDLabsAtomLabel));
        }

        /// <summary>
        /// Check reading of largest permissible label (50 char prefix + 3 digits + 50 char suffix).
        /// </summary>
        [TestMethod()]
        public void TestAcdChemSketchLabel_MaxSizeLabel()
        {
            var filename = "NCDK.Data.MDL.chemsketch-longest-label.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer mol = reader.Read(new AtomContainer());
            reader.Close();

            // Longest allowed atom label is 103 characters
            var prefix = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwx";
            var digits = "999";
            var suffix = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwx";
            var expected = prefix + digits + suffix;

            Assert.AreEqual(expected, mol.Atoms[0].GetProperty<string>(CDKPropertyName.ACDLabsAtomLabel));
        }

        [TestMethod()]
        public void TestSgroupAbbreviation()
        {
            using (var srm = ResourceLoader.GetAsStream("NCDK.Data.MDL.sgroup-abbrv.mol"))
            using (MDLV2000Reader mdlr = new MDLV2000Reader(srm))
            {
                IAtomContainer container = mdlr.Read(new AtomContainer());
                var sgroups = container.GetProperty<IList<SGroup>>(CDKPropertyName.CtabSgroups);
                Assert.IsNotNull(sgroups);
                Assert.AreEqual(1, sgroups.Count);
                SGroup sgroup = sgroups[0];
                Assert.AreEqual(SGroupTypes.CtabAbbreviation, sgroup.Type);
                Assert.AreEqual("Cs2CO3", sgroup.Subscript);
                Assert.AreEqual(6, sgroup.Atoms.Count);
            }
        }

        [TestMethod()]
        public void TestSgroupRepeatUnit()
        {
            using (var srm = ResourceLoader.GetAsStream("NCDK.Data.MDL.sgroup-sru.mol"))
            using (MDLV2000Reader mdlr = new MDLV2000Reader(srm))
            {
                IAtomContainer container = mdlr.Read(new AtomContainer());
                IList<SGroup> sgroups = container.GetProperty<IList<SGroup>>(CDKPropertyName.CtabSgroups);
                Assert.IsNotNull(sgroups);
                Assert.AreEqual(1, sgroups.Count);
                SGroup sgroup = sgroups[0];
                Assert.AreEqual(SGroupTypes.CtabStructureRepeatUnit, sgroup.Type);
                Assert.AreEqual("n", sgroup.Subscript);
                Assert.AreEqual("HT", (string)sgroup.GetValue(SGroupKeys.CtabConnectivity));
                Assert.AreEqual(10, sgroup.Atoms.Count);
                Assert.AreEqual(2, sgroup.Bonds.Count);
                IList<SGroupBracket> brackets = (IList<SGroupBracket>)sgroup.GetValue(SGroupKeys.CtabBracket);
                Assert.AreEqual(2, brackets.Count);
                // M  SDI   1  4    2.2579   -0.8756    1.7735   -1.6600
                Assert.AreEqual(2.2579, brackets[0].FirstPoint.X, 0.001);
                Assert.AreEqual(-0.8756, brackets[0].FirstPoint.Y, 0.001);
                Assert.AreEqual(1.7735, brackets[0].SecondPoint.X, 0.001);
                Assert.AreEqual(-1.6600, brackets[0].SecondPoint.Y, 0.001);
                // M  SDI   1  4   -0.9910   -1.7247   -0.4960   -0.8673
                Assert.AreEqual(-0.9910, brackets[1].FirstPoint.X, 0.001);
                Assert.AreEqual(-1.7247, brackets[1].FirstPoint.Y, 0.001);
                Assert.AreEqual(-0.4960, brackets[1].SecondPoint.X, 0.001);
                Assert.AreEqual(-0.8673, brackets[1].SecondPoint.Y, 0.001);

            }
        }

        [TestMethod()]
        public void TestSgroupUnorderedMixture()
        {
            using (var srm = ResourceLoader.GetAsStream("NCDK.Data.MDL.sgroup-unord-mixture.mol"))
            using (MDLV2000Reader mdlr = new MDLV2000Reader(srm))
            {
                IAtomContainer container = mdlr.Read(new AtomContainer());
                IList<SGroup> sgroups = container.GetProperty<IList<SGroup>>(CDKPropertyName.CtabSgroups);
                Assert.IsNotNull(sgroups);
                Assert.AreEqual(3, sgroups.Count);
                // first sgroup
                SGroup sgroup = sgroups[0];
                Assert.AreEqual(SGroupTypes.CtabComponent, sgroup.Type);
                Assert.AreEqual(1, sgroup.Parents.Count);
                Assert.IsTrue(sgroup.Parents.Contains(sgroups[2]));
                // second sgroup
                sgroup = sgroups[1];
                Assert.AreEqual(SGroupTypes.CtabComponent, sgroup.Type);
                Assert.AreEqual(1, sgroup.Parents.Count);
                Assert.IsTrue(sgroup.Parents.Contains(sgroups[2]));
                // third sgroup
                sgroup = sgroups[2];
                Assert.AreEqual(SGroupTypes.CtabMixture, sgroup.Type);
                Assert.AreEqual(0, sgroup.Parents.Count);
            }
        }

        [TestMethod()]
        public void TestSgroupExpandedAbbreviation()
        {
            using (var srm = ResourceLoader.GetAsStream("NCDK.Data.MDL.triphenyl-phosphate-expanded.mol"))
            using (MDLV2000Reader mdlr = new MDLV2000Reader(srm))
            {
                IAtomContainer container = mdlr.Read(new AtomContainer());
                IList<SGroup> sgroups = container.GetProperty<IList<SGroup>>(CDKPropertyName.CtabSgroups);
                Assert.IsNotNull(sgroups);
                Assert.AreEqual(3, sgroups.Count);
                // first sgroup
                SGroup sgroup = sgroups[0];
                Assert.AreEqual(SGroupTypes.CtabAbbreviation, sgroup.Type);
                Assert.AreEqual("Ph", sgroup.Subscript);
                Assert.IsNotNull(sgroup.GetValue(SGroupKeys.CtabExpansion));
                // second sgroup
                sgroup = sgroups[1];
                Assert.AreEqual(SGroupTypes.CtabAbbreviation, sgroup.Type);
                Assert.AreEqual("Ph", sgroup.Subscript);
                Assert.IsNotNull(sgroup.GetValue(SGroupKeys.CtabExpansion));
                // third sgroup
                sgroup = sgroups[2];
                Assert.AreEqual(SGroupTypes.CtabAbbreviation, sgroup.Type);
                Assert.AreEqual("Ph", sgroup.Subscript);
                Assert.IsNotNull(sgroup.GetValue(SGroupKeys.CtabExpansion));
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        public void TestSgroupInvalidConnectInStrictMode()
        {
            using (var srm = ResourceLoader.GetAsStream("NCDK.Data.MDL.sgroup-sru-bad-scn.mol"))
            using (MDLV2000Reader mdlr = new MDLV2000Reader(srm))
            {
                mdlr.ReaderMode = ChemObjectReaderModes.Strict;
                IAtomContainer container = mdlr.Read(new AtomContainer());
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        public void TestSgroupDefOrderInStrictMode()
        {
            using (var srm = ResourceLoader.GetAsStream("NCDK.Data.MDL.sgroup-sru-bad-def.mol"))
            using (MDLV2000Reader mdlr = new MDLV2000Reader(srm))
            {
                mdlr.ReaderMode = ChemObjectReaderModes.Strict;
                IAtomContainer container = mdlr.Read(new AtomContainer());
            }
        }

        [TestMethod()]
        public void TestSgroupBracketStyle()
        {
            using (var srm = ResourceLoader.GetAsStream("NCDK.Data.MDL.sgroup-sru-bracketstyles.mol"))
            using (MDLV2000Reader mdlr = new MDLV2000Reader(srm))
            {
                IAtomContainer container = mdlr.Read(new AtomContainer());
                IList<SGroup> sgroups = container.GetProperty<IList<SGroup>>(CDKPropertyName.CtabSgroups);
                Assert.IsNotNull(sgroups);
                Assert.AreEqual(2, sgroups.Count);
                SGroup sgroup = sgroups[0];
                Assert.AreEqual(SGroupTypes.CtabStructureRepeatUnit, sgroup.Type);
                Assert.AreEqual(1, (int)sgroup.GetValue(SGroupKeys.CtabBracketStyle));
                sgroup = sgroups[1];
                Assert.AreEqual(SGroupTypes.CtabStructureRepeatUnit, sgroup.Type);
                Assert.AreEqual(1, (int)sgroup.GetValue(SGroupKeys.CtabBracketStyle));
            }
        }

        [TestMethod()]
        public void TestReading0DStereochemistry()
        {
            using (var srm = ResourceLoader.GetAsStream("NCDK.Data.MDL.tetrahedral-parity-withImplH.mol"))
            using (MDLV2000Reader mdlr = new MDLV2000Reader(srm))
            {
                IAtomContainer container = mdlr.Read(new AtomContainer());
                var selements = container.StereoElements;
                var siter = selements.GetEnumerator();
                Assert.IsTrue(siter.MoveNext());
                var se = siter.Current;
                Assert.IsInstanceOfType(se, typeof(ITetrahedralChirality));
                Assert.AreEqual(TetrahedralStereo.Clockwise, ((ITetrahedralChirality)se).Stereo);
                AssertAreEqual(
                    new IAtom[] { container.Atoms[1], container.Atoms[3], container.Atoms[4], container.Atoms[0] },
                    ((ITetrahedralChirality)se).Ligands);
                Assert.IsFalse(siter.MoveNext());
            }
        }

        // explicit Hydrogen can reverse winding
        [TestMethod()]
        public void TestReading0DStereochemistryWithHydrogen()
        {
            using (var srm = ResourceLoader.GetAsStream("NCDK.Data.MDL.tetrahedral-parity-withExpH.mol"))
            using (MDLV2000Reader mdlr = new MDLV2000Reader(srm))
            {
                IAtomContainer container = mdlr.Read(new AtomContainer());
                var selements = container.StereoElements;
                var siter = selements.GetEnumerator();
                Assert.IsTrue(siter.MoveNext());
                var se = siter.Current;
                Assert.IsInstanceOfType(se, typeof(ITetrahedralChirality));
                Assert.AreEqual(TetrahedralStereo.AntiClockwise, ((ITetrahedralChirality)se).Stereo);
                AssertAreEqual(
                    new IAtom[] { container.Atoms[0], container.Atoms[2], container.Atoms[3], container.Atoms[4] },
                    ((ITetrahedralChirality)se).Ligands);
                Assert.IsFalse(siter.MoveNext());
            }
        }

        /// <summary>
        /// When atomic mass is defined as a delta some atoms don't have a reasonable
        /// default. Most tools will output an 'M  ISO' property, so can be specified
        /// </summary>
        /// <exception cref="Exception">expected format error</exception>
        [TestMethod()]
        [ExpectedException(typeof(CDKException), AllowDerivedTypes = true)]
        public void SeaborgiumMassDelta()
        {
            using (var ins = ResourceLoader.GetAsStream(GetType(), "seaborgium.mol"))
            {
                MDLV2000Reader mdlr = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
                IAtomContainer mol = mdlr.Read(new AtomContainer());
            }
        }

        [TestMethod()]
        public void SeaborgiumAbsMass()
        {
            using (var ins = ResourceLoader.GetAsStream(GetType(), "seaborgium_abs.mol"))
            {
                MDLV2000Reader mdlr = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
                IAtomContainer mol = mdlr.Read(new AtomContainer());
                Assert.AreEqual(261, mol.Atoms[0].MassNumber);
            }
        }

        [TestMethod()]
        public void TestBadAtomCoordinateFormat()
        {
            const string mol = "\n" +
                 "\n" +
                 "\n" +
                 " 17 18  0  0  0  0              1 V2000\n" +
                 "  -2.31474   0.50167  -3.30968 C   0  0  0  0  0  0\n" +
                 "  -2.43523  -0.19508  -2.16895 C   0  0  0  0  0  0\n" +
                 "  -1.14825   1.44681  -3.13364 C   0  0  0  0  0  0\n" +
                 "  -1.34937   0.28780  -1.23468 C   0  0  0  0  0  0\n" +
                 "  -1.31596   1.76332  -1.64443 C   0  0  0  0  0  0\n" +
                 "   0.00411  -0.20225  -1.79002 C   0  0  0  0  0  0\n" +
                 "   0.14340   0.60304  -3.11198 C   0  0  0  0  0  0\n" +
                 "  -1.14836   2.30981  -3.79997 H   0  0  0  0  0  0\n" +
                 "   0.23461  -0.05636  -3.98172 H   0  0  0  0  0  0\n" +
                 "   1.03045   1.24486  -3.06787 H   0  0  0  0  0  0\n" +
                 "   0.82300   0.05318  -1.10813 H   0  0  0  0  0  0\n" +
                 "   0.02443  -1.28271  -1.96799 H   0  0  0  0  0  0\n" +
                 "  -1.53255   0.09851  -0.17666 H   0  0  0  0  0  0\n" +
                 "  -0.46658   2.31096  -1.22026 H   0  0  0  0  0  0\n" +
                 "  -2.24286   2.30318  -1.41306 H   0  0  0  0  0  0\n" +
                 "  -3.13662  -0.99036  -1.96785 H   0  0  0  0  0  0\n" +
                 "  -2.90004   0.37818  -4.20802 H   0  0  0  0  0  0\n" +
                 "  1  2  2  0  0  0\n" +
                 "  1  3  1  0  0  0\n" +
                 "  2  4  1  0  0  0\n" +
                 "  4  5  1  0  0  0\n" +
                 "  3  5  1  0  0  0\n" +
                 "  4  6  1  0  0  0\n" +
                 "  6  7  1  0  0  0\n" +
                 "  7  3  1  0  0  0\n" +
                 "  3  8  1  0  0  0\n" +
                 "  7  9  1  0  0  0\n" +
                 "  7 10  1  0  0  0\n" +
                 "  6 11  1  0  0  0\n" +
                 "  6 12  1  0  0  0\n" +
                 "  4 13  1  0  0  0\n" +
                 "  5 14  1  0  0  0\n" +
                 "  5 15  1  0  0  0\n" +
                 "  2 16  1  0  0  0\n" +
                 "  1 17  1  0  0  0\n" +
                 "M  END\n" +
                 "\n";
            var bytes = Encoding.UTF8.GetBytes(mol);
            MDLV2000Reader mdlv2000Reader = new MDLV2000Reader(new MemoryStream(bytes));
            mdlv2000Reader.ReaderMode = ChemObjectReaderModes.Relaxed;
            var atomContainer = mdlv2000Reader.Read(new Silent.AtomContainer());
            Assert.AreEqual(17, atomContainer.Atoms.Count);
        }
    }
}
