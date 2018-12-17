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

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Renderers.Elements
{
    // @author John May
    // @cdk.module test-renderbasic
    [TestClass()]
    public class BoundsTest : AbstractElementTest
    {
        static BoundsTest()
        {
            SetRenderingElement(new Bounds(0, 0, 0, 0));
        }

        [TestMethod()]
        public void WidthTest()
        {
            Assert.AreEqual(5, new Bounds(2, 2, 7, 6).Width, 0.1);
        }

        [TestMethod()]
        public void HeightTest()
        {
            Assert.AreEqual(4, new Bounds(2, 2, 7, 6).Height, 0.1);
        }
    }
}
