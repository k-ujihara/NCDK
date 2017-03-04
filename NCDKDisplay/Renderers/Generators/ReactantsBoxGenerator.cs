/* Copyright (C) 2009  Stefan Kuhn <shk3@users.sf.net>
 *
 *  Contact: cdk-devel@list.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Renderers.Elements;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using static NCDK.Renderers.Generators.BasicSceneGenerator;
using static NCDK.Renderers.Generators.ReactionSceneGenerator;

namespace NCDK.Renderers.Generators
{
    /// <summary>
    /// Generate the symbols for radicals.
    /// </summary>
    // @author maclean
    // @cdk.module renderextra
    // @cdk.githash
    public class ReactantsBoxGenerator : IGenerator<IReaction>
    {
        /// <inheritdoc/>
        public IRenderingElement Generate(IReaction reaction, RendererModel model)
        {
            if (!model.GetV<bool>(typeof(ShowReactionBoxes))) return null;
            if (reaction.Reactants.Count == 0) return new ElementGroup();

            double separation = model.GetV<double>(typeof(BondLength)) / model.GetV<double>(typeof(Scale)) / 2;
            var totalBounds = BoundsCalculator.CalculateBounds(reaction.Reactants);

            ElementGroup diagram = new ElementGroup();
            double minX = totalBounds.Left;
            double minY = totalBounds.Top;
            double maxX = totalBounds.Right;
            double maxY = totalBounds.Bottom;
            var foregroundColor = model.GetParameter<Color>(typeof(BasicSceneGenerator.ForegroundColor)).Value;
            diagram.Add(new RectangleElement(new Rect(minX - separation, minY - separation, maxX + separation, maxY + separation), foregroundColor));
            diagram.Add(new TextElement(new Point((minX + maxX) / 2, minY - separation), "Reactants", foregroundColor));
            return diagram;
        }

        /// <inheritdoc/>
        public IList<IGeneratorParameter> Parameters => new IGeneratorParameter[] { };
    }
}
