/* Copyright (C) 2009  Gilleain Torrance <gilleain@users.sf.net>
 *               2009  Stefan Kuhn <shk3@users.sf.net>
 *
 * Contact: cdk-devel@list.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
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
using NCDK.Renderers.Elements;
using NCDK.Renderers.Generators.Parameters;
using System.Collections.Generic;
using System.Windows.Media;
using WPF = System.Windows;

namespace NCDK.Renderers.Generators
{
    /// <summary>
    /// Produce a bounding rectangle for various chem objects.
    /// </summary>
    // @author maclean
    // @cdk.module renderextra
    // @cdk.githash
    public class BoundsGenerator : IGenerator<IReaction>
    {
        /// <summary>
        /// The color of the box drawn at the bounds of a
        /// molecule, molecule set, or reaction.
        /// </summary>
        public class BoundsColor : AbstractGeneratorParameter<Color?>
        {
            /// <inheritdoc/>
            public override Color? Default => WPF.Media.Colors.LightGray;
        }

        private IGeneratorParameter<Color?> boundsColor = new BoundsColor();

        public BoundsGenerator() { }

        /// <inheritdoc/>
        public IRenderingElement Generate(IReaction reaction, RendererModel model)
        {
            ElementGroup elementGroup = new ElementGroup();
            var reactants = reaction.Reactants;
            if (reactants != null)
            {
                elementGroup.Add(this.Generate(reactants));
            }

            var products = reaction.Products;
            if (products != null)
            {
                elementGroup.Add(this.Generate(products));
            }

            return elementGroup;
        }

        private IRenderingElement Generate(IChemObjectSet<IAtomContainer> moleculeSet)
        {
            var totalBounds = BoundsCalculator.CalculateBounds(moleculeSet);

            return new RectangleElement(totalBounds, boundsColor.Value.Value);
        }

        /// <inheritdoc/>
        public IList<IGeneratorParameter> Parameters
            => new IGeneratorParameter[] { boundsColor };
    }
}
