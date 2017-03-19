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

using NCDK.Common.Base;
using NCDK.Common.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using NCDK.Numerics;
using System.Reflection;

namespace NCDK.Hash.Stereo
{
    /// <summary>
    // @author John May
    // @cdk.module test-hash
    /// </summary>
    [TestClass()]
    public class GeometricTetrahedralEncoderFactoryTest
    {

        [TestMethod()]
        public void TestCreate_2D()
        {

            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;
            m_container.SetupGet(n => n.Atoms.Count).Returns(5);

            var m_c1 = new Mock<IAtom>(); var c1 = m_c1.Object;
            var m_o2 = new Mock<IAtom>(); var o2 = m_o2.Object;
            var m_n3 = new Mock<IAtom>(); var n3 = m_n3.Object;
            var m_c4 = new Mock<IAtom>(); var c4 = m_c4.Object;
            var m_h5 = new Mock<IAtom>(); var h5 = m_h5.Object;

            m_container.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_container.SetupGet(n => n.Atoms[1]).Returns(o2);
            m_container.SetupGet(n => n.Atoms[2]).Returns(n3);
            m_container.SetupGet(n => n.Atoms[3]).Returns(c4);
            m_container.SetupGet(n => n.Atoms[4]).Returns(h5);

            Vector2 p1 = new Vector2(1.23, -0.29);
            Vector2 p2 = new Vector2(-0.30, -0.29);
            Vector2 p3 = new Vector2(2.00, -1.63);
            Vector2 p4 = new Vector2(2.00, 1.03);
            Vector2 p5 = new Vector2(2.32, -0.29);

            m_c1.SetupGet(n => n.Point2D).Returns(p1);
            m_o2.SetupGet(n => n.Point2D).Returns(p2);
            m_n3.SetupGet(n => n.Point2D).Returns(p3);
            m_c4.SetupGet(n => n.Point2D).Returns(p4);
            m_h5.SetupGet(n => n.Point2D).Returns(p5);

            var m_c1c4 = new Mock<IBond>(); var c1c4 = m_c1c4.Object;
            var m_c1o2 = new Mock<IBond>(); var c1o2 = m_c1o2.Object;
            var m_c1n3 = new Mock<IBond>(); var c1n3 = m_c1n3.Object;
            var m_c1h5 = new Mock<IBond>(); var c1h5 = m_c1h5.Object;

            int[][] graph = new int[][] { new[] { 1, 2, 3, 4 }, new[] { 0 }, new[] { 0 }, new[] { 0 }, new[] { 0 } };

            m_container.Setup(n => n.GetConnectedBonds(c1)).Returns(new[] { c1c4, c1o2, c1n3, c1h5 });

            // let's say c1 is a chiral carbon
            m_c1.SetupGet(n => n.Hybridization).Returns(Hybridization.SP3);
            // with a hatch bond from c1 to n3
            m_c1n3.SetupGet(n => n.Stereo).Returns(BondStereo.Down);
            m_c1n3.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_c1n3.SetupGet(n => n.Atoms[1]).Returns(n3);
            m_c1o2.SetupGet(n => n.Stereo).Returns(BondStereo.None);
            m_c1o2.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_c1o2.SetupGet(n => n.Atoms[1]).Returns(o2);
            m_c1c4.SetupGet(n => n.Stereo).Returns(BondStereo.None);
            m_c1c4.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_c1c4.SetupGet(n => n.Atoms[1]).Returns(c4);
            m_c1h5.SetupGet(n => n.Stereo).Returns(BondStereo.None);
            m_c1h5.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_c1h5.SetupGet(n => n.Atoms[1]).Returns(h5);

            IStereoEncoder encoder = new GeometricTetrahedralEncoderFactory().Create(container, graph);

            Assert.AreEqual(1, ExtractEncoders(encoder).Count);

            GeometricParity geometricParity = GetGeometricParity(ExtractEncoders(encoder)[0]);

            Assert.IsTrue(geometricParity is Tetrahedral2DParity);

            Assert.IsTrue(Compares.AreDeepEqual(new Vector2[] { p2, p3, p4, p5 }, Coords2D(geometricParity)));
        }

