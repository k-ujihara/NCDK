/* Copyright (C) 2007  Federico
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
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

using NCDK.Charges;
using NCDK.Graphs.Matrix;
using NCDK.QSAR.Result;
using NCDK.Tools.Manipulator;
using System;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// This class calculates ATS autocorrelation descriptor, where the weight equal
    /// to the charges.
    /// </summary>
    // @author      Federico
    // @cdk.created 2007-02-27
    // @cdk.module  qsarmolecular
    // @cdk.githash
    public class AutocorrelationDescriptorCharge : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private static readonly string[] NAMES = { "ATSc1", "ATSc2", "ATSc3", "ATSc4", "ATSc5" };

        private static double[] Listcharges(IAtomContainer container)
        {
            int natom = container.Atoms.Count;
            double[] charges = new double[natom];
            try
            {
                IAtomContainer mol = container.Builder.NewAtomContainer(((IAtomContainer)container.Clone()));
                GasteigerMarsiliPartialCharges peoe = new GasteigerMarsiliPartialCharges();
                peoe.AssignGasteigerMarsiliSigmaPartialCharges(mol, true);
                for (int i = 0; i < natom; i++)
                {
                    IAtom atom = mol.Atoms[i];
                    charges[i] = atom.Charge.Value;
                }
            }
            catch (Exception ex1)
            {
                throw new CDKException($"Problems with assignGasteigerMarsiliPartialCharges due to {ex1.ToString()}", ex1);
            }

            return charges;
        }

        public override DescriptorValue Calculate(IAtomContainer atomContainer)
        {
            IAtomContainer container = (IAtomContainer)atomContainer.Clone();
            container = AtomContainerManipulator.RemoveHydrogens(container);

            try
            {
                double[] w = Listcharges(container);
                int natom = container.Atoms.Count;
                int[][] distancematrix = TopologicalMatrix.GetMatrix(container);

                double[] chargeSum = new double[5];

                for (int k = 0; k < 5; k++)
                {
                    for (int i = 0; i < natom; i++)
                    {
                        for (int j = 0; j < natom; j++)
                        {
                            if (distancematrix[i][j] == k)
                            {
                                chargeSum[k] += w[i] * w[j];
                            }
                            else
                                chargeSum[k] += 0.0;
                        }
                    }
                    if (k > 0) chargeSum[k] = chargeSum[k] / 2;

                }
                DoubleArrayResult result = new DoubleArrayResult(5);
                foreach (var aChargeSum in chargeSum)
                {
                    result.Add(aChargeSum);
                }
                return new DescriptorValue(_Specification, ParameterNames, Parameters, result, NAMES);

            }
            catch (Exception ex)
            {
                DoubleArrayResult result = new DoubleArrayResult(5);
                for (int i = 0; i < 5; i++)
                    result.Add(double.NaN);
                return new DescriptorValue(_Specification, ParameterNames, Parameters, result, NAMES,
                        new CDKException("Error while calculating the ATS_charge descriptor: " + ex.Message, ex));
            }
        }

        public override string[] ParameterNames { get; } = new string[0];
        public override object GetParameterType(string name) => null;

        public override object[] Parameters
        {
            get { return null; }
            set { }
        }

        public override string[] DescriptorNames => NAMES;

        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
             new DescriptorSpecification(
                    "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#autoCorrelationCharge",
                    typeof(AutocorrelationDescriptorCharge).FullName,
                    "The Chemistry Development Kit");

        public override IDescriptorResult DescriptorResultType { get; } = new DoubleArrayResultType(5);
    }
}
