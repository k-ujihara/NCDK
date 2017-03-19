/*
 * Copyright (c) 2013, European Bioinformatics Institute (EMBL-EBI)
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice, this
 *    list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * Any EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
 * Any DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON Any THEORY OF LIABILITY, WHETHER IN CONTRACT, Strict LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN Any WAY OUT OF THE USE OF THIS
 * SOFTWARE, Even IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 * The views and conclusions contained in the software and documentation are those
 * of the authors and should not be interpreted as representing official policies,
 * either expressed or implied, of the FreeBSD Project.
 */

using NCDK.Common.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using static NCDK.Beam.Configuration;
using static NCDK.Beam.Configuration.Types;

namespace NCDK.Beam
{
    /// <summary>
    /// Defines the relative topology around a vertex (atom).
    /// </summary>
    // @author John May
    internal abstract class Topology
    {
        /// <summary>
        /// The vertex/atom which this topology describes.
        /// </summary>
        /// <exception cref="ArgumentException">Unknown topology</exception>
        public abstract int Atom { get; }

        /// <summary>
        /// The configuration of the topology.
        /// </summary>
        public abstract Configuration Configuration { get; }

        /// <summary>
        /// The configuration of the topology when it's carriers have the specified
        /// ranks.
        /// </summary>
        /// <param name="rank"></param>
        /// <returns>configuration for this topology</returns>
        public virtual Configuration ConfigurationOf(int[] rank)
        {
            return OrderBy(rank).Configuration;
        }

        /// <summary>
        /// What type of configuration is defined by this topology (e.g. Tetrahedral,
        /// DoubleBond etc).
        /// </summary>
        /// <returns>the type of the configuration</returns>
        public virtual Configuration.Types Type => Configuration.Type;

        /// <summary>
        /// Arrange the topology relative to a given ranking of vertices.
        /// </summary>
        /// <param name="rank">Ordering of vertices</param>
        /// <returns>a new topology with the neighbors arranged by the given rank</returns>
        public abstract Topology OrderBy(int[] rank);

        /// <summary>
        /// Transform the topology to one with the given <paramref name="mapping"/>.
        /// </summary>
        /// <param name="mapping">the mapping used to transform the topology</param>
        /// <returns>a new topology with it's vertices mapped</returns>
        public abstract Topology Transform(int[] mapping);

        public abstract void Copy(int[] dest);

        /// <summary>
        /// Compute the permutation parity of the vertices {@literal vs} for the
        /// given {@literal rank}. The parity defines the oddness or evenness of a
        /// permutation and is the number of inversions (swaps) one would need to
        /// make to place the 'vs' in the Order specified by rank.
        /// </summary>
        /// <remarks>
        /// <a href="http://en.wikipedia.org/wiki/Parity_of_a_permutation">Parity of a Permutation</a>
        /// </remarks>
        /// <param name="vs">  array of vertices</param>
        /// <param name="rank">rank of vertices</param>, |R| = Max(vs) + 1
        /// <returns>sign of the permutation, -1=odd or 1=even</returns>
        public static int Parity(int[] vs, int[] rank)
        {
            // count elements which are out of Order and by how much
            int count = 0;
            for (int i = 0; i < vs.Length; i++)
            {
                for (int j = i + 1; j < vs.Length; j++)
                {
                    if (rank[vs[i]] > rank[vs[j]])
                        count++;
                }
            }
            // odd parity = -1, even parity = 1
            return (count & 0x1) == 1 ? -1 : 1;
        }

        // help the compiler, array is a fixed size!
        public static int Parity4(int[] vs, int[] rank)
        {
            // count elements which are out of order and by how much
            int count = 0;
            for (int i = 0; i < 4; i++)
            {
                 int prev = rank[vs[i]];
                for (int j = i + 1; j < 4; j++)
                {
                    if (prev > rank[vs[j]])
                        count++;
                }
            }

            // odd parity = -1, even parity = 1
            return (count & 0x1) == 1 ? -1 : 1;
        }

