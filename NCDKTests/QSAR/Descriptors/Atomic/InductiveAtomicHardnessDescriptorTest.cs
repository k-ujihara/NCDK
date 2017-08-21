/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Hardware Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Hardware
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.QSAR.Results;

namespace NCDK.QSAR.Descriptors.Atomic
{
    // @cdk.module test-qsaratomic
    [TestClass()]
    public class InductiveAtomicHardnessDescriptorTest : AtomicDescriptorTest
    {
        public InductiveAtomicHardnessDescriptorTest()
        {
            SetDescriptor(typeof(InductiveAtomicHardnessDescriptor));
        }

        [TestMethod()]
        public void TestInductiveAtomicHardnessDescriptor()
        {
            double[] testResult = { 1.28 };

            Vector3 c_coord = new Vector3(1.392, 0.0, 0.0);
            Vector3 f_coord = new Vector3(0.0, 0.0, 0.0);
            Vector3 h1_coord = new Vector3(1.7439615035767404, 1.0558845107302222, 0.0);
            Vector3 h2_coord = new Vector3(1.7439615035767404, -0.5279422553651107, 0.914422809754875);
            Vector3 h3_coord = new Vector3(1.7439615035767402, -0.5279422553651113, -0.9144228097548747);

            IAtomContainer mol = new AtomContainer(); // molecule is CF

            Atom c = new Atom("C");
            mol.Atoms.Add(c);
            c.Point3D = c_coord;

            Atom f = new Atom("F");
            mol.Atoms.Add(f);
            f.Point3D = f_coord;

            Atom h1 = new Atom("H");
            mol.Atoms.Add(h1);
            h1.Point3D = h1_coord;

            Atom h2 = new Atom("H");
            mol.Atoms.Add(h2);
            h2.Point3D = h2_coord;

            Atom h3 = new Atom("H");
            mol.Atoms.Add(h3);
            h3.Point3D = h3_coord;

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single); // 1
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single); // 1
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single); // 1
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single); // 1

            IAtomicDescriptor descriptor = new InductiveAtomicHardnessDescriptor();

            double retval = ((DoubleResult)descriptor.Calculate(mol.Atoms[0], mol).Value).Value;
            Assert.AreEqual(testResult[0], retval, 0.1);
        }
    }
}
