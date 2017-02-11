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
using NCDK.Tools.Manipulator;
using System;
using System.Diagnostics;

namespace NCDK.Formula.Rules
{
    /**
    // This class validate if the rule of nitrogen is kept.
    // <p>If a compound has an odd number of nitrogen atoms,
    // then the molecular ion (the [M]+) will have an odd mass and the value for m/e will be odd.</p>
    // <p>If a compound has no nitrogen atom or an even number of nitrogen atoms, then the m/e value of [M]+ will be even.</p>
     *
     *
    // <p>This rule uses these parameters:
    // <table border="1">
    //   <tr>
    //     <td>Name</td>
    //     <td>Default</td>
    //     <td>Description</td>
    //   </tr>
    //   <tr>
    //     <td>charge</td>
    //     <td>0.0</td>
    //     <td>The Nitrogen rule of MolecularFormula</td>
    //   </tr>
    // </table>
     *
    // @cdk.module  formula
    // @author      miguelrojasch
    // @cdk.created 2008-06-11
    // @cdk.githash
     */
    public class NitrogenRule : IRule
    {
        /**
        //  Constructor for the NitrogenRule object.
         */
        public NitrogenRule() { }
        
        /// <summary>
        /// The parameters attribute of the NitrogenRule object.
        /// </summary>
        public object[] Parameters
        {
            get { return null; }
            set { if (value != null) throw new CDKException("NitrogenRule doesn't expect parameters"); }
        }

        /// <summary>
        /// Validate the nitrogen rule of this IMolecularFormula.
        /// </summary>
        /// <param name="formula">Parameter is the IMolecularFormula</param>
        /// <returns>A double value meaning 1.0 True, 0.0 False</returns>
        public double Validate(IMolecularFormula formula)
        {
            Trace.TraceInformation("Start validation of ", formula);

            double mass = MolecularFormulaManipulator.GetTotalMassNumber(formula);
            if (mass == 0) return 0.0;

            int numberN = MolecularFormulaManipulator.GetElementCount(formula,
                    formula.Builder.CreateElement("N"));
            numberN += GetOthers(formula);

            if (formula.Charge == null || formula.Charge == 0 || !IsOdd(Math.Abs(formula.Charge.Value)))
            {
                if (IsOdd(mass) && IsOdd(numberN))
                {
                    return 1.0;
                }
                else if (!IsOdd(mass) && (numberN == 0 || !IsOdd(numberN)))
                {
                    return 1.0;
                }
                else
                    return 0.0;
            }
            else
            {
                if (!IsOdd(mass) && IsOdd(numberN))
                {
                    return 1.0;
                }
                else if (IsOdd(mass) && (numberN == 0 || !IsOdd(numberN)))
                {
                    return 1.0;
                }
                else
                    return 0.0;
            }
        }

        /**
        // Get the number of other elements which affect to the calculation of the nominal mass.
        // For example Fe, Co, Hg, Pt, As.
         *
        // @param formula The IMolecularFormula to analyze
        // @return        Number of elements
         */
        private int GetOthers(IMolecularFormula formula)
        {
            int number = 0;
            string[] elements = { "Co", "Hg", "Pt", "As" };
            for (int i = 0; i < elements.Length; i++)
                number += MolecularFormulaManipulator.GetElementCount(formula,
                        formula.Builder.CreateElement(elements[i]));

            return number;
        }

        /**
        // Determine if a integer is odd.
         *
        // @param value The value to analyze
        // @return      True, if the integer is odd
         */
        private bool IsOdd(double value)
        {
            if (value % 2 == 0)
                return false;
            else
                return true;
        }

    }
}
