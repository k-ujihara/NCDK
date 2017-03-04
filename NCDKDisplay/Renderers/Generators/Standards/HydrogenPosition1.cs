// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2016-2017  Kazuya Ujihara




namespace NCDK.Renderers.Generators.Standards
{
    public partial struct HydrogenPosition : System.IComparable
    {
        public static class O
        {
            public const int Right = 0;
            public const int Left = 1;
            public const int Above = 2;
            public const int Below = 3;
          
        }

        private readonly int ordinal;
        public int Ordinal => ordinal;

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

        public override bool Equals(object obj)
        {
    
            if (!(obj is HydrogenPosition))
                return false;
            return this.Ordinal == ((HydrogenPosition)obj).Ordinal;
        }

        public override int GetHashCode()
        {
            return Ordinal;
        }

        public int CompareTo(object obj)
        {
            var o = (HydrogenPosition)obj;
            return ((int)Ordinal).CompareTo((int)o.Ordinal);
        }   
        public string Name => ToString();

        public bool IsUnset => this.Ordinal == 0;
    }
}
