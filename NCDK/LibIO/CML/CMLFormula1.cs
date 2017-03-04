// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2017  Kazuya Ujihara




namespace NCDK.LibIO.CML
{
    public partial class CMLFormula
    {
        public struct Types : System.IComparable
        {
        public static class O
        {
            public const int NOPUNCTUATION = 0;
            public const int ELEMENT_COUNT_WHITESPACE = 1;
            public const int ELEMENT_WHITESPACE_COUNT = 2;
            public const int CONCISE = 3;
            public const int MULTIPLIED_ELEMENT_COUNT_WHITESPACE = 4;
            public const int NESTEDBRACKETS = 5;
            public const int IUPAC = 6;
            public const int MOIETY = 7;
            public const int SUBMOIETY = 8;
            public const int STRUCTURAL = 9;
            public const int ANY = 10;
          
        }

        private readonly int ordinal;
        public int Ordinal => ordinal;

        public override string ToString()
        {
            return names[Ordinal];
        }

        private static readonly string[] names = new string[] 
        {
            "NoPunctuation", 
            "Element Count Whitespace", 
            "Element Whitespace Count", 
            "CML Concise", 
            "Multiplied Element Whitespace Count", 
            "NestedBrackets", 
            "IUPAC", 
            "Moiety", 
            "SubMoiety", 
            "STRUCTURAL", 
            "Any", 
         
        };

        private Types(int ordinal)
        {
            this.ordinal = ordinal;
        }

        public static explicit operator Types(int ordinal)
        {
            if (!(0 <= ordinal || ordinal < values.Length))
                throw new System.ArgumentOutOfRangeException();
            return values[ordinal];
        }

        public static explicit operator int(Types obj)
        {
            return obj.Ordinal;
        }

        /// <summary>
        /// the simplest representation. an input-only format. parsing is possible but fragile. The charge semantics are not defined. Not recommended for output.
        /// </summary>
        public static readonly Types NOPUNCTUATION = new Types(0);
        /// <summary>
        /// another simple representation. an input-only format. parsing is also fragile as charge sematics are not defined. Not recommended for output.
        /// </summary>
        public static readonly Types ELEMENT_COUNT_WHITESPACE = new Types(1);
        /// <summary>
        /// Yet another simple representation. an input-only format. Element counts of 1 should always be given. Fragile as charge field is likely to be undefined. Not recommended for output.
        /// </summary>
        public static readonly Types ELEMENT_WHITESPACE_COUNT = new Types(2);
        /// <summary>
        /// the format used in concise. recommended as the main output form. Element counts of 1 should always be given. the charge shoudl always be included. See concise.xsd and formulaType.xsd for syntax.
        /// </summary>
        public static readonly Types CONCISE = new Types(3);
        /// <summary>
        /// multipliers for moieties. an input only format. JUMBO will try to parse this correctly but no guarantee is given.
        /// </summary>
        public static readonly Types MULTIPLIED_ELEMENT_COUNT_WHITESPACE = new Types(4);
        /// <summary>
        /// hierarchical formula. an input-only format. JUMBO will try to parse this correctly but no guarantee is given.
        /// </summary>
        public static readonly Types NESTEDBRACKETS = new Types(5);
        /// <summary>
        /// an input only format. JUMBO will not yet parse this correctly. comments from IUCr
        /// </summary>
        public static readonly Types IUPAC = new Types(6);
        /// <summary>
        /// Moiety, used by IUCr. an input-only format. moieties assumed to be comma separated then ELEMENT_COUNT_WHITESPACE, with optional brackets and post or pre-multipliers
        /// </summary>
        public static readonly Types MOIETY = new Types(7);
        /// <summary>
        /// SubMoiety, used by IUCr. the part of a moiety within the brackets assumed to b ELEMENT_OPTIONALCOUNT followed by optional FORMULA
        /// </summary>
        public static readonly Types SUBMOIETY = new Types(8);
        /// <summary>
        /// Structural, used by IUCr. not currently implemented, I think. probably the same as nested brackets
        /// </summary>
        public static readonly Types STRUCTURAL = new Types(9);
        /// <summary>
        /// any of the above. input-only.
        /// </summary>
        public static readonly Types ANY = new Types(10);
        private static readonly Types[] values = new Types[]
        {
            NOPUNCTUATION, 
            ELEMENT_COUNT_WHITESPACE, 
            ELEMENT_WHITESPACE_COUNT, 
            CONCISE, 
            MULTIPLIED_ELEMENT_COUNT_WHITESPACE, 
            NESTEDBRACKETS, 
            IUPAC, 
            MOIETY, 
            SUBMOIETY, 
            STRUCTURAL, 
            ANY, 
    
        };
        public static System.Collections.Generic.IEnumerable<Types> Values => values;

