/* Copyright (C) 2009  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
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
 * 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
using NCDK.Config;
using NCDK.QSAR.Results;
using System;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Prediction of logP based on the number of carbon and hetero atoms. The
    /// implemented equation was proposed in <token>cdk-cite-Mannhold2009</token>.
    /// </summary>
    // @cdk.module     qsarmolecular
    // @cdk.githash
    // @cdk.dictref    qsar-descriptors:mannholdLogP
    // @cdk.keyword LogP
    // @cdk.keyword descriptor
    public class MannholdLogPDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private static readonly string[] NAMES = { "MLogP" };

        /// <summary>
        /// The specification attribute of the MannholdLogPDescriptor object.
        /// </summary>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/" + "chemoinformatics-algorithms/#mannholdLogP",
                typeof(MannholdLogPDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <summary>
        /// This <see cref="IDescriptor"/> does not have any parameters. If it had, this
        /// would have been the method to set them.
        /// </summary>
        /// <exception cref="CDKException">Exception throw when invalid parameter values are passed</exception>
        public override object[] Parameters
        {
            set
            {
                if (value != null && value.Length > 0)
                {
                    throw new CDKException("MannholdLogPDescriptor has no parameters.");
                }
            }
            get
            {
                return Array.Empty<object>();
            }
        }

        public override string[] DescriptorNames => NAMES;

        private DescriptorValue GetDummyDescriptorValue(Exception e)
        {
            return new DescriptorValue(_Specification, ParameterNames, Parameters, new Result<double>(double.NaN), DescriptorNames, e);
        }

        /// <summary>
        ///  Calculates the Mannhold LogP for an atom container.
        /// </summary>
        /// <param name="atomContainer"><see cref="IAtomContainer"/> to calculate the descriptor value for.</param>
        /// <returns>A descriptor value wrapping a <see cref="Result<double>"/>.</returns>
        public override DescriptorValue Calculate(IAtomContainer atomContainer)
        {
            IAtomContainer ac = (IAtomContainer)atomContainer.Clone();

            int carbonCount = 0;
            int heteroCount = 0;
            foreach (var atom in ac.Atoms)
            {
                if (!Elements.Hydrogen.ToIElement().Symbol.Equals(atom.Symbol))
                {
                    if (Elements.Carbon.ToIElement().Symbol.Equals(atom.Symbol))
                    {
                        carbonCount++;
                    }
                    else
                    {
                        heteroCount++;
                    }
                }
            }
            double mLogP = 1.46 + 0.11 * carbonCount - 0.11 * heteroCount;

            return new DescriptorValue(_Specification, ParameterNames, Parameters, new Result<double>(mLogP), DescriptorNames);
        }

        /// <summary>
        /// A type of return value calculated by this descriptor.
        /// </summary>
        public override IDescriptorResult DescriptorResultType { get; } = new Result<double>();

        /// <summary>
        /// The parameterNames attribute for this descriptor.
        /// A zero-length string array.
        /// </summary>
        public override string[] ParameterNames => Array.Empty<string>();

        /// <summary>
        /// Gets the parameterType attribute for a given parameter name. It
        /// always returns null, as this descriptor does not have any parameters.
        /// </summary>
        /// <param name="name">Name of the parameter for which the type is requested.</param>
        /// <returns>The parameterType of the given parameter.</returns>
        public override object GetParameterType(string name) => null;
    }
}
