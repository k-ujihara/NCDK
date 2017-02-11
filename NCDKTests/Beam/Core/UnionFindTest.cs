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
    /// <summary> <author>John May </author>*/
    [TestClass()]
    public class UnionFindTest
    {
        [TestMethod()]
        public void Connected()
        {
            UnionFind uf = new UnionFind(100);
            uf.Union(1, 5);
            uf.Union(7, 9);
            uf.Union(7, 5);
            uf.Union(10, 11);
            uf.Union(11, 50);
            uf.Union(15, 1);
            uf.Union(15, 50);
            Assert.IsTrue(uf.Connected(1, 5));
            Assert.IsTrue(uf.Connected(1, 7));
            Assert.IsTrue(uf.Connected(1, 9));
            Assert.IsTrue(uf.Connected(1, 10));
            Assert.IsTrue(uf.Connected(1, 11));
            Assert.IsTrue(uf.Connected(1, 15));
            Assert.IsTrue(uf.Connected(1, 50));
        }

        [TestMethod()]
        public void Find()
        {
            UnionFind uf = new UnionFind(100);
            uf.Union(1, 5);
            uf.Union(7, 9);
            uf.Union(10, 11);
            uf.Union(15, 1);
            uf.Union(15, 50);
            Assert.AreEqual(uf.Find(1), 50);
            Assert.AreEqual(uf.Find(5), 50);
            Assert.AreEqual(uf.Find(7), 7);
            Assert.AreEqual(uf.Find(8), 8);
            Assert.AreEqual(uf.Find(10), 10);
            Assert.AreEqual(uf.Find(11), 10);
            Assert.AreEqual(uf.Find(15), 50);
            Assert.AreEqual(uf.Find(50), 50);
        }
    }
}
