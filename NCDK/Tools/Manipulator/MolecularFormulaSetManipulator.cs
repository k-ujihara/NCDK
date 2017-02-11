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
    /**
	 * Class with convenience methods that provide methods to manipulate
	 * MolecularFormulaSet's. For example:
	 * <pre>
	 *  IMolecularFormula molecularFormula = MolecularManipulatorSet.GetMaxOccurrenceElements(molecularFormulaSet);
	 * </pre>
	 * .
	 *
	 * @cdk.module  formula
	 * @author      miguelrojasch
	 * @cdk.created 2007-11-20
	 * @cdk.githash
	 */
    public class MolecularFormulaSetManipulator
    {

        /**
		 * Extract from a set of MolecularFormula the maximum occurrence of each element found and
		 * put the element and occurrence in a new IMolecularFormula.
		 *
		 * @param mfSet    The set of molecularFormules to inspect
		 * @return         A IMolecularFormula containing the maximum occurrence of the elements
		 * @see            #GetMinOccurrenceElements(IMolecularFormulaSet)
		 */
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

        /**
		 * Extract from a set of MolecularFormula the minimal occurrence of each element found and
		 * put the element and occurrence in a new IMolecularFormula.
		 *
		 * @param mfSet    The set of molecularFormules to inspect
		 * @return         A IMolecularFormula containing the minimal occurrence of the elements
		 * @see            #GetMaxOccurrenceElements(IMolecularFormulaSet)
		 */
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

        /**
		 * Remove all those IMolecularFormula which are not fit theirs IElement
		 * occurrence into a limits. The limits are given from formulaMax and formulaMin.
		 * In the minimal IMolecularFormula must contain all those IElement found in the
		 * minimal IMolecularFormula.
		 *
		 * @param formulaSet  IMolecularFormulaSet to look for
		 * @param formulaMax  A IMolecularFormula which contains the maximal representation of the Elements
		 * @param formulaMin  A IMolecularFormula which contains the minimal representation of the Elements
		 * @return            A IMolecularFormulaSet with only the IMolecularFormula which the IElements
		 * 						are into the correct occurrence
		 */
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

        /**
		 * In the minimal IMolecularFormula must contain all those IElement found in the
		 * minimal IMolecularFormula.
		 *
		 * @param formulaMax  A IMolecularFormula which contains the maximal representation of the Elements
		 * @param formulaMin  A IMolecularFormula which contains the minimal representation of the Elements
		 * @return            True, if the correlation is valid
		 */
        private static bool ValidCorrelation(IMolecularFormula formulaMin, IMolecularFormula formulamax)
        {
            foreach (var element in MolecularFormulaManipulator.Elements(formulaMin))
            {
                if (!MolecularFormulaManipulator.ContainsElement(formulamax, element)) return false;

            }
            return true;
        }

        /**
		 *  True, if the IMolecularFormulaSet contains the given IMolecularFormula but not
		 *  as object. It compare according contains the same number and type of Isotopes.
		 *  It is not based on compare objects.
		 *
		 * @param formulaSet   The IMolecularFormulaSet
		 * @param  formula     The IMolecularFormula this IMolecularFormulaSet is searched for
		 * @return             True, if the IMolecularFormulaSet contains the given formula
		 *
		 * @see                IMolecularFormulaSet#Contains(IMolecularFormula)
		 */
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

        /**
		 * Remove all those IMolecularFormula which are not fit theirs IElement
		 * occurrence into a limits. The limits are given from formulaMax and formulaMin.
		 * In the minimal IMolecularFormula must contain all those IElement found in the
		 * minimal IMolecularFormula.
		 *
		 * @param formulaSet   IMolecularFormulaSet to look for
		 * @param formulaRange A IMolecularFormulaRange which contains the range representation of the IIsotope
		 */
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
