/* Copyright (C) 2007  Rajarshi Guha <rajarshi@users.sf.net>
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
 */
using NCDK.Common.Collections;
using NCDK.Numerics;
using MathNet.Numerics.LinearAlgebra;
using NCDK.Geometries;
using NCDK.QSAR.Result;
using NCDK.Tools;
using System;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Evaluates length over breadth descriptors.
    /// <p/>
    /// The current implementation reproduces the results obtained from the LOVERB descriptor
    /// routine in ADAPT. As a result ti does not perform any orientation and only considers the
    /// X & Y extents for a series of rotations about the Z axis (in 10 degree increments).
    /// <p/>
    /// The class gives two descriptors
    /// <ul>
    /// <li>LOBMAX - The maximum L/B ratio
    /// <li>LOBMIN - The L/B ratio for the rotation that results in the minimum area
    /// (defined by the product of the X & Y extents for that orientation)
    /// </ul>
    /// <B>Note:</B> The descriptor assumes that the atoms have been configured.
    ///
    // @author      Rajarshi Guha
    // @cdk.created 2006-09-26
    // @cdk.module  qsarmolecular
    // @cdk.githash
    // @cdk.set     qsar-descriptors
    // @cdk.dictref qsar-descriptors:lengthOverBreadth
    /// </summary>
    public class LengthOverBreadthDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private static readonly string[] NAMES = { "LOBMAX", "LOBMIN" };

        /// <summary>
        /// Constructor for the LengthOverBreadthDescriptor object.
        /// </summary>
        public LengthOverBreadthDescriptor() { }

        /// <summary>
        /// The specification attribute of the PetitjeanNumberDescriptor object
        /// </summary>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
         new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#lengthOverBreadth",
                typeof(LengthOverBreadthDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <summary>
        /// Sets the parameters attribute of the PetitjeanNumberDescriptor object
        /// </summary>
        /// <param name="params">The new parameters value</param>
        /// <exception cref="CDKException"></exception>
        public override object[] Parameters { get { return null; } set { } }

        public override string[] DescriptorNames => NAMES;

        private DescriptorValue GetDummyDescriptorValue(Exception e)
        {
            DoubleArrayResult result = new DoubleArrayResult(2);
            result.Add(double.NaN);
            result.Add(double.NaN);
            return new DescriptorValue(_Specification, ParameterNames, Parameters, result, DescriptorNames, e);
        }

        /// <summary>
        /// Evaluate the descriptor for the molecule.
        /// </summary>
        /// <param name="atomContainer">AtomContainer</param>
        /// <returns>A <see cref="DoubleArrayResult"/> containing LOBMAX and LOBMIN in that order</returns>
        public override DescriptorValue Calculate(IAtomContainer atomContainer)
        {
            if (!GeometryUtil.Has3DCoordinates(atomContainer))
                return GetDummyDescriptorValue(new CDKException("Molecule must have 3D coordinates"));

            double angle = 10.0;
            double maxLOB = 0;
            double minArea = 1e6;
            double mmLOB = 0;

            double lob, bol, area;
            double[] xyzRanges;

            double[][] coords = Arrays.CreateJagged<double>(atomContainer.Atoms.Count, 3);
            for (int i = 0; i < atomContainer.Atoms.Count; i++)
            {
                var p = atomContainer.Atoms[i].Point3D.Value;
                coords[i][0] = p.X;
                coords[i][1] = p.Y;
                coords[i][2] = p.Z;
            }

            // get the com
            Vector3? acom = GeometryUtil.Get3DCentreOfMass(atomContainer);
            if (acom == null)
                return GetDummyDescriptorValue(new CDKException("Error in center of mass calculation, has exact mass been set on all atoms?"));
            var com = acom.Value;

            // translate everything to COM
            for (int i = 0; i < coords.Length; i++)
            {
                coords[i][0] -= com.X;
                coords[i][1] -= com.Y;
                coords[i][2] -= com.Z;
            }

            int nangle = (int)(90 / angle);
            for (int i = 0; i < nangle; i++)
            {
                RotateZ(coords, Math.PI / 180.0 * angle);
                try
                {
                    xyzRanges = Extents(atomContainer, coords, true);
                }
                catch (CDKException e)
                {
                    return GetDummyDescriptorValue(e);
                }
                lob = xyzRanges[0] / xyzRanges[1];
                bol = 1.0 / lob;
                if (lob < bol)
                {
                    double tmp = lob;
                    lob = bol;
                    bol = tmp;
                }
                area = xyzRanges[0] * xyzRanges[1];
                if (lob > maxLOB) maxLOB = lob;
                if (area < minArea)
                {
                    minArea = area;
                    mmLOB = lob;
                }
            }

            DoubleArrayResult result = new DoubleArrayResult(2);
            result.Add(maxLOB);
            result.Add(mmLOB);

            return new DescriptorValue(_Specification, ParameterNames, Parameters, result, DescriptorNames);
        }

        /// <summary>
        /// The specific type of the DescriptorResult object.
        /// <p/>
        /// The return value from this method really indicates what type of result will
        /// be obtained from the <see cref="DescriptorValue"/> object. Note that the same result
        /// can be achieved by interrogating the <see cref="DescriptorValue"/> object; this method
        /// allows you to do the same thing, without actually calculating the descriptor.
        /// </summary>
        public override IDescriptorResult DescriptorResultType { get; } = new DoubleArrayResultType(2);

        private void RotateZ(double[][] coords, double theta)
        {
            int natom = coords.Length;
            double[][] m;
            m = Arrays.CreateJagged<double>(4, 4);
            m[0][0] = Math.Cos(theta);
            m[0][1] = Math.Sin(theta);
            m[1][0] = -1 * Math.Sin(theta);
            m[1][1] = Math.Cos(theta);
            m[2][2] = 1;
            m[3][3] = 1;
            var rZ = Matrix<double>.Build.DenseOfColumnArrays(m);
            m = Arrays.CreateJagged<double>(4, natom);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < natom; j++)
                {
                    m[i][j] = coords[j][i];
                    m[3][j] = 1;
                }
            }
            var newCoord = Matrix<double>.Build.DenseOfColumnArrays(m);
            newCoord = newCoord * rZ;
            for (int i = 0; i < natom; i++)
            {
                for (int j = 0; j < 3; j++)
                    coords[i][j] = newCoord[i, j];
            }
        }

        private double[] Extents(IAtomContainer atomContainer, double[][] coords, bool withRadii)
        {
            double xmax = -1e30;
            double ymax = -1e30;
            double zmax = -1e30;

            double xmin = 1e30;
            double ymin = 1e30;
            double zmin = 1e30;

            int natom = atomContainer.Atoms.Count;
            for (int i = 0; i < natom; i++)
            {
                double[] coord = new double[coords[0].Length];
                Array.Copy(coords[i], 0, coord, 0, coords[0].Length);
                if (withRadii)
                {
                    IAtom atom = atomContainer.Atoms[i];
                    double radius = PeriodicTable.GetCovalentRadius(atom.Symbol).Value;
                    xmax = Math.Max(xmax, coord[0] + radius);
                    ymax = Math.Max(ymax, coord[1] + radius);
                    zmax = Math.Max(zmax, coord[2] + radius);

                    xmin = Math.Min(xmin, coord[0] - radius);
                    ymin = Math.Min(ymin, coord[1] - radius);
                    zmin = Math.Min(zmin, coord[2] - radius);
                }
                else
                {
                    xmax = Math.Max(xmax, coord[0]);
                    ymax = Math.Max(ymax, coord[1]);
                    zmax = Math.Max(zmax, coord[2]);

                    xmin = Math.Min(xmin, coord[0]);
                    ymin = Math.Min(ymin, coord[1]);
                    zmin = Math.Min(zmin, coord[2]);
                }
            }
            double[] ranges = new double[3];
            ranges[0] = xmax - xmin;
            ranges[1] = ymax - ymin;
            ranges[2] = zmax - zmin;
            return ranges;
        }

        /// <summary>
        /// The parameterNames attribute of the PetitjeanNumberDescriptor object
        /// </summary>
        public override string[] ParameterNames => null;

        /// <summary>
        /// Gets the parameterType attribute of the PetitjeanNumberDescriptor object
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public override object GetParameterType(string name) => null;
    }
}
