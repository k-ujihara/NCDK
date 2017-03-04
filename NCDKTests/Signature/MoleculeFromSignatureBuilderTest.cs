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
using FaulonSignatures;

namespace NCDK.Signature
{
    // @cdk.module test-signature
    // @author maclean
    [TestClass()]
    public class MoleculeFromSignatureBuilderTest : AbstractSignatureTest
    {
        public string SignatureForAtom(IAtomContainer atomContainer, int atomIndex)
        {
            MoleculeSignature molSig = new MoleculeSignature(atomContainer);
            return molSig.SignatureStringForVertex(atomIndex);
        }

        public string CanonicalSignature(IAtomContainer atomContainer)
        {
            MoleculeSignature molSig = new MoleculeSignature(atomContainer);
            return molSig.ToCanonicalString();
        }

        public IAtomContainer Reconstruct(string signature)
        {
            ColoredTree tree = AbstractVertexSignature.Parse(signature);
            MoleculeFromSignatureBuilder builder = new MoleculeFromSignatureBuilder(Silent.ChemObjectBuilder.Instance);
            builder.MakeFromColoredTree(tree);
            return builder.GetAtomContainer();
        }

        public void ccBondTest(BondOrder order)
        {
            IAtomContainer cc = builder.CreateAtomContainer();
            cc.Atoms.Add(builder.CreateAtom("C"));
            cc.Atoms.Add(builder.CreateAtom("C"));
            cc.AddBond(cc.Atoms[0], cc.Atoms[1], order);
            string signature = SignatureForAtom(cc, 0);
            IAtomContainer reconstructed = Reconstruct(signature);
            Assert.AreEqual(2, reconstructed.Atoms.Count);
            Assert.AreEqual(1, reconstructed.Bonds.Count);
            Assert.AreEqual(order, reconstructed.Bonds[0].Order);
        }

        public IAtomContainer MakeRing(int ringSize)
        {
            IAtomContainer ring = builder.CreateAtomContainer();
            for (int i = 0; i < ringSize; i++)
            {
                ring.Atoms.Add(builder.CreateAtom("C"));
                if (i > 0)
                {
                    ring.AddBond(ring.Atoms[i - 1], ring.Atoms[i], BondOrder.Single);
                }
            }
            ring.AddBond(ring.Atoms[0], ring.Atoms[ringSize - 1], BondOrder.Single);
            return ring;
        }

        public void RingTest(int ringSize)
        {
            IAtomContainer ring = MakeRing(ringSize);
            string signature = CanonicalSignature(ring);
            IAtomContainer reconstructedRing = Reconstruct(signature);
            Assert.AreEqual(ringSize, reconstructedRing.Atoms.Count);
        }

        [TestMethod()]
        public void SingleCCBondTest()
        {
            ccBondTest(BondOrder.Single);
        }

        [TestMethod()]
        public void DoubleCCBondTest()
        {
            ccBondTest(BondOrder.Double);
        }

        [TestMethod()]
        public void TripleCCBondTest()
        {
            ccBondTest(BondOrder.Triple);
        }

        [TestMethod()]
        public void TriangleRingTest()
        {
            RingTest(3);
        }

        [TestMethod()]
        public void SquareRingTest()
        {
            RingTest(4);
        }

        [TestMethod()]
        public void PentagonRingTest()
        {
            RingTest(5);
        }

        [TestMethod()]
        public void HexagonRingTest()
        {
            RingTest(5);
        }

        [TestMethod()]
        public void MakeGraphTest()
        {
            MoleculeFromSignatureBuilder builder = new MoleculeFromSignatureBuilder(Silent.ChemObjectBuilder.Instance);
            builder.MakeGraph();
            Assert.IsNotNull(builder.GetAtomContainer());
        }

        [TestMethod()]
        public void MakeVertexTest()
        {
            MoleculeFromSignatureBuilder builder = new MoleculeFromSignatureBuilder(Silent.ChemObjectBuilder.Instance);
            builder.MakeGraph();
            builder.MakeVertex("C");
            IAtomContainer product = builder.GetAtomContainer();
            Assert.AreEqual(1, product.Atoms.Count);
        }

        [TestMethod()]
        public void MakeEdgeTest_singleBond()
        {
            MoleculeFromSignatureBuilder builder = new MoleculeFromSignatureBuilder(Silent.ChemObjectBuilder.Instance);
            builder.MakeGraph();
            builder.MakeVertex("C");
            builder.MakeVertex("C");
            builder.MakeEdge(0, 1, "C", "C", "");

            IAtomContainer product = builder.GetAtomContainer();
            Assert.AreEqual(2, product.Atoms.Count);
            Assert.AreEqual(1, product.Bonds.Count);
            Assert.AreEqual(BondOrder.Single, product.Bonds[0].Order);
        }

        [TestMethod()]
        public void MakeEdgeTest_DoubleBond()
        {
            MoleculeFromSignatureBuilder builder = new MoleculeFromSignatureBuilder(Silent.ChemObjectBuilder.Instance);
            builder.MakeGraph();
            builder.MakeVertex("C");
            builder.MakeVertex("C");
            builder.MakeEdge(0, 1, "C", "C", "=");

            IAtomContainer product = builder.GetAtomContainer();
            Assert.AreEqual(2, product.Atoms.Count);
            Assert.AreEqual(1, product.Bonds.Count);
            Assert.AreEqual(BondOrder.Double, product.Bonds[0].Order);
        }

        [TestMethod()]
        public void MakeEdgeTest_tripleBond()
        {
            MoleculeFromSignatureBuilder builder = new MoleculeFromSignatureBuilder(Silent.ChemObjectBuilder.Instance);
            builder.MakeGraph();
            builder.MakeVertex("C");
            builder.MakeVertex("C");
            builder.MakeEdge(0, 1, "C", "C", "#");

            IAtomContainer product = builder.GetAtomContainer();
            Assert.AreEqual(2, product.Atoms.Count);
            Assert.AreEqual(1, product.Bonds.Count);
            Assert.AreEqual(BondOrder.Triple, product.Bonds[0].Order);
        }

        [TestMethod()]
        public void MakeEdgeTest_aromaticBond()
        {
            MoleculeFromSignatureBuilder builder = new MoleculeFromSignatureBuilder(Silent.ChemObjectBuilder.Instance);
            builder.MakeGraph();
            builder.MakeVertex("C");
            builder.MakeVertex("C");
            builder.MakeEdge(0, 1, "C", "C", "p");

            IAtomContainer product = builder.GetAtomContainer();
            Assert.AreEqual(2, product.Atoms.Count);
            Assert.AreEqual(1, product.Bonds.Count);
            IBond bond = product.Bonds[0];
            Assert.AreEqual(BondOrder.Single, bond.Order);
            Assert.IsTrue(bond.IsAromatic);
        }

        [TestMethod()]
        public void GetAtomContainerTest()
        {
            MoleculeFromSignatureBuilder builder = new MoleculeFromSignatureBuilder(Silent.ChemObjectBuilder.Instance);
            builder.MakeGraph();
            Assert.IsNotNull(builder.GetAtomContainer());
        }
    }
}
