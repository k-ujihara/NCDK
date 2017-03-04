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
    public class AllEquivalentCyclicSetTest
    {

        [TestMethod()]
        public void TestFind()
        {
            IAtomContainer dummy = new Mock<IAtomContainer>().Object;
            int[][] g = new int[][] { new[] { 1, 5, 6 }, new[] { 0, 2 }, new[] { 1, 3 }, new[] { 2, 4, 7 }, new[] { 3, 5 }, new[] { 0, 4 }, new[] { 0 }, new[] { 3 } };

            // this mock the invariants
            long[] values = new long[] { 1, 0, 0, 1, 0, 0, 2, 2 };

            EquivalentSetFinder finder = new AllEquivalentCyclicSet();
            var set = finder.Find(values, dummy, g);

            Assert.AreEqual(6, set.Count);
            // the first size vertex are all in a cycle
            Assert.IsTrue(set.Contains(0));
            Assert.IsTrue(set.Contains(1));
            Assert.IsTrue(set.Contains(2));
            Assert.IsTrue(set.Contains(3));
            Assert.IsTrue(set.Contains(4));
            Assert.IsTrue(set.Contains(5));

        }

        [TestMethod()]
        public void TestFind_Distinct()
        {
            IAtomContainer dummy = new Mock<IAtomContainer>().Object;
            int[][] g = new int[][] { new[] { 1, 5, 6 }, new[] { 0, 2 }, new[] { 1, 3 }, new[] { 2, 4, 7 }, new[] { 3, 5 }, new[] { 0, 4 }, new[] { 0 }, new[] { 3 } };
            // all values distinct
            long[] values = new long[] { 10, 11, 12, 13, 14, 15, 16, 17 };

            EquivalentSetFinder finder = new AllEquivalentCyclicSet();
            var set = finder.Find(values, dummy, g);

            Assert.AreEqual(0, set.Count);
        }

        /// <summary>
        /// Test the method at perturbing the 2D representations of CID 138898 and
        /// CID 241107. These molecules are very similar but distinct. To tell these
        /// apart we must use {@link AllEquivalentCyclicSet} opposed to the faster
        /// methods. This test also serves to demonstrates the basic equivalent set
        /// finder does not tell them apart but that this more complex finder does.
        /// </summary>
        [TestMethod()]
        public void TestScenario()
        {

            IAtomContainer cid138898 = CID138898();
            IAtomContainer cid241107 = CID241107();

            MoleculeHashGenerator basic = new HashGeneratorMaker().Depth(12).Elemental().Perturbed().Molecular();
            // basic equivalence method can't tell these apart
            Assert.AreEqual(basic.Generate(cid138898), basic.Generate(cid241107));

            MoleculeHashGenerator cmplx = new HashGeneratorMaker().Depth(12).Elemental()
                    .PerturbWith(new AllEquivalentCyclicSet()).Molecular();

            // complex equivalence method can tell these apart
            Assert.AreNotEqual(cmplx.Generate(cid138898), cmplx.Generate(cid241107));

        }

        /// <summary>
        /// PubChem-Compound CID 241107 CC12CC3(SC(S3)(CC(S1)(S2)C)C)C
        ///
        // @cdk.inchi InChI=1S/C10H16S4/c1-7-5-8(2)13-10(4,14-8)6-9(3,11-7)12-7/h5-6H2,1-4H3
        /// </summary>
        private IAtomContainer CID241107()
        {
            IAtomContainer m = new AtomContainer();
            IAtom[] as_ = new IAtom[]{new Atom("C"), new Atom("C"), new Atom("C"), new Atom("C"), new Atom("S"),
                new Atom("C"), new Atom("S"), new Atom("C"), new Atom("C"), new Atom("S"), new Atom("S"),
                new Atom("C"), new Atom("C"), new Atom("C"),};
            IBond[] bs = new IBond[]{new Bond(as_[1], as_[0]), new Bond(as_[2], as_[1]), new Bond(as_[3], as_[2]),
                new Bond(as_[4], as_[3]), new Bond(as_[5], as_[4]), new Bond(as_[6], as_[5]), new Bond(as_[6], as_[3]),
                new Bond(as_[7], as_[5]), new Bond(as_[8], as_[7]), new Bond(as_[9], as_[8]), new Bond(as_[9], as_[1]),
                new Bond(as_[10], as_[8]), new Bond(as_[10], as_[1]), new Bond(as_[11], as_[8]), new Bond(as_[12], as_[5]),
                new Bond(as_[13], as_[3]),};
            m.SetAtoms(as_);
            m.SetBonds(bs);
            return m;
        }

        /// <summary>
        /// PubChem-Compound CID 138898 CC12CC3(SC(S1)(CC(S2)(S3)C)C)C
        ///
        // @cdk.inchi InChI=1S/C10H16S4/c1-7-5-8(2)13-9(3,11-7)6-10(4,12-7)14-8/h5-6H2,1-4H3
        /// </summary>
        private IAtomContainer CID138898()
        {
            IAtomContainer m = new AtomContainer();
            IAtom[] as_ = new IAtom[]{new Atom("C"), new Atom("C"), new Atom("C"), new Atom("C"), new Atom("S"),
                new Atom("C"), new Atom("S"), new Atom("C"), new Atom("C"), new Atom("S"), new Atom("S"),
                new Atom("C"), new Atom("C"), new Atom("C"),};
            IBond[] bs = new IBond[]{new Bond(as_[1], as_[0]), new Bond(as_[2], as_[1]), new Bond(as_[3], as_[2]),
                new Bond(as_[4], as_[3]), new Bond(as_[5], as_[4]), new Bond(as_[6], as_[5]), new Bond(as_[6], as_[1]),
                new Bond(as_[7], as_[5]), new Bond(as_[8], as_[7]), new Bond(as_[9], as_[8]), new Bond(as_[9], as_[1]),
                new Bond(as_[10], as_[8]), new Bond(as_[10], as_[3]), new Bond(as_[11], as_[8]), new Bond(as_[12], as_[5]),
                new Bond(as_[13], as_[3]),};
            m.SetAtoms(as_);
            m.SetBonds(bs);
            return m;
        }
    }
}
