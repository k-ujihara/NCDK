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
using NCDK.Graphs.Invariant;
using NCDK.QSAR.Results;
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    ///  This class evaluates if a proton is joined to a conjugated system.
    ///  </summary>
    /// <remarks>
    ///  This descriptor uses these parameters:
    /// <list type="table">
    /// <listheader><term>Name</term><term>Default</term><term>Description</term></listheader>
    /// <item><term>checkAromaticity</term><term>false</term><term>True is the aromaticity has to be checked</term></item>
    /// </list>
    /// </remarks>
    // @author      mfe4
    // @cdk.created 2004-11-03
    // @cdk.module  qsaratomic
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:isProtonInConjugatedPiSystem
    public class IsProtonInConjugatedPiSystemDescriptor : AbstractAtomicDescriptor, IAtomicDescriptor
    {
        private static readonly string[] NAMES = { "protonInConjSystem" };
        private bool checkAromaticity = false;
        private IAtomContainer acold = null;
        private IChemObjectSet<IAtomContainer> acSet = null;

        /// <summary>
        ///  Constructor for the IsProtonInConjugatedPiSystemDescriptor object
        /// </summary>
        public IsProtonInConjugatedPiSystemDescriptor() { }

        /// <summary>
        /// The specification attribute of the IsProtonInConjugatedPiSystemDescriptor object
        /// </summary>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#isProtonInConjugatedPiSystem",
                typeof(IsProtonInConjugatedPiSystemDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the IsProtonInConjugatedPiSystemDescriptor object
        /// <exception cref="CDKException"></exception>
        /// </summary>
        public override object[] Parameters
        {
            set
            {
                if (value.Length > 1)
                {
                    throw new CDKException("IsProtonInConjugatedPiSystemDescriptor only expects one parameters");
                }
                if (!(value[0] is bool))
                {
                    throw new CDKException("The parameter must be of type bool");
                }
                checkAromaticity = (bool)value[0];
            }
            get
            {
                // return the parameters as used for the descriptor calculation
                return new object[] { checkAromaticity };
            }
        }

        public override string[] DescriptorNames => NAMES;

        /// <summary>
        ///  The method is a proton descriptor that evaluates if a proton is joined to a conjugated system.
        /// </summary>
        /// <param name="atom">The <see cref="IAtom"/> for which the <see cref="DescriptorValue"/> is requested</param>
        /// <param name="atomContainer">AtomContainer</param>
        /// <returns>true if the proton is bonded to a conjugated system</returns>
        public override DescriptorValue Calculate(IAtom atom, IAtomContainer atomContainer)
        {
            IAtomContainer clonedAtomContainer;
            clonedAtomContainer = (IAtomContainer)atomContainer.Clone();
            IAtom clonedAtom = clonedAtomContainer.Atoms[atomContainer.Atoms.IndexOf(atom)];

            bool isProtonInPiSystem = false;
            IAtomContainer mol = clonedAtom.Builder.NewAtomContainer(clonedAtomContainer);
            if (checkAromaticity)
            {
                try
                {
                    AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
                    Aromaticity.CDKLegacy.Apply(mol);
                }
                catch (CDKException e)
                {
                    return new DescriptorValue(_Specification, ParameterNames, Parameters, new Result<bool>(false), NAMES, e);
                }
            }
            if (atom.Symbol.Equals("H"))
            {
                if (acold != clonedAtomContainer)
                {
                    acold = clonedAtomContainer;
                    acSet = ConjugatedPiSystemsDetector.Detect(mol);
                }
                var detected = acSet.GetEnumerator();
                var neighboors = mol.GetConnectedAtoms(clonedAtom);
                foreach (var neighboor in neighboors)
                {
                    while (detected.MoveNext())
                    {
                        IAtomContainer detectedAC = detected.Current;
                        if ((detectedAC != null) && (detectedAC.Contains(neighboor)))
                        {
                            isProtonInPiSystem = true;
                            break;
                        }
                    }
                }
            }
            return new DescriptorValue(_Specification, ParameterNames, Parameters, new Result<bool>(
                    isProtonInPiSystem), NAMES);
        }

        /// <summary>
        /// The parameterNames attribute of the IsProtonInConjugatedPiSystemDescriptor object
        /// </summary>
        public override string[] ParameterNames { get; } = new string[] { "checkAromaticity" };

        /// <summary>
        ///  Gets the parameterType attribute of the
        ///  IsProtonInConjugatedPiSystemDescriptor object
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public override object GetParameterType(string name) => true;
    }
}
