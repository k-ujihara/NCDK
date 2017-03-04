/*
 * Copyright (c) 2013. John May <jwmay@users.sf.net>
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
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCDK.Default;

namespace NCDK.Hash
{
    /// <summary>
    // @author John May
    // @cdk.module test-hash
    /// </summary>
    [TestClass()]
    public class MinimumEquivalentCyclicSetUnionTest
    {

        [TestMethod()]
        public void TestFind()
        {
            IAtomContainer dummy = new Mock<IAtomContainer>().Object;
            int[][] g = new int[][] { new[] { 1, 5, 6 }, new[] { 0, 2 }, new[] { 1, 3 }, new[] { 2, 4, 7 }, new[] { 3, 5 }, new[] { 0, 4 }, new[] { 0 }, new[] { 3 } };

            // mock the invariants
            long[] values = new long[] { 1, 4, 3, 1, 3, 5, 7, 8 };

            EquivalentSetFinder finder = new AllEquivalentCyclicSet();
            var set = finder.Find(values, dummy, g);

            Assert.AreEqual(4, set.Count);
            // the first size vertex are all in a cycle
            Assert.IsTrue(set.Contains(0));
            Assert.IsTrue(set.Contains(2));
            Assert.IsTrue(set.Contains(3));
            Assert.IsTrue(set.Contains(4));
        }

        [TestMethod()]
        public void TestFind_Distinct()
        {
            IAtomContainer dummy = new Mock<IAtomContainer>().Object;
            int[][] g = new int[][] { new[] { 1, 5, 6 }, new[] { 0, 2 }, new[] { 1, 3 }, new[] { 2, 4, 7 }, new[] { 3, 5 }, new[] { 0, 4 }, new[] { 0 }, new[] { 3 } };

            // mock the invariants
            long[] values = new long[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            EquivalentSetFinder finder = new AllEquivalentCyclicSet();
            var set = finder.Find(values, dummy, g);

            Assert.AreEqual(0, set.Count);
        }

        /// <summary>
        /// Test the method at perturbing the 2D representations of CID 44333798 and
        /// CID 57170558. These molecules are similar but distinct. To tell these
        /// apart we must use {@link org.openscience.cdk.hash.MinimumEquivalentCyclicSetUnion} opposed to the
        /// faster method. This test serves to demonstrates the basic equivalent set
        /// finder does not tell them apart but that a more comprehensive set finder
        /// does.
        /// </summary>
        [TestMethod()]
        public void TestScenario()
        {

            IAtomContainer cid4433798 = CID44333798();
            IAtomContainer cid57170558 = CID57170558();

            MoleculeHashGenerator basic = new HashGeneratorMaker().Depth(12).Elemental().Perturbed().Molecular();
            // basic equivalence method can't tell these apart
            Assert.AreEqual(basic.Generate(cid4433798), basic.Generate(cid57170558));

            MoleculeHashGenerator cmplx = new HashGeneratorMaker().Depth(12).Elemental()
                    .PerturbWith(new MinimumEquivalentCyclicSetUnion()).Molecular();

            // complex equivalence method can tell these apart
            Assert.AreNotEqual(cmplx.Generate(cid4433798), cmplx.Generate(cid57170558));
        }

        /// <summary>
        /// CC1=CC=C(C=C1)N2C3CCC2CC3
        ///
        // @cdk.inchi InChI=1S/C13H17N/c1-10-2-4-11(5-3-10)14-12-6-7-13(14)9-8-12/h2-5,12-13H,6-9H2,1H3
        /// </summary>
        private IAtomContainer CID44333798()
        {
            IAtomContainer m = new AtomContainer();
            IAtom[] as_ = new IAtom[]{new Atom("C"), new Atom("C"), new Atom("C"), new Atom("C"), new Atom("C"),
                new Atom("C"), new Atom("C"), new Atom("N"), new Atom("C"), new Atom("C"), new Atom("C"),
                new Atom("C"), new Atom("C"), new Atom("C"),};
            IBond[] bs = new IBond[]{new Bond(as_[1], as_[0]), new Bond(as_[2], as_[1], BondOrder.Double), new Bond(as_[3], as_[2]),
                new Bond(as_[4], as_[3], BondOrder.Double), new Bond(as_[5], as_[4]), new Bond(as_[6], as_[5], BondOrder.Double),
                new Bond(as_[6], as_[1]), new Bond(as_[7], as_[4]), new Bond(as_[8], as_[7]), new Bond(as_[9], as_[8]),
                new Bond(as_[10], as_[9]), new Bond(as_[11], as_[10]), new Bond(as_[11], as_[7]), new Bond(as_[12], as_[11]),
                new Bond(as_[13], as_[12]), new Bond(as_[13], as_[8]),};
            m.SetAtoms(as_);
            m.SetBonds(bs);
            return m;
        }

        /// <summary>
        /// CC1=CC=C(C=C1)N(C2CC2)C3CC3
        ///
        // @cdk.inchi InChI=1S/C13H17N/c1-10-2-4-11(5-3-10)14(12-6-7-12)13-8-9-13/h2-5,12-13H,6-9H2,1H3
        /// </summary>
        private IAtomContainer CID57170558()
        {
            IAtomContainer m = new AtomContainer();
            IAtom[] as_ = new IAtom[]{new Atom("C"), new Atom("C"), new Atom("C"), new Atom("C"), new Atom("C"),
                new Atom("C"), new Atom("C"), new Atom("N"), new Atom("C"), new Atom("C"), new Atom("C"),
                new Atom("C"), new Atom("C"), new Atom("C"),};
            IBond[] bs = new IBond[]{new Bond(as_[1], as_[0]), new Bond(as_[2], as_[1], BondOrder.Double), new Bond(as_[3], as_[2]),
                new Bond(as_[4], as_[3], BondOrder.Double), new Bond(as_[5], as_[4]), new Bond(as_[6], as_[5], BondOrder.Double),
                new Bond(as_[6], as_[1]), new Bond(as_[7], as_[4]), new Bond(as_[8], as_[7]), new Bond(as_[9], as_[8]),
                new Bond(as_[10], as_[9]), new Bond(as_[10], as_[8]), new Bond(as_[11], as_[7]), new Bond(as_[12], as_[11]),
                new Bond(as_[13], as_[12]), new Bond(as_[13], as_[11]),};
            m.SetAtoms(as_);
            m.SetBonds(bs);
            return m;
        }
    }
}
