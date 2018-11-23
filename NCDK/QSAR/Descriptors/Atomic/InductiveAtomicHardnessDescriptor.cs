/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Hardware Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Hardware
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using NCDK.Config;
using NCDK.Numerics;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    /// Inductive atomic hardness of an atom in a polyatomic system can be defined
    /// as the "resistance" to a change of the atomic charge. Only works with 3D coordinates, which must be calculated beforehand. 
    /// </summary>
    // @author         mfe4
    // @cdk.created    2004-11-03
    // @cdk.module     qsaratomic
    // @cdk.dictref   qsar-descriptors:atomicHardness
    [DescriptorSpecification("http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#atomicHardness")]
    public partial class InductiveAtomicHardnessDescriptor : AbstractDescriptor, IAtomicDescriptor
    {
        IAtomContainer container;

        public InductiveAtomicHardnessDescriptor(IAtomContainer container)
        {
            this.container = container;
        }

        public AtomTypeFactory AtomTypeFactory { get; set; } = CDK.JmolAtomTypeFactory;

        [DescriptorResult]
        public class Result : AbstractDescriptorResult
        {
            public Result(double value)
            {
                this.AtomicHardness = value;
            }

            [DescriptorResultProperty("indAtomHardnesss")]
            public double AtomicHardness { get; private set; }

            public double Value => AtomicHardness;
        }

        /// <summary>
        /// It is needed to call the addExplicitHydrogensToSatisfyValency method from
        /// the class tools.HydrogenAdder, and 3D coordinates.
        /// </summary>
        /// <param name="atom">The <see cref="IAtom"/> for which the <see cref="Result"/> is requested</param>
        /// <returns>a double with polarizability of the heavy atom</returns>
        public Result Calculate(IAtom atom)
        {
            var symbol = atom.Symbol;
            var type = this.AtomTypeFactory.GetAtomType(symbol);
            var radiusTarget = type.CovalentRadius.Value;
            double atomicHardness = 0;
            foreach (var curAtom in container.Atoms)
            {
                if (atom.Point3D == null || curAtom.Point3D == null)
                    throw new CDKException("The target atom or current atom had no 3D coordinates. These are required");

                if (!atom.Equals(curAtom))
                {
                    double partial = 0;
                    symbol = curAtom.Symbol;

                    type = this.AtomTypeFactory.GetAtomType(symbol);
                    var radius = type.CovalentRadius.Value;
                    partial += radius * radius;
                    partial += (radiusTarget * radiusTarget);
                    partial = partial / CalculateSquareDistanceBetweenTwoAtoms(atom, curAtom);
                    atomicHardness += partial;
                }
            }

            atomicHardness = 2 * atomicHardness;
            atomicHardness = atomicHardness * 0.172;
            atomicHardness = 1 / atomicHardness;
            return new Result(atomicHardness);
        }

        private static double CalculateSquareDistanceBetweenTwoAtoms(IAtom atom1, IAtom atom2)
        {
            var firstPoint = atom1.Point3D.Value;
            var secondPoint = atom2.Point3D.Value;
            var tmp = Vector3.Distance(firstPoint, secondPoint);
            var distance = tmp * tmp;
            return distance;
        }

        IDescriptorResult IAtomicDescriptor.Calculate(IAtom atom) => Calculate(atom);
    }
}
