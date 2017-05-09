/* Copyright (C) 2008-2009  Gilleain Torrance <gilleain@users.sf.net>
 *               2008-2009  Arvid Berg <goglepox@users.sf.net>
 *                    2009  Stefan Kuhn <shk3@users.sf.net>
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
using NCDK.Renderers;
using NCDK.Renderers.Colors;
using NCDK.Renderers.Fonts;
using NCDK.Renderers.Generators;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace NCDK.MolViewer.Renderers
{
    /// <summary>
    /// Model for <see cref="Renderer"/> that contains settings for drawing objects.
    /// </summary>
    // @cdk.module render
    // @cdk.svnrev $Revision$ 
    [Serializable]
    public class JChemPaintRendererModel : RendererModel
    {
        private RenderingParameters parameters;

        private IDictionary<int, bool> flags = new Dictionary<int, bool>();
        /// <summary>
        /// The color hash is used to color substructures.
        /// </summary>
        /// <seealso cref="GetColorHash()"/>
        private IDictionary<IChemObject, Color> colorHash =
            new Dictionary<IChemObject, Color>();

        /// <summary>
        /// Constructor for the RendererModel.
        /// </summary>
        public JChemPaintRendererModel()
        {
            this.parameters = new RenderingParameters();
        }

        /// <summary>
        /// Constructor for the RendererModel.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="useUserSettings">Should user setting (in $HOME/.jchempaint/properties) be used or not?</param>
        public JChemPaintRendererModel(RenderingParameters parameters, bool useUserSettings)
        {
            this.parameters = parameters;
        }

        public int GetArrowHeadWidth()
        {
            if (HasParameter(typeof(BasicSceneGenerator.ArrowHeadWidth)))
                return GetV<int>(typeof(BasicSceneGenerator.ArrowHeadWidth));
            return (int)new BasicSceneGenerator.ArrowHeadWidth().Default.Value;
        }

        public void SetArrowHeadWidth(int arrowHeadWidth)
        {
            if (HasParameter(typeof(BasicSceneGenerator.ArrowHeadWidth)))
                SetV(typeof(BasicSceneGenerator.ArrowHeadWidth), (double)arrowHeadWidth);
        }

        public bool GetHighlightShapeFilled()
        {
            return this.parameters.IsHighlightShapeFilled();
        }

        public void SetHighlightShapeFilled(bool highlightShapeFilled)
        {
            this.parameters.SetHighlightShapeFilled(highlightShapeFilled);
        }

        public double GetWedgeWidth()
        {
            if (HasParameter(typeof(BasicBondGenerator.WedgeWidth)))
                return GetV<double>(typeof(BasicBondGenerator.WedgeWidth));
            return new BasicBondGenerator.WedgeWidth().Default.Value;
        }

        public void SetWedgeWidth(double wedgeWidth)
        {
            if (HasParameter(typeof(BasicBondGenerator.WedgeWidth)))
                SetV(typeof(BasicBondGenerator.WedgeWidth), wedgeWidth);
        }

        public double GetRingProportion()
        {
            if (HasParameter(typeof(BasicBondGenerator.TowardsRingCenterProportion)))
                return Get<double>(typeof(BasicBondGenerator.TowardsRingCenterProportion));
            return new BasicBondGenerator.TowardsRingCenterProportion().Default.Value;
        }

        public void SetRingProportion(double ringProportion)
        {
            if (HasParameter(typeof(BasicBondGenerator.TowardsRingCenterProportion)))
                SetV(typeof(BasicBondGenerator.TowardsRingCenterProportion), ringProportion);
        }

        public BasicAtomGenerator.Shapes GetCompactShape()
        {
            if (HasParameter(typeof(BasicAtomGenerator.CompactShape)))
                return GetV<BasicAtomGenerator.Shapes>(typeof(BasicAtomGenerator.CompactShape));
            return new BasicAtomGenerator.CompactShape().Default.Value;
        }

        public void SetCompactShape(BasicAtomGenerator.Shapes compactShape)
        {
            if (HasParameter(typeof(BasicAtomGenerator.CompactShape)))
                SetV(typeof(BasicAtomGenerator.CompactShape), compactShape);
        }

        public double GetScale()
        {
            if (HasParameter(typeof(BasicSceneGenerator.Scale)))
                return GetV<double>(typeof(BasicSceneGenerator.Scale));
            return new BasicSceneGenerator.Scale().Default.Value;
        }

        public void SetScale(double scale)
        {
            if (HasParameter(typeof(BasicSceneGenerator.Scale)))
                base.SetV(typeof(BasicSceneGenerator.Scale), scale);
        }

        public RenderingParameters.AtomShape GetSelectionShape()
        {
            return this.parameters.GetSelectionShape();
        }

        public void SetSelectionShape(RenderingParameters.AtomShape selectionShape)
        {
            this.parameters.SetSelectionShape(selectionShape);
        }

        /// <summary>
        /// Get the name of the font family (Arial, etc).
        /// </summary>
        /// <returns>the name of the font family as a String.</returns>
        public string GetFontName()
        {
            if (HasParameter(typeof(BasicSceneGenerator.FontName)))
                return Get<string>(typeof(BasicSceneGenerator.FontName));
            return new BasicSceneGenerator.FontName().Default;
        }

        /// <summary>
        /// Set the name of the font family (Arial, etc).
        /// </summary>
        public void SetFontName(string fontName)
        {
            if (HasParameter(typeof(BasicSceneGenerator.FontName)))
                Set(typeof(BasicSceneGenerator.FontName), fontName);
        }

        /// <summary>
        /// Get the style of the font (Normal, Bold).
        /// </summary>
        /// <returns>the style of the font as a member of the FontStyles enum</returns>
        public FontStyles GetFontStyle()
        {
            if (HasParameter(typeof(BasicSceneGenerator.UsedFontStyle)))
                return GetV<FontStyles>(typeof(BasicSceneGenerator.UsedFontStyle));
            return new BasicSceneGenerator.UsedFontStyle().Default.Value;
        }

        /// <summary>
        /// Set the style of font to use (Normal, Bold).
        /// </summary>
        /// <param name="fontStyle">a member of the enum in <see cref="IFontManager"/></param>
        public void SetFontManager(FontStyles fontStyle)
        {
            if (HasParameter(typeof(BasicSceneGenerator.UsedFontStyle)))
                SetV(typeof(BasicSceneGenerator.UsedFontStyle), fontStyle);
        }

        public bool GetIsCompact()
        {
            if (HasParameter(typeof(BasicAtomGenerator.CompactAtom)))
                return GetV<bool>(typeof(BasicAtomGenerator.CompactAtom));
            return new BasicAtomGenerator.CompactAtom().Default.Value;
        }

        public void SetIsCompact(bool compact)
        {
            if (HasParameter(typeof(BasicAtomGenerator.CompactAtom)))
                SetV(typeof(BasicAtomGenerator.CompactAtom), compact);
        }

        public bool GetUseAntiAliasing()
        {
            if (HasParameter(typeof(BasicSceneGenerator.UseAntiAliasing)))
                return GetV<bool>(typeof(BasicSceneGenerator.UseAntiAliasing));
            return new BasicSceneGenerator.UseAntiAliasing().Default.Value;
        }

        public void SetUseAntiAliasing(bool b)
        {
            if (HasParameter(typeof(BasicSceneGenerator.UseAntiAliasing)))
                SetV(typeof(BasicSceneGenerator.UseAntiAliasing), b);
        }

        public bool GetShowReactionBoxes()
        {
            return this.parameters.IsShowReactionBoxes();
        }

        public void SetShowReactionBoxes(bool b)
        {
            this.parameters.SetShowReactionBoxes(b);
            FireChange();
        }

        public bool GetShowMoleculeTitle()
        {
            if (HasParameter(typeof(BasicSceneGenerator.ShowMoleculeTitle)))
                return GetV<bool>(typeof(BasicSceneGenerator.ShowMoleculeTitle));
            return new BasicSceneGenerator.ShowMoleculeTitle().Default.Value;
        }

        public void SetShowMoleculeTitle(bool b)
        {
            if (HasParameter(typeof(BasicSceneGenerator.ShowMoleculeTitle)))
                SetV(typeof(BasicSceneGenerator.ShowMoleculeTitle), b);
        }

        /// <summary>
        /// The length on the screen of a typical bond.
        /// </summary>
        /// <returns>the user-selected length of a bond, or the default length.</returns>
        public double GetBondLength()
        {
            if (HasParameter(typeof(BasicSceneGenerator.BondLength)))
                return GetV<double>(typeof(BasicSceneGenerator.BondLength));
            return new BasicSceneGenerator.BondLength().Default.Value;
        }

        /// <summary>
        /// Set the length on the screen of a typical bond.
        /// </summary>
        /// <param name="length">the length in pixels of a typical bond.</param>
        public void SetBondLength(double Length)
        {
            if (HasParameter(typeof(BasicSceneGenerator.BondLength)))
                SetV(typeof(BasicSceneGenerator.BondLength), Length);
        }

        /// <summary>
        /// Returns the distance between two lines in a double or triple bond
        /// </summary>
        /// <returns>the distance between two lines in a double or triple bond</returns>
        public double GetBondDistance()
        {
            if (HasParameter(typeof(BasicBondGenerator.BondDistance)))
                return GetV<double>(typeof(BasicBondGenerator.BondDistance));
            return new BasicBondGenerator.BondDistance().Default.Value;
        }

        /// <summary>
        /// Sets the distance between two lines in a double or triple bond
        /// </summary>
        /// <param name="bondDistance">the distance between two lines in a double or triple bond</param>
        public void SetBondDistance(double bondDistance)
        {
            if (HasParameter(typeof(BasicBondGenerator.BondDistance)))
                SetV(typeof(BasicBondGenerator.BondDistance), bondDistance);
        }

        /// <summary>
        /// Returns the thickness of a bond line.
        /// </summary>
        /// <returns>the thickness of a bond line</returns>
        public double GetBondWidth()
        {
            if (HasParameter(typeof(BasicBondGenerator.BondWidth)))
                return GetV<double>(typeof(BasicBondGenerator.BondWidth));
            return new BasicBondGenerator.BondWidth().Default.Value;
        }

        /// <summary>
        /// Sets the thickness of a bond line.
        /// </summary>
        /// <param name="bondWidth">the thickness of a bond line</param>
        public void SetBondWidth(double bondWidth)
        {
            if (HasParameter(typeof(BasicBondGenerator.BondWidth)))
                SetV(typeof(BasicBondGenerator.BondWidth), bondWidth);
        }

        /// <summary>
        /// Returns the thickness of an atom atom mapping line.
        /// </summary>
        /// <returns>the thickness of an atom atom mapping line</returns>
        public double GetMappingLineWidth()
        {
            return this.parameters.GetMappingLineWidth();
        }

        /// <summary>
        /// Sets the thickness of an atom atom mapping line.
        /// </summary>
        /// <param name="mappingLineWidth">the thickness of an atom atom mapping line</param>
        public void SetMappingLineWidth(double mappingLineWidth)
        {
            this.parameters.SetMappingLineWidth(mappingLineWidth);
            FireChange();
        }

        /// <summary>
        /// A zoom factor for the drawing.
        /// </summary>
        /// <returns>a zoom factor for the drawing</returns>
        public double GetZoomFactor()
        {
            if (HasParameter(typeof(BasicSceneGenerator.ZoomFactor)))
                return GetV<double>(typeof(BasicSceneGenerator.ZoomFactor));
            return new BasicSceneGenerator.ZoomFactor().Default.Value;
        }

        /// <summary>
        /// Returns the zoom factor for the drawing.
        /// </summary>
        /// <param name="zoomFactor">the zoom factor for the drawing</param>
        public void SetZoomFactor(double zoomFactor)
        {
            if (HasParameter(typeof(BasicSceneGenerator.ZoomFactor)))
                SetV(typeof(BasicSceneGenerator.ZoomFactor), zoomFactor);
        }

        public bool IsFitToScreen()
        {
            if (HasParameter(typeof(BasicSceneGenerator.FitToScreen)))
                return GetV<bool>(typeof(BasicSceneGenerator.FitToScreen));
            return new BasicSceneGenerator.FitToScreen().Default.Value;
        }

        public void SetFitToScreen(bool value)
        {
            if (HasParameter(typeof(BasicSceneGenerator.FitToScreen)))
                SetV(typeof(BasicSceneGenerator.FitToScreen), value);
        }

        /// <summary>
        /// Returns the foreground color for the drawing.
        /// </summary>
        /// <returns>the foreground color for the drawing</returns>
        public Color GetForeColor()
        {
            if (HasParameter(typeof(BasicSceneGenerator.ForegroundColor)))
                return GetV<Color>(typeof(BasicSceneGenerator.ForegroundColor));
            return new BasicSceneGenerator.ForegroundColor().Default.Value;
        }

        /// <summary>
        /// Sets the foreground color with which bonds and atoms are drawn
        /// </summary>
        /// <param name="foreColor">the foreground color with which bonds and atoms are drawn</param>
        public void SetForeColor(Color foreColor)
        {
            if (HasParameter(typeof(BasicSceneGenerator.ForegroundColor)))
                SetV(typeof(BasicSceneGenerator.ForegroundColor), foreColor);
        }

        /// <summary>
        /// Returns the background color
        /// </summary>
        /// <returns>the background color</returns>
        public Color GetBackColor()
        {
            if (HasParameter(typeof(BasicSceneGenerator.BackgroundColor)))
                return GetV<Color>(typeof(BasicSceneGenerator.BackgroundColor));
            return new BasicSceneGenerator.BackgroundColor().Default.Value;
        }

        /// <summary>
        /// Sets the background color
        /// </summary>
        /// <param name="backColor">the background color</param>
        public void SetBackColor(Color backColor)
        {
            if (HasParameter(typeof(BasicSceneGenerator.BackgroundColor)))
                SetV(typeof(BasicSceneGenerator.BackgroundColor), backColor);
        }

        /// <summary>
        /// Returns the atom-atom mapping line color
        /// </summary>
        /// <returns>the atom-atom mapping line color</returns>
        public Color GetAtomAtomMappingLineColor()
        {
            return this.parameters.GetMappingColor();
        }

        /// <summary>
        /// Sets the atom-atom mapping line color
        /// </summary>
        /// <param name="mappingColor">the atom-atom mapping line color</param>
        public void SetAtomAtomMappingLineColor(Color mappingColor)
        {
            this.parameters.SetMappingColor(mappingColor);
            FireChange();
        }

        /// <summary>
        /// Returns if the drawing of atom numbers is switched on for this model
        /// </summary>
        /// <returns>true if the drawing of atom numbers is switched on for this model</returns>
        public bool DrawNumbers()
        {
            return this.parameters.IsWillDrawNumbers();
        }

        public bool GetKekuleStructure()
        {
            if (HasParameter(typeof(BasicAtomGenerator.KekuleStructure)))
                return GetV<bool>(typeof(BasicAtomGenerator.KekuleStructure));
            return new BasicAtomGenerator.KekuleStructure().Default.Value;
        }

        public void SetKekuleStructure(bool kekule)
        {
            if (HasParameter(typeof(BasicAtomGenerator.KekuleStructure)))
                SetV(typeof(BasicAtomGenerator.KekuleStructure), kekule);
        }

        public bool GetColorAtomsByType()
        {
            if (HasParameter(typeof(BasicAtomGenerator.ColorByType)))
                return GetV<bool>(typeof(BasicAtomGenerator.ColorByType));
            return new BasicAtomGenerator.ColorByType().Default.Value;
        }

        public void SetColorAtomsByType(bool b)
        {
            if (HasParameter(typeof(BasicAtomGenerator.ColorByType)))
                SetV(typeof(BasicAtomGenerator.ColorByType), b);
        }

        public bool GetShowEndCarbons()
        {
            return this.parameters.IsShowEndCarbons();
        }

        public void SetShowEndCarbons(bool showThem)
        {
            this.parameters.SetShowEndCarbons(showThem);
            FireChange();
        }

        public bool GetShowImplicitHydrogens()
        {
            return this.parameters.IsShowImplicitHydrogens();
        }

        public void SetShowImplicitHydrogens(bool showThem)
        {
            this.parameters.SetShowImplicitHydrogens(showThem);
            FireChange();
        }

        public bool GetShowExplicitHydrogens()
        {
            return this.parameters.IsShowExplicitHydrogens();
        }

        public void SetShowExplicitHydrogens(bool showThem)
        {
            this.parameters.SetShowExplicitHydrogens(showThem);
            FireChange();
        }

        public bool GetShowAromaticity()
        {
            if (HasParameter(typeof(RingGenerator.ShowAromaticity)))
                return GetV<bool>(typeof(RingGenerator.ShowAromaticity));
            return new RingGenerator.ShowAromaticity().Default.Value;
        }

        public void SetShowAromaticity(bool showIt)
        {
            if (HasParameter(typeof(RingGenerator.ShowAromaticity)))
                SetV(typeof(RingGenerator.ShowAromaticity), showIt);
        }

        /// <summary>
        /// Sets if the drawing of atom numbers is switched on for this model.
        /// </summary>
        /// <param name="drawNumbers">true if the drawing of atom numbers is to be switched on for            this model</param>
        public void SetDrawNumbers(bool drawNumbers)
        {
            this.parameters.SetWillDrawNumbers(drawNumbers);
            FireChange();
        }

        /// <summary>
        /// Returns true if atom numbers are drawn.
        /// </summary>
        public bool GetDrawNumbers()
        {
            return this.parameters.IsWillDrawNumbers();
        }

        public Color GetDefaultBondColor()
        {
            if (HasParameter(typeof(BasicBondGenerator.DefaultBondColor)))
                return GetV<Color>(typeof(BasicBondGenerator.DefaultBondColor));
            return new BasicBondGenerator.DefaultBondColor().Default.Value;
        }

        public void SetDefaultBondColor(Color defaultBondColor)
        {
            if (HasParameter(typeof(BasicBondGenerator.DefaultBondColor)))
                SetV(typeof(BasicBondGenerator.DefaultBondColor), defaultBondColor);
        }

        /// <summary>
        /// Returns the radius around an atoms, for which the atom is marked
        /// highlighted if a pointer device is placed within this radius.
        /// </summary>
        /// <returns>The highlight distance for all atoms (in screen space)</returns>
        public double GetHighlightDistance()
        {
            return this.parameters.GetHighlightDistance();
        }

        /// <summary>
        /// Sets the radius around an atoms, for which the atom is marked highlighted
        /// if a pointer device is placed within this radius.
        /// </summary>
        /// <param name="highlightDistance">the highlight radius of all atoms (in screen space)</param>
        public void SetHighlightDistance(double highlightDistance)
        {
            this.parameters.SetHighlightDistance(highlightDistance);
            FireChange();
        }

        /// <summary>
        /// Returns whether Atom-Atom mapping must be shown.
        /// </summary>
        public bool GetShowAtomAtomMapping()
        {
            return this.parameters.IsShowAtomAtomMapping();
        }

        /// <summary>
        /// Sets whether Atom-Atom mapping must be shown.
        /// </summary>
        public void SetShowAtomAtomMapping(bool value)
        {
            this.parameters.SetShowAtomAtomMapping(value);
            FireChange();
        }

        /// <summary>
        /// This is used for the size of the compact atom element.
        /// </summary>
        public double GetAtomRadius()
        {
            if (HasParameter(typeof(BasicAtomGenerator.AtomRadius)))
                return GetV<double>(typeof(BasicAtomGenerator.AtomRadius));
            return new BasicAtomGenerator.AtomRadius().Default.Value;
        }

        /// <summary>
        /// Set the radius of the compact atom representation.
        /// </summary>
        /// <param name="atomRadius">the size of the compact atom symbol.</param>
        public void SetAtomRadius(double atomRadius)
        {
            if (HasParameter(typeof(BasicAtomGenerator.AtomRadius)))
                SetV(typeof(BasicAtomGenerator.AtomRadius), atomRadius);
        }

        /// <summary>
        /// Returns the <see cref="IDictionary"/> used for coloring substructures.
        /// </summary>
        /// <returns>the <see cref="IDictionary"/> used for coloring substructures</returns>
        public IDictionary<IChemObject, Color> GetColorHash()
        {
            return this.colorHash;
        }

        /// <summary>
        /// Returns the background color of the given atom.
        /// </summary>
        public Color GetAtomBackgroundColor(IAtom atom)
        {
            // logger.debug("Getting atom back color for " + atom.toString());
            Color atomColor = GetBackColor();
            // logger.debug("  BackColor: " + atomColor.toString());
            Color hashColor = (Color)this.GetColorHash()[atom];
            if (hashColor != null)
            {
                // logger.debug(
                // "Background color atom according to hashing (substructure)");
                atomColor = hashColor;
            }
            // logger.debug("Color: " + atomColor.toString());
            return atomColor;
        }

        /// <summary>
        /// Returns the current atom colorer.
        /// </summary>
        /// <returns>The AtomColorer.</returns>
        public IAtomColorer GetAtomColorer()
        {
            if (HasParameter(typeof(BasicAtomGenerator.AtomColorer)))
                return Get<IAtomColorer>(typeof(BasicAtomGenerator.AtomColorer));
            return new BasicAtomGenerator.AtomColorer().Default;
        }

        /// <summary>
        /// Sets the atom colorer.
        /// </summary>
        /// <param name="atomColorer">the new colorer.</param>
        public void SetAtomColorer(IAtomColorer atomColorer)
        {
            if (HasParameter(typeof(BasicAtomGenerator.AtomColorer)))
                Set(typeof(BasicAtomGenerator.AtomColorer), atomColorer);
        }

        /// <summary>
        /// Sets the <see cref="IDictionary"/> used for coloring substructures
        /// </summary>
        /// <param name="colorHash">the <see cref="IDictionary"/> used for coloring substructures</param>
        public void SetColorHash(IDictionary<IChemObject, Color> colorHash)
        {
            this.colorHash = colorHash;
            FireChange();
        }

        /// <summary>
        /// Sets the showTooltip attribute.
        /// </summary>
        /// <param name="showTooltip">The new value.</param>
        public void SetShowTooltip(bool showTooltip)
        {
            if (HasParameter(typeof(BasicSceneGenerator.ShowTooltip)))
                Set(typeof(BasicSceneGenerator.ShowTooltip), showTooltip);
        }

        /// <summary>
        /// Gets showTooltip attribute.
        /// </summary>
        /// <returns>The showTooltip value.</returns>
        public bool GetShowTooltip()
        {
            if (HasParameter(typeof(BasicSceneGenerator.ShowTooltip)))
                return GetV<bool>(typeof(BasicSceneGenerator.ShowTooltip));
            return new BasicSceneGenerator.ShowTooltip().Default.Value;
        }

        /// <summary>
        /// Gets the color used for drawing the part which was selected externally
        /// </summary>
        public Color GetExternalHighlightColor()
        {
            return this.parameters.GetExternalHighlightColor();
        }

        /// <summary>
        /// Sets the color used for drawing the part which was selected externally
        /// </summary>
        /// <param name="externalHighlightColor">The color</param>
        public void SetExternalHighlightColor(Color externalHighlightColor)
        {
            this.parameters.SetExternalHighlightColor(externalHighlightColor);
        }

        /// <summary>
        /// Gets the color used for drawing the part we are hovering over.
        /// </summary>
        public Color GetHoverOverColor()
        {
            return this.parameters.GetHoverOverColor();
        }

        /// <summary>
        /// Sets the color used for drawing the part we are hovering over.
        /// </summary>
        /// <param name="hoverOverColor">The color</param>
        public void SetHoverOverColor(Color hoverOverColor)
        {
            this.parameters.SetHoverOverColor(hoverOverColor);
        }

        /// <summary>
        /// Gets the color used for drawing the internally selected part.
        /// </summary>
        public Color GetSelectedPartColor()
        {
            return this.parameters.GetSelectedPartColor();
        }

        /// <summary>
        /// Sets the color used for drawing the internally selected part.
        /// </summary>
        /// <param name="selectedPartColor">The color</param>
        public void SetSelectedPartColor(Color selectedPartColor)
        {
            this.parameters.SetSelectedPartColor(selectedPartColor);
        }

        public bool ShowAtomTypeNames()
        {
            return this.parameters.IsShowAtomTypeNames();
        }

        public void SetShowAtomTypeNames(bool showAtomTypeNames)
        {
            this.parameters.SetShowAtomTypeNames(showAtomTypeNames);
        }

        public double GetMargin()
        {
            if (HasParameter(typeof(BasicSceneGenerator.Margin)))
                return GetV<double>(typeof(BasicSceneGenerator.Margin));
            return new BasicSceneGenerator.Margin().Default.Value;
        }

        public void SetMargin(double margin)
        {
            if (HasParameter(typeof(BasicSceneGenerator.Margin)))
                Set(typeof(BasicSceneGenerator.Margin), margin);
        }

        public Color GetBoundsColor()
        {
            return this.parameters.GetBoundsColor();
        }

        public void SetBoundsColor(Color color)
        {
            this.parameters.SetBoundsColor(color);
        }

        /// <returns>the on screen radius of the selection element</returns>
        public double GetSelectionRadius()
        {
            return this.parameters.GetSelectionRadius();
        }

        public void SetSelectionRadius(double selectionRadius)
        {
            this.parameters.SetSelectionRadius(selectionRadius);
        }

        /// <returns>The color used for underlining not typeable atoms.</returns>
        public Color GetNotTypeableUnderlineColor()
        {
            return this.parameters.GetNotTypeableUnderlineColor();
        }

        /// <param name="identifier"></param>
        /// <returns></returns>
        public bool GetFlag(int identifier)
        {
            return flags[identifier];
        }

        /// <returns></returns>
        public IDictionary<int, bool> GetFlags()
        {
            return flags;
        }

        /// <param name="identifier"></param>
        /// <param name="flag"></param>
        public void SetFlag(int identifier, bool flag)
        {
            flags.Remove(identifier);
            flags[identifier] = flag;
        }
    }
}
