/* Copyright (C) 2004-2007  Rajarshi Guha <rajarshi@users.sourceforge.net>
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

using NCDK.Config;
using NCDK.Geometries;
using NCDK.QSAR.Results;
using System;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// IDescriptor characterizing the mass distribution of the molecule.
    /// </summary>
    /// <remarks>
    /// Described by Katritzky et al. <token>cdk-cite-KAT96</token>.
    /// For modelling purposes the value of the descriptor is calculated
    /// both with and without H atoms. Furthermore the square and cube roots
    /// of the descriptor are also generated as described by Wessel et al. <token>cdk-cite-WES98</token>.
    /// <para>
    /// The descriptor routine generates 9 descriptors:
    /// <list type="bullet"> 
    /// <item>GRAV-1 -  gravitational index of heavy atoms</item>
    /// <item>GRAV-2 -  square root of gravitational index of heavy atoms</item>
    /// <item>GRAV-3 -  cube root of gravitational index of heavy atoms</item>
    /// <item>GRAVH-1 -  gravitational index - hydrogens included</item>
    /// <item>GRAVH-2 -  square root of hydrogen-included gravitational index</item>
    /// <item>GRAVH-3 -  cube root of hydrogen-included gravitational index</item>
    /// <item>GRAV-4 -  grav1 for all pairs of atoms (not just bonded pairs)</item>
    /// <item>GRAV-5 -  grav2 for all pairs of atoms (not just bonded pairs)</item>
    /// <item>GRAV-6 -  grav3 for all pairs of atoms (not just bonded pairs)</item>
    /// </list>
    /// </para>
    /// <para>This descriptor uses these parameters:
    /// <list type="table">
    /// <item>
    /// <term>Name</term>
    /// <term>Default</term>
    /// <term>Description</term>
    /// </item>
    /// <item>
    /// <term></term>
    /// <term></term>
    /// <term>no parameters</term>
    /// </item>
    /// </list>
    /// </para>
    /// </remarks>
    // @author Rajarshi Guha
    // @cdk.created 2004-11-23
    // @cdk.module qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:gravitationalIndex
    // @cdk.keyword gravitational index
    // @cdk.keyword descriptor
    public class GravitationalIndexDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private struct Pair
        {
            public int X;
            public int Y;
        }

        private static readonly string[] NAMES = { "GRAV-1", "GRAV-2", "GRAV-3", "GRAVH-1", "GRAVH-2", "GRAVH-3", "GRAV-4", "GRAV-5", "GRAV-6" };

        public GravitationalIndexDescriptor() { }

        public override IImplementationSpecification Specification => specification;
        private static readonly DescriptorSpecification specification =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#gravitationalIndex",
                typeof(GravitationalIndexDescriptor).FullName,
                "The Chemistry Development Kit");

        public override IReadOnlyList<object> Parameters { get { return null; } set { } }
        public override IReadOnlyList<string> DescriptorNames => NAMES;
        public override IReadOnlyList<string> ParameterNames => null;
        public override object GetParameterType(string name) => null;

        private DescriptorValue<ArrayResult<double>> GetDummyDescriptorValue(Exception e)
        {
            var ndesc = DescriptorNames.Count;
            var results = new ArrayResult<double>(ndesc);
            for (int i = 0; i < ndesc; i++)
                results.Add(double.NaN);
            return new DescriptorValue<ArrayResult<double>>(specification, ParameterNames, Parameters, results, DescriptorNames, e);
        }

        /// <summary>
        /// Calculates the 9 gravitational indices.
        /// </summary>
        /// <param name="container">Parameter is the atom container.</param>
        /// <returns>An ArrayList containing 9 elements in the order described above</returns>
        public DescriptorValue<ArrayResult<double>> Calculate(IAtomContainer container)
        {
            if (!GeometryUtil.Has3DCoordinates(container))
                return GetDummyDescriptorValue(new CDKException("Molecule must have 3D coordinates"));

            var factory = CDK.IsotopeFactory;
            double mass1;
            double mass2;
            double sum = 0;
            foreach (var bond in container.Bonds)
            {
                if (bond.Atoms.Count != 2)
                {
                    return GetDummyDescriptorValue(new CDKException("GravitationalIndex: Only handles 2 center bonds"));
                }

                mass1 = factory.GetMajorIsotope(bond.Atoms[0].Symbol).MassNumber.Value;
                mass2 = factory.GetMajorIsotope(bond.Atoms[1].Symbol).MassNumber.Value;

                var p1 = bond.Atoms[0].Point3D.Value;
                var p2 = bond.Atoms[1].Point3D.Value;

                var x1 = p1.X;
                var y1 = p1.Y;
                var z1 = p1.Z;
                var x2 = p2.X;
                var y2 = p2.Y;
                var z2 = p2.Z;

                var dist = (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) + (z1 - z2) * (z1 - z2);
                sum += (mass1 * mass2) / dist;
            }

            // heavy atoms only
            double heavysum = 0;
            foreach (var b in container.Bonds)
            {
                if (b.Atoms.Count != 2)
                {
                    return GetDummyDescriptorValue(new CDKException("GravitationalIndex: Only handles 2 center bonds"));
                }

                if (b.Atoms[0].AtomicNumber.Equals(NaturalElements.H.AtomicNumber) || b.Atoms[1].AtomicNumber.Equals(NaturalElements.H.AtomicNumber))
                    continue;

                mass1 = factory.GetMajorIsotope(b.Atoms[0].Symbol).MassNumber.Value;
                mass2 = factory.GetMajorIsotope(b.Atoms[1].Symbol).MassNumber.Value;

                var point0 = b.Atoms[0].Point3D.Value;
                var point1 = b.Atoms[1].Point3D.Value;

                var x1 = point0.X;
                var y1 = point0.Y;
                var z1 = point0.Z;
                var x2 = point1.X;
                var y2 = point1.Y;
                var z2 = point1.Z;

                var dist = (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) + (z1 - z2) * (z1 - z2);
                heavysum += (mass1 * mass2) / dist;
            }

            // all pairs
            var x = new List<int>();
            for (int i = 0; i < container.Atoms.Count; i++)
            {
                if (!container.Atoms[i].AtomicNumber.Equals(NaturalElements.H.AtomicNumber))
                    x.Add(i);
            }
            var npair = x.Count * (x.Count - 1) / 2;
            var p = new Pair[npair];
            for (int i = 0; i < npair; i++)
                p[i] = new Pair();
            int pcount = 0;
            for (int i = 0; i < x.Count - 1; i++)
            {
                for (int j = i + 1; j < x.Count; j++)
                {
                    int present = 0;
                    var a = x[i];
                    var b = x[j];
                    for (int k = 0; k < pcount; k++)
                    {
                        if ((p[k].X == a && p[k].Y == b) || (p[k].Y == a && p[k].X == b))
                            present = 1;
                    }
                    if (present == 1)
                        continue;
                    p[pcount].X = a;
                    p[pcount].Y = b;
                    pcount += 1;
                }
            }
            double allheavysum = 0;
            foreach (var aP in p)
            {
                var atomNumber1 = aP.X;
                var atomNumber2 = aP.Y;

                mass1 = factory.GetMajorIsotope(container.Atoms[atomNumber1].Symbol).MassNumber.Value;
                mass2 = factory.GetMajorIsotope(container.Atoms[atomNumber2].Symbol).MassNumber.Value;

                var x1 = container.Atoms[atomNumber1].Point3D.Value.X;
                var y1 = container.Atoms[atomNumber1].Point3D.Value.Y;
                var z1 = container.Atoms[atomNumber1].Point3D.Value.Z;
                var x2 = container.Atoms[atomNumber2].Point3D.Value.X;
                var y2 = container.Atoms[atomNumber2].Point3D.Value.Y;
                var z2 = container.Atoms[atomNumber2].Point3D.Value.Z;

                var dist = (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) + (z1 - z2) * (z1 - z2);
                allheavysum += (mass1 * mass2) / dist;
            }

            var retval = new ArrayResult<double>(9)
            {
                heavysum,
                Math.Sqrt(heavysum),
                Math.Pow(heavysum, 1.0 / 3.0),

                sum,
                Math.Sqrt(sum),
                Math.Pow(sum, 1.0 / 3.0),

                allheavysum,
                Math.Sqrt(allheavysum),
                Math.Pow(allheavysum, 1.0 / 3.0)
            };

            return new DescriptorValue<ArrayResult<double>>(specification, ParameterNames, Parameters, retval, DescriptorNames);
        }

        /// <inheritdoc/>
        public override IDescriptorResult DescriptorResultType { get; } = new ArrayResult<double>(9);

        IDescriptorValue IMolecularDescriptor.Calculate(IAtomContainer container) => Calculate(container);
    }
}
