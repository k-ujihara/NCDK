/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
 *                    2011  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Aromaticities;
using NCDK.Graphs.Invariant;
using NCDK.QSAR.Result;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.AtomPair
{
    /// <summary>
    /// This class checks if two atoms have pi-contact (this is true when there is
    /// one and the same conjugated pi-system which contains both atoms, or directly
    /// linked neighbours of the atoms).
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
    ///     <term>firstAtom</term>
    ///     <term>0</term>
    ///     <term>The position of the first atom</term>
    ///   </item>
    ///   <item>
    ///     <term>secondAtom</term>
    ///     <term>0</term>
    ///     <term>The position of the second atom</term>
    ///   </item>
    ///   <item>
    ///     <term>checkAromaticity</term>
    ///     <term>false</term>
    ///     <term>True is the aromaticity has to be checked</term>
    ///   </item>
    /// </list>
    /// </para>
    /// </remarks>
    // @author         mfe4
    // @cdk.created    2004-11-03
    // @cdk.module     qsarmolecular
    // @cdk.githash
    // @cdk.set        qsar-descriptors
    // @cdk.dictref    qsar-descriptors:piContact
    public class PiContactDetectionDescriptor : AbstractAtomPairDescriptor, IAtomPairDescriptor
    {
        private static readonly string[] NAMES = { "piContact" };

        private bool checkAromaticity = false;
        IAtomContainerSet<IAtomContainer> acSet = null;
        private IAtomContainer acold = null;

        public PiContactDetectionDescriptor() { }

        /// <summary>
        /// The specification attribute of the PiContactDetectionDescriptor object.
        /// </summary>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#piContact",
                typeof(PiContactDetectionDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the PiContactDetectionDescriptor object.
        /// <para>Parameters contains a bool (true if is needed a checkAromaticity)Parameters contains a bool (true if is needed a checkAromaticity)</para>
        /// </summary>
        /// <exception cref="CDKException">Description of the Exception</exception>
        public override object[] Parameters
        {
            set
            {
                if (value.Length != 1)
                {
                    throw new CDKException("PiContactDetectionDescriptor expects 1 parameters");
                }
                if (!(value[0] is bool))
                {
                    throw new CDKException("The first parameter must be of type bool");
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

        private DescriptorValue GetDummyDescriptorValue(Exception e)
        {
            return new DescriptorValue(_Specification, ParameterNames, Parameters, new BooleanResult(false), NAMES, e);
        }

        /// <summary>
        /// The method returns if two atoms have pi-contact.
        /// </summary>
        /// <param name="atomContainer">AtomContainer</param>
        /// <returns>true if the atoms have pi-contact</returns>
        public override DescriptorValue Calculate(IAtom first, IAtom second, IAtomContainer atomContainer)
        {
            IAtomContainer ac = (IAtomContainer)atomContainer.Clone();
            IAtom clonedFirst = ac.Atoms[atomContainer.Atoms.IndexOf(first)];
            IAtom clonedSecond = ac.Atoms[atomContainer.Atoms.IndexOf(first)];

            IAtomContainer mol = ac.Builder.CreateAtomContainer(ac);
            if (checkAromaticity)
            {
                try
                {
                    AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
                    Aromaticity.CDKLegacy.Apply(mol);
                }
                catch (CDKException e)
                {
                    return GetDummyDescriptorValue(e);
                }
            }
            bool piContact = false;
            int counter = 0;

            if (acold != ac)
            {
                acold = ac;
                acSet = ConjugatedPiSystemsDetector.Detect(mol);
            }
            var detected = acSet;

            var neighboorsFirst = mol.GetConnectedAtoms(clonedFirst);
            var neighboorsSecond = mol.GetConnectedAtoms(clonedSecond);

            foreach (var detectedAC in detected)
            {
                if (detectedAC.Contains(clonedFirst) && detectedAC.Contains(clonedSecond))
                {
                    counter += 1;
                    break;
                }
                if (IsANeighboorsInAnAtomContainer(neighboorsFirst, detectedAC)
                        && IsANeighboorsInAnAtomContainer(neighboorsSecond, detectedAC))
                {
                    counter += 1;
                    break;
                }
            }

            if (counter > 0)
            {
                piContact = true;
            }
            return new DescriptorValue(_Specification, ParameterNames, Parameters, new BooleanResult(
                    piContact), DescriptorNames);
        }

        /// <summary>
        /// Gets if neighbours of an atom are in an atom container.
        ///
        /// <param name="neighs">array of atoms</param>
        /// <param name="ac">AtomContainer</param>
        /// <returns>The bool result</returns>
        /// </summary>
        private bool IsANeighboorsInAnAtomContainer(IEnumerable<IAtom> neighs, IAtomContainer ac)
        {
            bool isIn = false;
            int count = 0;
            foreach (var neigh in neighs)
            {
                if (ac.Contains(neigh))
                {
                    count += 1;
                }
            }
            if (count > 0)
            {
                isIn = true;
            }
            return isIn;
        }

        /// <summary>
        /// The parameterNames attribute of the PiContactDetectionDescriptor object.
        /// </summary>
        public override string[] ParameterNames { get; } = new string[] { "checkAromaticity" };

        /// <summary>
        /// Gets the parameterType attribute of the PiContactDetectionDescriptor object.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public override object GetParameterType(string name)
        {
            if (name.Equals("checkAromaticity")) return true;
            return null;
        }
    }
}
