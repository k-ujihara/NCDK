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
    public class ConfigurationTest
    {
        [TestMethod()]
        public void TetrahedralShorthand()
        {
            Assert.AreEqual(Configuration.AntiClockwise, Configuration.TH1.Shorthand);
            Assert.AreEqual(Configuration.Clockwise, Configuration.TH2.Shorthand);
        }

        [TestMethod()]
        public void TetrahedralType()
        {
            Assert.AreEqual(Configuration.Types.Tetrahedral, Configuration.TH1.Type);
            Assert.AreEqual(Configuration.Types.Tetrahedral, Configuration.TH2.Type);
        }

        [TestMethod()]
        public void Read()
        {
            foreach (var config in Configuration.Values)
            {
                Assert.AreEqual(config,
                    Configuration.Read(CharBuffer.FromString(config.Symbol)));
            }
        }

        [TestMethod()]
        public void ReadNone()
        {
            Assert.AreEqual(Configuration.Unknown, Configuration.Read(CharBuffer.FromString("]")));
        }

        [TestMethod()]
        public void ReadNone1()
        {
            Assert.AreEqual(Configuration.Unknown, Configuration.Read(CharBuffer.FromString("")));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void NoTHNumber()
        {
            Configuration.Read(CharBuffer.FromString("@TH"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void InvalidTHNumber()
        {
            Configuration.Read(CharBuffer.FromString("@TH5"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void NoSPNumber()
        {
            Configuration.Read(CharBuffer.FromString("@SP"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void InvalidSPNumber()
        {
            Configuration.Read(CharBuffer.FromString("@SP4"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void NoALNumber()
        {
            Configuration.Read(CharBuffer.FromString("@AL"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void InvalidALNumber()
        {
            Configuration.Read(CharBuffer.FromString("@AL3"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void NoTBNumber()
        {
            Configuration.Read(CharBuffer.FromString("@TB"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void InvalidLoTBNumber()
        {
            Configuration.Read(CharBuffer.FromString("@TB0"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void InvalidHiTBNumber()
        {
            Configuration.Read(CharBuffer.FromString("@TB21"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void NoOHNumber()
        {
            Configuration.Read(CharBuffer.FromString("@OH"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void InvalidLoOHNumber()
        {
            Configuration.Read(CharBuffer.FromString("@OH0"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void InvalidHiOHNumber()
        {
            Configuration.Read(CharBuffer.FromString("@OH31"));
        }

        [TestMethod()]
        public void AntiClockwise()
        {
            Configuration.Read(CharBuffer.FromString("@H"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void IncompleteTHorTB()
        {
            Configuration.Read(CharBuffer.FromString("@T"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void IncompleteSP()
        {
            Configuration.Read(CharBuffer.FromString("@S"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void IncompleteOH()
        {
            Configuration.Read(CharBuffer.FromString("@O"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void IncompleteAL()
        {
            Configuration.Read(CharBuffer.FromString("@A"));
        }
    }
}
