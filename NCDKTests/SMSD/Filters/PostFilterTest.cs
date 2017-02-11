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
    /**
     * @author Syed Asad Rahman <asad@ebi.ac.uk>
     *
     * @cdk.module test-smsd
     * @cdk.require java1.6+
     */
    [TestClass()]
    public class PostFilterTest
    {

        public PostFilterTest() { }

        /**
         * Test of filter method, of class PostFilter.
         */
        [TestMethod()]
        public void TestFilter()
        {

            List<int> l1 = new List<int>(6);
            l1.Add(1);
            l1.Add(2);
            l1.Add(3);
            l1.Add(4);
            l1.Add(5);
            l1.Add(6);

            List<int> l2 = new List<int>(6);
            l2.Add(1);
            l2.Add(2);
            l2.Add(3);
            l2.Add(4);
            l2.Add(5);
            l2.Add(6);

            List<int> l3 = new List<int>(6);
            l3.Add(1);
            l3.Add(2);
            l3.Add(5);
            l3.Add(4);
            l3.Add(3);
            l3.Add(6);

            var mappings = new List<IList<int>>();
            mappings.Add(l1);
            mappings.Add(l2);
            mappings.Add(l3);

            Assert.AreEqual(3, mappings.Count);
            var expResult = PostFilter.Filter(mappings);
            Assert.AreEqual(2, expResult.Count);
        }
    }
}
