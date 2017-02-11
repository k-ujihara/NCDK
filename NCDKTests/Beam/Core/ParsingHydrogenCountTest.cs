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
    /// Unit tests verify correct handling of hydrogen count for bracket atoms.
    /// </summary>
    /// <author>John May</author>
	[TestClass()]
    public class ParsingHydrogenCountTest
    {
        [TestMethod()]
        public void impliedHCount()
        {
            Verify("H", 1);
        }

        [TestMethod()]
        public void H0()
        {
            Verify("H0", 0);
        }

        [TestMethod()]
        public void H1()
        {
            Verify("H1", 1);
        }

        [TestMethod()]
        public void H2()
        {
            Verify("H2", 2);
        }

        [TestMethod()]
        public void H3()
        {
            Verify("H3", 3);
        }

        [TestMethod()]
        public void H4()
        {
            Verify("H4", 4);
        }

        [TestMethod()]
        public void H42()
        {
            Verify("H42", 42);
        }

        [TestMethod()]
        public void H101()
        {
            Verify("H101", 101);
        }

        [TestMethod()]
        public void noHCount()
        {
            CharBuffer buffer = CharBuffer.FromString("-1");
            Assert.AreEqual(Parser.ReadHydrogens(buffer), 0);
            Assert.IsTrue(buffer.NextIs('-'));
        }

        private void Verify(string str, int count)
        {
            Assert.AreEqual(Parser.ReadHydrogens(CharBuffer.FromString(str)), count);
        }
    }
}

