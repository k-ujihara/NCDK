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
using NCDK.QSAR.Result;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// This Class contains a method that returns the number failures of the
    /// Lipinski's Rule Of 5.
    /// See <see href="http://en.wikipedia.org/wiki/Lipinski%27s_Rule_of_Five">http://en.wikipedia.org/wiki/Lipinski%27s_Rule_of_Five</see>.
    /// </summary>
    /// <remarks>
    /// <para>This descriptor uses these parameters:
    /// <list type="table">
    ///   <item>
    ///     <term>Name</term>
    ///     <term>Default</term>
    ///     <term>Description</term>
    ///   </item>
    ///   <item>
    ///     <term>checkAromaticity</term>
    ///     <term>false</term>
    ///     <term>True is the aromaticity has to be checked</term>
    ///   </item>
    /// </list>
    /// </para>
    /// Returns a single value named <i>LipinskiFailures</i>.
    /// </remarks>
    // @author      mfe4
    // @cdk.created 2004-11-03
    // @cdk.module  qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:lipinskifailures
    // @cdk.keyword Lipinski
    // @cdk.keyword rule-of-five
    // @cdk.keyword descriptor
    public class RuleOfFiveDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private bool checkAromaticity = false;

        private static readonly string[] NAMES = { "LipinskiFailures" };

        /// <summary>
        ///  Constructor for the RuleOfFiveDescriptor object.
        /// </summary>
        public RuleOfFiveDescriptor() { }

        /// <inheritdoc/>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#lipinskifailures",
                typeof(RuleOfFiveDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <summary>
        ///  The parameters attribute of the RuleOfFiveDescriptor object.
        /// </summary>
        /// <remarks>
        ///  There is only one parameter, which should be a bool indicating whether
        ///  aromaticity should be checked or has already been checked. The name of the paramete
        ///  is checkAromaticity.</remarks>
        /// <exception cref="CDKException">if more than 1 parameter or a non-bool parameter is specified</exception>
        public override object[] Parameters
        {
            set
            {
                if (value.Length != 1)
                {
                    throw new CDKException("RuleOfFiveDescriptor expects one parameter");
                }
                if (!(value[0] is bool))
                {
                    throw new CDKException("The first parameter must be of type bool");
                }
                // ok, all should be fine
                checkAromaticity = (bool)value[0];
            }
            get
            {
                // return the parameters as used for the descriptor calculation
                return new object[] { checkAromaticity };
            }
        }

        public override string[] DescriptorNames => NAMES;

        /// <summary>
        ///  the method take a bool checkAromaticity: if the bool is true, it means that
        ///  aromaticity has to be checked.
        /// </summary>
        /// <param name="mol">AtomContainer for which this descriptor is to be calculated</param>
        /// <returns>The number of failures of the Lipinski rule</returns>
        public override DescriptorValue Calculate(IAtomContainer mol)
        {
            int lipinskifailures = 0;

            IMolecularDescriptor xlogP = new XLogPDescriptor();
            object[] xlogPparams = { checkAromaticity, true, };

            try
            {
                xlogP.Parameters = xlogPparams;
                double xlogPvalue = ((DoubleResult)xlogP.Calculate(mol).GetValue()).Value;

                IMolecularDescriptor acc = new HBondAcceptorCountDescriptor();
                object[] hBondparams = { checkAromaticity };
                acc.Parameters = hBondparams;
                int acceptors = ((IntegerResult)acc.Calculate(mol).GetValue()).Value;

                IMolecularDescriptor don = new HBondDonorCountDescriptor();
                don.Parameters = hBondparams;
                int donors = ((IntegerResult)don.Calculate(mol).GetValue()).Value;

                IMolecularDescriptor mw = new WeightDescriptor();
                object[] mwparams = { "" };
                mw.Parameters = mwparams;
                double mwvalue = ((DoubleResult)mw.Calculate(mol).GetValue()).Value;

                // exclude (heavy atom) terminal bonds
                // exclude amide C-N bonds because of their high rotational barrier
                // see Veber, D.F. et al., 2002, 45(12), pp.2615–23.
                IMolecularDescriptor rotata = new RotatableBondsCountDescriptor();
                object[] rotatableBondsParams = { false, true };
                rotata.Parameters = rotatableBondsParams;
                int rotatablebonds = ((IntegerResult)rotata.Calculate(mol).GetValue()).Value;

                if (xlogPvalue > 5.0)
                {
                    lipinskifailures += 1;
                }
                if (acceptors > 10)
                {
                    lipinskifailures += 1;
                }
                if (donors > 5)
                {
                    lipinskifailures += 1;
                }
                if (mwvalue > 500.0)
                {
                    lipinskifailures += 1;
                }
                if (rotatablebonds > 10.0)
                {
                    lipinskifailures += 1;
                }
            }
            catch (CDKException e)
            {
                new DescriptorValue(_Specification, ParameterNames, Parameters, new IntegerResult(0), DescriptorNames, e);
            }

            return new DescriptorValue(_Specification, ParameterNames, Parameters, new IntegerResult(lipinskifailures), DescriptorNames);
        }

        /// <inheritdoc/>
        public override IDescriptorResult DescriptorResultType { get; } = new IntegerResult(1);

        /// <summary>
        ///  Gets the parameterNames attribute of the RuleOfFiveDescriptor object.
        /// </summary>
        public override string[] ParameterNames { get; } = new string[] { "checkAromaticity" };

        /// <summary>
        ///  Gets the parameterType attribute of the RuleOfFiveDescriptor object.
        /// </summary>
        /// <param name="name">The name of the parameter. In this case it is 'checkAromaticity'.</param>
        /// <returns>An Object of class equal to that of the parameter being requested</returns>
        public override object GetParameterType(string name)
        {
            return true;
        }
    }
}

