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
    // @cdk.module  isomorphism
    // @cdk.githash
    public class QueryAtomContainer : Silent.AtomContainer, IQueryAtomContainer
    {
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("QueryAtomContainer(");
            sb.Append(ToStringInner());
            sb.Append(')');
            return sb.ToString();
        }

        private readonly IChemObjectBuilder builder;

        /// <inheritdoc/>
        public override IChemObjectBuilder Builder => builder;

        /// <summary>
        /// Constructs an AtomContainer with a copy of the atoms and electronContainers
        /// of another AtomContainer (A shallow copy, i.e., with the same objects as in
        /// the original AtomContainer).
        /// </summary>
        /// <param name="container">An AtomContainer to copy the atoms and electronContainers from</param>
        /// <param name="builder"></param>
        public QueryAtomContainer(IAtomContainer container, IChemObjectBuilder builder)
            : base(container)
        {
            this.builder = builder;
        }

        /// <summary>
        ///  Constructs an empty AtomContainer that will contain a certain number of
        ///  atoms and electronContainers. It will set the starting array lengths to the
        ///  defined values, but will not create any Atom or ElectronContainer's.
        /// </summary>
        public QueryAtomContainer(IChemObjectBuilder builder)
            : base()
        {
            this.builder = builder;
        }
    }
}
       