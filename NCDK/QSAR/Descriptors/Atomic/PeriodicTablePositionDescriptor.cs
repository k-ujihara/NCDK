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
using NCDK.QSAR.Result;
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
    public class PeriodicTablePositionDescriptor : AbstractAtomicDescriptor, IAtomicDescriptor
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
                periodicTable = new Dictionary<string, int>();
                periodicTable["H"] = 1;
                periodicTable["Li"] = 2;
                periodicTable["Be"] = 2;
                periodicTable["B"] = 2;
                periodicTable["C"] = 2;
                periodicTable["N"] = 2;
                periodicTable["O"] = 2;
                periodicTable["F"] = 2;
                periodicTable["Na"] = 3;
                periodicTable["Mg"] = 3;
                periodicTable["Al"] = 3;
                periodicTable["Si"] = 3;
                periodicTable["P"] = 3;
                periodicTable["S"] = 3;
                periodicTable["Cl"] = 3;
                periodicTable["K"] = 4;
                periodicTable["Ca"] = 4;
                periodicTable["Ga"] = 4;
                periodicTable["Ge"] = 4;
                periodicTable["As"] = 4;
                periodicTable["Se"] = 4;
                periodicTable["Br"] = 4;
                periodicTable["Rb"] = 5;
                periodicTable["Sr"] = 5;
                periodicTable["In"] = 5;
                periodicTable["Sn"] = 5;
                periodicTable["Sb"] = 5;
                periodicTable["Te"] = 5;
                periodicTable["I"] = 5;
                periodicTable["Cs"] = 6;
                periodicTable["Ba"] = 6;
                periodicTable["Tl"] = 6;
                periodicTable["Pb"] = 6;
                periodicTable["Bi"] = 6;
                periodicTable["Po"] = 6;
                periodicTable["At"] = 6;
                periodicTable["Fr"] = 7;
                periodicTable["Ra"] = 7;
            }
        }

        /// <summary>
        /// The specification attribute of the PeriodicTablePositionDescriptor object
        /// </summary>
        /// <returns>The specification value</returns>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#period",
                typeof(PeriodicTablePositionDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the PeriodicTablePositionDescriptor object
        /// </summary>
        public override object[] Parameters { get { return null; } set { } }

        public override string[] DescriptorNames => NAMES;

        /// <summary>
        ///  This method calculates the period of an atom.
        /// </summary>
        /// <param name="atom">The <see cref="IAtom"/> for which the <see cref="DescriptorValue"/> is requested</param>
        /// <param name="container">Parameter is the atom container.</param>
        /// <returns>The period</returns>
        public override DescriptorValue Calculate(IAtom atom, IAtomContainer container)
        {
            int period;
            string symbol = atom.Symbol;
            period = periodicTable[symbol];
            return new DescriptorValue(_Specification, ParameterNames, Parameters, new IntegerResult(period), NAMES);
        }

        /// <summary>
        /// The parameterNames attribute of the PeriodicTablePositionDescriptor object
        /// </summary>
        public override string[] ParameterNames { get; } = Array.Empty<string>();

        /// <summary>
        ///  Gets the parameterType attribute of the PeriodicTablePositionDescriptor object
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public override object GetParameterType(string name) => null;
    }
}
