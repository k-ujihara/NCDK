/*
 * Copyright (c) 2013 John May <jwmay@users.sf.net>
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
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Hash
{
    /// <summary>
    /// Enumeration of atom encoders for seeding atomic hash codes. Generally these
    /// encoders return the direct value or a prime number if that value is null.
    /// These encoders are considered <i>basic</i> as the values generated are all in
    /// the same range. Better encoding can be achieved by assigning discrete values
    /// a section of the prime number table. However, In practice using a
    /// pseudorandom number generator to distribute the encoded values provides a
    /// good distribution.
    ///
    // @author John May
    // @cdk.module hash
    /// <seealso cref="ConjugatedAtomEncoder"/>
    // @see <a href="http://www.bigprimes.net/archive/prime/">Prime numbers
    ///      archive</a>
    // @cdk.githash
    /// </summary>
    public sealed class BasicAtomEncoder : AtomEncoder, IComparable<BasicAtomEncoder>
    {
        private delegate int EncodeDelegate(IAtom atom, IAtomContainer container);
        private EncodeDelegate OnEncode { get; set; }
        public int Ordinal { get; private set; }
        public static BasicAtomEncoder[] Values { get; set; } 

        private BasicAtomEncoder(EncodeDelegate setter)
        {
            this.OnEncode = setter;
        }

        public int Encode(IAtom atom, IAtomContainer container)
        {
            return OnEncode(atom, container);
        }

        public int CompareTo(BasicAtomEncoder other)
        {
            return Ordinal.CompareTo(other.Ordinal);
        }

        /// <summary>
        /// Encode the atomic number of an atom.
        /// </summary>
        public static readonly BasicAtomEncoder ATOMIC_NUMBER =
            new BasicAtomEncoder((atom, container) => atom.AtomicNumber ?? 32451169);

        /// <summary>
        /// Encode the mass number of an atom, allowing distinction of isotopes.
        /// </summary>
        public static readonly BasicAtomEncoder MASS_NUMBER =
            new BasicAtomEncoder((atom, container) => atom.MassNumber ?? 32451179);

        /// <summary>
        /// Encode the formal charge of an atom, allowing distinction of different
        /// protonation states.
        /// </summary>
        public static readonly BasicAtomEncoder FORMAL_CHARGE =
            new BasicAtomEncoder((atom, container) => atom.FormalCharge ?? 32451193);

        /// <summary>
        /// Encode the number of explicitly connected atoms (degree).
        /// </summary>
        public static readonly BasicAtomEncoder N_CONNECTED_ATOMS =
            new BasicAtomEncoder((atom, container) => container.GetConnectedAtoms(atom).Count());

        /// <summary>
        /// Encode the explicit bond order sum of an atom.
        /// </summary>
        public static readonly BasicAtomEncoder BOND_ORDER_SUM =
            new BasicAtomEncoder((atom, container) => container.GetBondOrderSum(atom).GetHashCode()); // Fixed CDK's bug?? HashCode() removed.

        /// <summary>
        /// Encode the orbital hybridization of an atom.
        /// </summary>
        public static readonly BasicAtomEncoder ORBITAL_HYBRIDIZATION =
            new BasicAtomEncoder((atom, container) =>
            {
                var hybridization = atom.Hybridization;
                return !hybridization.IsUnset ? (int)hybridization.Ordinal : 32451301;
            });

        public static readonly BasicAtomEncoder FREE_RADICALS =
            new BasicAtomEncoder((atom, container) => container.GetConnectedSingleElectrons(atom).Count());

        public const int Ordinal_ATOMIC_NUMBER = 0;
        public const int Ordinal_MASS_NUMBER = 1;
        public const int Ordinal_FORMAL_CHARGE = 2;
        public const int Ordinal_N_CONNECTED_ATOMS = 3;
        public const int Ordinal_BOND_ORDER_SUM = 4;
        public const int Ordinal_ORBITAL_HYBRIDIZATION = 5;
        public const int Ordinal_FREE_RADICALS = 6;

        static BasicAtomEncoder()
        {
            ATOMIC_NUMBER.Ordinal = Ordinal_ATOMIC_NUMBER;
            MASS_NUMBER.Ordinal = Ordinal_MASS_NUMBER;
            FORMAL_CHARGE.Ordinal = Ordinal_FORMAL_CHARGE;
            N_CONNECTED_ATOMS.Ordinal = Ordinal_N_CONNECTED_ATOMS;
            BOND_ORDER_SUM.Ordinal = Ordinal_BOND_ORDER_SUM;
            ORBITAL_HYBRIDIZATION.Ordinal = Ordinal_ORBITAL_HYBRIDIZATION;
            FREE_RADICALS.Ordinal = Ordinal_FREE_RADICALS;
            var list = new List<BasicAtomEncoder>();
            list.Add(ATOMIC_NUMBER);
            list.Add(MASS_NUMBER);
            list.Add(N_CONNECTED_ATOMS);
            list.Add(BOND_ORDER_SUM);
            list.Add(ORBITAL_HYBRIDIZATION);
            list.Add(FREE_RADICALS);
            Values = list.ToArray();
        }
    }
}
