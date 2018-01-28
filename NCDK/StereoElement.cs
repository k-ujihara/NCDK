/* Copyright (C) 2006-2007  Egon Willighagen <egonw@users.sf.net>
 *                             2017  John Mayfield
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
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

using System.Reflection;
using static NCDK.StereoElement;

namespace NCDK
{
    public struct StereoElement
    {
        /// <summary>
        /// The configuration is stored as an integral value. Although the common
        /// geometries like tetrahedral and cis/trans bonds only have 2 possible
        /// configurations (e.g. left vs right) more complex geometries like square
        /// planar and octahedral require more to describe. For convenience the
        /// constants <see cref="Left"/> and <see cref="Right"/> are provided but are synonymous
        /// with the values <c>1</c> (odd) and <c>2</c> (even).
        /// </summary>
        /// <remarks>
        /// Special values (e.g. <c>0</c>) may be used to represent unknown/unspecified or
        /// racemic in future but are currently undefined.
        /// </remarks>
        [Obfuscation(ApplyToMembers = true, Exclude = true)]
        public enum Configuration
            : short
        {
            Unset = 0,
            Left = 1,
            Right = 2,

            Opposite = Left,
            Together = Right,
        }

        /// <summary>
        /// There stereo class defines the type of stereochemistry/geometry that is
        /// captured. The stereo class is also defined as a integral value. The
        /// following classes are available with varied support through out the
        /// toolkit.        
        /// </summary>
        /// <seealso cref="Configuration"/>
        [Obfuscation(ApplyToMembers = true, Exclude = true)]
        public enum Classes
            : short
        {
            Unset = 0,

            /// <summary>Geometric CisTrans (e.g. but-2-ene)</summary>
            CisTrans = 0x21,

            /// <summary>Tetrahedral (T-4) (e.g. butan-2-ol)</summary>
            Tetrahedral = 0x42,

            /// <summary>ExtendedTetrahedral (e.g. 2,3-pentadiene)</summary>
            Allenal = 0x43,

            /// <summary>Atropisomeric (e.g. BiNAP)</summary>
            Atropisomeric = 0x44,

            /// <summary>Square Planar (SP-4) (e.g. cisplatin)</summary>
            SquarePlanar = 0x45,

            /// <summary>Square Pyramidal (SPY-5)</summary>
            SquarePyramidal = 0x51,

            /// <summary>Trigonal Bipyramidal (TBPY-5)</summary>
            TrigonalBipyramidal = 0x52,

            /// <summary>Octahedral (OC-6)</summary>
            Octahedral = 0x61,

            /// <summary>Pentagonal Bipyramidal (PBPY-7)</summary>
            PentagonalBipyramidal = 0x71,

            /// <summary>Hexagonal Bipyramidal (HBPY-8)</summary>
            HexagonalBipyramidal = 0x81,

            /// <summary>Heptagonal Bipyramidal (HBPY-9)</summary>
            HeptagonalBipyramidal = 0x91,
        }

        Classes cls;
        Configuration configure;

        public Classes Class => cls;

        public Configuration Configure
        {
            get { return configure; }
            set { configure = value; }
        }

        public StereoElement(Classes cls)
        {
            this.cls = cls;
            this.configure = Configuration.Unset;
        }

        public StereoElement(Configuration configure)
        {
            this.cls = Classes.Unset;
            this.configure = configure;
        }

        public StereoElement(Classes cls, Configuration configure)
        {
            this.cls = cls;
            this.configure = configure;
        }

        public StereoElement(Classes cls, int configure)
            : this(cls, (Configuration)configure)
        { }

        /*
         * Important! The forth nibble of the stereo-class defines the number of
         * carriers (or coordination number) the third nibble just increments when
         * there are two geometries with the same number of carriers.
         */

        public int CarrierLength => cls.GetLength();

        public static bool operator ==(StereoElement a, StereoElement b)
        {
            return a.configure == b.configure && a.cls == b.cls;
        }

        public static bool operator !=(StereoElement a, StereoElement b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is StereoElement))
                return false;
            return this == (StereoElement)obj;
        }

        public override string ToString()
        {
            return cls.ToString() + " " + configure.ToString();
        }

        public override int GetHashCode()
        {
            return cls.GetHashCode() << 16 + configure.GetHashCode();
        }

        /// <summary>Square Planar Configuration in U Shape</summary>
        public static readonly StereoElement SquarePlanarU = new StereoElement(Classes.SquarePlanar, 1);

        /// <summary>Square Planar Configuration in 4 Shape</summary>
        public static readonly StereoElement SquarePlanar4 = new StereoElement(Classes.SquarePlanar, 2);

        /// <summary>Square Planar Configuration in Z Shape</summary>
        public static readonly StereoElement SquarePlanarZ = new StereoElement(Classes.SquarePlanar, 3);
    }

    public static class StereoElementTools
    {
        public static int GetLength(this Classes value)
            => (int)(((uint)value) >> 4);

        /// <summary>
        /// The configuration order of the stereochemistry.
        /// </summary>
        public static short Order(this Configuration value)
            => (short)value;

        public static Configuration Flip(this Configuration value)
        {
            return (Configuration)((int)value ^ 0x3);
        }
    }
}
