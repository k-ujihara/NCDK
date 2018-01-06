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
using NCDK.QSAR.Results;
using System;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// This descriptor calculates the number of hydrogen bond donors using a slightly simplified version of the
    /// <see href="http://www.chemie.uni-erlangen.de/model2001/abstracts/rester.html">PHACIR atom types</see>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The following groups are counted as hydrogen bond donors:
    /// <list type="bullet"> 
    /// <item>Any-OH where the formal charge of the oxygen is non-negative (i.e. formal charge >= 0)</item>
    /// <item>Any-NH where the formal charge of the nitrogen is non-negative (i.e. formal charge >= 0)</item>
    /// </list>
    /// </para>
    /// <para>
    /// This descriptor uses no parameters.
    /// </para>
    /// <para>
    /// This descriptor works properly with AtomContainers whose atoms contain either <b>implicit</b> or <b>explicit
    /// hydrogen</b> atoms. It does not work with atoms that contain neither implicit nor explicit hydrogens.
    /// </para>
    /// <para>
    /// Returns a single value named <i>nHBDon</i>.
    /// </para>
    /// <para>This descriptor uses these parameters:
    /// <list type="table">
    ///   <item>
    ///     <term>Name</term>
    ///     <term>Default</term>
    ///     <term>Description</term>
    ///   </item>
    ///   <item>
    ///     <term></term>
    ///     <term></term>
    ///     <term>no parameters</term>
    ///   </item>
    /// </list>
    /// </para>
    /// </remarks>
    // @author      ulif
    // @cdk.created 2005-22-07
    // @cdk.module  qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:hBondDonors
    public class HBondDonorCountDescriptor : IMolecularDescriptor
    {
        private static readonly string[] NAMES = { "nHBDon" };

        /// <summary>
        ///  Constructor for the HBondDonorCountDescriptor object
        /// </summary>
        public HBondDonorCountDescriptor() { }

        /// <summary>
        /// The specification attribute of the HBondDonorCountDescriptor object
        /// </summary>
        public IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
         new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#hBondDonors",
                typeof(HBondDonorCountDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <summary>
        /// The parameter of this HBondDonorCountDescriptor instance.
        /// </summary>
        public object[] Parameters
        {
            set
            {
                // this descriptor has no parameters; nothing has to be done here
            }
            get
            {
                // no parameters; thus we return null
                return null;
            }
        }

        public IReadOnlyList<string> DescriptorNames => NAMES;

        private IDescriptorValue GetDummyDescriptorValue(Exception e)
        {
            return new DescriptorValue<Result<int>>(_Specification, ParameterNames, Parameters, new Result<int>(0), DescriptorNames, e);
        }

        /// <summary>
        /// Calculates the number of H bond donors.
        /// </summary>
        /// <param name="atomContainer">AtomContainer</param>
        /// <returns>number of H bond donors</returns>
        public DescriptorValue<Result<int>> Calculate(IAtomContainer atomContainer)
        {
            int hBondDonors = 0;
            IAtomContainer ac = (IAtomContainer)atomContainer.Clone();

            //IAtom[] atoms = ac.GetAtoms();
            // iterate over all atoms of this AtomContainer; use label atomloop to allow for labelled continue
        
            //atomloop:
            for (int atomIndex = 0; atomIndex < ac.Atoms.Count; atomIndex++)
            {
                IAtom atom = (IAtom)ac.Atoms[atomIndex];
                // checking for O and N atoms where the formal charge is >= 0
                if ((atom.Symbol.Equals("O") || atom.Symbol.Equals("N")) && atom.FormalCharge >= 0)
                {
                    // implicit hydrogens
                    int implicitH = atom.ImplicitHydrogenCount ?? 0;
                    if (implicitH > 0)
                    {
                        hBondDonors++;
                        goto continue_atomloop; // we skip the explicit hydrogens part cause we found implicit hydrogens
                    }
                    // explicit hydrogens
                    var neighbours = ac.GetConnectedAtoms(atom);
                    foreach (var neighbour in neighbours)
                    {
                        if (((IAtom)neighbour).Symbol.Equals("H"))
                        {
                            hBondDonors++;
                            goto continue_atomloop;
                        }
                    }
                }
                continue_atomloop:
                    ;
            }

            return new DescriptorValue<Result<int>>(_Specification, ParameterNames, Parameters, new Result<int>(hBondDonors), DescriptorNames);
        }

        /// <inheritdoc/>
        public IDescriptorResult DescriptorResultType { get; } = new Result<int>(1);

        /// <summary>
        /// The parameterNames of the HBondDonorCountDescriptor.
        /// <see langword="null"/> as this descriptor does not have any parameters.
        /// </summary>
        public IReadOnlyList<string> ParameterNames => null; // no parameters; thus we return null

        /// <summary>
        /// Gets the parameterType of the HBondDonorCountDescriptor.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns><see langword="null"/> as this descriptor does not have any parameters</returns>
        public object GetParameterType(string name) => null; // no parameters; thus we return null

        IDescriptorValue IMolecularDescriptor.Calculate(IAtomContainer container) => Calculate(container);
    }
}
