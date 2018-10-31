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

using NCDK.Isomorphisms.Matchers;
using NCDK.QSAR.Results;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Evaluates chi path descriptors.
    /// </summary>
    /// <remarks>
    /// It utilizes the graph isomorphism code of the CDK to find fragments matching
    /// SMILES strings representing the fragments corresponding to each type of chain.
    /// <para>
    /// The order of the values returned is
    /// <list type="bullet"> 
    /// <item>SP-0, SP-1, ..., SP-7 - Simple path, orders 0 to 7</item>
    /// <item>VP-0, VP-1, ..., VP-7 - Valence path, orders 0 to 7</item>
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
    // @cdk.dictref qsar-descriptors:chiPath
    // @cdk.keyword chi path index
    // @cdk.keyword descriptor
    public class ChiPathDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        public ChiPathDescriptor()
        {
        }

        public override IImplementationSpecification Specification => specification;
        private static readonly DescriptorSpecification specification =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#chiPath",
                typeof(ChiPathDescriptor).FullName, "The Chemistry Development Kit");

        public override IReadOnlyList<string> ParameterNames => null;
        public override object GetParameterType(string name) => null;
        public override IReadOnlyList<object> Parameters { get { return null; } set { } }

        public override IReadOnlyList<string> DescriptorNames { get; } = _DescriptorNames();
        private static string[] _DescriptorNames()
        {
            var names = new string[16];
            for (int i = 0; i < 8; i++)
            {
                names[i] = "SP-" + i;
                names[i + 8] = "VP-" + i;
            }
            return names;
        }

        public DescriptorValue<ArrayResult<double>> Calculate(IAtomContainer container)
        {
            var localAtomContainer = AtomContainerManipulator.RemoveHydrogens(container);
            var matcher = CDK.AtomTypeMatcher;
            foreach (var atom in localAtomContainer.Atoms)
            {
                try
                {
                    var type = matcher.FindMatchingAtomType(localAtomContainer, atom);
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

            try
            {
                var subgraph0 = Order0(localAtomContainer);
                var subgraph1 = Order1(localAtomContainer);
                var subgraph2 = Order2(localAtomContainer);
                var subgraph3 = Order3(localAtomContainer);
                var subgraph4 = Order4(localAtomContainer);
                var subgraph5 = Order5(localAtomContainer);
                var subgraph6 = Order6(localAtomContainer);
                var subgraph7 = Order7(localAtomContainer);

                var order0s = ChiIndexUtils.EvalSimpleIndex(localAtomContainer, subgraph0);
                var order1s = ChiIndexUtils.EvalSimpleIndex(localAtomContainer, subgraph1);
                var order2s = ChiIndexUtils.EvalSimpleIndex(localAtomContainer, subgraph2);
                var order3s = ChiIndexUtils.EvalSimpleIndex(localAtomContainer, subgraph3);
                var order4s = ChiIndexUtils.EvalSimpleIndex(localAtomContainer, subgraph4);
                var order5s = ChiIndexUtils.EvalSimpleIndex(localAtomContainer, subgraph5);
                var order6s = ChiIndexUtils.EvalSimpleIndex(localAtomContainer, subgraph6);
                var order7s = ChiIndexUtils.EvalSimpleIndex(localAtomContainer, subgraph7);

                var order0v = ChiIndexUtils.EvalValenceIndex(localAtomContainer, subgraph0);
                var order1v = ChiIndexUtils.EvalValenceIndex(localAtomContainer, subgraph1);
                var order2v = ChiIndexUtils.EvalValenceIndex(localAtomContainer, subgraph2);
                var order3v = ChiIndexUtils.EvalValenceIndex(localAtomContainer, subgraph3);
                var order4v = ChiIndexUtils.EvalValenceIndex(localAtomContainer, subgraph4);
                var order5v = ChiIndexUtils.EvalValenceIndex(localAtomContainer, subgraph5);
                var order6v = ChiIndexUtils.EvalValenceIndex(localAtomContainer, subgraph6);
                var order7v = ChiIndexUtils.EvalValenceIndex(localAtomContainer, subgraph7);

                var retval = new ArrayResult<double>
                {
                    order0s,
                    order1s,
                    order2s,
                    order3s,
                    order4s,
                    order5s,
                    order6s,
                    order7s,
                    order0v,
                    order1v,
                    order2v,
                    order3v,
                    order4v,
                    order5v,
                    order6v,
                    order7v,
                };

                return new DescriptorValue<ArrayResult<double>>(specification, ParameterNames, Parameters, retval, DescriptorNames);
            }
            catch (CDKException e)
            {
                return GetDummyDescriptorValue(new CDKException(e.Message));
            }
        }

        private DescriptorValue<ArrayResult<double>> GetDummyDescriptorValue(Exception e)
        {
            var ndesc = DescriptorNames.Count;
            var results = new ArrayResult<double>(ndesc);
            for (int i = 0; i < ndesc; i++)
                results.Add(double.NaN);
            return new DescriptorValue<ArrayResult<double>>(specification, ParameterNames, Parameters, results, DescriptorNames, e);
        }

        /// <summary>
        /// An object that implements the <see cref="IDescriptorResult"/> interface indicating
        /// the actual type of values returned by the descriptor in the <see cref="IDescriptorValue"/> object
        /// </summary>
        /// <remarks>
        /// The value really indicates what type of result will
        /// be obtained from the <see cref="IDescriptorValue"/> object. Note that the same result
        /// can be achieved by interrogating the <see cref="IDescriptorValue"/> object; this method
        /// allows you to do the same thing, without actually calculating the descriptor.
        /// </remarks>
        public override IDescriptorResult DescriptorResultType { get; } = new ArrayResult<double>(16);

        private static List<List<int>> Order0(IAtomContainer atomContainer)
        {
            var fragments = new List<List<int>>();
            foreach (var atom in atomContainer.Atoms)
            {
                var tmp = new List<int> { atomContainer.Atoms.IndexOf(atom) };
                fragments.Add(tmp);
            }
            return fragments;
        }

        private static List<List<int>> Order1(IAtomContainer atomContainer)
        {
            var fragments = new List<List<int>>();
            foreach (var bond in atomContainer.Bonds)
            {
                if (bond.Atoms.Count != 2)
                    throw new CDKException("We only consider 2 center bonds");
                var tmp = new List<int>
                {
                    atomContainer.Atoms.IndexOf(bond.Atoms[0]),
                    atomContainer.Atoms.IndexOf(bond.Atoms[1])
                };
                fragments.Add(tmp);
            }
            return fragments;
        }

        private static readonly IAtomContainer C3 = CDK.SilentSmilesParser.ParseSmiles("CCC");
        private static readonly IAtomContainer C4 = CDK.SilentSmilesParser.ParseSmiles("CCCC");
        private static readonly IAtomContainer C5 = CDK.SilentSmilesParser.ParseSmiles("CCCCC");
        private static readonly IAtomContainer C6 = CDK.SilentSmilesParser.ParseSmiles("CCCCCC");
        private static readonly IAtomContainer C7 = CDK.SilentSmilesParser.ParseSmiles("CCCCCCC");
        private static readonly IAtomContainer C8 = CDK.SilentSmilesParser.ParseSmiles("CCCCCCCC");

        private static List<List<int>> Order2(IAtomContainer atomContainer)
        {
            var queries = new QueryAtomContainer[1];
            try
            {
                queries[0] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(C3, false);
            }
            catch (InvalidSmilesException e)
            {
                Console.Error.WriteLine(e.StackTrace); //To change body of catch statement use File | Settings | File Templates.
            }
            return ChiIndexUtils.GetFragments(atomContainer, queries);
        }

        private static List<List<int>> Order3(IAtomContainer atomContainer)
        {
            var queries = new QueryAtomContainer[1];
            try
            {
                queries[0] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(C4, false);
            }
            catch (InvalidSmilesException e)
            {
                Console.Error.WriteLine(e.StackTrace); //To change body of catch statement use File | Settings | File Templates.
            }
            return ChiIndexUtils.GetFragments(atomContainer, queries);
        }

        private static List<List<int>> Order4(IAtomContainer atomContainer)
        {
            var queries = new QueryAtomContainer[1];
            try
            {
                queries[0] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(C5, false);
            }
            catch (InvalidSmilesException e)
            {
                Console.Error.WriteLine(e.StackTrace); //To change body of catch statement use File | Settings | File Templates.
            }
            return ChiIndexUtils.GetFragments(atomContainer, queries);
        }

        private static List<List<int>> Order5(IAtomContainer atomContainer)
        {
            var queries = new QueryAtomContainer[1];
            try
            {
                queries[0] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(C6, false);
            }
            catch (InvalidSmilesException e)
            {
                Console.Error.WriteLine(e.StackTrace); //To change body of catch statement use File | Settings | File Templates.
            }
            return ChiIndexUtils.GetFragments(atomContainer, queries);
        }

        private static List<List<int>> Order6(IAtomContainer atomContainer)
        {
            var queries = new QueryAtomContainer[1];
            try
            {
                queries[0] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(C7, false);
            }
            catch (InvalidSmilesException e)
            {
                Console.Error.WriteLine(e.StackTrace); //To change body of catch statement use File | Settings | File Templates.
            }
            return ChiIndexUtils.GetFragments(atomContainer, queries);
        }

        private static List<List<int>> Order7(IAtomContainer atomContainer)
        {
            var queries = new QueryAtomContainer[1];
            try
            {
                queries[0] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(C8, false);
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
