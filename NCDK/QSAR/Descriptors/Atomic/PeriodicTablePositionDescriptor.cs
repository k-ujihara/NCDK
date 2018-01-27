/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.QSAR.Results;
using System;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    ///  This class returns the period in the periodic table of an atom belonging to an atom container
    /// </summary>
    /// <remarks>
    /// <para>
    /// This descriptor uses these parameters:
    /// <list type="table">
    /// <listheader><term>Name</term><term>Default</term><term>Description</term></listheader>
    /// <item><term></term><term></term><term>no parameters</term></item>
    /// </list>
    /// </para> 
    /// </remarks> 
    // @author         mfe4
    // @cdk.created    2004-11-13
    // @cdk.module     qsaratomic
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:period
    public partial class PeriodicTablePositionDescriptor : IAtomicDescriptor
    {
        private static readonly string[] NAMES = { "periodicTablePosition" };
        public IDictionary<string, int> periodicTable;

        /// <summary>
        ///  Constructor for the PeriodicTablePositionDescriptor object
        /// </summary>
        public PeriodicTablePositionDescriptor()
        {
            if (periodicTable == null)
            {
                periodicTable = new Dictionary<string, int>
                {
                    ["H"] = 1,
                    ["Li"] = 2,
                    ["Be"] = 2,
                    ["B"] = 2,
                    ["C"] = 2,
                    ["N"] = 2,
                    ["O"] = 2,
                    ["F"] = 2,
                    ["Na"] = 3,
                    ["Mg"] = 3,
                    ["Al"] = 3,
                    ["Si"] = 3,
                    ["P"] = 3,
                    ["S"] = 3,
                    ["Cl"] = 3,
                    ["K"] = 4,
                    ["Ca"] = 4,
                    ["Ga"] = 4,
                    ["Ge"] = 4,
                    ["As"] = 4,
                    ["Se"] = 4,
                    ["Br"] = 4,
                    ["Rb"] = 5,
                    ["Sr"] = 5,
                    ["In"] = 5,
                    ["Sn"] = 5,
                    ["Sb"] = 5,
                    ["Te"] = 5,
                    ["I"] = 5,
                    ["Cs"] = 6,
                    ["Ba"] = 6,
                    ["Tl"] = 6,
                    ["Pb"] = 6,
                    ["Bi"] = 6,
                    ["Po"] = 6,
                    ["At"] = 6,
                    ["Fr"] = 7,
                    ["Ra"] = 7
                };
            }
        }

        /// <summary>
        /// The specification attribute of the PeriodicTablePositionDescriptor object
        /// </summary>
        /// <returns>The specification value</returns>
        public IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#period",
                typeof(PeriodicTablePositionDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the PeriodicTablePositionDescriptor object
        /// </summary>
        public object[] Parameters { get { return null; } set { } }

        public IReadOnlyList<string> DescriptorNames => NAMES;

        /// <summary>
        ///  This method calculates the period of an atom.
        /// </summary>
        /// <param name="atom">The <see cref="IAtom"/> for which the <see cref="IDescriptorValue"/> is requested</param>
        /// <param name="container">Parameter is the atom container.</param>
        /// <returns>The period</returns>
        public DescriptorValue<Result<int>> Calculate(IAtom atom, IAtomContainer container)
        {
            int period;
            string symbol = atom.Symbol;
            period = periodicTable[symbol];
            return new DescriptorValue<Result<int>>(_Specification, ParameterNames, Parameters, new Result<int>(period), NAMES);
        }

        /// <summary>
        /// The parameterNames attribute of the PeriodicTablePositionDescriptor object
        /// </summary>
        public IReadOnlyList<string> ParameterNames { get; } = Array.Empty<string>();

        /// <summary>
        ///  Gets the parameterType attribute of the PeriodicTablePositionDescriptor object
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public object GetParameterType(string name) => null;
    }
}
