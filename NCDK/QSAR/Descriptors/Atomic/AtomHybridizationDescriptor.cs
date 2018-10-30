/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.AtomTypes;
using NCDK.QSAR.Results;
using System;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    ///  This class returns the hybridization of an atom.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This descriptor uses these parameters:
    /// <list type="table">
    /// <listheader><term>Name</term><term>Default</term><term>Description</term></listheader>
    /// <item><term></term><term></term><term>no parameters</term></item>
    /// </list>
    /// </para> 
    /// </remarks> 
    // @author         mfe4
    // @cdk.created    2004-11-13
    // @cdk.module     qsaratomic
    // @cdk.githash
    // @cdk.dictref    qsar-descriptors:atomHybridization
    public partial class AtomHybridizationDescriptor : IAtomicDescriptor
    {
        /// <summary>
        /// The specification attribute of the AtomHybridizationDescriptor object
        /// </summary>
        public IImplementationSpecification Specification => specification;
        private static readonly DescriptorSpecification specification =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#atomHybridization",
                typeof(AtomHybridizationDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <summary>
        /// This descriptor does have any parameter.
        /// </summary>
        public IReadOnlyList<object> Parameters { get { return null; } set { } }

        public IReadOnlyList<string> DescriptorNames { get; } = new string[] { "aHyb" };

        private DescriptorValue<Result<Hybridization>> GetDummyDescriptorValue(Exception e)
        {
            return new DescriptorValue<Result<Hybridization>>(specification, ParameterNames, Parameters, new Result<Hybridization>(Hybridization.Unset), DescriptorNames, e);
        }

        /// <summary>
        ///  This method calculates the hybridization of an atom.
        /// </summary>
        /// <param name="atom">The <see cref="IAtom"/> for which the <see cref="IDescriptorValue"/> is requested</param>
        /// <param name="container">Parameter is the atom container.</param>
        /// <returns>The hybridization</returns>
        public DescriptorValue<Result<Hybridization>> Calculate(IAtom atom, IAtomContainer container)
        {
            IAtomType matched = null;
            try
            {
                matched = CDK.AtomTypeMatcher.FindMatchingAtomType(container, atom);
            }
            catch (CDKException e)
            {
                return GetDummyDescriptorValue(e);
            }
            if (matched == null)
            {
                int atnum = container.Atoms.IndexOf(atom);
                return GetDummyDescriptorValue(new CDKException("The matched atom type was null (atom number " + atnum
                        + ") " + atom.Symbol));
            }
            Hybridization atomHybridization = matched.Hybridization;
            var result = new Result<Hybridization>(atomHybridization);
            return new DescriptorValue<Result<Hybridization>>(specification, ParameterNames, Parameters, result, DescriptorNames);
        }

        /// <summary>
        /// The parameterNames attribute of the VdWRadiusDescriptor object.
        /// </summary>
        public IReadOnlyList<string> ParameterNames { get; } = Array.Empty<string>();

        /// <summary>
        /// Gets the parameterType attribute of the VdWRadiusDescriptor object.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>An Object of class equal to that of the parameter being requested</returns>
        public object GetParameterType(string name) => null;
    }
}
