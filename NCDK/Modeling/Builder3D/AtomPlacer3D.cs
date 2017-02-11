/* Copyright (C) 2005-2007  Christian Hoppe <chhoppe@users.sf.net>
 *                    2014  Mark B Vine (orcid:0000-0002-7794-0426)
 *
 *  Contact: cdk-devel@list.sourceforge.net
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
using NCDK.Common.Mathematics;
using NCDK.Default;
using NCDK.Geometries;
using System;
using System.Collections.Generic;
using NCDK.Numerics;
using System.Linq;

namespace NCDK.Modeling.Builder3D
{
    /**
     * Place aliphatic <b>chains</b> with Z matrix method. Please use {@link
     * ModelBuilder3D} to place general molecules.
     *
     * @author         chhoppe
     * @cdk.keyword    AtomPlacer3D
     * @cdk.created    2004-10-8
     * @cdk.module     builder3d
     * @cdk.githash
     * @see ModelBuilder3D
     */
    public class AtomPlacer3D
    {
        private IDictionary<string, object> pSet = null;
        private double[] distances;
        private int[] firstAtoms = null;
        private double[] angles = null;
        private int[] secondAtoms = null;
        private double[] dihedrals = null;
        private int[] thirdAtoms = null;
        private const double DIHEDRAL_EXTENDED_CHAIN = (180.0 / 180) * Math.PI;
        private const double DIHEDRAL_BRANCHED_CHAIN = 0.0;
        private const double DEFAULT_BOND_LENGTH = 1.5;
        private const double DEFAULT_SP3_ANGLE = 109.471;
        private const double DEFAULT_SP2_ANGLE = 120.000;
        private const double DEFAULT_SP_ANGLE = 180.000;

#if TEST
        public
#endif
        AtomPlacer3D() { }

        /**
         *  Initialize the atomPlacer class.
         *
         * @param  parameterSet  Force Field parameter as Dictionary
         */
        public void Initilize(IDictionary<string, object> parameterSet)
        {
            pSet = parameterSet;
        }

        /**
         *  Count and find first heavy Atom(s) (non Hydrogens) in a chain.
         *
         * @param  molecule the reference molecule for searching the chain
         * @param  chain  chain to be searched
         * @return        the atom number of the first heavy atom the number of heavy atoms in the chain
         */
        public int[] FindHeavyAtomsInChain(IAtomContainer molecule, IAtomContainer chain)
        {
            int[] heavy = { -1, -1 };
            int hc = 0;
            for (int i = 0; i < chain.Atoms.Count; i++)
            {
                if (IsHeavyAtom(chain.Atoms[i]))
                {
                    if (heavy[0] < 0)
                    {
                        heavy[0] = molecule.Atoms.IndexOf(chain.Atoms[i]);
                    }
                    hc++;
                }
            }
            heavy[1] = hc;
            return heavy;
        }

        /**
         *  Mark all atoms in chain as placed. (CDKConstant ISPLACED)
         *
         * @param  ac  chain
         * @return     chain all atoms marked as placed
         */
        public IAtomContainer MarkPlaced(IAtomContainer ac)
        {
            for (int i = 0; i < ac.Atoms.Count; i++)
            {
                ac.Atoms[i].IsPlaced = true;
            }
            return ac;
        }

        /**
         *  Method assigns 3D coordinates to the heavy atoms in an aliphatic chain.
         *
         * @param molecule        the reference molecule for the chain
         * @param  chain          the atoms to be assigned, must be connected
         * @throws CDKException the 'chain' was not a chain
         */
        public void PlaceAliphaticHeavyChain(IAtomContainer molecule, IAtomContainer chain)
        {
            //Debug.WriteLine("******** Place aliphatic Chain *********");
            int[] first = new int[2];
            int counter = 1;
            int nextAtomNr = 0;
            string id1 = "";
            string id2 = "";
            string id3 = "";
            first = FindHeavyAtomsInChain(molecule, chain);
            distances = new double[first[1]];
            firstAtoms = new int[first[1]];
            angles = new double[first[1]];
            secondAtoms = new int[first[1]];
            dihedrals = new double[first[1]];
            thirdAtoms = new int[first[1]];
            firstAtoms[0] = first[0];
            molecule.Atoms[firstAtoms[0]].IsVisited = true;
            int hybridisation = 0;
            for (int i = 0; i < chain.Atoms.Count; i++)
            {
                if (IsHeavyAtom(chain.Atoms[i]))
                {
                    if (!chain.Atoms[i].IsVisited)
                    {
                        //Debug.WriteLine("Counter:" + counter);
                        nextAtomNr = molecule.Atoms.IndexOf(chain.Atoms[i]);
                        id2 = molecule.Atoms[firstAtoms[counter - 1]].AtomTypeName;
                        id1 = molecule.Atoms[nextAtomNr].AtomTypeName;

                        if (molecule.GetBond(molecule.Atoms[firstAtoms[counter - 1]], molecule.Atoms[nextAtomNr]) == null)
                            throw new CDKException("atoms do not form a chain, please use ModelBuilder3D");

                        distances[counter] = GetBondLengthValue(id1, id2);
                        //Debug.WriteLine(" Distance:" + distances[counter]);
                        firstAtoms[counter] = nextAtomNr;
                        secondAtoms[counter] = firstAtoms[counter - 1];
                        if (counter > 1)
                        {
                            id3 = molecule.Atoms[firstAtoms[counter - 2]].AtomTypeName;
                            hybridisation = GetHybridisationState(molecule.Atoms[firstAtoms[counter - 1]]);
                            angles[counter] = GetAngleValue(id1, id2, id3);
                            //Check if sp,sp2
                            if (angles[counter] == -1)
                            {
                                if (hybridisation == 3)
                                {
                                    angles[counter] = DEFAULT_SP3_ANGLE;
                                }
                                else if (hybridisation == 2)
                                {
                                    angles[counter] = DEFAULT_SP2_ANGLE;
                                }
                                else if (hybridisation == 1)
                                {
                                    angles[counter] = DEFAULT_SP_ANGLE;
                                }
                            }
                            thirdAtoms[counter] = firstAtoms[counter - 2];
                            //Debug.WriteLine(" Angle:" + angles[counter]);
                        }
                        else
                        {
                            angles[counter] = -1;
                            thirdAtoms[counter] = -1;
                        }
                        if (counter > 2)
                        {
                            //double bond
                            try
                            {
                                if (GetDoubleBondConfiguration2D(
                                        molecule.GetBond(molecule.Atoms[firstAtoms[counter - 1]],
                                                molecule.Atoms[firstAtoms[counter - 2]]),
                                        (molecule.Atoms[firstAtoms[counter]]).Point2D,
                                        (molecule.Atoms[firstAtoms[counter - 1]]).Point2D,
                                        (molecule.Atoms[firstAtoms[counter - 2]]).Point2D,
                                        (molecule.Atoms[firstAtoms[counter - 3]]).Point2D) == 5)
                                {
                                    dihedrals[counter] = DIHEDRAL_BRANCHED_CHAIN;
                                }
                                else
                                {
                                    dihedrals[counter] = DIHEDRAL_EXTENDED_CHAIN;
                                }
                            }
                            catch (CDKException)
                            {
                                dihedrals[counter] = DIHEDRAL_EXTENDED_CHAIN;
                            }
                        }
                        else
                        {
                            dihedrals[counter] = -1;
                        }
                        counter++;
                    }
                }
            }
        }

        /**
         * Takes the given Z Matrix coordinates and converts them to cartesian coordinates.
         * The first Atom end up in the origin, the second on on the x axis, and the third
         * one in the XY plane. The rest is added by applying the Zmatrix distances, angles
         * and dihedrals. Assign coordinates directly to the atoms.
         *
         * @param  molecule  the molecule to be placed in 3D
         * @param  flagBranched  marks branched chain
         * author: egonw,cho
         */

        public void ZMatrixChainToCartesian(IAtomContainer molecule, bool flagBranched)
        {
            Vector3? result = null;
            for (int index = 0; index < distances.Length; index++)
            {
                if (index == 0)
                {
                    result = new Vector3(0d, 0d, 0d);
                }
                else if (index == 1)
                {
                    result = new Vector3(distances[1], 0d, 0d);
                }
                else if (index == 2)
                {
                    result = new Vector3(-Math.Cos((angles[2] / 180) * Math.PI) * distances[2] + distances[1],
                            Math.Sin((angles[2] / 180) * Math.PI) * distances[2], 0);
                }
                else
                {
                    var cd = molecule.Atoms[thirdAtoms[index]].Point3D.Value
                           - molecule.Atoms[secondAtoms[index]].Point3D.Value;
                    var bc = molecule.Atoms[secondAtoms[index]].Point3D.Value
                           - molecule.Atoms[firstAtoms[index - 3]].Point3D.Value;
                    var n1 = Vector3.Cross(cd, bc);
                    n1 = Vector3.Normalize(n1);

                    Vector3 n2;
                    if (index == 3 && flagBranched)
                    {
                        n2 = AtomTetrahedralLigandPlacer3D.Rotate(n1, bc, DIHEDRAL_BRANCHED_CHAIN);
                    }
                    else
                    {
                        n2 = AtomTetrahedralLigandPlacer3D.Rotate(n1, bc, dihedrals[index]);
                    }
                    n2 = Vector3.Normalize(n2);

                    Vector3 ba = new Vector3();
                    if (index == 3 && flagBranched)
                    {
                        ba = AtomTetrahedralLigandPlacer3D.Rotate(cd, n2, (-angles[index] / 180) * Math.PI);
                        ba = AtomTetrahedralLigandPlacer3D.Rotate(ba, cd, (-angles[index] / 180) * Math.PI);
                    }
                    else
                    {
                        ba = AtomTetrahedralLigandPlacer3D.Rotate(cd, n2, (-angles[index] / 180) * Math.PI);
                    }

                    ba = Vector3.Normalize(ba);

                    Vector3 ban = ba;
                    ban *= distances[index];

                    result = molecule.Atoms[firstAtoms[index - 1]].Point3D.Value + ban;
                }
                IAtom atom = molecule.Atoms[firstAtoms[index]];
                if ((atom.Point3D == null || !atom.IsPlaced)
                        && !atom.IsInRing && IsHeavyAtom(atom))
                {
                    atom.Point3D = result;
                    atom.IsPlaced = true;
                }
            }
        }

        /**
         *  Gets the hybridisation state of an atom.
         *
         *@param  atom1  atom
         *@return        The hybridisationState value (sp=1;sp2=2;sp3=3)
         */
        private int GetHybridisationState(IAtom atom1)
        {

            BondOrder maxBondOrder = atom1.MaxBondOrder;

            //        if (atom1.FormalNeighbourCount == 1 || maxBondOrder > 4) {
            if (atom1.FormalNeighbourCount == 1)
            {
                // WTF??
            }
            else if (atom1.FormalNeighbourCount == 2 || maxBondOrder == BondOrder.Triple)
            {
                return 1; //sp
            }
            else if (atom1.FormalNeighbourCount == 3 || (maxBondOrder == BondOrder.Double))
            {
                return 2; //sp2
            }
            else
            {
                return 3; //sp3
            }
            return -1;
        }

        /**
         *  Gets the doubleBondConfiguration2D attribute of the AtomPlacer3D object
         *  using existing 2D coordinates.
         *
         *@param  bond           the double bond
         *@param  a              coordinates (Vector2) of atom1 connected to bond
         *@param  b              coordinates (Vector2) of atom2 connected to bond
         *@param  c              coordinates (Vector2) of atom3 connected to bond
         *@param  d              coordinates (Vector2) of atom4 connected to bond
         *@return                The doubleBondConfiguration2D value
         */
        private int GetDoubleBondConfiguration2D(IBond bond, Vector2? aa, Vector2? bb, Vector2? cc, Vector2? dd)
        {
            if (bond.Order != BondOrder.Double)
            {
                return 0;
            }
            // no 2D coordinates or existing configuration
            if (aa == null || bb == null || cc == null || dd == null) return 0;
            var a = aa.Value;
            var b = bb.Value;
            var c = cc.Value;
            var d = dd.Value;
            Vector2 cb = new Vector2(c.X - b.X, c.Y - b.Y);
            Vector2 xT = new Vector2(cb.X - 1, cb.Y);
            a.Y = a.Y - b.Y - xT.Y;
            d.Y = d.Y - b.Y - xT.Y;
            if ((a.Y > 0 && d.Y > 0) || (a.Y < 0 && d.Y < 0))
            {
                return 5;
            }
            else
            {
                return 6;
            }
        }

        /**
         *  Gets the distanceValue attribute of the parameter set.
         *
         * @param  id1            atom1 id
         * @param  id2            atom2 id
         * @return                The distanceValue value from the force field parameter set
         */
        public double GetBondLengthValue(string id1, string id2)
        {
            string dkey = "";
            if (pSet.ContainsKey(("bond" + id1 + ";" + id2)))
            {
                dkey = "bond" + id1 + ";" + id2;
            }
            else if (pSet.ContainsKey(("bond" + id2 + ";" + id1)))
            {
                dkey = "bond" + id2 + ";" + id1;
            }
            else
            {
                Console.Out.WriteLine("KEYError: Unknown distance key in pSet: " + id2 + ";" + id1
                        + " take default bond length: " + DEFAULT_BOND_LENGTH);
                return DEFAULT_BOND_LENGTH;
            }
            return ((IList<double>)pSet[dkey])[0];
        }

        /**
         *  Gets the angleKey attribute of the AtomPlacer3D object.
         *
         * @param  id1            Description of the Parameter
         * @param  id2            Description of the Parameter
         * @param  id3            Description of the Parameter
         * @return                The angleKey value
         */
        public double GetAngleValue(string id1, string id2, string id3)
        {
            string akey = "";
            if (pSet.ContainsKey(("angle" + id1 + ";" + id2 + ";" + id3)))
            {
                akey = "angle" + id1 + ";" + id2 + ";" + id3;
            }
            else if (pSet.ContainsKey(("angle" + id3 + ";" + id2 + ";" + id1)))
            {
                akey = "angle" + id3 + ";" + id2 + ";" + id1;
            }
            else if (pSet.ContainsKey(("angle" + id2 + ";" + id1 + ";" + id3)))
            {
                akey = "angle" + id2 + ";" + id1 + ";" + id3;
            }
            else if (pSet.ContainsKey(("angle" + id1 + ";" + id3 + ";" + id2)))
            {
                akey = "angle" + id1 + ";" + id3 + ";" + id2;
            }
            else if (pSet.ContainsKey(("angle" + id3 + ";" + id1 + ";" + id2)))
            {
                akey = "angle" + id3 + ";" + id1 + ";" + id2;
            }
            else if (pSet.ContainsKey(("angle" + id2 + ";" + id3 + ";" + id1)))
            {
                akey = "angle" + id2 + ";" + id3 + ";" + id1;
            }
            else
            {
                //Debug.WriteLine("KEYErrorAngle:Unknown angle key in pSet: " +id2 + " ; " + id3 + " ; " + id1+" take default angle:"+DEFAULT_ANGLE);
                return -1;
            }
            return ((IList<double>)pSet[akey])[0];
        }

        /**
         *  Gets the nextUnplacedHeavyAtomWithAliphaticPlacedNeighbour from an atom container or molecule.
         *
         * @param molecule
         * @return    The nextUnplacedHeavyAtomWithAliphaticPlacedNeighbour value
         * author:    steinbeck,cho
         */
        public IAtom GetNextUnplacedHeavyAtomWithAliphaticPlacedNeighbour(IAtomContainer molecule)
        {
            foreach (var bond in molecule.Bonds)
            {
                if (bond.Atoms[0].IsPlaced && !(bond.Atoms[1].IsPlaced))
                {
                    if (IsAliphaticHeavyAtom(bond.Atoms[1]))
                    {
                        return bond.Atoms[1];
                    }
                }
                if (bond.Atoms[1].IsPlaced && !(bond.Atoms[0].IsPlaced))
                {
                    if (IsAliphaticHeavyAtom(bond.Atoms[0]))
                    {
                        return bond.Atoms[0];
                    }
                }
            }
            return null;
        }

        /**
         * Find the first unplaced atom.
         * 
         * @param molecule molecule being built
         * @return an unplaced heavy atom, null if none.
         */
        internal IAtom GetUnplacedHeavyAtom(IAtomContainer molecule)
        {
            foreach (var atom in molecule.Atoms)
            {
                if (IsUnplacedHeavyAtom(atom))
                    return atom;
            }
            return null;
        }

        /**
         *  Gets the nextPlacedHeavyAtomWithAliphaticPlacedNeigbor from an atom container or molecule.
         *
         * @param molecule
         * @return    The nextUnplacedHeavyAtomWithUnplacedAliphaticNeigbor
         * author: steinbeck,cho
         */
        public IAtom GetNextPlacedHeavyAtomWithUnplacedAliphaticNeighbour(IAtomContainer molecule)
        {
            foreach (var bond in molecule.Bonds)
            {
                IAtom atom0 = bond.Atoms[0];
                IAtom atom1 = bond.Atoms[1];
                if (atom0.IsPlaced && !(atom1.IsPlaced))
                {
                    if (IsAliphaticHeavyAtom(atom1) && IsHeavyAtom(atom0))
                    {
                        return atom0;
                    }
                }
                if (atom1.IsPlaced && !(atom0.IsPlaced))
                {
                    if (IsAliphaticHeavyAtom(atom0) && IsHeavyAtom(atom1))
                    {
                        return atom1;
                    }
                }
            }
            return null;
        }

        /**
         *  Gets the nextPlacedHeavyAtomWithUnplacedRingNeighbour attribute of the AtomPlacer3D object.
         *
         * @param molecule  The atom container under consideration
         * @return          The nextPlacedHeavyAtomWithUnplacedRingNeighbour value
         */
        public IAtom GetNextPlacedHeavyAtomWithUnplacedRingNeighbour(IAtomContainer molecule)
        {
            foreach (var bond in molecule.Bonds)
            {
                IAtom atom0 = bond.Atoms[0];
                IAtom atom1 = bond.Atoms[1];
                if (atom0.IsPlaced && !(atom1.IsPlaced))
                {
                    if (IsRingHeavyAtom(atom1) && IsHeavyAtom(atom0))
                    {
                        return atom0;
                    }
                }
                if (atom1.IsPlaced && !(atom0.IsPlaced))
                {
                    if (IsRingHeavyAtom(atom0) && IsHeavyAtom(atom1))
                    {
                        return atom1;
                    }
                }
            }
            return null;
        }

        /**
         *  Gets the farthestAtom attribute of the AtomPlacer3D object.
         *
         * @param  refAtomPoint  Description of the Parameter
         * @param  ac            Description of the Parameter
         * @return               The farthestAtom value
         */
        public IAtom GetFarthestAtom(Vector3 refAtomPoint, IAtomContainer ac)
        {
            double distance = 0;
            IAtom atom = null;
            for (int i = 0; i < ac.Atoms.Count; i++)
            {
                if (ac.Atoms[i].Point3D != null)
                {
                    if (Math.Abs(Vector3.Distance(refAtomPoint, ac.Atoms[i].Point3D.Value)) > distance)
                    {
                        atom = ac.Atoms[i];
                        distance = Math.Abs(Vector3.Distance(refAtomPoint, ac.Atoms[i].Point3D.Value));
                    }
                }
            }
            return atom;
        }

        /**
         *  Gets the unplacedRingHeavyAtom attribute of the AtomPlacer3D object.
         *
         * @param molecule
         * @param  atom  Description of the Parameter
         * @return       The unplacedRingHeavyAtom value
         */
        public IAtom GetUnplacedRingHeavyAtom(IAtomContainer molecule, IAtom atom)
        {
            var bonds = molecule.GetConnectedBonds(atom);
            IAtom connectedAtom = null;
            foreach (var bond in bonds)
            {
                connectedAtom = bond.GetConnectedAtom(atom);
                if (IsUnplacedHeavyAtom(connectedAtom) && connectedAtom.IsInRing)
                {
                    return connectedAtom;
                }
            }
            return connectedAtom;
        }

        /**
         *  Calculates the geometric center of all placed atoms in the atomcontainer.
         *
         * @param molecule
         * @return    Vector3 the geometric center
         */
        public Vector3 GeometricCenterAllPlacedAtoms(IAtomContainer molecule)
        {
            IAtomContainer allPlacedAtoms = GetAllPlacedAtoms(molecule);
            return GeometryUtil.Get3DCenter(allPlacedAtoms);
        }

        /**
         *  Returns a placed atom connected to a given atom.
         *
         * @param molecule
         * @param  atom  The Atom whose placed bonding partners are to be returned
         * @return       a placed heavy atom connected to a given atom
         * author:      steinbeck
         */
        public IAtom GetPlacedHeavyAtom(IAtomContainer molecule, IAtom atom)
        {
            var bonds = molecule.GetConnectedBonds(atom);
            foreach (var bond in bonds)
            {
                IAtom connectedAtom = bond.GetConnectedAtom(atom);
                if (IsPlacedHeavyAtom(connectedAtom))
                {
                    return connectedAtom;
                }
            }
            return null;
        }

        /**
         *  Gets the first placed Heavy Atom around atomA which is not atomB.
         *
         * @param molecule
         * @param  atomA  Description of the Parameter
         * @param  atomB  Description of the Parameter
         * @return        The placedHeavyAtom value
         */
        public IAtom GetPlacedHeavyAtom(IAtomContainer molecule, IAtom atomA, IAtom atomB)
        {
            var bonds = molecule.GetConnectedBonds(atomA);
            foreach (var bond in bonds)
            {
                IAtom connectedAtom = bond.GetConnectedAtom(atomA);
                if (IsPlacedHeavyAtom(connectedAtom) && connectedAtom != atomB)
                {
                    return connectedAtom;
                }
            }
            return null;
        }

        /**
         *  Gets the placed Heavy Atoms connected to an atom.
         *
         * @param molecule
         * @param  atom  The atom the atoms must be connected to.
         * @return       The placed heavy atoms.
         */
        public IAtomContainer GetPlacedHeavyAtoms(IAtomContainer molecule, IAtom atom)
        {

            var bonds = molecule.GetConnectedBonds(atom);
            IAtomContainer connectedAtoms = molecule.Builder.CreateAtomContainer();
            IAtom connectedAtom = null;
            foreach (var bond in bonds)
            {
                connectedAtom = bond.GetConnectedAtom(atom);
                if (IsPlacedHeavyAtom(connectedAtom))
                {
                    connectedAtoms.Atoms.Add(connectedAtom);
                }
            }
            return connectedAtoms;
        }

        /**
         *  Gets numberOfUnplacedHeavyAtoms (no Flag ISPLACED, no Hydrogens)
         *
         * @param  ac AtomContainer
         * @return #UnplacedAtoms
         */
        public int NumberOfUnplacedHeavyAtoms(IAtomContainer ac)
        {
            int nUnplacedHeavyAtoms = 0;
            for (int i = 0; i < ac.Atoms.Count; i++)
            {
                if (IsUnplacedHeavyAtom(ac.Atoms[i]))
                {
                    nUnplacedHeavyAtoms++;
                }
            }
            return nUnplacedHeavyAtoms;
        }

        /**
         *  Gets the allPlacedAtoms attribute of the AtomPlacer3D object.
         *
         * @return    The allPlacedAtoms value
         */
        private IAtomContainer GetAllPlacedAtoms(IAtomContainer molecule)
        {
            IAtomContainer placedAtoms = molecule.Builder.CreateAtomContainer();    // Changed by Kaz
            for (int i = 0; i < molecule.Atoms.Count; i++)
            {
                if (molecule.Atoms[i].IsPlaced)
                {
                    placedAtoms.Atoms.Add(molecule.Atoms[i]);
                }
            }
            return placedAtoms;
        }

        /**
         *  True is all the atoms in the given AtomContainer have been placed.
         *
         * @param  ac  The AtomContainer to be searched
         * @return     True is all the atoms in the given AtomContainer have been placed
         */
        public bool AllHeavyAtomsPlaced(IAtomContainer ac)
        {
            for (int i = 0; i < ac.Atoms.Count; i++)
            {
                if (IsUnplacedHeavyAtom(ac.Atoms[i]))
                {
                    return false;
                }
            }
            return true;
        }

        /**
         *  Determine if the atom is non-hydrogen and has not been placed.
         *
         * @param  atom The atom to be checked
         * @return      True if the atom is non-hydrogen and has not been placed
         */
#if TEST
        public
#endif
        bool IsUnplacedHeavyAtom(IAtom atom)
        {
            return (!atom.IsPlaced && IsHeavyAtom(atom));
        }

        /**
         *  Determine if the atom is non-hydrogen and has been placed.
         *
         * @param  atom The atom to be checked
         * @return      True if the atom is non-hydrogen and has been placed
         */
#if TEST
        public
#endif
        bool IsPlacedHeavyAtom(IAtom atom)
        {
            return atom.IsPlaced && IsHeavyAtom(atom);
        }

        /**
         *  Determine if the atom is non-hydrogen and is aliphatic.
         *
         * @param  atom The atom to be checked
         * @return      True if the atom is non-hydrogen and is aliphatic
         */
#if TEST
        public
#endif
        bool IsAliphaticHeavyAtom(IAtom atom)
        {
            return atom.IsAliphatic && IsHeavyAtom(atom);
        }

        /**
         * Determine if the atom is non-hydrogen and is in a ring.
         * Ring membership is determined from a property flag only, rather than a ring
         * membership test
         *
         * @param  atom The atom to be checked
         * @return      True if the atom is non-hydrogen and is in a ring
         */
#if TEST
        public
#endif
        bool IsRingHeavyAtom(IAtom atom)
        {
            return atom.IsInRing && IsHeavyAtom(atom);
        }

        /**
         * Determine if the atom is heavy (non-hydrogen).
         *
         * @param  atom The atom to be checked
         * @return      True if the atom is non-hydrogen
         */
#if TEST
        public
#endif
        bool IsHeavyAtom(IAtom atom)
        {
            return !atom.Symbol.Equals("H");
        }
    }
}
