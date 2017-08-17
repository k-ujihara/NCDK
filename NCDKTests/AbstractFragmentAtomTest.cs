/* Copyright (C) 2002-2007  Egon Willighagen <egonw@users.sf.net>
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
using System;

namespace NCDK
{
    /// <summary>
    /// Checks the functionality of <see cref="IFragmentAtom"/> implementations.
    /// </summary>
    // @cdk.module test-interfaces
    [TestClass()]
    public abstract class AbstractFragmentAtomTest
        : AbstractPseudoAtomTest
    {
        [TestMethod()]
        public virtual void TestGetFragment()
        {
            IFragmentAtom a = (IFragmentAtom)NewChemObject();
            // make sure that we start with a not-null, but empty container
            Assert.IsNotNull(a.Fragment);
            Assert.AreEqual(0, a.Fragment.Atoms.Count);
            Assert.AreEqual(0, a.Fragment.Bonds.Count);
        }

        [TestMethod()]
        public virtual void TestIsExpanded()
        {
            IFragmentAtom a = (IFragmentAtom)NewChemObject();
            Assert.IsNotNull(a);
            Assert.IsFalse(a.IsExpanded); // test the default state
        }

        [TestMethod()]
        public virtual void TestSetExpanded_bool()
        {
            IFragmentAtom a = (IFragmentAtom)NewChemObject();
            Assert.IsNotNull(a);
            a.IsExpanded = true;
            Assert.IsTrue(a.IsExpanded);
            a.IsExpanded = false;
            Assert.IsFalse(a.IsExpanded);
        }

        [TestMethod()]
        public virtual void TestSetFragment_IAtomContainer()
        {
            IFragmentAtom a = (IFragmentAtom)NewChemObject();
            Assert.IsNotNull(a);
            IAtomContainer container = a.Builder.NewAtomContainer();
            container.Atoms.Add(a.Builder.NewAtom("N"));
            container.Atoms.Add(a.Builder.NewAtom("C"));
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Triple);
            a.Fragment = container;
            Assert.AreEqual(container, a.Fragment);
        }

        [TestMethod()]

        public override void TestGetExactMass()
        {
            IFragmentAtom a = (IFragmentAtom)NewChemObject();
            Assert.IsNotNull(a);
            IAtomContainer container = a.Builder.NewAtomContainer();
            container.Atoms.Add(a.Builder.NewAtom("N"));
            container.Atoms[0].ExactMass = 5.5;
            container.Atoms.Add(a.Builder.NewAtom("C"));
            container.Atoms[1].ExactMass = 3.5;
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Triple);
            a.Fragment = container;
            Assert.AreEqual(9.0, a.ExactMass.Value, 0.0001);
        }

        /// <summary>Test for RFC #9</summary>
        [TestMethod()]

        public override void TestToString()
        {
            IFragmentAtom bond = (IFragmentAtom)NewChemObject();
            string description = bond.ToString();
            for (int i = 0; i < description.Length; i++)
            {
                Assert.IsTrue(description[i] != '\n');
                Assert.IsTrue(description[i] != '\r');
            }
        }

        /// <summary>
        /// Overwrites the <see cref="AbstractPseudoAtomTest"/> version.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public override void TestSetExactMass_Double()
        {
            IPseudoAtom atom = (IPseudoAtom)NewChemObject();
            atom.ExactMass = 12.001;
        }

        [TestMethod()]
        public override void TestClone_ExactMass()
        {
            // do not test this, as the exact mass is a implicit
            // property calculated from the fragment
        }
    }
}
