/*
 *  Copyright (C) 2004-2007  Rajarshi Guha <rajarshi@users.sourceforge.net>
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
using NCDK.Common.Collections;
using NCDK.Numerics;
using NCDK.Geometries;
using NCDK.Graphs;
using NCDK.QSAR.Result;
using NCDK.Tools.Manipulator;
using System;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Evaluates the Petitjean shape indices,
    /// <p/>
    /// These original Petitjean number was described by Petitjean ({@cdk.cite PET92})
    /// and considered the molecular graph. This class also implements the geometric analog
    /// of the topological shape index described by Bath et al ({@cdk.cite BAT95}).
    /// <p/>
    /// The descriptor returns a <code>DoubleArrayResult</code> which contains
    /// <ol>
    /// <li>topoShape - topological shape index
    /// <li>geomShape - geometric shape index
    /// </ol>
    ///
    /// <p>This descriptor uses these parameters:
    /// <table border="1">
    ///   <tr>
    ///     <td>Name</td>
    ///     <td>Default</td>
    ///     <td>Description</td>
    ///   </tr>
    ///   <tr>
    ///     <td></td>
    ///     <td></td>
    ///     <td>no parameters</td>
    ///   </tr>
    /// </table>
    ///
    ///
    // @author      Rajarshi Guha
    // @cdk.created 2006-01-14
    // @cdk.module  qsarmolecular
    // @cdk.githash
    // @cdk.set     qsar-descriptors
    // @cdk.dictref qsar-descriptors:petitjeanShapeIndex
    // @cdk.keyword Petit-Jean, shape index
    /// </summary>
    public class PetitjeanShapeIndexDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private static readonly string[] NAMES = { "topoShape", "geomShape" };

        public PetitjeanShapeIndexDescriptor() { }

        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#petitjeanShapeIndex",
                typeof(PetitjeanShapeIndexDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// Sets the parameters attribute of the PetitjeanShapeIndexDescriptor object.
        /// </summary>
        public override object[] Parameters { get { return null; } set { } }

        public override string[] DescriptorNames => NAMES;

        /// <summary>
        /// The parameterNames attribute of the PetitjeanShapeIndexDescriptor object.
        /// </summary>
        public override string[] ParameterNames => null;

        /// <summary>
        /// Gets the parameterType attribute of the PetitjeanShapeIndexDescriptor object.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public override object GetParameterType(string name) => null;

        /// <summary>
        /// Calculates the two Petitjean shape indices.
        /// </summary>
        /// <param name="container">Parameter is the atom container.</param>
        /// <returns>A DoubleArrayResult value representing the Petitjean shape indices</returns>
        public override DescriptorValue Calculate(IAtomContainer container)
        {
            IAtomContainer local = AtomContainerManipulator.RemoveHydrogens(container);

            int tradius = PathTools.GetMolecularGraphRadius(local);
            int tdiameter = PathTools.GetMolecularGraphDiameter(local);

            DoubleArrayResult retval = new DoubleArrayResult();
            retval.Add((double)(tdiameter - tradius) / (double)tradius);

            // get the 3D distance matrix
            if (GeometryUtil.Has3DCoordinates(container))
            {
                int natom = container.Atoms.Count;
                double[][] distanceMatrix = Arrays.CreateJagged<double>(natom, natom);
                for (int i = 0; i < natom; i++)
                {
                    for (int j = 0; j < natom; j++)
                    {
                        if (i == j)
                        {
                            distanceMatrix[i][j] = 0.0;
                            continue;
                        }

                        Vector3 a = container.Atoms[i].Point3D.Value;
                        Vector3 b = container.Atoms[j].Point3D.Value;
                        distanceMatrix[i][j] = Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y)
                                + (a.Z - b.Z) * (a.Z - b.Z));
                    }
                }
                double gradius = 999999;
                double gdiameter = -999999;
                double[] geta = new double[natom];
                for (int i = 0; i < natom; i++)
                {
                    double max = -99999;
                    for (int j = 0; j < natom; j++)
                    {
                        if (distanceMatrix[i][j] > max) max = distanceMatrix[i][j];
                    }
                    geta[i] = max;
                }
                for (int i = 0; i < natom; i++)
                {
                    if (geta[i] < gradius) gradius = geta[i];
                    if (geta[i] > gdiameter) gdiameter = geta[i];
                }
                retval.Add((gdiameter - gradius) / gradius);
            }
            else
            {
                retval.Add(double.NaN);
            }

            return new DescriptorValue(_Specification, ParameterNames, Parameters, retval,
                    DescriptorNames);
        }

        /// <summary>
        /// Returns the specific type of the DescriptorResult object.
        /// <p/>
        /// The return value from this method really indicates what type of result will
        /// be obtained from the <see cref="DescriptorValue"/> object. Note that the same result
        /// can be achieved by interrogating the <see cref="DescriptorValue"/> object; this method
        /// allows you to do the same thing, without actually calculating the descriptor.
        /// </summary>
        public override IDescriptorResult DescriptorResultType { get; } = new DoubleArrayResultType(2);
    }
}

