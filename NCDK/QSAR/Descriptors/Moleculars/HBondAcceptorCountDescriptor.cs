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

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// This descriptor calculates the number of hydrogen bond acceptors using a slightly simplified version of the
    /// <see href="http://www.chemie.uni-erlangen.de/model2001/abstracts/rester.html">PHACIR atom types</see>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The following groups are counted as hydrogen bond acceptors:
    /// <list type="bullet"> 
    /// <item>any oxygen where the formal charge of the oxygen is non-positive (i.e. formal charge &lt;= 0) <b>except</b></item>
    /// <list type="bullet"> 
    /// <item>an aromatic ether oxygen (i.e. an ether oxygen that is adjacent to at least one aromatic carbon)</item>
    /// <item>an oxygen that is adjacent to a nitrogen</item>
    /// </list>
    /// <item>any nitrogen where the formal charge of the nitrogen is non-positive (i.e. formal charge &lt;= 0) <b>except</b></item>
    /// <list type="bullet"> 
    /// <item>a nitrogen that is adjacent to an oxygen</item>
    /// </list>
    /// </list>
    /// </para>
    /// <para>
    /// Returns a single value named <i>nHBAcc</i>.
    /// </para>
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
    ///     <term>true if the aromaticity has to be checked</term>
    ///   </item>
    /// </list>
    /// </para>
    /// <para>
    /// This descriptor works properly with AtomContainers whose atoms contain <b>implicit hydrogens</b> or <b>explicit
    /// hydrogens</b>.
    /// </para>
    /// </remarks>
    // @author      ulif
    // @cdk.created 2005-22-07
    // @cdk.module  qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:hBondacceptors
    public class HBondAcceptorCountDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        // only parameter of this descriptor; true if aromaticity has to be checked prior to descriptor calculation, false otherwise
        private bool checkAromaticity = false;
        private static readonly string[] NAMES = { "nHBAcc" };

        public HBondAcceptorCountDescriptor() { }

        public override IImplementationSpecification Specification => specification;
        private static readonly DescriptorSpecification specification =
         new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#hBondacceptors",
                typeof(HBondAcceptorCountDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the <see cref="HBondAcceptorCountDescriptor"/> object.
        /// </summary>
        /// <value>a <see langword="true"/> means that aromaticity has to be checked</value>
        /// <exception cref="CDKException"></exception>
        public override IReadOnlyList<object> Parameters
        {
            set
            {
                if (value.Count != 1)
                {
                    throw new CDKException($"{nameof(HBondAcceptorCountDescriptor)} expects a single parameter");
                }
                if (!(value[0] is bool))
                {
                    throw new CDKException($"The parameter must be of type {nameof(Boolean)}");
                }
                // OK, all should be fine
                checkAromaticity = (bool)value[0];
            }
            get
            {
                return new object[] { checkAromaticity };
                // return the parameters as used for the descriptor calculation
            }
        }

        public override IReadOnlyList<string> DescriptorNames => NAMES;

        private DescriptorValue<Result<int>> GetDummyDescriptorValue(Exception e)
        {
            return new DescriptorValue<Result<int>>(specification, ParameterNames, Parameters, new Result<int>(0), DescriptorNames, e);
        }

        /// <summary>
        /// Calculates the number of H bond acceptors.
        /// </summary>
        /// <param name="atomContainer">AtomContainer</param>
        /// <returns>number of H bond acceptors</returns>
        public DescriptorValue<Result<int>> Calculate(IAtomContainer atomContainer)
        {
            int hBondAcceptors = 0;
            var ac = (IAtomContainer)atomContainer.Clone();

            // aromaticity is detected prior to descriptor calculation if the respective parameter is set to true

            if (checkAromaticity)
            {
                try
                {
                    AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(ac);
                    Aromaticity.CDKLegacy.Apply(ac);
                }
                catch (CDKException e)
                {
                    return GetDummyDescriptorValue(e);
                }
            }

            //IAtom[] atoms = ac.GetAtoms();
            // labelled for loop to allow for labelled continue statements within the loop
            foreach (var atom in ac.Atoms)
            {
                // looking for suitable nitrogen atoms
                if (atom.AtomicNumber.Equals(NaturalElement.AtomicNumbers.N) && atom.FormalCharge <= 0)
                {
                    // excluding nitrogens that are adjacent to an oxygen
                    var bonds = ac.GetConnectedBonds(atom);
                    int nPiBonds = 0;
                    foreach (var bond in bonds)
                    {
                        if (bond.GetConnectedAtom(atom).AtomicNumber.Equals(NaturalElement.AtomicNumbers.O))
                            goto continue_atomloop;
                        if (BondOrder.Double.Equals(bond.Order))
                            nPiBonds++;
                    }

                    // if the nitrogen is aromatic and there are no pi bonds then it's
                    // lone pair cannot accept any hydrogen bonds
                    if (atom.IsAromatic && nPiBonds == 0)
                        continue;

                    hBondAcceptors++;
                }
                // looking for suitable oxygen atoms
                else if (atom.AtomicNumber.Equals(NaturalElement.AtomicNumbers.O) && atom.FormalCharge <= 0)
                {
                    //excluding oxygens that are adjacent to a nitrogen or to an aromatic carbon
                    var neighbours = ac.GetConnectedBonds(atom);
                    foreach (var bond in neighbours)
                    {
                        var neighbor = bond.GetOther(atom);
                        switch (neighbor.AtomicNumber)
                        {
                            case NaturalElement.AtomicNumbers.N:
                                goto continue_atomloop;
                            case NaturalElement.AtomicNumbers.C:
                                if (neighbor.IsAromatic && bond.Order != BondOrder.Double)
                                    goto continue_atomloop;
                                break;
                        }
                    }
                    hBondAcceptors++;
                }
            continue_atomloop:
                ;
            }

            return new DescriptorValue<Result<int>>(specification, ParameterNames, Parameters, new Result<int>(hBondAcceptors), DescriptorNames);
        }

        /// <inheritdoc/>
        public override IDescriptorResult DescriptorResultType { get; } = new Result<int>(1);
        public override IReadOnlyList<string> ParameterNames { get; } = new string[] { "checkAromaticity" };
        public override object GetParameterType(string name) => false;

        IDescriptorValue IMolecularDescriptor.Calculate(IAtomContainer container) => Calculate(container);
    }
}
