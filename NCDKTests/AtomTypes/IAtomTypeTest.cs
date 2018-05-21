/* Copyright (C) 2013  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using NCDK.Config;

namespace NCDK.AtomTypes
{
    /// <summary>
    /// Interface for <see cref="IAtomTypeMatcher"/> unit tests. It provides various methods
    /// to allow such unit test classes to extend <see cref="AbstractAtomTypeTest"/> and
    /// take advantage of the functionality that abstract class provides.
    /// </summary>
    // @cdk.githash
    // @cdk.module test-core
    public interface IAtomTypeTest
    {
        /// <summary>
        /// Returns a name for the atom type scheme being tested.
        /// </summary>
        /// <returns>a string of the name.</returns>
        string AtomTypeListName { get; }

        /// <summary>
        /// Returns an <see cref="AtomTypeFactory"/> instance for the atom type scheme
        /// being tested. It is used to provide a list of atom types the scheme
        /// defines.
        /// </summary>
        /// <returns><see cref="AtomTypeFactory"/> instance</returns>
        AtomTypeFactory GetFactory();

        /// <summary>
        /// The <see cref="IAtomTypeMatcher"/> being tested.
        /// </summary>
        /// <param name="builder">the <see cref="IChemObjectBuilder"/> used to create atom types.</param>
        /// <returns><see cref="IAtomTypeMatcher"/> instance</returns>
        IAtomTypeMatcher GetAtomTypeMatcher(IChemObjectBuilder builder);
    }
}
