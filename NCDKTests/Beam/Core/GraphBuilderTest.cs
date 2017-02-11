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
using static NCDK.Beam.Configuration.DoubleBond;

namespace NCDK.Beam
{
    /// <summary> <author>John May </author>*/
    [TestClass()]
    public class GraphBuilderTest
    {
        [TestMethod()]
        public void Clockwise_parity()
        {

            GraphBuilder gb = GraphBuilder.Create(5);
            Graph g = gb.Add(AtomBuilder.Aliphatic("C").Build())
                                .Add(AtomImpl.AliphaticSubset.Nitrogen)
                                .Add(AtomImpl.AliphaticSubset.Oxygen)
                                .Add(AtomImpl.AliphaticSubset.Carbon)
                                .Add(AtomBuilder.ExplicitHydrogen())
                                .Add(0, 1)
                                .Add(0, 2)
                                .Add(0, 3)
                                .Add(0, 4)
                                .CreateTetrahedral(0).LookingFrom(1)
                                .Neighbors(2, 3, 4)
                                .Parity(1)
                                .Build()
                                .Build();

            Assert.AreEqual(g.ToSmiles(), "[C@@](N)(O)(C)[H]");
        }

        [TestMethod()]
        public void Anticlockwise_parity()
        {

            GraphBuilder gb = GraphBuilder.Create(5);
            Graph g = gb.Add(AtomBuilder.Aliphatic("C").Build())
                                .Add(AtomImpl.AliphaticSubset.Nitrogen)
                                .Add(AtomImpl.AliphaticSubset.Oxygen)
                                .Add(AtomImpl.AliphaticSubset.Carbon)
                                .Add(AtomBuilder.ExplicitHydrogen())
                                .Add(0, 1)
                                .Add(0, 2)
                                .Add(0, 3)
                                .Add(0, 4)
                                .CreateTetrahedral(0).LookingFrom(1)
                                .Neighbors(2, 3, 4)
                                .Parity(-1)
                                .Build()
                                .Build();

            Assert.AreEqual(g.ToSmiles(), "[C@](N)(O)(C)[H]");
        }

        [TestMethod()]
        public void e_1_2_difluroethene()
        {
            GraphBuilder gb = GraphBuilder.Create(5);
            Graph g = gb.Add(AtomImpl.AliphaticSubset.Fluorine)
                                .Add(AtomImpl.AliphaticSubset.Carbon)
                                .Add(AtomImpl.AliphaticSubset.Carbon)
                                .Add(AtomImpl.AliphaticSubset.Fluorine)
                                .Add(0, 1)
                                .ConnectWithDoubleBond(1, 2)
                                .Add(2, 3)
                                .Geometric(1, 2).Opposite(0, 3)
                                .Build();
            Assert.AreEqual(g.ToSmiles(), "F/C=C/F");
        }

        [TestMethod()]
        public void z_1_2_difluroethene()
        {
            GraphBuilder gb = GraphBuilder.Create(5);
            Graph g = gb.Add(AtomImpl.AliphaticSubset.Fluorine)
                                .Add(AtomImpl.AliphaticSubset.Carbon)
                                .Add(AtomImpl.AliphaticSubset.Carbon)
                                .Add(AtomImpl.AliphaticSubset.Fluorine)
                                .Add(0, 1)
                                .ConnectWithDoubleBond(1, 2)
                                .Add(2, 3)
                                .Geometric(1, 2).Together(0, 3)
                                .Build();
            Assert.AreEqual(g.ToSmiles(), "F/C=C\\F");
        }


        [TestMethod()]
        public void Conjugated_consider_existing()
        {
            // the second configuration considers the existing configuration
            GraphBuilder gb = GraphBuilder.Create(5);
            Graph g = gb.Add(AtomImpl.AliphaticSubset.Fluorine)
                                .Add(AtomImpl.AliphaticSubset.Carbon)
                                .Add(AtomImpl.AliphaticSubset.Carbon)
                                .Add(AtomImpl.AliphaticSubset.Carbon)
                                .Add(AtomImpl.AliphaticSubset.Carbon)
                                .Add(AtomImpl.AliphaticSubset.Fluorine)
                                .Add(0, 1)
                                .ConnectWithDoubleBond(1, 2)
                                .Add(2, 3)
                                .ConnectWithDoubleBond(3, 4)
                                .Add(4, 5)
                                .Geometric(1, 2).Together(0, 3)
                                .Geometric(3, 4).Together(2, 5)
                                .Build();
            Assert.AreEqual(g.ToSmiles(), "F/C=C\\C=C/F");
        }

        [TestMethod()]
        public void Conjugated_resolve_conflict()
        {
            // assigning the second one first means we have to consider this
            // on the first one
            GraphBuilder gb = GraphBuilder.Create(5);
            Graph g = gb.Add(AtomImpl.AliphaticSubset.Fluorine)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Fluorine)
                        .Add(0, 1)
                        .ConnectWithDoubleBond(1, 2)
                        .Add(2, 3)
                        .ConnectWithDoubleBond(3, 4)
                        .Add(4, 5)
                        .Geometric(3, 4).Together(2, 5)
                        .Geometric(1, 2).Together(0, 3)
                        .Build();
            Assert.AreEqual(g.ToSmiles(), "F\\C=C/C=C\\F");
        }

