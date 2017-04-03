/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
 *                    2008  Gilleain Torrance <gilleain@users.sf.net>
 *                    2014  Mark B Vine (orcid:0000-0002-7794-0426)
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */
using NCDK.Common.Mathematics;
using NCDK.Geometries;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NCDK.Numerics;

namespace NCDK.Layout
{
    /// <summary>
    /// Class providing methods for generating coordinates for ring atoms.
    /// Various situations are supported, like condensation, spiro-attachment, etc.
    /// They can be used for Automated Structure Diagram Generation or in the interactive
    /// buildup of ringsystems by the user.
    /// </summary>
    // @cdk.module sdg
    // @cdk.githash
    public class RingPlacer
    {
        // indicate we want to snap to regular polygons for bridges, not generally applicable
        // but useful for macro cycles
        internal const string SnapHint = "sdg.snap.bridged";
        const bool debug = false;

        public IAtomContainer Molecule { get; set; }
        public AtomPlacer AtomPlacer { get; set; } = new AtomPlacer();

        internal const int Fused = 0;
        internal const int Bridged = 1;
        internal const int Spiro = 2;

        /// <summary>
        /// Default ring start angles. Map contains pairs: ring size with start angle.
        /// </summary>
        public static IDictionary<int, double> DefaultAngles { get; } = new Dictionary<int, double>()
        {
            {3, Math.PI* (0.1666667)},
            {4, Math.PI * (0.25)},
            {5, Math.PI* (0.3)},
            {7, Math.PI* (0.07)},
            {8, Math.PI* (0.125)},
        };

        /// <summary>
        /// Suggested ring start angles for JChempaint, different due to Y inversion of canvas.
        /// </summary>
        public static IDictionary<int, double> JCPAngles { get; } = new Dictionary<int, double>()
        {
            {3, Math.PI * (0.5)},
            {4, Math.PI * (0.25)},
            {5, Math.PI * (0.5)},
            {7, Math.PI * (0.07)},
            {8, Math.PI * (0.125)},
        };

        public RingPlacer()
        {
        }

        /// <summary>
        /// Generated coordinates for a given ring. Multiplexes to special handlers
        /// for the different possible situations (spiro-, fusion-, bridged attachement)
        /// </summary>
        /// <param name="ring">The ring to be placed</param>
        /// <param name="sharedAtoms">The atoms of this ring, also members of another ring, which are already placed</param>
        /// <param name="sharedAtomsCenter">The geometric center of these atoms</param>
        /// <param name="ringCenterVector">A vector pointing the the center of the new ring</param>
        /// <param name="bondLength">The standard bondlength</param>
        public void PlaceRing(IRing ring, IAtomContainer sharedAtoms, Vector2 sharedAtomsCenter, Vector2 ringCenterVector, double bondLength)
        {
            int sharedAtomCount = sharedAtoms.Atoms.Count;
            Debug.WriteLine("placeRing -> sharedAtomCount: " + sharedAtomCount);
            if (sharedAtomCount > 2)
            {
                PlaceBridgedRing(ring, sharedAtoms, sharedAtomsCenter, ringCenterVector, bondLength);
            }
            else if (sharedAtomCount == 2)
            {
                PlaceFusedRing(ring, sharedAtoms, ringCenterVector, bondLength);
            }
            else if (sharedAtomCount == 1)
            {
                PlaceSpiroRing(ring, sharedAtoms, sharedAtomsCenter, ringCenterVector, bondLength);
            }
        }

        /// <summary>
        /// Place ring with default start angles, using <see cref="DefaultAngles"/>.
        /// </summary>
        /// <param name="ring">the ring to place.</param>
        /// <param name="ringCenter">center coordinates of the ring.</param>
        /// <param name="bondLength">given bond length.</param>
        public void PlaceRing(IRing ring, Vector2 ringCenter, double bondLength)
        {
            PlaceRing(ring, ringCenter, bondLength, DefaultAngles);
        }

