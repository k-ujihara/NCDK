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
using System.Windows.Media;
using static NCDK.Renderers.Generators.Standards.AtomSymbol.SymbolAlignment;
using WPF = System.Windows;

namespace NCDK.Renderers.Generators.Standards
{
    [TestClass()]
    public class AtomSymbolTest
    {
        private readonly Typeface font = new Typeface(new FontFamily("Verdana"), WPF::FontStyles.Normal, WPF::FontWeights.Normal, WPF::FontStretches.Normal);
        private readonly double emSize = 12;

        [TestMethod()]
        public void AlignToCenter()
        {
            TextOutline outline = new TextOutline("Cl", font, emSize);
            AtomSymbol symbol = new AtomSymbol(outline, Array.Empty<TextOutline>());
            AssertCloseTo(outline.GetCenter(), symbol.AlignTo(Center).GetAlignmentCenter(), 0.01);
        }

        [TestMethod()]
        public void AlignToLeft()
        {
            TextOutline outline = new TextOutline("Cl", font, emSize);
            AtomSymbol symbol = new AtomSymbol(outline, Array.Empty<TextOutline>());
            AssertCloseTo(outline.GetFirstGlyphCenter(), symbol.AlignTo(Left).GetAlignmentCenter(), 0.01);
        }

        [TestMethod()]
        public void AlignToRight()
        {
            TextOutline outline = new TextOutline("Cl", font, emSize);
            AtomSymbol symbol = new AtomSymbol(outline, Array.Empty<TextOutline>());
            AssertCloseTo(outline.GetLastGlyphCenter(), symbol.AlignTo(Right).GetAlignmentCenter(), 0.01);
        }

        [TestMethod()]
        public void TestGetOutlines()
        {
            TextOutline outline = new TextOutline("Cl", font, emSize);
            AtomSymbol symbol = new AtomSymbol(outline, Array.Empty<TextOutline>());
            var outlineBounds = outline.GetOutline().Bounds;
            var symbolBounds = symbol.GetOutlines()[0].Bounds;
            Assert.AreEqual(symbolBounds.X, outlineBounds.X, 0.01);
            Assert.AreEqual(symbolBounds.Y, outlineBounds.Y, 0.01);
            Assert.AreEqual(symbolBounds.Left, outlineBounds.Left, 0.01);
            Assert.AreEqual(symbolBounds.Bottom, outlineBounds.Bottom, 0.01);
        }

        [TestMethod()]
        public void TestGetOutlinesWithAdjunct()
        {
            TextOutline outline = new TextOutline("Cl", font, emSize);
            TextOutline adjunct = new TextOutline("H", font, emSize);
            AtomSymbol symbol = new AtomSymbol(outline, new[] { adjunct });
            var outlineBounds = adjunct.GetOutline().Bounds;
            var symbolBounds = symbol.GetOutlines()[1].Bounds;
            Assert.AreEqual(symbolBounds.X, outlineBounds.X, 0.01);
            Assert.AreEqual(symbolBounds.Y, outlineBounds.Y, 0.01);
            Assert.AreEqual(symbolBounds.Left, outlineBounds.Left, 0.01);
            Assert.AreEqual(symbolBounds.Bottom, outlineBounds.Bottom, 0.01);
        }

        [TestMethod()]
        public void TestGetConvexHull()
        {
            TextOutline outline = new TextOutline("Cl", font, emSize);
            AtomSymbol symbol = new AtomSymbol(outline, Array.Empty<TextOutline>());
            ConvexHull outlineHull = ConvexHull.OfShape(outline.GetOutline());
            ConvexHull symbolHull = symbol.GetConvexHull();

            var outlineBounds = outlineHull.Outline().Bounds;
            var symbolBounds = symbolHull.Outline().Bounds;

            Assert.AreEqual(symbolBounds.X, outlineBounds.X, 0.01);
            Assert.AreEqual(symbolBounds.Y, outlineBounds.Y, 0.01);
            Assert.AreEqual(symbolBounds.Left, outlineBounds.Left, 0.01);
            Assert.AreEqual(symbolBounds.Bottom, outlineBounds.Bottom, 0.01);
        }

