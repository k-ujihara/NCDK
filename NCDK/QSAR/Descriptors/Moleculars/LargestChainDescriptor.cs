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
    /// Class that returns the number of atoms in the largest chain.
    /// </summary>
    /// <remarks>
    /// <para>This descriptor uses these parameters:
    /// <list type="table">
    /// <item>
    /// <term>Name</term>
    /// <term>Default</term>
    /// <term>Description</term>
    /// </item>
    /// <item>
    /// <term>checkAromaticity (deprecated)</term>
    /// <term>false</term>
    /// <term>Old parameter is now ignored</term>
    /// </item>
    /// <item>
    /// <term>checkRingSystem</term>
    /// <term>false</term>
    /// <term>True is the CDKConstant.ISINRING has to be set</term>
    /// </item>
    /// </list>
    /// </para>
    /// Returns a single value named <i>nAtomLAC</i>. Note that a chain exists if there
    /// are two or more atoms. Thus single atom molecules will return 0
    /// </remarks>
    // @author chhoppe from EUROSCREEN
    // @cdk.created 2006-1-03
    // @cdk.module qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:largestChain
    public class LargestChainDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private bool checkAromaticity = false;
        private bool checkRingSystem = false;
        private static readonly string[] NAMES = { "nAtomLC" };

        public LargestChainDescriptor() { }

        /// <inheritdoc/> 
        public override IImplementationSpecification Specification => specification;
        private static readonly DescriptorSpecification specification =
         new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#largestChain",
                typeof(LargestChainDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the LargestChain object.
        /// </summary>
        /// <remarks>
        /// This descriptor takes two parameters, which should be Booleans to indicate whether
        /// aromaticity and ring member ship needs been checked (TRUE) or not (FALSE). The first
        /// parameter (aromaticity) is deprecated and ignored.
        /// </remarks>
        /// <exception cref="CDKException">if more than one parameter or a non-bool parameter is specified</exception>
        public override IReadOnlyList<object> Parameters
        {
            set
            {
                if (value.Count > 2)
                {
                    throw new CDKException("LargestChainDescriptor only expects two parameter");
                }
                if (!(value[0] is bool) || !(value[1] is bool))
                {
                    throw new CDKException("Both parameters must be of type bool");
                }
                // ok, all should be fine
                checkAromaticity = (bool)value[0];
                checkRingSystem = (bool)value[1];
            }
            get
            {
                // return the parameters as used for the descriptor calculation
                return new object[] { checkAromaticity, checkRingSystem };
            }
        }

        public override IReadOnlyList<string> DescriptorNames => NAMES;

        private IDescriptorValue GetDummyDescriptorValue(Exception e)
        {
            return new DescriptorValue<Result<int>>(specification, ParameterNames, Parameters, new Result<int>(0), DescriptorNames, e);
        }

        /// <summary>
        /// Calculate the count of atoms of the largest chain in the supplied <see cref="IAtomContainer"/>.
        /// </summary>
        /// <param name="atomContainer">The <see cref="IAtomContainer"/> for which this descriptor is to be calculated</param>
        /// <returns>the number of atoms in the largest chain of this AtomContainer</returns>
        /// <seealso cref="Parameters"/>
        public DescriptorValue<Result<int>> Calculate(IAtomContainer atomContainer)
        {
            if (checkRingSystem)
                Cycles.MarkRingAtomsAndBonds(atomContainer);

            // make a subset molecule only including acyclic non-hydrogen atoms
            var included = new HashSet<IAtom>();
            foreach (var atom in atomContainer.Atoms)
            {
                if (!atom.IsInRing && atom.AtomicNumber != 1)
                    included.Add(atom);
            }
            var subset = SubsetMol(atomContainer, included);

            var apsp = new AllPairsShortestPaths(subset);

            int max = 0;
            int numAtoms = subset.Atoms.Count;
            for (int i = 0; i < numAtoms; i++)
            {
                for (int j = i + 1; j < numAtoms; j++)
                {
                    int len = apsp.From(i).GetPathTo(j).Length;
                    if (len > max)
                        max = len;
                }
            }

            return new DescriptorValue<Result<int>>(specification, ParameterNames, Parameters, new Result<int>(max), DescriptorNames);
        }

        /// <inheritdoc/>
        public override IDescriptorResult DescriptorResultType { get; } = new Result<int>(1);
        public override IReadOnlyList<string> ParameterNames { get; } = new string[] { "checkAromaticity", "checkRingSystem" };
        public override object GetParameterType(string name) => true;

        private static IAtomContainer SubsetMol(IAtomContainer mol, ISet<IAtom> include)
        {
            var cpy = mol.Builder.NewAtomContainer();
            foreach (var atom in mol.Atoms)
            {
                if (include.Contains(atom))
                    cpy.Atoms.Add(atom);
            }
            foreach (var bond in mol.Bonds)
            {
                if (include.Contains(bond.Atoms[0]) && include.Contains(bond.Atoms[1]))
                    cpy.Bonds.Add(bond);
            }
            return cpy;
        }

        IDescriptorValue IMolecularDescriptor.Calculate(IAtomContainer container) => Calculate(container);
    }
}
