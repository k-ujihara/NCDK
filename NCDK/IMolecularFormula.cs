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
using System.Collections.Generic;

namespace NCDK
{
    /// <summary>
    /// Class defining a molecular formula object. It maintains a list of IISotope.
    /// <para>
    /// Examples:
    /// <list type="bullet">
    /// <item>[C5H5]-</item>
    /// <item>C6H6</item>
    /// <item><sup>12</sup>C<sub>5</sub><sup>13</sup>CH6</item>
    /// </list>
    /// </para>
    /// </summary>
    // fixed CDK bug  (sup --> sub)
    // @cdk.module  interfaces
    // @author      miguelrojasch
    // @cdk.created 2007-11-20
    // @cdk.keyword molecular formula
    // @cdk.githash
    public interface IMolecularFormula : ICDKObject
    {
        /// <summary>
        /// Adds an molecularFormula to this MolecularFormula.
        /// </summary>
        /// <param name="formula">The molecularFormula to be added to this chemObject</param>
        /// <returns>the new molecular formula</returns>
        IMolecularFormula Add(IMolecularFormula formula);

        /// <summary>
        /// Adds an Isotope to this MolecularFormula one time.
        /// </summary>
        /// <param name="isotope">The isotope to be added to this MolecularFormula</param>
        /// <returns>the new molecular formula</returns>
        /// <seealso cref="Add(IIsotope, int)"/>
        IMolecularFormula Add(IIsotope isotope);

        /// <summary>
        ///  Adds an Isotope to this MolecularFormula in a number of occurrences.
        /// </summary>
        /// <param name="isotope">The isotope to be added to this MolecularFormula</param>
        /// <param name="count">The number of occurrences to add</param>
        /// <returns>the new molecular formula</returns>
        /// <seealso cref="Add(IIsotope)"/>
        IMolecularFormula Add(IIsotope isotope, int count);

        /// <summary>
        ///  Checks a set of Nodes for the occurrence of the isotope in the
        ///  IMolecularFormula from a particular isotope. It returns 0 if the does not exist.
        /// </summary>
        /// <param name="isotope">The IIsotope to look for</param>
        /// <returns>The occurrence of this isotope in this IMolecularFormula</returns>
        int GetCount(IIsotope isotope);

        /// <summary>
        ///  Returns an <see cref="IEnumerable{T}"/> for looping over all isotopes in this IMolecularFormula.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> with the isotopes in this IMolecularFormula</returns>
        IEnumerable<IIsotope> Isotopes { get; }

        /// <summary>
        ///  Checks a set of Nodes for the number of different isotopes in the
        ///  IMolecularFormula.
        /// </summary>
        /// <value>The the number of different isotopes in this IMolecularFormula</value>
        int Count { get; }

        /// <summary>
        ///  True, if the MolecularFormula contains the given IIsotope object. Not
        ///  the instance. The method looks for other isotopes which has the same
        ///  symbol, natural abundance and exact mass.
        /// </summary>
        /// <param name="isotope">The IIsotope this IMolecularFormula is searched for</param>
        /// <returns>True, if the IMolecularFormula contains the given isotope object</returns>
        bool Contains(IIsotope isotope);

        /// <summary>
        /// Removes the given isotope from the MolecularFormula.
        /// </summary>
        /// <param name="isotope">The IIsotope to be removed</param>
        void Remove(IIsotope isotope);

        /// <summary>
        /// Removes all isotopes of this molecular formula.
        /// </summary>
        void Clear();

        /// <summary>
        /// The partial charge of this IMolecularFormula. If the charge
        /// has not been set the return value is null.
        /// </summary>
        int? Charge { get; set; }

        /// <summary>
        /// Sets a property for a IChemObject.
        /// </summary>
        /// <param name="description">An object description of the property (most likely a unique string)</param>
        /// <param name="property">An object with the property itself</param>
        /// <seealso cref="GetProperty(object)"/>
        /// <seealso cref="RemoveProperty(object)"/>
        void SetProperty(object description, object property);

        /// <summary>
        /// Set the properties of this object to the provided map (shallow copy). Any
        /// existing properties are removed.
        /// </summary>
        /// <param name="properties">map key-value pairs</param>
        void SetProperties(IEnumerable<KeyValuePair<object, object>> properties);

        /// <summary>
        /// Removes a property for a IChemObject.
        /// </summary>
        /// <param name="description">The object description of the property (most likely a unique string)</param>
        /// <see cref="SetProperty(object, object)"/>
        /// <see cref="GetProperty(object)"/> 
        void RemoveProperty(object description);

        /// <summary>
        /// Returns a property for the IChemObject - the object is automatically
        /// cast to the required type. This does however mean if the wrong type is
        /// provided then a runtime ClassCastException will be thrown.
        /// </summary>
        /// <example>
        /// <code>
        ///     IAtom atom = new Atom("C");
        ///     atom.SetProperty("number", 1); // set an integer property
        ///
        ///     // access the property and automatically cast to an int
        ///     int? number = atom.GetProperty("number");
        ///
        ///     // if the method is in a chain or needs to be nested the type
        ///     // can be provided
        ///     MethodAcceptingInt(atom.GetProperty("number", int?.class));
        ///
        ///     // the type cannot be checked and so...
        ///     string number = atom.GetProperty("number"); // ClassCastException
        ///
        ///     // if the type is provided a more meaningful error is thrown
        ///     atom.GetProperty("number", typeof(string)); // ArgumentException
        /// </code>
        /// </example>
        /// <typeparam name="T">generic return type</typeparam>
        /// <param name="description">An object description of the property (most likely a unique string)</param>
        /// <returns>The object containing the property. Returns null if property is not set.</returns>
        T GetProperty<T>(object description);

        /// <summary>
        ///  Returns a IDictionary with the IChemObject's properties.
        /// </summary>
        /// <returns>The object's properties as an IDictionary</returns>
        /// <seealso cref="SetProperties(IEnumerable{KeyValuePair{object, object}})"/>
        IDictionary<object, object> GetProperties();
    }
}
