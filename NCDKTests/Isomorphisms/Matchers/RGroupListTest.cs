/*
 * Copyright (C) 2010  Mark Rijnbeek <mark_rynbeek@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may
 * distribute with programs based on this work.
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

namespace NCDK.Isomorphisms.Matchers
{
    /// <summary>
    /// Checks the functionality of the {@link org.openscience.cdk.isomorphism.matchers.RGroupList},
    /// in particular setting valid 'occurrence' strings.
    ///
    // @cdk.module test-isomorphism
    /// </summary>
    [TestClass()]
    public class RGroupListTest : CDKTestCase
    {
        [TestMethod()]
        public void TestOccurrenceCorrect()
        {
            RGroupList rgrLst = new RGroupList(1);
            rgrLst.Occurrence = "1, 3-7, 9, >11";
            Assert.AreEqual(rgrLst.Occurrence, "1,3-7,9,>11");
        }

        [TestMethod()]
        public void TestOccurrenceNull()
        {
            RGroupList rgrLst = new RGroupList(1);
            rgrLst.Occurrence = null;
            Assert.AreEqual(rgrLst.Occurrence, RGroupList.DEFAULT_OCCURRENCE);
        }

        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        public void TestOccurrenceNumericValues()
        {
            RGroupList rgrLst = new RGroupList(1);
            rgrLst.Occurrence = "a,3,10";
        }

        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        public void TestOccurrenceNoNegativeNumber()
        {
            RGroupList rgrLst = new RGroupList(1);
            rgrLst.Occurrence = "-10";
        }

        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        public void TestOccurrenceNotSmallerThanZero()
        {
            RGroupList rgrLst = new RGroupList(1);
            rgrLst.Occurrence = "<0";
        }
    }
}
