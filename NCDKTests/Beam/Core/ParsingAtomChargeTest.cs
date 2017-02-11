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
    /// <summary>
    /// Unit tests verify correct handling of charge for bracket atoms.
    /// </summary>
    /// <author>John May</author>
	[TestClass()]
    public class ParsingAtomChargeTest
    {

        [TestMethod()]
        public void implicitPlusOne()
        {
            Verify("+", +1);
        }

        [TestMethod()]
        public void implicitPlusTwo()
        {
            Verify("++", +2);
        }

        [TestMethod()]
        public void implicitPlusThree()
        {
            Verify("+++", +3);
        }

        [TestMethod()]
        public void implicitPlusFour()
        {
            Verify("++++", +4);
        }

        [TestMethod()]
        public void implicitMinusOne()
        {
            Verify("-", -1);
        }

        [TestMethod()]
        public void implicitMinusTwo()
        {
            Verify("--", -2);
        }

        [TestMethod()]
        public void implicitMinusThree()
        {
            Verify("---", -3);
        }

        [TestMethod()]
        public void implicitMinusFour()
        {
            Verify("----", -4);
        }

        [TestMethod()]
        public void PlusOne()
        {
            Verify("+1", +1);
        }

        [TestMethod()]
        public void PlusTwo()
        {
            Verify("+2", +2);
        }

        [TestMethod()]
        public void MinusOne()
        {
            Verify("-1", -1);
        }

        [TestMethod()]
        public void MinusTwo()
        {
            Verify("-2", -2);
        }

        [TestMethod()]
        public void noCharge()
        {
            CharBuffer buffer = CharBuffer.FromString(":");
            Assert.AreEqual(Parser.ReadCharge(buffer), 0);
            Assert.IsTrue(buffer.NextIs(':'));
        }

        // really bad form but parsed okay
        [TestMethod()]
        public void MinusPlusOne()
        {
            Verify("-+1", 0);
        }

        // really bad form but parsed okay
        [TestMethod()]
        public void PlusPlusMinusOne()
        {
            Verify("++-1", +1);
        }

        // really bad form but parsed okay
        [TestMethod()]
        public void MinusMinusPlusOne()
        {
            Verify("--+1", -1);
        }

        // really bad form but parsed okay
        [TestMethod()]
        public void PlusMinusOne()
        {
            Verify("+-1", 0);
        }

        // really bad form but parsed okay
        [TestMethod()]
        public void PlusPlusOne()
        {
            Verify("++1", 2);
        }

        // really bad form but parsed okay
        [TestMethod()]
        public void PlusPlusTwo()
        {
            Verify("++2", 3);
        }

        // An implementation is required to accept charges in the range -15 to +15
        [TestMethod()]
        public void rangeCheck()
        {
            for (int i = -15; i <= 15; i++)
                Verify((i > 0 ? "+" : "") + i.ToString(), i);
        }

        private void Verify(string str, int charge)
        {
            Assert.AreEqual(Parser.ReadCharge(CharBuffer.FromString(str)), charge);
        }
    }
}
