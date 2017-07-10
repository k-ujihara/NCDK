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




using System.Collections.Generic;

namespace NCDK.SGroups
{
    /// <summary>
    /// Enumeration of Ctab Sgroup types.
    /// </summary>
    /// <remarks>
    /// <b>Display shortcuts</b>
    /// <list type="bullet">
    /// <item>SUP, abbreviation Sgroup (formerly called superatom)</item>
    /// <item>MUL, multiple group</item>
    /// <item>GEN, generic</item>
    /// </list>
    /// <b>Polymers</b>
    /// <list type="bullet">
    /// <item>SRU, SRU type</item>
    /// <item>MON, monomer</item>
    /// <item>MER, Mer type</item>
    /// <item>COP, copolymer</item>
    /// <item>CRO, crosslink</item>
    /// <item>MOD, modification</item>
    /// <item>GRA, graft</item>
    /// <item>ANY, any polymer</item> 
    /// </list>
    /// <b>Components, Mixtures, and formulations</b>
    /// <list type="bullet">
    /// <item>COM, component</item>
    /// <item>MIX, mixture</item>
    /// <item>FOR, formulation</item>
    /// </list>
    /// <b>Non-chemical</b>
    /// <list type="bullet">
    /// <item>DAT, data Sgroup</item>
    /// </list>
    /// </remarks>
    public partial struct SgroupType : System.IComparable<SgroupType>, System.IComparable
    {
		/// <summary>
		/// The <see cref="Ordinal"/> values of <see cref="SgroupType"/>.
		/// </summary>
		/// <seealso cref="SgroupType"/>
        public static class O
        {
            public const int Nil = 0;
            public const int CtabAbbreviation = 1;
            public const int CtabMultipleGroup = 2;
            public const int CtabStructureRepeatUnit = 3;
            public const int CtabMonomer = 4;
            public const int CtabModified = 5;
            public const int CtabCopolymer = 6;
            public const int CtabMer = 7;
            public const int CtabCrossLink = 8;
            public const int CtabGraft = 9;
            public const int CtabAnyPolymer = 10;
            public const int CtabComponent = 11;
            public const int CtabMixture = 12;
            public const int CtabFormulation = 13;
            public const int CtabData = 14;
            public const int CtabGeneric = 15;
            public const int ExtMulticenter = 16;
          
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
            "Nil", 
            "CtabAbbreviation", 
            "CtabMultipleGroup", 
            "CtabStructureRepeatUnit", 
            "CtabMonomer", 
            "CtabModified", 
            "CtabCopolymer", 
            "CtabMer", 
            "CtabCrossLink", 
            "CtabGraft", 
            "CtabAnyPolymer", 
            "CtabComponent", 
            "CtabMixture", 
            "CtabFormulation", 
            "CtabData", 
            "CtabGeneric", 
            "ExtMulticenter", 
         
        };

        private SgroupType(int ordinal)
        {
            this.ordinal = ordinal;
        }

        public static explicit operator SgroupType(int ordinal)
        {
            if (!(0 <= ordinal || ordinal < values.Length))
                throw new System.ArgumentOutOfRangeException();
            return values[ordinal];
        }

        public static explicit operator int(SgroupType obj)
        {
            return obj.Ordinal;
        }

