/* Copyright (C) 2009  Gilleain Torrance <gilleain.torrance@gmail.com>
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
using System;
using System.Collections.Generic;
using System.Collections;

namespace NCDK.Graphs
{
    /**
     * The base class for permutors of atom containers, with a single abstract
     * method <code>ContainerFromPermutation</code> that should be implemented in
     * concrete derived classes.
     *
     * @author maclean
     * @cdk.githash
     * @cdk.created    2009-09-09
     * @cdk.keyword    permutation
     * @cdk.module     standard
     */
    public abstract class AtomContainerPermutor : Permutor, IEnumerator<IAtomContainer>
    {
        /**
         * The atom container that is permuted at each step.
         */
        protected IAtomContainer atomContainer;

        public IAtomContainer Current { get; private set; }

        object IEnumerator.Current => this.Current;

        /**
         * Start the permutor off with an initial atom container, and the size of
         * the permutation.
         *
         * @param atomContainer
         */
        public AtomContainerPermutor(int size, IAtomContainer atomContainer)
            : base(size)
        {
            this.atomContainer = atomContainer;
        }

        /**
         * Convert a permutation (expressed as a list of numbers) into a permuted
         * atom container. This will differ depending on the desired effect of the
         * permutation (atoms or bonds, for example).
         *
         * @return the atom container corresponding to this permutation
         */
        public abstract IAtomContainer ContainerFromPermutation(int[] permutation);

        /**
         * Get a new container, but randomly skip forwards in the list of possible
         * permutations to generate it.
         *
         * @return a random next permuted atom container
         */
        public bool RandomNext()
        {
            if (!HasNext())
            {
                return false;
            }
            else
            {
                Current = this.ContainerFromPermutation(this.GetRandomNextPermutation());
                return true;
            }
        }

        public bool MoveNext()
        {
            if (!HasNext())
            {
                return false;
            }
            else
            {
                Current = this.ContainerFromPermutation(this.GetNextPermutation());
                return true;
            }
        }

        public void Dispose()
        {
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }
    }
}
