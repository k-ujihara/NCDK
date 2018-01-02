/* Copyright (C) 2006-2007  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
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
using NCDK.QSAR.Results;

namespace NCDK.QSAR
{
    /// <summary>
    /// Classes that implement this interface are QSAR descriptor calculators
    /// for <see cref="IAtomContainer"/> objects.
    /// </summary>
    // @cdk.module qsar
    // @cdk.githash
    public interface IMolecularDescriptor : IDescriptor
    {
        /// <summary>
        /// Calculates the descriptor value for the given IAtomContainer.
        /// </summary>
        /// <param name="container">An <see cref="IAtomContainer"/> for which this descriptor should be calculated</param>
        /// <returns>An object of <see cref="DescriptorValue"/> that contain the calculated value as well as specification details</returns>
        IDescriptorValue Calculate(IAtomContainer container);

        /// <summary>
        /// Returns the specific type of the <see cref="IDescriptorResult"/> object.
        /// The return value from this method really indicates what type of result will
        /// be obtained from the <see cref="DescriptorValue"/> object. Note that the same result
        /// can be achieved by interrogating the <see cref="DescriptorValue"/> object; this method
        /// allows you to do the same thing, without actually calculating the descriptor.
        /// <para>
        /// Additionally, the length indicated by the result type must match the actual
        /// length of a descriptor calculated with the current parameters. Typically, the
        /// length of array result types vary with the values of the parameters. See
        /// <see cref="IDescriptor"/> for more details.
        /// </para>
        /// </summary>
        /// <value>
        /// an object that implements the <see cref="IDescriptorResult"/> interface indicating
        /// the actual type of values returned by the descriptor in the <see cref="DescriptorValue"/> object
        /// </value>
        IDescriptorResult DescriptorResultType { get; }
    }
}

