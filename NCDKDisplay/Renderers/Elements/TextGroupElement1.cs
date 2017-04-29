// Copyright (C) 2017  Kazuya Ujihara <uzzy@users.sourceforge.net>
// This file is under LGPL-2.1 




namespace NCDK.Renderers.Elements
{
    public partial class TextGroupElement
	{
        /// <summary>
        /// Compass-point positions for text element annotation children.
        /// </summary>
    public partial struct Position : System.IComparable<Position>, System.IComparable
    {
		/// <summary>
		/// The <see cref="Ordinal"/> values of <see cref="Position"/>.
		/// </summary>
		/// <seealso cref="Position"/>
        public static class O
        {
            public const int NW = 0;
            public const int SW = 1;
            public const int SE = 2;
            public const int NE = 3;
            public const int S = 4;
            public const int N = 5;
            public const int W = 6;
            public const int E = 7;
          
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
            "NW", 
            "SW", 
            "SE", 
            "NE", 
            "S", 
            "N", 
            "W", 
            "E", 
         
        };

        private Position(int ordinal)
        {
            this.ordinal = ordinal;
        }

        public static explicit operator Position(int ordinal)
        {
            if (!(0 <= ordinal || ordinal < values.Length))
                throw new System.ArgumentOutOfRangeException();
            return values[ordinal];
        }

        public static explicit operator int(Position obj)
        {
            return obj.Ordinal;
        }

        public static readonly Position NW = new Position(0);
        public static readonly Position SW = new Position(1);
        public static readonly Position SE = new Position(2);
        public static readonly Position NE = new Position(3);
        public static readonly Position S = new Position(4);
        public static readonly Position N = new Position(5);
        public static readonly Position W = new Position(6);
        public static readonly Position E = new Position(7);
        private static readonly Position[] values = new Position[]
        {
            NW, 
            SW, 
            SE, 
            NE, 
            S, 
            N, 
            W, 
            E, 
    
        };
        public static System.Collections.Generic.IEnumerable<Position> Values => values;

        /* In order to cause compiling error */

        public static bool operator==(Position a, object b)
        {
            throw new System.Exception();
        }

        public static bool operator!=(Position a, object b)
        {
            throw new System.Exception();
        }

        public static bool operator==(object a, Position b)
        {
            throw new System.Exception();
        }

        public static bool operator!=(object a, Position b)
        {
            throw new System.Exception();
        }

        public static bool operator==(Position a, Position b)
        {
            
            return a.Ordinal == b.Ordinal;
        }

        public static bool operator !=(Position a, Position b)
        {
            return !(a == b);
        }

		/// <inheritdoc/>
        public override bool Equals(object obj)
        {
    
            if (!(obj is Position))
                return false;
            return this.Ordinal == ((Position)obj).Ordinal;
        }

		/// <inheritdoc/>
        public override int GetHashCode()
        {
            return Ordinal;
        }

		/// <inheritdoc/>
        public int CompareTo(object obj)
        {
            var o = (Position)obj;
            return ((int)Ordinal).CompareTo((int)o.Ordinal);
        }   

		/// <inheritdoc/>
        public int CompareTo(Position o)
        {
            return (Ordinal).CompareTo(o.Ordinal);
        }   	
	}
	public partial struct Position 
		{
		}
	}
}
