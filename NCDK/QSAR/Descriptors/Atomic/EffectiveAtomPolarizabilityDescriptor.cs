/* Copyright (C) 2004-2007  Miguel Rojas <miguel.rojas@uni-koeln.de>
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
using NCDK.QSAR.Results;
using System;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    /// Effective polarizability of a heavy atom
    /// </summary>
    /// <remarks>
    /// This descriptor uses these parameters:
    /// <list type="table">
    ///   <item>
    ///     <term>Name</term>
    ///     <term>Default</term>
    ///     <term>Description</term>
    ///   </item>
    ///   <item>
    ///     <term></term>
    ///     <term></term>
    ///     <term>no parameters</term>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso cref="Polarizability"/>
    // @author      Miguel Rojas
    // @cdk.created 2006-05-03
    // @cdk.module  qsaratomic
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:effectivePolarizability
    public partial class EffectiveAtomPolarizabilityDescriptor : IAtomicDescriptor
    {
        private Polarizability pol;

        /// <summary>
        ///  Constructor for the EffectiveAtomPolarizabilityDescriptor object
        /// </summary>
        public EffectiveAtomPolarizabilityDescriptor()
        {
            pol = new Polarizability();
        }

        /// <summary>
        /// The specification attribute of the EffectiveAtomPolarizabilityDescriptor object
        /// </summary>
        public IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#effectivePolarizability",
                typeof(EffectiveAtomPolarizabilityDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the EffectiveAtomPolarizabilityDescriptor object
        /// </summary>
        public object[] Parameters { get { return null; } set { } }

        public IReadOnlyList<string> DescriptorNames { get; } = new string[] { "effAtomPol" };

        /// <summary>
        ///  The method calculates the Effective Atom Polarizability of a given atom
        ///  It is needed to call the addExplicitHydrogensToSatisfyValency method from the class tools.HydrogenAdder.
        /// </summary>
        /// <param name="atom">The <see cref="IAtom"/> for which the <see cref="IDescriptorValue"/> is requested</param>
        /// <param name="ac">AtomContainer</param>
        /// <returns>return the effective polarizability</returns>
        public DescriptorValue<Result<double>> Calculate(IAtom atom, IAtomContainer ac)
        {
            double polarizability;
            try
            {
                // FIXME: for now I'll cache a few modified atomic properties, and restore them at the end of this method
                var originalAtomtypeName = atom.AtomTypeName;
                var originalNeighborCount = atom.FormalNeighbourCount;
                var originalHCount = atom.ImplicitHydrogenCount;
                var originalValency = atom.Valency;
                var originalHybridization = atom.Hybridization;
                var originalFlag = atom.IsVisited;
                var originalBondOrderSum = atom.BondOrderSum;
                var originalMaxBondOrder = atom.MaxBondOrder;
                polarizability = pol.CalculateGHEffectiveAtomPolarizability(ac, atom, 100, true);
                // restore original props
                atom.AtomTypeName = originalAtomtypeName;
                atom.FormalNeighbourCount = originalNeighborCount;
                atom.Valency = originalValency;
                atom.ImplicitHydrogenCount = originalHCount;
                atom.IsVisited = originalFlag;
                atom.Hybridization = originalHybridization;
                atom.MaxBondOrder = originalMaxBondOrder;
                atom.BondOrderSum = originalBondOrderSum;
                return new DescriptorValue<Result<double>>(_Specification, ParameterNames, Parameters, new Result<double>(polarizability), DescriptorNames);
            }
            catch (Exception ex1)
            {
                return new DescriptorValue<Result<double>>(_Specification, ParameterNames, Parameters, new Result<double>(double.NaN), DescriptorNames, ex1);
            }
        }

        /// <summary>
        /// The parameterNames attribute of the EffectiveAtomPolarizabilityDescriptor object
        /// </summary>
        public IReadOnlyList<string> ParameterNames { get; } = Array.Empty<string>();

        /// <summary>
        ///  Gets the parameterType attribute of the EffectiveAtomPolarizabilityDescriptor object
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public object GetParameterType(string name) => null;
    }
}
