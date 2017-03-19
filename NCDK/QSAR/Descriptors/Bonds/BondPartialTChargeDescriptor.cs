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
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Bonds
{
    /// <summary>
    ///  The calculation of bond total Partial charge is calculated
    ///  determining the difference the Partial Total Charge on atoms
    ///  A and B of a bond. Based in Gasteiger Charge.
    ///  </summary>
    /// <remarks>
    ///  <para>
    ///  This descriptor uses these parameters:
    /// <list type="table">
    /// <listheader><term>Name</term><term>Default</term><term>Description</term></listheader>
    /// <item><term>bondPosition</term><term>0</term><term>The position of the target bond</term></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <seealso cref="Atomic.PartialPiChargeDescriptor"/>
    /// <seealso cref="Atomic.PartialSigmaChargeDescriptor"/>
    // @author      Miguel Rojas
    // @cdk.created 2006-05-18
    // @cdk.module  qsarbond
    // @cdk.githash
    // @cdk.set     qsar-descriptors
    // @cdk.dictref qsar-descriptors:bondPartialTCharge
    public class BondPartialTChargeDescriptor : AbstractBondDescriptor
    {
        private GasteigerMarsiliPartialCharges peoe = null;
        private GasteigerPEPEPartialCharges pepe = null;

       /// <summary>Number of maximum iterations</summary>
        private int maxIterations = -1;
       /// <summary>Number of maximum resonance structures</summary>
        private int maxResonStruc = -1;
       /// <summary> make a lone pair electron checker. Default true</summary>
        private bool lpeChecker = true;

        private static readonly string[] NAMES = { "pCB" };

        /// <summary>
        ///  Constructor for the BondPartialTChargeDescriptor object.
        /// </summary>
        public BondPartialTChargeDescriptor()
        {
            peoe = new GasteigerMarsiliPartialCharges();
            pepe = new GasteigerPEPEPartialCharges();
        }

        /// <summary>
        /// The specification attribute of the BondPartialTChargeDescriptor object.
        /// </summary>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#bondPartialTCharge",
                typeof(BondPartialTChargeDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the BondPartialTChargeDescriptor object.
        /// </summary>
        public override object[] Parameters
        {
            set
            {
                if (value.Length > 3)
                    throw new CDKException("PartialPiChargeDescriptor only expects three parameter");

                if (!(value[0] is int))
                    throw new CDKException("The parameter must be of type int");
                maxIterations = (int)value[0];

                if (value.Length > 1 && value[1] != null)
                {
                    if (!(value[1] is bool))
                        throw new CDKException("The parameter must be of type bool");
                    lpeChecker = (bool)value[1];
                }

                if (value.Length > 2 && value[2] != null)
                {
                    if (!(value[2] is int))
                        throw new CDKException("The parameter must be of type int");
                    maxResonStruc = (int)value[2];
                }
            }
            get
            {
                // return the parameters as used for the descriptor calculation
                return new object[] { maxIterations, lpeChecker, maxResonStruc };
            }
        }

        public override string[] DescriptorNames => NAMES;

        private DescriptorValue GetDummyDescriptorValue(Exception e)
        {
            return new DescriptorValue(_Specification, ParameterNames, Parameters, new DoubleResult(double.NaN), NAMES, e);
        }

        /// <summary>
        ///  The method calculates the bond total Partial charge of a given bond
        ///  It is needed to call the addExplicitHydrogensToSatisfyValency method from the class tools.HydrogenAdder.
        /// </summary>
        /// <param name="ac">AtomContainer</param>
        /// <returns>return the sigma electronegativity</returns>
        public override DescriptorValue Calculate(IBond bond, IAtomContainer ac)
        {
            // FIXME: for now I'll cache a few modified atomic properties, and restore them at the end of this method
            var originalCharge1 = bond.Atoms[0].Charge;
            var originalAtomtypeName1 = bond.Atoms[0].AtomTypeName;
            var originalNeighborCount1 = bond.Atoms[0].FormalNeighbourCount;
            var originalHybridization1 = bond.Atoms[0].Hybridization;
            var originalValency1 = bond.Atoms[0].Valency;
            var originalCharge2 = bond.Atoms[1].Charge;
            var originalAtomtypeName2 = bond.Atoms[1].AtomTypeName;
            var originalNeighborCount2 = bond.Atoms[1].FormalNeighbourCount;
            var originalHybridization2 = bond.Atoms[1].Hybridization;
            var originalValency2 = bond.Atoms[1].Valency;
            var originalBondOrderSum1 = bond.Atoms[0].BondOrderSum;
            var originalMaxBondOrder1 = bond.Atoms[0].MaxBondOrder;
            var originalBondOrderSum2 = bond.Atoms[1].BondOrderSum;
            var originalMaxBondOrder2 = bond.Atoms[1].MaxBondOrder;
            if (!IsCachedAtomContainer(ac))
            {
                try
                {
                    AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(ac);
                    if (lpeChecker)
                    {
                        LonePairElectronChecker lpcheck = new LonePairElectronChecker();
                        lpcheck.Saturate(ac);
                    }
                }
                catch (CDKException e)
                {
                    return GetDummyDescriptorValue(e);
                }

                if (maxIterations != -1) pepe.MaxGasteigerIterations = maxIterations;
                if (maxResonStruc != -1) pepe.MaxResonanceStructures = maxResonStruc;

                try
                {
                    peoe.AssignGasteigerMarsiliSigmaPartialCharges(ac, true);
                    var peoeBond = new List<double>();
                    foreach (var bondi in ac.Bonds)
                    {
                        double result = Math.Abs(bondi.Atoms[0].Charge.Value - bondi.Atoms[1].Charge.Value);
                        peoeBond.Add(result);
                    }

                    foreach (var atom in ac.Atoms)
                        atom.Charge = 0.0;

                    pepe.AssignGasteigerPiPartialCharges(ac, true);
                    for (int i = 0; i < ac.Bonds.Count; i++)
                    {
                        IBond bondi = ac.Bonds[i];
                        double result = Math.Abs(bondi.Atoms[0].Charge.Value - bondi.Atoms[1].Charge.Value);
                        CacheDescriptorValue(bondi, ac, new DoubleResult(peoeBond[i] + result));
                    }
                }
                catch (Exception e)
                {
                    return GetDummyDescriptorValue(e);
                }
            }
            bond.Atoms[0].Charge = originalCharge1;
            bond.Atoms[0].AtomTypeName = originalAtomtypeName1;
            bond.Atoms[0].Hybridization = originalHybridization1;
            bond.Atoms[0].Valency = originalValency1;
            bond.Atoms[0].FormalNeighbourCount = originalNeighborCount1;
            bond.Atoms[1].Charge = originalCharge2;
            bond.Atoms[1].AtomTypeName = originalAtomtypeName2;
            bond.Atoms[1].Hybridization = originalHybridization2;
            bond.Atoms[1].Valency = originalValency2;
            bond.Atoms[1].FormalNeighbourCount = originalNeighborCount2;
            bond.Atoms[0].MaxBondOrder = originalMaxBondOrder1;
            bond.Atoms[0].BondOrderSum = originalBondOrderSum1;
            bond.Atoms[1].MaxBondOrder = originalMaxBondOrder2;
            bond.Atoms[1].BondOrderSum = originalBondOrderSum2;

            return GetCachedDescriptorValue(bond) != null ? new DescriptorValue(_Specification, ParameterNames,
                    Parameters, GetCachedDescriptorValue(bond), NAMES) : null;
        }

        /// <summary>
        /// Gets the parameterNames attribute of the BondPartialTChargeDescriptor object.
        /// </summary>
        public override string[] ParameterNames { get; } = new string[] { "maxIterations", "lpeChecker", "maxResonStruc" };

        /// <summary>
        /// Gets the parameterType attribute of the BondPartialTChargeDescriptor object.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>An Object of class equal to that of the parameter being requested</returns>
        public override object GetParameterType(string name)
        {
            if ("maxIterations".Equals(name)) return int.MaxValue;
            if ("lpeChecker".Equals(name)) return true;
            if ("maxResonStruc".Equals(name)) return int.MaxValue;
            return null;
        }
    }
}
