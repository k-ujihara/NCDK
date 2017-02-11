















namespace NCDK
{
    public struct TetrahedralStereo
    {
        public static class O
        {
			public const int Unset = 0;
			public const int Clockwise = 1;
			public const int AntiClockwise = 2;
      	
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
            "Clockwise", 
            "AntiClockwise", 
         
        };

        private TetrahedralStereo(int ordinal)
        {
            this.ordinal = ordinal;
        }

		public static explicit operator TetrahedralStereo(int ordinal)
        {
            if (!(0 <= ordinal || ordinal < values.Length))
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
        public static System.Collections.Generic.IEnumerable<TetrahedralStereo> Values => values;

		/* In order to cause compiling error */

		public static bool operator==(TetrahedralStereo a, object b)
        {
			throw new System.Exception();
		}

		public static bool operator!=(TetrahedralStereo a, object b)
        {
			throw new System.Exception();
		}

        public static bool operator==(object a, TetrahedralStereo b)
        {
			throw new System.Exception();
		}

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

		public override bool Equals(object obj)
        {
	
			if (!(obj is TetrahedralStereo))
				return false;
            return this.Ordinal == ((TetrahedralStereo)obj).Ordinal;
        }

        public override int GetHashCode()
        {
            return Ordinal;
        }

        public int CompareTo(object obj)
        {
            var o = (TetrahedralStereo)obj;
            return ((int)Ordinal).CompareTo((int)o.Ordinal);
        }   
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
    }
}

