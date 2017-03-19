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
    public class CharBufferTest
    {
        [TestMethod()]
        public void EmptyBufferHasNoneRemaining()
        {
            CharBuffer buffer = CharBuffer.FromString("");
            Assert.IsFalse(buffer.HasRemaining());
        }

        [TestMethod()]
        public void NonEmptyBufferHasRemaining()
        {
            CharBuffer buffer = CharBuffer.FromString("-");
            Assert.IsTrue(buffer.HasRemaining());
        }

        [TestMethod()]
        public void EndOfBufferHasNoneRemaining()
        {
            CharBuffer buffer = CharBuffer.FromString("-");
            Assert.IsTrue(buffer.HasRemaining());
            buffer.Get();
            Assert.IsFalse(buffer.HasRemaining());
        }

        [TestMethod()]
        public void Position()
        {
            Assert.AreEqual(CharBuffer.FromString("").Position, 0);
            CharBuffer buffer = CharBuffer.FromString("...");
            Assert.AreEqual(buffer.Position, 0);
            Assert.AreEqual(buffer.Get(), '.');
            Assert.AreEqual(buffer.Position, 1);
            Assert.AreEqual(buffer.Get(), '.');
            Assert.AreEqual(buffer.Position, 2);
            Assert.AreEqual(buffer.Get(), '.');
            Assert.AreEqual(buffer.Position, 3);
        }

        [TestMethod()]
        public void Length()
        {
            Assert.AreEqual(CharBuffer.FromString("").Length, 0);
            Assert.AreEqual(CharBuffer.FromString(".").Length, 1);
            Assert.AreEqual(CharBuffer.FromString("..").Length, 2);
            Assert.AreEqual(CharBuffer.FromString("...").Length, 3);
        }

        [TestMethod()]
        public void GetProgressesPosition()
        {
            CharBuffer buffer = CharBuffer.FromString("abcd");
            Assert.AreEqual(buffer.Get(), 'a');
            Assert.AreEqual(buffer.Get(), 'b');
            Assert.AreEqual(buffer.Get(), 'c');
            Assert.AreEqual(buffer.Get(), 'd');
        }

        [TestMethod()]
        public void NextDoesNotProgressPosition()
        {
            CharBuffer buffer = CharBuffer.FromString("abcd");
            Assert.AreEqual(buffer.NextChar, 'a');
            Assert.AreEqual(buffer.Position, 0);
            Assert.AreEqual(buffer.NextChar, 'a');
            Assert.AreEqual(buffer.Position, 0);
            Assert.AreEqual(buffer.NextChar, 'a');
            Assert.AreEqual(buffer.Position, 0);
            Assert.AreEqual(buffer.NextChar, 'a');
            Assert.AreEqual(buffer.Position, 0);
            buffer.Get();
            Assert.AreEqual(buffer.Position, 1);
            Assert.AreEqual(buffer.NextChar, 'b');
            Assert.AreEqual(buffer.Position, 1);
            Assert.AreEqual(buffer.NextChar, 'b');
        }

        [TestMethod()]
        public void IsDigit()
        {
            for (char c = '0'; c <= '9'; c++)
                Assert.IsTrue(CharBuffer.IsDigit(c));
            for (char c = 'a'; c <= 'z'; c++)
                Assert.IsFalse(CharBuffer.IsDigit(c));
            for (char c = 'A'; c <= 'Z'; c++)
                Assert.IsFalse(CharBuffer.IsDigit(c));
        }

        [TestMethod()]
        public void ToDigit()
        {
            Assert.AreEqual(CharBuffer.ToDigit('0'), 0);
            Assert.AreEqual(CharBuffer.ToDigit('1'), 1);
            Assert.AreEqual(CharBuffer.ToDigit('2'), 2);
            Assert.AreEqual(CharBuffer.ToDigit('3'), 3);
            Assert.AreEqual(CharBuffer.ToDigit('4'), 4);
            Assert.AreEqual(CharBuffer.ToDigit('5'), 5);
            Assert.AreEqual(CharBuffer.ToDigit('6'), 6);
            Assert.AreEqual(CharBuffer.ToDigit('7'), 7);
            Assert.AreEqual(CharBuffer.ToDigit('8'), 8);
            Assert.AreEqual(CharBuffer.ToDigit('9'), 9);
        }

        [TestMethod()]
        public void NextIsDigit()
        {
            CharBuffer buffer = CharBuffer.FromString("c1");
            Assert.IsFalse(buffer.NextIsDigit());
            Assert.AreEqual(buffer.Get(), 'c');
            Assert.IsTrue(buffer.NextIsDigit());
            Assert.AreEqual(buffer.Get(), '1');
            Assert.IsFalse(buffer.NextIsDigit());
        }

        [TestMethod()]
        public void GetAsDigit()
        {
            CharBuffer buffer = CharBuffer.FromString("c1");
            Assert.IsFalse(buffer.NextIsDigit());
            Assert.AreEqual(buffer.Get(), 'c');
            Assert.IsTrue(buffer.NextIsDigit());
            Assert.AreEqual(buffer.GetAsDigit(), 1);
            Assert.IsFalse(buffer.NextIsDigit());
        }

        [TestMethod()]
        public void GetNextAsDigit()
        {
            CharBuffer buffer = CharBuffer.FromString("c1");
            Assert.IsFalse(buffer.NextIsDigit());
            Assert.AreEqual(buffer.Get(), 'c');
            Assert.IsTrue(buffer.NextIsDigit());
            Assert.AreEqual(buffer.GetNextAsDigit(), 1);
            Assert.IsTrue(buffer.NextIsDigit());
            Assert.AreEqual(buffer.GetAsDigit(), 1);
            Assert.IsFalse(buffer.NextIsDigit());
        }

        [TestMethod()]
        public void NextIsEmpty()
        {
            Assert.IsFalse(CharBuffer.FromString("").NextIs('?'));
        }

        [TestMethod()]
        public void NextIs()
        {
            CharBuffer buffer = CharBuffer.FromString("[C@H]");

            Assert.IsFalse(buffer.NextIs('C'));
            Assert.IsFalse(buffer.NextIs('@'));
            Assert.IsFalse(buffer.NextIs('H'));
            Assert.IsFalse(buffer.NextIs(']'));
            Assert.IsTrue(buffer.NextIs('['));
            Assert.AreEqual(buffer.Get(), '[');

            Assert.IsFalse(buffer.NextIs('['));
            Assert.IsFalse(buffer.NextIs('@'));
            Assert.IsFalse(buffer.NextIs('H'));
            Assert.IsFalse(buffer.NextIs(']'));
            Assert.IsTrue(buffer.NextIs('C'));
            Assert.AreEqual(buffer.Get(), 'C');

            Assert.IsFalse(buffer.NextIs('['));
            Assert.IsFalse(buffer.NextIs('C'));
            Assert.IsFalse(buffer.NextIs('H'));
            Assert.IsFalse(buffer.NextIs(']'));
            Assert.IsTrue(buffer.NextIs('@'));
            Assert.AreEqual(buffer.Get(), '@');

            Assert.IsFalse(buffer.NextIs('['));
            Assert.IsFalse(buffer.NextIs('C'));
            Assert.IsFalse(buffer.NextIs('@'));
            Assert.IsFalse(buffer.NextIs(']'));
            Assert.IsTrue(buffer.NextIs('H'));
            Assert.AreEqual(buffer.Get(), 'H');

            Assert.IsFalse(buffer.NextIs('['));
            Assert.IsFalse(buffer.NextIs('C'));
            Assert.IsFalse(buffer.NextIs('@'));
            Assert.IsFalse(buffer.NextIs('H'));
            Assert.IsTrue(buffer.NextIs(']'));
            Assert.AreEqual(buffer.Get(), ']');
        }

        [TestMethod()]
        public void GetSingleDigitNumber()
        {
            Assert.AreEqual(CharBuffer.FromString("1").GetNumber(), 1);
            CharBuffer buffer = CharBuffer.FromString("2C");
            Assert.AreEqual(buffer.GetNumber(), 2);
            Assert.AreEqual(buffer.NextChar, 'C');
        }

        [TestMethod()]
        public void GetTwoDigitNumber()
        {
            Assert.AreEqual(CharBuffer.FromString("12").GetNumber(), 12);
            CharBuffer buffer = CharBuffer.FromString("20C");
            Assert.AreEqual(buffer.GetNumber(), 20);
            Assert.AreEqual(buffer.NextChar, 'C');
        }

        [TestMethod()]
        public void GetThreeDigitNumber()
        {
            Assert.AreEqual(CharBuffer.FromString("123").GetNumber(), 123);
            CharBuffer buffer = CharBuffer.FromString("212C");
            Assert.AreEqual(buffer.GetNumber(), 212);
            Assert.AreEqual(buffer.NextChar, 'C');
        }

        [TestMethod()]
        public void GetThreeDigitNumber_2DigitsOnly()
        {
            Assert.AreEqual(CharBuffer.FromString("123").GetNumber(2), 12);
        }

        [TestMethod()]
        public void GetNumberWithLeadingZeros()
        {
            Assert.AreEqual(CharBuffer.FromString("0002").GetNumber(), 2);
            CharBuffer buffer = CharBuffer.FromString("002H");
            Assert.AreEqual(buffer.GetNumber(), 2);
            Assert.AreEqual(buffer.NextChar, 'H');
        }

        [TestMethod()]
        public void NonNumber()
        {
            CharBuffer buffer = CharBuffer.FromString("H3");
            Assert.AreEqual(buffer.GetNumber(), -1);
            Assert.AreEqual(buffer.NextChar, 'H');
        }
    }
}
