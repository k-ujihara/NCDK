/*
 * Copyright (C) 2010  Rajarshi Guha <rajarshi.guha@gmail.com>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All I ask is that proper credit is given for my work, which includes
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
using NCDK.Aromaticities;
using NCDK.Graphs;
using NCDK.Smiles;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NCDK.Fingerprints
{
    /// <summary>
    /// An implementation of the LINGO fingerprint <token>cdk-cite-Vidal2005</token>.
    /// </summary>
    /// <remarks>
    /// While the current
    /// implementation converts ring closure symbols to 0's it does not convert 2-letter element symbols
    /// to single letters (ala OpenEye).
    /// </remarks>
    // @author Rajarshi Guha
    // @cdk.module smiles
    // @cdk.keyword fingerprint
    // @cdk.keyword hologram
    // @cdk.githash
    public class LingoFingerprinter : IFingerprinter
    {
        private readonly int n;
        private readonly SmilesGenerator gen = SmilesGenerator.Unique().Aromatic();
        private readonly Regex DIGITS = new Regex("[0-9]+", RegexOptions.Compiled);
        private readonly Aromaticity aromaticity = new Aromaticity(ElectronDonation.DaylightModel, Cycles.Or(Cycles.AllFinder, Cycles.RelevantFinder));

        /// <summary>
        /// Initialize the fingerprinter with a default substring length of 4.
        /// </summary>
        public LingoFingerprinter()
            : this(4)
        { }

        /// <summary>
        /// Initialize the fingerprinter.
        /// </summary>
        /// <param name="n">The length of substrings to consider</param>
        public LingoFingerprinter(int n)
        {
            this.n = n;
        }

        public IBitFingerprint GetBitFingerprint(IAtomContainer iAtomContainer)
        {
            return FingerprinterTool.MakeBitFingerprint(GetRawFingerprint(iAtomContainer));
        }

        public IDictionary<string, int> GetRawFingerprint(IAtomContainer atomContainer)
        {
            aromaticity.Apply(atomContainer);
            string smiles = ReplaceDigits(gen.Create(atomContainer));
            IDictionary<string, int> map = new Dictionary<string, int>();
            for (int i = 0, l = smiles.Length - n + 1; i < l; i++)
            {
                string subsmi = smiles.Substring(i, n);
                int count;
                if (!map.TryGetValue(subsmi, out count))
                    map[subsmi] = 1;
                else
                    map[subsmi] = count + 1;
            }
            return map;
        }

        public int Count => -1; // 1L << 32

        private string ReplaceDigits(string smiles)
        {
            return DIGITS.Replace(smiles, "0");
        }

        public ICountFingerprint GetCountFingerprint(IAtomContainer container)
        {
            return FingerprinterTool.MakeCountFingerprint(GetRawFingerprint(container));
        }
    }
}
