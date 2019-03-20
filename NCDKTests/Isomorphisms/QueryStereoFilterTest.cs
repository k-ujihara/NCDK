/*
 * Copyright (C) 2018  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Isomorphisms.Matchers;
using NCDK.Silent;
using NCDK.SMARTS;
using NCDK.Stereo;

namespace NCDK.Isomorphisms
{
    /// <summary>
    /// Some simple isolated tests on functionality.
    /// </summary>
    // @author John May
    // @cdk.module test-smarts
    [TestClass()]
    public class QueryStereoFilterTest
    {
        /* target does not have an element */
        [TestMethod()]
        public void Tetrahedral_missingInTarget()
        {
            var query = Sma("[C@@](C)(C)(C)C");
            var target = Dimethylpropane();
            Assert.IsFalse(new QueryStereoFilter(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
        }

        /// <summary>
        /// Query does not have an element but the target does - the query therefore
        /// is a valid mapping.
        /// </summary>
        [TestMethod()]
        public void Tetrahedral_missingInQuery()
        {
            var query = Sma("[C@@](C)(C)(C)C");
            var target = Dimethylpropane();
            target.StereoElements.Add(new TetrahedralChirality(target.Atoms[0], new IAtom[]{target.Atoms[1],
                target.Atoms[2], target.Atoms[3], target.Atoms[4]}, TetrahedralStereo.Clockwise));
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
        }

        [TestMethod()]
        public void Tetrahedral_match()
        {
            var query = Sma("[C@@](C)(C)(C)C"); ;
            var target = Dimethylpropane();
            target.StereoElements.Add(new TetrahedralChirality(target.Atoms[0], new IAtom[]{target.Atoms[1],
                target.Atoms[2], target.Atoms[3], target.Atoms[4]}, TetrahedralStereo.Clockwise));
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
        }

        [TestMethod()]
        public void Tetrahedral_mismatch()
        {
            var query = Sma("[C@@](C)(C)(C)C");
            var target = Dimethylpropane();
            target.StereoElements.Add(new TetrahedralChirality(target.Atoms[0], new IAtom[]{target.Atoms[1],
                target.Atoms[2], target.Atoms[3], target.Atoms[4]}, TetrahedralStereo.AntiClockwise));
            Assert.IsFalse(new QueryStereoFilter(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
        }

        /// <summary>
        /// Map to different atom order which means the clockwise and anticlockwise
        /// match
        /// </summary>
        [TestMethod()]
        public void Tetrahedral_match_swap()
        {
            var query = Sma("[C@@](C)(C)(C)C");
            var target = Dimethylpropane();
            target.StereoElements.Add(new TetrahedralChirality(target.Atoms[0], new IAtom[]{target.Atoms[1],
                target.Atoms[2], target.Atoms[3], target.Atoms[4]}, TetrahedralStereo.AntiClockwise));
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 0, 1, 3, 2, 4 }));
        }

        /* These don't match because we don't map the atoms in order. */
        [TestMethod()]
        public void Tetrahedral_mismatch_swap()
        {
            var query = Sma("[C@@](C)(C)(C)C");
            var target = Dimethylpropane();
            target.StereoElements.Add(new TetrahedralChirality(target.Atoms[0], new IAtom[]{target.Atoms[1],
                target.Atoms[2], target.Atoms[3], target.Atoms[4]}, TetrahedralStereo.Clockwise));
            Assert.IsFalse(new QueryStereoFilter(query, target).Apply(new int[] { 0, 1, 3, 2, 4 }));
        }

        [TestMethod()]
        public void Tetrahedral_unspecified_match()
        {
            var query = Sma("[C@@?](C)(C)(C)C");
            var target = Dimethylpropane();
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
            target.StereoElements.Add(new TetrahedralChirality(target.Atoms[0], new IAtom[]{target.Atoms[1],
                target.Atoms[2], target.Atoms[3], target.Atoms[4]}, TetrahedralStereo.Clockwise));
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
        }

        [TestMethod()]
        public void Tetrahedral_unspecified_match2()
        {
            var query = Sma("[C@?](C)(C)(C)C");
            var target = Dimethylpropane();
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
            target.StereoElements.Add(new TetrahedralChirality(target.Atoms[0], new IAtom[]{target.Atoms[1],
                target.Atoms[2], target.Atoms[3], target.Atoms[4]}, TetrahedralStereo.AntiClockwise));
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
        }

        [TestMethod()]
        public void Tetrahedral_unspecified_mismatch()
        {
            var query = Sma("[C@@?](C)(C)(C)C");
            var target = Dimethylpropane();
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
            target.StereoElements.Add(new TetrahedralChirality(target.Atoms[0], new IAtom[]{target.Atoms[1],
                target.Atoms[2], target.Atoms[3], target.Atoms[4]}, TetrahedralStereo.AntiClockwise));
            Assert.IsFalse(new QueryStereoFilter(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
        }

        [TestMethod()]
        public void Tetrahedral_unspecified_mismatch2()
        {
            var query = Sma("[C@?](C)(C)(C)C");
            var target = Dimethylpropane();
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
            target.StereoElements.Add(new TetrahedralChirality(target.Atoms[0], new IAtom[]{target.Atoms[1],
                target.Atoms[2], target.Atoms[3], target.Atoms[4]}, TetrahedralStereo.Clockwise));
            Assert.IsFalse(new QueryStereoFilter(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
        }

        [TestMethod()]
        public void Tetrahedral_either_match()
        {
            var query = Sma("[@,@@](C)(C)(C)C");
            var target = Dimethylpropane();
            Assert.IsFalse(new QueryStereoFilter(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
            target.StereoElements.Add(new TetrahedralChirality(target.Atoms[0], new IAtom[]{target.Atoms[1],
                target.Atoms[2], target.Atoms[3], target.Atoms[4]}, TetrahedralStereo.Clockwise));
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
            target.StereoElements.Clear();
            target.StereoElements.Add(new TetrahedralChirality(target.Atoms[0], new IAtom[]{target.Atoms[1],
                target.Atoms[2], target.Atoms[3], target.Atoms[4]}, TetrahedralStereo.AntiClockwise));
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
        }

        [TestMethod()]
        public void Geometric_match_together1()
        {
            var query = Sma("C/C=C\\C");
            var target = But2ene();
            Assert.IsFalse(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Together));
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_match_together2()
        {
            var query = Sma("C\\C=C/C");
            var target = But2ene();
            Assert.IsFalse(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Together));
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_match_opposite1()
        {
            var query = Sma("C/C=C/C");
            var target = But2ene();
            Assert.IsFalse(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Opposite));
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_match_opposite2()
        {
            var query = Sma("C\\C=C\\C");
            var target = But2ene();
            Assert.IsFalse(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Opposite));
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_mismatch_together1()
        {
            var query = Sma("C/C=C\\C");
            var target = But2ene();
            Assert.IsFalse(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Opposite));
            Assert.IsFalse(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_mismatch_together2()
        {
            var query = Sma("C\\C=C/C");
            var target = But2ene();
            Assert.IsFalse(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Opposite));
            Assert.IsFalse(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_mismatch_opposite1()
        {
            var query = Sma("C/C=C/C");
            var target = But2ene();
            Assert.IsFalse(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Together));
            Assert.IsFalse(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_mismatch_opposite2()
        {
            var query = Sma("C\\C=C\\C");
            var target = But2ene();
            Assert.IsFalse(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Together));
            Assert.IsFalse(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_match_together_unspecified1()
        {
            var query = Sma("C/C=C\\?C");
            var target = But2ene();
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Together));
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_match_together_unspecified2()
        {
            var query = Sma("C/?C=C\\C");
            var target = But2ene();
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Together));
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Clear();
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Opposite));
            Assert.IsFalse(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_match_together_unspecified3()
        {
            var query = Sma("C\\C=C/?C");
            var target = But2ene();
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Together));
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Clear();
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Opposite));
            Assert.IsFalse(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_match_together_unspecified4()
        {
            var query = Sma("C\\?C=C/C");
            var target = But2ene();
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Together));
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Clear();
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Opposite));
            Assert.IsFalse(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_match_opposite_unspecified1()
        {
            var query = Sma("C/C=C/?C");
            var target = But2ene();
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Opposite));
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Clear();
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Together));
            Assert.IsFalse(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_match_opposite_unspecified2()
        {
            var query = Sma("C/?C=C/C");
            var target = But2ene();
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Opposite));
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Clear();
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Together));
            Assert.IsFalse(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_match_opposite_unspecified3()
        {
            var query = Sma("C\\C=C\\?C");
            var target = But2ene();
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Opposite));
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Clear();
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Together));
            Assert.IsFalse(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_match_opposite_unspecified4()
        {
            var query = Sma("C\\?C=C\\C");
            var target = But2ene();
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Opposite));
            Assert.IsTrue(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Clear();
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Together));
            Assert.IsFalse(new QueryStereoFilter(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        static IAtomContainer Dimethylpropane()
        {
            var container = new AtomContainer();
            container.Atoms.Add(Atom("C", 0));
            container.Atoms.Add(Atom("C", 3));
            container.Atoms.Add(Atom("C", 3));
            container.Atoms.Add(Atom("C", 3));
            container.Atoms.Add(Atom("C", 3));
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);
            container.AddBond(container.Atoms[0], container.Atoms[2], BondOrder.Single);
            container.AddBond(container.Atoms[0], container.Atoms[3], BondOrder.Single);
            container.AddBond(container.Atoms[0], container.Atoms[4], BondOrder.Single);
            return container;
        }

        static IAtomContainer But2ene()
        {
            var container = new AtomContainer();
            container.Atoms.Add(Atom("C", 1));
            container.Atoms.Add(Atom("C", 1));
            container.Atoms.Add(Atom("C", 3));
            container.Atoms.Add(Atom("C", 3));
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Double);
            container.AddBond(container.Atoms[0], container.Atoms[2], BondOrder.Single);
            container.AddBond(container.Atoms[1], container.Atoms[3], BondOrder.Single);
            return container;
        }

        static IAtom Atom(string symbol, int hCount)
        {
            var atom = new Atom(symbol) { ImplicitHydrogenCount = hCount };
            return atom;
        }

        static IAtomContainer Sma(string smarts)
        {
            var query = new QueryAtomContainer();
            Smarts.Parse(query, smarts);
            return query;
        }
    }
}
