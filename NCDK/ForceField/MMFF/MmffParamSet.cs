/*
 * Copyright (c) 2015 John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using NCDK.Common.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NCDK.ForceField.MMFF
{
    /**
    // Internal class for accessing MMFF parameters.
    // 
    // @author John May
     */
#if TEST
        public
#endif
    sealed class MmffParamSet
    {
        public static readonly MmffParamSet Instance = new MmffParamSet();

        private const int MAX_MMFF_ATOMTYPE = 99;


        /**
        // Bond charge increments.
         */
        private IDictionary<BondKey, decimal> bcis = new Dictionary<BondKey, decimal>();

        /**
        // Atom type properties.
         */
        private MmffProp[] properties = new MmffProp[MAX_MMFF_ATOMTYPE + 1];

        private IDictionary<string, int> typeMap = new Dictionary<string, int>();

        /**
        // Symbolic formal charges - some are varible and assigned in code.
         */
        private IDictionary<string, decimal> fCharges = new Dictionary<string, decimal>();

        MmffParamSet()
        {
            using (Stream in_ = GetType().Assembly.GetManifestResourceStream(GetType(), "MMFFCHG.PAR"))
            {
                ParseMMFFCHARGE(in_, bcis);
            }
            using (Stream in_ = GetType().Assembly.GetManifestResourceStream(GetType(), "MMFFFORMCHG.PAR"))
            {
                ParseMMFFFORMCHG(in_, fCharges);
            }
            using (Stream in_ = GetType().Assembly.GetManifestResourceStream(GetType(), "MMFFPROP.PAR"))
            {
                ParseMMFFPPROP(in_, properties);
            }
            using (Stream in_ = GetType().Assembly.GetManifestResourceStream(GetType(), "MMFFPBCI.PAR"))
            {
                ParseMMFFPBCI(in_, properties);
            }
            using (Stream in_ = GetType().Assembly.GetManifestResourceStream(GetType(), "mmff-symb-mapping.tsv"))
            {
                ParseMMFFTypeMap(in_, typeMap);
            }
        }

        /**
        // Obtain the integer MMFF atom type for a given symbolic MMFF type.
         *
        // @param sym Symbolic MMFF type
        // @return integer MMFF type
         */
        public int IntType(string sym)
        {
            int i;
            if (!typeMap.TryGetValue(sym, out i))
                return 0;
            return i;
        }

        /**
        // Access bond charge increment (bci) for a bond between two atoms (referred
        // to by MMFF integer type).
         *
        // @param cls   bond class
        // @param type1 first atom type
        // @param type2 second atom type
        // @return bci
         */
        public decimal? GetBondChargeIncrement(int cls, int type1, int type2)
        {
            decimal ret;
            if (!bcis.TryGetValue(new BondKey(cls, type1, type2), out ret))
                return null;
            return ret;
        }

        /**
        // Access Partial Bond Charge Increments (pbci).
         *
        // @param atype integer atom type
        // @return pbci
         */
        public decimal GetPartialBondChargeIncrement(int atype)
        {
            return properties[CheckType(atype)].pbci;
        }

        /**
        // Access Formal charge adjustment factor.
         *
        // @param atype integer atom type
        // @return adjustment factor
         */
        public decimal GetFormalChargeAdjustment(int atype)
        {
            return properties[CheckType(atype)].fcAdj;
        }

        /**
        // Access the CRD for an MMFF int type.
        // 
        // @param atype int atom type
        // @return CRD
         */
        public int GetCrd(int atype)
        {
            return properties[CheckType(atype)].crd;
        }

        /**
        // Access the tabulated formal charge (may be fractional) for
        // a symbolic atom type. Some formal charges are variable and
        // need to be implemented in code.
        // 
        // @param symb symbolic type
        // @return formal charge
         */
        public decimal? GetFormalCharge(string symb)
        {
            decimal ret;
            if (!fCharges.TryGetValue(symb, out ret))
                return null;
            return ret;
        }

        /**
        // see. MMFF Part V - p 620, a nonstandard bond-type index of “1” is
        // assigned whenever a single bond (formal bond order 1) is found: (a)
        // between atoms i and j of types that are not both aromatic and for which
        // ”sbmb” entries of ”1” appear in Table I; or (b) between pairs of atoms
        // belonging to different aromatic rings (as in the case of the connecting
        // C-C bond in biphenyl).
         */
        public int GetBondCls(int type1, int type2, int bord, bool barom)
        {
            MmffProp prop1 = properties[CheckType(type1)];
            MmffProp prop2 = properties[CheckType(type2)];
            // non-arom atoms with sbmb (single-bond-multi-bond)
            if (bord == 1 && !prop1.arom && prop1.sbmb && !prop2.arom && prop2.sbmb)
                return 1;
            // non-arom bond between arom atoms
            if (bord == 1 && !barom && prop1.arom && prop2.arom)
                return 1;
            return 0;
        }

        private int CheckType(int atype)
        {
            if (atype < 0 || atype > MAX_MMFF_ATOMTYPE)
                throw new ArgumentException("Invalid MMFF atom type:" + atype);
            return atype;
        }

        private static void ParseMMFFCHARGE(Stream in_, IDictionary<BondKey, decimal> map)
        {
            using (var br = new StreamReader(in_, System.Text.Encoding.UTF8))
            {
                string line;
                while ((line = br.ReadLine()) != null)
                {
                    if (line.Length == 0 || line[0] == '*')
                        continue;
                    var cols = Strings.Tokenize(line);
                    if (cols.Count != 5)
                        throw new IOException("Malformed MMFFBOND.PAR file.");
                    BondKey key = new BondKey(int.Parse(cols[0]),
                                                   int.Parse(cols[1]),
                                                   int.Parse(cols[2]));
                    decimal bci = decimal.Parse(cols[3]);
                    map[key] = bci;
                    map[key.Inv()] = -bci;
                }
            }
        }

        private static void ParseMMFFPBCI(Stream in_, MmffProp[] props)
        {
            using (var br = new StreamReader(in_, Encoding.UTF8))
            {
                string line;
                while ((line = br.ReadLine()) != null)
                {
                    if (line.Length == 0 || line[0] == '*')
                        continue;
                    var cols = Strings.Tokenize(line);
                    if (cols.Count < 5)
                        throw new IOException("Malformed MMFFPCBI.PAR file.");
                    int type = int.Parse(cols[1]);
                    props[type].pbci = decimal.Parse(cols[2]);
                    props[type].fcAdj = decimal.Parse(cols[3]);
                }
            }
        }

        private static void ParseMMFFPPROP(Stream in_, MmffProp[] props)
        {
            using (var br = new StreamReader(in_, Encoding.UTF8))
            {
                string line;
                while ((line = br.ReadLine()) != null)
                {
                    if (line.Length == 0 || line[0] == '*')
                        continue;
                    var cols = Strings.Tokenize(line);
                    if (cols.Count != 9)
                        throw new IOException("Malformed MMFFPROP.PAR file.");
                    int type = int.Parse(cols[0]);
                    props[type] = new MmffProp(int.Parse(cols[1]), 
                                               int.Parse(cols[2]),
                                               int.Parse(cols[3]),
                                               int.Parse(cols[4]),
                                               int.Parse(cols[5]),
                                               int.Parse(cols[6]),
                                               int.Parse(cols[7]),
                                               int.Parse(cols[8]));
                }
            }
        }

        private static void ParseMMFFTypeMap(Stream in_, IDictionary<string, int> types)
        {
            using (var br = new StreamReader(in_, Encoding.UTF8))
            {
                string line = br.ReadLine(); // header
                while ((line = br.ReadLine()) != null)
                {
                    if (line.Length == 0 || line[0] == '*')
                        continue;
                    var cols = Strings.Tokenize(line, '\t');
                    int intType = int.Parse(cols[1]);
                    types[cols[0]] = intType;
                    types[cols[2]] = intType;
                }
            }
        }

        private static void ParseMMFFFORMCHG(Stream in_, IDictionary<string, decimal> fcharges)
        {
            using (var br = new StreamReader(in_, Encoding.UTF8))
            {
                string line = br.ReadLine(); // header
                while ((line = br.ReadLine()) != null)
                {
                    if (line.Length == 0 || line[0] == '*')
                        continue;
                    var cols = Strings.Tokenize(line);
                    fcharges[cols[0]] = decimal.Parse(cols[1]);
                }
            }
        }

        /**
        // Key for indexing bond parameters by
         */
        sealed class BondKey
        {

            /// <summary>Bond class.</summary>
            private readonly int cls;

            /**
            // MMFF atom types for the bond.
             */
            private readonly int type1, type2;


            public BondKey(int cls, int type1, int type2)
            {
                this.cls = cls;
                this.type1 = type1;
                this.type2 = type2;
            }

            public BondKey Inv()
            {
                return new BondKey(cls, type2, type1);
            }

            public override bool Equals(object o)
            {
                if (this == o) return true;
                if (o == null || GetType() != o.GetType()) return false;

                BondKey bondKey = (BondKey)o;

                if (cls != bondKey.cls) return false;
                if (type1 != bondKey.type1) return false;
                return type2 == bondKey.type2;

            }

            public override int GetHashCode()
            {
                int result = cls;
                result = 31 * result + type1;
                result = 31 * result + type2;
                return result;
            }
        }

        /**
        // Properties of an MMFF atom type.
         */
        private sealed class MmffProp
        {
            public readonly int aspec;
            public readonly int crd;
            public readonly int val;
            public readonly int pilp;
            public readonly int mltb;
            public readonly bool arom;
            public readonly bool lin;
            public readonly bool sbmb;
            public decimal pbci;
            public decimal fcAdj;

            public MmffProp(int aspec, int crd, int val, int pilp, int mltb, int arom, int lin, int sbmb)
            {
                this.aspec = aspec;
                this.crd = crd;
                this.val = val;
                this.pilp = pilp;
                this.mltb = mltb;
                this.arom = arom != 0;
                this.lin = lin != 0;
                this.sbmb = sbmb != 0;
            }
        }
    }
}
