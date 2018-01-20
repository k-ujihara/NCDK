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
    public class WPFDrawVisitorTest
    {
        [TestMethod()]
        public void TestConstructor()
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext g2d = drawingVisual.RenderOpen();
            WPFDrawVisitor visitor = new WPFDrawVisitor(g2d);
            Assert.IsNotNull(visitor);
        }

        [TestMethod()]
        public void TestSetFontManager()
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext g2d = drawingVisual.RenderOpen();
            WPFDrawVisitor visitor = new WPFDrawVisitor(g2d)
            {
                FontManager = new WPFFontManager()
            };
            // at least we now know it did not crash...
            Assert.IsNotNull(visitor);
        }

        [TestMethod()]
        public void TestSetRendererModel()
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext g2d = drawingVisual.RenderOpen();
            WPFDrawVisitor visitor = new WPFDrawVisitor(g2d)
            {
                RendererModel = new RendererModel()
            };
            // at least we now know it did not crash...
            Assert.IsNotNull(visitor);
        }

        [TestMethod()]
        public void TestGetRendererModel()
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext g2d = drawingVisual.RenderOpen();
            WPFDrawVisitor visitor = new WPFDrawVisitor(g2d);
            RendererModel model = new RendererModel();
            visitor.RendererModel = model;
            Assert.AreEqual(model, visitor.RendererModel);
        }

        [TestMethod()]
        public void TestVisit()
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext g2d = drawingVisual.RenderOpen();
            WPFDrawVisitor visitor = new WPFDrawVisitor(g2d)
            {
                FontManager = new WPFFontManager()
            };
            visitor.Visit(new TextElement(new WPF.Point(2, 3), "Foo", WPF::Media.Colors.Black), Transform.Identity);
            // at least we now know it did not crash...
            Assert.IsNotNull(visitor);
        }

        [TestMethod()]
        public void TestGetGraphics()
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext dc = drawingVisual.RenderOpen();
            WPFDrawVisitor visitor = new WPFDrawVisitor(dc);
            Assert.AreEqual(dc, visitor.dc);
        }
    }
}
