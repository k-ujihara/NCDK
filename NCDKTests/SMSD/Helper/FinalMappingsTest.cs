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
using NCDK.Common.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.SMSD.Helper
{
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    // @cdk.module test-smsd
    [TestClass()]
    public class FinalMappingsTest
    {
        public FinalMappingsTest() { }

        /// <summary>
        /// Test of getInstance method, of class FinalMappings.
        /// </summary>
        [TestMethod()]
        public void TestInstance()
        {
            FinalMappings result = FinalMappings.Instance;
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Test of add method, of class FinalMappings.
        /// </summary>
        [TestMethod()]
        public void TestAdd()
        {
            IDictionary<int, int> mapping = new SortedDictionary<int, int>();
            mapping[1] = 1;
            mapping[2] = 2;
            mapping[3] = 3;

            FinalMappings instance = new FinalMappings();
            instance.Add(mapping);
            Assert.AreEqual(1, instance.Count);
        }

        /// <summary>
        /// Test of set method, of class FinalMappings.
        /// </summary>
        [TestMethod()]
        public void TestSet()
        {
            IDictionary<int, int> mapping1 = new SortedDictionary<int, int>();
            mapping1[1] = 1;
            mapping1[2] = 2;
            mapping1[3] = 3;
            IDictionary<int, int> mapping2 = new SortedDictionary<int, int>();
            mapping2[1] = 2;
            mapping2[2] = 1;
            mapping2[3] = 3;

            List<IDictionary<int, int>> mappings = new List<IDictionary<int, int>>(2);
            mappings.Add(mapping1);
            mappings.Add(mapping2);
            FinalMappings instance = new FinalMappings();
            instance.Set(mappings);

            Assert.AreEqual(2, instance.Count);
        }

        /// <summary>
        /// Test of getIterator method, of class FinalMappings.
        /// </summary>
        [TestMethod()]
        public void TestGetIterator()
        {
            IDictionary<int, int> mapping1 = new SortedDictionary<int, int>();
            mapping1[1] = 1;
            mapping1[2] = 2;
            mapping1[3] = 3;
            IDictionary<int, int> mapping2 = new SortedDictionary<int, int>();
            mapping2[1] = 2;
            mapping2[2] = 1;
            mapping2[3] = 3;

            List<IDictionary<int, int>> mappings = new List<IDictionary<int, int>>(2);
            mappings.Add(mapping1);
            mappings.Add(mapping2);
            FinalMappings instance = new FinalMappings();
            instance.Set(mappings);

            Assert.AreEqual(true, instance.Any());
        }

        /// <summary>
        /// Test of clear method, of class FinalMappings.
        /// </summary>
        [TestMethod()]
        public void TestClear()
        {
            IDictionary<int, int> mapping1 = new SortedDictionary<int, int>();
            mapping1[1] = 1;
            mapping1[2] = 2;
            mapping1[3] = 3;
            IDictionary<int, int> mapping2 = new SortedDictionary<int, int>();
            mapping2[1] = 2;
            mapping2[2] = 1;
            mapping2[3] = 3;

            List<IDictionary<int, int>> mappings = new List<IDictionary<int, int>>(2);
            mappings.Add(mapping1);
            mappings.Add(mapping2);
            FinalMappings instance = new FinalMappings();
            instance.Set(mappings);
            Assert.AreEqual(2, instance.Count);
            instance.Clear();
            Assert.AreEqual(0, instance.Count);
        }

        /// <summary>
        /// Test of getFinalMapping method, of class FinalMappings.
        /// </summary>
        [TestMethod()]
        public void TestGetFinalMapping()
        {
            IDictionary<int, int> mapping1 = new SortedDictionary<int, int>();
            mapping1[1] = 1;
            mapping1[2] = 2;
            mapping1[3] = 3;
            IDictionary<int, int> mapping2 = new SortedDictionary<int, int>();
            mapping2[1] = 2;
            mapping2[2] = 1;
            mapping2[3] = 3;

            List<IDictionary<int, int>> mappings = new List<IDictionary<int, int>>(2);
            mappings.Add(mapping1);
            mappings.Add(mapping2);
            FinalMappings instance = new FinalMappings();
            instance.Set(mappings);

            var expResult = mappings;
            var result = instance.GetFinalMapping();
            Assert.IsTrue(Compares.AreEqual(expResult, result));
        }

        /// <summary>
        /// Test of getSize method, of class FinalMappings.
        /// </summary>
        [TestMethod()]
        public void TestGetSize()
        {
            IDictionary<int, int> mapping1 = new SortedDictionary<int, int>();
            mapping1[1] = 1;
            mapping1[2] = 2;
            mapping1[3] = 3;
            IDictionary<int, int> mapping2 = new SortedDictionary<int, int>();
            mapping2[1] = 2;
            mapping2[2] = 1;
            mapping2[3] = 3;

            List<IDictionary<int, int>> mappings = new List<IDictionary<int, int>>(2);
            mappings.Add(mapping1);
            mappings.Add(mapping2);
            FinalMappings instance = new FinalMappings();
            instance.Set(mappings);
            Assert.AreEqual(2, instance.Count);
        }
    }
}
