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
    public class AtomCentricDBConfigTest
    {
        [TestMethod()]
        public void Difluoroethene_implConfig()
        {
            string input = "F[C@H]=[C@H]F";
            Graph g = Parser.Parse(input);
            Assert.AreEqual(g.TopologyOf(1).Configuration, Configuration.DB1);
            Assert.AreEqual(g.TopologyOf(2).Configuration, Configuration.DB1);
        }

        [TestMethod()]
        public void Difluoroethene_implConfig2()
        {
            string input = "F[C@@H]=[C@@H]F";
            Graph g = Parser.Parse(input);
            Assert.AreEqual(g.TopologyOf(1).Configuration, Configuration.DB2);
            Assert.AreEqual(g.TopologyOf(2).Configuration, Configuration.DB2);
        }

        [TestMethod()]
        public void Difluoroethene_expConfig()
        {
            string input = "F[C@DB1H]=[C@DB1H]F";
            Graph g = Parser.Parse(input);
            Assert.AreEqual(g.TopologyOf(1).Configuration, Configuration.DB1);
            Assert.AreEqual(g.TopologyOf(2).Configuration, Configuration.DB1);
        }

        [TestMethod()]
        public void Difluoroethene_expConfig2()
        {
            string input = "F[C@DB2H]=[C@DB2H]F";
            Graph g = Parser.Parse(input);
            Assert.AreEqual(g.TopologyOf(1).Configuration, Configuration.DB2);
            Assert.AreEqual(g.TopologyOf(2).Configuration, Configuration.DB2);
        }

        [TestMethod()]
        public void Difluoroethene()
        {
            GeneratorTest.RoundTrip("F[C@H]=[C@H]F");
            GeneratorTest.RoundTrip("F[C@@H]=[C@@H]F");
            GeneratorTest.RoundTrip("F[C@H]=[C@@H]F");
            GeneratorTest.RoundTrip("F[C@@H]=[C@H]F");
        }

        [TestMethod()]
        public void Difluoroethene_permute()
        {
            GeneratorTest.RoundTrip("F[C@H]=[C@H]F",
                                    new int[] { 1, 0, 2, 3 },
                                    "[C@@H](F)=[C@H]F");
            GeneratorTest.RoundTrip("[C@@H](F)=[C@H]F",
                                    new int[] { 1, 0, 2, 3 },
                                    "F[C@H]=[C@H]F");
        }
    }
}
