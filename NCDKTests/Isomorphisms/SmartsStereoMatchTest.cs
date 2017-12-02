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
using NCDK.Default;
using NCDK.Smiles.SMARTS.Parser;
using NCDK.Stereo;
using System.Collections.Generic;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace NCDK.Isomorphisms
{
    /// <summary>
    /// Some simple isolated tests on functionality.
    /// </summary>
    [TestClass()]
    // @author John May
    // @cdk.module test-smarts
    public class SmartsStereoMatchTest
    {
        /* target does not have an element */
        [TestMethod()]
        public void Tetrahedral_missingInTarget()
        {
            IAtomContainer query = Sma("[C@@](C)(C)(C)C");
            IAtomContainer target = Dimethylpropane();
            Assert.IsFalse(new SmartsStereoMatch(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
        }

        /// <summary>
        /// Query does not have an element but the target does - the query therefore
        /// is a valid mapping.
        /// </summary>
        [TestMethod()]
        public void Tetrahedral_missingInQuery()
        {
            IAtomContainer query = Sma("[C@@](C)(C)(C)C");
            IAtomContainer target = Dimethylpropane();
            target.StereoElements.Add(new TetrahedralChirality(target.Atoms[0], new IAtom[]{target.Atoms[1],
                target.Atoms[2], target.Atoms[3], target.Atoms[4]}, TetrahedralStereo.Clockwise));
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
        }

        [TestMethod()]
        public void Tetrahedral_match()
        {
            IAtomContainer query = Sma("[C@@](C)(C)(C)C"); ;
            IAtomContainer target = Dimethylpropane();
            target.StereoElements.Add(new TetrahedralChirality(target.Atoms[0], new IAtom[]{target.Atoms[1],
                target.Atoms[2], target.Atoms[3], target.Atoms[4]}, TetrahedralStereo.Clockwise));
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
        }

        [TestMethod()]
        public void Tetrahedral_mismatch()
        {
            IAtomContainer query = Sma("[C@@](C)(C)(C)C");
            IAtomContainer target = Dimethylpropane();
            target.StereoElements.Add(new TetrahedralChirality(target.Atoms[0], new IAtom[]{target.Atoms[1],
                target.Atoms[2], target.Atoms[3], target.Atoms[4]}, TetrahedralStereo.AntiClockwise));
            Assert.IsFalse(new SmartsStereoMatch(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
        }

        /// <summary>
        /// Map to different atom order which means the clockwise and anticlockwise
        /// match
        /// </summary>
        [TestMethod()]
        public void Tetrahedral_match_swap()
        {
            IAtomContainer query = Sma("[C@@](C)(C)(C)C");
            IAtomContainer target = Dimethylpropane();
            target.StereoElements.Add(new TetrahedralChirality(target.Atoms[0], new IAtom[]{target.Atoms[1],
                target.Atoms[2], target.Atoms[3], target.Atoms[4]}, TetrahedralStereo.AntiClockwise));
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 0, 1, 3, 2, 4 }));
        }

        /* These don't match because we don't map the atoms in order. */
        [TestMethod()]
        public void Tetrahedral_mismatch_swap()
        {
            IAtomContainer query = Sma("[C@@](C)(C)(C)C");
            IAtomContainer target = Dimethylpropane();
            target.StereoElements.Add(new TetrahedralChirality(target.Atoms[0], new IAtom[]{target.Atoms[1],
                target.Atoms[2], target.Atoms[3], target.Atoms[4]}, TetrahedralStereo.Clockwise));
            Assert.IsFalse(new SmartsStereoMatch(query, target).Apply(new int[] { 0, 1, 3, 2, 4 }));
        }

        [TestMethod()]
        public void Tetrahedral_unspecified_match()
        {
            IAtomContainer query = Sma("[C@@?](C)(C)(C)C");
            IAtomContainer target = Dimethylpropane();
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
            target.StereoElements.Add(new TetrahedralChirality(target.Atoms[0], new IAtom[]{target.Atoms[1],
                target.Atoms[2], target.Atoms[3], target.Atoms[4]}, TetrahedralStereo.Clockwise));
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
        }

        [TestMethod()]
        public void Tetrahedral_unspecified_match2()
        {
            IAtomContainer query = Sma("[C@?](C)(C)(C)C");
            IAtomContainer target = Dimethylpropane();
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
            target.StereoElements.Add(new TetrahedralChirality(target.Atoms[0], new IAtom[]{target.Atoms[1],
                target.Atoms[2], target.Atoms[3], target.Atoms[4]}, TetrahedralStereo.AntiClockwise));
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
        }

        [TestMethod()]
        public void Tetrahedral_unspecified_mismatch()
        {
            IAtomContainer query = Sma("[C@@?](C)(C)(C)C");
            IAtomContainer target = Dimethylpropane();
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
            target.StereoElements.Add(new TetrahedralChirality(target.Atoms[0], new IAtom[]{target.Atoms[1],
                target.Atoms[2], target.Atoms[3], target.Atoms[4]}, TetrahedralStereo.AntiClockwise));
            Assert.IsFalse(new SmartsStereoMatch(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
        }

        [TestMethod()]
        public void Tetrahedral_unspecified_mismatch2()
        {
            IAtomContainer query = Sma("[C@?](C)(C)(C)C");
            IAtomContainer target = Dimethylpropane();
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
            target.StereoElements.Add(new TetrahedralChirality(target.Atoms[0], new IAtom[]{target.Atoms[1],
                target.Atoms[2], target.Atoms[3], target.Atoms[4]}, TetrahedralStereo.Clockwise));
            Assert.IsFalse(new SmartsStereoMatch(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
        }

        [TestMethod()]
        public void Tetrahedral_either_match()
        {
            IAtomContainer query = Sma("[@,@@](C)(C)(C)C");
            IAtomContainer target = Dimethylpropane();
            Assert.IsFalse(new SmartsStereoMatch(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
            target.StereoElements.Add(new TetrahedralChirality(target.Atoms[0], new IAtom[]{target.Atoms[1],
                target.Atoms[2], target.Atoms[3], target.Atoms[4]}, TetrahedralStereo.Clockwise));
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
            target.SetStereoElements(new List<IReadOnlyStereoElement<IChemObject, IChemObject>>(1));
            target.StereoElements.Add(new TetrahedralChirality(target.Atoms[0], new IAtom[]{target.Atoms[1],
                target.Atoms[2], target.Atoms[3], target.Atoms[4]}, TetrahedralStereo.AntiClockwise));
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 0, 1, 2, 3, 4 }));
        }

        [TestMethod()]
        public void Geometric_match_together1()
        {
            IAtomContainer query = Sma("C/C=C\\C");
            IAtomContainer target = But2ene();
            Assert.IsFalse(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Together));
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_match_together2()
        {
            IAtomContainer query = Sma("C\\C=C/C");
            IAtomContainer target = But2ene();
            Assert.IsFalse(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Together));
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_match_opposite1()
        {
            IAtomContainer query = Sma("C/C=C/C");
            IAtomContainer target = But2ene();
            Assert.IsFalse(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Opposite));
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_match_opposite2()
        {
            IAtomContainer query = Sma("C\\C=C\\C");
            IAtomContainer target = But2ene();
            Assert.IsFalse(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Opposite));
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_mismatch_together1()
        {
            IAtomContainer query = Sma("C/C=C\\C");
            IAtomContainer target = But2ene();
            Assert.IsFalse(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Opposite));
            Assert.IsFalse(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_mismatch_together2()
        {
            IAtomContainer query = Sma("C\\C=C/C");
            IAtomContainer target = But2ene();
            Assert.IsFalse(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Opposite));
            Assert.IsFalse(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_mismatch_opposite1()
        {
            IAtomContainer query = Sma("C/C=C/C");
            IAtomContainer target = But2ene();
            Assert.IsFalse(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Together));
            Assert.IsFalse(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_mismatch_opposite2()
        {
            IAtomContainer query = Sma("C\\C=C\\C");
            IAtomContainer target = But2ene();
            Assert.IsFalse(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Together));
            Assert.IsFalse(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_match_together_unspecified1()
        {
            IAtomContainer query = Sma("C/C=C\\?C");
            IAtomContainer target = But2ene();
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Together));
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_match_together_unspecified2()
        {
            IAtomContainer query = Sma("C/?C=C\\C");
            IAtomContainer target = But2ene();
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Together));
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.SetStereoElements(new List<IReadOnlyStereoElement<IChemObject, IChemObject>>(1));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Opposite));
            Assert.IsFalse(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_match_together_unspecified3()
        {
            IAtomContainer query = Sma("C\\C=C/?C");
            IAtomContainer target = But2ene();
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Together));
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.SetStereoElements(new List<IReadOnlyStereoElement<IChemObject, IChemObject>>(1));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Opposite));
            Assert.IsFalse(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_match_together_unspecified4()
        {
            IAtomContainer query = Sma("C\\?C=C/C");
            IAtomContainer target = But2ene();
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Together));
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.SetStereoElements(new List<IReadOnlyStereoElement<IChemObject, IChemObject>>(1));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Opposite));
            Assert.IsFalse(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_match_opposite_unspecified1()
        {
            IAtomContainer query = Sma("C/C=C/?C");
            IAtomContainer target = But2ene();
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Opposite));
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.SetStereoElements(new List<IReadOnlyStereoElement<IChemObject, IChemObject>>(1));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Together));
            Assert.IsFalse(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_match_opposite_unspecified2()
        {
            IAtomContainer query = Sma("C/?C=C/C");
            IAtomContainer target = But2ene();
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Opposite));
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.SetStereoElements(new List<IReadOnlyStereoElement<IChemObject, IChemObject>>(1));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Together));
            Assert.IsFalse(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_match_opposite_unspecified3()
        {
            IAtomContainer query = Sma("C\\C=C\\?C");
            IAtomContainer target = But2ene();
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Opposite));
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.SetStereoElements(new List<IReadOnlyStereoElement<IChemObject, IChemObject>>(1));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Together));
            Assert.IsFalse(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
        }

        [TestMethod()]
        public void Geometric_match_opposite_unspecified4()
        {
            IAtomContainer query = Sma("C\\?C=C\\C");
            IAtomContainer target = But2ene();
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Opposite));
            Assert.IsTrue(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
            target.SetStereoElements(new List<IReadOnlyStereoElement<IChemObject, IChemObject>>(1));
            target.StereoElements.Add(new DoubleBondStereochemistry(target.Bonds[0], new IBond[]{target.Bonds[1],
                target.Bonds[2]}, DoubleBondConformation.Together));
            Assert.IsFalse(new SmartsStereoMatch(query, target).Apply(new int[] { 2, 0, 1, 3 }));
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

        static IAtomContainer Sma(string smarts)
        {
            return SMARTSParser.Parse(smarts, Silent.ChemObjectBuilder.Instance);
        }
    }
}
