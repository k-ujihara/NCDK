/* Copyright (C) 2006-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.ForceField.MMFF;
using NCDK.QSAR.Results;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    /// The calculation of total partial charges of an heavy atom is based on MMFF94 model.
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
    /// <seealso cref="Charges.MMFF94PartialCharges"/>
    // @author Miguel Rojas
    // @cdk.created 2006-04-11
    // @cdk.module qsaratomic
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:partialTChargeMMFF94
    // @cdk.bug 1628461
    public partial class PartialTChargeMMFF94Descriptor : IAtomicDescriptor
    {
        private static readonly string[] NAMES = { "partialTCMMFF94" };
        private static readonly string CHARGE_CACHE = "mmff.qsar.charge.cache";
        private Mmff mmff;

        /// <summary>
        /// Constructor for the PartialTChargeMMFF94Descriptor object
        /// </summary>
        public PartialTChargeMMFF94Descriptor()
        {
            mmff = new Mmff();
        }

        /// <summary>
        /// The specification attribute of the PartialTChargeMMFF94Descriptor object
        /// </summary>
        public IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#partialTChargeMMFF94",
                typeof(PartialTChargeMMFF94Descriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the PartialTChargeMMFF94Descriptor object
        /// </summary>
        public object[] Parameters { get { return null; } set { } }

        public IReadOnlyList<string> DescriptorNames => NAMES;

        /// <summary>
        /// The method returns partial charges assigned to an heavy atom through
        /// MMFF94 method. It is needed to call the addExplicitHydrogensToSatisfyValency
        /// method from the class tools.HydrogenAdder.
        /// </summary>
        /// <param name="atom">The <see cref="IAtom"/> for which the <see cref="DescriptorValue"/> is requested</param>
        /// <param name="org">AtomContainer</param>
        /// <returns>partial charge of parameter atom</returns>
        public DescriptorValue<Result<double>> Calculate(IAtom atom, IAtomContainer org)
        {
            if (atom.GetProperty<double?>(CHARGE_CACHE) == null)
            {
                IAtomContainer copy = (IAtomContainer)org.Clone();
                foreach (var a in org.Atoms)
                {
                    if (a.ImplicitHydrogenCount == null || a.ImplicitHydrogenCount != 0)
                    {
                        Trace.TraceError("Hydrogens must be explict for MMFF charge calculation");
                        return new DescriptorValue<Result<double>>(_Specification, ParameterNames, Parameters,
                                                   new Result<double>(double.NaN), NAMES);
                    }
                }

                if (!mmff.AssignAtomTypes(copy))
                    Trace.TraceWarning("One or more atoms could not be assigned an MMFF atom type");
                mmff.PartialCharges(copy);
                mmff.ClearProps(copy);

                // cache charges
                for (int i = 0; i < org.Atoms.Count; i++)
                {
                    org.Atoms[i].SetProperty(CHARGE_CACHE, copy.Atoms[i].Charge.Value);
                }
            }

            return new DescriptorValue<Result<double>>(_Specification,
                                       ParameterNames,
                                       Parameters,
                                       new Result<double>(atom.GetProperty<double>(CHARGE_CACHE)),
                                       NAMES);
        }

        /// <summary>
        /// The parameterNames attribute of the PartialTChargeMMFF94Descriptor object
        /// </summary>
        public IReadOnlyList<string> ParameterNames { get; } = Array.Empty<string>();

        /// <summary>
        /// Gets the parameterType attribute of the PartialTChargeMMFF94Descriptor object
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public object GetParameterType(string name) => null;
    }
}