        /// <summary>
        /// Place ring with user provided angles.
        /// </summary>
        /// <param name="ring">the ring to place.</param>
        /// <param name="ringCenter">center coordinates of the ring.</param>
        /// <param name="bondLength">given bond length.</param>
        /// <param name="startAngles">a map with start angles when drawing the ring.</param>
        public void PlaceRing(IRing ring, Vector2 ringCenter, double bondLength, IDictionary<int, double> startAngles)
        {
            var radius = this.GetNativeRingRadius(ring, bondLength);
            double addAngle = 2 * Math.PI / ring.RingSize;

            IAtom startAtom = ring.Atoms.First();
            Vector2 p = new Vector2((ringCenter.X + radius), ringCenter.Y);
            startAtom.Point2D = p;
            double startAngle = Math.PI * 0.5;

            // Different ring sizes get different start angles to have visually
            // correct placement
            int ringSize = ring.RingSize;
            startAngle = startAngles[ringSize];

            var bonds = ring.GetConnectedBonds(startAtom);
            // Store all atoms to draw in consecutive order relative to the chosen bond.
            var atomsToDraw = new List<IAtom>();
            IAtom currentAtom = startAtom;
            IBond currentBond = (IBond)bonds.First();
            for (int i = 0; i < ring.Bonds.Count; i++)
            {
                currentBond = ring.GetNextBond(currentBond, currentAtom);
                currentAtom = currentBond.GetConnectedAtom(currentAtom);
                atomsToDraw.Add(currentAtom);
            }
            AtomPlacer.PopulatePolygonCorners(atomsToDraw, ringCenter, startAngle, addAngle, radius);
        }

        /// <summary>
        /// Positions the aliphatic substituents of a ring system
        /// </summary>
        /// <param name="rs">The RingSystem for which the substituents are to be laid out</param>
        /// <returns>A list of atoms that where laid out</returns>
        public IAtomContainer PlaceRingSubstituents(IRingSet rs, double bondLength)
        {
            Debug.WriteLine("RingPlacer.PlaceRingSubstituents() start");
            IRing ring = null;
            IAtom atom = null;
            IAtomContainer unplacedPartners = rs.Builder.CreateAtomContainer();
            IAtomContainer sharedAtoms = rs.Builder.CreateAtomContainer();
            IAtomContainer primaryAtoms = rs.Builder.CreateAtomContainer();
            IAtomContainer treatedAtoms = rs.Builder.CreateAtomContainer();
            for (int j = 0; j < rs.Count; j++)
            {
                ring = (IRing)rs[j]; // Get the j-th Ring in RingSet rs 
                for (int k = 0; k < ring.Atoms.Count; k++)
                {
                    unplacedPartners.RemoveAllElements();
                    sharedAtoms.RemoveAllElements();
                    primaryAtoms.RemoveAllElements();
                    atom = ring.Atoms[k];
                    var rings = rs.GetRings(atom);
                    var centerOfRingGravity = GeometryUtil.Get2DCenter(rings);
                    AtomPlacer.PartitionPartners(atom, unplacedPartners, sharedAtoms);
                    AtomPlacer.MarkNotPlaced(unplacedPartners);
                    try
                    {
                        for (int f = 0; f < unplacedPartners.Atoms.Count; f++)
                        {
                            Debug.WriteLine("placeRingSubstituents->unplacedPartners: "
                                    + (Molecule.Atoms.IndexOf(unplacedPartners.Atoms[f]) + 1));
                        }
                    }
                    catch (Exception)
                    {
                    }

                    treatedAtoms.Add(unplacedPartners);
                    if (unplacedPartners.Atoms.Count > 0)
                    {
                        AtomPlacer.DistributePartners(atom, sharedAtoms, centerOfRingGravity, unplacedPartners, bondLength);
                    }
                }
            }
            Debug.WriteLine("RingPlacer.PlaceRingSubstituents() end");
            return treatedAtoms;
        }

