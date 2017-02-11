















namespace NCDK
{
    /// <summary>
    /// A list of permissible bond orders.
    /// </summary>
    public struct BondOrder : System.IComparable
    {
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
		public int Ordinal => ordinal;

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

		/* In order to cause compiling error */

		public static bool operator==(BondOrder a, object b)
        {
			throw new System.Exception();
		}

		public static bool operator!=(BondOrder a, object b)
        {
			throw new System.Exception();
		}

        public static bool operator==(object a, BondOrder b)
        {
			throw new System.Exception();
		}

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

		public override bool Equals(object obj)
        {
	
			if (!(obj is BondOrder))
				return false;
            return this.Ordinal == ((BondOrder)obj).Ordinal;
        }

        public override int GetHashCode()
        {
            return Ordinal;
        }

        public int CompareTo(object obj)
        {
            var o = (BondOrder)obj;
            return ((int)Ordinal).CompareTo((int)o.Ordinal);
        }   
		/// <summary>
		/// A numeric value for the number of bonded electron pairs.
		/// </summary>
        public int Numeric => (int)Ordinal;

		public bool IsUnset => this.Ordinal == 0;
    }
}
