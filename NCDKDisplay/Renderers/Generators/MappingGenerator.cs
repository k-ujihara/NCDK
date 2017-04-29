/* Copyright (C) 2009  Gilleain Torrance <gilleain@users.sf.net>
 *               2009  Stefan Kuhn <sh3@users.sf.net>
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
using NCDK.Numerics;
using NCDK.Renderers.Elements;
using NCDK.Renderers.Generators.Parameters;
using System.Collections.Generic;
using System.Windows.Media;
using static NCDK.Renderers.Generators.BasicSceneGenerator;
using static NCDK.Renderers.Generators.Standards.VecmathUtil;
using WPF = System.Windows;

namespace NCDK.Renderers.Generators
{
    /// <summary>
    /// <see cref="IGenerator{T}"/> that will show how atoms map between the reactant and product side.
    /// </summary>
    // @cdk.module renderextra
    // @cdk.githash
    public class MappingGenerator : IGenerator<IReaction>
    {
        /// <summary>
        /// The width on screen of an atom-atom mapping line.
        /// </summary>
        public class AtomAtomMappingLineColor : AbstractGeneratorParameter<Color?>
        {
            /// <inheritdoc/>
            public override Color? Default => WPF.Media.Colors.Gray;
        }

        private IGeneratorParameter<Color?> atomAtomMappingLineColor = new AtomAtomMappingLineColor();

        /// <summary>
        /// The width on screen of an atom-atom mapping line.
        /// </summary>
        public class MappingLineWidth : AbstractGeneratorParameter<double>
        {
            /// <inheritdoc/>
            public override double Default => 1;
        }

        private IGeneratorParameter<double> mappingLineWidth = new MappingLineWidth();

        /// <summary>bool by which atom-atom mapping depiction can be temporarily disabled.</summary>
        public class ShowAtomAtomMapping : AbstractGeneratorParameter<bool?>
        {
            /// <inheritdoc/>
            public override bool? Default => true;
        }

        private IGeneratorParameter<bool?> showAtomAtomMapping = new ShowAtomAtomMapping();

        public MappingGenerator() { }

        /// <inheritdoc/>
        public IRenderingElement Generate(IReaction reaction, RendererModel model)
        {
            if (!showAtomAtomMapping.Value.Value) return null;
            ElementGroup elementGroup = new ElementGroup();
            Color mappingColor = atomAtomMappingLineColor.Value.Value;
            foreach (var mapping in reaction.Mappings)
            {
                // XXX assume that there are only 2 endpoints!
                // XXX assume that the ChemObjects are actually IAtoms...
                IAtom endPointA = (IAtom)mapping[0];
                IAtom endPointB = (IAtom)mapping[1];
                var pointA = ToPoint(endPointA.Point2D.Value);
                var pointB = ToPoint(endPointB.Point2D.Value);
                elementGroup.Add(new LineElement(pointA, pointB, GetWidthForMappingLine(model), mappingColor));
            }
            return elementGroup;
        }

        /// <summary>
        /// Determine the width of an atom atom mapping, returning the width defined
        /// in the model. Note that this will be scaled
        /// to the space of the model.
        /// </summary>
        /// <param name="model">the renderer model</param>
        /// <returns>a double in chem-model space</returns>
        private double GetWidthForMappingLine(RendererModel model)
        {
            double scale = model.GetV<double>(typeof(Scale));
            return mappingLineWidth.Value / scale;
        }

        /// <inheritdoc/>
        public IList<IGeneratorParameter> Parameters =>
            new IGeneratorParameter[] { showAtomAtomMapping, mappingLineWidth, atomAtomMappingLineColor };
    }
}
