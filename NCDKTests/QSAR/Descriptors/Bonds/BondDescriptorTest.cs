/* Copyright (C) 2007-2008  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Common.Base;
using NCDK.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Tools.Diff.Tree;
using System;

namespace NCDK.QSAR.Descriptors.Bonds
{
    /// <summary>
    /// Tests for bond descriptors.
    /// </summary>
    // @cdk.module test-qsarbond
    [TestClass()]
    public abstract class BondDescriptorTest : DescriptorTest<IBondDescriptor>
    {
        protected IBondDescriptor descriptor;

        public BondDescriptorTest() { }

        public override void SetDescriptor(Type descriptorClass)
        {
            if (descriptor == null)
            {
                object descriptor = descriptorClass.GetConstructor(Type.EmptyTypes).Invoke(Array.Empty<object>());
                if (!(descriptor is IBondDescriptor))
                {
                    throw new CDKException("The passed descriptor class must be a IBondDescriptor");
                }
                this.descriptor = (IBondDescriptor)descriptor;
            }
            base.SetDescriptor(descriptorClass);
        }

        [TestMethod()]
        public void TestCalculate_IBond_IAtomContainer()
        {
            IAtomContainer mol = SomeoneBringMeSomeWater();

            DescriptorValue v = null;
            try
            {
                v = descriptor.Calculate(mol.Bonds[0], mol);
            }
            catch (Exception)
            {
                Assert.Fail("A descriptor must not throw an exception");
            }
            Assert.IsNotNull(v);
            Assert.AreNotEqual(0, v.Value.Length, "The descriptor did not calculate any value.");
        }

        /// <summary>
        /// Checks if the given labels are consistent.
        ///
        /// <exception cref="Exception">Passed on from calculate.</exception>
        /// </summary>
        [TestMethod()]
        public void TestLabels()
        {
            IAtomContainer mol = SomeoneBringMeSomeWater();

            DescriptorValue v = descriptor.Calculate(mol.Bonds[0], mol);
            Assert.IsNotNull(v);
            string[] names = v.Names;
            Assert.IsNotNull(names, "The descriptor must return labels using the Names method.");
            Assert.AreNotEqual(0, names.Length, "At least one label must be given.");
            for (int i = 0; i < names.Length; i++)
            {
                Assert.IsNotNull("A descriptor label may not be null.", names[i]);
                Assert.AreNotEqual(0, names[i].Length, "The label string must not be empty.");
                //            Console.Out.WriteLine("Label: " + names[i]);
            }
            Assert.IsNotNull(v.Value);
            int valueCount = v.Value.Length;
            Assert.AreEqual(names.Length, valueCount, "The number of labels must equals the number of values.");
        }

        /// <summary>
       /// Check if the names obtained directly from the descriptor without
       /// calculation match those obtained from the descriptor value object.
       /// Also ensure that the number of actual values matches the length
       /// of the names
       /// </summary>
        [TestMethod()]
        public void TestNamesConsistency()
        {
            IAtomContainer mol = SomeoneBringMeSomeWater();

            string[] names1 = descriptor.DescriptorNames;
            DescriptorValue v = descriptor.Calculate(mol.Bonds[1], mol);
            string[] names2 = v.Names;

            Assert.AreEqual(names1.Length, names2.Length);
            Assert.IsTrue(Compares.AreDeepEqual(names1, names2));

            int valueCount = v.Value.Length;
            Assert.AreEqual(valueCount, names1.Length);
        }

        [TestMethod()]
        public void TestCalculate_NoModifications()
        {
            IAtomContainer mol = SomeoneBringMeSomeWater();
            IBond bond = mol.Bonds[0];
            IBond clone = (IBond)mol.Bonds[0].Clone();
            descriptor.Calculate(bond, mol);
            string diff = BondDiff.Diff(clone, bond);
            Assert.AreEqual(0, diff.Length,
                $"({descriptor.GetType().FullName}) The descriptor must not change the passed bond in any respect, but found this diff: {diff}");
        }

        private IAtomContainer SomeoneBringMeSomeWater()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            IAtom c1 = Default.ChemObjectBuilder.Instance.NewAtom("O");
            c1.Point3D = new Vector3(0.0, 0.0, 0.0);
            IAtom h1 = Default.ChemObjectBuilder.Instance.NewAtom("H");
            h1.Point3D = new Vector3(1.0, 0.0, 0.0);
            IAtom h2 = Default.ChemObjectBuilder.Instance.NewAtom("H");
            h2.Point3D = new Vector3(-1.0, 0.0, 0.0);
            mol.Atoms.Add(c1);
            mol.Atoms.Add(h1);
            mol.Atoms.Add(h2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            return mol;
        }
    }
}
