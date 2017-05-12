/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *  */

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK
{
    /// <summary>
    /// TestCase for <see cref="IMonomer"/> implementations.
    /// </summary>
    // @cdk.module test-interfaces
    // @author  Edgar Luttman <edgar@uni-paderborn.de>
    // @cdk.created 2001-08-09
    [TestClass()]
    public abstract class AbstractMonomerTest
        : AbstractAtomContainerTest
    {
        [TestMethod()]
        public virtual void TestSetMonomerName_String()
        {
            IMonomer m = (IMonomer)NewChemObject();
            m.MonomerName = "TRP279";
            Assert.AreEqual("TRP279", m.MonomerName);
        }

        [TestMethod()]
        public virtual void TestGetMonomerName()
        {
            TestSetMonomerName_String();
        }

        [TestMethod()]
        public virtual void TestSetMonomerType_String()
        {
            IMonomer oMonomer = (IMonomer)NewChemObject();
            oMonomer.MonomerType = "TRP";
            Assert.AreEqual("TRP", oMonomer.MonomerType);
        }

        [TestMethod()]
        public virtual void TestGetMonomerType()
        {
            TestSetMonomerType_String();
        }

        /// <summary>
        /// Method to test whether the class complies with RFC #9.
        /// </summary>
        [TestMethod()]
        public override void TestToString()
        {
            IMonomer oMonomer = (IMonomer)NewChemObject();
            oMonomer.MonomerType = "TRP";
            string description = oMonomer.ToString();
            for (int i = 0; i < description.Length; i++)
            {
                Assert.IsTrue('\n' != description[i]);
                Assert.IsTrue('\r' != description[i]);
            }
        }

        [TestMethod()]
        public override void TestClone()
        {
            IMonomer oMonomer = (IMonomer)NewChemObject();
            object clone = oMonomer.Clone();
            Assert.IsTrue(clone is IMonomer);
            Assert.AreNotSame(oMonomer, clone);
        }
    }
}