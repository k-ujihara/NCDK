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
using NCDK.QSAR.Result;
using NCDK.Tools.Manipulator;
using System;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /**
     * This descriptor calculates the number of hydrogen bond acceptors using a slightly simplified version of the
     * <a href="http://www.chemie.uni-erlangen.de/model2001/abstracts/rester.html">PHACIR atom types</a>.
     * The following groups are counted as hydrogen bond acceptors:
     * <ul>
     * <li>any oxygen where the formal charge of the oxygen is non-positive (i.e. formal charge <= 0) <b>except</b></li>
     * <ol>
     * <li>an aromatic ether oxygen (i.e. an ether oxygen that is adjacent to at least one aromatic carbon)</li>
     * <li>an oxygen that is adjacent to a nitrogen</li>
     * </ol>
     * <li>any nitrogen where the formal charge of the nitrogen is non-positive (i.e. formal charge <= 0) <b>except</b></li>
     * <ol>
     * <li>a nitrogen that is adjacent to an oxygen</li>
     * </ol>
     * </ul>
     *
     * Returns a single value named <i>nHBAcc</i>.
     *
     * <p>This descriptor uses these parameters:
     * <table>
     *   <tr>
     *     <td>Name</td>
     *     <td>Default</td>
     *     <td>Description</td>
     *   </tr>
     *   <tr>
     *     <td>checkAromaticity</td>
     *     <td>false</td>
     *     <td>true if the aromaticity has to be checked</td>
     *   </tr>
     * </table>
     * <p>
     * This descriptor works properly with AtomContainers whose atoms contain <b>implicit hydrogens</b> or <b>explicit
     * hydrogens</b>.
     *
     * @author      ulif
     * @cdk.created 2005-22-07
     * @cdk.module  qsarmolecular
     * @cdk.githash
     * @cdk.set     qsar-descriptors
     * @cdk.dictref qsar-descriptors:hBondacceptors
     */
    public class HBondAcceptorCountDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {

        // only parameter of this descriptor; true if aromaticity has to be checked prior to descriptor calculation, false otherwise
        private bool checkAromaticity = false;
        private static readonly string[] NAMES = { "nHBAcc" };

        /// <summary>
        ///  Constructor for the HBondAcceptorCountDescriptor object
        /// </summary>
        public HBondAcceptorCountDescriptor() { }

        /// <summary>
        /// Gets the specification attribute of the HBondAcceptorCountDescriptor object.
        ///
        /// <returns>   The specification value</returns>
        /// </summary>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
         new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#hBondacceptors",
                typeof(HBondAcceptorCountDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the HBondAcceptorCountDescriptor object.
        ///
        // @param  params            a bool true means that aromaticity has to be checked
        // @exception  CDKException  Description of the Exception
        /// </summary>
        public override object[] Parameters
        {
            set
            {
                if (value.Length != 1)
                {
                    throw new CDKException("HBondAcceptorCountDescriptor expects a single parameter");
                }
                if (!(value[0] is bool))
                {
                    throw new CDKException("The parameter must be of type bool");
                }
                // ok, all should be fine
                checkAromaticity = (bool)value[0];
            }
            get
            {
                return new object[] { checkAromaticity };
                // return the parameters as used for the descriptor calculation
            }
        }

        public override string[] DescriptorNames => NAMES;

        private DescriptorValue GetDummyDescriptorValue(Exception e)
        {
            return new DescriptorValue(_Specification, ParameterNames, Parameters, new IntegerResult(0), DescriptorNames, e);
        }

        /// <summary>
        ///  Calculates the number of H bond acceptors.
        /// </summary>
        /// <param name="atomContainer">AtomContainer</param>
        /// <returns>                  number of H bond acceptors</returns>
        public override DescriptorValue Calculate(IAtomContainer atomContainer)
        {
            int hBondAcceptors = 0;
            IAtomContainer ac = (IAtomContainer)atomContainer.Clone();

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
            atomloop: foreach (var atom in ac.Atoms)
            {
                // looking for suitable nitrogen atoms
                if (atom.Symbol.Equals("N") && atom.FormalCharge <= 0)
                {

                    // excluding nitrogens that are adjacent to an oxygen
                    var bonds = ac.GetConnectedBonds(atom);
                    int nPiBonds = 0;
                    foreach (var bond in bonds)
                    {
                        if (bond.GetConnectedAtom(atom).Symbol.Equals("O")) goto continue_atomloop;
                        if (BondOrder.Double.Equals(bond.Order)) nPiBonds++;
                    }

                    // if the nitrogen is aromatic and there are no pi bonds then it's
                    // lone pair cannot accept any hydrogen bonds
                    if (atom.IsAromatic && nPiBonds == 0) continue;

                    hBondAcceptors++;
                }
                // looking for suitable oxygen atoms
                else if (atom.Symbol.Equals("O") && atom.FormalCharge <= 0)
                {
                    //excluding oxygens that are adjacent to a nitrogen or to an aromatic carbon
                    var neighbours = ac.GetConnectedAtoms(atom);
                    foreach (var neighbour in neighbours)
                        if (neighbour.Symbol.Equals("N")
                                || (neighbour.Symbol.Equals("C") && neighbour.IsAromatic))
                            goto continue_atomloop;
                    hBondAcceptors++;
                }
                continue_atomloop:
                ;
            }

            return new DescriptorValue(_Specification, ParameterNames, Parameters, new IntegerResult(hBondAcceptors), DescriptorNames);
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
        /// The parameterNames attribute of the HBondAcceptorCountDescriptor object.
        /// </summary>
        public override string[] ParameterNames { get; } = new string[] { "checkAromaticity" };

        /// <summary>
        /// Gets the parameterType attribute of the HBondAcceptorCountDescriptor object.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public override object GetParameterType(string name) => false;
    }
}
