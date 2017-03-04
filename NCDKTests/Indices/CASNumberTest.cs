/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Indices
{
    /// <summary>
    /// Tests CDK's CAS Number class.
    /// </summary>
    // @cdk.module test-extra
    // @author     Egon Willighagen <egonw@sci.kun.nl>
    // @author Nathana&euml;l "M.Le_maudit" Mazuir
    // @cdk.created    2003-07-01
    [TestClass()]
    public class CASNumberTest : CDKTestCase
    {
        [TestMethod()]
        public void TestValidNumbers()
        {
            // valid cas numbers
            Assert.IsTrue(CASNumber.IsValid("50-00-0")); // formaldehyde
            Assert.IsTrue(CASNumber.IsValid("548-00-5"));
            Assert.IsTrue(CASNumber.IsValid("2622-26-6"));
            Assert.IsTrue(CASNumber.IsValid("15299-99-7"));
            Assert.IsTrue(CASNumber.IsValid("673434-32-7"));
        }

        [TestMethod()]
        public void TestInvalidCheckDigits()
        {
            // invalid R value
            Assert.IsFalse(CASNumber.IsValid("50-00-1"));
            Assert.IsFalse(CASNumber.IsValid("50-00-2"));
            Assert.IsFalse(CASNumber.IsValid("50-00-3"));
            Assert.IsFalse(CASNumber.IsValid("50-00-4"));
            Assert.IsFalse(CASNumber.IsValid("50-00-5"));
            Assert.IsFalse(CASNumber.IsValid("50-00-6"));
            Assert.IsFalse(CASNumber.IsValid("50-00-7"));
            Assert.IsFalse(CASNumber.IsValid("50-00-8"));
            Assert.IsFalse(CASNumber.IsValid("50-00-9"));
        }

        [TestMethod()]
        public void TestLargerThanFirst()
        {
            // valid format, but wrong number, the first is 50-00-0
            Assert.IsFalse(CASNumber.IsValid("3-21-4"));
        }

        [TestMethod()]
        public void TestWrongHyphenPositions()
        {
            // invalid format due to invalid hyphen positions
            Assert.IsFalse(CASNumber.IsValid("3-21-40"));
            Assert.IsFalse(CASNumber.IsValid("3-210-4"));
            Assert.IsFalse(CASNumber.IsValid("03-1-4"));
            Assert.IsFalse(CASNumber.IsValid("03-21-"));
        }

        [TestMethod()]
        public void TestInvalidCharacters()
        {
            // invalid characters
            Assert.IsFalse(CASNumber.IsValid("a-21-4"));
            Assert.IsFalse(CASNumber.IsValid("3-a1-4"));
            Assert.IsFalse(CASNumber.IsValid("3-2a-4"));
            Assert.IsFalse(CASNumber.IsValid("3-21-a"));
            Assert.IsFalse(CASNumber.IsValid("d-cb-a"));
        }

        [TestMethod()]
        public void TestSanity()
        {
            // completely stupid value
            Assert.IsFalse(CASNumber.IsValid("0&z003-!0>/-0a"));
        }

        [TestMethod()]
        public void TestCharacterSet()
        {
            // invalid value even with the '0' unicode character '\u0030'
            Assert.IsFalse(CASNumber.IsValid("\u0030-21-4"));
        }
    }
}
