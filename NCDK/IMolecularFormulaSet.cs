/* Copyright (C) 2007  Miguel Rojasch <miguelrojasch@users.sf.net>
 *
 *  Contact: cdk-devel@lists.sourceforge.net
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
 
 using System.Collections.Generic;

namespace NCDK
{
    /*
 *  <p>
 *

 *
 */
    /// <summary>
    /// Class defining a molecular formula object. It maintains a list of list <see cref="IMolecularFormula"/>.
    /// </summary>
    /// <remarks>
    /// Examples:
    /// <list type="bullet">
    /// <item>[C5H5]-</item>
    /// <item>C6H6</item>
    /// <item><sup>12</sup>C5<sup>13</sup>CH6</item>
    /// </list>
    /// </remarks>
    // @cdk.module  interfaces
    // @author      miguelrojasch
    // @cdk.created 2007-11-20
    // @cdk.keyword molecular formula
    // @cdk.githash
    public interface IMolecularFormulaSet
        : ICDKObject, IList<IMolecularFormula>
    {
        /// <summary>
        /// Adds all molecular formulas in the <see cref="IEnumerable{T}"/> of <see cref="IMolecularFormula"/> to this <see cref="IChemObject"/>.
        /// </summary>
        /// <param name="collection"></param>
        void AddRange(IEnumerable<IMolecularFormula> collection);
    }
}
