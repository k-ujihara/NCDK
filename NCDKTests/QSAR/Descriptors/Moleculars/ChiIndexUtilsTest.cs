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

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class ChiIndexUtilsTest : CDKTestCase
    {
        IChemObjectBuilder builder = CDK.Builder;

        [TestMethod()]
        public void TestDeltaVSulphurSO()
        {
            var s = builder.NewAtom("S");
            var o = builder.NewAtom("O");
            var b = builder.NewBond(s, o);
            b.Order = BondOrder.Double;

            var m = builder.NewAtomContainer();
            m.Atoms.Add(s);
            m.Atoms.Add(o);
            m.Bonds.Add(b);

            var deltav = ChiIndexUtils.DeltavSulphur(s, m);
            Assert.AreEqual(1.33, deltav, 0.01);
        }

        [TestMethod()]
        public void TestDeltaVSulphurSO2()
        {
            var s = builder.NewAtom("S");
            var o1 = builder.NewAtom("O");
            var o2 = builder.NewAtom("O");
            var b1 = builder.NewBond(s, o1);
            var b2 = builder.NewBond(s, o2);
            b1.Order = BondOrder.Double;
            b2.Order = BondOrder.Double;

            var m = builder.NewAtomContainer();
            m.Atoms.Add(s);
            m.Atoms.Add(o1);
            m.Bonds.Add(b1);
            m.Atoms.Add(o2);
            m.Bonds.Add(b2);

            var deltav = ChiIndexUtils.DeltavSulphur(s, m);
            Assert.AreEqual(2.67, deltav, 0.01);
        }
    }
}