        [TestMethod()]
        public void TestCreate_2D_Implicit()
        {

            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;
            m_container.SetupGet(n => n.Atoms.Count).Returns(4);

            var m_c1 = new Mock<IAtom>(); var c1 = m_c1.Object;
            var m_o2 = new Mock<IAtom>(); var o2 = m_o2.Object;
            var m_n3 = new Mock<IAtom>(); var n3 = m_n3.Object;
            var m_c4 = new Mock<IAtom>(); var c4 = m_c4.Object;

            m_container.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_container.SetupGet(n => n.Atoms[1]).Returns(o2);
            m_container.SetupGet(n => n.Atoms[2]).Returns(n3);
            m_container.SetupGet(n => n.Atoms[3]).Returns(c4);

            Vector2 p1 = new Vector2(1.23, -0.29);
            Vector2 p2 = new Vector2(-0.30, -0.29);
            Vector2 p3 = new Vector2(2.00, -1.63);
            Vector2 p4 = new Vector2(2.00, 1.03);

            m_c1.SetupGet(n => n.Point2D).Returns(p1);
            m_o2.SetupGet(n => n.Point2D).Returns(p2);
            m_n3.SetupGet(n => n.Point2D).Returns(p3);
            m_c4.SetupGet(n => n.Point2D).Returns(p4);

            var m_c1c4 = new Mock<IBond>(); var c1c4 = m_c1c4.Object;
            var m_c1o2 = new Mock<IBond>(); var c1o2 = m_c1o2.Object;
            var m_c1n3 = new Mock<IBond>(); var c1n3 = m_c1n3.Object;
            var m_c1h5 = new Mock<IBond>(); var c1h5 = m_c1h5.Object;

            int[][] graph = new int[][] { new[] { 1, 2, 3 }, new[] { 0 }, new[] { 0 }, new[] { 0 }, };

            m_container.Setup(n => n.GetConnectedBonds(c1)).Returns(new[] { c1c4, c1o2, c1n3 });

            // let's say c1 is a chiral carbon
            m_c1.SetupGet(n => n.Hybridization).Returns(Hybridization.SP3);
            // with a hatch bond from c1 to n3
            m_c1n3.SetupGet(n => n.Stereo).Returns(BondStereo.Down);
            m_c1n3.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_c1n3.SetupGet(n => n.Atoms[1]).Returns(n3);
            m_c1o2.SetupGet(n => n.Stereo).Returns(BondStereo.None);
            m_c1o2.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_c1o2.SetupGet(n => n.Atoms[1]).Returns(o2);
            m_c1c4.SetupGet(n => n.Stereo).Returns(BondStereo.None);
            m_c1c4.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_c1c4.SetupGet(n => n.Atoms[1]).Returns(c4);

            IStereoEncoder encoder = new GeometricTetrahedralEncoderFactory().Create(container, graph);

            Assert.AreEqual(1, ExtractEncoders(encoder).Count);

            GeometricParity geometricParity = GetGeometricParity(ExtractEncoders(encoder)[0]);

            Assert.IsTrue(geometricParity is Tetrahedral2DParity);

            Assert.IsTrue(Compares.AreDeepEqual(
                new Vector2[]{ p2, p3, p4, p1 // p1 is from central atom
                }, Coords2D(geometricParity)));
        }