        /// <summary>
        /// Sorts the array {@literal vs} into the Order given by the {@literal
        /// rank}.
        /// </summary>
        /// <param name="vs">vertices to sort</param>
        /// <param name="rank">rank of vertices</param>
        /// <returns>sorted array (cpy of vs)</returns>
        public static int[] Sort(int[] vs, int[] rank)
        {
            int[] ws = (int[])vs.Clone();    // Arrays.CopyOf(vs, vs.Length);

            // insertion sort using rank for the Ordering
            for (int i = 0, j = i; i < vs.Length - 1; j = ++i)
            {
                int v = ws[i + 1];
                while (rank[v] < rank[ws[j]])
                {
                    ws[j + 1] = ws[j];
                    if (--j < 0)
                        break;
                }
                ws[j + 1] = v;
            }
            return ws;
        }

        /// <summary>
        /// Specify Unknown configuration on atom - there is no vertex data stored.
        /// </summary>
        /// <returns>Unknown topology</returns>
        public static Topology Unknown => unknown;

        private static readonly Topology unknown = new UnknownTopology();

        /// <summary>
        /// Define tetrahedral topology of the given configuration.
        /// </summary>
        /// <param name="u">central atom</param>
        /// <param name="vs">vertices surrounding u, the first is the vertex we are looking from</param>
        /// <param name="configuration">the tetrahedral configuration, @TH1, @TH2, @ or @@</param>
        /// <returns>topology instance for that configuration</returns>
        /// <seealso cref="Configuration"/>
        public static Topology CreateTetrahedral(int u, int[] vs, Configuration configuration)
        {
            if (configuration.Type != Types.Implicit
                    && configuration.Type != Types.Tetrahedral)
                throw new ArgumentException(configuration.Type
                    + "invalid tetrahedral configuration");

            int p = configuration.Shorthand == Clockwise ? 1 : -1;

            return new Tetrahedral(u, (int[])vs.Clone(), p);
        }

        public static Topology CreateExtendedTetrahedral(int u, int[] vs, Configuration configuration)
        {
            if (configuration.Type != Implicit
                    && configuration.Type != Types.ExtendedTetrahedral)
                throw new ArgumentException(configuration.Type
                                                           + "invalid extended tetrahedral configuration");

            int p = configuration.Shorthand == Clockwise ? 1 : -1;

            return new ExtendedTetrahedral(u, 
                                           Arrays.CopyOf(vs, vs.Length),
                                           p);
        }

        /// <summary>
        /// Define trigonal topology of the given configuration.
        /// </summary>
        /// <param name="u">central atom</param>
        /// <param name="vs">vertices surrounding u, the first is the vertex we are looking from</param>
        /// <param name="configuration">the trigonal configuration, @DB1, @Db1, @ or @@</param>
        /// <returns>topology instance for that configuration</returns>
        /// <seealso cref="Configuration"/>
        public static Topology CreateTrigonal(int u, int[] vs, Configuration configuration)
        {
            if (configuration.Type != Implicit
                    && configuration.Type != Types.DoubleBond)
                throw new ArgumentException(configuration.Type
                                                           + "invalid tetrahedral configuration");

            int p = configuration.Shorthand == Clockwise ? 1 : -1;

            return new Trigonal(u,
                                Arrays.CopyOf(vs, vs.Length),
                                p);
        }

