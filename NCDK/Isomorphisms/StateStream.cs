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
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using System.Collections;
using System.Collections.Generic;

namespace NCDK.Isomorphisms
{
    /**
     * Given a (subgraph-)isomorphism state this class can lazily iterate over the
     * mappings in a non-recursive manner. The class currently implements and {@link
     * IEnumerator} but is better suited to the {@code Stream} class (which will be
     * available in JDK 8).
     *
     * @author John May
     * @cdk.module isomorphism
     */
#if TEST
    public
#endif
    sealed class StateStream : IEnumerable<int[]>
    {

        /// <summary>A mapping state.</summary>
        private readonly State state;

        /// <summary>The stack replaces the call-stack in a recursive matcher.</summary>
        private readonly CandidateStack stack;

        /// <summary>Current candidates.</summary>
        private int n = 0, m = -1;

        /**
         * Create a stream for the provided state.
         *
         * @param state the state to stream over
         */
        public StateStream(State state)
        {
            this.state = state;
            this.stack = new CandidateStack(state.NMax());
        }

        public IEnumerator<int[]> GetEnumerator()
        {
            if (state.NMax() == 0 || state.MMax() == 0)
                yield break;
            int[] current;
            while ((current = FindNext()) != null)
                yield return current;
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /**
         * Finds the next mapping from the current state.
         *
         * @return the next state (or null if none)
         */
        private int[] FindNext()
        {
            while (Map()) ;
            if (state.Count == state.NMax()) return state.Mapping();
            return null;
        }

        /**
         * Progress the state-machine - the function return false when a mapping is
         * found on the mapping is done.
         *
         * @return the state is partial
         */
        private bool Map()
        {

            // backtrack - we've tried all possible n or m, remove the last mapping
            if ((n == state.NMax() || m == state.MMax()) && !stack.IsEmpty)
                state.Remove(n = stack.PopN(), m = stack.PopM());

            while ((m = state.NextM(n, m)) < state.MMax())
            {
                if (state.Add(n, m))
                {
                    stack.Push(n, m);
                    n = state.NextN(-1);
                    m = -1;
                    return n < state.NMax();
                }
            }

            return state.Count > 0 || m < state.MMax();
        }


        /**
         * A fixed size stack to keep track of which vertices are mapped. This stack
         * allows us to turn the recursive algorithms it to lazy iterating mappers.
         * A reclusive call is usually implemented as call-stack which stores the
         * variable in each subroutine invocation. For the mapping we actually only
         * need store the candidates.
         */
        private sealed class CandidateStack
        {

            /// <summary>Candidate storage.</summary>
            private readonly int[] ns, ms;

            /// <summary>Size of each stack.</summary>
            private int nSize, mSize;

            public CandidateStack(int capacity)
            {
                ns = new int[capacity];
                ms = new int[capacity];
            }

            /**
             * Push a candidate mapping on to the stack.
             *
             * @param n query candidate
             * @param m target candidate
             */
            public void Push(int n, int m)
            {
                ns[nSize++] = n;
                ms[mSize++] = m;
            }

            /**
             * Pops the G1 candidate.
             *
             * @return the previous 'n' candidate
             */
            public int PopN()
            {
                return ns[--nSize];
            }

            /**
             * Pops the G2 candidate.
             *
             * @return the previous 'm' candidate
             */
            public int PopM()
            {
                return ms[--mSize];
            }

            /**
             * Is the stack empty - if so no candidates can be popped.
             *
             * @return
             */
            public bool IsEmpty => nSize == 0 && mSize == 0;
        }
    }
}