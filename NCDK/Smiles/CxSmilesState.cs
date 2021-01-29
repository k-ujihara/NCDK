/*
 * Copyright (c) 2016 John May <jwmay@users.sf.net>
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
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using NCDK.Common.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NCDK.Smiles
{
    /// <summary>
    /// Light-weight intermediate data-structure for transferring information CDK to/from
    /// CXSMILES.
    /// </summary>
    internal sealed class CxSmilesState
    {
        internal SortedDictionary<int, string> atomLabels = null;
        internal SortedDictionary<int, string> atomValues = null;
        internal List<double[]> atomCoords  = null;
        internal List<List<int>> fragGroups = null;
        internal SortedDictionary<int, Radical> atomRads = null;
        internal SortedDictionary<int, IList<int>> ligandOrdering = null;
        internal SortedDictionary<int, IList<int>> positionVar = null;
        internal List<CxSgroup> mysgroups = null;
        internal bool coordFlag = false;

        public enum Radical
        {
            Monovalent,
            Divalent,
            DivalentSinglet,
            DivalentTriplet,
            Trivalent,
            TrivalentDoublet,
            TrivalentQuartet,
        }

        public class CxSgroup
        {
            internal HashSet<CxSgroup> children = new HashSet<CxSgroup>();
            internal List<int> atoms = new List<int>();
            internal int id = -1;
        }

        public sealed class CxDataSgroup : CxSgroup
        {
            internal readonly string field;
            internal readonly string value;
            internal readonly string operator_;
            internal readonly string unit;
            internal readonly string tag;

            public CxDataSgroup(List<int> atoms, string field, string value, string operator_, string unit, string tag)
            {
                this.atoms = atoms;
                this.field = field;
                this.value = value;
                this.operator_ = operator_;
                this.unit = unit;
                this.tag = tag;
            }

            public override bool Equals(object o)
            {
                if (!(o is CxDataSgroup that))
                    return false;

                if (atoms != null ? !Compares.AreEqual(atoms, that.atoms) : that.atoms != null)
                    return false;
                if (field != null ? !field.Equals(that.field, StringComparison.Ordinal) : that.field != null)
                    return false;
                if (value != null ? !value.Equals(that.value, StringComparison.Ordinal) : that.value != null)
                    return false;
                if (operator_ != null ? !operator_.Equals(that.operator_, StringComparison.Ordinal) : that.operator_ != null)
                    return false;
                if (unit != null ? !unit.Equals(that.unit, StringComparison.Ordinal) : that.unit != null)
                    return false;
                return tag != null ? tag.Equals(that.tag, StringComparison.Ordinal) : that.tag == null;
            }

            public override int GetHashCode()
            {
                int result = atoms != null ? atoms.GetHashCode() : 0;
                result = 31 * result + (field != null ? field.GetHashCode() : 0);
                result = 31 * result + (value != null ? value.GetHashCode() : 0);
                result = 31 * result + (operator_ != null ? operator_.GetHashCode() : 0);
                result = 31 * result + (unit != null ? unit.GetHashCode() : 0);
                result = 31 * result + (tag != null ? tag.GetHashCode() : 0);
                return result;
            }

            public override string ToString()
            {
                return "DataSgroup{" +
                       "atoms=" + atoms +
                       ", field='" + field + '\'' +
                       ", value='" + value + '\'' +
                       ", operator='" + operator_ + '\'' +
                       ", unit='" + unit + '\'' +
                       ", tag='" + tag + '\'' +
                       '}';
            }

            internal string Field => field;
            internal string Value => value;
        }

        public sealed class CxPolymerSgroup : CxSgroup
        {
            internal readonly string type;
            internal readonly string subscript;
            internal readonly string supscript;

            public CxPolymerSgroup(string type, IList<int> atomset, string subscript, string supscript)
            {
                Trace.Assert(type != null && atomset != null);
                this.type = type;
                this.atoms = new List<int>(atomset);
                this.subscript = subscript;
                this.supscript = supscript;
            }

            public override bool Equals(object o)
            {
                if (!(o is CxPolymerSgroup that))
                    return false;

                return type.Equals(that.type, StringComparison.Ordinal) &&
                       Compares.AreEqual(atoms, that.atoms) &&
                       subscript.Equals(that.subscript, StringComparison.Ordinal) &&
                       supscript.Equals(that.supscript, StringComparison.Ordinal);
            }

            public override int GetHashCode()
            {
                int result = type.GetHashCode();
                foreach (var a in atoms)
                    result = 31 * result + a.GetHashCode();
                result = 31 * result + subscript.GetHashCode();
                result = 31 * result + supscript.GetHashCode();
                return result;
            }

            public override string ToString()
            {
                return "PolymerSgroup{" +
                       "type='" + type + '\'' +
                       ", atomset=" + atoms +
                       ", subscript='" + subscript + '\'' +
                       ", supscript='" + supscript + '\'' +
                       '}';
            }
        }

        internal static string Escape(string str)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (IsEscapeChar(c))
                    sb.Append("&#").Append((int)c).Append(';');
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        private static bool IsEscapeChar(char c)
        {
            return c < 32 || c > 126 || c == '|' || c == '{' || c == '}' || c == ',' || c == ';' || c == ':' || c == '$';
        }
    }
}