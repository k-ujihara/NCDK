/* Copyright (C) 2004-2007  Matteo Floris <mfe4@users.sf.net>
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

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// <see cref="IDescriptor"/> based on the number of bonds of a certain bond order.
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
    ///     <term>order</term>
    ///     <term>""</term>
    ///     <term>The bond order</term>
    ///   </item>
    /// </list>
    /// </para>
    /// <para>
    /// Returns a single value with name <i>nBX</i> where <i>X</i> can be
    /// <list type="bullet">
    /// <item>s for single bonds</item>
    /// <item>d for double bonds</item>
    /// <item>t for triple bonds</item>
    /// <item>a for aromatic bonds</item>
    /// <item>"" for all bonds</item>
    /// </list>
    /// </para>
    /// Note that the descriptor does not consider bonds to H's.
    /// </remarks>
    // @author      mfe4
    // @cdk.created 2004-11-13
    // @cdk.module  qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:bondCount
    public class BondCountDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        /// <summary>defaults to UNSET, which means: count all bonds </summary>
        private string order = "";

        /// <summary>
        ///  Constructor for the BondCountDescriptor object
        /// </summary>
        public BondCountDescriptor() { }

        /// <summary>
        ///  The specification attribute of the BondCountDescriptor object
        /// </summary>
        public override IImplementationSpecification Specification => specification;
        private static readonly DescriptorSpecification specification =
         new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#bondCount",
                typeof(BondCountDescriptor).FullName,
                "The Chemistry Development Kit");

        public override IReadOnlyList<object> Parameters
        {
            set
            {
                if (value.Count > 1)
                {
                    throw new CDKException("BondCount only expects one parameter");
                }
                if (!(value[0] is string))
                {
                    throw new CDKException("The parameter must be of type BondOrder");
                }
                string bondType = (string)value[0];
                if (bondType.Length > 1 || !"sdtq".Contains(bondType))
                {
                    throw new CDKException("The only allowed values for this parameter are 's', 'd', 't', 'q' and ''.");
                }
                // ok, all should be fine
                order = bondType;
            }
            get
            {
                // return the parameters as used for the descriptor calculation
                return new object[] { order };
            }
        }

        public override IReadOnlyList<string> DescriptorNames
        {
            get
            {
                if (string.IsNullOrEmpty(order))
                    return new string[] { "nB" };
                else
                    return new string[] { "nB" + order };
            }
        }

        /// <summary>
        /// This method calculate the number of bonds of a given type in an atomContainer
        /// </summary>
        /// <param name="container">AtomContainer</param>
        /// <returns>The number of bonds of a certain type.</returns>
        public DescriptorValue<Result<int>> Calculate(IAtomContainer container)
        {
            if (string.IsNullOrEmpty(order))
            {
                int bondCount = 0;
                foreach (var bond in container.Bonds)
                {
                    bool hasHydrogen = false;
                    for (int i = 0; i < bond.Atoms.Count; i++)
                    {
                        if (string.Equals(bond.Atoms[i].Symbol, "H", StringComparison.Ordinal))
                        {
                            hasHydrogen = true;
                            break;
                        }
                    }
                    if (!hasHydrogen) bondCount++;
                }
                return new DescriptorValue<Result<int>>(specification, ParameterNames, Parameters, new Result<int>(bondCount), DescriptorNames, null);
            }
            else
            {
                int bondCount = 0;
                foreach (var bond in container.Bonds)
                {
                    if (BondMatch(bond.Order, order))
                    {
                        bondCount += 1;
                    }
                }
                return new DescriptorValue<Result<int>>(specification, ParameterNames, Parameters, new Result<int>(bondCount), DescriptorNames);
            }
        }

        private static bool BondMatch(BondOrder order, string orderString)
        {
            if (order == BondOrder.Single && "s".Equals(orderString, StringComparison.Ordinal))
                return true;
            else if (order == BondOrder.Double && "d".Equals(orderString, StringComparison.Ordinal))
                return true;
            else if (order == BondOrder.Triple && "t".Equals(orderString, StringComparison.Ordinal))
                return true;
            else
                return (order == BondOrder.Quadruple && "q".Equals(orderString, StringComparison.Ordinal));
        }

        /// <inheritdoc/>
        public override IDescriptorResult DescriptorResultType { get; } = new Result<int>(1);

        public override IReadOnlyList<string> ParameterNames { get; } = new string[] { "order" };

        public override object GetParameterType(string name)
        {
            if (string.Equals("order", name, StringComparison.Ordinal)) return "";
            return null;
        }

        IDescriptorValue IMolecularDescriptor.Calculate(IAtomContainer container) => Calculate(container);
    }
}
