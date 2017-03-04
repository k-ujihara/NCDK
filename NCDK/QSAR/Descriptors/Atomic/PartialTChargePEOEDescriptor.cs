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

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    ///  <p>The calculation of total partial charges of an heavy atom is based on
    ///  Partial Equalization of Electronegativity method (PEOE-PEPE) from Gasteiger. </p>
    ///  <p>They are obtained by summation of the results of the calculations on
    ///  sigma- and pi-charges. </p>
    /// <p>This descriptor uses these parameters:
    /// <table border="1">
    ///   <tr>
    ///     <td>Name</td>
    ///     <td>Default</td>
    ///     <td>Description</td>
    ///   </tr>
    ///   <tr>
    ///     <td></td>
    ///     <td></td>
    ///     <td>no parameters</td>
    ///   </tr>
    /// </table>
    ///
    ///
    // @author      Miguel Rojas
    // @cdk.created 2006-04-11
    // @cdk.module  qsaratomic
    // @cdk.githash
    // @cdk.set     qsar-descriptors
    // @cdk.dictref qsar-descriptors:PartialTChargePEOE
    ///
    // @see         GasteigerMarsiliPartialCharges
    // @see         GasteigerPEPEPartialCharges
    /// </summary>
    public class PartialTChargePEOEDescriptor : AbstractAtomicDescriptor
    {
        private static readonly string[] NAMES = { "pepeT" };

        private GasteigerMarsiliPartialCharges peoe = null;
        private GasteigerPEPEPartialCharges pepe = null;

        /// <summary>Number of maximum iterations*/
        private int maxIterations = -1;
        /// <summary>Number of maximum resonance structures*/
        private int maxResonStruc = -1;
        /// <summary> make a lone pair electron checker. Default true*/
        private bool lpeChecker = true;

        /// <summary>
        ///  Constructor for the PartialTChargePEOEDescriptor object
        /// </summary>
        public PartialTChargePEOEDescriptor()
        {
            peoe = new GasteigerMarsiliPartialCharges();
            pepe = new GasteigerPEPEPartialCharges();
        }

        /// <summary>
        /// The specification attribute of the PartialTChargePEOEDescriptor object
        /// </summary>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#PartialTChargePEOE",
                typeof(PartialTChargePEOEDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the PartialTChargePEOEDescriptor object
        /// </summary>
        public override object[] Parameters
        {
            set
            {
                if (value.Length > 3) throw new CDKException("PartialPiChargeDescriptor only expects three parameter");

                if (!(value[0] is int)) throw new CDKException("The parameter must be of type int");
                maxIterations = (int)value[0];

                if (value.Length > 1 && value[1] != null)
                {
                    if (!(value[1] is bool)) throw new CDKException("The parameter must be of type bool");
                    lpeChecker = (bool)value[1];
                }

                if (value.Length > 2 && value[2] != null)
                {
                    if (!(value[2] is int)) throw new CDKException("The parameter must be of type int");
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

        /// <summary>
        ///  The method returns partial total charges assigned to an heavy atom through PEOE method.
        ///  It is needed to call the addExplicitHydrogensToSatisfyValency method from the class tools.HydrogenAdder.
        /// </summary>
        /// <param name="atom">The IAtom for which the DescriptorValue is requested</param>
        /// <param name="ac">AtomContainer</param>
        /// <returns>an array of doubles with partial charges of [heavy, proton_1 ... proton_n]</returns>
        public override DescriptorValue Calculate(IAtom atom, IAtomContainer ac)
        {
            // FIXME: for now I'll cache a few modified atomic properties, and restore them at the end of this method
            var originalCharge = atom.Charge;
            var originalAtomtypeName = atom.AtomTypeName;
            var originalNeighborCount = atom.FormalNeighbourCount;
            var originalValency = atom.Valency;
            var originalHybridization = atom.Hybridization;
            var originalBondOrderSum = atom.BondOrderSum;
            var originalMaxBondOrder = atom.MaxBondOrder;
            if (!IsCachedAtomContainer(ac))
            {
                try
                {
                    AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(ac);
                }
                catch (CDKException e)
                {
                    new DescriptorValue(_Specification, ParameterNames, Parameters, new DoubleResult(double.NaN), NAMES, e);
                }

                if (lpeChecker)
                {
                    LonePairElectronChecker lpcheck = new LonePairElectronChecker();
                    try
                    {
                        lpcheck.Saturate(ac);
                    }
                    catch (CDKException e)
                    {
                        new DescriptorValue(_Specification, ParameterNames, Parameters, new DoubleResult(
                                double.NaN), NAMES, e);
                    }
                }

                if (maxIterations != -1) pepe.MaxGasteigerIterations = maxIterations;   // fixed CDK's mistake
                if (maxResonStruc != -1) pepe.MaxResonanceStructures = maxResonStruc;

                try
                {
                    peoe.AssignGasteigerMarsiliSigmaPartialCharges(ac, true);
                    var peoeAtom = new List<double>();
                    foreach (var aatom in ac.Atoms)
                        peoeAtom.Add(aatom.Charge.Value);
                    foreach (var aatom in ac.Atoms)
                        aatom.Charge = 0;

                    pepe.AssignGasteigerPiPartialCharges(ac, true);
                    for (int i = 0; i < ac.Atoms.Count; i++)
                        CacheDescriptorValue(ac.Atoms[i], ac, new DoubleResult(peoeAtom[i] + ac.Atoms[i].Charge.Value));
                }
                catch (Exception e)
                {
                    new DescriptorValue(_Specification, ParameterNames, Parameters, new DoubleResult(double.NaN), NAMES, e);
                }
            }
            // restore original props
            atom.Charge = originalCharge;
            atom.AtomTypeName = originalAtomtypeName;
            atom.FormalNeighbourCount = originalNeighborCount;
            atom.Valency = originalValency;
            atom.Hybridization = originalHybridization;
            atom.MaxBondOrder = originalMaxBondOrder;
            atom.BondOrderSum = originalBondOrderSum;

            return GetCachedDescriptorValue(atom) != null ? new DescriptorValue(_Specification, ParameterNames,
                    Parameters, GetCachedDescriptorValue(atom), NAMES) : null;
        }

        /// <summary>
        /// The parameterNames attribute of the PartialTChargePEOEDescriptor object
        /// </summary>
        public override string[] ParameterNames { get; } = new string[] { "maxIterations", "lpeChecker", "maxResonStruc" };

        /// <summary>
        ///  Gets the parameterType attribute of the PartialTChargePEOEDescriptor object
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
