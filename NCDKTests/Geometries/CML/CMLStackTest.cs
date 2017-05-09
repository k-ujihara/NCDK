/* Copyright (C) 2006-2007  Egon Willighagen <egonw@users.sf.net>
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
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NCDK.IO.CML
{
    /// <summary>
    /// TestCase for the CMLStack class.
    /// </summary>
    // @cdk.module test-io
    [TestClass()]
    public class CMLStackTest : CDKTestCase
    {
        [TestMethod()]
        public void TestPush_String()
        {
            // the class has a hardcoded default length. Test going beyond this.
            CMLStack stack = new CMLStack();
            for (int i = 0; i < 100; i++)
            {
                stack.Push("element");
            }
        }

        [TestMethod()]
        public void TestPop()
        {
            CMLStack stack = new CMLStack();
            stack.Push("first");
            stack.Push("second");
            stack.Push("third");
            Assert.AreEqual("third", stack.Pop());
            Assert.AreEqual("second", stack.Pop());
            Assert.AreEqual("first", stack.Pop());
            try
            {
                Assert.AreEqual("doesNotExist", stack.Pop());
                Assert.Fail("Should have received an ArrayIndexOutOfBoundsException");
            }
            catch (Exception)
            {
                // OK, should happen
            }
        }

        [TestMethod()]
        public void TestCurrent()
        {
            CMLStack stack = new CMLStack();
            stack.Push("first");
            Assert.AreEqual("first", stack.Peek());
            stack.Push("second");
            Assert.AreEqual("second", stack.Peek());
            stack.Push("third");
            Assert.AreEqual("third", stack.Peek());
            stack.Pop();
            Assert.AreEqual("second", stack.Peek());
            stack.Pop();
            Assert.AreEqual("first", stack.Peek());
        }

        [TestMethod()]
        public void TestEndsWith_String()
        {
            CMLStack stack = new CMLStack();
            stack.Push("first");
            Assert.IsTrue(stack.EndsWith("first"));
            stack.Push("second");
            Assert.IsFalse(stack.EndsWith("first"));
            Assert.IsTrue(stack.EndsWith("second"));
            stack.Push("third");
            Assert.IsTrue(stack.EndsWith("third"));
        }

        [TestMethod()]
        public void TestEndsWith_String_String()
        {
            CMLStack stack = new CMLStack();
            stack.Push("first");
            stack.Push("second");
            Assert.IsFalse(stack.EndsWith("second", "first"));
            Assert.IsTrue(stack.EndsWith("first", "second"));
            stack.Push("third");
            Assert.IsTrue(stack.EndsWith("second", "third"));
        }

        [TestMethod()]
        public void TestEndsWith_String_String_String()
        {
            CMLStack stack = new CMLStack();
            stack.Push("first");
            stack.Push("second");
            stack.Push("third");
            Assert.IsTrue(stack.EndsWith("first", "second", "third"));
        }

        [TestMethod()]
        public void TestSize()
        {
            CMLStack stack = new CMLStack();
            Assert.AreEqual(0, stack.Count);
            stack.Push("first");
            Assert.AreEqual(1, stack.Count);
            stack.Push("second");
            Assert.AreEqual(2, stack.Count);
            stack.Push("third");
            Assert.AreEqual(3, stack.Count);
        }
    }
}
