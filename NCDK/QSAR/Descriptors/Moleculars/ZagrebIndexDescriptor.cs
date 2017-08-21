/* Copyright (C) 2004-2007  Rajarshi Guha <rajarshi@users.sourceforge.net>
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

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Zagreb index: the sum of the squares of atom degree over all heavy atoms i.
    /// </summary>
    // @author      mfe4
    // @cdk.created 2004-11-03
    // @cdk.module  qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:zagrebIndex
    // @cdk.keyword Zagreb index
    // @cdk.keyword descriptor
    public class ZagrebIndexDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private static readonly string[] NAMES = { "Zagreb" };

        /// <summary>
        ///  Constructor for the ZagrebIndexDescriptor object.
        /// </summary>
        public ZagrebIndexDescriptor() { }

        /// <summary>
        /// The specification attribute of the ZagrebIndexDescriptor object.
        /// </summary>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#zagrebIndex",
                typeof(ZagrebIndexDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the ZagrebIndexDescriptor object.
        /// </summary>
        public override object[] Parameters { get { return null; } set { } }

        public override string[] DescriptorNames => NAMES;

        /// <summary>
        ///  Evaluate the Zagreb Index for a molecule.
        /// </summary>
        /// <param name="atomContainer">AtomContainer</param>
        /// <returns> zagreb index</returns>
        public override DescriptorValue Calculate(IAtomContainer atomContainer)
        {
            double zagreb = 0;
            foreach (var atom in atomContainer.Atoms)
            {
                if (atom.Symbol.Equals("H")) continue;
                int atomDegree = 0;
                var neighbours = atomContainer.GetConnectedAtoms(atom);
                foreach (var neighbour in neighbours)
                {
                    if (!neighbour.Symbol.Equals("H"))
                    {
                        atomDegree += 1;
                    }
                }
                zagreb += (atomDegree * atomDegree);
            }
            return new DescriptorValue(_Specification, ParameterNames, Parameters, new DoubleResult(zagreb), DescriptorNames);
        }

        /// <inheritdoc/>
        public override IDescriptorResult DescriptorResultType { get; } = new DoubleResult(0.0);

        /// <summary>
        /// The parameterNames attribute of the ZagrebIndexDescriptor object.
        /// </summary>
        public override string[] ParameterNames => null;

        /// <summary>
        /// The parameterType attribute of the ZagrebIndexDescriptor object.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public override object GetParameterType(string name) => null;
    }
}
