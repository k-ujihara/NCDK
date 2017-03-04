/* Copyright (C) 2007  Miguel Rojasch <miguelrojasch@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
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
 *  */
using NCDK.Formula;

namespace NCDK.Tools.Manipulator
{
    /// <summary>
    /// Class with convenience methods that provide methods to manipulate
    /// MolecularFormulaSet's. For example:
    /// <code>
    ///  IMolecularFormula molecularFormula = MolecularManipulatorSet.GetMaxOccurrenceElements(molecularFormulaSet);
    /// </code>
    /// .
    ///
    // @cdk.module  formula
    // @author      miguelrojasch
    // @cdk.created 2007-11-20
    // @cdk.githash
    /// </summary>
    public class MolecularFormulaSetManipulator
    {

        /// <summary>
        /// Extract from a set of MolecularFormula the maximum occurrence of each element found and
        /// put the element and occurrence in a new IMolecularFormula.
        ///
        /// <param name="mfSet">The set of molecularFormules to inspect</param>
        /// <returns>A IMolecularFormula containing the maximum occurrence of the elements</returns>
        /// @see            #GetMinOccurrenceElements(IMolecularFormulaSet)
        /// </summary>
        public static IMolecularFormula GetMaxOccurrenceElements(IMolecularFormulaSet mfSet)
        {

            IMolecularFormula molecularFormula = mfSet.Builder.CreateMolecularFormula();
            foreach (var mf in mfSet)
            {
                foreach (var isotope in mf.Isotopes)
                {
                    IElement element = mfSet.Builder.CreateElement(isotope);
                    int occur_new = MolecularFormulaManipulator.GetElementCount(mf, element);
                    if (!MolecularFormulaManipulator.ContainsElement(molecularFormula, element))
                    {
                        molecularFormula.Add(mfSet.Builder.CreateIsotope(element), occur_new);
                    }
                    else
                    {
                        int occur_old = MolecularFormulaManipulator.GetElementCount(molecularFormula, element);
                        if (occur_new > occur_old)
                        {
                            MolecularFormulaManipulator.RemoveElement(molecularFormula, element);
                            molecularFormula.Add(mfSet.Builder.CreateIsotope(element), occur_new);
                        }
                    }
                }
            }
            return molecularFormula;
        }

        /// <summary>
        /// Extract from a set of MolecularFormula the minimal occurrence of each element found and
        /// put the element and occurrence in a new IMolecularFormula.
        ///
        /// <param name="mfSet">The set of molecularFormules to inspect</param>
        /// <returns>A IMolecularFormula containing the minimal occurrence of the elements</returns>
        /// @see            #GetMaxOccurrenceElements(IMolecularFormulaSet)
        /// </summary>
        public static IMolecularFormula GetMinOccurrenceElements(IMolecularFormulaSet mfSet)
        {

            IMolecularFormula molecularFormula = mfSet.Builder.CreateMolecularFormula();
            foreach (var mf in mfSet)
            {
                foreach (var isotope in mf.Isotopes)
                {
                    IElement element = mfSet.Builder.CreateElement(isotope);
                    int occur_new = MolecularFormulaManipulator.GetElementCount(mf, element);
                    if (!MolecularFormulaManipulator.ContainsElement(molecularFormula, element))
                    {
                        molecularFormula.Add(mfSet.Builder.CreateIsotope(element), occur_new);
                    }
                    else
                    {
                        int occur_old = MolecularFormulaManipulator.GetElementCount(molecularFormula, element);
                        if (occur_new < occur_old)
                        {
                            MolecularFormulaManipulator.RemoveElement(molecularFormula, element);
                            molecularFormula.Add(mfSet.Builder.CreateIsotope(element), occur_new);
                        }
                    }
                }
            }
            return molecularFormula;
        }