        public static readonly SgroupType Nil = new SgroupType(0);
        /// <summary>
        /// SUP, abbreviation Sgroup (formerly called superatom)
        /// </summary>
        public static readonly SgroupType CtabAbbreviation = new SgroupType(1);
        /// <summary>
        /// MUL, multiple group
        /// </summary>
        public static readonly SgroupType CtabMultipleGroup = new SgroupType(2);
        /// <summary>
        /// GEN, generic
        /// </summary>
        public static readonly SgroupType CtabStructureRepeatUnit = new SgroupType(3);
        /// <summary>
        /// SRU, SRU type
        /// </summary>
        public static readonly SgroupType CtabMonomer = new SgroupType(4);
        /// <summary>
        /// MON, monomer
        /// </summary>
        public static readonly SgroupType CtabModified = new SgroupType(5);
        /// <summary>
        /// MER, Mer type
        /// </summary>
        public static readonly SgroupType CtabCopolymer = new SgroupType(6);
        /// <summary>
        /// COP, copolymer
        /// </summary>
        public static readonly SgroupType CtabMer = new SgroupType(7);
        /// <summary>
        /// CRO, crosslink
        /// </summary>
        public static readonly SgroupType CtabCrossLink = new SgroupType(8);
        /// <summary>
        /// MOD, modification
        /// </summary>
        public static readonly SgroupType CtabGraft = new SgroupType(9);
        /// <summary>
        /// GRA, graft
        /// </summary>
        public static readonly SgroupType CtabAnyPolymer = new SgroupType(10);
        /// <summary>
        /// ANY, any polymer
        /// </summary>
        public static readonly SgroupType CtabComponent = new SgroupType(11);
        /// <summary>
        /// COM, component
        /// </summary>
        public static readonly SgroupType CtabMixture = new SgroupType(12);
        /// <summary>
        /// MIX, mixture
        /// </summary>
        public static readonly SgroupType CtabFormulation = new SgroupType(13);
        /// <summary>
        /// FOR, formulation
        /// </summary>
        public static readonly SgroupType CtabData = new SgroupType(14);
        /// <summary>
        /// DAT, data Sgroup
        /// </summary>
        public static readonly SgroupType CtabGeneric = new SgroupType(15);
        public static readonly SgroupType ExtMulticenter = new SgroupType(16);
        private static readonly SgroupType[] values = new SgroupType[]
        {
            Nil, 
            CtabAbbreviation, 
            CtabMultipleGroup, 
            CtabStructureRepeatUnit, 
            CtabMonomer, 
            CtabModified, 
            CtabCopolymer, 
            CtabMer, 
            CtabCrossLink, 
            CtabGraft, 
            CtabAnyPolymer, 
            CtabComponent, 
            CtabMixture, 
            CtabFormulation, 
            CtabData, 
            CtabGeneric, 
            ExtMulticenter, 
    
        };
        public static System.Collections.Generic.IEnumerable<SgroupType> Values => values;

        /* In order to cause compiling error */

        public static bool operator==(SgroupType a, object b)
        {
            throw new System.Exception();
        }

        public static bool operator!=(SgroupType a, object b)
        {
            throw new System.Exception();
        }

        public static bool operator==(object a, SgroupType b)
        {
            throw new System.Exception();
        }

        public static bool operator!=(object a, SgroupType b)
        {
            throw new System.Exception();
        }

        public static bool operator==(SgroupType a, SgroupType b)
        {
            
            return a.Ordinal == b.Ordinal;
        }

        public static bool operator !=(SgroupType a, SgroupType b)
        {
            return !(a == b);
        }

		/// <inheritdoc/>
        public override bool Equals(object obj)
        {
    
            if (!(obj is SgroupType))
                return false;
            return this.Ordinal == ((SgroupType)obj).Ordinal;
        }

		/// <inheritdoc/>
        public override int GetHashCode()
        {
            return Ordinal;
        }

		/// <inheritdoc/>
        public int CompareTo(object obj)
        {
            var o = (SgroupType)obj;
            return ((int)Ordinal).CompareTo((int)o.Ordinal);
        }   

		/// <inheritdoc/>
        public int CompareTo(SgroupType o)
        {
            return (Ordinal).CompareTo(o.Ordinal);
        }   	
	}
	public partial struct SgroupType 
    {
        private static Dictionary<string, int> map;

        private static string[] ctabKeys = new string[]
        {
            "",
           "SUP",
           "MUL",
           "SRU",
           "MON",
           "MOD", 
           "COP", 
           "MER", 
           "CRO",
           "GRA", 
           "ANY", 
           "COM",
           "MIX",
           "FOR",
           "DAT",
           "GEN", 
           "N/A",
        };

        static SgroupType()
        {
            map = new Dictionary<string, int>();

            for (int i = 0; i < O.ExtMulticenter; i++)
                map.Add(ctabKeys[i], i);
        }
        
        public string Key => ctabKeys[Ordinal];

        public static SgroupType ParseCtabKey(string str)
        {
            int o;
            if (!map.TryGetValue(str, out o))
            {
                o = SgroupType.CtabGeneric.Ordinal;
            }
            return values[o];
        }
    }
}


