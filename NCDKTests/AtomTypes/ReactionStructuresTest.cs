/* Copyright (C) 2008  Miguel Rojas <miguelrojasch@yahoo.es>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.Tools;
using NCDK.Tools.Manipulator;

namespace NCDK.AtomTypes
{
    /// <summary>
    // @cdk.module test-reaction
    /// </summary>
    [TestClass()]
    public class ReactionStructuresTest : CDKTestCase
    {

        private readonly static IChemObjectBuilder builder;
        private readonly static CDKAtomTypeMatcher matcher;

        static ReactionStructuresTest()
        {
            builder = Silent.ChemObjectBuilder.Instance;
            matcher = CDKAtomTypeMatcher.GetInstance(builder);
        }

        /// <summary>
        /// Constructor of the ReactionStructuresTest.
        /// </summary>
        public ReactionStructuresTest()
            : base()
        { }

        /// <summary>
        /// A unit test suite for JUnit. Compound and its fragments to be tested
        // @throws Exception
        ///
        // @see       SharingChargeDBReactionTest#TestAtomTypesMolecule1()
        /// </summary>
        [TestMethod()]
        public void TestM0()
        {

            //COMPOUND
            //[C*]=C-C
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.SingleElectrons.Add(new SingleElectron(molecule.Atoms[0]));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[3], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[4], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[5], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[6], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[7], BondOrder.Single);

            string[] expectedTypes = { "C.radical.sp2", "C.sp2", "C.sp3", "H", "H", "H", "H", "H" };
            Assert.AreEqual(expectedTypes.Length, molecule.Atoms.Count);
            for (int i = 0; i < expectedTypes.Length; i++)
            {
                IAtom nextAtom = molecule.Atoms[i];
                IAtomType perceivedType = matcher.FindMatchingAtomType(molecule, nextAtom);
                Assert.IsNotNull(perceivedType, "Missing atom type for: " + nextAtom + " " + i + " expected: " + expectedTypes[i]);
                Assert.AreEqual(expectedTypes[i], perceivedType.AtomTypeName, "Incorrect atom type perceived for: " + nextAtom);
            }
        }

        /// <summary>
        /// A unit test suite for JUnit. Compound and its fragments to be tested
        // @throws Exception
        ///
        // @see       HeterolyticCleavageSBReactionTest#TestCspSingleB()
        /// </summary>
        [TestMethod()]
        public void TestM4()
        {
            //CreateFromSmiles("C#[C+]")
            IAtomContainer expected1 = builder.NewAtomContainer();
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.Atoms[1].FormalCharge = +1;
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Triple);
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "C.sp", "C.plus.sp1", "H" };
            Assert.AreEqual(expectedTypes.Length, expected1.Atoms.Count);
            for (int i = 0; i < expectedTypes.Length; i++)
            {
                IAtom nextAtom = expected1.Atoms[i];
                IAtomType perceivedType = matcher.FindMatchingAtomType(expected1, nextAtom);
                Assert.IsNotNull(perceivedType, "Missing atom type for: " + nextAtom + " " + i + " expected: " + expectedTypes[i]);
                Assert.AreEqual(expectedTypes[i], perceivedType.AtomTypeName, "Incorrect atom type perceived for: " + nextAtom);
            }

        }

        /// <summary>
        /// A unit test suite for JUnit. Compound and its fragments to be tested
        // @throws Exception
        ///
        // @see       HomolyticCleavageReactionTest#TestCsp2SingleB()
        /// </summary>
        [TestMethod()]
        public void TestM5()
        {
            //CreateFromSmiles("C=[C*]")
            IAtomContainer expected1 = builder.NewAtomContainer();
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.SingleElectrons.Add(builder.NewSingleElectron(expected1.Atoms[1]));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Double);
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[2], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[3], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "C.sp2", "C.radical.sp2", "H", "H", "H" };
            Assert.AreEqual(expectedTypes.Length, expected1.Atoms.Count);
            for (int i = 0; i < expectedTypes.Length; i++)
            {
                IAtom nextAtom = expected1.Atoms[i];
                IAtomType perceivedType = matcher.FindMatchingAtomType(expected1, nextAtom);
                Assert.IsNotNull(perceivedType, "Missing atom type for: " + nextAtom + " " + i + " expected: " + expectedTypes[i]);
                Assert.AreEqual(expectedTypes[i], perceivedType.AtomTypeName, "Incorrect atom type perceived for: " + nextAtom);
            }

        }

        /// <summary>
        /// A unit test suite for JUnit. Compound and its fragments to be tested
        // @throws Exception
        ///
        // @see       HomolyticCleavageReactionTest#TestCsp2SingleB()
        /// </summary>
        [TestMethod()]
        public void TestM6()
        {
            //CreateFromSmiles("C#[C*]")
            IAtomContainer expected1 = builder.NewAtomContainer();
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.SingleElectrons.Add(builder.NewSingleElectron(expected1.Atoms[1]));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Triple);
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "C.sp", "C.radical.sp1", "H" };
            Assert.AreEqual(expectedTypes.Length, expected1.Atoms.Count);
            for (int i = 0; i < expectedTypes.Length; i++)
            {
                IAtom nextAtom = expected1.Atoms[i];
                IAtomType perceivedType = matcher.FindMatchingAtomType(expected1, nextAtom);
                Assert.IsNotNull(perceivedType, "Missing atom type for: " + nextAtom + " " + i + " expected: " + expectedTypes[i]);
                Assert.AreEqual(expectedTypes[i], perceivedType.AtomTypeName, "Incorrect atom type perceived for: " + nextAtom);
            }
        }

        /// <summary>
        /// A unit test suite for JUnit. Compound and its fragments to be tested
        // @throws Exception
        ///
        // @see       HomolyticCleavageReactionTest#TestCsp2DoubleB()
        /// </summary>
        [TestMethod()]
        public void TestM7()
        {
            //CreateFromSmiles("C[C*][C*]")
            IAtomContainer expected1 = builder.NewAtomContainer();
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.SingleElectrons.Add(builder.NewSingleElectron(expected1.Atoms[1]));
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.SingleElectrons.Add(builder.NewSingleElectron(expected1.Atoms[2]));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[2], BondOrder.Single);
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[3], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[4], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[5], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[6], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[2], expected1.Atoms[7], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[2], expected1.Atoms[8], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "C.radical.planar", "C.radical.planar", "H", "H", "H", "H", "H", "H" };
            Assert.AreEqual(expectedTypes.Length, expected1.Atoms.Count);
            for (int i = 0; i < expectedTypes.Length; i++)
            {
                IAtom nextAtom = expected1.Atoms[i];
                IAtomType perceivedType = matcher.FindMatchingAtomType(expected1, nextAtom);
                Assert.IsNotNull(perceivedType, "Missing atom type for: " + nextAtom + " " + i + " expected: " + expectedTypes[i]);
                Assert.AreEqual(expectedTypes[i], perceivedType.AtomTypeName, "Incorrect atom type perceived for: " + nextAtom);
            }
        }

        /// <summary>
        /// A unit test suite for JUnit. Compound and its fragments to be tested
        // @throws Exception
        ///
        // @see       HomolyticCleavageReactionTest#TestCspDoubleB()
        /// </summary>
        [TestMethod()]
        public void TestM8()
        {
            //CreateFromSmiles("C=[C*][C*]")
            IAtomContainer expected1 = builder.NewAtomContainer();
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.SingleElectrons.Add(builder.NewSingleElectron(expected1.Atoms[1]));
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.SingleElectrons.Add(builder.NewSingleElectron(expected1.Atoms[2]));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Double);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[2], BondOrder.Single);
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[3], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[4], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[2], expected1.Atoms[5], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[2], expected1.Atoms[6], BondOrder.Single);

            string[] expectedTypes = { "C.sp2", "C.radical.sp2", "C.radical.planar", "H", "H", "H", "H" };
            Assert.AreEqual(expectedTypes.Length, expected1.Atoms.Count);
            for (int i = 0; i < expectedTypes.Length; i++)
            {
                IAtom nextAtom = expected1.Atoms[i];
                IAtomType perceivedType = matcher.FindMatchingAtomType(expected1, nextAtom);
                Assert.IsNotNull(perceivedType, "Missing atom type for: " + nextAtom + " " + i + " expected: " + expectedTypes[i]);
                Assert.AreEqual(expectedTypes[i], perceivedType.AtomTypeName, "Incorrect atom type perceived for: " + nextAtom);
            }
        }

        /// <summary>
        /// A unit test suite for JUnit. Compound and its fragments to be tested
        // @throws Exception
        ///
        // @see       HomolyticCleavageReactionTest#TestNsp3SingleB()
        /// </summary>
        [TestMethod()]
        public void TestM9()
        {
            //CreateFromSmiles("C[N*]")
            IAtomContainer expected1 = builder.NewAtomContainer();
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.Atoms.Add(builder.NewAtom("N"));
            expected1.SingleElectrons.Add(builder.NewSingleElectron(expected1.Atoms[1]));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Single);
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[2], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[3], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[4], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[5], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "N.sp3.radical", "H", "H", "H", "H" };
            Assert.AreEqual(expectedTypes.Length, expected1.Atoms.Count);
            for (int i = 0; i < expectedTypes.Length; i++)
            {
                IAtom nextAtom = expected1.Atoms[i];
                IAtomType perceivedType = matcher.FindMatchingAtomType(expected1, nextAtom);
                Assert.IsNotNull(perceivedType, "Missing atom type for: " + nextAtom + " " + i + " expected: " + expectedTypes[i]);
                Assert.AreEqual(expectedTypes[i], perceivedType.AtomTypeName, "Incorrect atom type perceived for: " + nextAtom);
            }
        }

        /// <summary>
        /// A unit test suite for JUnit. Compound and its fragments to be tested
        // @throws Exception
        ///
        // @see       HomolyticCleavageReactionTest#TestNsp2SingleB()
        /// </summary>
        [TestMethod()]
        public void TestM10()
        {
            //CreateFromSmiles("C=[N*]")
            IAtomContainer expected1 = builder.NewAtomContainer();
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.Atoms.Add(builder.NewAtom("N"));
            expected1.SingleElectrons.Add(builder.NewSingleElectron(expected1.Atoms[1]));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Double);
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[2], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "C.sp2", "N.sp2.radical", "H", "H" };
            Assert.AreEqual(expectedTypes.Length, expected1.Atoms.Count);
            for (int i = 0; i < expectedTypes.Length; i++)
            {
                IAtom nextAtom = expected1.Atoms[i];
                IAtomType perceivedType = matcher.FindMatchingAtomType(expected1, nextAtom);
                Assert.IsNotNull(perceivedType, "Missing atom type for: " + nextAtom + " " + i + " expected: " + expectedTypes[i]);
                Assert.AreEqual(expectedTypes[i], perceivedType.AtomTypeName, "Incorrect atom type perceived for: " + nextAtom);
            }
        }

        /// <summary>
        /// A unit test suite for JUnit. Compound and its fragments to be tested
        // @throws Exception
        ///
        // @see       HomolyticCleavageReactionTest#TestOsp2SingleB()
        /// </summary>
        [TestMethod()]
        public void TestM13()
        {
            //CreateFromSmiles("[O+*][C*]")
            IAtomContainer expected1 = builder.NewAtomContainer();
            expected1.Atoms.Add(builder.NewAtom("O"));
            expected1.Atoms[0].FormalCharge = 1;
            expected1.SingleElectrons.Add(builder.NewSingleElectron(expected1.Atoms[0]));
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.SingleElectrons.Add(builder.NewSingleElectron(expected1.Atoms[1]));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Single);
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[2], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "O.plus.radical", "C.radical.planar", "H", "H" };
            Assert.AreEqual(expectedTypes.Length, expected1.Atoms.Count);
            for (int i = 0; i < expectedTypes.Length; i++)
            {
                IAtom nextAtom = expected1.Atoms[i];
                IAtomType perceivedType = matcher.FindMatchingAtomType(expected1, nextAtom);
                Assert.IsNotNull(perceivedType, "Missing atom type for: " + nextAtom + " " + i + " expected: " + expectedTypes[i]);
                Assert.AreEqual(expectedTypes[i], perceivedType.AtomTypeName, "Incorrect atom type perceived for: " + nextAtom);
            }
        }

        /// <summary>
        /// A unit test suite for JUnit. Compound and its fragments to be tested
        // @throws Exception
        ///
        // @see       HomolyticCleavageReactionTest#TestFspSingleB()
        /// </summary>
        [TestMethod()]
        public void TestM14()
        {
            //CreateFromSmiles("[F*]")
            IAtomContainer expected1 = builder.NewAtomContainer();
            expected1.Atoms.Add(builder.NewAtom("F"));
            expected1.SingleElectrons.Add(builder.NewSingleElectron(expected1.Atoms[0]));

            string[] expectedTypes = { "F.radical" };
            Assert.AreEqual(expectedTypes.Length, expected1.Atoms.Count);
            for (int i = 0; i < expectedTypes.Length; i++)
            {
                IAtom nextAtom = expected1.Atoms[i];
                IAtomType perceivedType = matcher.FindMatchingAtomType(expected1, nextAtom);
                Assert.IsNotNull(perceivedType, "Missing atom type for: " + nextAtom + " " + i + " expected: " + expectedTypes[i]);
                Assert.AreEqual(expectedTypes[i], perceivedType.AtomTypeName, "Incorrect atom type perceived for: " + nextAtom);
            }
        }

        /// <summary>
        /// A unit test suite for JUnit. Compound and its fragments to be tested
        // @throws Exception
        ///
        // @see       HomolyticCleavageReactionTest#TestOsp2SingleB()
        /// </summary>
        [TestMethod()]
        public void TestM15()
        {
            //CreateFromSmiles("C[O*]")
            IAtomContainer expected1 = builder.NewAtomContainer();
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.Atoms.Add(builder.NewAtom("O"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Single);
            expected1.SingleElectrons.Add(builder.NewSingleElectron(expected1.Atoms[1]));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[2], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[3], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "O.sp3.radical", "H", "H", "H" };
            Assert.AreEqual(expectedTypes.Length, expected1.Atoms.Count);
            for (int i = 0; i < expectedTypes.Length; i++)
            {
                IAtom nextAtom = expected1.Atoms[i];
                IAtomType perceivedType = matcher.FindMatchingAtomType(expected1, nextAtom);
                Assert.IsNotNull(perceivedType, "Missing atom type for: " + nextAtom + " " + i + " expected: " + expectedTypes[i]);
                Assert.AreEqual(expectedTypes[i], perceivedType.AtomTypeName, "Incorrect atom type perceived for: " + nextAtom);
            }
        }

        /// <summary>
        /// A unit test suite for JUnit. Compound and its fragments to be tested
        // @throws Exception
        ///
        // @see       ElectronImpactNBEReaction#TestNsp2SingleB()
        /// </summary>
        [TestMethod()]
        public void TestM17()
        {
            //CreateFromSmiles("[N*+]=C")
            IAtomContainer expected1 = builder.NewAtomContainer();
            expected1.Atoms.Add(builder.NewAtom("N"));
            expected1.Atoms[0].FormalCharge = 1;
            expected1.SingleElectrons.Add(builder.NewSingleElectron(expected1.Atoms[0]));
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Double);
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[2], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[3], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "N.plus.sp2.radical", "C.sp2", "H", "H", "H" };
            Assert.AreEqual(expectedTypes.Length, expected1.Atoms.Count);
            for (int i = 0; i < expectedTypes.Length; i++)
            {
                IAtom nextAtom = expected1.Atoms[i];
                IAtomType perceivedType = matcher.FindMatchingAtomType(expected1, nextAtom);
                Assert.IsNotNull(perceivedType, "Missing atom type for: " + nextAtom + " " + i + " expected: " + expectedTypes[i]);
                Assert.AreEqual(expectedTypes[i], perceivedType.AtomTypeName, "Incorrect atom type perceived for: " + nextAtom);
            }
        }

        /// <summary>
        /// A unit test suite for JUnit. Compound and its fragments to be tested
        // @throws Exception
        ///
        // @see       ElectronImpactNBEReaction#TestNsp3SingleB()
        /// </summary>
        [TestMethod()]
        public void TestM18()
        {
            //CreateFromSmiles("C[N*+]")
            IAtomContainer expected1 = builder.NewAtomContainer();
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.Atoms.Add(builder.NewAtom("N"));
            expected1.Atoms[1].FormalCharge = 1;
            expected1.SingleElectrons.Add(builder.NewSingleElectron(expected1.Atoms[1]));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Single);
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[2], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[3], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[4], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[5], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[6], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "N.plus.sp3.radical", "H", "H", "H", "H", "H" };
            Assert.AreEqual(expectedTypes.Length, expected1.Atoms.Count);
            for (int i = 0; i < expectedTypes.Length; i++)
            {
                IAtom nextAtom = expected1.Atoms[i];
                IAtomType perceivedType = matcher.FindMatchingAtomType(expected1, nextAtom);
                Assert.IsNotNull(perceivedType, "Missing atom type for: " + nextAtom + " " + i + " expected: " + expectedTypes[i]);
                Assert.AreEqual(expectedTypes[i], perceivedType.AtomTypeName, "Incorrect atom type perceived for: " + nextAtom);
            }
        }

        /// <summary>
        /// A unit test suite for JUnit. Compound and its fragments to be tested
        // @throws Exception
        ///
        // @see       ElectronImpactNBEReaction#TestNsp3SingleB()
        /// </summary>
        [TestMethod()]
        public void TestM19()
        {
            //CreateFromSmiles("C=[N*+]")
            IAtomContainer expected1 = builder.NewAtomContainer();
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.Atoms.Add(builder.NewAtom("N"));
            expected1.Atoms[1].FormalCharge = 1;
            expected1.SingleElectrons.Add(builder.NewSingleElectron(expected1.Atoms[1]));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Double);
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[2], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[3], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "C.sp2", "N.plus.sp2.radical", "H", "H", "H" };
            Assert.AreEqual(expectedTypes.Length, expected1.Atoms.Count);
            for (int i = 0; i < expectedTypes.Length; i++)
            {
                IAtom nextAtom = expected1.Atoms[i];
                IAtomType perceivedType = matcher.FindMatchingAtomType(expected1, nextAtom);
                Assert.IsNotNull(perceivedType, "Missing atom type for: " + nextAtom + " " + i + " expected: " + expectedTypes[i]);
                Assert.AreEqual(expectedTypes[i], perceivedType.AtomTypeName, "Incorrect atom type perceived for: " + nextAtom);
            }
        }

        /// <summary>
        /// A unit test suite for JUnit. Compound and its fragments to be tested
        // @throws Exception
        ///
        // @see       RadicalSiteInitiationHReactionTest#TestManuallyCentreActive()
        /// </summary>
        [TestMethod()]
        public void TestM20()
        {
            //CreateFromSmiles("H*]")
            IAtomContainer expected1 = builder.NewAtomContainer();
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.SingleElectrons.Add(builder.NewSingleElectron(expected1.Atoms[0]));

            string[] expectedTypes = { "H.radical" };
            Assert.AreEqual(expectedTypes.Length, expected1.Atoms.Count);
            for (int i = 0; i < expectedTypes.Length; i++)
            {
                IAtom nextAtom = expected1.Atoms[i];
                IAtomType perceivedType = matcher.FindMatchingAtomType(expected1, nextAtom);
                Assert.IsNotNull(perceivedType, "Missing atom type for: " + nextAtom + " " + i + " expected: " + expectedTypes[i]);
                Assert.AreEqual(expectedTypes[i], perceivedType.AtomTypeName, "Incorrect atom type perceived for: " + nextAtom);
            }
        }

        /// <summary>
        /// A unit test suite for JUnit. Compound and its fragments to be tested
        // @throws Exception
        ///
        /// </summary>
        [TestMethod()]
        public void TestM21()
        {
            //CreateFromSmiles("NaH")
            IAtomContainer expected1 = builder.NewAtomContainer();
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("Na"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "H", "Na" };
            Assert.AreEqual(expectedTypes.Length, expected1.Atoms.Count);
            for (int i = 0; i < expectedTypes.Length; i++)
            {
                IAtom nextAtom = expected1.Atoms[i];
                IAtomType perceivedType = matcher.FindMatchingAtomType(expected1, nextAtom);
                Assert.IsNotNull(perceivedType, "Missing atom type for: " + nextAtom + " " + i + " expected: " + expectedTypes[i]);
                Assert.AreEqual(expectedTypes[i], perceivedType.AtomTypeName, "Incorrect atom type perceived for: " + nextAtom);
            }
        }

        /// <summary>
        /// A unit test suite for JUnit. Compound and its fragments to be tested
        // @throws Exception
        ///
        /// <seealso cref="AdductionSodiumLPReactionTest"/>
        /// </summary>
        [TestMethod()]
        public void TestM22()
        {
            IAtomContainer expected1 = builder.NewAtomContainer();
            expected1.Atoms.Add(builder.NewAtom("O"));
            expected1.Atoms[0].FormalCharge = 1;
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Double);
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[2], BondOrder.Single);
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("Na"));
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[3], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[2], expected1.Atoms[4], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[2], expected1.Atoms[5], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[2], expected1.Atoms[6], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[7], BondOrder.Single);

            string[] expectedTypes = { "O.plus.sp2", "C.sp2", "C.sp3", "H", "H", "H", "H", "Na" };
            Assert.AreEqual(expectedTypes.Length, expected1.Atoms.Count);
            for (int i = 0; i < expectedTypes.Length; i++)
            {
                IAtom nextAtom = expected1.Atoms[i];
                IAtomType perceivedType = matcher.FindMatchingAtomType(expected1, nextAtom);
                Assert.IsNotNull(perceivedType, "Missing atom type for: " + nextAtom + " " + i + " expected: " + expectedTypes[i]);
                Assert.AreEqual(expectedTypes[i], perceivedType.AtomTypeName, "Incorrect atom type perceived for: " + nextAtom);
            }
        }

        /// <summary>
        /// A unit test suite for JUnit. Compound and its fragments to be tested
        // @throws Exception
        ///
        /// <seealso cref="AdductionSodiumLPReactionTest"/>
        /// </summary>
        [TestMethod()]
        public void TestM23()
        {
            IAtomContainer expected1 = builder.NewAtomContainer();
            expected1.Atoms.Add(builder.NewPseudoAtom("R"));
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Single);
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[2], BondOrder.Single);
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.Atoms[3].FormalCharge = 1;
            expected1.AddBond(expected1.Atoms[2], expected1.Atoms[3], BondOrder.Single);
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.AddBond(expected1.Atoms[3], expected1.Atoms[4], BondOrder.Single);
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.AddBond(expected1.Atoms[4], expected1.Atoms[5], BondOrder.Single);
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.AddBond(expected1.Atoms[5], expected1.Atoms[6], BondOrder.Single);
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[7], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[8], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[2], expected1.Atoms[9], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[2], expected1.Atoms[10], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[3], expected1.Atoms[11], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[4], expected1.Atoms[12], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[4], expected1.Atoms[13], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[5], expected1.Atoms[14], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[5], expected1.Atoms[15], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[6], expected1.Atoms[16], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[6], expected1.Atoms[17], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[6], expected1.Atoms[18], BondOrder.Single);

            string[] expectedTypes = {"X", "C.sp3", "C.sp3", "C.plus.planar", "C.sp3", "C.sp3", "C.sp3", "H", "H", "H",
                "H", "H", "H", "H", "H", "H", "H", "H", "H"};
            Assert.AreEqual(expectedTypes.Length, expected1.Atoms.Count);
            for (int i = 0; i < expectedTypes.Length; i++)
            {
                IAtom nextAtom = expected1.Atoms[i];
                IAtomType perceivedType = matcher.FindMatchingAtomType(expected1, nextAtom);
                Assert.IsNotNull(perceivedType, "Missing atom type for: " + nextAtom + " " + i + " expected: " + expectedTypes[i]);
                Assert.AreEqual(expectedTypes[i], perceivedType.AtomTypeName, "Incorrect atom type perceived for: " + nextAtom);
            }
        }

        /// <summary>
        /// A unit test suite for JUnit. Compound and its fragments to be tested
        // @throws Exception
        ///
        // @see HomolyticCleavageReactionTest#testNsp2DoubleB
        /// </summary>
        [TestMethod()]
        public void TestM24()
        {
            //CreateFromSmiles("C[N*]-[C*]")
            IAtomContainer expected1 = builder.NewAtomContainer();
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.Atoms.Add(builder.NewAtom("N"));
            expected1.SingleElectrons.Add(builder.NewSingleElectron(expected1.Atoms[1]));
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.SingleElectrons.Add(builder.NewSingleElectron(expected1.Atoms[2]));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[2], BondOrder.Single);
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[3], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[4], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[5], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[2], expected1.Atoms[6], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[2], expected1.Atoms[7], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "N.sp3.radical", "C.radical.planar", "H", "H", "H", "H", "H" };
            Assert.AreEqual(expectedTypes.Length, expected1.Atoms.Count);
            for (int i = 0; i < expectedTypes.Length; i++)
            {
                IAtom nextAtom = expected1.Atoms[i];
                IAtomType perceivedType = matcher.FindMatchingAtomType(expected1, nextAtom);
                Assert.IsNotNull(perceivedType, "Missing atom type for: " + nextAtom + " " + i + " expected: " + expectedTypes[i]);
                Assert.AreEqual(expectedTypes[i], perceivedType.AtomTypeName, "Incorrect atom type perceived for: " + nextAtom);
            }
        }

        /// <summary>
        /// A unit test suite for JUnit. Compound and its fragments to be tested
        // @throws Exception
        ///
        // @see HomolyticCleavageReactionTest#testNsp2DoubleB
        /// </summary>
        [TestMethod()]
        public void TestM25()
        {
            //CreateFromSmiles("C[O*]")
            IAtomContainer expected1 = builder.NewAtomContainer();
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.Atoms.Add(builder.NewAtom("O"));
            expected1.SingleElectrons.Add(builder.NewSingleElectron(expected1.Atoms[1]));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Single);
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[2], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[3], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[4], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected1);

            string[] expectedTypes = { "C.sp3", "O.sp3.radical", "H", "H", "H" };
            Assert.AreEqual(expectedTypes.Length, expected1.Atoms.Count);
            for (int i = 0; i < expectedTypes.Length; i++)
            {
                IAtom nextAtom = expected1.Atoms[i];
                IAtomType perceivedType = matcher.FindMatchingAtomType(expected1, nextAtom);
                Assert.IsNotNull(perceivedType, "Missing atom type for: " + nextAtom + " " + i + " expected: " + expectedTypes[i]);
                Assert.AreEqual(expectedTypes[i], perceivedType.AtomTypeName, "Incorrect atom type perceived for: " + nextAtom);
            }
        }

        /// <summary>
        /// A unit test suite for JUnit. Compound and its fragments to be tested
        // @throws Exception
        ///
        // @see HomolyticCleavageReactionTest#testNsp2DoubleB
        /// </summary>
        [TestMethod()]
        public void TestM26()
        {
            IAtomContainer expected1 = builder.NewAtomContainer();
            expected1.Atoms.Add(builder.NewAtom("F"));
            expected1.Atoms[0].FormalCharge = 1;
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Double);
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.Atoms[2].FormalCharge = -1;
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[2], BondOrder.Single);
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.AddBond(expected1.Atoms[2], expected1.Atoms[3], BondOrder.Single);
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.AddBond(expected1.Atoms[3], expected1.Atoms[4], BondOrder.Double);
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.AddBond(expected1.Atoms[4], expected1.Atoms[5], BondOrder.Single);
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.AddBond(expected1.Atoms[5], expected1.Atoms[6], BondOrder.Double);
            expected1.AddBond(expected1.Atoms[6], expected1.Atoms[1], BondOrder.Single);
            AddExplicitHydrogens(expected1);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected1);
            LonePairElectronChecker lpcheck = new LonePairElectronChecker();
            lpcheck.Saturate(expected1);

            string[] expectedTypes = {"F.plus.sp2", "C.sp2", "C.minus.planar", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "H",
                "H", "H", "H", "H"

        };
            Assert.AreEqual(expectedTypes.Length, expected1.Atoms.Count);
            for (int i = 0; i < expectedTypes.Length; i++)
            {
                IAtom nextAtom = expected1.Atoms[i];
                IAtomType perceivedType = matcher.FindMatchingAtomType(expected1, nextAtom);

                Assert.IsNotNull(perceivedType, "Missing atom type for: " + nextAtom + " " + i + " expected: " + expectedTypes[i]);
                Assert.AreEqual(expectedTypes[i], perceivedType.AtomTypeName, "Incorrect atom type perceived for: " + nextAtom);
                nextAtom.Hybridization = Hybridization.Unset;
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected1);
                IAtomType type = matcher.FindMatchingAtomType(expected1, nextAtom);
                Assert.IsNotNull(type);
            }
        }
    }
}
