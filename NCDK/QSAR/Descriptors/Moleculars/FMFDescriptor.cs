/*
 *  Copyright (C) 2010  Rajarshi Guha <rajarshi.guha@gmail.com>
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

using NCDK.Fragments;
using NCDK.QSAR.Results;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// An implementation of the FMF descriptor characterizing complexity of a molecule.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The descriptor is described in <token>cdk-cite-YANG2010</token> and is an approach to
    /// characterizing molecular complexity based on the Murcko framework present
    /// in the molecule. The descriptor is the ratio of heavy atoms in the framework to the
    /// total number of heavy atoms in the molecule. By definition, acyclic molecules
    /// which have no frameworks, will have a value of 0.
    /// </para>
    /// <para>
    /// Note that the authors consider an isolated ring system to be a framework (even
    /// though there is no linker).
    /// </para>
    /// <para>
    /// This descriptor returns a single double value, labeled as "FMF"
    /// </para>
    /// </remarks>
    // @author Rajarshi Guha
    // @cdk.module qsarmolecular
    // @cdk.dictref qsar-descriptors:FMF
    // @cdk.githash
    // @see org.openscience.cdk.fragment.MurckoFragmenter
    public class FMFDescriptor : IMolecularDescriptor
    {
        public FMFDescriptor() { }

        /// <summary>
        /// Calculates the FMF descriptor value for the given <see cref="IAtomContainer"/>.
        /// </summary>
        /// <param name="container">An <see cref="IAtomContainer"/> for which this descriptor should be calculated</param>
        /// <returns>An object of <see cref="DescriptorValue"/> that contains the
        ///         calculated FMF descriptor value as well as specification details</returns>
        public DescriptorValue<Result<double>> Calculate(IAtomContainer container)
        {
            MurckoFragmenter fragmenter = new MurckoFragmenter(true, 3);
            Result<double> result;
            try
            {
                fragmenter.GenerateFragments(container);
                var framework = fragmenter.GetFrameworksAsContainers().ToList();
                var ringSystems = fragmenter.GetRingSystemsAsContainers().ToList();
                if (framework.Count == 1)
                {
                    result = new Result<double>(framework[0].Atoms.Count / (double)container.Atoms.Count);
                }
                else if (framework.Count == 0 && ringSystems.Count == 1)
                {
                    result = new Result<double>(ringSystems[0].Atoms.Count / (double)container.Atoms.Count);
                }
                else
                    result = new Result<double>(0.0);
            }
            catch (CDKException)
            {
                result = new Result<double>(double.NaN);
            }
            return new DescriptorValue<Result<double>>(_Specification, ParameterNames, Parameters, result, DescriptorNames);
        }

        /// <summary>
        /// Returns the specific type of the FMF descriptor value.
        ///
        /// The FMF descriptor is a single, double value.
        /// 
        /// The return value from this method really indicates what type of result will
        /// be obtained from the <see cref="DescriptorValue"/> object. Note that the same result
        /// can be achieved by interrogating the <see cref="DescriptorValue"/> object; this method
        /// allows you to do the same thing, without actually calculating the descriptor.
        /// <para>
        /// Additionally, the length indicated by the result type must match the actual
        /// length of a descriptor calculated with the current parameters. Typically, the
        /// length of array result types vary with the values of the parameters. See
        /// <see cref="IDescriptor"/> for more details.</para>
        /// </summary>
        /// <returns>an instance of the <see cref="Result{Double}"/></returns>
        public IDescriptorResult DescriptorResultType { get; } = new Result<double>();

        /// <inheritdoc/>
        public IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
         new DescriptorSpecification(
             "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#fmf",
             typeof(FMFDescriptor).FullName,
             "The Chemistry Development Kit");

        /// <summary>
        /// Returns the names of the parameters for this descriptor.
        /// Since this descriptor takes no parameters, <see langword="null"/> is returned
        /// </summary>
        /// <returns><see langword="null"/>, since there are no parameters</returns>
        public IReadOnlyList<string> ParameterNames => null;

        /// <summary>
        /// Returns a class matching that of the parameter with the given name.
        /// Since this descriptor has no parameters, <see langword="null"/> is always returned
        /// </summary>
        /// <param name="name">The name of the parameter whose type is requested</param>
        /// <returns><see langword="null"/>, since this descriptor has no parameters</returns>
        public object GetParameterType(string name) => null;

        /// <summary>
        /// The parameters for this descriptor.
        /// </summary>
        public object[] Parameters { get { return null; } set { } }

        /// <summary>
        /// Returns an array of names for each descriptor value calculated.
        /// <para>
        /// Since this descriptor returns a single value, the array has a single element, viz., "FMF"
        /// </para>
        /// </summary>
        /// <returns>A 1-element string array, with the value "FMF"</returns>
        public IReadOnlyList<string> DescriptorNames { get; } = new string[] { "FMF" };

        IDescriptorValue IMolecularDescriptor.Calculate(IAtomContainer container) => Calculate(container);
    }
}
