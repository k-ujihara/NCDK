/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
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
using System.Text;

namespace NCDK.Isomorphisms.Matchers
{
    /**
     * @cdk.module  isomorphism
     * @cdk.githash
     */
    public class QueryAtomContainer : Default.AtomContainer, IQueryAtomContainer
    {
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("QueryAtomContainer(");
            sb.Append(ToStringInner());
            sb.Append(')');
            return sb.ToString();
        }

        /**
         * Constructs an AtomContainer with a copy of the atoms and electronContainers
         * of another AtomContainer (A shallow copy, i.e., with the same objects as in
         * the original AtomContainer).
         *
         * @param  container  An AtomContainer to copy the atoms and electronContainers from
         */
        public QueryAtomContainer(IAtomContainer container, IChemObjectBuilder builder)
            : base(container)
        {
            this.Builder = builder;
        }

        /**
         *  Constructs an empty AtomContainer that will contain a certain number of
         *  atoms and electronContainers. It will set the starting array lengths to the
         *  defined values, but will not create any Atom or ElectronContainer's.
         *
         *@param  atomCount        Number of atoms to be in this container
         *@param  bondCount        Number of bonds to be in this container
         *@param  lpCount          Number of lone pairs to be in this container
         *@param  seCount          Number of single electrons to be in this container
         *
         */
        public QueryAtomContainer(IChemObjectBuilder builder)
			: base()
        {
            Builder = builder;
        }
    }
}
       