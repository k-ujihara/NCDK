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
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Formula
{
    /**
    // Class defining a molecular formula object. It maintains
    // a list of list {@link IIsotope}.
     *
    // <p>Examples:
    // <ul>
    //   <li><code>[C<sub>5</sub>H<sub>5</sub>]-</code></li>
    //   <li><code>C<sub>6</sub>H<sub>6</sub></code></li>
    //   <li><code><sup>12</sup>C<sub>5</sub><sup>13</sup>CH<sub>6</sub></code></li>
    // </ul>
     *
    // @cdk.module  data
    // @author      miguelrojasch
    // @cdk.created 2007-11-20
    // @cdk.keyword molecular formula
    // @cdk.githash
     */
    public class MolecularFormula : IMolecularFormula
    {
        private IDictionary<IIsotope, int?> isotopes;

        /**
        //  The partial charge of the molecularFormula. The default value is double.NaN.
         */
        public int? Charge { get; set; } = null;

        /**
        //  A hashtable for the storage of any kind of properties of this IChemObject.
         */
        private IDictionary<string, object> properties = new Dictionary<string, object>();

        /**
        //  Constructs an empty MolecularFormula.
         */
        public MolecularFormula()
        {
            isotopes = new Dictionary<IIsotope, int?>(new IsotopeComparer(this));
        }

        internal class IsotopeComparer : IEqualityComparer<IIsotope>
        {
            public MolecularFormula parent;

            public IsotopeComparer(MolecularFormula parent)
            {
                this.parent = parent;
            }

            public bool Equals(IIsotope isotopeOne, IIsotope isotopeTwo)
            {
                return parent.IsTheSame(isotopeOne, isotopeTwo);
            }

            public int GetHashCode(IIsotope obj)
            {
                return 0;   // hash is not implemented.
            }
        }

        /**
        // Adds an molecularFormula to this MolecularFormula.
         *
        // @param  formula  The molecularFormula to be added to this chemObject
        // @return          The IMolecularFormula
         */
        public IMolecularFormula Add(IMolecularFormula formula)
        {
            foreach (var newIsotope in formula.Isotopes)
            {
                Add(newIsotope, formula.GetCount(newIsotope));
            }
            if (formula.Charge != null)
            {
                if (Charge != null)
                    Charge += formula.Charge;
                else
                    Charge = formula.Charge;
            }
            return this;
        }

        /**
        //  Adds an Isotope to this MolecularFormula one time.
         *
        // @param  isotope  The isotope to be added to this MolecularFormula
        // @see             #Isotopes.Add(IIsotope, int)
         */
        public IMolecularFormula Add(IIsotope isotope)
        {
            return Add(isotope, 1);
        }

        /**
        //  Adds an Isotope to this MolecularFormula in a number of occurrences.
         *
        // @param  isotope  The isotope to be added to this MolecularFormula
        // @param  count    The number of occurrences to add
        // @see             #Isotopes.Add(IIsotope)
         */
        public IMolecularFormula Add(IIsotope isotope, int count)
        {
            foreach (var thisIsotope in isotopes)
            {
                if (IsTheSame(thisIsotope.Key, isotope))
                {
                    isotopes[thisIsotope.Key] = (thisIsotope.Value ?? 0) + count;
                    return this;
                }
            }
            isotopes.Add(isotope, count);
            return this;
        }

        /**
        //  True, if the MolecularFormula contains the given IIsotope object and not
        //  the instance. The method looks for other isotopes which has the same
        //  symbol, natural abundance and exact mass.
         *
        // @param  isotope  The IIsotope this MolecularFormula is searched for
        // @return          True, if the MolecularFormula contains the given isotope object
         */
        public virtual bool Contains(IIsotope isotope)
            => isotopes.ContainsKey(isotope);

        /**
        //  Checks a set of Nodes for the occurrence of the isotope in the
        //  IMolecularFormula from a particular isotope. It returns 0 if the does not exist.
         *
        // @param   isotope          The IIsotope to look for
        // @return                   The occurrence of this isotope in this IMolecularFormula
        // @see                      #Isotopes.Count
         */
        public int GetCount(IIsotope isotope)
            => !Contains(isotope) ? 0 : isotopes[isotope] ?? 0;

        /**
        //  Checks a set of Nodes for the number of different isotopes in the
        //  IMolecularFormula.
         *
        // @return        The the number of different isotopes in this IMolecularFormula
        // @see           #Isotopes.Count(IIsotope)
         */
        public int Count => isotopes.Count;

        /**
        //  Returns an IEnumerator for looping over all isotopes in this IMolecularFormula.
         *
        // @return    An IEnumerator with the isotopes in this IMolecularFormula
         */
        public IEnumerable<IIsotope> Isotopes => isotopes.Keys;

        /**
        // Removes all isotopes of this molecular formula.
         */
        public void Clear()
        {
            isotopes.Clear();
        }

        /**
        //  Removes the given isotope from the MolecularFormula.
         *
        // @param isotope  The IIsotope to be removed
         */
        public void Remove(IIsotope isotope)
        {
            isotopes.Remove(isotope);
        }

        /// <summary>
        /// Clones this <see cref="MolecularFormula"/> object and its content. I should integrate into ChemObject.
        /// </summary>
        /// <returns>The cloned object</returns>
        public object Clone()
        {
            //		/* it is not a super class of chemObject */
            //		MolecularFormula clone = (MolecularFormula) base.Clone();
            //        // start from scratch
            //		clone.Clear();
            //        // clone all isotopes
            //		IEnumerator<IIsotope> iterIso = this.Isotopes;
            //		while(iterIso.MoveNext()){
            //			IIsotope isotope = iterIso.Next();
            //			clone.Isotopes.Add((IIsotope) isotope.Clone(),Isotopes.Count(isotope));
            //		}

            MolecularFormula clone = new MolecularFormula();
            foreach (var isotope_count in isotopes)
            {
                clone.isotopes.Add(isotope_count.Key, isotope_count.Value);
            }
            clone.Charge = Charge;
            return clone;
        }

        public ICDKObject Clone(CDKObjectMap map) => (ICDKObject)Clone();


        /**
        //  Sets a property for a IChemObject. I should
        // integrate into ChemObject.
         *
         *@param  description  An object description of the property (most likely a
        //      unique string)
         *@param  property     An object with the property itself
         *@see                 #GetProperty
         *@see                 #removeProperty
         */
        public virtual void SetProperty(string key, object value)
        {
            properties[key] = value;
        }

        /**
        //  Removes a property for a IChemObject. I should
        // integrate into ChemObject.
         *
         *@param  description  The object description of the property (most likely a
        //      unique string)
         *@see                 #setProperty
         *@see                 #GetProperty
         */
        public virtual void RemoveProperty(string description)
        {
            properties.Remove(description);
        }

        /**
        // @inheritDoc
         */
        public virtual object GetProperty(string key)
        {
            object property;
            if (properties.TryGetValue(key, out property))
                return property;
            return null;
        }

        /**
        //  Returns a IDictionary with the IChemObject's properties.I should
        // integrate into ChemObject.
         *
         *@return    The object's properties as an Dictionary
         *@see       #setProperties
         */
        public virtual IDictionary<string, object> GetProperties()
            => properties;

        /**
        //  Sets the properties of this object.
         *
         *@param  properties  a Dictionary specifying the property values
         *@see                #getProperties
         */
        public void SetProperties(IEnumerable<KeyValuePair<string, object>> properties)
        {
            this.properties = new Dictionary<string, object>();
            foreach (var pair in properties)
                this.properties.Add(pair);
        }

        /**
        // Compare to IIsotope. The method doesn't compare instance but if they
        // have the same symbol, natural abundance and exact mass.
         *
        // @param isotopeOne   The first Isotope to compare
        // @param isotopeTwo   The second Isotope to compare
        // @return             True, if both isotope are the same
         */
#if TEST
        public
#else
        protected
#endif
        bool IsTheSame(IIsotope isotopeOne, IIsotope isotopeTwo)
        {
            double? natAbund1 = isotopeOne.NaturalAbundance;
            double? natAbund2 = isotopeTwo.NaturalAbundance;

            double? exactMass1 = isotopeOne.ExactMass;
            double? exactMass2 = isotopeTwo.ExactMass;

            if (natAbund1 == null) natAbund1 = -1.0;
            if (natAbund2 == null) natAbund2 = -1.0;
            if (exactMass1 == null) exactMass1 = -1.0;
            if (exactMass2 == null) exactMass2 = -1.0;

            if (!isotopeOne.Symbol.Equals(isotopeTwo.Symbol)) return false;
            if (natAbund1.Value != natAbund2) return false;
            return exactMass1.Value == exactMass2;
        }

        public virtual IChemObjectBuilder Builder => Default.ChemObjectBuilder.Instance;
    }
}
