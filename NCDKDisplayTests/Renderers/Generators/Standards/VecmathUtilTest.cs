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
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCDK.Common.Mathematics;
using NCDK.Numerics;
using System;
using WPF = System.Windows;

namespace NCDK.Renderers.Generators.Standards
{
    [TestClass()]
    public class VecmathUtilTest
    {
        [TestMethod()]
        public void TestToAwtPoint()
        {
            var p = VecmathUtil.ToPoint(new Vector2(4, 2));
            Assert.AreEqual(4, p.X, 0.01);
            Assert.AreEqual(2, p.Y, 0.01);
        }

        [TestMethod()]
        public void TestToVecmathPoint()
        {
            Vector2 p = VecmathUtil.ToVector(new WPF::Point(4, 2));
            Assert.AreEqual(4, p.X, 0.01);
            Assert.AreEqual(2, p.Y, 0.01);
        }

        [TestMethod()]
        public void TestNewUnitVector()
        {
            Vector2 unit = VecmathUtil.NewUnitVector(new Vector2(4, 2), new Vector2(6, 7));
            Assert.AreEqual(0.371, unit.X, 0.01);
            Assert.AreEqual(0.928, unit.Y, 0.01);
            Assert.AreEqual(1, unit.Length(), 0.01);
        }

        [TestMethod()]
        public void TestNewUnitVectorFromBond()
        {
            var mock_a1 = new Mock<IAtom>(); var a1 = mock_a1.Object;
            var mock_a2 = new Mock<IAtom>(); var a2 = mock_a2.Object;
            mock_a1.Setup(n => n.Point2D).Returns(new Vector2(0, 1));
            mock_a2.Setup(n => n.Point2D).Returns(new Vector2(1, 0));
            var mock_bond = new Mock<IBond>(); var bond = mock_bond.Object;
            mock_bond.Setup(n => n.GetOther(a1)).Returns(a2);
            mock_bond.Setup(n => n.GetOther(a2)).Returns(a1);
            Vector2 unit = VecmathUtil.NewUnitVector(a1, bond);
            Assert.AreEqual(0.707, unit.X, 0.01);
            Assert.AreEqual(-0.707, unit.Y, 0.01);
            Assert.AreEqual(1, unit.Length(), 0.01);
        }

        [TestMethod()]
        public void TestNewUnitVectors()
        {
            var mock_fromAtom = new Mock<IAtom>(); var fromAtom = mock_fromAtom.Object;
            var mock_toAtom1 = new Mock<IAtom>(); var toAtom1 = mock_toAtom1.Object;
            var mock_toAtom2 = new Mock<IAtom>(); var toAtom2 = mock_toAtom2.Object;
            var mock_toAtom3 = new Mock<IAtom>(); var toAtom3 = mock_toAtom3.Object;
            mock_fromAtom.Setup(n => n.Point2D).Returns(new Vector2(4, 2));
            mock_toAtom1.Setup(n => n.Point2D).Returns(new Vector2(-5, 3));
            mock_toAtom2.Setup(n => n.Point2D).Returns(new Vector2(6, -4));
            mock_toAtom3.Setup(n => n.Point2D).Returns(new Vector2(7, 5));
            var vectors = VecmathUtil.NewUnitVectors(fromAtom, new[] { toAtom1, toAtom2, toAtom3 });

            Assert.AreEqual(3, vectors.Count);
            Assert.AreEqual(-0.993, vectors[0].X, 0.01);
            Assert.AreEqual(0.110, vectors[0].Y, 0.01);
            Assert.AreEqual(0.316, vectors[1].X, 0.01);
            Assert.AreEqual(-0.948, vectors[1].Y, 0.01);
            Assert.AreEqual(0.707, vectors[2].X, 0.01);
            Assert.AreEqual(0.707, vectors[2].Y, 0.01);
        }

        [TestMethod()]
        public void TestNewPerpendicularVector()
        {
            Vector2 perpendicular = VecmathUtil.NewPerpendicularVector(new Vector2(5, 2));
            Assert.AreEqual(-2, perpendicular.X, 0.01);
            Assert.AreEqual(5, perpendicular.Y, 0.01);
        }

        [TestMethod()]
        public void TestScale()
        {
            Vector2 vector = VecmathUtil.Scale(new Vector2(4, 2), 2.5);
            Assert.AreEqual(10, vector.X, 0.01);
            Assert.AreEqual(5, vector.Y, 0.01);
        }

        [TestMethod()]
        public void TestSum()
        {
            Vector2 vector = VecmathUtil.Sum(new Vector2(4, 2), new Vector2(2, 5));
            Assert.AreEqual(6, vector.X, 0.01);
            Assert.AreEqual(7, vector.Y, 0.01);
        }

        [TestMethod()]
        public void TestNegate()
        {
            Vector2 vector = VecmathUtil.Negate(new Vector2(4, 2));
            Assert.AreEqual(-4, vector.X, 0.01);
            Assert.AreEqual(-2, vector.Y, 0.01);
        }

        [TestMethod()]
        public void TestAdjacentLength()
        {
            double length = VecmathUtil.AdjacentLength(new Vector2(2, 4), new Vector2(9, 4), 6d);
            Assert.AreEqual(4.94, length, 0.01);
        }

        [TestMethod()]
        public void TestAverage()
        {
            Vector2 mean = VecmathUtil.Average(new[] { new Vector2(0.5, 0.5), new Vector2(0.5, -0.5) });
            Assert.AreEqual(0.5, mean.X, 0.01);
            Assert.AreEqual(0, mean.Y, 0.01);
        }

