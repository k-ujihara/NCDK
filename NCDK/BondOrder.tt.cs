


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
    /// A list of permissible bond orders.
    /// </summary>
    [System.Serializable]
    public partial struct BondOrder : System.IComparable<BondOrder>, System.IComparable
    {
		/// <summary>
		/// The <see cref="Ordinal"/> values of <see cref="BondOrder"/>.
		/// </summary>
		/// <seealso cref="BondOrder"/>
        public static class O
        {
            public const int Unset = 0;
            public const int Single = 1;
            public const int Double = 2;
            public const int Triple = 3;
            public const int Quadruple = 4;
            public const int Quintuple = 5;
            public const int Sextuple = 6;
          
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
            "Single", 
            "Double", 
            "Triple", 
            "Quadruple", 
            "Quintuple", 
            "Sextuple", 
         
        };

        private BondOrder(int ordinal)
        {
            this.ordinal = ordinal;
        }

        public static explicit operator BondOrder(int ordinal)
        {
            if (!(0 <= ordinal || ordinal < values.Length))
                throw new System.ArgumentOutOfRangeException();
            return values[ordinal];
        }

        public static explicit operator int(BondOrder obj)
        {
            return obj.Ordinal;
        }

        public static readonly BondOrder Unset = new BondOrder(0);
        public static readonly BondOrder Single = new BondOrder(1);
        public static readonly BondOrder Double = new BondOrder(2);
        public static readonly BondOrder Triple = new BondOrder(3);
        public static readonly BondOrder Quadruple = new BondOrder(4);
        public static readonly BondOrder Quintuple = new BondOrder(5);
        public static readonly BondOrder Sextuple = new BondOrder(6);
        private static readonly BondOrder[] values = new BondOrder[]
        {
            Unset, 
            Single, 
            Double, 
            Triple, 
            Quadruple, 
            Quintuple, 
            Sextuple, 
    
        };
        public static System.Collections.Generic.IEnumerable<BondOrder> Values => values;

        /* Avoid to cause compiling error */

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public static bool operator==(BondOrder a, object b)
        {
            throw new System.Exception();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public static bool operator!=(BondOrder a, object b)
        {
            throw new System.Exception();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public static bool operator==(object a, BondOrder b)
        {
            throw new System.Exception();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public static bool operator!=(object a, BondOrder b)
        {
            throw new System.Exception();
        }


        public static bool operator==(BondOrder a, BondOrder b)
        {
            
            return a.Ordinal == b.Ordinal;
        }

        public static bool operator !=(BondOrder a, BondOrder b)
        {
            return !(a == b);
        }

		/// <inheritdoc/>
        public override bool Equals(object obj)
        {
    
            if (!(obj is BondOrder))
                return false;
            return this.Ordinal == ((BondOrder)obj).Ordinal;
        }

		/// <inheritdoc/>
        public override int GetHashCode()
        {
            return Ordinal;
        }

		/// <inheritdoc/>
        public int CompareTo(object obj)
        {
            var o = (BondOrder)obj;
            return ((int)Ordinal).CompareTo((int)o.Ordinal);
        }   

		/// <inheritdoc/>
        public int CompareTo(BondOrder o)
        {
            return (Ordinal).CompareTo(o.Ordinal);
        }   	
	}
	public partial struct BondOrder 
    {
        /// <summary>
        /// A numeric value for the number of bonded electron pairs.
        /// </summary>
        public int Numeric => (int)Ordinal;

        public bool IsUnset => this.Ordinal == 0;
    }
}
