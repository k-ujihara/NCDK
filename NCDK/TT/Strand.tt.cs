


// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2016-2017  Kazuya Ujihara <ujihara.kazuya@gmail.com>

/* Copyright (C) 2004-2007  Martin Eklund <martin.eklund@farmbio.uu.se>
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace NCDK.Default
{
    /// <summary>
    /// A Strand is an AtomContainer which stores additional strand specific
    /// informations for a group of Atoms.
    /// </summary>
    // @cdk.module  data
    // @cdk.githash
    // @cdk.created 2004-12-20
    // @author      Martin Eklund <martin.eklund@farmbio.uu.se>
    // @author      Ola Spjuth <ola.spjuth@farmbio.uu.se>
    [Serializable]
    public class Strand 
        : AtomContainer, IStrand
    {
        private IDictionary<string, IMonomer> monomers = new Dictionary<string, IMonomer>();

        /// <summary>
        /// The strand name.
        /// </summary>
        public string StrandName { get; set; } = "";

        /// <summary>
        ///  The strand type.
        /// </summary>
        public string StrandType { get; set; }

        public Strand()
            : base()
        {
            Monomer oMonomer = new Monomer();
            oMonomer.MonomerName = "";
            oMonomer.MonomerType = "Unknown";
            monomers.Add("", oMonomer);
        }

        /// <summary>
        /// Adds the atom oAtom without specifying a Monomer or a Strand. Therefore the
        /// atom gets added to a Monomer of type "Unknown" in a Strand of type  "Unknown".
        /// </summary>
        /// <param name="oAtom">The atom to add</param>
        public virtual void AddAtom(IAtom oAtom)
        {
            AddAtom(oAtom, GetMonomer(""));
        }

        /// <summary>
        /// Adds the atom oAtom to a specific Monomer.
        /// </summary>
        /// <param name="oAtom">The atom to add</param>
        /// <param name="oMonomer">The monomer the atom belongs to</param>
        public virtual void AddAtom(IAtom oAtom, IMonomer oMonomer)
        {
            int atomCount = base.Atoms.Count;
            Atoms.Add(oAtom);
            if (atomCount != base.Atoms.Count)
            {
                if (oMonomer == null)
                    oMonomer = GetMonomer("");

                oMonomer.Atoms.Add(oAtom);
                if (!monomers.ContainsKey(oMonomer.MonomerName))
                    monomers.Add(oMonomer.MonomerName, oMonomer);
            }
        }

        private class  ReadOnlyNonEmptyDictionary<T>
            : IReadOnlyDictionary<string, T>
        {
            IDictionary<string, T> dictionary;

            public ReadOnlyNonEmptyDictionary(IDictionary<string, T> dictionary)
            {
                this.dictionary = dictionary;
            }

            public T this[string key] => dictionary[key];
            public int Count => dictionary.Count - 1;
            public IEnumerable<string> Keys => dictionary.Keys;
            public IEnumerable<T> Values => dictionary.Values;
            public bool ContainsKey(string key) => dictionary.ContainsKey(key);
            public IEnumerator<KeyValuePair<string, T>> GetEnumerator() 
            {
                foreach (var pair in dictionary)
                    if (pair.Key != "")
                        yield return pair;
                yield break;
            }

            public bool TryGetValue(string key, out T value) => TryGetValue(key, out value);
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public IReadOnlyDictionary<string, IMonomer> GetMonomerMap()
        {
            return new ReadOnlyNonEmptyDictionary<IMonomer>(monomers);
        }

        public IMonomer GetMonomer(string cName)
        {
            IMonomer monomer;
            if (monomers.TryGetValue(cName, out monomer))
                return monomer;
            return null;
        }

        public IEnumerable<string> GetMonomerNames()
        {
            return monomers.Keys;
        }

        public void RemoveMonomer(string name)
        {
            if (monomers.ContainsKey(name))
            {
                var monomer = monomers[name];
                Remove(monomer);
                monomers.Remove(name);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder(32);
            sb.Append("Strand(");
            sb.Append(GetHashCode());
            if (StrandName != null) 
                sb.Append(", N:").Append(StrandName);
            if (StrandType != null) 
                sb.Append(", T:").Append(StrandType).Append(", ");
            sb.Append(base.ToString());
            sb.Append(')');
            return sb.ToString();
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone = (Strand)base.Clone(map);
            clone.monomers = new Dictionary<string, IMonomer>();
            foreach (var pair in monomers)
            {
                string monomerName = pair.Key;
                IMonomer monomer = pair.Value;
                var clonedMonomer = (IMonomer)monomer.Clone(map);
                clone.monomers.Add(monomerName, clonedMonomer);
            }
            return clone;
        }
    }
}
namespace NCDK.Silent
{
    /// <summary>
    /// A Strand is an AtomContainer which stores additional strand specific
    /// informations for a group of Atoms.
    /// </summary>
    // @cdk.module  data
    // @cdk.githash
    // @cdk.created 2004-12-20
    // @author      Martin Eklund <martin.eklund@farmbio.uu.se>
    // @author      Ola Spjuth <ola.spjuth@farmbio.uu.se>
    [Serializable]
    public class Strand 
        : AtomContainer, IStrand
    {
        private IDictionary<string, IMonomer> monomers = new Dictionary<string, IMonomer>();

        /// <summary>
        /// The strand name.
        /// </summary>
        public string StrandName { get; set; } = "";

        /// <summary>
        ///  The strand type.
        /// </summary>
        public string StrandType { get; set; }

        public Strand()
            : base()
        {
            Monomer oMonomer = new Monomer();
            oMonomer.MonomerName = "";
            oMonomer.MonomerType = "Unknown";
            monomers.Add("", oMonomer);
        }

        /// <summary>
        /// Adds the atom oAtom without specifying a Monomer or a Strand. Therefore the
        /// atom gets added to a Monomer of type "Unknown" in a Strand of type  "Unknown".
        /// </summary>
        /// <param name="oAtom">The atom to add</param>
        public virtual void AddAtom(IAtom oAtom)
        {
            AddAtom(oAtom, GetMonomer(""));
        }

        /// <summary>
        /// Adds the atom oAtom to a specific Monomer.
        /// </summary>
        /// <param name="oAtom">The atom to add</param>
        /// <param name="oMonomer">The monomer the atom belongs to</param>
        public virtual void AddAtom(IAtom oAtom, IMonomer oMonomer)
        {
            int atomCount = base.Atoms.Count;
            Atoms.Add(oAtom);
            if (atomCount != base.Atoms.Count)
            {
                if (oMonomer == null)
                    oMonomer = GetMonomer("");

                oMonomer.Atoms.Add(oAtom);
                if (!monomers.ContainsKey(oMonomer.MonomerName))
                    monomers.Add(oMonomer.MonomerName, oMonomer);
            }
        }

        private class  ReadOnlyNonEmptyDictionary<T>
            : IReadOnlyDictionary<string, T>
        {
            IDictionary<string, T> dictionary;

            public ReadOnlyNonEmptyDictionary(IDictionary<string, T> dictionary)
            {
                this.dictionary = dictionary;
            }

            public T this[string key] => dictionary[key];
            public int Count => dictionary.Count - 1;
            public IEnumerable<string> Keys => dictionary.Keys;
            public IEnumerable<T> Values => dictionary.Values;
            public bool ContainsKey(string key) => dictionary.ContainsKey(key);
            public IEnumerator<KeyValuePair<string, T>> GetEnumerator() 
            {
                foreach (var pair in dictionary)
                    if (pair.Key != "")
                        yield return pair;
                yield break;
            }

            public bool TryGetValue(string key, out T value) => TryGetValue(key, out value);
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public IReadOnlyDictionary<string, IMonomer> GetMonomerMap()
        {
            return new ReadOnlyNonEmptyDictionary<IMonomer>(monomers);
        }

        public IMonomer GetMonomer(string cName)
        {
            IMonomer monomer;
            if (monomers.TryGetValue(cName, out monomer))
                return monomer;
            return null;
        }

        public IEnumerable<string> GetMonomerNames()
        {
            return monomers.Keys;
        }

        public void RemoveMonomer(string name)
        {
            if (monomers.ContainsKey(name))
            {
                var monomer = monomers[name];
                Remove(monomer);
                monomers.Remove(name);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder(32);
            sb.Append("Strand(");
            sb.Append(GetHashCode());
            if (StrandName != null) 
                sb.Append(", N:").Append(StrandName);
            if (StrandType != null) 
                sb.Append(", T:").Append(StrandType).Append(", ");
            sb.Append(base.ToString());
            sb.Append(')');
            return sb.ToString();
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone = (Strand)base.Clone(map);
            clone.monomers = new Dictionary<string, IMonomer>();
            foreach (var pair in monomers)
            {
                string monomerName = pair.Key;
                IMonomer monomer = pair.Value;
                var clonedMonomer = (IMonomer)monomer.Clone(map);
                clone.monomers.Add(monomerName, clonedMonomer);
            }
            return clone;
        }
    }
}
