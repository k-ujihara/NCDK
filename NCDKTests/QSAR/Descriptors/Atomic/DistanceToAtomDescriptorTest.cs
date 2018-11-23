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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Numerics;

namespace NCDK.QSAR.Descriptors.Atomic
{
    // @cdk.module test-qsaratomic
    [TestClass()]
    public class DistanceToAtomDescriptorTest : AtomicDescriptorTest<DistanceToAtomDescriptor>
    {
        [TestMethod()]
        public void TestDistanceToAtomDescriptor()
        {
            var mol = CDK.Builder.NewAtomContainer();
            var a0 = CDK.Builder.NewAtom("C");
            mol.Atoms.Add(a0);
            a0.Point3D = new Vector3(1.2492, -0.2810, 0.0000);
            var a1 = CDK.Builder.NewAtom("C");
            mol.Atoms.Add(a1);
            a1.Point3D = new Vector3(0.0000, 0.6024, -0.0000);
            var a2 = CDK.Builder.NewAtom("C");
            mol.Atoms.Add(a2);
            a2.Point3D = new Vector3(-1.2492, -0.2810, 0.0000);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single); // 1
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single); // 2
                                                                       // mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single); // 3 // jwmay: there is no atom at index 3
            var descriptor = CreateDescriptor(mol);
            Assert.AreEqual(2.46, descriptor.Calculate(mol.Atoms[0], focusPosition: 2).Value, 0.1);
        }
    }
}
