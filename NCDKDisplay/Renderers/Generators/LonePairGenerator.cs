/* Copyright (C) 2009  Gilleain Torrance <gilleain.torrance@gmail.com>
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

using NCDK.Geometries;
using NCDK.Numerics;
using NCDK.Renderers.Elements;
using System.Windows.Media;
using static NCDK.Renderers.Generators.Standards.VecmathUtil;
using WPF = System.Windows;

namespace NCDK.Renderers.Generators
{
    /// <summary>
    /// Generate the symbols for lone pairs.
    /// </summary>
    // @author maclean
    // @cdk.module renderextra
    // @cdk.githash
    public class LonePairGenerator : IGenerator<IAtomContainer>
    {
        public LonePairGenerator() { }

        /// <inheritdoc/>
        public IRenderingElement Generate(IAtomContainer container, RendererModel model)
        {
            ElementGroup group = new ElementGroup();

            // TODO : put into RendererModel
            const double SCREEN_RADIUS = 1.0;
            // separation between centers
            const double SCREEN_SEPARATION = 2.5;
            Color RADICAL_COLOR = WPF.Media.Colors.Black;

            // XXX : is this the best option?
            double ATOM_RADIUS = model.GetAtomRadius();

            double scale = model.GetScale();
            double modelAtomRadius = ATOM_RADIUS / scale;
            double modelPointRadius = SCREEN_RADIUS / scale;
            double modelSeparation = SCREEN_SEPARATION / scale;
            foreach (var lonePair in container.LonePairs)
            {
                IAtom atom = lonePair.Atom;
                Vector2 point = atom.Point2D.Value;
                int align = GeometryUtil.GetBestAlignmentForLabelXY(container, atom);
                var center = ToPoint(point);
                var diff = new WPF::Vector();
                if (align == 1)
                {
                    center.X += modelAtomRadius;
                    diff.Y += modelSeparation;
                }
                else if (align == -1)
                {
                    center.X -= modelAtomRadius;
                    diff.Y += modelSeparation;
                }
                else if (align == 2)
                {
                    center.Y -= modelAtomRadius;
                    diff.X += modelSeparation;
                }
                else if (align == -2)
                {
                    center.Y += modelAtomRadius;
                    diff.X += modelSeparation;
                }
                group.Add(new OvalElement(center + diff, modelPointRadius, true, RADICAL_COLOR));
                group.Add(new OvalElement(center - diff, modelPointRadius, true, RADICAL_COLOR));
            }
            return group;
        }
    }
}
