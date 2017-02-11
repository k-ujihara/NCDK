/*  Copyright (C) 1997-2014  The Chemistry Development Kit (CDK) project
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *  All we ask is that proper credit is given for our work, which includes
 *  - but is not limited to - adding the above copyright notice to the beginning
 *  of your source code files, and to any copyright notice that you may distribute
 *  with programs based on this work.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT Any WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using NCDK.Common.Collections;
using NCDK.Common.Mathematics;
using NCDK.SGroups;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NCDK.Numerics;

namespace NCDK.Geometries
{
    /**
     * A set of static utility classes for geometric calculations and operations. This class is
     * extensively used, for example, by JChemPaint to edit molecule. All methods in this class change
     * the coordinates of the atoms. Use GeometryTools if you use an external set of coordinates (e. g.
     * renderingCoordinates from RendererModel)
     *
     * @author seb
     * @author Stefan Kuhn
     * @author Egon Willighagen
     * @author Ludovic Petain
     * @author Christian Hoppe
     * @author Niels Out
     * @author John May
     * @cdk.githash
     */
    public sealed class GeometryUtil
    {
        /**
		 * Provides the coverage of coordinates for this molecule.
		 *
		 * @see GeometryUtil#Get2DCoordinateCoverage(IAtomContainer)
		 * @see GeometryUtil#Get3DCoordinateCoverage(IAtomContainer)
		 */
        public enum CoordinateCoverage
        {
            /**
			 * All atoms have coordinates.
			 */
            FULL,

            /**
			 * At least one atom has coordinates but not all.
			 */
            PARTIAL,

            /**
			 * No atoms have coordinates.
			 */
            None
        }

        /**
		 * Static utility class can not be instantiated.
		 */
        private GeometryUtil() { }

        /**
		 * Adds an automatically calculated offset to the coordinates of all atoms such that all
		 * coordinates are positive and the smallest x or y coordinate is exactly zero. See comment for
		 * Center(IAtomContainer atomCon, Dimension areaDim, Dictionary renderingCoordinates) for details
		 * on coordinate sets
		 *
		 * @param atomCon AtomContainer for which all the atoms are translated to positive coordinates
		 */
        public static void TranslateAllPositive(IAtomContainer atomCon)
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            foreach (var atom in atomCon.Atoms)
            {
                if (atom.Point2D != null)
                {
                    if (atom.Point2D.Value.X < minX)
                    {
                        minX = atom.Point2D.Value.X;
                    }
                    if (atom.Point2D.Value.Y < minY)
                    {
                        minY = atom.Point2D.Value.Y;
                    }
                }
            }
            Debug.WriteLine("Translating: minx=" + minX + ", minY=" + minY);
            Translate2D(atomCon, minX * -1, minY * -1);
        }

        /**
		 * Translates the given molecule by the given Vector. See comment for Center(IAtomContainer
		 * atomCon, Dimension areaDim, Dictionary renderingCoordinates) for details on coordinate sets
		 *
		 * @param atomCon The molecule to be translated
		 * @param transX  translation in x direction
		 * @param transY  translation in y direction
		 */
        public static void Translate2D(IAtomContainer atomCon, double transX, double transY)
        {
            Translate2D(atomCon, new Vector2(transX, transY));
        }

        /**
		 * Scales a molecule such that it fills a given percentage of a given dimension. See comment for
		 * Center(IAtomContainer atomCon, Dimension areaDim, Dictionary renderingCoordinates) for details
		 * on coordinate sets
		 *
		 * @param atomCon    The molecule to be scaled {width, height}
		 * @param areaDim    The dimension to be filled {width, height}
		 * @param fillFactor The percentage of the dimension to be filled
		 */
        public static void ScaleMolecule(IAtomContainer atomCon, double[] areaDim, double fillFactor)
        {
            double[] molDim = Get2DDimension(atomCon);
            double widthFactor = (double)areaDim[0] / (double)molDim[0];
            double heightFactor = (double)areaDim[1] / (double)molDim[1];
            double scaleFactor = Math.Min(widthFactor, heightFactor) * fillFactor;
            ScaleMolecule(atomCon, scaleFactor);
        }

        /**
		 * Multiplies all the coordinates of the atoms of the given molecule with the scalefactor. See
		 * comment for Center(IAtomContainer atomCon, Dimension areaDim, Dictionary renderingCoordinates)
		 * for details on coordinate sets
		 *
		 * @param atomCon     The molecule to be scaled
		 * @param scaleFactor Description of the Parameter
		 */
        public static void ScaleMolecule(IAtomContainer atomCon, double scaleFactor)
        {
            for (int i = 0; i < atomCon.Atoms.Count; i++)
            {
                if (atomCon.Atoms[i].Point2D != null)
                {
                    atomCon.Atoms[i].Point2D = atomCon.Atoms[i].Point2D.Value * scaleFactor;
                }
            }
            // scale Sgroup brackets
            if (atomCon.GetProperty<IList<Sgroup>>(CDKPropertyName.CTAB_SGROUPS) != null)
            {
                IList<Sgroup> sgroups = atomCon.GetProperty<IList<Sgroup>>(CDKPropertyName.CTAB_SGROUPS);
                foreach (var sgroup in sgroups)
                {
                    IList<SgroupBracket> brackets = (IList<SgroupBracket>)sgroup.GetValue(SgroupKey.CtabBracket);
                    if (brackets != null)
                    {
                        foreach (var bracket in brackets)
                        {
                            bracket.FirstPoint = bracket.FirstPoint * scaleFactor;
                            bracket.SecondPoint = bracket.SecondPoint * scaleFactor;
                        }
                    }
                }
            }
        }

        /**
		 * Centers the molecule in the given area. See comment for Center(IAtomContainer atomCon,
		 * Dimension areaDim, Dictionary renderingCoordinates) for details on coordinate sets
		 *
		 * @param atomCon molecule to be centered
		 * @param areaDim dimension in which the molecule is to be centered, array containing
		 *                {width, height}
		 */
        public static void Center(IAtomContainer atomCon, double[] areaDim)
        {
            double[] molDim = Get2DDimension(atomCon);
            double transX = (areaDim[0] - molDim[0]) / 2;
            double transY = (areaDim[1] - molDim[1]) / 2;
            TranslateAllPositive(atomCon);
            Translate2D(atomCon, new Vector2(transX, transY));
        }

        /**
		 * Translates a molecule from the origin to a new point denoted by a vector. See comment for
		 * Center(IAtomContainer atomCon, Dimension areaDim, Dictionary renderingCoordinates) for details
		 * on coordinate sets
		 *
		 * @param atomCon molecule to be translated
		 * @param vector  dimension that represents the translation vector
		 */
        public static void Translate2D(IAtomContainer atomCon, Vector2 vector)
        {
            foreach (var atom in atomCon.Atoms)
            {
                if (atom.Point2D != null)
                {
                    atom.Point2D = atom.Point2D.Value + vector;
                }
                else
                {
                    Trace.TraceWarning("Could not translate atom in 2D space");
                }
            }
            // translate Sgroup brackets
            if (atomCon.GetProperty<IList<Sgroup>>(CDKPropertyName.CTAB_SGROUPS) != null)
            {
                IList<Sgroup> sgroups = atomCon.GetProperty<IList<Sgroup>>(CDKPropertyName.CTAB_SGROUPS);
                foreach (var sgroup in sgroups)
                {
                    IList<SgroupBracket> brackets = (IList<SgroupBracket>)sgroup.GetValue(SgroupKey.CtabBracket);
                    if (brackets != null)
                    {
                        foreach (var bracket in brackets)
                        {
                            bracket.FirstPoint = bracket.FirstPoint + vector;
                            bracket.SecondPoint = bracket.SecondPoint + vector;
                        }
                    }
                }
            }
        }

        /**
		 * Rotates a molecule around a given center by a given angle.
		 *
		 * @param atomCon The molecule to be rotated
		 * @param center  A point giving the rotation center
		 * @param angle   The angle by which to rotate the molecule, in radians
		 */
        public static void Rotate(IAtomContainer atomCon, Vector2 center, double angle)
        {
            Vector2 point;
            double costheta = Math.Cos(angle);
            double sintheta = Math.Sin(angle);
            IAtom atom;
            for (int i = 0; i < atomCon.Atoms.Count; i++)
            {
                atom = atomCon.Atoms[i];
                point = atom.Point2D.Value;
                double relativex = point.X - center.X;
                double relativey = point.Y - center.Y;
                point.X = (relativex * costheta - relativey * sintheta + center.X);
                point.Y = (relativex * sintheta + relativey * costheta + center.Y);
                atom.Point2D = point;
            }
        }

        /**
		 * Rotates a 3D point about a specified line segment by a specified angle.
		 *
		 * The code is based on code available <a href="http://astronomy.swin.edu.au/~pbourke/geometry/rotate/source.c">here</a>.
		 * Positive angles are anticlockwise looking down the axis towards the origin. Assume right hand
		 * coordinate system.
		 *
		 * @param atom  The atom to rotate
		 * @param p1    The  first point of the line segment
		 * @param p2    The second point of the line segment
		 * @param angle The angle to rotate by (in degrees)
		 */
        public static void Rotate(IAtom atom, Vector3 p1, Vector3 p2, double angle)
        {
            double costheta, sintheta;

            Vector3 r = new Vector3();

            r.X = p2.X - p1.X;
            r.Y = p2.Y - p1.Y;
            r.Z = p2.Z - p1.Z;
            r = Vector3.Normalize(r);

            angle = angle * Math.PI / 180.0;
            costheta = Math.Cos(angle);
            sintheta = Math.Sin(angle);

            Vector3 p = atom.Point3D.Value;
            p.X -= p1.X;
            p.Y -= p1.Y;
            p.Z -= p1.Z;

            Vector3 q = Vector3.Zero;
            q.X += ((costheta + (1 - costheta) * r.X * r.X) * p.X);
            q.X += (((1 - costheta) * r.X * r.Y - r.Z * sintheta) * p.Y);
            q.X += (((1 - costheta) * r.X * r.Z + r.Y * sintheta) * p.Z);

            q.Y += (((1 - costheta) * r.X * r.Y + r.Z * sintheta) * p.X);
            q.Y += ((costheta + (1 - costheta) * r.Y * r.Y) * p.Y);
            q.Y += (((1 - costheta) * r.Y * r.Z - r.X * sintheta) * p.Z);

            q.Z += (((1 - costheta) * r.X * r.Z - r.Y * sintheta) * p.X);
            q.Z += (((1 - costheta) * r.Y * r.Z + r.X * sintheta) * p.Y);
            q.Z += ((costheta + (1 - costheta) * r.Z * r.Z) * p.Z);

            q.X += p1.X;
            q.Y += p1.Y;
            q.Z += p1.Z;

            atom.Point3D = q;
        }

        /**
		 * Returns the dimension of a molecule (width/height).
		 *
		 * @param atomCon of which the dimension should be returned
		 * @return array containing {width, height}
		 */
        public static double[] Get2DDimension(IAtomContainer atomCon)
        {
            double[] minmax = GetMinMax(atomCon);
            double maxX = minmax[2];
            double maxY = minmax[3];
            double minX = minmax[0];
            double minY = minmax[1];
            return new double[] { maxX - minX, maxY - minY };
        }

        /**
		 * Returns the minimum and maximum X and Y coordinates of the atoms in the
		 * AtomContainer. The output is returned as: <pre>
		 *   minmax[0] = minX;
		 *   minmax[1] = minY;
		 *   minmax[2] = maxX;
		 *   minmax[3] = maxY;
		 * </pre>
		 * See comment for Center(IAtomContainer atomCon, Dimension areaDim, Dictionary
		 * renderingCoordinates) for details on coordinate sets
		 *
		 * @param container Description of the Parameter
		 * @return An four int array as defined above.
		 */
        public static double[] GetMinMax(IAtomContainer container)
        {
            double maxX = -double.MaxValue;
            double maxY = -double.MaxValue;
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            for (int i = 0; i < container.Atoms.Count; i++)
            {
                IAtom atom = container.Atoms[i];
                if (atom.Point2D != null)
                {
                    if (atom.Point2D.Value.X > maxX)
                    {
                        maxX = atom.Point2D.Value.X;
                    }
                    if (atom.Point2D.Value.X < minX)
                    {
                        minX = atom.Point2D.Value.X;
                    }
                    if (atom.Point2D.Value.Y > maxY)
                    {
                        maxY = atom.Point2D.Value.Y;
                    }
                    if (atom.Point2D.Value.Y < minY)
                    {
                        minY = atom.Point2D.Value.Y;
                    }
                }
            }
            double[] minmax = new double[4];
            minmax[0] = minX;
            minmax[1] = minY;
            minmax[2] = maxX;
            minmax[3] = maxY;
            return minmax;
        }

        /**
		 * Translates a molecule from the origin to a new point denoted by a vector. See comment for
		 * Center(IAtomContainer atomCon, Dimension areaDim, Dictionary renderingCoordinates) for details
		 * on coordinate sets
		 *
		 * @param atomCon molecule to be translated
		 * @param p       Description of the Parameter
		 */
        public static void Translate2DCentreOfMassTo(IAtomContainer atomCon, Vector2 p)
        {
            Vector2? com = Get2DCentreOfMass(atomCon);
            Vector2 translation = new Vector2(p.X - com.Value.X, p.Y - com.Value.Y);
            foreach (var atom in atomCon.Atoms)
            {
                if (atom.Point2D != null)
                {
                    atom.Point2D = atom.Point2D.Value + translation;
                }
            }
        }

        /**
		 * Calculates the center of the given atoms and returns it as a Vector2. See comment for
		 * Center(IAtomContainer atomCon, Dimension areaDim, Dictionary renderingCoordinates) for details
		 * on coordinate sets
		 *
		 * @param atoms The vector of the given atoms
		 * @return The center of the given atoms as Vector2
		 */
        public static Vector2 Get2DCenter(IEnumerable<IAtom> atoms)
        {
            double xsum = 0;
            double ysum = 0;
            int length = 0;
            foreach (var atom in atoms)
            {
                if (atom.Point2D != null)
                {
                    xsum += atom.Point2D.Value.X;
                    ysum += atom.Point2D.Value.Y;
                    length++;
                }
            }
            return new Vector2(xsum / length, ysum / length);
        }

        /**
		 * Returns the geometric center of all the rings in this ringset. See comment for
		 * Center(IAtomContainer atomCon, Dimension areaDim, Dictionary renderingCoordinates) for details
		 * on coordinate sets
		 *
		 * @param ringSet Description of the Parameter
		 * @return the geometric center of the rings in this ringset
		 */
        public static Vector2 Get2DCenter(IEnumerable<IRing> ringSet)
        {
            double centerX = 0;
            double centerY = 0;
            int count = 0;
            foreach (var ring in ringSet)
            {
                Vector2 centerPoint = Get2DCenter(ring);
                centerX += centerPoint.X;
                centerY += centerPoint.Y;
                count++;
            }
            return new Vector2(centerX / count, centerY / count);
        }

        /**
		 * Calculates the center of mass for the <code>Atom</code>s in the AtomContainer for the 2D
		 * coordinates. See comment for Center(IAtomContainer atomCon, Dimension areaDim, Dictionary
		 * renderingCoordinates) for details on coordinate sets
		 *
		 * @param ac AtomContainer for which the center of mass is calculated
		 * @return Null, if any of the atomcontainer {@link IAtom}'s
		 * masses are null
		 * @cdk.keyword center of mass
		 */
        public static Vector2? Get2DCentreOfMass(IAtomContainer ac)
        {
            double xsum = 0.0;
            double ysum = 0.0;

            double totalmass = 0.0;

            foreach (var a in ac.Atoms)
            {
                double? mass = a.ExactMass;
                if (mass == null) return null;
                totalmass += mass.Value;
                xsum += mass.Value * a.Point2D.Value.X;
                ysum += mass.Value * a.Point2D.Value.Y;
            }

            return new Vector2((xsum / totalmass), (ysum / totalmass));
        }

        /**
		 * Returns the geometric center of all the atoms in the atomContainer. See comment for
		 * Center(IAtomContainer atomCon, Dimension areaDim, Dictionary renderingCoordinates) for details
		 * on coordinate sets
		 *
		 * @param container Description of the Parameter
		 * @return the geometric center of the atoms in this atomContainer
		 */
        public static Vector2 Get2DCenter(IAtomContainer container)
        {
            double centerX = 0;
            double centerY = 0;
            double counter = 0;
            foreach (var atom in container.Atoms)
            {
                if (atom.Point2D != null)
                {
                    centerX += atom.Point2D.Value.X;
                    centerY += atom.Point2D.Value.Y;
                    counter++;
                }
            }
            return new Vector2((centerX / counter), (centerY / counter));
        }

        /**
		 * Translates the geometric 2DCenter of the given AtomContainer container to the specified
		 * Vector2 p.
		 *
		 * @param container AtomContainer which should be translated.
		 * @param p         New Location of the geometric 2D Center.
		 * @see #get2DCenter
		 * @see #translate2DCentreOfMassTo
		 */
        public static void Translate2DCenterTo(IAtomContainer container, Vector2 p)
        {
            Vector2 com = Get2DCenter(container);
            Vector2 translation = new Vector2(p.X - com.X, p.Y - com.Y);
            foreach (var atom in container.Atoms)
            {
                if (atom.Point2D != null)
                {
                    atom.Point2D = atom.Point2D.Value + translation;
                }
            }
        }

        /**
		 * Calculates the center of mass for the <code>Atom</code>s in the AtomContainer for the 2D
		 * coordinates. See comment for Center(IAtomContainer atomCon, Dimension areaDim, Dictionary
		 * renderingCoordinates) for details on coordinate sets
		 *
		 * @param ac AtomContainer for which the center of mass is calculated
		 * @return Description of the Return Value
		 * @cdk.keyword center of mass
		 * @cdk.dictref blue-obelisk:calculate3DCenterOfMass
		 */
        public static Vector3? Get3DCentreOfMass(IAtomContainer ac)
        {
            double xsum = 0.0;
            double ysum = 0.0;
            double zsum = 0.0;

            double totalmass = 0.0;

            foreach (var a in ac.Atoms)
            {
                double? mass = a.ExactMass;
                // some sanity checking
                if (a.Point3D == null) return null;
                if (mass == null) return null;

                totalmass += mass.Value;
                xsum += mass.Value * a.Point3D.Value.X;
                ysum += mass.Value * a.Point3D.Value.Y;
                zsum += mass.Value * a.Point3D.Value.Z;
            }

            return new Vector3((xsum / totalmass), (ysum / totalmass), (zsum / totalmass));
        }

        /**
		 * Returns the geometric center of all the atoms in this atomContainer. See comment for
		 * Center(IAtomContainer atomCon, Dimension areaDim, Dictionary renderingCoordinates) for details
		 * on coordinate sets
		 *
		 * @param ac Description of the Parameter
		 * @return the geometric center of the atoms in this atomContainer
		 */
        public static Vector3 Get3DCenter(IAtomContainer ac)
        {
            double centerX = 0;
            double centerY = 0;
            double centerZ = 0;
            double counter = 0;
            foreach (var atom in ac.Atoms)
            {
                if (atom.Point3D != null)
                {
                    centerX += atom.Point3D.Value.X;
                    centerY += atom.Point3D.Value.Y;
                    centerZ += atom.Point3D.Value.Z;
                    counter++;
                }
            }
            return new Vector3((centerX / counter), (centerY / counter), (centerZ / counter));
        }

        /**
		 * Gets the angle attribute of the GeometryTools class.
		 *
		 * @param xDiff Description of the Parameter
		 * @param yDiff Description of the Parameter
		 * @return The angle value
		 */
        public static double GetAngle(double xDiff, double yDiff)
        {
            double angle = 0;
            //		Debug.WriteLine("getAngle->xDiff: " + xDiff);
            //		Debug.WriteLine("getAngle->yDiff: " + yDiff);
            if (xDiff >= 0 && yDiff >= 0)
            {
                angle = Math.Atan(yDiff / xDiff);
            }
            else if (xDiff < 0 && yDiff >= 0)
            {
                angle = Math.PI + Math.Atan(yDiff / xDiff);
            }
            else if (xDiff < 0 && yDiff < 0)
            {
                angle = Math.PI + Math.Atan(yDiff / xDiff);
            }
            else if (xDiff >= 0 && yDiff < 0)
            {
                angle = 2 * Math.PI + Math.Atan(yDiff / xDiff);
            }
            return angle;
        }

        /**
		 * Gets the coordinates of two points (that represent a bond) and calculates for each the
		 * coordinates of two new points that have the given distance vertical to the bond.
		 *
		 * @param coords The coordinates of the two given points of the bond like this [point1x,
		 *               point1y, point2x, point2y]
		 * @param dist   The vertical distance between the given points and those to be calculated
		 * @return The coordinates of the calculated four points
		 */
        public static int[] DistanceCalculator(int[] coords, double dist)
        {
            double angle;
            if ((coords[2] - coords[0]) == 0)
            {
                angle = Math.PI / 2;
            }
            else
            {
                angle = Math.Atan(((double)coords[3] - (double)coords[1]) / ((double)coords[2] - (double)coords[0]));
            }
            int begin1X = (int)(Math.Cos(angle + Math.PI / 2) * dist + coords[0]);
            int begin1Y = (int)(Math.Sin(angle + Math.PI / 2) * dist + coords[1]);
            int begin2X = (int)(Math.Cos(angle - Math.PI / 2) * dist + coords[0]);
            int begin2Y = (int)(Math.Sin(angle - Math.PI / 2) * dist + coords[1]);
            int end1X = (int)(Math.Cos(angle - Math.PI / 2) * dist + coords[2]);
            int end1Y = (int)(Math.Sin(angle - Math.PI / 2) * dist + coords[3]);
            int end2X = (int)(Math.Cos(angle + Math.PI / 2) * dist + coords[2]);
            int end2Y = (int)(Math.Sin(angle + Math.PI / 2) * dist + coords[3]);

            return new int[] { begin1X, begin1Y, begin2X, begin2Y, end1X, end1Y, end2X, end2Y };
        }

        public static double[] DistanceCalculator(double[] coords, double dist)
        {
            double angle;
            if ((coords[2] - coords[0]) == 0)
            {
                angle = Math.PI / 2;
            }
            else
            {
                angle = Math.Atan((coords[3] - coords[1]) / (coords[2] - coords[0]));
            }
            double begin1X = (Math.Cos(angle + Math.PI / 2) * dist + coords[0]);
            double begin1Y = (Math.Sin(angle + Math.PI / 2) * dist + coords[1]);
            double begin2X = (Math.Cos(angle - Math.PI / 2) * dist + coords[0]);
            double begin2Y = (Math.Sin(angle - Math.PI / 2) * dist + coords[1]);
            double end1X = (Math.Cos(angle - Math.PI / 2) * dist + coords[2]);
            double end1Y = (Math.Sin(angle - Math.PI / 2) * dist + coords[3]);
            double end2X = (Math.Cos(angle + Math.PI / 2) * dist + coords[2]);
            double end2Y = (Math.Sin(angle + Math.PI / 2) * dist + coords[3]);

            return new double[] { begin1X, begin1Y, begin2X, begin2Y, end1X, end1Y, end2X, end2Y };
        }

        /**
		 * Writes the coordinates of the atoms participating the given bond into an array. See comment
		 * for Center(IAtomContainer atomCon, Dimension areaDim, Dictionary renderingCoordinates) for
		 * details on coordinate sets
		 *
		 * @param bond The given bond
		 * @return The array with the coordinates
		 */
        public static int[] GetBondCoordinates(IBond bond)
        {
            if (bond.Atoms[0].Point2D == null || bond.Atoms[1].Point2D == null)
            {
                Trace.TraceError("GetBondCoordinates() called on Bond without 2D coordinates!");
                return new int[0];
            }
            int beginX = (int)bond.Atoms[0].Point2D.Value.X;
            int endX = (int)bond.Atoms[1].Point2D.Value.X;
            int beginY = (int)bond.Atoms[0].Point2D.Value.Y;
            int endY = (int)bond.Atoms[1].Point2D.Value.Y;
            return new int[] { beginX, beginY, endX, endY };
        }

        /**
		 * Returns the atom of the given molecule that is closest to the given coordinates. See comment
		 * for Center(IAtomContainer atomCon, Dimension areaDim, Dictionary renderingCoordinates) for
		 * details on coordinate sets
		 *
		 * @param xPosition The x coordinate
		 * @param yPosition The y coordinate
		 * @param atomCon   The molecule that is searched for the closest atom
		 * @return The atom that is closest to the given coordinates
		 */
        public static IAtom GetClosestAtom(int xPosition, int yPosition, IAtomContainer atomCon)
        {
            IAtom closestAtom = null;
            IAtom currentAtom;
            double smallestMouseDistance = -1;
            double mouseDistance;
            double atomX;
            double atomY;
            for (int i = 0; i < atomCon.Atoms.Count; i++)
            {
                currentAtom = atomCon.Atoms[i];
                atomX = currentAtom.Point2D.Value.X;
                atomY = currentAtom.Point2D.Value.Y;
                mouseDistance = Math.Sqrt(Math.Pow(atomX - xPosition, 2) + Math.Pow(atomY - yPosition, 2));
                if (mouseDistance < smallestMouseDistance || smallestMouseDistance == -1)
                {
                    smallestMouseDistance = mouseDistance;
                    closestAtom = currentAtom;
                }
            }
            return closestAtom;
        }

        /**
		 * Returns the atom of the given molecule that is closest to the given atom (excluding itself).
		 *
		 * @param atomCon The molecule that is searched for the closest atom
		 * @param atom    The atom to search around
		 * @return The atom that is closest to the given coordinates
		 */
        public static IAtom GetClosestAtom(IAtomContainer atomCon, IAtom atom)
        {
            IAtom closestAtom = null;
            double min = double.MaxValue;
            Vector2 atomPosition = atom.Point2D.Value;
            for (int i = 0; i < atomCon.Atoms.Count; i++)
            {
                IAtom currentAtom = atomCon.Atoms[i];
                if (currentAtom != atom)
                {
                    double d = Vector2.Distance(atomPosition, currentAtom.Point2D.Value);
                    if (d < min)
                    {
                        min = d;
                        closestAtom = currentAtom;
                    }
                }
            }
            return closestAtom;
        }

        /**
		 * Returns the atom of the given molecule that is closest to the given coordinates and is not
		 * the atom. See comment for Center(IAtomContainer atomCon, Dimension areaDim, Dictionary
		 * renderingCoordinates) for details on coordinate sets
		 *
		 * @param xPosition The x coordinate
		 * @param yPosition The y coordinate
		 * @param atomCon   The molecule that is searched for the closest atom
		 * @param toignore  This molecule will not be returned.
		 * @return The atom that is closest to the given coordinates
		 */
        public static IAtom GetClosestAtom(double xPosition, double yPosition, IAtomContainer atomCon, IAtom toignore)
        {
            IAtom closestAtom = null;
            IAtom currentAtom;
            // we compare squared distances, allowing us to do one Sqrt()
            // calculation less
            double smallestSquaredMouseDistance = -1;
            double mouseSquaredDistance;
            double atomX;
            double atomY;
            for (int i = 0; i < atomCon.Atoms.Count; i++)
            {
                currentAtom = atomCon.Atoms[i];
                if (currentAtom != toignore)
                {
                    atomX = currentAtom.Point2D.Value.X;
                    atomY = currentAtom.Point2D.Value.Y;
                    mouseSquaredDistance = Math.Pow(atomX - xPosition, 2) + Math.Pow(atomY - yPosition, 2);
                    if (mouseSquaredDistance < smallestSquaredMouseDistance || smallestSquaredMouseDistance == -1)
                    {
                        smallestSquaredMouseDistance = mouseSquaredDistance;
                        closestAtom = currentAtom;
                    }
                }
            }
            return closestAtom;
        }

        /**
		 * Returns the atom of the given molecule that is closest to the given coordinates. See comment
		 * for Center(IAtomContainer atomCon, Dimension areaDim, Dictionary renderingCoordinates) for
		 * details on coordinate sets
		 *
		 * @param xPosition The x coordinate
		 * @param yPosition The y coordinate
		 * @param atomCon   The molecule that is searched for the closest atom
		 * @return The atom that is closest to the given coordinates
		 */
        public static IAtom GetClosestAtom(double xPosition, double yPosition, IAtomContainer atomCon)
        {
            IAtom closestAtom = null;
            IAtom currentAtom;
            double smallestMouseDistance = -1;
            double mouseDistance;
            double atomX;
            double atomY;
            for (int i = 0; i < atomCon.Atoms.Count; i++)
            {
                currentAtom = atomCon.Atoms[i];
                atomX = currentAtom.Point2D.Value.X;
                atomY = currentAtom.Point2D.Value.Y;
                mouseDistance = Math.Sqrt(Math.Pow(atomX - xPosition, 2) + Math.Pow(atomY - yPosition, 2));
                if (mouseDistance < smallestMouseDistance || smallestMouseDistance == -1)
                {
                    smallestMouseDistance = mouseDistance;
                    closestAtom = currentAtom;
                }
            }
            return closestAtom;
        }

        /**
		 * Returns the bond of the given molecule that is closest to the given coordinates. See comment
		 * for Center(IAtomContainer atomCon, Dimension areaDim, Dictionary renderingCoordinates) for
		 * details on coordinate sets
		 *
		 * @param xPosition The x coordinate
		 * @param yPosition The y coordinate
		 * @param atomCon   The molecule that is searched for the closest bond
		 * @return The bond that is closest to the given coordinates
		 */
        public static IBond GetClosestBond(int xPosition, int yPosition, IAtomContainer atomCon)
        {
            Vector2 bondCenter;
            IBond closestBond = null;

            double smallestMouseDistance = -1;
            double mouseDistance;
            foreach (var currentBond in atomCon.Bonds)
            {
                bondCenter = Get2DCenter(currentBond.Atoms);
                mouseDistance = Math.Sqrt(Math.Pow(bondCenter.X - xPosition, 2) + Math.Pow(bondCenter.Y - yPosition, 2));
                if (mouseDistance < smallestMouseDistance || smallestMouseDistance == -1)
                {
                    smallestMouseDistance = mouseDistance;
                    closestBond = currentBond;
                }
            }
            return closestBond;
        }

        /**
		 * Returns the bond of the given molecule that is closest to the given coordinates. See comment
		 * for Center(IAtomContainer atomCon, Dimension areaDim, Dictionary renderingCoordinates) for
		 * details on coordinate sets
		 *
		 * @param xPosition The x coordinate
		 * @param yPosition The y coordinate
		 * @param atomCon   The molecule that is searched for the closest bond
		 * @return The bond that is closest to the given coordinates
		 */
        public static IBond GetClosestBond(double xPosition, double yPosition, IAtomContainer atomCon)
        {
            Vector2 bondCenter;
            IBond closestBond = null;

            double smallestMouseDistance = -1;
            double mouseDistance;
            foreach (var currentBond in atomCon.Bonds)
            {
                bondCenter = Get2DCenter(currentBond.Atoms);
                mouseDistance = Math.Sqrt(Math.Pow(bondCenter.X - xPosition, 2) + Math.Pow(bondCenter.Y - yPosition, 2));
                if (mouseDistance < smallestMouseDistance || smallestMouseDistance == -1)
                {
                    smallestMouseDistance = mouseDistance;
                    closestBond = currentBond;
                }
            }
            return closestBond;
        }

        /**
		 * Sorts a Vector of atoms such that the 2D distances of the atom locations from a given point
		 * are smallest for the first atoms in the vector. See comment for Center(IAtomContainer
		 * atomCon, Dimension areaDim, Dictionary renderingCoordinates) for details on coordinate sets
		 *
		 * @param point The point from which the distances to the atoms are measured
		 * @param atoms The atoms for which the distances to point are measured
		 */
        public static void SortBy2DDistance(IAtom[] atoms, Vector2 point)
        {
            double distance1;
            double distance2;
            IAtom atom1;
            IAtom atom2;
            bool doneSomething;
            do
            {
                doneSomething = false;
                for (int f = 0; f < atoms.Length - 1; f++)
                {
                    atom1 = atoms[f];
                    atom2 = atoms[f + 1];
                    distance1 = Vector2.Distance(point, atom1.Point2D.Value);
                    distance2 = Vector2.Distance(point, atom2.Point2D.Value);
                    if (distance2 < distance1)
                    {
                        atoms[f] = atom2;
                        atoms[f + 1] = atom1;
                        doneSomething = true;
                    }
                }
            } while (doneSomething);
        }

        /**
		 * Determines the scale factor for displaying a structure loaded from disk in a frame. An
		 * average of all bond length values is produced and a scale factor is determined which would
		 * scale the given molecule such that its See comment for Center(IAtomContainer atomCon,
		 * Dimension areaDim, Dictionary renderingCoordinates) for details on coordinate sets
		 *
		 * @param container  The AtomContainer for which the ScaleFactor is to be calculated
		 * @param bondLength The target bond length
		 * @return The ScaleFactor with which the AtomContainer must be scaled to have the target bond
		 * length
		 */

        public static double GetScaleFactor(IAtomContainer container, double bondLength)
        {
            double currentAverageBondLength = GetBondLengthMedian(container);
            if (currentAverageBondLength == 0 || double.IsNaN(currentAverageBondLength)) return 1;
            return bondLength / currentAverageBondLength;
        }

        /**
		 * An average of all 2D bond length values is produced. Bonds which have Atom's with no
		 * coordinates are disregarded. See comment for Center(IAtomContainer atomCon, Dimension
		 * areaDim, Dictionary renderingCoordinates) for details on coordinate sets
		 *
		 * @param container The AtomContainer for which the average bond length is to be calculated
		 * @return the average bond length
		 */
        public static double GetBondLengthAverage(IAtomContainer container)
        {
            double bondLengthSum = 0;
            int bondCounter = 0;
            foreach (var bond in container.Bonds)
            {
                IAtom atom1 = bond.Atoms[0];
                IAtom atom2 = bond.Atoms[1];
                if (atom1.Point2D != null && atom2.Point2D != null)
                {
                    bondCounter++;
                    bondLengthSum += GetLength2D(bond);
                }
            }
            return bondLengthSum / bondCounter;
        }

        /**
		 * Returns the geometric length of this bond in 2D space. See comment for Center(IAtomContainer
		 * atomCon, Dimension areaDim, Dictionary renderingCoordinates) for details on coordinate sets
		 *
		 * @param bond Description of the Parameter
		 * @return The geometric length of this bond
		 */
        public static double GetLength2D(IBond bond)
        {
            if (bond.Atoms[0] == null || bond.Atoms[1] == null)
            {
                return 0.0;
            }
            Vector2 point1 = bond.Atoms[0].Point2D.Value;
            Vector2 point2 = bond.Atoms[1].Point2D.Value;
            if (point1 == null || point2 == null)
            {
                return 0.0;
            }
            return Vector2.Distance(point1, point2);
        }

        /**
		 * Determines if all this <see cref="IAtomContainer"/>'s atoms contain
		 * 2D coordinates. If any atom is null or has unset 2D coordinates this method will return
		 * false.
		 *
		 * @param container the atom container to examine
		 * @return indication that all 2D coordinates are available
		 * @see IAtom#Point2D
		 */
        public static bool Has2DCoordinates(IAtomContainer container)
        {

            if (container == null || container.Atoms.Count == 0) return false;

            foreach (var atom in container.Atoms)
            {

                if (atom == null || atom.Point2D == null) return false;

            }

            return true;

        }

        /**
		 * Determines the coverage of this <see cref="IAtomContainer"/>'s 2D
		 * coordinates. If all atoms are non-null and have 2D coordinates this method will return {@link
		 * CoordinateCoverage#FULL}. If one or more atoms does have 2D coordinates and any others atoms
		 * are null or are missing 2D coordinates this method will return {@link
		 * CoordinateCoverage#PARTIAL}. If all atoms are null or are all missing 2D coordinates this
		 * method will return {@link CoordinateCoverage#None}. If the provided container is null {@link
		 * CoordinateCoverage#None} is also returned.
		 *
		 * @param container the container to inspect
		 * @return {@link CoordinateCoverage#FULL}, {@link CoordinateCoverage#PARTIAL} or {@link
		 * CoordinateCoverage#None} depending on the number of 3D coordinates present
		 * @see CoordinateCoverage
		 * @see #Has2DCoordinates(IAtomContainer)
		 * @see #Get3DCoordinateCoverage(IAtomContainer)
		 * @see IAtom#Point2D
		 */
        public static CoordinateCoverage Get2DCoordinateCoverage(IAtomContainer container)
        {

            if (container == null || container.Atoms.Count == 0) return CoordinateCoverage.None;

            int count = 0;

            foreach (var atom in container.Atoms)
            {
                count += atom != null && atom.Point2D != null ? 1 : 0;
            }

            return count == 0 ? CoordinateCoverage.None : count == container.Atoms.Count ? CoordinateCoverage.FULL
                    : CoordinateCoverage.PARTIAL;

        }

        /**
		 * Determines if this AtomContainer contains 2D coordinates for some or all molecules. See
		 * comment for Center(IAtomContainer atomCon, Dimension areaDim, Dictionary renderingCoordinates)
		 * for details on coordinate sets
		 *
		 * @param container the molecule to be considered
		 * @return 0 no 2d, 1=some, 2= for each atom
		 * @see #Get2DCoordinateCoverage(IAtomContainer)
		 * @deprecated use {@link #Get2DCoordinateCoverage(IAtomContainer)}
		 * for determining partial coordinates
		 */
        [Obsolete]
        public static int Has2DCoordinatesNew(IAtomContainer container)
        {
            if (container == null) return 0;

            bool no2d = false;
            bool with2d = false;
            foreach (var atom in container.Atoms)
            {
                if (atom.Point2D == null)
                {
                    no2d = true;
                }
                else
                {
                    with2d = true;
                }
            }
            if (!no2d && with2d)
            {
                return 2;
            }
            else if (no2d && with2d)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        /**
		 * Determines if this Atom contains 2D coordinates. See comment for Center(IAtomContainer
		 * atomCon, Dimension areaDim, Dictionary renderingCoordinates) for details on coordinate sets
		 *
		 * @param atom Description of the Parameter
		 * @return bool indication that 2D coordinates are available
		 */
        public static bool Has2DCoordinates(IAtom atom)
        {
            return (atom.Point2D != null);
        }

        /**
		 * Determines if this Bond contains 2D coordinates. See comment for Center(IAtomContainer
		 * atomCon, Dimension areaDim, Dictionary renderingCoordinates) for details on coordinate sets
		 *
		 * @param bond Description of the Parameter
		 * @return bool indication that 2D coordinates are available
		 */
        public static bool Has2DCoordinates(IBond bond)
        {
            foreach (var iAtom in bond.Atoms)
            {
                if (iAtom.Point2D == null)
                {
                    return false;
                }
            }
            return true;
        }

        /**
		 * Determines if all this <see cref="IAtomContainer"/>'s atoms contain
		 * 3D coordinates. If any atom is null or has unset 3D coordinates this method will return
		 * false. If the provided container is null false is returned.
		 *
		 * @param container the atom container to examine
		 * @return indication that all 3D coordinates are available
		 * @see IAtom#Point3D
		 */
        public static bool Has3DCoordinates(IAtomContainer container)
        {

            if (container == null || container.Atoms.Count == 0) return false;

            foreach (var atom in container.Atoms)
            {

                if (atom == null || atom.Point3D == null) return false;

            }

            return true;

        }

        /**
		 * Determines the coverage of this <see cref="IAtomContainer"/>'s 3D
		 * coordinates. If all atoms are non-null and have 3D coordinates this method will return {@link
		 * CoordinateCoverage#FULL}. If one or more atoms does have 3D coordinates and any others atoms
		 * are null or are missing 3D coordinates this method will return {@link
		 * CoordinateCoverage#PARTIAL}. If all atoms are null or are all missing 3D coordinates this
		 * method will return {@link CoordinateCoverage#None}. If the provided container is null {@link
		 * CoordinateCoverage#None} is also returned.
		 *
		 * @param container the container to inspect
		 * @return {@link CoordinateCoverage#FULL}, {@link CoordinateCoverage#PARTIAL} or {@link
		 * CoordinateCoverage#None} depending on the number of 3D coordinates present
		 * @see CoordinateCoverage
		 * @see #Has3DCoordinates(IAtomContainer)
		 * @see #Get2DCoordinateCoverage(IAtomContainer)
		 * @see IAtom#Point3D
		 */
        public static CoordinateCoverage Get3DCoordinateCoverage(IAtomContainer container)
        {

            if (container == null || container.Atoms.Count == 0) return CoordinateCoverage.None;

            int count = 0;

            foreach (var atom in container.Atoms)
            {
                count += atom != null && atom.Point3D != null ? 1 : 0;
            }

            return count == 0 ? CoordinateCoverage.None : count == container.Atoms.Count ? CoordinateCoverage.FULL
                    : CoordinateCoverage.PARTIAL;

        }

        /**
		 * Determines the normalized vector orthogonal on the vector p1->p2.
		 *
		 * @param point1 Description of the Parameter
		 * @param point2 Description of the Parameter
		 * @return Description of the Return Value
		 */
        public static Vector2 CalculatePerpendicularUnitVector(Vector2 point1, Vector2 point2)
        {
            Vector2 vector = point2 - point1;
            vector = Vector2.Normalize(vector);

            // Return the perpendicular vector
            return new Vector2(-1 * vector.Y, vector.X);
        }

        /**
		 * Calculates the normalization factor in order to get an average bond length of 1.5. It takes
		 * only into account Bond's with two atoms. See comment for Center(IAtomContainer atomCon,
		 * Dimension areaDim, Dictionary renderingCoordinates) for details on coordinate sets
		 *
		 * @param container Description of the Parameter
		 * @return The normalizationFactor value
		 */
        public static double GetNormalizationFactor(IAtomContainer container)
        {
            double bondlength = 0.0;
            double ratio;
            /*
			 * Desired bond length for storing structures in MDL mol files This
			 * should probably be set externally (from system wide settings)
			 */
            double desiredBondLength = 1.5;
            // loop over all bonds and determine the mean bond distance
            int counter = 0;
            foreach (var bond in container.Bonds)
            {
                // only consider two atom bonds into account
                if (bond.Atoms.Count == 2)
                {
                    counter++;
                    IAtom atom1 = bond.Atoms[0];
                    IAtom atom2 = bond.Atoms[1];
                    bondlength += Math.Sqrt(Math.Pow(atom1.Point2D.Value.X - atom2.Point2D.Value.X, 2)
                            + Math.Pow(atom1.Point2D.Value.Y - atom2.Point2D.Value.Y, 2));
                }
            }
            bondlength = bondlength / counter;
            ratio = desiredBondLength / bondlength;
            return ratio;
        }

        /**
		 * Determines the best alignment for the label of an atom in 2D space. It returns 1 if left
		 * aligned, and -1 if right aligned. See comment for Center(IAtomContainer atomCon, Dimension
		 * areaDim, Dictionary renderingCoordinates) for details on coordinate sets
		 *
		 * @param container Description of the Parameter
		 * @param atom      Description of the Parameter
		 * @return The bestAlignmentForLabel value
		 */
        public static int GetBestAlignmentForLabel(IAtomContainer container, IAtom atom)
        {
            double overallDiffX = 0;
            foreach (var connectedAtom in container.GetConnectedAtoms(atom))
            {
                overallDiffX += connectedAtom.Point2D.Value.X - atom.Point2D.Value.X;
            }
            if (overallDiffX <= 0)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        /**
		 * Determines the best alignment for the label of an atom in 2D space. It returns 1 if right
		 * (=default) aligned, and -1 if left aligned. returns 2 if top aligned, and -2 if H is aligned
		 * below the atom See comment for Center(IAtomContainer atomCon, Dimension areaDim, Dictionary
		 * renderingCoordinates) for details on coordinate sets
		 *
		 * @param container Description of the Parameter
		 * @param atom      Description of the Parameter
		 * @return The bestAlignmentForLabel value
		 */
        public static int GetBestAlignmentForLabelXY(IAtomContainer container, IAtom atom)
        {
            double overallDiffX = 0;
            double overallDiffY = 0;
            foreach (var connectedAtom in container.GetConnectedAtoms(atom))
            {
                overallDiffX += connectedAtom.Point2D.Value.X - atom.Point2D.Value.X;
                overallDiffY += connectedAtom.Point2D.Value.Y - atom.Point2D.Value.Y;
            }
            if (Math.Abs(overallDiffY) > Math.Abs(overallDiffX))
            {
                if (overallDiffY < 0)
                    return 2;
                else
                    return -2;
            }
            else
            {
                if (overallDiffX <= 0)
                    return 1;
                else
                    return -1;
            }
        }

        /**
		 * Returns the atoms which are closes to an atom in an AtomContainer by distance in 3d.
		 *
		 * @param container The AtomContainer to examine
		 * @param startAtom the atom to start from
		 * @param max       the number of neighbours to return
		 * @return the average bond length
		 * @.openscience.cdk.exception.CDKException Description of the Exception
		 */
        public static IList<IAtom> FindClosestInSpace(IAtomContainer container, IAtom startAtom, int max)
        {
            if (startAtom.Point3D == null)
            {
                throw new CDKException("No point3d, but FindClosestInSpace is working on point3ds");
            }
            Vector3 originalPoint = startAtom.Point3D.Value;
            IDictionary<Double, IAtom> atomsByDistance = new SortedDictionary<Double, IAtom>();
            foreach (var atom in container.Atoms)
            {
                if (atom != startAtom)
                {
                    if (atom.Point3D == null)
                    {
                        throw new CDKException("No point3d, but FindClosestInSpace is working on point3ds");
                    }
                    double distance = Vector3.Distance(atom.Point3D.Value, originalPoint);
                    atomsByDistance.Add(distance, atom);
                }
            }
            // FIXME: should there not be some sort here??
            List<IAtom> returnValue = new List<IAtom>();
            int i = 0;
            foreach (var key in atomsByDistance.Keys)
            {
                if (!(i < max)) break;
                returnValue.Add(atomsByDistance[key]);
                i++;
            }
            return (returnValue);
        }

        /**
		 * Returns a IDictionary with the AtomNumbers, the first number corresponds to the first (or the largest
		 * AtomContainer) atomcontainer. It is recommend to sort the atomContainer due to their number
		 * of atoms before calling this function.
		 *
		 * The molecules needs to be aligned before! (coordinates are needed)
		 *
		 * @param firstAtomContainer  the (largest) first aligned AtomContainer which is the reference
		 * @param secondAtomContainer the second aligned AtomContainer
		 * @param searchRadius        the radius of space search from each atom
		 * @return a IDictionary of the mapped atoms
		 * @.openscience.cdk.exception.CDKException Description of the Exception
		 */
        public static IDictionary<int, int> MapAtomsOfAlignedStructures(IAtomContainer firstAtomContainer,
                IAtomContainer secondAtomContainer, double searchRadius, IDictionary<int, int> mappedAtoms)
        {
            GetLargestAtomContainer(firstAtomContainer, secondAtomContainer);
            double[][] distanceMatrix = Arrays.CreateJagged<double>(firstAtomContainer.Atoms.Count, secondAtomContainer.Atoms.Count);
            for (int i = 0; i < firstAtomContainer.Atoms.Count; i++)
            {
                Vector3 firstAtomPoint = firstAtomContainer.Atoms[i].Point3D.Value;
                for (int j = 0; j < secondAtomContainer.Atoms.Count; j++)
                {
                    distanceMatrix[i][j] = Vector3.Distance(firstAtomPoint, secondAtomContainer.Atoms[j].Point3D.Value);
                }
            }

            double minimumDistance;
            for (int i = 0; i < firstAtomContainer.Atoms.Count; i++)
            {
                minimumDistance = searchRadius;
                for (int j = 0; j < secondAtomContainer.Atoms.Count; j++)
                {
                    if (distanceMatrix[i][j] < searchRadius && distanceMatrix[i][j] < minimumDistance)
                    {
                        //check atom properties
                        if (CheckAtomMapping(firstAtomContainer, secondAtomContainer, i, j))
                        {
                            minimumDistance = distanceMatrix[i][j];
                            mappedAtoms.Add(firstAtomContainer.Atoms.IndexOf(firstAtomContainer.Atoms[i]),
                                    secondAtomContainer.Atoms.IndexOf(secondAtomContainer.Atoms[j]));
                        }
                    }
                }
            }
            return mappedAtoms;
        }

        // FIXME: huh!?!?!
        private static void GetLargestAtomContainer(IAtomContainer firstAC, IAtomContainer secondAC)
        {
            if (firstAC.Atoms.Count < secondAC.Atoms.Count)
            {
                IAtomContainer tmp;
                try
                {
                    tmp = (IAtomContainer)firstAC.Clone();
                    firstAC = (IAtomContainer)secondAC.Clone();
                    secondAC = (IAtomContainer)tmp.Clone();
                }
                catch (Exception e)
                {
                    // TODO Auto-generated catch block
                    Console.Error.WriteLine(e.StackTrace);
                }
            }
        }

        private static bool CheckAtomMapping(IAtomContainer firstAC, IAtomContainer secondAC, int posFirstAtom,
                int posSecondAtom)
        {
            IAtom firstAtom = firstAC.Atoms[posFirstAtom];
            IAtom secondAtom = secondAC.Atoms[posSecondAtom];
            return firstAtom.Symbol.Equals(secondAtom.Symbol)
                    && firstAC.GetConnectedAtoms(firstAtom).Count() == secondAC.GetConnectedAtoms(secondAtom).Count()
                    && firstAtom.BondOrderSum.Equals(secondAtom.BondOrderSum)
                    && firstAtom.MaxBondOrder == secondAtom.MaxBondOrder;
        }

        private static IAtomContainer SetVisitedFlagsToFalse(IAtomContainer atomContainer)
        {
            for (int i = 0; i < atomContainer.Atoms.Count; i++)
            {
                atomContainer.Atoms[i].IsVisited = false;
            }
            return atomContainer;
        }

        /**
		 * Return the RMSD of bonds length between the 2 aligned molecules.
		 *
		 * @param firstAtomContainer  the (largest) first aligned AtomContainer which is the reference
		 * @param secondAtomContainer the second aligned AtomContainer
		 * @param mappedAtoms         IDictionary: a IDictionary of the mapped atoms
		 * @param Coords3d            bool: true if moecules has 3D coords, false if molecules has 2D
		 *                            coords
		 * @return double: all the RMSD of bonds length
		 */
        public static double GetBondLengthRMSD(IAtomContainer firstAtomContainer, IAtomContainer secondAtomContainer,
                IDictionary<int, int> mappedAtoms, bool Coords3d)
        {
            //Debug.WriteLine("**** GT getBondLengthRMSD ****");
            var firstAtoms = mappedAtoms.Keys;
            IAtom centerAtomFirstMolecule;
            IAtom centerAtomSecondMolecule;
            IEnumerable<IAtom> connectedAtoms;
            double sum = 0;
            double n = 0;
            double distance1 = 0;
            double distance2 = 0;
            SetVisitedFlagsToFalse(firstAtomContainer);
            SetVisitedFlagsToFalse(secondAtomContainer);
            foreach (var firstAtom in firstAtoms)
            {
                centerAtomFirstMolecule = firstAtomContainer.Atoms[firstAtom];
                centerAtomFirstMolecule.IsVisited = true;
                centerAtomSecondMolecule = secondAtomContainer.Atoms[mappedAtoms[firstAtomContainer
                        .Atoms.IndexOf(centerAtomFirstMolecule)]];
                connectedAtoms = firstAtomContainer.GetConnectedAtoms(centerAtomFirstMolecule);
                foreach (var conAtom in connectedAtoms)
                {
                    //this step is built to know if the program has already calculate a bond length (so as not to have duplicate values)
                    if (!conAtom.IsVisited)
                    {
                        if (Coords3d)
                        {
                            distance1 = Vector3.Distance(centerAtomFirstMolecule.Point3D.Value, conAtom.Point3D.Value);
                            distance2 = Vector3.Distance(centerAtomSecondMolecule.Point3D.Value,
                                    secondAtomContainer.Atoms[mappedAtoms[firstAtomContainer.Atoms.IndexOf(conAtom)]]
                                            .Point3D.Value);
                            sum = sum + Math.Pow((distance1 - distance2), 2);
                            n++;
                        }
                        else
                        {
                            distance1 = Vector2.Distance(centerAtomFirstMolecule.Point2D.Value, conAtom.Point2D.Value);
                            distance2 = Vector2.Distance(centerAtomSecondMolecule.Point2D.Value,
                                    secondAtomContainer.Atoms[
                                            (mappedAtoms[firstAtomContainer.Atoms.IndexOf(conAtom)])].Point2D.Value);
                            sum = sum + Math.Pow((distance1 - distance2), 2);
                            n++;
                        }
                    }
                }
            }
            SetVisitedFlagsToFalse(firstAtomContainer);
            SetVisitedFlagsToFalse(secondAtomContainer);
            return Math.Sqrt(sum / n);
        }

        /**
		 * Return the variation of each angle value between the 2 aligned molecules.
		 *
		 * @param firstAtomContainer  the (largest) first aligned AtomContainer which is the reference
		 * @param secondAtomContainer the second aligned AtomContainer
		 * @param mappedAtoms         IDictionary: a IDictionary of the mapped atoms
		 * @return double: the value of the RMSD
		 */
        public static double GetAngleRMSD(IAtomContainer firstAtomContainer, IAtomContainer secondAtomContainer,
                IDictionary<int, int> mappedAtoms)
        {
            //Debug.WriteLine("**** GT GetAngleRMSD ****");
            IEnumerable<int> firstAtoms = mappedAtoms.Keys;
            //Debug.WriteLine("mappedAtoms:"+mappedAtoms.ToString());
            IAtom firstAtomfirstAC;
            IAtom centerAtomfirstAC;
            IAtom firstAtomsecondAC;
            IAtom secondAtomsecondAC;
            IAtom centerAtomsecondAC;
            double angleFirstMolecule;
            double angleSecondMolecule;
            double sum = 0;
            double n = 0;
            foreach (var firstAtomNumber in firstAtoms)
            {
                centerAtomfirstAC = firstAtomContainer.Atoms[firstAtomNumber];
                IList<IAtom> connectedAtoms = firstAtomContainer.GetConnectedAtoms(centerAtomfirstAC).ToList();
                if (connectedAtoms.Count > 1)
                {
                    //Debug.WriteLine("If "+centerAtomfirstAC.Symbol+" is the center atom :");
                    for (int i = 0; i < connectedAtoms.Count - 1; i++)
                    {
                        firstAtomfirstAC = connectedAtoms[i];
                        for (int j = i + 1; j < connectedAtoms.Count; j++)
                        {
                            angleFirstMolecule = GetAngle(centerAtomfirstAC, firstAtomfirstAC, connectedAtoms[j]);
                            centerAtomsecondAC = secondAtomContainer.Atoms[mappedAtoms[firstAtomContainer
                                    .Atoms.IndexOf(centerAtomfirstAC)]];
                            firstAtomsecondAC = secondAtomContainer.Atoms[mappedAtoms[firstAtomContainer
                                    .Atoms.IndexOf(firstAtomfirstAC)]];
                            secondAtomsecondAC = secondAtomContainer.Atoms[mappedAtoms[firstAtomContainer
                                    .Atoms.IndexOf(connectedAtoms[j])]];
                            angleSecondMolecule = GetAngle(centerAtomsecondAC, firstAtomsecondAC, secondAtomsecondAC);
                            sum = sum + Math.Pow(angleFirstMolecule - angleSecondMolecule, 2);
                            n++;
                            //Debug.WriteLine("Error for the "+firstAtomfirstAC.Symbol.ToLowerInvariant()+"-"+centerAtomfirstAC.Symbol+"-"+connectedAtoms[j].Symbol.ToLowerInvariant()+" Angle :"+deltaAngle+" degrees");
                        }
                    }
                }//if
            }
            return Math.Sqrt(sum / n);
        }

        private static double GetAngle(IAtom atom1, IAtom atom2, IAtom atom3)
        {
            Vector3 centerAtom = new Vector3();
            centerAtom.X = atom1.Point3D.Value.X;
            centerAtom.Y = atom1.Point3D.Value.Y;
            centerAtom.Z = atom1.Point3D.Value.Z;
            Vector3 firstAtom = new Vector3();
            Vector3 secondAtom = new Vector3();

            firstAtom.X = atom2.Point3D.Value.X;
            firstAtom.Y = atom2.Point3D.Value.Y;
            firstAtom.Z = atom2.Point3D.Value.Z;

            secondAtom.X = atom3.Point3D.Value.X;
            secondAtom.Y = atom3.Point3D.Value.Y;
            secondAtom.Z = atom3.Point3D.Value.Z;

            firstAtom = firstAtom - centerAtom;
            secondAtom = secondAtom - centerAtom;

            return Vectors.Angle(firstAtom, secondAtom);
            //return Math.Acos(Vector3.Dot(Vector3.Normalize(firstAtom), Vector3.Normalize(secondAtom)));
        }

        /**
		 * Return the RMSD between the 2 aligned molecules.
		 *
		 * @param firstAtomContainer  the (largest) first aligned AtomContainer which is the reference
		 * @param secondAtomContainer the second aligned AtomContainer
		 * @param mappedAtoms         IDictionary: a IDictionary of the mapped atoms
		 * @param Coords3d            bool: true if molecules has 3D coords, false if molecules has
		 *                            2D coords
		 * @return double: the value of the RMSD
		 * @.openscience.cdk.exception.CDKException if there is an error in getting mapped
		 *                                                    atoms
		 */
        public static double GetAllAtomRMSD(IAtomContainer firstAtomContainer, IAtomContainer secondAtomContainer,
                IDictionary<int, int> mappedAtoms, bool Coords3d)
        {
            //Debug.WriteLine("**** GT getAllAtomRMSD ****");
            double sum = 0;
            double RMSD;
            IEnumerable<int> firstAtoms = mappedAtoms.Keys;
            int secondAtomNumber;
            int n = 0;
            foreach (var firstAtomNumber in firstAtoms)
            {
                try
                {
                    secondAtomNumber = mappedAtoms[firstAtomNumber];
                    IAtom firstAtom = firstAtomContainer.Atoms[firstAtomNumber];
                    if (Coords3d)
                    {
                        sum = sum
                                + Math.Pow(
                                    Vector3.Distance(firstAtom.Point3D.Value,
                                                secondAtomContainer.Atoms[secondAtomNumber].Point3D.Value), 2);
                        n++;
                    }
                    else
                    {
                        sum = sum
                                + Math.Pow(
                                    Vector2.Distance(firstAtom.Point2D.Value,
                                                secondAtomContainer.Atoms[secondAtomNumber].Point2D.Value), 2);
                        n++;
                    }
                }
                catch (Exception ex)
                {
                    throw new CDKException(ex.Message, ex);
                }
            }
            RMSD = Math.Sqrt(sum / n);
            return RMSD;
        }

        /**
		 * Return the RMSD of the heavy atoms between the 2 aligned molecules.
		 *
		 * @param firstAtomContainer  the (largest) first aligned AtomContainer which is the reference
		 * @param secondAtomContainer the second aligned AtomContainer
		 * @param mappedAtoms         IDictionary: a IDictionary of the mapped atoms
		 * @param hetAtomOnly         bool: true if only hetero atoms should be considered
		 * @param Coords3d            bool: true if molecules has 3D coords, false if molecules has
		 *                            2D coords
		 * @return double: the value of the RMSD
		 */
        public static double GetHeavyAtomRMSD(IAtomContainer firstAtomContainer, IAtomContainer secondAtomContainer,
                IDictionary<int, int> mappedAtoms, bool hetAtomOnly, bool Coords3d)
        {
            //Debug.WriteLine("**** GT getAllAtomRMSD ****");
            double sum = 0;
            double RMSD;
            IEnumerable<int> firstAtoms = mappedAtoms.Keys;
            int secondAtomNumber;
            int n = 0;
            foreach (var firstAtomNumber in firstAtoms)
            {
                secondAtomNumber = mappedAtoms[firstAtomNumber];
                IAtom firstAtom = firstAtomContainer.Atoms[firstAtomNumber];
                if (hetAtomOnly)
                {
                    if (!firstAtom.Symbol.Equals("H") && !firstAtom.Symbol.Equals("C"))
                    {
                        if (Coords3d)
                        {
                            sum = sum
                                    + Math.Pow(
                                        Vector3.Distance(
                                            firstAtom.Point3D.Value,
                                            secondAtomContainer.Atoms[secondAtomNumber].Point3D.Value), 2);
                            n++;
                        }
                        else
                        {
                            sum = sum
                                    + Math.Pow(
                                        Vector2.Distance(
                                            firstAtom.Point2D.Value,
                                            secondAtomContainer.Atoms[secondAtomNumber].Point2D.Value), 2);
                            n++;
                        }
                    }
                }
                else
                {
                    if (!firstAtom.Symbol.Equals("H"))
                    {
                        if (Coords3d)
                        {
                            sum = sum
                                    + Math.Pow(
                                        Vector3.Distance(
                                            firstAtom.Point3D.Value,
                                            secondAtomContainer.Atoms[secondAtomNumber].Point3D.Value), 2);
                            n++;
                        }
                        else
                        {
                            sum = sum
                                    + Math.Pow(
                                        Vector2.Distance(
                                            firstAtom.Point2D.Value,
                                            secondAtomContainer.Atoms[secondAtomNumber].Point2D.Value), 2);
                            n++;
                        }
                    }
                }

            }
            RMSD = Math.Sqrt(sum / n);
            return RMSD;
        }

        /**
		 * An average of all 3D bond length values is produced, using point3ds in atoms. Atom's with no
		 * coordinates are disregarded.
		 *
		 * @param container The AtomContainer for which the average bond length is to be calculated
		 * @return the average bond length
		 */
        public static double GetBondLengthAverage3D(IAtomContainer container)
        {
            double bondLengthSum = 0;
            int bondCounter = 0;
            foreach (var bond in container.Bonds)
            {
                IAtom atom1 = bond.Atoms[0];
                IAtom atom2 = bond.Atoms[1];
                if (atom1.Point3D != null && atom2.Point3D != null)
                {
                    bondCounter++;
                    bondLengthSum += Vector3.Distance(atom1.Point3D.Value, atom2.Point3D.Value);
                }
            }
            return bondLengthSum / bondCounter;
        }

        /**
		 * Shift the container horizontally to the right to make its bounds not overlap with the other
		 * bounds. To avoid dependence on Java AWT, rectangles are described by arrays of double. Each
		 * rectangle is specified by {minX, minY, maxX, maxY}.
		 *
		 * @param container the <see cref="IAtomContainer"/> to shift to the
		 *                  right
		 * @param bounds    the bounds of the <see cref="IAtomContainer"/> to shift
		 * @param last      the bounds that is used as reference
		 * @param gap       the gap between the two rectangles
		 * @return the rectangle of the <see cref="IAtomContainer"/> after the shift
		 */
        public static double[] ShiftContainer(IAtomContainer container, double[] bounds, double[] last, double gap)
        {

            Trace.Assert(bounds.Length == 4);
            Trace.Assert(last.Length == 4);

            double boundsMinX = bounds[0];
            double boundsMinY = bounds[1];
            double boundsMaxX = bounds[2];
            double boundsMaxY = bounds[3];

            double lastMaxX = last[2];

            // determine if the containers are overlapping
            if (lastMaxX + gap >= boundsMinX)
            {
                double xShift = lastMaxX + gap - boundsMinX;
                Vector2 shift = new Vector2(xShift, 0);
                GeometryUtil.Translate2D(container, shift);
                return new double[] { boundsMinX + xShift, boundsMinY, boundsMaxX + xShift, boundsMaxY };
            }
            else
            {
                // the containers are not overlapping
                return bounds;
            }
        }

        /*
		 * Returns the average 2D bond length values of all products and reactants
		 * of the given reaction. The method uses {@link
		 * #GetBondLengthAverage(IAtomContainer)} internally.
		 * @param reaction The IReaction for which the average 2D bond length is
		 * calculated
		 * @return the average 2D bond length
		 * @see #GetBondLengthAverage(IAtomContainer)
		 */
        public static double GetBondLengthAverage(IReaction reaction)
        {
            double bondlenghtsum = 0.0;
            int containercount = 0;
            var containers = ReactionManipulator.GetAllAtomContainers(reaction);
            foreach (var container in containers)
            {
                containercount++;
                bondlenghtsum += GetBondLengthAverage(container);
            }
            return bondlenghtsum / containercount;
        }

        /**
		 * Calculate the median bond length of an atom container.
		 *
		 * @param container structure representation
		 * @return median bond length
		 * @.lang.ArgumentException unset coordinates or no bonds
		 */
        public static double GetBondLengthMedian(IAtomContainer container)
        {
            if (container.Bonds.Count == 0) throw new ArgumentException("Container has no bonds.");
            int nBonds = 0;
            double[] lengths = new double[container.Bonds.Count];
            for (int i = 0; i < container.Bonds.Count; i++)
            {
                IBond bond = container.Bonds[i];
                IAtom atom1 = bond.Atoms[0];
                IAtom atom2 = bond.Atoms[1];
                if (atom1.Point2D == null || atom2.Point2D == null)
                    throw new ArgumentException("An atom has no 2D coordinates.");
                Vector2 p1 = atom1.Point2D.Value;
                Vector2 p2 = atom2.Point2D.Value;
                if (p1.X != p2.X || p1.Y != p2.Y)

                    lengths[nBonds++] = Vector2.Distance(p1, p2);
            }
            Array.Sort(lengths, 0, nBonds);
            return lengths[nBonds / 2];
        }

        /**
		 * Determines if this model contains 3D coordinates for all atoms.
		 *
		 * @param chemModel the ChemModel to consider
		 * @return bool indication that 3D coordinates are available for all atoms.
		 */
        public static bool Has3DCoordinates(IChemModel chemModel)
        {
            var acs = ChemModelManipulator.GetAllAtomContainers(chemModel);
            foreach (var ac in acs)
            {
                if (!Has3DCoordinates(ac))
                {
                    return false;
                }
            }
            return true;
        }

        /**
		 * Shift the containers in a reaction vertically upwards to not overlap with the reference
		 * rectangle. The shift is such that the given gap is realized, but only if the reactions are
		 * actually overlapping. To avoid dependence on Java AWT, rectangles are described by
		 * arrays of double. Each rectangle is specified by {minX, minY, maxX, maxY}.
		 *
		 * @param reaction the reaction to shift
		 * @param bounds   the bounds of the reaction to shift
		 * @param last     the bounds of the last reaction
		 * @return the rectangle of the shifted reaction
		 */
        public static double[] ShiftReactionVertical(IReaction reaction, double[] bounds, double[] last, double gap)
        {
            Trace.Assert(bounds.Length == 4);
            Trace.Assert(last.Length == 4);

            double boundsMinX = bounds[0];
            double boundsMinY = bounds[1];
            double boundsMaxX = bounds[2];
            double boundsMaxY = bounds[3];

            double lastMinY = last[1];
            double lastMaxY = last[3];

            double boundsHeight = boundsMaxY - boundsMinY;
            double lastHeight = lastMaxY - lastMinY;

            // determine if the reactions are overlapping
            if (lastMaxY + gap >= boundsMinY)
            {
                double yShift = boundsHeight + lastHeight + gap;
                Vector2 shift = new Vector2(0, yShift);
                var containers = ReactionManipulator.GetAllAtomContainers(reaction);
                foreach (var container in containers)
                {
                    Translate2D(container, shift);
                }
                return new double[] { boundsMinX, boundsMinY + yShift, boundsMaxX, boundsMaxY + yShift };
            }
            else
            {
                // the reactions were not overlapping
                return bounds;
            }
        }
    }
}
