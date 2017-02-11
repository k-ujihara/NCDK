/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
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

namespace NCDK
{
    /**
     * Checks the functionality of {@link ILonePair} implementations.
     *
     * @see org.openscience.cdk.LonePair
     *
     * @cdk.module test-interfaces
     */
	[TestClass()]
    public abstract class AbstractLonePairTest
            : AbstractElectronContainerTest
    {

        [TestMethod()]
        public virtual void TestSetAtom_IAtom()
        {
            ILonePair lp = (ILonePair)NewChemObject();
            IAtom atom = lp.Builder.CreateAtom("N");
            lp.Atom = atom;
            Assert.AreEqual(atom, lp.Atom);
        }

        [TestMethod()]
        public virtual void TestGetAtom()
        {
            ILonePair lp = (ILonePair)NewChemObject();
            IAtom atom = lp.Builder.CreateAtom("N");
            Assert.IsNull(lp.Atom);
            lp.Atom = atom;
            Assert.AreEqual(atom, lp.Atom);
        }

        [TestMethod()]

        public override void TestGetElectronCount()
        {
            ILonePair lp = (ILonePair)NewChemObject();
            Assert.AreEqual(2, lp.ElectronCount.Value);

            lp = lp.Builder.CreateLonePair(lp.Builder.CreateAtom("N"));
            Assert.AreEqual(2, lp.ElectronCount.Value);
        }

        [TestMethod()]
        public virtual void TestContains_IAtom()
        {
            ILonePair lp = (ILonePair)NewChemObject();
            IAtom atom = lp.Builder.CreateAtom("N");
            lp.Atom = atom;
            Assert.IsTrue(lp.Contains(atom));
        }

        [TestMethod()]

        public override void TestClone()
        {
            ILonePair lp = (ILonePair)NewChemObject();
            object clone = lp.Clone();
            Assert.IsTrue(clone is ILonePair);
        }

        [TestMethod()]
        public virtual void TestClone_IAtom()
        {
            ILonePair lp = (ILonePair)NewChemObject();
            IAtom atom = lp.Builder.CreateAtom("N");
            lp.Atom = atom;

            // test cloning of atom
            ILonePair clone = (ILonePair)lp.Clone();
            Assert.AreNotSame(atom, clone.Atom);
        }

        /// <summary>Test for RFC #9</summary>
        [TestMethod()]

        public override void TestToString()
        {
            ILonePair lp = (ILonePair)NewChemObject();
            string description = lp.ToString();
            for (int i = 0; i < description.Length; i++)
            {
                Assert.IsTrue(description[i] != '\n');
                Assert.IsTrue(description[i] != '\r');
            }
        }

        /**
         * The electron count of an LP is always exactly 2.
         */
        [TestMethod()]
        public override void TestSetElectronCount_Integer()
        {
            IElectronContainer ec = (IElectronContainer)NewChemObject();
            ec.ElectronCount = 3;
            Assert.AreEqual(2, ec.ElectronCount.Value);
            ec.ElectronCount = null;
            Assert.AreEqual(2, ec.ElectronCount.Value);
        }

    }
}
