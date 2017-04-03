// Copyright (C) 2017  Kazuya Ujihara
// This file is under LGPL-2.1 




/*
 * Copyright (c) 2014 European Bioinformatics Institute (EMBL-EBI)
 *                    John May <jwmay@users.sf.net>
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

namespace NCDK.Stereo
{
    /// <summary>
    /// Stereochemistry projection types. 
    /// </summary>
    // @author John May
    public partial struct Projection : System.IComparable<Projection>, System.IComparable
    {
		/// <summary>
		/// The <see cref="Ordinal"/> values of <see cref="Projection"/>.
		/// </summary>
		/// <seealso cref="Projection"/>
        public static class O
        {
            public const int Unset = 0;
            public const int Fischer = 1;
            public const int Haworth = 2;
            public const int Chair = 3;
          
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
            "Fischer", 
            "Haworth", 
            "Chair", 
         
        };

        private Projection(int ordinal)
        {
            this.ordinal = ordinal;
        }

        public static explicit operator Projection(int ordinal)
        {
            if (!(0 <= ordinal || ordinal < values.Length))
                throw new System.ArgumentOutOfRangeException();
            return values[ordinal];
        }

        public static explicit operator int(Projection obj)
        {
            return obj.Ordinal;
        }

        public static readonly Projection Unset = new Projection(0);
        /// <summary>
        /// Fischer projections are used for linear chain-form carbohydrates. They are drawn vertically with all atoms at right angles around stereocenters.
        /// </summary>
        public static readonly Projection Fischer = new Projection(1);
        /// <summary>
        /// Haworth projection are used to depict ring-form carbohydrates. The ring may be of size 5, 6, or 7 (rarer). Here the ring is flat and the substituents connected to stereocenters are drawn directly above or below the plane of the ring.
        /// </summary>
        public static readonly Projection Haworth = new Projection(2);
        /// <summary>
        /// Projection of the low energy conformation (chair) of a cyclohexane. Used for carbohydrates.
        /// </summary>
        public static readonly Projection Chair = new Projection(3);
        private static readonly Projection[] values = new Projection[]
        {
            Unset, 
            Fischer, 
            Haworth, 
            Chair, 
    
        };
        public static System.Collections.Generic.IEnumerable<Projection> Values => values;

        /* In order to cause compiling error */

        public static bool operator==(Projection a, object b)
        {
            throw new System.Exception();
        }

        public static bool operator!=(Projection a, object b)
        {
            throw new System.Exception();
        }

        public static bool operator==(object a, Projection b)
        {
            throw new System.Exception();
        }

        public static bool operator!=(object a, Projection b)
        {
            throw new System.Exception();
        }

        public static bool operator==(Projection a, Projection b)
        {
            
            return a.Ordinal == b.Ordinal;
        }

        public static bool operator !=(Projection a, Projection b)
        {
            return !(a == b);
        }

		/// <inheritdoc/>
        public override bool Equals(object obj)
        {
    
            if (!(obj is Projection))
                return false;
            return this.Ordinal == ((Projection)obj).Ordinal;
        }

		/// <inheritdoc/>
        public override int GetHashCode()
        {
            return Ordinal;
        }

		/// <inheritdoc/>
        public int CompareTo(object obj)
        {
            var o = (Projection)obj;
            return ((int)Ordinal).CompareTo((int)o.Ordinal);
        }   

		/// <inheritdoc/>
        public int CompareTo(Projection o)
        {
            return (Ordinal).CompareTo(o.Ordinal);
        }   	
	}
	public partial struct Projection 
	{
        public bool IsUnset => this.Ordinal == 0;
    }
}
