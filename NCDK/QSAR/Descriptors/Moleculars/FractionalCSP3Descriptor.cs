/* Copyright (c) 2018 Kazuya Ujihara <ujihara.kazuya@gmail.com>
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

using NCDK.QSAR.Results;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Calculates F<sub>sp</sub><sup>3</sup> (number of sp<sup>3</sup> hybridized carbons/total carbon count) <token>cdk-cite-Lovering2009</token>.
    /// </summary>
    public class FractionalCSP3Descriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        public FractionalCSP3Descriptor() { }

        public override IImplementationSpecification Specification => specification;
        private static readonly DescriptorSpecification specification =
         new DescriptorSpecification("http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#Fsp3",
               typeof(FractionalCSP3Descriptor).FullName, 
               "The Chemistry Development Kit");

        public override IReadOnlyList<object> Parameters
        {
            set
            {
                if (value.Count != 0)
                    throw new CDKException($"The {nameof(FractionalCSP3Descriptor)} expects zero parameters");
            }
            get => Array.Empty<object>();
        }

        public override IReadOnlyList<string> DescriptorNames { get; } = new string[] { "Fsp3" };

        private DescriptorValue<Result<double>> GetDummyDescriptorValue(Exception e)
        {
            return new DescriptorValue<Result<double>>(specification, ParameterNames, Parameters, new Result<double>(double.NaN), DescriptorNames, e);
        }

        /// <summary>
        /// Calculates the fraction of C atoms that are SP3 hybridized.
        /// </summary>
        /// <param name="mol">The <see cref="IAtomContainer"/> whose F<sub>sp</sub><sup>3</sup> is to be calculated</param>
        /// <returns>F<sub>sp</sub><sup>3</sup></returns>
        public DescriptorValue<Result<double>> Calculate(IAtomContainer mol)
        {
            mol = (IAtomContainer)mol.Clone();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            int nC = 0;
            int nCSP3 = 0;
            foreach (var atom in mol.Atoms)
            {
                if (atom.AtomicNumber == 6)
                {
                    nC++;
                    if (atom.Hybridization == Hybridization.SP3)
                        nCSP3++;
                }
            }
            return new DescriptorValue<Result<double>>(specification, ParameterNames, Parameters, 
                new Result<double>(nC == 0 ? 0 : (double)nCSP3 / nC), DescriptorNames);
        }

        public override IDescriptorResult DescriptorResultType { get; } = new Result<double>();
        public override IReadOnlyList<string> ParameterNames { get; } = Array.Empty<string>();
        public override object GetParameterType(string name) => null;

        IDescriptorValue IMolecularDescriptor.Calculate(IAtomContainer container) => Calculate(container);
    }
}
