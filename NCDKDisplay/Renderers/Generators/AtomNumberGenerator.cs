/*  Copyright (C) 2009  Gilleain Torrance <gilleain@users.sf.net>
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *  All we ask is that proper credit is given for our work, which includes
 *  - but is not limited to - adding the above copyright notice to the beginning
 *  of your source code files, and to any copyright notice that you may distribute
 *  with programs based on this work.
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
using NCDK.Numerics;
using NCDK.Renderers.Colors;
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
    /// <see cref="IGenerator"/> for <see cref="IAtomContainer"/>s that will draw atom numbers for the atoms.
    /// </summary>
    // @author      maclean
    // @cdk.module  renderextra
    // @cdk.githash
    public class AtomNumberGenerator : IGenerator<IAtomContainer>
    {
        /// <summary>Color to draw the atom numbers with.</summary>
        public class AtomNumberTextColor : AbstractGeneratorParameter<Color?>
        {
            /// <inheritdoc/>
            public override Color? Default => WPF.Media.Colors.Black;
        }

        private IGeneratorParameter<Color?> textColor = new AtomNumberTextColor();

        /// <summary> bool parameter indicating if atom numbers should be drawn, allowing
        /// this feature to be disabled temporarily. 
        /// </summary>
        public class WillDrawAtomNumbers : AbstractGeneratorParameter<bool?>
        {
            /// <inheritdoc/>
            public override bool? Default => true;
        }

        private WillDrawAtomNumbers willDrawAtomNumbers = new WillDrawAtomNumbers();

        /// <summary> The color scheme by which to color the atom numbers, if
        /// the <see cref="ColorByType"/> bool is true.</summary>
        public class AtomColorer : AbstractGeneratorParameter<IAtomColorer>
        {
            /// <inheritdoc/>
            public override IAtomColorer Default => new CDK2DAtomColors();
        }

        private IGeneratorParameter<IAtomColorer> atomColorer = new AtomColorer();

        /// <summary>bool to indicate of the <see cref="AtomColorer"/> scheme will be used.</summary>
        public class ColorByType : AbstractGeneratorParameter<bool?>
        {
            /// <inheritdoc/>
            public override bool? Default => false;
        }
        private IGeneratorParameter<bool?> colorByType = new ColorByType();

        /// <summary>
        /// Offset vector in screen space coordinates where the atom number label
        /// will be placed.
        /// </summary>
        public class Offset : AbstractGeneratorParameter<Vector2?>
        {
            /// <inheritdoc/>
            public override Vector2? Default => Vector2.Zero;
        }

        private Offset offset = new Offset();

        /// <inheritdoc/>
        public IRenderingElement Generate(IAtomContainer container, RendererModel model)
        {
            ElementGroup numbers = new ElementGroup();
            if (!model.GetV<bool>(typeof(WillDrawAtomNumbers))) return numbers;

            Vector2 offset = new Vector2(this.offset.Value.Value.X, -this.offset.Value.Value.Y);
            offset *= (1 / model.GetV<double>(typeof(Scale)));

            int number = 1;
            foreach (var atom in container.Atoms)
            {
                Vector2 point = atom.Point2D.Value + offset;
                numbers.Add(
                    new TextElement(ToPoint(point), number.ToString(), 
                        colorByType.Value.Value ? atomColorer.Value.GetAtomColor(atom) : textColor.Value.Value));
                number++;
            }
            return numbers;
        }

        /// <inheritdoc/>
        public IList<IGeneratorParameter> Parameters =>
            new IGeneratorParameter[] { textColor, willDrawAtomNumbers, offset, atomColorer, colorByType };
    }
}
