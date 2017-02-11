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
using NCDK.Common.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace NCDK.Signature {
    /// <summary>
    /// A list of atom indices, and the label of the orbit.
    /// </summary>
    // @cdk.module signature
    // @author maclean
    // @cdk.githash
    public class Orbit : IEnumerable<int>, ICloneable
    {
        /// <summary>
        /// The atom indices in this orbit
        /// </summary>
        private List<int> atomIndices;

        /// <summary>
        /// The label that all the atoms in the orbit share
        /// </summary>
        private string label;

		 /// <summary>
         /// The maximum height of the signature string
         /// </summary>
        private int height;

        public Orbit(string label, int height)
        {
            this.label = label;
            this.atomIndices = new List<int>();
            this.height = height;
        }

        public IEnumerator<int> GetEnumerator()
        {
            return this.atomIndices.GetEnumerator();
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public object Clone()
        {
            Orbit orbit = new Orbit(this.label, this.height);
            foreach (var i in this.atomIndices)
            {
                orbit.atomIndices.Add(i);
            }
            return orbit;
        }

        /// <summary>
        /// Sorts the atom indices in this orbit.
        /// </summary>
        public void Sort()
        {
            // TODO : change the list to a sorted set?
            this.atomIndices.Sort();
        }

        /// <summary>
        /// The height of the signature of this orbit.
        /// </summary>
        public int Height => this.height;

        /// <summary>
        /// All the atom indices as a list.
        /// </summary>
        public List<int> AtomIndices => this.atomIndices;

        /// <summary>
        /// Adds an atom index to the orbit.
        /// </summary>
        /// <param name="atomIndex">the atom index</param>
        public void AddAtomAt(int atomIndex)
        {
            this.atomIndices.Add(atomIndex);
        }

        /// <summary>
        /// Checks to see if the orbit has this string as a label.
        /// </summary>
        /// <param name="otherLabel">the label to compare with</param>
        /// <returns> if it has this label</returns>
        public bool HasLabel(string otherLabel)
        {
            return this.label.Equals(otherLabel);
        }

        /// <summary>
        /// Checks to see if the orbit is empty.
        /// </summary>
        /// <returns><see langword="true"/> if there are no atom indices in the orbit</returns>
        public bool IsEmpty()
        {
            return !this.atomIndices.Any();
        }

        /// <summary>
        /// The first atom index of the orbit.
        /// </summary>
        public int FirstAtom => this.atomIndices[0];

        /// <summary>
        /// Removes an atom index from the orbit.
        /// </summary>
        /// <param name="atomIndex">the atom index to remove</param>
        public void Remove(int atomIndex)
        {
            this.atomIndices.RemoveAt(this.atomIndices.IndexOf(atomIndex));
        }

        /// <summary>
        /// The label of the orbit.
        /// </summary>
        public string Label => this.label;

        /// <summary>
        /// Checks to see if the orbit contains this atom index.
        /// </summary>
        /// <param name="atomIndex">the atom index to look for</param>
        /// <returns><see langword="true"/> if the orbit contains this atom index</returns>
        public bool Contains(int atomIndex)
        {
            return this.atomIndices.Contains(atomIndex);
        }

        public override string ToString()
        {
            return label + " " + Arrays.DeepToString(atomIndices.ToArray());
        }
    }
}
