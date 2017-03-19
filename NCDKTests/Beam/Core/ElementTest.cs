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
using static NCDK.Beam.Element;

namespace NCDK.Beam
{
   /// <summary> <author>John May </author></summary>
    [TestClass()]
    public class ElementTest
    {

        [TestMethod()]
        public void OrganicSymbols()
        {
            Assert.AreEqual(Element.OfSymbol("B"), Boron);
            Assert.AreEqual(Element.OfSymbol("C"), Carbon);
            Assert.AreEqual(Element.OfSymbol("N"), Nitrogen);
            Assert.AreEqual(Element.OfSymbol("O"), Oxygen);
            Assert.AreEqual(Element.OfSymbol("P"), Phosphorus);
            Assert.AreEqual(Element.OfSymbol("S"), Sulfur);
            Assert.AreEqual(Element.OfSymbol("F"), Fluorine);
            Assert.AreEqual(Element.OfSymbol("Br"), Bromine);
            Assert.AreEqual(Element.OfSymbol("Cl"), Chlorine);
            Assert.AreEqual(Element.OfSymbol("I"), Iodine);
        }

        [TestMethod()]
        public void IsAromaticSymbols()
        {
            Assert.AreEqual(Element.OfSymbol("b"), Boron);
            Assert.AreEqual(Element.OfSymbol("c"), Carbon);
            Assert.AreEqual(Element.OfSymbol("n"), Nitrogen);
            Assert.AreEqual(Element.OfSymbol("o"), Oxygen);
            Assert.AreEqual(Element.OfSymbol("p"), Phosphorus);
            Assert.AreEqual(Element.OfSymbol("s"), Sulfur);
            Assert.AreEqual(Element.OfSymbol("se"), Selenium);
            Assert.AreEqual(Element.OfSymbol("as"), Arsenic);
        }

        [TestMethod()]
        public void Symbols()
        {
            foreach (var e in Element.Values)
            {
                Assert.AreEqual(Element.OfSymbol(e.Symbol), e);
            }
        }

        [TestMethod()]
        public void InvalidSymbol()
        {
            Assert.IsNull(Element.OfSymbol("J"));
        }

        [TestMethod()]
        public void IsOrganic()
        {
            foreach (var e in new[] {
                Boron,
                Carbon,
                Nitrogen,
                Oxygen,
                Phosphorus,
                Sulfur,
                Fluorine,
                Chlorine,
                Bromine,
                Iodine })
            {
                Assert.IsTrue(e.IsOrganic());
            }
        }

        [TestMethod()]
        public void IsAromatic()
        {
            foreach (var e in new[] { Boron,
                                           Carbon,
                                           Nitrogen,
                                           Oxygen,
                                           Phosphorus,
                                           Sulfur,
                                           Selenium,
                                           Arsenic })
            {
                Assert.IsTrue(e.IsAromatic());
            }
        }

        [TestMethod()]
        public void UnknownHydrogens()
        {
            Assert.AreEqual(Unknown.NumOfImplicitHydrogens(0), 0);
            Assert.AreEqual(Unknown.NumOfImplicitHydrogens(1), 0);
            Assert.AreEqual(Unknown.NumOfImplicitHydrogens(2), 0);
            Assert.AreEqual(Unknown.NumOfImplicitHydrogens(3), 0);
            Assert.AreEqual(Unknown.NumOfImplicitHydrogens(4), 0);
        }

        // now deprecated
        [ExpectedException(typeof(InvalidOperationException))]
        public void InorganicHydrogens()
        {
            Calcium.NumOfImplicitHydrogens(0);
        }

        [TestMethod()]
        public void BoronHydrogens()
        {
            Assert.AreEqual(Boron.NumOfImplicitHydrogens(0), 3);
            Assert.AreEqual(Boron.NumOfImplicitHydrogens(1), 2);
            Assert.AreEqual(Boron.NumOfImplicitHydrogens(2), 1);
            Assert.AreEqual(Boron.NumOfImplicitHydrogens(3), 0);
            Assert.AreEqual(Boron.NumOfImplicitHydrogens(4), 0);
        }

        [TestMethod()]
        public void CarbonHydrogens()
        {
            Assert.AreEqual(Carbon.NumOfImplicitHydrogens(0), 4);
            Assert.AreEqual(Carbon.NumOfImplicitHydrogens(1), 3);
            Assert.AreEqual(Carbon.NumOfImplicitHydrogens(2), 2);
            Assert.AreEqual(Carbon.NumOfImplicitHydrogens(3), 1);
            Assert.AreEqual(Carbon.NumOfImplicitHydrogens(4), 0);
            Assert.AreEqual(Carbon.NumOfImplicitHydrogens(5), 0);
            Assert.AreEqual(Carbon.NumOfImplicitHydrogens(6), 0);
        }