        /// <summary>
        /// Remove all those IMolecularFormula which are not fit theirs IElement
        /// occurrence into a limits. The limits are given from formulaMax and formulaMin.
        /// In the minimal IMolecularFormula must contain all those IElement found in the
        /// minimal IMolecularFormula.
        ///
        /// <param name="formulaSet">IMolecularFormulaSet to look for</param>
        /// <param name="formulaMax">A IMolecularFormula which contains the maximal representation of the Elements</param>
        /// <param name="formulaMin">A IMolecularFormula which contains the minimal representation of the Elements</param>
        /// <returns>A IMolecularFormulaSet with only the IMolecularFormula which the IElements</returns>
        ///                         are into the correct occurrence
        /// </summary>
        public static IMolecularFormulaSet Remove(IMolecularFormulaSet formulaSet, IMolecularFormula formulaMin,
                IMolecularFormula formulaMax)
        {

            // prove the correlation between maximum and minimum molecularFormula
            if (!ValidCorrelation(formulaMin, formulaMax)) return null;

            IMolecularFormulaSet newFormulaSet = formulaSet.Builder.CreateMolecularFormulaSet();

            foreach (var formula in formulaSet)
            {
                bool flagPass = true;

                // the formula must contain all element found into the formulaMin
                if (!ValidCorrelation(formula, formulaMin)) continue;

                foreach (var element in MolecularFormulaManipulator.Elements(formulaMin))
                {
                    int occur = MolecularFormulaManipulator.GetElementCount(formula, element);
                    int occurMax = MolecularFormulaManipulator.GetElementCount(formulaMax, element);
                    int occurMin = MolecularFormulaManipulator.GetElementCount(formulaMin, element);

                    if (!(occurMin <= occur) || !(occur <= occurMax))
                    {
                        flagPass = false;
                        break;
                    }

                }
                if (flagPass) // stored if each IElement occurrence is into the limits
                    newFormulaSet.Add(formula);

            }
            return newFormulaSet;
        }

        /// <summary>
        /// In the minimal IMolecularFormula must contain all those IElement found in the
        /// minimal IMolecularFormula.
        ///
        /// <param name="formulaMax">A IMolecularFormula which contains the maximal representation of the Elements</param>
        /// <param name="formulaMin">A IMolecularFormula which contains the minimal representation of the Elements</param>
        /// <returns>True, if the correlation is valid</returns>
        /// </summary>
        private static bool ValidCorrelation(IMolecularFormula formulaMin, IMolecularFormula formulamax)
        {
            foreach (var element in MolecularFormulaManipulator.Elements(formulaMin))
            {
                if (!MolecularFormulaManipulator.ContainsElement(formulamax, element)) return false;

            }
            return true;
        }

        /// <summary>
        ///  True, if the IMolecularFormulaSet contains the given IMolecularFormula but not
        ///  as object. It compare according contains the same number and type of Isotopes.
        ///  It is not based on compare objects.
        ///
        /// <param name="formulaSet">The IMolecularFormulaSet</param>
        /// <param name="formula">The IMolecularFormula this IMolecularFormulaSet is searched for</param>
        /// <returns>True, if the IMolecularFormulaSet contains the given formula</returns>
        ///
        /// @see                IMolecularFormulaSet#Contains(IMolecularFormula)
        /// </summary>
        public static bool Contains(IMolecularFormulaSet formulaSet, IMolecularFormula formula)
        {
            foreach (var fm in formulaSet)
            {
                if (MolecularFormulaManipulator.Compare(fm, formula))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Remove all those IMolecularFormula which are not fit theirs IElement
        /// occurrence into a limits. The limits are given from formulaMax and formulaMin.
        /// In the minimal IMolecularFormula must contain all those IElement found in the
        /// minimal IMolecularFormula.
        ///
        /// <param name="formulaSet">IMolecularFormulaSet to look for</param>
        /// <param name="formulaRange">A IMolecularFormulaRange which contains the range representation of the IIsotope</param>
        /// </summary>
        public static IMolecularFormulaSet Remove(IMolecularFormulaSet formulaSet, MolecularFormulaRange formulaRange)
        {

            IMolecularFormulaSet newFormulaSet = formulaSet.Builder.CreateMolecularFormulaSet();

            foreach (var formula in formulaSet)
            {

                bool flagCorrect = true;
                foreach (var isotope in formulaRange.GetIsotopes())
                {
                    if (formula.GetCount(isotope) != 0)
                    {
                        if ((formula.GetCount(isotope) < formulaRange.GetIsotopeCountMin(isotope))
                                || (formula.GetCount(isotope) > formulaRange.GetIsotopeCountMax(isotope)))
                        {
                            flagCorrect = false;
                            break;
                        }
                    }
                    else if (formulaRange.GetIsotopeCountMin(isotope) != 0)
                    {
                        flagCorrect = false;
                        break;
                    }
                }
                foreach (var isotope in formula.Isotopes)
                {
                    if (!formulaRange.Contains(isotope))
                    {
                        flagCorrect = false;
                        break;
                    }
                }
                if (flagCorrect) // stored if each IElement occurrence is into the limits
                    newFormulaSet.Add(formula);

            }
            return newFormulaSet;
        }
    }
}
