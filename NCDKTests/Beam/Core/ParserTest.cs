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
    public class ParserTest
    {
        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void RingBondMismatch()
        {
            Parser.DecideBond(Bond.Single, Bond.Double, CharBuffer.FromString(""));
        }

        [TestMethod()]
        public void RingBondDecision()
        {
            Assert.AreEqual(Parser.DecideBond(Bond.Double, Bond.Double, CharBuffer.FromString("")), Bond.Double);
            Assert.AreEqual(Parser.DecideBond(Bond.Double, Bond.Implicit, CharBuffer.FromString("")), Bond.Double);
            Assert.AreEqual(Parser.DecideBond(Bond.Implicit, Bond.Double, CharBuffer.FromString("")), Bond.Double);
        }

        [TestMethod()]
        public void InvalidTetrahedral()
        {
            Graph g = Parser.Parse("[C@-](N)(O)C");
            Assert.AreEqual(g.TopologyOf(0), Topology.Unknown);
        }

        [TestMethod()]
        public void InvalidTetrahedral2()
        {
            Graph g = Parser.Parse("[C@](N)(O)C");
            Assert.AreEqual(g.TopologyOf(0), Topology.Unknown);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void UnclosedRing1()
        {
            Parser.Parse("C1CCCCC");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void UnclosedRing2()
        {
            Parser.Parse("C1CCCCC1CCCC1CCCC");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void UnclosedBranch1()
        {
            Parser.Parse("CCCC(CCCC");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void UnclosedBranch2()
        {
            Parser.Parse("CCCC(CCC(CC)");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void UnopenedBranch1()
        {
            Parser.Parse("CCCCCC)CCC");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void UnopenedBranch2()
        {
            Parser.Parse("CCCCCC))CCC");
        }

        [TestMethod()]
        public void Tellurophene()
        {
            Parser.Parse("c1cc[te]c1");
        }

        [TestMethod()]
        public void MixingAromaticAndKekule()
        {
            Graph g = Parser.Parse("C:1:C:C:C:C:C1");
            foreach (var e in g.Edges)
            {
                Assert.AreEqual(e.Bond, Bond.Aromatic);
            }
        }

        [TestMethod()]
        public void Hydrogen()
        {
            Graph g = Parser.Losse("HH");
            Assert.AreEqual(g.Order, 2);
            Assert.AreEqual(g.ToSmiles(), "[H][H]");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void Hydrogen_strict()
        {
            Graph g = Parser.GetStrict("HH");
        }

        [TestMethod()]
        public void Deuterium()
        {
            Graph g = Parser.Losse("DD");
            Assert.AreEqual(g.Order, 2);
            Assert.AreEqual(g.ToSmiles(), "[2H][2H]");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void Deuterium_strict()
        {
            Graph g = Parser.GetStrict("DD");
        }

        [TestMethod()]
        public void Tritium()
        {
            Graph g = Parser.Losse("TT");
            Assert.AreEqual(g.Order, 2);
            Assert.AreEqual(g.ToSmiles(), "[3H][3H]");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void Tritium_strict()
        {
            Graph g = Parser.GetStrict("TT");
        }

        [TestMethod()]
        public void Tellurium()
        {
            Graph g = Parser.Losse("[te]");
            Assert.IsTrue(g.GetAtom(0).IsAromatic());
            Assert.AreEqual(g.GetAtom(0).Element, Element.Tellurium);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void Tellurium_strict()
        {
            Graph g = Parser.GetStrict("[te]");
        }

        [TestMethod()]
        public void LargeRnum()
        {
            Graph g = Parser.Parse("C%99CCCC%99");
        }

        // not part of spec
        [TestMethod()]
        public void R_label()
        {
            Graph g = Parser.Parse("CC(C)C[R]");
            Assert.AreEqual(g.GetAtom(4).Label, "R");
        }

        [TestMethod()]
        public void Random_label()
        {
            Graph g = Parser.Parse("CC(C)C[Really?]");
            Assert.AreEqual(g.GetAtom(4).Label, "Really?");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void Bad_label()
        {
            Parser.Parse("[Nope-[not]-[ok]");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void Bad_label2()
        {
            Parser.Parse("[this-[is-not-okay]");
        }

        [TestMethod()]
        public void ParseCLockwiseExtendedTetrahedral()
        {
            Graph g = Graph.FromSmiles("C(C)=[C@@]=CC");
            Assert.AreEqual(g.TopologyOf(2).Configuration, Configuration.AL2);
        }

        [TestMethod()]
        public void ParseAnticlockwiseExtendedTetrahedral()
        {
            Graph g = Graph.FromSmiles("C(C)=[C@]=CC");
            Assert.AreEqual(g.TopologyOf(2).Configuration, Configuration.AL1);
        }

        // ek! what a difficult one - this example is from MetaCyc
        [TestMethod()]
        public void Nested_label()
        {
            Graph g = Parser.Parse("CCCCCCC=CCCCCCCCC=CC(=O)[a holo-[acyl-carrier protein]]");
            Assert.AreEqual(g.GetAtom(g.Order - 1).Label, "a holo-[acyl-carrier protein]");
        }

        [TestMethod()]
        public void SeleniumTh()
        {
            Assert.AreEqual(Parser.Parse("[Se@](=O)(C)CC").ToSmiles(), "[Se@](=O)(C)CC");
        }

        [TestMethod()]
        public void SulphurIonTh()
        {
            Assert.AreEqual(Parser.Parse("[S@+]([O-])(C)CC").ToSmiles(), "[S@+]([O-])(C)CC");
        }

        // chembl has some of these odditites, not sure which tool produced them
        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void RejectChEMBLBadBonds()
        {
            Parser.Parse("C\\=C");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void RejectMultipleUpBonds()
        {
            Parser.Parse("C/C=C(/C)/C");
        }

        [TestMethod()]
        public void AcceptMultipleBonds()
        {
            Parser.Parse("C/C=C/C\\C=C/C");
        }

        [TestMethod()]
        public void ParseTitleSpace()
        {
            Graph g = Parser.Parse("CCO ethanol");
            Assert.AreEqual("ethanol", g.Title);
        }

        [TestMethod()]
        public void ParseTitleTab()
        {
            Graph g = Parser.Parse("CCO\tethanol");
            Assert.AreEqual("ethanol", g.Title);
        }

        [TestMethod()]
        public void ParseTitleTabNewline()
        {
            Graph g = Parser.Parse("CCO\tethanol\n");
            Assert.AreEqual("ethanol", g.Title);
        }

        // this one has been mistreated... ignore for now
        [ExpectedException(typeof(InvalidSmilesException))]
        public void Chembl345045Mangleded()
        {
            Parser.Parse("c1c(ccc(c1)F)c2/c3n/c(c(\\c4[nH]c(/c(c/5\\nc(/c(c/6\\s\\c2\\cc6)/c7ccc(cc7)F)C=C5)/c8ccc(cc8)S(=O)(=O)[O-])cc4)/c9ccc(cc9)S(=O)(=O)[O-])/C=C3.[Na+].[Na+] CHEMBL345045");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void OpenBracketIsInvalid()
        {
            Parser.Parse("[");
        }
    }
}
