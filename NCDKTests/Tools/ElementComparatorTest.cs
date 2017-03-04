/* Copyright (C) 2007  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Tools
{
    /// <summary>
    // @cdk.module test-standard
    /// </summary>
    [TestClass()]
    public class ElementComparatorTest : CDKTestCase
    {

        public ElementComparatorTest()
            : base()
        { }


        [TestMethod()]
        public void TestElementComparator()
        {
            ElementComparator comp = new ElementComparator();
            Assert.IsNotNull(comp);
        }

        /// <summary>
        // @cdk.bug 1638375
        /// </summary>
        [TestMethod()]
        public void TestCompare_Object_Object()
        {
            ElementComparator comp = new ElementComparator();

            Assert.IsTrue(comp.Compare("C", "H") < 0);
            Assert.IsTrue(comp.Compare("H", "O") < 0);
            Assert.IsTrue(comp.Compare("N", "O") < 0);
            Assert.AreEqual(0, comp.Compare("Cl", "Cl"));
            Assert.IsTrue(comp.Compare("Cl", "C") > 0);
        }
    }
}
