/*  Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.QSAR.Result;
using NCDK.RingSearches;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Calculation of topological polar surface area based on fragment
    /// contributions (TPSA) <token>cdk-cite-ERTL2000</token>.
    /// </summary>
    /// <remarks>
    /// <para>This descriptor uses these parameters:
    /// <list type="table">
    /// <item>
    /// <term>Name</term>
    /// <term>Default</term>
    /// <term>Description</term>
    /// </item>
    /// <item>
    /// <term>checkAromaticity</term>
    /// <term>false</term>
    /// <term>If true, it will check aromaticity</term>
    /// </item>
    /// </list>
    /// </para>
    /// <para>
    /// This descriptor works properly with AtomContainers whose atoms contain either <b>explicit hydrogens</b> or
    /// <b>implicit hydrogens</b>.
    /// </para>
    /// <para>
    /// Returns a single value named <i>TopoPSA</i>
    /// </para>
    /// </remarks>
    // @author mfe4
    // @author ulif
    // @cdk.created 2004-11-03
    // @cdk.module qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:tpsa
    // @cdk.keyword TPSA
    // @cdk.keyword total polar surface area
    // @cdk.keyword descriptor
    public class TPSADescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private bool checkAromaticity = false;
        private static Dictionary<string, double> map;
        private static readonly string[] NAMES = { "TopoPSA" };

        /// <summary>
        /// Constructor for the TPSADescriptor object.
        /// </summary>
        public TPSADescriptor()
        {
            if (map == null)
            {
                map = new Dictionary<string, double>();
                // contributions:
                // every contribution is given by an atom profile;
                // positions in atom profile strings are: symbol, max-bond-order, bond-order-sum,
                // number-of-neighbours, Hcount, formal charge, aromatic-bonds, is-in-3-membered-ring,
                // single-bonds, double-bonds, triple-bonds.

                map["N+1.0+3.0+3+0+0+0+0+3+0+0"] = 3.24; // 1
                map["N+2.0+3.0+2+0+0+0+0+1+1+0"] = 12.36; // 2
                map["N+3.0+3.0+1+0+0+0+0+0+0+1"] = 23.79; // 3
                map["N+2.0+5.0+3+0+0+0+0+1+2+0"] = 11.68; // 4
                map["N+3.0+5.0+2+0+0+0+0+0+1+1"] = 13.6; // 5
                map["N+1.0+3.0+3+0+0+0+1+3+0+0"] = 3.01; // 6
                map["N+1.0+3.0+3+1+0+0+0+3+0+0"] = 12.03; // 7
                map["N+1.0+3.0+3+1+0+0+1+3+0+0"] = 21.94; // 8
                map["N+2.0+3.0+2+1+0+0+0+1+1+0"] = 23.85; //9
                map["N+1.0+3.0+3+2+0+0+0+3+0+0"] = 26.02; // 10
                map["N+1.0+4.0+4+0+1+0+0+4+0+0"] = 0.0; //11
                map["N+2.0+4.0+3+0+1+0+0+2+1+0"] = 3.01; //12
                map["N+3.0+4.0+2+0+1+0+0+1+0+1"] = 4.36; //13
                map["N+1.0+4.0+4+1+1+0+0+4+0+0"] = 4.44; //14
                map["N+2.0+4.0+3+1+1+0+0+2+1+0"] = 13.97; //15
                map["N+1.0+4.0+4+2+1+0+0+4+0+0"] = 16.61; //16
                map["N+2.0+4.0+3+2+1+0+0+2+1+0"] = 25.59; //17
                map["N+1.0+4.0+4+3+1+0+0+4+0+0"] = 27.64; //18
                map["N+1.5+3.0+2+0+0+2+0+0+0+0"] = 12.89; //19
                map["N+1.5+4.5+3+0+0+3+0+0+0+0"] = 4.41; //20
                map["N+1.5+4.0+3+0+0+2+0+1+0+0"] = 4.93; //21
                map["N+2.0+5.0+3+0+0+2+0+0+1+0"] = 8.39; //22
                map["N+1.5+4.0+3+1+0+2+0+1+0+0"] = 15.79; //23
                map["N+1.5+4.5+3+0+1+3+0+0+0+0"] = 4.1; //24
                map["N+1.5+4.0+3+0+1+2+0+1+0+0"] = 3.88; //25
                map["N+1.5+4.0+3+1+1+2+0+1+0+0"] = 14.14; //26

                map["O+1.0+2.0+2+0+0+0+0+2+0+0"] = 9.23; //27
                map["O+1.0+2.0+2+0+0+0+1+2+0+0"] = 12.53; //28
                map["O+2.0+2.0+1+0+0+0+0+0+1+0"] = 17.07; //29
                map["O+1.0+1.0+1+0+-1+0+0+1+0+0"] = 23.06; //30
                map["O+1.0+2.0+2+1+0+0+0+2+0+0"] = 20.23; //31
                map["O+1.5+3.0+2+0+0+2+0+0+0+0"] = 13.14; //32

                map["S+1.0+2.0+2+0+0+0+0+2+0+0"] = 25.3; //33
                map["S+2.0+2.0+1+0+0+0+0+0+1+0"] = 32.09; //34
                map["S+2.0+4.0+3+0+0+0+0+2+1+0"] = 19.21; //35
                map["S+2.0+6.0+4+0+0+0+0+2+2+0"] = 8.38; //36
                map["S+1.0+2.0+2+1+0+0+0+2+0+0"] = 38.8; //37
                map["S+1.5+3.0+2+0+0+2+0+0+0+0"] = 28.24; //38
                map["S+2.0+5.0+3+0+0+2+0+0+1+0"] = 21.7; //39

                map["P+1.0+3.0+3+0+0+0+0+3+0+0"] = 13.59; //40
                map["P+2.0+3.0+3+0+0+0+0+1+1+0"] = 34.14; //41
                map["P+2.0+5.0+4+0+0+0+0+3+1+0"] = 9.81; //42
                map["P+2.0+5.0+4+1+0+0+0+3+1+0"] = 23.47; //43
            }
        }

        /// <summary>
        /// Gets the specification attribute of the TPSADescriptor object.
        /// </summary>
        /// <returns>The specification value</returns>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#tpsa",
                typeof(TPSADescriptor).FullName,
                "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the <see cref="TPSADescriptor"/> object.
        /// <para>
        /// The descriptor takes a bool parameter to indicate whether
        /// the descriptor routine should check for aromaticity (<see langword="true"/>) or
        /// not (<see langword="false"/>).</para>
        /// </summary>
        public override object[] Parameters
        {
            set
            {
                if (value.Length != 1)
                {
                    throw new CDKException("TPSADescriptor expects one parameter");
                }
                if (!(value[0] is bool))
                {
                    throw new CDKException("The first parameter must be of type bool");
                }
                // ok, all should be fine
                checkAromaticity = (bool)value[0];
            }
            get
            {
                // return the parameters as used for the descriptor calculation
                return new object[] { checkAromaticity };
            }
        }

        public override string[] DescriptorNames => NAMES;

        private DescriptorValue GetDummyDescriptorValue(Exception e)
        {
            return new DescriptorValue(_Specification, ParameterNames, Parameters, new DoubleResult(double.NaN), DescriptorNames, e);
        }

        /// <summary>
        /// Calculates the TPSA for an atom container.
        /// <p/>
        /// Before calling this method, you may want to set the parameter
        /// indicating that aromaticity should be checked. If no parameter is specified
        /// (or if it is set to <see langword="false"/>) then it is assumed that aromaticaity has already been
        /// checked.
        /// <p/>
        /// Prior to calling this method it is necessary to either add implicit or explicit hydrogens
        /// using <see cref="Tools.CDKHydrogenAdder.AddImplicitHydrogens(IAtomContainer)"/> or
        /// <see cref="AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(IAtomContainer)"/>.
        /// </summary>
        /// <param name="atomContainer">The AtomContainer whose TPSA is to be calculated</param>
        /// <returns>A double containing the topological surface area</returns>
        public override DescriptorValue Calculate(IAtomContainer atomContainer)
        {
            IAtomContainer ac = (IAtomContainer)atomContainer.Clone();
            List<string> profiles = new List<string>();

            // calculate the set of all rings
            IRingSet rs;
            try
            {
                rs = (new AllRingsFinder()).FindAllRings(ac);
            }
            catch (CDKException e)
            {
                return GetDummyDescriptorValue(e);
            }
            // check aromaticity if the descriptor parameter is set to true
            if (checkAromaticity)
            {
                try
                {
                    AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(ac);
                    Aromaticity.CDKLegacy.Apply(ac);
                }
                catch (CDKException e)
                {
                    return GetDummyDescriptorValue(e);
                }
            }

            // iterate over all atoms of ac
            foreach (var atom in ac.Atoms)
            {
                if (atom.Symbol.Equals("N") || atom.Symbol.Equals("O") || atom.Symbol.Equals("S") || atom.Symbol.Equals("P"))
                {
                    int singleBondCount = 0;
                    int doubleBondCount = 0;
                    int tripleBondCount = 0;
                    int aromaticBondCount = 0;
                    double maxBondOrder = 0;
                    double bondOrderSum = 0;
                    int hCount = 0;
                    int isIn3MemberRing = 0;

                    // counting the number of single/double/triple/aromatic bonds
                    var connectedBonds = ac.GetConnectedBonds(atom);
                    foreach (var connectedBond in connectedBonds)
                    {
                        if (connectedBond.IsAromatic)
                            aromaticBondCount++;
                        else if (connectedBond.Order == BondOrder.Single)
                            singleBondCount++;
                        else if (connectedBond.Order == BondOrder.Double)
                            doubleBondCount++;
                        else if (connectedBond.Order == BondOrder.Triple) tripleBondCount++;
                    }
                    int formalCharge = atom.FormalCharge.Value;
                    var connectedAtoms = ac.GetConnectedAtoms(atom).ToList();
                    int numberOfNeighbours = connectedAtoms.Count;

                    // EXPLICIT hydrogens: count the number of hydrogen atoms
                    for (int neighbourIndex = 0; neighbourIndex < numberOfNeighbours; neighbourIndex++)
                        if (((IAtom)connectedAtoms[neighbourIndex]).Symbol.Equals("H")) hCount++;
                    // IMPLICIT hydrogens: count the number of hydrogen atoms and adjust other atom profile properties
                    int implicitHAtoms = atom.ImplicitHydrogenCount ?? 0;

                    for (int hydrogenIndex = 0; hydrogenIndex < implicitHAtoms; hydrogenIndex++)
                    {
                        hCount++;
                        numberOfNeighbours++;
                        singleBondCount++;
                    }
                    // Calculate bond order sum using the counters of single/double/triple/aromatic bonds
                    bondOrderSum += singleBondCount * 1.0;
                    bondOrderSum += doubleBondCount * 2.0;
                    bondOrderSum += tripleBondCount * 3.0;
                    bondOrderSum += aromaticBondCount * 1.5;
                    // setting maxBondOrder
                    if (singleBondCount > 0) maxBondOrder = 1.0;
                    if (aromaticBondCount > 0) maxBondOrder = 1.5;
                    if (doubleBondCount > 0) maxBondOrder = 2.0;
                    if (tripleBondCount > 0) maxBondOrder = 3.0;

                    // isIn3MemberRing checker
                    if (rs.Contains(atom))
                    {
                        var rsAtom = rs.GetRings(atom);
                        foreach (var ring in rsAtom)
                        {
                            if (ring.RingSize == 3) isIn3MemberRing = 1;
                        }
                    }
                    // create a profile of the current atom (atoms[atomIndex]) according to the profile definition in the constructor
                    string profile = atom.Symbol + "+" + maxBondOrder.ToString("F1") + "+" + bondOrderSum.ToString("F1") + "+" + numberOfNeighbours
                            + "+" + hCount + "+" + formalCharge + "+" + aromaticBondCount + "+" + isIn3MemberRing + "+"
                            + singleBondCount + "+" + doubleBondCount + "+" + tripleBondCount;
                    //Debug.WriteLine("tpsa profile: "+ profile);
                    profiles.Add(profile);
                }
            }
            // END OF ATOM LOOP
            // calculate the tpsa for the AtomContainer ac
            double tpsa = 0;
            for (int profileIndex = 0; profileIndex < profiles.Count; profileIndex++)
            {
                if (map.ContainsKey(profiles[profileIndex]))
                {
                    tpsa += (double)map[profiles[profileIndex]];
                    //Debug.WriteLine("tpsa contribs: " + profiles.ElementAt(profileIndex) + "\t" + ((double)map[profiles.ElementAt(profileIndex])).Value);
                }
            }
            profiles.Clear(); // remove all profiles from the profiles-Vector
                              //Debug.WriteLine("tpsa: " + tpsa);

            return new DescriptorValue(_Specification, ParameterNames, Parameters, new DoubleResult(tpsa), DescriptorNames);
        }

        /// <inheritdoc/>
        public override IDescriptorResult DescriptorResultType { get; } = new DoubleResult(0.0);

        /// <summary>
        /// Gets the parameterNames attribute of the  TPSADescriptor object.
        /// </summary>
        /// <returns>The parameterNames value</returns>
        public override string[] ParameterNames { get; } = new string[] { "checkAromaticity" };

        /// <summary>
        /// Gets the parameterType attribute of the TPSADescriptor object.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public override object GetParameterType(string name) => true;
    }
}
