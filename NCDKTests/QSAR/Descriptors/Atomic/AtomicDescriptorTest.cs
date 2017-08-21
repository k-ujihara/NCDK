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
using NCDK.Common.Base;
using NCDK.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Tools.Diff;
using System;
using System.Diagnostics;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    /// Tests for molecular descriptors.
    /// </summary>
    // @cdk.module test-qsaratomic
    [TestClass()]
    public abstract class AtomicDescriptorTest : DescriptorTest<IAtomicDescriptor>
    {
        protected IAtomicDescriptor descriptor;

        public AtomicDescriptorTest() { }

        /// <summary>
        /// </summary>
        /// <param name="descriptorClass">IAtomicDescriptor</param>
        public override void SetDescriptor(Type descriptorClass)
        {
            if (descriptor == null)
            {
                object descriptor = descriptorClass.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
                if (!(descriptor is IAtomicDescriptor))
                {
                    throw new CDKException("The passed descriptor class must be a IAtomicDescriptor");
                }
                this.descriptor = (IAtomicDescriptor)descriptor;
            }
            base.SetDescriptor(descriptorClass);
        }

        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestCalculate_IAtomContainer()
        {
            IAtomContainer mol = SomeoneBringMeSomeWater();

            DescriptorValue v = null;
            try
            {
                v = descriptor.Calculate(mol.Atoms[1], mol);
            }
            catch (Exception)
            {
                Assert.Fail("A descriptor must not throw an exception");
            }
            Assert.IsNotNull(v);
            Trace.Assert(v != null);
            Assert.AreNotEqual(0, v.Value.Length, "The descriptor did not calculate any value.");
        }

        /// <summary>
        /// Checks if the given labels are consistent.
        /// </summary>
        /// <exception cref="Exception">Passed on from calculate.</exception>
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestLabels()
        {
            IAtomContainer mol = SomeoneBringMeSomeWater();

            DescriptorValue v = descriptor.Calculate(mol.Atoms[1], mol);
            Assert.IsNotNull(v);
            string[] names = v.Names;
            Assert.IsNotNull(names, "The descriptor must return labels using the Names method.");
            Assert.AreNotEqual(0, names.Length, "At least one label must be given.");
            foreach (var name in names)
            {
                Assert.IsNotNull("A descriptor label may not be null.", name);
                Assert.AreNotEqual(0, name.Length, "The label string must not be empty.");
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
        [TestCategory("SlowTest")]
        public void TestNamesConsistency()
        {
            IAtomContainer mol = SomeoneBringMeSomeWater();

            string[] names1 = descriptor.DescriptorNames;
            DescriptorValue v = descriptor.Calculate(mol.Atoms[1], mol);
            string[] names2 = v.Names;

            Assert.AreEqual(names1.Length, names2.Length, $"({descriptor.GetType().FullName}) fails.");
            Assert.IsTrue(Compares.AreDeepEqual(names1, names2));

            int valueCount = v.Value.Length;
            Assert.AreEqual(valueCount, names1.Length);
        }

        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestCalculate_NoModifications()
        {
            IAtomContainer mol = SomeoneBringMeSomeWater();
            IAtom atom = mol.Atoms[1];
            IAtom clone = (IAtom)mol.Atoms[1].Clone();
            descriptor.Calculate(atom, mol);
            string diff = AtomDiff.Diff(clone, atom);
            Assert.AreEqual(0, diff.Length,
                $"The descriptor must not change the passed atom in any respect, but found this diff: {diff}");
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
