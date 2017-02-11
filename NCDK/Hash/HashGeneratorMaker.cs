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
using System.Collections;
using System.Collections.Generic;

namespace NCDK.Hash
{
    /**
     * Fluent API for creating hash generators. The maker is first configured with
     * one or more attributes. Once fully configured the generator is made by
     * invoking {@link #Atomic()}, {@link #Molecular()} or {@link #Ensemble()}. The
     * order of the built-in configuration methods does not matter however when
     * specifying custom encoders with {@link #Encode(AtomEncoder)} the order they
     * are added is the order they will be used. Therefore one can expect different
     * hash codes if there is a change in the order they are specified.
     *
     * <h4>Examples</h4>
     * <blockquote><pre>
     * // simple
     * MoleculeHashGenerator generator = new HashGeneratorMaker().Depth(16)
     *                                                           .Elemental()
     *                                                           .Molecular();
     *
     * // fast
     * MoleculeHashGenerator generator = new HashGeneratorMaker().Depth(8)
     *                                                           .Elemental()
     *                                                           .Isotopic()
     *                                                           .Charged()
     *                                                           .orbital()
     *                                                           .Molecular();
     * // comprehensive
     * MoleculeHashGenerator generator = new HashGeneratorMaker().Depth(32)
     *                                                           .Elemental()
     *                                                           .Isotopic()
     *                                                           .Charged()
     *                                                           .Chiral()
     *                                                           .Perturbed()
     *                                                           .Molecular();
     * </pre></blockquote>
     *
     * @author John May
     * @cdk.module hash
     * @cdk.githash
     */
    public sealed class HashGeneratorMaker
    {
        /* no default depth */
        private int depth = -1;

        /* ordered list of custom encoders */
        private List<AtomEncoder> customEncoders = new List<AtomEncoder>();

        /* ordered set of basic encoders */
        private SortedSet<BasicAtomEncoder> encoderSet = new SortedSet<BasicAtomEncoder>();

        /* list of stereo encoders */
        private List<IStereoEncoderFactory> stereoEncoders = new List<IStereoEncoderFactory>();

        /* whether we want to use perturbed hash generators */
        private EquivalentSetFinder equivSetFinder = null;

        /* function determines whether any atoms are suppressed */
        private AtomSuppression suppression = AtomSuppression.Unsuppressed;

        /**
         * Specify the depth of the hash generator. Larger values discriminate more
         * molecules.
         *
         * @param depth how deep should the generator hash
         * @return reference for fluent API
         * @throws ArgumentException if the depth was less then zero
         */
        public HashGeneratorMaker Depth(int depth)
        {
            if (depth < 0) throw new ArgumentException("depth must not be less than 0");
            this.depth = depth;
            return this;
        }

        /**
         * Discriminate elements.
         *
         * @return fluent API reference (self)
         * @see BasicAtomEncoder#ATOMIC_NUMBER
         */
        public HashGeneratorMaker Elemental()
        {
            encoderSet.Add(BasicAtomEncoder.ATOMIC_NUMBER);
            return this;
        }

        /**
         * Discriminate isotopes.
         *
         * @return fluent API reference (self)
         * @see BasicAtomEncoder#MASS_NUMBER
         */
        public HashGeneratorMaker Isotopic()
        {
            encoderSet.Add(BasicAtomEncoder.MASS_NUMBER);
            return this;
        }

        /**
         * Discriminate protonation states.
         *
         * @return fluent API reference (self)
         * @see BasicAtomEncoder#FORMAL_CHARGE
         */
        public HashGeneratorMaker Charged()
        {
            encoderSet.Add(BasicAtomEncoder.FORMAL_CHARGE);
            return this;
        }

        /**
         * Discriminate atomic orbitals.
         *
         * @return fluent API reference (self)
         * @see BasicAtomEncoder#ORBITAL_HYBRIDIZATION
         */
        public HashGeneratorMaker orbital()
        {
            encoderSet.Add(BasicAtomEncoder.ORBITAL_HYBRIDIZATION);
            return this;
        }

        /**
         * Discriminate free radicals.
         *
         * @return fluent API reference (self)
         * @see BasicAtomEncoder#FREE_RADICALS
         */
        public HashGeneratorMaker radical()
        {
            encoderSet.Add(BasicAtomEncoder.FREE_RADICALS);
            return this;
        }