        /// <summary>
        /// Generated coordinates for a given ring, which is connected to another ring a bridged ring,
        /// i.e. it shares more than two atoms with another ring.
        /// </summary>
        /// <param name="ring">The ring to be placed</param>
        /// <param name="sharedAtoms">The atoms of this ring, also members of another ring, which are already placed</param>
        /// <param name="sharedAtomsCenter">The geometric center of these atoms</param>
        /// <param name="ringCenterVector">A vector pointing the the center of the new ring</param>
        /// <param name="bondLength">The standard bondlength</param>
        private void PlaceBridgedRing(IRing ring, IAtomContainer sharedAtoms, Vector2 sharedAtomsCenter, Vector2 ringCenterVector, double bondLength)
        {
            IAtom[] bridgeAtoms = GetBridgeAtoms(sharedAtoms);
            IAtom bondAtom1 = bridgeAtoms[0];
            IAtom bondAtom2 = bridgeAtoms[1];

            Vector2 bondAtom1Vector = bondAtom1.Point2D.Value;
            Vector2 bondAtom2Vector = bondAtom2.Point2D.Value;

            bool snap = ring.GetProperty<bool>(SnapHint);

            Vector2 midPoint = GetMidPoint(bondAtom1Vector, bondAtom2Vector);
            Vector2 ringCenter;
            double radius = GetNativeRingRadius(ring, bondLength);
            double offset = 0;

            if (snap)
            {
                ringCenter = midPoint;
                ringCenterVector = GetPerpendicular(bondAtom1Vector, bondAtom2Vector,
                                                    new Vector2(midPoint.X - sharedAtomsCenter.X, midPoint.Y - sharedAtomsCenter.Y));

                offset = 0;
                foreach (var atom in sharedAtoms.Atoms)
                {
                    if (atom == bondAtom1 || atom == bondAtom2)
                        continue;
                    double dist = Vector2.Distance(atom.Point2D.Value, midPoint);
                    if (dist > offset)
                        offset = dist;
                }
            }
            else
            {
                ringCenter = sharedAtomsCenter;
            }

            Vector2.Normalize(ringCenterVector);
            ringCenterVector = ringCenterVector * (radius - offset);
            ringCenter += ringCenterVector;

            Vector2 originRingCenterVector = ringCenter;

            bondAtom1Vector -= originRingCenterVector;
            bondAtom2Vector -= originRingCenterVector;

            var occupiedAngle = Vectors.Angle(bondAtom1Vector, bondAtom2Vector);

            double remainingAngle = (2 * Math.PI) - occupiedAngle;
            double addAngle = remainingAngle / (ring.RingSize - sharedAtoms.Atoms.Count + 1);

            Debug.WriteLine("placeBridgedRing->occupiedAngle: " + Vectors.RadianToDegree(occupiedAngle));
            Debug.WriteLine("placeBridgedRing->remainingAngle: " + Vectors.RadianToDegree(remainingAngle));

            Debug.WriteLine("placeBridgedRing->addAngle: " + Vectors.RadianToDegree(addAngle));

            IAtom startAtom;

            double centerX = ringCenter.X;
            double centerY = ringCenter.Y;

            double xDiff = bondAtom1.Point2D.Value.X - bondAtom2.Point2D.Value.X;
            double yDiff = bondAtom1.Point2D.Value.Y - bondAtom2.Point2D.Value.Y;

            double startAngle;

            int direction = 1;
            // if bond is vertical
            if (xDiff == 0)
            {
                Debug.WriteLine("placeBridgedRing->Bond is vertical");
                //starts with the lower Atom
                if (bondAtom1.Point2D.Value.Y > bondAtom2.Point2D.Value.Y)
                {
                    startAtom = bondAtom1;
                }
                else
                {
                    startAtom = bondAtom2;
                }

                //changes the drawing direction
                if (centerX < sharedAtomsCenter.X)
                {
                    direction = 1;
                }
                else
                {
                    direction = -1;
                }
            }

            // if bond is not vertical
            else
            {
                //starts with the left Atom
                if (bondAtom1.Point2D.Value.X > bondAtom2.Point2D.Value.X)
                {
                    startAtom = bondAtom1;
                }
                else
                {
                    startAtom = bondAtom2;
                }

                //changes the drawing direction
                if (centerY - sharedAtomsCenter.Y > (centerX - sharedAtomsCenter.X) * yDiff / xDiff)
                {
                    direction = 1;
                }
                else
                {
                    direction = -1;
                }
            }
            startAngle = GeometryUtil.GetAngle(startAtom.Point2D.Value.X - ringCenter.X, startAtom.Point2D.Value.Y
                    - ringCenter.Y);

            IAtom currentAtom = startAtom;
            IBond currentBond = sharedAtoms.GetConnectedBonds(currentAtom).First();

            var atomsToDraw = new List<IAtom>();
            for (int i = 0; i < ring.Bonds.Count; i++)
            {
                currentBond = ring.GetNextBond(currentBond, currentAtom);
                currentAtom = currentBond.GetConnectedAtom(currentAtom);
                if (!sharedAtoms.Contains(currentAtom))
                {
                    atomsToDraw.Add(currentAtom);
                }
            }
            try
            {
                Debug.WriteLine("placeBridgedRing->atomsToPlace: " + AtomPlacer.ListNumbers(Molecule, atomsToDraw));
                Debug.WriteLine("placeBridgedRing->startAtom is: " + (Molecule.Atoms.IndexOf(startAtom) + 1));
                Debug.WriteLine("placeBridgedRing->startAngle: " + Vectors.RadianToDegree(startAngle));
                Debug.WriteLine("placeBridgedRing->addAngle: " + Vectors.RadianToDegree(addAngle));
            }
            catch (Exception)
            {
                Debug.WriteLine("Caught an exception while logging in RingPlacer");
            }

            addAngle = addAngle * direction;
            AtomPlacer.PopulatePolygonCorners(atomsToDraw, ringCenter, startAngle, addAngle, radius);
        }

