/*
 * Copyright (c) 2013, European Bioinformatics Institute (EMBL-EBI)
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice, this
 *    list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 * The views and conclusions contained in the software and documentation are those
 * of the authors and should not be interpreted as representing official policies,
 * either expressed or implied, of the FreeBSD Project.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Beam
{
   /// <summary> <author>John May </author></summary>
    [TestClass()]
    public class IntStackTest
    {
        [TestMethod()]
        public void Push()
        {
            IntStack stack = new IntStack(4);
            stack.Push(1);
            Assert.AreEqual(stack.Peek(), 1);
            Assert.AreEqual(stack.Count, 1);
            stack.Push(2);
            Assert.AreEqual(stack.Peek(), 2);
            Assert.AreEqual(stack.Count, 2);
            stack.Push(4);
            Assert.AreEqual(stack.Peek(), 4);
            Assert.AreEqual(stack.Count, 3);
        }

        [TestMethod()]
        public void PushWithResize()
        {
            IntStack stack = new IntStack(1);
            stack.Push(1);
            Assert.AreEqual(stack.Peek(), 1);
            Assert.AreEqual(stack.Count, 1);
            stack.Push(2);
            Assert.AreEqual(stack.Peek(), 2);
            Assert.AreEqual(stack.Count, 2);
            stack.Push(4);
            Assert.AreEqual(stack.Peek(), 4);
            Assert.AreEqual(stack.Count, 3);
        }

        [TestMethod()]
        public void PushDuplicate()
        {
            IntStack stack = new IntStack(4);
            stack.Push(1);
            Assert.AreEqual(stack.Peek(), 1);
            Assert.AreEqual(stack.Count, 1);
            stack.Push(stack.Peek());
            Assert.AreEqual(stack.Peek(), 1);
            Assert.AreEqual(stack.Count, 2);
            stack.Push(stack.Peek());
            Assert.AreEqual(stack.Peek(), 1);
            Assert.AreEqual(stack.Count, 3);
        }

        [TestMethod()]
        public void Pop()
        {
            IntStack stack = new IntStack(4);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            Assert.AreEqual(stack.Pop(), 3);
            Assert.AreEqual(stack.Pop(), 2);
            Assert.AreEqual(stack.Pop(), 1);
        }

        [TestMethod()]
        public void PopWithResize()
        {
            IntStack stack = new IntStack(1);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            Assert.AreEqual(stack.Pop(), 3);
            Assert.AreEqual(stack.Pop(), 2);
            Assert.AreEqual(stack.Pop(), 1);
        }

        [TestMethod()]
        public void IsEmptyTest()
        {
            Assert.IsTrue(new IntStack(4).IsEmpty);
        }

        [TestMethod()]
        public void NonEmpty()
        {
            IntStack stack = new IntStack(4);
            Assert.IsTrue(stack.IsEmpty);
            stack.Push(1);
            Assert.IsFalse(stack.IsEmpty);
            stack.Pop();
            Assert.IsTrue(stack.IsEmpty);
        }

        [TestMethod()]
        public void CountTest()
        {
            Assert.AreEqual(new IntStack(4).Count, 0);
        }

        [TestMethod()]
        public void Clear()
        {
            IntStack stack = new IntStack(1);
            stack.Push(1);
            Assert.AreEqual(stack.Peek(), 1);
            Assert.AreEqual(stack.Count, 1);
            stack.Push(2);
            Assert.AreEqual(stack.Peek(), 2);
            Assert.AreEqual(stack.Count, 2);
            stack.Push(4);
            Assert.AreEqual(stack.Peek(), 4);
            Assert.AreEqual(stack.Count, 3);
            stack.Clear();
            Assert.AreEqual(stack.Count, 0);
            stack.Push(4);
            Assert.AreEqual(stack.Peek(), 4);
            Assert.AreEqual(stack.Count, 1);
            stack.Push(8);
            Assert.AreEqual(stack.Peek(), 8);
            Assert.AreEqual(stack.Count, 2);
            stack.Push(9);
            Assert.AreEqual(stack.Peek(), 9);
            Assert.AreEqual(stack.Count, 3);
        }
    }
}
