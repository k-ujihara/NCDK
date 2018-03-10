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
using NCDK.Tools.Manipulator;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    ///  According to the Petitjean definition, the eccentricity of a vertex corresponds to
    ///  the distance from that vertex to the most remote vertex in the graph.
    ///  The distance is obtained from the distance matrix as the count of edges between the two vertices.
    ///  If r(i) is the largest matrix entry in row i of the distance matrix D, then the radius is defined as the smallest of the r(i).
    ///  The graph diameter D is defined as the largest vertex eccentricity in the graph.
    ///  (http://www.edusoft-lc.com/molconn/manuals/400/chaptwo.html)
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
    ///     <term></term>
    ///     <term></term>
    ///     <term>no parameters</term>
    ///   </item>
    /// </list>
    ///
    /// Returns a single value named <i>PetitjeanNumber</i>.
    /// </para>
    /// </remarks>
    // @author         mfe4
    // @cdk.created    December 7, 2004
    // @cdk.created    2004-11-03
    // @cdk.module     qsarmolecular
    // @cdk.githash
    // @cdk.dictref    qsar-descriptors:petitjeanNumber
    // @cdk.keyword    Petit-Jean, number
    public class PetitjeanNumberDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private static readonly string[] NAMES = { "PetitjeanNumber" };

        /// <summary>
        ///  Constructor for the PetitjeanNumberDescriptor object
        /// </summary>
        public PetitjeanNumberDescriptor() { }

        /// <summary>
        /// The specification attribute of the PetitjeanNumberDescriptor object
        /// </summary>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#petitjeanNumber",
                typeof(PetitjeanNumberDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the PetitjeanNumberDescriptor object
        /// </summary>
        public override object[] Parameters { get { return null; } set { } }

        public override IReadOnlyList<string> DescriptorNames => NAMES;

        /// <summary>
        /// Evaluate the descriptor for the molecule.
        /// </summary>
        /// <param name="atomContainer">AtomContainer</param>
        /// <returns>petitjean number</returns>
        public DescriptorValue<Result<double>> Calculate(IAtomContainer atomContainer)
        {
            IAtomContainer cloneContainer = AtomContainerManipulator.RemoveHydrogens(atomContainer);
            double petitjeanNumber; //weinerPath
            int diameter = PathTools.GetMolecularGraphDiameter(cloneContainer);
            int radius = PathTools.GetMolecularGraphRadius(cloneContainer);

            if (diameter == 0)
                petitjeanNumber = 0;
            else
                petitjeanNumber = (diameter - radius) / (double)diameter;
            return new DescriptorValue<Result<double>>(_Specification, ParameterNames, Parameters, new Result<double>(petitjeanNumber), DescriptorNames);
        }

        /// <inheritdoc/>
        public override IDescriptorResult DescriptorResultType { get; } = new Result<double>(0.0);

        /// <summary>
        /// The parameterNames attribute of the PetitjeanNumberDescriptor object
        /// </summary>
        public override IReadOnlyList<string> ParameterNames => null;

        /// <summary>
        /// The parameterType attribute of the PetitjeanNumberDescriptor object
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object GetParameterType(string name) => null;

        IDescriptorValue IMolecularDescriptor.Calculate(IAtomContainer container) => Calculate(container);
    }
}