        [TestMethod()]
        public void TestCreate_3D()
        {

            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;
            m_container.SetupGet(n => n.Atoms.Count).Returns(5);

            var m_c1 = new Mock<IAtom>(); var c1 = m_c1.Object;
            var m_o2 = new Mock<IAtom>(); var o2 = m_o2.Object;
            var m_n3 = new Mock<IAtom>(); var n3 = m_n3.Object;
            var m_c4 = new Mock<IAtom>(); var c4 = m_c4.Object;
            var m_h5 = new Mock<IAtom>(); var h5 = m_h5.Object;

            m_container.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_container.SetupGet(n => n.Atoms[1]).Returns(o2);
            m_container.SetupGet(n => n.Atoms[2]).Returns(n3);
            m_container.SetupGet(n => n.Atoms[3]).Returns(c4);
            m_container.SetupGet(n => n.Atoms[4]).Returns(h5);

            Vector3 p1 = new Vector3(1.23, -0.29, 0);
            Vector3 p2 = new Vector3(-0.30, -0.29, 0);
            Vector3 p3 = new Vector3(2.00, -1.63, 0);
            Vector3 p4 = new Vector3(2.00, 1.03, 0);
            Vector3 p5 = new Vector3(2.32, -0.29, 0);

            m_c1.SetupGet(n => n.Point3D).Returns(p1);
            m_o2.SetupGet(n => n.Point3D).Returns(p2);
            m_n3.SetupGet(n => n.Point3D).Returns(p3);
            m_c4.SetupGet(n => n.Point3D).Returns(p4);
            m_h5.SetupGet(n => n.Point3D).Returns(p5);

            var m_c1c4 = new Mock<IBond>(); var c1c4 = m_c1c4.Object;
            var m_c1o2 = new Mock<IBond>(); var c1o2 = m_c1o2.Object;
            var m_c1n3 = new Mock<IBond>(); var c1n3 = m_c1n3.Object;
            var m_c1h5 = new Mock<IBond>(); var c1h5 = m_c1h5.Object;

            int[][] graph = new int[][] { new[] { 1, 2, 3, 4 }, new[] { 0 }, new[] { 0 }, new[] { 0 }, new[] { 0 } };

            m_container.Setup(n => n.GetConnectedBonds(c1)).Returns(new[] { c1c4, c1o2, c1n3, c1h5 });

            // let's say c1 is a chiral carbon
            m_c1.SetupGet(n => n.Hybridization).Returns(Hybridization.SP3);
            // with a hatch bond from c1 to n3
            m_c1n3.SetupGet(n => n.Stereo).Returns(BondStereo.None);
            m_c1n3.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_c1n3.SetupGet(n => n.Atoms[1]).Returns(n3);
            m_c1o2.SetupGet(n => n.Stereo).Returns(BondStereo.None);
            m_c1o2.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_c1o2.SetupGet(n => n.Atoms[1]).Returns(o2);
            m_c1c4.SetupGet(n => n.Stereo).Returns(BondStereo.None);
            m_c1c4.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_c1c4.SetupGet(n => n.Atoms[1]).Returns(c4);
            m_c1h5.SetupGet(n => n.Stereo).Returns(BondStereo.None);
            m_c1h5.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_c1h5.SetupGet(n => n.Atoms[1]).Returns(h5);

            IStereoEncoder encoder = new GeometricTetrahedralEncoderFactory().Create(container, graph);

            Assert.AreEqual(1, ExtractEncoders(encoder).Count);

            GeometricParity geometricParity = GetGeometricParity(ExtractEncoders(encoder)[0]);

            Assert.IsTrue(geometricParity is Tetrahedral3DParity);

            Assert.IsTrue(Compares.AreDeepEqual(
                new Vector3[] { p2, p3, p4, p5 },
                Coords3D(geometricParity)));
        }

        [TestMethod()]
        public void TestCreate_3D_Implicit()
        {

            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;
            m_container.SetupGet(n => n.Atoms.Count).Returns(4);

            var m_c1 = new Mock<IAtom>(); var c1 = m_c1.Object;
            var m_o2 = new Mock<IAtom>(); var o2 = m_o2.Object;
            var m_n3 = new Mock<IAtom>(); var n3 = m_n3.Object;
            var m_c4 = new Mock<IAtom>(); var c4 = m_c4.Object;

            m_container.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_container.SetupGet(n => n.Atoms[1]).Returns(o2);
            m_container.SetupGet(n => n.Atoms[2]).Returns(n3);
            m_container.SetupGet(n => n.Atoms[3]).Returns(c4);

            Vector3 p1 = new Vector3(1.23, -0.29, 0);
            Vector3 p2 = new Vector3(-0.30, -0.29, 0);
            Vector3 p3 = new Vector3(2.00, -1.63, 0);
            Vector3 p4 = new Vector3(2.00, 1.03, 0);

            m_c1.SetupGet(n => n.Point3D).Returns(p1);
            m_o2.SetupGet(n => n.Point3D).Returns(p2);
            m_n3.SetupGet(n => n.Point3D).Returns(p3);
            m_c4.SetupGet(n => n.Point3D).Returns(p4);

            var m_c1c4 = new Mock<IBond>(); var c1c4 = m_c1c4.Object;
            var m_c1o2 = new Mock<IBond>(); var c1o2 = m_c1o2.Object;
            var m_c1n3 = new Mock<IBond>(); var c1n3 = m_c1n3.Object;

            int[][] graph = new int[][] { new[] { 1, 2, 3 }, new[] { 0 }, new[] { 0 }, new[] { 0 } };

            m_container.Setup(n => n.GetConnectedBonds(c1)).Returns(new[] { c1c4, c1o2, c1n3 });

            // let's say c1 is a chiral carbon
            m_c1.SetupGet(n => n.Hybridization).Returns(Hybridization.SP3);
            // with a hatch bond from c1 to n3
            m_c1n3.SetupGet(n => n.Stereo).Returns(BondStereo.None);
            m_c1n3.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_c1n3.SetupGet(n => n.Atoms[1]).Returns(n3);
            m_c1o2.SetupGet(n => n.Stereo).Returns(BondStereo.None);
            m_c1o2.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_c1o2.SetupGet(n => n.Atoms[1]).Returns(o2);
            m_c1c4.SetupGet(n => n.Stereo).Returns(BondStereo.None);
            m_c1c4.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_c1c4.SetupGet(n => n.Atoms[1]).Returns(c4);

            IStereoEncoder encoder = new GeometricTetrahedralEncoderFactory().Create(container, graph);

            Assert.AreEqual(1, ExtractEncoders(encoder).Count);

            GeometricParity geometricParity = GetGeometricParity(ExtractEncoders(encoder)[0]);

            Assert.IsTrue(geometricParity is Tetrahedral3DParity);

            Assert.IsTrue(Compares.AreDeepEqual(
                new Vector3[]{p2, p3, p4, p1 // p1 = central atom
                }, Coords3D(geometricParity)));
        }

