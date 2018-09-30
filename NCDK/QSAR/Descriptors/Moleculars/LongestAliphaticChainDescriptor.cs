/*  Copyright (C) 2005-2007  Christian Hoppe <chhoppe@users.sf.net>
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
using System;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Counts the number of atoms in the longest aliphatic chain.
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
    ///     <term>checkRingSystem</term>
    ///     <term>false</term>
    ///     <term>True is the CDKConstant.ISINRING has to be set</term>
    ///   </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// Returns a single value named <i>nAtomLAC</i>
    // @author      chhoppe from EUROSCREEN
    // @author John Mayfield
    // @cdk.created 2006-1-03
    // @cdk.module  qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:largestAliphaticChain
    public class LongestAliphaticChainDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private const string CHECK_RING_SYSTEM = "checkRingSystem";
        private bool checkRingSystem = false;
        private static readonly string[] NAMES = { "nAtomLAC" };

        public LongestAliphaticChainDescriptor() { }

        /// <inheritdoc/> 
        public override IImplementationSpecification Specification => specification;
        private static readonly DescriptorSpecification specification =
         new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#longestAliphaticChain",
                typeof(LongestAliphaticChainDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// Sets the parameters attribute of the LongestAliphaticChainDescriptor object.
        /// </summary>
        /// <remarks>
        /// This descriptor takes one parameter, which should be bool to indicate whether
        /// aromaticity has been checked <see langword="true"/> or not <see langword="false"/>.
        /// </remarks>
        /// <exception cref="CDKException">if more than one parameter or a non-bool parameter is specified</exception>
        public override IReadOnlyList<object> Parameters
        {
            set
            {
                if (value.Count > 1)
                {
                    throw new CDKException("LongestAliphaticChainDescriptor only expects one parameter");
                }
                if (!(value[0] is bool))
                {
                    throw new CDKException("Expected parameter of type " + typeof(bool).ToString());
                }
                // ok, all should be fine
                checkRingSystem = (bool)value[0];
            }
            get
            {
                // return the parameters as used for the descriptor calculation
                return new object[] { checkRingSystem };
            }
        }

        public override IReadOnlyList<string> DescriptorNames => NAMES;

        private DescriptorValue<Result<int>> GetDummyDescriptorValue(Exception e)
        {
            return new DescriptorValue<Result<int>>(
                specification,
                ParameterNames,
                Parameters,
                new Result<int>(0),
                DescriptorNames,
                e);
        }

        private static bool IsAcyclicCarbon(IAtom atom)
        {
            return atom.AtomicNumber == 6 && !atom.IsInRing;
        }

        /// <summary>
        /// Depth-First-Search on an acyclic graph. Since we have no cycles we
        /// don't need the visit flags and only need to know which atom we came from.
        /// </summary>
        /// <param name="adjlist">adjacency list representation of grah</param>
        /// <param name="v">the current atom index</param>
        /// <param name="prev">the previous atom index</param>
        /// <returns>the max length traversed</returns>
        private static int GetMaxDepth(int[][] adjlist, int v, int prev)
        {
            int longest = 0;
            foreach (var w in adjlist[v])
            {
                if (w == prev) continue;
                // no cycles so don't need to check previous
                int length = GetMaxDepth(adjlist, w, v);
                if (length > longest)
                    longest = length;
            }
            return 1 + longest;
        }

        /// <summary>
        /// Calculate the count of atoms of the longest aliphatic chain in the supplied <see cref="IAtomContainer"/>.
        /// </summary>
        /// <remarks>
        /// The method require one parameter:
        /// if checkRingSyste is true the <see cref="IMolecularEntity.IsInRing"/> will be set
        /// </remarks>
        /// <param name="mol">The <see cref="IAtomContainer"/> for which this descriptor is to be calculated</param>
        /// <returns>the number of atoms in the longest aliphatic chain of this AtomContainer</returns>
        /// <seealso cref="Parameters"/>
        public DescriptorValue<Result<int>> Calculate(IAtomContainer mol)
        {
            if (checkRingSystem)
                Cycles.MarkRingAtomsAndBonds(mol);

            IAtomContainer aliphaticParts = mol.Builder.NewAtomContainer();
            foreach (var atom in mol.Atoms)
            {
                if (IsAcyclicCarbon(atom))
                    aliphaticParts.Atoms.Add(atom);
            }
            foreach (var bond in mol.Bonds)
            {
                if (IsAcyclicCarbon(bond.Begin) &&
                    IsAcyclicCarbon(bond.End))
                    aliphaticParts.Bonds.Add(bond);
            }

            int longest = 0;
            var adjlist = GraphUtil.ToAdjList(aliphaticParts);
            for (int i = 0; i < adjlist.Length; i++)
            {
                // atom deg > 1 can't find the longest chain
                if (adjlist[i].Length != 1)
                    continue;
                int length = GetMaxDepth(adjlist, i, -1);
                if (length > longest)
                    longest = length;
            }

            return new DescriptorValue<Result<int>>(
                specification,
                ParameterNames,
                Parameters,
                new Result<int>(longest),
                DescriptorNames);
        }

        /// <inheritdoc/>
        public override IDescriptorResult DescriptorResultType { get; } = new Result<int>(1);
        public override IReadOnlyList<string> ParameterNames { get; } = new string[] { CHECK_RING_SYSTEM };
        public override object GetParameterType(string name)
        {
            if (name.Equals(CHECK_RING_SYSTEM, StringComparison.Ordinal))
                return true;
            else
                throw new ArgumentException("No parameter for name", nameof(name));
        }

        IDescriptorValue IMolecularDescriptor.Calculate(IAtomContainer container) => Calculate(container);
    }
}
