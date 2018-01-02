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
    /// The calculation of sigma partial charges in sigma-bonded systems of an heavy atom
    /// was made by Marsilli-Gasteiger. It is implemented with the Partial Equalization
    /// of Orbital Electronegativity (PEOE).
    /// </summary>
    /// <remarks>
    /// <para>
    /// This descriptor uses these parameters:
    /// <list type="table">
    /// <listheader><term>Name</term><term>Default</term><term>Description</term></listheader>
    /// <item><term>maxIterations</term><term>0</term><term>Number of maximum iterations</term></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <seealso cref="GasteigerMarsiliPartialCharges"/>
    // @author      Miguel Rojas
    // @cdk.created 2006-04-15
    // @cdk.module  qsaratomic
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:partialSigmaCharge
    public partial class PartialSigmaChargeDescriptor : IAtomicDescriptor
    {
        private static readonly string[] NAMES = { "partialSigmaCharge" };

        private GasteigerMarsiliPartialCharges peoe = null;
        /// <summary>Number of maximum iterations</summary>
        private int maxIterations;

        /// <summary>
        ///  Constructor for the PartialSigmaChargeDescriptor object
        /// </summary>
        public PartialSigmaChargeDescriptor()
        {
            peoe = new GasteigerMarsiliPartialCharges();
        }

        /// <summary>
        /// The specification attribute of the PartialSigmaChargeDescriptor object
        /// </summary>
        public IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#partialSigmaCharge",
                typeof(PartialSigmaChargeDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the PartialSigmaChargeDescriptor object
        /// <para>Number of maximum iterations</para>
        /// </summary>
        /// <exception cref="CDKException"></exception>
        public object[] Parameters
        {
            set
            {
                if (value.Length > 1)
                {
                    throw new CDKException("PartialSigmaChargeDescriptor only expects one parameter");
                }
                if (!(value[0] is int))
                {
                    throw new CDKException("The parameter 1 must be of type int");
                }
                maxIterations = (int)value[0];
            }
            get
            {
                // return the parameters as used for the descriptor calculation
                return new object[] { maxIterations };
            }
        }

        public IReadOnlyList<string> DescriptorNames => NAMES;

        /// <summary>
        ///  The method returns apha partial charges assigned to an heavy atom through Gasteiger Marsili
        ///  It is needed to call the addExplicitHydrogensToSatisfyValency method from the class tools.HydrogenAdder.
        ///  For this method will be only possible if the heavy atom has single bond.
        /// </summary>
        /// <param name="atom">The <see cref="IAtom"/> for which the <see cref="IDescriptorValue"/> is requested</param>
        /// <param name="ac">AtomContainer</param>
        /// <returns>Value of the alpha partial charge</returns>
        public DescriptorValue<Result<double>> Calculate(IAtom atom, IAtomContainer ac)
        {
            // FIXME: for now I'll cache the original charges, and restore them at the end of this method
            var originalCharge = atom.Charge;
            if (!IsCachedAtomContainer(ac))
            {
                IAtomContainer mol = atom.Builder.NewAtomContainer(ac);
                if (maxIterations != 0) peoe.MaxGasteigerIterations = maxIterations;
                try
                {
                    peoe.AssignGasteigerMarsiliSigmaPartialCharges(mol, true);

                    for (int i = 0; i < ac.Atoms.Count; i++)
                    {
                        // assume same order, so mol.Atoms[i] == ac.Atoms[i]
                        CacheDescriptorValue(ac.Atoms[i], ac, new Result<double>(mol.Atoms[i].Charge.Value));
                    }
                }
                catch (Exception e)
                {
                    return new DescriptorValue<Result<double>>(_Specification, ParameterNames, Parameters, new Result<double>(double.NaN), NAMES, e);
                }
            }
            atom.Charge = originalCharge;

            return GetCachedDescriptorValue(atom) != null ? new DescriptorValue<Result<double>>(_Specification, ParameterNames,
                    Parameters, (Result<double>)GetCachedDescriptorValue(atom), NAMES) : null;
        }

        /// <summary>
        ///  The parameterNames attribute of the PartialSigmaChargeDescriptor object
        /// </summary>
        public IReadOnlyList<string> ParameterNames { get; } = new string[] { "maxIterations" };

        /// <summary>
        ///  Gets the parameterType attribute of the PartialSigmaChargeDescriptor object
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public object GetParameterType(string name)
        {
            if ("maxIterations".Equals(name)) return int.MaxValue;
            return null;
        }
    }
}
