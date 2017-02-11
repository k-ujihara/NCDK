/*
 * Copyright (c) 2013 European Bioinformatics Institute (EMBL-EBI)
 *                    John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;
using NCDK.Stereo;

namespace NCDK.Isomorphisms
{
    /**
     * Some simple isolated tests on functionality.
     *
     * @author John May
     * @cdk.module test-isomorphism
     */
    [TestClass()]
    public class StereoMatchTest
    {
        /* target does not have an element */
        [TestMethod()]
        public void Tetrahedral_missingInTarget()
        {
            IAtomContainer query = Dimethylpropane();
            IAtomContainer target = Dimethylpropane();
            query.AddStereoElement(new TetrahedralChirality(query.Atoms[0], new IAtom[]{query.Atoms[1],
                query.Atoms[2], query.Atoms[3], query.Atoms[4]}, TetrahedralStereo.Clockwise));
            Assert.IsFalse(new StereoMatch(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
        }

        /*
         * Query does not have an element but the target does - the query therefore
         * is a valid mapping.
         */
        [TestMethod()]
        public void Tetrahedral_missingInQuery()
        {
            IAtomContainer query = Dimethylpropane();
            IAtomContainer target = Dimethylpropane();
            target.AddStereoElement(new TetrahedralChirality(target.Atoms[0], new IAtom[]{target.Atoms[1],
                target.Atoms[2], target.Atoms[3], target.Atoms[4]}, TetrahedralStereo.Clockwise));
            Assert.IsTrue(new StereoMatch(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
        }

        [TestMethod()]
        public void Tetrahedral_match()
        {
            IAtomContainer query = Dimethylpropane();
            IAtomContainer target = Dimethylpropane();
            query.AddStereoElement(new TetrahedralChirality(query.Atoms[0], new IAtom[]{query.Atoms[1],
                query.Atoms[2], query.Atoms[3], query.Atoms[4]}, TetrahedralStereo.Clockwise));
            target.AddStereoElement(new TetrahedralChirality(target.Atoms[0], new IAtom[]{target.Atoms[1],
                target.Atoms[2], target.Atoms[3], target.Atoms[4]}, TetrahedralStereo.Clockwise));
            Assert.IsTrue(new StereoMatch(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
        }

        [TestMethod()]
        public void Tetrahedral_mismatch()
        {
            IAtomContainer query = Dimethylpropane();
            IAtomContainer target = Dimethylpropane();
            query.AddStereoElement(new TetrahedralChirality(query.Atoms[0], new IAtom[]{query.Atoms[1],
                query.Atoms[2], query.Atoms[3], query.Atoms[4]}, TetrahedralStereo.Clockwise));
            target.AddStereoElement(new TetrahedralChirality(target.Atoms[0], new IAtom[]{target.Atoms[1],
                target.Atoms[2], target.Atoms[3], target.Atoms[4]}, TetrahedralStereo.AntiClockwise));
            Assert.IsFalse(new StereoMatch(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
        }

        /*
         * Map to different atom order which means the clockwise and anticlockwise
         * match
         */
        [TestMethod()]
        public void Tetrahedral_match_swap()
        {
            IAtomContainer query = Dimethylpropane();
            IAtomContainer target = Dimethylpropane();
            query.AddStereoElement(new TetrahedralChirality(query.Atoms[0], new IAtom[]{query.Atoms[1],
                query.Atoms[2], query.Atoms[3], query.Atoms[4]}, TetrahedralStereo.Clockwise));
            target.AddStereoElement(new TetrahedralChirality(target.Atoms[0], new IAtom[]{target.Atoms[1],
                target.Atoms[2], target.Atoms[3], target.Atoms[4]}, TetrahedralStereo.AntiClockwise));
            Assert.IsTrue(new StereoMatch(query, target).Apply(new int[] { 0, 1, 3, 2, 4 }));
        }

        /* These don't match because we don't map the atoms in order. */
        [TestMethod()]
        public void Tetrahedral_mismatch_swap()
        {
            IAtomContainer query = Dimethylpropane();
            IAtomContainer target = Dimethylpropane();
            query.AddStereoElement(new TetrahedralChirality(query.Atoms[0], new IAtom[]{query.Atoms[1],
                query.Atoms[2], query.Atoms[3], query.Atoms[4]}, TetrahedralStereo.Clockwise));
            target.AddStereoElement(new TetrahedralChirality(target.Atoms[0], new IAtom[]{target.Atoms[1],
                target.Atoms[2], target.Atoms[3], target.Atoms[4]}, TetrahedralStereo.Clockwise));
            Assert.IsFalse(new StereoMatch(query, target).Apply(new int[] { 0, 1, 3, 2, 4 }));
        }

        [TestMethod()]
        public void Geometric_match_together()
        {
            IAtomContainer query = But2ene();
            IAtomContainer target = But2ene();
            query.AddStereoElement(new DoubleBondStereochemistry(query.Bonds[0], new IBond[]{query.Bonds[1],
                query.Bonds[2]}, DoubleBondConformation.Together));
            target.AddStereoElement(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Together));
            Assert.IsTrue(new StereoMatch(query, target).Apply(new int[] { 0, 1, 2, 3 }));
        }

        [TestMethod()]
        public void Geometric_match_opposite()
        {
            IAtomContainer query = But2ene();
            IAtomContainer target = But2ene();
            query.AddStereoElement(new DoubleBondStereochemistry(query.Bonds[0], new IBond[]{query.Bonds[1],
                query.Bonds[2]}, DoubleBondConformation.Opposite));
            target.AddStereoElement(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Opposite));
            Assert.IsTrue(new StereoMatch(query, target).Apply(new int[] { 0, 1, 2, 3 }));
        }

        [TestMethod()]
        public void Geometric_mismatch_together()
        {
            IAtomContainer query = But2ene();
            IAtomContainer target = But2ene();
            query.AddStereoElement(new DoubleBondStereochemistry(query.Bonds[0], new IBond[]{query.Bonds[1],
                query.Bonds[2]}, DoubleBondConformation.Together));
            target.AddStereoElement(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Opposite));
            Assert.IsFalse(new StereoMatch(query, target).Apply(new int[] { 0, 1, 2, 3 }));
        }

        [TestMethod()]
        public void Geometric_mismatch_opposite()
        {
            IAtomContainer query = But2ene();
            IAtomContainer target = But2ene();
            query.AddStereoElement(new DoubleBondStereochemistry(query.Bonds[0], new IBond[]{query.Bonds[1],
                query.Bonds[2]}, DoubleBondConformation.Opposite));
            target.AddStereoElement(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Together));
            Assert.IsFalse(new StereoMatch(query, target).Apply(new int[] { 0, 1, 2, 3 }));
        }

        [TestMethod()]
        public void Geometric_missingInQuery()
        {
            IAtomContainer query = But2ene();
            IAtomContainer target = But2ene();
            target.AddStereoElement(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Together));
            Assert.IsTrue(new StereoMatch(query, target).Apply(new int[] { 0, 1, 2, 3 }));
        }

        [TestMethod()]
        public void Geometric_missingInTarget()
        {
            IAtomContainer query = But2ene();
            IAtomContainer target = But2ene();
            query.AddStereoElement(new DoubleBondStereochemistry(query.Bonds[0], new IBond[]{query.Bonds[1],
                query.Bonds[2]}, DoubleBondConformation.Opposite));
            Assert.IsFalse(new StereoMatch(query, target).Apply(new int[] { 0, 1, 2, 3 }));
        }

        static IAtomContainer Dimethylpropane()
        {
            IAtomContainer container = new AtomContainer();
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
            IAtomContainer container = new AtomContainer();
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
            IAtom atom = new Atom(symbol);
            atom.ImplicitHydrogenCount = hCount;
            return atom;
        }
    }
}
