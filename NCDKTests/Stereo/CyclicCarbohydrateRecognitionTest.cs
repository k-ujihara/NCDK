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
using NCDK.Common.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.Geometries;
using NCDK.Graphs;
using NCDK.Numerics;
using static NCDK.Graphs.GraphUtil;
using static NCDK.Stereo.CyclicCarbohydrateRecognition;
using static NCDK.Stereo.CyclicCarbohydrateRecognition.Turn;

namespace NCDK.Stereo
{
    [TestClass()]
    public class CyclicCarbohydrateRecognitionTest
    {

        [TestMethod()]
        public void haworthAnticlockwise()
        {
            Assert.IsTrue(Compares.AreDeepEqual(
                new Turn[] { Left, Left, Left, Left, Left, Left },
                CyclicCarbohydrateRecognition.Turns(new Vector2[]{
                new Vector2(4.1, 3.0),
                new Vector2(3.3, 2.6),
                new Vector2(3.3, 1.8),
                new Vector2(4.1, 1.4),
                new Vector2(4.8, 1.8),
                new Vector2(4.8, 2.6),
                    })));
        }

        [TestMethod()]
        public void haworthClockwise()
        {
            Assert.IsTrue(Compares.AreDeepEqual(
                new Turn[] { Right, Right, Right, Right, Right, Right },
                CyclicCarbohydrateRecognition.Turns(new Vector2[]{
                    new Vector2(4.1, 3.0),
                    new Vector2(4.8, 2.6),
                    new Vector2(4.8, 1.8),
                    new Vector2(4.1, 1.4),
                    new Vector2(3.3, 1.8),
                    new Vector2(3.3, 2.6)
                })));
        }

        [TestMethod()]
        public void chairAnticlockwise()
        {
            Assert.IsTrue(Compares.AreDeepEqual(
                new Turn[] { Left, Right, Right, Left, Right, Right },
                CyclicCarbohydrateRecognition.Turns(new Vector2[]{
                new Vector2(0.9, 2.6),
                new Vector2(0.1, 2.4),
                new Vector2(0.2, 3.1),
                new Vector2(0.5, 2.9),
                new Vector2(1.3, 3.1),
                new Vector2(1.7, 2.4)
                })));
        }

        [TestMethod()]
        public void chairClockwise()
        {
            Assert.IsTrue(Compares.AreDeepEqual(
                new Turn[] { Left, Left, Right, Left, Left, Right },
                CyclicCarbohydrateRecognition.Turns(new Vector2[]{
                new Vector2(1.7, 2.4),
                new Vector2(1.3, 3.1),
                new Vector2(0.5, 2.9),
                new Vector2(0.2, 3.1),
                new Vector2(0.1, 2.4),
                new Vector2(0.9, 2.6)
                })));
        }

        [TestMethod()]
        public void boatAnticlockwise()
        {
            Assert.IsTrue(Compares.AreDeepEqual(
                new Turn[] { Right, Right, Right, Left, Left, Right },
CyclicCarbohydrateRecognition.Turns(new Vector2[]{
                new Vector2(3.3, 3.8),
                new Vector2(2.1, 3.8),
                new Vector2(1.6, 4.9),
                new Vector2(2.3, 4.2),
                new Vector2(3.1, 4.2),
                new Vector2(3.8, 4.8)
                })));
        }

        [TestMethod()]
        public void boatClockwise()
        {
            Assert.IsTrue(Compares.AreDeepEqual(
                new Turn[] { Left, Right, Right, Left, Left, Left },
CyclicCarbohydrateRecognition.Turns(new Vector2[]{
                new Vector2(3.8, 4.8),
                new Vector2(3.1, 4.2),
                new Vector2(2.3, 4.2),
                new Vector2(1.6, 4.9),
                new Vector2(2.1, 3.8),
                new Vector2(3.3, 3.8)
        })));
        }

