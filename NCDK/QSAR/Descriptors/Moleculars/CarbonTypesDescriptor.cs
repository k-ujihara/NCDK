/* Copyright (C) 2007  Rajarshi Guha <rajarshi@users.sourceforge.net>
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
using NCDK.QSAR.Results;
using NCDK.Tools.Manipulator;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Topological descriptor characterizing the carbon connectivity.
    /// </summary>
    /// <remarks>
    /// The class calculates 9 descriptors in the following order
    /// <list type="bullet">
    /// <item>C1SP1 triply hound carbon bound to one other carbon</item>
    /// <item>C2SP1    triply bound carbon bound to two other carbons</item>
    /// <item>C1SP2    doubly hound carbon bound to one other carbon</item>
    /// <item>C2SP2    doubly bound carbon bound to two other carbons</item>
    /// <item>C3SP2    doubly bound carbon bound to three other carbons</item>
    /// <item>C1SP3    singly bound carbon bound to one other carbon</item>
    /// <item>C2SP3    singly bound carbon bound to two other carbons</item>
    /// <item>C3SP3    singly bound carbon bound to three other carbons</item>
    /// <item>C4SP3    singly bound carbon bound to four other carbons</item>
    /// </list>
    /// <para>This descriptor uses these parameters:
    /// <list type="table">
    /// <listheader><term>Name</term><term>Default</term><term>Description</term></listheader>
    /// <item><term></term><term></term><term>no parameters</term></item>
    /// </list>
    /// </para>
    /// </remarks>
    // @author Rajarshi Guha
    // @cdk.created 2007-09-28
    // @cdk.module qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:carbonTypes
    // @cdk.keyword topological bond order ctypes
    // @cdk.keyword descriptor
    public partial class CarbonTypesDescriptor : IMolecularDescriptor
    {
        private readonly static string[] NAMES = { "C1SP1", "C2SP1", "C1SP2", "C2SP2", "C3SP2", "C1SP3", "C2SP3", "C3SP3", "C4SP3" };

        public CarbonTypesDescriptor() { }

        public IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
         new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#carbonTypes",
                typeof(CarbonTypesDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <summary>
        /// Sets the parameters attribute of the GravitationalIndexDescriptor object.
        /// </summary>
        /// <exception cref="CDKException"></exception>
        public object[] Parameters
        {
            set
            {
                // no parameters for this descriptor
            }
            get
            {
                // no parameters to return
                return (null);
            }
        }

        public IReadOnlyList<string> DescriptorNames => NAMES;

        /// <summary>
        /// The parameterNames attribute of the GravitationalIndexDescriptor object.
        /// </summary>
        public IReadOnlyList<string> ParameterNames => null; // no param names to return

        /// <summary>
        /// Gets the parameterType attribute of the GravitationalIndexDescriptor object.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public object GetParameterType(string name) => null;

        /// <summary>
        /// Calculates the 9 carbon types descriptors
        /// </summary>
        /// <param name="container">Parameter is the atom container.</param>
        /// <returns>An ArrayList containing 9 elements in the order described above</returns>
        public DescriptorValue<ArrayResult<int>> Calculate(IAtomContainer container)
        {
            int c1sp1 = 0;
            int c2sp1 = 0;
            int c1sp2 = 0;
            int c2sp2 = 0;
            int c3sp2 = 0;
            int c1sp3 = 0;
            int c2sp3 = 0;
            int c3sp3 = 0;
            int c4sp3 = 0;

            foreach (var atom in container.Atoms)
            {
                if (!atom.Symbol.Equals("C") && !atom.Symbol.Equals("c")) continue;
                var connectedAtoms = container.GetConnectedAtoms(atom);

                int cc = 0;
                foreach (var connectedAtom in connectedAtoms)
                {
                    if (connectedAtom.Symbol.Equals("C") || connectedAtom.Symbol.Equals("c")) cc++;
                }

                BondOrder maxBondOrder = GetHighestBondOrder(container, atom);

                if (maxBondOrder == BondOrder.Triple && cc == 1)
                    c1sp1++;
                else if (maxBondOrder == BondOrder.Triple && cc == 2)
                    c2sp1++;
                else if (maxBondOrder == BondOrder.Double && cc == 1)
                    c1sp2++;
                else if (maxBondOrder == BondOrder.Double && cc == 2)
                    c2sp2++;
                else if (maxBondOrder == BondOrder.Double && cc == 3)
                    c3sp2++;
                else if (maxBondOrder == BondOrder.Single && cc == 1)
                    c1sp3++;
                else if (maxBondOrder == BondOrder.Single && cc == 2)
                    c2sp3++;
                else if (maxBondOrder == BondOrder.Single && cc == 3)
                    c3sp3++;
                else if (maxBondOrder == BondOrder.Single && cc == 4) c4sp3++;
            }

            ArrayResult<int> retval = new ArrayResult<int>(9);
            retval.Add(c1sp1);
            retval.Add(c2sp1);
            retval.Add(c1sp2);
            retval.Add(c2sp2);
            retval.Add(c3sp2);
            retval.Add(c1sp3);
            retval.Add(c2sp3);
            retval.Add(c3sp3);
            retval.Add(c4sp3);

            return new DescriptorValue<ArrayResult<int>>(_Specification, ParameterNames, Parameters, retval, DescriptorNames);
        }

        private BondOrder GetHighestBondOrder(IAtomContainer container, IAtom atom)
        {
            var bonds = container.GetConnectedBonds(atom);
            BondOrder maxOrder = BondOrder.Single;
            foreach (var bond in bonds)
            {
                if (BondManipulator.IsHigherOrder(bond.Order, maxOrder)) maxOrder = bond.Order;
            }
            return maxOrder;
        }

        /// <inheritdoc/>
        public IDescriptorResult DescriptorResultType { get; } = new ArrayResult<int>(9);
    }
}
