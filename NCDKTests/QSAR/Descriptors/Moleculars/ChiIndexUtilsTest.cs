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
        Default.ChemObjectBuilder builder;

        public ChiIndexUtilsTest()
        {
            builder = (Default.ChemObjectBuilder)Default.ChemObjectBuilder.Instance;
        }

        [TestMethod()]
        public void TestDeltaVSulphurSO()
        {
            IAtom s = builder.NewAtom("S");
            IAtom o = builder.NewAtom("O");
            IBond b = builder.NewBond(s, o);
            b.Order = BondOrder.Double;

            IAtomContainer m = builder.NewAtomContainer();
            m.Atoms.Add(s);
            m.Atoms.Add(o);
            m.Bonds.Add(b);

            double deltav = ChiIndexUtils.DeltavSulphur(s, m);
            Assert.AreEqual(1.33, deltav, 0.01);
        }

        [TestMethod()]
        public void TestDeltaVSulphurSO2()
        {
            IAtom s = builder.NewAtom("S");
            IAtom o1 = builder.NewAtom("O");
            IAtom o2 = builder.NewAtom("O");
            IBond b1 = builder.NewBond(s, o1);
            IBond b2 = builder.NewBond(s, o2);
            b1.Order = BondOrder.Double;
            b2.Order = BondOrder.Double;

            IAtomContainer m = builder.NewAtomContainer();
            m.Atoms.Add(s);
            m.Atoms.Add(o1);
            m.Bonds.Add(b1);
            m.Atoms.Add(o2);
            m.Bonds.Add(b2);

            double deltav = ChiIndexUtils.DeltavSulphur(s, m);
            Assert.AreEqual(2.67, deltav, 0.01);
        }
    }
}
