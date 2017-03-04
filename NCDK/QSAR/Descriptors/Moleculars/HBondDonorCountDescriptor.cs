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
using NCDK.QSAR.Result;
using System;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// This descriptor calculates the number of hydrogen bond donors using a slightly simplified version of the
    /// <a href="http://www.chemie.uni-erlangen.de/model2001/abstracts/rester.html">PHACIR atom types</a>.
    /// The following groups are counted as hydrogen bond donors:
    /// <ul>
    /// <li>Any-OH where the formal charge of the oxygen is non-negative (i.e. formal charge >= 0)</li>
    /// <li>Any-NH where the formal charge of the nitrogen is non-negative (i.e. formal charge >= 0)</li>
    /// </ul>
    /// <p>
    /// This descriptor uses no parameters.
    /// <p>
    /// This descriptor works properly with AtomContainers whose atoms contain either <b>implicit</b> or <b>explicit
    /// hydrogen</b> atoms. It does not work with atoms that contain neither implicit nor explicit hydrogens.
    ///
    /// Returns a single value named <i>nHBDon</i>.
    ///
    /// <p>This descriptor uses these parameters:
    /// <table border="1">
    ///   <tr>
    ///     <td>Name</td>
    ///     <td>Default</td>
    ///     <td>Description</td>
    ///   </tr>
    ///   <tr>
    ///     <td></td>
    ///     <td></td>
    ///     <td>no parameters</td>
    ///   </tr>
    /// </table>
    ///
    // @author      ulif
    // @cdk.created 2005-22-07
    // @cdk.module  qsarmolecular
    // @cdk.githash
    // @cdk.set     qsar-descriptors
    // @cdk.dictref qsar-descriptors:hBondDonors
    /// </summary>
    public class HBondDonorCountDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private static readonly string[] NAMES = { "nHBDon" };

        /// <summary>
        ///  Constructor for the HBondDonorCountDescriptor object
        /// </summary>
        public HBondDonorCountDescriptor() { }

        /// <summary>
        /// The specification attribute of the HBondDonorCountDescriptor object
        /// </summary>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
         new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#hBondDonors",
                typeof(HBondDonorCountDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <summary>
        /// The parameter of this HBondDonorCountDescriptor instance.
        /// </summary>
        public override object[] Parameters
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

        public override string[] DescriptorNames => NAMES;

        private DescriptorValue GetDummyDescriptorValue(Exception e)
        {
            return new DescriptorValue(_Specification, ParameterNames, Parameters, new IntegerResult(0), DescriptorNames, e);
        }

        /// <summary>
        /// Calculates the number of H bond donors.
        /// </summary>
        /// <param name="atomContainer">AtomContainer</param>
        /// <returns>number of H bond donors</returns>
        public override DescriptorValue Calculate(IAtomContainer atomContainer)
        {
            int hBondDonors = 0;
            IAtomContainer ac = (IAtomContainer)atomContainer.Clone();

            //IAtom[] atoms = ac.GetAtoms();
            // iterate over all atoms of this AtomContainer; use label atomloop to allow for labelled continue
            atomloop: for (int atomIndex = 0; atomIndex < ac.Atoms.Count; atomIndex++)
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

            return new DescriptorValue(_Specification, ParameterNames, Parameters, new IntegerResult(hBondDonors), DescriptorNames);
        }

        /// <summary>
        /// Returns the specific type of the DescriptorResult object.
        /// <para>
        /// The return value from this method really indicates what type of result will
        /// be obtained from the <see cref="DescriptorValue"/> object. Note that the same result
        /// can be achieved by interrogating the <see cref="DescriptorValue"/> object; this method
        /// allows you to do the same thing, without actually calculating the descriptor.</para>
        /// </summary>
        public override IDescriptorResult DescriptorResultType { get; } = new IntegerResult(1);

        /// <summary>
        /// The parameterNames of the HBondDonorCountDescriptor.
        /// <see langword="null"/> as this descriptor does not have any parameters.
        /// </summary>
        public override string[] ParameterNames => null; // no parameters; thus we return null

        /// <summary>
        /// Gets the parameterType of the HBondDonorCountDescriptor.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns><see langword="null"/> as this descriptor does not have any parameters</returns>
        public override object GetParameterType(string name) => null; // no parameters; thus we return null
    }
}
