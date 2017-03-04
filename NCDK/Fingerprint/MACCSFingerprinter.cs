/* Copyright (C) 2008 Rajarshi Guha <rajarshi@users.sourceforge.net>
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
using NCDK.Common.Primitives;
using NCDK.Graphs;
using NCDK.Isomorphisms;
using NCDK.Isomorphisms.Matchers.SMARTS;
using NCDK.RingSearches;
using NCDK.Smiles.SMARTS.Parser;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace NCDK.Fingerprint {
    /// <summary>
    /// This fingerprinter generates 166 bit MACCS keys.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The SMARTS patterns for each of the features was taken from
    /// <a href="http://www.rdkit.org"> RDKit</a>. However given that there is no
    /// official and explicit listing of the original key definitions, the results
    /// of this implementation may differ from others.
    /// </para>
    /// <para>
    /// This class assumes that aromaticity perception, atom typing and adding of
    /// implicit hydrogens have been performed prior to generating the fingerprint.
    /// </para>
    /// <para>
    /// <b>Note</b> Currently bits 1 and 44 are completely ignored since the RDKit
    /// defs do not provide a definition and I can't find an official description
    /// of them.
    /// </para>
    /// <para>
    /// <b>Warning - MACCS substructure keys cannot be used for substructure
    /// filtering. It is possible for some keys to match substructures and not match
    /// the superstructures. Some keys check for hydrogen counts which may not be
    /// preserved in a superstructure.</b>
    /// </para>
    /// </remarks>
    // @author Rajarshi Guha
    // @cdk.created 2008-07-23
    // @cdk.keyword fingerprint
    // @cdk.keyword similarity
    // @cdk.module  fingerprint
    // @cdk.githash
    internal class MACCSFingerprinter : IFingerprinter
    {
        private const string KEY_DEFINITIONS = "Data.maccs.txt";

        private volatile IList<MaccsKey> keys            = null;

        public MACCSFingerprinter() {}

        public MACCSFingerprinter(IChemObjectBuilder builder) {
            try {
                keys = ReadKeyDef(builder);
            } catch (IOException e) {
                Debug.WriteLine(e);
            } catch (CDKException e) {
                Debug.WriteLine(e);
            }
        }

        /// <inheritdoc/>
    
        public IBitFingerprint GetBitFingerprint(IAtomContainer container)  {
            IList<MaccsKey> keys = GetKeys(container.Builder);
            BitArray fp = new BitArray(keys.Count);

            // init SMARTS invariants (connectivity, degree, etc)
            SmartsMatchers.Prepare(container, false);

            for (int i = 0; i < keys.Count; i++) {
                Pattern pattern = keys[i].Pattern;
                if (pattern == null) continue;

                // check if there are at least 'count' unique hits, key.count = 0
                // means find at least one match hence we add 1 to out limit
                if (pattern.MatchAll(container).GetUniqueAtoms().AtLeast(keys[i].Count + 1)) fp.Set(i, true);
            }

            // at this point we have skipped the entries whose pattern is "?"
            // (bits 1,44,125,166) so let try and do those features by hand

            // bit 125 aromatic ring count > 1
            // bit 101 a ring with more than 8 members
            AllRingsFinder ringFinder = new AllRingsFinder();
            IRingSet rings = ringFinder.FindAllRings(container);
            int ringCount = 0;
            for (int i = 0; i < rings.Count; i++) {
                IAtomContainer ring = rings[i];
                bool allAromatic = true;
                if (ringCount < 2) { // already found enough aromatic rings
                    foreach (var bond in ring.Bonds) {
                        if (!bond.IsAromatic) {
                            allAromatic = false;
                            break;
                        }
                    }
                }
                if (allAromatic) ringCount++;
                if (ringCount > 1) fp.Set(124, true);
                if (ring.Atoms.Count >= 8) fp.Set(100, true);
            }

            // bit 166 (*).(*) we can match this in SMARTS but it's faster to just
            // count the number of component
            ConnectedComponents cc = new ConnectedComponents(GraphUtil.ToAdjList(container));
            if (cc.NComponents > 1) fp.Set(165, true);

            return new BitSetFingerprint(fp);
        }

        /// <inheritdoc/>
    
        public IDictionary<string, int> GetRawFingerprint(IAtomContainer iAtomContainer)  {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
    
        public int Count
        {
            get
            {
                if (keys != null)
                    return keys.Count;
                else
                    return 0; // throw exception when keys aren't loaded?
            }
        }

        private IList<MaccsKey> ReadKeyDef(IChemObjectBuilder builder)
        {
            List<MaccsKey> keys = new List<MaccsKey>(166);
            var reader = new StreamReader(ResourceLoader.GetAsStream(GetType(), KEY_DEFINITIONS));

            // now process the keys
            string line;
            while ((line = reader.ReadLine()) != null) {
                if (line[0] == '#') continue;
                string data = line.Substring(0, line.IndexOf('|')).Trim();
                var toks = Strings.Tokenize(data);
                
                keys.Add(new MaccsKey(toks[1], CreatePattern(toks[1], builder), int.Parse(toks[2])));
            }
            if (keys.Count != 166) throw new CDKException("Found " + keys.Count + " keys during setup. Should be 166");
            return keys;
        }

        private class MaccsKey {

            private string smarts;
            private int count;
            private Pattern pattern;

            public MaccsKey(string smarts, Pattern pattern, int count) {
                this.smarts = smarts;
                this.pattern = pattern;
                this.count = count;
            }

            public string Smarts => smarts;

            public int Count => count;

            public Pattern Pattern => pattern;
        }


        /// <inheritdoc/>
    
        public ICountFingerprint GetCountFingerprint(IAtomContainer container)  {
            throw new NotSupportedException();
        }

        private readonly object syncLock = new object();

        /// <summary>
        /// Access MACCS keys definitions.
        ///
        /// <returns>array of MACCS keys.</returns>
        // @ maccs keys could not be loaded
        /// </summary>
        private IList<MaccsKey> GetKeys(IChemObjectBuilder builder)  {
            var result = keys;
            if (result == null) {
                lock (syncLock) {
                    result = keys;
                    if (result == null) {
                        try {
                            keys = result = ReadKeyDef(builder);
                        } catch (IOException e) {
                            throw new CDKException("could not read MACCS definitions", e);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Create a pattern for the provided SMARTS - if the SMARTS is '?' a pattern
        /// is not created.
        /// </summary>
        /// <param name="smarts">a smarts pattern</param>
        /// <param name="builder">chem object builder</param>
        /// <returns>the pattern to match</returns>
        private Pattern CreatePattern(string smarts, IChemObjectBuilder builder)
        {
            if (smarts.Equals("?")) return null;
            return Ullmann.FindSubstructure(SMARTSParser.Parse(smarts, builder));
        }
    }
}