        [TestMethod()]
        public void Conjugated_resolve_conflict2()
        {
            // we assign the first, third then second - the second one cause
            // a conflict and we must flip one of the others
            GraphBuilder gb = GraphBuilder.Create(5);
            Graph g = gb.Add(AtomImpl.AliphaticSubset.Fluorine)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Fluorine)
                        .Add(0, 1)
                        .ConnectWithDoubleBond(1, 2)
                        .Add(2, 3)
                        .ConnectWithDoubleBond(3, 4)
                        .Add(4, 5)
                        .ConnectWithDoubleBond(5, 6)
                        .Add(6, 7)
                        .Geometric(1, 2).Opposite(0, 3)
                        .Geometric(5, 6).Together(4, 7)
                        .Geometric(3, 4).Together(2, 5)
                        .Build();
            Assert.AreEqual(g.ToSmiles(), "F/C=C/C=C\\C=C/F");
        }

        [TestMethod()]
        public void resolveConflict3()
        {
            GraphBuilder gb = GraphBuilder.Create(5);
            Graph g = gb.Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(0, 1)
                        .ConnectWithDoubleBond(1, 2)
                        .Add(2, 3)
                        .ConnectWithDoubleBond(3, 4)
                        .Add(4, 5)
                        .Add(3, 6)
                        .ConnectWithDoubleBond(6, 7)
                        .Add(7, 8)
                        .Geometric(1, 2).Configure(0, 3, Opposite)
                        .Geometric(7, 6).Configure(8, 3, Opposite)
                        .Geometric(3, 4).Configure(2, 5, Opposite)
                        .Build();
        }

        [TestMethod()]
        public void All_trans_octatetraene()
        {
            GraphBuilder gb = GraphBuilder.Create(5);
            Graph g = gb.Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(0, 1)
                        .ConnectWithDoubleBond(1, 2)
                        .Add(2, 3)
                        .ConnectWithDoubleBond(3, 4)
                        .Add(4, 5)
                        .ConnectWithDoubleBond(5, 6)
                        .Add(6, 7)
                        .ConnectWithDoubleBond(7, 0)
                        .Geometric(1, 2).Together(0, 3)
                        .Geometric(3, 4).Together(2, 5)
                        .Geometric(5, 6).Together(4, 7)
                        .Geometric(7, 0).Together(6, 1)
                        .Build();
            Assert.AreEqual(g.ToSmiles(), "C=1/C=C\\C=C/C=C\\C1");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void impossible_octatetraene()
        {
            GraphBuilder gb = GraphBuilder.Create(5);
            Graph g = gb.Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(0, 1)
                        .ConnectWithDoubleBond(1, 2)
                        .Add(2, 3)
                        .ConnectWithDoubleBond(3, 4)
                        .Add(4, 5)
                        .ConnectWithDoubleBond(5, 6)
                        .Add(6, 7)
                        .ConnectWithDoubleBond(7, 0)
                        .Geometric(1, 2).Together(0, 3)
                        .Geometric(3, 4).Opposite(2, 5)
                        .Geometric(5, 6).Together(4, 7)
                        .Geometric(7, 0).Together(6, 1)
                        .Build();
            Assert.AreEqual(g.ToSmiles(), "C=1/C=C\\C=C/C=C\\C1");
        }

        // example from: CHEBI:27711
        // C[C@]1(CC(O)=O)[C@H](CCC(O)=O)C2=C/c3[nH]c(Cc4[nH]c(c(CC(O)=O)c4CCC(O)=O)[C@](C)(O)[C@@]45N/C(=C\C1=N\2)[C@@H](CCC(O)=O)[C@]4(C)CC(=O)O5)c(CCC(O)=O)c3CC(O)=O
        [TestMethod()]
        public void CorrectCyclicDb()
        {
            // C\C=C/C1=C(CCC1)\C=C/C
            // 0 1 2 3  4 567   8 9 0
            GraphBuilder gb = GraphBuilder.Create(5);
            Graph g = gb.Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(AtomImpl.AliphaticSubset.Carbon)
                        .Add(0, 1)
                        .ConnectWithDoubleBond(1, 2)
                        .Add(2, 3)
                        .ConnectWithDoubleBond(3, 4)
                        .Add(4, 5)
                        .Add(5, 6)
                        .Add(6, 7)
                        .Add(7, 3)
                        .Add(4, 8)
                        .ConnectWithDoubleBond(8, 9)
                        .Add(9, 10)
                        .Geometric(1, 2).Opposite(0, 3)
                        .Geometric(8, 9).Opposite(4, 10)
                        .Geometric(3, 4).Opposite(2, 8)
                        .Build();
        }

        [TestMethod()]
        public void suppress_benzene()
        {
            GraphBuilder gb = GraphBuilder.Create(5);
            Assert.AreEqual("C=1C=CC=CC1",
              gb.Add(Element.Carbon, 1)
                .Add(Element.Carbon, 1)
                .Add(Element.Carbon, 1)
                .Add(Element.Carbon, 1)
                .Add(Element.Carbon, 1)
                .Add(Element.Carbon, 1)
                .Add(0, 1)
                .Add(1, 2, Bond.Double)
                .Add(2, 3)
                .Add(3, 4, Bond.Double)
                .Add(4, 5)
                .Add(5, 0, Bond.Double).Build().ToSmiles());
        }

        [TestMethod()]
        public void BuildExtendedTetrahedral()
        {
            GraphBuilder gb = GraphBuilder.Create(4);
            gb = gb.Add(Element.Carbon, 3)
                   .Add(Element.Carbon, 1)
                   .Add(Element.Carbon, 0)
                   .Add(Element.Carbon, 1)
                   .Add(Element.Carbon, 3)
                   .ConnectWithSingleBond(0, 1)
                   .ConnectWithDoubleBond(1, 2)
                   .ConnectWithDoubleBond(2, 3)
                   .ConnectWithSingleBond(3, 4)
                   .CreateExtendedTetrahedral(2).LookingFrom(1)
					.Neighbors(2, 3, 4)
					.Winding(Configuration.AL1)
					.Build();
        }
    }
}
