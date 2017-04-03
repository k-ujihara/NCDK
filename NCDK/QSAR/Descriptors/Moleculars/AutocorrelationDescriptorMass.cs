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
using NCDK.Config;
using NCDK.Graphs.Matrix;
using NCDK.QSAR.Result;
using NCDK.Tools.Manipulator;
using System.IO;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// This class calculates ATS autocorrelation descriptor, where the weight equal
    /// to the scaled atomic mass <token>cdk-cite-Moreau1980</token>.
    /// </summary>
    // @author      Federico
    // @cdk.created 2007-02-08
    // @cdk.module  qsarmolecular
    // @cdk.githash
    // @cdk.set     qsar-descriptors
    public class AutocorrelationDescriptorMass : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private readonly static string[] NAMES = { "ATSm1", "ATSm2", "ATSm3", "ATSm4", "ATSm5" };
        private readonly static double CARBON_MASS = 12.010735896788;

        private static double ScaledAtomicMasses(IElement element)
        {
            IsotopeFactory isofac = Isotopes.Instance;
            double realmasses = isofac.GetNaturalMass(element);
            return (realmasses / CARBON_MASS);
        }

        private static double[] ListConvertion(IAtomContainer container)
        {
            int natom = container.Atoms.Count;

            double[] scalated = new double[natom];

            for (int i = 0; i < natom; i++)
            {
                scalated[i] = ScaledAtomicMasses(container.Atoms[i]);
            }
            return scalated;
        }

        /// <summary>
        /// This method calculate the ATS Autocorrelation descriptor.
        /// </summary>
        public override DescriptorValue Calculate(IAtomContainer atomContainer)
        {
            IAtomContainer container;
            container = (IAtomContainer)atomContainer.Clone();
            container = AtomContainerManipulator.RemoveHydrogens(container);

            try
            {
                double[] w = ListConvertion(container);
                int natom = container.Atoms.Count;
                int[][] distancematrix = TopologicalMatrix.GetMatrix(container);
                double[] masSum = new double[5];

                for (int k = 0; k < 5; k++)
                {
                    for (int i = 0; i < natom; i++)
                    {
                        for (int j = 0; j < natom; j++)
                        {

                            if (distancematrix[i][j] == k)
                            {
                                masSum[k] += w[i] * w[j];
                            }
                            else
                                masSum[k] += 0.0;
                        }
                    }
                    if (k > 0) masSum[k] = masSum[k] / 2;

                }
                DoubleArrayResult result = new DoubleArrayResult(5);
                foreach (var aMasSum in masSum)
                {
                    result.Add(aMasSum);
                }

                return new DescriptorValue(_Specification, ParameterNames, Parameters, result,
                        DescriptorNames);

            }
            catch (IOException ex)
            {
                DoubleArrayResult result = new DoubleArrayResult(5);
                for (int i = 0; i < 5; i++)
                    result.Add(double.NaN);
                return new DescriptorValue(_Specification, ParameterNames, Parameters, result,
                        DescriptorNames, new CDKException("Error while calculating the ATS_mass descriptor: "
                                + ex.Message, ex));
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
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#autoCorrelationMass",
                typeof(AutocorrelationDescriptorMass).FullName,
                "The Chemistry Development Kit");

        public override IDescriptorResult DescriptorResultType { get; } = new DoubleArrayResultType(5);
    }
}
