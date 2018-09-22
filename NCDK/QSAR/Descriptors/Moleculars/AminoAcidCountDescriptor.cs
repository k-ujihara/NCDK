/* Copyright (C) 2006-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using NCDK.Isomorphisms;
using NCDK.Isomorphisms.MCSS;
using NCDK.QSAR.Results;
using NCDK.Templates;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Class that returns the number of each amino acid in an atom container.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This descriptor uses these parameters:
    /// <list type="table">
    /// <listheader><term>Name</term><term>Default</term><term>Description</term></listheader>
    /// <item><term></term><term></term><term>no parameters</term></item>
    /// </list>
    /// Returns 20 values with names of the form <i>nX</i>, where <i>X</i> is the short versio
    /// of the amino acid name
    /// </para>
    /// </remarks>
    // @author      egonw
    // @cdk.created 2006-01-15
    // @cdk.module  qsarprotein
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:aminoAcidsCount
    public class AminoAcidCountDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private IChemObjectSet<IAtomContainer> substructureSet;

        private static string[] names;

        public AminoAcidCountDescriptor()
        {
            var aas = AminoAcids.Proteinogenics;
            substructureSet = aas[0].Builder.NewAtomContainerSet();
            foreach (var aa in aas)
            {
                substructureSet.Add(aa);
            }

            names = new string[substructureSet.Count];
            for (int i = 0; i < aas.Count; i++)
                names[i] = "n" + aas[i].GetProperty<string>(AminoAcids.ResidueNameShortKey);
        }

        /// <inheritdoc/>
        public override IImplementationSpecification Specification => specification;
        private static readonly DescriptorSpecification specification =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#aminoAcidsCount",
                typeof(AminoAcidCountDescriptor).FullName,
                "The Chemistry Development Kit");
        
        public override IReadOnlyList<object> Parameters
        {
            get { return null; }
            set { } // no parameters exist
        }

        public override IReadOnlyList<string> DescriptorNames => names;

        /// <summary>
        /// Determine the number of amino acids groups the supplied <see cref="IAtomContainer"/>.
        /// </summary>
        /// <param name="ac">The <see cref="IAtomContainer"/> for which this descriptor is to be calculated</param>
        /// <returns>the number of aromatic atoms of this AtomContainer</returns>
        /// <seealso cref="Parameters"/>
        public DescriptorValue<ArrayResult<int>> Calculate(IAtomContainer ac)
        {
            ac = Clone(ac); // don't modify input
            var resultLength = substructureSet.Count;
            var results = new ArrayResult<int>(resultLength);

            var universalIsomorphismTester = new UniversalIsomorphismTester();
            IAtomContainer substructure;
            for (int i = 0; i < resultLength; i++)
            {
                substructure = substructureSet[i];
                IEnumerable<IReadOnlyList<RMap>> maps = null;
                try
                {
                    maps = universalIsomorphismTester.GetSubgraphMaps(ac, substructure);
                }
                catch (CDKException e)
                {
                    for (int j = 0; j < resultLength; j++)
                        results.Add(0);    // TODO: original code is (int)double.NaN.
                    return new DescriptorValue<ArrayResult<int>>(specification, ParameterNames, Parameters, results, DescriptorNames, new CDKException("Error in substructure search: " + e.Message));
                }
                if (maps != null)
                {
                    results.Add(maps.Count());
                }
            }

            return new DescriptorValue<ArrayResult<int>>(specification, ParameterNames, Parameters, results, DescriptorNames);
        }

        /// <summary>
        /// Returns the specific type of the DescriptorResult object.
        /// </summary>
        /// <remarks>
        /// The return value from this method really indicates what type of result will
        /// be obtained from the <see cref="IDescriptorValue"/> object. Note that the same result
        /// can be achieved by interrogating the <see cref="IDescriptorValue"/> object; this method
        /// allows you to do the same thing, without actually calculating the descriptor.
        /// </remarks>
        public override IDescriptorResult DescriptorResultType => new ArrayResult<int>(20);

        /// <summary>
        /// Gets the parameterNames attribute of the AromaticAtomsCountDescriptor object.
        /// </summary>
        public override IReadOnlyList<string> ParameterNames => System.Array.Empty<string>();

        public override object GetParameterType(string name)
        {
            return null;
        }

        IDescriptorValue IMolecularDescriptor.Calculate(IAtomContainer container) => Calculate(container);
    }
}