        [TestMethod()]
        public void TestCreate_NonSP3()
        {

            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;
            m_container.SetupGet(n => n.Atoms.Count).Returns(5);

            var m_c1 = new Mock<IAtom>(); var c1 = m_c1.Object;
            var m_o2 = new Mock<IAtom>(); var o2 = m_o2.Object;
            var m_n3 = new Mock<IAtom>(); var n3 = m_n3.Object;
            var m_c4 = new Mock<IAtom>(); var c4 = m_c4.Object;
            var m_h5 = new Mock<IAtom>(); var h5 = m_h5.Object;

            m_container.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_container.SetupGet(n => n.Atoms[1]).Returns(o2);
            m_container.SetupGet(n => n.Atoms[2]).Returns(n3);
            m_container.SetupGet(n => n.Atoms[3]).Returns(c4);
            m_container.SetupGet(n => n.Atoms[4]).Returns(h5);

            m_c1.SetupGet(n => n.Point2D).Returns(new Vector2(1.23, -0.29));
            m_o2.SetupGet(n => n.Point2D).Returns(new Vector2(-0.30, -0.29));
            m_n3.SetupGet(n => n.Point2D).Returns(new Vector2(2.00, -1.63));
            m_c4.SetupGet(n => n.Point2D).Returns(new Vector2(2.00, 1.03));
            m_h5.SetupGet(n => n.Point2D).Returns(new Vector2(2.32, -0.29));

            var m_c1c4 = new Mock<IBond>(); var c1c4 = m_c1c4.Object;
            var m_c1o2 = new Mock<IBond>(); var c1o2 = m_c1o2.Object;
            var m_c1n3 = new Mock<IBond>(); var c1n3 = m_c1n3.Object;
            var m_c1h5 = new Mock<IBond>(); var c1h5 = m_c1h5.Object;

            int[][] graph = new int[][] { new[] { 1, 2, 3, 4 }, new[] { 0 }, new[] { 0 }, new[] { 0 }, new[] { 0 } };

            m_container.Setup(n => n.GetConnectedBonds(c1)).Returns(new[] { c1c4, c1o2, c1n3, c1h5 });

            // ATOM is not SP3
            // m_c1.SetupGet(n => n.Hybridization).Returns(Hybridization.SP3);
            // with a hatch bond from c1 to n3
            m_c1n3.SetupGet(n => n.Stereo).Returns(BondStereo.Down);
            m_c1n3.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_c1n3.SetupGet(n => n.Atoms[1]).Returns(n3);
            m_c1o2.SetupGet(n => n.Stereo).Returns(BondStereo.None);
            m_c1o2.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_c1o2.SetupGet(n => n.Atoms[1]).Returns(o2);
            m_c1c4.SetupGet(n => n.Stereo).Returns(BondStereo.None);
            m_c1c4.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_c1c4.SetupGet(n => n.Atoms[1]).Returns(c4);
            m_c1h5.SetupGet(n => n.Stereo).Returns(BondStereo.None);
            m_c1h5.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_c1h5.SetupGet(n => n.Atoms[1]).Returns(h5);

            IStereoEncoder encoder = new GeometricTetrahedralEncoderFactory().Create(container, graph);

            Assert.AreEqual(StereoEncoder.Empty, encoder);

        }

