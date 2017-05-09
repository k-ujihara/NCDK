/* Copyright (C) 2009-2010 maclean {gilleain.torrance@gmail.com}
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

namespace NCDK.Signatures
{
    // @cdk.module test-signature
    // @author maclean
    [TestClass()]
    public class OrbitTest
    {
        private string orbitLabel;
        private Orbit orbit;
        private Orbit unsortedOrbit;

        public OrbitTest()
        {
            // make a test orbit instance, with a nonsense
            // string label, and some number of 'indices'
            orbitLabel = "ORBIT";
            int height = 2;
            orbit = new Orbit(orbitLabel, height);
            int[] atomIndices = new int[] { 0, 1, 2, 3 };
            foreach (var atomIndex in atomIndices)
            {
                orbit.AddAtomAt(atomIndex);
            }

            // also make an unsorted orbit
            string unsortedOrbitLabel = "UNSORTED_ORBIT";
            int unsortedHeight = 2;
            unsortedOrbit = new Orbit(unsortedOrbitLabel, unsortedHeight);
            int[] unsortedAtomIndices = new int[] { 3, 1, 0, 2 };
            foreach (var atomIndex in unsortedAtomIndices)
            {
                unsortedOrbit.AddAtomAt(atomIndex);
            }
        }

        [TestMethod()]
        public void IteratorTest()
        {
            int count = 0;
            List<int> indices = orbit.AtomIndices;
            foreach (var i in orbit)
            {
                Assert.AreEqual(i, indices[count]);
                count++;
            }
            Assert.AreEqual(indices.Count, count);
        }

        [TestMethod()]
        public void TestClone()
        {
            Orbit clonedOrbit = (Orbit)orbit.Clone();
            List<int> indices = orbit.AtomIndices;
            List<int> clonedIndices = clonedOrbit.AtomIndices;
            Assert.IsTrue(Compares.AreDeepEqual(indices, clonedIndices));
            Assert.AreEqual(orbit.Label, clonedOrbit.Label);
        }

        [TestMethod()]
        public void IsEmptyTest()
        {
            Assert.IsFalse(orbit.IsEmpty(), "The setUp method should have made an orbit with " + "some indices in it");
            List<int> indices = new List<int>();
            foreach (var index in orbit)
            {
                indices.Add(index);
            }
            foreach (var index in indices)
            {
                orbit.Remove(index);
            }
            Assert.IsTrue(orbit.IsEmpty(), "Orbit should now be empty");
        }

        private bool IsSorted(Orbit orbit)
        {
            int prev = -1;
            foreach (var index in orbit)
            {
                if (prev == -1 || index > prev)
                {
                    prev = index;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        [TestMethod()]
        public void SortTest()
        {
            Assert.IsFalse(IsSorted(unsortedOrbit), "Unsorted orbit is actually sorted");
            unsortedOrbit.Sort();
            Assert.IsTrue(IsSorted(unsortedOrbit), "Orbit is not sorted after sort called");
        }

        [TestMethod()]
        public void GetHeightTest()
        {
            Assert.AreEqual(2, orbit.Height);
        }

        [TestMethod()]
        public void GetAtomIndicesTest()
        {
            Assert.IsNotNull(orbit.AtomIndices);
        }

        [TestMethod()]
        public void AddAtomTest()
        {
            Assert.AreEqual(4, orbit.AtomIndices.Count);
            orbit.AddAtomAt(4);
            Assert.AreEqual(5, orbit.AtomIndices.Count);
        }

        [TestMethod()]
        public void HasLabelTest()
        {
            Assert.IsTrue(orbit.HasLabel(orbitLabel));
        }

        [TestMethod()]
        public void GetFirstAtomTest()
        {
            Assert.AreEqual(0, orbit.FirstAtom);
        }

        [TestMethod()]
        public void RemoveTest()
        {
            Assert.AreEqual(4, orbit.AtomIndices.Count);
            orbit.Remove(0);
            Assert.AreEqual(3, orbit.AtomIndices.Count);
        }

        [TestMethod()]
        public void GetLabelTest()
        {
            Assert.AreEqual(orbitLabel, orbit.Label);
        }

        [TestMethod()]
        public void ContainsTest()
        {
            foreach (var index in orbit)
            {
                Assert.IsTrue(orbit.Contains(index), "Index " + index + " not in orbit");
            }
        }

        [TestMethod()]
        public void ToStringTest()
        {
            Assert.AreEqual("ORBIT [0, 1, 2, 3]", orbit.ToString());
        }
    }
}
