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
using NCDK.Numerics;
using NCDK.Config;
using NCDK.QSAR.Results;
using System;
using System.Diagnostics;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    ///  Inductive atomic hardness of an atom in a polyatomic system can be defined
    ///  as the "resistance" to a change of the atomic charge. Only works with 3D coordinates, which must be calculated beforehand. 
    /// </summary>
    /// <remarks>
    ///  This descriptor uses these parameters:
    /// <list type="table">
    /// <listheader>
    ///   <term>Name</term>
    ///   <term>Default</term>
    ///   <term>Description</term>
    /// </listheader>
    /// <item>
    ///   <term></term>
    ///   <term></term>
    ///   <term>no parameters</term>
    /// </item>
    /// </list>
    /// </remarks>
    // @author         mfe4
    // @cdk.created    2004-11-03
    // @cdk.module     qsaratomic
    // @cdk.githash
    // @cdk.dictref   qsar-descriptors:atomicHardness
    public class InductiveAtomicHardnessDescriptor : AbstractAtomicDescriptor, IAtomicDescriptor
    {
        private static readonly string[] NAMES = { "indAtomHardnesss" };
        private AtomTypeFactory factory = null;

        /// <summary>
        ///  Constructor for the InductiveAtomicHardnessDescriptor object
        /// </summary>
        public InductiveAtomicHardnessDescriptor() { }

        /// <summary>
        /// The specification attribute of the InductiveAtomicHardnessDescriptor object
        /// </summary>
        /// <returns>The specification value</returns>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#atomicHardness",
                typeof(InductiveAtomicHardnessDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the InductiveAtomicHardnessDescripto object
        /// </summary>
        public override object[] Parameters { get { return null; } set { } }

        public override string[] DescriptorNames => NAMES;

        private DescriptorValue GetDummyDescriptorValue(Exception e)
        {
            return new DescriptorValue(_Specification, ParameterNames, Parameters, new Result<double>(double.NaN), NAMES, e);
        }

        /// <summary>
        ///  It is needed to call the addExplicitHydrogensToSatisfyValency method from
        ///  the class tools.HydrogenAdder, and 3D coordinates.
        /// </summary>
        /// <param name="atom">The <see cref="IAtom"/> for which the <see cref="DescriptorValue"/> is requested</param>
        /// <param name="ac">AtomContainer</param>
        /// <returns>a double with polarizability of the heavy atom</returns>
        public override DescriptorValue Calculate(IAtom atom, IAtomContainer ac)
        {
            if (factory == null)
                try
                {
                    factory = AtomTypeFactory.GetInstance("NCDK.Config.Data.jmol_atomtypes.txt",
                            ac.Builder);
                }
                catch (Exception exception)
                {
                    return GetDummyDescriptorValue(exception);
                }

            double atomicHardness;

            double radiusTarget;

            var allAtoms = ac.Atoms;
            atomicHardness = 0;
            double partial;
            double radius;
            string symbol;
            IAtomType type;
            try
            {
                symbol = atom.Symbol;
                type = factory.GetAtomType(symbol);
                radiusTarget = type.CovalentRadius.Value;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                return GetDummyDescriptorValue(exception);
            }

            foreach (var curAtom in allAtoms)
            {
                if (atom.Point3D == null || curAtom.Point3D == null)
                {
                    return GetDummyDescriptorValue(new CDKException(
                            "The target atom or current atom had no 3D coordinates. These are required"));
                }

                if (!atom.Equals(curAtom))
                {
                    partial = 0;
                    symbol = curAtom.Symbol;

                    try
                    {
                        type = factory.GetAtomType(symbol);
                    }
                    catch (Exception exception)
                    {
                        Debug.WriteLine(exception);
                        return GetDummyDescriptorValue(exception);
                    }
                    radius = type.CovalentRadius.Value;
                    partial += radius * radius;
                    partial += (radiusTarget * radiusTarget);
                    partial = partial / (CalculateSquareDistanceBetweenTwoAtoms(atom, curAtom));
                    atomicHardness += partial;
                }
            }

            atomicHardness = 2 * atomicHardness;
            atomicHardness = atomicHardness * 0.172;
            atomicHardness = 1 / atomicHardness;
            return new DescriptorValue(_Specification, ParameterNames, Parameters, new Result<double>(
                    atomicHardness), NAMES);
        }

        private double CalculateSquareDistanceBetweenTwoAtoms(IAtom atom1, IAtom atom2)
        {
            double distance;
            double tmp;
            Vector3 firstPoint = atom1.Point3D.Value;
            Vector3 secondPoint = atom2.Point3D.Value;
            tmp = Vector3.Distance(firstPoint, secondPoint);
            distance = tmp * tmp;
            return distance;
        }

        /// <summary>
        /// The parameterNames attribute of the InductiveAtomicHardnessDescriptor object
        /// </summary>
        public override string[] ParameterNames { get; } = new string[0];

        /// <summary>
        ///  Gets the parameterType attribute of the InductiveAtomicHardnessDescriptor object
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>An Object of class equal to that of the parameter being requested</returns>
        public override object GetParameterType(string name) => null;
    }
}
