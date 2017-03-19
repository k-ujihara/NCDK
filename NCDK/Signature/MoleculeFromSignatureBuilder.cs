/* Copyright (C) 2009-2010 maclean {gilleain.torrance@gmail.com}
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
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.FaulonSignatures;

namespace NCDK.Signature
{
    /// <summary>
    /// Builds a molecule from a signature.
    /// </summary>
    // @cdk.module signature
    // @author maclean
    // @cdk.githash
    public class MoleculeFromSignatureBuilder : AbstractGraphBuilder
    {
         /// <summary>
         /// The chem object builder
         /// </summary>
        private IChemObjectBuilder builder;

        /// <summary>
        /// The container that is being constructed
        /// </summary>
        private IAtomContainer container;

         /// <summary>
         /// Uses the chem object builder for making molecules.
         /// </summary>
         /// <param name="builder">a builder for CDK molecules.</param>
        public MoleculeFromSignatureBuilder(IChemObjectBuilder builder)
        {
            this.builder = builder;
        }

        /// <inheritdoc/>
        public override void MakeEdge(int vertexIndex1, int vertexIndex2, string vertexSymbol1, string vertexSymbol2,
                string edgeLabel)
        {
            if (edgeLabel.Equals(""))
            {
                container.AddBond(container.Atoms[vertexIndex1], container.Atoms[vertexIndex2], BondOrder.Single);
            }
            else if (edgeLabel.Equals("="))
            {
                container.AddBond(container.Atoms[vertexIndex1], container.Atoms[vertexIndex2], BondOrder.Double);
            }
            else if (edgeLabel.Equals("#"))
            {
                container.AddBond(container.Atoms[vertexIndex1], container.Atoms[vertexIndex2], BondOrder.Triple);
            }
            else if (edgeLabel.Equals("p"))
            {
                IBond bond = builder.CreateBond(container.Atoms[vertexIndex1],
                        container.Atoms[vertexIndex2], BondOrder.Single);
                bond.IsAromatic = true;
                container.Bonds.Add(bond);
            }
        }

        /// <inheritdoc/>
        public override void MakeGraph()
        {
            this.container = this.builder.CreateAtomContainer();
        }

        /// <inheritdoc/>
        public override void MakeVertex(string label)
        {
            this.container.Atoms.Add(this.builder.CreateAtom(label));
        }

        /// <summary>
        /// Gets the atom container.
        /// </summary>
        /// <returns>the constructed atom container</returns>
        public IAtomContainer GetAtomContainer()
        {
            return this.container;
        }
    }
}

