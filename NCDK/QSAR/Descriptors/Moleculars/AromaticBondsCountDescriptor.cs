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
    /// This Class contains a method that returns the number of aromatic atoms in an AtomContainer.
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
    /// Returns a single value with name <i>nAromBond</i>
    /// </remarks>
    // @author      mfe4
    // @cdk.created 2004-11-03
    // @cdk.module  qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:aromaticBondsCount
    public partial class AromaticBondsCountDescriptor : IMolecularDescriptor
    {
        private bool checkAromaticity = false;
        private static readonly string[] NAMES = { "nAromBond" };

        /// <summary>
        ///  Constructor for the AromaticBondsCountDescriptor object.
        /// </summary>
        public AromaticBondsCountDescriptor() { }

        /// <inheritdoc/>
        public IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#aromaticBondsCount",
                typeof(AromaticBondsCountDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <summary>
        ///  Sets the parameters attribute of the AromaticBondsCountDescriptor object.
        ///
        /// This descriptor takes one parameter, which should be bool to indicate whether
        /// aromaticity has been checked (TRUE) or not (FALSE).
        /// </summary>
        /// <exception cref="CDKException">if more than one parameter or a non-bool parameter is specified</exception>
        public object[] Parameters
        {
            set
            {
                if (value.Length != 1)
                {
                    throw new CDKException("AromaticBondsCountDescriptor expects one parameter");
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
        ///  The method take a bool checkAromaticity: if the bool is true, it means that
        ///  aromaticity has to be checked.
        /// </summary>
        /// <param name="atomContainer">The <see cref="IAtomContainer"/> for which this descriptor is to be calculated</param>
        /// <returns>the number of aromatic atoms of this AtomContainer</returns>
        /// <seealso cref="Parameters"/>
        public DescriptorValue<Result<int>> Calculate(IAtomContainer atomContainer)
        {
            IAtomContainer ac = (IAtomContainer)atomContainer.Clone();

            int aromaticBondsCount = 0;
            if (checkAromaticity)
            {
                try
                {
                    AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(ac);
                }
                catch (CDKException)
                {
                    return new DescriptorValue<Result<int>>(_Specification, ParameterNames, Parameters,
                        new Result<int>(0), DescriptorNames, new CDKException("Error during atom type perception"));
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
            foreach (var bond in ac.Bonds)
            {
                if (bond.IsAromatic)
                {
                    aromaticBondsCount += 1;
                }
            }
            return new DescriptorValue<Result<int>>(_Specification, ParameterNames, Parameters, new Result<int>(aromaticBondsCount), DescriptorNames);
        }

        /// <inheritdoc/>
        public IDescriptorResult DescriptorResultType { get; } = new Result<int>(1);

        /// <summary>
        ///  The parameterNames attribute of the AromaticBondsCountDescriptor object.
        /// </summary>
        public IReadOnlyList<string> ParameterNames { get; } = new string[] { "checkAromaticity" };

        /// <summary>
        ///  Gets the parameterType attribute of the AromaticBondsCountDescriptor object.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>An Object of class equal to that of the parameter being requested</returns>
        public object GetParameterType(string name) => true;
    }
}
