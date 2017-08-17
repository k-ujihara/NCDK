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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Dict;
using NCDK.Tools.Diff;
using NCDK.Common.Base;
using NCDK.QSAR.Result;
using NCDK.Tools.Manipulator;
using NCDK.Default;
using NCDK.Numerics;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Tests for molecular descriptors.
    /// </summary>
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public abstract class MolecularDescriptorTest : DescriptorTest<IMolecularDescriptor>
    {
        private static DictionaryDatabase dictDB = new DictionaryDatabase();
        private static EntryDictionary dict = dictDB.GetDictionary("descriptor-algorithms");

        public MolecularDescriptorTest() { }

        [TestMethod()]
        public void TestDescriptorIdentifierExistsInOntology()
        {
            Entry ontologyEntry = dict[Descriptor.Specification.SpecificationReference.Substring(dict.NS.ToString().Length).ToLowerInvariant()];
            Assert.IsNotNull(ontologyEntry);
        }

        [TestMethod()]
        public void TestCalculate_IAtomContainer()
        {
            IAtomContainer mol = null;
            try
            {
                mol = SomeoneBringMeSomeWater(Default.ChemObjectBuilder.Instance);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.StackTrace);
                Assert.Fail("Error in generating the test molecule");
            }

            DescriptorValue v = null;
            try
            {
                v = Descriptor.Calculate(mol);
            }
            catch (Exception e)
            {
                Assert.Fail("A descriptor must not throw an exception. Exception was:\n" + e.Message);
            }
            Assert.IsNotNull(v);
            Assert.IsTrue(0 != v.GetValue().Length, "The descriptor did not calculate any value.");
        }

        [TestMethod()]
        public void TestCalculate_NoModifications()
        {
            IAtomContainer mol = SomeoneBringMeSomeWater(Default.ChemObjectBuilder.Instance);
            IAtomContainer clone = (IAtomContainer)mol.Clone();
            Descriptor.Calculate(mol);
            string diff = AtomContainerDiff.Diff(clone, mol);
            Assert.AreEqual(0, diff.Length,
            $"The descriptor must not change the passed molecule in any respect, but found this diff: {diff}");
        }

        /// <summary>
        /// Checks if the given labels are consistent.
        /// </summary>
        /// <exception cref="Exception">Passed on from calculate.</exception>
        [TestMethod()]
        public void TestLabels()
        {
            IAtomContainer mol = SomeoneBringMeSomeWater(Default.ChemObjectBuilder.Instance);

            DescriptorValue v = Descriptor.Calculate(mol);
            Assert.IsNotNull(v);
            string[] names = v.Names;
            Assert.IsNotNull(names, "The descriptor must return labels using the Names method.");
            Assert.AreNotEqual(0, names.Length, "At least one label must be given.");
            for (int i = 0; i < names.Length; i++)
            {
                Assert.IsNotNull(names[i], "A descriptor label may not be null.");
                Assert.AreNotSame(0, names[i].Length, "The label string must not be empty.");
                //            Console.Out.WriteLine("Label: " + names[i]);
            }
            Assert.IsNotNull(v.GetValue());
            int valueCount = v.GetValue().Length;
            Assert.AreEqual(names.Length, valueCount, "The number of labels must equals the number of values.");
        }

        /// <summary>
        /// Check if the names obtained directly from the decsriptor without
        /// calculation match those obtained from the descriptor value object.
        /// Also ensure that the number of actual values matches the length
        /// of the names
        /// </summary>
        [TestMethod()]
        public void TestNamesConsistency()
        {
            IAtomContainer mol = SomeoneBringMeSomeWater(Default.ChemObjectBuilder.Instance);

            string[] names1 = Descriptor.DescriptorNames;
            DescriptorValue v = Descriptor.Calculate(mol);
            string[] names2 = v.Names;

            Assert.AreEqual(names1.Length, names2.Length);
            Assert.IsTrue(Compares.AreDeepEqual(names1, names2));

            int valueCount = v.GetValue().Length;
            Assert.AreEqual(valueCount, names1.Length);
        }

        [TestMethod()]
        public void TestGetDescriptorResultType()
        {
            IDescriptorResult result = Descriptor.DescriptorResultType;
            Assert.IsNotNull(result, "The DescriptorResultType must not be null.");

            IAtomContainer mol = SomeoneBringMeSomeWater(Default.ChemObjectBuilder.Instance);
            DescriptorValue v = Descriptor.Calculate(mol);

            Assert.IsTrue(
                    result.GetType().FullName.Contains(v.GetValue().GetType().FullName),
                    "The DescriptorResultType is inconsistent with the calculated descriptor results");
            Assert.AreEqual(v.GetValue().Length, result.Length,
                    "The specified DescriptorResultType length does not match the actually calculated result vector length");
        }

        [TestMethod()]
        public void TestTakeIntoAccountImplicitHydrogens()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer methane1 = builder.NewAtomContainer();
            IAtom c1 = builder.NewAtom("C");
            c1.ImplicitHydrogenCount = 4;
            methane1.Atoms.Add(c1);

            IAtomContainer methane2 = builder.NewAtomContainer();
            IAtom c2 = builder.NewAtom("C");
            methane2.Atoms.Add(c2);
            IAtom h1 = builder.NewAtom("H");
            methane2.Atoms.Add(h1);
            IAtom h2 = builder.NewAtom("H");
            methane2.Atoms.Add(h2);
            IAtom h3 = builder.NewAtom("H");
            methane2.Atoms.Add(h3);
            IAtom h4 = builder.NewAtom("H");
            methane2.Atoms.Add(h4);
            methane2.AddBond(methane2.Atoms[0], methane2.Atoms[1], BondOrder.Single);
            methane2.AddBond(methane2.Atoms[0], methane2.Atoms[2], BondOrder.Single);
            methane2.AddBond(methane2.Atoms[0], methane2.Atoms[3], BondOrder.Single);
            methane2.AddBond(methane2.Atoms[0], methane2.Atoms[4], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(methane1);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(methane2);
            AddImplicitHydrogens(methane1);
            AddImplicitHydrogens(methane2);

            IDescriptorResult v1 = Descriptor.Calculate(methane1).GetValue();
            IDescriptorResult v2 = Descriptor.Calculate(methane2).GetValue();

            string errorMessage = "("
                    + Descriptor.GetType().ToString()
                    + ") The descriptor does not give the same results depending on whether hydrogens are implicit or explicit.";
            AssertEqualOutput(v1, v2, errorMessage);
        }

        [TestMethod()]
        public void TestTakeIntoAccountImplicitHydrogensInEthane()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer ethane1 = builder.NewAtomContainer();
            IAtom c1 = builder.NewAtom("C");
            IAtom c2 = builder.NewAtom("C");
            c1.ImplicitHydrogenCount = 3;
            c2.ImplicitHydrogenCount = 3;
            ethane1.Atoms.Add(c1);
            ethane1.Atoms.Add(c2);
            ethane1.AddBond(ethane1.Atoms[0], ethane1.Atoms[1], BondOrder.Single);

            IAtomContainer ethane2 = builder.NewAtomContainer();
            IAtom c3 = builder.NewAtom("C");
            IAtom c4 = builder.NewAtom("C");
            ethane2.Atoms.Add(c3);
            ethane2.Atoms.Add(c4);

            IAtom h1 = builder.NewAtom("H");
            ethane2.Atoms.Add(h1);
            IAtom h2 = builder.NewAtom("H");
            ethane2.Atoms.Add(h2);
            IAtom h3 = builder.NewAtom("H");
            ethane2.Atoms.Add(h3);

            IAtom h4 = builder.NewAtom("H");
            IAtom h5 = builder.NewAtom("H");
            IAtom h6 = builder.NewAtom("H");
            ethane2.Atoms.Add(h4);
            ethane2.Atoms.Add(h5);
            ethane2.Atoms.Add(h6);

            ethane2.AddBond(ethane2.Atoms[0], ethane2.Atoms[1], BondOrder.Single);
            ethane2.AddBond(ethane2.Atoms[0], ethane2.Atoms[2], BondOrder.Single);
            ethane2.AddBond(ethane2.Atoms[0], ethane2.Atoms[3], BondOrder.Single);
            ethane2.AddBond(ethane2.Atoms[0], ethane2.Atoms[4], BondOrder.Single);

            ethane2.AddBond(ethane2.Atoms[1], ethane2.Atoms[5], BondOrder.Single);
            ethane2.AddBond(ethane2.Atoms[1], ethane2.Atoms[6], BondOrder.Single);
            ethane2.AddBond(ethane2.Atoms[1], ethane2.Atoms[7], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(ethane1);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(ethane2);
            AddImplicitHydrogens(ethane1);
            AddImplicitHydrogens(ethane2);

            IDescriptorResult v1 = Descriptor.Calculate(ethane1).GetValue();
            IDescriptorResult v2 = Descriptor.Calculate(ethane2).GetValue();

            string errorMessage = "("
                    + Descriptor.GetType().ToString()
                    + ") The descriptor does not give the same results depending on whether hydrogens are implicit or explicit.";
            AssertEqualOutput(v1, v2, errorMessage);
        }
        
        /// <summary>
        /// Checks that the results of the first and the second descriptor results are identical.
        /// </summary>
        /// <param name="v1">first <see cref="IDescriptorResult"/></param>
        /// <param name="v2">second <see cref="IDescriptorResult"/></param>
        /// <param name="errorMessage">error message to report when the results are not the same</param>
        private void AssertEqualOutput(IDescriptorResult v1, IDescriptorResult v2, string errorMessage)
        {
            if (v1 is IntegerResult)
            {
                Assert.AreEqual(((IntegerResult)v1).Value, ((IntegerResult)v2).Value, errorMessage);
            }
            else if (v1 is DoubleResult)
            {
                var p1 = ((DoubleResult)v1).Value;
                var p2 = ((DoubleResult)v2).Value;
                if (!(double.IsNaN(p1) && double.IsNaN(p2)))
                    Assert.AreEqual(p1, p2, 0.00001, errorMessage);
            }
            else if (v1 is BooleanResult)
            {
                Assert.AreEqual(((BooleanResult)v1).Value, ((BooleanResult)v2).Value, errorMessage);
            }
            else if (v1 is DoubleArrayResult)
            {
                DoubleArrayResult da1 = (DoubleArrayResult)v1;
                DoubleArrayResult da2 = (DoubleArrayResult)v2;
                for (int i = 0; i < da1.Length; i++)
                {
                    if (!(double.IsNaN(da1[i])) && double.IsNaN(da2[i]))
                        Assert.AreEqual(da1[i], da2[i], 0.00001, errorMessage);
                }
            }
            else if (v1 is IntegerArrayResult)
            {
                IntegerArrayResult da1 = (IntegerArrayResult)v1;
                IntegerArrayResult da2 = (IntegerArrayResult)v2;
                for (int i = 0; i < da1.Length; i++)
                {
                    Assert.AreEqual(da1[i], da2[i], errorMessage);
                }
            }
        }

        [TestMethod()]
        public void TestImplementationIndependence()
        {
            IAtomContainer water1 = SomeoneBringMeSomeWater(Default.ChemObjectBuilder.Instance);
            IAtomContainer water2 = SomeoneBringMeSomeWater(Silent.ChemObjectBuilder.Instance);

            IDescriptorResult v1 = Descriptor.Calculate(water1).GetValue();
            IDescriptorResult v2 = Descriptor.Calculate(water2).GetValue();

            string errorMessage = "(" + Descriptor.GetType().ToString()
                    + ") The descriptor does not give the same results depending on "
                    + "the actual IChemObject implementation set (data, nonotify).";
            AssertEqualOutput(v1, v2, errorMessage);
        }

        [TestMethod()]
        public void TestAtomContainerHandling()
        {
            IAtomContainer water1 = SomeoneBringMeSomeWater(Default.ChemObjectBuilder.Instance);
            // creates an AtomContainer with the atoms / bonds from water1
            IAtomContainer water2 = new AtomContainer(water1);

            IDescriptorResult v1 = Descriptor.Calculate(water1).GetValue();
            IDescriptorResult v2 = Descriptor.Calculate(water2).GetValue();

            string errorMessage = "(" + Descriptor.GetType().ToString()
                    + ") The descriptor does not give the same results depending on "
                    + "it being passed an IAtomContainer or an IAtomContainer.";
            AssertEqualOutput(v1, v2, errorMessage);
        }

        /// <summary>
        /// Descriptors should not throw Exceptions on disconnected structures, but return NA instead.
        /// </summary>
        [TestMethod()]
        public void TestDisconnectedStructureHandling()
        {
            IAtomContainer disconnected = new AtomContainer();
            IAtom chloride = new Atom("Cl");
            chloride.FormalCharge = -1;
            disconnected.Atoms.Add(chloride);
            IAtom sodium = new Atom("Na");
            sodium.FormalCharge = +1;
            disconnected.Atoms.Add(sodium);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(disconnected);
            AddImplicitHydrogens(disconnected);

            IDescriptorResult v1 = Descriptor.Calculate(disconnected).GetValue();
        }

        //@Ignore
        //[TestMethod()]
        private static int TestMethod()
        {
            throw new NotImplementedException();
        }

        public void TestTakeIntoAccountBondHybridization()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer ethane1 = builder.NewAtomContainer();
            IAtom c1 = builder.NewAtom("C");
            IAtom c2 = builder.NewAtom("C");
            ethane1.Atoms.Add(c1);
            ethane1.Atoms.Add(c2);
            ethane1.AddBond(ethane1.Atoms[0], ethane1.Atoms[1], BondOrder.Double);

            IAtomContainer ethane2 = builder.NewAtomContainer();
            IAtom c3 = builder.NewAtom("C");
            c3.Hybridization = Hybridization.SP2;
            IAtom c4 = builder.NewAtom("C");
            c4.Hybridization = Hybridization.SP2;
            ethane2.Atoms.Add(c3);
            ethane2.Atoms.Add(c4);
            ethane2.AddBond(ethane2.Atoms[0], ethane2.Atoms[1], BondOrder.Single);

            IDescriptorResult v1 = Descriptor.Calculate(ethane1).GetValue();
            IDescriptorResult v2 = Descriptor.Calculate(ethane2).GetValue();

            string errorMessage = "("
                    + Descriptor.GetType().ToString()
                    + ") The descriptor does not give the same results depending on whether bond order or atom type are considered.";
            AssertEqualOutput(v1, v2, errorMessage);
        }

        private IAtomContainer SomeoneBringMeSomeWater(IChemObjectBuilder builder)
        {
            IAtomContainer mol = builder.NewAtomContainer();
            IAtom c1 = builder.NewAtom("O");
            c1.Point3D = new Vector3(0.0, 0.0, 0.0);
            IAtom h1 = builder.NewAtom("H");
            h1.Point3D = new Vector3(1.0, 0.0, 0.0);
            IAtom h2 = builder.NewAtom("H");
            h2.Point3D = new Vector3(-1.0, 0.0, 0.0);
            mol.Atoms.Add(c1);
            mol.Atoms.Add(h1);
            mol.Atoms.Add(h2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddImplicitHydrogens(mol);
            return mol;
        }
    }
}
