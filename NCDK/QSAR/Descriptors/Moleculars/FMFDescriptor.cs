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

using NCDK.Fragment;
using NCDK.QSAR.Result;
using System.Linq;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /**
     * An implementation of the FMF descriptor characterizing complexity of a molecule.
     * <p/>
     * The descriptor is described in {@cdk.cite YANG2010} and is an approach to
     * characterizing molecular complexity based on the Murcko framework present
     * in the molecule. The descriptor is the ratio of heavy atoms in the framework to the
     * total number of heavy atoms in the molecule. By definition, acyclic molecules
     * which have no frameworks, will have a value of 0.
     *
     * Note that the authors consider an isolated ring system to be a framework (even
     * though there is no linker).
     *
     * This descriptor returns a single double value, labeled as "FMF"
     *
     * @author Rajarshi Guha
     * @cdk.module qsarmolecular
     * @cdk.set qsar-descriptors
     * @cdk.dictref qsar-descriptors:FMF
     * @cdk.githash
     * @see org.openscience.cdk.fragment.MurckoFragmenter
     */
    public class FMFDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        public FMFDescriptor() { }

        /// <summary>
        /// Calculates the FMF descriptor value for the given <see cref="IAtomContainer"/>.
        /// </summary>
        /// <param name="container">An <see cref="IAtomContainer"/> for which this descriptor should be calculated</param>
        /// <returns>An object of <see cref="DescriptorValue"/> that contains the
        ///         calculated FMF descriptor value as well as specification details</returns>
        public override DescriptorValue Calculate(IAtomContainer container)
        {
            MurckoFragmenter fragmenter = new MurckoFragmenter(true, 3);
            DoubleResult result;
            try
            {
                fragmenter.GenerateFragments(container);
                var framework = fragmenter.GetFrameworksAsContainers().ToList();
                var ringSystems = fragmenter.GetRingSystemsAsContainers().ToList();
                if (framework.Count == 1)
                {
                    result = new DoubleResult(framework[0].Atoms.Count / (double)container.Atoms.Count);
                }
                else if (framework.Count == 0 && ringSystems.Count == 1)
                {
                    result = new DoubleResult(ringSystems[0].Atoms.Count / (double)container.Atoms.Count);
                }
                else
                    result = new DoubleResult(0.0);
            }
            catch (CDKException)
            {
                result = new DoubleResult(double.NaN);
            }
            return new DescriptorValue(_Specification, ParameterNames, Parameters, result, DescriptorNames);
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
        /// <returns>an instance of the <see cref="DoubleResultType"/></returns>
        public override IDescriptorResult DescriptorResultType { get; } = new DoubleResultType();

        /// <summary>
        /// Returns a map which specifies which descriptor is implemented by this class.
        /// <para>
        /// These fields are used in the map:
        /// <list type="bullet">
        /// <item>
        /// <term>Specification-Reference</term>
        /// <description>refers to an entry in a unique dictionary</description>
        /// </item>
        /// <item>
        /// <term>Implementation-Title</term>
        /// <description>anything</description>
        /// </item>
        /// <item>
        /// <term>Implementation-Identifier</term>
        /// <description>a unique identifier for this version of this class</description>
        /// </item>
        /// <item>
        /// <term>Implementation-Vendor</term>
        /// <description>CDK, JOELib, or anything else</description>
        /// </item>
        /// </list>
        /// </para>
        /// </summary>
        /// <returns>An object containing the descriptor specification</returns>
        public override IImplementationSpecification Specification => _Specification;
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
        public override string[] ParameterNames => null;

        /// <summary>
        /// Returns a class matching that of the parameter with the given name.
        /// Since this descriptor has no parameters, <see langword="null"/> is always returned
        /// </summary>
        /// <param name="name">The name of the parameter whose type is requested</param>
        /// <returns><see langword="null"/>, since this descriptor has no parameters</returns>
        public override object GetParameterType(string name) => null;

        /// <summary>
        /// The parameters for this descriptor.
        /// </summary>
        public override object[] Parameters { get { return null; } set { } }

        /// <summary>
        /// Returns an array of names for each descriptor value calculated.
        /// <para>
        /// Since this descriptor returns a single value, the array has a single element, viz., "FMF"
        /// </para>
        /// </summary>
        /// <returns>A 1-element string array, with the value "FMF"</returns>
        public override string[] DescriptorNames { get; } = new string[] { "FMF" };
    }
}
