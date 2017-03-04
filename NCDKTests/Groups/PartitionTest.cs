/* Copyright (C) 2012  Gilleain Torrance <gilleain.torrance@gmail.com>
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

namespace NCDK.Groups
{
    /// <summary>
    // @author maclean
    // @cdk.module test-group
    ///
    /// </summary>
    [TestClass()]
    public class PartitionTest : CDKTestCase
    {
        [TestMethod()]
        public void EmptyConstructor()
        {
            Partition p = new Partition();
            Assert.AreEqual(0, p.Count);
        }

        [TestMethod()]
        public void CopyConstructor()
        {
            Partition p = new Partition();
            p.AddCell(0, 1);
            p.AddCell(2, 3);
            Partition q = new Partition(p);
            Assert.IsTrue(Compares.AreDeepEqual(p, q));
        }

        [TestMethod()]
        public void CellDataConstructor()
        {
            int[][] cellData = new int[][] { new[] { 0, 1 }, new[] { 2, 3, 4 }, new[] { 5, 6 } };
            Partition p = new Partition(cellData);
            Assert.AreEqual(cellData.Length, p.Count);
            Assert.AreEqual(7, p.NumberOfElements());
        }

        [TestMethod()]
        public void UnitStaticConstructor()
        {
            int size = 5;
            Partition p = Partition.Unit(size);
            Assert.AreEqual(1, p.Count);
            Assert.AreEqual(size, p.GetCell(0).Count);
        }

        [TestMethod()]
        public void SizeTest()
        {
            Partition p = new Partition();
            p.AddCell(0, 1);
            p.AddCell(2, 3);
            Assert.AreEqual(2, p.Count);
            Assert.AreEqual(2, p.GetCell(0).Count);
            Assert.AreEqual(2, p.GetCell(1).Count);
        }

        [TestMethod()]
        public void NumberOfElementsTest()
        {
            Partition p = new Partition();
            p.AddCell(0, 1);
            p.AddCell(2, 3);
            Assert.AreEqual(4, p.NumberOfElements());
        }

        [TestMethod()]
        public void IsDiscreteTest()
        {
            int size = 5;
            Partition p = new Partition();
            for (int i = 0; i < size; i++)
            {
                p.AddSingletonCell(i);
            }
            Assert.IsTrue(p.IsDiscrete());
        }

        [TestMethod()]
        public void ToPermutationTest()
        {
            int size = 5;
            Partition partition = new Partition();
            for (int i = 0; i < size; i++)
            {
                partition.AddSingletonCell(i);
            }
            Permutation permutation = partition.ToPermutation();
            Assert.AreEqual(size, permutation.Count);
            for (int i = 0; i < size; i++)
            {
                Assert.AreEqual(i, permutation[i]);
            }
        }

        [TestMethod()]
        public void InOrderTest()
        {
            int[][] cellData = new int[][] { new[] { 0, 1 }, new[] { 2, 3, 4 }, new[] { 5, 6 } };
            Partition p = new Partition(cellData);
            Assert.IsTrue(p.InOrder());
        }

        [TestMethod()]
        public void GetFirstInCellTest()
        {
            int[][] cellData = new int[][] { new[] { 0, 1 }, new[] { 2, 3, 4 }, new[] { 5, 6 } };
            Partition p = new Partition(cellData);
            for (int i = 0; i < cellData.Length; i++)
            {
                Assert.AreEqual(cellData[i][0], p.GetFirstInCell(i));
            }
        }

        [TestMethod()]
        public void GetCellTest()
        {
            int[][] cellData = new int[][] { new[] { 0, 1 }, new[] { 2, 3, 4 }, new[] { 5, 6 } };
            Partition p = new Partition(cellData);
            for (int i = 0; i < cellData.Length; i++)
            {
                int[] cell = p.GetCell(i).ToArray();
                Assert.AreEqual(cellData[i].Length, cell.Length);
                for (int j = 0; j < cell.Length; j++)
                {
                    Assert.AreEqual(cellData[i][j], (int)cell[j]);
                }
            }
        }

        [TestMethod()]
        public void SplitBeforeTest()
        {
            int[][] cellData = new int[][] { new[] { 0, 1 }, new[] { 2, 3, 4 }, new[] { 5, 6 } };
            Partition p = new Partition(cellData);
            int cellIndex = 1;
            int splitElement = 3;
            Partition q = p.SplitBefore(cellIndex, splitElement);
            Assert.AreEqual(p.NumberOfElements(), q.NumberOfElements());
            Assert.AreEqual(p.Count + 1, q.Count);
            SortedSet<int> cell = q.GetCell(cellIndex);
            Assert.IsTrue(cell.Count == 1);
            Assert.AreEqual(splitElement, (int)cell.First());
        }

        [TestMethod()]
        public void SplitAfterTest()
        {
            int[][] cellData = new int[][] { new[] { 0, 1 }, new[] { 2, 3, 4 }, new[] { 5, 6 } };
            Partition p = new Partition(cellData);
            int cellIndex = 1;
            int splitElement = 3;
            Partition q = p.SplitAfter(cellIndex, splitElement);
            Assert.AreEqual(p.NumberOfElements(), q.NumberOfElements());
            Assert.AreEqual(p.Count + 1, q.Count);
            SortedSet<int> cell = q.GetCell(cellIndex + 1);
            Assert.IsTrue(cell.Count == 1);
            Assert.AreEqual(splitElement, (int)cell.First());
        }

        [TestMethod()]
        public void SetAsPermutationTest()
        {
            int partitionSize = 5;
            int permutationSize = 3;
            Partition partition = new Partition();
            for (int i = 0; i < partitionSize; i++)
            {
                partition.AddSingletonCell(i);
            }
            Permutation permutation = partition.SetAsPermutation(permutationSize);
            for (int i = 0; i < permutationSize; i++)
            {
                Assert.AreEqual(i, permutation[i]);
            }
        }

        [TestMethod()]
        public void IsDiscreteCellTest()
        {
            int[][] cellData = new int[][] { new[] { 0 }, new[] { 1 }, new[] { 2 }, new[] { 3 }, new[] { 4 }, new[] { 5 } };
            Partition p = new Partition(cellData);
            for (int i = 0; i < p.Count; i++)
            {
                Assert.IsTrue(p.IsDiscreteCell(i));
            }
        }

        [TestMethod()]
        public void GetIndexOfFirstNonDiscreteCellTest()
        {
            int[][] cellData = new int[][] { new[] { 0 }, new[] { 1 }, new[] { 2, 3, 4 }, new[] { 5, 6 } };
            Partition p = new Partition(cellData);
            Assert.AreEqual(2, p.GetIndexOfFirstNonDiscreteCell());
        }

        [TestMethod()]
        public void AddSingletonCellTest()
        {
            Partition p = new Partition();
            p.AddSingletonCell(0);
            Assert.AreEqual(1, p.Count);
            Assert.AreEqual(1, p.NumberOfElements());
        }

        [TestMethod()]
        public void RemoveCellTest()
        {
            int size = 5;
            Partition p = Partition.Unit(size);
            p.RemoveCell(0);
            Assert.AreEqual(0, p.Count);
        }

        [TestMethod()]
        public void AddCell_VarArgsTest()
        {
            Partition p = new Partition();
            p.AddCell(0, 1, 2);
            Assert.AreEqual(1, p.Count);
            Assert.AreEqual(3, p.NumberOfElements());
        }

        [TestMethod()]
        public void AddCell_CollectionTest()
        {
            Partition p = new Partition();
            List<int> cell = new List<int>();
            cell.Add(0);
            cell.Add(1);
            cell.Add(2);
            p.AddCell(cell);
            Assert.AreEqual(1, p.Count);
            Assert.AreEqual(3, p.NumberOfElements());
        }

        [TestMethod()]
        public void AddToCellTest()
        {
            Partition p = new Partition();
            p.AddToCell(0, 0);
            Assert.AreEqual(1, p.Count);
            Assert.AreEqual(1, p.NumberOfElements());
            p.AddToCell(0, 1);
            Assert.AreEqual(1, p.Count);
            Assert.AreEqual(2, p.NumberOfElements());
        }

        [TestMethod()]
        public void InsertCellTest()
        {
            int[][] cellData = new int[][] { new[] { 0 }, new[] { 2 }, new[] { 3 } };
            Partition p = new Partition(cellData);
            SortedSet<int> cell = new SortedSet<int>();
            cell.Add(1);
            p.InsertCell(1, cell);
            Assert.IsTrue(p.IsDiscrete());
        }

        [TestMethod()]
        public void CopyBlockTest()
        {
            int[][] cellData = new int[][] { new[] { 0 }, new[] { 1 }, new[] { 2 } };
            Partition p = new Partition(cellData);
            int cellIndex = 1;
            SortedSet<int> copyCell = p.CopyBlock(cellIndex);
            SortedSet<int> refCell = p.GetCell(cellIndex);
            Assert.IsTrue(copyCell != refCell);
        }

        [TestMethod()]
        public void FromStringTest()
        {
            Partition p = Partition.FromString("[0,1|2,3]");
            Assert.AreEqual(2, p.Count);
            Assert.AreEqual(4, p.NumberOfElements());
        }

        [TestMethod()]
        public void FromStringTest2()
        {
            Partition p = Partition.FromString("[0|1,2,3]");
            Assert.AreEqual(2, p.Count);
            Assert.AreEqual(4, p.NumberOfElements());
        }

        [TestMethod()]
        public void EqualsTest_null()
        {
            Partition p = new Partition(new int[][] { new[] { 0 }, new[] { 1 } });
            Assert.AreNotSame(p, null);
        }

        [TestMethod()]
        public void EqualsTest_different()
        {
            Partition p = new Partition(new int[][] { new[] { 0 }, new[] { 1 } });
            Partition o = new Partition(new int[][] { new[] { 1 }, new[] { 0 } });
            Assert.AreNotSame(p, o);
        }

        [TestMethod()]
        public void EqualsTest()
        {
            Partition p = new Partition(new int[][] { new[] { 0 }, new[] { 1 } });
            Partition o = new Partition(new int[][] { new[] { 0 }, new[] { 1 } });
            Assert.IsTrue(Compares.AreDeepEqual(p, o));
        }

        [TestMethod()]
        public void OrderTest()
        {
            Partition p = new Partition(new int[][] { new[] { 1, 3 }, new[] { 0, 2 } });
            p.Order();
            SortedSet<int> cell0 = p.GetCell(0);
            SortedSet<int> cell1 = p.GetCell(1);
            Assert.IsTrue(cell0.First() < cell1.First());
            Assert.IsTrue(cell0.Last() < cell1.Last());
        }

        [TestMethod()]
        public void InSameCellTest()
        {
            Partition p = new Partition(new int[][] { new[] { 0, 2 }, new[] { 1, 3 } });
            Assert.IsTrue(p.InSameCell(1, 3));
        }
    }
}
