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
using NCDK.QSAR.Result;
using NCDK.Tools;
using System;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    ///  This class return the VdW radius of a given atom.
    /// </summary>
    /// <remarks>
    ///  This descriptor uses these parameters:
    /// <list type="table">
    /// <listheader><term>Name</term><term>Default</term><term>Description</term></listheader>
    /// <item><term></term><term></term><term></term></item>
    /// </list>
    /// </remarks>
    // @author         mfe4
    // @cdk.created    2004-11-13
    // @cdk.module     qsaratomic
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:vdwradius
    public class VdWRadiusDescriptor : AbstractAtomicDescriptor, IAtomicDescriptor
    {
        private static readonly string[] NAMES = { "vdwRadius" };

        /// <summary>
        ///  Constructor for the VdWRadiusDescriptor object.
        /// </summary>
        /// <exception cref="System.IO.IOException">if an error ocurrs when reading atom type information</exception>
        public VdWRadiusDescriptor() { }

        /// <summary>
        /// A map which specifies which descriptor is implemented by this class.
        /// </summary>
        /// <remarks>
        /// These fields are used in the map:
        /// <list type="bullet">
        /// <item>
        /// <term>Specification-Reference</term>
        /// <description>refers to an entry in a unique dictionary</description>
        /// </item>
        /// <item>
        /// <term>Implementation-Title</term>
        /// <description>anything</description>
        /// </item>    
        /// <item>
        /// <term>Implementation-Identifier</term>
        /// <description>a unique identifier for this version of this class</description>
        /// </item>    
        /// <item>
        /// <term>Implementation-Vendor</term>
        /// <description>CDK, JOELib, or anything else</description>
        /// </item>  
        /// </list>
        /// </remarks>
        /// <returns>An object containing the descriptor specification</returns>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#vdwradius",
                typeof(VdWRadiusDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the VdWRadiusDescriptor object.
        /// </summary>
        public override object[] Parameters { get { return null; } set { } }

        public override string[] DescriptorNames => NAMES;

        /// <summary>
        ///  This method calculate the Van der Waals radius of an atom.
        /// </summary>
        /// <param name="container">The <see cref="IAtomContainer"/> for which the descriptor is to be calculated</param>
        /// <returns>The Van der Waals radius of the atom</returns>
        public override DescriptorValue Calculate(IAtom atom, IAtomContainer container)
        {
            string symbol = atom.Symbol;
            double vdwradius = PeriodicTable.GetVdwRadius(symbol).Value;
            return new DescriptorValue(_Specification, ParameterNames, Parameters, new DoubleResult(vdwradius), NAMES);
        }

        /// <summary>
        /// The parameterNames attribute of the VdWRadiusDescriptor object.
        /// </summary>
        /// <returns>The parameterNames value</returns>
        public override string[] ParameterNames { get; } = Array.Empty<string>();

        /// <summary>
        /// Gets the parameterType attribute of the VdWRadiusDescriptor object.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>An Object of class equal to that of the parameter being requested</returns>
        public override object GetParameterType(string name) => null;
    }
}