        /// <summary>
        /// Generated coordinates for a given ring, which is connected to a spiro ring.
        /// The rings share exactly one atom.
        /// </summary>
        /// <param name="ring">The ring to be placed</param>
        /// <param name="sharedAtoms">The atoms of this ring, also members of another ring, which are already placed</param>
        /// <param name="sharedAtomsCenter">The geometric center of these atoms</param>
        /// <param name="ringCenterVector">A vector pointing the the center of the new ring</param>
        /// <param name="bondLength">The standard bondlength</param>
        public void PlaceSpiroRing(IRing ring, IAtomContainer sharedAtoms, Vector2 sharedAtomsCenter, Vector2 ringCenterVector, double bondLength)
        {
            Debug.WriteLine("placeSpiroRing");
            double radius = GetNativeRingRadius(ring, bondLength);
            Vector2 ringCenter = sharedAtomsCenter;
            ringCenterVector = Vector2.Normalize(ringCenterVector);
            ringCenterVector *= radius;
            ringCenter += ringCenterVector;
            double addAngle = 2 * Math.PI / ring.RingSize;

            IAtom startAtom = sharedAtoms.Atoms[0];

            //double centerX = ringCenter.X;
            //double centerY = ringCenter.Y;

            //int direction = 1;

            IAtom currentAtom = startAtom;
            double startAngle = GeometryUtil.GetAngle(startAtom.Point2D.Value.X - ringCenter.X, startAtom.Point2D.Value.Y
                    - ringCenter.Y);
            // Get one bond connected to the spiro bridge atom. It doesn't matter in which direction we draw.
            var bonds = ring.GetConnectedBonds(startAtom);

            IBond currentBond = (IBond)bonds.First();

            var atomsToDraw = new List<IAtom>();
            // Store all atoms to draw in consequtive order relative to the chosen bond.
            for (int i = 0; i < ring.Bonds.Count; i++)
            {
                currentBond = ring.GetNextBond(currentBond, currentAtom);
                currentAtom = currentBond.GetConnectedAtom(currentAtom);
                atomsToDraw.Add(currentAtom);
            }
            Debug.WriteLine("currentAtom  " + currentAtom);
            Debug.WriteLine("startAtom  " + startAtom);

            AtomPlacer.PopulatePolygonCorners(atomsToDraw, ringCenter, startAngle, addAngle, radius);
        }

