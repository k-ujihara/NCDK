/* Copyright (C) 2011-2015  Egon Willighagen <egonw@users.sf.net>
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License as published by the Free
 * Software Foundation; either version 2.1 of the License, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more
 * details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.QSAR.Result;

namespace NCDK.QSAR.Descriptors.Substances
{
    /// <summary>
    /// Classes that implement this interface are QSAR substance calculators.
    /// </summary>
    // @cdk.githash
    public interface ISubstanceDescriptor : IDescriptor
    {
        /// <summary>
        /// Calculates the descriptor value for the given <see cref="ISubstance"/>.
        /// </summary>
        /// <param name="substance">An <see cref="ISubstance"/> for which this descriptor
        ///                  should be calculated</param>
        /// <returns>An object of <see cref="DescriptorValue"/> that contain the
        ///         calculated value as well as specification details</returns>
        DescriptorValue Calculate(ISubstance substance);

        /// <summary>
        /// Returns the specific type of the DescriptorResult object.
        /// The return value from this method really indicates what type of result will
        /// be obtained from the <see cref="DescriptorValue"/> object. Note that the same result
        /// can be achieved by interrogating the <see cref="DescriptorValue"/> object; this method
        /// allows you to do the same thing, without actually calculating the descriptor.
        /// <para>Additionally, the length indicated by the result type must match the actual
        /// length of a descriptor calculated with the current parameters. Typically, the
        /// length of array result types vary with the values of the parameters. See
        /// <see cref="IDescriptor"/> for more details.
        /// </para>
        /// </summary>
        IDescriptorResult DescriptorResultType { get; }
    }
}
