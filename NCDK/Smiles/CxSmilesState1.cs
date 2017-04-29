// Copyright (C) 2017  Kazuya Ujihara <uzzy@users.sourceforge.net>
// This file is under LGPL-2.1 




namespace NCDK.Smiles
{
    internal sealed partial class CxSmilesState
    {
    public partial struct Radical : System.IComparable<Radical>, System.IComparable
    {
		/// <summary>
		/// The <see cref="Ordinal"/> values of <see cref="Radical"/>.
		/// </summary>
		/// <seealso cref="Radical"/>
        public static class O
        {
            public const int Monovalent = 0;
            public const int Divalent = 1;
            public const int DivalentSinglet = 2;
            public const int DivalentTriplet = 3;
            public const int Trivalent = 4;
            public const int TrivalentDoublet = 5;
            public const int TrivalentQuartet = 6;
          
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
            "Monovalent", 
            "Divalent", 
            "DivalentSinglet", 
            "DivalentTriplet", 
            "Trivalent", 
            "TrivalentDoublet", 
            "TrivalentQuartet", 
         
        };

        private Radical(int ordinal)
        {
            this.ordinal = ordinal;
        }

        public static explicit operator Radical(int ordinal)
        {
            if (!(0 <= ordinal || ordinal < values.Length))
                throw new System.ArgumentOutOfRangeException();
            return values[ordinal];
        }

        public static explicit operator int(Radical obj)
        {
            return obj.Ordinal;
        }

        public static readonly Radical Monovalent = new Radical(0);
        public static readonly Radical Divalent = new Radical(1);
        public static readonly Radical DivalentSinglet = new Radical(2);
        public static readonly Radical DivalentTriplet = new Radical(3);
        public static readonly Radical Trivalent = new Radical(4);
        public static readonly Radical TrivalentDoublet = new Radical(5);
        public static readonly Radical TrivalentQuartet = new Radical(6);
        private static readonly Radical[] values = new Radical[]
        {
            Monovalent, 
            Divalent, 
            DivalentSinglet, 
            DivalentTriplet, 
            Trivalent, 
            TrivalentDoublet, 
            TrivalentQuartet, 
    
        };
        public static System.Collections.Generic.IEnumerable<Radical> Values => values;

        /* In order to cause compiling error */

        public static bool operator==(Radical a, object b)
        {
            throw new System.Exception();
        }

        public static bool operator!=(Radical a, object b)
        {
            throw new System.Exception();
        }

        public static bool operator==(object a, Radical b)
        {
            throw new System.Exception();
        }

        public static bool operator!=(object a, Radical b)
        {
            throw new System.Exception();
        }

        public static bool operator==(Radical a, Radical b)
        {
            
            return a.Ordinal == b.Ordinal;
        }

        public static bool operator !=(Radical a, Radical b)
        {
            return !(a == b);
        }

		/// <inheritdoc/>
        public override bool Equals(object obj)
        {
    
            if (!(obj is Radical))
                return false;
            return this.Ordinal == ((Radical)obj).Ordinal;
        }

		/// <inheritdoc/>
        public override int GetHashCode()
        {
            return Ordinal;
        }

		/// <inheritdoc/>
        public int CompareTo(object obj)
        {
            var o = (Radical)obj;
            return ((int)Ordinal).CompareTo((int)o.Ordinal);
        }   

		/// <inheritdoc/>
        public int CompareTo(Radical o)
        {
            return (Ordinal).CompareTo(o.Ordinal);
        }   	
	}
	public partial struct Radical 
		{}
	}
}

	