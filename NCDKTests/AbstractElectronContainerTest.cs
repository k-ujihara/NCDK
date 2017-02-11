/* Copyright (C) 2002-2007  The Chemistry Development Kit (CDK) project
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCDK
{
    /**
     * Checks the functionality of {@link IElectronContainer} implementations.
     *
     * @cdk.module test-interfaces
     *
     * @see org.openscience.cdk.ElectronContainer
     */
    [TestClass()]
    public abstract class AbstractElectronContainerTest : AbstractChemObjectTest
    {

        [TestMethod()]
        public virtual void TestSetElectronCount_Integer()
        {
            IElectronContainer ec = (IElectronContainer)NewChemObject();
            ec.ElectronCount = 3;
            Assert.AreEqual(3, ec.ElectronCount.Value);
        }

        [TestMethod()]
        public virtual void TestGetElectronCount()
        {
            TestSetElectronCount_Integer();
        }

        [TestMethod()]
        public override void TestClone()
        {
            IElectronContainer ec = (IElectronContainer)NewChemObject();
            ec.ElectronCount = 2;
            object clone = ec.Clone();
            Assert.IsNotNull(clone);
            Assert.IsTrue(clone is IElectronContainer);
        }

        /**
         * Method to test whether the class complies with RFC #9.
         */
        [TestMethod()]
        public virtual void TestToString()
        {
            IElectronContainer at = (IElectronContainer)NewChemObject();
            string description = at.ToString();
            for (int i = 0; i < description.Length; i++)
            {
                Assert.IsTrue(description[i] != '\n');
                Assert.IsTrue(description[i] != '\r');
            }
        }
    }
}
