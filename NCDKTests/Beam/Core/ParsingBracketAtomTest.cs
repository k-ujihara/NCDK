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
    /// Unit tests for bracket atoms. Examples are lifted from the specification.
    /// </summary>
    // @author John May
    [TestClass()]
    public class ParsingBracketAtomTest
    {

        [TestMethod()]
        public void Uranium()
        {
            Assert.AreEqual(Parse("[U]"), GetAtom(Element.Uranium));
        }

        [TestMethod()]
        public void Lead()
        {
            Assert.AreEqual(Parse("[Pb]"), GetAtom(Element.Lead));
        }

        [TestMethod()]
        public void Helium()
        {
            Assert.AreEqual(Parse("[He]"), GetAtom(Element.Helium));
        }

        [TestMethod()]
        public void UnknownTest()
        {
            Assert.AreEqual(Parse("[*]"), GetAtom(Element.Unknown));
        }

        [TestMethod()]
        public void Identical()
        {
            Assert.AreEqual(Parse("[C]"), Parse("[CH0]"));
        }

        [TestMethod()]
        public void Identical2()
        {
            Assert.AreEqual(Parse("[CH]"), Parse("[CH1]"));
        }

        [TestMethod()]
        public void Methane()
        {
            Assert.AreEqual(Parse("[CH4]"), GetAtom(Element.Carbon, 4));
        }

        [TestMethod()]
        public void HydrochloricAcid()
        {
            Assert.AreEqual(Parse("[ClH]"), GetAtom(Element.Chlorine, 1));
        }

        [TestMethod()]
        public void HydrochloricAcid1()
        {
            Assert.AreEqual(Parse("[ClH1]"), GetAtom(Element.Chlorine, 1));
        }

        [TestMethod()]
        public void ChlorineAnion()
        {
            Assert.AreEqual(Parse("[Cl-]"), GetAtom(Element.Chlorine, 0, -1));
        }

        [TestMethod()]
        public void HydroxylAnion()
        {
            Assert.AreEqual(Parse("[OH1-]"), GetAtom(Element.Oxygen, 1, -1));
        }

        [TestMethod()]
        public void HydroxylAnionAlt()
        {
            Assert.AreEqual(Parse("[OH-1]"), GetAtom(Element.Oxygen, 1, -1));
        }

        [TestMethod()]
        public void CopperCation()
        {
            Assert.AreEqual(Parse("[Cu+2]"), GetAtom(Element.Copper, 0, +2));
        }

        [TestMethod()]
        public void CopperCationAlt()
        {
            Assert.AreEqual(Parse("[Cu++]"), GetAtom(Element.Copper, 0, +2));
        }

        [TestMethod()]
        public void MethaneIsotope()
        {
            Assert.AreEqual(Parse("[13CH4]"), GetAtom(13, Element.Carbon, 4, 0));
        }

        [TestMethod()]
        public void DeuteriumIon()
        {
            Assert.AreEqual(Parse("[2H+]"), GetAtom(2, Element.Hydrogen, 0, +1));
        }

        [TestMethod()]
        public void Uranium238Atom()
        {
            Assert.AreEqual(Parse("[238U]"), GetAtom(238, Element.Uranium, 0, 0));
        }

        // An isotope is interpreted as a number, so that [2H], [02H] and [002H] all mean deuterium.
        [TestMethod()]
        public void IsotopePadding()
        {
            Assert.AreEqual(Parse("[2H]"), Parse("[02H]"));
            Assert.AreEqual(Parse("[2H]"), Parse("[002H]"));
            Assert.AreEqual(Parse("[2H]"), Parse("[0002H]"));
        }

        [TestMethod()]
        public void Chlorine36()
        {
            Assert.AreEqual(Parse("[36Cl]"), GetAtom(36, Element.Chlorine, 0, 0));
        }

        // A general-purpose SMILES parser must accept at least three digits for the isotope and values from 0 to 999.
        [TestMethod()]
        public void RangeCheck()
        {
            for (int i = 0; i < 999; i++)
            {
                Assert.AreEqual(GetAtom(i, Element.Carbon, 0, 0), Parse("[" + i.ToString() + "C]"));
            }
        }

        [TestMethod()]
        public void MethaneAtomClassIs2()
        {
            Assert.AreEqual(Parse("[CH4:2]").AtomClass, 2);
        }

        private IAtom Parse(string str)
        {
            CharBuffer buffer = CharBuffer.FromString(str);
            return new Parser(buffer, false).Molecule().GetAtom(0);
        }

        private IAtom GetAtom(Element e)
        {
            return new AtomImpl.BracketAtom(e, 0, 0);
        }

        private IAtom GetAtom(Element e, int hCount)
        {
            return new AtomImpl.BracketAtom(e, hCount, 0);
        }

        private IAtom GetAtom(Element e, int hCount, int charge)
        {
            return new AtomImpl.BracketAtom(e, hCount, charge);
        }

        private IAtom GetAtom(int isotope, Element e, int hCount, int charge)
        {
            return new AtomImpl.BracketAtom(isotope, e, hCount, charge, 0, false);
        }
    }
}
