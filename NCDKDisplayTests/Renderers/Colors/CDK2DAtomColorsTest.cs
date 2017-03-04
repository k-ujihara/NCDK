/* Copyright (C) 2009  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License as published by the Free
 * Software Foundation; either version 2.1 of the License, or (at your option)
 * any later version. All we ask is that proper credit is given for our work,
 * which includes - but is not limited to - adding the above copyright notice to
 * the beginning of your source code files, and to any copyright notice that you
 * may distribute with programs based on this work.
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
using NCDK.Default;
using System.Windows.Media;
using WPF = System.Windows;

namespace NCDK.Renderers.Colors
{
    // @cdk.module test-render
    [TestClass()]
    public class CDK2DAtomColorsTest : CDKTestCase
    {
        [TestMethod()]
        public void TestGetAtomColor()
        {
            CDK2DAtomColors colors = new CDK2DAtomColors();
            Assert.IsNotNull(colors);
            IAtom hydrogen = new Atom("H");
            hydrogen.AtomicNumber = 1;
            Assert.AreEqual(Color.FromRgb(0, 0, 0), colors.GetAtomColor(hydrogen));
            IAtom helium = new Atom("He");
            helium.AtomicNumber = 2;
            Assert.AreEqual(Color.FromRgb(217, 255, 255), colors.GetAtomColor(helium));
        }

        [TestMethod()]
        public void TestGetDefaultAtomColor()
        {
            CDK2DAtomColors colors = new CDK2DAtomColors();

            Assert.IsNotNull(colors);
            IAtom imaginary = new Atom("Ix");
            Assert.AreEqual(Color.FromRgb(0, 0, 0), colors.GetAtomColor(imaginary, WPF::Media.Colors.Black));
        }
    }
}