        [TestMethod()]
        public void TestCreate_NoStereoBonds()
        {

            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;
            m_container.SetupGet(n => n.Atoms.Count).Returns(5);

            var m_c1 = new Mock<IAtom>(); var c1 = m_c1.Object;
            var m_o2 = new Mock<IAtom>(); var o2 = m_o2.Object;
            var m_n3 = new Mock<IAtom>(); var n3 = m_n3.Object;
            var m_c4 = new Mock<IAtom>(); var c4 = m_c4.Object;
            var m_h5 = new Mock<IAtom>(); var h5 = m_h5.Object;

            m_container.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_container.SetupGet(n => n.Atoms[1]).Returns(o2);
            m_container.SetupGet(n => n.Atoms[2]).Returns(n3);
            m_container.SetupGet(n => n.Atoms[3]).Returns(c4);
            m_container.SetupGet(n => n.Atoms[4]).Returns(h5);

            m_c1.SetupGet(n => n.Point2D).Returns(new Vector2(1.23, -0.29));
            m_o2.SetupGet(n => n.Point2D).Returns(new Vector2(-0.30, -0.29));
            m_n3.SetupGet(n => n.Point2D).Returns(new Vector2(2.00, -1.63));
            m_c4.SetupGet(n => n.Point2D).Returns(new Vector2(2.00, 1.03));
            m_h5.SetupGet(n => n.Point2D).Returns(new Vector2(2.32, -0.29));

            var m_c1c4 = new Mock<IBond>(); var c1c4 = m_c1c4.Object;
            var m_c1o2 = new Mock<IBond>(); var c1o2 = m_c1o2.Object;
            var m_c1n3 = new Mock<IBond>(); var c1n3 = m_c1n3.Object;
            var m_c1h5 = new Mock<IBond>(); var c1h5 = m_c1h5.Object;

            int[][] graph = new int[][] { new[] { 1, 2, 3, 4 }, new[] { 0 }, new[] { 0 }, new[] { 0 }, new[] { 0 } };

            m_container.Setup(n => n.GetConnectedBonds(c1)).Returns(new[] { c1c4, c1o2, c1n3, c1h5 });

            // ATOM is not SP3
            m_c1.SetupGet(n => n.Hybridization).Returns(Hybridization.SP3);
            // with a hatch bond from c1 to n3
            //m_c1n3.SetupGet(n => n.Stereo).Returns(BondStereo.Down);
            m_c1n3.SetupGet(n => n.Stereo).Returns(BondStereo.None);
            m_c1n3.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_c1n3.SetupGet(n => n.Atoms[1]).Returns(n3);
            m_c1o2.SetupGet(n => n.Stereo).Returns(BondStereo.None);
            m_c1o2.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_c1o2.SetupGet(n => n.Atoms[1]).Returns(o2);
            m_c1c4.SetupGet(n => n.Stereo).Returns(BondStereo.None);
            m_c1c4.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_c1c4.SetupGet(n => n.Atoms[1]).Returns(c4);
            m_c1h5.SetupGet(n => n.Stereo).Returns(BondStereo.None);
            m_c1h5.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_c1h5.SetupGet(n => n.Atoms[1]).Returns(h5);

            IStereoEncoder encoder = new GeometricTetrahedralEncoderFactory().Create(container, graph);

            Assert.AreEqual(StereoEncoder.Empty, encoder);

        }

