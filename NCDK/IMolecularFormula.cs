/* Copyright (C) 2007  Miguel Rojasch <miguelrojasch@users.sf.net>
 *               2012  John May <john.wilkinsonmay@gmail.com>
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
using System.ComponentModel;

namespace NCDK
{
    /**
     *  Class defining a molecular formula object. It maintains
     *   a list of IISotope.<p>
     *
     *  Examples:
     * <ul>
     *   <li><code>[C5H5]-</code></li>
     *   <li><code>C6H6</code></li>
     *   <li><code><sup>12</sup>C5</sup><sup>13</sup>CH6</code></li>
     * </ul>
     *
     * @cdk.module  interfaces
     * @author      miguelrojasch
     * @cdk.created 2007-11-20
     * @cdk.keyword molecular formula
     * @cdk.githash
     */
    public interface IMolecularFormula : ICDKObject
    {
        /**
         * Adds an molecularFormula to this MolecularFormula.
         *
         * @param  formula  The molecularFormula to be added to this chemObject
         * @return the new molecular formula
         */
        IMolecularFormula Add(IMolecularFormula formula);

        /**
         *  Adds an Isotope to this MolecularFormula one time.
         *
         * @param  isotope  The isotope to be added to this MolecularFormula
         * @see             #Add(IIsotope, int)
         * @return the new molecular formula
         */
        IMolecularFormula Add(IIsotope isotope);

        /**
         *  Adds an Isotope to this MolecularFormula in a number of occurrences.
         *
         * @param  isotope  The isotope to be added to this MolecularFormula
         * @param  count    The number of occurrences to add
         * @see             #Add(IIsotope)
         * @return the new molecular formula
         */
        IMolecularFormula Add(IIsotope isotope, int count);

        /**
         *  Checks a set of Nodes for the occurrence of the isotope in the
         *  IMolecularFormula from a particular isotope. It returns 0 if the does not exist.
         *
         * @param   isotope          The IIsotope to look for
         * @return                   The occurrence of this isotope in this IMolecularFormula
         * @see                      #Isotopes.Count()
         */
        int GetCount(IIsotope isotope);

        /**
         *  Returns an {@link Iterable} for looping over all isotopes in this IMolecularFormula.
         *
         * @return    An {@link Iterable} with the isotopes in this IMolecularFormula
         */
        IEnumerable<IIsotope> Isotopes { get; }

        /**
         *  Checks a set of Nodes for the number of different isotopes in the
         *  IMolecularFormula.
         *
         * @return        The the number of different isotopes in this IMolecularFormula
         * @see           #Isotopes.Count(IIsotope)
         */
         int Count { get; }

        /**
         *  True, if the MolecularFormula contains the given IIsotope object. Not
         *  the instance. The method looks for other isotopes which has the same
         *  symbol, natural abundance and exact mass.
         *
         * @param  isotope  The IIsotope this IMolecularFormula is searched for
         * @return          True, if the IMolecularFormula contains the given isotope object
         */
        bool Contains(IIsotope isotope);

        /**
         *  Removes the given isotope from the MolecularFormula.
         *
         * @param isotope  The IIsotope to be removed
         */
        void Remove(IIsotope isotope);

        /**
         * Removes all isotopes of this molecular formula.
         */
        void Clear();

        /// <summary>
        /// The partial charge of this IMolecularFormula. If the charge
        /// has not been set the return value is null.
        /// </summary>
        int? Charge { get; set; }

        /**
         * Sets a property for a IChemObject.
         *
         * @param  description  An object description of the property (most likely a
         *                      unique string)
         * @param  property     An object with the property itself
         * @see                 #GetProperty
         * @see                 #removeProperty
         */
        void SetProperty(string key, object value);

        /**
         * Set the properties of this object to the provided map (shallow copy). Any
         * existing properties are removed.
         *
         * @param properties map key-value pairs
         */
        void SetProperties(IEnumerable<KeyValuePair<string, object>> properties);

        /**
         * Removes a property for a IChemObject.
         *
         * @param  description  The object description of the property (most likely a
         *                      unique string)
         * @see                 #SetProperty
         * @see                 #GetProperty
         */
        void RemoveProperty(string description);

        /**
         * Returns a property for the IChemObject - the object is automatically
         * cast to the required type. This does however mean if the wrong type is
         * provided then a runtime ClassCastException will be thrown.
         *
         * <p/>
         * <pre>{@code
         *
         *     IAtom atom = new Atom("C");
         *     atom.SetProperty("number", 1); // set an integer property
         *
         *     // access the property and automatically cast to an int
         *     int? number = atom.GetProperty("number");
         *
         *     // if the method is in a chain or needs to be nested the type
         *     // can be provided
         *     MethodAcceptingInt(atom.GetProperty("number", int?.class));
         *
         *     // the type cannot be checked and so...
         *     string number = atom.GetProperty("number"); // ClassCastException
         *
         *     // if the type is provided a more meaningful error is thrown
         *     atom.GetProperty("number", typeof(string)); // ArgumentException
         *
         * }</pre>
         * @param  description  An object description of the property (most likely a
         *                      unique string)
         * @param  <T>          generic return type
         * @return              The object containing the property. Returns null if
         *                      property is not set.
         * @see                 #SetProperty
         * @see                 #GetProperty(object, Class)
         * @see                 #removeProperty
         */
        object GetProperty(string key);

        /**
         *  Returns a IDictionary with the IChemObject's properties.
         *
         *@return    The object's properties as an IDictionary
         *@see       #addProperties
         */
        IDictionary<string, object> GetProperties();
    }
}
