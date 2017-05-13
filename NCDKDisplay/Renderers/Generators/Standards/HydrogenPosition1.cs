// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2016-2017  Kazuya Ujihara

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




namespace NCDK.Renderers.Generators.Standards
{
    public partial struct HydrogenPosition : System.IComparable<HydrogenPosition>, System.IComparable
    {
		/// <summary>
		/// The <see cref="Ordinal"/> values of <see cref="HydrogenPosition"/>.
		/// </summary>
		/// <seealso cref="HydrogenPosition"/>
        public static class O
        {
            public const int Right = 0;
            public const int Left = 1;
            public const int Above = 2;
            public const int Below = 3;
          
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
            "Right", 
            "Left", 
            "Above", 
            "Below", 
         
        };

        private HydrogenPosition(int ordinal)
        {
            this.ordinal = ordinal;
        }

        public static explicit operator HydrogenPosition(int ordinal)
        {
            if (!(0 <= ordinal || ordinal < values.Length))
                throw new System.ArgumentOutOfRangeException();
            return values[ordinal];
        }

        public static explicit operator int(HydrogenPosition obj)
        {
            return obj.Ordinal;
        }

        public static readonly HydrogenPosition Right = new HydrogenPosition(0);
        public static readonly HydrogenPosition Left = new HydrogenPosition(1);
        public static readonly HydrogenPosition Above = new HydrogenPosition(2);
        public static readonly HydrogenPosition Below = new HydrogenPosition(3);
        private static readonly HydrogenPosition[] values = new HydrogenPosition[]
        {
            Right, 
            Left, 
            Above, 
            Below, 
    
        };
        public static System.Collections.Generic.IEnumerable<HydrogenPosition> Values => values;

        /* In order to cause compiling error */

        public static bool operator==(HydrogenPosition a, object b)
        {
            throw new System.Exception();
        }

        public static bool operator!=(HydrogenPosition a, object b)
        {
            throw new System.Exception();
        }

        public static bool operator==(object a, HydrogenPosition b)
        {
            throw new System.Exception();
        }

        public static bool operator!=(object a, HydrogenPosition b)
        {
            throw new System.Exception();
        }

        public static bool operator==(HydrogenPosition a, HydrogenPosition b)
        {
            
            return a.Ordinal == b.Ordinal;
        }

        public static bool operator !=(HydrogenPosition a, HydrogenPosition b)
        {
            return !(a == b);
        }

		/// <inheritdoc/>
        public override bool Equals(object obj)
        {
    
            if (!(obj is HydrogenPosition))
                return false;
            return this.Ordinal == ((HydrogenPosition)obj).Ordinal;
        }

		/// <inheritdoc/>
        public override int GetHashCode()
        {
            return Ordinal;
        }

		/// <inheritdoc/>
        public int CompareTo(object obj)
        {
            var o = (HydrogenPosition)obj;
            return ((int)Ordinal).CompareTo((int)o.Ordinal);
        }   

		/// <inheritdoc/>
        public int CompareTo(HydrogenPosition o)
        {
            return (Ordinal).CompareTo(o.Ordinal);
        }   	
	}
	public partial struct HydrogenPosition 
    {
    }
}
