/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.QSAR.Results;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    /// TestSuite that runs all QSAR tests.
    /// </summary>
    // @cdk.module test-qsaratomic
    [TestClass()]
    public class DistanceToAtomDescriptorTest : AtomicDescriptorTest
    {
        public DistanceToAtomDescriptorTest()
        {
            SetDescriptor(typeof(DistanceToAtomDescriptor));
        }

        [TestMethod()]
        public void TestDistanceToAtomDescriptor()
        {
            IAtomicDescriptor descriptor = new DistanceToAtomDescriptor();
            descriptor.Parameters = new object[] { 2 };

            IAtomContainer mol = new AtomContainer();
            Atom a0 = new Atom("C");
            mol.Atoms.Add(a0);
            a0.Point3D = new Vector3(1.2492, -0.2810, 0.0000);
            Atom a1 = new Atom("C");
            mol.Atoms.Add(a1);
            a1.Point3D = new Vector3(0.0000, 0.6024, -0.0000);
            Atom a2 = new Atom("C");
            mol.Atoms.Add(a2);
            a2.Point3D = new Vector3(-1.2492, -0.2810, 0.0000);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single); // 1
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single); // 2
                                                                       // mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single); // 3 // jwmay: there is no atom at index 3

            Assert.AreEqual(2.46, ((DoubleResult)descriptor.Calculate(mol.Atoms[0], mol).Value).Value, 0.1);
        }
    }
}
