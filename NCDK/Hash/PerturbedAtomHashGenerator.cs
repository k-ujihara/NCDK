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

using NCDK.Common.Collections;
using NCDK.Hash.Stereo;
using System;
using System.Linq;

namespace NCDK.Hash
{
    /**
     * A perturbed hash generator {@cdk.cite Ihlenfeldt93} which differentiates
     * molecules with uniform atom environments and symmetry. The generator first
     * calculates the basic hash codes ({@link BasicAtomHashGenerator}) and then
     * checks for duplicate values (uniform environments). These duplicate values
     * are then filtered down ({@link EquivalentSetFinder}) to a set (<i>S</i>)
     * which can introduce systematic differences with. We then combine the
     * |<i>S</i>| different invariant values with the original value to produce a
     * unique value of each atom. There may still be duplicate values but providing
     * the depth is appropriate then the atoms are truly equivalent.
     * <p/><br/>
     * The class requires a lot of configuration however it can be easily built with
     * the {@link HashGeneratorMaker}.
     * <blockquote><pre>
     * MoleculeHashGenerator generator = new HashGeneratorMaker().Depth(8)
     *                                                           .Elemental()
     *                                                           .Perturbed()
     *                                                           .Molecular();
     * IAtomContainer molecule = ...;
     * long hash = generator.Generate(molecule);
     * </pre></blockquote>
     *
     * @author John May
     * @cdk.module hash
     * @see org.openscience.cdk.hash.SeedGenerator
     * @see <a href="http://onlinelibrary.wiley.com/doi/10.1002/jcc.540150802/abstract">Original
     *      Publication</a>
     * @cdk.githash
     * @see HashGeneratorMaker
     */
#if TEST
    public
#endif
    sealed class PerturbedAtomHashGenerator : AbstractHashGenerator, AtomHashGenerator
    {

        /* creates stereo encoders for IAtomContainers */
        private readonly IStereoEncoderFactory factory;

        /* simple hash generator */
        private readonly AbstractAtomHashGenerator simple;

        /* seed generator */
        private readonly AtomHashGenerator seeds;

        /* find the set of vertices in which we will add systematic differences */
        private readonly EquivalentSetFinder finder;

        /* suppression of atoms */
        private readonly AtomSuppression suppression;

        /**
         * Create a perturbed hash generator using the provided seed generator to
         * initialise atom invariants and using the provided stereo factory.
         *
         * @param simple        generator to encode the initial values of atoms
         * @param pseudorandom  pseudorandom number generator used to randomise hash
         *                      distribution
         * @param factory       a stereo encoder factory
         * @param finder        equivalent set finder for driving the systematic
         *                      perturbation
         * @param suppression   suppression of atoms (these atoms are 'ignored'
         *                      in the hash generation)
         * @throws ArgumentException depth was less then 0
         * @throws NullPointerException     seed generator or pseudo random was
         *                                  null
         * @see org.openscience.cdk.hash.SeedGenerator
         */
        public PerturbedAtomHashGenerator(SeedGenerator seeds, AbstractAtomHashGenerator simple, Pseudorandom pseudorandom,
               IStereoEncoderFactory factory, EquivalentSetFinder finder, AtomSuppression suppression)
                : base(pseudorandom)
        {
            if (simple == null) throw new ArgumentNullException("no simple generator provided");
            if (seeds == null) throw new ArgumentNullException("no seed generator provided");
            if (suppression == null) throw new ArgumentNullException("no suppression provided, use AtomSuppression.none()");
            this.finder = finder;
            this.factory = factory;
            this.simple = simple;
            this.seeds = seeds;
            this.suppression = suppression;
        }

        public long[] Generate(IAtomContainer container)
        {
            int[][] graph = ToAdjList(container);
            return Generate(container, seeds.Generate(container), factory.Create(container, graph), graph);
        }

        private long[] Generate(IAtomContainer container, long[] seeds, IStereoEncoder encoder, int[][] graph)
        {

            Suppressed suppressed = suppression.Suppress(container);

            // compute original values then find indices equivalent values
            long[] original = simple.Generate(seeds, encoder, graph, suppressed);
            var equivalentSet = finder.Find(original, container, graph);
            var equivalents = equivalentSet.ToArray();

            // size of the matrix we need to make
            int n = original.Length;
            int m = equivalents.Length;

            // skip when there are no equivalent atoms
            if (m < 2) return original;

            // matrix of perturbed values and identity values
            long[][] perturbed = Arrays.CreateJagged<long>(n, m + 1);

            // set the original values in the first column
            for (int i = 0; i < n; i++)
            {
                perturbed[i][0] = original[i];
            }

            // systematically perturb equivalent vertex
            for (int i = 0; i < m; i++)
            {

                int equivalentIndex = equivalents[i];

                // perturb the value and reset stereo configuration
                original[equivalentIndex] = Rotate(original[equivalentIndex]);
                encoder.Reset();

                // compute new hash codes and copy the values a column in the matrix
                long[] tmp = simple.Generate(Copy(original), encoder, graph, suppressed);
                for (int j = 0; j < n; j++)
                {
                    perturbed[j][i + 1] = tmp[j];
                }

                // reset value
                original[equivalentIndex] = perturbed[equivalentIndex][0];
            }

            return Combine(perturbed);
        }

        /**
         * Combines the values in an n x m matrix into a single array of size n.
         * This process scans the rows and xors all unique values in the row
         * together. If a duplicate value is found it is rotated using a
         * pseudorandom number generator.
         *
         * @param perturbed n x m, matrix
         * @return the combined values of each row
         */
#if TEST
        public
#endif
            long[] Combine(long[][] perturbed)
        {

            int n = perturbed.Length;
            int m = perturbed[0].Length;

            long[] combined = new long[n];
            long[] rotated = new long[m];

            for (int i = 0; i < n; i++)
            {

                Array.Sort(perturbed[i]);

                for (int j = 0; j < m; j++)
                {
                    // if non-unique, then get the next random number
                    if (j > 0 && perturbed[i][j] == perturbed[i][j - 1])
                    {
                        combined[i] ^= rotated[j] = Rotate(rotated[j - 1]);
                    }
                    else
                    {
                        combined[i] ^= rotated[j] = perturbed[i][j];
                    }
                }

            }

            return combined;
        }
    }
}
