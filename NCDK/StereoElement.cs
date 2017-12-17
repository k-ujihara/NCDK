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
 *
 */

using System;

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
        /// with the values <pre>1</pre> (odd) and <pre>2</pre> (even).
        /// </summary>
        /// <remarks>
        /// Special values (e.g. <pre>0</pre>) may be used to represent unknown/unspecified or
        /// racemic in future but are currently undefined.
        /// </remarks>
        public partial struct Configurations : IComparable<Configurations>, IComparable
        {
            /// <summary>
            /// The <see cref="Ordinal"/> values of <see cref="Configurations"/>.
            /// </summary>
            /// <seealso cref="Configurations"/>
            public static partial class O
            {
                public const Int16 Unset = 0;
                public const Int16 Left = 1;
                public const Int16 Right = 2;
            }

            private readonly Int16 ordinal;

            /// <summary>
            /// The ordinal of this enumeration constant. The list is in <see cref="O"/>.
            /// </summary>
            /// <seealso cref="O"/>
            public int Ordinal => ordinal;

            /// <summary>
            /// The configuration order of the stereochemistry.
            /// </summary>
            public int Order => ordinal;

            /// <inheritdoc/>
            public override string ToString()
            {
                return Ordinal.ToString();
            }

            private Configurations(int ordinal)
            {
                this.ordinal = (Int16)ordinal;
            }

            public static explicit operator Configurations(int ordinal)
            {
                return new Configurations(ordinal);
            }

            public static explicit operator int(Configurations obj)
            {
                return obj.ordinal;
            }

            public static readonly Configurations Unset = new Configurations(0);
            public static readonly Configurations Left = new Configurations(1);
            public static readonly Configurations Right = new Configurations(2);

            /* Avoid to cause compiling error */

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
            public static bool operator ==(Configurations a, object b)
            {
                throw new System.Exception();
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
            public static bool operator !=(Configurations a, object b)
            {
                throw new System.Exception();
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
            public static bool operator ==(object a, Configurations b)
            {
                throw new System.Exception();
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
            public static bool operator !=(object a, Configurations b)
            {
                throw new System.Exception();
            }

            public static bool operator ==(Configurations a, Configurations b)
            {
                return a.Ordinal == b.Ordinal;
            }

            public static bool operator !=(Configurations a, Configurations b)
            {
                return !(a == b);
            }

            /// <inheritdoc/>
            public override bool Equals(object obj)
            {
                if (!(obj is Configurations))
                    return false;
                return this.Ordinal == ((Configurations)obj).Ordinal;
            }

            /// <inheritdoc/>
            public override int GetHashCode()
            {
                return Ordinal;
            }

            /// <inheritdoc/>
            public int CompareTo(object obj)
            {
                var o = (Configurations)obj;
                return ordinal.CompareTo(o.ordinal);
            }

            /// <inheritdoc/>
            public int CompareTo(Configurations o)
            {
                return ordinal.CompareTo(o.ordinal);
            }

            public Configurations Flip()
            {
                return new Configurations(this.ordinal ^ 0x3);
            }

            public static partial class O
            {
                public const int Opposite = Left;
                public const int Together = Right;
            }

            public static Configurations Opposite = Left;
            public static Configurations Together = Right;
        }

        /// <summary>
        /// There stereo class defines the type of stereochemistry/geometry that is
        /// captured. The stereo class is also defined as a integral value. The
        /// following classes are available with varied support through out the
        /// toolkit.        
        /// </summary>
        public struct Classes
        {
            /// <summary>
            /// The <see cref="Ordinal"/> values of <see cref="Configurations"/>.
            /// </summary>
            /// <seealso cref="Configurations"/>
            public static class O
            {
                public const Int16 Unset = 0;
                public const Int16 CisTrans = 0x21;
                public const Int16 Tetrahedral = 0x42;
                public const Int16 Allenal = 0x43;
                public const Int16 Atropisomeric = 0x44;
                public const Int16 SquarePlanar = 0x45;
                public const Int16 SquarePyramidal = 0x51;
                public const Int16 TrigonalBipyramidal = 0x52;
                public const Int16 Octahedral = 0x61;
                public const Int16 PentagonalBipyramidal = 0x71;
                public const Int16 HexagonalBipyramidal = 0x81;
                public const Int16 HeptagonalBipyramidal = 0x91;
            };

            private Int16 ordinal;
            /// <summary>
            /// The ordinal of this enumeration constant. The list is in <see cref="O"/>.
            /// </summary>
            /// <seealso cref="O"/>
            public int Ordinal => ordinal;

            private Classes(Int16 ordinal)
            {
                this.ordinal = ordinal;
            }

            public static Classes Unset = new Classes(0);

            /// <summary>Geomeric CisTrans (e.g. but-2-ene)</summary>
            public static Classes CisTrans = new Classes(0x21);

            /// <summary>Tetrahedral (T-4) (e.g. butan-2-ol)</summary>
            public static Classes Tetrahedral = new Classes(0x42);

            /// <summary>ExtendedTetrahedral (e.g. 2,3-pentadiene)</summary>
            public static Classes Allenal = new Classes(0x43);

            /// <summary>Atropisomeric (e.g. BiNAP)</summary>
            public static Classes Atropisomeric = new Classes(0x44);

            /// <summary>Square Planar (SP-4) (e.g. cisplatin)</summary>
            public static Classes SquarePlanar = new Classes(0x45);

            /// <summary>Square Pyramidal (SPY-5)</summary>
            public static Classes SquarePyramidal = new Classes(0x51);

            /// <summary>Trigonal Bipyramidal (TBPY-5)</summary>
            public static Classes TrigonalBipyramidal = new Classes(0x52);

            /// <summary>Octahedral (OC-6)</summary>
            public static Classes Octahedral = new Classes(0x61);

            /// <summary>Pentagonal Bipyramidal (PBPY-7)</summary>
            public static Classes PentagonalBipyramidal = new Classes(0x71);

            /// <summary>Hexagonal Bipyramidal (HBPY-8)</summary>
            public static Classes HexagonalBipyramidal = new Classes(0x81);

            /// <summary>Heptagonal Bipyramidal (HBPY-9)</summary>
            public static Classes HeptagonalBipyramidal = new Classes(0x91);

            private static readonly Classes[] values = new Classes[]
            {
                Unset,
                CisTrans,
                Tetrahedral,
                Allenal,
                Atropisomeric,
                SquarePlanar,
                SquarePyramidal,
                TrigonalBipyramidal,
                Octahedral,
                PentagonalBipyramidal,
                HexagonalBipyramidal,
                HeptagonalBipyramidal,
            };

            public static Classes[] Values => values;

            /* Avoid to cause compiling error */

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
            public static bool operator ==(Classes a, object b)
            {
                throw new System.Exception();
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
            public static bool operator !=(Classes a, object b)
            {
                throw new System.Exception();
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
            public static bool operator ==(object a, Classes b)
            {
                throw new System.Exception();
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
            public static bool operator !=(object a, Classes b)
            {
                throw new System.Exception();
            }


            public static bool operator ==(Classes a, Classes b)
            {
                return a.Ordinal == b.Ordinal;
            }

            public static bool operator !=(Classes a, Classes b)
            {
                return !(a == b);
            }

            /// <inheritdoc/>
            public override bool Equals(object obj)
            {

                if (!(obj is Classes))
                    return false;
                return this.Ordinal == ((Classes)obj).Ordinal;
            }

            /// <inheritdoc/>
            public override int GetHashCode()
            {
                return Ordinal;
            }

            /// <inheritdoc/>
            public int CompareTo(object obj)
            {
                var o = (Classes)obj;
                return ((int)Ordinal).CompareTo((int)o.Ordinal);
            }

            /// <inheritdoc/>
            public int CompareTo(Configurations o)
            {
                return (Ordinal).CompareTo(o.Ordinal);
            }

            public int Length => (int)(((uint)ordinal) >> 4);
        }

        Classes cls;
        Configurations configure;

        public Classes Class => cls;

        public Configurations Configure
        {
            get { return configure; }
            set { configure = value; }
        }

        public StereoElement(Classes cls)
        {
            this.cls = cls;
            this.configure = Configurations.Unset;
        }

        public StereoElement(Configurations configure)
        {
            this.cls = Classes.Unset;
            this.configure = configure;
        }

        public StereoElement(Classes cls, Configurations configure)
        {
            this.cls = cls;
            this.configure = configure;
        }

        public StereoElement(Classes cls, int configure)
            : this(cls, (Configurations)configure)
        { }

        /*
         * Important! The forth nibble of the stereo-class defines the number of
         * carriers (or coordination number) the third nibble just increments when
         * there are two geometries with the same number of carriers.
         */

        public int CarrierLength => cls.Length;

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

        /// <summary>Square Planar Configutation in U Shape</summary>
        public static readonly StereoElement SquarePlanarU = new StereoElement(Classes.SquarePlanar, 1);

        /// <summary>Square Planar Configutation in 4 Shape</summary>
        public static readonly StereoElement SquarePlanar4 = new StereoElement(Classes.SquarePlanar, 2);

        /// <summary>Square Planar Configutation in Z Shape</summary>
        public static readonly StereoElement SquarePlanarZ = new StereoElement(Classes.SquarePlanar, 3);
    }
}
