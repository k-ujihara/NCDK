/* Copyright (C) 2004-2007  Miguel Rojas <miguel.rojas@uni-koeln.de>
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
using NCDK.AtomTypes;
using NCDK.QSAR.Result;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /**
     *  This class returns the hybridization of an atom.
     *
     *  <p>This class try to find a SIMPLE WAY the molecular geometry for following from
     *    Valence Shell Electron Pair Repulsion or VSEPR model and at the same time its
     *    hybridization of atoms in a molecule.
     *
     *  <p>The basic premise of the model is that the electrons are paired in a molecule
     *    and that the molecule geometry is determined only by the repulsion between the pairs.
     *    The geometry adopted by a molecule is then the one in which the repulsions are minimized.
     *
     *  <p>It counts the number of electron pairs in the Lewis dot diagram which
     *   are attached to an atom. Then uses the following table.
     * <pre>
     * <table border="1">
     *   <tr>
     * 	  <td>pairs on an atom</td>
     *    <td>hybridization of the atom</td>
     *    <td>geometry</td>
     *    <td>number for CDK.Constants</td>
     *   </tr>
     *   <tr><td>2</td><td>sp</td><td>linear</td><td>1</td></tr>
     *   <tr><td>3</td><td>sp^2</td><td>trigonal planar</td><td>2</td></tr>
     *   <tr><td>4</td><td>sp^3</td><td>tetrahedral</td><td>3</td></tr>
     *   <tr><td>5</td><td>sp^3d</td><td>trigonal bipyramid</td><td>4</td></tr>
     *   <tr><td>6</td><td>sp^3d^2</td><td>octahedral</td><td>5</td></tr>
     *   <tr><td>7</td><td>sp^3d^3</td><td>pentagonal bipyramid</td><td>6</td></tr>
     *   <tr><td>8</td><td>sp^3d^4</td><td>square antiprim</td><td>7</td></tr>
     *   <tr><td>9</td><td>sp^3d^5</td><td>tricapped trigonal prism</td><td>8</td></tr>
     * </table>
     * </pre>
     *
     *  <p>This table only works if the central atom is a p-block element
     *   (groups IIA through VIIIA), not a transition metal.
     *
     *
     * <p>This descriptor uses these parameters:
     * <table border="1">
     *   <tr>
     *     <td>Name</td>
     *     <td>Default</td>
     *     <td>Description</td>
     *   </tr>
     *   <tr>
     *     <td></td>
     *     <td></td>
     *     <td>no parameters</td>
     *   </tr>
     * </table>
     *
     *@author         Miguel Rojas
     *@cdk.created    2005-03-24
     *@cdk.module     qsaratomic
     * @cdk.githash
     *@cdk.set        qsar-descriptors
     * @cdk.dictref qsar-descriptors:atomHybridizationVSEPR
     */
    public class AtomHybridizationVSEPRDescriptor : AbstractAtomicDescriptor, IAtomicDescriptor
    {
        /// <summary>
        ///  Constructor for the AtomHybridizationVSEPRDescriptor object
        /// </summary>
        public AtomHybridizationVSEPRDescriptor() { }

        /// <summary>
        /// The specification attribute of the AtomHybridizationVSEPRDescriptor object
        /// </summary>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#atomHybridizationVSEPR",
                typeof(AtomHybridizationVSEPRDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <summary>
        ///  The parameters attribute of the AtomHybridizationVSEPRDescriptor object
        /// </summary>
        public override object[] Parameters { get { return null; } set { } }

        public override string[] DescriptorNames { get; } = new string[] { "hybr" };

        /// <summary>
        ///  This method calculates the hybridization of an atom.
        /// </summary>
        /// <param name="atom">The IAtom for which the DescriptorValue is requested</param>
        /// <param name="container">Parameter is the atom container.</param>
        /// <returns>The hybridization</returns>
        public override DescriptorValue Calculate(IAtom atom, IAtomContainer container)
        {
            IAtomType atomType;
            try
            {
                atomType = CDKAtomTypeMatcher.GetInstance(atom.Builder).FindMatchingAtomType(container, atom);
            }
            catch (CDKException)
            {
                return new DescriptorValue(_Specification, ParameterNames, Parameters, new IntegerResult(0), // does that work??
                        DescriptorNames, new CDKException("Atom type was null"));
            }
            if (atomType == null)
            {
                return new DescriptorValue(_Specification, ParameterNames, Parameters, new IntegerResult(0), // does that work??
                        DescriptorNames, new CDKException("Atom type was null"));
            }
            if (atomType.Hybridization == Hybridization.Unset)
            {
                return new DescriptorValue(_Specification, ParameterNames, Parameters, new IntegerResult(0), // does that work??
                        DescriptorNames, new CDKException("Hybridization was null"));
            }
            int hybridizationCDK = atomType.Hybridization.Ordinal;

            return new DescriptorValue(_Specification, ParameterNames, Parameters, new IntegerResult(
                    hybridizationCDK), DescriptorNames);
        }

        /// <summary>
        /// The parameterNames attribute of the AtomHybridizationVSEPRDescriptor object
        /// </summary>
        public override string[] ParameterNames { get; } = new string[0];

        /// <summary>
        ///  Gets the parameterType attribute of the AtomHybridizationVSEPRDescriptor object
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>An Object of class equal to that of the parameter being requested</returns>
        public override object GetParameterType(string name) => null;
    }
}
