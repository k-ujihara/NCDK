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
    public class PharmacophoreQueryAtomTest
    {
        [TestMethod()]
        public void TestGetSmarts()
        {
            PharmacophoreQueryAtom qatom = new PharmacophoreQueryAtom("aromatic", "c1ccccc1");
            Assert.AreEqual("c1ccccc1", qatom.Smarts);
        }

        [TestMethod()]
        public void TestMatches()
        {
            PharmacophoreQueryAtom qatom = new PharmacophoreQueryAtom("aromatic", "c1ccccc1");

            PharmacophoreAtom patom1 = new PharmacophoreAtom("c1ccccc1", "aromatic", Vector3.Zero);
            PharmacophoreAtom patom2 = new PharmacophoreAtom("c1ccccc1", "hydrophobic", Vector3.Zero);
            PharmacophoreAtom patom3 = new PharmacophoreAtom("Cc1ccccc1", "aromatic", Vector3.Zero);
            PharmacophoreAtom patom4 = new PharmacophoreAtom("[CX2]N", "amine", Vector3.Zero);

            Assert.IsTrue(qatom.Matches(patom1));
            Assert.IsFalse(qatom.Matches(patom2));

            Assert.IsTrue(qatom.Matches(patom3));
            Assert.IsFalse(qatom.Matches(patom4));
        }

        [TestMethod()]
        public void TestToString()
        {
            PharmacophoreQueryAtom qatom = new PharmacophoreQueryAtom("aromatic", "c1ccccc1");
            string repr = qatom.ToString();
            Assert.AreEqual("aromatic [c1ccccc1]", repr);
        }
    }
}
