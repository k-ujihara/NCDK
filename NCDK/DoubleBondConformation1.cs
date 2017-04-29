// Copyright (C) 2017  Kazuya Ujihara <uzzy@users.sourceforge.net>
// This file is under LGPL-2.1 




namespace NCDK
{
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

        /* In order to cause compiling error */

        public static bool operator==(DoubleBondConformation a, object b)
        {
            throw new System.Exception();
        }

        public static bool operator!=(DoubleBondConformation a, object b)
        {
            throw new System.Exception();
        }

        public static bool operator==(object a, DoubleBondConformation b)
        {
            throw new System.Exception();
        }

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

        public DoubleBondConformation Invert() => Ordinal == O.Together ? Opposite : Together;
    }
}
