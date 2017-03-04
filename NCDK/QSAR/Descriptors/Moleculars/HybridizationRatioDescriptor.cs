/*
 *
 *
 *  Copyright (C) 2010 Rajarshi Guha <rajarshi.guha@gmail.com>
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
using NCDK.Tools.Manipulator;
using System;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// <see cref="IMolecularDescriptor"/> that reports the fraction of sp3 carbons to sp2 carbons.
    /// <p/>
    /// Note that it only considers carbon atoms and rather than use a simple ratio
    /// it reports the value of N<sub>sp3</sub>/ (N<sub>sp3</sub> + N<sub>sp2</sub>).
    /// The original form of the descriptor (i.e., simple ratio) has been used to
    /// characterize molecular complexity, especially in the are of natural products
    /// , which usually have a high value of the sp3 to sp2 ratio.
    ///
    // @author Rajarshi Guha
    // @cdk.module qsarmolecular
    // @cdk.githash
    // @cdk.set qsar-descriptors
    // @cdk.dictref qsar-descriptors:hybratio
    /// </summary>
    public class HybridizationRatioDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        /// <summary>
        /// Constructor for the HybridizationRatioDescriptor object.
        /// </summary>
        public HybridizationRatioDescriptor() { }

        /// <summary>
        /// A <see cref="DescriptorSpecification"/> which specifies which descriptor is implemented by this class.
        /// </summary>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
         new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#hybratio",
                typeof(HybridizationRatioDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the HybridizationRatioDescriptor object.
        /// This descriptor takes no parameters
        /// </summary>
        public override object[] Parameters { get { return Array.Empty<object>(); } set { } }

        public override string[] DescriptorNames { get; } = new string[] { "HybRatio" };

        /// <summary>
        /// </summary>
        /// <param name="e">the exception</param>
        /// <returns>a dummy value</returns>
        private DescriptorValue GetDummyDescriptorValue(Exception e)
        {
            return new DescriptorValue(_Specification, ParameterNames, Parameters, new DoubleResult(double.NaN), DescriptorNames, e);
        }

        /// <summary>
        /// Calculate sp3/sp2 hybridization ratio in the supplied <see cref="IAtomContainer"/>.
        /// </summary>
        /// <param name="container">The AtomContainer for which this descriptor is to be calculated.</param>
        /// <returns>The ratio of sp3 to sp2 carbons</returns>
        public override DescriptorValue Calculate(IAtomContainer container)
        {
            try
            {
                IAtomContainer clone = (IAtomContainer)container.Clone();
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(clone);
                int nsp2 = 0;
                int nsp3 = 0;
                foreach (var atom in clone.Atoms)
                {
                    if (!atom.Symbol.Equals("C")) continue;
                    if (atom.Hybridization == Hybridization.SP2)
                        nsp2++;
                    else if (atom.Hybridization == Hybridization.SP3) nsp3++;
                }
                double ratio = nsp3 / (double)(nsp2 + nsp3);
                return new DescriptorValue(_Specification, ParameterNames, Parameters,
                        new DoubleResult(ratio), DescriptorNames);
            }
            catch (CDKException e)
            {
                return GetDummyDescriptorValue(e);
            }
        }

        /// <summary>
        /// Returns the specific type of the DescriptorResult object.
        /// <para>
        /// The return value from this method really indicates what type of result will
        /// be obtained from the <see cref="DescriptorValue"/> object. Note that the same result
        /// can be achieved by interrogating the <see cref="DescriptorValue"/> object; this method
        /// allows you to do the same thing, without actually calculating the descriptor.</para>
        /// </summary>
        public override IDescriptorResult DescriptorResultType { get; } = new DoubleResult(0.0);

        /// <summary>
        /// The parameterNames attribute of the HybridizationRatioDescriptor object.
        /// This descriptor takes no parameters
        /// </summary>
        public override string[] ParameterNames => Array.Empty<string>();

        /// <summary>
        /// Gets the parameterType attribute of the HybridizationRatioDescriptor object.
        /// This descriptor takes no parameters
        /// </summary>
        /// <param name="name">the parameter name</param>
        /// <returns>An Object whose class is that of the parameter requested</returns>
        public override object GetParameterType(string name) => "";
    }
}
