

// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2016-2017  Kazuya Ujihara <uzzy@users.sourceforge.net>

/* Copyright (C) 2001-2007  Edgar Luttmann <edgar@uni-paderborn.de>
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
 *  */

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NCDK.Default
{
    /// <summary>
    /// Subclass of Molecule to store Polymer specific attributes that a Polymer has.
    /// </summary>
    // @cdk.module  silent
    // @cdk.githash
    // @author      Edgar Luttmann <edgar@uni-paderborn.de>
    // @author      Martin Eklund <martin.eklund@farmbio.uu.se>
    // @cdk.created 2001-08-06
    // @cdk.keyword polymer
    [Serializable]
    public class Polymer
        : AtomContainer, IPolymer
    {
        private IDictionary<string, IMonomer> monomers;

        /// <summary>
        /// Constructs a new Polymer to store the Monomers.
        /// </summary>
        public Polymer()
        {
            monomers = new Dictionary<string, IMonomer>();
        }

        /// <summary>
        /// Adds the atom oAtom to a specified Monomer.
        /// </summary>
        /// <param name="oAtom">The atom to add</param>
        /// <param name="oMonomer">The monomer the atom belongs to</param>
        public void AddAtom(IAtom oAtom, IMonomer oMonomer)
        {
            if (!Contains(oAtom))
            {
                base.Atoms.Add(oAtom);    // this calls notify

                if (oMonomer != null)
                { // Not sure what's better here...throw nullpointer exception?
                    oMonomer.Atoms.Add(oAtom);
                    if (!monomers.ContainsKey(oMonomer.MonomerName))
                    {
                        monomers.Add(oMonomer.MonomerName, oMonomer);
                    }
                }
            }
        }

		/// <inheritdoc/>
        public virtual IEnumerable<KeyValuePair<string, IMonomer>> GetMonomerMap()
        {
            return monomers.Where(n => n.Key != "");
        }

		/// <inheritdoc/>
        public virtual IMonomer GetMonomer(string cName)
        {
            IMonomer ret;
            if (!monomers.TryGetValue(cName, out ret))
                ret = null;
            return ret;
        }

		/// <inheritdoc/>
        public virtual IEnumerable<string> GetMonomerNames()
        {
            return monomers.Keys;
        }

		/// <inheritdoc/>
        public virtual void RemoveMonomer(string name)
        {
            IMonomer monomer;
            if (monomers.TryGetValue(name, out monomer))
            {
                Remove(monomer);
                monomers.Remove(name);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Polymer(");
            sb.Append(GetHashCode()).Append(", ");
            sb.Append(base.ToString());
            sb.Append(')');
            return sb.ToString();
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone = (Polymer)base.Clone(map);
            clone.monomers = new Dictionary<string, IMonomer>();
            foreach (var monomerInfo in monomers)
            {
                string name = monomerInfo.Key;
                IMonomer original = monomerInfo.Value;
                IMonomer cloned = (IMonomer)original.Clone(map);
                clone.monomers.Add(name, cloned);
            }
            return clone;
        }
    }
}
namespace NCDK.Silent
{
    /// <summary>
    /// Subclass of Molecule to store Polymer specific attributes that a Polymer has.
    /// </summary>
    // @cdk.module  silent
    // @cdk.githash
    // @author      Edgar Luttmann <edgar@uni-paderborn.de>
    // @author      Martin Eklund <martin.eklund@farmbio.uu.se>
    // @cdk.created 2001-08-06
    // @cdk.keyword polymer
    [Serializable]
    public class Polymer
        : AtomContainer, IPolymer
    {
        private IDictionary<string, IMonomer> monomers;

        /// <summary>
        /// Constructs a new Polymer to store the Monomers.
        /// </summary>
        public Polymer()
        {
            monomers = new Dictionary<string, IMonomer>();
        }

        /// <summary>
        /// Adds the atom oAtom to a specified Monomer.
        /// </summary>
        /// <param name="oAtom">The atom to add</param>
        /// <param name="oMonomer">The monomer the atom belongs to</param>
        public void AddAtom(IAtom oAtom, IMonomer oMonomer)
        {
            if (!Contains(oAtom))
            {
                base.Atoms.Add(oAtom);    // this calls notify

                if (oMonomer != null)
                { // Not sure what's better here...throw nullpointer exception?
                    oMonomer.Atoms.Add(oAtom);
                    if (!monomers.ContainsKey(oMonomer.MonomerName))
                    {
                        monomers.Add(oMonomer.MonomerName, oMonomer);
                    }
                }
            }
        }

		/// <inheritdoc/>
        public virtual IEnumerable<KeyValuePair<string, IMonomer>> GetMonomerMap()
        {
            return monomers.Where(n => n.Key != "");
        }

		/// <inheritdoc/>
        public virtual IMonomer GetMonomer(string cName)
        {
            IMonomer ret;
            if (!monomers.TryGetValue(cName, out ret))
                ret = null;
            return ret;
        }

		/// <inheritdoc/>
        public virtual IEnumerable<string> GetMonomerNames()
        {
            return monomers.Keys;
        }

		/// <inheritdoc/>
        public virtual void RemoveMonomer(string name)
        {
            IMonomer monomer;
            if (monomers.TryGetValue(name, out monomer))
            {
                Remove(monomer);
                monomers.Remove(name);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Polymer(");
            sb.Append(GetHashCode()).Append(", ");
            sb.Append(base.ToString());
            sb.Append(')');
            return sb.ToString();
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone = (Polymer)base.Clone(map);
            clone.monomers = new Dictionary<string, IMonomer>();
            foreach (var monomerInfo in monomers)
            {
                string name = monomerInfo.Key;
                IMonomer original = monomerInfo.Value;
                IMonomer cloned = (IMonomer)original.Clone(map);
                clone.monomers.Add(name, cloned);
            }
            return clone;
        }
    }
}
