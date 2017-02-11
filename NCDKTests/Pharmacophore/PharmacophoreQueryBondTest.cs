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
    public class PharmacophoreQueryBondTest
    {
        [TestMethod()]
        public void TestMatches()
        {
            PharmacophoreAtom patom1 = new PharmacophoreAtom("[CX2]N", "Amine", Vector3.Zero);
            PharmacophoreAtom patom2 = new PharmacophoreAtom("c1ccccc1", "Aromatic", new Vector3(1, 1, 1));
            PharmacophoreBond pbond = new PharmacophoreBond(patom1, patom2);

            PharmacophoreQueryAtom qatom1 = new PharmacophoreQueryAtom("Amine", "[CX2]N");
            PharmacophoreQueryAtom qatom2 = new PharmacophoreQueryAtom("aromatic", "c1ccccc1");
            PharmacophoreQueryBond qbond1 = new PharmacophoreQueryBond(qatom1, qatom2, 1.0, 2.0);
            PharmacophoreQueryBond qbond2 = new PharmacophoreQueryBond(qatom1, qatom2, 1.732);
            PharmacophoreQueryBond qbond3 = new PharmacophoreQueryBond(qatom1, qatom2, 0.1, 1.0);

            Assert.IsTrue(qbond1.Matches(pbond));
            Assert.IsTrue(qbond2.Matches(pbond));
            Assert.IsFalse(qbond3.Matches(pbond));
        }

        [TestMethod()]
        public void TestUpper()
        {
            PharmacophoreQueryAtom qatom1 = new PharmacophoreQueryAtom("Amine", "[CX2]N");
            PharmacophoreQueryAtom qatom2 = new PharmacophoreQueryAtom("aromatic", "c1ccccc1");
            PharmacophoreQueryBond qbond1 = new PharmacophoreQueryBond(qatom1, qatom2, 1.0, 2.0);
            PharmacophoreQueryBond qbond2 = new PharmacophoreQueryBond(qatom1, qatom2, 1.732);

            Assert.AreEqual(2.0, qbond1.GetUpper(), 0.01);
            Assert.AreEqual(1.732, qbond2.GetUpper(), 0.01);
        }

        [TestMethod()]
        public void TestLower()
        {
            PharmacophoreQueryAtom qatom1 = new PharmacophoreQueryAtom("Amine", "[CX2]N");
            PharmacophoreQueryAtom qatom2 = new PharmacophoreQueryAtom("aromatic", "c1ccccc1");
            PharmacophoreQueryBond qbond1 = new PharmacophoreQueryBond(qatom1, qatom2, 1.0, 2.0);
            PharmacophoreQueryBond qbond2 = new PharmacophoreQueryBond(qatom1, qatom2, 1.732);

            Assert.AreEqual(1.0, qbond1.GetLower(), 0.01);
            Assert.AreEqual(1.732, qbond2.GetLower(), 0.01);
        }

        [TestMethod()]
        public void TestToString()
        {
            PharmacophoreQueryAtom qatom1 = new PharmacophoreQueryAtom("Amine", "[CX2]N");
            PharmacophoreQueryAtom qatom2 = new PharmacophoreQueryAtom("aromatic", "c1ccccc1");
            PharmacophoreQueryBond qbond1 = new PharmacophoreQueryBond(qatom1, qatom2, 1.0, 2.0);
            string repr = qbond1.ToString();
            Assert.AreEqual("DC::Amine [[CX2]N]::aromatic [c1ccccc1]::[1.0 - 2.0] ", repr);
        }
    }
}
