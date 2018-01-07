/* Copyright (C) 2011  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License as published by the Free
 * Software Foundation; either version 2.1 of the License, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more
 * details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Renderers.Elements;
using NCDK.Renderers.Fonts;
using System.Windows.Media;
using WPF = System.Windows;

namespace NCDK.Renderers.Visitors
{
    // @cdk.module  test-renderawt
    // @cdk.githash
    [TestClass()]
    public class AbstractWPFDrawVisitorTest
    {
        private sealed class NestedAWTDrawVisitor : AbstractWPFDrawVisitor
        {
            public override void Visit(IRenderingElement element) { }
            public override RendererModel RendererModel { get => null; set { } }
            public override IFontManager FontManager { get => null; set { } }
        }

        [TestMethod()]
        public void TestExtension()
        {
            AbstractWPFDrawVisitor visitor = new NestedAWTDrawVisitor();
            Assert.IsNotNull(visitor);
        }

        [TestMethod()]
        public void TestSetAffineTransformation()
        {
            AbstractWPFDrawVisitor visitor = new NestedAWTDrawVisitor
            {
                Transform = Transform.Identity
            };
            Assert.IsNotNull(visitor);
        }

        [TestMethod()]
        public void TestGetTextBounds()
        {
            AbstractWPFDrawVisitor visitor = new NestedAWTDrawVisitor
            {
                Transform = Transform.Identity
            };
            var typeface = new Typeface(
                  new FontFamily("Arial"),
                  WPF::FontStyles.Normal,
                  WPF::FontWeights.Normal,
                  WPF::FontStretches.Normal);
            var rectangle = visitor.GetTextBounds("Foo", new WPF.Point(3, 5), typeface, 9);
            Assert.IsNotNull(rectangle);
        }

        [TestMethod()]
        public void TestTransformPoint()
        {
            AbstractWPFDrawVisitor visitor = new NestedAWTDrawVisitor
            {
                Transform = Transform.Identity // no transform
            };
            var transformed = visitor.TransformPoint(new WPF.Point(1, 2));
            Assert.AreEqual(1, transformed.X);
            Assert.AreEqual(2, transformed.Y);
        }

        [TestMethod()]
        public void TestGetTextBasePoint()
        {
            AbstractWPFDrawVisitor visitor = new NestedAWTDrawVisitor
            {
                Transform = Transform.Identity
            };
            var typeface = new Typeface(
                  new FontFamily("Arial"),
                  WPF::FontStyles.Normal,
                  WPF::FontWeights.Normal,
                  WPF::FontStretches.Normal);
            var point = visitor.GetTextBasePoint("Foo", new WPF.Point(3, 5), typeface, 9);
            Assert.IsNotNull(point);
        }
    }
}
