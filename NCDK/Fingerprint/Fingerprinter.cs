/* Copyright (C) 2002-2007  Christoph Steinbeck <steinbeck@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
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
using NCDK.Aromaticities;
using NCDK.Common.Mathematics;
using NCDK.Common.Primitives;
using NCDK.Graphs;
using NCDK.RingSearches;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NCDK.Fingerprint
{
    /**
	 *  Generates a fingerprint for a given AtomContainer. Fingerprints are
	 *  one-dimensional bit arrays, where bits are set according to a the
	 *  occurrence of a particular structural feature (See for example the
	 *  Daylight inc. theory manual for more information). Fingerprints allow for
	 *  a fast screening step to exclude candidates for a substructure search in a
	 *  database. They are also a means for determining the similarity of chemical
	 *  structures. <p>
	 *
	 *  A fingerprint is generated for an AtomContainer with this code: <pre>
	 *   Molecule molecule = new Molecule();
	 *   IFingerprinter fingerprinter = new Fingerprinter();
	 *   IBitFingerprint fingerprint = fingerprinter.GetBitFingerprint(molecule);
	 *   fingerprint.Count; // returns 1024 by default
	 *   fingerprint.Length(); // returns the highest set bit
	 * </pre> <p>
	 *
	 *  The FingerPrinter assumes that hydrogens are explicitly given! Furthermore,
	 *  if pseudo atoms or atoms with malformed symbols are present, their atomic
	 *  number is taken as one more than the last element currently supported in
	 *  {@link org.openscience.cdk.tools.periodictable.PeriodicTable}.
	 *
	 *  <font color="#FF0000">Warning: The aromaticity detection for this
	 *  FingerPrinter relies on AllRingsFinder, which is known to take very long
	 *  for some molecules with many cycles or special cyclic topologies. Thus,
	 *  the AllRingsFinder has a built-in timeout of 5 seconds after which it
	 *  aborts and  Exception. If you want your SMILES generated at any
	 *  expense, you need to create your own AllRingsFinder, set the timeout to a
	 *  higher value, and assign it to this FingerPrinter. In the vast majority of
	 *  cases, however, the defaults will be fine. </font> <p>
	 *
	 *  <font color="#FF0000">Another Warning : The daylight manual says:
	 *  "Fingerprints are not so definite: if a fingerprint indicates a pattern is
	 *  missing then it certainly is, but it can only indicate a pattern's presence
	 *  with some probability." In the case of very small molecules, the
	 *  probability that you get the same fingerprint for different molecules is
	 *  high. </font>
	 *  </p>
	 *
	 * @author         steinbeck
	 * @cdk.created    2002-02-24
	 * @cdk.keyword    fingerprint
	 * @cdk.keyword    similarity
	 * @cdk.module     standard
	 * @cdk.githash
	 */
    public class Fingerprinter : IFingerprinter
    {
        /// <summary>Throw an exception if too many paths (per atom) are generated.</summary>
        private const int DEFAULT_PATH_LIMIT = 1500;

        /// <summary>The default length of created fingerprints.</summary>
        public const int DEFAULT_SIZE = 1024;
        /// <summary>The default search depth used to create the fingerprints.</summary>
        public const int DEFAULT_SEARCH_DEPTH = 8;

        private int size;
        private int searchDepth;
        private int pathLimit = DEFAULT_PATH_LIMIT;

        private static readonly IDictionary<string, string> QUERY_REPLACE = new Dictionary<string, string>() {
            { "Cl", "X" },
            { "Br", "Z" },
            { "Si", "Y" },
            { "As", "D" },
            { "Li", "L" },
            { "Se", "E" },
            { "Na", "G" },
            { "Ca", "J" },
            { "Al", "A" },
        };

        /**
		 * Creates a fingerprint generator of length <code>DEFAULT_SIZE</code>
		 * and with a search depth of <code>DEFAULT_SEARCH_DEPTH</code>.
		 */
        public Fingerprinter()
            : this(DEFAULT_SIZE, DEFAULT_SEARCH_DEPTH)
        { }

        public Fingerprinter(int size)
            : this(size, DEFAULT_SEARCH_DEPTH)
        { }

        /**
		 * Constructs a fingerprint generator that creates fingerprints of
		 * the given size, using a generation algorithm with the given search
		 * depth.
		 *
		 * @param  size        The desired size of the fingerprint
		 * @param  searchDepth The desired depth of search
		 */
        public Fingerprinter(int size, int searchDepth)
        {
            this.size = size;
            this.searchDepth = searchDepth;
        }

        /**
		 * Generates a fingerprint of the default size for the given AtomContainer.
		 *
		 * @param container The AtomContainer for which a Fingerprint is generated
		 * @param ringFinder An instance of
		 *                   {@link org.openscience.cdk.ringsearch.AllRingsFinder}
		 * @exception CDKException if there is a timeout in ring or aromaticity
		 *                         perception
		 * @return A {@link BitArray} representing the fingerprint
		 */
        public IBitFingerprint GetBitFingerprint(IAtomContainer container, AllRingsFinder ringFinder)
        {
            int position = -1;
            Debug.WriteLine("Entering Fingerprinter");
            Debug.WriteLine("Starting Aromaticity Detection");
            long before = DateTime.Now.Ticks;
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(container);
            Aromaticity.CDKLegacy.Apply(container);
            long after = DateTime.Now.Ticks;
            Debug.WriteLine("time for aromaticity calculation: " + (after - before) + " ticks");
            Debug.WriteLine("Finished Aromaticity Detection");
            BitArray bitSet = new BitArray(size);

            int[] hashes = FindPathes(container, searchDepth);
            foreach (var hash in hashes)
            {
                position = new JavaRandom(hash).Next(size);
                bitSet.Set(position, true);
            }

            return new BitSetFingerprint(bitSet);
        }

        /**
		 * Generates a fingerprint of the default size for the given AtomContainer.
		 *
		 *@param container The AtomContainer for which a Fingerprint is generated
		 */

        public IBitFingerprint GetBitFingerprint(IAtomContainer container)
        {
            return GetBitFingerprint(container, null);
        }

        /// <inheritdoc/>

        public IDictionary<string, int> GetRawFingerprint(IAtomContainer iAtomContainer)
        {
            throw new NotSupportedException();
        }

        /**
		 * Get all paths of lengths 0 to the specified length.
		 *
		 * This method will find all paths up to length N starting from each
		 * atom in the molecule and return the unique set of such paths.
		 *
		 * @param container The molecule to search
		 * @param searchDepth The maximum path length desired
		 * @return A IDictionary of path strings, keyed on themselves
		 */
        protected int[] FindPathes(IAtomContainer container, int searchDepth)
        {
            List<string> allPaths = new List<string>();

            IDictionary<IAtom, IDictionary<IAtom, IBond>> cache = new Dictionary<IAtom, IDictionary<IAtom, IBond>>();

            foreach (var startAtom in container.Atoms)
            {
                IList<IList<IAtom>> p = PathTools.GetLimitedPathsOfLengthUpto(container, startAtom, searchDepth, pathLimit);
                foreach (var path in p)
                {
                    StringBuilder sb = new StringBuilder();
                    IAtom x = path[0];

                    // TODO if we ever get more than 255 elements, this will
                    // fail maybe we should use 0 for pseudo atoms and
                    // malformed symbols? - nope a char 16 bit, up to 65,535
                    // is okay :)
                    if (x is IPseudoAtom)
                        sb.Append((char)PeriodicTable.ElementCount + 1);
                    else
                    {
                        int atnum = PeriodicTable.GetAtomicNumber(x.Symbol);
                        // fixed CDK bug
                        if (atnum != 0)
                            sb.Append(ConvertSymbol(x.Symbol));
                        else
                            sb.Append((char)(PeriodicTable.ElementCount + 1));
                    }

                    for (int i_ = 1; i_ < path.Count; i_++)
                    {
                        IAtom[] y = { path[i_] };
                        IBond[] b;
                        IDictionary<IAtom, IBond> m;
                        {
                            IBond val = null;
                            if (cache.TryGetValue(x, out m))
                                if (m.ContainsKey(y[0]))
                                    val = m[y[0]];
                            b = new IBond[] { val };
                        }
                        if (b[0] == null)
                        {
                            b[0] = container.GetBond(x, y[0]);
                            var dic = new Dictionary<IAtom, IBond>();
                            dic[y[0]] = b[0];
                            cache[x] = dic;
                        }
                        sb.Append(GetBondSymbol(b[0]));
                        sb.Append(ConvertSymbol(y[0].Symbol));
                        x = y[0];
                    }

                    // we store the lexicographically lower one of the
                    // string and its reverse
                    var sb_str = sb.ToString();
                    var rev_str = Strings.Reverse(sb_str);
                    if (string.Compare(sb_str, rev_str, StringComparison.Ordinal) <= 0)  
                        allPaths.Add(sb_str);
                    else
                        allPaths.Add(rev_str);
                }
            }
            // now lets clean stuff up
            ICollection<string> cleanPath = new HashSet<string>();
            foreach (var s in allPaths)
            {
                string s1 = s.ToString().Trim();
                if (s1.Equals("")) continue;
                if (cleanPath.Contains(s1)) continue;
                string s2 = Strings.Reverse(s).ToString().Trim();
                if (cleanPath.Contains(s2)) continue;
                cleanPath.Add(s2);
            }

            // convert paths to hashes
            int[] hashes = new int[cleanPath.Count];
            int i = 0;
            foreach (var s in cleanPath)
                hashes[i++] = Strings.GetJavaHashCode(s);

            return hashes;
        }

        private string ConvertSymbol(string symbol)
        {
            string returnSymbol;
            if (!QUERY_REPLACE.TryGetValue(symbol, out returnSymbol))
                return symbol;
            return returnSymbol;
        }

        /**
		 *  Gets the bondSymbol attribute of the Fingerprinter class
		 *
		 *@param  bond  Description of the Parameter
		 *@return       The bondSymbol value
		 */
        protected virtual string GetBondSymbol(IBond bond)
        {
            string bondSymbol = "";
            if (bond.IsAromatic)
            {
                bondSymbol = ":";
            }
            else if (bond.Order == BondOrder.Single)
            {
                bondSymbol = "-";
            }
            else if (bond.Order == BondOrder.Double)
            {
                bondSymbol = "=";
            }
            else if (bond.Order == BondOrder.Triple)
            {
                bondSymbol = "#";
            }
            return bondSymbol;
        }

        public void SetPathLimit(int limit)
        {
            this.pathLimit = limit;
        }

        public int SearchDepth => searchDepth;

        public int Count => size;

        public ICountFingerprint GetCountFingerprint(IAtomContainer container)
        {
            throw new NotSupportedException();
        }
    }
}

