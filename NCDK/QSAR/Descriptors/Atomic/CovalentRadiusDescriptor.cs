/* Copyright (C) 2004-2007  Miguel Rojas <miguel.rojas@uni-koeln.de>
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
using NCDK.QSAR.Results;
using System;
using System.Diagnostics;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    ///  This class returns the covalent radius of a given atom.
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
    ///     <term></term>
    ///     <term></term>
    ///     <term>no parameters</term>
    ///   </item>
    /// </list>
    /// </remarks>
    // @author         Miguel Rojas
    // @cdk.created    2006-05-17
    // @cdk.module     qsaratomic
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:covalentradius
    public class CovalentRadiusDescriptor : AbstractAtomicDescriptor, IAtomicDescriptor
    {
        private AtomTypeFactory factory = null;

        public CovalentRadiusDescriptor() { }

        /// <inheritdoc/>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#covalentradius",
                typeof(CovalentRadiusDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        ///  The parameters attribute of the VdWRadiusDescriptor object.
        /// </summary>
        public override object[] Parameters { get { return null; } set { } }

        public override string[] DescriptorNames { get; } = new string[] { "covalentRadius" };

        /// <summary>
        ///  This method calculates the Covalent radius of an atom.
        /// </summary>
        /// <param name="atom">The <see cref="IAtom"/> for which the <see cref="DescriptorValue"/> is requested</param>
        /// <param name="container">The <see cref="IAtomContainer"/> for which the descriptor is to be calculated</param>
        /// <returns>The Covalent radius of the atom</returns>
        public override DescriptorValue Calculate(IAtom atom, IAtomContainer container)
        {
            if (factory == null)
                try
                {
                    factory = AtomTypeFactory.GetInstance("NCDK.Config.Data.jmol_atomtypes.txt",
                            container.Builder);
                }
                catch (Exception exception)
                {
                    return new DescriptorValue(_Specification, ParameterNames, Parameters, new Result<double>(double.NaN), DescriptorNames, exception);
                }

            double covalentradius;
            try
            {
                string symbol = atom.Symbol;
                IAtomType type = factory.GetAtomType(symbol);
                covalentradius = type.CovalentRadius.Value;
                return new DescriptorValue(_Specification, ParameterNames, Parameters, new Result<double>(
                        covalentradius), DescriptorNames);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                return new DescriptorValue(_Specification, ParameterNames, Parameters, new Result<double>(
                        double.NaN), DescriptorNames, exception);
            }
        }

        /// <summary>
        /// The parameterNames attribute of the VdWRadiusDescriptor object.
        /// </summary>
        public override string[] ParameterNames { get; } = Array.Empty<string>();

        /// <summary>
        /// Gets the parameterType attribute of the VdWRadiusDescriptor object.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>An Object of class equal to that of the parameter being requested</returns>
        public override object GetParameterType(string name) => null;
    }
}