        /// <summary>
        /// Convert an implicit configuration ('@' or '@@') c, to an explicit one
        /// (e.g. @TH1).
        /// </summary>
        /// <remarks>
        /// Implicit Valence Explicit Example
        /// <code>
        /// @ 4       @TH1     O[C@H](N)C or O[C@]([H])(N)C
        /// @@ 4       @TH2     O[C@@H](N)C or O[C@@]([H])(N)C
        /// @ 3       @TH1     C[S@](N)=O
        /// @@ 3       @TH2     C[S@@](N)=O
        /// @ 2       @AL1     OC=[C@]=CO
        /// @ 2       @AL2     OC=[C@@]=CO
        /// @ 5       @TB1     S[As@](F)(Cl)(Br)C=O
        /// @@ 5       @TB2     S[As@@](F)(Cl)(Br)C=O
        /// @ 5       @OH1     S[Co@@](F)(Cl)(Br)(I)C=O
        /// @@ 5       @OH2     O=C[Co@](F)(Cl)(Br)(I)S
        /// </code>
        /// </remarks>
        /// <param name="g">chemical graph</param>
        /// <param name="u">the atom to which the configuration is associated</param>
        /// <param name="c">implicit configuration (<see cref="Configuration.AntiClockwise"/> or <see cref="Configuration.Clockwise"/>)</param>
        /// <returns>an explicit configuration or <see cref="Configuration.Unknown"/></returns>
        public static Configuration ToExplicit(Graph g, int u, Configuration c)
        {
            // already explicit
            if (c.Type != Implicit)
                return c;

            int deg = g.Degree(u);
            int valence = deg + g.GetAtom(u).NumOfHydrogens;

            // tetrahedral topology, square planar must always be explicit
            if (valence == 4)
            {
                return c == AntiClockwise ? TH1 : TH2;
            }

            // tetrahedral topology with implicit lone pair or double bond (Sp2)
            // atoms (todo)
            else if (valence == 3)
            {
                // XXX: sulfoxide and selenium special case... would be better to compute
                // hybridization don't really like doing this here but is sufficient
                // for now
                if (g.GetAtom(u).Element == Element.Sulfur || g.GetAtom(u).Element == Element.Selenium)
                {
                    int sb = 0, db = 0;
                    int dd = g.Degree(u);
                    for (int j = 0; j < dd; ++j)
                    {
                        Edge e = g.EdgeAt(u, j);
                        if (e.Bond.Order == 1)
                            sb++;
                        else if (e.Bond.Order == 2)
                            db++;
                        else return Configuration.Unknown;
                    }
                    int q = g.GetAtom(u).Charge;
                    if ((q == 0 && sb == 2 && db == 1) || (q == 1 && sb == 3))
                        return c == AntiClockwise ? TH1 : TH2;
                    else
                        return Configuration.Unknown;
                }

                if (g.GetAtom(u).Element == Element.Phosphorus)
                {
                    if (g.BondedValence(u) == 3 && g.ImplHCount(u) == 0 && g.GetAtom(u).Charge == 0)
                    {
                        return c == AntiClockwise ? TH1 : TH2;
                    }
                }

                // for the atom centric double bond configuration check there is
                // a double bond and it's not sill tetrahedral specification such
                // as [C@-](N)(O)C
                int nDoubleBonds = 0;
                int d = g.Degree(u);
                for (int j = 0; j < d; ++j)
                {
                     Edge e = g.EdgeAt(u, j);
                    if (e.Bond == Bond.Double)
                        nDoubleBonds++;
                }

                if (nDoubleBonds == 1)
                {
                    return c == AntiClockwise ? DB1 : DB2;
                }
                else
                {
                    return Configuration.Unknown;
                }
            }

            // odd number of cumulated double bond systems (e.g. allene)
            else if (deg == 2)
            {
                // check both bonds are double
                int d = g.Degree(u);
                for (int j = 0; j < d; ++j)
                {
                    Edge e = g.EdgeAt(u, j);
                    if (e.Bond != Bond.Double)
                        return Configuration.Unknown;
                }
                return c == AntiClockwise ? AL1 : AL2;
            }

            // trigonal bipyramidal
            else if (valence == 5)
            {
                return c == AntiClockwise ? TB1 : TB2;
            }

            // octahedral
            else if (valence == 6)
            {
                return c == AntiClockwise ? OH1 : OH2;
            }

            return Configuration.Unknown;
        }

        public static Topology Create(int u, int[] vs, IList<Edge> es, Configuration c)
        {
            if (c.Type == Types.Implicit)
                throw new ArgumentOutOfRangeException(nameof(c), "configuration must be explicit, @TH1/@TH2 instead of @/@@");

            // only tetrahedral is handled for now
            if (c.Type == Types.Tetrahedral)
            {
                return CreateTetrahedral(u, vs, c);
            }
            else if (c.Type == Types.DoubleBond)
            {
                return CreateTrigonal(u, vs, c);
            }
            else if (c.Type == Types.ExtendedTetrahedral)
            {
                return CreateExtendedTetrahedral(u, vs, c);
            }

            return Unknown;
        }

        class UnknownTopology : Topology
        {
            public override int Atom
            {
                get
                {
                    throw new NotSupportedException("Unknown topology");
                }
            }
            public override Configuration Configuration => Configuration.Unknown;

            public override Topology OrderBy(int[] rank)
            {
                return this;
            }

            public override Topology Transform(int[] mapping)
            {
                return this;
            }

            public override void Copy(int[] dest)
            {
            }
        }

        private sealed class Tetrahedral : Topology
        {
            private readonly int u;
            private readonly int[] vs;
            private readonly int p;

