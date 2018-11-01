
using NCDK.Common.Mathematics;
using NCDK.Numerics;
using NCDK.Aromaticities;
using NCDK.Charges;
using NCDK.Graphs.Invariant;
using NCDK.QSAR.Results;
using NCDK.RingSearches;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Linq;
using NCDK.Config;

namespace NCDK.QSAR.Descriptors.Atomic
{
    public partial class RDFProtonDescriptorG3R
    {
        private bool CheckAromaticity { get; set; } = false;
        private IAtomContainer acold = null;
        private IRingSet varRingSet = null;
        private IChemObjectSet<IAtomContainer> varAtomContainerSet = null;

        /// <summary>
        /// The parameters attribute of the RDFProtonDescriptor object
        /// </summary>
        /// <value>Parameters are the proton position and a boolean (<see langword="true"/> if you need to detect aromaticity)</value>
        /// <exception cref="CDKException"></exception>
        public virtual IReadOnlyList<object> Parameters
        {
            set
            {
                if (value.Count > 1)
                {
                    throw new CDKException($"{nameof(RDFProtonDescriptorG3R)} only expects one parameters");
                }
                if (!(value[0] is bool))
                {
                    throw new CDKException($"The second parameter must be of type {nameof(System.Boolean)}");
                }
                CheckAromaticity = (bool)value[0];
            }
            get
            {
                // return the parameters as used for the descriptor calculation
                return new object[] { CheckAromaticity };
            }
        }

        private DescriptorValue<ArrayResult<double>> GetDummyDescriptorValue(Exception e)
        {
            ArrayResult<double> result = new ArrayResult<double>(desc_length);
            for (int i = 0; i < desc_length; i++)
                result.Add(double.NaN);
            return new DescriptorValue<ArrayResult<double>>(specification, ParameterNames, Parameters, result, DescriptorNames, e);
        }

        public virtual IDescriptorValue Calculate(IAtom atom, IAtomContainer varAtomContainerSet)
        {
            return (Calculate(atom, varAtomContainerSet, null));
        }

