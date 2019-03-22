/* Copyright (C) 2007  Rajarshi Guha <rajarshi@users.sourceforge.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using NCDK.Aromaticities;
using NCDK.Common.Collections;
using NCDK.Graphs;
using NCDK.Isomorphisms;
using NCDK.Isomorphisms.Matchers;
using NCDK.Isomorphisms.Matchers.SMARTS;
using NCDK.Smiles.SMARTS.Parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static NCDK.Common.Base.Preconditions;

namespace NCDK.Smiles.SMARTS
{
    /// <summary>
    /// This class provides a easy to use wrapper around SMARTS matching functionality. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// User code that wants to do
    /// SMARTS matching should use this rather than using SMARTSParser (and <see cref="UniversalIsomorphismTester"/>) directly.
    /// </para>
    /// <list type="bullet">
    /// <listheader><description>Unsupported Features</description></listheader>
    /// <item><description>Component level grouping</description></item>
    /// <item><description>Stereochemistry</description></item>
    /// <item><description>Reaction support</description></item>
    /// </list>
    /// <h3>SMARTS Extensions</h3>
    /// <para>
    /// Currently the CDK supports the following SMARTS symbols, that are not described in the Daylight specification.
    /// However they are supported by other packages and are noted as such.
    /// </para>
    /// <list type="table">
    /// <listheader>
    /// <term>Symbol</term>
    /// <term>Meaning</term>
    /// <term>Default</term>
    /// <term>Notes</term>
    /// </listheader>
    /// <item>
    /// <term>Gx</term>
    /// <term>Periodic group number</term>
    /// <term>None</term>
    /// <term>x must be specified and must be a number between 1 and 18. This symbol is supported by the MOE SMARTS implementation</term>
    /// </item>
    /// <item>
    /// <term>#X</term>
    /// <term>Any non-carbon heavy element</term>
    /// <term>None</term>
    /// <term>This symbol is supported by the MOE SMARTS implementation</term>
    /// </item>
    /// <item>
    /// <term>^x</term>
    /// <term>Any atom with the a specified hybridization state</term>
    /// <term>None</term>
    /// <term>x must be specified and should be between 1 and 8 (inclusive), corresponding to SP1, SP2, SP3, SP3D1, SP3D2 SP3D3, SP3D4 and SP3D5. Supported by the OpenEye SMARTS implementation</term>
    /// </item>
    /// </list>
    /// <note type="note">
    /// <list type="bullet">
    /// <item>
    /// <term>
    /// As <see href="http://sourceforge.net/mailarchive/message.php?msg_name=4964F605.1070502%40emolecules.com">described</see>
    /// by Craig James the "h&lt;n&gt;" SMARTS pattern should not be used. It was included in the Daylight spec
    /// for backwards compatibility. To match hydrogens, use the "H&lt;n&gt;" pattern.
    /// </term>
    /// <term>
    /// The wild card pattern ("*") will not match hydrogens (explicit or implicit) unless an isotope is specified. In other
    /// words, "*" gives two hits against "C[2H]" but 1 hit against "C[H]". This also means
    /// that it gives no hits against "[H][H]". This is contrary to what is shown by Daylights 
    /// <see href="http://www.daylight.com/daycgi_tutorials/depictmatch.cgi">depictmatch</see> service, but is based on this 
    /// <see href="https://sourceforge.net/mailarchive/message.php?msg_name=4964FF9D.3040004%40emolecules.com">discussion</see>. A
    /// work around to get "*" to match "[H][H]" is to write it in the form "[1H][1H]".
    /// <para>
    /// It's not entirely clear what the behavior of * should be with respect to hydrogens. it is possible that the code will
    /// be updated so that "*" will not match <i>any</i> hydrogen in the future.</para>
    /// </term>
    /// <term>
    /// The CDKHueckelAromaticityDetector only considers single rings and two fused non-spiro
    /// rings. As a result, it does not properly detect aromaticity in polycyclic systems such as
    /// "[O-]C(=O)c1ccccc1c2c3ccc([O-])cc3oc4cc(=O)ccc24". Thus SMARTS patterns that depend on proper aromaticity
    /// detection may not work correctly in such polycyclic systems
    /// </term>
    /// </item>
    /// </list>
    /// </note>
    /// </remarks>
    /// <example>
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Smiles.SMARTS.SMARTSQueryTool_Example.cs"]/*' />
    /// </example>
    // @author Rajarshi Guha
    // @cdk.created 2007-04-08
    // @cdk.module smarts
    // @cdk.keyword SMARTS
    // @cdk.keyword substructure search
    [Obsolete("Use " + nameof(SmartsPattern))]
    public class SMARTSQueryTool
    {
        private string smarts;
        private IAtomContainer atomContainer = null;
        private QueryAtomContainer query = null;
        private List<int[]> mappings;

        /// <summary>
        /// Defines which set of rings to define rings in the target.
        /// </summary>
        private abstract class RingSet
        {
            private RingSet()
            { }

            /// <summary>
            /// Smallest Set of Smallest Rings (or Minimum Cycle Basis - but not
            /// strictly the same). Defines what is typically thought of as a 'ring'
            /// however the non-uniqueness leads to ambiguous matching.
            /// </summary>
            public static readonly RingSet SmallestSetOfSmallestRings = new SmallestSetOfSmallestRings_Impl();
            private class SmallestSetOfSmallestRings_Impl
                : RingSet
            {
                public override IRingSet ComputeRingSet(IAtomContainer m)
                {
                    return Cycles.FindSSSR(m).ToRingSet();
                }
            }

            /// <summary>
            /// Intersect of all Minimum Cycle Bases (or SSSR) and thus is a subset.
            /// The set is unique but may excludes rings (e.g. from bridged systems).
            /// </summary>
            public static readonly RingSet EssentialRings = new EssentialRings_Impl();
            private class EssentialRings_Impl : RingSet
            {
                public override IRingSet ComputeRingSet(IAtomContainer m)
                {
                    return Cycles.FindEssential(m).ToRingSet();
                }
            }

            /// <summary>
            /// Union of all Minimum Cycle Bases (or SSSR) and thus is a superset.
            /// The set is unique but may include more rings then is necessary.
            /// </summary>
            public static readonly RingSet RelevantRings = new RelevantRings_Impl();
            private class RelevantRings_Impl : RingSet
            {
                public override IRingSet ComputeRingSet(IAtomContainer m)
                {
                    return Cycles.FindRelevant(m).ToRingSet();
                }
            }

            /// <summary>
            /// Compute a ring set for a molecule.
            /// </summary>
            /// <param name="m">molecule</param>
            /// <returns>the ring set for the molecule</returns>
            public abstract IRingSet ComputeRingSet(IAtomContainer m);

            public static readonly RingSet[] Values = new[] {
                SmallestSetOfSmallestRings,
                EssentialRings,
                RelevantRings, };
        }

        /// <summary>Which short cyclic set should be used.</summary>
        private RingSet ringSet = RingSet.EssentialRings;

        private readonly IChemObjectBuilder builder;

        /// <summary>
        /// Aromaticity perception - dealing with SMARTS we should use the Daylight
        /// model. This can be set to a different model using <see cref="SetAromaticity(Aromaticity)"/>.
        /// </summary>
        private Aromaticity aromaticity = new Aromaticity(ElectronDonation.DaylightModel, Cycles.AllOrVertexShortFinder);

        /// <summary>
        /// Logical flag indicates whether the aromaticity model should be skipped.
        /// Generally this should be left as false to ensure the structures being
        /// matched are all treated the same. The flag can however be turned off if
        /// the molecules being tests are known to all have the same aromaticity
        /// model.
        /// </summary>
        private readonly bool skipAromaticity = false;

        // a simplistic cache to store parsed SMARTS queries
        private int MAX_ENTRIES = 20;
        Dictionary<string, QueryAtomContainer> cache = new Dictionary<string, QueryAtomContainer>();
        // TODO: handle MAX_ENTRIES

        /// <summary>
        /// Create a new SMARTS query tool for the specified SMARTS string. Query
        /// objects will contain a reference to the specified <see cref="IChemObjectBuilder"/>.
        /// </summary>
        /// <param name="smarts">SMARTS query string</param>
        /// <exception cref="ArgumentException">if the SMARTS string can not be handled</exception>
        public SMARTSQueryTool(string smarts, IChemObjectBuilder builder)
        {
            this.builder = builder;
            this.smarts = smarts;
            try
            {
                InitializeQuery();
            }
            catch (TokenManagerException error)
            {
                throw new ArgumentException("Error parsing SMARTS", error);
            }
            catch (CDKException error)
            {
                throw new ArgumentException("Error parsing SMARTS", error);
            }
        }

        /// <summary>
        /// Set the maximum size of the query cache.
        /// </summary>
        /// <param name="maxEntries">The maximum number of entries</param>
        public void SetQueryCacheSize(int maxEntries)
        {
            MAX_ENTRIES = maxEntries;
        }

        /// <summary>
        /// Indicates that ring properties should use the Smallest Set of Smallest
        /// Rings. The set is not unique and may lead to ambiguous matches.
        /// </summary>
        /// <seealso cref="UseEssentialRings"/>
        /// <seealso cref="UseRelevantRings"/>
        public void UseSmallestSetOfSmallestRings()
        {
            this.ringSet = RingSet.SmallestSetOfSmallestRings;
        }

        /// <summary>
        /// Indicates that ring properties should use the Relevant Rings. The set is
        /// unique and includes all of the SSSR but may be exponential in size.
        /// </summary>
        /// <seealso cref="UseSmallestSetOfSmallestRings"/>
        /// <seealso cref="UseEssentialRings"/>
        public void UseRelevantRings()
        {
            this.ringSet = RingSet.RelevantRings;
        }

        /// <summary>
        /// Indicates that ring properties should use the Essential Rings (default).
        /// The set is unique but only includes a subset of the SSSR.
        /// </summary>
        /// <seealso cref="UseSmallestSetOfSmallestRings"/>
        /// <seealso cref="UseEssentialRings"/>
        public void UseEssentialRings()
        {
            this.ringSet = RingSet.EssentialRings;
        }

        /// <summary>
        /// Set the aromaticity perception to use. Different aromaticity models
        /// may required certain attributes to be set (e.g. atom typing). These
        /// will not be automatically configured and should be preset before matching.
        /// </summary>
        /// <example>
        /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Smiles.SMARTS.SMARTSQueryTool_Example.cs+SetAromaticity"]/*' />
        /// </example>
        /// <param name="aromaticity">the new aromaticity perception</param>
        /// <seealso cref="ElectronDonation"/>
        /// <seealso cref="Cycles"/>
        public void SetAromaticity(Aromaticity aromaticity)
        {
            this.aromaticity = CheckNotNull(aromaticity, "aromaticity was not provided");
        }

        /// <summary>
        /// The current SMARTS pattern being used.
        /// </summary>
        public string Smarts
        {
            get
            {
                return smarts;
            }

            set
            {
                this.smarts = value;
                InitializeQuery();
            }
        }

        /// <summary>
        /// Perform a SMARTS match and check whether the query is present in the target molecule. 
        /// <para>
        /// This function simply
        /// checks whether the query pattern matches the specified molecule. However the function will also, internally, save
        /// the mapping of query atoms to the target molecule</para>
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// This method performs a simple caching scheme, by comparing the current molecule to the previous
        /// molecule by reference. If you repeatedly match different SMARTS on the same molecule, this method will avoid
        /// initializing ( ring perception, aromaticity etc.) the molecule each time. If however, you modify the molecule
        /// between such multiple matchings you should use the other form of this method to force initialization.
        /// </note>
        /// </remarks>
        /// <param name="atomContainer">The target molecule</param>
        /// <returns>true if the pattern is found in the target molecule, false otherwise</returns>
        /// <exception cref="CDKException">if there is an error in ring, aromaticity or isomorphism perception</exception>
        /// <seealso cref="GetMatchingAtoms"/>
        /// <seealso cref="MatchesCount"/>
        /// <see cref="Matches(IAtomContainer, bool)"/>
        public bool Matches(IAtomContainer atomContainer)
        {
            return Matches(atomContainer, false);
        }

        /// <summary>
        /// Perform a SMARTS match and check whether the query is present in the target molecule. 
        /// <para>This function simply
        /// checks whether the query pattern matches the specified molecule. However the function will also, internally, save
        /// the mapping of query atoms to the target molecule</para>
        /// </summary>
        /// <param name="atomContainer">The target moleculoe</param>
        /// <param name="forceInitialization">If true, then the molecule is initialized (ring perception, aromaticity etc). If
        ///                            false, the molecule is only initialized if it is different (in terms of object
        ///                            reference) than one supplied in a previous call to this method.</param>
        /// <returns>true if the pattern is found in the target molecule, false otherwise</returns>
        /// <exception cref="CDKException">if there is an error in ring, aromaticity or isomorphism perception</exception>
        /// <seealso cref="GetMatchingAtoms"/>
        /// <seealso cref="MatchesCount"/>
        /// <seealso cref="Matches(IAtomContainer)"/>
        public bool Matches(IAtomContainer atomContainer, bool forceInitialization)
        {
            if (this.atomContainer == atomContainer)
            {
                if (forceInitialization)
                    InitializeMolecule();
            }
            else
            {
                this.atomContainer = atomContainer;
                InitializeMolecule();
            }

            // lets see if we have a single atom query
            if (query.Atoms.Count == 1)
            {
                // lets get the query atom
                var queryAtom = (IQueryAtom)query.Atoms[0];

                mappings = new List<int[]>();
                for (int i = 0; i < atomContainer.Atoms.Count; i++)
                {
                    if (queryAtom.Matches(atomContainer.Atoms[i]))
                    {
                        mappings.Add(new int[] { i });
                    }
                }
            }
            else
            {
                mappings = Ullmann.CreateSubstructureFinder(query).MatchAll(atomContainer)
                        .Where(n => new SmartsStereoMatch(query, atomContainer).Apply(n))
                        .ToList();
            }

            return mappings.Count != 0;
        }

        /// <summary>
        /// Returns the number of times the pattern was found in the target molecule. 
        /// <para>This function should be called
        /// after <see cref="Matches(IAtomContainer)"/>. If not, the results may be undefined.</para>
        /// </summary>
        /// <returns>The number of times the pattern was found in the target molecule</returns>
        public int MatchesCount => mappings.Count;

        /// <summary>
        /// Get the atoms in the target molecule that match the query pattern. 
        /// <para>Since there may be multiple matches, the
        /// return value is a List of List objects. Each List object contains the indices of the atoms in the target
        /// molecule, that match the query pattern</para>
        /// </summary>
        /// <returns>A List of List of atom indices in the target molecule</returns>
        public IEnumerable<IReadOnlyList<int>> GetMatchingAtoms()
        {
            foreach (var mapping in mappings)
                yield return mapping;
            yield break;
        }

        /// <summary>
        /// Get the atoms in the target molecule that match the query pattern. 
        /// <para>
        /// Since there may be multiple matches, the
        /// return value is a List of List objects. Each List object contains the unique set of indices of the atoms in the
        /// target molecule, that match the query pattern
        /// </para>
        /// </summary>
        /// <returns>A List of List of atom indices in the target molecule</returns>
        public IEnumerable<IReadOnlyList<int>> GetUniqueMatchingAtoms()
        {
            var atomSets = new HashSet<BitArray>(BitArrays.EqualityComparer);
            foreach (var mapping in mappings)
            {
                BitArray atomSet = new BitArray(0);
                foreach (var x in mapping)
                    BitArrays.SetValue(atomSet, x, true);
                if (atomSets.Add(atomSet))
                    yield return mapping;
            }
            yield break;
        }

        /// <summary>
        /// Prepare the target molecule for analysis. 
        /// <para>
        /// We perform ring perception and aromaticity detection and set up
        /// the appropriate properties. Right now, this function is called each time we need to do a query and this is
        /// inefficient.</para>
        /// </summary>
        /// <exception cref="CDKException">if there is a problem in ring perception or aromaticity detection, which is usually related to a timeout in the ring finding code.</exception>
        private void InitializeMolecule()
        {
            // initialise required invariants - the query has ISINRING set if
            // the query contains ring queries [R?] [r?] [x?] etc.
            SmartsMatchers.Prepare(atomContainer, true);

            // providing skip aromaticity has not be set apply the desired
            // aromaticity model
            try
            {
                if (!skipAromaticity)
                {
                    aromaticity.Apply(atomContainer);
                }
            }
            catch (CDKException e)
            {
                Debug.WriteLine(e.ToString());
                throw new CDKException(e.ToString(), e);
            }
        }

        private void InitializeQuery()
        {
            mappings = null;
            if (!cache.TryGetValue(smarts, out query))
            {
                query = SMARTSParser.Parse(smarts, builder);
                cache[smarts] = query;
            }
        }
    }
}