        /// <summary>
        /// Generated coordinates for a given ring, which is fused to another ring.
        /// The rings share exactly one bond.
        /// </summary>
        /// <param name="ring">The ring to be placed</param>
        /// <param name="sharedAtoms">The atoms of this ring, also members of another ring, which are already placed</param>
        /// <param name="ringCenterVector">A vector pointing the the center of the new ring</param>
        /// <param name="bondLength">The standard bondlength</param>
        public void PlaceFusedRing(IRing ring,
                                   IAtomContainer sharedAtoms,
                                   Vector2 ringCenterVector,
                                   double bondLength)
        {
            Debug.WriteLine("RingPlacer.PlaceFusedRing() start");

            IAtom beg = sharedAtoms.Atoms[0];
            IAtom end = sharedAtoms.Atoms[1];

            Vector2 pBeg = beg.Point2D.Value;
            Vector2 pEnd = end.Point2D.Value;

            // fuse the ring perpendicular to the bond, ring center is not
            // sub-optimal if non-regular/convex polygon (e.g. macro cycle)
            ringCenterVector = GetPerpendicular(pBeg, pEnd, ringCenterVector);

            double radius = GetNativeRingRadius(ring, bondLength);
            double newRingPerpendicular = Math.Sqrt(Math.Pow(radius, 2) - Math.Pow(bondLength / 2, 2));
            ringCenterVector = Vector2.Normalize(ringCenterVector);
            Debug.WriteLine($"placeFusedRing->: ringCenterVector.Length {ringCenterVector.Length()}");
            ringCenterVector *= newRingPerpendicular;
            Vector2 ringCenter = GetMidPoint(pBeg, pEnd);
            ringCenter += ringCenterVector;

            Vector2 originRingCenterVector = ringCenter;

            pBeg -= originRingCenterVector;
            pEnd -= originRingCenterVector;

            double occupiedAngle = Angle(pBeg, pEnd);

            double remainingAngle = (2 * Math.PI) - occupiedAngle;
            double addAngle = remainingAngle / (ring.RingSize - 1);

            Debug.WriteLine("placeFusedRing->occupiedAngle: " + Vectors.RadianToDegree(occupiedAngle));
            Debug.WriteLine("placeFusedRing->remainingAngle: " + Vectors.RadianToDegree(remainingAngle));
            Debug.WriteLine("placeFusedRing->addAngle: " + Vectors.RadianToDegree(addAngle));

            IAtom startAtom;

            double centerX = ringCenter.X;
            double centerY = ringCenter.Y;

            double xDiff = beg.Point2D.Value.X - end.Point2D.Value.X;
            double yDiff = beg.Point2D.Value.Y - end.Point2D.Value.Y;

            double startAngle; ;

            int direction = 1;
            // if bond is vertical
            if (xDiff == 0)
            {
                Debug.WriteLine("placeFusedRing->Bond is vertical");
                //starts with the lower Atom
                if (beg.Point2D.Value.Y > end.Point2D.Value.Y)
                {
                    startAtom = beg;
                }
                else
                {
                    startAtom = end;
                }

                //changes the drawing direction
                if (centerX < beg.Point2D.Value.X)
                {
                    direction = 1;
                }
                else
                {
                    direction = -1;
                }
            }

            // if bond is not vertical
            else
            {
                //starts with the left Atom
                if (beg.Point2D.Value.X > end.Point2D.Value.X)
                {
                    startAtom = beg;
                }
                else
                {
                    startAtom = end;
                }

                //changes the drawing direction
                if (centerY - beg.Point2D.Value.Y > (centerX - beg.Point2D.Value.X) * yDiff / xDiff)
                {
                    direction = 1;
                }
                else
                {
                    direction = -1;
                }
            }
            startAngle = GeometryUtil.GetAngle(startAtom.Point2D.Value.X - ringCenter.X, startAtom.Point2D.Value.Y - ringCenter.Y);

            IAtom currentAtom = startAtom;
            // determine first bond in Ring
            //        int k = 0;
            //        for (k = 0; k < ring.GetElectronContainerCount(); k++) {
            //            if (ring.GetElectronContainer(k) is IBond) break;
            //        }
            IBond currentBond = sharedAtoms.Bonds[0];
            var atomsToDraw = new List<IAtom>();
            for (int i = 0; i < ring.Bonds.Count - 2; i++)
            {
                currentBond = ring.GetNextBond(currentBond, currentAtom);
                currentAtom = currentBond.GetConnectedAtom(currentAtom);
                atomsToDraw.Add(currentAtom);
            }
            addAngle = addAngle * direction;
            try
            {
                Debug.WriteLine("placeFusedRing->startAngle: " + Vectors.RadianToDegree(startAngle));
                Debug.WriteLine("placeFusedRing->addAngle: " + Vectors.RadianToDegree(addAngle));
                Debug.WriteLine("placeFusedRing->startAtom is: " + (Molecule.Atoms.IndexOf(startAtom) + 1));
                Debug.WriteLine("AtomsToDraw: " + AtomPlacer.ListNumbers(Molecule, atomsToDraw));
            }
            catch (Exception)
            {
                Debug.WriteLine("Caught an exception while logging in RingPlacer");
            }
            AtomPlacer.PopulatePolygonCorners(atomsToDraw, ringCenter, startAngle, addAngle, radius);
        }

