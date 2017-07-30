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




/* Copyright (C) 2012  Egon Willighagen <egonw@users.sf.net>
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
    /// Enumeration that defines the two possible values for this stereochemistry type.
    /// </summary>
    public partial struct DoubleBondConformation : System.IComparable<DoubleBondConformation>, System.IComparable
    {
		/// <summary>
		/// The <see cref="Ordinal"/> values of <see cref="DoubleBondConformation"/>.
		/// </summary>
		/// <seealso cref="DoubleBondConformation"/>
        public static class O
        {
            public const int Unset = 0;
            public const int Together = 1;
            public const int Opposite = 2;
          
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
            "Together", 
            "Opposite", 
         
        };

        private DoubleBondConformation(int ordinal)
        {
            this.ordinal = ordinal;
        }

        public static explicit operator DoubleBondConformation(int ordinal)
        {
            if (!(0 <= ordinal || ordinal < values.Length))
                throw new System.ArgumentOutOfRangeException();
            return values[ordinal];
        }

        public static explicit operator int(DoubleBondConformation obj)
        {
            return obj.Ordinal;
        }

        public static readonly DoubleBondConformation Unset = new DoubleBondConformation(0);
        /// <summary>
        /// Z-form
        /// </summary>
        public static readonly DoubleBondConformation Together = new DoubleBondConformation(1);
        /// <summary>
        /// E-form
        /// </summary>
        public static readonly DoubleBondConformation Opposite = new DoubleBondConformation(2);
        private static readonly DoubleBondConformation[] values = new DoubleBondConformation[]
        {
            Unset, 
            Together, 
            Opposite, 
    
        };
        public static System.Collections.Generic.IEnumerable<DoubleBondConformation> Values => values;

        /* Avoid to cause compiling error */

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public static bool operator==(DoubleBondConformation a, object b)
        {
            throw new System.Exception();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public static bool operator!=(DoubleBondConformation a, object b)
        {
            throw new System.Exception();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public static bool operator==(object a, DoubleBondConformation b)
        {
            throw new System.Exception();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public static bool operator!=(object a, DoubleBondConformation b)
        {
            throw new System.Exception();
        }


        public static bool operator==(DoubleBondConformation a, DoubleBondConformation b)
        {
            
            return a.Ordinal == b.Ordinal;
        }

        public static bool operator !=(DoubleBondConformation a, DoubleBondConformation b)
        {
            return !(a == b);
        }

		/// <inheritdoc/>
        public override bool Equals(object obj)
        {
    
            if (!(obj is DoubleBondConformation))
                return false;
            return this.Ordinal == ((DoubleBondConformation)obj).Ordinal;
        }

		/// <inheritdoc/>
        public override int GetHashCode()
        {
            return Ordinal;
        }

		/// <inheritdoc/>
        public int CompareTo(object obj)
        {
            var o = (DoubleBondConformation)obj;
            return ((int)Ordinal).CompareTo((int)o.Ordinal);
        }   

		/// <inheritdoc/>
        public int CompareTo(DoubleBondConformation o)
        {
            return (Ordinal).CompareTo(o.Ordinal);
        }   	
	}
	public partial struct DoubleBondConformation 
    {
        public bool IsUnset => this.Ordinal == 0;

        /// <summary>
        /// Invert this conformation, Together.Invert() = Opposite, Opposite.Invert() = Together.
        /// </summary>
        /// <returns>the inverse conformation</returns>
        public DoubleBondConformation Invert() => Ordinal == O.Together ? Opposite : Together;
    }
}
