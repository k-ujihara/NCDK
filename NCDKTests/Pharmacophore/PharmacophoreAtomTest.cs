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
    public class PharmacophoreAtomTest
    {
        [TestMethod()]
        public void TestGetterSetter()
        {
            PharmacophoreAtom patom = new PharmacophoreAtom("[CX2]N", "Amine", Vector3.Zero);
            Assert.AreEqual("[CX2]N", patom.Smarts);

            patom.Smarts = "[OX2]";
            Assert.AreEqual("[OX2]", patom.Smarts);
        }

        [TestMethod()]
        public void TestMatchingAtoms()
        {
            PharmacophoreAtom patom = new PharmacophoreAtom("[CX2]N", "Amine", Vector3.Zero);
            patom.SetMatchingAtoms(new int[] { 1, 4, 5 });
            int[] indices = patom.GetMatchingAtoms();
            Assert.AreEqual(1, indices[0]);
            Assert.AreEqual(4, indices[1]);
            Assert.AreEqual(5, indices[2]);
        }

        [TestMethod()]
        public void TestEquals()
        {
            PharmacophoreAtom patom1 = new PharmacophoreAtom("[CX2]N", "Amine", Vector3.Zero);
            patom1.SetMatchingAtoms(new int[] { 1, 4, 5 });

            PharmacophoreAtom patom2 = new PharmacophoreAtom("[CX2]N", "Amine", Vector3.Zero);
            patom2.SetMatchingAtoms(new int[] { 1, 4, 5 });

            PharmacophoreAtom patom3 = new PharmacophoreAtom("[CX2]N", "Amine", new Vector3(0, 1, 0));
            patom3.SetMatchingAtoms(new int[] { 1, 4, 5 });

            PharmacophoreAtom patom4 = new PharmacophoreAtom("[CX2]N", "Amine", Vector3.Zero);
            patom4.SetMatchingAtoms(new int[] { 1, 4, 6 });

            Assert.AreEqual(patom2, patom1);
            Assert.AreNotSame(patom3, patom1);
            Assert.AreNotSame(patom4, patom1);
        }
    }
}
