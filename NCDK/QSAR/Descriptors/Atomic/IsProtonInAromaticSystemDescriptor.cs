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
using NCDK.Config;
using NCDK.QSAR.Results;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    ///  This descriptor returns 1 if the protons is directly bonded to an aromatic system,
    ///  it returns 2 if the distance between aromatic system and proton is 2 bonds,
    ///  and it return 0 for other positions. It is needed to use addExplicitHydrogensToSatisfyValency method.
    ///  </summary>
    /// <remarks>
    ///  This descriptor uses these parameters:
    /// <list type="table">
    /// <listheader>
    ///   <term>Name</term>
    ///   <term>Default</term>
    ///   <term>Description</term>
    /// </listheader>
    /// <item>
    ///     <term>checkAromaticity</term>
    ///     <term>false</term>
    ///     <term>True is the aromaticity has to be checked</term>
    /// </item>
    /// </list>
    /// </remarks>
    // @author      mfe4
    // @cdk.created 2004-11-03
    // @cdk.module  qsaratomic
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:isProtonInAromaticSystem
    public partial class IsProtonInAromaticSystemDescriptor : IAtomicDescriptor
    {
        private static readonly string[] NAMES = { "protonInArmaticSystem" };

        private bool checkAromaticity = false;

        /// <summary>
        ///  Constructor for the IsProtonInAromaticSystemDescriptor object
        /// </summary>
        public IsProtonInAromaticSystemDescriptor() { }

        /// <summary>
        /// The specification attribute of the IsProtonInAromaticSystemDescriptor object
        /// </summary>
        public IImplementationSpecification Specification => specification;
        private static readonly DescriptorSpecification specification =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#isProtonInAromaticSystem",
                typeof(IsProtonInAromaticSystemDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the IsProtonInAromaticSystemDescriptor object
        /// </summary>
        /// <exception cref="CDKException"></exception>
        public IReadOnlyList<object> Parameters
        {
            set
            {
                if (value.Count > 1)
                {
                    throw new CDKException("IsProtonInAromaticSystemDescriptor only expects two parameters");
                }
                if (!(value[0] is bool))
                {
                    throw new CDKException("The second parameter must be of type bool");
                }
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
        ///  The method is a proton descriptor that evaluate if a proton is bonded to an aromatic system or if there is distance of 2 bonds.
        ///  It is needed to call the addExplicitHydrogensToSatisfyValency method from the class tools.HydrogenAdder.
        /// </summary>
        /// <param name="atom">The <see cref="IAtom"/> for which the <see cref="IDescriptorValue"/> is requested</param>
        /// <param name="atomContainer">AtomContainer</param>
        /// <returns>true if the proton is bonded to an aromatic atom.</returns>
        public DescriptorValue<Result<int>> Calculate(IAtom atom, IAtomContainer atomContainer)
        {
            IAtomContainer clonedAtomContainer = (IAtomContainer)atomContainer.Clone();
            IAtom clonedAtom = clonedAtomContainer.Atoms[atomContainer.Atoms.IndexOf(atom)];

            int isProtonInAromaticSystem = 0;
            IAtomContainer mol = atom.Builder.NewAtomContainer(clonedAtomContainer);
            if (checkAromaticity)
            {
                try
                {
                    AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
                    Aromaticity.CDKLegacy.Apply(mol);
                }
                catch (CDKException e)
                {
                    return new DescriptorValue<Result<int>>(specification, ParameterNames, Parameters, new Result<int>(0), NAMES, e);
                }
            }
            var neighboor = mol.GetConnectedAtoms(clonedAtom);
            var neighbour0 = neighboor.First();
            if (atom.AtomicNumber.Equals(ChemicalElement.AtomicNumbers.H))
            {
                //Debug.WriteLine("aromatic proton");
                if (neighbour0.IsAromatic)
                {
                    isProtonInAromaticSystem = 1;
                }
                else
                {
                    var betaAtoms = clonedAtomContainer.GetConnectedAtoms(neighbour0);
                    foreach (var betaAtom in betaAtoms)
                    {
                        if (betaAtom.IsAromatic)
                        {
                            isProtonInAromaticSystem = 2;
                        }
                        else
                        {
                            isProtonInAromaticSystem = 0;
                        }
                    }
                }
            }
            else
            {
                isProtonInAromaticSystem = 0;
            }
            return new DescriptorValue<Result<int>>(specification, ParameterNames, Parameters, new Result<int>(isProtonInAromaticSystem), NAMES);
        }

        /// <summary>
        /// The parameterNames attribute of the IsProtonInAromaticSystemDescriptor object
        /// </summary>
        public IReadOnlyList<string> ParameterNames { get; } = new string[] { "checkAromaticity" };

        /// <summary>
        ///  Gets the parameterType attribute of the IsProtonInAromaticSystemDescriptor object
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public object GetParameterType(string name) => true;
    }
}