        /// <summary>
        /// Get the middle of two provide points.
        /// </summary>
        /// <param name="a">first point</param>
        /// <param name="b">second point</param>
        /// <returns>mid</returns>
        private static Vector2 GetMidPoint(Vector2 a, Vector2 b)
        {
            return new Vector2((a.X + b.X) / 2, (a.Y + b.Y) / 2);
        }

        private static double Angle(Vector2 pBeg, Vector2 pEnd)
        {
            // TODO inline to allow generic Tuple2ds
            return Vectors.Angle(pBeg, pEnd);
        }

        /// <summary>
        /// Gat a vector perpendicular to the line, a-b, that is pointing
        /// the same direction as 'ref'.
        /// </summary>
        /// <param name="a">first coordinate</param>
        /// <param name="b">second coordinate</param>
        /// <param name="reference">reference vector</param>
        /// <returns>perpendicular vector</returns>
        private static Vector2 GetPerpendicular(Vector2 a, Vector2 b, Vector2 reference)
        {
            Vector2 pVec = new Vector2(-(a.Y - b.Y), a.X - b.X);
            if (Vector2.Dot(pVec, reference) < 0)
                pVec = -pVec;
            return pVec;
        }

        /// <summary>
        /// True if coordinates have been assigned to all atoms in all rings.
        /// </summary>
        /// <param name="rs">The ringset to be checked</param>
        /// <returns>True if coordinates have been assigned to all atoms in all rings.</returns>
        public bool AllPlaced(IRingSet rs)
        {
            for (int i = 0; i < rs.Count; i++)
            {
                if (!((IRing)rs[i]).IsPlaced)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Walks throught the atoms of each ring in a ring set and marks
        /// a ring as PLACED if all of its atoms have been placed.
        /// </summary>
        /// <param name="rs">The ringset to be checked</param>
        public void CheckAndMarkPlaced(IEnumerable<IRing> rs)
        {
            bool allPlaced = true;
            foreach (var ring in rs)
            { 
                allPlaced = true;
                for (int j = 0; j < ring.Atoms.Count; j++)
                {
                    if (!(ring.Atoms[j]).IsPlaced)
                    {
                        allPlaced = false;
                        break;
                    }
                }
                ring.IsPlaced = allPlaced;
            }
        }

        /// <summary>
        /// Returns the bridge atoms, that is the outermost atoms in
        /// the chain of more than two atoms which are shared by two rings
        /// </summary>
        /// <param name="sharedAtoms">The atoms (n > 2) which are shared by two rings</param>
        /// <returns>The bridge atoms, i.e. the outermost atoms in the chain of more than two atoms which are shared by two rings</returns>
        private IAtom[] GetBridgeAtoms(IAtomContainer sharedAtoms)
        {
            IAtom[] bridgeAtoms = new IAtom[2];
            IAtom atom;
            int counter = 0;
            for (int f = 0; f < sharedAtoms.Atoms.Count; f++)
            {
                atom = sharedAtoms.Atoms[f];
                if (sharedAtoms.GetConnectedAtoms(atom).Count() == 1)
                {
                    bridgeAtoms[counter] = atom;
                    counter++;
                }
            }
            return bridgeAtoms;
        }

        /// <summary>
        /// Partition the bonding partners of a given atom into ring atoms and non-ring atoms
        /// </summary>
        /// <param name="atom">The atom whose bonding partners are to be partitioned</param>
        /// <param name="ring">The ring against which the bonding partners are checked</param>
        /// <param name="ringAtoms">An AtomContainer to store the ring bonding partners</param>
        /// <param name="nonRingAtoms">An AtomContainer to store the non-ring bonding partners</param>
        public void PartitionNonRingPartners(IAtom atom, IRing ring, IAtomContainer ringAtoms, IAtomContainer nonRingAtoms)
        {
            var atoms = Molecule.GetConnectedAtoms(atom);
            foreach (var curAtom in atoms)
            {
                if (!ring.Contains(curAtom))
                {
                    nonRingAtoms.Atoms.Add(curAtom);
                }
                else
                {
                    ringAtoms.Atoms.Add(curAtom);
                }
            }
        }

        /// <summary>
        /// Returns the ring radius of a perfect polygons of size ring.Atoms.Count
        /// The ring radius is the distance of each atom to the ringcenter.
        /// </summary>
        /// <param name="ring">The ring for which the radius is to calculated</param>
        /// <param name="bondLength">The bond length for each bond in the ring</param>
        /// <returns>The radius of the ring.</returns>
        public double GetNativeRingRadius(IRing ring, double bondLength)
        {
            int size = ring.Atoms.Count;
            double radius = bondLength / (2 * Math.Sin((Math.PI) / size));
            return radius;
        }

        /// <summary>
        /// Calculated the center for the first ring so that it can
        /// layed out. Only then, all other rings can be assigned
        /// coordinates relative to it.
        /// </summary>
        /// <param name="ring">The ring for which the center is to be calculated</param>
        /// <returns>A Vector2 pointing to the new ringcenter</returns>
        public Vector2 GetRingCenterOfFirstRing(IRing ring, Vector2 bondVector, double bondLength)
        {
            int size = ring.Atoms.Count;
            double radius = bondLength / (2 * Math.Sin((Math.PI) / size));
            double newRingPerpendicular = Math.Sqrt(Math.Pow(radius, 2) - Math.Pow(bondLength / 2, 2));
            /* get the angle between the x axis and the bond vector */
            double rotangle = GeometryUtil.GetAngle(bondVector.X, bondVector.Y);
            // Add 90 Degrees to this angle, this is supposed to be the new ringcenter vector
            rotangle += Math.PI / 2;
            return new Vector2((Math.Cos(rotangle) * newRingPerpendicular), (Math.Sin(rotangle) * newRingPerpendicular));
        }

        /// <summary>
        /// Layout all rings in the given RingSet that are connected to a given Ring
        /// </summary>
        /// <param name="rs">The RingSet to be searched for rings connected to Ring</param>
        /// <param name="ring">The Ring for which all connected rings in RingSet are to be layed out.</param>
        public void PlaceConnectedRings(IRingSet rs, IRing ring, int handleType, double bondLength)
        {
            var connectedRings = rs.GetConnectedRings(ring);

            //        Debug.WriteLine(rs.ReportRingList(Molecule));
            foreach (var container in connectedRings)
            {
                IRing connectedRing = (IRing)container;
                if (!connectedRing.IsPlaced)
                {
                    //                Debug.WriteLine(ring.ToString(Molecule));
                    //                Debug.WriteLine(connectedRing.ToString(Molecule));
                    IAtomContainer sharedAtoms = AtomContainerManipulator.GetIntersection(ring, connectedRing);
                    int numSharedAtoms = sharedAtoms.Atoms.Count;
                    Debug.WriteLine("placeConnectedRings-> connectedRing: " + (ring.ToString()));
                    if ((numSharedAtoms == 2 && handleType == Fused) ||
                        (numSharedAtoms == 1 && handleType == Spiro) ||
                        (numSharedAtoms > 2 && handleType == Bridged))
                    {
                        Vector2 sharedAtomsCenter = GeometryUtil.Get2DCenter(sharedAtoms);
                        Vector2 oldRingCenter = GeometryUtil.Get2DCenter(ring);
                        Vector2 tempVector = sharedAtomsCenter;
                        Vector2 newRingCenterVector = tempVector;
                        newRingCenterVector -= oldRingCenter;
                        Vector2 oldRingCenterVector = newRingCenterVector;
                        Debug.WriteLine($"placeConnectedRing -> tempVector: {tempVector}, tempVector.Length: {tempVector.Length()}");
                        Debug.WriteLine("placeConnectedRing -> bondCenter: " + sharedAtomsCenter);
                        Debug.WriteLine($"placeConnectedRing -> oldRingCenterVector.Length: {oldRingCenterVector.Length()}");
                        Debug.WriteLine($"placeConnectedRing -> newRingCenterVector.Length: {newRingCenterVector.Length()}");
                        Vector2 tempPoint = sharedAtomsCenter;
                        tempPoint += newRingCenterVector;
                        PlaceRing(connectedRing, sharedAtoms, sharedAtomsCenter, newRingCenterVector, bondLength);
                        connectedRing.IsPlaced = true;
                        PlaceConnectedRings(rs, connectedRing, handleType, bondLength);
                    }
                }
            }
        }
    }
}
