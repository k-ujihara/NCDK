/* Copyright (C) 2004-2007  Matteo Floris <mfe4@users.sf.net>
 * Copyright (C) 2006-2007  Federico
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
using NCDK.Common.Mathematics;
using NCDK.Numerics;
using NCDK.Aromaticities;
using NCDK.Charges;
using NCDK.Graphs.Invariant;
using NCDK.QSAR.Result;
using NCDK.RingSearches;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /**
     * This class calculates GHR proton descriptors used in neural networks for H1 NMR
     * shift {@cdk.cite AiresDeSousa2002}. It only applies to (explicit) hydrogen atoms,
     * requires aromaticity to be perceived (possibly done via a parameter), and
     * needs 3D coordinates for all atoms.
     * <p/>
     * <p>This descriptor uses these parameters:
     * <table border="1">
     * <tr>
     * <td>Name</td>
     * <td>Default</td>
     * <td>Description</td>
     * </tr>
     * <tr>
     * <td>checkAromaticity</td>
     * <td>false</td>
     * <td>True is the aromaticity has to be checked</td>
     * </tr>
     * </table>
     *
     * @author      Federico
     * @cdk.created 2006-12-11
     * @cdk.module  qsaratomic
     * @cdk.githash
     * @cdk.set     qsar-descriptors
     * @cdk.dictref qsar-descriptors:rdfProtonCalculatedValues
     * @cdk.bug     1632419
     */
    public class RDFProtonDescriptor_GHR : AbstractAtomicDescriptor, IAtomicDescriptor
    {
        private bool checkAromaticity = false;
        private IAtomContainer acold = null;
        private IRingSet varRingSet = null;
        private IAtomContainerSet<IAtomContainer> varAtomContainerSet = null;
        private static string[] descriptorNames;
        private const int ghr_desc_length = 15;

        /// <summary>
        /// Constructor for the RDFProtonDescriptor object
        /// </summary>
        public RDFProtonDescriptor_GHR()
        {
            descriptorNames = new string[ghr_desc_length];
            for (int i = 0; i < ghr_desc_length; i++)
            {
                descriptorNames[i] = "RDF_GHR_" + i;
            }
        }

        /// <summary>
        /// The specification attribute of the RDFProtonDescriptor_GHR object
        /// </summary>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#rdfProtonCalculatedValues",
                typeof(RDFProtonDescriptor_GHR).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the RDFProtonDescriptor object
        /// </summary>
        /// <exception cref="CDKException">Possible Exceptions</exception>
        public override object[] Parameters
        {
            set
            {
                if (value.Length > 1)
                {
                    throw new CDKException("RDFProtonDescriptor only expects one parameters");
                }
                if (!(value[0] is bool))
                {
                    throw new CDKException("The second parameter must be of type bool");
                }
                checkAromaticity = (bool)value[0];
            }
            get
            {
                // return the parameters as used for the descriptor calculation
                return new object[] { checkAromaticity };
            }
        }

        public override string[] DescriptorNames => descriptorNames;

        private DescriptorValue GetDummyDescriptorValue(Exception e)
        {
            DoubleArrayResult result = new DoubleArrayResult(ghr_desc_length);
            for (int i = 0; i < ghr_desc_length; i++)
                result.Add(double.NaN);
            return new DescriptorValue(_Specification, ParameterNames, Parameters, result,
                    DescriptorNames, e);
        }

        public override DescriptorValue Calculate(IAtom atom, IAtomContainer varAtomContainerSet)
        {
            return (Calculate(atom, varAtomContainerSet, null));
        }

        public DescriptorValue Calculate(IAtom atom, IAtomContainer atomContainer, IRingSet precalculatedringset)
        {
            IAtomContainer varAtomContainer;
            varAtomContainer = (IAtomContainer)atomContainer.Clone();

            int atomPosition = atomContainer.Atoms.IndexOf(atom);
            IAtom clonedAtom = varAtomContainer.Atoms[atomPosition];

            DoubleArrayResult rdfProtonCalculatedValues = new DoubleArrayResult(ghr_desc_length);
            if (!atom.Symbol.Equals("H"))
            {
                return GetDummyDescriptorValue(new CDKException("Invalid atom specified"));
            }

            /////////////////////////FIRST SECTION OF MAIN METHOD: DEFINITION OF MAIN VARIABLES
            /////////////////////////AND AROMATICITY AND PI-SYSTEM AND RINGS DETECTION

            IAtomContainer mol = varAtomContainer.Builder.CreateAtomContainer(varAtomContainer);
            if (varAtomContainer != acold)
            {
                acold = varAtomContainer;
                // DETECTION OF pi SYSTEMS
                varAtomContainerSet = ConjugatedPiSystemsDetector.Detect(mol);
                if (precalculatedringset == null)
                    try
                    {
                        varRingSet = (new AllRingsFinder()).FindAllRings(varAtomContainer);
                    }
                    catch (CDKException e)
                    {
                        return GetDummyDescriptorValue(e);
                    }
                else
                    varRingSet = precalculatedringset;
                try
                {
                    GasteigerMarsiliPartialCharges peoe = new GasteigerMarsiliPartialCharges();
                    peoe.AssignGasteigerMarsiliSigmaPartialCharges(mol, true);
                }
                catch (Exception ex1)
                {
                    return GetDummyDescriptorValue(ex1);
                }
            }
            if (checkAromaticity)
            {
                try
                {
                    AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(varAtomContainer);
                    Aromaticity.CDKLegacy.Apply(varAtomContainer);
                }
                catch (CDKException e)
                {
                    return GetDummyDescriptorValue(e);
                }
            }
            // SET ISINRING FLAGS FOR BONDS
            var bondsInContainer = varAtomContainer.Bonds;
            foreach (var bond in bondsInContainer)
            {
                var ringsWithThisBond = varRingSet.GetRings(bond);
                if (ringsWithThisBond.Any())
                {
                    bond.IsInRing = true;
                }
            }

            // SET ISINRING FLAGS FOR ATOMS

            for (int w = 0; w < varAtomContainer.Atoms.Count; w++)
            {
                var ringsWithThisAtom = varRingSet.GetRings(varAtomContainer.Atoms[w]);
                if (ringsWithThisAtom.Any())
                {
                    varAtomContainer.Atoms[w].IsInRing = true;
                }
            }

            IAtomContainer detected = varAtomContainerSet.FirstOrDefault();

            // neighboors[0] is the atom joined to the target proton:
            var neighboors = mol.GetConnectedAtoms(clonedAtom);
            IAtom neighbour0 = neighboors.First();

            // 2', 3', 4', 5', 6', and 7' atoms up to the target are detected:
            var atomsInSecondSphere = mol.GetConnectedAtoms(neighbour0);

            // SOME LISTS ARE CREATED FOR STORING OF INTERESTING ATOMS AND BONDS DURING DETECTION
            List<int> singles = new List<int>(); // list of any bond not rotatable
            List<int> doubles = new List<int>(); // list with only double bonds
            List<int> atoms = new List<int>(); // list with all the atoms in spheres
                                               //atoms.Add( int.ValueOf( mol.Atoms.IndexOf(neighboors[0]) ) );
            List<int> bondsInCycloex = new List<int>(); // list for bonds in cycloexane-like rings

            // 2', 3', 4', 5', 6', and 7' bonds up to the target are detected:
            IBond secondBond; // (remember that first bond is proton bond)
            IBond thirdBond; //
            IBond fourthBond; //
            IBond fifthBond; //
            IBond sixthBond; //
            IBond seventhBond; //

            // definition of some variables used in the main FOR loop for detection of interesting atoms and bonds:
            bool theBondIsInA6MemberedRing; // this is like a flag for bonds which are in cycloexane-like rings (rings with more than 4 at.)
            BondOrder bondOrder;
            int bondNumber;
            int sphere;

            // THIS MAIN FOR LOOP DETECT RIGID BONDS IN 7 SPHERES:
            foreach (var anAtomsInSecondSphere in atomsInSecondSphere)
            {
                IAtom curAtomSecond = (IAtom)anAtomsInSecondSphere;
                secondBond = mol.GetBond(neighbour0, curAtomSecond);
                if (mol.Atoms.IndexOf(curAtomSecond) != atomPosition && GetIfBondIsNotRotatable(mol, secondBond, detected))
                {
                    sphere = 2;
                    bondOrder = secondBond.Order;
                    bondNumber = mol.Bonds.IndexOf(secondBond);
                    theBondIsInA6MemberedRing = false;
                    CheckAndStore(bondNumber, bondOrder, singles, doubles, bondsInCycloex,
                            mol.Atoms.IndexOf(curAtomSecond), atoms, sphere, theBondIsInA6MemberedRing);
                    var atomsInThirdSphere = mol.GetConnectedAtoms(curAtomSecond);
                    foreach (var anAtomsInThirdSphere in atomsInThirdSphere)
                    {
                        IAtom curAtomThird = (IAtom)anAtomsInThirdSphere;
                        thirdBond = mol.GetBond(curAtomThird, curAtomSecond);
                        // IF THE ATOMS IS IN THE THIRD SPHERE AND IN A CYCLOEXANE-LIKE RING, IT IS STORED IN THE PROPER LIST:
                        if (mol.Atoms.IndexOf(curAtomThird) != atomPosition
                                && GetIfBondIsNotRotatable(mol, thirdBond, detected))
                        {
                            sphere = 3;
                            bondOrder = thirdBond.Order;
                            bondNumber = mol.Bonds.IndexOf(thirdBond);
                            theBondIsInA6MemberedRing = false;

                            // if the bond is in a cyclohexane-like ring (a ring with 5 or more atoms, not aromatic)
                            // the bool "theBondIsInA6MemberedRing" is set to true
                            if (!thirdBond.IsAromatic)
                            {
                                if (!curAtomThird.Equals(neighbour0))
                                {
                                    var rsAtom = varRingSet.GetRings(thirdBond);
                                    foreach (var ring in rsAtom)
                                    {
                                        if (ring.RingSize > 4 && ring.Contains(thirdBond))
                                        {
                                            theBondIsInA6MemberedRing = true;
                                        }
                                    }
                                }
                            }
                            CheckAndStore(bondNumber, bondOrder, singles, doubles, bondsInCycloex,
                                    mol.Atoms.IndexOf(curAtomThird), atoms, sphere, theBondIsInA6MemberedRing);
                            theBondIsInA6MemberedRing = false;
                            var atomsInFourthSphere = mol.GetConnectedAtoms(curAtomThird);
                            foreach (var anAtomsInFourthSphere in atomsInFourthSphere)
                            {
                                IAtom curAtomFourth = (IAtom)anAtomsInFourthSphere;
                                fourthBond = mol.GetBond(curAtomThird, curAtomFourth);
                                if (mol.Atoms.IndexOf(curAtomFourth) != atomPosition
                                        && GetIfBondIsNotRotatable(mol, fourthBond, detected))
                                {
                                    sphere = 4;
                                    bondOrder = fourthBond.Order;
                                    bondNumber = mol.Bonds.IndexOf(fourthBond);
                                    theBondIsInA6MemberedRing = false;
                                    CheckAndStore(bondNumber, bondOrder, singles, doubles, bondsInCycloex,
                                            mol.Atoms.IndexOf(curAtomFourth), atoms, sphere,
                                            theBondIsInA6MemberedRing);
                                    var atomsInFifthSphere = mol.GetConnectedAtoms(curAtomFourth);
                                    foreach (var anAtomsInFifthSphere in atomsInFifthSphere)
                                    {
                                        IAtom curAtomFifth = (IAtom)anAtomsInFifthSphere;
                                        fifthBond = mol.GetBond(curAtomFifth, curAtomFourth);
                                        if (mol.Atoms.IndexOf(curAtomFifth) != atomPosition
                                                && GetIfBondIsNotRotatable(mol, fifthBond, detected))
                                        {
                                            sphere = 5;
                                            bondOrder = fifthBond.Order;
                                            bondNumber = mol.Bonds.IndexOf(fifthBond);
                                            theBondIsInA6MemberedRing = false;
                                            CheckAndStore(bondNumber, bondOrder, singles, doubles,
                                                    bondsInCycloex, mol.Atoms.IndexOf(curAtomFifth), atoms,
                                                    sphere, theBondIsInA6MemberedRing);
                                            var atomsInSixthSphere = mol.GetConnectedAtoms(curAtomFifth);
                                            foreach (var anAtomsInSixthSphere in atomsInSixthSphere)
                                            {
                                                IAtom curAtomSixth = (IAtom)anAtomsInSixthSphere;
                                                sixthBond = mol.GetBond(curAtomFifth, curAtomSixth);
                                                if (mol.Atoms.IndexOf(curAtomSixth) != atomPosition
                                                        && GetIfBondIsNotRotatable(mol, sixthBond, detected))
                                                {
                                                    sphere = 6;
                                                    bondOrder = sixthBond.Order;
                                                    bondNumber = mol.Bonds.IndexOf(sixthBond);
                                                    theBondIsInA6MemberedRing = false;
                                                    CheckAndStore(bondNumber, bondOrder, singles, doubles,
                                                            bondsInCycloex,
                                                            mol.Atoms.IndexOf(curAtomSixth), atoms, sphere,
                                                            theBondIsInA6MemberedRing);
                                                    var atomsInSeventhSphere = mol.GetConnectedAtoms(curAtomSixth);
                                                    foreach (var anAtomsInSeventhSphere in atomsInSeventhSphere)
                                                    {
                                                        IAtom curAtomSeventh = (IAtom)anAtomsInSeventhSphere;
                                                        seventhBond = mol.GetBond(curAtomSeventh,
                                                                curAtomSixth);
                                                        if (mol.Atoms.IndexOf(curAtomSeventh) != atomPosition
                                                                && GetIfBondIsNotRotatable(mol, seventhBond, detected))
                                                        {
                                                            sphere = 7;
                                                            bondOrder = seventhBond.Order;
                                                            bondNumber = mol.Bonds.IndexOf(seventhBond);
                                                            theBondIsInA6MemberedRing = false;
                                                            CheckAndStore(bondNumber, bondOrder,
                                                                    singles, doubles, bondsInCycloex,
                                                                    mol.Atoms.IndexOf(curAtomSeventh),
                                                                    atoms, sphere,
                                                                    theBondIsInA6MemberedRing);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //Variables
            double distance;
            double sum;
            double smooth = -20;
            double partial;
            int position;
            double limitInf = 1.4;
            double limitSup = 4;
            double step = (limitSup - limitInf) / 15;
            IAtom atom2;

            ///////////////////////THE FIRST CALCULATED DESCRIPTOR IS g(H)r	 WITH PARTIAL CHARGES:
            if (atoms.Any())
            {
                for (double ghr = limitInf; ghr < limitSup; ghr = ghr + step)
                {
                    sum = 0;
                    foreach (var atom1 in atoms)
                    {
                        distance = 0;
                        partial = 0;
                        int thisAtom = (int)atom1;
                        position = thisAtom;
                        atom2 = mol.Atoms[position];
                        distance = CalculateDistanceBetweenTwoAtoms(atom, atom2);
                        partial = atom2.Charge.Value * Math.Exp(smooth * (Math.Pow((ghr - distance), 2)));
                        sum += partial;
                    }
                    rdfProtonCalculatedValues.Add(sum);

                    Debug.WriteLine("RDF gr distance prob.: " + sum + " at distance " + ghr);
                }
            }
            else
            {
                return GetDummyDescriptorValue(new CDKException("Some error occurred. Please report"));
            }
            return new DescriptorValue(_Specification, ParameterNames, Parameters, rdfProtonCalculatedValues,
                    DescriptorNames);
        }

        //Others definitions

        private bool GetIfBondIsNotRotatable(IAtomContainer mol, IBond bond, IAtomContainer detected)
        {
            bool isBondNotRotatable = false;
            int counter = 0;
            IAtom atom0 = bond.Atoms[0];
            IAtom atom1 = bond.Atoms[1];
            if (detected != null)
            {
                if (detected.Contains(bond)) counter += 1;
            }
            if (atom0.IsInRing)
            {
                if (atom1.IsInRing)
                {
                    counter += 1;
                }
                else
                {
                    if (atom1.Symbol.Equals("H"))
                        counter += 1;
                    else
                        counter += 0;
                }
            }
            if (atom0.Symbol.Equals("N") && atom1.Symbol.Equals("C"))
            {
                if (GetIfACarbonIsDoubleBondedToAnOxygen(mol, atom1)) counter += 1;
            }
            if (atom0.Symbol.Equals("C") && atom1.Symbol.Equals("N"))
            {
                if (GetIfACarbonIsDoubleBondedToAnOxygen(mol, atom0)) counter += 1;
            }
            if (counter > 0) isBondNotRotatable = true;
            return isBondNotRotatable;
        }

        private bool GetIfACarbonIsDoubleBondedToAnOxygen(IAtomContainer mol, IAtom carbonAtom)
        {
            bool isDoubleBondedToOxygen = false;
            var neighToCarbon = mol.GetConnectedAtoms(carbonAtom);
            IBond tmpBond;
            int counter = 0;
            foreach (var neighbour in neighToCarbon)
            {
                if (neighbour.Symbol.Equals("O"))
                {
                    tmpBond = mol.GetBond(neighbour, carbonAtom);
                    if (tmpBond.Order == BondOrder.Double) counter += 1;
                }
            }
            if (counter > 0) isDoubleBondedToOxygen = true;
            return isDoubleBondedToOxygen;
        }

        // this method calculates the angle between two bonds given coordinates of their atoms

        private double CalculateAngleBetweenTwoLines(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            Vector3 firstLine = a - b;
            Vector3 secondLine = c - d;
            Vector3 firstVec = firstLine;
            Vector3 secondVec = secondLine;
            return Vectors.Angle(firstVec, secondVec);
        }

        // this method store atoms and bonds in proper lists:
        private void CheckAndStore(int bondToStore, BondOrder bondOrder, List<int> singleVec,
                List<int> doubleVec, List<int> cycloexVec, int a1, List<int> atomVec,
                int sphere, bool isBondInCycloex)
        {
            if (!atomVec.Contains(a1))
            {
                if (sphere < 6) atomVec.Add(a1);
            }
            if (!cycloexVec.Contains(bondToStore))
            {
                if (isBondInCycloex)
                {
                    cycloexVec.Add(bondToStore);
                }
            }
            if (bondOrder == BondOrder.Double)
            {
                if (!doubleVec.Contains(bondToStore)) doubleVec.Add(bondToStore);
            }
            if (bondOrder == BondOrder.Single)
            {
                if (!singleVec.Contains(bondToStore)) singleVec.Add(bondToStore);
            }
        }

        // generic method for calculation of distance btw 2 atoms
        private double CalculateDistanceBetweenTwoAtoms(IAtom atom1, IAtom atom2)
        {
            double distance;
            Vector3 firstPoint = atom1.Point3D.Value;
            Vector3 secondPoint = atom2.Point3D.Value;
            distance = Vector3.Distance(firstPoint, secondPoint);
            return distance;
        }

        // given a double bond
        // this method returns a bond bonded to this double bond
        private int GetNearestBondtoAGivenAtom(IAtomContainer mol, IAtom atom, IBond bond)
        {
            int nearestBond = -1;
            double[] values;
            double distance = 0;
            IAtom atom0 = bond.Atoms[0];
            var bondsAtLeft = mol.GetConnectedBonds(atom0);
            int partial;
            foreach (var curBond in bondsAtLeft)
            {
                values = CalculateDistanceBetweenAtomAndBond(atom, curBond);
                partial = mol.Bonds.IndexOf(curBond);
                if (nearestBond == -1)
                {
                    nearestBond = mol.Bonds.IndexOf(curBond);
                    distance = values[0];
                }
                else
                {
                    if (values[0] < distance)
                    {
                        nearestBond = partial;
                    }
                }
            }
            return nearestBond;

        }

        // method which calculated distance btw an atom and the middle point of a bond
        // and returns distance and coordinates of middle point
        private double[] CalculateDistanceBetweenAtomAndBond(IAtom proton, IBond theBond)
        {
            Vector3 middlePoint = theBond.Geometric3DCenter;
            Vector3 protonPoint = proton.Point3D.Value;
            double[] values = new double[4];
            values[0] = Vector3.Distance(middlePoint, protonPoint);
            values[1] = middlePoint.X;
            values[2] = middlePoint.Y;
            values[3] = middlePoint.Z;
            return values;
        }

        /// <summary>
        /// Tthe parameterNames attribute of the RDFProtonDescriptor object
        /// </summary>
        public override string[] ParameterNames { get; } = new string[] { "checkAromaticity" };

        /// <summary>
        /// Gets the parameterType attribute of the RDFProtonDescriptor object
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public override object GetParameterType(string name)
        {
            if (name.Equals("checkAromaticity")) return true;
            return null;
        }
    }
}
