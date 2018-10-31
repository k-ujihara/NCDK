/*  Copyright (C) 2004-2007  Rajarshi Guha <rajarshi@users.sourceforge.net>
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

using NCDK.Isomorphisms.Matchers;
using NCDK.QSAR.Results;
using NCDK.Smiles;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Evaluates chi cluster descriptors.
    /// </summary>
    /// <remarks>
    /// The code currently evluates the simple and valence chi chain descriptors of orders 3, 4,5 and 6.
    /// It utilizes the graph isomorphism code of the CDK to find fragments matching
    /// SMILES strings representing the fragments corresponding to each type of chain.
    /// <para>
    /// The order of the values returned is
    /// <list type="bullet">
    /// <item>SC-3 - Simple cluster, order 3</item>
    /// <item>SC-4 - Simple cluster, order 4</item>
    /// <item>SC-5 - Simple cluster, order 5</item>
    /// <item>SC-6 - Simple cluster, order 6</item>
    /// <item>VC-3 - Valence cluster, order 3</item>
    /// <item>VC-4 - Valence cluster, order 4</item>
    /// <item>VC-5 - Valence cluster, order 5</item>
    /// <item>VC-6 - Valence cluster, order 6</item>
    /// </list>
    /// </para>
    /// <para>
    /// <b>Note</b>: These descriptors are calculated using graph isomorphism to identify
    /// the various fragments. As a result calculations may be slow. In addition, recent
    /// versions of Molconn-Z use simplified fragment definitions (i.e., rings without
    /// branches etc.) whereas these descriptors use the older more complex fragment
    /// definitions.
    /// </para>
    /// </remarks>
    // @author Rajarshi Guha
    // @cdk.created 2006-11-13
    // @cdk.module qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:chiCluster
    // @cdk.keyword chi cluster index
    // @cdk.keyword descriptor
    public class ChiClusterDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private static readonly string[] NAMES = { "SC-3", "SC-4", "SC-5", "SC-6", "VC-3", "VC-4", "VC-5", "VC-6" };

        public ChiClusterDescriptor() { }

        public override IImplementationSpecification Specification => specification;
        private static readonly DescriptorSpecification specification =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#chiCluster",
                typeof(ChiClusterDescriptor).FullName,
                "The Chemistry Development Kit");

        public override IReadOnlyList<string> ParameterNames => null;
        public override object GetParameterType(string name) => null;
        public override IReadOnlyList<string> DescriptorNames => NAMES;
        public override IReadOnlyList<object> Parameters { get { return null; } set { } }

        private DescriptorValue<ArrayResult<double>> GetDummyDescriptorValue(Exception e)
        {
            var ndesc = DescriptorNames.Count;
            var results = new ArrayResult<double>(ndesc);
            for (int i = 0; i < ndesc; i++)
                results.Add(double.NaN);
            return new DescriptorValue<ArrayResult<double>>(specification, ParameterNames, Parameters, results, DescriptorNames, e);
        }

        public DescriptorValue<ArrayResult<double>> Calculate(IAtomContainer container)
        {
            // removeHydrogens does a deep copy, so no need to clone
            var localAtomContainer = AtomContainerManipulator.RemoveHydrogens(container);
            var matcher = CDK.AtomTypeMatcher;
            foreach (var atom in localAtomContainer.Atoms)
            {
                IAtomType type;
                try
                {
                    type = matcher.FindMatchingAtomType(localAtomContainer, atom);
                    AtomTypeManipulator.Configure(atom, type);
                }
                catch (Exception e)
                {
                    return GetDummyDescriptorValue(new CDKException("Error in atom typing: " + e.Message));
                }
            }
            var hAdder = CDK.HydrogenAdder;
            try
            {
                hAdder.AddImplicitHydrogens(localAtomContainer);
            }
            catch (CDKException e)
            {
                return GetDummyDescriptorValue(new CDKException("Error in hydrogen addition: " + e.Message));
            }

            var subgraph3 = Order3(localAtomContainer);
            var subgraph4 = Order4(localAtomContainer);
            var subgraph5 = Order5(localAtomContainer);
            var subgraph6 = Order6(localAtomContainer);

            var order3s = ChiIndexUtils.EvalSimpleIndex(localAtomContainer, subgraph3);
            var order4s = ChiIndexUtils.EvalSimpleIndex(localAtomContainer, subgraph4);
            var order5s = ChiIndexUtils.EvalSimpleIndex(localAtomContainer, subgraph5);
            var order6s = ChiIndexUtils.EvalSimpleIndex(localAtomContainer, subgraph6);

            double order3v, order4v, order5v, order6v;
            try
            {
                order3v = ChiIndexUtils.EvalValenceIndex(localAtomContainer, subgraph3);
                order4v = ChiIndexUtils.EvalValenceIndex(localAtomContainer, subgraph4);
                order5v = ChiIndexUtils.EvalValenceIndex(localAtomContainer, subgraph5);
                order6v = ChiIndexUtils.EvalValenceIndex(localAtomContainer, subgraph6);
            }
            catch (CDKException e)
            {
                return GetDummyDescriptorValue(new CDKException("Error in substructure search: " + e.Message));
            }
            ArrayResult<double> retval = new ArrayResult<double>
            {
                order3s,
                order4s,
                order5s,
                order6s,
                order3v,
                order4v,
                order5v,
                order6v,
            };

            return new DescriptorValue<ArrayResult<double>>(specification, ParameterNames, Parameters, retval, DescriptorNames);
        }

        /// <inheritdoc/>
        public override IDescriptorResult DescriptorResultType { get; } = new ArrayResult<double>(8);

        private List<List<int>> Order3(IAtomContainer atomContainer)
        {
            var queries = new QueryAtomContainer[1];
            try
            {
                queries[0] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(CDK.SilentSmilesParser.ParseSmiles("C(C)(C)(C)"), false);
            }
            catch (InvalidSmilesException e)
            {
                Console.Error.WriteLine(e.StackTrace); //To change body of catch statement use File | Settings | File Templates.
            }
            return ChiIndexUtils.GetFragments(atomContainer, queries);
        }

        private List<List<int>> Order4(IAtomContainer atomContainer)
        {
            var queries = new QueryAtomContainer[1];
            try
            {
                queries[0] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(CDK.SilentSmilesParser.ParseSmiles("C(C)(C)(C)(C)"), false);
            }
            catch (InvalidSmilesException e)
            {
                Console.Error.WriteLine(e.StackTrace); //To change body of catch statement use File | Settings | File Templates.
            }
            return ChiIndexUtils.GetFragments(atomContainer, queries);
        }

        private List<List<int>> Order5(IAtomContainer atomContainer)
        {
            var queries = new QueryAtomContainer[1];
            try
            {
                queries[0] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(CDK.SilentSmilesParser.ParseSmiles("CC(C)C(C)(C)"), false);
            }
            catch (InvalidSmilesException e)
            {
                Console.Error.WriteLine(e.StackTrace); //To change body of catch statement use File | Settings | File Templates.
            }
            return ChiIndexUtils.GetFragments(atomContainer, queries);
        }

        private List<List<int>> Order6(IAtomContainer atomContainer)
        {
            var queries = new QueryAtomContainer[1];
            try
            {
                queries[0] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(CDK.SilentSmilesParser.ParseSmiles("CC(C)C(C)(C)C"), false);
            }
            catch (InvalidSmilesException e)
            {
                Console.Error.WriteLine(e.StackTrace); //To change body of catch statement use File | Settings | File Templates.
            }
            return ChiIndexUtils.GetFragments(atomContainer, queries);
        }

        IDescriptorValue IMolecularDescriptor.Calculate(IAtomContainer container) => Calculate(container);
    }
}
