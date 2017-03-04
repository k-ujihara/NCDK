/* Copyright (C) 2002-2007  Christoph Steinbeck <steinbeck@users.sf.net>
 *               2009-2011  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Common.Mathematics;
using NCDK.Common.Primitives;
using NCDK.Graphs;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCDK.Fingerprint
{
    /// <summary>
    /// Generates a fingerprint for a given <see cref="IAtomContainer"/>. Fingerprints are
    /// one-dimensional bit arrays, where bits are set according to a the occurrence
    /// of a particular structural feature (See for example the Daylight inc. theory
    /// manual for more information). Fingerprints allow for a fast screening step to
    /// exclude candidates for a substructure search in a database. They are also a
    /// means for determining the similarity of chemical structures.
    /// </summary>
    /// <example>
    /// A fingerprint is generated for an AtomContainer with this code:
    /// <code>
    ///   Molecule molecule = new Molecule();
    ///   IFingerprinter fingerprinter = new HybridizationFingerprinter();
    ///   BitArray fingerprint = fingerprinter.GetFingerprint(molecule);
    ///   fingerprint.Count; // returns 1024 by default
    ///   fingerprint.Length(); // returns the highest set bit
    /// </code>
    /// </example>
    /// <remarks>The FingerPrinter assumes that hydrogens are explicitly given!
    /// Furthermore, if pseudo atoms or atoms with malformed symbols are present,
    /// their atomic number is taken as one more than the last element currently
    /// supported in <see cref="PeriodicTable"/>.
    /// <para>Unlike the <see cref="Fingerprinter"/>, this fingerprinter does not take into
    /// account aromaticity. Instead, it takes into account SP2 <see cref="Hybridization"/>.
    /// </para>
    /// </remarks>
    // @cdk.keyword    fingerprint
    // @cdk.keyword    similarity
    // @cdk.module     standard
    // @cdk.githash
    public class HybridizationFingerprinter : IFingerprinter
    {
        /// <summary>The default length of created fingerprints.</summary>
        public const int DEFAULT_SIZE = 1024;
        /// <summary>The default search depth used to create the fingerprints.</summary>
        public const int DEFAULT_SEARCH_DEPTH = 8;

        private int size;
        private int searchDepth;

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

        /// <summary>
        /// Creates a fingerprint generator of length <see cref="DEFAULT_SIZE"/>
        /// and with a search depth of <see cref="DEFAULT_SEARCH_DEPTH"/>.
        /// </summary>
        public HybridizationFingerprinter()
           : this(DEFAULT_SIZE, DEFAULT_SEARCH_DEPTH)
        { }

        public HybridizationFingerprinter(int size)
            : this(size, DEFAULT_SEARCH_DEPTH)
        { }

        /// <summary>
        /// Constructs a fingerprint generator that creates fingerprints of
        /// the given size, using a generation algorithm with the given search
        /// depth.
        /// </summary>
        /// <param name="size">The desired size of the fingerprint</param>
        /// <param name="searchDepth">The desired depth of search</param>
        public HybridizationFingerprinter(int size, int searchDepth)
        {
            this.size = size;
            this.searchDepth = searchDepth;
        }

        /// <summary>
        /// Generates a fingerprint of the default size for the given AtomContainer.
        /// </summary>
        /// <param name="container">The <see cref="IAtomContainer"/> for which a fingerprint is generated.</param>
        public IBitFingerprint GetBitFingerprint(IAtomContainer container)
        {
            BitArray bitSet = new BitArray(size);

            IAtomContainer clonedContainer = (IAtomContainer)container.Clone();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureUnsetProperties(clonedContainer);
            int[] hashes = FindPathes(clonedContainer, searchDepth);
            foreach (var hash in hashes)
            {
                bitSet.Set(new JavaRandom(hash).Next(size), true);
            }

            return new BitSetFingerprint(bitSet);
        }

        /// <summary>
        /// Get all paths of lengths 0 to the specified length.
        /// This method will find all paths up to length N starting from each
        /// atom in the molecule and return the unique set of such paths.
        /// </summary>
        /// <param name="container">The molecule to search</param>
        /// <param name="searchDepth">The maximum path length desired</param>
        /// <returns>Path strings</returns>
        protected int[] FindPathes(IAtomContainer container, int searchDepth)
        {
            var allPaths = new List<string>();
            var cache = new Dictionary<IAtom, IDictionary<IAtom, IBond>>();

            foreach (var startAtom in container.Atoms)
            {
                var p = PathTools.GetPathsOfLengthUpto(container, startAtom, searchDepth);
                foreach (var path in p)
                {
                    StringBuilder sb = new StringBuilder();
                    IAtom x = path[0];

                    // TODO if we ever get more than 255 elements, this will
                    // fail maybe we should use 0 for pseudo atoms and
                    // malformed symbols?
                    if (x is IPseudoAtom)
                        sb.Append('0');
                    else
                    {
                        int atnum = PeriodicTable.GetAtomicNumber(x.Symbol);
                        if (atnum > 0)
                            sb.Append((char)atnum);
                        else
                            sb.Append('0');
                    }

                    for (int i = 1; i < path.Count; i++)
                    {
                        IAtom[] y = { path[i] };
                        IDictionary<IAtom, IBond> m;
                        if (!cache.TryGetValue(x, out m))
                        {
                            m = null;
                        }
                        IBond bond = null;
                        if (m != null)
                        {
                            if (!m.TryGetValue(y[0], out bond))
                                bond = null;
                        }
                        IBond[] b = new IBond[] { bond };
                        if (b[0] == null)
                        {
                            b[0] = container.GetBond(x, y[0]);
                            cache[x] = new Dictionary<IAtom, IBond>() { { y[0], b[0] } };
                        }
                        sb.Append(GetBondSymbol(b[0]));
                        Strings.Append(sb, ConvertSymbol(y[0].Symbol));
                        x = y[0];
                    }

                    /// we store the lexicographically lower one of the
                    /// string and its reverse
                    string sb_string = sb.ToString();
                    char[] revForm_array = sb_string.ToCharArray();
                    Array.Reverse(revForm_array);
                    string rev_string = new string(revForm_array);
                    if (string.Compare(sb.ToString(), rev_string, StringComparison.Ordinal) <= 0)
                        allPaths.Add(sb_string);
                    else
                        allPaths.Add(rev_string);
                }
            }
            /// now lets clean stuff up
            ICollection<string> cleanPath = new HashSet<string>();
            foreach (var s in allPaths)
            {
                if (cleanPath.Contains(s.ToString())) continue;
                string s2 = new string(s.Reverse().ToArray());
                if (cleanPath.Contains(s2)) continue;
                cleanPath.Add(s2);
            }

            /// convert paths to hashes
            int[] hashes = new int[cleanPath.Count];
            {
                int i = 0;
                foreach (var s in cleanPath)
                    hashes[i++] = Strings.GetJavaHashCode(s);
            }

            return hashes;
        }

        /// <summary>
        /// Maps two character element symbols unto unique single character equivalents.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private string ConvertSymbol(string symbol)
        {
            if (symbol == null)
                return null;
            string returnSymbol;
            if (!QUERY_REPLACE.TryGetValue(symbol, out returnSymbol))
                return symbol;
            return returnSymbol;
        }

        /// <summary>
        /// Gets the bond Symbol attribute of the Fingerprinter class.
        /// </summary>
        /// <returns>The bondSymbol value</returns>
        protected string GetBondSymbol(IBond bond)
        {
            string bondSymbol = "";
            if (bond.Order == BondOrder.Single)
            {
                if (IsSP2Bond(bond))
                {
                    bondSymbol = ":";
                }
                else
                {
                    bondSymbol = "-";
                }
            }
            else if (bond.Order == BondOrder.Double)
            {
                if (IsSP2Bond(bond))
                {
                    bondSymbol = ":";
                }
                else
                {
                    bondSymbol = "=";
                }
            }
            else if (bond.Order == BondOrder.Triple)
            {
                bondSymbol = "#";
            }
            else if (bond.Order == BondOrder.Quadruple)
            {
                bondSymbol = "*";
            }
            return bondSymbol;
        }

        /// <summary>
        /// Returns true if the bond binds two atoms, and both atoms are SP2.
        /// </summary>
        private bool IsSP2Bond(IBond bond)
        {
            if (bond.Atoms.Count == 2 && bond.Atoms[0].Hybridization == Hybridization.SP2
                    && bond.Atoms[1].Hybridization == Hybridization.SP2) return true;
            return false;
        }

        public int SearchDepth => searchDepth;

        public int Count => size;

        /// <inheritdoc/>
        public IDictionary<string, int> GetRawFingerprint(IAtomContainer container)
        {
            throw new NotSupportedException();
        }

        public ICountFingerprint GetCountFingerprint(IAtomContainer container)
        {
            throw new NotSupportedException();
        }
    }
}
