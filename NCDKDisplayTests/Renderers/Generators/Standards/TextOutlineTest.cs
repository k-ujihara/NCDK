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
using System.Windows.Media;
using WPF = System.Windows;

namespace NCDK.Renderers.Generators.Standards
{
    [TestClass()]
    public class TextOutlineTest
    {
        private readonly Typeface font = new Typeface(new FontFamily("Verdana"), WPF::FontStyles.Normal, WPF::FontWeights.Normal, WPF::FontStretches.Normal);
        private readonly double emSize = 12;

        [TestMethod()]
        public void GetOutline()
        {
            // not sure how to test... we have a complex shape with floating point
            // values?
        }

        [TestMethod(), Ignore()] // Font bounds vary between systems
        public void UntransformedBounds()
        {
            TextOutline clOutline = new TextOutline("Cl", font, emSize);
            var bounds = clOutline.GetBounds();
            Assert.AreEqual(0.67, bounds.X, 0.01);
            Assert.AreEqual(-9.12, bounds.Y, 0.01);
            Assert.AreEqual(9.90, bounds.Width, 0.01);
            Assert.AreEqual(9.28, bounds.Height, 0.01);
        }

        [TestMethod()]
        public void boundsTransformedWithXTranslation()
        {
            TextOutline original = new TextOutline("Cl", font, emSize);
            TextOutline transformed = original.Translate(5, 0);
            var oBounds = original.GetBounds();
            var tBounds = transformed.GetBounds();
            Assert.AreEqual(oBounds.X + 5, tBounds.X, 0.01);
            Assert.AreEqual(oBounds.Y, tBounds.Y, 0.01);
            Assert.AreEqual(oBounds.Width, tBounds.Width, 0.01);
            Assert.AreEqual(oBounds.Height, tBounds.Height, 0.01);
        }

        [TestMethod()]
        public void boundsTransformedWithYTranslation()
        {
            TextOutline original = new TextOutline("Cl", font, emSize);
            TextOutline transformed = original.Translate(0, -5);
            var oBounds = original.GetBounds();
            var tBounds = transformed.GetBounds();
            Assert.AreEqual(oBounds.X, tBounds.X, 0.01);
            Assert.AreEqual(oBounds.Y - 5, tBounds.Y, 0.01);
            Assert.AreEqual(oBounds.Width, tBounds.Width, 0.01);
            Assert.AreEqual(oBounds.Height, tBounds.Height, 0.01);
        }

        [TestMethod(), Ignore()]  // Font bounds vary between systems
        public void UntransformedCenter()
        {
            TextOutline clOutline = new TextOutline("Cl", font, emSize);
            var center = clOutline.GetCenter();
            Assert.AreEqual(5.62, center.X, 0.01);
            Assert.AreEqual(-4.47, center.Y, 0.01);
        }

        [TestMethod()]
        public void TransformedCenter()
        {
            TextOutline original = new TextOutline("Cl", font, emSize);
            TextOutline transformed = original.Translate(0, -5);
            var oCenter = original.GetCenter();
            var tCenter = transformed.GetCenter();
            Assert.AreEqual(oCenter.X, tCenter.X, 0.01);
            Assert.AreEqual(oCenter.Y - 5, tCenter.Y, 0.01);
        }

        [TestMethod()]
        public void TestGetFirstGlyphCenter()
        {
            TextOutline original = new TextOutline("Cl", font, emSize);
            var oCenter = original.GetCenter();
            var tCenter = original.GetFirstGlyphCenter();
            Assert.IsTrue(tCenter.X < oCenter.X);
        }

        [TestMethod()]
        public void TestGetLastGlyphCenter()
        {
            TextOutline original = new TextOutline("Cl", font, emSize);
            var oCenter = original.GetCenter();
            var tCenter = original.GetLastGlyphCenter();
            Assert.IsTrue(tCenter.X > oCenter.X);
        }

        [TestMethod()]
        public void ResizeModifiesBounds()
        {
            TextOutline original = new TextOutline("Cl", font, emSize);
            TextOutline transformed = original.Resize(2, 2);
            var oBounds = original.GetBounds();
            var tBounds = transformed.GetBounds();
            Assert.AreEqual(oBounds.X - oBounds.Width / 2, tBounds.X, 0.01);
            Assert.AreEqual(oBounds.Y - oBounds.Height / 2, tBounds.Y, 0.01);
            Assert.AreEqual(oBounds.Width * 2, tBounds.Width, 0.01);
            Assert.AreEqual(oBounds.Height * 2, tBounds.Height, 0.01);
        }

        [TestMethod()]
        public void ResizeMaintainsCenter()
        {
            TextOutline clOutline = new TextOutline("Cl", font, emSize);
            var orgCenter = clOutline.GetCenter();
            var newCenter = clOutline.Resize(21, 5).GetCenter();
            Assert.AreEqual(newCenter.X, orgCenter.X, 0.01);
            Assert.AreEqual(newCenter.Y, orgCenter.Y, 0.01);
        }

        [TestMethod()]
        public void FirstAndLastCenterIsTheSameForSingleLetterOutline()
        {
            TextOutline oOutline = new TextOutline("O", font, emSize);
            var firstCenter = oOutline.GetFirstGlyphCenter();
            var lastCenter = oOutline.GetLastGlyphCenter();
            Assert.AreEqual(lastCenter.X, firstCenter.X, 0.01);
            Assert.AreEqual(lastCenter.Y, firstCenter.Y, 0.01);
        }

        [TestMethod()]
        public void TestToString()
        {
            TextOutline outline = new TextOutline("Cl", font, emSize);
            var bounds = outline.GetBounds();
            Assert.AreEqual(
                "Cl [x=" + ToString(bounds.X) + ", y=" + ToString(bounds.Y)
                + ", w=" + ToString(bounds.Width) + ", h=" + ToString(bounds.Height) + "]",
                outline.ToString());
        }

        static string ToString(double x)
        {
            return x.ToString("F2");
        }
    }
}
