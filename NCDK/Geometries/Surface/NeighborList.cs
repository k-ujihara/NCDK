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

using System;
using System.Collections.Generic;

namespace NCDK.Geometries.Surface
{
    /// <summary>
    /// Creates a list of atoms neighboring each atom in the molecule.
    /// </summary>
    /// <remarks>
    /// The routine is a simplified version of the neighbor list described
    /// in {@cdk.cite EIS95} and is based on the implementation by Peter McCluskey.
    /// Due to the fact that it divides the cube into a fixed number of sub cubes,
    /// some accuracy may be lost.
    /// </remarks>
    // @author Rajarshi Guha
    // @cdk.created 2005-05-09
    // @cdk.module  qsarmolecular
    // @cdk.githash
    public class NeighborList
    {
        Dictionary<string, List<int>> boxes;
        double boxSize;
        IAtom[] atoms;

        public NeighborList(IAtom[] atoms, double radius)
        {
            this.atoms = atoms;
            this.boxes = new Dictionary<string, List<int>>();
            this.boxSize = 2 * radius;
            for (int i = 0; i < atoms.Length; i++)
            {
                string key = GetKeyString(atoms[i]);

                if (this.boxes.ContainsKey(key))
                {
                    List<int> arl = this.boxes[key];
                    arl.Add(i);
                    this.boxes[key] = arl;
                }
                else
                {
                    this.boxes[key] = new List<int>();
                }
            }
        }

        private string GetKeyString(IAtom atom)
        {
            double x = atom.Point3D.Value.X;
            double y = atom.Point3D.Value.Y;
            double z = atom.Point3D.Value.Z;

            int k1, k2, k3;
            k1 = (int)(Math.Floor(x / boxSize));
            k2 = (int)(Math.Floor(y / boxSize));
            k3 = (int)(Math.Floor(z / boxSize));

            string key = k1.ToString() + " " + k2.ToString() + " " + k3.ToString() + " ";
            return (key);
        }

        private int[] GetKeyArray(IAtom atom)
        {
            double x = atom.Point3D.Value.X;
            double y = atom.Point3D.Value.Y;
            double z = atom.Point3D.Value.Z;

            int k1, k2, k3;
            k1 = (int)(Math.Floor(x / boxSize));
            k2 = (int)(Math.Floor(y / boxSize));
            k3 = (int)(Math.Floor(z / boxSize));

            int[] ret = { k1, k2, k3 };
            return (ret);
        }

        public int GetNumberOfNeighbors(int i)
        {
            return GetNeighbors(i).Length;
        }

        public int[] GetNeighbors(int ii)
        {
            double maxDist2 = this.boxSize * this.boxSize;

            IAtom ai = this.atoms[ii];
            int[] key = GetKeyArray(ai);
            List<int> nlist = new List<int>();

            int[] bval = { -1, 0, 1 };
            for (int i = 0; i < bval.Length; i++)
            {
                int x = bval[i];
                for (int j = 0; j < bval.Length; j++)
                {
                    int y = bval[j];
                    for (int k = 0; k < bval.Length; k++)
                    {
                        int z = bval[k];

                        string keyj = (key[0] + x).ToString() + " " + (key[1] + y).ToString() + " " + (key[2] + z).ToString() + " ";
                        if (boxes.ContainsKey(keyj))
                        {
                            var nbrs = boxes[keyj];
                            for (int l = 0; l < nbrs.Count; l++)
                            {
                                int i2 = nbrs[l];
                                if (i2 != ii)
                                {
                                    IAtom aj = atoms[i2];
                                    double x12 = aj.Point3D.Value.X - ai.Point3D.Value.X;
                                    double y12 = aj.Point3D.Value.Y - ai.Point3D.Value.Y;
                                    double z12 = aj.Point3D.Value.Z - ai.Point3D.Value.Z;
                                    double d2 = x12 * x12 + y12 * y12 + z12 * z12;
                                    if (d2 < maxDist2) nlist.Add(i2);
                                }
                            }
                        }
                    }
                }
            }
            var tmp = nlist.ToArray();
            int[] ret = new int[tmp.Length];
            for (int j = 0; j < tmp.Length; j++)
                ret[j] = tmp[j];
            return (ret);
        }
    }
}
