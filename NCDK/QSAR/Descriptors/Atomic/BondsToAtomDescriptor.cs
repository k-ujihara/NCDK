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
using NCDK.Graphs;
using NCDK.QSAR.Results;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    ///  This class returns the number of bonds on the shortest path between two atoms.
    /// </summary>
    /// <remarks>
    /// This descriptor uses these parameters:
    /// <list type="table">
    ///   <item>
    ///     <term>Name</term>
    ///     <term>Default</term>
    ///     <term>Description</term>
    ///   </item>
    ///   <item>
    ///     <term>focusPosition</term>
    ///     <term>0</term>
    ///     <term>The position of the second atom</term>
    ///   </item>
    /// </list> 
    /// </remarks>
    // @author         mfe4
    // @cdk.created    2004-11-13
    // @cdk.module     qsaratomic
    // @cdk.githash
    // @cdk.dictref    qsar-descriptors:bondsToAtom
    public class BondsToAtomDescriptor : AbstractAtomicDescriptor, IAtomicDescriptor
    {
        private int focusPosition = 0;

        /// <summary>
        ///  Constructor for the BondsToAtomDescriptor object
        /// </summary>
        public BondsToAtomDescriptor() { }

        /// <summary>
        /// The specification attribute of the BondsToAtomDescriptor object
        /// </summary>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#bondsToAtom",
                typeof(BondsToAtomDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the BondsToAtomDescriptor object
        /// </summary>
        /// <exception cref="CDKException">Description of the Exception</exception>
        /// <param name="value">The parameter is the position to focus</param>
        public override object[] Parameters
        {
            set
            {
                if (value.Length > 1)
                {
                    throw new CDKException("BondsToAtomDescriptor only expects one parameters");
                }
                if (!(value[0] is int))
                {
                    throw new CDKException("The parameter must be of type int");
                }
                focusPosition = (int)value[0];
            }
            get
            {
                return new object[] { focusPosition };
            }
        }

        public override string[] DescriptorNames { get; } = new string[] { "bondsToAtom" };

        /// <summary>
        ///  This method calculate the number of bonds on the shortest path between two atoms.
        /// </summary>
        /// <param name="atom">The <see cref="IAtom"/> for which the <see cref="DescriptorValue"/> is requested</param>
        /// <param name="container">Parameter is the atom container.</param>
        /// <returns>The number of bonds on the shortest path between two atoms</returns>
        public override DescriptorValue Calculate(IAtom atom, IAtomContainer container)
        {
            IAtom focus = container.Atoms[focusPosition];

            // could be cached
            int bondsToAtom = new ShortestPaths(container, atom).GetDistanceTo(focus);

            return new DescriptorValue(_Specification, ParameterNames, Parameters, new Result<int>(
                    bondsToAtom), DescriptorNames);
        }

        /// <summary>
        /// The parameterNames attribute of the BondsToAtomDescriptor object
        /// </summary>
        /// <returns>The parameterNames value</returns>
        public override string[] ParameterNames { get; } = new string[] { "focusPosition" };

        /// <summary>
        ///  Gets the parameterType attribute of the BondsToAtomDescriptor object
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public override object GetParameterType(string name) => 0;
    }
}
