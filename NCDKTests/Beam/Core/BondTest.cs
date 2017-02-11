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
using static NCDK.Beam.Bond;

namespace NCDK.Beam
{
    /// <summary> <author>John May </author>*/
	[TestClass()]
    public class BondTest
    {

        [TestMethod()]
        public void dotElectrons()
        {
            Assert.AreEqual(Dot.Order, 0);
        }

        [TestMethod()]
        public void SingleElectrons()
        {
            Assert.AreEqual(Bond.Single.Order, 1);
        }

        [TestMethod()]
        public void doubleElectrons()
        {
            Assert.AreEqual(Bond.Double.Order, 2);
        }

        [TestMethod()]
        public void TripleElectrons()
        {
            Assert.AreEqual(Bond.Triple.Order, 3);
        }

        [TestMethod()]
        public void quadrupleElectrons()
        {
            Assert.AreEqual(Bond.Quadruple.Order, 4);
        }

        [TestMethod()]
        public void IsAromaticElectrons()
        {
            Assert.AreEqual(Bond.Aromatic.Order, 1);
        }

        [TestMethod()]
        public void upElectrons()
        {
            Assert.AreEqual(Bond.Up.Order, 1);
        }

        [TestMethod()]
        public void downElectrons()
        {
            Assert.AreEqual(Bond.Down.Order, 1);
        }

        [TestMethod()]
        public void dotInverse()
        {
            Assert.AreEqual(Bond.Dot.Inverse(), Bond.Dot);
        }

        [TestMethod()]
        public void SingleInverse()
        {
            Assert.AreEqual(Bond.Single.Inverse(), Bond.Single);
        }

        [TestMethod()]
        public void doubleInverse()
        {
            Assert.AreEqual(Bond.Double.Inverse(), Bond.Double);
        }

        [TestMethod()]
        public void TripleInverse()
        {
            Assert.AreEqual(Bond.Triple.Inverse(), Bond.Triple);
        }

        [TestMethod()]
        public void quadrupleInverse()
        {
            Assert.AreEqual(Bond.Quadruple.Inverse(), Bond.Quadruple);
        }

        [TestMethod()]
        public void IsAromaticInverse()
        {
            Assert.AreEqual(Bond.Aromatic.Inverse(), Bond.Aromatic);
        }

        [TestMethod()]
        public void upInverse()
        {
            Assert.AreEqual(Bond.Up.Inverse(), Bond.Down);
        }

        [TestMethod()]
        public void downInverse()
        {
            Assert.AreEqual(Bond.Down.Inverse(), Bond.Up);
        }

        [TestMethod()]
        public void implicitInverse()
        {
            Assert.AreEqual(Bond.Implicit.Inverse(), Bond.Implicit);
        }

        [TestMethod()]
        public void dotSymbol()
        {
            Assert.AreEqual(Bond.Dot.Token, ".");
        }

        [TestMethod()]
        public void SingleSymbol()
        {
            Assert.AreEqual(Bond.Single.Token, "-");
        }

        [TestMethod()]
        public void doubleSymbol()
        {
            Assert.AreEqual(Bond.Double.Token, "=");
        }

        [TestMethod()]
        public void TripleSymbol()
        {
            Assert.AreEqual(Bond.Triple.Token, "#");
        }

        [TestMethod()]
        public void quadrupleSymbol()
        {
            Assert.AreEqual(Bond.Quadruple.Token, "$");
        }

        [TestMethod()]
        public void IsAromaticSymbol()
        {
            Assert.AreEqual(Bond.Aromatic.Token, ":");
        }

        [TestMethod()]
        public void upSymbol()
        {
            Assert.AreEqual(Bond.Up.Token, "/");
        }

        [TestMethod()]
        public void downSymbol()
        {
            Assert.AreEqual(Bond.Down.Token, "\\");
        }

        [TestMethod()]
        public void implicitSymbol()
        {
            Assert.AreEqual(Bond.Implicit.Token, "");
        }
    }
}
