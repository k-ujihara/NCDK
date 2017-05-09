/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Default
{
    /// <summary>
    /// Checks the functionality of the Molecule class.
    /// </summary>
    /// <seealso cref="AtomContainer"/>  
    // @cdk.module test-data
    // @see org.openscience.cdk.Molecule
    [TestClass()]
    public class MoleculeTest : AbstractMoleculeTest
    {
        public override IChemObject NewChemObject()
        {
            return new AtomContainer();
        }

        // test constructors

        [TestMethod()]
        public virtual void TestMolecule()
        {
            IAtomContainer m = new AtomContainer();
            Assert.IsNotNull(m);
        }

        [TestMethod()]
        public virtual void TestMolecule_int_int_int_int()
        {
            IAtomContainer m = new AtomContainer();
            Assert.IsNotNull(m);
            Assert.AreEqual(0, m.Atoms.Count);
            Assert.AreEqual(0, m.Bonds.Count);
            Assert.AreEqual(0, m.LonePairs.Count);
            Assert.AreEqual(0, m.SingleElectrons.Count);
        }

        [TestMethod()]
        public virtual void TestMolecule_IAtomContainer()
        {
            IAtomContainer acetone = new AtomContainer();
            IAtom c1 = acetone.Builder.CreateAtom("C");
            IAtom c2 = acetone.Builder.CreateAtom("C");
            IAtom o = acetone.Builder.CreateAtom("O");
            IAtom c3 = acetone.Builder.CreateAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.CreateBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.CreateBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.CreateBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            IAtomContainer m = new AtomContainer(acetone);
            Assert.IsNotNull(m);
            Assert.AreEqual(4, m.Atoms.Count);
            Assert.AreEqual(3, m.Bonds.Count);
        }
    }
}