        public DescriptorValue<ArrayResult<double>> Calculate(IAtom atom, IAtomContainer atomContainer, IRingSet precalculatedringset)
        {
            var varAtomContainer = (IAtomContainer)atomContainer.Clone();

            int atomPosition = atomContainer.Atoms.IndexOf(atom);
            var clonedAtom = varAtomContainer.Atoms[atomPosition];
            var rdfProtonCalculatedValues = new ArrayResult<double>(desc_length);
            if (!atom.AtomicNumber.Equals(ChemicalElement.AtomicNumbers.H))
            {
                return GetDummyDescriptorValue(new CDKException("Invalid atom specified"));
            }

            // ///////////////////////FIRST SECTION OF MAIN METHOD: DEFINITION OF MAIN VARIABLES
            // ///////////////////////AND AROMATICITY AND PI-SYSTEM AND RINGS DETECTION

            IAtomContainer mol = varAtomContainer.Builder.NewAtomContainer(varAtomContainer);
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
            if (CheckAromaticity)
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

            // neighbors[0] is the atom joined to the target proton:
            var neighbors = mol.GetConnectedAtoms(clonedAtom);
            IAtom neighbour0 = neighbors.First();

            // 2', 3', 4', 5', 6', and 7' atoms up to the target are detected:
            var atomsInSecondSphere = mol.GetConnectedAtoms(neighbour0);

            // SOME LISTS ARE CREATED FOR STORING OF INTERESTING ATOMS AND BONDS DURING DETECTION
            List<int> singles = new List<int>(); // list of any bond not rotatable
            List<int> doubles = new List<int>(); // list with only double bonds
            List<int> atoms = new List<int>(); // list with all the atoms in spheres
                                               // atoms.Add( int.ValueOf( mol.Atoms.IndexOf(neighbors[0]) ) );
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
            foreach (var curAtomSecond in atomsInSecondSphere)
            {
                secondBond = mol.GetBond(neighbour0, curAtomSecond);
                if (mol.Atoms.IndexOf(curAtomSecond) != atomPosition && GetIfBondIsNotRotatable(mol, secondBond, detected))
                {
                    sphere = 2;
                    bondOrder = secondBond.Order;
                    bondNumber = mol.Bonds.IndexOf(secondBond);
                    theBondIsInA6MemberedRing = false;
                    CheckAndStore(bondNumber, bondOrder, singles, doubles, bondsInCycloex, mol.Atoms.IndexOf(curAtomSecond), atoms, sphere, theBondIsInA6MemberedRing);
                    var atomsInThirdSphere = mol.GetConnectedAtoms(curAtomSecond);
                    foreach (var curAtomThird in atomsInThirdSphere)
                    {
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
                            foreach (var curAtomFourth in atomsInFourthSphere)
                            {
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
                                    foreach (var curAtomFifth in atomsInFifthSphere)
                                    {
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
                                            foreach (var curAtomSixth in atomsInSixthSphere)
                                            {
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
                                                    foreach (var curAtomSeventh in atomsInSeventhSphere)
                                                    {
                                                        seventhBond = mol.GetBond(curAtomSeventh, curAtomSixth);
                                                        if (mol.Atoms.IndexOf(curAtomSeventh) != atomPosition && GetIfBondIsNotRotatable(mol, seventhBond, detected))
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

            if (MakeDescriptorLastStage(
                rdfProtonCalculatedValues, 
                atom, 
                clonedAtom,
                mol,
                neighbour0, 
                singles,
                doubles,
                atoms, 
                bondsInCycloex))
            {
                return new DescriptorValue<ArrayResult<double>>(specification, ParameterNames, Parameters, rdfProtonCalculatedValues, DescriptorNames);
            }
            else
            {
                return GetDummyDescriptorValue(new CDKException("Some error occurred. Please report"));
            }
        }

        // Others definitions

        private static bool GetIfBondIsNotRotatable(IAtomContainer mol, IBond bond, IAtomContainer detected)
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
                    if (atom1.AtomicNumber.Equals(ChemicalElement.AtomicNumbers.H))
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

        private static bool GetIfACarbonIsDoubleBondedToAnOxygen(IAtomContainer mol, IAtom carbonAtom)
        {
            bool isDoubleBondedToOxygen = false;
            var neighToCarbon = mol.GetConnectedAtoms(carbonAtom);
            IBond tmpBond;
            int counter = 0;
            foreach (var neighbour in neighToCarbon)
            {
                if (neighbour.AtomicNumber.Equals(ChemicalElement.AtomicNumbers.O))
                {
                    tmpBond = mol.GetBond(neighbour, carbonAtom);
                    if (tmpBond.Order == BondOrder.Double) counter += 1;
                }
            }
            if (counter > 0) isDoubleBondedToOxygen = true;
            return isDoubleBondedToOxygen;
        }

        // this method calculates the angle between two bonds given coordinates of their atoms

        internal static double CalculateAngleBetweenTwoLines(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            Vector3 firstLine = a - b;
            Vector3 secondLine = c - d;
            Vector3 firstVec = firstLine;
            Vector3 secondVec = secondLine;
            return Vectors.Angle(firstVec, secondVec);
        }

        // this method store atoms and bonds in proper lists:
        private static void CheckAndStore(int bondToStore, BondOrder bondOrder, List<int> singleVec,
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
        internal static double CalculateDistanceBetweenTwoAtoms(IAtom atom1, IAtom atom2)
        {
            double distance;
            Vector3 firstPoint = atom1.Point3D.Value;
            Vector3 secondPoint = atom2.Point3D.Value;
            distance = Vector3.Distance(firstPoint, secondPoint);
            return distance;
        }

        // given a double bond
        // this method returns a bond bonded to this double bond
        internal static int GetNearestBondtoAGivenAtom(IAtomContainer mol, IAtom atom, IBond bond)
        {
            int nearestBond = -1;
            double distance = 0;
            IAtom atom0 = bond.Atoms[0];
            var bondsAtLeft = mol.GetConnectedBonds(atom0);
            foreach (var curBond in bondsAtLeft)
            {
                var values = CalculateDistanceBetweenAtomAndBond(atom, curBond);
                var partial = mol.Bonds.IndexOf(curBond);
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
        internal static double[] CalculateDistanceBetweenAtomAndBond(IAtom proton, IBond theBond)
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
        /// The parameterNames attribute of the RDFProtonDescriptor object
        /// </summary>
        public virtual IReadOnlyList<string> ParameterNames { get; } = new string[] { "checkAromaticity" };

        /// <summary>
        /// Gets the parameterType attribute of the RDFProtonDescriptor object
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public virtual object GetParameterType(string name)
        {
            if (string.Equals(name, "checkAromaticity", StringComparison.Ordinal)) return true;
            return null;
        }
    }
    public partial class RDFProtonDescriptorGDR
    {
        private bool CheckAromaticity { get; set; } = false;
        private IAtomContainer acold = null;
        private IRingSet varRingSet = null;
        private IChemObjectSet<IAtomContainer> varAtomContainerSet = null;

        /// <summary>
        /// The parameters attribute of the RDFProtonDescriptor object
        /// </summary>
        /// <value>Parameters are the proton position and a boolean (<see langword="true"/> if you need to detect aromaticity)</value>
        /// <exception cref="CDKException"></exception>
        public virtual IReadOnlyList<object> Parameters
        {
            set
            {
                if (value.Count > 1)
                {
                    throw new CDKException($"{nameof(RDFProtonDescriptorGDR)} only expects one parameters");
                }
                if (!(value[0] is bool))
                {
                    throw new CDKException($"The second parameter must be of type {nameof(System.Boolean)}");
                }
                CheckAromaticity = (bool)value[0];
            }
            get
            {
                // return the parameters as used for the descriptor calculation
                return new object[] { CheckAromaticity };
            }
        }

        private DescriptorValue<ArrayResult<double>> GetDummyDescriptorValue(Exception e)
        {
            ArrayResult<double> result = new ArrayResult<double>(desc_length);
            for (int i = 0; i < desc_length; i++)
                result.Add(double.NaN);
            return new DescriptorValue<ArrayResult<double>>(specification, ParameterNames, Parameters, result, DescriptorNames, e);
        }

        public virtual IDescriptorValue Calculate(IAtom atom, IAtomContainer varAtomContainerSet)
        {
            return (Calculate(atom, varAtomContainerSet, null));
        }

        public DescriptorValue<ArrayResult<double>> Calculate(IAtom atom, IAtomContainer atomContainer, IRingSet precalculatedringset)
        {
            IAtomContainer varAtomContainer = (IAtomContainer)atomContainer.Clone();

            int atomPosition = atomContainer.Atoms.IndexOf(atom);
            IAtom clonedAtom = varAtomContainer.Atoms[atomPosition];
            ArrayResult<double> rdfProtonCalculatedValues = new ArrayResult<double>(desc_length);
            if (!atom.AtomicNumber.Equals(ChemicalElement.AtomicNumbers.H))
            {
                return GetDummyDescriptorValue(new CDKException("Invalid atom specified"));
            }

            // ///////////////////////FIRST SECTION OF MAIN METHOD: DEFINITION OF MAIN VARIABLES
            // ///////////////////////AND AROMATICITY AND PI-SYSTEM AND RINGS DETECTION

            IAtomContainer mol = varAtomContainer.Builder.NewAtomContainer(varAtomContainer);
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
            if (CheckAromaticity)
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

            // neighbors[0] is the atom joined to the target proton:
            var neighbors = mol.GetConnectedAtoms(clonedAtom);
            IAtom neighbour0 = neighbors.First();

            // 2', 3', 4', 5', 6', and 7' atoms up to the target are detected:
            var atomsInSecondSphere = mol.GetConnectedAtoms(neighbour0);

            // SOME LISTS ARE CREATED FOR STORING OF INTERESTING ATOMS AND BONDS DURING DETECTION
            List<int> singles = new List<int>(); // list of any bond not rotatable
            List<int> doubles = new List<int>(); // list with only double bonds
            List<int> atoms = new List<int>(); // list with all the atoms in spheres
                                               // atoms.Add( int.ValueOf( mol.Atoms.IndexOf(neighbors[0]) ) );
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
            foreach (var curAtomSecond in atomsInSecondSphere)
            {
                secondBond = mol.GetBond(neighbour0, curAtomSecond);
                if (mol.Atoms.IndexOf(curAtomSecond) != atomPosition && GetIfBondIsNotRotatable(mol, secondBond, detected))
                {
                    sphere = 2;
                    bondOrder = secondBond.Order;
                    bondNumber = mol.Bonds.IndexOf(secondBond);
                    theBondIsInA6MemberedRing = false;
                    CheckAndStore(bondNumber, bondOrder, singles, doubles, bondsInCycloex, mol.Atoms.IndexOf(curAtomSecond), atoms, sphere, theBondIsInA6MemberedRing);
                    var atomsInThirdSphere = mol.GetConnectedAtoms(curAtomSecond);
                    foreach (var curAtomThird in atomsInThirdSphere)
                    {
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
                            foreach (var curAtomFourth in atomsInFourthSphere)
                            {
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
                                    foreach (var curAtomFifth in atomsInFifthSphere)
                                    {
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
                                            foreach (var curAtomSixth in atomsInSixthSphere)
                                            {
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
                                                    foreach (var curAtomSeventh in atomsInSeventhSphere)
                                                    {
                                                        seventhBond = mol.GetBond(curAtomSeventh, curAtomSixth);
                                                        if (mol.Atoms.IndexOf(curAtomSeventh) != atomPosition && GetIfBondIsNotRotatable(mol, seventhBond, detected))
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

            if (MakeDescriptorLastStage(
                rdfProtonCalculatedValues, 
                atom, 
                clonedAtom,
                mol,
                neighbour0, 
                singles,
                doubles,
                atoms, 
                bondsInCycloex))
            {
                return new DescriptorValue<ArrayResult<double>>(specification, ParameterNames, Parameters, rdfProtonCalculatedValues, DescriptorNames);
            }
            else
            {
                return GetDummyDescriptorValue(new CDKException("Some error occurred. Please report"));
            }
        }

        // Others definitions

        private static bool GetIfBondIsNotRotatable(IAtomContainer mol, IBond bond, IAtomContainer detected)
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
                    if (atom1.AtomicNumber.Equals(ChemicalElement.AtomicNumbers.H))
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

        private static bool GetIfACarbonIsDoubleBondedToAnOxygen(IAtomContainer mol, IAtom carbonAtom)
        {
            bool isDoubleBondedToOxygen = false;
            var neighToCarbon = mol.GetConnectedAtoms(carbonAtom);
            IBond tmpBond;
            int counter = 0;
            foreach (var neighbour in neighToCarbon)
            {
                if (neighbour.AtomicNumber.Equals(ChemicalElement.AtomicNumbers.O))
                {
                    tmpBond = mol.GetBond(neighbour, carbonAtom);
                    if (tmpBond.Order == BondOrder.Double) counter += 1;
                }
            }
            if (counter > 0) isDoubleBondedToOxygen = true;
            return isDoubleBondedToOxygen;
        }

        // this method calculates the angle between two bonds given coordinates of their atoms

        internal static double CalculateAngleBetweenTwoLines(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            Vector3 firstLine = a - b;
            Vector3 secondLine = c - d;
            Vector3 firstVec = firstLine;
            Vector3 secondVec = secondLine;
            return Vectors.Angle(firstVec, secondVec);
        }

        // this method store atoms and bonds in proper lists:
        private static void CheckAndStore(int bondToStore, BondOrder bondOrder, List<int> singleVec,
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
        internal static double CalculateDistanceBetweenTwoAtoms(IAtom atom1, IAtom atom2)
        {
            double distance;
            Vector3 firstPoint = atom1.Point3D.Value;
            Vector3 secondPoint = atom2.Point3D.Value;
            distance = Vector3.Distance(firstPoint, secondPoint);
            return distance;
        }

        // given a double bond
        // this method returns a bond bonded to this double bond
        internal static int GetNearestBondtoAGivenAtom(IAtomContainer mol, IAtom atom, IBond bond)
        {
            int nearestBond = -1;
            double distance = 0;
            IAtom atom0 = bond.Atoms[0];
            var bondsAtLeft = mol.GetConnectedBonds(atom0);
            foreach (var curBond in bondsAtLeft)
            {
                var values = CalculateDistanceBetweenAtomAndBond(atom, curBond);
                var partial = mol.Bonds.IndexOf(curBond);
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
        internal static double[] CalculateDistanceBetweenAtomAndBond(IAtom proton, IBond theBond)
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
        /// The parameterNames attribute of the RDFProtonDescriptor object
        /// </summary>
        public virtual IReadOnlyList<string> ParameterNames { get; } = new string[] { "checkAromaticity" };

        /// <summary>
        /// Gets the parameterType attribute of the RDFProtonDescriptor object
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public virtual object GetParameterType(string name)
        {
            if (string.Equals(name, "checkAromaticity", StringComparison.Ordinal)) return true;
            return null;
        }
    }
    public partial class RDFProtonDescriptorGHR
    {
        private bool CheckAromaticity { get; set; } = false;
        private IAtomContainer acold = null;
        private IRingSet varRingSet = null;
        private IChemObjectSet<IAtomContainer> varAtomContainerSet = null;

        /// <summary>
        /// The parameters attribute of the RDFProtonDescriptor object
        /// </summary>
        /// <value>Parameters are the proton position and a boolean (<see langword="true"/> if you need to detect aromaticity)</value>
        /// <exception cref="CDKException"></exception>
        public virtual IReadOnlyList<object> Parameters
        {
            set
            {
                if (value.Count > 1)
                {
                    throw new CDKException($"{nameof(RDFProtonDescriptorGHR)} only expects one parameters");
                }
                if (!(value[0] is bool))
                {
                    throw new CDKException($"The second parameter must be of type {nameof(System.Boolean)}");
                }
                CheckAromaticity = (bool)value[0];
            }
            get
            {
                // return the parameters as used for the descriptor calculation
                return new object[] { CheckAromaticity };
            }
        }

        private DescriptorValue<ArrayResult<double>> GetDummyDescriptorValue(Exception e)
        {
            ArrayResult<double> result = new ArrayResult<double>(desc_length);
            for (int i = 0; i < desc_length; i++)
                result.Add(double.NaN);
            return new DescriptorValue<ArrayResult<double>>(specification, ParameterNames, Parameters, result, DescriptorNames, e);
        }

        public virtual IDescriptorValue Calculate(IAtom atom, IAtomContainer varAtomContainerSet)
        {
            return (Calculate(atom, varAtomContainerSet, null));
        }

        public DescriptorValue<ArrayResult<double>> Calculate(IAtom atom, IAtomContainer atomContainer, IRingSet precalculatedringset)
        {
            IAtomContainer varAtomContainer = (IAtomContainer)atomContainer.Clone();

            int atomPosition = atomContainer.Atoms.IndexOf(atom);
            IAtom clonedAtom = varAtomContainer.Atoms[atomPosition];
            ArrayResult<double> rdfProtonCalculatedValues = new ArrayResult<double>(desc_length);
            if (!atom.AtomicNumber.Equals(ChemicalElement.AtomicNumbers.H))
            {
                return GetDummyDescriptorValue(new CDKException("Invalid atom specified"));
            }

            // ///////////////////////FIRST SECTION OF MAIN METHOD: DEFINITION OF MAIN VARIABLES
            // ///////////////////////AND AROMATICITY AND PI-SYSTEM AND RINGS DETECTION

            IAtomContainer mol = varAtomContainer.Builder.NewAtomContainer(varAtomContainer);
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
            if (CheckAromaticity)
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

            // neighbors[0] is the atom joined to the target proton:
            var neighbors = mol.GetConnectedAtoms(clonedAtom);
            IAtom neighbour0 = neighbors.First();

            // 2', 3', 4', 5', 6', and 7' atoms up to the target are detected:
            var atomsInSecondSphere = mol.GetConnectedAtoms(neighbour0);

            // SOME LISTS ARE CREATED FOR STORING OF INTERESTING ATOMS AND BONDS DURING DETECTION
            List<int> singles = new List<int>(); // list of any bond not rotatable
            List<int> doubles = new List<int>(); // list with only double bonds
            List<int> atoms = new List<int>(); // list with all the atoms in spheres
                                               // atoms.Add( int.ValueOf( mol.Atoms.IndexOf(neighbors[0]) ) );
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
            foreach (var curAtomSecond in atomsInSecondSphere)
            {
                secondBond = mol.GetBond(neighbour0, curAtomSecond);
                if (mol.Atoms.IndexOf(curAtomSecond) != atomPosition && GetIfBondIsNotRotatable(mol, secondBond, detected))
                {
                    sphere = 2;
                    bondOrder = secondBond.Order;
                    bondNumber = mol.Bonds.IndexOf(secondBond);
                    theBondIsInA6MemberedRing = false;
                    CheckAndStore(bondNumber, bondOrder, singles, doubles, bondsInCycloex, mol.Atoms.IndexOf(curAtomSecond), atoms, sphere, theBondIsInA6MemberedRing);
                    var atomsInThirdSphere = mol.GetConnectedAtoms(curAtomSecond);
                    foreach (var curAtomThird in atomsInThirdSphere)
                    {
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
                            foreach (var curAtomFourth in atomsInFourthSphere)
                            {
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
                                    foreach (var curAtomFifth in atomsInFifthSphere)
                                    {
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
                                            foreach (var curAtomSixth in atomsInSixthSphere)
                                            {
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
                                                    foreach (var curAtomSeventh in atomsInSeventhSphere)
                                                    {
                                                        seventhBond = mol.GetBond(curAtomSeventh, curAtomSixth);
                                                        if (mol.Atoms.IndexOf(curAtomSeventh) != atomPosition && GetIfBondIsNotRotatable(mol, seventhBond, detected))
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

            if (MakeDescriptorLastStage(
                rdfProtonCalculatedValues, 
                atom, 
                clonedAtom,
                mol,
                neighbour0, 
                singles,
                doubles,
                atoms, 
                bondsInCycloex))
            {
                return new DescriptorValue<ArrayResult<double>>(specification, ParameterNames, Parameters, rdfProtonCalculatedValues, DescriptorNames);
            }
            else
            {
                return GetDummyDescriptorValue(new CDKException("Some error occurred. Please report"));
            }
        }

        // Others definitions

        private static bool GetIfBondIsNotRotatable(IAtomContainer mol, IBond bond, IAtomContainer detected)
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
                    if (atom1.AtomicNumber.Equals(ChemicalElement.AtomicNumbers.H))
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

        private static bool GetIfACarbonIsDoubleBondedToAnOxygen(IAtomContainer mol, IAtom carbonAtom)
        {
            bool isDoubleBondedToOxygen = false;
            var neighToCarbon = mol.GetConnectedAtoms(carbonAtom);
            IBond tmpBond;
            int counter = 0;
            foreach (var neighbour in neighToCarbon)
            {
                if (neighbour.AtomicNumber.Equals(ChemicalElement.AtomicNumbers.O))
                {
                    tmpBond = mol.GetBond(neighbour, carbonAtom);
                    if (tmpBond.Order == BondOrder.Double) counter += 1;
                }
            }
            if (counter > 0) isDoubleBondedToOxygen = true;
            return isDoubleBondedToOxygen;
        }

        // this method calculates the angle between two bonds given coordinates of their atoms

        internal static double CalculateAngleBetweenTwoLines(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            Vector3 firstLine = a - b;
            Vector3 secondLine = c - d;
            Vector3 firstVec = firstLine;
            Vector3 secondVec = secondLine;
            return Vectors.Angle(firstVec, secondVec);
        }

        // this method store atoms and bonds in proper lists:
        private static void CheckAndStore(int bondToStore, BondOrder bondOrder, List<int> singleVec,
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
        internal static double CalculateDistanceBetweenTwoAtoms(IAtom atom1, IAtom atom2)
        {
            double distance;
            Vector3 firstPoint = atom1.Point3D.Value;
            Vector3 secondPoint = atom2.Point3D.Value;
            distance = Vector3.Distance(firstPoint, secondPoint);
            return distance;
        }

        // given a double bond
        // this method returns a bond bonded to this double bond
        internal static int GetNearestBondtoAGivenAtom(IAtomContainer mol, IAtom atom, IBond bond)
        {
            int nearestBond = -1;
            double distance = 0;
            IAtom atom0 = bond.Atoms[0];
            var bondsAtLeft = mol.GetConnectedBonds(atom0);
            foreach (var curBond in bondsAtLeft)
            {
                var values = CalculateDistanceBetweenAtomAndBond(atom, curBond);
                var partial = mol.Bonds.IndexOf(curBond);
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
        internal static double[] CalculateDistanceBetweenAtomAndBond(IAtom proton, IBond theBond)
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
        /// The parameterNames attribute of the RDFProtonDescriptor object
        /// </summary>
        public virtual IReadOnlyList<string> ParameterNames { get; } = new string[] { "checkAromaticity" };

        /// <summary>
        /// Gets the parameterType attribute of the RDFProtonDescriptor object
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public virtual object GetParameterType(string name)
        {
            if (string.Equals(name, "checkAromaticity", StringComparison.Ordinal)) return true;
            return null;
        }
    }
    public partial class RDFProtonDescriptorGHRTopology
    {
        private bool CheckAromaticity { get; set; } = false;
        private IAtomContainer acold = null;
        private IRingSet varRingSet = null;
        private IChemObjectSet<IAtomContainer> varAtomContainerSet = null;

        /// <summary>
        /// The parameters attribute of the RDFProtonDescriptor object
        /// </summary>
        /// <value>Parameters are the proton position and a boolean (<see langword="true"/> if you need to detect aromaticity)</value>
        /// <exception cref="CDKException"></exception>
        public virtual IReadOnlyList<object> Parameters
        {
            set
            {
                if (value.Count > 1)
                {
                    throw new CDKException($"{nameof(RDFProtonDescriptorGHRTopology)} only expects one parameters");
                }
                if (!(value[0] is bool))
                {
                    throw new CDKException($"The second parameter must be of type {nameof(System.Boolean)}");
                }
                CheckAromaticity = (bool)value[0];
            }
            get
            {
                // return the parameters as used for the descriptor calculation
                return new object[] { CheckAromaticity };
            }
        }

        private DescriptorValue<ArrayResult<double>> GetDummyDescriptorValue(Exception e)
        {
            ArrayResult<double> result = new ArrayResult<double>(desc_length);
            for (int i = 0; i < desc_length; i++)
                result.Add(double.NaN);
            return new DescriptorValue<ArrayResult<double>>(specification, ParameterNames, Parameters, result, DescriptorNames, e);
        }

        public virtual IDescriptorValue Calculate(IAtom atom, IAtomContainer varAtomContainerSet)
        {
            return (Calculate(atom, varAtomContainerSet, null));
        }

        public DescriptorValue<ArrayResult<double>> Calculate(IAtom atom, IAtomContainer atomContainer, IRingSet precalculatedringset)
        {
            IAtomContainer varAtomContainer = (IAtomContainer)atomContainer.Clone();

            int atomPosition = atomContainer.Atoms.IndexOf(atom);
            IAtom clonedAtom = varAtomContainer.Atoms[atomPosition];
            ArrayResult<double> rdfProtonCalculatedValues = new ArrayResult<double>(desc_length);
            if (!atom.AtomicNumber.Equals(ChemicalElement.AtomicNumbers.H))
            {
                return GetDummyDescriptorValue(new CDKException("Invalid atom specified"));
            }

            // ///////////////////////FIRST SECTION OF MAIN METHOD: DEFINITION OF MAIN VARIABLES
            // ///////////////////////AND AROMATICITY AND PI-SYSTEM AND RINGS DETECTION

            IAtomContainer mol = varAtomContainer.Builder.NewAtomContainer(varAtomContainer);
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
            if (CheckAromaticity)
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

            // neighbors[0] is the atom joined to the target proton:
            var neighbors = mol.GetConnectedAtoms(clonedAtom);
            IAtom neighbour0 = neighbors.First();

            // 2', 3', 4', 5', 6', and 7' atoms up to the target are detected:
            var atomsInSecondSphere = mol.GetConnectedAtoms(neighbour0);

            // SOME LISTS ARE CREATED FOR STORING OF INTERESTING ATOMS AND BONDS DURING DETECTION
            List<int> singles = new List<int>(); // list of any bond not rotatable
            List<int> doubles = new List<int>(); // list with only double bonds
            List<int> atoms = new List<int>(); // list with all the atoms in spheres
                                               // atoms.Add( int.ValueOf( mol.Atoms.IndexOf(neighbors[0]) ) );
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
            foreach (var curAtomSecond in atomsInSecondSphere)
            {
                secondBond = mol.GetBond(neighbour0, curAtomSecond);
                if (mol.Atoms.IndexOf(curAtomSecond) != atomPosition && GetIfBondIsNotRotatable(mol, secondBond, detected))
                {
                    sphere = 2;
                    bondOrder = secondBond.Order;
                    bondNumber = mol.Bonds.IndexOf(secondBond);
                    theBondIsInA6MemberedRing = false;
                    CheckAndStore(bondNumber, bondOrder, singles, doubles, bondsInCycloex, mol.Atoms.IndexOf(curAtomSecond), atoms, sphere, theBondIsInA6MemberedRing);
                    var atomsInThirdSphere = mol.GetConnectedAtoms(curAtomSecond);
                    foreach (var curAtomThird in atomsInThirdSphere)
                    {
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
                            foreach (var curAtomFourth in atomsInFourthSphere)
                            {
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
                                    foreach (var curAtomFifth in atomsInFifthSphere)
                                    {
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
                                            foreach (var curAtomSixth in atomsInSixthSphere)
                                            {
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
                                                    foreach (var curAtomSeventh in atomsInSeventhSphere)
                                                    {
                                                        seventhBond = mol.GetBond(curAtomSeventh, curAtomSixth);
                                                        if (mol.Atoms.IndexOf(curAtomSeventh) != atomPosition && GetIfBondIsNotRotatable(mol, seventhBond, detected))
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

            if (MakeDescriptorLastStage(
                rdfProtonCalculatedValues, 
                atom, 
                clonedAtom,
                mol,
                neighbour0, 
                singles,
                doubles,
                atoms, 
                bondsInCycloex))
            {
                return new DescriptorValue<ArrayResult<double>>(specification, ParameterNames, Parameters, rdfProtonCalculatedValues, DescriptorNames);
            }
            else
            {
                return GetDummyDescriptorValue(new CDKException("Some error occurred. Please report"));
            }
        }

        // Others definitions

        private static bool GetIfBondIsNotRotatable(IAtomContainer mol, IBond bond, IAtomContainer detected)
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
                    if (atom1.AtomicNumber.Equals(ChemicalElement.AtomicNumbers.H))
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

        private static bool GetIfACarbonIsDoubleBondedToAnOxygen(IAtomContainer mol, IAtom carbonAtom)
        {
            bool isDoubleBondedToOxygen = false;
            var neighToCarbon = mol.GetConnectedAtoms(carbonAtom);
            IBond tmpBond;
            int counter = 0;
            foreach (var neighbour in neighToCarbon)
            {
                if (neighbour.AtomicNumber.Equals(ChemicalElement.AtomicNumbers.O))
                {
                    tmpBond = mol.GetBond(neighbour, carbonAtom);
                    if (tmpBond.Order == BondOrder.Double) counter += 1;
                }
            }
            if (counter > 0) isDoubleBondedToOxygen = true;
            return isDoubleBondedToOxygen;
        }

        // this method calculates the angle between two bonds given coordinates of their atoms

        internal static double CalculateAngleBetweenTwoLines(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            Vector3 firstLine = a - b;
            Vector3 secondLine = c - d;
            Vector3 firstVec = firstLine;
            Vector3 secondVec = secondLine;
            return Vectors.Angle(firstVec, secondVec);
        }

        // this method store atoms and bonds in proper lists:
        private static void CheckAndStore(int bondToStore, BondOrder bondOrder, List<int> singleVec,
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
        internal static double CalculateDistanceBetweenTwoAtoms(IAtom atom1, IAtom atom2)
        {
            double distance;
            Vector3 firstPoint = atom1.Point3D.Value;
            Vector3 secondPoint = atom2.Point3D.Value;
            distance = Vector3.Distance(firstPoint, secondPoint);
            return distance;
        }

        // given a double bond
        // this method returns a bond bonded to this double bond
        internal static int GetNearestBondtoAGivenAtom(IAtomContainer mol, IAtom atom, IBond bond)
        {
            int nearestBond = -1;
            double distance = 0;
            IAtom atom0 = bond.Atoms[0];
            var bondsAtLeft = mol.GetConnectedBonds(atom0);
            foreach (var curBond in bondsAtLeft)
            {
                var values = CalculateDistanceBetweenAtomAndBond(atom, curBond);
                var partial = mol.Bonds.IndexOf(curBond);
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
        internal static double[] CalculateDistanceBetweenAtomAndBond(IAtom proton, IBond theBond)
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
        /// The parameterNames attribute of the RDFProtonDescriptor object
        /// </summary>
        public virtual IReadOnlyList<string> ParameterNames { get; } = new string[] { "checkAromaticity" };

        /// <summary>
        /// Gets the parameterType attribute of the RDFProtonDescriptor object
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public virtual object GetParameterType(string name)
        {
            if (string.Equals(name, "checkAromaticity", StringComparison.Ordinal)) return true;
            return null;
        }
    }
    public partial class RDFProtonDescriptorGSR
    {
        private bool CheckAromaticity { get; set; } = false;
        private IAtomContainer acold = null;
        private IRingSet varRingSet = null;
        private IChemObjectSet<IAtomContainer> varAtomContainerSet = null;

        /// <summary>
        /// The parameters attribute of the RDFProtonDescriptor object
        /// </summary>
        /// <value>Parameters are the proton position and a boolean (<see langword="true"/> if you need to detect aromaticity)</value>
        /// <exception cref="CDKException"></exception>
        public virtual IReadOnlyList<object> Parameters
        {
            set
            {
                if (value.Count > 1)
                {
                    throw new CDKException($"{nameof(RDFProtonDescriptorGSR)} only expects one parameters");
                }
                if (!(value[0] is bool))
                {
                    throw new CDKException($"The second parameter must be of type {nameof(System.Boolean)}");
                }
                CheckAromaticity = (bool)value[0];
            }
            get
            {
                // return the parameters as used for the descriptor calculation
                return new object[] { CheckAromaticity };
            }
        }

        private DescriptorValue<ArrayResult<double>> GetDummyDescriptorValue(Exception e)
        {
            ArrayResult<double> result = new ArrayResult<double>(desc_length);
            for (int i = 0; i < desc_length; i++)
                result.Add(double.NaN);
            return new DescriptorValue<ArrayResult<double>>(specification, ParameterNames, Parameters, result, DescriptorNames, e);
        }

        public virtual IDescriptorValue Calculate(IAtom atom, IAtomContainer varAtomContainerSet)
        {
            return (Calculate(atom, varAtomContainerSet, null));
        }

        public DescriptorValue<ArrayResult<double>> Calculate(IAtom atom, IAtomContainer atomContainer, IRingSet precalculatedringset)
        {
            IAtomContainer varAtomContainer = (IAtomContainer)atomContainer.Clone();

            int atomPosition = atomContainer.Atoms.IndexOf(atom);
            IAtom clonedAtom = varAtomContainer.Atoms[atomPosition];
            ArrayResult<double> rdfProtonCalculatedValues = new ArrayResult<double>(desc_length);
            if (!atom.AtomicNumber.Equals(ChemicalElement.AtomicNumbers.H))
            {
                return GetDummyDescriptorValue(new CDKException("Invalid atom specified"));
            }

            // ///////////////////////FIRST SECTION OF MAIN METHOD: DEFINITION OF MAIN VARIABLES
            // ///////////////////////AND AROMATICITY AND PI-SYSTEM AND RINGS DETECTION

            IAtomContainer mol = varAtomContainer.Builder.NewAtomContainer(varAtomContainer);
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
            if (CheckAromaticity)
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

            // neighbors[0] is the atom joined to the target proton:
            var neighbors = mol.GetConnectedAtoms(clonedAtom);
            IAtom neighbour0 = neighbors.First();

            // 2', 3', 4', 5', 6', and 7' atoms up to the target are detected:
            var atomsInSecondSphere = mol.GetConnectedAtoms(neighbour0);

            // SOME LISTS ARE CREATED FOR STORING OF INTERESTING ATOMS AND BONDS DURING DETECTION
            List<int> singles = new List<int>(); // list of any bond not rotatable
            List<int> doubles = new List<int>(); // list with only double bonds
            List<int> atoms = new List<int>(); // list with all the atoms in spheres
                                               // atoms.Add( int.ValueOf( mol.Atoms.IndexOf(neighbors[0]) ) );
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
            foreach (var curAtomSecond in atomsInSecondSphere)
            {
                secondBond = mol.GetBond(neighbour0, curAtomSecond);
                if (mol.Atoms.IndexOf(curAtomSecond) != atomPosition && GetIfBondIsNotRotatable(mol, secondBond, detected))
                {
                    sphere = 2;
                    bondOrder = secondBond.Order;
                    bondNumber = mol.Bonds.IndexOf(secondBond);
                    theBondIsInA6MemberedRing = false;
                    CheckAndStore(bondNumber, bondOrder, singles, doubles, bondsInCycloex, mol.Atoms.IndexOf(curAtomSecond), atoms, sphere, theBondIsInA6MemberedRing);
                    var atomsInThirdSphere = mol.GetConnectedAtoms(curAtomSecond);
                    foreach (var curAtomThird in atomsInThirdSphere)
                    {
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
                            foreach (var curAtomFourth in atomsInFourthSphere)
                            {
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
                                    foreach (var curAtomFifth in atomsInFifthSphere)
                                    {
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
                                            foreach (var curAtomSixth in atomsInSixthSphere)
                                            {
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
                                                    foreach (var curAtomSeventh in atomsInSeventhSphere)
                                                    {
                                                        seventhBond = mol.GetBond(curAtomSeventh, curAtomSixth);
                                                        if (mol.Atoms.IndexOf(curAtomSeventh) != atomPosition && GetIfBondIsNotRotatable(mol, seventhBond, detected))
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

            if (MakeDescriptorLastStage(
                rdfProtonCalculatedValues, 
                atom, 
                clonedAtom,
                mol,
                neighbour0, 
                singles,
                doubles,
                atoms, 
                bondsInCycloex))
            {
                return new DescriptorValue<ArrayResult<double>>(specification, ParameterNames, Parameters, rdfProtonCalculatedValues, DescriptorNames);
            }
            else
            {
                return GetDummyDescriptorValue(new CDKException("Some error occurred. Please report"));
            }
        }

        // Others definitions

        private static bool GetIfBondIsNotRotatable(IAtomContainer mol, IBond bond, IAtomContainer detected)
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
                    if (atom1.AtomicNumber.Equals(ChemicalElement.AtomicNumbers.H))
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

        private static bool GetIfACarbonIsDoubleBondedToAnOxygen(IAtomContainer mol, IAtom carbonAtom)
        {
            bool isDoubleBondedToOxygen = false;
            var neighToCarbon = mol.GetConnectedAtoms(carbonAtom);
            IBond tmpBond;
            int counter = 0;
            foreach (var neighbour in neighToCarbon)
            {
                if (neighbour.AtomicNumber.Equals(ChemicalElement.AtomicNumbers.O))
                {
                    tmpBond = mol.GetBond(neighbour, carbonAtom);
                    if (tmpBond.Order == BondOrder.Double) counter += 1;
                }
            }
            if (counter > 0) isDoubleBondedToOxygen = true;
            return isDoubleBondedToOxygen;
        }

        // this method calculates the angle between two bonds given coordinates of their atoms

        internal static double CalculateAngleBetweenTwoLines(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            Vector3 firstLine = a - b;
            Vector3 secondLine = c - d;
            Vector3 firstVec = firstLine;
            Vector3 secondVec = secondLine;
            return Vectors.Angle(firstVec, secondVec);
        }

        // this method store atoms and bonds in proper lists:
        private static void CheckAndStore(int bondToStore, BondOrder bondOrder, List<int> singleVec,
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
        internal static double CalculateDistanceBetweenTwoAtoms(IAtom atom1, IAtom atom2)
        {
            double distance;
            Vector3 firstPoint = atom1.Point3D.Value;
            Vector3 secondPoint = atom2.Point3D.Value;
            distance = Vector3.Distance(firstPoint, secondPoint);
            return distance;
        }

        // given a double bond
        // this method returns a bond bonded to this double bond
        internal static int GetNearestBondtoAGivenAtom(IAtomContainer mol, IAtom atom, IBond bond)
        {
            int nearestBond = -1;
            double distance = 0;
            IAtom atom0 = bond.Atoms[0];
            var bondsAtLeft = mol.GetConnectedBonds(atom0);
            foreach (var curBond in bondsAtLeft)
            {
                var values = CalculateDistanceBetweenAtomAndBond(atom, curBond);
                var partial = mol.Bonds.IndexOf(curBond);
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
        internal static double[] CalculateDistanceBetweenAtomAndBond(IAtom proton, IBond theBond)
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
        /// The parameterNames attribute of the RDFProtonDescriptor object
        /// </summary>
        public virtual IReadOnlyList<string> ParameterNames { get; } = new string[] { "checkAromaticity" };

        /// <summary>
        /// Gets the parameterType attribute of the RDFProtonDescriptor object
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public virtual object GetParameterType(string name)
        {
            if (string.Equals(name, "checkAromaticity", StringComparison.Ordinal)) return true;
            return null;
        }
    }
}
