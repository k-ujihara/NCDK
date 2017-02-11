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
    public class PharmacophoreQueryAngleBondTest
    {
        [TestMethod()]
        public void TestMatches()
        {
            PharmacophoreAtom patom1 = new PharmacophoreAtom("[CX2]N", "Amine", new Vector3(1, 1, 1));
            PharmacophoreAtom patom2 = new PharmacophoreAtom("c1ccccc1", "Aromatic", Vector3.Zero);
            PharmacophoreAtom patom3 = new PharmacophoreAtom("C", "Blah", new Vector3(1, 0, 0));
            PharmacophoreAngleBond pbond = new PharmacophoreAngleBond(patom1, patom2, patom3);

            PharmacophoreQueryAtom qatom1 = new PharmacophoreQueryAtom("Amine", "[CX2]N");
            PharmacophoreQueryAtom qatom2 = new PharmacophoreQueryAtom("aromatic", "c1ccccc1");
            PharmacophoreQueryAtom qatom3 = new PharmacophoreQueryAtom("blah", "C");
            PharmacophoreQueryAngleBond qbond1 = new PharmacophoreQueryAngleBond(qatom1, qatom2, qatom3, 54.735);
            PharmacophoreQueryAngleBond qbond2 = new PharmacophoreQueryAngleBond(qatom1, qatom2, qatom3, 50, 60);
            PharmacophoreQueryAngleBond qbond3 = new PharmacophoreQueryAngleBond(qatom1, qatom2, qatom3, 60, 80);
            PharmacophoreQueryBond qbond4 = new PharmacophoreQueryBond(qatom1, qatom2, 1, 2);

            Assert.IsTrue(qbond1.Matches(pbond));
            Assert.IsTrue(qbond2.Matches(pbond));
            Assert.IsFalse(qbond3.Matches(pbond));
            Assert.IsFalse(qbond4.Matches(pbond));
        }

        [TestMethod()]
        public void TestUpper()
        {
            PharmacophoreQueryAtom qatom1 = new PharmacophoreQueryAtom("Amine", "[CX2]N");
            PharmacophoreQueryAtom qatom2 = new PharmacophoreQueryAtom("aromatic", "c1ccccc1");
            PharmacophoreQueryAtom qatom3 = new PharmacophoreQueryAtom("blah", "C");
            PharmacophoreQueryAngleBond qbond1 = new PharmacophoreQueryAngleBond(qatom1, qatom2, qatom3, 54.735);
            PharmacophoreQueryAngleBond qbond2 = new PharmacophoreQueryAngleBond(qatom1, qatom2, qatom3, 50, 60);

            Assert.AreEqual(54.74, qbond1.GetUpper(), 0.01);
            Assert.AreEqual(60.00, qbond2.GetUpper(), 0.01);
        }

        [TestMethod()]
        public void TestLower()
        {
            PharmacophoreQueryAtom qatom1 = new PharmacophoreQueryAtom("Amine", "[CX2]N");
            PharmacophoreQueryAtom qatom2 = new PharmacophoreQueryAtom("aromatic", "c1ccccc1");
            PharmacophoreQueryAtom qatom3 = new PharmacophoreQueryAtom("blah", "C");
            PharmacophoreQueryAngleBond qbond1 = new PharmacophoreQueryAngleBond(qatom1, qatom2, qatom3, 54.735);
            PharmacophoreQueryAngleBond qbond2 = new PharmacophoreQueryAngleBond(qatom1, qatom2, qatom3, 50, 60);

            Assert.AreEqual(54.74, qbond1.GetLower(), 0.01);
            Assert.AreEqual(50.00, qbond2.GetLower(), 0.01);
        }

        [TestMethod()]
        public void TestToString()
        {
            PharmacophoreQueryAtom qatom1 = new PharmacophoreQueryAtom("Amine", "[CX2]N");
            PharmacophoreQueryAtom qatom2 = new PharmacophoreQueryAtom("aromatic", "c1ccccc1");
            PharmacophoreQueryAtom qatom3 = new PharmacophoreQueryAtom("blah", "C");
            PharmacophoreQueryAngleBond qbond1 = new PharmacophoreQueryAngleBond(qatom1, qatom2, qatom3, 54.735);
            string repr = qbond1.ToString();
            Assert.AreEqual(repr, "AC::Amine [[CX2]N]::aromatic [c1ccccc1]::blah [C]::[54.74 - 54.74] ");
        }
    }
}
