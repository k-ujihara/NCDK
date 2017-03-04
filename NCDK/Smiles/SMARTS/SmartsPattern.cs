/*
 * Copyright (c) 2014 European Bioinformatics Institute (EMBL-EBI)
 *                    John May <jwmay@users.sf.net>
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
using NCDK.Aromaticities;
using NCDK.Graphs;
using NCDK.Isomorphisms;
using NCDK.Isomorphisms.Matchers.SMARTS;
using NCDK.Smiles.SMARTS.Parser;
using System;
using System.Diagnostics;
using System.IO;

namespace NCDK.Smiles.SMARTS
{
    /**
     * A <see cref="Pattern"/> for matching a single SMARTS query against multiple target
     * compounds. The class should <b>not</b> be used for matching many queries
     * against a single target as in substructure keyed fingerprints. The {@link
     * SMARTSQueryTool} is currently a better option as less target initialistion is
     * performed.
     *
     * Simple usage:
     *
     * <blockquote><code>
     * Pattern ptrn = SmartsPattern.Create("O[C@?H](C)CC");
     *
     * foreach (var ac in acs) {
     *   if (ptrn.Matches(ac)) {
     *       // 'ac' contains the pattern
     *   }
     * }
     * </code></blockquote>
     *
     * Obtaining a <see cref="Mappings"/> instance and determine the number of unique
     * matches.
     *
     * <blockquote><code>
     * Pattern ptrn = SmartsPattern.Create("O[C@?H](C)CC");
     *
     * foreach (var ac in acs) {
     *   nUniqueHits += ptrn.MatchAll(ac)
     *                      .CountUnique();
     * }
     * </code></blockquote>
     *
     * @author John May
     */
    public sealed class SmartsPattern : Pattern
    {

        /// <summary>Parsed query.</summary>
        private readonly IAtomContainer query;

        /// <summary>Subgraph mapping.</summary>
        private readonly Pattern pattern;

        /// <summary>Include invariants about ring size / number.</summary>
        private readonly bool ringInfo;

        /// <summary>Aromaticity model.</summary>
        private readonly Aromaticity arom = new Aromaticity(ElectronDonation.Daylight(), Cycles.Or(Cycles.All(),
                                                  Cycles.Relevant));

        /**
         * Internal constructor.
         *
         * @param smarts  pattern
         * @param builder the builder
         * @throws IOException the pattern could not be parsed
         */
        private SmartsPattern(string smarts, IChemObjectBuilder builder)
        {
            try
            {
                this.query = SMARTSParser.Parse(smarts, builder);
            }
            catch (Exception e)
            {
                throw new IOException(e.Message);
            }
            this.pattern = Pattern.FindSubstructure(query);

            // X<num>, R and @ are cheap and done always but R<num>, r<num> are not
            // we inspect the SMARTS pattern string to determine if ring
            // size or number queries are needed
            this.ringInfo = RingSizeOrNumber(smarts);
        }

        /**
         * @inheritDoc
         */
        public override int[] Match(IAtomContainer container)
        {
            return MatchAll(container).First();
        }

        /**
         * Obtain the mappings of the query pattern against the target compound. Any
         * initialisations required for the SMARTS match are automatically
         * performed. The Daylight aromaticity model is applied clearing existing
         * aromaticity. <b>Do not use this for matching multiple SMARTS againsts the
         * same container</b>.
         *
         * <blockquote><code>
         * Pattern ptrn = SmartsPattern.Create("O[C@?H](C)CC");
         * int nUniqueHits = 0;
         *
         * foreach (var ac in acs) {
         *   nUniqueHits += ptrn.MatchAll(ac)
         *                      .CountUnique();
         * }
         * </code></blockquote>
         *
         * See <see cref="Mappings"/> for available methods.
         *
         * @param target the target compound in which we want to match the pattern
         * @return mappings of the query to the target compound
         */
        public override Mappings MatchAll(IAtomContainer target)
        {

            // TODO: prescreen target for element frequency before intialising
            // invariants and applying aromaticity, requires pattern enumeration -
            // see http://www.daylight.com/meetings/emug00/Sayle/substruct.html.

            // assign additional atom invariants for SMARTS queries, a CDK quirk
            // as each atom knows not which molecule from wence it came
            SmartsMatchers.Prepare(target, ringInfo);

            // apply the daylight aromaticity model
            try
            {
                arom.Apply(target);
            }
            catch (CDKException e)
            {
                Trace.TraceError(e.Message);
            }

            Mappings mappings = pattern.MatchAll(target);

            // stereochemistry and component grouping filters are skipped if the
            // query does not contain them
            foreach (var stereoElement in query.StereoElements)
                mappings = mappings.Filter(new SmartsStereoMatch(query, target));
            if (query.GetProperty<object>(ComponentGrouping.Key) != null)
                mappings = mappings.Filter(new ComponentGrouping(query, target));

            // Note: Mappings is lazy, we can't reset aromaticity etc as the
            // substructure match may not have finished

            return mappings;
        }

        /**
         * Create a <see cref="Pattern"/> that will match the given {@code smarts} query.
         *
         * @param smarts  SMARTS pattern string
         * @param builder chem object builder used to create objects
         * @return a new pattern
         * @throws java.IOException the smarts could not be parsed
         */
        public static SmartsPattern Create(string smarts, IChemObjectBuilder builder)
        {
            return new SmartsPattern(smarts, builder);
        }

        /**
         * Checks a smarts string for !R, R<num> or r<num>. If found then the more
         * expensive ring info needs to be initlised before querying.
         *
         * @param smarts pattern string
         * @return the pattern has a ring size or number query
         */
        internal static bool RingSizeOrNumber(string smarts)
        {
            for (int i = 0, end = smarts.Length - 1; i <= end; i++)
            {
                char c = smarts[i];
                if ((c == 'r' || c == 'R') && i < end && char.IsDigit(smarts[i + 1])) return true;
                // !R = R0
                if (c == '!' && i < end && smarts[i + 1] == 'R') return true;
            }
            return false;
        }
    }
}