        /**
         * Generate different hash codes for stereoisomers. The currently supported
         * geometries are:
         *
         * <ul>
         *     <li>Tetrahedral</li>
         *     <li>Double Bond</li>
         *     <li>Cumulative Double Bonds</li>
         * </ul>
         *
         * @return fluent API reference (self)
         */
        public HashGeneratorMaker Chiral()
        {
            this.stereoEncoders.Add(new GeometricTetrahedralEncoderFactory());
            this.stereoEncoders.Add(new GeometricDoubleBondEncoderFactory());
            this.stereoEncoders.Add(new GeometricCumulativeDoubleBondFactory());
            this.stereoEncoders.Add(new TetrahedralElementEncoderFactory());
            this.stereoEncoders.Add(new DoubleBondElementEncoderFactory());
            return this;
        }

        /**
         * Suppress any explicit hydrogens in the encoding of hash values. The
         * generation of hashes acts as though the hydrogens are not present and as
         * such preserves stereo-encoding.
         *
         * @return fluent API reference (self)
         */
        public HashGeneratorMaker SuppressHydrogens()
        {
            this.suppression = AtomSuppression.AnyHydrogens;
            return this;
        }

        /**
         * Discriminate atoms experiencing uniform environments. This method uses
         * {@link MinimumEquivalentCyclicSet}  to break symmetry but depending on
         * application one may need a more comprehensive method. Please refer to
         * {@link #PerturbWith(EquivalentSetFinder)} for further configuration
         * details.
         *
         * @return fluent API reference (self)
         * @see MinimumEquivalentCyclicSet
         * @see #PerturbWith(EquivalentSetFinder)
         */
        public HashGeneratorMaker Perturbed()
        {
            return PerturbWith(new MinimumEquivalentCyclicSet());
        }

        /**
         * Discriminate atoms experiencing uniform environments using the provided
         * method. Depending on the level of identity required one can choose how
         * the atoms a perturbed in an attempt to break symmetry.  As with all
         * hashing there is always a probability of collision but some of these
         * collisions may be due to an insufficiency in the algorithm opposed to a
         * random chance of collision. Currently there are three strategies but one
         * should choose either to use the fast, but good, heuristic {@link
         * MinimumEquivalentCyclicSet} or the exact {@link AllEquivalentCyclicSet}.
         * In practice {@link MinimumEquivalentCyclicSet} is good enough for most
         * applications but it is important to understand the potential trade off.
         * The {@link MinimumEquivalentCyclicSetUnion} is provided for demonstration
         * only, and as such, is deprecated.
         *
         * <ul> <li>MinimumEquivalentCyclicSet - fastest, attempt to break symmetry
         * by changing a single smallest set of the equivalent atoms which occur in
         * a ring</li> <li><strike>MinimumEquivalentCyclicSetUnion</strike>
         * (deprecated) - distinguishes more molecules by changing all smallest sets
         * of the equivalent atoms which occur in a ring. This method is provided
         * from example only</li> <li>AllEquivalentCyclicSet - slowest,
         * systematically perturb all equivalent atoms that occur in a ring</li>
         * </ul>
         *
         * At the time of writing (Feb, 2013) the number of known false possibles
         * found in PubChem-Compound (aprx. 46,000,000 structures) are as follows:
         *
         * <ul> <li>MinimumEquivalentCyclicSet - 128 molecules, 64 false positives
         * (128/2)</li> <li>MinimumEquivalentCyclicSetUnion - 8 molecules, 4 false
         * positives (8/2)</li> <li>AllEquivalentCyclicSet - 0 molecules</li> </ul>
         *
         * @param equivSetFinder equivalent set finder, used to determine which
         *                       atoms will be perturbed to try and break symmetry.
         * @return fluent API reference (self)
         * @see AllEquivalentCyclicSet
         * @see MinimumEquivalentCyclicSet
         * @see MinimumEquivalentCyclicSetUnion
         */
#if TEST
        public
#endif
        HashGeneratorMaker PerturbWith(EquivalentSetFinder equivSetFinder)
        {
            this.equivSetFinder = equivSetFinder;
            return this;
        }

        /**
         * Add a custom encoder to the hash generator which will be built. Although
         * not enforced, the encoder should be stateless and should not modify any
         * passed inputs.
         *
         * @param encoder an atom encoder
         * @return fluent API reference (self)
         * @throws NullPointerException no encoder provided
         */
        public HashGeneratorMaker Encode(AtomEncoder encoder)
        {
            if (encoder == null) throw new ArgumentNullException("no encoder provided");
            customEncoders.Add(encoder);
            return this;
        }

