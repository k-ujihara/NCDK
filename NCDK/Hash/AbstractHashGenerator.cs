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
using System;

namespace NCDK.Hash
{
    /**
     * An abstract hash function providing several utility methods to be used by
     * other hashing functions.
     *
     * @author John May
     * @cdk.module hash
     * @cdk.githash
     */
#if TEST
    public
#endif
    class AbstractHashGenerator
    {
        /* pseudorandom number generator */
        private readonly Pseudorandom pseudorandom;

        /**
         * Construct an abstract hash function providing the pseudorandom number
         * generator.
         *
         * @param pseudorandom a pseudorandom number generator
         * @throws NullPointerException the pseudorandom number generator was null
         */
        public AbstractHashGenerator(Pseudorandom pseudorandom)
        {
            if (pseudorandom == null) throw new ArgumentNullException("null pseduorandom number generator provided");
            this.pseudorandom = pseudorandom;
        }

        /**
         * Create a copy of the array of long values.
         *
         * @param src original values
         * @return copy of the original values
         * @see Arrays#CopyOf(long[], int)
         */
        public static long[] Copy(long[] src)
        {
            var ret = new long[src.Length];
            Array.Copy(src, ret, src.Length);
            return ret;
        }

        /**
         * Copy the values from the source (src) array to the destination (dest).
         *
         * @param src  source values
         * @param dest destination of the source copy
         * @see System#Arraycopy(Object, int, Object, int, int);
         */
        public static void Copy(long[] src, long[] dest)
        {
            Array.Copy(src, 0, dest, 0, dest.Length);
        }

        /**
         * Generate the next random number.
         *
         * @param seed a {@literal long} value to seed a pseudorandom number
         *             generator
         * @return next pseudorandom number
         */
        public long Rotate(long seed)
        {
            return pseudorandom.Next(seed);
        }

        /**
         * Rotate a <i>value</i>, <i>n</i> times. The rotation uses a pseudorandom
         * number generator to sequentially generate values seed on the previous
         * value.
         *
         * @param value the {@literal long} value to rotate
         * @param n     the number of times to rotate the value
         * @return the {@literal long} value rotated the specified number of times
         */
        public long Rotate(long value, int n)
        {
            while (n-- > 0)
                value = pseudorandom.Next(value);
            return value;
        }

        /**
         * Returns the value of the lowest three bits. This value is between 0 and 7
         * inclusive.
         *
         * @param value a {@literal long} value
         * @return the {@literal int} value of the lowest three bits.
         */
        public static int LowestThreeBits(long value)
        {
            return (int)(value & 0x7);
        }

        /**
         * Distribute the provided value across the set of {@literal long} values.
         *
         * @param value a {@literal long} value to distribute
         * @return the {@literal long} value distributed a set amount
         */
        public long Distribute(long value)
        {
            // rotate 1-8 times
            return Rotate(value, 1 + LowestThreeBits(value));
        }

        /**
         * Convert an IAtomContainer to an adjacency list.
         *
         * @param container the container to convert
         * @return adjacency list representation
         */
        public static int[][] ToAdjList(IAtomContainer container)
        {

            if (container == null) throw new ArgumentException("atom container was null");

            int n = container.Atoms.Count;

            int[][] graph = Arrays.CreateJagged<int>(n, 16);
            int[] degree = new int[n];

            foreach (var bond in container.Bonds)
            {

                int v = container.Atoms.IndexOf(bond.Atoms[0]);
                int w = container.Atoms.IndexOf(bond.Atoms[1]);

                if (v < 0 || w < 0)
                    throw new ArgumentException("bond at index " + container.Bonds.IndexOf(bond)
                            + " contained an atom not pressent in molecule");

                graph[v][degree[v]++] = w;
                graph[w][degree[w]++] = v;

                // if the vertex degree of v or w reaches capacity, double the size
                if (degree[v] == graph[v].Length) graph[v] = Arrays.CopyOf(graph[v], degree[v] * 2);
                if (degree[w] == graph[w].Length) graph[w] = Arrays.CopyOf(graph[w], degree[w] * 2);
            }

            for (int v = 0; v < n; v++)
            {
                graph[v] = Arrays.CopyOf(graph[v], degree[v]);
            }

            return graph;
        }
    }
}
