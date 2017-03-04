/*
 * Copyright (c) 2013 John May <jwmay@users.sf.net>
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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using NCDK.Hash.Stereo;
using System;

namespace NCDK.Hash
{
    /// <summary>
    /// A generator for basic atom hash codes. This implementation is based on the
    /// description by {@cdk.cite Ihlenfeldt93}. The hash codes use an initial
    /// generator to seed the values of each atom. The initial values are then
    /// combined over a series of cycles up to a specified depth. At each cycle the
    /// hash values of adjacent invariants are incorporated.
    ///
    /// <h4>Which depth should I use?</h4> The <i>depth</i> determines the number of
    /// cycles and thus how <i>deep</i> the hashing is, larger values discriminate
    /// more molecules but can take longer to compute. The original publication
    /// recommends a depth of 32 however values as low as 6 can yield good results.
    /// The actual depth required is related to the <i>diameter</i> of the chemical
    /// graph. The <i>diameter</i> is the longest shortest path, that is, the
    /// furthest distance one must travel between any two vertex. Unfortunately the
    /// time complexity of finding the longest shortest path in an undirected graph
    /// is O(n<sup>2</sup>) which is larger then the time required for this hash
    /// function. Depending on the types of molecules in your data set the depth
    /// should be adjusted accordingly. For example, a library of large-lipids would
    /// require deeper hashing to discriminate differences in chain length.
    ///
    /// <h4>Usage</h4>
    /// <example><code>
    /// SeedGenerator     seeding   = ...
    /// AtomHashGenerator generator = new BasicAtomHashGenerator(seeding,
    ///                                                          new Xorshift(),
    ///                                                          32);
    ///
    /// IAtomContainer benzene = MoleculeFactory.Benzene();
    /// long[]         hashes  = generator.Generate(benzene);
    /// </code></example>
    ///
    // @author John May
    // @cdk.module hash
    /// <seealso cref="SeedGenerator"/>
    // @see <a href="http://mathworld.wolfram.com/GraphDiameter.html">Graph
    ///      Diameter</a>
    // @see <a href="http://onlinelibrary.wiley.com/doi/10.1002/jcc.540150802/abstract">Original
    ///      Publication</a>
    // @cdk.githash
    /// </summary>
    internal sealed class BasicAtomHashGenerator : AbstractAtomHashGenerator, AtomHashGenerator
    {
        /* a generator for the initial atom seeds */
        private readonly AtomHashGenerator seedGenerator;

        /* creates stereo encoders for IAtomContainers */
        private readonly IStereoEncoderFactory factory;

        /* number of cycles to include adjacent invariants */
        private readonly int depth;

        /// <summary>
        /// Create a basic hash generator using the provided seed generator to
        /// initialise atom invariants and using the provided stereo factory.
        ///
        /// <param name="seedGenerator">generator to seed the initial values of atoms</param>
        /// <param name="pseudorandom">pseudorandom number generator used to randomise hash</param>
        ///                      distribution
        /// <param name="factory">a stereo encoder factory</param>
        /// <param name="depth">depth of the hashing function, larger values take</param>
        ///                      longer
        /// <exception cref="ArgumentException">depth was less then 0</exception>
        /// <exception cref="NullPointerException">    seed generator or pseudo random was</exception>
        ///                                  null
        /// <seealso cref="SeedGenerator"/>
        /// </summary>
        public BasicAtomHashGenerator(AtomHashGenerator seedGenerator, Pseudorandom pseudorandom,
                IStereoEncoderFactory factory, int depth)
                : base(pseudorandom)
        {
            if (seedGenerator == null) throw new ArgumentNullException("seed generator cannot be null");
            if (depth < 0) throw new ArgumentOutOfRangeException("depth cannot be less then 0");
            this.seedGenerator = seedGenerator;
            this.factory = factory;
            this.depth = depth;
        }

        /// <summary>
        /// Create a basic hash generator using the provided seed generator to
        /// initialise atom invariants and no stereo configuration.
        ///
        /// <param name="seedGenerator">generator to seed the initial values of atoms</param>
        /// <param name="pseudorandom">pseudorandom number generator used to randomise hash</param>
        ///                      distribution
        /// <param name="depth">depth of the hashing function, larger values take</param>
        ///                      longer
        /// <exception cref="ArgumentException">depth was less then 0</exception>
        /// <exception cref="NullPointerException">    seed generator or pseudo random was</exception>
        ///                                  null
        /// <seealso cref="SeedGenerator"/>
        /// </summary>
        public BasicAtomHashGenerator(AtomHashGenerator seedGenerator, Pseudorandom pseudorandom, int depth)
                : this(seedGenerator, pseudorandom, StereoEncoderFactory.EMPTY, depth)
        { }

        public override long[] Generate(IAtomContainer container)
        {
            int[][] graph = ToAdjList(container);
            return Generate(seedGenerator.Generate(container), factory.Create(container, graph), graph, Suppressed.None);
        }

        /// <summary>
        /// Package-private method for generating the hash for the given molecule.
        /// The initial invariants are passed as to the method along with an
        /// adjacency list representation of the graph.
        ///
        /// <param name="current">initial invariants</param>
        /// <param name="graph">adjacency list representation</param>
        /// <returns>hash codes for atoms</returns>
        /// </summary>
        public override long[] Generate(long[] current, IStereoEncoder encoder, int[][] graph, Suppressed suppressed)
        {

            int n = graph.Length;
            long[] next = Copy(current);

            // buffers for including adjacent invariants
            long[] unique = new long[n];
            long[] included = new long[n];

            while (encoder.Encode(current, next))
            {
                Copy(next, current);
            }

            for (int d = 0; d < depth; d++)
            {

                for (int v = 0; v < n; v++)
                {
                    next[v] = Next(graph, v, current, unique, included);
                }

                Copy(next, current);

                while (encoder.Encode(current, next))
                {
                    Copy(next, current);
                }

            }

            return current;
        }

        /// <summary>
        /// Determine the next value of the atom at index <i>v</i>. The value is
        /// calculated by combining the current values of adjacent atoms. When a
        /// duplicate value is found it can not be directly included and is
        /// <i>rotated</i> the number of times it has previously been seen.
        ///
        /// <param name="graph">adjacency list representation of connected atoms</param>
        /// <param name="v">the atom to calculate the next value for</param>
        /// <param name="current">the current values</param>
        /// <param name="unique">buffer for working out which adjacent values are unique</param>
        /// <param name="included">buffer for storing the rotated <i>unique</i> value, this</param>
        ///                 value is <i>rotated</i> each time the same value is
        ///                 found.
        /// <returns>the next value for <i>v</i></returns>
        /// </summary>
        internal long Next(int[][] graph, int v, long[] current, long[] unique, long[] included)
        {

            long invariant = Distribute(current[v]);
            int nUnique = 0;

            foreach (var w in graph[v])
            {

                long adjInv = current[w];

                // find index of already included neighbor
                int i = 0;
                while (i < nUnique && unique[i] != adjInv)
                {
                    ++i;
                }

                // no match, then the value is unique, use adjInv
                // match, then rotate the previously included value
                included[i] = (i == nUnique) ? unique[nUnique++] = adjInv : Rotate(included[i]);

                invariant ^= included[i];
            }

            return invariant;
        }
    }
}