        /**
         * Combines the separate stereo encoder factories into a single factory.
         *
         * @return a single stereo encoder factory
         */
        private IStereoEncoderFactory MakeStereoEncoderFactory()
        {
            if (stereoEncoders.Count == 0)
            {
                return StereoEncoderFactory.EMPTY;
            }
            else if (stereoEncoders.Count == 1)
            {
                return stereoEncoders[0];
            }
            else
            {
                var factory = new ConjugatedEncoderFactory(stereoEncoders[0], stereoEncoders[1]);
                for (int i = 2; i < stereoEncoders.Count; i++)
                {
                    factory = new ConjugatedEncoderFactory(factory, stereoEncoders[i]);
                }
                return factory;
            }
        }

        /**
         * Given the current configuration create an {@link EnsembleHashGenerator}.
         *
         * @return instance of the generator
         * @throws ArgumentException no depth or encoders were configured
         */
        public EnsembleHashGenerator Ensemble()
        {
            throw new NotSupportedException("not yet supported");
        }

        /**
         * Given the current configuration create an {@link MoleculeHashGenerator}.
         *
         * @return instance of the generator
         * @throws ArgumentException no depth or encoders were configured
         */
        public MoleculeHashGenerator Molecular()
        {
            return new BasicMoleculeHashGenerator(Atomic());
        }

        /**
         * Given the current configuration create an {@link AtomHashGenerator}.
         *
         * @return instance of the generator
         * @throws ArgumentException no depth or encoders were configured
         */
        public AtomHashGenerator Atomic()
        {
            if (depth < 0) throw new ArgumentException("no depth specified, use .Depth(int)");

            List<AtomEncoder> encoders = new List<AtomEncoder>();

            // set is ordered
            encoders.AddRange(encoderSet);
            encoders.AddRange(this.customEncoders);

            // check if suppression of atoms is wanted - if not use a default value
            // we also use the 'Basic' generator (see below)
            bool suppress = suppression != AtomSuppression.Unsuppressed;

            AtomEncoder encoder = new ConjugatedAtomEncoder(encoders);
            SeedGenerator seeds = new SeedGenerator(encoder, suppression);

            AbstractAtomHashGenerator simple = suppress ? (AbstractAtomHashGenerator)new SuppressedAtomHashGenerator(seeds, new Xorshift(),
                    MakeStereoEncoderFactory(), suppression, depth) : (AbstractAtomHashGenerator)new BasicAtomHashGenerator(seeds, new Xorshift(),
                    MakeStereoEncoderFactory(), depth);

            // if there is a finder for checking equivalent vertices then the user
            // wants to 'perturb' the hashed
            if (equivSetFinder != null)
            {
                return new PerturbedAtomHashGenerator(seeds, simple, new Xorshift(), MakeStereoEncoderFactory(),
                        equivSetFinder, suppression);
            }
            else
            {
                // no equivalence set finder - just use the simple hash
                return simple;
            }
        }

        /**
         * Help class to combined two stereo encoder factories
         */
        private sealed class ConjugatedEncoderFactory : IStereoEncoderFactory
        {

            private readonly IStereoEncoderFactory left, right;

            /**
             * Create a new conjugated encoder factory from the left and right
             * factories.
             *
             * @param left  encoder factory
             * @param right encoder factory
             */
            public ConjugatedEncoderFactory(IStereoEncoderFactory left, IStereoEncoderFactory right)
            {
                this.left = left;
                this.right = right;
            }

            public IStereoEncoder Create(IAtomContainer container, int[][] graph)
            {
                return new ConjugatedEncoder(left.Create(container, graph), right.Create(container, graph));
            }
        }

        /**
         * Help class to combined two stereo encoders
         */
        private sealed class ConjugatedEncoder : IStereoEncoder
        {

            private readonly IStereoEncoder left, right;

            /**
             * Create a new conjugated encoder from a left and right encoder.
             *
             * @param left  encoder
             * @param right encoder
             */
            public ConjugatedEncoder(IStereoEncoder left, IStereoEncoder right)
            {
                this.left = left;
                this.right = right;
            }

            /**
             * Encodes using the left and then the right encoder.
             *
             * @param current current invariants
             * @param next    next invariants
             * @return whether either encoder modified any values
             */
            public bool Encode(long[] current, long[] next)
            {
                bool modified = left.Encode(current, next);
                return right.Encode(current, next) || modified;
            }

            /**
             * reset the left and right encoders
             */
            public void Reset()
            {
                left.Reset();
                right.Reset();
            }
        }

    }
}