        [TestMethod()]
        public void TestCreate_WrongDegree()
        {

            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;
            m_container.SetupGet(n => n.Atoms.Count).Returns(5);

            var m_c1 = new Mock<IAtom>(); var c1 = m_c1.Object;
            var m_o2 = new Mock<IAtom>(); var o2 = m_o2.Object;
            var m_n3 = new Mock<IAtom>(); var n3 = m_n3.Object;
            var m_c4 = new Mock<IAtom>(); var c4 = m_c4.Object;
            var m_h5 = new Mock<IAtom>(); var h5 = m_h5.Object;

            m_container.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_container.SetupGet(n => n.Atoms[1]).Returns(o2);
            m_container.SetupGet(n => n.Atoms[2]).Returns(n3);
            m_container.SetupGet(n => n.Atoms[3]).Returns(c4);
            m_container.SetupGet(n => n.Atoms[4]).Returns(h5);

            m_c1.SetupGet(n => n.Point2D).Returns(new Vector2(1.23, -0.29));
            m_o2.SetupGet(n => n.Point2D).Returns(new Vector2(-0.30, -0.29));
            m_n3.SetupGet(n => n.Point2D).Returns(new Vector2(2.00, -1.63));
            m_c4.SetupGet(n => n.Point2D).Returns(new Vector2(2.00, 1.03));
            m_h5.SetupGet(n => n.Point2D).Returns(new Vector2(2.32, -0.29));

            var m_c1c4 = new Mock<IBond>(); var c1c4 = m_c1c4.Object;
            var m_c1o2 = new Mock<IBond>(); var c1o2 = m_c1o2.Object;
            var m_c1n3 = new Mock<IBond>(); var c1n3 = m_c1n3.Object;
            var m_c1h5 = new Mock<IBond>(); var c1h5 = m_c1h5.Object;

            int[][] graph = new int[][]{ new[] {1, 2}, // 3, 4}, ignore these
                new[] {0}, new[] {0}, new[] {0}, new[] {0}};

            m_container.Setup(n => n.GetConnectedBonds(c1)).Returns(new[] { c1c4, c1o2, c1n3, c1h5 });

            // ATOM is not SP3
            m_c1.SetupGet(n => n.Hybridization).Returns(Hybridization.SP3);
            // with a hatch bond from c1 to n3
            m_c1n3.SetupGet(n => n.Stereo).Returns(BondStereo.Down);
            m_c1n3.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_c1n3.SetupGet(n => n.Atoms[1]).Returns(n3);
            m_c1o2.SetupGet(n => n.Stereo).Returns(BondStereo.None);
            m_c1o2.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_c1o2.SetupGet(n => n.Atoms[1]).Returns(o2);
            m_c1c4.SetupGet(n => n.Stereo).Returns(BondStereo.None);
            m_c1c4.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_c1c4.SetupGet(n => n.Atoms[1]).Returns(c4);
            m_c1h5.SetupGet(n => n.Stereo).Returns(BondStereo.None);
            m_c1h5.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_c1h5.SetupGet(n => n.Atoms[1]).Returns(h5);

            IStereoEncoder encoder = new GeometricTetrahedralEncoderFactory().Create(container, graph);

            Assert.AreEqual(StereoEncoder.Empty, encoder);

        }

        private static Vector2[] Coords2D(GeometricParity parity)
        {
            if (parity is Tetrahedral2DParity)
            {
                FieldInfo field = null;
                field = parity.GetType().GetField("coordinates", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (field == null)
                {
                    Console.Error.WriteLine("Error on accessing coordinates field.");
                    return null;
                }
                return (Vector2[])field.GetValue(parity);
            }
            return null;
        }

        private static Vector3[] Coords3D(GeometricParity parity)
        {
            if (parity is Tetrahedral3DParity)
            {
                FieldInfo field = null;
                field = parity.GetType().GetField("coordinates", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (field == null)
                {
                    Console.Error.WriteLine("Error on accessing coordinates field.");
                    return null;
                }
                return (Vector3[])field.GetValue(parity);
            }
            return null;
        }

        private static GeometricParity GetGeometricParity(IStereoEncoder encoder)
        {
            if (encoder is GeometryEncoder)
            {
                FieldInfo field = null;
                field = encoder.GetType().GetField("geometric", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (field == null)
                {
                    Console.Error.WriteLine("Error on accessing geometric field.");
                    return null;
                }
                return (GeometricParity)field.GetValue(encoder);
            }
            return null;
        }

        private static IList<IStereoEncoder> ExtractEncoders(IStereoEncoder encoder)
        {
            if (encoder is MultiStereoEncoder)
            {
                FieldInfo field = null;
                field = encoder.GetType().GetField("encoders", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (field == null)
                {
                    Console.Error.WriteLine("Error on accessing encoders field.");
                    return null;
                }
                return (IList<IStereoEncoder>)field.GetValue(encoder);
            }
            return new IStereoEncoder[0];
        }
    }
}
