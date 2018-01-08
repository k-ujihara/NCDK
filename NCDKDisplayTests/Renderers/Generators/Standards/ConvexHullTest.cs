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
using System;
using System.Windows;
using System.Windows.Media;

namespace NCDK.Renderers.Generators.Standards
{
    [TestClass()]
    public class ConvexHullTest
    {
        [TestMethod()]
        public void OfOvalShape()
        {
            var oval = new RectangleGeometry(new Rect(-5, -5, 10, 10), 5, 5);
            ConvexHull hull = ConvexHull.OfShape(oval);
            Rect bounds = hull.Outline.Bounds;
            Assert.AreEqual(-5, bounds.Left, 0.01);
            Assert.AreEqual(-5, bounds.Top, 0.01);
            Assert.AreEqual(5, bounds.Right, 0.01);
            Assert.AreEqual(5, bounds.Bottom, 0.01);
        }

        [TestMethod()]
        public void OfTriangle()
        {
            var path = new PathGeometry(
                new[] {
                    new PathFigure(
                    new Point(-5, 0),
                    new[]
                    {
                        new LineSegment(new Point(0, 10), true),
                        new LineSegment(new Point(5, 0), true),
                    }, true)
                });
            ConvexHull hull = ConvexHull.OfShape(path);
            var bounds = hull.Outline.Bounds;
            Assert.AreEqual(-5, bounds.Left, 0.01);
            Assert.AreEqual(0, bounds.Top, 0.01);
            Assert.AreEqual(5, bounds.Right, 0.01);
            Assert.AreEqual(10, bounds.Bottom, 0.01);
        }

        [TestMethod()]
        public void OfRectangles()
        {
            var rect1 = new RectangleGeometry(new Rect(-10, -10, 5, 5));
            var rect2 = new RectangleGeometry(new Rect(15, 16, 20, 25));
            var rect3 = new RectangleGeometry(new Rect(-15, 6, 2, 5));
            ConvexHull hull = ConvexHull.OfShapes(new[] { rect1, rect2, rect3 });
            var bounds = hull.Outline.Bounds;
            Assert.AreEqual(-15, bounds.Left, 0.01);
            Assert.AreEqual(-10, bounds.Top, 0.01);
            Assert.AreEqual(35, bounds.Right, 0.01);
            Assert.AreEqual(41, bounds.Bottom, 0.01);
        }

        [TestMethod()]
        public void TransformDoesNotModifyOriginal()
        {
            var rect1 = new RectangleGeometry(new Rect(-10, -10, 5, 5));
            var rect2 = new RectangleGeometry(new Rect(15, 16, 20, 25));
            var rect3 = new RectangleGeometry(new Rect(-15, 6, 2, 5));
            ConvexHull hull = ConvexHull.OfShapes(new[] { rect1, rect2, rect3 });

            ConvexHull transformedHull = hull.Transform(new TranslateTransform(10, 15));

            var bounds = hull.Outline.Bounds;
            var transformedBounds = transformedHull.Outline.Bounds;

            Assert.AreNotEqual(transformedBounds.Left, bounds.Left, 0.01);
            Assert.AreNotEqual(transformedBounds.Top, bounds.Top, 0.01);
            Assert.AreNotEqual(transformedBounds.Right, bounds.Right, 0.01);
            Assert.AreNotEqual(transformedBounds.Bottom, bounds.Bottom, 0.01);
        }

        [TestMethod()]
        public void TestShapeOf()
        {
            var points = new[]
            {
                new Point(-5, -5),
                new Point(-5, 5),
                new Point(5, 5),
                new Point(5, -5),
            };
            var bounds = ConvexHull.ShapeOf(points).Bounds;
            Assert.AreEqual(-5, bounds.Left, 0.01);
            Assert.AreEqual(-5, bounds.Top, 0.01);
            Assert.AreEqual(5, bounds.Right, 0.01);
            Assert.AreEqual(5, bounds.Bottom, 0.01);
        }

        [TestMethod()]
        public void EmptyShapeDoesBreak()
        {
            var shape = ConvexHull.ShapeOf(Array.Empty<Point>());
        }

        [TestMethod()]
        public void TestPointsOf()
        {
            var rect = new RectangleGeometry(new Rect(-5, -5, 10, 10));
            var points = ConvexHull.PointsOf(rect);
            Assert.AreEqual(4, points.Count);
            Assert.AreEqual(-5, points[0].X, 0.01);
            Assert.AreEqual(-5, points[0].Y, 0.01);
            Assert.AreEqual(5, points[1].X, 0.01);
            Assert.AreEqual(-5, points[1].Y, 0.01);
            Assert.AreEqual(5, points[2].X, 0.01);
            Assert.AreEqual(5, points[2].Y, 0.01);
            Assert.AreEqual(-5, points[3].X, 0.01);
            Assert.AreEqual(5, points[3].Y, 0.01);
        }

        [TestMethod()]
        public void IntersectionOfRect()
        {
            var rect = new RectangleGeometry(new Rect(-5, -5, 10, 10));
            ConvexHull hull = ConvexHull.OfShape(rect);
            var intersect = hull.Intersect(new Point(0, 0), new Point(10, 0));
            Assert.AreEqual(5, intersect.X, 0.01);
            Assert.AreEqual(0, intersect.Y, 0.01);
        }
    }
}
