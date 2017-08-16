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

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    /// The calculation of pi partial charges in pi-bonded systems of an heavy
    /// atom was made by Saller-Gasteiger. It is based on the qualitative concept of resonance and
    /// implemented with the Partial Equalization of Pi-Electronegativity (PEPE).
    /// </summary>
    /// <remarks>
    /// This descriptor uses these parameters:
    /// <list type="table">
    /// <listheader>
    ///   <term>Name</term>
    ///   <term>Default</term>
    ///   <term>Description</term>
    /// </listheader>
    /// <item>
    ///     <term>maxIterations</term>
    ///     <term>0</term>
    ///     <term>Number of maximum iterations</term>
    /// </item>
    /// </list>
    /// </remarks>
    // @author      Miguel Rojas
    // @cdk.created 2006-04-15
    // @cdk.module  qsaratomic
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:partialPiCharge
    // @see         GasteigerPEPEPartialCharges
    public class PartialPiChargeDescriptor : AbstractAtomicDescriptor
    {
        private static readonly string[] NAMES = { "pepe" };

        private GasteigerPEPEPartialCharges pepe = null;
        /// <summary>Number of maximum iterations</summary>
        private int maxIterations = -1;
        /// <summary>Number of maximum resonance structures</summary>
        private int maxResonStruc = -1;
        /// <summary> make a lone pair electron checker. Default true</summary>
        private bool lpeChecker = true;

        /// <summary>
        ///  Constructor for the PartialPiChargeDescriptor object
        /// </summary>
        public PartialPiChargeDescriptor()
        {
            pepe = new GasteigerPEPEPartialCharges();
        }

        /// <summary>
        /// The specification attribute of the PartialPiChargeDescriptor object
        /// </summary>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#partialPiCharge",
                typeof(PartialPiChargeDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the PartialPiChargeDescriptor object
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>
        /// <term>1</term>
        /// <description>Number of maximum iterations</description>
        /// </item>
        /// <item>
        /// <term>2</term>
        /// <description>checking lone pair electrons</description>
        /// </item>
        /// <item>
        /// <term>3</term>
        /// <description>number of maximum resonance structures to be searched</description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <exception cref="CDKException">Description of the Exception</exception>
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
                return new object[] { maxIterations, lpeChecker, maxResonStruc, };
            }
        }

        public override string[] DescriptorNames => NAMES;

        private DescriptorValue GetDummyDescriptorValue(Exception e)
        {
            return new DescriptorValue(_Specification, ParameterNames, Parameters, new DoubleResult(double.NaN), NAMES, e);
        }

        /// <summary>
        ///  The method returns apha partial charges assigned to an heavy atom through Gasteiger Marsili
        ///  It is needed to call the addExplicitHydrogensToSatisfyValency method from the class tools.HydrogenAdder.
        ///  For this method will be only possible if the heavy atom has single bond.
        /// </summary>
        /// <param name="atom">The <see cref="IAtom"/> for which the <see cref="DescriptorValue"/> is requested</param>
        /// <param name="ac">AtomContainer</param>
        /// <returns>Value of the alpha partial charge</returns>
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
                    return GetDummyDescriptorValue(e);
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
                        return GetDummyDescriptorValue(e);
                    }
                }

                if (maxIterations != -1) pepe.MaxGasteigerIterations = maxIterations;
                if (maxResonStruc != -1) pepe.MaxResonanceStructures = maxResonStruc;
                try
                {
                    for (int i = 0; i < ac.Atoms.Count; i++)
                        ac.Atoms[i].Charge = 0.0;
                    pepe.AssignGasteigerPiPartialCharges(ac, true);
                    for (int i = 0; i < ac.Atoms.Count; i++)
                    {
                        // assume same order, so mol.Atoms[i] == ac.Atoms[i]
                        CacheDescriptorValue(ac.Atoms[i], ac, new DoubleResult(ac.Atoms[i].Charge.Value));
                    }
                }
                catch (Exception exception)
                {
                    return GetDummyDescriptorValue(exception);
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
        /// The parameterNames attribute of the PartialPiChargeDescriptor object
        /// </summary>
        public override string[] ParameterNames { get; } = { "maxIterations", "lpeChecker", "maxResonStruc", };

        /// <summary>
        ///  Gets the parameterType attribute of the PartialPiChargeDescriptor object
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public override object GetParameterType(string name)
        {
            if ("maxIterations".Equals(name)) return int.MaxValue;
            if ("lpeChecker".Equals(name)) return true;
            if ("maxResonStruc".Equals(name)) return int.MaxValue;
            return null;
        }
    }
}
