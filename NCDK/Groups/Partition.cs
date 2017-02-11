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
using NCDK.Common.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCDK.Groups
{
    /**
     * A partition of a set of integers, such as the discrete partition {{1}, {2},
     * {3}, {4}} or the unit partition {{1, 2, 3, 4}} or an intermediate like {{1,
     * 2}, {3, 4}}.
     *
     * @author maclean
     * @cdk.module group
     */
    public class Partition
    {

        /**
         * The subsets of the partition, known as cells.
         */
        private List<SortedSet<int>> cells;

        /**
         * Creates a new, empty partition with no cells.
         */
        public Partition()
        {
            this.cells = new List<SortedSet<int>>();
        }

        /**
         * Copy constructor to make one partition from another.
         *
         * @param other the partition to copy
         */
        public Partition(Partition other)
            : this()
        {
            foreach (var block in other.cells)
            {
                this.cells.Add(new SortedSet<int>(block));
            }
        }

        /**
         * Constructor to make a partition from an array of int arrays.
         *
         * @param cellData the partition to copy
         */
        public Partition(int[][] cellData)
                : this()
        {
            foreach (var aCellData in cellData)
            {
                AddCell(aCellData);
            }
        }

        /**
         * Create a unit partition - in other words, the coarsest possible partition
         * where all the elements are in one cell.
         *
         * @param size the number of elements
         * @return a new Partition with one cell containing all the elements
         */
        public static Partition Unit(int size)
        {
            Partition unit = new Partition();
            unit.cells.Add(new SortedSet<int>());
            for (int i = 0; i < size; i++)
            {
                unit.cells[0].Add(i);
            }
            return unit;
        }

        public override bool Equals(object o)
        {

            if (this == o) return true;
            if (o == null || GetType() != o.GetType()) return false;

            Partition partition = (Partition)o;

            return cells != null ? Compares.AreDeepEqual(cells, partition.cells) : partition.cells == null;

        }

        public override int GetHashCode()
        {
            return cells != null ? Lists.GetDeepHashCode(cells) : 0;
        }

        /**
         * Gets the size of the partition, in terms of the number of cells.
         *
         * @return the number of cells in the partition
         */
        public int Count => this.cells.Count;

        /**
         * Calculate the size of the partition as the sum of the sizes of the cells.
         *
         * @return the number of elements in the partition
         */
        public int NumberOfElements()
        {
            int n = 0;
            foreach (var cell in cells)
            {
                n += cell.Count;
            }
            return n;
        }

        /**
         * Checks that all the cells are singletons - that is, they only have one
         * element. A discrete partition is equivalent to a permutation.
         *
         * @return true if all the cells are discrete
         */
        public bool IsDiscrete()
        {
            foreach (var cell in cells)
            {
                if (cell.Count != 1)
                {
                    return false;
                }
            }
            return true;
        }

        /**
         * Converts the whole partition into a permutation.
         *
         * @return the partition as a permutation
         */
        public Permutation ToPermutation()
        {
            Permutation p = new Permutation(this.Count);
            for (int i = 0; i < this.Count; i++)
            {
                p[i] = this.cells[i].First();
            }
            return p;
        }

        /**
         * Check whether the cells are ordered such that for cells i and j,
         * First(j) > First(i) and Last(j) > Last(i).
         *
         * @return true if all cells in the partition are ordered
         */
        public bool InOrder()
        {
            SortedSet<int> prev = null;
            foreach (var cell in cells)
            {
                if (prev == null)
                {
                    prev = cell;
                }
                else
                {
                    int first = cell.First();
                    int last = cell.Last();
                    if (first > prev.First() && last > prev.Last())
                    {
                        prev = cell;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /**
         * Gets the first element in the specified cell.
         *
         * @param cellIndex the cell to use
         * @return the first element in this cell
         */
        public int GetFirstInCell(int cellIndex)
        {
            return this.cells[cellIndex].First();
        }

        /**
         * Gets the cell at this index.
         *
         * @param cellIndex the index of the cell to return
         * @return the cell at this index
         */
        public SortedSet<int> GetCell(int cellIndex)
        {
            return this.cells[cellIndex];
        }

        /**
         * Splits this partition by taking the cell at cellIndex and making two
         * new cells - the first with the singleton splitElement and the second
         * with the rest of the elements from that cell.
         *
         * @param cellIndex the index of the cell to split on
         * @param splitElement the element to put in its own cell
         * @return a new (finer) Partition
         */
        public Partition SplitBefore(int cellIndex, int splitElement)
        {
            Partition r = new Partition();
            // copy the cells up to cellIndex
            for (int j = 0; j < cellIndex; j++)
            {
                r.AddCell(this.CopyBlock(j));
            }

            // split the block at block index
            r.AddSingletonCell(splitElement);
            SortedSet<int> splitBlock = this.CopyBlock(cellIndex);
            splitBlock.Remove(splitElement);
            r.AddCell(splitBlock);

            // copy the blocks after blockIndex, shuffled up by one
            for (int j = cellIndex + 1; j < this.Count; j++)
            {
                r.AddCell(this.CopyBlock(j));
            }
            return r;
        }

        /**
         * Splits this partition by taking the cell at cellIndex and making two
         * new cells - the first with the the rest of the elements from that cell
         * and the second with the singleton splitElement.
         *
         * @param cellIndex the index of the cell to split on
         * @param splitElement the element to put in its own cell
         * @return a new (finer) Partition
         */
        public Partition SplitAfter(int cellIndex, int splitElement)
        {
            Partition r = new Partition();
            // copy the blocks up to blockIndex
            for (int j = 0; j < cellIndex; j++)
            {
                r.AddCell(this.CopyBlock(j));
            }

            // split the block at block index
            SortedSet<int> splitBlock = this.CopyBlock(cellIndex);
            splitBlock.Remove(splitElement);
            r.AddCell(splitBlock);
            r.AddSingletonCell(splitElement);

            // copy the blocks after blockIndex, shuffled up by one
            for (int j = cellIndex + 1; j < this.Count; j++)
            {
                r.AddCell(this.CopyBlock(j));
            }
            return r;
        }

        /**
         * Fill the elements of a permutation from the first element of each
         * cell, up to the point <code>upTo</code>.
         *
         * @param upTo take values from cells up to this one
         * @return the permutation representing the first element of each cell
         */
        public Permutation SetAsPermutation(int upTo)
        {
            int[] p = new int[upTo];
            for (int i = 0; i < upTo; i++)
            {
                p[i] = this.cells[i].First();
            }
            return new Permutation(p);
        }

        /**
         * Check to see if the cell at <code>cellIndex</code> is discrete - that is,
         * it only has one element.
         *
         * @param cellIndex the index of the cell to check
         * @return true of the cell at this index is discrete
         */
        public bool IsDiscreteCell(int cellIndex)
        {
            return this.cells[cellIndex].Count == 1;
        }

        /**
         * Gets the index of the first cell in the partition that is discrete.
         *
         * @return the index of the first discrete cell
         */
        public int GetIndexOfFirstNonDiscreteCell()
        {
            for (int i = 0; i < this.cells.Count; i++)
            {
                if (!IsDiscreteCell(i)) return i;
            }
            return -1; // XXX
        }

        /**
         * Add a new singleton cell to the end of the partition containing only
         * this element.
         *
         * @param element the element to add in its own cell
         */
        public void AddSingletonCell(int element)
        {
            SortedSet<int> cell = new SortedSet<int>();
            cell.Add(element);
            this.cells.Add(cell);
        }

        /**
         * Removes the cell at the specified index.
         *
         * @param index the index of the cell to remove
         */
        public void RemoveCell(int index)
        {
            this.cells.RemoveAt(index);
        }

        /**
         * Adds a new cell to the end of the partition containing these elements.
         *
         * @param elements the elements to add in a new cell
         */
        public void AddCell(params int[] elements)
        {
            SortedSet<int> cell = new SortedSet<int>();
            foreach (var element in elements)
            {
                cell.Add(element);
            }
            this.cells.Add(cell);
        }

        /**
         * Adds a new cell to the end of the partition.
         *
         * @param elements the collection of elements to put in the cell
         */
        public void AddCell(ICollection<int> elements)
        {
            cells.Add(new SortedSet<int>(elements));
        }

        /**
         * Add an element to a particular cell.
         *
         * @param index the index of the cell to add to
         * @param element the element to add
         */
        public void AddToCell(int index, int element)
        {
            if (cells.Count < index + 1)
            {
                AddSingletonCell(element);
            }
            else
            {
                cells[index].Add(element);
            }
        }

        /**
         * Insert a cell into the partition at the specified index.
         *
         * @param index the index of the cell to add
         * @param cell the cell to add
         */
        public void InsertCell(int index, SortedSet<int> cell)
        {
            this.cells.Insert(index, cell);
        }

        /**
         * Creates and returns a copy of the cell at cell index.
         *
         * @param cellIndex the cell to copy
         * @return the copy of the cell
         */
        public SortedSet<int> CopyBlock(int cellIndex)
        {
            return new SortedSet<int>(this.cells[cellIndex]);
        }

        /**
         * Sort the cells in increasing order.
         */
        public void Order()
        {
            cells.Sort(delegate (SortedSet<int> cellA, SortedSet<int> cellB) { return cellA.First().CompareTo(cellB.First()); });
        }

        /**
         * Check that two elements are in the same cell of the partition.
         *
         * @param elementI an element in the partition
         * @param elementJ an element in the partition
         * @return true if both elements are in the same cell
         */
        public bool InSameCell(int elementI, int elementJ)
        {
            for (int cellIndex = 0; cellIndex < Count; cellIndex++)
            {
                SortedSet<int> cell = GetCell(cellIndex);
                if (cell.Contains(elementI) && cell.Contains(elementJ))
                {
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            for (int cellIndex = 0; cellIndex < cells.Count; cellIndex++)
            {
                SortedSet<int> cell = cells[cellIndex];
                int elementIndex = 0;
                foreach (var element in cell)
                {
                    sb.Append(element);
                    if (cell.Count > 1 && elementIndex < cell.Count - 1)
                    {
                        sb.Append(',');
                    }
                    elementIndex++;
                }
                if (cells.Count > 1 && cellIndex < cells.Count - 1)
                {
                    sb.Append('|');
                }
            }
            sb.Append(']');
            return sb.ToString();
        }

        /**
         * Parse a string like "[0,2|1,3]" to form the partition; cells are
         * separated by '|' characters and elements within the cell by commas.
         *
         * @param strForm the partition in string form
         * @return the partition corresponding to the string
         * @throws ArgumentException thrown if the provided strFrom is
         *         null or empty
         */
        public static Partition FromString(string strForm)
        {

            if (strForm == null || strForm.Length == 0) throw new ArgumentException("null or empty string provided");

            Partition p = new Partition();
            int index = 0;
            if (strForm[0] == '[')
            {
                index++;
            }
            int endIndex;
            if (strForm[strForm.Length - 1] == ']')
            {
                endIndex = strForm.Length - 2;
            }
            else
            {
                endIndex = strForm.Length - 1;
            }
            int currentCell = -1;
            int numStart = -1;
            while (index <= endIndex)
            {
                char c = strForm[index];
                if (char.IsDigit(c))
                {
                    if (numStart == -1)
                    {
                        numStart = index;
                    }
                }
                else if (c == ',')
                {
                    int element = int.Parse(strForm.Substring(numStart, index - numStart));
                    if (currentCell == -1)
                    {
                        p.AddCell(element);
                        currentCell = 0;
                    }
                    else
                    {
                        p.AddToCell(currentCell, element);
                    }
                    numStart = -1;
                }
                else if (c == '|')
                {
                    int element = int.Parse(strForm.Substring(numStart, index - numStart));
                    if (currentCell == -1)
                    {
                        p.AddCell(element);
                        currentCell = 0;
                    }
                    else
                    {
                        p.AddToCell(currentCell, element);
                    }
                    currentCell++;
                    p.AddCell();
                    numStart = -1;
                }
                index++;
            }
            int lastElement = int.Parse(strForm.Substring(numStart, endIndex + 1 - numStart));
            p.AddToCell(currentCell, lastElement);
            return p;
        }
    }
}
