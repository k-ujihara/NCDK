/*
 *  Copyright (C) 2004-2007  Rajarshi Guha <rajarshi@users.sourceforge.net>
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
using NCDK.QSAR.Result;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Evaluates the weighted path descriptors.
    /// </summary>
    /// <remarks>
    /// These decsriptors were described  by Randic (<token>cdk-cite-RAN84</token>) and characterize molecular
    /// branching. Five descriptors are calculated, based on the implementation in the ADAPT
    /// software package. Note that the descriptor is based on identifying <b>all</b> pahs between pairs of
    /// atoms and so is NP-hard. This means that it can take some time for large, complex molecules.
    /// The class returns a <see cref="DoubleArrayResult"/> containing the five
    /// descriptors in the order described below.
    /// <list type="bullet">
    /// <listheader>
    /// <term>DMWP</term>
    /// </listheader>
    /// <item><term>WTPT1</term><description>molecular ID</description></item>
    /// <item><term>WTPT2</term><description>molecular ID / number of atoms</description></item>
    /// <item><term>WTPT3</term><description>sum of path lengths starting from heteroatoms</description></item>
    /// <item><term>WTPT4</term><description>sum of path lengths starting from oxygens</description></item>
    /// <item><term>WTPT5</term><description>sum of path lengths starting from nitrogens</description></item>
    /// </list> 
    /// This descriptor uses these parameters:
    /// <list type="table">
    /// <listheader>
    ///     <term>Name</term>
    ///     <term>Default</term>
    ///     <term>Description</term>
    /// </listheader>
    /// <item>
    /// <term></term>
    /// <term></term>
    /// <term>no parameters</term>
    /// </item>
    /// </list>
    /// </remarks>
    // @author Rajarshi Guha
    // @cdk.created 2006-01-15
    // @cdk.module qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:weightedPath
    public class WeightedPathDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private static readonly string[] NAMES = { "WTPT-1", "WTPT-2", "WTPT-3", "WTPT-4", "WTPT-5" };

        public WeightedPathDescriptor() { }

        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#weightedPath",
                typeof(WeightedPathDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the WeightedPathDescriptor object.
        /// </summary>
        public override object[] Parameters { get { return null; } set { } }

        public override string[] DescriptorNames => NAMES;

        /// <summary>
        /// The parameterNames attribute of the WeightedPathDescriptor object.
        /// </summary>
        public override string[] ParameterNames => null;

        /// <summary>
        /// Gets the parameterType attribute of the WeightedPathDescriptor object.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public override object GetParameterType(string name) => null;

        /// <summary>
        /// Calculates the weighted path descriptors.
        /// </summary>
        /// <param name="container">Parameter is the atom container.</param>
        /// <returns>A DoubleArrayResult value representing the weighted path values</returns>
        public override DescriptorValue Calculate(IAtomContainer container)
        {
            IAtomContainer local = AtomContainerManipulator.RemoveHydrogens(container);
            int natom = local.Atoms.Count;
            DoubleArrayResult retval = new DoubleArrayResult();

            var pathList = new List<IList<IAtom>>();

            // unique paths
            for (int i = 0; i < natom - 1; i++)
            {
                IAtom a = local.Atoms[i];
                for (int j = i + 1; j < natom; j++)
                {
                    IAtom b = local.Atoms[j];
                    pathList.AddRange(PathTools.GetAllPaths(local, a, b));
                }
            }

            // heteroatoms
            double[] pathWts = GetPathWeights(pathList, local);
            double mid = 0.0;
            foreach (var pathWt3 in pathWts)
                mid += pathWt3;
            mid += natom; // since we don't calculate paths of length 0 above

            retval.Add(mid);
            retval.Add(mid / (double)natom);

            pathList.Clear();
            int count = 0;
            for (int i = 0; i < natom; i++)
            {
                IAtom a = local.Atoms[i];
                if (string.Equals(a.Symbol, "C", StringComparison.OrdinalIgnoreCase)) continue;
                count++;
                for (int j = 0; j < natom; j++)
                {
                    IAtom b = local.Atoms[j];
                    if (a.Equals(b)) continue;
                    pathList.AddRange(PathTools.GetAllPaths(local, a, b));
                }
            }
            pathWts = GetPathWeights(pathList, local);
            mid = 0.0;
            foreach (var pathWt2 in pathWts)
                mid += pathWt2;
            mid += count;
            retval.Add(mid);

            // oxygens
            pathList.Clear();
            count = 0;
            for (int i = 0; i < natom; i++)
            {
                IAtom a = local.Atoms[i];
                if (!string.Equals(a.Symbol, "O", StringComparison.OrdinalIgnoreCase)) continue;
                count++;
                for (int j = 0; j < natom; j++)
                {
                    IAtom b = local.Atoms[j];
                    if (a.Equals(b)) continue;
                    pathList.AddRange(PathTools.GetAllPaths(local, a, b));
                }
            }
            pathWts = GetPathWeights(pathList, local);
            mid = 0.0;
            foreach (var pathWt1 in pathWts)
                mid += pathWt1;
            mid += count;
            retval.Add(mid);

            // nitrogens
            pathList.Clear();
            count = 0;
            for (int i = 0; i < natom; i++)
            {
                IAtom a = local.Atoms[i];
                if (!string.Equals(a.Symbol, "N", StringComparison.OrdinalIgnoreCase)) continue;
                count++;
                for (int j = 0; j < natom; j++)
                {
                    IAtom b = local.Atoms[j];
                    if (a.Equals(b)) continue;
                    pathList.AddRange(PathTools.GetAllPaths(local, a, b));
                }
            }
            pathWts = GetPathWeights(pathList, local);
            mid = 0.0;
            foreach (var pathWt in pathWts)
                mid += pathWt;
            mid += count;
            retval.Add(mid);

            return new DescriptorValue(_Specification, ParameterNames, Parameters, retval, DescriptorNames);
        }

        /// <inheritdoc/>
        public override IDescriptorResult DescriptorResultType { get; } = new DoubleArrayResultType(5);

        private double[] GetPathWeights(List<IList<IAtom>> pathList, IAtomContainer atomContainer)
        {
            double[] pathWts = new double[pathList.Count];
            for (int i = 0; i < pathList.Count; i++)
            {
                var p = pathList[i];
                pathWts[i] = 1.0;
                for (int j = 0; j < p.Count - 1; j++)
                {
                    IAtom a = (IAtom)p[j];
                    IAtom b = (IAtom)p[j + 1];
                    int n1 = atomContainer.GetConnectedAtoms(a).Count();
                    int n2 = atomContainer.GetConnectedAtoms(b).Count();
                    pathWts[i] /= Math.Sqrt(n1 * n2);
                }
            }
            return pathWts;
        }
    }
}
