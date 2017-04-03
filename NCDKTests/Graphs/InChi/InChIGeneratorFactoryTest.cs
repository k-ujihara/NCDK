/* Copyright (C)      2006  Sam Adams <sea36@users.sf.net>
 *                    2007  Rajarshi Guha <rajarshi.guha@gmail.com>
 *               2010,2012  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Aromaticities;
using NCDK.Default;
using NCDK.Smiles;
using NCDK.Templates;
using NCDK.Tools.Manipulator;
using NCDK.NInChI;
using System;
using System.Collections.Generic;

namespace NCDK.Graphs.InChI
{
    /// <summary>
    // @cdk.module test-inchi
    /// </summary>
    [TestClass()]
    public class InChIGeneratorFactoryTest
    {

        [TestMethod()]
        public void TestInstance()
        {
            InChIGeneratorFactory factory = InChIGeneratorFactory.Instance;
            Assert.IsNotNull(factory);
        }

        /// <summary>
        /// Because we are not setting any options, we get an Standard InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInChIGenerator_IAtomContainer()
        {
            IAtomContainer ac = new AtomContainer();
            IAtom a = new Atom("Cl");
            a.ImplicitHydrogenCount = 1;
            ac.Atoms.Add(a);
            InChIGenerator gen = InChIGeneratorFactory.Instance.GetInChIGenerator(ac);
            Assert.AreEqual(gen.ReturnStatus, INCHI_RET.OKAY);
            Assert.AreEqual("InChI=1S/ClH/h1H", gen.InChI);
        }

        /// <summary>
        /// Because we are setting an options, we get a non-standard InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInChIGenerator_IAtomContainer_String()
        {
            IAtomContainer ac = new AtomContainer();
            IAtom a = new Atom("Cl");
            a.ImplicitHydrogenCount = 1;
            ac.Atoms.Add(a);
            InChIGenerator gen = InChIGeneratorFactory.Instance.GetInChIGenerator(ac, "FixedH");
            Assert.AreEqual(gen.ReturnStatus, INCHI_RET.OKAY);
            Assert.AreEqual("InChI=1/ClH/h1H", gen.InChI);
        }

        /// <summary>
        /// Because we are setting no option, we get a Standard InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInChIGenerator_IAtomContainer_NullString()
        {
            IAtomContainer ac = new AtomContainer();
            IAtom a = new Atom("Cl");
            a.ImplicitHydrogenCount = 1;
            ac.Atoms.Add(a);
            InChIGenerator gen = InChIGeneratorFactory.Instance.GetInChIGenerator(ac, (string)null);
            Assert.AreEqual(gen.ReturnStatus, INCHI_RET.OKAY);
            Assert.AreEqual("InChI=1S/ClH/h1H", gen.InChI);
        }

        /// <summary>
        /// Because we are setting an options, we get a non-standard InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInChIGenerator_IAtomContainer_List()
        {
            IAtomContainer ac = new AtomContainer();
            IAtom a = new Atom("Cl");
            a.ImplicitHydrogenCount = 1;
            ac.Atoms.Add(a);
            List<INCHI_OPTION> options = new List<INCHI_OPTION>();
            options.Add(INCHI_OPTION.FixedH);
            InChIGenerator gen = InChIGeneratorFactory.Instance.GetInChIGenerator(ac, options);
            Assert.AreEqual(gen.ReturnStatus, INCHI_RET.OKAY);
            Assert.AreEqual("InChI=1/ClH/h1H", gen.InChI);
        }

        /// <summary>
        /// Because we are setting an options, we get a non-standard InChI.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetInChIGenerator_IAtomContainer_NullList()
        {
            IAtomContainer ac = new AtomContainer();
            IAtom a = new Atom("Cl");
            a.ImplicitHydrogenCount = 1;
            ac.Atoms.Add(a);
            InChIGeneratorFactory.Instance.GetInChIGenerator(ac, (List<INCHI_OPTION>)null);
        }

        [TestMethod()]
        public void TestGetInChIToStructure_String_IChemObjectBuilder()
        {
            InChIToStructure parser = InChIGeneratorFactory.Instance.GetInChIToStructure("InChI=1/ClH/h1H",
                    Default.ChemObjectBuilder.Instance);
            Assert.IsNotNull(parser);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetInChIToStructure_String_IChemObjectBuilder_NullString()
        {
            InChIGeneratorFactory.Instance.GetInChIToStructure("InChI=1/ClH/h1H",
                    Default.ChemObjectBuilder.Instance, (string)null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetInChIToStructure_String_IChemObjectBuilder_NullList()
        {
            InChIGeneratorFactory.Instance.GetInChIToStructure("InChI=1/ClH/h1H",
                    Default.ChemObjectBuilder.Instance, (List<string>)null);
        }

        /// <summary>
        /// No options set.
        /// </summary>
        [TestMethod()]
        public void TestGetInChIToStructure_String_IChemObjectBuilder_List()
        {
            InChIToStructure parser = InChIGeneratorFactory.Instance.GetInChIToStructure("InChI=1/ClH/h1H",
                    Default.ChemObjectBuilder.Instance, new List<string>());
            Assert.IsNotNull(parser);
        }

        [TestMethod()]
        public void TestSMILESConversion_TopologicalCentre()
        {

            // (2R,3R,4S,5R,6S)-3,5-dimethylheptane-2,4,6-triol
            SmilesParser parser = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer container = parser.ParseSmiles("C[C@@H](O)[C@@H](C)[C@@H](O)[C@H](C)[C@H](C)O");

            InChIGenerator generator = InChIGeneratorFactory.Instance.GetInChIGenerator(container);

            string expected = "InChI=1S/C9H20O3/c1-5(7(3)10)9(12)6(2)8(4)11/h5-12H,1-4H3/t5-,6-,7-,8+,9-/m1/s1";
            string actual = generator.InChI;

            Assert.AreEqual(expected, actual, "Incorrect InCHI generated for topological centre");

        }

        /// <summary>
        /// Tests the aromatic bonds option in the InChI factory class.
        /// </summary>
        [TestMethod()]
        public void TestInChIGenerator_AromaticBonds()
        {

            try
            {
                // create a fairly complex aromatic molecule
                IAtomContainer tetrazole = TestMoleculeFactory.MakeTetrazole();
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(tetrazole);
                Aromaticity.CDKLegacy.Apply(tetrazole);

                InChIGeneratorFactory inchiFactory = InChIGeneratorFactory.Instance;
                inchiFactory.IgnoreAromaticBonds = false;

                // include aromatic bonds by default
                InChIGenerator genAromaticity1 = inchiFactory.GetInChIGenerator(tetrazole);

                // exclude aromatic bonds
                Assert.IsFalse(inchiFactory.IgnoreAromaticBonds);
                inchiFactory.IgnoreAromaticBonds = true;
                Assert.IsTrue(inchiFactory.IgnoreAromaticBonds);
                InChIGenerator genNoAromaticity = inchiFactory.GetInChIGenerator(tetrazole);

                // include aromatic bonds again
                inchiFactory.IgnoreAromaticBonds = false;
                Assert.IsFalse(inchiFactory.IgnoreAromaticBonds);
                InChIGenerator genAromaticity2 = inchiFactory.GetInChIGenerator(tetrazole);

                // with the aromatic bonds included, no InChI can be generated
                Assert.AreEqual(INCHI_RET.ERROR, genAromaticity1.ReturnStatus, "return status was not in error");
                Assert.AreEqual(INCHI_RET.ERROR, genAromaticity2.ReturnStatus, "return status was not in error");
                // excluding the aromatic bonds gives the normal InChI
                Assert.AreEqual(INCHI_RET.OKAY, genNoAromaticity.ReturnStatus, "return status was not okay");
                Assert.AreEqual("InChI=1S/CH2N4/c1-2-4-5-3-1/h1H,(H,2,3,4,5)",
                        genNoAromaticity.InChI, "InChIs did not match");
            }
            finally
            {
                InChIGeneratorFactory.Instance.IgnoreAromaticBonds = true;
            }
        }
    }
}
