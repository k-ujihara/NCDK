/*
 * Copyright (C) 2017  Kazuya Ujihara <ujihara.kazuya@gmail.com>
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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */




/* Copyright (C) 2006-2007  Egon Willighagen <egonw@users.sf.net>
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

namespace NCDK
{
    /// <summary>
    /// Enumeration of possible stereo types of two-atom bonds. The
    /// Stereo type defines not just define the stereochemistry, but also the
    /// which atom is the stereo center for which the Stereo is defined.
    /// The first atom in the IBond (index = 0) is the <i>start</i> atom, while
    /// the second atom (index = 1) is the <i>end</i> atom.
    /// </summary>
    [System.Serializable]
    public partial struct BondStereo : System.IComparable<BondStereo>, System.IComparable
    {
		/// <summary>
		/// The <see cref="Ordinal"/> values of <see cref="BondStereo"/>.
		/// </summary>
		/// <seealso cref="BondStereo"/>
        public static class O
        {
            public const int None = 0;
            public const int Up = 1;
            public const int UpInverted = 2;
            public const int Down = 3;
            public const int DownInverted = 4;
            public const int UpOrDown = 5;
            public const int UpOrDownInverted = 6;
            public const int EOrZ = 7;
            public const int E = 8;
            public const int Z = 9;
            public const int EZByCoordinates = 10;
          
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
            "None", 
            "Up", 
            "UpInverted", 
            "Down", 
            "DownInverted", 
            "UpOrDown", 
            "UpOrDownInverted", 
            "EOrZ", 
            "E", 
            "Z", 
            "EZByCoordinates", 
         
        };

        private BondStereo(int ordinal)
        {
            this.ordinal = ordinal;
        }

        public static explicit operator BondStereo(int ordinal)
        {
            if (!(0 <= ordinal || ordinal < values.Length))
                throw new System.ArgumentOutOfRangeException();
            return values[ordinal];
        }

        public static explicit operator int(BondStereo obj)
        {
            return obj.Ordinal;
        }

        /// <summary>
        /// A bond for which there is no stereochemistry.
        /// </summary>
        public static readonly BondStereo None = new BondStereo(0);
        /// <summary>
        /// A bond pointing up of which the start atom is the stereocenter and the end atom is above the drawing plane.
        /// </summary>
        public static readonly BondStereo Up = new BondStereo(1);
        /// <summary>
        /// A bond pointing up of which the end atom is the stereocenter and the start atom is above the drawing plane.
        /// </summary>
        public static readonly BondStereo UpInverted = new BondStereo(2);
        /// <summary>
        /// A bond pointing down of which the start atom is the stereocenter and the end atom is below the drawing plane.
        /// </summary>
        public static readonly BondStereo Down = new BondStereo(3);
        /// <summary>
        /// A bond pointing down of which the end atom is the stereocenter and the start atom is below the drawing plane.
        /// </summary>
        public static readonly BondStereo DownInverted = new BondStereo(4);
        /// <summary>
        /// A bond for which there is stereochemistry, we just do not know if it is UP or Down. The start atom is the stereocenter.
        /// </summary>
        public static readonly BondStereo UpOrDown = new BondStereo(5);
        /// <summary>
        /// A bond for which there is stereochemistry, we just do not know if it is UP or Down. The end atom is the stereocenter.
        /// </summary>
        public static readonly BondStereo UpOrDownInverted = new BondStereo(6);
        /// <summary>
        /// Indication that this double bond has a fixed, but unknown E/Z configuration.
        /// </summary>
        public static readonly BondStereo EOrZ = new BondStereo(7);
        /// <summary>
        /// Indication that this double bond has a E configuration.
        /// </summary>
        public static readonly BondStereo E = new BondStereo(8);
        /// <summary>
        /// Indication that this double bond has a Z configuration.
        /// </summary>
        public static readonly BondStereo Z = new BondStereo(9);
        /// <summary>
        /// Indication that this double bond has a fixed configuration, defined by the 2D and/or 3D coordinates.
        /// </summary>
        public static readonly BondStereo EZByCoordinates = new BondStereo(10);
        private static readonly BondStereo[] values = new BondStereo[]
        {
            None, 
            Up, 
            UpInverted, 
            Down, 
            DownInverted, 
            UpOrDown, 
            UpOrDownInverted, 
            EOrZ, 
            E, 
            Z, 
            EZByCoordinates, 
    
        };
        public static System.Collections.Generic.IEnumerable<BondStereo> Values => values;

        /* Avoid to cause compiling error */

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public static bool operator==(BondStereo a, object b)
        {
            throw new System.Exception();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public static bool operator!=(BondStereo a, object b)
        {
            throw new System.Exception();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public static bool operator==(object a, BondStereo b)
        {
            throw new System.Exception();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public static bool operator!=(object a, BondStereo b)
        {
            throw new System.Exception();
        }


        public static bool operator==(BondStereo a, BondStereo b)
        {
            
            return a.Ordinal == b.Ordinal;
        }

        public static bool operator !=(BondStereo a, BondStereo b)
        {
            return !(a == b);
        }

		/// <inheritdoc/>
        public override bool Equals(object obj)
        {
    
            if (!(obj is BondStereo))
                return false;
            return this.Ordinal == ((BondStereo)obj).Ordinal;
        }

		/// <inheritdoc/>
        public override int GetHashCode()
        {
            return Ordinal;
        }

		/// <inheritdoc/>
        public int CompareTo(object obj)
        {
            var o = (BondStereo)obj;
            return ((int)Ordinal).CompareTo((int)o.Ordinal);
        }   

		/// <inheritdoc/>
        public int CompareTo(BondStereo o)
        {
            return (Ordinal).CompareTo(o.Ordinal);
        }   	
	}
	public partial struct BondStereo 
    {
    }
}

