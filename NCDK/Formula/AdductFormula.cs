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
 *  but WITHOUT Any WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Formula
{
    /// <summary>
    /// Class defining an adduct object in a MolecularFormula. It maintains
    /// a list of list IMolecularFormula.
    /// <para>
    /// Examples:
    /// <list type="bullet">
    /// <item>[C2H4O2+Na]+</item>
    /// </list>
    /// </para>
    /// </summary>
    // @cdk.module  data
    // @author      miguelrojasch
    // @cdk.created 2007-11-20
    // @cdk.keyword molecular formula
    // @cdk.githash
    public class AdductFormula : IEnumerable<IMolecularFormula>, IAdductFormula
    {
        /// <summary> Internal List of IMolecularFormula.</summary>
        private List<IMolecularFormula> components;

        /// <summary>
        /// Constructs an empty AdductFormula.
        /// </summary>
        public AdductFormula()
        {
            components = new List<IMolecularFormula>();
        }

        /// <summary>
        /// Constructs an AdductFormula with a copy AdductFormula of another
        /// AdductFormula (A shallow copy, i.e., with the same objects as in
        /// the original AdductFormula).
        /// </summary>
        /// <param name="formula">An MolecularFormula to copy from</param>
        public AdductFormula(IMolecularFormula formula)
        {
            components = new List<IMolecularFormula>();
            components.Insert(0, formula);
        }

        /// <summary>
        /// Adds an molecularFormula to this chemObject.
        /// </summary>
        /// <param name="formula">The molecularFormula to be added to this chemObject</param>
        public virtual void Add(IMolecularFormula formula)
        {
            components.Add(formula);
        }

        /// <summary>
        /// Adds all molecularFormulas in the AdductFormula to this chemObject.
        /// </summary>
        /// <param name="formulaSet">The MolecularFormulaSet</param>
        public virtual void Add(IMolecularFormulaSet formulaSet)
        {
            foreach (var mf in formulaSet)
            {
                Add(mf);
            }
            // NotifyChanged() is called by Add()
        }

        /// <summary>
        /// True, if the AdductFormula contains the given IIsotope object and not
        /// the instance. The method looks for other isotopes which has the same
        /// symbol, natural abundance and exact mass.
        /// </summary>
        /// <param name="isotope">The IIsotope this AdductFormula is searched for</param>
        /// <returns>True, if the AdductFormula contains the given isotope object</returns>
        public virtual bool Contains(IIsotope isotope)
        {
            foreach (var thisIsotope in GetIsotopes())
                if (IsTheSame(thisIsotope, isotope))
                    return true;
            return false;
        }

        /// <summary>
        /// The partial charge of this Adduct. If the charge
        /// has not been set the return value is double.NaN.
        /// </summary>
        public virtual int? Charge
        {
            get { return components.Select(n => n.Charge ?? 0).Sum(); }
            set { new FieldAccessException(); }
        }

        /// <summary>
        /// Checks a set of Nodes for the occurrence of the isotope in the
        /// adduct formula from a particular isotope. It returns 0 if the does not exist.
        /// </summary>
        /// <param name="isotope">The IIsotope to look for</param>
        /// <returns>The occurrence of this isotope in this adduct</returns>
        public virtual int GetCount(IIsotope isotope)
        {
            return components.Select(nn => nn.GetCount(isotope)).Sum();
        }

        /// <summary>
        /// The the number of different isotopes in this adduct formula
        /// </summary>
        public virtual int IsotopeCount => IsotopesList().Count;

        /// <summary>
        /// An IEnumerator for looping over all isotopes in this adduct formula.
        /// </summary>
        public virtual IEnumerable<IIsotope> GetIsotopes() => IsotopesList();

        /// <summary>
        /// Returns a List for looping over all isotopes in this adduct formula.
        /// </summary>
        /// <returns>A List with the isotopes in this adduct formula</returns>
        private List<IIsotope> IsotopesList()
        {
            List<IIsotope> isotopes = new List<IIsotope>();
            foreach (var component in components)
            {
                foreach (var isotope in component.Isotopes)
                    if (!isotopes.Contains(isotope))
                        isotopes.Add(isotope);
            }
            return isotopes;
        }

        /// <summary>
        /// Returns an Iterable for looping over all IMolecularFormula
        /// in this adduct formula.
        /// </summary>
        /// <returns>An Iterable with the IMolecularFormula in this adduct formula</returns>
        public virtual IEnumerator<IMolecularFormula> GetEnumerator() => components.GetEnumerator();

        /// <summary>
        /// The number of MolecularFormulas in this AdductFormula.
        /// </summary>
        public virtual int Count => components.Count;

        /// <summary>
        /// True, if the AdductFormula contains the given IMolecularFormula object.
        ///
        /// <param name="formula">The IMolecularFormula this AdductFormula is searched for</param>
        /// <returns>True, if the AdductFormula contains the given IMolecularFormula object</returns>
        /// </summary>
        public virtual bool Contains(IMolecularFormula formula)
        {
            return components.Contains(formula);
        }

        /// <summary>
        /// The MolecularFormula at position <paramref name="position"/> in the hemObject.
        /// </summary>
        /// <param name="position">The position of the IMolecularFormula to be returned.</param>
        /// <returns>The IMolecularFormula at position <paramref name="position"/>.</returns>
        public virtual IMolecularFormula this[int position]
        {
            get { return components[position]; }
            set { components[position] = value; }
        }

        /// <summary>
        /// Removes all IMolecularFormula from this chemObject.
        /// </summary>
        public virtual void Clear()
        {
            components.Clear();
        }

        /// <summary>
        /// Removes an IMolecularFormula from this chemObject.
        /// </summary>
        /// <param name="formula">The IMolecularFormula to be removed from this chemObject</param>
        public virtual bool Remove(IMolecularFormula formula)
        {
            return components.Remove(formula);
        }

        /// <summary>
        /// Removes an MolecularFormula from this chemObject.
        /// </summary>
        /// <param name="position">The position of the MolecularFormula to be removed from this chemObject</param>
        public virtual void RemoveAt(int position)
        {
            components.RemoveAt(position);
        }

        /// <summary>
        /// Clones this AdductFormula object and its content.
        /// </summary>
        /// <returns> The cloned object</returns>
        public virtual object Clone()
        {
            //        /* it is not a super class of chemObject */
            //        AdductFormula clone = (AdductFormula) base.Clone();
            //        // start from scratch
            //        clone.Clear();
            //        // clone all molecularFormulas
            //        IEnumerator<IMolecularFormula> iterForm = this;
            //        while(iterForm.MoveNext()){
            //            clone.AddMolecularFormula((IMolecularFormula) iterForm.Next().Clone());
            //        }
            AdductFormula clone = new AdductFormula();
            foreach (var form in this)
            {
                clone.Add((IMolecularFormula)form.Clone());
            }
            return clone;
        }

        public ICDKObject Clone(CDKObjectMap map)
        {
            return (ICDKObject)Clone();
        }

        /// <summary>
        /// Compare to IIsotope. The method doesn't compare instance but if they
        /// have the same symbol, natural abundance and exact mass.
        /// </summary>
        /// <param name="isotopeOne">The first Isotope to compare</param>
        /// <param name="isotopeTwo">The second Isotope to compare</param>
        /// <returns>True, if both isotope are the same</returns>
        private bool IsTheSame(IIsotope isotopeOne, IIsotope isotopeTwo)
        {
            if (isotopeOne.Symbol != isotopeTwo.Symbol) return false;
            if (isotopeOne.NaturalAbundance != isotopeTwo.NaturalAbundance) return false;
            if (isotopeOne.ExactMass != isotopeTwo.ExactMass) return false;

            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddRange(IEnumerable<IMolecularFormula> collection)
        {
            components.AddRange(collection);
        }

        public int IndexOf(IMolecularFormula item)
        {
            return components.IndexOf(item);
        }

        public void Insert(int index, IMolecularFormula item)
        {
            components.Insert(index, item);
        }

        public void CopyTo(IMolecularFormula[] array, int arrayIndex)
        {
            components.CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly => ((IList<MolecularFormula>)(components)).IsReadOnly;

        public IChemObjectBuilder Builder
            => Default.ChemObjectBuilder.Instance;
    }
}
