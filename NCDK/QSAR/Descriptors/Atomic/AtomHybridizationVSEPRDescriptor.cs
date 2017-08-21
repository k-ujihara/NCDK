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
using NCDK.QSAR.Results;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    ///  This class returns the hybridization of an atom.
    /// </summary>
    /// <remarks>
    ///  <para>This class try to find a SIMPLE WAY the molecular geometry for following from
    ///    Valence Shell Electron Pair Repulsion or VSEPR model and at the same time its
    ///    hybridization of atoms in a molecule.</para>
    ///
    ///  <para>The basic premise of the model is that the electrons are paired in a molecule
    ///    and that the molecule geometry is determined only by the repulsion between the pairs.
    ///    The geometry adopted by a molecule is then the one in which the repulsions are minimized.</para>
    ///
    ///  <para>It counts the number of electron pairs in the Lewis dot diagram which
    ///   are attached to an atom. Then uses the following table.</para>
    /// 
    /// <list type="table">
    /// <listheader>
    ///    <term>pairs on an atom</term>
    ///    <term>hybridization of the atom</term>
    ///    <term>geometry</term>
    ///    <term>number for CDK.Constants</term>
    /// </listheader>
    ///   <item><term>2</term><term>sp</term><term>linear</term><term>1</term></item>
    ///   <item><term>3</term><term>sp^2</term><term>trigonal planar</term><term>2</term></item>
    ///   <item><term>4</term><term>sp^3</term><term>tetrahedral</term><term>3</term></item>
    ///   <item><term>5</term><term>sp^3d</term><term>trigonal bipyramid</term><term>4</term></item>
    ///   <item><term>6</term><term>sp^3d^2</term><term>octahedral</term><term>5</term></item>
    ///   <item><term>7</term><term>sp^3d^3</term><term>pentagonal bipyramid</term><term>6</term></item>
    ///   <item><term>8</term><term>sp^3d^4</term><term>square antiprim</term><term>7</term></item>
    ///   <item><term>9</term><term>sp^3d^5</term><term>tricapped trigonal prism</term><term>8</term></item>
    /// </list> 
    ///
    ///  <para>This table only works if the central atom is a p-block element
    ///   (groups IIA through VIIIA), not a transition metal.</para>
    ///
    /// <para>
    /// This descriptor uses these parameters:
    /// <list type="table">
    /// <listheader><term>Name</term><term>Default</term><term>Description</term></listheader>
    /// <item><term></term><term></term><term>no parameters</term></item>
    /// </list>
    /// </para> 
    /// </remarks>
    // @author         Miguel Rojas
    // @cdk.created    2005-03-24
    // @cdk.module     qsaratomic
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:atomHybridizationVSEPR
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
        /// <param name="atom">The <see cref="IAtom"/> for which the <see cref="DescriptorValue"/> is requested</param>
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
