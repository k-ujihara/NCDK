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
using NCDK.QSAR.Result;
using System;

namespace NCDK.QSAR.Descriptors.Bonds
{
    /// <summary>
    ///  The calculation of bond-sigma Partial charge is calculated
    ///  determining the difference the Partial Sigma Charge on atoms
    ///  A and B of a bond. Based in Gasteiger Charge.
    ///  <p>This descriptor uses these parameters:
    /// <table border="1">
    ///   <tr>
    ///     <td>Name</td>
    ///     <td>Default</td>
    ///     <td>Description</td>
    ///   </tr>
    ///   <tr>
    ///     <td>bondPosition</td>
    ///     <td>0</td>
    ///     <td>The position of the target bond</td>
    ///   </tr>
    /// </table>
    ///
    ///
    // @author      Miguel Rojas
    // @cdk.created 2006-05-08
    // @cdk.module  qsarbond
    // @cdk.githash
    // @cdk.set     qsar-descriptors
    // @cdk.dictref qsar-descriptors:bondPartialSigmaCharge
    ///
    // @see NCDK.QSAR.Descriptors.Atomic.PartialSigmaChargeDescriptor
    /// </summary>
    public class BondPartialSigmaChargeDescriptor : AbstractBondDescriptor
    {
        private GasteigerMarsiliPartialCharges peoe = null;
        /// <summary>Number of maximum iterations*/
        private int maxIterations;

        private static readonly string[] NAMES = { "peoeB" };

        /// <summary>
        ///  Constructor for the BondPartialSigmaChargeDescriptor object.
        /// </summary>
        public BondPartialSigmaChargeDescriptor()
        {
            peoe = new GasteigerMarsiliPartialCharges();
        }

        /// <summary>
        /// The specification attribute of the BondPartialSigmaChargeDescriptor object.
        /// </summary>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#bondPartialSigmaCharge",
                typeof(BondPartialSigmaChargeDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the BondPartialSigmaChargeDescriptor object
        /// </summary>
        public override object[] Parameters
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
                return new object[] { (int)maxIterations };
            }
        }

        public override string[] DescriptorNames => NAMES;

        private DescriptorValue GetDummyDescriptorValue(Exception e)
        {
            return new DescriptorValue(_Specification, ParameterNames, Parameters, new DoubleResult(double.NaN), NAMES, e);
        }

        /// <summary>
        ///  The method calculates the bond-sigma Partial charge of a given bond
        ///  It is needed to call the addExplicitHydrogensToSatisfyValency method from the class tools.HydrogenAdder.
        /// </summary>
        /// <param name="ac">AtomContainer</param>
        /// <returns>return the sigma electronegativity</returns>
        public override DescriptorValue Calculate(IBond bond, IAtomContainer ac)
        {
            // FIXME: for now I'll cache a few modified atomic properties, and restore them at the end of this method
            var originalCharge1 = bond.Atoms[0].Charge;
            var originalCharge2 = bond.Atoms[1].Charge;
            if (!IsCachedAtomContainer(ac))
            {
                IAtomContainer mol = ac.Builder.CreateAtomContainer(ac);
                if (maxIterations != 0) peoe.MaxGasteigerIterations = maxIterations;
                try
                {
                    peoe.AssignGasteigerMarsiliSigmaPartialCharges(mol, true);
                    foreach (var bondi in ac.Bonds)
                    {
                        double result = Math.Abs(bondi.Atoms[0].Charge.Value - bondi.Atoms[1].Charge.Value);
                        CacheDescriptorValue(bondi, ac, new DoubleResult(result));
                    }
                }
                catch (Exception ex1)
                {
                    return GetDummyDescriptorValue(ex1);
                }
            }
            bond.Atoms[0].Charge = originalCharge1;
            bond.Atoms[1].Charge = originalCharge2;
            return GetCachedDescriptorValue(bond) != null ? new DescriptorValue(_Specification, ParameterNames, Parameters, GetCachedDescriptorValue(bond), NAMES) : null;
        }

        /// <summary>
        /// The parameterNames attribute of the BondPartialSigmaChargeDescriptor object.
        /// </summary>
        public override string[] ParameterNames { get; } = new string[] { "maxIterations" };

        /// <summary>
        /// Gets the parameterType attribute of the BondPartialSigmaChargeDescriptor object.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>An Object of class equal to that of the parameter being requested</returns>
        public override object GetParameterType(string name)
        {
            if ("maxIterations".Equals(name)) return int.MaxValue;
            return null;
        }
    }
}
