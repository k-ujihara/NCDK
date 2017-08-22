/*  Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.Charges;
using NCDK.QSAR.Results;
using NCDK.Tools.Manipulator;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    /// Atomic descriptor that reflects that Gasteiger-Marsili sigma electronegativity.
    /// The used approach is given by <pre>X = a + bq + c(q*q)</pre> where a, b, and c are
    /// the Gasteiger-Marsili parameters and q is the sigma charge. For the actual
    /// calculation it uses the <see cref="Electronegativity"/> class.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This descriptor uses these parameters:
    /// <list type="table">
    /// <listheader><term>Name</term><term>Default</term><term>Description</term></listheader>
    /// <item><term>maxIterations</term><term>0</term><term>Number of maximum iterations</term></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <seealso cref="Electronegativity"/>
    // @author      mfe4
    // @cdk.created 2004-11-03
    // @cdk.module  qsaratomic
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:sigmaElectronegativity

    public class SigmaElectronegativityDescriptor : AbstractAtomicDescriptor, IAtomicDescriptor
    {
        /// <summary>Number of maximum iterations</summary>
        private int maxIterations = 0;

        private static readonly string[] NAMES = { "elecSigmA" };

        private Electronegativity electronegativity;

        /// <summary>
        ///  Constructor for the SigmaElectronegativityDescriptor object
        /// </summary>
        public SigmaElectronegativityDescriptor()
        {
            electronegativity = new Electronegativity();
        }

        /// <summary>
        /// The specification attribute of the SigmaElectronegativityDescriptor object
        /// </summary>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#sigmaElectronegativity",
                typeof(SigmaElectronegativityDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <summary>
        ///  Sets the parameters attribute of the SigmaElectronegativityDescriptor object
        ///  <list type="bullet">
        /// <item>
        /// <term>1</term>
        /// <description>max iterations (optional, defaults to 20)</description>
        /// </item>
        /// </list>
        /// </summary>
        /// <exception cref="CDKException"></exception>
        public override object[] Parameters
        {
            set
            {
                if (value.Length > 1)
                {
                    throw new CDKException("SigmaElectronegativityDescriptor only expects one parameter");
                }
                if (!(value[0] is int))
                {
                    throw new CDKException("The parameter must be of type int");
                }
                if (value.Length == 0) return;
                maxIterations = (int)value[0];
            }
            get
            {
                // return the parameters as used for the descriptor calculation
                return new object[] { maxIterations };
            }
        }

        public override IReadOnlyList<string> DescriptorNames { get; } = NAMES;

        /// <summary>
        ///  The method calculates the sigma electronegativity of a given atom
        ///  It is needed to call the addExplicitHydrogensToSatisfyValency method from the class tools.HydrogenAdder.
        /// </summary>
        /// <param name="atom">The <see cref="IAtom"/> for which the <see cref="DescriptorValue"/> is requested</param>
        /// <param name="ac">AtomContainer</param>
        /// <returns>return the sigma electronegativity</returns>
        public override DescriptorValue Calculate(IAtom atom, IAtomContainer ac)
        {
            IAtomContainer clone;
            IAtom localAtom;
            try
            {
                clone = (IAtomContainer)ac.Clone();
                localAtom = clone.Atoms[ac.Atoms.IndexOf(atom)];
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(clone);
            }
            catch (CDKException e)
            {
                return new DescriptorValue(_Specification, ParameterNames, Parameters, new Result<double>(double.NaN), NAMES, e);
            }

            if (maxIterations != -1 && maxIterations != 0) electronegativity.MaxIterations = maxIterations;

            double result = electronegativity.CalculateSigmaElectronegativity(clone, localAtom);

            return new DescriptorValue(_Specification, ParameterNames, Parameters, new Result<double>(result), NAMES);
        }

        /// <summary>
        /// The parameterNames attribute of the SigmaElectronegativityDescriptor object
        /// </summary>
        public override IReadOnlyList<string> ParameterNames { get; } = new string[] { "maxIterations" };

        /// <summary>
        /// Gets the parameterType attribute of the SigmaElectronegativityDescriptor object
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>An Object of class equal to that of the parameter being requested</returns>
        public override object GetParameterType(string name)
        {
            return 0;
        }
    }
}
