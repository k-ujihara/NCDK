


/* Copyright (C) 2010  Egon Willighagen <egonw@users.sf.net>
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

namespace NCDK
{
    /// <summary>
    /// Enumeration that defines the two possible chiralities for this stereochemistry type.
    /// </summary>
    public partial struct TetrahedralStereo : System.IComparable<TetrahedralStereo>, System.IComparable
    {
		/// <summary>
		/// The <see cref="Ordinal"/> values of <see cref="TetrahedralStereo"/>.
		/// </summary>
		/// <seealso cref="TetrahedralStereo"/>
        public static partial class O
        {
            public const int Unset = 0;
            public const int Clockwise = 1;
            public const int AntiClockwise = 2;
          
        }

        private readonly int ordinal;
		/// <summary>
		/// The ordinal of this enumeration constant. The list is in <see cref="O"/>.
		/// </summary>
		/// <seealso cref="O"/>
        public int Ordinal => ordinal;

		/// <inheritdoc/>
        public override string ToString()
        {
            return names[Ordinal];
        }

        private static readonly string[] names = new string[] 
        {
            "Unset", 
            "Clockwise", 
            "AntiClockwise", 
         
        };

        private TetrahedralStereo(int ordinal)
        {
            this.ordinal = ordinal;
        }

        public static explicit operator TetrahedralStereo(int ordinal)
        {
            if (!(0 <= ordinal && ordinal < values.Length))
                throw new System.ArgumentOutOfRangeException();
            return values[ordinal];
        }

        public static explicit operator int(TetrahedralStereo obj)
        {
            return obj.Ordinal;
        }

        public static readonly TetrahedralStereo Unset = new TetrahedralStereo(0);
        public static readonly TetrahedralStereo Clockwise = new TetrahedralStereo(1);
        public static readonly TetrahedralStereo AntiClockwise = new TetrahedralStereo(2);
        private static readonly TetrahedralStereo[] values = new TetrahedralStereo[]
        {
            Unset, 
            Clockwise, 
            AntiClockwise, 
    
        };
        public static TetrahedralStereo[] Values => values;

        /* Avoid to cause compiling error */

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public static bool operator==(TetrahedralStereo a, object b)
        {
            throw new System.Exception();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public static bool operator!=(TetrahedralStereo a, object b)
        {
            throw new System.Exception();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public static bool operator==(object a, TetrahedralStereo b)
        {
            throw new System.Exception();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public static bool operator!=(object a, TetrahedralStereo b)
        {
            throw new System.Exception();
        }


        public static bool operator==(TetrahedralStereo a, TetrahedralStereo b)
        {
            
            return a.Ordinal == b.Ordinal;
        }

        public static bool operator !=(TetrahedralStereo a, TetrahedralStereo b)
        {
            return !(a == b);
        }

		/// <inheritdoc/>
        public override bool Equals(object obj)
        {
    
            if (!(obj is TetrahedralStereo))
                return false;
            return this.Ordinal == ((TetrahedralStereo)obj).Ordinal;
        }

		/// <inheritdoc/>
        public override int GetHashCode()
        {
            return Ordinal;
        }

		/// <inheritdoc/>
        public int CompareTo(object obj)
        {
            var o = (TetrahedralStereo)obj;
            return ((int)Ordinal).CompareTo((int)o.Ordinal);
        }   

		/// <inheritdoc/>
        public int CompareTo(TetrahedralStereo o)
        {
            return (Ordinal).CompareTo(o.Ordinal);
        }   	
	}
	public partial struct TetrahedralStereo 
    {
        public bool IsUnset => this.Ordinal == 0;

        /// <summary>
        /// Invert this conformation,
        /// Invert(clockwise) = anti_clockwise, Invert(anti_clockwise) = clockwise.
        /// </summary>
        /// <returns> the inverse conformation</returns>
        public TetrahedralStereo Invert()
        {
            if (this == Clockwise)
                return AntiClockwise;
            if (this == AntiClockwise)
                return Clockwise;
            return this;
        }

        public static StereoElement.Configurations ToConfigure(TetrahedralStereo stereo)
        {
            switch (stereo.Ordinal)
            {
                case TetrahedralStereo.O.AntiClockwise:
                    return StereoElement.Configurations.Left;
                case TetrahedralStereo.O.Clockwise:
                    return StereoElement.Configurations.Right;
                default:
                    throw new System.ArgumentException("Unknown enum value: " + stereo);
            }
        }

        public static TetrahedralStereo ToStereo(StereoElement.Configurations configure)
        {
            switch (configure.Ordinal)
            {
                case StereoElement.Configurations.O.Left:
                    return AntiClockwise;
                case StereoElement.Configurations.O.Right:
                    return Clockwise;
                default:
                    throw new System.ArgumentException("Cannot map to enum value: " + configure);
            }
        }
    }
}

