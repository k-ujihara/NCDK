/* Copyright (C) 2009-2010 maclean {gilleain.torrance@gmail.com}
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Signatures
{
    // @cdk.module test-signature
    // @author maclean
    [TestClass()]
    public class SignatureQuotientGraphTest : AbstractSignatureTest
    {
        [TestMethod()]
        public void IsConnectedTest()
        {
            IAtomContainer singleBond = builder.NewAtomContainer();
            singleBond.Atoms.Add(builder.NewAtom("C"));
            singleBond.Atoms.Add(builder.NewAtom("C"));
            singleBond.AddBond(singleBond.Atoms[0], singleBond.Atoms[1], BondOrder.Single);
            SignatureQuotientGraph quotientGraph = new SignatureQuotientGraph(singleBond);
            Assert.IsTrue(quotientGraph.IsConnected(0, 1));
        }

        public void CheckParameters(SignatureQuotientGraph qGraph, int expectedVertexCount, int expectedEdgeCount,
                int expectedLoopEdgeCount)
        {
            Assert.AreEqual(expectedVertexCount, qGraph.GetVertexCount());
            Assert.AreEqual(expectedEdgeCount, qGraph.GetEdgeCount());
            Assert.AreEqual(expectedLoopEdgeCount, qGraph.NumberOfLoopEdges());
        }

        [TestMethod()]
        public void TestCubane()
        {
            IAtomContainer cubane = AbstractSignatureTest.MakeCubane();
            SignatureQuotientGraph qGraph = new SignatureQuotientGraph(cubane);
            CheckParameters(qGraph, 1, 1, 1);
        }

        [TestMethod()]
        public void TestCuneaneAtHeight1()
        {
            IAtomContainer cuneane = AbstractSignatureTest.MakeCuneane();
            SignatureQuotientGraph qGraph = new SignatureQuotientGraph(cuneane, 1);
            CheckParameters(qGraph, 1, 1, 1);
        }

        [TestMethod()]
        public void TestCuneaneAtHeight2()
        {
            IAtomContainer cuneane = AbstractSignatureTest.MakeCuneane();
            SignatureQuotientGraph qGraph = new SignatureQuotientGraph(cuneane, 2);
            CheckParameters(qGraph, 3, 5, 3);
        }

        [TestMethod()]
        public void TestPropellane()
        {
            IAtomContainer propellane = AbstractSignatureTest.MakePropellane();
            SignatureQuotientGraph qGraph = new SignatureQuotientGraph(propellane);
            CheckParameters(qGraph, 2, 2, 1);
        }

        [TestMethod()]
        public void TestTwistane()
        {
            IAtomContainer twistane = AbstractSignatureTest.MakeTwistane();
            SignatureQuotientGraph qGraph = new SignatureQuotientGraph(twistane);
            CheckParameters(qGraph, 3, 4, 2);
        }

        [TestMethod()]
        public void TestC7H16Isomers()
        {
            IAtomContainer c7H16A = AbstractSignatureTest.MakeC7H16A();
            IAtomContainer c7H16B = AbstractSignatureTest.MakeC7H16B();
            IAtomContainer c7H16C = AbstractSignatureTest.MakeC7H16C();
            SignatureQuotientGraph qGraphA = new SignatureQuotientGraph(c7H16A, 1);
            SignatureQuotientGraph qGraphB = new SignatureQuotientGraph(c7H16B, 1);
            SignatureQuotientGraph qGraphC = new SignatureQuotientGraph(c7H16C, 1);
            CheckParameters(qGraphA, 4, 7, 1);
            CheckParameters(qGraphB, 4, 5, 0);
            CheckParameters(qGraphC, 4, 7, 1);
        }

        [TestMethod()]
        public void TestAromatic()
        {
            IAtomContainer benzene = AbstractSignatureTest.MakeBenzene();
            SignatureQuotientGraph qGraph = new SignatureQuotientGraph(benzene);
            CheckParameters(qGraph, 1, 1, 1);
        }
    }
}
