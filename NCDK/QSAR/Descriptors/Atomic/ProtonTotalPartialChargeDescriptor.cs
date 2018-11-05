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
using NCDK.Charges;
using NCDK.Config;
using NCDK.QSAR.Results;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    /// The calculation of partial charges of an heavy atom and its protons is based on Gasteiger Marsili (PEOE).
    ///
    /// This descriptor has no parameters. The result of this descriptor is a vector of 5 values, corresponding
    /// to a maximum of four protons for any given atom. If an atom has fewer than four protons, the remaining values
    /// are set to double.NaN. Also note that the values for the neighbors are not returned in a particular order
    /// (though the order is fixed for multiple runs for the same atom).
    /// </summary>
    // @author mfe4
    // @cdk.created 2004-11-03
    // @cdk.module qsaratomic
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:protonPartialCharge
    public partial class ProtonTotalPartialChargeDescriptor : IAtomicDescriptor
    {
        private GasteigerMarsiliPartialCharges peoe = null;
        private IReadOnlyList<IAtom> neighboors;
        private const int MAX_PROTON_COUNT = 5;

        /// <summary>
        ///  Constructor for the ProtonTotalPartialChargeDescriptor object
        /// </summary>
        public ProtonTotalPartialChargeDescriptor() { }

        /// <summary>
        /// The specification attribute of the ProtonTotalPartialChargeDescriptor object
        /// </summary>
        public IImplementationSpecification Specification => specification;
        private static readonly DescriptorSpecification specification =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#protonPartialCharge",
                typeof(ProtonTotalPartialChargeDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the ProtonTotalPartialChargeDescriptor
        /// </summary>
        public IReadOnlyList<object> Parameters { get { return null; } set { } }

        public IReadOnlyList<string> DescriptorNames { get; } = MakeDescriptorNames();
        private static string[] MakeDescriptorNames()
        {
            string[] labels = new string[MAX_PROTON_COUNT];
            for (int i = 0; i < MAX_PROTON_COUNT; i++)
            {
                labels[i] = "protonTotalPartialCharge" + (i + 1);
            }
            return labels;
        }

        private DescriptorValue<ArrayResult<double>> GetDummyDescriptorValue(Exception e)
        {
            var result = new ArrayResult<double>(MAX_PROTON_COUNT);
            for (int i = 0; i < neighboors.Count + 1; i++)
                result.Add(double.NaN);
            return new DescriptorValue<ArrayResult<double>>(specification, ParameterNames, Parameters, result, DescriptorNames, e);
        }

        /// <summary>
        ///  The method returns partial charges assigned to an heavy atom and its protons through Gasteiger Marsili
        ///  It is needed to call the addExplicitHydrogensToSatisfyValency method from the class tools.HydrogenAdder.
        /// </summary>
        /// <param name="atom">The <see cref="IAtom"/> for which the <see cref="IDescriptorValue"/> is requested</param>
        /// <param name="ac">AtomContainer</param>
        /// <returns>an array of doubles with partial charges of [heavy, proton_1 ... proton_n]</returns>
        public DescriptorValue<ArrayResult<double>> Calculate(IAtom atom, IAtomContainer ac)
        {
            var clone = (IAtomContainer)ac.Clone();
            try
            {
                peoe = new GasteigerMarsiliPartialCharges { MaxGasteigerIterations = 6 };
                peoe.AssignGasteigerMarsiliSigmaPartialCharges(clone, true);
            }
            catch (Exception exception)
            {
                return GetDummyDescriptorValue(exception);
            }

            var localAtom = clone.Atoms[ac.Atoms.IndexOf(atom)];
            neighboors = clone.GetConnectedAtoms(localAtom).ToReadOnlyList();

            // we assume that an atom has a max number of protons = MAX_PROTON_COUNT
            // if it has less, we pad with NaN
            var protonPartialCharge = new ArrayResult<double>(MAX_PROTON_COUNT);
            Trace.Assert(neighboors.Count < MAX_PROTON_COUNT);

            protonPartialCharge.Add(localAtom.Charge.Value);
            int hydrogenNeighbors = 0;
            foreach (var neighboor in neighboors)
            {
                if (neighboor.AtomicNumber.Equals(NaturalElements.H.AtomicNumber))
                {
                    hydrogenNeighbors++;
                    protonPartialCharge.Add(neighboor.Charge.Value);
                }
            }
            var remainder = MAX_PROTON_COUNT - (hydrogenNeighbors + 1);
            for (int i = 0; i < remainder; i++)
                protonPartialCharge.Add(double.NaN);

            return new DescriptorValue<ArrayResult<double>>(specification, ParameterNames, Parameters, protonPartialCharge, DescriptorNames);
        }

        /// <summary>
        /// The parameterNames attribute of the ProtonTotalPartialChargeDescriptor object
        /// </summary>
        public IReadOnlyList<string> ParameterNames { get; } = Array.Empty<string>();

        /// <summary>
        ///  Gets the parameterType attribute of the ProtonTotalPartialChargeDescriptor object
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>An Object of class equal to that of the parameter being requested</returns>
        public object GetParameterType(string name) => null;
    }
}
