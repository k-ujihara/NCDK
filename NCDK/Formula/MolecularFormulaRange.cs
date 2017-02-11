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

using System;
using System.Collections.Generic;

namespace NCDK.Formula
{
    /**
	 *  Class defining a expanded molecular formula object. The Isotopes don't have
	 *  a fix occurrence in the MolecularFormula but they have a range.<p>
	 *  With this class man can define a MolecularFormula which contains certain IIsotope
	 *  with a maximum and minimum occurrence.
	 *
	 *  Examples:
	 * <ul>
	 *   <li><code>[C(1-5)H(4-10)]-</code></li>
	 * </ul>
	 *
	 * @cdk.module  formula
	 * @author      miguelrojasch
	 * @cdk.created 2007-11-20
	 * @cdk.keyword molecular formula
	 * @cdk.githash
	 */
    public class MolecularFormulaRange : ICloneable
    {

        private IDictionary<IIsotope, int> isotopesMax;
        private IDictionary<IIsotope, int> isotopesMin;

        /**
		 *  Constructs an empty MolecularFormulaExpand.
		 */
        public MolecularFormulaRange()
        {
            isotopesMax = new Dictionary<IIsotope, int>();
            isotopesMin = new Dictionary<IIsotope, int>();
        }

        /**
		 *  Adds an Isotope to this MolecularFormulaExpand in a number of
		 *  maximum and minimum occurrences allowed.
		 *
		 * @param  isotope  The isotope to be added to this MolecularFormulaExpand
		 * @param  countMax The maximal number of occurrences to add
		 * @param  countMin The minimal number of occurrences to add
		 *
		 */
        public void Add(IIsotope isotope, int countMin, int countMax)
        {
            bool flag = false;
            foreach (var thisIsotope in GetIsotopes())
            {
                if (IsTheSame(thisIsotope, isotope))
                {
                    isotopesMax[thisIsotope] = countMax;
                    isotopesMin[thisIsotope] = countMin;
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                isotopesMax[isotope] = countMax;
                isotopesMin[isotope] = countMin;
            }
        }

        /**
		 *  True, if the MolecularFormulaExpand contains the given IIsotope.
		 *  The method looks for other isotopes which has the same
		 *  symbol, natural abundance and exact mass.
		 *
		 * @param  isotope  The IIsotope this MolecularFormula is searched for
		 * @return          True, if the MolecularFormula contains the given isotope object
		 */
        public bool Contains(IIsotope isotope)
        {
            foreach (var thisIsotope in GetIsotopes())
            {
                if (IsTheSame(thisIsotope, isotope))
                {
                    return true;
                }
            }
            return false;
        }

        /**
		 *  Checks a set of Nodes for the maximal occurrence of the isotope in the
		 *  MolecularFormulaExpand from a particular isotope. It returns -1 if the Isotope
		 *  does not exist.
		 *
		 * @param   isotope          The IIsotope to look for
		 * @return                   The occurrence of this isotope in this IMolecularFormula
		 */
        public int GetIsotopeCountMax(IIsotope isotope)
        {
            return !Contains(isotope) ? -1 : isotopesMax[GetIsotope(isotope)];
        }

        /**
		 *  Checks a set of Nodes for the minimal occurrence of the isotope in the
		 *  MolecularFormulaExpand from a particular isotope. It returns -1 if the Isotope
		 *  does not exist.
		 *
		 * @param   isotope          The IIsotope to look for
		 * @return                   The occurrence of this isotope in this IMolecularFormula
		 */
        public int GetIsotopeCountMin(IIsotope isotope)
        {
            return !Contains(isotope) ? -1 : isotopesMin[GetIsotope(isotope)];
        }

        /**
		 *  Checks a set of Nodes for the number of different isotopes in the
		 *  MolecularFormulaExpand.
		 *
		 * @return        The the number of different isotopes in this MolecularFormulaExpand
		 */
        public int Count => isotopesMax.Count;

        /**
		 *  Get the isotope instance given an IIsotope. The instance is those
		 *  that has the isotope with the same symbol, natural abundance and
		 *  exact mass.
		 *
		 * @param  isotope The IIsotope for looking for
		 * @return         The IIsotope instance
		 * @see            #isotopes
		 */
        private IIsotope GetIsotope(IIsotope isotope)
        {
            foreach (var thisIsotope in GetIsotopes())
            {
                if (IsTheSame(isotope, thisIsotope)) return thisIsotope;
            }
            return null;
        }

        /// <summary>
        /// Get all isotopes in this MolecularFormulaExpand.
        /// </summary>
        /// <returns>The isotopes in this MolecularFormulaExpand</returns>
        public IEnumerable<IIsotope> GetIsotopes() => isotopesMax.Keys;

        /**
		 * Removes all isotopes of this molecular formula.
		 */
        public void Clear()
        {
            isotopesMax.Clear();
            isotopesMin.Clear();
        }

        /**
		 *  Removes the given isotope from the MolecularFormulaExpand.
		 *
		 * @param isotope  The IIsotope to be removed
		 */
        public void Remove(IIsotope isotope)
        {
            {
                var k = GetIsotope(isotope);
                if (k != null) isotopesMax.Remove(k);
            }
            {
                var k = GetIsotope(isotope);
                if (k != null) isotopesMin.Remove(k);
            }
        }

        /**
		 * Clones this MolecularFormulaExpand object and its content. I should
		 * integrate into ChemObject.
		 *
		 * @return    The cloned object
		 */
        public virtual object Clone()
        {
            MolecularFormulaRange clone = new MolecularFormulaRange();
            foreach (var isotope in GetIsotopes())
            {
                clone.Add((IIsotope)isotope.Clone(), GetIsotopeCountMin(isotope), GetIsotopeCountMax(isotope));
            }
            return clone;
        }

        /**
		 * Compare to IIsotope. The method doesn't compare instance but if they
		 * have the same symbol, natural abundance and exact mass.
		 *
		 * @param isotopeOne   The first Isotope to compare
		 * @param isotopeTwo   The second Isotope to compare
		 * @return             True, if both isotope are the same
		 */
        private bool IsTheSame(IIsotope isotopeOne, IIsotope isotopeTwo)
        {
            if (!isotopeOne.Symbol.Equals(isotopeTwo.Symbol)) return false;
            if (isotopeOne.NaturalAbundance != isotopeTwo.NaturalAbundance) return false;
            if (isotopeOne.ExactMass != isotopeTwo.ExactMass) return false;

            return true;
        }
    }
}
