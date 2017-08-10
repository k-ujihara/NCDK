/* Copyright (C) 2008  Arvid Berg <goglepox@users.sf.net>
 *               2010  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Renderers.Fonts;
using NCDK.Renderers.Generators.Parameters;
using System.Collections.Generic;
using System.Windows.Media;
using WPF = System.Windows;

namespace NCDK.Renderers.Generators
{
    /// <summary>
    /// This generator does not create any elements, but acts as a holding place
    /// for various generator parameters used by most drawings, such as the zoom,
    /// background color, margin, etc.
    /// </summary>
    // @cdk.module renderbasic
    // @cdk.githash
    public class BasicSceneGenerator : IGenerator<IAtomContainer>
    {
        /// <summary>
        /// The width of the head of arrows.
        /// </summary>
        // @author egonw
        public class ArrowHeadWidth : AbstractGeneratorParameter<double?>
        {
            /// <summary>Returns the default arrow head width.</summary>
            /// <returns>10.0 */</returns>
            public override double? Default => 10;
        }

        private IGeneratorParameter<double?> arrowHeadWidth = new ArrowHeadWidth();

        /// <summary>
        /// Determines if tooltips are to be shown.
        /// </summary>
        public class ShowTooltip : AbstractGeneratorParameter<bool?>
        {
            /// <summary> Returns the default value. <see langword="false"/></summary>
            public override bool? Default => false;
        }

        private ShowTooltip showTooltip = new ShowTooltip();

        /// <summary>
        /// Determines if the molecule's title is depicted.
        /// </summary>
        public class ShowMoleculeTitle : AbstractGeneratorParameter<bool?>
        {
            /// <summary> Returns the default value.</summary>
            /// <returns><see langword="false"/></returns>
            public override bool? Default => false;
        }

        /// <summary>
        /// Determines if the reaction's title is depicted.
        /// </summary>
        public class ShowReactionTitle : AbstractGeneratorParameter<bool?>
        {
            /// <summary> Returns the default value.</summary>
            /// <returns><see langword="false"/></returns>
            public override bool? Default => false;
        }

        private ShowMoleculeTitle showMoleculeTitle = new ShowMoleculeTitle();

        /// <summary>
        /// If true, the scale is set such that the diagram
        /// fills the whole screen. </summary>
        public class FitToScreen : AbstractGeneratorParameter<bool?>
        {
            /// <summary> Returns the default value.</summary>
            /// <returns><see langword="false"/></returns>
            public override bool? Default => false;
        }

        private FitToScreen fitToScreen = new FitToScreen();

        /// <summary>
        /// The scale is the factor to multiply model coordinates by to convert the
        /// coordinates to screen space coordinate, such that the entire structure
        /// fits the visible screen dimension.
        /// </summary>
        public class Scale : AbstractGeneratorParameter<double?>
        {
            /// <summary> Returns the default value.</summary>
            /// <returns>1.0</returns>
            public override double? Default => 1;
        }

        private IGeneratorParameter<double?> scale = new Scale();

        /// <summary>
        /// The background color of the drawn image.
        /// </summary>
        public class BackgroundColor : AbstractGeneratorParameter<Color?>
        {
            /// <summary> Returns the default value. <see cref="WPF.Media.Colors.White"/></summary>
            public override Color? Default => WPF.Media.Colors.White;
        }

        /// <summary>
        /// The length on the screen of a typical bond.
        /// </summary>
        public class BondLength : AbstractGeneratorParameter<double?>
        {
            /// <summary>Returns the default value.</summary>
            /// <returns>40.0</returns>
            public override double? Default => 40;
        }

        private IGeneratorParameter<double?> bondLength = new BondLength();
        private IGeneratorParameter<Color?> backgroundColor = new BackgroundColor();

        /// <summary>
        /// The foreground color, with which objects are drawn.
        /// </summary>
        public class ForegroundColor : AbstractGeneratorParameter<Color?>
        {
            /// <summary>Returns the default value.</summary>
            /// <returns><see cref="System.Windows.Media.Colors.Black"/></returns>
            public override Color? Default => WPF.Media.Colors.Black;
        }

        private IGeneratorParameter<Color?> foregroundColor = new ForegroundColor();

        /// <summary>
        /// If set to true, uses anti-aliasing for drawing. Anti-aliasing makes
        /// drawing slower, but at lower resolutions it makes drawings look more
        /// smooth.
        /// </summary>
        public class UseAntiAliasing : AbstractGeneratorParameter<bool?>
        {
            /// <summary> Returns the default value.</summary>
            /// <returns><see langword="true"/></returns>
            public override bool? Default => true;
        }

        private IGeneratorParameter<bool?> useAntiAliasing = new UseAntiAliasing();

        /// <summary>
        /// Area on each of the four margins to keep empty.
        /// </summary>
        public class Margin : AbstractGeneratorParameter<double?>
        {
            /// <summary> Returns the default value.</summary>
            /// <returns>10.0</returns>
            public override double? Default => 10;
        }

        private IGeneratorParameter<double?> margin = new Margin();

        /// <summary>The font style to use for text.</summary>
        public class UsedFontStyle : AbstractGeneratorParameter<FontStyles?>
        {
            /// <summary>Returns the default value.</summary>
            /// <returns><see cref="FontStyles.Normal"/></returns>
            public override FontStyles? Default => FontStyles.Normal;
        }

        private IGeneratorParameter<FontStyles?> fontStyle = new UsedFontStyle();

        /// <summary>
        /// Font to use for text.
        /// </summary>
        public class FontName : AbstractGeneratorParameter<string>
        {
            /// <summary> Returns the default value. "Arial"</summary>
            public override string Default => "Arial";
        }

        private IGeneratorParameter<string> fontName = new FontName();

        /// <summary>
        /// The zoom factor which is a user oriented parameter allowing the
        /// user to zoom in on parts of the molecule. When the zoom is 1.0,
        /// then the molecule is depicted in its normal coordinates.
        /// </summary>
        /// <seealso cref="Scale"/>
        public class ZoomFactor : AbstractGeneratorParameter<double?>
        {
            /// <summary> Returns the default value.</summary>
            /// <returns>1.0</returns>
            public override double? Default => 1;
        }

        /// <summary>A zoom of 100% is defined to be a value of 1.0</summary>
        private IGeneratorParameter<double?> zoomFactor = new ZoomFactor();

        /// <summary>
        /// An empty constructor necessary for reflection.
        /// </summary>
        public BasicSceneGenerator() { }

        /// <inheritdoc/>
        public IRenderingElement Generate(IAtomContainer ac, RendererModel model)
        {
            return new ElementGroup();
        }

        public IList<IGeneratorParameter> Parameters =>
            new IGeneratorParameter[] { backgroundColor, foregroundColor, margin, useAntiAliasing, fontStyle,
                fontName, zoomFactor, scale, bondLength, fitToScreen, showMoleculeTitle, showTooltip,
                arrowHeadWidth, new ShowReactionTitle() };
    }
}
