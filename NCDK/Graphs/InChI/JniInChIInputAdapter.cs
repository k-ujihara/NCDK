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
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */
using NCDK.Common.Primitives;
using NCDK.NInChI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NCDK.Graphs.InChI
{
    public class JniInChIInputAdapter : NInchiInput
    {
        public const string FIVE_SECOND_TIMEOUT = "-W5";

        public JniInChIInputAdapter(string options)
        {
            this.Options = options == null ? "" : CheckOptions(options);
        }

        public JniInChIInputAdapter(IList<INCHI_OPTION> options)
        {
            this.Options = options == null ? "" : CheckOptions(options);
        }

        private static bool IsTimeoutOptions(string op)
        {
            if (op == null || op.Length < 2) return false;
            int pos = 0;
            int len = op.Length;
            if (op[pos] == 'W')
                pos++;
            while (pos < len && char.IsDigit(op[pos]))
                pos++;
            if (pos < len && op[pos] == '.')
                pos++;
            while (pos < len && char.IsDigit(op[pos]))
                pos++;
            return pos == len;
        }

        private static string CheckOptions(string ops)
        {
            if (ops == null)
            {
                throw new ArgumentNullException(nameof(ops));
            }
            StringBuilder sbOptions = new StringBuilder();

            bool hasUserSpecifiedTimeout = false;

            var tok = Strings.Tokenize(ops).GetEnumerator();
            while (tok.MoveNext())
            {
                string op = tok.Current;
                if (op.StartsWith("-") || op.StartsWith("/"))
                {
                    op = op.Substring(1);
                }

                INCHI_OPTION option = INCHI_OPTION.ValueOfIgnoreCase(op);
                if (option != null)
                {
                    sbOptions.Append('-').Append(option.Name);
                    if (tok.MoveNext())
                    {
                        sbOptions.Append(" ");
                    }
                }
                else if (IsTimeoutOptions(op))
                {
                    sbOptions.Append('-').Append(op);
                    hasUserSpecifiedTimeout = true;
                    if (tok.MoveNext())
                    {
                        sbOptions.Append(" ");
                    }
                }
                // 1,5 tautomer option
                else if ("15T".Equals(op))
                {
                    sbOptions.Append('-').Append("15T");
                    if (tok.MoveNext())
                    {
                        sbOptions.Append(" ");
                    }
                }
                // keto-enol tautomer option
                else if ("KET".Equals(op))
                {
                    sbOptions.Append('-').Append("KET");
                    if (tok.MoveNext())
                    {
                        sbOptions.Append(" ");
                    }
                }
                else
                {
                    throw new NInchiException("Unrecognised InChI option");
                }
            }

            if (!hasUserSpecifiedTimeout)
            {
                if (sbOptions.Length > 0)
                    sbOptions.Append(' ');
                sbOptions.Append(FIVE_SECOND_TIMEOUT);
            }

            return sbOptions.ToString();
        }

        private static string CheckOptions(IList<INCHI_OPTION> ops)
        {
            if (ops == null)
            {
                throw new ArgumentException("Null options");
            }
            StringBuilder sbOptions = new StringBuilder();

            foreach (INCHI_OPTION op in ops)
            {
                sbOptions.Append('-').Append(op.Name).Append(" ");
            }

            if (sbOptions.Length > 0)
                sbOptions.Append(' ');
            sbOptions.Append(FIVE_SECOND_TIMEOUT);

            return sbOptions.ToString();
        }
    }
}