        [TestMethod()]
        public void TestGetNearestVector1()
        {
            // not unit vectors, but okay for test
            Vector2 nearest = VecmathUtil.GetNearestVector(new Vector2(0, 1), new[] { new Vector2(0.5, 0.5), new Vector2(0.5, -0.5) });
            Assert.AreEqual(0.5, nearest.X, 0.01);
            Assert.AreEqual(0.5, nearest.Y, 0.01);
        }

        [TestMethod()]
        public void TestGetNearestVector2()
        {
            // not unit vectors, but okay for test
            Vector2 nearest = VecmathUtil.GetNearestVector(new Vector2(0, -1), new[] { new Vector2(0.5, 0.5), new Vector2(0.5, -0.5) });
            Assert.AreEqual(0.5, nearest.X, 0.01);
            Assert.AreEqual(-0.5, nearest.Y, 0.01);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestGetNearestVectorComplainsWhenNoVectorsProvided()
        {
            VecmathUtil.GetNearestVector(new Vector2(1, 0), Array.Empty<Vector2>());
        }

        [TestMethod()]
        public void TestGetNearestVectorFromBonds()
        {
            var mock_a1 = new Mock<IAtom>(); var a1 = mock_a1.Object;
            var mock_a2 = new Mock<IAtom>(); var a2 = mock_a2.Object;
            var mock_a3 = new Mock<IAtom>(); var a3 = mock_a3.Object;
            var mock_a4 = new Mock<IAtom>(); var a4 = mock_a4.Object;
            var mock_b1 = new Mock<IBond>(); var b1 = mock_b1.Object;
            var mock_b2 = new Mock<IBond>(); var b2 = mock_b2.Object;
            var mock_b3 = new Mock<IBond>(); var b3 = mock_b3.Object;
            mock_b1.Setup(n => n.GetOther(a1)).Returns(a2);
            mock_b2.Setup(n => n.GetOther(a1)).Returns(a3);
            mock_b3.Setup(n => n.GetOther(a1)).Returns(a4);
            mock_a1.Setup(n => n.Point2D).Returns(new Vector2(0, 0));
            mock_a2.Setup(n => n.Point2D).Returns(new Vector2(0, 1));
            mock_a3.Setup(n => n.Point2D).Returns(new Vector2(1, 0));
            mock_a4.Setup(n => n.Point2D).Returns(new Vector2(1, 1)); // this one is found

            Vector2 nearest = VecmathUtil.GetNearestVector(new Vector2(0.5, 0.5), a1, new[] { b1, b2, b3 });
            Assert.AreEqual(0.707, nearest.X, 0.01);
            Assert.AreEqual(0.707, nearest.Y, 0.01);
        }

        [TestMethod()]
        public void Intersection1()
        {
            var intersect = VecmathUtil.Intersection(new Vector2(1, 1), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 0));
            Assert.AreEqual(1, intersect.X, 0.01);
            Assert.AreEqual(0, intersect.Y, 0.01);
        }

        [TestMethod()]
        public void Intersection2()
        {
            var intersect = VecmathUtil.Intersection(new Vector2(6, 1), new Vector2(-4, -2), new Vector2(1, 6), new Vector2(2, 4));
            Console.Out.WriteLine(intersect);
            Assert.AreEqual(-4, intersect.X, 0.01);
            Assert.AreEqual(-4, intersect.Y, 0.01);
        }

        [TestMethod()]
        public void ParallelLines()
        {
            var intersect = VecmathUtil.Intersection(new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, -1), new Vector2(0, 1));
            Assert.IsTrue(double.IsNaN(intersect.X));
            Assert.IsTrue(double.IsNaN(intersect.Y));
        }

        [TestMethod()]
        public void sweepEast()
        {
            Assert.AreEqual(Vectors.DegreeToRadian(0), VecmathUtil.Extent(new Vector2(1, 0)), 0.01);
        }

        [TestMethod()]
        public void sweepNorth()
        {
            Assert.AreEqual(Vectors.DegreeToRadian(90), VecmathUtil.Extent(new Vector2(0, 1)), 0.01);
        }

        [TestMethod()]
        public void sweepWest()
        {
            Assert.AreEqual(Vectors.DegreeToRadian(180), VecmathUtil.Extent(new Vector2(-1, 0)), 0.01);
        }

        [TestMethod()]
        public void sweepSouth()
        {
            Assert.AreEqual(Vectors.DegreeToRadian(270), VecmathUtil.Extent(new Vector2(0, -1)), 0.01);
        }

        [TestMethod()]
        public void LargestGapSouthWest()
        {
            Vector2 vector = VecmathUtil.NewVectorInLargestGap(new[] { new Vector2(0, 1), new Vector2(1, 0) });
            Assert.AreEqual(-0.707, vector.X, 0.01);
            Assert.AreEqual(-0.707, vector.Y, 0.01);
            Assert.AreEqual(1, vector.Length(), 0.01);
        }

        [TestMethod()]
        public void LargestGapEast()
        {
            Vector2 vector = VecmathUtil.NewVectorInLargestGap(new[]
            {
            new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, -1), new Vector2(-1, 1),
            new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1) });
            Assert.AreEqual(1, vector.X, 0.01);
            Assert.AreEqual(0, vector.Y, 0.01);
            Assert.AreEqual(1, vector.Length(), 0.01);
        }
    }
}
