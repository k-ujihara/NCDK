/* Copyright (C) 2008  Egon Willighagen <egonw@users.sf.net>
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
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;
using WPF = System.Windows;

namespace NCDK.Renderers.Colors
{
    // @cdk.module test-render
    [TestClass()]
    public class CPKAtomColorsTest
    {
        [TestMethod()]
        public void TestGetAtomColor()
        {
            CPKAtomColors colors = new CPKAtomColors();
            Assert.IsNotNull(colors);
            Assert.AreEqual(WPF::Media.Colors.White, colors.GetAtomColor(new Atom("H")));
            Assert.AreEqual(WPF::Media.Color.FromRgb(0xFF, 0xC0, 0xCB), colors.GetAtomColor(new Atom("He")));
        }

        [TestMethod()]
        public void TestGetDefaultAtomColor()
        {
            CPKAtomColors colors = new CPKAtomColors();

            Assert.IsNotNull(colors);
            IAtom imaginary = new PseudoAtom("Ix");
            Assert.AreEqual(WPF::Media.Colors.Orange, colors.GetAtomColor(imaginary, WPF::Media.Colors.Orange));
        }
    }
}

