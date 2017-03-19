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
    // @cdk.set qsar-descriptors
    // @cdk.dictref qsar-descriptors:chiPath
    // @cdk.keyword chi path index
    // @cdk.keyword descriptor
    public class ChiPathDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private SmilesParser sp;

        public ChiPathDescriptor() { }

        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
         new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#chiPath",
                typeof(ChiPathDescriptor).FullName, "The Chemistry Development Kit");

        public override string[] ParameterNames => null;
        public override object GetParameterType(string name) => null;
        public override object[] Parameters { get { return null; } set { } }

        public override string[] DescriptorNames { get; } = _DescriptorNames();
        private static string[] _DescriptorNames()
        {
            string[] names = new string[16];
            for (int i = 0; i < 8; i++)
            {
                names[i] = "SP-" + i;
                names[i + 8] = "VP-" + i;
            }
            return names;
        }

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

                double order0s = ChiIndexUtils.EvalSimpleIndex(localAtomContainer, subgraph0);
                double order1s = ChiIndexUtils.EvalSimpleIndex(localAtomContainer, subgraph1);
                double order2s = ChiIndexUtils.EvalSimpleIndex(localAtomContainer, subgraph2);
                double order3s = ChiIndexUtils.EvalSimpleIndex(localAtomContainer, subgraph3);
                double order4s = ChiIndexUtils.EvalSimpleIndex(localAtomContainer, subgraph4);
                double order5s = ChiIndexUtils.EvalSimpleIndex(localAtomContainer, subgraph5);
                double order6s = ChiIndexUtils.EvalSimpleIndex(localAtomContainer, subgraph6);
                double order7s = ChiIndexUtils.EvalSimpleIndex(localAtomContainer, subgraph7);

                double order0v = ChiIndexUtils.EvalValenceIndex(localAtomContainer, subgraph0);
                double order1v = ChiIndexUtils.EvalValenceIndex(localAtomContainer, subgraph1);
                double order2v = ChiIndexUtils.EvalValenceIndex(localAtomContainer, subgraph2);
                double order3v = ChiIndexUtils.EvalValenceIndex(localAtomContainer, subgraph3);
                double order4v = ChiIndexUtils.EvalValenceIndex(localAtomContainer, subgraph4);
                double order5v = ChiIndexUtils.EvalValenceIndex(localAtomContainer, subgraph5);
                double order6v = ChiIndexUtils.EvalValenceIndex(localAtomContainer, subgraph6);
                double order7v = ChiIndexUtils.EvalValenceIndex(localAtomContainer, subgraph7);

                DoubleArrayResult retval = new DoubleArrayResult();
                retval.Add(order0s);
                retval.Add(order1s);
                retval.Add(order2s);
                retval.Add(order3s);
                retval.Add(order4s);
                retval.Add(order5s);
                retval.Add(order6s);
                retval.Add(order7s);

                retval.Add(order0v);
                retval.Add(order1v);
                retval.Add(order2v);
                retval.Add(order3v);
                retval.Add(order4v);
                retval.Add(order5v);
                retval.Add(order6v);
                retval.Add(order7v);

                return new DescriptorValue(_Specification, ParameterNames, Parameters, retval, DescriptorNames);
            }
            catch (CDKException e)
            {
                return GetDummyDescriptorValue(new CDKException(e.Message));
            }

        }

        private DescriptorValue GetDummyDescriptorValue(Exception e)
        {
            int ndesc = DescriptorNames.Length;
            DoubleArrayResult results = new DoubleArrayResult(ndesc);
            for (int i = 0; i < ndesc; i++)
                results.Add(double.NaN);
            return new DescriptorValue(_Specification, ParameterNames, Parameters, results, DescriptorNames, e);
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
        public override IDescriptorResult DescriptorResultType { get; } = new DoubleArrayResultType(16);

        private List<IList<int>> Order0(IAtomContainer atomContainer)
        {
            var fragments = new List<IList<int>>();
            foreach (var atom in atomContainer.Atoms)
            {
                List<int> tmp = new List<int>();
                tmp.Add(atomContainer.Atoms.IndexOf(atom));
                fragments.Add(tmp);
            }
            return fragments;
        }

        private List<IList<int>> Order1(IAtomContainer atomContainer)
        {
            var fragments = new List<IList<int>>();
            foreach (var bond in atomContainer.Bonds)
            {
                if (bond.Atoms.Count != 2) throw new CDKException("We only consider 2 center bonds");
                List<int> tmp = new List<int>();
                tmp.Add(atomContainer.Atoms.IndexOf(bond.Atoms[0]));
                tmp.Add(atomContainer.Atoms.IndexOf(bond.Atoms[1]));
                fragments.Add(tmp);
            }
            return fragments;
        }

        private IList<IList<int>> Order2(IAtomContainer atomContainer)
        {
            QueryAtomContainer[] queries = new QueryAtomContainer[1];
            try
            {
                queries[0] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles("CCC"), false);
            }
            catch (InvalidSmilesException e)
            {
                Console.Error.WriteLine(e.StackTrace); //To change body of catch statement use File | Settings | File Templates.
            }
            return ChiIndexUtils.GetFragments(atomContainer, queries);
        }

        private IList<IList<int>> Order3(IAtomContainer atomContainer)
        {
            QueryAtomContainer[] queries = new QueryAtomContainer[1];
            try
            {
                queries[0] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles("CCCC"), false);
            }
            catch (InvalidSmilesException e)
            {
                Console.Error.WriteLine(e.StackTrace); //To change body of catch statement use File | Settings | File Templates.
            }
            return ChiIndexUtils.GetFragments(atomContainer, queries);
        }

        private IList<IList<int>> Order4(IAtomContainer atomContainer)
        {
            QueryAtomContainer[] queries = new QueryAtomContainer[1];
            try
            {
                queries[0] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles("CCCCC"), false);
            }
            catch (InvalidSmilesException e)
            {
                Console.Error.WriteLine(e.StackTrace); //To change body of catch statement use File | Settings | File Templates.
            }
            return ChiIndexUtils.GetFragments(atomContainer, queries);
        }

        private IList<IList<int>> Order5(IAtomContainer atomContainer)
        {
            QueryAtomContainer[] queries = new QueryAtomContainer[1];
            try
            {
                queries[0] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles("CCCCCC"), false);
            }
            catch (InvalidSmilesException e)
            {
                Console.Error.WriteLine(e.StackTrace); //To change body of catch statement use File | Settings | File Templates.
            }
            return ChiIndexUtils.GetFragments(atomContainer, queries);
        }

        private IList<IList<int>> Order6(IAtomContainer atomContainer)
        {
            QueryAtomContainer[] queries = new QueryAtomContainer[1];
            try
            {
                queries[0] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles("CCCCCCC"), false);
            }
            catch (InvalidSmilesException e)
            {
                Console.Error.WriteLine(e.StackTrace); //To change body of catch statement use File | Settings | File Templates.
            }
            return ChiIndexUtils.GetFragments(atomContainer, queries);
        }

        private IList<IList<int>> Order7(IAtomContainer atomContainer)
        {
            QueryAtomContainer[] queries = new QueryAtomContainer[1];
            try
            {
                queries[0] = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(sp.ParseSmiles("CCCCCCCC"), false);
            }
            catch (InvalidSmilesException e)
            {
                Console.Error.WriteLine(e.StackTrace); //To change body of catch statement use File | Settings | File Templates.
            }
            return ChiIndexUtils.GetFragments(atomContainer, queries);
        }
    }
}
