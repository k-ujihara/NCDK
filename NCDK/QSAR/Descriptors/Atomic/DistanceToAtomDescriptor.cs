/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.Common.Mathematics;
using NCDK.Numerics;
using NCDK.QSAR.Results;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    ///  This class returns the 3D distance between two atoms. Only works with 3D coordinates, which must be calculated beforehand.
    /// </summary>
    /// <remarks>
    /// This descriptor uses these parameters:
    /// <list type="table">
    ///   <item>
    ///     <term>Name</term>
    ///     <term>Default</term>
    ///     <term>Description</term>
    ///   </item>
    ///   <item>
    ///     <term>focusPosition</term>
    ///     <term>0</term>
    ///     <term>The position of the second atom</term>
    ///   </item>
    /// </list>
    /// </remarks>
    // @author         mfe4
    // @cdk.created    2004-11-13
    // @cdk.module     qsaratomic
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:distanceToAtom
    public class DistanceToAtomDescriptor : AbstractAtomicDescriptor, IAtomicDescriptor
    {
        private int focusPosition = 0;

        /// <summary>
        ///  Constructor for the DistanceToAtomDescriptor object
        /// </summary>
        public DistanceToAtomDescriptor() { }

        /// <summary>
        /// The specification attribute of the DistanceToAtomDescriptor object
        /// </summary>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#distanceToAtom",
                typeof(DistanceToAtomDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the DistanceToAtomDescriptor object
        /// </summary>
        /// <exception cref="CDKException"></exception>
        public override object[] Parameters
        {
            set
            {
                if (value.Length > 1)
                {
                    throw new CDKException("DistanceToAtomDescriptor only expects two parameters");
                }
                if (!(value[0] is int))
                {
                    throw new CDKException("The parameter must be of type int");
                }
                focusPosition = (int)value[0];
            }
            get
            {
                return new object[] { focusPosition };
            }
        }

        public override string[] DescriptorNames { get; } = new string[] { "distanceToAtom" };

        /// <summary>
        ///  This method calculate the 3D distance between two atoms.
        /// </summary>
        /// <param name="atom">The <see cref="IAtom"/> for which the <see cref="DescriptorValue"/> is requested</param>
        /// <param name="container">Parameter is the atom container.</param>
        /// <returns>The number of bonds on the shortest path between two atoms</returns>
        public override DescriptorValue Calculate(IAtom atom, IAtomContainer container)
        {
            double distanceToAtom;

            IAtom focus = container.Atoms[focusPosition];

            if (atom.Point3D == null || focus.Point3D == null)
            {
                return new DescriptorValue(_Specification, ParameterNames, Parameters, new DoubleResult(
                        double.NaN), DescriptorNames, new CDKException(
                        "Target or focus atom must have 3D coordinates."));
            }

            distanceToAtom = CalculateDistanceBetweenTwoAtoms(atom, focus);

            return new DescriptorValue(_Specification, ParameterNames, Parameters, new DoubleResult(
                    distanceToAtom), DescriptorNames);
        }

        /// <summary>
        /// generic method for calculation of distance between 2 atoms
        /// </summary>
        /// <param name="atom1">The IAtom 1</param>
        /// <param name="atom2">The IAtom 2</param>
        /// <returns>distance between atom1 and atom2</returns>
        private double CalculateDistanceBetweenTwoAtoms(IAtom atom1, IAtom atom2)
        {
            double distance;
            Vector3 firstPoint = atom1.Point3D.Value;
            Vector3 secondPoint = atom2.Point3D.Value;
            distance = Vector3.Distance(firstPoint, secondPoint);
            return distance;
        }

        /// <summary>
        /// The parameterNames attribute of the DistanceToAtomDescriptor object
        /// </summary>
        public override string[] ParameterNames { get; } = new string[] { "The position of the focus atom" };

        /// <summary>
        ///  The parameterType attribute of the DistanceToAtomDescriptor object
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public override object GetParameterType(string name) => 0;
    }
}

