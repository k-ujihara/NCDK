/* Copyright (C) 2006-2007  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using System.Collections.Generic;

namespace NCDK
{
    /// <summary>
    /// The base class for all chemical objects in this cdk. It provides methods for
    /// adding listeners and for their notification of events, as well a a hash
    /// table for administration of physical or chemical properties
    /// </summary>
    // @author        egonw
    // @cdk.githash
    // @cdk.module    interfaces
    public interface IChemObject
        : ICDKObject
    {
        /// <summary>
        /// Deep comparator of <see cref="IChemObject"/>.  
        /// </summary>
        /// <param name="obj">Object to compare with.</param>
        /// <returns><see langword="true"/> if all properties of this object equals to <paramref name="obj"/>.</returns>
        bool Compare(object obj);

        /// <summary>
        /// <see cref="IChemObjectListener"/>s of this <see cref="IChemObject"/>.
        /// </summary>
        ICollection<IChemObjectListener> Listeners { get; }

        /// <summary>
        /// The flag that indicates whether notification messages are sent around.
        /// </summary>
        bool Notification { get; set; }

        /// <summary>
        /// This should be triggered by an method that changes the content of an object
        /// to that the registered listeners can react to it.
        /// </summary>
        void NotifyChanged();

        /// <summary>
        /// Sets a property for a <see cref="IChemObject"/>.
        /// </summary>
        /// <param name="description">An object description of the property (most likely a unique string)</param>
        /// <param name="property">An object with the property itself</param>
        /// <seealso cref="GetProperty{T}(object)"/>
        /// <seealso cref="RemoveProperty(object)"/>
        void SetProperty(object description, object property);

        /// <summary>
        /// Set the properties of this object to the provided map (shallow copy). Any
        ///existing properties are removed.
        /// </summary>
        /// <param name="properties">key-value pairs</param>
        void SetProperties(IEnumerable<KeyValuePair<object, object>> properties);

        /// <summary>
        /// Add properties to this object, duplicate keys will replace any existing value.
        /// </summary>
        /// <param name="properties"><see cref="KeyValuePair{T, T}"/>s specifying the property values</param>
        void AddProperties(IEnumerable<KeyValuePair<object, object>> properties);

        /// <summary>
        /// Removes a property for a IChemObject.
        /// </summary>
        /// <param name="description">The object description of the property (most likely a unique string)</param>
        /// <seealso cref="SetProperty(object, object)"/>
        /// <seealso cref="GetProperty{T}(object)"/>
        void RemoveProperty(object description);

        /// <summary>
        /// Returns a property for the <see cref="IChemObject"/> - the object is automatically
        /// cast to <typeparamref name="T"/> type. 
        /// </summary>
        /// <typeparam name="T">Generic return type</typeparam>
        /// <param name="description">An object description of the property</param>
        /// <returns>The property</returns>
        /// <exception cref="System.InvalidCastException">If the wrong type is provided.</exception>
        /// <example>
        /// <code>
        ///     IAtom atom = new Atom("C");
        ///     atom.SetProperty("number", 1); // set an integer property
        ///
        ///     // access the property and cast to an int
        ///     int number = atom.GetProperty&lt;int&gt;("number");
        ///
        ///     // the type cannot be checked and so...
        ///     string number = atom.GetProperty&lt;string&gt;("number"); // InvalidCastException
        /// </code>
        /// </example>
        /// <seealso cref="SetProperty(object, object)"/>
        /// <seealso cref="GetProperties"/>
        /// <seealso cref="RemoveProperty(object)"/>
        T GetProperty<T>(object description);

        /// <summary>
        /// Returns a property for the <see cref="IChemObject"/> or <paramref name="defaultValue"/> if the <paramref name="description"/> key is not in it. 
        /// </summary>
        /// <typeparam name="T">Generic return type</typeparam>
        /// <param name="description">An object description of the property</param>
        /// <param name="defaultValue">A default value</param>
        /// <returns>The property</returns>
        T GetProperty<T>(object description, T defaultValue);

        /// <summary>
        /// Returns a <see cref="IDictionary{T,T}"/> with the <see cref="IChemObject"/>'s properties.
        /// </summary>
        /// <returns>The object's properties as an <see cref="IDictionary{T, T}"/></returns>
        /// <seealso cref="AddProperties(IEnumerable{KeyValuePair{object, object}})"/>
        IDictionary<object, object> GetProperties();

        /// <summary>
        /// Identifier (ID) of this object.
        /// </summary>
        string Id { get; set; }

        /// <summary>Flag that is set if the chemobject is placed (somewhere).</summary>
        bool IsPlaced { get; set; }

        /// <summary>Flag is set if chemobject has been visited</summary>
        bool IsVisited { get; set; }
    }

    internal struct ChemObjectFlagBag
    {
        public static ChemObjectFlagBag Save(IChemObject source)
        {
            var bag = new ChemObjectFlagBag();
            return bag;
        }

        public static void Restore(IChemObject dest, ChemObjectFlagBag savedFlags)
        {
            dest.IsPlaced = savedFlags.IsPlaced;
            dest.IsVisited = savedFlags.IsVisited;
        }

        public static void Transfer(IChemObject src, IChemObject dest)
        {
            var fs = Save(src);
            Restore(src, fs);
        }

        bool IsPlaced { get; set; }
        bool IsVisited { get; set; }
    }
}
