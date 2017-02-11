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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Pharmacophore
{
    // @cdk.module test-pcore
    [TestClass()]
    public class PharmacophoreQueryTest
    {
        private PharmacophoreQuery query;

        public PharmacophoreQueryTest()
        {
            query = new PharmacophoreQuery();

            PharmacophoreQueryAtom o = new PharmacophoreQueryAtom("D", "[OX1]");
            PharmacophoreQueryAtom n1 = new PharmacophoreQueryAtom("A", "[N]");
            PharmacophoreQueryAtom n2 = new PharmacophoreQueryAtom("A", "[N]");

            query.Atoms.Add(o);
            query.Atoms.Add(n1);
            query.Atoms.Add(n2);

            PharmacophoreQueryBond b1 = new PharmacophoreQueryBond(o, n1, 4.0, 4.5);
            PharmacophoreQueryBond b2 = new PharmacophoreQueryBond(o, n2, 4.0, 5.0);
            PharmacophoreQueryBond b3 = new PharmacophoreQueryBond(n1, n2, 5.4, 5.8);

            query.Bonds.Add(b1);
            query.Bonds.Add(b2);
            query.Bonds.Add(b3);
        }

        [TestMethod()]
        public void TestToString()
        {
            string repr = query.ToString();
            Assert.IsTrue(repr
                    .IndexOf(" #A:3, #EC:3, D, A, A, DC::D [[OX1]]::A [[N]]::[4.0 - 4.5] , DC::D [[OX1]]::A [[N]]::[4.0 - 5.0] , DC::A [[N]]::A [[N]]::[5.4 - 5.8] , )") > 0);
        }
    }
}
