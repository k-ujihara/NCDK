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
using NCDK.Templates;
using NCDK.Tools.Diff;
using NCDK.Tools.Manipulator;
using System;
using System.Linq;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Tests for molecular descriptors.
    /// </summary>
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public abstract class MolecularDescriptorTest<T> : DescriptorTest<T> where T : IMolecularDescriptor
    {
        private static readonly DictionaryDatabase dictDB = new DictionaryDatabase();
        private static readonly EntryDictionary dict = dictDB.GetDictionary("descriptor-algorithms");

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
            var descriptor = CreateDescriptor(mol);
            try
            {
                descriptor.Calculate();
            }
            catch (ThreeDRequiredException)
            {
                // ignore
            }
            string name = descriptor.GetType().FullName;
            Assert.AreEqual(mflags, FlagsToInt(mol), $"{name}: Molecule flags were modified by descriptor!");
            Assert.IsTrue(Compares.AreDeepEqual(aflags, GetAtomFlags(mol)), $"{name}: Molecule's Atom flags were modified by descriptor!");
            Assert.IsTrue(Compares.AreDeepEqual(bflags, GetBondFlags(mol)), $"{name}: Molecule's Bond flags were modified by descriptor!");
        }

        [TestMethod()]
        public void TestCalculate_IAtomContainer()
        {
            var mol = SomeoneBringMeSomeWater();
            var v = CreateDescriptor(mol).Calculate();
            Assert.IsNotNull(v);
            Assert.AreNotEqual(0, v.Count, "The descriptor did not calculate any value.");
        }

        [TestMethod()]
        public void TestCalculate_NoModifications()
        {
            var mol = SomeoneBringMeSomeWater();
            var clone = (IAtomContainer)mol.Clone();
            CreateDescriptor(mol).Calculate();
            var diff = AtomContainerDiff.Diff(clone, mol);
            Assert.AreEqual(0, diff.Length, $"The descriptor must not change the passed molecule in any respect, but found this diff: {diff}");
        }

        /// <summary>
        /// Checks if the given labels are consistent.
        /// </summary>
        /// <exception cref="Exception">Passed on from calculate.</exception>
        [TestMethod()]
        public void TestLabels()
        {
            var mol = SomeoneBringMeSomeWater();

            var v = CreateDescriptor(mol).Calculate();
            Assert.IsNotNull(v);
            var names = v.Keys.ToReadOnlyList();
            Assert.IsNotNull(names, "The descriptor must return labels using the Names method.");
            Assert.AreNotEqual(0, names.Count, "At least one label must be given.");
            foreach (var name in names)
            {
                Assert.IsFalse(string.IsNullOrEmpty(name), "A descriptor label must not be null or empty.");
            }
            Assert.IsNotNull(v.Values);
            Assert.AreEqual(names.Count, v.Values.Count(), "The number of labels must equals the number of values.");
        }

        [TestMethod()]
        public void TestTakeIntoAccountImplicitHydrogens()
        {
            var builder = CDK.Builder;
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

            try
            {
                var v1 = CreateDescriptor(methane1).Calculate();
                var v2 = CreateDescriptor(methane2).Calculate();

                var errorMessage = $"({Descriptor.GetType().FullName}) The descriptor does not give the same results depending on whether hydrogens are implicit or explicit.";
                AssertEqualOutput(v1, v2, errorMessage);
            }
            catch (ThreeDRequiredException)
            {
                // ignore
            }
        }

        [TestMethod()]
        public void TestTakeIntoAccountImplicitHydrogensInEthane()
        {
            var builder = CDK.Builder;
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

            try
            {
                var v1 = CreateDescriptor(ethane1).Calculate();
                var v2 = CreateDescriptor(ethane2).Calculate();
                var errorMessage = $"({Descriptor.GetType().FullName}) The descriptor does not give the same results depending on whether hydrogens are implicit or explicit.";
                AssertEqualOutput(v1, v2, errorMessage);
            }
            catch (ThreeDRequiredException)
            {
                // ignore
            }
        }

        [TestMethod()]
        public void TestImplementationIndependence()
        {
            var water1 = SomeoneBringMeSomeWater();
            var water2 = SomeoneBringMeSomeWater();

            var v1 = CreateDescriptor(water1).Calculate();
            var v2 = CreateDescriptor(water2).Calculate();

            var errorMessage = $"({Descriptor.GetType().FullName}) The descriptor does not give the same results depending on the actual IChemObject implementation set (data, nonotify).";
            AssertEqualOutput(v1, v2, errorMessage);
        }

        [TestMethod()]
        public void TestAtomContainerHandling()
        {
            var water1 = SomeoneBringMeSomeWater();
            // creates an AtomContainer with the atoms / bonds from water1
            var water2 = CDK.Builder.NewAtomContainer();
            water2.Add(water1);

            try
            {
                var v1 = CreateDescriptor(water1).Calculate();
                var v2 = CreateDescriptor(water2).Calculate();

                var errorMessage = $"({Descriptor.GetType().FullName}) The descriptor does not give the same results depending on it being passed an IAtomContainer or an IAtomContainer.";
                AssertEqualOutput(v1, v2, errorMessage);
            }
            catch (ThreeDRequiredException)
            {
                // ignore
            }
        }

        /// <summary>
        /// Descriptors should not throw Exceptions on disconnected structures, but return NA instead.
        /// </summary>
        [TestMethod()]
        public void TestDisconnectedStructureHandling()
        {
            var disconnected = CDK.Builder.NewAtomContainer();
            var chloride = CDK.Builder.NewAtom("Cl");
            chloride.FormalCharge = -1;
            disconnected.Atoms.Add(chloride);
            var sodium = CDK.Builder.NewAtom("Na");
            sodium.FormalCharge = +1;
            disconnected.Atoms.Add(sodium);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(disconnected);
            AddImplicitHydrogens(disconnected);

            try
            {
                var v1 = CreateDescriptor(disconnected).Calculate();
            }
            catch (ThreeDRequiredException)
            {
                // ignore
            }
        }

        [TestMethod()]
        [Ignore()]
        public void TestTakeIntoAccountBondHybridization()
        {
            var builder = CDK.Builder;
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

            try
            {
                var v1 = CreateDescriptor(ethane1).Calculate();
                var v2 = CreateDescriptor(ethane2).Calculate();

                string errorMessage = $"({Descriptor.GetType().ToString()}) The descriptor does not give the same results depending on whether bond order or atom type are considered.";
                AssertEqualOutput(v1, v2, errorMessage);
            }
            catch (ThreeDRequiredException)
            {
                // ignore
            }
        }

        /// <summary>
        /// Checks that the results of the first and the second descriptor results are identical.
        /// </summary>
        /// <param name="v1">first <see cref="IDescriptorResult"/></param>
        /// <param name="v2">second <see cref="IDescriptorResult"/></param>
        /// <param name="errorMessage">error message to report when the results are not the same</param>
        private static void AssertEqualOutput(IDescriptorResult v1, IDescriptorResult v2, string errorMessage)
        {
            var vv1 = v1.Values.ToReadOnlyList();
            var vv2 = v2.Values.ToReadOnlyList();

            Assert.AreEqual(vv1.Count, vv2.Count);

            for (int i = 0; i < vv1.Count; i++)
            {
                var p1 = vv1[i];
                var p2 = vv2[i];
                switch (p1)
                {
                    case int p:
                        {
                            var pp = (int)p2;
                            Assert.AreEqual(p, pp, errorMessage);
                        }
                        break;
                    case double p:
                        {
                            var pp = (double)p2;
                            if (!(double.IsNaN(p) && double.IsNaN(pp)))
                                Assert.AreEqual(p, pp, 0.00001, errorMessage);
                        }
                        break;
                    case bool p:
                        {
                            var pp = (bool)p2;
                            Assert.AreEqual(p, pp, errorMessage);
                        }
                        break;
                }
            }
        }

        protected IAtomContainer SomeoneBringMeSomeWater()
        {
            var mol = TestMoleculeFactory.MakeWater();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddImplicitHydrogens(mol);
            return mol;
        }
    }
}
