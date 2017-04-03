

// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2016-2017  Kazuya Ujihara

/* 
 * Copyright (C) 2003-2007  Christoph Steinbeck <steinbeck@users.sf.net>
 *                    2014  Mark B Vine (orcid:0000-0002-7794-0426)
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT Any WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections;

namespace NCDK.Default
{
    /// <summary>
    /// A set of AtomContainers.
    /// </summary>
    // @author        hel
    // @cdk.module    data
    // @cdk.githash
    [Serializable]
    public class AtomContainerSet<T>
        : ChemObject, IAtomContainerSet<T>, IChemObjectListener, ICloneable
         where T : IAtomContainer
    {
        internal IList<T> atomContainers;
        internal IList<double?> multipliers;

        public AtomContainerSet()
        {
            atomContainers = new ObservableChemObjectCollection<T>(this);
            multipliers = new List<double?>();
        }

        public void Add(T atomContainer)
        {
            Add(atomContainer, 1.0); // this calls notify
        }

        public bool Remove(T atomContainer)
        {
            bool ret = false;
            while (atomContainers.Contains(atomContainer))
            {
                ret = true;
                atomContainers.Remove(atomContainer);
            }
            return ret;
        }

        public void Clear()
        {
            atomContainers.Clear();
            multipliers.Clear();
        }

        public void RemoveAt(int pos)
        {
            atomContainers.RemoveAt(pos);
            multipliers.RemoveAt(pos);
        }

        public T this[int position]
        {
            get { return atomContainers[position]; }
            set { atomContainers[position] = value; }
        }

        public void SetMultiplier(int position, double? multiplier)
        {
            multipliers[position] = multiplier;
             NotifyChanged();         }

        public void SetMultiplier(T container, double? multiplier)
        {
            var index = atomContainers.IndexOf(container);
            if (index == -1)
                return;
            multipliers[index] = multiplier;
             NotifyChanged();         }

        public IReadOnlyList<double?> GetMultipliers() => new ReadOnlyCollection<double?>(multipliers);

        public bool SetMultipliers(IEnumerable<double?> multipliers)
        {
            if (multipliers.Count() == atomContainers.Count)
            {
                this.multipliers = multipliers.ToList();
                 NotifyChanged();                 return true;
            }
            return false;
        }

        public void Add(T atomContainer, double? multiplier)
        {
            atomContainers.Add(atomContainer);
            multipliers.Add(multiplier);
        }

        /// <summary>
        ///  Adds all atomContainers in the AtomContainerSet to this container.
        /// </summary>
        /// <param name="atomContainerSet">The AtomContainerSet</param>
        public virtual void Add(IAtomContainerSet<T> atomContainerSet)
        {
            foreach (var iter in atomContainerSet)
            {
                Add(iter);
            }
            
            // NotifyChanged() is called by Add()
		}

        public IEnumerator<T> GetEnumerator() => atomContainers.GetEnumerator();

        public double? GetMultiplier(int number) => multipliers[number];

        /// <summary>
        /// Returns the multiplier of the given AtomContainer.
        /// </summary>
        /// <param name="container">The AtomContainer for which the multiplier is given</param>
        /// <returns>-1, if the given molecule is not a container in this set</returns>
        public double? GetMultiplier(T container)
        {
            var index = atomContainers.IndexOf(container);
            if (index == -1)
                return -1;
            return multipliers[index];
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("AtomContainerSet(");
            sb.Append(GetHashCode());
            if (atomContainers.Count > 0)
            {
                sb.Append(", M=").Append(atomContainers.Count);
                foreach (var ac in atomContainers)
                    sb.Append(", ").Append(ac.ToString());
            }
            sb.Append(')');
            return sb.ToString();
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone = (AtomContainerSet<T>)base.Clone(map);
            clone.atomContainers = new List<T>(atomContainers.Count);
            clone.multipliers = new List<double?>(atomContainers.Count);
            for (var i = 0; i < atomContainers.Count; i++)
            {
                clone.atomContainers.Add((T)atomContainers[i].Clone(map));
                clone.multipliers.Add(multipliers[i]);
            }
            return clone;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return atomContainers.GetEnumerator();
        }

        public void AddRange(IEnumerable<T> atomContainerSet)
        {
            foreach (var ac in atomContainerSet)
                Add(ac);
        }

        public bool Contains(T atomContainer) => atomContainers.Contains(atomContainer);

        public bool IsEmpty() => atomContainers.Count == 0;

        public int Count => atomContainers.Count;

        public bool IsReadOnly => false;

        /// <summary>
        ///  Called by objects to which this object has
        ///  registered as a listener.
        /// </summary>
        /// <param name="evt">A change event pointing to the source of the change</param>
        public void OnStateChanged(ChemObjectChangeEventArgs evt)
        {
             NotifyChanged(evt);         }
        
        /// <summary>
        /// Sort the AtomContainers and multipliers using a provided Comparator.
        /// </summary>
        /// <param name="comparator">defines the sorting method</param>
        public void Sort(IComparer<T> comparator)
        {
            // need to use boxed primitives as we can't customise sorting of int primitives
            int[] indexes = new int[Count];
            for (int i = 0; i < indexes.Length; i++)
                indexes[i] = i;

            // proxy the index comparison to the atom container comparator
            Array.Sort(indexes, new CompareRefByIndex(this, comparator));

            // copy the original arrays (we could modify in place with swaps but this is cleaner)
            var containersTmp = atomContainers.ToArray();
            var multipliersTmp = multipliers.ToArray();

            // order the arrays based on the order of the indices
            for (int i = 0; i < indexes.Length; i++)
            {
                atomContainers[i] = containersTmp[indexes[i]];
                multipliers[i] = multipliersTmp[indexes[i]];
            }
        }

        public int IndexOf(T item) => atomContainers.IndexOf(item);

        public void Insert(int index, T item) => atomContainers.Insert(index, item);

        public void CopyTo(T[] array, int arrayIndex)
            => atomContainers.CopyTo(array, arrayIndex);

        class CompareRefByIndex : IComparer<int>
        {
            AtomContainerSet<T> parent;
            IComparer<T> comparator;

            public CompareRefByIndex(AtomContainerSet<T> parent, IComparer<T> comparator)
            {
                this.parent = parent;
                this.comparator = comparator;
            }

            public int Compare(int o1, int o2)
            {
                return comparator.Compare(parent[o1], parent[o2]);
            }
        }
    }
}
namespace NCDK.Silent
{
    /// <summary>
    /// A set of AtomContainers.
    /// </summary>
    // @author        hel
    // @cdk.module    data
    // @cdk.githash
    [Serializable]
    public class AtomContainerSet<T>
        : ChemObject, IAtomContainerSet<T>, IChemObjectListener, ICloneable
         where T : IAtomContainer
    {
        internal IList<T> atomContainers;
        internal IList<double?> multipliers;

        public AtomContainerSet()
        {
            atomContainers = new ObservableChemObjectCollection<T>(this);
            multipliers = new List<double?>();
        }

        public void Add(T atomContainer)
        {
            Add(atomContainer, 1.0); // this calls notify
        }

        public bool Remove(T atomContainer)
        {
            bool ret = false;
            while (atomContainers.Contains(atomContainer))
            {
                ret = true;
                atomContainers.Remove(atomContainer);
            }
            return ret;
        }

        public void Clear()
        {
            atomContainers.Clear();
            multipliers.Clear();
        }

        public void RemoveAt(int pos)
        {
            atomContainers.RemoveAt(pos);
            multipliers.RemoveAt(pos);
        }

        public T this[int position]
        {
            get { return atomContainers[position]; }
            set { atomContainers[position] = value; }
        }

        public void SetMultiplier(int position, double? multiplier)
        {
            multipliers[position] = multiplier;
                    }

        public void SetMultiplier(T container, double? multiplier)
        {
            var index = atomContainers.IndexOf(container);
            if (index == -1)
                return;
            multipliers[index] = multiplier;
                    }

        public IReadOnlyList<double?> GetMultipliers() => new ReadOnlyCollection<double?>(multipliers);

        public bool SetMultipliers(IEnumerable<double?> multipliers)
        {
            if (multipliers.Count() == atomContainers.Count)
            {
                this.multipliers = multipliers.ToList();
                                return true;
            }
            return false;
        }

        public void Add(T atomContainer, double? multiplier)
        {
            atomContainers.Add(atomContainer);
            multipliers.Add(multiplier);
        }

        /// <summary>
        ///  Adds all atomContainers in the AtomContainerSet to this container.
        /// </summary>
        /// <param name="atomContainerSet">The AtomContainerSet</param>
        public virtual void Add(IAtomContainerSet<T> atomContainerSet)
        {
            foreach (var iter in atomContainerSet)
            {
                Add(iter);
            }
            
            // NotifyChanged() is called by Add()
		}

        public IEnumerator<T> GetEnumerator() => atomContainers.GetEnumerator();

        public double? GetMultiplier(int number) => multipliers[number];

        /// <summary>
        /// Returns the multiplier of the given AtomContainer.
        /// </summary>
        /// <param name="container">The AtomContainer for which the multiplier is given</param>
        /// <returns>-1, if the given molecule is not a container in this set</returns>
        public double? GetMultiplier(T container)
        {
            var index = atomContainers.IndexOf(container);
            if (index == -1)
                return -1;
            return multipliers[index];
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("AtomContainerSet(");
            sb.Append(GetHashCode());
            if (atomContainers.Count > 0)
            {
                sb.Append(", M=").Append(atomContainers.Count);
                foreach (var ac in atomContainers)
                    sb.Append(", ").Append(ac.ToString());
            }
            sb.Append(')');
            return sb.ToString();
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone = (AtomContainerSet<T>)base.Clone(map);
            clone.atomContainers = new List<T>(atomContainers.Count);
            clone.multipliers = new List<double?>(atomContainers.Count);
            for (var i = 0; i < atomContainers.Count; i++)
            {
                clone.atomContainers.Add((T)atomContainers[i].Clone(map));
                clone.multipliers.Add(multipliers[i]);
            }
            return clone;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return atomContainers.GetEnumerator();
        }

        public void AddRange(IEnumerable<T> atomContainerSet)
        {
            foreach (var ac in atomContainerSet)
                Add(ac);
        }

        public bool Contains(T atomContainer) => atomContainers.Contains(atomContainer);

        public bool IsEmpty() => atomContainers.Count == 0;

        public int Count => atomContainers.Count;

        public bool IsReadOnly => false;

        /// <summary>
        ///  Called by objects to which this object has
        ///  registered as a listener.
        /// </summary>
        /// <param name="evt">A change event pointing to the source of the change</param>
        public void OnStateChanged(ChemObjectChangeEventArgs evt)
        {
                    }
        
        /// <summary>
        /// Sort the AtomContainers and multipliers using a provided Comparator.
        /// </summary>
        /// <param name="comparator">defines the sorting method</param>
        public void Sort(IComparer<T> comparator)
        {
            // need to use boxed primitives as we can't customise sorting of int primitives
            int[] indexes = new int[Count];
            for (int i = 0; i < indexes.Length; i++)
                indexes[i] = i;

            // proxy the index comparison to the atom container comparator
            Array.Sort(indexes, new CompareRefByIndex(this, comparator));

            // copy the original arrays (we could modify in place with swaps but this is cleaner)
            var containersTmp = atomContainers.ToArray();
            var multipliersTmp = multipliers.ToArray();

            // order the arrays based on the order of the indices
            for (int i = 0; i < indexes.Length; i++)
            {
                atomContainers[i] = containersTmp[indexes[i]];
                multipliers[i] = multipliersTmp[indexes[i]];
            }
        }

        public int IndexOf(T item) => atomContainers.IndexOf(item);

        public void Insert(int index, T item) => atomContainers.Insert(index, item);

        public void CopyTo(T[] array, int arrayIndex)
            => atomContainers.CopyTo(array, arrayIndex);

        class CompareRefByIndex : IComparer<int>
        {
            AtomContainerSet<T> parent;
            IComparer<T> comparator;

            public CompareRefByIndex(AtomContainerSet<T> parent, IComparer<T> comparator)
            {
                this.parent = parent;
                this.comparator = comparator;
            }

            public int Compare(int o1, int o2)
            {
                return comparator.Compare(parent[o1], parent[o2]);
            }
        }
    }
}