        /**
         * @cdk.inchi InChI=1/C6H12O6/c7-1-2-3(8)4(9)5(10)6(11)12-2/h2-11H,1H2/t2-,3-,4+,5-,6-/s2
         */
        [TestMethod()]
        public void betaDGlucose_Haworth()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, 4.16d, 1.66d));
            m.Atoms.Add(Atom("C", 1, 3.75d, 0.94d));
            m.Atoms.Add(Atom("C", 1, 4.16d, 0.23d));
            m.Atoms.Add(Atom("C", 1, 5.05d, 0.23d));
            m.Atoms.Add(Atom("C", 1, 5.46d, 0.94d));
            m.Atoms.Add(Atom("O", 0, 5.05d, 1.66d));
            m.Atoms.Add(Atom("O", 1, 5.46d, 1.77d));
            m.Atoms.Add(Atom("C", 2, 4.16d, 2.48d));
            m.Atoms.Add(Atom("O", 1, 3.45d, 2.89d));
            m.Atoms.Add(Atom("O", 1, 3.75d, 0.12d));
            m.Atoms.Add(Atom("O", 1, 4.16d, 1.05d));
            m.Atoms.Add(Atom("O", 1, 5.05d, -0.60d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[7], BondOrder.Single);
            m.AddBond(m.Atoms[7], m.Atoms[8], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[9], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[10], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[11], BondOrder.Single);

            EdgeToBondMap bondMap = EdgeToBondMap.WithSpaceFor(m);
            int[][] graph = GraphUtil.ToAdjList(m, bondMap);
            CyclicCarbohydrateRecognition recon = new CyclicCarbohydrateRecognition(m, graph, bondMap,
                                                                                      new Stereocenters(m, graph, bondMap));

            var elements = recon.Recognise(new[] { Projection.Haworth });
            AssertTetrahedralCenter(elements[0],
                                    m.Atoms[1],
                                    TetrahedralStereo.AntiClockwise,
                                    m.Atoms[1], m.Atoms[0], m.Atoms[9], m.Atoms[2]);
            AssertTetrahedralCenter(elements[1],
                                    m.Atoms[2],
                                    TetrahedralStereo.AntiClockwise,
                                    m.Atoms[10], m.Atoms[1], m.Atoms[2], m.Atoms[3]);
            AssertTetrahedralCenter(elements[2],
                                    m.Atoms[3],
                                    TetrahedralStereo.AntiClockwise,
                                    m.Atoms[3], m.Atoms[2], m.Atoms[11], m.Atoms[4]);
            AssertTetrahedralCenter(elements[3],
                                    m.Atoms[4],
                                    TetrahedralStereo.AntiClockwise,
                                    m.Atoms[6], m.Atoms[3], m.Atoms[4], m.Atoms[5]);
            AssertTetrahedralCenter(elements[4],
                                    m.Atoms[0],
                                    TetrahedralStereo.AntiClockwise,
                                    m.Atoms[7], m.Atoms[5], m.Atoms[0], m.Atoms[1]);
        }

        /**
         * @cdk.inchi InChI=1/C6H12O6/c7-1-2-3(8)4(9)5(10)6(11)12-2/h2-11H,1H2/t2-,3-,4+,5-,6-/s2
         */
        [TestMethod()]
        public void betaDGlucose_Chair()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, -0.77d, 10.34d));
            m.Atoms.Add(Atom("C", 1, 0.03d, 10.13d));
            m.Atoms.Add(Atom("O", 0, 0.83d, 10.34d));
            m.Atoms.Add(Atom("C", 1, 1.24d, 9.63d));
            m.Atoms.Add(Atom("C", 1, 0.44d, 9.84d));
            m.Atoms.Add(Atom("C", 1, -0.35d, 9.63d));
            m.Atoms.Add(Atom("O", 1, 0.86d, 9.13d));
            m.Atoms.Add(Atom("O", 1, 2.04d, 9.84d));
            m.Atoms.Add(Atom("C", 2, -0.68d, 10.54d));
            m.Atoms.Add(Atom("O", 1, -0.68d, 11.37d));
            m.Atoms.Add(Atom("O", 1, -1.48d, 9.93d));
            m.Atoms.Add(Atom("O", 1, -1.15d, 9.84d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[5], m.Atoms[0], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[7], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[8], BondOrder.Single);
            m.AddBond(m.Atoms[8], m.Atoms[9], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[10], BondOrder.Single);
            m.AddBond(m.Atoms[5], m.Atoms[11], BondOrder.Single);


            EdgeToBondMap bondMap = EdgeToBondMap.WithSpaceFor(m);
            int[][] graph = GraphUtil.ToAdjList(m, bondMap);
            CyclicCarbohydrateRecognition recon = new CyclicCarbohydrateRecognition(m, graph, bondMap,
                                                                                      new Stereocenters(m, graph, bondMap));

            var elements = recon.Recognise(new[] { Projection.Chair });
            AssertTetrahedralCenter(elements[0],
                                    m.Atoms[1],
                                    TetrahedralStereo.Clockwise,
                                    m.Atoms[8], m.Atoms[0], m.Atoms[1], m.Atoms[2]);
            AssertTetrahedralCenter(elements[1],
                                    m.Atoms[3],
                                    TetrahedralStereo.Clockwise,
                                    m.Atoms[7], m.Atoms[2], m.Atoms[3], m.Atoms[4]);
            AssertTetrahedralCenter(elements[2],
                                    m.Atoms[4],
                                    TetrahedralStereo.Clockwise,
                                    m.Atoms[4], m.Atoms[3], m.Atoms[6], m.Atoms[5]);
            AssertTetrahedralCenter(elements[3],
                                    m.Atoms[5],
                                    TetrahedralStereo.Clockwise,
                                    m.Atoms[11], m.Atoms[4], m.Atoms[5], m.Atoms[0]);
            AssertTetrahedralCenter(elements[4],
                                    m.Atoms[0],
                                    TetrahedralStereo.Clockwise,
                                    m.Atoms[0], m.Atoms[5], m.Atoms[10], m.Atoms[1]);
        }

        /**
         * @cdk.inchi InChI=1/C6H12O6/c7-1-2-3(8)4(9)5(10)6(11)12-2/h2-11H,1H2/t2-,3-,4+,5-,6-/s2
         */
        [TestMethod()]
        public void betaDGlucoseWithExplicitHydrogens_Haworth()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 0, 4.16d, 1.66d));
            m.Atoms.Add(Atom("C", 0, 3.75d, 0.94d));
            m.Atoms.Add(Atom("C", 0, 4.16d, 0.23d));
            m.Atoms.Add(Atom("C", 0, 5.05d, 0.23d));
            m.Atoms.Add(Atom("C", 0, 5.46d, 0.94d));
            m.Atoms.Add(Atom("O", 0, 5.05d, 1.66d));
            m.Atoms.Add(Atom("O", 1, 5.46d, 1.48d));
            m.Atoms.Add(Atom("C", 2, 4.16d, 2.20d));
            m.Atoms.Add(Atom("O", 1, 3.45d, 2.61d));
            m.Atoms.Add(Atom("O", 1, 3.74d, 0.50d));
            m.Atoms.Add(Atom("O", 1, 4.16d, 0.77d));
            m.Atoms.Add(Atom("O", 1, 5.04d, -0.21d));
            m.Atoms.Add(Atom("H", 0, 4.15d, -0.21d));
            m.Atoms.Add(Atom("H", 0, 5.05d, 0.77d));
            m.Atoms.Add(Atom("H", 0, 5.45d, 0.50d));
            m.Atoms.Add(Atom("H", 0, 3.75d, 1.48d));
            m.Atoms.Add(Atom("H", 0, 4.17d, 1.15d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[7], BondOrder.Single);
            m.AddBond(m.Atoms[7], m.Atoms[8], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[9], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[10], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[11], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[12], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[13], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[14], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[15], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[16], BondOrder.Single);

            EdgeToBondMap bondMap = EdgeToBondMap.WithSpaceFor(m);
            int[][] graph = GraphUtil.ToAdjList(m, bondMap);

            CyclicCarbohydrateRecognition recon = new CyclicCarbohydrateRecognition(m, graph, bondMap,
                                                                     new Stereocenters(m, graph, bondMap));
            var elements = recon.Recognise(new[] { Projection.Haworth });
            AssertTetrahedralCenter(elements[0],
                                    m.Atoms[1],
                                    TetrahedralStereo.AntiClockwise,
                                    m.Atoms[15], m.Atoms[0], m.Atoms[9], m.Atoms[2]);
            AssertTetrahedralCenter(elements[1],
                                    m.Atoms[2],
                                    TetrahedralStereo.AntiClockwise,
                                    m.Atoms[10], m.Atoms[1], m.Atoms[12], m.Atoms[3]);
            AssertTetrahedralCenter(elements[2],
                                    m.Atoms[3],
                                    TetrahedralStereo.AntiClockwise,
                                    m.Atoms[13], m.Atoms[2], m.Atoms[11], m.Atoms[4]);
            AssertTetrahedralCenter(elements[3],
                                    m.Atoms[4],
                                    TetrahedralStereo.AntiClockwise,
                                    m.Atoms[6], m.Atoms[3], m.Atoms[14], m.Atoms[5]);
            AssertTetrahedralCenter(elements[4],
                                    m.Atoms[0],
                                    TetrahedralStereo.AntiClockwise,
                                    m.Atoms[7], m.Atoms[5], m.Atoms[16], m.Atoms[1]);

        }

        /**
         * Example from: http://www.google.com/patents/WO2008025160A1?cl=en
         * @cdk.inchi InChI=1S/C13H26O5/c1-4-10-7-11(17-6-5-16-3)9(2)18-12(8-14)13(10)15/h9-15H,4-8H2,1-3H3/t9-,10+,11+,12+,13-/m0/s1
         */
        [TestMethod()]
        public void oxpene()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, 1.39d, 3.65d));
            m.Atoms.Add(Atom("C", 2, 2.22d, 3.65d));
            m.Atoms.Add(Atom("C", 1, 2.93d, 4.07d));
            m.Atoms.Add(Atom("C", 1, 0.68d, 4.07d));
            m.Atoms.Add(Atom("C", 1, 1.01d, 4.63d));
            m.Atoms.Add(Atom("C", 1, 2.52d, 4.64d));
            m.Atoms.Add(Atom("O", 0, 1.76d, 4.89d));
            m.Atoms.Add(Atom("O", 1, 0.68d, 3.24d));
            m.Atoms.Add(Atom("C", 2, 1.01d, 5.45d));
            m.Atoms.Add(Atom("O", 1, 0.18d, 5.45d));
            m.Atoms.Add(Atom("C", 3, 2.52d, 5.46d));
            m.Atoms.Add(Atom("O", 0, 2.93d, 3.24d));
            m.Atoms.Add(Atom("C", 2, 1.39d, 4.48d));
            m.Atoms.Add(Atom("C", 3, 2.22d, 4.48d));
            m.Atoms.Add(Atom("C", 2, 3.76d, 3.24d));
            m.Atoms.Add(Atom("C", 2, 4.34d, 2.66d));
            m.Atoms.Add(Atom("O", 0, 5.16d, 2.66d));
            m.Atoms.Add(Atom("C", 3, 5.58d, 3.37d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[5], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[7], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[8], BondOrder.Single);
            m.AddBond(m.Atoms[8], m.Atoms[9], BondOrder.Single);
            m.AddBond(m.Atoms[5], m.Atoms[10], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[11], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[12], BondOrder.Single);
            m.AddBond(m.Atoms[12], m.Atoms[13], BondOrder.Single);
            m.AddBond(m.Atoms[11], m.Atoms[14], BondOrder.Single);
            m.AddBond(m.Atoms[14], m.Atoms[15], BondOrder.Single);
            m.AddBond(m.Atoms[15], m.Atoms[16], BondOrder.Single);
            m.AddBond(m.Atoms[16], m.Atoms[17], BondOrder.Single);
            EdgeToBondMap bondMap = EdgeToBondMap.WithSpaceFor(m);
            int[][] graph = GraphUtil.ToAdjList(m, bondMap);

            CyclicCarbohydrateRecognition recon = new CyclicCarbohydrateRecognition(m, graph, bondMap,
                                                                                    new Stereocenters(m, graph, bondMap));
            var elements = recon.Recognise(new[] { Projection.Haworth });
            AssertTetrahedralCenter(elements[0],
                                    m.Atoms[2],
                                    TetrahedralStereo.AntiClockwise,
                                    m.Atoms[2], m.Atoms[1], m.Atoms[11], m.Atoms[5]);
            AssertTetrahedralCenter(elements[1],
                                    m.Atoms[5],
                                    TetrahedralStereo.AntiClockwise,
                                    m.Atoms[10], m.Atoms[2], m.Atoms[5], m.Atoms[6]);
            AssertTetrahedralCenter(elements[2],
                                    m.Atoms[4],
                                    TetrahedralStereo.AntiClockwise,
                                    m.Atoms[8], m.Atoms[6], m.Atoms[4], m.Atoms[3]);
            AssertTetrahedralCenter(elements[3],
                                    m.Atoms[3],
                                    TetrahedralStereo.AntiClockwise,
                                    m.Atoms[3], m.Atoms[4], m.Atoms[7], m.Atoms[0]);
            AssertTetrahedralCenter(elements[4],
                                    m.Atoms[0],
                                    TetrahedralStereo.AntiClockwise,
                                    m.Atoms[12], m.Atoms[3], m.Atoms[0], m.Atoms[1]);
        }

        /**
         * @cdk.inchi InChI=1S/C10H16N5O13P3/c11-8-5-9(13-2-12-8)15(3-14-5)10-7(17)6(16)4(26-10)1-25-30(21,22)28-31(23,24)27-29(18,19)20/h2-4,6-7,10,16-17H,1H2,(H,21,22)(H,23,24)(H2,11,12,13)(H2,18,19,20)/t4-,6-,7-,10-/m1/s1
         */
        [TestMethod()]
        public void Atp_Haworth()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("O", 0, 2.56d, -6.46d));
            m.Atoms.Add(Atom("C", 1, 1.90d, -6.83d));
            m.Atoms.Add(Atom("C", 1, 2.15d, -7.46d));
            m.Atoms.Add(Atom("C", 1, 2.98d, -7.46d));
            m.Atoms.Add(Atom("C", 1, 3.23d, -6.83d));
            m.Atoms.Add(Atom("C", 2, 1.90d, -6.00d));
            m.Atoms.Add(Atom("O", 0, 1.18d, -5.59d));
            m.Atoms.Add(Atom("O", 1, 2.15d, -8.29d));
            m.Atoms.Add(Atom("O", 1, 2.98d, -8.29d));
            m.Atoms.Add(Atom("P", 0, 0.36d, -5.59d));
            m.Atoms.Add(Atom("O", 0, -0.47d, -5.59d));
            m.Atoms.Add(Atom("O", 0, 0.36d, -4.76d));
            m.Atoms.Add(Atom("O", 1, 0.36d, -6.41d));
            m.Atoms.Add(Atom("P", 0, -1.29d, -5.59d));
            m.Atoms.Add(Atom("O", 0, -2.12d, -5.59d));
            m.Atoms.Add(Atom("O", 0, -1.29d, -4.76d));
            m.Atoms.Add(Atom("O", 1, -1.29d, -6.41d));
            m.Atoms.Add(Atom("P", 0, -2.94d, -5.59d));
            m.Atoms.Add(Atom("O", 1, -3.77d, -5.59d));
            m.Atoms.Add(Atom("O", 0, -2.94d, -4.76d));
            m.Atoms.Add(Atom("O", 1, -2.94d, -6.41d));
            m.Atoms.Add(Atom("C", 0, 4.73d, -4.51d));
            m.Atoms.Add(Atom("C", 0, 4.02d, -4.92d));
            m.Atoms.Add(Atom("C", 0, 4.02d, -5.75d));
            m.Atoms.Add(Atom("N", 0, 4.73d, -6.16d));
            m.Atoms.Add(Atom("N", 0, 5.44d, -5.75d));
            m.Atoms.Add(Atom("C", 1, 5.44d, -4.92d));
            m.Atoms.Add(Atom("C", 1, 2.75d, -5.33d));
            m.Atoms.Add(Atom("N", 0, 3.23d, -4.67d));
            m.Atoms.Add(Atom("N", 2, 4.73d, -3.68d));
            m.Atoms.Add(Atom("N", 0, 3.23d, -6.00d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[5], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[7], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[8], BondOrder.Single);
            m.AddBond(m.Atoms[6], m.Atoms[9], BondOrder.Single);
            m.AddBond(m.Atoms[9], m.Atoms[10], BondOrder.Single);
            m.AddBond(m.Atoms[9], m.Atoms[11], BondOrder.Double, BondStereo.EZByCoordinates);
            m.AddBond(m.Atoms[9], m.Atoms[12], BondOrder.Single);
            m.AddBond(m.Atoms[13], m.Atoms[14], BondOrder.Single);
            m.AddBond(m.Atoms[13], m.Atoms[15], BondOrder.Double, BondStereo.EZByCoordinates);
            m.AddBond(m.Atoms[13], m.Atoms[16], BondOrder.Single);
            m.AddBond(m.Atoms[10], m.Atoms[13], BondOrder.Single);
            m.AddBond(m.Atoms[17], m.Atoms[18], BondOrder.Single);
            m.AddBond(m.Atoms[17], m.Atoms[19], BondOrder.Double, BondStereo.EZByCoordinates);
            m.AddBond(m.Atoms[17], m.Atoms[20], BondOrder.Single);
            m.AddBond(m.Atoms[14], m.Atoms[17], BondOrder.Single);
            m.AddBond(m.Atoms[21], m.Atoms[22], BondOrder.Double, BondStereo.EZByCoordinates);
            m.AddBond(m.Atoms[22], m.Atoms[23], BondOrder.Single);
            m.AddBond(m.Atoms[23], m.Atoms[24], BondOrder.Double, BondStereo.EZByCoordinates);
            m.AddBond(m.Atoms[24], m.Atoms[25], BondOrder.Single);
            m.AddBond(m.Atoms[25], m.Atoms[26], BondOrder.Double, BondStereo.EZByCoordinates);
            m.AddBond(m.Atoms[21], m.Atoms[26], BondOrder.Single);
            m.AddBond(m.Atoms[27], m.Atoms[28], BondOrder.Double, BondStereo.EZByCoordinates);
            m.AddBond(m.Atoms[22], m.Atoms[28], BondOrder.Single);
            m.AddBond(m.Atoms[21], m.Atoms[29], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[30], BondOrder.Single);
            m.AddBond(m.Atoms[30], m.Atoms[27], BondOrder.Single);
            m.AddBond(m.Atoms[23], m.Atoms[30], BondOrder.Single);

            EdgeToBondMap bondMap = EdgeToBondMap.WithSpaceFor(m);
            int[][] graph = GraphUtil.ToAdjList(m, bondMap);

            CyclicCarbohydrateRecognition recon = new CyclicCarbohydrateRecognition(m, graph, bondMap,
                                                                                           new Stereocenters(m, graph, bondMap));
            var elements = recon.Recognise(new[] { Projection.Haworth });
            AssertTetrahedralCenter(elements[0],
                                    m.Atoms[1],
                                    TetrahedralStereo.AntiClockwise,
                                    m.Atoms[5], m.Atoms[0], m.Atoms[1], m.Atoms[2]);
            AssertTetrahedralCenter(elements[1],
                                    m.Atoms[2],
                                    TetrahedralStereo.AntiClockwise,
                                    m.Atoms[2], m.Atoms[1], m.Atoms[7], m.Atoms[3]);
            AssertTetrahedralCenter(elements[2],
                                    m.Atoms[3],
                                    TetrahedralStereo.AntiClockwise,
                                    m.Atoms[3], m.Atoms[2], m.Atoms[8], m.Atoms[4]);
            AssertTetrahedralCenter(elements[3],
                                    m.Atoms[4],
                                    TetrahedralStereo.AntiClockwise,
                                    m.Atoms[30], m.Atoms[3], m.Atoms[4], m.Atoms[0]);
        }

        /**
         * avoid false positive
         * @cdk.inchi InChI=1S/C6H12O6/c7-1-2-3(8)4(9)5(10)6(11)12-2/h2-11H,1H2
         */
        [TestMethod()]
        public void hexopyranose()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("O", 1, 0.00d, 2.48d));
            m.Atoms.Add(Atom("C", 2, 0.71d, 2.06d));
            m.Atoms.Add(Atom("C", 1, 0.71d, 1.24d));
            m.Atoms.Add(Atom("O", 0, 1.43d, 0.82d));
            m.Atoms.Add(Atom("C", 1, 1.43d, -0.00d));
            m.Atoms.Add(Atom("O", 1, 2.14d, -0.41d));
            m.Atoms.Add(Atom("C", 1, 0.71d, -0.41d));
            m.Atoms.Add(Atom("O", 1, 0.71d, -1.24d));
            m.Atoms.Add(Atom("C", 1, -0.00d, 0.00d));
            m.Atoms.Add(Atom("O", 1, -0.71d, -0.41d));
            m.Atoms.Add(Atom("C", 1, 0.00d, 0.83d));
            m.Atoms.Add(Atom("O", 1, -0.71d, 1.24d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[6], m.Atoms[7], BondOrder.Single);
            m.AddBond(m.Atoms[6], m.Atoms[8], BondOrder.Single);
            m.AddBond(m.Atoms[8], m.Atoms[9], BondOrder.Single);
            m.AddBond(m.Atoms[8], m.Atoms[10], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[10], BondOrder.Single);
            m.AddBond(m.Atoms[10], m.Atoms[11], BondOrder.Single);
            EdgeToBondMap bondMap = EdgeToBondMap.WithSpaceFor(m);
            int[][] graph = GraphUtil.ToAdjList(m, bondMap);

            CyclicCarbohydrateRecognition recon = new CyclicCarbohydrateRecognition(m, graph, bondMap,
                                                                                    new Stereocenters(m, graph, bondMap));
            Assert.IsTrue(recon.Recognise(new[] { Projection.Haworth }).Count == 0);
        }

        /**
         * Given a chair projection of beta-D-glucose we rotate it from -80 -> +80
         * and check the interpretation is the same. Going upside down inverts all
         * configurations.
         * 
         * @cdk.inchi InChI=1/C6H12O6/c7-1-2-3(8)4(9)5(10)6(11)12-2/h2-11H,1H2/t2-,3-,4+,5?,6-/s2
         */
        [TestMethod()]
        public void betaDGlucose_Chair_Rotated()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, -0.77d, 10.34d));
            m.Atoms.Add(Atom("C", 1, 0.03d, 10.13d));
            m.Atoms.Add(Atom("O", 0, 0.83d, 10.34d));
            m.Atoms.Add(Atom("C", 1, 1.24d, 9.63d));
            m.Atoms.Add(Atom("C", 1, 0.44d, 9.84d));
            m.Atoms.Add(Atom("C", 1, -0.35d, 9.63d));
            m.Atoms.Add(Atom("O", 1, 0.86d, 9.13d));
            m.Atoms.Add(Atom("O", 1, 2.04d, 9.84d));
            m.Atoms.Add(Atom("C", 2, -0.68d, 10.54d));
            m.Atoms.Add(Atom("O", 1, -0.68d, 11.37d));
            m.Atoms.Add(Atom("O", 1, -1.48d, 9.93d));
            m.Atoms.Add(Atom("O", 1, -1.15d, 9.84d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[5], m.Atoms[0], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[7], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[8], BondOrder.Single);
            m.AddBond(m.Atoms[8], m.Atoms[9], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[10], BondOrder.Single);
            m.AddBond(m.Atoms[5], m.Atoms[11], BondOrder.Single);

            Vector2 center = GeometryUtil.Get2DCenter(m);
            GeometryUtil.Rotate(m, center, Vectors.DegreeToRadian(-80));

            for (int i = 0; i < 30; i++)
            {
                GeometryUtil.Rotate(m, center, Vectors.DegreeToRadian(5));

                EdgeToBondMap bondMap = EdgeToBondMap.WithSpaceFor(m);
                int[][] graph = GraphUtil.ToAdjList(m, bondMap);
                CyclicCarbohydrateRecognition recon = new CyclicCarbohydrateRecognition(m, graph, bondMap,
                                                                                        new Stereocenters(m, graph, bondMap));

                var elements = recon.Recognise(new[] { Projection.Chair });
                m.SetStereoElements(elements);

                AssertTetrahedralCenter(elements[0],
                                        m.Atoms[1],
                                        TetrahedralStereo.Clockwise,
                                        m.Atoms[8], m.Atoms[0], m.Atoms[1], m.Atoms[2]);
                AssertTetrahedralCenter(elements[1],
                                        m.Atoms[3],
                                        TetrahedralStereo.Clockwise,
                                        m.Atoms[7], m.Atoms[2], m.Atoms[3], m.Atoms[4]);
                AssertTetrahedralCenter(elements[2],
                                        m.Atoms[4],
                                        TetrahedralStereo.Clockwise,
                                        m.Atoms[4], m.Atoms[3], m.Atoms[6], m.Atoms[5]);
                AssertTetrahedralCenter(elements[3],
                                        m.Atoms[5],
                                        TetrahedralStereo.Clockwise,
                                        m.Atoms[11], m.Atoms[4], m.Atoms[5], m.Atoms[0]);
                AssertTetrahedralCenter(elements[4],
                                        m.Atoms[0],
                                        TetrahedralStereo.Clockwise,
                                        m.Atoms[0], m.Atoms[5], m.Atoms[10], m.Atoms[1]);
            }
        }

        /**
         * p-menthane (CHEBI:25826)
         * @cdk.inchi InChI=1S/C10H20/c1-8(2)10-6-4-9(3)5-7-10/h8-10H,4-7H2,1-3H3
         */
        [TestMethod()]
        public void haworthFalsePositive()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 2, -0.71d, 0.41d));
            m.Atoms.Add(Atom("C", 2, 0.71d, -0.41d));
            m.Atoms.Add(Atom("C", 2, 0.71d, 0.41d));
            m.Atoms.Add(Atom("C", 2, -0.71d, -0.41d));
            m.Atoms.Add(Atom("C", 1, 0.00d, 0.82d));
            m.Atoms.Add(Atom("C", 3, 0.00d, 1.65d));
            m.Atoms.Add(Atom("C", 3, -0.71d, -2.06d));
            m.Atoms.Add(Atom("C", 1, -0.00d, -1.65d));
            m.Atoms.Add(Atom("C", 3, 0.71d, -2.06d));
            m.Atoms.Add(Atom("C", 1, -0.00d, -0.83d));
            m.AddBond(m.Atoms[9], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[9], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[0], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[9], m.Atoms[7], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[7], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[7], m.Atoms[8], BondOrder.Single);

            EdgeToBondMap bondMap = EdgeToBondMap.WithSpaceFor(m);
            int[][] graph = GraphUtil.ToAdjList(m, bondMap);
            CyclicCarbohydrateRecognition recon = new CyclicCarbohydrateRecognition(m, graph, bondMap,
                                                                                    new Stereocenters(m, graph, bondMap));

            var elements = recon.Recognise(new[] { Projection.Haworth });
            Assert.IsTrue(elements.Count == 0);
        }

        /**
         * prolinate (CHEBI:32871)
         * @cdk.cite InChI=1S/C5H9NO2/c7-5(8)4-2-1-3-6-4/h4,6H,1-3H2,(H,7,8)/p-1
         */
        [TestMethod()]
        public void requireAtLeastTwoProjectedSubstituents()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("O", 0, -0.71d, 1.24d));
            m.Atoms.Add(Atom("C", 0, 0.00d, 0.83d));
            m.Atoms.Add(Atom("O", 0, 0.71d, 1.24d));
            m.Atoms.Add(Atom("C", 1, 0.00d, 0.00d));
            m.Atoms.Add(Atom("C", 2, -0.67d, -0.48d));
            m.Atoms.Add(Atom("C", 2, -0.41d, -1.27d));
            m.Atoms.Add(Atom("C", 2, 0.41d, -1.27d));
            m.Atoms.Add(Atom("N", 1, 0.67d, -0.48d));
            m.AddBond(m.Atoms[6], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[0], BondOrder.Double, BondStereo.EZByCoordinates);
            m.AddBond(m.Atoms[2], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[5], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[7], BondOrder.Single);
            m.AddBond(m.Atoms[6], m.Atoms[7], BondOrder.Single);
            EdgeToBondMap bondMap = EdgeToBondMap.WithSpaceFor(m);
            int[][] graph = GraphUtil.ToAdjList(m, bondMap);
            CyclicCarbohydrateRecognition recon = new CyclicCarbohydrateRecognition(m, graph, bondMap,
                                                                                    new Stereocenters(m, graph, bondMap));

            var elements = recon.Recognise(new[] { Projection.Haworth });
            Assert.IsTrue(elements.Count == 0);
        }

        static void AssertTetrahedralCenter(IStereoElement element,
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
            IAtom a = new Atom(symbol);
            a.ImplicitHydrogenCount = h;
            a.Point2D = new Vector2(x, y);
            return a;
        }
    }
}