            public Tetrahedral(int u, int[] vs, int p)
            {
                if (vs.Length != 4)
                    throw new ArgumentOutOfRangeException(nameof(vs), "Tetrahedral topology requires 4 vertices - use the 'centre' vertex to mark implicit verticies");
                this.u = u;
                this.vs = vs;
                this.p = p;
            }

            /// <inheritdoc/>
            public override int Atom => u;

            /// <inheritdoc/>
            public override Configuration Configuration
                => p < 0 ? Configuration.TH1 : Configuration.TH2;

            /// <inheritdoc/>
            public override Topology OrderBy(int[] rank)
            {
                return new Tetrahedral(u,
                                       Sort(vs, rank),
                                       p * Parity4(vs, rank));
            }

            /// <inheritdoc/>
            public override Topology Transform(int[] mapping)
            {
                int[] ws = new int[vs.Length];
                for (int i = 0; i < vs.Length; i++)
                    ws[i] = mapping[vs[i]];
                return new Tetrahedral(mapping[u], ws, p);
            }

            public override void Copy(int[] dest)
            {
                Array.Copy(vs, 0, dest, 0, 4);
            }

            public override Configuration ConfigurationOf(int[] rank)
            {
                return p * Parity4(vs, rank) < 0 ? TH1 : TH2;
            }

            public override string ToString()
            {
                return u + " " + Arrays_ToString(vs) + ":" + p;
            }
        }

        private sealed class ExtendedTetrahedral : Topology
        {
            private readonly int u;
            private readonly int[] vs;
            private readonly int p;

            public ExtendedTetrahedral(int u, int[] vs, int p)
            {
                if (vs.Length != 4)
                    throw new ArgumentOutOfRangeException(nameof(vs), "Tetrahedral topology requires 4 vertices - use the 'centre' vertex to mark implicit verticies");
                this.u = u;
                this.vs = vs;
                this.p = p;
            }

            /// <inheritdoc/>
            public override int Atom => u;

            /// <inheritdoc/>
            public override Configuration Configuration
                =>  p < 0 ? Configuration.AL1 : Configuration.AL2;

            /// <inheritdoc/>
            public override Topology OrderBy(int[] rank)
            {
                return new ExtendedTetrahedral(u,
                                               Sort(vs, rank),
                                               p * Parity(vs, rank));
            }

            /// <inheritdoc/>
            public override Topology Transform(int[] mapping)
            {
                int[] ws = new int[vs.Length];
                for (int i = 0; i < vs.Length; i++)
                    ws[i] = mapping[vs[i]];
                return new ExtendedTetrahedral(mapping[u], ws, p);
            }

            public override void Copy(int[] dest)
            {
                Array.Copy(vs, 0, dest, 0, 3);
            }

            public override string ToString()
            {
                return u + " " + Arrays_ToString(vs) + ":" + p;
            }
        }

        private sealed class Trigonal : Topology
        {
            private readonly int u;
            private readonly int[] vs;
            private readonly int p;

            public Trigonal(int u, int[] vs, int p)
            {
                if (vs.Length != 3)
                    throw new ArgumentOutOfRangeException(nameof(vs), "Trigonal topology requires 3 vertices - use the 'centre' vertex to mark implicit verticies");
                this.u = u;
                this.vs = vs;
                this.p = p;
            }

            /// <inheritdoc/>
            public override int Atom => u;

            /// <inheritdoc/>
            public override Configuration Configuration
                => p < 0 ? Configuration.DB1 : Configuration.DB2;

            /// <inheritdoc/>
            public override Topology OrderBy(int[] rank)
            {
                return new Trigonal(u,
                                    Sort(vs, rank),
                                    p * Parity(vs, rank));
            }

            /// <inheritdoc/>
            public override Topology Transform(int[] mapping)
            {
                int[] ws = new int[vs.Length];
                for (int i = 0; i < vs.Length; i++)
                    ws[i] = mapping[vs[i]];
                return new Trigonal(mapping[u], ws, p);
            }

            public override void Copy(int[] dest)
            {
                Array.Copy(vs, 0, dest, 0, 3);
            }

            public override string ToString()
            {
                return u + " " + Arrays_ToString(vs) + ":" + p;
            }
        }

        private static string Arrays_ToString(int[] vs)
        {
            var sb = new StringBuilder();
            sb.Append("[");
            foreach (var v in vs)
            {
                if (sb.Length > 1)
                    sb.Append(", ");
                sb.Append(v);
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}

