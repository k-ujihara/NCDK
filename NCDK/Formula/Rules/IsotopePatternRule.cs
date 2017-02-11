/* Copyright (C) 2007  Miguel Rojasch <miguelrojasch@users.sf.net>
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NCDK.Formula.Rules
{
    /// <summary>
    /// This class validate if the Isotope Pattern from a given <see cref="IMolecularFormula"/> correspond with other to compare.
    /// </summary>
    /// <remarks>
    /// This rule uses these parameters:
    /// <table border="1">
    ///   <tr>
    ///     <td>Name</td>
    ///     <td>Default</td>
    ///     <td>Description</td>
    ///   </tr>
    ///   <tr>
    ///     <td>isotopePattern</td>
    ///     <td>List &lt;Double[]&gt</td>
    ///     <td>The Isotope Pattern to compare</td>
    ///   </tr>
    /// </table>
    /// </remarks> 
    // @cdk.module  formula
    // @author      Miguel Rojas Cherto
    // @cdk.created 2007-11-20
    // @cdk.githash    
    public class IsotopePatternRule : IRule
    {
        /// <summary>Accuracy on the mass measuring isotope pattern</summary>
        private double toleranceMass = 0.001;
        private IsotopePattern pattern;
        IsotopePatternGenerator isotopeGe;
        private IsotopePatternSimilarity isotopePatternSimilarity;

        /// <summary>
        /// Constructor for the <see cref="IsotopePatternRule"/> object.
        /// </summary>
        /// <exception cref="IOException">If an error occurs when reading atom type information</exception>
        /// <exception cref="">If an error occurs during tom typing</exception>
        public IsotopePatternRule()
        {
            isotopeGe = new IsotopePatternGenerator(0.01);
            isotopePatternSimilarity = new IsotopePatternSimilarity();
            isotopePatternSimilarity.Tolerance = toleranceMass;
        }

        /// <summary>
        /// The parameters attribute of the <see cref="IsotopePatternRule"/> object.
        /// </summary>
        public object[] Parameters
        {
            get
            {
                // return the parameters as used for the rule validation
                var parameters = new object[]
                {
                // fixed CDK
                pattern == null ? (IList<double[]>)null : pattern.Isotopes.Select(n => new double[] { n.Mass, n.Intensity }).ToList(),
                toleranceMass
                };
                return parameters;
            }
            set
            {
                if (value.Length != 2) throw new CDKException("IsotopePatternRule expects two parameter");

                if (!(value[0] is IList<double[]>)) throw new CDKException("The parameter one must be of type List<Double[]>");

                if (!(value[1] is double)) throw new CDKException("The parameter two must be of type Double");

                pattern = new IsotopePattern();
                foreach (var listISO in (IList<double[]>)value[0])
                {
                    pattern.Isotopes.Add(new IsotopeContainer(listISO[0], listISO[1]));
                }

                isotopePatternSimilarity.Tolerance = (double)value[1];
            }
        }
        
        /// <summary>
        /// Validate the isotope pattern of this <see cref="IMolecularFormula"/>. Important, first
        /// you have to add with the <see cref="Parameters"/> a <see cref="IMolecularFormulaSet"/>
        /// which represents the isotope pattern to compare.
        /// </summary>
        /// <param name="formula">arameter is the IMolecularFormula</param>
        /// <returns>A double value meaning 1.0 True, 0.0 False</returns>
        public double Validate(IMolecularFormula formula)
        {
            Trace.TraceInformation("Start validation of ", formula);

            IsotopePatternGenerator isotopeGe = new IsotopePatternGenerator(0.1);
            IsotopePattern patternIsoPredicted = isotopeGe.GetIsotopes(formula);
            IsotopePattern patternIsoNormalize = IsotopePatternManipulator.Normalize(patternIsoPredicted);

            return isotopePatternSimilarity.Compare(pattern, patternIsoNormalize);
        }
    }
}
