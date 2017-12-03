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
using NCDK.Aromaticities;
using NCDK.QSAR.Results;
using NCDK.Tools.Manipulator;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    ///  Class that returns the number of aromatic atoms in an atom container.
    /// </summary>
    /// <remarks>
    /// <para>This descriptor uses these parameters:
    /// <list type="table">
    ///   <item>
    ///     <term>Name</term>
    ///     <term>Default</term>
    ///     <term>Description</term>
    ///   </item>
    ///   <item>
    ///     <term>checkAromaticity</term>
    ///     <term>false</term>
    ///     <term>True is the aromaticity has to be checked</term>
    ///   </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// Returns a single value with name <i>nAromAtom</i>
    // @author      mfe4
    // @cdk.created 2004-11-03
    // @cdk.module  qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:aromaticAtomsCount
    public class AromaticAtomsCountDescriptor : IMolecularDescriptor
    {
        private bool checkAromaticity = false;
        private static readonly string[] NAMES = { "naAromAtom" };

        /// <summary>
        ///  Constructor for the AromaticAtomsCountDescriptor object.
        /// </summary>
        public AromaticAtomsCountDescriptor() { }

        /// <inheritdoc/>
        public IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
         new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#aromaticAtomsCount",
                typeof(AromaticAtomsCountDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the AromaticAtomsCountDescriptor object.
        /// </summary>
        /// <exception cref="CDKException">if more than one parameter or a non-bool parameter is specified</exception>
        public object[] Parameters
        {
            set
            {
                if (value.Length != 1)
                {
                    throw new CDKException("AromaticAtomsCountDescriptor expects one parameter");
                }
                if (!(value[0] is bool))
                {
                    throw new CDKException("The first parameter must be of type bool");
                }
                // ok, all should be fine
                checkAromaticity = (bool)value[0];
            }
            get
            {
                // return the parameters as used for the descriptor calculation
                return new object[] { checkAromaticity };
            }
        }

        public IReadOnlyList<string> DescriptorNames => NAMES;

        /// <summary>
        /// Calculate the count of aromatic atoms in the supplied <see cref="IAtomContainer"/>.
        ///
        ///  The method require one parameter:
        ///  if checkAromaticity is true, the method check the aromaticity,
        ///  if false, means that the aromaticity has already been checked
        /// </summary>
        /// <param name="atomContainer">The <see cref="IAtomContainer"/> for which this descriptor is to be calculated</param>
        /// <returns>the number of aromatic atoms of this AtomContainer</returns>
        /// <seealso cref="Parameters"/>
        public DescriptorValue<Result<int>> Calculate(IAtomContainer atomContainer)
        {
            IAtomContainer ac = (IAtomContainer)atomContainer.Clone();

            int aromaticAtomsCount = 0;
            if (checkAromaticity)
            {
                try
                {
                    AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(ac);
                }
                catch (CDKException)
                {
                    return new DescriptorValue<Result<int>>(_Specification, ParameterNames, Parameters,
                        new Result<int>(0), DescriptorNames,
                        new CDKException("Error during atom type perception"));
                }
                try
                {
                    Aromaticity.CDKLegacy.Apply(ac);
                }
                catch (CDKException e)
                {
                    return new DescriptorValue<Result<int>>(_Specification, ParameterNames, Parameters,
                        new Result<int>(0), DescriptorNames,
                        new CDKException($"Error during aromaticity detection: {e.Message}"));
                }
            }
            for (int i = 0; i < ac.Atoms.Count; i++)
            {
                if (ac.Atoms[i].IsAromatic)
                {
                    aromaticAtomsCount += 1;
                }
            }
            return new DescriptorValue<Result<int>>(_Specification, ParameterNames, Parameters, new Result<int>(aromaticAtomsCount), DescriptorNames);
        }

        /// <summary>
        /// Returns the specific type of the DescriptorResult object.
        /// <p/>
        /// The return value from this method really indicates what type of result will
        /// be obtained from the <see cref="DescriptorValue"/> object. Note that the same result
        /// can be achieved by interrogating the <see cref="DescriptorValue"/> object; this method
        /// allows you to do the same thing, without actually calculating the descriptor.
        ///
        /// <returns>an object that implements the <see cref="IDescriptorResult"/> interface indicating</returns>
        ///         the actual type of values returned by the descriptor in the <see cref="DescriptorValue"/> object
        /// </summary>
        public IDescriptorResult DescriptorResultType { get; } = new Result<int>(1);

        /// <summary>
        ///  Gets the parameterNames attribute of the AromaticAtomsCountDescriptor object.
        ///
        /// <returns>The parameterNames value</returns>
        /// </summary>
        public IReadOnlyList<string> ParameterNames { get; } = new string[] { "checkAromaticity" };

        /// <summary>
        ///  Gets the parameterType attribute of the AromaticAtomsCountDescriptor object.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>An Object of class equal to that of the parameter being requested</returns>
        public object GetParameterType(string name) => true;

        DescriptorValue IMolecularDescriptor.Calculate(IAtomContainer container) => Calculate(container);
    }
}
