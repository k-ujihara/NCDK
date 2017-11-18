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
using NCDK.Graphs;
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
    /// Evaluates chi chain descriptors.
    /// </summary>
    /// <remarks>
    /// The code currently evluates the simple and valence chi chain descriptors of orders 3, 4, 5, 6 and 7.
    /// It utilizes the graph isomorphism code of the CDK to find fragments matching
    /// SMILES strings representing the fragments corresponding to each type of chain.
    /// <para>
    /// The order of the values returned is
    /// <list type="bullet">
    /// <item>SCH-3 - Simple chain, order 3</item>
    /// <item>SCH-4 - Simple chain, order 4</item>
    /// <item>SCH-5 - Simple chain, order 5</item>
    /// <item>SCH-6 - Simple chain, order 6</item>
    /// <item>SCH-7 - Simple chain, order 7</item>
    /// <item>VCH-3 - Valence chain, order 3</item>
    /// <item>VCH-4 - Valence chain, order 4</item>
    /// <item>VCH-5 - Valence chain, order 5</item>
    /// <item>VCH-6 - Valence chain, order 6</item>
    /// <item>VCH-7 - Valence chain, order 7</item>
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
    // @cdk.created 2006-11-12
    // @cdk.module qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:chiChain
    // @cdk.keyword chi chain index
    // @cdk.keyword descriptor
    public class ChiChainDescriptor : IMolecularDescriptor
    {
        private SmilesParser sp;

        private static readonly string[] NAMES = { "SCH-3", "SCH-4", "SCH-5", "SCH-6", "SCH-7", "VCH-3", "VCH-4", "VCH-5", "VCH-6", "VCH-7" };

        public ChiChainDescriptor() { }

        public IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
         new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#chiChain",
                typeof(ChiChainDescriptor).FullName,
                "The Chemistry Development Kit");

        public IReadOnlyList<string> ParameterNames => null; //To change body of implemented methods use File | Settings | File Templates.

        public object GetParameterType(string name)
        {
            return null; //To change body of implemented methods use File | Settings | File Templates.
        }

        public object[] Parameters
        {
            set
            {
                //To change body of implemented methods use File | Settings | File Templates.
            }
            get
            {
                return null; //To change body of implemented methods use File | Settings | File Templates.
            }
        }

        public IReadOnlyList<string> DescriptorNames => NAMES;

        private DescriptorValue<ArrayResult<double>> GetDummyDescriptorValue(Exception e)
        {
            int ndesc = DescriptorNames.Count;
            ArrayResult<double> results = new ArrayResult<double>(ndesc);
            for (int i = 0; i < ndesc; i++)
                results.Add(double.NaN);
            return new DescriptorValue<ArrayResult<double>>(_Specification, ParameterNames, Parameters, results,
                    DescriptorNames, e);
        }

        public DescriptorValue<ArrayResult<double>> Calculate(IAtomContainer container)
        {
            if (sp == null) sp = new SmilesParser(container.Builder);

            // we don't make a clone, since removeHydrogens returns a deep copy
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
                catch (Exception)
                {
                    return GetDummyDescriptorValue(new CDKException($"Error in atom typing: {atom}"));
                }
            }
            CDKHydrogenAdder hAdder = CDKHydrogenAdder.GetInstance(container.Builder);
            try
            {
                hAdder.AddImplicitHydrogens(localAtomContainer);
            }
            catch (CDKException e)
            {
                return GetDummyDescriptorValue(new CDKException($"Error in adding hydrogens: {e.Message}"));
            }

            var subgraph3 = Order3(localAtomContainer);
            var subgraph4 = Order4(localAtomContainer);
            var subgraph5 = Order5(localAtomContainer);
            var subgraph6 = Order6(localAtomContainer);
            var subgraph7 = Order7(localAtomContainer);

            double order3s = ChiIndexUtils.EvalSimpleIndex(localAtomContainer, subgraph3);
            double order4s = ChiIndexUtils.EvalSimpleIndex(localAtomContainer, subgraph4);
            double order5s = ChiIndexUtils.EvalSimpleIndex(localAtomContainer, subgraph5);
            double order6s = ChiIndexUtils.EvalSimpleIndex(localAtomContainer, subgraph6);
            double order7s = ChiIndexUtils.EvalSimpleIndex(localAtomContainer, subgraph7);

            double order3v, order4v, order5v, order6v, order7v;
            try
            {
                order3v = ChiIndexUtils.EvalValenceIndex(localAtomContainer, subgraph3);
                order4v = ChiIndexUtils.EvalValenceIndex(localAtomContainer, subgraph4);
                order5v = ChiIndexUtils.EvalValenceIndex(localAtomContainer, subgraph5);
                order6v = ChiIndexUtils.EvalValenceIndex(localAtomContainer, subgraph6);
                order7v = ChiIndexUtils.EvalValenceIndex(localAtomContainer, subgraph7);
            }
            catch (CDKException e)
            {
                return GetDummyDescriptorValue(new CDKException("Error in substructure search: " + e.Message));
            }

            ArrayResult<double> retval = new ArrayResult<double>();
            retval.Add(order3s);
            retval.Add(order4s);
            retval.Add(order5s);
            retval.Add(order6s);
            retval.Add(order7s);

            retval.Add(order3v);
            retval.Add(order4v);
            retval.Add(order5v);
            retval.Add(order6v);
            retval.Add(order7v);

            return new DescriptorValue<ArrayResult<double>>(_Specification, ParameterNames, Parameters, retval, DescriptorNames);
        }

        /// <inheritdoc/>
        public IDescriptorResult DescriptorResultType { get; } = new ArrayResult<double>(10);

        private IList<IList<int>> Order3(IAtomContainer container)
        {
            var ret = new List<IList<int>>();

            IRingSet rings = Cycles.FindSSSR(container).ToRingSet();

            int nring = rings.Count;
            for (int i = 0; i < nring; i++)
            {
                IAtomContainer ring = rings[i];
                if (ring.Atoms.Count == 3)
                {
                    var tmp = new List<int>();
                    foreach (var atom in ring.Atoms)
                    {
                        tmp.Add(container.Atoms.IndexOf(atom));
                    }
                    ret.Add(tmp);
                }
            }
            return ret;
        }

        private IList<IList<int>> Order4(IAtomContainer atomContainer)
        {
            QueryAtomContainer[] queries = new QueryAtomContainer[2];
            try
            {
                queries[0] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles("C1CCC1"), false);
                queries[1] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles("CC1CC1"), false);
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
                queries[0] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles("C1CCCC1"), false);
                queries[1] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles("CC1CCC1"), false);
                queries[2] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles("CC1CC1(C)"), false);
            }
            catch (InvalidSmilesException e)
            {
                Console.Error.WriteLine(e.StackTrace); //To change body of catch statement use File | Settings | File Templates.
            }
            return ChiIndexUtils.GetFragments(atomContainer, queries);
        }

        private IList<IList<int>> Order6(IAtomContainer atomContainer)
        {
            QueryAtomContainer[] queries = new QueryAtomContainer[9];
            try
            {
                queries[0] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles("CC1CCCC1"), false);
                queries[1] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles("CC1CC(C)C1"), false);
                queries[2] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles("CC1(C)(CCC1)"), false);
                queries[3] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles("CCC1CCC1"), false);
                queries[4] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles("C1CCCCC1"), false);
                queries[5] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles("CC1CCC1(C)"), false);
                queries[6] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles("CC1C(C)C1(C)"), false);
                queries[7] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles("CCCC1CC1"), false);
                queries[8] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles("CCC1CC1(C)"), false);
            }
            catch (InvalidSmilesException e)
            {
                Console.Error.WriteLine(e.StackTrace); //To change body of catch statement use File | Settings | File Templates.
            }
            return ChiIndexUtils.GetFragments(atomContainer, queries);
        }

        private IList<IList<int>> Order7(IAtomContainer atomContainer)
        {
            string[] smiles = {
                "C1CCCCC1C",
                // 5-ring cases
                "C1CCCC1(C)(C)", "C1(C)C(C)CCC1", "C1(C)CC(C)CC1",
                "C1CCCC1(CC)",
                // 4-ring cases
                "C1(C)C(C)C(C)C1", "C1CC(C)C1(CC)", "C1C(C)CC1(CC)", "C1CCC1(CCC)", "C1CCC1C(C)(C)", "C1CCC1(C)(CC)",
                "C1CC(C)C1(C)(C)", "C1C(C)CC1(C)(C)",
                // 3-ring cases
                "C1(C)C(C)C1(CC)", "C1C(C)(C)C1(C)(C)", "C1CC1CCCC", "C1C(C)C1(CCC)", "C1C(CC)C1(CC)",
                "C1C(C)C1C(C)(C)", "C1C(C)C1(C)(CC)", "C1CC1CC(C)(C)", "C1CC1C(C)CC", "C1CC1C(C)(C)(C)"};
            QueryAtomContainer[] queries = new QueryAtomContainer[smiles.Length];
            try
            {
                for (int i = 0; i < smiles.Length; i++)
                    queries[i] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles(smiles[i]), false);
            }
            catch (InvalidSmilesException e)
            {
                Console.Error.WriteLine(e.StackTrace);
            }
            return ChiIndexUtils.GetFragments(atomContainer, queries);
        }

        DescriptorValue IMolecularDescriptor.Calculate(IAtomContainer container) => Calculate(container);
    }
}
