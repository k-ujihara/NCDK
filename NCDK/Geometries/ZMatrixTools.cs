/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *  */
using NCDK.Common.Mathematics;
using System;
using NCDK.Numerics;

namespace NCDK.Geometries
{
    /// <summary>
    /// A set of static utility classes for dealing with Z matrices.
    /// </summary>
    // @cdk.module  io
    // @cdk.githash
    // @cdk.keyword Z-matrix
    // @cdk.created 2004-02-09
    public class ZMatrixTools
    {
        /// <summary>
        /// Takes the given Z Matrix coordinates and converts them to cartesian coordinates.
        /// The first Atom end up in the origin, the second on on the x axis, and the third
        /// one in the XY plane. The rest is added by applying the Zmatrix distances, angles
        /// and dihedrals. Angles are in degrees.
        /// </summary>
        /// <param name="distances">Array of distance variables of the Z matrix</param>
        /// <param name="first_atoms">Array of angle variables of the Z matrix</param>
        /// <param name="angles">Array of distance variables of the Z matrix</param>
        /// <param name="second_atoms">Array of atom ids of the first invoked atom in distance, angle and dihedral</param>
        /// <param name="dihedrals">Array of atom ids of the second invoked atom in angle and dihedral</param>
        /// <param name="third_atoms">Array of atom ids of the third invoked atom in dihedral</param>
        /// <returns>The cartesian coordinates</returns>
       // @cdk.dictref blue-obelisk:zmatrixCoordinatesIntoCartesianCoordinates
        public static Vector3[] ZMatrixToCartesian(double[] distances, int[] first_atoms, double[] angles,
                int[] second_atoms, double[] dihedrals, int[] third_atoms)
        {
            Vector3[] cartesianCoords = new Vector3[distances.Length];
            for (int index = 0; index < distances.Length; index++)
            {
                if (index == 0)
                {
                    cartesianCoords[index] = Vector3.Zero;
                }
                else if (index == 1)
                {
                    cartesianCoords[index] = new Vector3(distances[1], 0, 0);
                }
                else if (index == 2)
                {
                    cartesianCoords[index] = new Vector3(
                        -Math.Cos((angles[2] / 180) * Math.PI) * distances[2] + distances[1], 
                        Math.Sin((angles[2] / 180) * Math.PI) * distances[2], 
                        0);
                    if (first_atoms[index] == 0)
                        cartesianCoords[index].X = (cartesianCoords[index].X - distances[1]) * -1;
                }
                else
                {
                    Vector3 cd = cartesianCoords[third_atoms[index]] - cartesianCoords[second_atoms[index]];
                    Vector3 bc = cartesianCoords[second_atoms[index]] - cartesianCoords[first_atoms[index]];
                    Vector3 n1 = Vector3.Cross(cd, bc);
                    Vector3 n2 = Rotate(n1, bc, -dihedrals[index]);
                    Vector3 ba = Rotate(bc, n2, -angles[index]);
                    ba = Vector3.Normalize(ba);
                    ba *= distances[index];
                    Vector3 result = cartesianCoords[first_atoms[index]] + ba;
                    cartesianCoords[index] = result;
                }
            }
            return cartesianCoords;
        }

        private static Vector3 Rotate(Vector3 vector, Vector3 axis, double angle)
        {
            var rotate = Vectors.NewQuaternionFromAxisAngle(axis, Vectors.DegreeToRadian(angle));
            var result = Vector3.Transform(vector, rotate);
            return result;
        }
    }
}

