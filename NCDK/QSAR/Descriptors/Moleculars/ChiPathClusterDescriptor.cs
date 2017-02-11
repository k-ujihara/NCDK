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
using NCDK.AtomTypes;

using NCDK.Isomorphisms.Matchers;
using NCDK.QSAR.Result;
using NCDK.Smiles;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /**
     * Evaluates chi path cluster descriptors.
     * <p/>
     * The code currently evluates the simple and valence chi chain descriptors of orders 3, 4,5 and 6.
     * It utilizes the graph isomorphism code of the CDK to find fragments matching
     * SMILES strings representing the fragments corresponding to each type of chain.
     * <p/>
     * The order of the values returned is
     * <ol>
     * <li>SPC-4 - Simple path cluster, order 4
     * <li>SPC-5 - Simple path cluster, order 5
     * <li>SPC-6 - Simple path cluster, order 6
     * <li>VPC-4 - Valence path cluster, order 4
     * <li>VPC-5 - Valence path cluster, order 5
     * <li>VPC-6 - Valence path cluster, order 6
     * </ol>
     * <p/>
     * <p/>
     * <b>Note</b>: These descriptors are calculated using graph isomorphism to identify
     * the various fragments. As a result calculations may be slow. In addition, recent
     * versions of Molconn-Z use simplified fragment definitions (i.e., rings without
     * branches etc.) whereas these descriptors use the older more complex fragment
     * definitions.
     *
     * @author Rajarshi Guha
     * @cdk.created 2006-11-13
     * @cdk.module qsarmolecular
     * @cdk.githash
     * @cdk.set qsar-descriptors
     * @cdk.dictref qsar-descriptors:chiPathCluster
     * @cdk.keyword chi path cluster index
     * @cdk.keyword descriptor
     */
    public class ChiPathClusterDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private SmilesParser sp;

        private static readonly string[] NAMES = { "SPC-4", "SPC-5", "SPC-6", "VPC-4", "VPC-5", "VPC-6" };

        public ChiPathClusterDescriptor() { }

        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
         new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#chiPathCluster",
                typeof(ChiPathClusterDescriptor).FullName,
                "The Chemistry Development Kit");

        public override string[] ParameterNames => null;
        public override object GetParameterType(string name) => null;
        public override string[] DescriptorNames => NAMES;
        public override object[] Parameters { get { return null; } set { } }

        public override DescriptorValue Calculate(IAtomContainer container)
        {
            if (sp == null) sp = new SmilesParser(container.Builder);

            IAtomContainer localAtomContainer = AtomContainerManipulator.RemoveHydrogens(container);
            CDKAtomTypeMatcher matcher = CDKAtomTypeMatcher.GetInstance(container.Builder);
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
            CDKHydrogenAdder hAdder = CDKHydrogenAdder.GetInstance(container.Builder);
            try
            {
                hAdder.AddImplicitHydrogens(localAtomContainer);
            }
            catch (Exception e)
            {
                return GetDummyDescriptorValue(new CDKException("Error in hydrogen addition: " + e.Message));
            }

            var subgraph4 = Order4(localAtomContainer);
            var subgraph5 = Order5(localAtomContainer);
            var subgraph6 = Order6(localAtomContainer);

            double order4s = ChiIndexUtils.EvalSimpleIndex(localAtomContainer, subgraph4);
            double order5s = ChiIndexUtils.EvalSimpleIndex(localAtomContainer, subgraph5);
            double order6s = ChiIndexUtils.EvalSimpleIndex(localAtomContainer, subgraph6);

            double order4v, order5v, order6v;
            try
            {
                order4v = ChiIndexUtils.EvalValenceIndex(localAtomContainer, subgraph4);
                order5v = ChiIndexUtils.EvalValenceIndex(localAtomContainer, subgraph5);
                order6v = ChiIndexUtils.EvalValenceIndex(localAtomContainer, subgraph6);
            }
            catch (CDKException e)
            {
                return GetDummyDescriptorValue(new CDKException("Error in substructure search: " + e.Message));
            }

            DoubleArrayResult retval = new DoubleArrayResult();
            retval.Add(order4s);
            retval.Add(order5s);
            retval.Add(order6s);

            retval.Add(order4v);
            retval.Add(order5v);
            retval.Add(order6v);

            return new DescriptorValue(_Specification, ParameterNames, Parameters, retval, DescriptorNames);
        }

        private DescriptorValue GetDummyDescriptorValue(Exception e)
        {
            int ndesc = DescriptorNames.Length;
            DoubleArrayResult results = new DoubleArrayResult(ndesc);
            for (int i = 0; i < ndesc; i++)
                results.Add(double.NaN);
            return new DescriptorValue(_Specification, ParameterNames, Parameters, results,
                    DescriptorNames, e);
        }

        /// <summary>
        /// Returns the specific type of the DescriptorResult object.
        /// <p/>
        /// The return value from this method really indicates what type of result will
        /// be obtained from the <see cref="DescriptorValue"/> object. Note that the same result
        /// can be achieved by interrogating the <see cref="DescriptorValue"/> object; this method
        /// allows you to do the same thing, without actually calculating the descriptor.
        ///
        /// <returns>an object that implements the <see cref="IDescriptorResult"/> interface indicating</returns>
        ///         the actual type of values returned by the descriptor in the <see cref="DescriptorValue"/> object
        /// </summary>
        public override IDescriptorResult DescriptorResultType { get; } = new DoubleArrayResultType(6);

        private IList<IList<int>> Order4(IAtomContainer atomContainer)
        {
            QueryAtomContainer[] queries = new QueryAtomContainer[1];
            try
            {
                queries[0] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles("CC(C)CC"), false);
            }
            catch (InvalidSmilesException e)
            {
                Console.Error.WriteLine(e.StackTrace); //To change body of catch statement use File | Settings | File Templates.
            }
            return ChiIndexUtils.GetFragments(atomContainer, queries);
        }

        private IList<IList<int>> Order5(IAtomContainer atomContainer)
        {
            QueryAtomContainer[] queries = new QueryAtomContainer[3];
            try
            {
                queries[0] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles("CC(C)CCC"), false);
                queries[1] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles("CC(C)(C)CC"), false);
                queries[2] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles("CCC(C)CC"), false);
            }
            catch (InvalidSmilesException e)
            {
                Console.Error.WriteLine(e.StackTrace); //To change body of catch statement use File | Settings | File Templates.
            }
            return ChiIndexUtils.GetFragments(atomContainer, queries);
        }

        private IList<IList<int>> Order6(IAtomContainer atomContainer)
        {
            QueryAtomContainer[] queries = new QueryAtomContainer[5];
            try
            {
                queries[0] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles("CC(C)(C)CCC"), false);
                queries[1] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles("CC(C)C(C)CC"), false);
                queries[2] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles("CC(C)CC(C)C"), false);
                queries[3] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles("CC(C)CCCC"), false);
                queries[4] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles("CCC(C)CCC"), false);
            }
            catch (InvalidSmilesException e)
            {
                Console.Error.WriteLine(e.StackTrace); //To change body of catch statement use File | Settings | File Templates.
            }
            return ChiIndexUtils.GetFragments(atomContainer, queries);
        }
    }
}
