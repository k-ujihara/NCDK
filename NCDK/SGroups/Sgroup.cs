/*
 * Copyright (c) 2015 John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using System;
using System.Collections.Generic;

namespace NCDK.SGroups
{
    /**
     * Generic CTab Sgroup (substructure group) that stores all other types of group. This representation
     * is allows reading from CTfiles (e.g. Molfile, SDfile).
     * <p/>
     * The class uses a key-value store for Sgroup attributes simplifying both input and output.
     */
    public class Sgroup
    {
        /// <summary>
        /// the atoms of this substructure group.
        /// </summary>
        public ICollection<IAtom> Atoms { get; private set; }

        /// <summary>
        /// Access the bonds that belong to this substructure group.
        /// For data Sgroups, the bonds are the containment bonds,
        /// for all other <see cref="Sgroup"/> types, they are crossing bonds.
        /// </summary>
        public ICollection<IBond> Bonds { get; private set; }

        /// <summary>
        /// the parents of this Sgroup.
        /// </summary>
        public ICollection<Sgroup> Parents { get; private set; }

        private readonly IDictionary<SgroupKey, object> attributes = new SortedDictionary<SgroupKey, object>();

        /**
		 * Create a new generic Sgroup.
		 */
        public Sgroup()
        {
            Atoms = new HashSet<IAtom>();
            Bonds = new HashSet<IBond>();
            Parents = new HashSet<Sgroup>();
            Type = SgroupType.CtabGeneric;
        }

        /**
		 * Copy constructor.
		 *
		 * @param org original Sgroup instance
		 */
        Sgroup(Sgroup org)
        {
            Atoms = new HashSet<IAtom>(org.Atoms);
            Bonds = new HashSet<IBond>(org.Bonds);
            Parents = new HashSet<Sgroup>(org.Parents);
            this.attributes = new Dictionary<SgroupKey, object>(org.attributes);
        }

        /**
		 * Access all the attribute keys of this Sgroup.
		 *
		 * @return attribute keys
		 */
        public ICollection<SgroupKey> AttributeKeys => attributes.Keys;

        /**
		 * The type of the Sgroup.
		 */
        public SgroupType Type
        {
            set
            {
                PutValue(SgroupKey.CtabType, value);
            }

            get
            {
                return (SgroupType)GetValue(SgroupKey.CtabType);
            }
        }

        /**
         * Add a bond to this Sgroup.
         *
         * @param atom the atom
         */
        public void Add(IAtom atom)
        {
            Atoms.Add(atom);
        }

        /**
         * Add a bond to this Sgroup. The bond list
         *
         * @param bond bond to add
         */
        public void Add(IBond bond)
        {
            Bonds.Add(bond);
        }

        /**
         * Add a parent Sgroup.
         *
         * @param parent parent sgroup
         */
        public void AddParent(Sgroup parent)
        {
            Parents.Add(parent);
        }

        /**
         * Remove the specified parent associations from this Sgroup.
         *
         * @param parents parent associations
         */
        public void RemoveParents(IEnumerable<Sgroup> parents)
        {
            foreach (var p in parents)
                Parents.Remove(p);
        }

        /**
         * Store an attribute for the Sgroup.
         *
         * @param key attribute key
         * @param val attribute value
         */
        public void PutValue(SgroupKey key, object val)
        {
            attributes[key] = val;
        }

        /**
         * Access an attribute for the Sgroup.
         *
         * @param key attribute key
         */
        public object GetValue(SgroupKey key)
        {
            object o;
            if (!attributes.TryGetValue(key, out o))
                return null;
            return o;
        }

        /**
         * Access the subscript value.
         *
         * @return subscript value (or null if not present)
         */
        public string Subscript
        {
            get { return (string)GetValue(SgroupKey.CtabSubScript); }
            set { PutValue(SgroupKey.CtabSubScript, value); }
        }

        /**
		 * Add a bracket for this Sgroup.
		 *
		 * @param bracket sgroup bracket
		 */
        public void AddBracket(SgroupBracket bracket)
        {
            IList<SgroupBracket> brackets = (IList<SgroupBracket>)GetValue(SgroupKey.CtabBracket);
            if (brackets == null)
            {
                PutValue(SgroupKey.CtabBracket,
                         brackets = new List<SgroupBracket>(2));
            }
            brackets.Add(bracket);
        }

        /**
         * Downcast this, maybe generic, Sgroup to a specific concrete implementation. This
         * method should be called on load by a reader once all data has been added to the sgroup.
         *
         * @param <T> return type
         * @return downcast instance
         */
        public T Downcast<T>() where T : Sgroup
        {
            // ToDo - Implement
            return (T)this;
        }
    }
}
