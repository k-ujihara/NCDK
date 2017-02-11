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
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.Default
{
    /**
     * Checks the functionality of the AtomContainer.
     *
     * @cdk.module test-data
     */
    [TestClass()]
    public class AtomContainerTest
        : AbstractAtomContainerTest
    {
        public override IChemObject NewChemObject()
        {
            return new AtomContainer();
        }


        [TestMethod()]
        public virtual void TestAtomContainer()
        {
            // create an empty container with in the constructor defined array lengths
            IAtomContainer container = new AtomContainer();

            Assert.AreEqual(0, container.Atoms.Count);
            Assert.AreEqual(0, container.Bonds.Count);

            // test whether the ElectronContainer is correctly initialized
            container.Add(container.Builder.CreateBond(
                container.Builder.CreateAtom("C"),
                container.Builder.CreateAtom("C"), BondOrder.Double));
            container.Add(container.Builder.CreateLonePair(container.Builder.CreateAtom("N")));
        }

        [TestMethod()]
        public virtual void TestAtomContainer_IAtomContainer()
        {
            IAtomContainer acetone = new ChemObject().Builder.CreateAtomContainer();
            IAtom c1 = acetone.Builder.CreateAtom("C");
            IAtom c2 = acetone.Builder.CreateAtom("C");
            IAtom o = acetone.Builder.CreateAtom("O");
            IAtom c3 = acetone.Builder.CreateAtom("C");
            acetone.Add(c1);
            acetone.Add(c2);
            acetone.Add(c3);
            acetone.Add(o);
            IBond b1 = acetone.Builder.CreateBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.CreateBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.CreateBond(c1, c3, BondOrder.Single);
            acetone.Add(b1);
            acetone.Add(b2);
            acetone.Add(b3);

            IAtomContainer container = new AtomContainer(acetone);
            Assert.AreEqual(4, container.Atoms.Count);
            Assert.AreEqual(3, container.Bonds.Count);
        }
    }
}
