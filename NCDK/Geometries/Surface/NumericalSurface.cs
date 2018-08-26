/*
 * Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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

using NCDK.Common.Collections;
using NCDK.Numerics;
using NCDK.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NCDK.Geometries.Surface
{
    /// <summary>
    /// A class representing the solvent accessible surface area surface of a molecule.
    /// </summary>
    /// <remarks>
    /// <para>This class is based on the Python implementation of the DCLM method
    /// (<token>cdk-cite-EIS95</token>) by Peter McCluskey, which is a non-analytical method to generate a set of points
    /// representing the solvent accessible surface area of a molecule.
    /// </para>
    /// <para>
    /// The neighbor list is a simplified version of that
    /// described in <token>cdk-cite-EIS95</token> and as a result, the surface areas of the atoms may not be exact
    /// (compared to analytical calculations). The tessellation is slightly different from
    /// that described by McCluskey and uses recursive subdivision starting from an icosahedral
    /// representation.
    /// </para>
    /// <para>
    /// The default solvent radius used is 1.4A and setting this to 0 will give the
    /// van der Waals surface. The accuracy can be increased by increasing the tessellation
    /// level, though the default of 4 is a good balance between accuracy and speed.
    /// </para>
    /// </remarks>
    // @author      Rajarshi Guha
    // @cdk.created 2005-05-08
    // @cdk.module  qsarmolecular
    // @cdk.githash
    // @cdk.bug     1846421
    public class NumericalSurface
    {
        readonly double solventRadius = 1.4;
        readonly int tesslevel = 4;
        IAtom[] atoms;
        List<Vector3>[] surfPoints;
        double[] areas;
        double[] volumes;

        /// <summary>
        /// Constructor to initialize the surface calculation with default values.
        /// This constructor use the van der Waals radii as defined in <i>NCDK.Config.Data.jmol_atomtypes.txt</i>
        /// of the source distribution. Also uses a tesselation level of 4 and solvent radius of 1.4A.
        /// </summary>
        /// <param name="atomContainer">The <see cref="IAtomContainer"/> for which the surface is to be calculated</param>
        public NumericalSurface(IAtomContainer atomContainer)
        {
            this.atoms = atomContainer.Atoms.ToArray();
        }

        /// <summary>
        /// Constructor to initialize the surface calculation with user specified values.
        /// This constructor use the van der Waals radii as defined in <i>NCDK.Config.Data.jmol_atomtypes.txt</i>
        /// of the source distribution
        /// </summary>
        /// <param name="atomContainer">The <see cref="IAtomContainer"/> for which the surface is to be calculated</param>
        /// <param name="solventRadius">The radius of a solvent molecule that is used to extend
        /// the radius of each atom. Setting to 0 gives the van der Waals surface</param>
        /// <param name="tesslevel">The number of levels that the subdivision algorithm for tessellation should use</param>
        public NumericalSurface(IAtomContainer atomContainer, double solventRadius, int tesslevel)
        {
            this.solventRadius = solventRadius;
            this.atoms = atomContainer.Atoms.ToArray();
            this.tesslevel = tesslevel;
        }

        /// <summary>
        /// Evaluate the surface.
        /// </summary>
        /// <remarks>
        /// This method generates the points on the accessible surface area of each atom
        /// as well as calculating the surface area of each atom
        /// </remarks>
        public void CalculateSurface()
        {
            // invariants
            foreach (var atom in atoms)
            {
                if (atom.Point3D == null)
                    throw new ArgumentException("One or more atoms had no 3D coordinate set");
            }

            // get r_f and geometric center
            Vector3 cp = new Vector3(0, 0, 0);
            double maxRadius = 0;
            for (int i = 0; i < atoms.Length; i++)
            {
                double vdwr = PeriodicTable.GetVdwRadius(atoms[i].Symbol).Value;
                if (vdwr + solventRadius > maxRadius)
                    maxRadius = PeriodicTable.GetVdwRadius(atoms[i].Symbol).Value + solventRadius;

                cp.X = cp.X + atoms[i].Point3D.Value.X;
                cp.Y = cp.Y + atoms[i].Point3D.Value.Y;
                cp.Z = cp.Z + atoms[i].Point3D.Value.Z;
            }
            cp.X = cp.X / atoms.Length;
            cp.Y = cp.Y / atoms.Length;
            cp.Z = cp.Z / atoms.Length;

            // do the tesselation
            Tessellate tess = new Tessellate("ico", tesslevel);
            tess.DoTessellate();
            Trace.TraceInformation("Got tesselation, number of triangles = " + tess.GetNumberOfTriangles());

            // get neighbor list
            NeighborList nbrlist = new NeighborList(atoms, maxRadius + solventRadius);
            Trace.TraceInformation("Got neighbor list");

            // loop over atoms and get surface points
            this.surfPoints = new List<Vector3>[atoms.Length];
            this.areas = new double[atoms.Length];
            this.volumes = new double[atoms.Length];

            for (int i = 0; i < atoms.Length; i++)
            {
                int pointDensity = tess.GetNumberOfTriangles() * 3;
                Vector3[][] points = AtomicSurfacePoints(nbrlist, i, atoms[i], tess);
                TranslatePoints(i, points, pointDensity, atoms[i], cp);
            }
            Trace.TraceInformation("Obtained points, areas and volumes");
        }

        /// <summary>
        /// Get an array of all the points on the molecular surface.
        /// </summary>
        /// <remarks>
        /// This returns an array of Vector3 objects representing all the points
        /// on the molecular surface
        /// </remarks>
        /// <returns>An array of Vector3 objects</returns>
        public Vector3[] GetAllSurfacePoints()
        {
            int npt = 0;
            for (int i = 0; i < this.surfPoints.Length; i++)
                npt += this.surfPoints[i].Count;
            Vector3[] ret = new Vector3[npt];
            int j = 0;
            for (int i = 0; i < this.surfPoints.Length; i++)
            {
                List<Vector3> arl = this.surfPoints[i];
                foreach (var v in arl)
                {
                    ret[j] = v;
                    j++;
                }
            }
            return (ret);
        }

        /// <summary>
        /// Get an array of the points on the accessible surface of a specific atom.
        /// </summary>
        /// <param name="atomIdx">The index of the atom. Ranges from 0 to n-1, where n is the</param>
        /// number of atoms in the AtomContainer that the surface was calculated for
        /// <returns>An array of Vector3 objects</returns>
        /// <exception cref="CDKException">if the atom index is outside the range of allowable indices</exception>
        public Vector3[] GetSurfacePoints(int atomIdx)
        {
            if (atomIdx >= this.surfPoints.Length)
            {
                throw new CDKException("Atom index was out of bounds");
            }
            List<Vector3> arl = this.surfPoints[atomIdx];
            Vector3[] ret = new Vector3[arl.Count];
            for (int i = 0; i < arl.Count; i++)
                ret[i] = arl[i];
            return (ret);
        }

        /// <summary>
        /// Get the surface area for the specified atom.
        /// </summary>
        /// <param name="atomIdx">The index of the atom. Ranges from 0 to n-1, where n is the
        /// number of atoms in the AtomContainer that the surface was calculated for</param>
        /// <returns>A double representing the accessible surface area of the atom</returns>
        /// <exception cref="CDKException">if the atom index is outside the range of allowable indices</exception>
        public double GetSurfaceArea(int atomIdx)
        {
            if (atomIdx >= this.surfPoints.Length)
            {
                throw new CDKException("Atom index was out of bounds");
            }
            return (this.areas[atomIdx]);
        }

        /// <summary>
        /// Get an array containing the accessible surface area for each atom.
        /// </summary>
        /// <returns>An array of double giving the surface areas of all the atoms</returns>
        public double[] GetAllSurfaceAreas()
        {
            return (this.areas);
        }

        /// <summary>
        /// Get the total surface area for the AtomContainer.
        /// </summary>
        /// <returns>A double containing the total surface area of the AtomContainer for
        /// which the surface was calculated for</returns>
        public double GetTotalSurfaceArea()
        {
            double ta = 0.0;
            for (int i = 0; i < this.areas.Length; i++)
                ta += this.areas[i];
            return (ta);
        }

        private void TranslatePoints(int atmIdx, Vector3[][] points, int pointDensity, IAtom atom, Vector3 cp)
        {
            double totalRadius = PeriodicTable.GetVdwRadius(atom.Symbol).Value + solventRadius;

            double area = 4 * Math.PI * (totalRadius * totalRadius) * points.Length / pointDensity;

            double sumx = 0.0;
            double sumy = 0.0;
            double sumz = 0.0;
            for (int i = 0; i < points.Length; i++)
            {
                Vector3 p = points[i][1];
                sumx += p.X;
                sumy += p.Y;
                sumz += p.Z;
            }
            double vconst = 4.0 / 3.0 * Math.PI / (double)pointDensity;
            double dotp1 = (atom.Point3D.Value.X - cp.X) * sumx + (atom.Point3D.Value.Y - cp.Y) * sumy
                    + (atom.Point3D.Value.Z - cp.Z) * sumz;
            double volume = vconst * (totalRadius * totalRadius) * dotp1 + (totalRadius * totalRadius * totalRadius) * points.Length;

            this.areas[atmIdx] = area;
            this.volumes[atmIdx] = volume;

            List<Vector3> tmp = new List<Vector3>();
            for (int i = 0; i < points.Length; i++)
                tmp.Add(points[i][0]);
            this.surfPoints[atmIdx] = tmp;
        }

        private Vector3[][] AtomicSurfacePoints(NeighborList nbrlist, int currAtomIdx, IAtom atom, Tessellate tess)
        {
            double totalRadius = PeriodicTable.GetVdwRadius(atom.Symbol).Value + solventRadius;
            double totalRadius2 = totalRadius * totalRadius;
            double twiceTotalRadius = 2 * totalRadius;

            int[] nlist = nbrlist.GetNeighbors(currAtomIdx);
            double[][] data = Arrays.CreateJagged<double>(nlist.Length, 4);
            for (int i = 0; i < nlist.Length; i++)
            {
                double x12 = atoms[nlist[i]].Point3D.Value.X - atom.Point3D.Value.X;
                double y12 = atoms[nlist[i]].Point3D.Value.Y - atom.Point3D.Value.Y;
                double z12 = atoms[nlist[i]].Point3D.Value.Z - atom.Point3D.Value.Z;

                double d2 = x12 * x12 + y12 * y12 + z12 * z12;
                double tmp = PeriodicTable.GetVdwRadius(atoms[nlist[i]].Symbol).Value + solventRadius;
                tmp = tmp * tmp;
                double thresh = (d2 + totalRadius2 - tmp) / twiceTotalRadius;

                data[i][0] = x12;
                data[i][1] = y12;
                data[i][2] = z12;
                data[i][3] = thresh;
            }

            Vector3[] tessPoints = tess.GetTessAsPoint3ds();
            List<Vector3[]> points = new List<Vector3[]>();
            for (int i = 0; i < tessPoints.Length; i++)
            {
                Vector3 pt = tessPoints[i];
                bool buried = false;
                for (int j = 0; j < data.Length; j++)
                {
                    if (data[j][0] * pt.X + data[j][1] * pt.Y + data[j][2] * pt.Z > data[j][3])
                    {
                        buried = true;
                        break;
                    }
                }
                if (buried == false)
                {
                    Vector3[] tmp = new Vector3[2];
                    tmp[0] = new Vector3(totalRadius * pt.X + atom.Point3D.Value.X, totalRadius * pt.Y
                            + atom.Point3D.Value.Y, totalRadius * pt.Z + atom.Point3D.Value.Z);
                    tmp[1] = pt;
                    points.Add(tmp);
                }
            }

            // the first column contains the transformed points
            // and the second column contains the points from the
            // original unit tesselation
            Vector3[][] ret = Arrays.CreateJagged<Vector3>(points.Count, 2);
            for (int i = 0; i < points.Count; i++)
            {
                Vector3[] tmp = points[i];
                ret[i][0] = tmp[0];
                ret[i][1] = tmp[1];
            }
            return (ret);
        }
    }
}
