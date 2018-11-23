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
using NCDK.Silent;

namespace NCDK.AtomTypes
{
    // @cdk.module test-reaction
    [TestClass()]
    public class ResonanceStructuresTest : CDKTestCase
    {
        private readonly static IChemObjectBuilder builder;
        private readonly static IAtomTypeMatcher matcher;

        static ResonanceStructuresTest()
        {
            builder = CDK.Builder;
            matcher = CDK.AtomTypeMatcher;
        }

        /// <summary>
        /// Constructor of the ResonanceStructuresTest.
        /// </summary>
        public ResonanceStructuresTest()
            : base()
        { }

        /// <summary>
        /// A unit test suite. Compound and its fragments to be tested
        /// </summary>
        /// <seealso cref="Tools.StructureResonanceGeneratorTest.TestGetAllStructures_IAtomContainer"/>
        // @cdk.inchi InChI=1/C8H10/c1-7-5-3-4-6-8(7)2/h3-6H,1-2H3
        [TestMethod()]
        public void TestGetAllStructures_IAtomContainer()
        {
            //COMPOUND
            //O=C([H])C(=[O+])C([H])([H])[H]
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("O"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("O"));
            molecule.Atoms[3].FormalCharge = 1;
            molecule.SingleElectrons.Add(new SingleElectron(molecule.Atoms[3]));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[2], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[4], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[5], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[6], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[7], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[8], BondOrder.Single);

            string[] expectedTypes = { "C.sp2", "O.sp2", "C.sp2", "O.plus.sp2.radical", "C.sp3", "H", "H", "H", "H" };
            Assert.AreEqual(expectedTypes.Length, molecule.Atoms.Count);
            for (int i = 0; i < expectedTypes.Length; i++)
            {
                IAtom nextAtom = molecule.Atoms[i];
                IAtomType perceivedType = matcher.FindMatchingAtomType(molecule, nextAtom);
                Assert.IsNotNull(perceivedType, "Missing atom type for: " + nextAtom);
                Assert.AreEqual(expectedTypes[i], perceivedType.AtomTypeName, "Incorrect atom type perceived for: " + nextAtom);
            }
            //
            //        //FRAGMENT_1
            //        //
            //        IAtomContainer expectedStructure = builder.NewAtomContainer();
            //        expectedStructure.Atoms.Add(builder.NewInstance(typeof(IAtom),"C"));
            //        expectedStructure.Atoms.Add(builder.NewInstance(typeof(IAtom),"C"));
            //        expectedStructure.Atoms.Add(builder.NewInstance(typeof(IAtom),"C"));
            //        expectedStructure.Atoms.Add(builder.NewInstance(typeof(IAtom),"C"));
            //        expectedStructure.Atoms.Add(builder.NewInstance(typeof(IAtom),"C"));
            //        expectedStructure.Atoms.Add(builder.NewInstance(typeof(IAtom),"C"));
            //        expectedStructure.Atoms.Add(builder.NewInstance(typeof(IAtom),"C"));
            //        expectedStructure.Atoms.Add(builder.NewInstance(typeof(IAtom),"C"));
            //        expectedStructure.Bonds.Add(0,1,BondOrder.Double);
            //        expectedStructure.Bonds.Add(1,2,BondOrder.Single);
            //        expectedStructure.Bonds.Add(2,3,BondOrder.Double);
            //        expectedStructure.Bonds.Add(3,4,BondOrder.Single);
            //        expectedStructure.Bonds.Add(4,5,BondOrder.Double);
            //        expectedStructure.Bonds.Add(5,0,BondOrder.Single);
            //        expectedStructure.Bonds.Add(0,6,BondOrder.Single);
            //        expectedStructure.Bonds.Add(1,7,BondOrder.Single);
            //        AddExplicitHydrogens(expectedStructure);
            //
            //        string[] expectedTypes1 = {
            //            "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2",
            //            "C.sp3", "C.sp3", "H", "H", "H", "H", "H", "H", "H",
            //            "H", "H", "H"
            //        };
            //        Assert.AreEqual(expectedTypes.Length, expectedStructure.Atoms.Count);
            //        for (int i=0; i<expectedTypes1.Length; i++) {
            //            IAtom nextAtom = expectedStructure.Atoms[i];
            //            IAtomType perceivedType = matcher.FindMatchingAtomType(expectedStructure, nextAtom);
            //            Assert.IsNotNull(
            //                "Missing atom type for: " + nextAtom,
            //                perceivedType
            //            );
            //            Assert.AreEqual(
            //                "Incorrect atom type perceived for: " + nextAtom,
            //                expectedTypes1[i], perceivedType.AtomTypeName
            //            );
            //        }
        }

