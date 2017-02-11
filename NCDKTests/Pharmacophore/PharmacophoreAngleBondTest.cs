/* Copyright (C) 2004-2008  Rajarshi Guha <rajarshi.guha@gmail.com>
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Pharmacophore
{
    // @cdk.module test-pcore
    [TestClass()]
    public class PharmacophoreAngleBondTest
    {
        [TestMethod()]
        public void TestGetAngle1()
        {
            PharmacophoreAtom patom1 = new PharmacophoreAtom("[CX2]N", "Amine", Vector3.Zero);
            PharmacophoreAtom patom2 = new PharmacophoreAtom("c1ccccc1", "Aromatic", new Vector3(1, 1, 1));
            PharmacophoreAtom patom3 = new PharmacophoreAtom("C", "Blah", new Vector3(2, 2, 2));
            PharmacophoreAngleBond pbond = new PharmacophoreAngleBond(patom1, patom2, patom3);
            Assert.AreEqual(180, pbond.BondLength, 0.00001);
        }

        [TestMethod()]
        public void TestGetAngle2()
        {
            PharmacophoreAtom patom1 = new PharmacophoreAtom("[CX2]N", "Amine", Vector3.Zero);
            PharmacophoreAtom patom2 = new PharmacophoreAtom("c1ccccc1", "Aromatic", new Vector3(1, 1, 1));
            PharmacophoreAtom patom3 = new PharmacophoreAtom("C", "Blah", Vector3.Zero);
            PharmacophoreAngleBond pbond = new PharmacophoreAngleBond(patom1, patom2, patom3);
            Assert.AreEqual(0, pbond.BondLength, 0.00001);
        }

        [TestMethod()]
        public void TestGetAngle3()
        {
            PharmacophoreAtom patom1 = new PharmacophoreAtom("[CX2]N", "Amine", new Vector3(0, 1, 0));
            PharmacophoreAtom patom2 = new PharmacophoreAtom("c1ccccc1", "Aromatic", Vector3.Zero);
            PharmacophoreAtom patom3 = new PharmacophoreAtom("C", "Blah", new Vector3(1, 0, 0));
            PharmacophoreAngleBond pbond = new PharmacophoreAngleBond(patom1, patom2, patom3);
            Assert.AreEqual(90.0, pbond.BondLength, 0.00001);
        }

        [TestMethod()]
        public void TestGetAngle4()
        {
            PharmacophoreAtom patom1 = new PharmacophoreAtom("[CX2]N", "Amine", new Vector3(1, 1, 0));
            PharmacophoreAtom patom2 = new PharmacophoreAtom("c1ccccc1", "Aromatic", Vector3.Zero);
            PharmacophoreAtom patom3 = new PharmacophoreAtom("C", "Blah", new Vector3(1, 0, 0));
            PharmacophoreAngleBond pbond = new PharmacophoreAngleBond(patom1, patom2, patom3);
            Assert.AreEqual(45.0, pbond.BondLength, 0.00001);
        }

        [TestMethod()]
        public void TestGetAngle5()
        {
            PharmacophoreAtom patom1 = new PharmacophoreAtom("[CX2]N", "Amine", new Vector3(1, 1, 1));
            PharmacophoreAtom patom2 = new PharmacophoreAtom("c1ccccc1", "Aromatic", Vector3.Zero);
            PharmacophoreAtom patom3 = new PharmacophoreAtom("C", "Blah", new Vector3(1, 0, 0));
            PharmacophoreAngleBond pbond = new PharmacophoreAngleBond(patom1, patom2, patom3);
            Assert.AreEqual(54.7356, pbond.BondLength, 0.0001);
        }
    }
}
