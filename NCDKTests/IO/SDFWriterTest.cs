/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;
using NCDK.IO.Listener;
using NCDK.Smiles;
using NCDK.Templates;
using System;
using System.Collections.Specialized;
using System.IO;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the writer MDL SD file writer.
    /// </summary>
    /// <seealso cref="SDFWriter"/>
    // @cdk.module test-io
    [TestClass()]
    public class SDFWriterTest : ChemObjectWriterTest
    {
        protected override Type ChemObjectIOToTestType => typeof(SDFWriter);
        private static IChemObjectBuilder builder = ChemObjectBuilder.Instance;

        [TestMethod()]
        public void TestAccepts()
        {
            SDFWriter reader = new SDFWriter(new StringWriter());
            Assert.IsTrue(reader.Accepts(typeof(ChemFile)));
            Assert.IsTrue(reader.Accepts(typeof(ChemModel)));
            Assert.IsTrue(reader.Accepts(typeof(ChemObjectSet<IAtomContainer>)));
        }

        [TestMethod()]
        public void TestWrite_IAtomContainerSet_Properties_Off()
        {
            StringWriter writer = new StringWriter();
            IChemObjectSet<IAtomContainer> molSet = new ChemObjectSet<IAtomContainer>();
            IAtomContainer molecule = new AtomContainer();
            molecule.Atoms.Add(new Atom("C"));
            molecule.SetProperty("foo", "bar");
            molSet.Add(molecule);

            SDFWriter sdfWriter = new SDFWriter(writer);
            var sdfWriterProps = new NameValueCollection();
            sdfWriterProps["writeProperties"] = "false";
            sdfWriter.Listeners.Add(new PropertiesListener(sdfWriterProps));
            sdfWriter.CustomizeJob();
            sdfWriter.Write(molSet);
            sdfWriter.Close();
            string result = writer.ToString();
            Assert.IsFalse(result.Contains("<foo>"));
        }

        // @cdk.bug 2827745
        [TestMethod()]
        public void TestWrite_IAtomContainerSet()
        {
            StringWriter writer = new StringWriter();
            IChemObjectSet<IAtomContainer> molSet = builder.NewAtomContainerSet();
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molSet.Add(molecule);

            SDFWriter sdfWriter = new SDFWriter(writer);
            sdfWriter.Write(molSet);
            sdfWriter.Close();
            Assert.AreNotSame(0, writer.ToString().Length);
        }

        [TestMethod()]
        public void TestWrite_IAtomContainerSet_Properties()
        {
            StringWriter writer = new StringWriter();
            IChemObjectSet<IAtomContainer> molSet = new ChemObjectSet<IAtomContainer>();
            IAtomContainer molecule = new AtomContainer();
            molecule.Atoms.Add(new Atom("C"));
            molecule.SetProperty("foo", "bar");
            molSet.Add(molecule);

            SDFWriter sdfWriter = new SDFWriter(writer);
            sdfWriter.Write(molSet);
            sdfWriter.Close();
            Assert.IsTrue(writer.ToString().IndexOf("<foo>") != -1);
            Assert.IsTrue(writer.ToString().IndexOf("bar") != -1);
        }

        [TestMethod()]
        public void TestWrite_IAtomContainerSet_CDKProperties()
        {
            StringWriter writer = new StringWriter();
            IChemObjectSet<IAtomContainer> molSet = new ChemObjectSet<IAtomContainer>();
            IAtomContainer molecule = new AtomContainer();
            molecule.Atoms.Add(new Atom("C"));
            molecule.SetProperty(InvPair.CanonicalLabelPropertyKey, "bar");
            molSet.Add(molecule);

            SDFWriter sdfWriter = new SDFWriter(writer);
            sdfWriter.Write(molSet);
            sdfWriter.Close();
            Assert.IsTrue(writer.ToString().IndexOf(InvPair.CanonicalLabelPropertyKey) == -1);
        }

        [TestMethod()]
        public void TestWrite_IAtomContainerSet_SingleMolecule()
        {
            StringWriter writer = new StringWriter();
            IChemObjectSet<IAtomContainer> molSet = new ChemObjectSet<IAtomContainer>();
            IAtomContainer molecule = new AtomContainer();
            molecule.Atoms.Add(new Atom("C"));
            molSet.Add(molecule);

            SDFWriter sdfWriter = new SDFWriter(writer);
            sdfWriter.Write(molSet);
            sdfWriter.Close();
            Assert.IsTrue(writer.ToString().IndexOf("$$$$") != -1);
        }

        [TestMethod()]
        public void TestWrite_IAtomContainerSet_MultIAtomContainer()
        {
            StringWriter writer = new StringWriter();
            IChemObjectSet<IAtomContainer> molSet = new ChemObjectSet<IAtomContainer>();
            IAtomContainer molecule = new AtomContainer();
            molecule.Atoms.Add(new Atom("C"));
            molSet.Add(molecule);
            molecule = new AtomContainer();
            molecule.Atoms.Add(new Atom("C"));
            molSet.Add(molecule);

            SDFWriter sdfWriter = new SDFWriter(writer);
            sdfWriter.Write(molSet);
            sdfWriter.Close();
            Assert.IsTrue(writer.ToString().IndexOf("$$$$") != -1);
        }

        [TestMethod()]
        public void TestWrite_IAtomContainer_MultIAtomContainer()
        {
            StringWriter writer = new StringWriter();
            SDFWriter sdfWriter = new SDFWriter(writer);

            IAtomContainer molecule = new AtomContainer();
            molecule.Atoms.Add(new Atom("C"));
            molecule.SetProperty("foo", "bar");
            sdfWriter.Write(molecule);

            molecule = new AtomContainer();
            molecule.Atoms.Add(new Atom("C"));
            molecule.SetProperty("toys", "r-us");
            sdfWriter.Write(molecule);

            sdfWriter.Close();
            Assert.IsTrue(writer.ToString().IndexOf("foo") != -1);
            Assert.IsTrue(writer.ToString().IndexOf("bar") != -1);
            Assert.IsTrue(writer.ToString().IndexOf("toys") != -1);
            Assert.IsTrue(writer.ToString().IndexOf("r-us") != -1);
            Assert.IsTrue(writer.ToString().IndexOf("$$$$") != -1);
        }

        [TestMethod()]
        public void InvalidSDfileHeaderTags()
        {
            StringWriter writer = new StringWriter();
            SDFWriter sdfWriter = new SDFWriter(writer);

            IAtomContainer molecule = new AtomContainer();
            molecule.Atoms.Add(new Atom("C"));
            molecule.SetProperty("http://not-valid.com", "URL");
            sdfWriter.Write(molecule);

            sdfWriter.Close();
            Assert.IsTrue(writer.ToString().Contains("> <http://not_valid_com>"));
        }

        [TestMethod()]
        public void ChooseFormatToWrite()
        {
            StringWriter writer = new StringWriter();
            SDFWriter sdfWriter = new SDFWriter(writer);

            IAtomContainer molecule = new AtomContainer();
            molecule.Atoms.Add(new Atom("CH4"));
            sdfWriter.Write(molecule);

            molecule = new AtomContainer();
            for (int i = 0; i < 1000; i++)
                molecule.Atoms.Add(new Atom("CH4"));
            sdfWriter.Write(molecule);

            molecule = new AtomContainer();
            molecule.Atoms.Add(new Atom("CH4"));
            sdfWriter.Write(molecule);

            sdfWriter.Close();
            string result = writer.ToString();
            Assert.IsTrue(result.Contains("V2000"));
            Assert.IsTrue(result.Contains("V3000"));
        }

        [TestMethod()]
        public void ChooseFormatToWrite2()
        {
            StringWriter writer = new StringWriter();
            SDFWriter sdfWriter = new SDFWriter(writer);
            sdfWriter.SetAlwaysV3000(true);

            IAtomContainer molecule = new AtomContainer();
            molecule.Atoms.Add(new Atom("CH4"));
            sdfWriter.Write(molecule);

            molecule = new AtomContainer();
            for (int i = 0; i < 1000; i++)
                molecule.Atoms.Add(new Atom("CH4"));
            sdfWriter.Write(molecule);

            molecule = new AtomContainer();
            molecule.Atoms.Add(new Atom("CH4"));
            sdfWriter.Write(molecule);

            sdfWriter.Close();
            string result = writer.ToString();
            Assert.IsFalse(result.Contains("V2000"));
            Assert.IsTrue(result.Contains("V3000"));
        }

        // @cdk.bug 3392485
        [TestMethod()]
        public void TestIOPropPropagation()
        {
            IAtomContainer mol = TestMoleculeFactory.MakeBenzene();
            foreach (var atom in mol.Atoms)
            {
                atom.IsAromatic = true;
            }
            foreach (var bond in mol.Bonds)
            {
                bond.IsAromatic = true;
            }

            StringWriter strWriter = new StringWriter();
            SDFWriter writer = new SDFWriter(strWriter);

            var sdfWriterProps = new NameValueCollection();
            sdfWriterProps["WriteAromaticBondTypes"] = "true";
            writer.Listeners.Add(new PropertiesListener(sdfWriterProps));
            writer.CustomizeJob();
            writer.Write(mol);
            writer.Close();

            string output = strWriter.ToString();
            Assert.IsTrue(output.Contains("4  0  0  0  0"));
        }

        [TestMethod()]
        public void TestPropertyOutput_All()
        {
            IAtomContainer adenine = TestMoleculeFactory.MakeAdenine();
            StringWriter sw = new StringWriter();
            SDFWriter sdf = new SDFWriter(sw);
            adenine.SetProperty("one", "a");
            adenine.SetProperty("two", "b");
            sdf.Write(adenine);
            sdf.Close();
            string output = sw.ToString();
            Assert.IsTrue(output.Contains("> <one>"));
            Assert.IsTrue(output.Contains("> <two>"));
        }

        [TestMethod()]
        public void TestPropertyOutput_one()
        {
            IAtomContainer adenine = TestMoleculeFactory.MakeAdenine();
            StringWriter sw = new StringWriter();
            SDFWriter sdf = new SDFWriter(sw, new[] { "one" });
            adenine.SetProperty("one", "a");
            adenine.SetProperty("two", "b");
            sdf.Write(adenine);
            sdf.Close();
            string output = sw.ToString();
            Assert.IsTrue(output.Contains("> <one>"));
            Assert.IsFalse(output.Contains("> <two>"));
        }

        [TestMethod()]
        public void TestPropertyOutput_two()
        {
            IAtomContainer adenine = TestMoleculeFactory.MakeAdenine();
            StringWriter sw = new StringWriter();
            SDFWriter sdf = new SDFWriter(sw, new[] { "two" });
            adenine.SetProperty("one", "a");
            adenine.SetProperty("two", "b");
            sdf.Write(adenine);
            sdf.Close();
            string output = sw.ToString();
            Assert.IsTrue(output.Contains("> <two>"));
            Assert.IsFalse(output.Contains("> <one>"));
        }

        [TestMethod()]
        public void TestPropertyOutput_none()
        {
            IAtomContainer adenine = TestMoleculeFactory.MakeAdenine();
            StringWriter sw = new StringWriter();
            SDFWriter sdf = new SDFWriter(sw, Array.Empty<string>());
            adenine.SetProperty("one", "a");
            adenine.SetProperty("two", "b");
            sdf.Write(adenine);
            sdf.Close();
            string output = sw.ToString();
            Assert.IsFalse(output.Contains("> <two>"));
            Assert.IsFalse(output.Contains("> <one>"));
        }
    }
}
