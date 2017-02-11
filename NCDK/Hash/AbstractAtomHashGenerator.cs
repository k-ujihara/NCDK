/*
 * Copyright (c) 2013 European Bioinformatics Institute (EMBL-EBI)
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

using NCDK.Hash.Stereo;
using System.Collections;

namespace NCDK.Hash
{
    /**
     * Defines an internal super-class for AtomHashGenerators. The single required
     * method allows atom hash generators to either ignore 'suppressed' atoms or use
     * the information.
     *
     * @author John May
     * @cdk.module hash
     */
#if TEST
        public
#endif
        abstract class AbstractAtomHashGenerator : AbstractHashGenerator, AtomHashGenerator
    {
        /**
         * Empty BitArray for use when the 'suppressed' atoms are ignored.
         */
        readonly BitArray EMPTY_BITSET = new BitArray(0);

        public AbstractAtomHashGenerator(Pseudorandom pseudorandom)
                : base(pseudorandom)
        { }

        public abstract long[] Generate(IAtomContainer container);

        /**
         * Internal method invoked by 'molecule' hash generators.
         *
         * @param current    the current invariants
         * @param encoder    encoder used for encoding stereo-chemistry
         * @param graph      adjacency list representation of the molecule
         * @param suppressed bit set marks vertices which are 'suppressed' (may be
         *                   ignored)
         * @return the atom hash values
         */
        public abstract long[] Generate(long[] current, IStereoEncoder encoder, int[][] graph, Suppressed suppressed);
    }
}
