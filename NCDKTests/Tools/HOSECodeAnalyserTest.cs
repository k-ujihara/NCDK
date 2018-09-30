/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
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
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace NCDK.Tools
{
    // @cdk.module test-extra
    [TestClass()]
    public class HOSECodeAnalyserTest : CDKTestCase
    {
        [TestMethod()]
        public void TestGetElementsString()
        {
            var elements = HOSECodeAnalyser.GetElements("CCY(CF,C,/C,,&/&)//");
            Assert.AreEqual(3, elements.Count);
            Assert.IsTrue(elements.Contains("C"));
            Assert.IsTrue(elements.Contains("F"));
            Assert.IsTrue(elements.Contains("Br"));
        }

        [TestMethod()]
        public void TestCode1()
        {
            var elements = HOSECodeAnalyser.GetElements("*C*CC(*C,*C,=C/*C,*&,CC/*&O,=OO,%N),C,,C,/");
            Assert.AreEqual(3, elements.Count);
            Assert.IsTrue(elements.Contains("C"));
            Assert.IsTrue(elements.Contains("O"));
            Assert.IsTrue(elements.Contains("N"));
        }
    }
}
