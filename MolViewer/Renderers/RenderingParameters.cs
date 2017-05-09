/* Copyright (C) 2009  Gilleain Torrance <gilleain@users.sf.net>
 *               2009  Arvid Berg <goglepox@users.sf.net>
 *               2009  Egon Willighagen <egonw@users.sf.net>
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

using System.Windows.Media;

namespace NCDK.MolViewer.Renderers
{
    // @cdk.module render 
    public class RenderingParameters
    {
        /// <summary>
        /// When atoms are selected or in compact mode, they will
        /// be covered by a shape determined by this enumeration
        /// </summary>
        public enum AtomShape { OVAL, SQUARE };

        /// <summary>
        /// The color used for underlining not typeable atoms.
        /// </summary>
        private Color notTypeableUnderlineColor = Colors.Red;

        /// <summary>
        /// The width on screen of an atom-atom mapping line
        /// </summary>
        private double mappingLineWidth = 1.0;

        /// <summary>
        /// The color of the box drawn at the bounds of a
        /// molecule, molecule set, or reaction
        /// </summary>
        private Color boundsColor = Colors.LightGray;

        private Color externalHighlightColor = Colors.Red;

        private Color hoverOverColor = Colors.Blue;

        /// <summary>
        /// The maximum distance on the screen the mouse pointer has to be to
        /// highlight an element.
        /// </summary>
        private double highlightDistance;

        private bool highlightShapeFilled = false;

        private Color mappingColor = Colors.Gray;

        private Color selectedPartColor = Color.FromRgb(00, 153, 204); //Color.lightGray;

        /// <summary>
        /// The shape to display over selected atoms
        /// </summary>
        private AtomShape selectionShape = AtomShape.SQUARE;

        /// <summary>
        /// The radius on screen of the selection shape
        /// </summary>
        private double selectionRadius = 3;

        private bool showAtomAtomMapping = true;

        private bool showAtomTypeNames = false;

        /// <summary>
        /// Determines whether methyl carbons' symbols should be drawn explicit for
        /// methyl carbons. Example C/\C instead of /\.
        /// </summary>
        private bool showEndCarbons;

        /// <summary>Determines whether explicit hydrogens should be drawn.</summary>
        private bool showExplicitHydrogens;

        /// <summary>Determines whether implicit hydrogens should be drawn.</summary>
        private bool showImplicitHydrogens;

        private bool showReactionBoxes = false;

        private bool willDrawNumbers;


        public bool IsHighlightShapeFilled()
        {
            return highlightShapeFilled;
        }

        public void SetHighlightShapeFilled(bool highlightShapeFilled)
        {
            this.highlightShapeFilled = highlightShapeFilled;
        }

        public double GetHighlightDistance()
        {
            return highlightDistance;
        }

        public void SetHighlightDistance(double highlightDistance)
        {
            this.highlightDistance = highlightDistance;
        }

        public AtomShape GetSelectionShape()
        {
            return this.selectionShape;
        }

        public void SetSelectionShape(AtomShape selectionShape)
        {
            this.selectionShape = selectionShape;
        }

        public double GetMappingLineWidth()
        {
            return mappingLineWidth;
        }

        public Color GetExternalHighlightColor()
        {
            return externalHighlightColor;
        }

        public Color GetHoverOverColor()
        {
            return hoverOverColor;
        }

        public Color GetMappingColor()
        {
            return mappingColor;
        }

        public Color GetSelectedPartColor()
        {
            return selectedPartColor;
        }

        public bool IsShowAtomAtomMapping()
        {
            return showAtomAtomMapping;
        }

        public bool IsShowAtomTypeNames()
        {
            return showAtomTypeNames;
        }

        public bool IsShowEndCarbons()
        {
            return showEndCarbons;
        }

        public bool IsShowExplicitHydrogens()
        {
            return showExplicitHydrogens;
        }

        public bool IsShowImplicitHydrogens()
        {
            return showImplicitHydrogens;
        }

        public bool IsShowReactionBoxes()
        {
            return showReactionBoxes;
        }

        public bool IsWillDrawNumbers()
        {
            return willDrawNumbers;
        }

        public void SetMappingLineWidth(double mappingLineWidth)
        {
            this.mappingLineWidth = mappingLineWidth;
        }

        public void SetExternalHighlightColor(Color externalHighlightColor)
        {
            this.externalHighlightColor = externalHighlightColor;
        }

        public void SetHoverOverColor(Color hoverOverColor)
        {
            this.hoverOverColor = hoverOverColor;
        }

        public void SetMappingColor(Color mappingColor)
        {
            this.mappingColor = mappingColor;
        }

        public void SetSelectedPartColor(Color selectedPartColor)
        {
            this.selectedPartColor = selectedPartColor;
        }

        public void SetShowAtomAtomMapping(bool showAtomAtomMapping)
        {
            this.showAtomAtomMapping = showAtomAtomMapping;
        }

        public void SetShowAtomTypeNames(bool showAtomTypeNames)
        {
            this.showAtomTypeNames = showAtomTypeNames;
        }

        public void SetShowEndCarbons(bool showEndCarbons)
        {
            this.showEndCarbons = showEndCarbons;
        }

        public void SetShowExplicitHydrogens(bool showExplicitHydrogens)
        {
            this.showExplicitHydrogens = showExplicitHydrogens;
        }

        public void SetShowImplicitHydrogens(bool showImplicitHydrogens)
        {
            this.showImplicitHydrogens = showImplicitHydrogens;
        }

        public void SetShowReactionBoxes(bool showReactionBoxes)
        {
            this.showReactionBoxes = showReactionBoxes;
        }

        public void SetWillDrawNumbers(bool willDrawNumbers)
        {
            this.willDrawNumbers = willDrawNumbers;
        }

        public Color GetBoundsColor()
        {
            return this.boundsColor;
        }

        public void SetBoundsColor(Color color)
        {
            this.boundsColor = color;
        }

        public double GetSelectionRadius()
        {
            return this.selectionRadius;
        }

        public void SetSelectionRadius(double selectionRadius)
        {
            this.selectionRadius = selectionRadius;
        }

        public Color GetNotTypeableUnderlineColor()
        {
            return notTypeableUnderlineColor;
        }

        public void SetNotTypeableUnderlineColor(Color notTypeableUnderlineColor)
        {
            this.notTypeableUnderlineColor = notTypeableUnderlineColor;
        }

    }

}