        /* In order to cause compiling error */

        public static bool operator==(Types a, object b)
        {
            throw new System.Exception();
        }

        public static bool operator!=(Types a, object b)
        {
            throw new System.Exception();
        }

        public static bool operator==(object a, Types b)
        {
            throw new System.Exception();
        }

        public static bool operator!=(object a, Types b)
        {
            throw new System.Exception();
        }

        public static bool operator==(Types a, Types b)
        {
            
            return a.Ordinal == b.Ordinal;
        }

        public static bool operator !=(Types a, Types b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
    
            if (!(obj is Types))
                return false;
            return this.Ordinal == ((Types)obj).Ordinal;
        }

        public override int GetHashCode()
        {
            return Ordinal;
        }

        public int CompareTo(object obj)
        {
            var o = (Types)obj;
            return ((int)Ordinal).CompareTo((int)o.Ordinal);
        }   
        }

        public struct Sorts : System.IComparable
        {
        public static class O
        {
            public const int ALPHABETIC_ELEMENTS = 0;
            public const int CHFIRST = 1;
          
        }

        private readonly int ordinal;
        public int Ordinal => ordinal;

        public override string ToString()
        {
            return names[Ordinal];
        }

        private static readonly string[] names = new string[] 
        {
            "Alphabetic Elements", 
            "C and H first):C H and then alphabetically. (output only", 
         
        };

        private Sorts(int ordinal)
        {
            this.ordinal = ordinal;
        }

        public static explicit operator Sorts(int ordinal)
        {
            if (!(0 <= ordinal || ordinal < values.Length))
                throw new System.ArgumentOutOfRangeException();
            return values[ordinal];
        }

        public static explicit operator int(Sorts obj)
        {
            return obj.Ordinal;
        }

        /// <summary>
        /// sort alphabetically. output only. Not sure where this is
        /// </summary>
        public static readonly Sorts ALPHABETIC_ELEMENTS = new Sorts(0);
        /// <summary>
        /// C H and then alphabetically. (output only)
        /// </summary>
        public static readonly Sorts CHFIRST = new Sorts(1);
        private static readonly Sorts[] values = new Sorts[]
        {
            ALPHABETIC_ELEMENTS, 
            CHFIRST, 
    
        };
        public static System.Collections.Generic.IEnumerable<Sorts> Values => values;

        /* In order to cause compiling error */

        public static bool operator==(Sorts a, object b)
        {
            throw new System.Exception();
        }

        public static bool operator!=(Sorts a, object b)
        {
            throw new System.Exception();
        }

        public static bool operator==(object a, Sorts b)
        {
            throw new System.Exception();
        }

        public static bool operator!=(object a, Sorts b)
        {
            throw new System.Exception();
        }

        public static bool operator==(Sorts a, Sorts b)
        {
            
            return a.Ordinal == b.Ordinal;
        }

        public static bool operator !=(Sorts a, Sorts b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
    
            if (!(obj is Sorts))
                return false;
            return this.Ordinal == ((Sorts)obj).Ordinal;
        }

        public override int GetHashCode()
        {
            return Ordinal;
        }

        public int CompareTo(object obj)
        {
            var o = (Sorts)obj;
            return ((int)Ordinal).CompareTo((int)o.Ordinal);
        }   
        }
    }
}

