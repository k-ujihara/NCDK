/* Copyright (C) 2006-2007  Egon Willighagen <egonw@users.sf.net>
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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Tools
{
    /**
     * Class with constants for possible data features defined in the
     * a Data Feature Ontology. Actual integers are random
     * and should <b>not</b> be used directly.
     *
     * <p>To test whether a IChemFormat supports a certain feature, the
     * following code can be used:
     * <pre>
     * int features = new XYZFormat().SupportedDataFeatures;
     * bool has3DCoords = (features & HAS_3D_COORDINATES) == HAS_3D_COORDINATES;
     * </pre>
     *
     * <p>This list of constants matches the latest <a href="http://qsar.sourceforge.net/ontologies/data-features/index.xhtml"
     * >Blue Obelisk Data Features Ontology</a>.
     *
     * @author     Egon Willighagen <ewilligh@uni-koeln.de>
     * @cdk.module annotation
     * @cdk.githash
     **/
    public class DataFeatures
    {
        /// <summary>Indicated that no feature are defined.</summary>
        public const int None = 0;

        // The int allows for up to 750 different properties. Should
        // be enough for now.

        // COORDINATE SYSTEMS

        /// <summary>@cdk.dictref bodf:coordinates2D</summary>
        public const int HAS_2D_COORDINATES = 1 << 0;
        /// <summary>@cdk.dictref bodf:coordinates3D</summary>
        public const int HAS_3D_COORDINATES = 1 << 1;
        /// <summary>@cdk.dictref bodf:fractionalUnitCellCoordinatesCoordinates</summary>
        public const int HAS_FRACTIONAL_CRYSTAL_COORDINATES = 1 << 2;

        // ATOMIC FEATURES
        //                      HAS_ATOMS ??

        /// <summary>@cdk.dictref bodf:hasAtomElementSymbol</summary>
        public const int HAS_ATOM_ELEMENT_SYMBOL = 1 << 3;
        /// <summary>@cdk.dictref bodf:partialAtomicCharges</summary>
        public const int HAS_ATOM_PARTIAL_CHARGES = 1 << 4;
        /// <summary>@cdk.dictref bodf:formalAtomicCharges</summary>
        public const int HAS_ATOM_FORMAL_CHARGES = 1 << 5;
        /** FIXME: NOT YET IN BODF !!! **/
        public const int HAS_ATOM_HYBRIDIZATIONS = 1 << 6;
        /// <summary>@cdk.dictref bodf:massNumbers</summary>
        public const int HAS_ATOM_MASS_NUMBERS = 1 << 7;
        /// <summary>@cdk.dictref bodf:isotopeNumbers</summary>
        public const int HAS_ATOM_ISOTOPE_NUMBERS = 1 << 8;

        // GRAPH FEATURES

        /// <summary>@cdk.dictref bodf:graphRepresentation</summary>
        public const int HAS_GRAPH_REPRESENTATION = 1 << 9;
        /// <summary>@cdk.dictref bodf:dietzRepresentation</summary>
        public const int HAS_DIETZ_REPRESENTATION = 1 << 10;

        // MODEL FEATURES

        /** FIXME: NOT YET IN BODF !!! **/
        public const int HAS_UNITCELL_PARAMETERS = 1 << 11;
        /** FIXME: NOT YET IN BODF !!! **/
        public const int HAS_REACTIONS = 1 << 12;
    }
}