        [TestMethod()]
        public void TestResize()
        {
            TextOutline outline = new TextOutline("Cl", font, emSize);
            AtomSymbol symbol = new AtomSymbol(outline, Array.Empty<TextOutline>());
            AtomSymbol transformed = symbol.Resize(2, 2);
            var orgBounds = outline.Bounds;
            var newBounds = transformed.GetOutlines()[0].Bounds;
            Assert.AreEqual(orgBounds.Left - orgBounds.Width / 2, newBounds.Left, 0.01);
            Assert.AreEqual(orgBounds.Top - orgBounds.Height / 2, newBounds.Top, 0.01);
            Assert.AreEqual(orgBounds.Right + orgBounds.Width / 2, newBounds.Right, 0.01);
            Assert.AreEqual(orgBounds.Bottom + orgBounds.Height / 2, newBounds.Bottom, 0.01);
        }

        [TestMethod()]
        public void TestCenter()
        {
            TextOutline outline = new TextOutline("Cl", font, emSize);
            AtomSymbol symbol = new AtomSymbol(outline, Array.Empty<TextOutline>());
            AtomSymbol transformed = symbol.Center(2, 2);
            var oBounds = outline.Bounds;
            var newBounds = transformed.GetOutlines()[0].Bounds;

            double dx = 2 - oBounds.GetCenterX();
            double dy = 2 - oBounds.GetCenterY();

            Assert.AreEqual(oBounds.Left + dx, newBounds.X, 0.01);
            Assert.AreEqual(oBounds.Top + dy, newBounds.Y, 0.01);
            Assert.AreEqual(oBounds.Right + dx, newBounds.Right, 0.01);
            Assert.AreEqual(oBounds.Bottom + dy, newBounds.Bottom, 0.01);
        }

        [TestMethod()]
        public void TestTranslate()
        {
            TextOutline outline = new TextOutline("Cl", font, emSize);
            AtomSymbol symbol = new AtomSymbol(outline, Array.Empty<TextOutline>());
            AtomSymbol transformed = symbol.Translate(4, 2);
            var orgBounds = symbol.GetOutlines()[0].Bounds;
            var newBounds = transformed.GetOutlines()[0].Bounds;
            Assert.AreEqual(orgBounds.X + 4, newBounds.X, 0.01);
            Assert.AreEqual(orgBounds.Y + 2, newBounds.Y, 0.01);
            Assert.AreEqual(orgBounds.Right + 4, newBounds.Right, 0.01);
            Assert.AreEqual(orgBounds.Bottom + 2, newBounds.Bottom, 0.01);
        }

        [TestMethod()]
        public void TestTranslateAdjunct()
        {
            TextOutline outline = new TextOutline("Cl", font, emSize);
            TextOutline adjunct = new TextOutline("H", font, emSize);
            AtomSymbol symbol = new AtomSymbol(outline, new[] { adjunct });
            AtomSymbol transformed = symbol.Translate(4, 2);
            var orgBounds = symbol.GetOutlines()[0].Bounds;
            var newBounds = transformed.GetOutlines()[0].Bounds;
            Assert.AreEqual(orgBounds.X + 4, newBounds.X, 0.01);
            Assert.AreEqual(orgBounds.Y + 2, newBounds.Y, 0.01);
            Assert.AreEqual(orgBounds.Right + 4, newBounds.Right, 0.01);
            Assert.AreEqual(orgBounds.Bottom + 2, newBounds.Bottom, 0.01);
        }

        void AssertCloseTo(WPF::Point actual, WPF::Point expected, double epsilon)
        {
            Assert.AreEqual(expected.X, actual.X, epsilon);
            Assert.AreEqual(expected.Y, actual.Y, epsilon);
        }
    }
}
