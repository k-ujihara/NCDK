/*  Copyright (C) 2005-2007  Christian Hoppe <chhoppe@users.sf.net>
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
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Class that returns the number of atoms in the largest pi system.
    /// </summary>
    /// <remarks>
    /// <para>This descriptor uses these parameters:
    /// <list type="table">
    /// <item>
    /// <term>Name</term>
    /// <term>Default</term>
    /// <term>Description</term>
    /// </item>
    /// <item>
    /// <term>checkAromaticity</term>
    /// <term>false</term>
    /// <term>True is the aromaticity has to be checked</term>
    /// </item>
    /// </list>
    /// </para>
    /// Returns a single value named <i>nAtomPi</i>
    /// </remarks>
    // @author chhoppe from EUROSCREEN
    // @cdk.created 2006-1-03
    // @cdk.module qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:largestPiSystem
    public class LargestPiSystemDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private bool checkAromaticity = false;
        private static readonly string[] NAMES = { "nAtomP" };

        /// <summary>
        /// Constructor for the LargestPiSystemDescriptor object.
        /// </summary>
        public LargestPiSystemDescriptor() { }

        /// <inheritdoc/> 
        public override IImplementationSpecification Specification => specification;
        private static readonly DescriptorSpecification specification =
         new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#largestPiSystem",
                typeof(LargestPiSystemDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the LargestPiSystemDescriptor object.
        /// </summary>
        /// <remarks>
        /// This descriptor takes one parameter, which should be bool to indicate whether
        /// aromaticity has been checked <see langword="true"/> or not <see langword="false"/>.
        /// </remarks>
        /// <exception cref="CDKException">if more than one parameter or a non-bool parameter is specified</exception>
        public override IReadOnlyList<object> Parameters
        {
            set
            {
                if (value.Count > 1)
                {
                    throw new CDKException("LargestPiSystemDescriptor only expects one parameter");
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

        public override IReadOnlyList<string> DescriptorNames => NAMES;

        private DescriptorValue<Result<int>> GetDummyDescriptorValue(Exception e)
        {
            return new DescriptorValue<Result<int>>(specification, ParameterNames, Parameters, new Result<int>(0), DescriptorNames, e);
        }

        /// <summary>
        /// Calculate the count of atoms of the largest pi system in the supplied <see cref="IAtomContainer"/>.
        /// </summary>
        /// <remarks>
        /// <para>The method require one parameter:
        /// <list type="bullet"> 
        /// <item>if checkAromaticity is true, the method check the aromaticity,</item>
        /// <item>if false, means that the aromaticity has already been checked</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="container">The <see cref="IAtomContainer"/> for which this descriptor is to be calculated</param>
        /// <returns>the number of atoms in the largest pi system of this AtomContainer</returns>
        /// <seealso cref="Parameters"/>
        public DescriptorValue<Result<int>> Calculate(IAtomContainer container)
        {
            var originalFlag4 = new bool[container.Atoms.Count];
            for (int i = 0; i < originalFlag4.Length; i++)
            {
                originalFlag4[i] = container.Atoms[i].IsVisited;
            }
            if (checkAromaticity)
            {
                try
                {
                    AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(container);
                    Aromaticity.CDKLegacy.Apply(container);
                }
                catch (CDKException e)
                {
                    return GetDummyDescriptorValue(e);
                }
            }
            int largestPiSystemAtomsCount = 0;
            //Set all VisitedFlags to False
            for (int i = 0; i < container.Atoms.Count; i++)
            {
                container.Atoms[i].IsVisited = false;
            }
            //Debug.WriteLine("Set all atoms to Visited False");
            for (int i = 0; i < container.Atoms.Count; i++)
            {
                //Possible pi System double bond or triple bond, charge, N or O (free electron pair)
                if ((container.GetMaximumBondOrder(container.Atoms[i]) != BondOrder.Single
                        || Math.Abs(container.Atoms[i].FormalCharge.Value) >= 1
                        || container.Atoms[i].IsAromatic
                        || container.Atoms[i].Symbol.Equals("N", StringComparison.Ordinal) || container.Atoms[i].Symbol.Equals("O", StringComparison.Ordinal))
                        & !container.Atoms[i].IsVisited)
                {
                    //Debug.WriteLine("...... -> Accepted");
                    var startSphere = new List<IAtom>();
                    var path = new List<IAtom>();
                    startSphere.Add(container.Atoms[i]);
                    try
                    {
                        BreadthFirstSearch(container, startSphere, path);
                    }
                    catch (CDKException e)
                    {
                        return GetDummyDescriptorValue(e);
                    }
                    if (path.Count > largestPiSystemAtomsCount)
                    {
                        largestPiSystemAtomsCount = path.Count;
                    }
                }

            }
            // restore original flag values
            for (int i = 0; i < originalFlag4.Length; i++)
            {
                container.Atoms[i].IsVisited = originalFlag4[i];
            }

            return new DescriptorValue<Result<int>>(specification, ParameterNames, Parameters, new Result<int>(
                    largestPiSystemAtomsCount), DescriptorNames);
        }

        /// <inheritdoc/>
        public override IDescriptorResult DescriptorResultType { get; } = new Result<int>(1);

        /// <summary>
        /// Performs a breadthFirstSearch in an AtomContainer starting with a
        /// particular sphere, which usually consists of one start atom, and searches
        /// for a pi system.
        /// </summary>
        /// <param name="container">The AtomContainer to be searched</param>
        /// <param name="sphere">A sphere of atoms to start the search with</param>
        /// <param name="path">An array list which stores the atoms belonging to the pi system</param>
        /// <exception cref="CDKException"></exception>
        private void BreadthFirstSearch(IAtomContainer container, List<IAtom> sphere, List<IAtom> path)
        {
            IAtom nextAtom;
            List<IAtom> newSphere = new List<IAtom>();
            //Debug.WriteLine("Start of breadthFirstSearch");
            foreach (var atom in sphere)
            {
                //Debug.WriteLine("BreadthFirstSearch around atom " + (atomNr + 1));
                var bonds = container.GetConnectedBonds(atom);
                foreach (var bond in bonds)
                {
                    nextAtom = ((IBond)bond).GetConnectedAtom(atom);
                    if ((container.GetMaximumBondOrder(nextAtom) != BondOrder.Single
                            || Math.Abs(nextAtom.FormalCharge.Value) >= 1 || nextAtom.IsAromatic
                            || nextAtom.Symbol.Equals("N", StringComparison.Ordinal) || nextAtom.Symbol.Equals("O", StringComparison.Ordinal))
                            & !nextAtom.IsVisited)
                    {
                        //Debug.WriteLine("BDS> AtomNr:"+container.Atoms.IndexOf(nextAtom)+" maxBondOrder:"+container.GetMaximumBondOrder(nextAtom)+" Aromatic:"+nextAtom.IsAromatic+" FormalCharge:"+nextAtom.FormalCharge+" Charge:"+nextAtom.Charge+" Flag:"+nextAtom.IsVisited);
                        path.Add(nextAtom);
                        //Debug.WriteLine("BreadthFirstSearch is meeting new atom " + (nextAtomNr + 1));
                        nextAtom.IsVisited = true;
                        if (container.GetConnectedBonds(nextAtom).Count() > 1)
                        {
                            newSphere.Add(nextAtom);
                        }
                    }
                    else
                    {
                        nextAtom.IsVisited = true;
                    }
                }
            }
            if (newSphere.Count > 0)
            {
                BreadthFirstSearch(container, newSphere, path);
            }
        }

        public override IReadOnlyList<string> ParameterNames { get; } = new string[] { "checkAromaticity" };
        public override object GetParameterType(string name)
        {
            if (string.Equals("checkAromaticity", name, StringComparison.Ordinal))
                return false;
            return null;
        }

        IDescriptorValue IMolecularDescriptor.Calculate(IAtomContainer container) => Calculate(container);
    }
}
