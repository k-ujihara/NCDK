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
    /**
    //  Class defining an set object of MolecularFormulas. It maintains
    //   a list of list IMolecularFormula.<p>
     *
    // @cdk.module  data
    // @author      miguelrojasch
    // @cdk.created 2007-11-20
    // @cdk.keyword molecular formula
    // @cdk.githash
     */
    public class MolecularFormulaSet : IMolecularFormulaSet, ICloneable
    {
        /// <summary> Internal List of IMolecularFormula.</summary>
        private IList<IMolecularFormula> components;

        /**
		 *  Constructs an empty MolecularFormulaSet.
		 *
		 *  @see #MolecularFormulaSet(IMolecularFormula)
		 */
        public MolecularFormulaSet()
        {
            components = new List<IMolecularFormula>();
        }

        /**
		 * Constructs a MolecularFormulaSet with a copy MolecularFormulaSet of another
		 * MolecularFormulaSet (A shallow copy, i.e., with the same objects as in
		 * the original MolecularFormulaSet).
		 *
		 *  @param  formula  An MolecularFormula to copy from
		 *  @see             #MolecularFormulaSet()
		 */
        public MolecularFormulaSet(IMolecularFormula formula)
        {
            components = new List<IMolecularFormula>();
            components.Insert(0, formula);
        }

        /**
		 *  Adds all molecularFormulas in the MolecularFormulaSet to this chemObject.
		 *
		 * @param  formulaSet  The MolecularFormulaSet
		 */
        public virtual void AddRange(IEnumerable<IMolecularFormula> formulaSet)
        {
            foreach (var mf in formulaSet)
            {
                Add(mf);
            }
            /*
			 * notifyChanged() is called by Add()
			 */
        }

        /**
		 * Adds an molecularFormula to this chemObject.
		 *
		 * @param  formula  The molecularFormula to be added to this chemObject
		 */
        public virtual void Add(IMolecularFormula formula)
        {
            components.Add(formula);
        }

        /**
		 *  Returns an Enumerator for looping over all IMolecularFormula
		 *   in this MolecularFormulaSet.
		 *
		 * @return    An Iterable with the IMolecularFormula in this MolecularFormulaSet
		 */
        public virtual IEnumerator<IMolecularFormula> GetEnumerator()
        {
            return components.GetEnumerator();
        }

        /**
		 * Returns the number of MolecularFormulas in this MolecularFormulaSet.
		 *
		 * @return     The number of MolecularFormulas in this MolecularFormulaSet
		 */
        public virtual int Count => components.Count;

        /**
		 *  True, if the MolecularFormulaSet contains the given IMolecularFormula object.
		 *
		 * @param  formula  The IMolecularFormula this MolecularFormulaSet is searched for
		 * @return          True, if the MolecularFormulaSet contains the given IMolecularFormula object
		 */
        public virtual bool Contains(IMolecularFormula formula) => components.Contains(formula);

        /**
		 * RThe MolecularFormula at position <code>number</code> in the
		 * chemObject.
		 *
		 * @param  position The position of the IMolecularFormula to be returned.
		 */
        public virtual IMolecularFormula this[int position]
        {
            get { return components[position]; }
            set { components[position] = value; }
        }

        /**
		 * Removes all IMolecularFormula from this chemObject.
		 */
        public virtual void Clear()
        {
            components.Clear();
        }

        /**
		 * Removes an IMolecularFormula from this chemObject.
		 *
		 * @param  formula  The IMolecularFormula to be removed from this chemObject
		 */
        public virtual bool Remove(IMolecularFormula formula)
        {
            return components.Remove(formula);
        }

        /**
		 * Removes an MolecularFormula from this chemObject.
		 *
		 * @param  position  The position of the MolecularFormula to be removed from this chemObject
		 */
        public void RemoveAt(int position)
        {
            components.RemoveAt(position);
        }

        /**
		 * Clones this MolecularFormulaSet object and its content.
		 *
		 * @return    The cloned object
		 */
        public virtual object Clone()
        {
            //		/* it is not a super class of chemObject */
            //		MolecularFormulaSet clone = (MolecularFormulaSet) base.Clone();
            //        // start from scratch
            //		clone.Clear();
            //        // clone all molecularFormulas
            //		IEnumerator<IMolecularFormula> iterForm = this;
            //		while(iterForm.MoveNext()){
            //			clone.AddMolecularFormula((IMolecularFormula) iterForm.Next().Clone());
            //		}
            MolecularFormulaSet clone = new MolecularFormulaSet();
            foreach (var mf in this)
            {
                clone.Add((IMolecularFormula)mf.Clone());
            }
            return clone;
        }

        public ICDKObject Clone(CDKObjectMap map) => (ICDKObject)Clone();

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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IChemObjectBuilder Builder => Default.ChemObjectBuilder.Instance;

        public bool IsReadOnly => components.IsReadOnly;
    }
}
