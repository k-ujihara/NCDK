/*
 * Copyright (c) 2014 European Bioinformatics Institute (EMBL-EBI)
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
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using NCDK.Common.Base;
using NCDK.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;
using NCDK.Graphs;
using static NCDK.Graphs.GraphUtil;
using System.Linq;

namespace NCDK.Stereo
{
    [TestClass()]
    public class FischerRecognitionTest
    {
        // @cdk.inchi InChI=1/C3H6O3/c4-1-3(6)2-5/h1,3,5-6H,2H2/t3-/s2
        [TestMethod()]
        public void RecogniseRightHandedGlyceraldehyde()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 0, 0.80d, 1.24d));
            m.Atoms.Add(Atom("C", 0, 0.80d, 0.42d));
            m.Atoms.Add(Atom("O", 1, 0.09d, 1.66d));
            m.Atoms.Add(Atom("O", 0, 1.52d, 1.66d));
            m.Atoms.Add(Atom("O", 1, 1.63d, 0.42d));
            m.Atoms.Add(Atom("C", 2, 0.80d, -0.41d));
            m.Atoms.Add(Atom("H", 0, -0.02d, 0.42d));
            m.Atoms.Add(Atom("O", 1, 1.52d, -0.82d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[3], BondOrder.Double, BondStereo.EZByCoordinates);
            m.AddBond(m.Atoms[1], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[5], m.Atoms[7], BondOrder.Single);

            EdgeToBondMap bondMap = EdgeToBondMap.WithSpaceFor(m);
            int[][] graph = GraphUtil.ToAdjList(m, bondMap);
            FischerRecognition recogniser = new FischerRecognition(m,
                                                                   graph,
                                                                   bondMap,
                                                                   Stereocenters.Of(m));
            var elements = recogniser.Recognise(new[] { Projection.Fischer }).ToList();
            Assert.AreEqual(1, elements.Count);
            AssertTetrahedralCenter(elements[0],
                                    m.Atoms[1],
                                    TetrahedralStereo.AntiClockwise,
                                    m.Atoms[0], m.Atoms[4], m.Atoms[5], m.Atoms[6]);
        }

        // @cdk.inchi InChI=1S/C3H6O3/c4-1-3(6)2-5/h1,3,5-6H,2H2/t3-/m1/s1
        [TestMethod()]
        public void RecogniseLeftHandedGlyceraldehyde()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 0, 0.80d, 1.24d));
            m.Atoms.Add(Atom("C", 0, 0.80d, 0.42d));
            m.Atoms.Add(Atom("O", 1, 0.09d, 1.66d));
            m.Atoms.Add(Atom("O", 0, 1.52d, 1.66d));
            m.Atoms.Add(Atom("O", 0, -0.02d, 0.42d));
            m.Atoms.Add(Atom("C", 2, 0.80d, -0.41d));
            m.Atoms.Add(Atom("H", 1, 1.63d, 0.42d));
            m.Atoms.Add(Atom("O", 1, 1.52d, -0.82d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[3], BondOrder.Double, BondStereo.EZByCoordinates);
            m.AddBond(m.Atoms[1], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[5], m.Atoms[7], BondOrder.Single);

            EdgeToBondMap bondMap = EdgeToBondMap.WithSpaceFor(m);
            int[][] graph = GraphUtil.ToAdjList(m, bondMap);
            FischerRecognition recogniser = new FischerRecognition(m,
                                                                   graph,
                                                                   bondMap,
                                                                   Stereocenters.Of(m));
            var elements = recogniser.Recognise(new[] { Projection.Fischer }).ToList();
            Assert.AreEqual(1, elements.Count);
            AssertTetrahedralCenter(elements[0],
                                    m.Atoms[1],
                                    TetrahedralStereo.AntiClockwise,
                                    m.Atoms[0], m.Atoms[6], m.Atoms[5], m.Atoms[4]);
        }

        // @cdk.inchi InChI=1/C3H6O3/c4-1-3(6)2-5/h1,3,5-6H,2H2/t3-/s2
        [TestMethod()]
        public void RecogniseRightHandedGlyceraldehydeWithImplicitHydrogen()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 0, 0.80d, 1.24d));
            m.Atoms.Add(Atom("C", 1, 0.80d, 0.42d));
            m.Atoms.Add(Atom("O", 1, 0.09d, 1.66d));
            m.Atoms.Add(Atom("O", 0, 1.52d, 1.66d));
            m.Atoms.Add(Atom("O", 1, 1.63d, 0.42d));
            m.Atoms.Add(Atom("C", 2, 0.80d, -0.41d));
            m.Atoms.Add(Atom("O", 1, 1.52d, -0.82d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[3], BondOrder.Double, BondStereo.EZByCoordinates);
            m.AddBond(m.Atoms[1], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[5], m.Atoms[6], BondOrder.Single);

            EdgeToBondMap bondMap = EdgeToBondMap.WithSpaceFor(m);
            int[][] graph = GraphUtil.ToAdjList(m, bondMap);
            FischerRecognition recogniser = new FischerRecognition(m,
                                                                   graph,
                                                                   bondMap,
                                                                   Stereocenters.Of(m));
            var elements = recogniser.Recognise(new[] { Projection.Fischer }).ToList();
            Assert.AreEqual(1, elements.Count);
            AssertTetrahedralCenter(elements[0],
                                    m.Atoms[1],
                                    TetrahedralStereo.AntiClockwise,
                                    m.Atoms[0], m.Atoms[4], m.Atoms[5], m.Atoms[1]);
        }

        // @cdk.inchi InChI=1S/C6H14O6/c7-1-3(9)5(11)6(12)4(10)2-8/h3-12H,1-2H2/t3-,4-,5-,6-/m1/s1
        [TestMethod()]
        public void Mannitol()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 2, -0.53d, 6.25d));
            m.Atoms.Add(Atom("C", 1, -0.53d, 5.42d));
            m.Atoms.Add(Atom("O", 1, 0.18d, 6.66d));
            m.Atoms.Add(Atom("O", 1, -1.36d, 5.42d));
            m.Atoms.Add(Atom("C", 1, -0.53d, 4.60d));
            m.Atoms.Add(Atom("O", 1, -1.36d, 4.60d));
            m.Atoms.Add(Atom("C", 1, -0.53d, 3.77d));
            m.Atoms.Add(Atom("O", 1, 0.29d, 3.77d));
            m.Atoms.Add(Atom("C", 1, -0.53d, 2.95d));
            m.Atoms.Add(Atom("O", 1, 0.29d, 2.95d));
            m.Atoms.Add(Atom("C", 2, -0.53d, 2.12d));
            m.Atoms.Add(Atom("O", 1, 0.05d, 1.54d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[6], m.Atoms[7], BondOrder.Single);
            m.AddBond(m.Atoms[6], m.Atoms[8], BondOrder.Single);
            m.AddBond(m.Atoms[8], m.Atoms[9], BondOrder.Single);
            m.AddBond(m.Atoms[8], m.Atoms[10], BondOrder.Single);
            m.AddBond(m.Atoms[10], m.Atoms[11], BondOrder.Single);

            EdgeToBondMap bondMap = EdgeToBondMap.WithSpaceFor(m);
            int[][] graph = GraphUtil.ToAdjList(m, bondMap);
            FischerRecognition recogniser = new FischerRecognition(m,
                                                                   graph,
                                                                   bondMap,
                                                                   Stereocenters.Of(m));
            var elements = recogniser.Recognise(new[] { Projection.Fischer }).ToList();

            Assert.AreEqual(4, elements.Count);
            AssertTetrahedralCenter(elements[0],
                                    m.Atoms[1],
                                    TetrahedralStereo.AntiClockwise,
                                    m.Atoms[0], m.Atoms[1], m.Atoms[4], m.Atoms[3]);
            AssertTetrahedralCenter(elements[1],
                                    m.Atoms[4],
                                    TetrahedralStereo.AntiClockwise,
                                    m.Atoms[1], m.Atoms[4], m.Atoms[6], m.Atoms[5]);
            AssertTetrahedralCenter(elements[2],
                                    m.Atoms[6],
                                    TetrahedralStereo.AntiClockwise,
                                    m.Atoms[4], m.Atoms[7], m.Atoms[8], m.Atoms[6]);
            AssertTetrahedralCenter(elements[3],
                                    m.Atoms[8],
                                    TetrahedralStereo.AntiClockwise,
                                    m.Atoms[6], m.Atoms[9], m.Atoms[10], m.Atoms[8]);

            m.SetStereoElements(elements);
        }

        [TestMethod()]
        public void ObtainCardinalBonds()
        {
            IAtom focus = Atom("C", 0, 0.80d, 0.42d);

            IAtom north = Atom("C", 0, 0.80d, 1.24d);
            IAtom east = Atom("O", 1, 1.63d, 0.42d);
            IAtom south = Atom("C", 2, 0.80d, -0.41d);
            IAtom west = Atom("H", 0, -0.02d, 0.42d);

            IBond[] expected = new IBond[]{
                new Bond(focus, north),
                new Bond(focus, east),
                new Bond(focus, south),
                new Bond(focus, west)
        };

            IBond[] actual = FischerRecognition.CardinalBonds(focus,
                                                              new IBond[]{expected[1],
                                                                      expected[2],
                                                                      expected[3],
                                                                      expected[0]}
                                                             );
            Assert.IsTrue(Compares.AreDeepEqual(actual, expected));
        }

        /// <summary>
        /// In reality, bonds may not be perfectly orthogonal. Here the N, E, S, and
        /// W atoms are all slightly offset from the focus.
        /// </summary>
        [TestMethod()]
        public void ObtainNonPerfectCardinalBonds()
        {
            IAtom focus = Atom("C", 0, -0.40d, 3.37d);

            IAtom north = Atom("C", 0, -0.43d, 4.18d);
            IAtom east = Atom("O", 1, 0.44d, 3.33d);
            IAtom south = Atom("C", 2, -0.42d, 2.65d);
            IAtom west = Atom("H", 0, -1.21d, 3.36d);

            IBond[] expected = new IBond[]{
                new Bond(focus, north),
                new Bond(focus, east),
                new Bond(focus, south),
                new Bond(focus, west)
        };

            IBond[] actual = FischerRecognition.CardinalBonds(focus,
                                                              new IBond[]{expected[1],
                                                                      expected[2],
                                                                      expected[3],
                                                                      expected[0]}
                                                             );
            Assert.IsTrue(Compares.AreDeepEqual(actual, expected));
        }

        [TestMethod()]
        public void CreateCenterWithFourNeighbors()
        {
            IAtom focus = Atom("C", 0, 0.80d, 0.42d);

            IAtom north = Atom("C", 0, 0.80d, 1.24d);
            IAtom east = Atom("O", 1, 1.63d, 0.42d);
            IAtom south = Atom("C", 2, 0.80d, -0.41d);
            IAtom west = Atom("H", 0, -0.02d, 0.42d);

            IBond[] bonds = new IBond[]{
                new Bond(focus, south),
                new Bond(focus, west),
                new Bond(focus, north),
                new Bond(focus, east)
        };

            ITetrahedralChirality element = FischerRecognition.NewTetrahedralCenter(focus,
                                                                                    bonds);
            Assert.AreSame(focus, element.ChiralAtom);
            Assert.AreEqual(TetrahedralStereo.AntiClockwise, element.Stereo);
            Assert.AreSame(north, element.Ligands[0]);
            Assert.AreSame(east, element.Ligands[1]);
            Assert.AreSame(south, element.Ligands[2]);
            Assert.AreSame(west, element.Ligands[3]);
        }

        [TestMethod()]
        public void CreateCenterWithThreeNeighbors_right()
        {
            IAtom focus = Atom("C", 0, 0.80d, 0.42d);

            IAtom north = Atom("C", 0, 0.80d, 1.24d);
            IAtom east = Atom("O", 1, 1.63d, 0.42d);
            IAtom south = Atom("C", 2, 0.80d, -0.41d);

            IBond[] bonds = new IBond[]{
                new Bond(focus, south),
                new Bond(focus, north),
                new Bond(focus, east)
        };

            ITetrahedralChirality element = FischerRecognition.NewTetrahedralCenter(focus,
                                                                                    bonds);
            Assert.AreSame(focus, element.ChiralAtom);
            Assert.AreEqual(TetrahedralStereo.AntiClockwise, element.Stereo);
            Assert.AreSame(north, element.Ligands[0]);
            Assert.AreSame(east, element.Ligands[1]);
            Assert.AreSame(south, element.Ligands[2]);
            Assert.AreSame(focus, element.Ligands[3]);
        }

        [TestMethod()]
        public void CreateCenterWithThreeNeighbors_left()
        {
            IAtom focus = Atom("C", 0, 0.80d, 0.42d);

            IAtom north = Atom("C", 0, 0.80d, 1.24d);
            IAtom south = Atom("C", 2, 0.80d, -0.41d);
            IAtom west = Atom("O", 1, -0.02d, 0.42d);

            IBond[] bonds = new IBond[]
                {
                    new Bond(focus, south),
                    new Bond(focus, north),
                    new Bond(focus, west)
                };

            ITetrahedralChirality element = FischerRecognition.NewTetrahedralCenter(focus,
                                                                                    bonds);
            Assert.AreSame(focus, element.ChiralAtom);
            Assert.AreEqual(TetrahedralStereo.AntiClockwise, element.Stereo);
            Assert.AreSame(north, element.Ligands[0]);
            Assert.AreSame(focus, element.Ligands[1]);
            Assert.AreSame(south, element.Ligands[2]);
            Assert.AreSame(west, element.Ligands[3]);
        }

        [TestMethod()]
        public void DoNotCreateCenterWhenNorthIsMissing()
        {
            IAtom focus = Atom("C", 0, 0.80d, 0.42d);

            IAtom east = Atom("O", 1, 1.63d, 0.42d);
            IAtom south = Atom("C", 2, 0.80d, -0.41d);
            IAtom west = Atom("H", 0, -0.02d, 0.42d);

            IBond[] bonds = new IBond[]{
                new Bond(focus, south),
                new Bond(focus, west),
                new Bond(focus, east)
        };

            ITetrahedralChirality element = FischerRecognition.NewTetrahedralCenter(focus,
                                                                                    bonds);
            Assert.IsNull(element);
        }

        [TestMethod()]
        public void DoNotCreateCenterWhenSouthIsMissing()
        {
            IAtom focus = Atom("C", 0, 0.80d, 0.42d);

            IAtom north = Atom("C", 0, 0.80d, 1.24d);
            IAtom east = Atom("O", 1, 1.63d, 0.42d);
            IAtom west = Atom("H", 0, -0.02d, 0.42d);

            IBond[] bonds = new IBond[]{
                new Bond(focus, north),
                new Bond(focus, west),
                new Bond(focus, east)
        };

            ITetrahedralChirality element = FischerRecognition.NewTetrahedralCenter(focus,
                                                                                    bonds);
            Assert.IsNull(element);
        }

        [TestMethod()]
        public void DoNotCreateCenterWhenNorthIsOffCenter()
        {
            IAtom focus = Atom("C", 0, 0.80d, 0.42d);

            IAtom north = Atom("C", 0, 1d, 1.24d);
            IAtom east = Atom("O", 1, 1.63d, 0.42d);
            IAtom south = Atom("C", 2, 0.80d, -0.41d);
            IAtom west = Atom("H", 0, -0.02d, 0.42d);

            IBond[] bonds = new IBond[]{
                new Bond(focus, north),
                new Bond(focus, south),
                new Bond(focus, west),
                new Bond(focus, east)
        };

            ITetrahedralChirality element = FischerRecognition.NewTetrahedralCenter(focus,
                                                                                    bonds);
            Assert.IsNull(element);
        }

        [TestMethod()]
        public void DoNotCreateCenterWhenSouthIsOffCenter()
        {
            IAtom focus = Atom("C", 0, 0.80d, 0.42d);

            IAtom north = Atom("C", 0, 0.80d, 1.24d);
            IAtom east = Atom("O", 1, 1.63d, 0.42d);
            IAtom south = Atom("C", 2, 1d, -0.41d);
            IAtom west = Atom("H", 0, -0.02d, 0.42d);

            IBond[] bonds = new IBond[]{
                new Bond(focus, north),
                new Bond(focus, south),
                new Bond(focus, west),
                new Bond(focus, east)
        };

            ITetrahedralChirality element = FischerRecognition.NewTetrahedralCenter(focus,
                                                                                    bonds);
            Assert.IsNull(element);
        }

        [TestMethod()]
        public void DoNotCreateCenterWhenEastIsOffCenter()
        {
            IAtom focus = Atom("C", 0, 0.80d, 0.42d);

            IAtom north = Atom("C", 0, 0.80d, 1.24d);
            IAtom east = Atom("O", 1, 1.63d, 0.8d);
            IAtom south = Atom("C", 2, 0.80d, -0.41d);
            IAtom west = Atom("H", 0, -0.02d, 0.42d);

            IBond[] bonds = new IBond[]{
                new Bond(focus, north),
                new Bond(focus, south),
                new Bond(focus, west),
                new Bond(focus, east)
        };

            ITetrahedralChirality element = FischerRecognition.NewTetrahedralCenter(focus,
                                                                                    bonds);
            Assert.IsNull(element);
        }


        [TestMethod()]
        public void DoNotCreateCenterWhenWestIsOffCenter()
        {
            IAtom focus = Atom("C", 0, 0.80d, 0.42d);

            IAtom north = Atom("C", 0, 0.80d, 1.24d);
            IAtom east = Atom("O", 1, 1.63d, 0.8d);
            IAtom south = Atom("C", 2, 0.80d, -0.41d);
            IAtom west = Atom("H", 0, -0.02d, 0.42d);

            IBond[] bonds = new IBond[]{
                new Bond(focus, north),
                new Bond(focus, south),
                new Bond(focus, west),
                new Bond(focus, east)
        };

            ITetrahedralChirality element = FischerRecognition.NewTetrahedralCenter(focus,
                                                                                    bonds);
            Assert.IsNull(element);
        }

        [TestMethod()]
        public void DoNotCreateCenterWhenEastAndWestAreMissing()
        {
            IAtom focus = Atom("C", 0, 0.80d, 0.42d);

            IAtom north = Atom("C", 0, 0.80d, 1.24d);
            IAtom south = Atom("C", 2, 0.80d, -0.41d);

            IBond[] bonds = new IBond[]{
                new Bond(focus, north),
                new Bond(focus, south)
        };

            ITetrahedralChirality element = FischerRecognition.NewTetrahedralCenter(focus,
                                                                                    bonds);
            Assert.IsNull(element);
        }

        // rotate
        [TestMethod()]
        public void DoNotCreateCenterWhenRotated()
        {
            IAtom focus = Atom("C", 0, 0.44d, 3.30d);

            IAtom north = Atom("C", 3, -0.16d, 3.86d);
            IAtom east = Atom("O", 1, 1.00d, 3.90d);
            IAtom south = Atom("C", 3, 1.05d, 2.74d);
            IAtom west = Atom("H", 0, -0.12d, 2.70d);


            IBond[] bonds = new IBond[]{
                new Bond(focus, west),
                new Bond(focus, north),
                new Bond(focus, south),
                new Bond(focus, east)
        };

            ITetrahedralChirality element = FischerRecognition.NewTetrahedralCenter(focus,
                                                                                    bonds);
            Assert.IsNull(element);
        }

        /// <summary>
        /// asperaculin A (CHEBI:68202)
        /// </summary>
        // @cdk.inchi InChI=1S/C15H20O5/c1-12(2)6-13(3)7-19-10(16)9-15(13)8(12)4-5-14(15,18)11(17)20-9/h8-9,18H,4-7H2,1-3H3/t8-,9+,13+,14+,15?/m0/s1 
        [TestMethod()]
        public void IgnoreCyclicStereocenters()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 0, 6.87d, -5.59d));
            m.Atoms.Add(Atom("C", 0, 6.87d, -6.61d));
            m.Atoms.Add(Atom("C", 0, 7.82d, -5.62d));
            m.Atoms.Add(Atom("C", 0, 6.87d, -4.59d));
            m.Atoms.Add(Atom("O", 0, 8.18d, -6.34d));
            m.Atoms.Add(Atom("C", 0, 7.62d, -6.91d));
            m.Atoms.Add(Atom("C", 0, 5.90d, -5.59d));
            m.Atoms.Add(Atom("C", 0, 8.39d, -5.06d));
            m.Atoms.Add(Atom("C", 0, 5.60d, -4.80d));
            m.Atoms.Add(Atom("C", 2, 6.16d, -4.24d));
            m.Atoms.Add(Atom("O", 0, 8.22d, -4.29d));
            m.Atoms.Add(Atom("C", 2, 6.10d, -6.90d));
            m.Atoms.Add(Atom("C", 2, 5.54d, -6.29d));
            m.Atoms.Add(Atom("C", 2, 7.46d, -4.07d));
            m.Atoms.Add(Atom("O", 0, 7.79d, -7.72d));
            m.Atoms.Add(Atom("O", 0, 9.18d, -5.29d));
            m.Atoms.Add(Atom("O", 1, 6.87d, -7.44d));
            m.Atoms.Add(Atom("C", 3, 6.76d, -3.77d));
            m.Atoms.Add(Atom("C", 3, 4.82d, -5.07d));
            m.Atoms.Add(Atom("C", 3, 5.19d, -4.08d));
            m.Atoms.Add(Atom("H", 0, 8.64d, -5.76d));
            m.Atoms.Add(Atom("H", 0, 5.08d, -5.69d));
            m.AddBond(m.Atoms[1], m.Atoms[0], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[0], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[5], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[7], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[8], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[9], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[10], m.Atoms[7], BondOrder.Single);
            m.AddBond(m.Atoms[11], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[12], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[13], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[14], m.Atoms[5], BondOrder.Double, BondStereo.EZByCoordinates);
            m.AddBond(m.Atoms[15], m.Atoms[7], BondOrder.Double, BondStereo.EZByCoordinates);
            m.AddBond(m.Atoms[1], m.Atoms[16], BondOrder.Single, BondStereo.Up);
            m.AddBond(m.Atoms[3], m.Atoms[17], BondOrder.Single, BondStereo.Up);
            m.AddBond(m.Atoms[18], m.Atoms[8], BondOrder.Single);
            m.AddBond(m.Atoms[19], m.Atoms[8], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[20], BondOrder.Single, BondStereo.Down);
            m.AddBond(m.Atoms[6], m.Atoms[21], BondOrder.Single, BondStereo.Down);
            m.AddBond(m.Atoms[5], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[11], m.Atoms[12], BondOrder.Single);
            m.AddBond(m.Atoms[10], m.Atoms[13], BondOrder.Single);
            m.AddBond(m.Atoms[8], m.Atoms[9], BondOrder.Single);

            EdgeToBondMap bondMap = EdgeToBondMap.WithSpaceFor(m);
            int[][] graph = GraphUtil.ToAdjList(m, bondMap);
            FischerRecognition recogniser = new FischerRecognition(m,
                                                                   graph,
                                                                   bondMap,
                                                                   Stereocenters.Of(m));
            Assert.IsTrue(recogniser.Recognise(new[] { Projection.Fischer }).Count() == 0);
        }

        /// <summary>
        /// atrolactic acid (CHEBI:50392)
        /// </summary>
        // @cdk.inchi InChI=1S/C9H10O3/c1-9(12,8(10)11)7-5-3-2-4-6-7/h2-6,12H,1H3,(H,10,11)
        [TestMethod()]
        public void HorizontalBondsMustBeTerminal()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 0, 12.71d, -16.51d));
            m.Atoms.Add(Atom("C", 1, 12.30d, -17.22d));
            m.Atoms.Add(Atom("C", 1, 11.47d, -17.22d));
            m.Atoms.Add(Atom("C", 1, 11.06d, -16.51d));
            m.Atoms.Add(Atom("C", 1, 11.47d, -15.79d));
            m.Atoms.Add(Atom("C", 1, 12.30d, -15.79d));
            m.Atoms.Add(Atom("O", 1, 13.54d, -17.33d));
            m.Atoms.Add(Atom("C", 0, 13.54d, -16.51d));
            m.Atoms.Add(Atom("C", 0, 14.36d, -16.51d));
            m.Atoms.Add(Atom("O", 1, 14.77d, -17.22d));
            m.Atoms.Add(Atom("O", 0, 14.77d, -15.79d));
            m.Atoms.Add(Atom("C", 3, 13.54d, -15.68d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[5], BondOrder.Double, BondStereo.EZByCoordinates);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double, BondStereo.EZByCoordinates);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Double, BondStereo.EZByCoordinates);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[7], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[7], m.Atoms[8], BondOrder.Single);
            m.AddBond(m.Atoms[8], m.Atoms[9], BondOrder.Single);
            m.AddBond(m.Atoms[8], m.Atoms[10], BondOrder.Double, BondStereo.EZByCoordinates);
            m.AddBond(m.Atoms[0], m.Atoms[7], BondOrder.Single);
            m.AddBond(m.Atoms[11], m.Atoms[7], BondOrder.Single);

            EdgeToBondMap bondMap = EdgeToBondMap.WithSpaceFor(m);
            int[][] graph = GraphUtil.ToAdjList(m, bondMap);
            FischerRecognition recogniser = new FischerRecognition(m,
                                                                   graph,
                                                                   bondMap,
                                                                   Stereocenters.Of(m));
            Assert.IsTrue(recogniser.Recognise(new[] { Projection.Fischer }).Count() == 0);
        }

        static void AssertTetrahedralCenter(IStereoElement<IChemObject, IChemObject> element,
                                            IAtom focus,
                                            TetrahedralStereo winding,
                                            params IAtom[] neighbors)
        {
            Assert.IsInstanceOfType(element, typeof(ITetrahedralChirality));
            ITetrahedralChirality actual = (ITetrahedralChirality)element;
            Assert.AreSame(focus, actual.ChiralAtom);
            Assert.AreEqual(winding, actual.Stereo);
            Assert.IsTrue(Compares.AreDeepEqual(neighbors, actual.Ligands));
        }

        static IAtom Atom(string symbol, int h, double x, double y)
        {
            IAtom a = new Atom(symbol)
            {
                ImplicitHydrogenCount = h,
                Point2D = new Vector2(x, y),
            };
            return a;
        }
    }
}
