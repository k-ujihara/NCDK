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
using NCDK.QSAR.Result;
using System;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Class that returns the number of atoms in the largest chain.
    /// <p/>
    /// <p>This descriptor uses these parameters:
    /// <table border="1">
    /// <tr>
    /// <td>Name</td>
    /// <td>Default</td>
    /// <td>Description</td>
    /// </tr>
    /// <tr>
    /// <td>checkAromaticity (deprecated)</td>
    /// <td>false</td>
    /// <td>Old parameter is now ignored</td>
    /// </tr>
    /// <tr>
    /// <td>checkRingSystem</td>
    /// <td>false</td>
    /// <td>True is the CDKConstant.ISINRING has to be set</td>
    /// </tr>
    /// </table>
    /// <p/>
    /// Returns a single value named <i>nAtomLAC</i>. Note that a chain exists if there
    /// are two or more atoms. Thus single atom molecules will return 0
    ///
    // @author chhoppe from EUROSCREEN
    // @cdk.created 2006-1-03
    // @cdk.module qsarmolecular
    // @cdk.githash
    // @cdk.set qsar-descriptors
    // @cdk.dictref qsar-descriptors:largestChain
    /// </summary>
    public class LargestChainDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private bool checkAromaticity = false;
        private bool checkRingSystem = false;
        private static readonly string[] NAMES = { "nAtomLC" };

        /// <summary>
        /// Constructor for the LargestChain object.
        /// </summary>
        public LargestChainDescriptor() { }

        /// <inheritdoc/> 
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
         new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#largestChain",
                typeof(LargestChainDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the LargestChain object.
        /// <para>
        /// This descriptor takes two parameters, which should be Booleans to indicate whether
        /// aromaticity and ring member ship needs been checked (TRUE) or not (FALSE). The first
        /// parameter (aromaticity) is deprecated and ignored.
        /// </para>
        /// </summary>
        /// <exception cref="CDKException">if more than one parameter or a non-bool parameter is specified</exception>
        public override object[] Parameters
        {
            set
            {
                if (value.Length > 2)
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

        public override string[] DescriptorNames => NAMES;

        private DescriptorValue GetDummyDescriptorValue(Exception e)
        {
            return new DescriptorValue(_Specification, ParameterNames, Parameters, new IntegerResult(0), DescriptorNames, e);
        }

        /// <summary>
        /// Calculate the count of atoms of the largest chain in the supplied <see cref="IAtomContainer"/>.
        /// </summary>
        /// <param name="atomContainer">The <see cref="IAtomContainer"/> for which this descriptor is to be calculated</param>
        /// <returns>the number of atoms in the largest chain of this AtomContainer</returns>
        /// <seealso cref="Parameters"/>
        public override DescriptorValue Calculate(IAtomContainer atomContainer)
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
            IAtomContainer subset = SubsetMol(atomContainer, included);

            AllPairsShortestPaths apsp = new AllPairsShortestPaths(subset);

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

            return new DescriptorValue(_Specification, ParameterNames, Parameters, new IntegerResult(max), DescriptorNames);
        }

        /// <summary>
        /// Returns the specific type of the DescriptorResult object.
        /// <para>
        /// The return value from this method really indicates what type of result will
        /// be obtained from the <see cref="DescriptorValue"/> object. Note that the same result
        /// can be achieved by interrogating the <see cref="DescriptorValue"/> object; this method
        /// allows you to do the same thing, without actually calculating the descriptor.
        /// </para>
        /// </summary>
        /// <returns>an object that implements the <see cref="IDescriptorResult"/> interface indicating
        ///         the actual type of values returned by the descriptor in the <see cref="DescriptorValue"/> object</returns>
        public override IDescriptorResult DescriptorResultType { get; } = new IntegerResult(1);

        /// <summary>
        /// Gets the parameterNames attribute of the LargestPiSystemDescriptor object.
        /// </summary>
        /// <returns>The parameterNames value</returns>
        public override string[] ParameterNames { get; } = new string[] { "checkAromaticity", "checkRingSystem" };

        /// <summary>
        /// Gets the parameterType attribute of the LargestChainDescriptor object.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>An Object of class equal to that of the parameter being requested</returns>
        public override object GetParameterType(string name) => true;

        private static IAtomContainer SubsetMol(IAtomContainer mol, ISet<IAtom> include)
        {
            IAtomContainer cpy = mol.Builder.CreateAtomContainer();
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
    }
}
