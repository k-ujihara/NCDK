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
using NCDK.Silent;
using NCDK.Smiles;
using NCDK.Templates;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;

namespace NCDK.Graphs.InChI
{
    // @cdk.module test-inchi
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
            var ac = new AtomContainer();
            var a = new Atom("Cl") { ImplicitHydrogenCount = 1 };
            ac.Atoms.Add(a);
            var gen = InChIGeneratorFactory.Instance.GetInChIGenerator(ac);
            Assert.AreEqual(gen.ReturnStatus, InChIReturnCode.Ok);
            Assert.AreEqual("InChI=1S/ClH/h1H", gen.InChI);
        }

        /// <summary>
        /// Because we are setting an options, we get a non-standard InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInChIGenerator_IAtomContainer_String()
        {
            var ac = new AtomContainer();
            var a = new Atom("Cl") { ImplicitHydrogenCount = 1 };
            ac.Atoms.Add(a);
            var gen = InChIGeneratorFactory.Instance.GetInChIGenerator(ac, "FixedH");
            Assert.AreEqual(gen.ReturnStatus, InChIReturnCode.Ok);
            Assert.AreEqual("InChI=1/ClH/h1H", gen.InChI);
        }

        /// <summary>
        /// Because we are setting no option, we get a Standard InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInChIGenerator_IAtomContainer_NullString()
        {
            var ac = new AtomContainer();
            var a = new Atom("Cl") { ImplicitHydrogenCount = 1 };
            ac.Atoms.Add(a);
            var gen = InChIGeneratorFactory.Instance.GetInChIGenerator(ac, (string)null);
            Assert.AreEqual(gen.ReturnStatus, InChIReturnCode.Ok);
            Assert.AreEqual("InChI=1S/ClH/h1H", gen.InChI);
        }

        /// <summary>
        /// Because we are setting an options, we get a non-standard InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInChIGenerator_IAtomContainer_List()
        {
            var ac = new AtomContainer();
            var a = new Atom("Cl") { ImplicitHydrogenCount = 1 };
            ac.Atoms.Add(a);
            var options = new List<InChIOption> { InChIOption.FixedH };
            var gen = InChIGeneratorFactory.Instance.GetInChIGenerator(ac, options);
            Assert.AreEqual(gen.ReturnStatus, InChIReturnCode.Ok);
            Assert.AreEqual("InChI=1/ClH/h1H", gen.InChI);
        }

        /// <summary>
        /// Because we are setting an options, we get a non-standard InChI.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetInChIGenerator_IAtomContainer_NullList()
        {
            var ac = new AtomContainer();
            var a = new Atom("Cl") { ImplicitHydrogenCount = 1 };
            ac.Atoms.Add(a);
            InChIGeneratorFactory.Instance.GetInChIGenerator(ac, (List<InChIOption>)null);
        }

        [TestMethod()]
        public void TestGetInChIToStructure_String_IChemObjectBuilder()
        {
            var parser = InChIToStructure.FromInChI("InChI=1/ClH/h1H", ChemObjectBuilder.Instance);
            Assert.IsNotNull(parser);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetInChIToStructure_String_IChemObjectBuilder_NullString()
        {
            InChIToStructure.FromInChI("InChI=1/ClH/h1H", ChemObjectBuilder.Instance, (string)null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetInChIToStructure_String_IChemObjectBuilder_NullList()
        {
            InChIToStructure.FromInChI("InChI=1/ClH/h1H", ChemObjectBuilder.Instance, (List<string>)null);
        }

        /// <summary>
        /// No options set.
        /// </summary>
        [TestMethod()]
        public void TestGetInChIToStructure_String_IChemObjectBuilder_List()
        {
            var parser = InChIToStructure.FromInChI("InChI=1/ClH/h1H", ChemObjectBuilder.Instance, new List<string>());
            Assert.IsNotNull(parser);
        }

        [TestMethod()]
        public void TestSMILESConversion_TopologicalCentre()
        {
            // (2R,3R,4S,5R,6S)-3,5-dimethylheptane-2,4,6-triol
            var parser = CDK.SmilesParser;
            var container = parser.ParseSmiles("C[C@@H](O)[C@@H](C)[C@@H](O)[C@H](C)[C@H](C)O");

            InChIGenerator generator = InChIGeneratorFactory.Instance.GetInChIGenerator(container);

            string expected = "InChI=1S/C9H20O3/c1-5(7(3)10)9(12)6(2)8(4)11/h5-12H,1-4H3/t5-,6-,7-,8+,9-/m1/s1";
            string actual = generator.InChI;

            Assert.AreEqual(expected, actual, "Incorrect InCHI generated for topological centre");
        }

        [TestMethod()]
        public void DontIgnoreMajorIsotopes()
        {
            var smipar = CDK.SmilesParser;
            var inchifact = InChIGeneratorFactory.Instance;
            Assert.IsTrue(inchifact.GetInChIGenerator(smipar.ParseSmiles("[12CH4]")).InChI.Contains("/i"));
            Assert.IsFalse(inchifact.GetInChIGenerator(smipar.ParseSmiles("C")).InChI.Contains("/i"));
        }

        // InChI only supports cumulenes of length 2 (CC=[C@]=CC) and 3
        // (C/C=C=C=C=C/C) longer ones should be ignored
        [TestMethod()]
        public void LongerExtendedTetrahedralsIgnored()
        {
            var smipar = CDK.SmilesParser;
            var mol = smipar.ParseSmiles("CC=C=C=[C@]=C=C=CC");
            var gen = InChIGeneratorFactory.Instance.GetInChIGenerator(mol);
            Assert.AreEqual(InChIReturnCode.Ok, gen.ReturnStatus);
            Assert.AreEqual("InChI=1S/C9H8/c1-3-5-7-9-8-6-4-2/h3-4H,1-2H3", gen.InChI);
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
                foreach (IAtom atom in tetrazole.Atoms)
                    atom.ImplicitHydrogenCount = null;
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
                Assert.AreEqual(InChIReturnCode.Error, genAromaticity1.ReturnStatus, "return status was not in error");
                Assert.AreEqual(InChIReturnCode.Error, genAromaticity2.ReturnStatus, "return status was not in error");
                // excluding the aromatic bonds gives the normal InChI
                Assert.AreEqual(InChIReturnCode.Ok, genNoAromaticity.ReturnStatus, "return status was not okay");
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
