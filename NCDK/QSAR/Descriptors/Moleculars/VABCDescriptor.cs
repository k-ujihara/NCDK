/* Copyright (C) 2011  Egon Willighagen <egon.willighagen@gmail.com>
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
using NCDK.Geometries.Volume;
using NCDK.QSAR.Results;
using System;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Volume descriptor using the method implemented in the <see cref="VABCVolume"/> class.
    /// </summary>
    // @cdk.module qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:vabc
    // @cdk.keyword volume
    // @cdk.keyword descriptor
    public class VABCDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        /// <inheritdoc/>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#vabc",
                typeof(VABCDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <inheritdoc/>
        public override object[] Parameters
        {
            set
            {
                if (value.Length != 0)
                {
                    throw new CDKException("The VABCDescriptor expects zero parameters");
                }
            }
            get
            {
                return Array.Empty<object>();
            }
        }

        public override string[] DescriptorNames { get; } = new string[] { "VABC" };

        private DescriptorValue GetDummyDescriptorValue(Exception e)
        {
            return new DescriptorValue(_Specification, ParameterNames, Parameters, new Result<double>(double.NaN), DescriptorNames, e);
        }

        /// <summary>
        /// Calculates the descriptor value using the <see cref="VABCVolume"/> class.
        /// </summary>
        /// <param name="atomContainer">The <see cref="IAtomContainer"/> whose volume is to be calculated</param>
        /// <returns>A double containing the volume</returns>
        public override DescriptorValue Calculate(IAtomContainer atomContainer)
        {
            double volume;
            try
            {
                volume = VABCVolume.Calculate(atomContainer);
            }
            catch (CDKException exception)
            {
                return GetDummyDescriptorValue(exception);
            }

            return new DescriptorValue(_Specification, ParameterNames, Parameters, new Result<double>(volume), DescriptorNames);
        }

        /// <inheritdoc/>
        public override IDescriptorResult DescriptorResultType { get; } = new Result<double>();


        /// <inheritdoc/>
        public override string[] ParameterNames { get; } = new string[0];

        /// <inheritdoc/>
        public override object GetParameterType(string name) => null;
    }
}
