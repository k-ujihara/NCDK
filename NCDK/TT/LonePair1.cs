















// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2015-2016  Kazuya Ujihara

/* Copyright (C) 2002-2007  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */

using System;
using System.Text;

namespace NCDK.Default
{
    /**
     * A LonePair is an orbital primarily located with one Atom, containing
     * two electrons.
     *
     * @cdk.module data
     * @cdk.githash
     *
     * @cdk.keyword orbital
     * @cdk.keyword lone-pair
     */
    [Serializable]
    public class LonePair
        : ElectronContainer, ILonePair, ICloneable
    {
        /// <summary>Number of electrons in the lone pair.</summary>
        protected readonly int electronCount = 2;

        /// <summary>The atom with which this lone pair is associated.</summary>
        protected IAtom atom;

        /**
         * Constructs an unconnected lone pair.
         *
         */
        public LonePair()
        {
            this.atom = null;
        }

        /**
         * Constructs an lone pair on an Atom.
         *
         * @param atom  Atom to which this lone pair is connected
         */
        public LonePair(IAtom atom)
        {
            this.atom = atom;
        }

        /**
         * The number of electrons in a LonePair.
         */
        public override int? ElectronCount
        {
            get { return this.electronCount; }
        }

        /**
         * The associated Atom.
         */
        public IAtom Atom
        {
            get { return atom; }
            set
            {
                atom = value;
                 NotifyChanged();             }
        }

        /**
         * Returns true if the given atom participates in this lone pair.
         *
         * @param   atom  The atom to be tested if it participates in this bond
         * @return     true if this lone pair is associated with the atom
         */
        public bool Contains(IAtom atom)
        {
            return (this.atom == atom);
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone = (LonePair)base.Clone(map);
            clone.atom = (IAtom)atom?.Clone(map);
            return clone;
        }

        /**
         * Returns a one line string representation of this LonePair.
         * This method is conform RFC #9.
         *
         * @return    The string representation of this LonePair
         */
        public override string ToString()
        {
            StringBuilder resultString = new StringBuilder();
            resultString.Append("LonePair(");
            resultString.Append(this.GetHashCode());
            if (atom != null)
            {
                resultString.Append(", ").Append(atom.ToString());
            }
            resultString.Append(')');
            return resultString.ToString();
        }
    }
}

namespace NCDK.Silent
{
    /**
     * A LonePair is an orbital primarily located with one Atom, containing
     * two electrons.
     *
     * @cdk.module data
     * @cdk.githash
     *
     * @cdk.keyword orbital
     * @cdk.keyword lone-pair
     */
    [Serializable]
    public class LonePair
        : ElectronContainer, ILonePair, ICloneable
    {
        /// <summary>Number of electrons in the lone pair.</summary>
        protected readonly int electronCount = 2;

        /// <summary>The atom with which this lone pair is associated.</summary>
        protected IAtom atom;

        /**
         * Constructs an unconnected lone pair.
         *
         */
        public LonePair()
        {
            this.atom = null;
        }

        /**
         * Constructs an lone pair on an Atom.
         *
         * @param atom  Atom to which this lone pair is connected
         */
        public LonePair(IAtom atom)
        {
            this.atom = atom;
        }

        /**
         * The number of electrons in a LonePair.
         */
        public override int? ElectronCount
        {
            get { return this.electronCount; }
        }

        /**
         * The associated Atom.
         */
        public IAtom Atom
        {
            get { return atom; }
            set
            {
                atom = value;
                            }
        }

        /**
         * Returns true if the given atom participates in this lone pair.
         *
         * @param   atom  The atom to be tested if it participates in this bond
         * @return     true if this lone pair is associated with the atom
         */
        public bool Contains(IAtom atom)
        {
            return (this.atom == atom);
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone = (LonePair)base.Clone(map);
            clone.atom = (IAtom)atom?.Clone(map);
            return clone;
        }

        /**
         * Returns a one line string representation of this LonePair.
         * This method is conform RFC #9.
         *
         * @return    The string representation of this LonePair
         */
        public override string ToString()
        {
            StringBuilder resultString = new StringBuilder();
            resultString.Append("LonePair(");
            resultString.Append(this.GetHashCode());
            if (atom != null)
            {
                resultString.Append(", ").Append(atom.ToString());
            }
            resultString.Append(')');
            return resultString.ToString();
        }
    }
}

