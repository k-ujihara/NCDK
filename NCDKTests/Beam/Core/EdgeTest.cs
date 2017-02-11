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
    /// <summary> <author>John May </author>*/
	[TestClass()]
    public class EdgeTest
    {

        [TestMethod()]
        public void Either()
        {
            Assert.AreEqual(new Edge(2, 3, Bond.Implicit).Either(), 2);
            Assert.AreEqual(new Edge(3, 2, Bond.Implicit).Either(), 3);
        }

        [TestMethod()]
        public void Other()
        {
            Assert.AreEqual(new Edge(2, 3, Bond.Implicit).Other(2), 3);
            Assert.AreEqual(new Edge(2, 3, Bond.Implicit).Other(3), 2);
            Assert.AreEqual(new Edge(3, 2, Bond.Implicit).Other(2), 3);
            Assert.AreEqual(new Edge(3, 2, Bond.Implicit).Other(3), 2);
        }

        // no longer thrown
        [ExpectedException(typeof(ArgumentException))]
        public void invalidEndpoint()
        {
            new Edge(2, 3, Bond.Implicit).Other(1);
        }

        [TestMethod()]
        public void BondTest()
        {
            Assert.AreEqual(new Edge(2, 3, Bond.Single).Bond, Bond.Single);
            Assert.AreEqual(new Edge(2, 3, Bond.Up).Bond, Bond.Up);
            Assert.AreEqual(new Edge(2, 3, Bond.Down).Bond, Bond.Down);
        }

        [TestMethod()]
        public void relativeBond()
        {
            Assert.AreEqual(new Edge(2, 3, Bond.Single).GetBond(2), Bond.Single);
            Assert.AreEqual(new Edge(2, 3, Bond.Single).GetBond(3), Bond.Single);
            Assert.AreEqual(new Edge(2, 3, Bond.Up).GetBond(2), Bond.Up);
            Assert.AreEqual(new Edge(2, 3, Bond.Up).GetBond(3), Bond.Down);
            Assert.AreEqual(new Edge(2, 3, Bond.Down).GetBond(2), Bond.Down);
            Assert.AreEqual(new Edge(2, 3, Bond.Down).GetBond(3), Bond.Up);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void invalidRelativeBond()
        {
            new Edge(2, 3, Bond.Implicit).GetBond(1);
        }

        [TestMethod()]
        public void UndirectedHashCode()
        {
            Assert.AreEqual(
                new Edge(1, 0, Bond.Implicit).GetHashCode(),
                new Edge(0, 1, Bond.Implicit).GetHashCode());
        }

        [TestMethod()]
        public void DirectedHashCode()
        {
            Assert.AreEqual(
                new Edge(1, 0, Bond.Down).GetHashCode(),
                new Edge(0, 1, Bond.Up).GetHashCode());
            Assert.AreEqual(
                new Edge(1, 0, Bond.Up).GetHashCode(),
                new Edge(0, 1, Bond.Up).GetHashCode());
        }

        [TestMethod()]
        public void UndirectedEquality()
        {
            Assert.AreEqual(
                new Edge(0, 1, Bond.Implicit),
                new Edge(0, 1, Bond.Implicit));
            Assert.AreEqual(
                new Edge(1, 0, Bond.Implicit),
                new Edge(0, 1, Bond.Implicit));
        }


        [TestMethod()]
        public void UndirectedInequality()
        {
            Assert.AreNotEqual(
                new Edge(0, 1, Bond.Double),
                new Edge(0, 1, Bond.Single));
            Assert.AreNotEqual(
                new Edge(1, 0, Bond.Single),
                new Edge(0, 1, Bond.Double));
        }

        [TestMethod()]
        public void DirectedEquality()
        {
            Assert.AreEqual(new Edge(0, 1, Bond.Up), new Edge(0, 1, Bond.Up));
            Assert.AreEqual(new Edge(0, 1, Bond.Up), new Edge(1, 0, Bond.Down));
            Assert.AreEqual(new Edge(1, 0, Bond.Down), new Edge(0, 1, Bond.Up));
            Assert.AreEqual(new Edge(1, 0, Bond.Down), new Edge(1, 0, Bond.Down));
        }

        [TestMethod()]
        public void DirectedInequality()
        {
            Assert.AreNotEqual(new Edge(0, 1, Bond.Up), new Edge(0, 1, Bond.Down));
            Assert.AreNotEqual(new Edge(0, 1, Bond.Up), new Edge(1, 0, Bond.Up));
            Assert.AreNotEqual(new Edge(1, 0, Bond.Up), new Edge(0, 1, Bond.Up));
            Assert.AreNotEqual(new Edge(1, 0, Bond.Down), new Edge(1, 0, Bond.Up));
        }
    }
}
