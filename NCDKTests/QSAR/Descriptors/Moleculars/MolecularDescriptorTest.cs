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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Common.Base;
using NCDK.Dict;
using NCDK.Numerics;
using NCDK.QSAR.Results;
using NCDK.Silent;
using NCDK.Templates;
using NCDK.Tools.Diff;
using NCDK.Tools.Manipulator;
using System;

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

        protected MolecularDescriptorTest()
        {
        }

        private static uint FlagsToInt(IAtomContainer mol)
        {
            uint f = 0;
            if (mol.IsPlaced)
                f++;
            f <<= 1;
            if (mol.IsVisited)
                f++;
            f <<= 1;
            if (mol.IsAromatic)
                f++;
            f <<= 1;
            if (mol.IsSingleOrDouble)
                f++;
            return f;
        }

        private static uint FlagsToInt(IAtom atom)
        {
            uint f = 0;
            if (atom.IsPlaced)
                f++;
            f <<= 1;
            if (atom.IsVisited)
                f++;
            f <<= 1;
            if (atom.IsAromatic)
                f++;
            f <<= 1;
            if (atom.IsAliphatic)
                f++;
            f <<= 1;
            if (atom.IsInRing)
                f++;
            f <<= 1;
            if (atom.IsSingleOrDouble)
                f++;
            f <<= 1;
            if (atom.IsHydrogenBondAcceptor)
                f++;
            f <<= 1;
            if (atom.IsHydrogenBondDonor)
                f++;
            f <<= 1;
            if (atom.IsReactiveCenter)
                f++;
            return f;
        }

        private static uint FlagsToInt(IBond bond)
        {
            uint f = 0;
            if (bond.IsPlaced)
                f++;
            f <<= 1;
            if (bond.IsVisited)
                f++;
            f <<= 1;
            if (bond.IsAromatic)
                f++;
            f <<= 1;
            if (bond.IsAliphatic)
                f++;
            f <<= 1;
            if (bond.IsInRing)
                f++;
            f <<= 1;
            if (bond.IsSingleOrDouble)
                f++;
            f <<= 1;
            if (bond.IsReactiveCenter)
                f++;
            return f;
        }

        private static uint[] GetAtomFlags(IAtomContainer mol)
        {
            var flags = new uint[mol.Atoms.Count];
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                flags[i] = FlagsToInt(mol.Atoms[i]);
            }
            return flags;
        }

        private static uint[] GetBondFlags(IAtomContainer mol)
        {
            var flags = new uint[mol.Bonds.Count];
            for (int i = 0; i < mol.Bonds.Count; i++)
            {
                flags[i] = FlagsToInt(mol.Bonds[i]);
            }
            return flags;
        }

        [TestMethod()]
        public void DescriptorDoesNotChangeFlags()
        {
            var mol = TestMoleculeFactory.MakeBenzene();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            var mflags = FlagsToInt(mol);
            var aflags = GetAtomFlags(mol);
            var bflags = GetBondFlags(mol);
            Descriptor.Calculate(mol);
            Assert.AreEqual(mflags, FlagsToInt(mol), "Molecule flags were modified by descriptor!");
            Assert.IsTrue(Compares.AreDeepEqual(aflags, GetAtomFlags(mol)), "Molecule's Atom flags were modified by descriptor!");
            Assert.IsTrue(Compares.AreDeepEqual(bflags, GetBondFlags(mol)), "Molecule's Bond flags were modified by descriptor!");
        }

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
                mol = SomeoneBringMeSomeWater(ChemObjectBuilder.Instance);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.StackTrace);
                Assert.Fail("Error in generating the test molecule");
            }

            IDescriptorValue v = null;
            try
            {
                v = Descriptor.Calculate(mol);
            }
            catch (Exception e)
            {
                Assert.Fail("A descriptor must not throw an exception. Exception was:\n" + e.Message);
            }
            Assert.IsNotNull(v);
            Assert.IsTrue(0 != v.Value.Length, "The descriptor did not calculate any value.");
        }

        [TestMethod()]
        public void TestCalculate_NoModifications()
        {
            var mol = SomeoneBringMeSomeWater(ChemObjectBuilder.Instance);
            var clone = (IAtomContainer)mol.Clone();
            Descriptor.Calculate(mol);
            var diff = AtomContainerDiff.Diff(clone, mol);
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
            var mol = SomeoneBringMeSomeWater(ChemObjectBuilder.Instance);

            var v = Descriptor.Calculate(mol);
            Assert.IsNotNull(v);
            var names = v.Names;
            Assert.IsNotNull(names, "The descriptor must return labels using the Names method.");
            Assert.AreNotEqual(0, names.Count, "At least one label must be given.");
            for (int i = 0; i < names.Count; i++)
            {
                Assert.IsNotNull(names[i], "A descriptor label may not be null.");
                Assert.AreNotSame(0, names[i].Length, "The label string must not be empty.");
            }
            Assert.IsNotNull(v.Value);
            var valueCount = v.Value.Length;
            Assert.AreEqual(names.Count, valueCount, "The number of labels must equals the number of values.");
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
            var mol = SomeoneBringMeSomeWater(ChemObjectBuilder.Instance);

            var names1 = Descriptor.DescriptorNames;
            var v = Descriptor.Calculate(mol);
            var names2 = v.Names;

            Assert.AreEqual(names1.Count, names2.Count);
            Assert.IsTrue(Compares.AreDeepEqual(names1, names2));

            var valueCount = v.Value.Length;
            Assert.AreEqual(valueCount, names1.Count);
        }

        [TestMethod()]
        public void TestGetDescriptorResultType()
        {
            var result = Descriptor.DescriptorResultType;
            Assert.IsNotNull(result, "The DescriptorResultType must not be null.");

            var mol = SomeoneBringMeSomeWater(ChemObjectBuilder.Instance);
            var v = Descriptor.Calculate(mol);

            Assert.IsTrue(
                    result.GetType().FullName.Contains(v.Value.GetType().FullName),
                    "The DescriptorResultType is inconsistent with the calculated descriptor results");
            Assert.AreEqual(v.Value.Length, result.Length,
                    "The specified DescriptorResultType length does not match the actually calculated result vector length");
        }

        [TestMethod()]
        public void TestTakeIntoAccountImplicitHydrogens()
        {
            var builder = ChemObjectBuilder.Instance;
            var methane1 = builder.NewAtomContainer();
            var c1 = builder.NewAtom("C");
            c1.ImplicitHydrogenCount = 4;
            methane1.Atoms.Add(c1);

            var methane2 = builder.NewAtomContainer();
            var c2 = builder.NewAtom("C");
            methane2.Atoms.Add(c2);
            var h1 = builder.NewAtom("H");
            methane2.Atoms.Add(h1);
            var h2 = builder.NewAtom("H");
            methane2.Atoms.Add(h2);
            var h3 = builder.NewAtom("H");
            methane2.Atoms.Add(h3);
            var h4 = builder.NewAtom("H");
            methane2.Atoms.Add(h4);
            methane2.AddBond(methane2.Atoms[0], methane2.Atoms[1], BondOrder.Single);
            methane2.AddBond(methane2.Atoms[0], methane2.Atoms[2], BondOrder.Single);
            methane2.AddBond(methane2.Atoms[0], methane2.Atoms[3], BondOrder.Single);
            methane2.AddBond(methane2.Atoms[0], methane2.Atoms[4], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(methane1);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(methane2);
            AddImplicitHydrogens(methane1);
            AddImplicitHydrogens(methane2);

            var v1 = Descriptor.Calculate(methane1).Value;
            var v2 = Descriptor.Calculate(methane2).Value;

            string errorMessage = "("
                    + Descriptor.GetType().ToString()
                    + ") The descriptor does not give the same results depending on whether hydrogens are implicit or explicit.";
            AssertEqualOutput(v1, v2, errorMessage);
        }

        [TestMethod()]
        public void TestTakeIntoAccountImplicitHydrogensInEthane()
        {
            var builder = ChemObjectBuilder.Instance;
            var ethane1 = builder.NewAtomContainer();
            var c1 = builder.NewAtom("C");
            var c2 = builder.NewAtom("C");
            c1.ImplicitHydrogenCount = 3;
            c2.ImplicitHydrogenCount = 3;
            ethane1.Atoms.Add(c1);
            ethane1.Atoms.Add(c2);
            ethane1.AddBond(ethane1.Atoms[0], ethane1.Atoms[1], BondOrder.Single);

            var ethane2 = builder.NewAtomContainer();
            var c3 = builder.NewAtom("C");
            var c4 = builder.NewAtom("C");
            ethane2.Atoms.Add(c3);
            ethane2.Atoms.Add(c4);

            var h1 = builder.NewAtom("H");
            ethane2.Atoms.Add(h1);
            var h2 = builder.NewAtom("H");
            ethane2.Atoms.Add(h2);
            var h3 = builder.NewAtom("H");
            ethane2.Atoms.Add(h3);

            var h4 = builder.NewAtom("H");
            var h5 = builder.NewAtom("H");
            var h6 = builder.NewAtom("H");
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

            var v1 = Descriptor.Calculate(ethane1).Value;
            var v2 = Descriptor.Calculate(ethane2).Value;

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
        private static void AssertEqualOutput(IDescriptorResult v1, IDescriptorResult v2, string errorMessage)
        {
            if (v1 is Result<int>)
            {
                Assert.AreEqual(((Result<int>)v1).Value, ((Result<int>)v2).Value, errorMessage);
            }
            else if (v1 is Result<double>)
            {
                var p1 = ((Result<double>)v1).Value;
                var p2 = ((Result<double>)v2).Value;
                if (!(double.IsNaN(p1) && double.IsNaN(p2)))
                    Assert.AreEqual(p1, p2, 0.00001, errorMessage);
            }
            else if (v1 is Result<bool>)
            {
                Assert.AreEqual(((Result<bool>)v1).Value, ((Result<bool>)v2).Value, errorMessage);
            }
            else if (v1 is ArrayResult<double>)
            {
                ArrayResult<double> da1 = (ArrayResult<double>)v1;
                ArrayResult<double> da2 = (ArrayResult<double>)v2;
                for (int i = 0; i < da1.Length; i++)
                {
                    if (!(double.IsNaN(da1[i])) && double.IsNaN(da2[i]))
                        Assert.AreEqual(da1[i], da2[i], 0.00001, errorMessage);
                }
            }
            else if (v1 is ArrayResult<int>)
            {
                var da1 = (ArrayResult<int>)v1;
                var da2 = (ArrayResult<int>)v2;
                for (int i = 0; i < da1.Length; i++)
                {
                    Assert.AreEqual(da1[i], da2[i], errorMessage);
                }
            }
        }

        [TestMethod()]
        public void TestImplementationIndependence()
        {
            var water1 = SomeoneBringMeSomeWater(ChemObjectBuilder.Instance);
            var water2 = SomeoneBringMeSomeWater(Silent.ChemObjectBuilder.Instance);

            var v1 = Descriptor.Calculate(water1).Value;
            var v2 = Descriptor.Calculate(water2).Value;

            string errorMessage = $"({Descriptor.GetType().ToString()}) The descriptor does not give the same results depending on the actual IChemObject implementation set (data, nonotify).";
            AssertEqualOutput(v1, v2, errorMessage);
        }

        [TestMethod()]
        public void TestAtomContainerHandling()
        {
            var water1 = SomeoneBringMeSomeWater(ChemObjectBuilder.Instance);
            // creates an AtomContainer with the atoms / bonds from water1
            var water2 = Silent.ChemObjectBuilder.Instance.NewAtomContainer();
            water2.Add(water1);

            var v1 = Descriptor.Calculate(water1).Value;
            var v2 = Descriptor.Calculate(water2).Value;

            string errorMessage = $"({Descriptor.GetType().ToString()}) The descriptor does not give the same results depending on it being passed an IAtomContainer or an IAtomContainer.";
            AssertEqualOutput(v1, v2, errorMessage);
        }

        /// <summary>
        /// Descriptors should not throw Exceptions on disconnected structures, but return NA instead.
        /// </summary>
        [TestMethod()]
        public void TestDisconnectedStructureHandling()
        {
            var disconnected = Silent.ChemObjectBuilder.Instance.NewAtomContainer();
            var chloride = new Atom("Cl") { FormalCharge = -1 };
            disconnected.Atoms.Add(chloride);
            var sodium = new Atom("Na") { FormalCharge = +1 };
            disconnected.Atoms.Add(sodium);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(disconnected);
            AddImplicitHydrogens(disconnected);

            var v1 = Descriptor.Calculate(disconnected).Value;
        }

        [TestMethod(), Ignore()]
        //[TestMethod()]
        private static int TestMethod()
        {
            throw new NotImplementedException();
        }

        public void TestTakeIntoAccountBondHybridization()
        {
            var builder = ChemObjectBuilder.Instance;
            var ethane1 = builder.NewAtomContainer();
            var c1 = builder.NewAtom("C");
            var c2 = builder.NewAtom("C");
            ethane1.Atoms.Add(c1);
            ethane1.Atoms.Add(c2);
            ethane1.AddBond(ethane1.Atoms[0], ethane1.Atoms[1], BondOrder.Double);

            var ethane2 = builder.NewAtomContainer();
            var c3 = builder.NewAtom("C");
            c3.Hybridization = Hybridization.SP2;
            var c4 = builder.NewAtom("C");
            c4.Hybridization = Hybridization.SP2;
            ethane2.Atoms.Add(c3);
            ethane2.Atoms.Add(c4);
            ethane2.AddBond(ethane2.Atoms[0], ethane2.Atoms[1], BondOrder.Single);

            var v1 = Descriptor.Calculate(ethane1).Value;
            var v2 = Descriptor.Calculate(ethane2).Value;

            string errorMessage = "("
                    + Descriptor.GetType().ToString()
                    + ") The descriptor does not give the same results depending on whether bond order or atom type are considered.";
            AssertEqualOutput(v1, v2, errorMessage);
        }

        private IAtomContainer SomeoneBringMeSomeWater(IChemObjectBuilder builder)
        {
            var mol = builder.NewAtomContainer();
            var c1 = builder.NewAtom("O");
            c1.Point3D = new Vector3(0.0, 0.0, 0.0);
            var h1 = builder.NewAtom("H");
            h1.Point3D = new Vector3(1.0, 0.0, 0.0);
            var h2 = builder.NewAtom("H");
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
