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
using NCDK.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.Graphs;
using NCDK.Stereo;

namespace NCDK.Layout
{
    /// <summary>
    // @author John May
    // @cdk.module test-sdg
    /// </summary>
    [TestClass()]
    public class NonPlanarBondsTest
    {

        // [C@H](C)(N)O
        [TestMethod()]
        public void Clockwise_implH_1()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, 0.00d, 1.50d));
            m.Atoms.Add(Atom("C", 3, 0.00d, 0.00d));
            m.Atoms.Add(Atom("N", 2, -1.30d, 2.25d));
            m.Atoms.Add(Atom("O", 1, 1.30d, 2.25d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[3], BondOrder.Single);
            m.Add(new TetrahedralChirality(m.Atoms[0], new IAtom[]{m.Atoms[0], m.Atoms[1], m.Atoms[2],
                m.Atoms[3]}, TetrahedralStereo.Clockwise));
            NonplanarBonds.Assign(m);
            Assert.AreEqual(BondStereo.Down, m.Bonds[0].Stereo);
            Assert.AreEqual(BondStereo.None, m.Bonds[1].Stereo);
            Assert.AreEqual(BondStereo.None, m.Bonds[2].Stereo);
        }

        // [C@H](CC)(N)O
        // N is favoured over CC
        [TestMethod()]
        public void Clockwise_implH_2()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, -1.30d, 2.25d));
            m.Atoms.Add(Atom("C", 2, 0.00d, 1.50d));
            m.Atoms.Add(Atom("C", 3, 0.00d, 0.00d));
            m.Atoms.Add(Atom("N", 2, -1.30d, 3.75d));
            m.Atoms.Add(Atom("O", 1, -2.60d, 1.50d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[4], BondOrder.Single);
            m.Add(new TetrahedralChirality(m.Atoms[0], new IAtom[]{m.Atoms[0], m.Atoms[1], m.Atoms[3],
                m.Atoms[4]}, TetrahedralStereo.Clockwise));
            NonplanarBonds.Assign(m);
            Assert.AreEqual(BondStereo.None, m.Bonds[0].Stereo);
            Assert.AreEqual(BondStereo.None, m.Bonds[1].Stereo);
            Assert.AreEqual(BondStereo.Up, m.Bonds[2].Stereo);
            Assert.AreEqual(BondStereo.None, m.Bonds[3].Stereo);
        }

        // [C@H](C)(N)O
        [TestMethod()]
        public void Anticlockwise_implH_1()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, 0.00d, 1.50d));
            m.Atoms.Add(Atom("C", 3, 0.00d, 0.00d));
            m.Atoms.Add(Atom("N", 2, -1.30d, 2.25d));
            m.Atoms.Add(Atom("O", 1, 1.30d, 2.25d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[3], BondOrder.Single);
            m.Add(new TetrahedralChirality(m.Atoms[0], new IAtom[]{m.Atoms[0], m.Atoms[1], m.Atoms[2],
                m.Atoms[3]}, TetrahedralStereo.AntiClockwise));
            NonplanarBonds.Assign(m);
            Assert.AreEqual(BondStereo.Up, m.Bonds[0].Stereo);
            Assert.AreEqual(BondStereo.None, m.Bonds[1].Stereo);
            Assert.AreEqual(BondStereo.None, m.Bonds[2].Stereo);
        }

        // [C@H](CC)(N)O
        // N is favoured over CC
        [TestMethod()]
        public void Anticlockwise_implH_2()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, -1.30d, 2.25d));
            m.Atoms.Add(Atom("C", 2, 0.00d, 1.50d));
            m.Atoms.Add(Atom("C", 3, 0.00d, 0.00d));
            m.Atoms.Add(Atom("N", 2, -1.30d, 3.75d));
            m.Atoms.Add(Atom("O", 1, -2.60d, 1.50d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[4], BondOrder.Single);
            m.Add(new TetrahedralChirality(m.Atoms[0], new IAtom[]{m.Atoms[0], m.Atoms[1], m.Atoms[3],
                m.Atoms[4]}, TetrahedralStereo.AntiClockwise));
            NonplanarBonds.Assign(m);
            Assert.AreEqual(BondStereo.None, m.Bonds[0].Stereo);
            Assert.AreEqual(BondStereo.None, m.Bonds[1].Stereo);
            Assert.AreEqual(BondStereo.Down, m.Bonds[2].Stereo);
            Assert.AreEqual(BondStereo.None, m.Bonds[3].Stereo);
        }

        // [C@@](CCC)(C)(N)O
        [TestMethod()]
        public void Clockwise_1()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 0, -1.47d, 3.62d));
            m.Atoms.Add(Atom("C", 2, -1.13d, 2.16d));
            m.Atoms.Add(Atom("C", 2, 0.30d, 1.72d));
            m.Atoms.Add(Atom("C", 3, 0.64d, 0.26d));
            m.Atoms.Add(Atom("C", 3, -2.90d, 4.06d));
            m.Atoms.Add(Atom("N", 2, 0.03d, 3.70d));
            m.Atoms.Add(Atom("O", 1, -1.28d, 5.11d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[6], BondOrder.Single);
            m.Add(new TetrahedralChirality(m.Atoms[0], new IAtom[]{m.Atoms[1], m.Atoms[4], m.Atoms[5],
                m.Atoms[6]}, TetrahedralStereo.Clockwise));
            NonplanarBonds.Assign(m);
            Assert.AreEqual(BondStereo.None, m.Bonds[0].Stereo);
            Assert.AreEqual(BondStereo.Up, m.Bonds[3].Stereo);
            Assert.AreEqual(BondStereo.None, m.Bonds[4].Stereo);
            Assert.AreEqual(BondStereo.None, m.Bonds[5].Stereo);
        }

        // [C@@](CCC)(C1)(C)C1 (favour acyclic)
        [TestMethod()]
        public void Clockwise_2()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 0, -0.96d, -1.04d));
            m.Atoms.Add(Atom("C", 2, 0.18d, -0.08d));
            m.Atoms.Add(Atom("C", 2, -0.08d, 1.40d));
            m.Atoms.Add(Atom("C", 3, 1.07d, 2.36d));
            m.Atoms.Add(Atom("C", 2, -1.71d, -2.34d));
            m.Atoms.Add(Atom("C", 3, -2.11d, -0.08d));
            m.Atoms.Add(Atom("C", 1, -0.21d, -2.34d));
            m.Atoms.Add(Atom("C", 3, 1.08d, -3.09d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[6], m.Atoms[7], BondOrder.Single);
            m.Add(new TetrahedralChirality(m.Atoms[0], new IAtom[]{m.Atoms[1], m.Atoms[4], m.Atoms[5],
                m.Atoms[6]}, TetrahedralStereo.Clockwise));
            NonplanarBonds.Assign(m);
            Assert.AreEqual(BondStereo.None, m.Bonds[0].Stereo);
            Assert.AreEqual(BondStereo.None, m.Bonds[3].Stereo);
            Assert.AreEqual(BondStereo.Down, m.Bonds[4].Stereo);
            Assert.AreEqual(BondStereo.None, m.Bonds[5].Stereo);
        }

        // [C@](CCC)(C)(N)O
        [TestMethod()]
        public void Anticlockwise_1()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 0, -1.47d, 3.62d));
            m.Atoms.Add(Atom("C", 2, -1.13d, 2.16d));
            m.Atoms.Add(Atom("C", 2, 0.30d, 1.72d));
            m.Atoms.Add(Atom("C", 3, 0.64d, 0.26d));
            m.Atoms.Add(Atom("C", 3, -2.90d, 4.06d));
            m.Atoms.Add(Atom("N", 2, 0.03d, 3.70d));
            m.Atoms.Add(Atom("O", 1, -1.28d, 5.11d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[6], BondOrder.Single);
            m.Add(new TetrahedralChirality(m.Atoms[0], new IAtom[]{m.Atoms[1], m.Atoms[4], m.Atoms[5],
                m.Atoms[6]}, TetrahedralStereo.AntiClockwise));
            NonplanarBonds.Assign(m);
            Assert.AreEqual(BondStereo.None, m.Bonds[0].Stereo);
            Assert.AreEqual(BondStereo.Down, m.Bonds[3].Stereo);
            Assert.AreEqual(BondStereo.None, m.Bonds[4].Stereo);
            Assert.AreEqual(BondStereo.None, m.Bonds[5].Stereo);
        }

        // [C@](CCC)(C1)(C)C1 (favour acyclic)
        [TestMethod()]
        public void Anticlockwise_2()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 0, -0.96d, -1.04d));
            m.Atoms.Add(Atom("C", 2, 0.18d, -0.08d));
            m.Atoms.Add(Atom("C", 2, -0.08d, 1.40d));
            m.Atoms.Add(Atom("C", 3, 1.07d, 2.36d));
            m.Atoms.Add(Atom("C", 2, -1.71d, -2.34d));
            m.Atoms.Add(Atom("C", 3, -2.11d, -0.08d));
            m.Atoms.Add(Atom("C", 1, -0.21d, -2.34d));
            m.Atoms.Add(Atom("C", 3, 1.08d, -3.09d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[6], m.Atoms[7], BondOrder.Single);
            m.Add(new TetrahedralChirality(m.Atoms[0], new IAtom[]{m.Atoms[1], m.Atoms[4], m.Atoms[5],
                m.Atoms[6]}, TetrahedralStereo.AntiClockwise));
            NonplanarBonds.Assign(m);
            Assert.AreEqual(BondStereo.None, m.Bonds[0].Stereo);
            Assert.AreEqual(BondStereo.None, m.Bonds[3].Stereo);
            Assert.AreEqual(BondStereo.Up, m.Bonds[4].Stereo);
            Assert.AreEqual(BondStereo.None, m.Bonds[5].Stereo);
        }

        [TestMethod()]
        public void NonPlanarBondsForAntiClockwsieExtendedTetrahedral()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 3, -1.56d, 0.78d));
            m.Atoms.Add(Atom("C", 0, -1.13d, 1.49d));
            m.Atoms.Add(Atom("C", 0, -0.31d, 1.47d));
            m.Atoms.Add(Atom("C", 0, 0.52d, 1.46d));
            m.Atoms.Add(Atom("C", 3, 0.94d, 2.17d));
            m.Atoms.Add(Atom("H", 0, 0.92d, 0.74d));
            m.Atoms.Add(Atom("H", 0, -1.53d, 2.21d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double, BondStereo.None);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Double, BondStereo.None);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[5], BondOrder.Single);
            m.Add(new ExtendedTetrahedral(m.Atoms[2], new IAtom[]{m.Atoms[0], m.Atoms[6], m.Atoms[4],
                m.Atoms[5]}, TetrahedralStereo.AntiClockwise));
            NonplanarBonds.Assign(m);
            Assert.AreEqual(BondStereo.Down, m.GetBond(m.Atoms[1], m.Atoms[0]).Stereo);
            Assert.AreEqual(BondStereo.Up, m.GetBond(m.Atoms[1], m.Atoms[6]).Stereo);
        }

        [TestMethod()]
        public void NonPlanarBondsForClockwsieExtendedTetrahedral()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 3, -1.56d, 0.78d));
            m.Atoms.Add(Atom("C", 0, -1.13d, 1.49d));
            m.Atoms.Add(Atom("C", 0, -0.31d, 1.47d));
            m.Atoms.Add(Atom("C", 0, 0.52d, 1.46d));
            m.Atoms.Add(Atom("C", 3, 0.94d, 2.17d));
            m.Atoms.Add(Atom("H", 0, 0.92d, 0.74d));
            m.Atoms.Add(Atom("H", 0, -1.53d, 2.21d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double, BondStereo.None);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Double, BondStereo.None);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[5], BondOrder.Single);
            m.Add(new ExtendedTetrahedral(m.Atoms[2], new IAtom[]{m.Atoms[0], m.Atoms[6], m.Atoms[4],
                m.Atoms[5]}, TetrahedralStereo.Clockwise));
            NonplanarBonds.Assign(m);
            Assert.AreEqual(BondStereo.Up, m.GetBond(m.Atoms[1], m.Atoms[0]).Stereo);
            Assert.AreEqual(BondStereo.Down, m.GetBond(m.Atoms[1], m.Atoms[6]).Stereo);
        }

        [TestMethod()]
        public void ClockwiseSortShouldHandleExactlyOppositeAtoms()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 0, 4.50d, -14.84d));
            m.Atoms.Add(Atom("C", 3, 4.51d, -13.30d));
            m.Atoms.Add(Atom("C", 2, 4.93d, -14.13d));
            m.Atoms.Add(Atom("C", 2, 3.68d, -14.81d));
            m.Atoms.Add(Atom("O", 0, 4.05d, -15.54d));
            m.Atoms.Add(Atom("O", 1, 3.23d, -15.50d));
            m.Atoms.Add(Atom("C", 3, 5.32d, -14.86d));
            m.Atoms.Add(Atom("C", 3, 4.45d, -16.27d));
            m.AddBond(m.Atoms[0], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[7], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[3], BondOrder.Single);
            m.Add(new TetrahedralChirality(m.Atoms[0], new IAtom[]{m.Atoms[2], m.Atoms[4], m.Atoms[6],
                m.Atoms[3],}, TetrahedralStereo.AntiClockwise));
            NonplanarBonds.Assign(m);
            Assert.AreEqual(BondStereo.Down, m.Bonds[4].Stereo);
        }


        // ethene is left alone and not marked as crossed
        [TestMethod()]
        public void DontCrossEtheneDoubleBond()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 2, 0.000, 0.000));
            m.Atoms.Add(Atom("C", 2, 1.299, -0.750));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Double);
            NonplanarBonds.Assign(m);
            Assert.AreEqual(BondStereo.None, m.Bonds[0].Stereo);
        }

        /// <summary>
        // @cdk.inchi InChI=1S/C4H8O/c1-3-4(2)5/h3H2,1-2H3
        /// </summary>
        [TestMethod()]
        public void DontMarkTerminalBonds()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 3, 0.000, 0.000));
            m.Atoms.Add(Atom("C", 0, 1.299, -0.750));
            m.Atoms.Add(Atom("C", 2, 2.598, -0.000));
            m.Atoms.Add(Atom("C", 3, 3.897, -0.750));
            m.Atoms.Add(Atom("O", 0, 1.299, -2.250));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[4], BondOrder.Double);
            NonplanarBonds.Assign(m);
            Assert.AreEqual(BondStereo.None, m.Bonds[0].Stereo);
            Assert.AreEqual(BondStereo.None, m.Bonds[1].Stereo);
            Assert.AreEqual(BondStereo.None, m.Bonds[2].Stereo);
        }

        /// <summary>
        // @cdk.inchi InChI=1S/C4H8/c1-3-4-2/h3-4H,1-2H3
        /// </summary>
        [TestMethod()]
        public void MarkBut2eneWithWavyBond()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 3, 0.000, 0.000));
            m.Atoms.Add(Atom("C", 1, 1.299, -0.750));
            m.Atoms.Add(Atom("C", 1, 2.598, -0.000));
            m.Atoms.Add(Atom("C", 3, 3.897, -0.750));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);
            NonplanarBonds.Assign(m);
            Assert.AreEqual(BondStereo.UpOrDown, m.Bonds[0].Stereo);
        }

        /// <summary>
        // @cdk.inchi InChI=1S/C8H12/c1-3-5-7-8-6-4-2/h3-8H,1-2H3/b5-3+,6-4+,8-7?
        /// </summary>
        [TestMethod()]
        public void UseCrossedBondIfNeeded()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 3, 0.000, 0.000));
            m.Atoms.Add(Atom("C", 1, 1.299, -0.750));
            m.Atoms.Add(Atom("C", 1, 2.598, -0.000));
            m.Atoms.Add(Atom("C", 1, 3.897, -0.750));
            m.Atoms.Add(Atom("C", 1, 5.196, -0.000));
            m.Atoms.Add(Atom("C", 1, 6.495, -0.750));
            m.Atoms.Add(Atom("C", 1, 7.794, -0.000));
            m.Atoms.Add(Atom("C", 3, 9.093, -0.750));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Double);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[5], m.Atoms[6], BondOrder.Double);
            m.AddBond(m.Atoms[6], m.Atoms[7], BondOrder.Single);
            m.Add(new DoubleBondStereochemistry(m.Bonds[1],
                                                             new IBond[]{
                                                                 m.Bonds[0],
                                                                 m.Bonds[2]
                                                             },
                                                             DoubleBondConformation.Opposite));
            m.Add(new DoubleBondStereochemistry(m.Bonds[5],
                                                             new IBond[]{
                                                                 m.Bonds[4],
                                                                 m.Bonds[6]
                                                             },
                                                             DoubleBondConformation.Opposite));
            NonplanarBonds.Assign(m);
            Assert.AreEqual(BondStereo.EOrZ, m.Bonds[3].Stereo);
        }

        /// <summary>
        // @cdk.inchi InChI=1S/C6H14S/c1-5-7(4)6(2)3/h5H2,1-4H3/t7-/m0/s1 
        /// </summary>
        [TestMethod()]
        public void DontMarkTetrahedralCentresWithDoubleBondsAsUnspecified()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 3, 2.598, 1.500));
            m.Atoms.Add(Atom("S", 0, 2.598, -0.000));
            m.Atoms.Add(Atom("C", 1, 1.299, -0.750));
            m.Atoms.Add(Atom("C", 3, 0.000, 0.000));
            m.Atoms.Add(Atom("C", 2, 3.897, -0.750));
            m.Atoms.Add(Atom("C", 3, 5.196, -0.000));
            m.AddBond(m.Atoms[1], m.Atoms[0], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            m.Add(new TetrahedralChirality(m.Atoms[1],
                                                        new IAtom[] { m.Atoms[0], m.Atoms[1], m.Atoms[2], m.Atoms[4] },
                                                        TetrahedralStereo.AntiClockwise));

            NonplanarBonds.Assign(m);
            Assert.AreNotEqual(BondStereo.UpOrDown, m.Bonds[0].Stereo);
            Assert.AreNotEqual(BondStereo.UpOrDown, m.Bonds[2].Stereo);
            Assert.AreNotEqual(BondStereo.UpOrDown, m.Bonds[3].Stereo);
        }

        [TestMethod()]
        public void DontMarkRingBondsInBezeneAsUnspecified()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, -1.299, 0.750));
            m.Atoms.Add(Atom("C", 1, 0.000, 1.500));
            m.Atoms.Add(Atom("C", 1, 1.299, 0.750));
            m.Atoms.Add(Atom("C", 1, 1.299, -0.750));
            m.Atoms.Add(Atom("C", 1, 0.000, -1.500));
            m.Atoms.Add(Atom("C", 1, -1.299, -0.750));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Double);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Double);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Double);
            m.AddBond(m.Atoms[0], m.Atoms[5], BondOrder.Single);
            NonplanarBonds.Assign(m);
            foreach (var bond in m.Bonds)
            {
                Assert.AreEqual(BondStereo.None, bond.Stereo);
            }
        }

        /// <summary>
        /// {@code SMILES: *CN=C(N)N}
        /// </summary>
        [TestMethod()]
        public void DontMarkGuanidineAsUnspecified()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("R", 0, 0.00, 0.00));
            m.Atoms.Add(Atom("C", 2, 1.30, -0.75));
            m.Atoms.Add(Atom("N", 0, 2.60, -0.00));
            m.Atoms.Add(Atom("C", 0, 3.90, -0.75));
            m.Atoms.Add(Atom("N", 2, 5.20, -0.00));
            m.Atoms.Add(Atom("N", 2, 3.90, -2.25));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Double);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[5], BondOrder.Single);
            NonplanarBonds.Assign(m);
            foreach (var bond in m.Bonds)
                Assert.AreEqual(BondStereo.None, bond.Stereo);
        }

        /// <summary>
        /// {@code SMILES: *CN=C(CCC)CCC[H]}
        /// </summary>
        [TestMethod()]
        public void DontUnspecifiedDueToHRepresentation()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("R", 0, 0.00, 0.00));
            m.Atoms.Add(Atom("C", 2, 1.30, -0.75));
            m.Atoms.Add(Atom("N", 0, 2.60, -0.00));
            m.Atoms.Add(Atom("C", 0, 3.90, -0.75));
            m.Atoms.Add(Atom("C", 2, 3.90, -2.25));
            m.Atoms.Add(Atom("C", 2, 2.60, -3.00));
            m.Atoms.Add(Atom("C", 3, 2.60, -4.50));
            m.Atoms.Add(Atom("C", 2, 5.20, -0.00));
            m.Atoms.Add(Atom("C", 2, 6.50, -0.75));
            m.Atoms.Add(Atom("C", 2, 7.79, -0.00));
            m.Atoms.Add(Atom("H", 0, 9.09, -0.75));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Double);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[5], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[7], BondOrder.Single);
            m.AddBond(m.Atoms[7], m.Atoms[8], BondOrder.Single);
            m.AddBond(m.Atoms[8], m.Atoms[9], BondOrder.Single);
            m.AddBond(m.Atoms[9], m.Atoms[10], BondOrder.Single);
            NonplanarBonds.Assign(m);
            foreach (var bond in m.Bonds)
                Assert.AreEqual(BondStereo.None, bond.Stereo);
        }

        /// <summary>
        /// {@code SMILES: *CN=C(CCC)CCC}
        /// </summary>
        [TestMethod()]
        public void DontMarkUnspecifiedForLinearEqualChains()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("R", 0, 0.00, -0.00));
            m.Atoms.Add(Atom("C", 2, 1.30, -0.75));
            m.Atoms.Add(Atom("N", 0, 2.60, -0.00));
            m.Atoms.Add(Atom("C", 0, 3.90, -0.75));
            m.Atoms.Add(Atom("C", 2, 5.20, -0.00));
            m.Atoms.Add(Atom("C", 2, 6.50, -0.75));
            m.Atoms.Add(Atom("C", 3, 7.79, -0.00));
            m.Atoms.Add(Atom("C", 2, 3.90, -2.25));
            m.Atoms.Add(Atom("C", 2, 5.20, -3.00));
            m.Atoms.Add(Atom("C", 3, 5.20, -4.50));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Double);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[5], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[7], BondOrder.Single);
            m.AddBond(m.Atoms[7], m.Atoms[8], BondOrder.Single);
            m.AddBond(m.Atoms[8], m.Atoms[9], BondOrder.Single);
            NonplanarBonds.Assign(m);
            foreach (var bond in m.Bonds)
                Assert.AreEqual(BondStereo.None, bond.Stereo);
        }

        /// <summary>
        /// {@code SMILES: *CN=C1CCCCC1}
        /// </summary>
        [TestMethod()]
        public void MarkUnspecifiedForCyclicLigands()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("R", 0, -4.22, 3.05));
            m.Atoms.Add(Atom("C", 2, -2.92, 2.30));
            m.Atoms.Add(Atom("N", 0, -1.62, 3.05));
            m.Atoms.Add(Atom("C", 0, -0.32, 2.30));
            m.Atoms.Add(Atom("C", 2, -0.33, 0.80));
            m.Atoms.Add(Atom("C", 2, 0.97, 0.05));
            m.Atoms.Add(Atom("C", 2, 2.27, 0.80));
            m.Atoms.Add(Atom("C", 2, 2.27, 2.30));
            m.Atoms.Add(Atom("C", 2, 0.97, 3.05));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Double);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[5], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[6], m.Atoms[7], BondOrder.Single);
            m.AddBond(m.Atoms[7], m.Atoms[8], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[8], BondOrder.Single);
            Cycles.MarkRingAtomsAndBonds(m);
            NonplanarBonds.Assign(m);
            int wavyCount = 0;
            foreach (var bond in m.Bonds)
                if (bond.Stereo == BondStereo.UpOrDown)
                    wavyCount++;
            Assert.AreEqual(1, wavyCount);
        }

        /// <summary>
        /// {@code SMILES: *CN=C(CCC)CCN}
        /// </summary>
        [TestMethod()]
        public void UnspecifiedMarkedOnDifferentLigands()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("R", 0, 0.00, -0.00));
            m.Atoms.Add(Atom("C", 2, 1.30, -0.75));
            m.Atoms.Add(Atom("N", 0, 2.60, -0.00));
            m.Atoms.Add(Atom("C", 0, 3.90, -0.75));
            m.Atoms.Add(Atom("C", 2, 5.20, -0.00));
            m.Atoms.Add(Atom("C", 2, 6.50, -0.75));
            m.Atoms.Add(Atom("C", 3, 7.79, -0.00));
            m.Atoms.Add(Atom("C", 2, 3.90, -2.25));
            m.Atoms.Add(Atom("C", 2, 5.20, -3.00));
            m.Atoms.Add(Atom("N", 2, 5.20, -4.50));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Double);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[5], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[7], BondOrder.Single);
            m.AddBond(m.Atoms[7], m.Atoms[8], BondOrder.Single);
            m.AddBond(m.Atoms[8], m.Atoms[9], BondOrder.Single);
            NonplanarBonds.Assign(m);
            int wavyCount = 0;
            foreach (var bond in m.Bonds)
                if (bond.Stereo == BondStereo.UpOrDown)
                    wavyCount++;
            Assert.AreEqual(1, wavyCount);
        }

        static IAtom Atom(string symbol, int hCount, double x, double y)
        {
            IAtom a = new Atom(symbol);
            a.ImplicitHydrogenCount = hCount;
            a.Point2D = new Vector2(x, y);
            return a;
        }
    }
}
