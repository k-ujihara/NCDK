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
using System;

namespace NCDK.Beam
{
    /// <summary> <author>John May </author></summary>
    [TestClass()]
    public class ElementTest
    {
        [TestMethod()]
        public void OrganicSymbols()
        {
            Assert.AreEqual(Beam.Element.OfSymbol("B"), Beam.Element.Boron);
            Assert.AreEqual(Beam.Element.OfSymbol("C"), Beam.Element.Carbon);
            Assert.AreEqual(Beam.Element.OfSymbol("N"), Beam.Element.Nitrogen);
            Assert.AreEqual(Beam.Element.OfSymbol("O"), Beam.Element.Oxygen);
            Assert.AreEqual(Beam.Element.OfSymbol("P"), Beam.Element.Phosphorus);
            Assert.AreEqual(Beam.Element.OfSymbol("S"), Beam.Element.Sulfur);
            Assert.AreEqual(Beam.Element.OfSymbol("F"), Beam.Element.Fluorine);
            Assert.AreEqual(Beam.Element.OfSymbol("Br"), Beam.Element.Bromine);
            Assert.AreEqual(Beam.Element.OfSymbol("Cl"), Beam.Element.Chlorine);
            Assert.AreEqual(Beam.Element.OfSymbol("I"), Beam.Element.Iodine);
        }

        [TestMethod()]
        public void IsAromaticSymbols()
        {
            Assert.AreEqual(Beam.Element.OfSymbol("b"), Beam.Element.Boron);
            Assert.AreEqual(Beam.Element.OfSymbol("c"), Beam.Element.Carbon);
            Assert.AreEqual(Beam.Element.OfSymbol("n"), Beam.Element.Nitrogen);
            Assert.AreEqual(Beam.Element.OfSymbol("o"), Beam.Element.Oxygen);
            Assert.AreEqual(Beam.Element.OfSymbol("p"), Beam.Element.Phosphorus);
            Assert.AreEqual(Beam.Element.OfSymbol("s"), Beam.Element.Sulfur);
            Assert.AreEqual(Beam.Element.OfSymbol("se"), Beam.Element.Selenium);
            Assert.AreEqual(Beam.Element.OfSymbol("as"), Beam.Element.Arsenic);
        }

        [TestMethod()]
        public void Symbols()
        {
            foreach (var e in Element.Values)
            {
                Assert.AreEqual(Beam.Element.OfSymbol(e.Symbol), e);
            }
        }

        [TestMethod()]
        public void InvalidSymbol()
        {
            Assert.IsNull(Beam.Element.OfSymbol("J"));
        }

        [TestMethod()]
        public void IsOrganic()
        {
            foreach (var e in new[] {
                Beam.Element.Boron,
                Beam.Element.Carbon,
                Beam.Element.Nitrogen,
                Beam.Element.Oxygen,
                Beam.Element.Phosphorus,
                Beam.Element.Sulfur,
                Beam.Element.Fluorine,
                Beam.Element.Chlorine,
                Beam.Element.Bromine,
                Beam.Element.Iodine })
            {
                Assert.IsTrue(e.IsOrganic());
            }
        }

        [TestMethod()]
        public void IsAromatic()
        {
            foreach (var e in new[] 
                {
                    Beam.Element.Boron,
                    Beam.Element.Carbon,
                    Beam.Element.Nitrogen,
                    Beam.Element.Oxygen,
                    Beam.Element.Phosphorus,
                    Beam.Element.Sulfur,
                    Beam.Element.Selenium,
                    Beam.Element.Arsenic,
                })
            {
                Assert.IsTrue(e.IsAromatic());
            }
        }

        [TestMethod()]
        public void Verify()
        {
            foreach (var e in Element.Values)
            {
                bool valid = e.Verify(0, 0);
            }
        }

        [TestMethod()]
        public void OfNumber()
        {
            Assert.AreEqual(Beam.Element.OfNumber(6), Beam.Element.Carbon);
            Assert.AreEqual(Beam.Element.OfNumber(8), Beam.Element.Oxygen);
        }
    }
}