/* Copyright (C) 2009  Egon Willighagen <egonw@users.lists.sf>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All I ask is that proper credit is given for my work, which includes
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
using NCDK.Renderers.Generators;
using System.Collections.Generic;
using System.Windows;

namespace NCDK.MolViewer.Renderers
{
    /// <summary>
    /// Interface that all 2D renderers implement.
    /// </summary>
    // @cdk.module render 
    public interface IRenderer
    {
        /// <summary>
        /// Returns the drawing model, giving access to drawing parameters.
        /// </summary>
        /// <returns>the rendering model</returns>
        JChemPaintRendererModel GetRenderer2DModel();

        /// <summary>
        /// Converts screen coordinates into model (or world) coordinates.
        /// </summary>
        /// <param name="screenXTo">the screen's x coordinate</param>
        /// <param name="screenYTo">the screen's y coordinate</param>
        /// <returns>the matching model coordinates</returns>
        /// <seealso cref="ToScreenCoordinates(double, double)"/>
        Point ToModelCoordinates(double screenXTo, double screenYTo);

        /// <summary>
        /// Converts model (or world) coordinates into screen coordinates.
        /// </summary>
        /// <param name="screenXTo">the model's x coordinate</param>
        /// <param name="screenYTo">the model's y coordinate</param>
        /// <returns>the matching screen coordinates</returns>
        /// <seealso cref="ToModelCoordinates(double, double)"/>
        Point ToScreenCoordinates(double screenXTo, double screenYTo);

        /// <summary>
        /// Set a new zoom factor.
        /// </summary>
        /// <param name="zoomFactor">the new zoom factor</param>
        void SetZoom(double zoomFactor);

        /// <summary>
        /// Set a new drawing center in screen coordinates.
        /// </summary>
        /// <param name="zoomFactor">the new new drawing center</param>
        void ShiftDrawCenter(double screenX, double screenY);

        IList<IGenerator<IAtomContainer>> GetGenerators();
    }
}
