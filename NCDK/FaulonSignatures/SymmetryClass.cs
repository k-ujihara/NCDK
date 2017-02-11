using System;
using System.Collections;
using System.Collections.Generic;

namespace FaulonSignatures
{
    /// <summary>
    /// A collection of vertex indices with the same canonical signature string.
    /// </summary>
    // @author maclean
    public class SymmetryClass : IComparable<SymmetryClass>, IEnumerable<int>
    {
        /// <summary>
        /// The signature string that the vertices all share
        /// </summary>
        private readonly string signatureString;

        /// <summary>
        /// The set of vertex indices that have this signature string
        /// </summary>
        private readonly SortedSet<int> vertexIndices;

        /// <summary>
        /// Make a symmetry class for the signature string 
        /// <code>signatureString</code>.
        /// <param name="signatureString">the signature string for this symmetry class</param>
        /// </summary>
        public SymmetryClass(string signatureString)
        {
            this.signatureString = signatureString;
            this.vertexIndices = new SortedSet<int>();
        }

        public IEnumerator<int> GetEnumerator()
        {
            return this.vertexIndices.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int Count => vertexIndices.Count;

        public string GetSignatureString()
        {
            return this.signatureString;
        }

        /// <summary>
        /// Check that the symmetry class' string is the same as the supplied string.
        /// </summary>
        /// <param name="otherSignatureString">the string to check</param>
        /// <returns>true if the strings are equal</returns>
        public bool HasSignature(string otherSignatureString)
        {
            return this.signatureString.Equals(otherSignatureString);
        }

        /// <summary>
        /// Add a vertex index to the list.
        /// 
        /// <param name="vertexIndex">the vertex index to add</param>
        /// </summary>
        public void AddIndex(int vertexIndex)
        {
            this.vertexIndices.Add(vertexIndex);
        }

        /// <summary>
        /// If the vertex indexed by <code>vertexIndex</code> is in the symmetry 
        /// class then return the smaller of it and the lowest element. If it is not
        /// in the symmetry class, return -1.
        /// 
        /// <param name="vertexIndex">/// @return</param>
        /// </summary>
        public int GetMinimal(int vertexIndex, List<int> used)
        {
            int min = -1;
            foreach (var classIndex in this.vertexIndices)
            {
                if (classIndex == vertexIndex)
                {
                    if (min == -1)
                    {
                        return vertexIndex;
                    }
                    else
                    {
                        return min;
                    }
                }
                else
                {
                    if (used.Contains(classIndex))
                    {
                        continue;
                    }
                    else
                    {
                        min = classIndex;
                    }
                }
            }

            // the vertexIndex is not in the symmetry class
            return -1;
        }

        /// <seealso cref="IComparable.CompareTo(object)"/>
        public int CompareTo(SymmetryClass o)
        {
            return this.signatureString.CompareTo(o.signatureString);
        }

        public override string ToString()
        {
            return this.signatureString + " " + this.vertexIndices;
        }
    }
}
