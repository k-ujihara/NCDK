/*
 * Copyright (C) 2009  Mark Rijnbeek <mark_rynbeek@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using WPF = System.Windows;

namespace NCDK.Renderers.Colors
{
    // @cdk.module test-render
    [TestClass()]
    public class RasmolColorsTest : CDKTestCase
    {
        [TestMethod()]
        public void TestGetAtomColor()
        {
            RasmolColors colors = new RasmolColors();

            Assert.IsNotNull(colors);
            IAtom sulfur = new Atom("S");
            Assert.AreEqual(WPF::Media.Color.FromRgb(255, 200, 50), colors.GetAtomColor(sulfur));

            IAtom helium = new Atom("He");
            Assert.AreEqual(WPF::Media.Color.FromRgb(255, 192, 203), colors.GetAtomColor(helium));
        }

        [TestMethod()]
        public void TestGetDefaultAtomColor()
        {
            RasmolColors colors = new RasmolColors();

            Assert.IsNotNull(colors);
            IAtom imaginary = new Atom("Ix");
            Assert.AreEqual(WPF::Media.Colors.Orange, colors.GetAtomColor(imaginary, WPF::Media.Colors.Orange));
        }
    }
}

