/* Copyright (C) 2009-2010 Syed Asad Rahman <asad@ebi.ac.uk>
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
using System.Collections.Generic;

namespace NCDK.SMSD.Filters
{
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    // @cdk.module test-smsd
    [TestClass()]
    public class PostFilterTest
    {
        public PostFilterTest() { }

        /// <summary>
        /// Test of filter method, of class PostFilter.
        /// </summary>
        [TestMethod()]
        public void TestFilter()
        {
            List<int> l1 = new List<int>(6)
            {
                1,
                2,
                3,
                4,
                5,
                6
            };

            List<int> l2 = new List<int>(6)
            {
                1,
                2,
                3,
                4,
                5,
                6
            };

            List<int> l3 = new List<int>(6)
            {
                1,
                2,
                5,
                4,
                3,
                6
            };

            var mappings = new List<IReadOnlyList<int>>
            {
                l1,
                l2,
                l3
            };

            Assert.AreEqual(3, mappings.Count);
            var expResult = PostFilter.Filter(mappings);
            Assert.AreEqual(2, expResult.Count);
        }
    }
}
