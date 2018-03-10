/*
 * Copyright (c) 2017 John Mayfield <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Default
{
    /// <summary>
    /// Checks the functionality of the AtomContainer.
    /// </summary>
    // @cdk.module test-data
    [TestClass()]
    public class AtomContainerTest2
        : AbstractAtomContainerTest
    {
        public override IChemObject NewChemObject()
        {
            return new AtomContainer2();
        }

        [TestMethod()]
        public virtual void TestAtomContainer()
        {
            // create an empty container with in the constructor defined array lengths
            IAtomContainer container = new AtomContainer2();

            Assert.AreEqual(0, container.Atoms.Count);
            Assert.AreEqual(0, container.Bonds.Count);

            // test whether the ElectronContainer is correctly initialized
            IAtom a1 = container.Builder.NewAtom("C");
            IAtom a2 = container.Builder.NewAtom("C");
            container.Atoms.Add(a1);
            container.Atoms.Add(a2);
            container.Bonds.Add(container.Builder.NewBond(
                a1, a2, BondOrder.Double));
            container.LonePairs.Add(container.Builder.NewLonePair(container.Builder.NewAtom("N")));
        }

        [TestMethod()]
        public virtual void TestAtomContainer_IAtomContainer()
        {
            IAtomContainer acetone = new ChemObject().Builder.NewAtomContainer();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            IAtomContainer container = new AtomContainer(acetone);
            Assert.AreEqual(4, container.Atoms.Count);
            Assert.AreEqual(3, container.Bonds.Count);
        }

        [TestMethod()]
        public void TestAtomGetBond()
        {
            IAtomContainer mol = (IAtomContainer)NewChemObject();
            IAtom a1 = mol.Builder.NewAtom();
            IAtom a2 = mol.Builder.NewAtom();
            IAtom a3 = mol.Builder.NewAtom();
            a1.Symbol ="CH3";
            a2.Symbol = "CH2";
            a3.Symbol = "OH";
            mol.Atoms.Add(a1);
            mol.Atoms.Add(a2);
            mol.Atoms.Add(a3);
            mol.AddBond(a1, a2, BondOrder.Single);
            mol.AddBond(a2, a3, BondOrder.Single);
            Assert.AreEqual(mol.Atoms[0].GetBond(mol.Atoms[1]), mol.Bonds[0]);
            Assert.AreEqual(mol.Atoms[1].GetBond(mol.Atoms[2]), mol.Bonds[1]);
            Assert.IsNull(mol.Atoms[0].GetBond(mol.Atoms[2]));
        }
    }
}