        /// <summary>
        /// A unit test suite. Compound and its fragments to be tested
        /// </summary>
        /// <seealso cref="Tools.StructureResonanceGeneratorTest.Test12DimethylBenzene"/>
        // @cdk.inchi InChI=1/C8H10/c1-7-5-3-4-6-8(7)2/h3-6H,1-2H3
        [TestMethod()]
        public void Test12DimethylBenzene()
        {
            //COMPOUND
            //[H]C1=C([H])C([H])=C(C(=C1([H]))C([H])([H])[H])C([H])([H])[H]
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[0], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[6], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[7], BondOrder.Single);
            AddExplicitHydrogens(molecule);

            string[] expectedTypes = { "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp3", "C.sp3", "H", "H", "H", "H", "H", "H", "H", "H", "H", "H" };
            Assert.AreEqual(expectedTypes.Length, molecule.Atoms.Count);
            for (int i = 0; i < expectedTypes.Length; i++)
            {
                IAtom nextAtom = molecule.Atoms[i];
                IAtomType perceivedType = matcher.FindMatchingAtomType(molecule, nextAtom);
                Assert.IsNotNull(perceivedType, "Missing atom type for: " + nextAtom);
                Assert.AreEqual(expectedTypes[i], perceivedType.AtomTypeName, "Incorrect atom type perceived for: " + nextAtom);
            }

            //FRAGMENT_1
            //[H]C=1C([H])=C([H])C(=C(C=1([H]))C([H])([H])[H])C([H])([H])[H]
            IAtomContainer expectedStructure = builder.NewAtomContainer();
            expectedStructure.Atoms.Add(builder.NewAtom("C"));
            expectedStructure.Atoms.Add(builder.NewAtom("C"));
            expectedStructure.Atoms.Add(builder.NewAtom("C"));
            expectedStructure.Atoms.Add(builder.NewAtom("C"));
            expectedStructure.Atoms.Add(builder.NewAtom("C"));
            expectedStructure.Atoms.Add(builder.NewAtom("C"));
            expectedStructure.Atoms.Add(builder.NewAtom("C"));
            expectedStructure.Atoms.Add(builder.NewAtom("C"));
            expectedStructure.AddBond(expectedStructure.Atoms[0], expectedStructure.Atoms[1], BondOrder.Double);
            expectedStructure.AddBond(expectedStructure.Atoms[1], expectedStructure.Atoms[2], BondOrder.Single);
            expectedStructure.AddBond(expectedStructure.Atoms[2], expectedStructure.Atoms[3], BondOrder.Double);
            expectedStructure.AddBond(expectedStructure.Atoms[3], expectedStructure.Atoms[4], BondOrder.Single);
            expectedStructure.AddBond(expectedStructure.Atoms[4], expectedStructure.Atoms[5], BondOrder.Double);
            expectedStructure.AddBond(expectedStructure.Atoms[5], expectedStructure.Atoms[0], BondOrder.Single);
            expectedStructure.AddBond(expectedStructure.Atoms[0], expectedStructure.Atoms[6], BondOrder.Single);
            expectedStructure.AddBond(expectedStructure.Atoms[1], expectedStructure.Atoms[7], BondOrder.Single);
            AddExplicitHydrogens(expectedStructure);

            string[] expectedTypes1 = { "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp3", "C.sp3", "H", "H", "H", "H", "H", "H", "H", "H", "H", "H"};
            Assert.AreEqual(expectedTypes.Length, expectedStructure.Atoms.Count);
            for (int i = 0; i < expectedTypes1.Length; i++)
            {
                IAtom nextAtom = expectedStructure.Atoms[i];
                IAtomType perceivedType = matcher.FindMatchingAtomType(expectedStructure, nextAtom);
                Assert.IsNotNull(perceivedType, "Missing atom type for: " + nextAtom);
                Assert.AreEqual(expectedTypes[i], perceivedType.AtomTypeName, "Incorrect atom type perceived for: " + nextAtom);
            }
        }
    }
}