        [TestMethod()]
        public void NitrogenHydrogens()
        {
            Assert.AreEqual(Nitrogen.NumOfImplicitHydrogens(0), 3);
            Assert.AreEqual(Nitrogen.NumOfImplicitHydrogens(1), 2);
            Assert.AreEqual(Nitrogen.NumOfImplicitHydrogens(2), 1);
            Assert.AreEqual(Nitrogen.NumOfImplicitHydrogens(3), 0);
            Assert.AreEqual(Nitrogen.NumOfImplicitHydrogens(4), 1);
            Assert.AreEqual(Nitrogen.NumOfImplicitHydrogens(5), 0);
            Assert.AreEqual(Nitrogen.NumOfImplicitHydrogens(6), 0);
        }

        [TestMethod()]
        public void OxygenHydrogens()
        {
            Assert.AreEqual(Oxygen.NumOfImplicitHydrogens(0), 2);
            Assert.AreEqual(Oxygen.NumOfImplicitHydrogens(1), 1);
            Assert.AreEqual(Oxygen.NumOfImplicitHydrogens(2), 0);
            Assert.AreEqual(Oxygen.NumOfImplicitHydrogens(3), 0);
        }

        [TestMethod()]
        public void PhosphorusHydrogens()
        {
            Assert.AreEqual(Phosphorus.NumOfImplicitHydrogens(0), 3);
            Assert.AreEqual(Phosphorus.NumOfImplicitHydrogens(1), 2);
            Assert.AreEqual(Phosphorus.NumOfImplicitHydrogens(2), 1);
            Assert.AreEqual(Phosphorus.NumOfImplicitHydrogens(3), 0);
            Assert.AreEqual(Phosphorus.NumOfImplicitHydrogens(4), 1);
            Assert.AreEqual(Phosphorus.NumOfImplicitHydrogens(5), 0);
            Assert.AreEqual(Phosphorus.NumOfImplicitHydrogens(6), 0);
        }

        [TestMethod()]
        public void SulfurHydrogens()
        {
            Assert.AreEqual(Sulfur.NumOfImplicitHydrogens(0), 2);
            Assert.AreEqual(Sulfur.NumOfImplicitHydrogens(1), 1);
            Assert.AreEqual(Sulfur.NumOfImplicitHydrogens(2), 0);
            Assert.AreEqual(Sulfur.NumOfImplicitHydrogens(3), 1);
            Assert.AreEqual(Sulfur.NumOfImplicitHydrogens(4), 0);
            Assert.AreEqual(Sulfur.NumOfImplicitHydrogens(5), 1);
            Assert.AreEqual(Sulfur.NumOfImplicitHydrogens(6), 0);
            Assert.AreEqual(Sulfur.NumOfImplicitHydrogens(7), 0);
        }

        [TestMethod()]
        public void HalogenHydrogens()
        {
            foreach (var e in new[] { Fluorine, Chlorine, Bromine, Iodine })
            {
                Assert.AreEqual(e.NumOfImplicitHydrogens(0), 1);
                Assert.AreEqual(e.NumOfImplicitHydrogens(1), 0);
                Assert.AreEqual(e.NumOfImplicitHydrogens(2), 0);
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
            Assert.AreEqual(Element.OfNumber(6), Element.Carbon);
            Assert.AreEqual(Element.OfNumber(8), Element.Oxygen);
        }

        // Read() is no longer used.
        //[TestMethod()]
        //public void Read()
        //{
        //    foreach (var e in Element.Values)
        //    {
        //        if (e.IsAromatic())
        //            Assert.AreEqual(Element.Read(CharBuffer.FromString(e.Symbol)), e);
        //        Assert.AreEqual(Element.Read(CharBuffer.FromString(e.Symbol)), e);
        //    }
        //}

        //[TestMethod()]
        //public void ReadNone()
        //{
        //    Assert.IsNull(Element.Read(CharBuffer.FromString("")));
        //}

        //[TestMethod()]
        //public void ReadInvalidElement()
        //{
        //    Assert.IsNull(Element.Read(CharBuffer.FromString("J")));
        //}
    }
}