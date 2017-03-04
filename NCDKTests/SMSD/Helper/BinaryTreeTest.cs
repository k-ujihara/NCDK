/* Copyright (C) 2009-2010 Syed Asad Rahman <asad@ebi.ac.uk>
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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.SMSD.Helper
{
    /// <summary>
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    ///
    // @cdk.module test-smsd
    // @cdk.require java1.6+
    /// </summary>
    [TestClass()]
    public class BinaryTreeTest
    {

        public BinaryTreeTest() { }

        /// <summary>
        /// Test of getValue method, of class BinaryTree.
        /// </summary>
        [TestMethod()]
        public void TestGetValue()
        {
            BinaryTree instance = new BinaryTree(15);
            int expResult = 15;
            int result = instance.Value;
            Assert.AreEqual(expResult, result);
        }

        /// <summary>
        /// Test of getEqual method, of class BinaryTree.
        /// </summary>
        [TestMethod()]
        public void TestGetEqual()
        {
            BinaryTree instance = new BinaryTree(15);
            BinaryTree equal = new BinaryTree(15);
            instance.Equal = equal;
            instance.NotEqual = new BinaryTree(10);
            BinaryTree expResult = equal;
            BinaryTree result = instance.Equal;
            Assert.AreEqual(expResult, result);
        }

        /// <summary>
        /// Test of setEqual method, of class BinaryTree.
        /// </summary>
        [TestMethod()]
        public void TestSetEqual()
        {
            BinaryTree instance = new BinaryTree(15);
            BinaryTree equal = new BinaryTree(15);
            instance.Equal = equal;
            instance.NotEqual = new BinaryTree(10);
            BinaryTree expResult = equal;
            BinaryTree result = instance.Equal;
            Assert.AreEqual(expResult, result);
        }

        /// <summary>
        /// Test of getNotEqual method, of class BinaryTree.
        /// </summary>
        [TestMethod()]
        public void TestGetNotEqual()
        {
            BinaryTree instance = new BinaryTree(15);
            BinaryTree equal = new BinaryTree(15);
            BinaryTree notEqual = new BinaryTree(10);
            instance.Equal = equal;
            instance.NotEqual = notEqual;
            BinaryTree expResult = notEqual;
            BinaryTree result = instance.NotEqual;
            Assert.AreEqual(expResult, result);
        }

        /// <summary>
        /// Test of setNotEqual method, of class BinaryTree.
        /// </summary>
        [TestMethod()]
        public void TestSetNotEqual()
        {
            BinaryTree instance = new BinaryTree(15);
            BinaryTree equal = new BinaryTree(15);
            BinaryTree notEqual = new BinaryTree(10);
            instance.Equal = equal;
            instance.NotEqual = notEqual;
            BinaryTree expResult = notEqual;
            BinaryTree result = instance.NotEqual;
            Assert.AreEqual(expResult, result);
        }
    }
}
