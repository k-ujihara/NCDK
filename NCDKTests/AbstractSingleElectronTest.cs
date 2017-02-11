/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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
     * Checks the functionality of {@link ISingleElectron} implementations.
     *
     * @see org.openscience.cdk.SingleElectron
     *
     * @cdk.module test-interfaces
     */
    [TestClass()]
    public abstract class AbstractSingleElectronTest
        : AbstractElectronContainerTest
    {

        [TestMethod()]

        public override void TestGetElectronCount()
        {
            ISingleElectron radical = (ISingleElectron)NewChemObject();
            Assert.AreEqual(1, radical.ElectronCount.Value);
        }

        [TestMethod()]
        public virtual void TestContains_IAtom()
        {
            IChemObject obj = NewChemObject();
            IAtom atom = obj.Builder.CreateAtom("N");
            ISingleElectron radical = obj.Builder.CreateSingleElectron(atom);
            Assert.IsTrue(radical.Contains(atom));
        }

        [TestMethod()]
        public virtual void TestSetAtom_IAtom()
        {
            ISingleElectron radical = (ISingleElectron)NewChemObject();
            IAtom atom = radical.Builder.CreateAtom("N");
            Assert.IsNull(radical.Atom);
            radical.Atom = atom;
            Assert.AreEqual(atom, radical.Atom);
        }

        [TestMethod()]
        public virtual void TestGetAtom()
        {
            IChemObject obj = NewChemObject();
            IAtom atom = obj.Builder.CreateAtom("N");
            ISingleElectron radical = obj.Builder.CreateSingleElectron(atom);
            Assert.AreEqual(atom, radical.Atom);
        }

        [TestMethod()]

        public override void TestClone()
        {
            ISingleElectron radical = (ISingleElectron)NewChemObject();
            object clone = radical.Clone();
            Assert.IsNotNull(clone);
            Assert.IsTrue(clone is ISingleElectron);
        }

        [TestMethod()]
        public virtual void TestClone_IAtom()
        {
            ISingleElectron radical = (ISingleElectron)NewChemObject();
            IAtom atom = radical.Builder.CreateAtom("N");
            radical.Atom = atom;

            // test cloning of atom
            ISingleElectron clone = (ISingleElectron)radical.Clone();
            Assert.AreNotSame(atom, clone.Atom);
        }

        /// <summary>Test for RFC #9</summary>
        [TestMethod()]

        public override void TestToString()
        {
            ISingleElectron radical = (ISingleElectron)NewChemObject();
            string description = radical.ToString();
            for (int i = 0; i < description.Length; i++)
            {
                Assert.IsTrue(description[i] != '\n');
                Assert.IsTrue(description[i] != '\r');
            }
        }

        /**
         * The electron count of a single electron is always exactly 1.
         */
        [TestMethod()]

        public override void TestSetElectronCount_Integer()
        {
            IElectronContainer ec = (IElectronContainer)NewChemObject();
            ec.ElectronCount = 3;
            Assert.AreEqual(1, ec.ElectronCount.Value);
            ec.ElectronCount = null;
            Assert.AreEqual(1, ec.ElectronCount.Value);
        }
    }
}
