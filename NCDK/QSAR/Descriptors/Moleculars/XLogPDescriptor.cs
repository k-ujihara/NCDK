/*  Copyright (C) 2005-2007  Christian Hoppe <chhoppe@users.sf.net>
 *                     2008  Rajarshi Guha <rajarshi.guha@gmail.com>
 *                2008-2009  Egon Willighagen <egonw@users.sf.net>
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

using NCDK.Aromaticities;
using NCDK.Common.Collections;
using NCDK.Config;
using NCDK.Graphs;
using NCDK.Isomorphisms;
using NCDK.Isomorphisms.Matchers;
using NCDK.Isomorphisms.Matchers.SMARTS;
using NCDK.Isomorphisms.MCSS;
using NCDK.QSAR.Results;
using NCDK.SMARTS;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Prediction of logP based on the atom-type method called XLogP. 
    /// </summary>
    /// <remarks>
    /// <b>Requires all hydrogens to be explicit</b>.
    /// <para>For description of the methodology see Ref. <token>cdk-cite-WANG97</token> and <token>cdk-cite-WANG00</token>.
    /// Actually one molecular factor is missing (presence of para Hs donor pair).</para>
    /// This descriptor uses these parameters:
    /// <list type="table">
    /// <listheader>
    /// <term>Name</term>
    /// <term>Default</term>
    /// <term>Description</term>
    /// </listheader>
    /// <item>
    /// <term>checkAromaticity</term>
    /// <term>false</term>
    /// <term>True is the aromaticity has to be checked</term>
    /// </item>
    /// <item>
    /// <term>salicylFlag</term>
    /// <term>false</term>
    /// <term>True is to use the salicyl acid correction factor</term>
    /// </item>
    /// </list>
    /// <para>changed 2005-11-03 by chhoppe
    /// <list type="bullet">
    /// <item>Internal hydrogen bonds are implemented</item>
    /// </list>
    /// </para>
    /// <para>CDK IDescriptor was validated against xlogp2.1</para>
    /// <para>As mentioned in the xlogP tutorial don't use charges, always draw bonds. To some extend we can support charges but not in every case.</para>
    /// <para>CDK follows the program in following points (which is not documented in the paper):
    /// <list type="bullet">
    /// <item>Atomtyp 7 is -0.137</item>
    /// <item>Atomtype 81 is -0.447</item>
    /// <item>pi system does not consider P or S</item>
    /// <item>ring system >3</item>
    /// <item>aromatic ring systems >=6</item>
    /// <item>N atomtypes: (ring) is always (ring)c</item>
    /// <item>F 83 is not 0.375, the program uses 0.512 [2005-11-21]</item>
    /// <item>hydrophobic carbon is 1-3 relationship not 1-4 [2005-11-22]</item>
    /// <item>Atomtyp C 34/35/36 perception corrected [2005-11-22]; before Atomtyp perception ring perception is done -> slows run time</item>
    /// </list>
    /// </para>
    /// <para>In question:
    /// <list type="bullet">
    /// <item>Correction factor for salicylic acid (in paper, but not used by the program)</item>
    /// <item>Amid classification is not consequent (in 6 rings (R2)N-C(R)=0 is eg 46 and in !6 membered rings it is amid)</item>
    /// <item>sometimes O=C(R)-N(R)-C(R)=O is an amid ... sometimes not</item>
    /// <item>Value for internal H bonds is in paper 0.429 but for no454 it is 0.643</item>
    /// <item>pi system defintion, the neighbourhood is unclear</item>
    /// </list>
    /// </para>
    /// <para>changed 2005-11-21 by chhoppe
    /// <list type="bullet">
    /// <item>added new parameter for the salicyl acid correction factor</item>
    /// <item>Corrected P and S perception for charges</item>
    /// </list>
    /// </para>
    /// </remarks>
    // @author         mfe4, chhoppe
    // @cdk.created    2004-11-03
    // @cdk.module qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:xlogP
    // @cdk.keyword XLogP
    // @cdk.keyword descriptor
    public class XLogPDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private bool checkAromaticity = false;
        private bool salicylFlag = false;
        private static readonly string[] NAMES = { "XLogP" };

        public XLogPDescriptor() { }

        /// <summary>
        /// The specification attribute of the XLogPDescriptor object.
        /// </summary>
        public override IImplementationSpecification Specification => specification;
        private static readonly DescriptorSpecification specification =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#xlogP",
                typeof(XLogPDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <inheritdoc/>
        public override IReadOnlyList<object> Parameters
        {
            set
            {
                if (value.Count != 2)
                {
                    throw new CDKException("XLogPDescriptor expects two parameter");
                }
                if (!(value[0] is bool))
                {
                    throw new CDKException($"The first parameter must be of type {typeof(bool)}");
                }
                else if (!(value[1] is bool))
                {
                    throw new CDKException($"The second parameter must be of type {typeof(bool)}");
                }
                // ok, all should be fine
                checkAromaticity = (bool)value[0];
                salicylFlag = (bool)value[1];
            }

            get
            {
                // return the parameters as used for the descriptor calculation
                return new object[] { checkAromaticity, salicylFlag };
            }
        }

        public override IReadOnlyList<string> DescriptorNames => NAMES;

        private DescriptorValue<Result<double>> GetDummyDescriptorValue(Exception e)
        {
            return new DescriptorValue<Result<double>>(specification, ParameterNames, Parameters, new Result<double>(double.NaN), DescriptorNames, e);
        }

        /// <summary>
        /// Calculates the xlogP for an atom container.
        /// <para>
        /// If checkAromaticity is true, the method check the aromaticity, if false, means that the aromaticity has
        /// already been checked. It is necessary to use before the call of this mehtod the
        /// addExplicitHydrogensToSatisfyValency method (HydrogenAdder classe).
        /// </para>
        /// </summary>
        /// <param name="atomContainer">AtomContainer</param>
        /// <returns>XLogP is a double</returns>
        public DescriptorValue<Result<double>> Calculate(IAtomContainer atomContainer)
        {
            IAtomContainer ac;
            try
            {
                ac = (IAtomContainer)atomContainer.Clone();
                AtomContainerManipulator.PercieveAtomTypesAndConfigureUnsetProperties(ac);
                var hAdder = CDK.HydrogenAdder;
                hAdder.AddImplicitHydrogens(ac);
                AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(ac);
            }
            catch (CDKException e)
            {
                return GetDummyDescriptorValue(e);
            }

            IRingSet rs = Cycles.FindSSSR(ac).ToRingSet();
            if (checkAromaticity)
            {
                try
                {
                    Aromaticity.CDKLegacy.Apply(ac);
                }
                catch (CDKException e)
                {
                    return GetDummyDescriptorValue(e);
                }
            }
            double xlogP = 0;
            string symbol = "";
            int bondCount = 0;
            int atomCount = ac.Atoms.Count;
            int hsCount = 0;
            double xlogPOld = 0;
            BondOrder maxBondOrder = BondOrder.Single;
            List<int> hBondAcceptors = new List<int>();
            List<int> hBondDonors = new List<int>();
            int checkAminoAcid = 1;//if 0 no check, if >1 check
            IAtom atomi = null;
            for (int i = 0; i < atomCount; i++)
            {
                atomi = (IAtom)ac.Atoms[i];
                // Problem fused ring systems
                var atomRingSet = rs.GetRings(atomi).ToList();
                atomi.SetProperty("IS_IN_AROMATIC_RING", false);
                atomi.SetProperty(CDKPropertyName.PartOfRingOfSize, 0);
                if (atomRingSet.Count > 0)
                {
                    if (atomRingSet.Count > 1)
                    {
                        var containers = RingSetManipulator.GetAllAtomContainers(atomRingSet);
                        atomRingSet = rs.Builder.NewRingSet().ToList();
                        foreach (var container in containers)
                        {
                            // XXX: we're already in the SSSR, but then get the esential cycles
                            // of this atomRingSet... this code doesn't seem to make sense as
                            // essential cycles are a subset of SSSR and can be found directly
                            foreach (var ring in Cycles.FindEssential(container).ToRingSet())
                                atomRingSet.Add(ring);
                        }
                    }
                    for (int j = 0; j < atomRingSet.Count; j++)
                    {
                        if (j == 0)
                        {
                            atomi.SetProperty(CDKPropertyName.PartOfRingOfSize, (atomRingSet[j]).RingSize);
                        }

                        if ((atomRingSet[j]).Contains(atomi))
                        {
                            if ((atomRingSet[j]).RingSize >= 6
                                    && atomi.IsAromatic)
                            {
                                atomi.SetProperty("IS_IN_AROMATIC_RING", true);
                            }
                            if ((atomRingSet[j]).RingSize < atomi.GetProperty<int>(CDKPropertyName.PartOfRingOfSize))
                            {
                                atomi.SetProperty(CDKPropertyName.PartOfRingOfSize, (atomRingSet[j]).RingSize);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < atomCount; i++)
            {
                atomi = ac.Atoms[i];
                xlogPOld = xlogP;
                symbol = atomi.Symbol;
                bondCount = ac.GetConnectedBonds(atomi).Count();
                hsCount = GetHydrogenCount(ac, atomi);
                maxBondOrder = ac.GetMaximumBondOrder(atomi);
                switch (atomi.AtomicNumber)
                {
                    case NaturalElement.AtomicNumbers.C:
                        switch (bondCount)
                        {
                            case 2:
                                // C sp
                                if (hsCount >= 1)
                                {
                                    xlogP += 0.209;
                                }
                                else
                                {
                                    switch (maxBondOrder)
                                    {
                                        case BondOrder.Double:
                                            xlogP += 2.073;
                                            break;
                                        case BondOrder.Triple:
                                            xlogP += 0.33;
                                            break;
                                    }
                                }
                                break;
                            case 3:
                                // C sp2
                                if (atomi.GetProperty<bool>("IS_IN_AROMATIC_RING"))
                                {
                                    if (GetAromaticCarbonsCount(ac, atomi) >= 2 && GetAromaticNitrogensCount(ac, atomi) == 0)
                                    {
                                        if (hsCount == 0)
                                        {
                                            if (GetAtomTypeXCount(ac, atomi) == 0)
                                            {
                                                xlogP += 0.296;
                                            }
                                            else
                                            {
                                                xlogP -= 0.151;
                                            }
                                        }
                                        else
                                        {
                                            xlogP += 0.337;
                                        }
                                    }
                                    else if (GetAromaticNitrogensCount(ac, atomi) >= 1)
                                    {
                                        if (hsCount == 0)
                                        {
                                            if (GetAtomTypeXCount(ac, atomi) == 0)
                                            {
                                                xlogP += 0.174;
                                            }
                                            else
                                            {
                                                xlogP += 0.366;
                                            }
                                        }
                                        else if (GetHydrogenCount(ac, atomi) == 1)
                                        {
                                            xlogP += 0.126;
                                        }
                                    }
                                    //NOT aromatic, but sp2
                                }
                                else
                                {
                                    switch (hsCount)
                                    {
                                        case 0:
                                            if (GetAtomTypeXCount(ac, atomi) == 0)
                                            {
                                                if (GetPiSystemsCount(ac, atomi) <= 1)
                                                {
                                                    xlogP += 0.05;
                                                }
                                                else
                                                {
                                                    xlogP += 0.013;
                                                }
                                            }
                                            else if (GetAtomTypeXCount(ac, atomi) == 1)
                                            {
                                                if (GetPiSystemsCount(ac, atomi) == 0)
                                                {
                                                    xlogP -= 0.03;
                                                }
                                                else
                                                {
                                                    xlogP -= 0.027;
                                                }
                                            }
                                            else if (GetAtomTypeXCount(ac, atomi) == 2)
                                            {
                                                if (GetPiSystemsCount(ac, atomi) == 0)
                                                {
                                                    xlogP += 0.005;
                                                }
                                                else
                                                {
                                                    xlogP -= 0.315;
                                                }
                                            }
                                            break;
                                        case 1:
                                            if (GetAtomTypeXCount(ac, atomi) == 0)
                                            {
                                                if (GetPiSystemsCount(ac, atomi) == 0)
                                                {
                                                    xlogP += 0.466;
                                                }
                                                if (GetPiSystemsCount(ac, atomi) == 1)
                                                {
                                                    xlogP += 0.136;
                                                }
                                            }
                                            else
                                            {
                                                if (GetPiSystemsCount(ac, atomi) == 0)
                                                {
                                                    xlogP += 0.001;
                                                }
                                                if (GetPiSystemsCount(ac, atomi) == 1)
                                                {
                                                    xlogP -= 0.31;
                                                }
                                            }
                                            break;
                                        case 2:
                                            xlogP += 0.42;
                                            break;
                                    }
                                    if (GetIfCarbonIsHydrophobic(ac, atomi))
                                    {
                                        xlogP += 0.211;
                                    }
                                }//sp2 NOT aromatic
                                break;
                            case 4:
                                // C sp3
                                switch (hsCount)
                                {
                                    case 0:
                                        if (GetAtomTypeXCount(ac, atomi) == 0)
                                        {
                                            if (GetPiSystemsCount(ac, atomi) == 0)
                                            {
                                                xlogP -= 0.006;
                                            }
                                            if (GetPiSystemsCount(ac, atomi) == 1)
                                            {
                                                xlogP -= 0.57;
                                            }
                                            if (GetPiSystemsCount(ac, atomi) >= 2)
                                            {
                                                xlogP -= 0.317;
                                            }
                                        }
                                        else
                                        {
                                            if (GetPiSystemsCount(ac, atomi) == 0)
                                            {
                                                xlogP -= 0.316;
                                            }
                                            else
                                            {
                                                xlogP -= 0.723;
                                            }
                                        }
                                        break;
                                    case 1:
                                        if (GetAtomTypeXCount(ac, atomi) == 0)
                                        {
                                            if (GetPiSystemsCount(ac, atomi) == 0)
                                            {
                                                xlogP += 0.127;
                                            }
                                            if (GetPiSystemsCount(ac, atomi) == 1)
                                            {
                                                xlogP -= 0.243;
                                            }
                                            if (GetPiSystemsCount(ac, atomi) >= 2)
                                            {
                                                xlogP -= 0.499;
                                            }
                                        }
                                        else
                                        {
                                            if (GetPiSystemsCount(ac, atomi) == 0)
                                            {
                                                xlogP -= 0.205;
                                            }
                                            if (GetPiSystemsCount(ac, atomi) == 1)
                                            {
                                                xlogP -= 0.305;
                                            }
                                            if (GetPiSystemsCount(ac, atomi) >= 2)
                                            {
                                                xlogP -= 0.709;
                                            }
                                        }
                                        break;
                                    case 2:
                                        if (GetAtomTypeXCount(ac, atomi) == 0)
                                        {
                                            if (GetPiSystemsCount(ac, atomi) == 0)
                                            {
                                                xlogP += 0.358;
                                            }
                                            if (GetPiSystemsCount(ac, atomi) == 1)
                                            {
                                                xlogP -= 0.008;
                                            }
                                            if (GetPiSystemsCount(ac, atomi) == 2)
                                            {
                                                xlogP -= 0.185;
                                            }
                                        }
                                        else
                                        {
                                            if (GetPiSystemsCount(ac, atomi) == 0)
                                            {
                                                xlogP -= 0.137;
                                            }
                                            if (GetPiSystemsCount(ac, atomi) == 1)
                                            {
                                                xlogP -= 0.303;
                                            }
                                            if (GetPiSystemsCount(ac, atomi) == 2)
                                            {
                                                xlogP -= 0.815;
                                            }
                                        }
                                        break;
                                    default:
                                        if (GetAtomTypeXCount(ac, atomi) == 0)
                                        {
                                            if (GetPiSystemsCount(ac, atomi) == 0)
                                            {
                                                xlogP += 0.528;
                                            }
                                            if (GetPiSystemsCount(ac, atomi) == 1)
                                            {
                                                xlogP += 0.267;
                                            }
                                        }
                                        else
                                        {
                                            xlogP -= 0.032;
                                        }
                                        break;
                                }
                                if (GetIfCarbonIsHydrophobic(ac, atomi))
                                {
                                    xlogP += 0.211;
                                }
                                break;
                        }
                        break;//C
                    case NaturalElement.AtomicNumbers.N:
                        //NO2
                        if (ac.GetBondOrderSum(atomi) >= 3.0 && GetOxygenCount(ac, atomi) >= 2
                                && maxBondOrder == BondOrder.Double)
                        {
                            xlogP += 1.178;
                        }
                        else
                        {
                            if (GetPresenceOfCarbonil(ac, atomi) >= 1)
                            {
                                // amidic nitrogen
                                if (hsCount == 0)
                                {
                                    if (GetAtomTypeXCount(ac, atomi) == 0)
                                    {
                                        xlogP += 0.078;
                                    }
                                    if (GetAtomTypeXCount(ac, atomi) == 1)
                                    {
                                        xlogP -= 0.118;
                                    }
                                }
                                if (hsCount == 1)
                                {
                                    if (GetAtomTypeXCount(ac, atomi) == 0)
                                    {
                                        xlogP -= 0.096;
                                        hBondDonors.Add(i);
                                    }
                                    else
                                    {
                                        xlogP -= 0.044;
                                        hBondDonors.Add(i);
                                    }
                                }
                                if (hsCount == 2)
                                {
                                    xlogP -= 0.646;
                                    hBondDonors.Add(i);
                                }
                            }
                            else
                            {//NO amidic nitrogen
                                if (bondCount == 1)
                                {
                                    // -C#N
                                    if (GetCarbonsCount(ac, atomi) == 1)
                                    {
                                        xlogP -= 0.566;
                                    }
                                }
                                else if (bondCount == 2)
                                {
                                    // N sp2
                                    if (atomi.GetProperty<bool>("IS_IN_AROMATIC_RING"))
                                    {
                                        xlogP -= 0.493;
                                        if (checkAminoAcid != 0)
                                        {
                                            checkAminoAcid += 1;
                                        }
                                    }
                                    else
                                    {
                                        if (GetDoubleBondedCarbonsCount(ac, atomi) == 0)
                                        {
                                            if (GetDoubleBondedNitrogenCount(ac, atomi) == 0)
                                            {
                                                if (GetDoubleBondedOxygenCount(ac, atomi) == 1)
                                                {
                                                    xlogP += 0.427;
                                                }
                                            }
                                            if (GetDoubleBondedNitrogenCount(ac, atomi) == 1)
                                            {
                                                if (GetAtomTypeXCount(ac, atomi) == 0)
                                                {
                                                    xlogP += 0.536;
                                                }
                                                if (GetAtomTypeXCount(ac, atomi) == 1)
                                                {
                                                    xlogP -= 0.597;
                                                }
                                            }
                                        }
                                        else if (GetDoubleBondedCarbonsCount(ac, atomi) == 1)
                                        {
                                            if (GetAtomTypeXCount(ac, atomi) == 0)
                                            {
                                                if (GetPiSystemsCount(ac, atomi) == 0)
                                                {
                                                    xlogP += 0.007;
                                                }
                                                if (GetPiSystemsCount(ac, atomi) == 1)
                                                {
                                                    xlogP -= 0.275;
                                                }
                                            }
                                            else if (GetAtomTypeXCount(ac, atomi) == 1)
                                            {
                                                if (GetPiSystemsCount(ac, atomi) == 0)
                                                {
                                                    xlogP += 0.366;
                                                }
                                                if (GetPiSystemsCount(ac, atomi) == 1)
                                                {
                                                    xlogP += 0.251;
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (bondCount == 3)
                                {
                                    // N sp3
                                    if (hsCount == 0)
                                    {
                                        if (atomi.IsAromatic
                                                || (rs.Contains(atomi)
                                                        && atomi.GetProperty<int>(CDKPropertyName.PartOfRingOfSize) > 3 && GetPiSystemsCount(ac, atomi) >= 1))
                                        {
                                            if (GetAtomTypeXCount(ac, atomi) == 0)
                                            {
                                                xlogP += 0.881;
                                            }
                                            else
                                            {
                                                xlogP -= 0.01;
                                            }
                                        }
                                        else
                                        {
                                            if (GetAtomTypeXCount(ac, atomi) == 0)
                                            {
                                                if (GetPiSystemsCount(ac, atomi) == 0)
                                                {
                                                    xlogP += 0.159;
                                                }
                                                if (GetPiSystemsCount(ac, atomi) > 0)
                                                {
                                                    xlogP += 0.761;
                                                }
                                            }
                                            else
                                            {
                                                xlogP -= 0.239;
                                            }
                                        }
                                    }
                                    else if (hsCount == 1)
                                    {
                                        if (GetAtomTypeXCount(ac, atomi) == 0)
                                        {
                                            // like pyrrole
                                            if (atomi.IsAromatic
                                                    || (rs.Contains(atomi)
                                                            && atomi.GetProperty<int>(CDKPropertyName.PartOfRingOfSize) > 3 && GetPiSystemsCount(ac, atomi) >= 2))
                                            {
                                                xlogP += 0.545;
                                                hBondDonors.Add(i);
                                            }
                                            else
                                            {
                                                if (GetPiSystemsCount(ac, atomi) == 0)
                                                {
                                                    xlogP -= 0.112;
                                                    hBondDonors.Add(i);
                                                }
                                                if (GetPiSystemsCount(ac, atomi) > 0)
                                                {
                                                    xlogP += 0.166;
                                                    hBondDonors.Add(i);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (rs.Contains(atomi))
                                            {
                                                xlogP += 0.153;
                                                hBondDonors.Add(i);
                                            }
                                            else
                                            {
                                                xlogP += 0.324;
                                                hBondDonors.Add(i);
                                            }
                                        }
                                    }
                                    else if (hsCount == 2)
                                    {
                                        if (GetAtomTypeXCount(ac, atomi) == 0)
                                        {
                                            if (GetPiSystemsCount(ac, atomi) == 0)
                                            {
                                                xlogP -= 0.534;
                                                hBondDonors.Add(i);
                                            }
                                            if (GetPiSystemsCount(ac, atomi) == 1)
                                            {
                                                xlogP -= 0.329;
                                                hBondDonors.Add(i);
                                            }

                                            if (checkAminoAcid != 0)
                                            {
                                                checkAminoAcid += 1;
                                            }
                                        }
                                        else
                                        {
                                            xlogP -= 1.082;
                                            hBondDonors.Add(i);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case NaturalElement.AtomicNumbers.O:
                        if (bondCount == 1 && maxBondOrder == BondOrder.Double)
                        {
                            xlogP -= 0.399;
                            if (!GetPresenceOfHydroxy(ac, atomi))
                            {
                                hBondAcceptors.Add(i);
                            }
                        }
                        else if (bondCount == 1 && hsCount == 0
                              && (GetPresenceOfNitro(ac, atomi) || GetPresenceOfCarbonil(ac, atomi) == 1)
                              || GetPresenceOfSulfat(ac, atomi))
                        {
                            xlogP -= 0.399;
                            if (!GetPresenceOfHydroxy(ac, atomi))
                            {
                                hBondAcceptors.Add(i);
                            }
                        }
                        else if (bondCount >= 1)
                        {
                            if (hsCount == 0 && bondCount == 2)
                            {
                                if (GetAtomTypeXCount(ac, atomi) == 0)
                                {
                                    if (GetPiSystemsCount(ac, atomi) == 0)
                                    {
                                        xlogP += 0.084;
                                    }
                                    if (GetPiSystemsCount(ac, atomi) > 0)
                                    {
                                        xlogP += 0.435;
                                    }
                                }
                                else if (GetAtomTypeXCount(ac, atomi) == 1)
                                {
                                    xlogP += 0.105;
                                }
                            }
                            else
                            {
                                if (GetAtomTypeXCount(ac, atomi) == 0)
                                {
                                    if (GetPiSystemsCount(ac, atomi) == 0)
                                    {
                                        xlogP -= 0.467;
                                        hBondDonors.Add(i);
                                        hBondAcceptors.Add(i);
                                    }
                                    if (GetPiSystemsCount(ac, atomi) == 1)
                                    {
                                        xlogP += 0.082;
                                        hBondDonors.Add(i);
                                        hBondAcceptors.Add(i);
                                    }
                                }
                                else if (GetAtomTypeXCount(ac, atomi) == 1)
                                {
                                    xlogP -= 0.522;
                                    hBondDonors.Add(i);
                                    hBondAcceptors.Add(i);
                                }
                            }
                        }
                        break;
                    case NaturalElement.AtomicNumbers.S:
                        if ((bondCount == 1 && maxBondOrder == BondOrder.Double)
                                || (bondCount == 1 && atomi.FormalCharge == -1))
                        {
                            xlogP -= 0.148;
                        }
                        else if (bondCount == 2)
                        {
                            if (hsCount == 0)
                            {
                                xlogP += 0.255;
                            }
                            else
                            {
                                xlogP += 0.419;
                            }
                        }
                        else if (bondCount == 3)
                        {
                            if (GetOxygenCount(ac, atomi) >= 1)
                            {
                                xlogP -= 1.375;
                            }
                        }
                        else if (bondCount == 4)
                        {
                            if (GetDoubleBondedOxygenCount(ac, atomi) >= 2)
                            {
                                xlogP -= 0.168;
                            }
                        }
                        break;
                    case NaturalElement.AtomicNumbers.P:
                        if (GetDoubleBondedSulfurCount(ac, atomi) >= 1 && bondCount >= 4)
                        {
                            xlogP += 1.253;
                        }
                        else if (GetOxygenCount(ac, atomi) >= 1 || GetDoubleBondedOxygenCount(ac, atomi) == 1
                              && bondCount >= 4)
                        {
                            xlogP -= 0.447;
                        }
                        break;
                    case NaturalElement.AtomicNumbers.F:
                        if (GetPiSystemsCount(ac, atomi) == 0)
                        {
                            xlogP += 0.375;
                        }
                        else if (GetPiSystemsCount(ac, atomi) == 1)
                        {
                            xlogP += 0.202;
                        }
                        break;
                    case NaturalElement.AtomicNumbers.Cl:
                        if (GetPiSystemsCount(ac, atomi) == 0)
                        {
                            xlogP += 0.512;
                        }
                        else if (GetPiSystemsCount(ac, atomi) >= 1)
                        {
                            xlogP += 0.663;
                        }
                        break;
                    case NaturalElement.AtomicNumbers.Br:
                        if (GetPiSystemsCount(ac, atomi) == 0)
                        {
                            xlogP += 0.85;
                        }
                        else if (GetPiSystemsCount(ac, atomi) == 1)
                        {
                            xlogP += 0.839;
                        }
                        break;
                    case NaturalElement.AtomicNumbers.I:
                        if (GetPiSystemsCount(ac, atomi) == 0)
                        {
                            xlogP += 1.05;
                        }
                        else if (GetPiSystemsCount(ac, atomi) == 1)
                        {
                            xlogP += 1.109;
                        }
                        break;
                }

                // Halogen pair 1-3
                int halcount = GetHalogenCount(ac, atomi);
                switch (halcount)
                {
                    case 2:
                        xlogP += 0.137;
                        break;
                    case 3:
                        xlogP += (3 * 0.137);
                        break;
                    case 4:
                        xlogP += (6 * 0.137);
                        break;
                }

                //            sp2 Oxygen 1-5 pair
                if (GetPresenceOfCarbonil(ac, atomi) == 2)
                {// sp2 oxygen 1-5 pair
                    if (!rs.Contains(atomi))
                    {
                        xlogP += 0.580;
                    }
                }
            }

            int[][] pairCheck = null;
            if (hBondAcceptors.Count > 0 && hBondDonors.Count > 0)
            {
                pairCheck = InitializeHydrogenPairCheck(Arrays.CreateJagged<int>(atomCount, atomCount));
            }
            var apsp = new AllPairsShortestPaths(ac);
            for (int i = 0; i < hBondAcceptors.Count; i++)
            {
                for (int j = 0; j < hBondDonors.Count; j++)
                {
                    if (CheckRingLink(rs, ac, ac.Atoms[hBondAcceptors[i]])
                            || CheckRingLink(rs, ac, ac.Atoms[hBondDonors[j]]))
                    {
                        int dist = apsp.From(ac.Atoms[hBondAcceptors[i]]).GetDistanceTo(ac.Atoms[hBondDonors[j]]);
                        if (CheckRingLink(rs, ac, ac.Atoms[hBondAcceptors[i]])
                                && CheckRingLink(rs, ac, ac.Atoms[hBondDonors[j]]))
                        {
                            if (dist == 3 && pairCheck[hBondAcceptors[i]][hBondDonors[j]] == 0)
                            {
                                xlogP += 0.429;
                                pairCheck[hBondAcceptors[i]][hBondDonors[j]] = 1;
                                pairCheck[hBondDonors[j]][hBondAcceptors[i]] = 1;
                            }
                        }
                        else
                        {
                            if (dist == 4 && pairCheck[hBondAcceptors[i]][hBondDonors[j]] == 0)
                            {
                                xlogP += 0.429;
                                pairCheck[hBondAcceptors[i]][hBondDonors[j]] = 1;
                                pairCheck[hBondDonors[j]][hBondAcceptors[i]] = 1;
                            }
                        }
                    }
                }
            }

            var universalIsomorphismTester = new UniversalIsomorphismTester();
            if (checkAminoAcid > 1)
            {
                //            alpha amino acid
                QueryAtomContainer aminoAcid = QueryAtomContainerCreator.CreateBasicQueryContainer(NewAminoAcid(ac.Builder));

                IAtom bondAtom0 = null;
                IAtom bondAtom1 = null;
                foreach (var bond in aminoAcid.Bonds)
                {
                    bondAtom0 = bond.Atoms[0];
                    bondAtom1 = bond.Atoms[1];
                    if ((bondAtom0.AtomicNumber.Equals(NaturalElement.AtomicNumbers.C) && bondAtom1.AtomicNumber.Equals(NaturalElement.AtomicNumbers.N))
                     || (bondAtom0.AtomicNumber.Equals(NaturalElement.AtomicNumbers.N) && bondAtom1.AtomicNumber.Equals(NaturalElement.AtomicNumbers.C))
                     && bond.Order == BondOrder.Single)
                    {
                        aminoAcid.RemoveBond(bondAtom0, bondAtom1);
                        var qbond = new QueryBond(bondAtom0, bondAtom1, ExprType.SingleOrAromatic);
                        aminoAcid.Bonds.Add(qbond);
                        break;
                    }
                }

                try
                {
                    if (universalIsomorphismTester.IsSubgraph(ac, aminoAcid))
                    {
                        var list = universalIsomorphismTester.GetSubgraphAtomsMap(ac, aminoAcid);
                        RMap map = null;
                        IAtom atom1_ = null;
                        for (int j = 0; j < list.Count; j++)
                        {
                            map = (RMap)list[j];
                            atom1_ = ac.Atoms[map.Id1];
                            if (atom1_.AtomicNumber.Equals(NaturalElement.AtomicNumbers.O) && ac.GetMaximumBondOrder(atom1_) == BondOrder.Single)
                            {
                                if (ac.GetConnectedBonds(atom1_).Count() == 2 && GetHydrogenCount(ac, atom1_) == 0)
                                {
                                }
                                else
                                {
                                    xlogP -= 2.166;
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (CDKException e)
                {
                    return GetDummyDescriptorValue(e);
                }
            }

            var paba = CreatePaba(ac.Builder);
            // p-amino sulphonic acid
            try
            {
                if (universalIsomorphismTester.IsSubgraph(ac, paba))
                {
                    xlogP -= 0.501;
                }
            }
            catch (CDKException e)
            {
                return GetDummyDescriptorValue(e);
            }

            // salicylic acid
            if (salicylFlag)
            {
                var salicilic = CreateSalicylicAcid(ac.Builder);
                try
                {
                    if (universalIsomorphismTester.IsSubgraph(ac, salicilic))
                    {
                        xlogP += 0.554;
                    }
                }
                catch (CDKException e)
                {
                    return GetDummyDescriptorValue(e);
                }
            }

            // ortho oxygen pair
            var orthopair = SmartsPattern.Create("OccO");
            if (orthopair.Matches(ac))
            {
                xlogP -= 0.268;
                //logger.debug("XLOGP: Ortho oxygen pair	-0.268");
            }

            return new DescriptorValue<Result<double>>(specification, ParameterNames, Parameters, new Result<double>(xlogP), DescriptorNames);
        }

        /// <summary>
        /// The specific type of the DescriptorResult object.
        /// <p/>
        /// The return value from this method really indicates what type of result will
        /// be obtained from the <see cref="IDescriptorValue"/> object. Note that the same result
        /// can be achieved by interrogating the <see cref="IDescriptorValue"/> object; this method
        /// allows you to do the same thing, without actually calculating the descriptor.
        /// </summary>
        public override IDescriptorResult DescriptorResultType { get; } = new Result<double>(0.0);

        /// <summary>
        /// Method initialise the HydrogenpairCheck with a value
        ///
        /// <param name="pairCheck">value</param>
        /// <returns>void</returns>
        /// </summary>
        private static int[][] InitializeHydrogenPairCheck(int[][] pairCheck)
        {
            for (int i = 0; i < pairCheck.Length; i++)
            {
                for (int j = 0; j < pairCheck[0].Length; j++)
                {
                    pairCheck[i][j] = 0;
                }
            }
            return pairCheck;
        }

        /// <summary>
        /// Check if atom or neighbour atom is part of a ring
        /// </summary>
        /// <param name="ringSet"></param>
        /// <param name="ac"></param>
        /// <param name="atom"></param>
        /// <returns>The hydrogenCount value</returns>
        private static bool CheckRingLink(IRingSet ringSet, IAtomContainer ac, IAtom atom)
        {
            var neighbours = ac.GetConnectedAtoms(atom);
            if (ringSet.Contains(atom))
            {
                return true;
            }
            foreach (var neighbour in neighbours)
            {
                if (ringSet.Contains(neighbour))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the hydrogenCount attribute of the XLogPDescriptor object.
        /// </summary>
        /// <param name="ac"></param>
        /// <param name="atom"></param>
        /// <returns>The hydrogenCount value</returns>
        private static int GetHydrogenCount(IAtomContainer ac, IAtom atom)
        {
            var neighbours = ac.GetConnectedAtoms(atom);
            int hcounter = 0;
            foreach (var neighbour in neighbours)
            {
                if (neighbour.AtomicNumber.Equals(NaturalElement.AtomicNumbers.H))
                {
                    hcounter += 1;
                }
            }
            return hcounter;
        }

        /// <summary>
        /// Gets the HalogenCount attribute of the XLogPDescriptor object.
        /// </summary>
        /// <param name="ac"></param>
        /// <param name="atom"></param>
        /// <returns>The alogenCount value</returns>
        private static int GetHalogenCount(IAtomContainer ac, IAtom atom)
        {
            var neighbours = ac.GetConnectedAtoms(atom);
            int acounter = 0;
            foreach (var neighbour in neighbours)
            {
                switch (neighbour.AtomicNumber)
                {
                    case NaturalElement.AtomicNumbers.F:
                    case NaturalElement.AtomicNumbers.I:
                    case NaturalElement.AtomicNumbers.Cl:
                    case NaturalElement.AtomicNumbers.Br:
                        acounter += 1;
                        break;
                }
            }
            return acounter;
        }

        /// <summary>
        /// Gets the atomType X Count attribute of the XLogPDescriptor object.
        /// </summary>
        /// <param name="ac"></param>
        /// <param name="atom"></param>
        /// <returns>The nitrogenOrOxygenCount value</returns>
        private static int GetAtomTypeXCount(IAtomContainer ac, IAtom atom)
        {
            var neighbours = ac.GetConnectedAtoms(atom);
            int nocounter = 0;
            IBond bond;
            foreach (var neighbour in neighbours)
            {
                switch (neighbour.AtomicNumber)
                {
                    case NaturalElement.AtomicNumbers.N:
                    case NaturalElement.AtomicNumbers.O:
                        if (!neighbour.GetProperty<bool>("IS_IN_AROMATIC_RING"))
                        {
                            bond = ac.GetBond(neighbour, atom);
                            if (bond.Order != BondOrder.Double)
                            {
                                nocounter += 1;
                            }
                        }
                        break;
                }
            }
            return nocounter;
        }

        /// <summary>
        /// Gets the aromaticCarbonsCount attribute of the XLogPDescriptor object.
        /// </summary>
        /// <param name="ac"></param>
        /// <param name="atom"></param>
        /// <returns>The aromaticCarbonsCount value</returns>
        private static int GetAromaticCarbonsCount(IAtomContainer ac, IAtom atom)
        {
            var neighbours = ac.GetConnectedAtoms(atom);
            int carocounter = 0;
            foreach (var neighbour in neighbours)
            {
                switch (neighbour.AtomicNumber)
                {
                    case NaturalElement.AtomicNumbers.C:
                        if (neighbour.IsAromatic)
                        {
                            carocounter += 1;
                        }
                        break;
                }
            }
            return carocounter;
        }

        /// <summary>
        /// Gets the carbonsCount attribute of the XLogPDescriptor object.
        /// </summary>
        /// <param name="ac"></param>
        /// <param name="atom"></param>
        /// <returns>The carbonsCount value</returns>
        private static int GetCarbonsCount(IAtomContainer ac, IAtom atom)
        {
            var neighbours = ac.GetConnectedAtoms(atom);
            int ccounter = 0;
            foreach (var neighbour in neighbours)
            {
                if (neighbour.AtomicNumber.Equals(NaturalElement.AtomicNumbers.C))
                {
                    if (!neighbour.IsAromatic)
                    {
                        ccounter += 1;
                    }
                }
            }
            return ccounter;
        }

        /// <summary>
        /// Gets the oxygenCount attribute of the XLogPDescriptor object.
        /// </summary>
        /// <param name="ac"></param>
        /// <param name="atom"></param>
        /// <returns>The carbonsCount value</returns>
        private static int GetOxygenCount(IAtomContainer ac, IAtom atom)
        {
            var neighbours = ac.GetConnectedAtoms(atom);
            int ocounter = 0;
            foreach (var neighbour in neighbours)
            {
                switch (neighbour.AtomicNumber)
                {
                    case NaturalElement.AtomicNumbers.O:
                        if (!neighbour.IsAromatic)
                        {
                            ocounter += 1;
                        }
                        break;
                }
            }
            return ocounter;
        }

        /// <summary>
        /// Gets the doubleBondedCarbonsCount attribute of the XLogPDescriptor object.
        /// </summary>
        /// <param name="ac">Description of the Parameter</param>
        /// <param name="atom">Description of the Parameter</param>
        /// <returns>The doubleBondedCarbonsCount value</returns>
        private static int GetDoubleBondedCarbonsCount(IAtomContainer ac, IAtom atom)
        {
            var neighbours = ac.GetConnectedAtoms(atom);
            IBond bond;
            int cdbcounter = 0;
            foreach (var neighbour in neighbours)
            {
                switch (neighbour.AtomicNumber)
                {
                    case NaturalElement.AtomicNumbers.C:
                        bond = ac.GetBond(neighbour, atom);
                        if (bond.Order == BondOrder.Double)
                        {
                            cdbcounter += 1;
                        }
                        break;
                }
            }
            return cdbcounter;
        }

        /// <summary>
        /// Gets the doubleBondedOxygenCount attribute of the XLogPDescriptor object.
        /// </summary>
        /// <param name="ac">Description of the Parameter</param>
        /// <param name="atom">Description of the Parameter</param>
        /// <returns>The doubleBondedOxygenCount value</returns>
        private static int GetDoubleBondedOxygenCount(IAtomContainer ac, IAtom atom)
        {
            var neighbours = ac.GetConnectedAtoms(atom);
            IBond bond;
            int odbcounter = 0;
            bool chargeFlag = false;
            if (atom.FormalCharge >= 1)
            {
                chargeFlag = true;
            }
            foreach (var neighbour in neighbours)
            {
                switch (neighbour.AtomicNumber)
                {
                    case NaturalElement.AtomicNumbers.O:
                        bond = ac.GetBond(neighbour, atom);
                        if (chargeFlag && neighbour.FormalCharge == -1 && bond.Order == BondOrder.Single)
                        {
                            odbcounter += 1;
                        }
                        if (!neighbour.IsAromatic)
                        {
                            if (bond.Order == BondOrder.Double)
                            {
                                odbcounter += 1;
                            }
                        }
                        break;
                }
            }
            return odbcounter;
        }

        /// <summary>
        /// Gets the doubleBondedSulfurCount attribute of the XLogPDescriptor object.
        /// </summary>
        /// <param name="ac">Description of the Parameter</param>
        /// <param name="atom">Description of the Parameter</param>
        /// <returns>The doubleBondedSulfurCount value</returns>
        private static int GetDoubleBondedSulfurCount(IAtomContainer ac, IAtom atom)
        {
            var neighbours = ac.GetConnectedAtoms(atom);
            IBond bond;
            int sdbcounter = 0;
            foreach (var neighbour in neighbours)
            {
                switch (neighbour.AtomicNumber)
                {
                    case NaturalElement.AtomicNumbers.S:
                        if (atom.FormalCharge == 1 && neighbour.FormalCharge == -1)
                        {
                            sdbcounter += 1;
                        }
                        bond = ac.GetBond(neighbour, atom);
                        if (!neighbour.IsAromatic)
                        {
                            if (bond.Order == BondOrder.Double)
                            {
                                sdbcounter += 1;
                            }
                        }
                        break;
                }
            }
            return sdbcounter;
        }

        /// <summary>
        /// Gets the doubleBondedNitrogenCount attribute of the XLogPDescriptor object.
        /// </summary>
        /// <param name="ac">Description of the Parameter</param>
        /// <param name="atom">Description of the Parameter</param>
        /// <returns>The doubleBondedNitrogenCount value</returns>
        private static int GetDoubleBondedNitrogenCount(IAtomContainer ac, IAtom atom)
        {
            var neighbours = ac.GetConnectedAtoms(atom);
            IBond bond;
            int ndbcounter = 0;
            foreach (var neighbour in neighbours)
            {
                switch (neighbour.AtomicNumber)
                {
                    case NaturalElement.AtomicNumbers.N:
                        bond = ac.GetBond(neighbour, atom);
                        if (!neighbour.IsAromatic)
                        {
                            if (bond.Order == BondOrder.Double)
                            {
                                ndbcounter += 1;
                            }
                        }
                        break;
                }
            }
            return ndbcounter;
        }

        /// <summary>
        /// Gets the aromaticNitrogensCount attribute of the XLogPDescriptor object.
        /// </summary>
        /// <param name="ac">Description of the Parameter</param>
        /// <param name="atom">Description of the Parameter</param>
        /// <returns>The aromaticNitrogensCount value</returns>
        private static int GetAromaticNitrogensCount(IAtomContainer ac, IAtom atom)
        {
            var neighbours = ac.GetConnectedAtoms(atom);
            int narocounter = 0;
            foreach (var neighbour in neighbours)
            {
                switch (neighbour.AtomicNumber)
                {
                    case NaturalElement.AtomicNumbers.N:
                        if (neighbour.GetProperty<bool>("IS_IN_AROMATIC_RING"))
                        {
                            narocounter += 1;
                        }
                        break;
                }
            }
            return narocounter;
        }

        // a piSystem is a double or triple or aromatic bond:

        /// <summary>
        /// Gets the piSystemsCount attribute of the XLogPDescriptor object.
        /// </summary>
        /// <param name="ac">Description of the Parameter</param>
        /// <param name="atom">Description of the Parameter</param>
        /// <returns>The piSystemsCount value</returns>
        private static int GetPiSystemsCount(IAtomContainer ac, IAtom atom)
        {
            var neighbours = ac.GetConnectedAtoms(atom);
            int picounter = 0;
            foreach (var neighbour in neighbours)
            {
                var bonds = ac.GetConnectedBonds(neighbour);
                foreach (var bond in bonds)
                {
                    if (bond.Order != BondOrder.Single && !bond.GetConnectedAtom(neighbour).Equals(atom))
                    {
                        switch (neighbour.AtomicNumber)
                        {
                            case NaturalElement.AtomicNumbers.P:
                            case NaturalElement.AtomicNumbers.S:
                                break;
                            default:
                                picounter += 1;
                                break;
                        }
                    }
                }
            }
            return picounter;
        }

        /// <summary>
        /// Gets the presenceOf Hydroxy group attribute of the XLogPDescriptor object.
        /// </summary>
        /// <param name="ac">Description of the Parameter</param>
        /// <param name="atom">Description of the Parameter</param>
        /// <returns>The presenceOfCarbonil value</returns>
        private static bool GetPresenceOfHydroxy(IAtomContainer ac, IAtom atom)
        {
            IAtom neighbour0 = (IAtom)ac.GetConnectedAtoms(atom).First();
            if (neighbour0.AtomicNumber.Equals(NaturalElement.AtomicNumbers.C))
            {
                var first = ac.GetConnectedAtoms(neighbour0);
                foreach (var conAtom in first)
                {
                    if (conAtom.AtomicNumber.Equals(NaturalElement.AtomicNumbers.O))
                    {
                        if (ac.GetBond(neighbour0, conAtom).Order == BondOrder.Single)
                        {
                            if (ac.GetConnectedBonds(conAtom).Count() > 1 && GetHydrogenCount(ac, conAtom) == 0)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the presenceOfN=O attribute of the XLogPDescriptor object.
        /// </summary>
        /// <param name="ac">Description of the Parameter</param>
        /// <param name="atom">Description of the Parameter</param>
        /// <returns>The presenceOfNitor [bool]</returns>
        private static bool GetPresenceOfNitro(IAtomContainer ac, IAtom atom)
        {
            var neighbours = ac.GetConnectedAtoms(atom);
            //int counter = 0;
            foreach (var neighbour in neighbours)
            {
                if (neighbour.AtomicNumber.Equals(NaturalElement.AtomicNumbers.N))
                {
                    var second = ac.GetConnectedAtoms(neighbour);
                    foreach (var conAtom in second)
                    {
                        if (conAtom.AtomicNumber.Equals(NaturalElement.AtomicNumbers.O))
                        {
                            var bond = ac.GetBond(neighbour, conAtom);
                            if (bond.Order == BondOrder.Double)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the presenceOfSulfat A-S(O2)-A attribute of the XLogPDescriptor object.
        /// </summary>
        /// <param name="ac">Description of the Parameter</param>
        /// <param name="atom">Description of the Parameter</param>
        /// <returns>The presenceOfSulfat [bool]</returns>
        private static bool GetPresenceOfSulfat(IAtomContainer ac, IAtom atom)
        {
            var neighbours = ac.GetConnectedAtoms(atom);
            foreach (var neighbour in neighbours)
            {
                if (neighbour.AtomicNumber.Equals(NaturalElement.AtomicNumbers.S) && GetOxygenCount(ac, neighbour) >= 2
                        && ac.GetConnectedBonds(neighbour).Count() == 4)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the presenceOfCarbonil attribute of the XLogPDescriptor object.
        /// </summary>
        /// <param name="ac">Description of the Parameter</param>
        /// <param name="atom">Description of the Parameter</param>
        /// <returns>The presenceOfCarbonil value</returns>
        private static int GetPresenceOfCarbonil(IAtomContainer ac, IAtom atom)
        {
            var neighbours = ac.GetConnectedAtoms(atom);
            int counter = 0;
            foreach (var neighbour in neighbours)
            {
                if (neighbour.AtomicNumber.Equals(NaturalElement.AtomicNumbers.C))
                {
                    var second = ac.GetConnectedAtoms(neighbour);
                    foreach (var conAtom in second)
                    {
                        if (conAtom.AtomicNumber.Equals(NaturalElement.AtomicNumbers.O))
                        {
                            var bond = ac.GetBond(neighbour, conAtom);
                            if (bond.Order == BondOrder.Double)
                            {
                                counter += 1;
                            }
                        }
                    }
                }
            }
            return counter;
        }

        /// <summary>
        /// Gets the ifCarbonIsHydrophobic attribute of the XLogPDescriptor object.
        /// C must be sp2 or sp3 and, for all distances C-1-2-3 only C atoms are permitted
        /// </summary>
        /// <param name="ac">Description of the Parameter</param>
        /// <param name="atom">Description of the Parameter</param>
        /// <returns>The ifCarbonIsHydrophobic value</returns>
        private static bool GetIfCarbonIsHydrophobic(IAtomContainer ac, IAtom atom)
        {
            var first = ac.GetConnectedAtoms(atom);
            //IAtom[] fourth = null;
            if (first.Any())
            {
                foreach (var firstAtom in first)
                {
                    switch (firstAtom.AtomicNumber)
                    {
                        case NaturalElement.AtomicNumbers.C:
                        case NaturalElement.AtomicNumbers.H:
                            break;
                        default:
                            return false;
                    }
                    var second = ac.GetConnectedAtoms(firstAtom);
                    if (second.Any())
                    {
                        foreach (var secondAtom in second)
                        {
                            switch (secondAtom.AtomicNumber)
                            {
                                case NaturalElement.AtomicNumbers.C:
                                case NaturalElement.AtomicNumbers.H:
                                    break;
                                default:
                                    return false;
                            }
                            var third = ac.GetConnectedAtoms(secondAtom);
                            if (third.Any())
                            {
                                foreach (var thirdAtom in third)
                                {
                                    switch (thirdAtom.AtomicNumber)
                                    {
                                        case NaturalElement.AtomicNumbers.C:
                                        case NaturalElement.AtomicNumbers.H:
                                            break;
                                        default:
                                            return false;
                                    }
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// The parameterNames attribute of the XLogPDescriptor object.
        /// </summary>
        public override IReadOnlyList<string> ParameterNames { get; } = new string[] { "checkAromaticity", "salicylFlag" };

        /// <summary>
        /// Gets the parameterType attribute of the XLogPDescriptor object.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public override object GetParameterType(string name) => true;

        private static IAtomContainer CreatePaba(IChemObjectBuilder builder)
        {
            // SMILES CS(=O)(=O)c1ccc(N)cc1
            IAtomContainer container = builder.NewAtomContainer();
            IAtom atom1 = builder.NewAtom("C");
            container.Atoms.Add(atom1);
            IAtom atom2 = builder.NewAtom("S");
            container.Atoms.Add(atom2);
            IAtom atom3 = builder.NewAtom("O");
            container.Atoms.Add(atom3);
            IAtom atom4 = builder.NewAtom("O");
            container.Atoms.Add(atom4);
            IAtom atom5 = builder.NewAtom("C");
            atom5.Hybridization = Hybridization.SP2;
            container.Atoms.Add(atom5);
            IAtom atom6 = builder.NewAtom("C");
            atom6.Hybridization = Hybridization.SP2;
            container.Atoms.Add(atom6);
            IAtom atom7 = builder.NewAtom("C");
            atom7.Hybridization = Hybridization.SP2;
            container.Atoms.Add(atom7);
            IAtom atom8 = builder.NewAtom("C");
            atom8.Hybridization = Hybridization.SP2;
            container.Atoms.Add(atom8);
            IAtom atom9 = builder.NewAtom("N");
            container.Atoms.Add(atom9);
            IAtom atom10 = builder.NewAtom("C");
            atom10.Hybridization = Hybridization.SP2;
            container.Atoms.Add(atom10);
            IAtom atom11 = builder.NewAtom("C");
            atom11.Hybridization = Hybridization.SP2;
            container.Atoms.Add(atom11);
            IBond bond1 = builder.NewBond(atom1, atom2, BondOrder.Single);
            container.Bonds.Add(bond1);
            IBond bond2 = builder.NewBond(atom2, atom3, BondOrder.Double);
            container.Bonds.Add(bond2);
            IBond bond3 = builder.NewBond(atom2, atom4, BondOrder.Double);
            container.Bonds.Add(bond3);
            IBond bond4 = builder.NewBond(atom2, atom5, BondOrder.Single);
            container.Bonds.Add(bond4);
            IBond bond5 = builder.NewBond(atom5, atom6, BondOrder.Double);
            bond5.IsAromatic = true;
            container.Bonds.Add(bond5);
            IBond bond6 = builder.NewBond(atom6, atom7, BondOrder.Single);
            bond6.IsAromatic = true;
            container.Bonds.Add(bond6);
            IBond bond7 = builder.NewBond(atom7, atom8, BondOrder.Double);
            bond7.IsAromatic = true;
            container.Bonds.Add(bond7);
            IBond bond8 = builder.NewBond(atom8, atom9, BondOrder.Single);
            container.Bonds.Add(bond8);
            IBond bond9 = builder.NewBond(atom8, atom10, BondOrder.Single);
            bond9.IsAromatic = true;
            container.Bonds.Add(bond9);
            IBond bond10 = builder.NewBond(atom10, atom11, BondOrder.Double);
            bond10.IsAromatic = true;
            container.Bonds.Add(bond10);
            IBond bond11 = builder.NewBond(atom5, atom11, BondOrder.Single);
            bond11.IsAromatic = true;
            container.Bonds.Add(bond11);

            return container;
        }

        private static IAtomContainer NewAminoAcid(IChemObjectBuilder builder)
        {
            // SMILES NCC(=O)O
            IAtomContainer container = builder.NewAtomContainer();
            IAtom atom1 = builder.NewAtom("N");
            container.Atoms.Add(atom1);
            IAtom atom2 = builder.NewAtom("C");
            container.Atoms.Add(atom2);
            IAtom atom3 = builder.NewAtom("C"); // carbonyl
            container.Atoms.Add(atom3);
            IAtom atom4 = builder.NewAtom("O"); // carbonyl
            container.Atoms.Add(atom4);
            IAtom atom5 = builder.NewAtom("O");
            container.Atoms.Add(atom5);
            container.Bonds.Add(builder.NewBond(atom1, atom2, BondOrder.Single));
            container.Bonds.Add(builder.NewBond(atom2, atom3, BondOrder.Single));
            container.Bonds.Add(builder.NewBond(atom3, atom4, BondOrder.Double));
            container.Bonds.Add(builder.NewBond(atom3, atom5, BondOrder.Single));
            return container;
        }

        private static IAtomContainer CreateSalicylicAcid(IChemObjectBuilder builder)
        {
            // SMILES O=C(O)c1ccccc1O
            IAtomContainer container = builder.NewAtomContainer();
            IAtom atom1 = builder.NewAtom("C");
            container.Atoms.Add(atom1);
            IAtom atom2 = builder.NewAtom("O");
            container.Atoms.Add(atom2);
            IAtom atom3 = builder.NewAtom("O");
            container.Atoms.Add(atom3);
            IAtom atom4 = builder.NewAtom("C");
            atom4.Hybridization = Hybridization.SP2;
            container.Atoms.Add(atom4);
            IAtom atom5 = builder.NewAtom("C");
            atom5.Hybridization = Hybridization.SP2;
            container.Atoms.Add(atom5);
            IAtom atom6 = builder.NewAtom("C");
            atom6.Hybridization = Hybridization.SP2;
            container.Atoms.Add(atom6);
            IAtom atom7 = builder.NewAtom("C");
            atom7.Hybridization = Hybridization.SP2;
            container.Atoms.Add(atom7);
            IAtom atom8 = builder.NewAtom("C");
            atom8.Hybridization = Hybridization.SP2;
            container.Atoms.Add(atom8);
            IAtom atom9 = builder.NewAtom("C");
            atom9.Hybridization = Hybridization.SP2;
            container.Atoms.Add(atom9);
            IAtom atom10 = builder.NewAtom("O");
            container.Atoms.Add(atom10);
            IBond bond1 = builder.NewBond(atom1, atom2, BondOrder.Double);
            container.Bonds.Add(bond1);
            IBond bond2 = builder.NewBond(atom1, atom3, BondOrder.Single);
            container.Bonds.Add(bond2);
            IBond bond3 = builder.NewBond(atom1, atom4, BondOrder.Single);
            container.Bonds.Add(bond3);
            IBond bond4 = builder.NewBond(atom4, atom5, BondOrder.Double);
            bond4.IsAromatic = true;
            container.Bonds.Add(bond4);
            IBond bond5 = builder.NewBond(atom5, atom6, BondOrder.Single);
            bond5.IsAromatic = true;
            container.Bonds.Add(bond5);
            IBond bond6 = builder.NewBond(atom6, atom7, BondOrder.Double);
            bond6.IsAromatic = true;
            container.Bonds.Add(bond6);
            IBond bond7 = builder.NewBond(atom7, atom8, BondOrder.Single);
            bond7.IsAromatic = true;
            container.Bonds.Add(bond7);
            IBond bond8 = builder.NewBond(atom8, atom9, BondOrder.Double);
            bond8.IsAromatic = true;
            container.Bonds.Add(bond8);
            IBond bond9 = builder.NewBond(atom9, atom4, BondOrder.Single);
            bond9.IsAromatic = true;
            container.Bonds.Add(bond9);
            IBond bond10 = builder.NewBond(atom9, atom10, BondOrder.Single);
            container.Bonds.Add(bond10);

            return container;
        }

        IDescriptorValue IMolecularDescriptor.Calculate(IAtomContainer container) => Calculate(container);
    }
}
