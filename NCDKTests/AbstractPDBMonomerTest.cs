/* Copyright (C) 2004-2007  Miguel Rojas <miguel.rojas@uni-koeln.de>
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
     * Checks the functionality of {@link IPDBMonomer} implementations.
     *
     * @cdk.module test-interfaces
     */
    [TestClass()]
    public abstract class AbstractPDBMonomerTest : AbstractMonomerTest
    {
        [TestMethod()]
        public virtual void TestSetICode_String()
        {
            IPDBMonomer monomer = (IPDBMonomer)NewChemObject();
            monomer.ICode = null;
            Assert.IsNull(monomer.ICode);
        }

        [TestMethod()]
        public virtual void TestGetICode()
        {
            IPDBMonomer monomer = (IPDBMonomer)NewChemObject();
            Assert.IsNull(monomer.ICode);
            monomer.ICode = "iCode";
            Assert.IsNotNull(monomer.ICode);
            Assert.AreEqual("iCode", monomer.ICode);
        }

        [TestMethod()]
        public virtual void TestSetChainID_String()
        {
            IPDBMonomer monomer = (IPDBMonomer)NewChemObject();
            monomer.ChainID = null;
            Assert.IsNull(monomer.ChainID);
        }

        [TestMethod()]
        public virtual void TestGetChainID()
        {
            IPDBMonomer monomer = (IPDBMonomer)NewChemObject();
            Assert.IsNull(monomer.ChainID);
            monomer.ChainID = "chainA";
            Assert.IsNotNull(monomer.ChainID);
            Assert.AreEqual("chainA", monomer.ChainID);
        }

        [TestMethod()]
        public virtual void TestSetResSeq_String()
        {
            IPDBMonomer monomer = (IPDBMonomer)NewChemObject();
            monomer.ResSeq = null;
            Assert.IsNull(monomer.ResSeq);
        }

        [TestMethod()]
        public virtual void TestGetResSeq()
        {
            IPDBMonomer monomer = (IPDBMonomer)NewChemObject();
            Assert.IsNull(monomer.ResSeq);
            monomer.ResSeq = "reqSeq";
            Assert.IsNotNull(monomer.ResSeq);
            Assert.AreEqual("reqSeq", monomer.ResSeq);
        }

        [TestMethod()]
        public override void TestToString()
        {
            IPDBMonomer monomer = (IPDBMonomer)NewChemObject();
            string description = monomer.ToString();
            for (int i = 0; i < description.Length; i++)
            {
                Assert.IsTrue('\n' != description[i]);
                Assert.IsTrue('\r' != description[i]);
            }
        }
    }
}
