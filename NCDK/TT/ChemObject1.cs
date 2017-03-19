

// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2015-2017  Kazuya Ujihara

/* Copyright (C) 1997-2007  Christoph Steinbeck <steinbeck@users.sf.net>
 *
 *  Contact: cdk-devel@lists.sourceforge.net
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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */

using NCDK.Common.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NCDK.Default
{
    /// <summary>
    /// The base class for all chemical objects in this cdk. It provides methods for
    /// adding listeners and for their notification of events, as well a a hash
    /// table for administration of physical or chemical properties
    /// </summary>
    // @author        steinbeck
    // @cdk.githash
    // @cdk.module data
    [Serializable]
    public class ChemObject
        : IChemObject
    {
#if DEBUG
        private static Type[] AcceptablePropertyKeyTypes { get; } = new Type[]
        {
            typeof(string),
            typeof(NCDK.Dict.DictRef),
            typeof(NCDK.QSAR.DescriptorSpecification),
        };
#endif
        private ICollection<IChemObjectListener> listeners;
        /// <summary>
        /// List for listener administration.
        /// </summary>
        public ICollection<IChemObjectListener> Listeners 
        { 
            get
            {
                if (listeners == null)
                    listeners = new HashSet<IChemObjectListener>(); 
                return listeners;
            }
        }
        public bool Notification { get; set; } = true;
        public IChemObjectBuilder Builder { get; protected set; } = ChemObjectBuilder.Instance;

        private bool isPlaced;
        public bool IsPlaced
        {
            get { return isPlaced; }
            set
            {
                isPlaced = value;
                NotifyChanged();
            }
        }

        private bool isVisited;
        /// <summary>
        /// Flag is set if chemobject has been visited
        /// </summary>
        public bool IsVisited
        {
            get { return isVisited; }
            set
            {
                isVisited = value;
                NotifyChanged();
            }
        }

        /// <summary>
        /// Constructs a new IChemObject.
        /// </summary>
        public ChemObject()
            : this(null)
        {
        }

        /// <summary>
        /// Constructs a new IChemObject by copying the flags, and the. It does not copy the listeners and properties.
        /// </summary>
        /// <param name="chemObject">the object to copy</param>
        public ChemObject(IChemObject chemObject)
        {
            if (chemObject != null)
            {
                // copy the flags
                IsVisited = chemObject.IsVisited;
                IsPlaced = chemObject.IsPlaced;
                // copy the identifier
                id = chemObject.Id;
            }
        }

        /// <summary>
        /// This should be triggered by an method that changes the content of an object
        ///  to that the registered listeners can react to it.
        /// </summary>
        public void NotifyChanged()
        {
            if (Notification)
                NotifyChanged(new ChemObjectChangeEventArgs(this));
        }

        /// <summary>
        /// This should be triggered by an method that changes the content of an object
        /// to that the registered listeners can react to it. This is a version of
        /// NotifyChanged() which allows to propagate a change event while preserving
        /// the original origin.
        /// </summary>
        /// <param name="evt">A ChemObjectChangeEvent pointing to the source of where the change happened</param>
        public virtual void NotifyChanged(ChemObjectChangeEventArgs evt)
        {
            if (Notification)
            {
                foreach (var listener in Listeners)
                {
                    listener.OnStateChanged(evt);
                }
            }
        }

        /// <summary>
        /// A dictionary for the storage of any kind of properties of this <see cref="IChemObject"/>.
        /// </summary>
        IDictionary<object, object> properties;

        private void InitProperties()
        {
            properties = new Dictionary<object, object>();
        }

        /// <summary>
        /// Sets a property for a IChemObject.
        /// </summary>
        /// <param name="description"> An object description of the property (most likely an unique string)</param>
        /// <param name="property">An object with the property itself</param>
        /// <seealso cref="GetProperty{T}(object)"/>
        /// <seealso cref="RemoveProperty(object)"/>
        public virtual void SetProperty(object description, object property)
        {
#if DEBUG
            if (description != null && !AcceptablePropertyKeyTypes.Contains(description.GetType()))
                throw new Exception();
#endif
            if (this.properties == null)
                InitProperties();
            properties[description] = property;
            NotifyChanged();
        }

        /// <summary>
        /// Removes a property for a IChemObject.
        /// </summary>
        /// <param name="description">The object description of the property (most likely an unique string)</param>
        /// <seealso cref="SetProperty(object, object)"/>
        /// <seealso cref="GetProperty{T}(object)"/>
        public virtual void RemoveProperty(object description)
        {
#if DEBUG
            if (!AcceptablePropertyKeyTypes.Contains(description.GetType()))
                throw new Exception();
#endif
            if (this.properties == null)
                return;
            var removed = properties.Remove(description);
            if (removed)
                NotifyChanged();
        }

        /// <summary>
        /// Returns a property for the IChemObject.
        /// </summary>
        /// <param name="description">An object description of the property (most likely an unique string)</param>
        /// <returns>The object containing the property. Returns null if propert is not set.</returns>
        /// <seealso cref="SetProperty(object, object)"/>
        /// <seealso cref="RemoveProperty(object)"/>
        public virtual T GetProperty<T>(object description)
        {
#if DEBUG
            if (!AcceptablePropertyKeyTypes.Contains(description.GetType()))
                throw new Exception();
#endif
            return GetProperty(description, default(T));
        }

        public virtual T GetProperty<T>(object description, T defaultValue)
        {
#if DEBUG
            if (!AcceptablePropertyKeyTypes.Contains(description.GetType()))
                throw new Exception();
#endif
            if (this.properties == null)
                return defaultValue;
            object property;
            if (properties.TryGetValue(description, out property))
                return (T)property;
            return defaultValue;
        }

        private static readonly IDictionary<object, object> EmptyProperties = new ReadOnlyDictionary<object, object>(new Dictionary<object, object>(0));

        /// <summary>
        /// A <see cref="IDictionary{T, T}"/> with the <see cref="IChemObject"/>'s properties.
        /// </summary>
        /// <returns>The object's properties as an <see cref="IDictionary{T, T}"/></returns>
        /// <seealso cref="AddProperties(IEnumerable{KeyValuePair{object, object}})"/>
        public virtual IDictionary<object, object> GetProperties() 
        {
            if (this.properties == null)
                return EmptyProperties;
            return this.properties;
        }

        public void SetProperties(IEnumerable<KeyValuePair<object, object>> properties)
        {
            this.properties = null;
            if (properties == null)
                return;
            AddProperties(properties);
        }

        /// <summary>
        /// Sets the properties of this object.
        /// </summary>
        /// <param name="properties">a Dictionary specifying the property values</param>
        /// <seealso cref="GetProperties"/>
        public virtual void AddProperties(IEnumerable<KeyValuePair<object, object>> properties)
        {
            if (properties == null)
                return;
            if (this.properties == null)
                InitProperties();
            foreach (var pair in properties)
                this.properties[pair.Key] = pair.Value;
            NotifyChanged();
        }

        public virtual object Clone()
        {
            return Clone(new CDKObjectMap());
        }

        public virtual ICDKObject Clone(CDKObjectMap map)
        {
            var clone = (ChemObject)MemberwiseClone();

            // clone the properties - using the Dictionary copy constructor
            // this doesn't deep clone the keys/values but this wasn't happening
            // already
            clone.SetProperties(this.properties);
            // delete all listeners
            clone.listeners = null;
            return clone;
        }

        /// <summary>
        /// Compares a <see cref="IChemObject"/> with this <see cref="IChemObject"/>.
        /// </summary>
        /// <param name="obj">Object of type <see cref="AtomType"/></param>
        /// <returns><see langword="true"/> if the atom types are equal</returns>
        public virtual bool Compare(object obj)
        {
            var o = obj as IChemObject;
            return o == null ? false : Id == o.Id;
        }

        private string id;
        /// <summary>
        /// The identifier (ID) of this object.
        /// </summary>
        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                NotifyChanged();
            }
        }
    }
}
namespace NCDK.Silent
{
    /// <summary>
    /// The base class for all chemical objects in this cdk. It provides methods for
    /// adding listeners and for their notification of events, as well a a hash
    /// table for administration of physical or chemical properties
    /// </summary>
    // @author        steinbeck
    // @cdk.githash
    // @cdk.module data
    [Serializable]
    public class ChemObject
        : IChemObject
    {
#if DEBUG
        private static Type[] AcceptablePropertyKeyTypes { get; } = new Type[]
        {
            typeof(string),
            typeof(NCDK.Dict.DictRef),
            typeof(NCDK.QSAR.DescriptorSpecification),
        };
#endif
        private ICollection<IChemObjectListener> listeners;
        /// <summary>
        /// List for listener administration.
        /// </summary>
        public ICollection<IChemObjectListener> Listeners 
        { 
            get
            {
                if (listeners == null)
                    listeners = new ImmutableCollection<IChemObjectListener>(); 
                return listeners;
            }
        }
        public bool Notification { get; set; } = true;
        public IChemObjectBuilder Builder { get; protected set; } = ChemObjectBuilder.Instance;

        private bool isPlaced;
        public bool IsPlaced
        {
            get { return isPlaced; }
            set
            {
                isPlaced = value;
            }
        }

        private bool isVisited;
        /// <summary>
        /// Flag is set if chemobject has been visited
        /// </summary>
        public bool IsVisited
        {
            get { return isVisited; }
            set
            {
                isVisited = value;
            }
        }

        /// <summary>
        /// Constructs a new IChemObject.
        /// </summary>
        public ChemObject()
            : this(null)
        {
        }

        /// <summary>
        /// Constructs a new IChemObject by copying the flags, and the. It does not copy the listeners and properties.
        /// </summary>
        /// <param name="chemObject">the object to copy</param>
        public ChemObject(IChemObject chemObject)
        {
            if (chemObject != null)
            {
                // copy the flags
                IsVisited = chemObject.IsVisited;
                IsPlaced = chemObject.IsPlaced;
                // copy the identifier
                id = chemObject.Id;
            }
        }

        /// <summary>
        /// This should be triggered by an method that changes the content of an object
        ///  to that the registered listeners can react to it.
        /// </summary>
        public void NotifyChanged()
        {
        }

        /// <summary>
        /// This should be triggered by an method that changes the content of an object
        /// to that the registered listeners can react to it. This is a version of
        /// NotifyChanged() which allows to propagate a change event while preserving
        /// the original origin.
        /// </summary>
        /// <param name="evt">A ChemObjectChangeEvent pointing to the source of where the change happened</param>
        public virtual void NotifyChanged(ChemObjectChangeEventArgs evt)
        {
        }

        /// <summary>
        /// A dictionary for the storage of any kind of properties of this <see cref="IChemObject"/>.
        /// </summary>
        IDictionary<object, object> properties;

        private void InitProperties()
        {
            properties = new Dictionary<object, object>();
        }

        /// <summary>
        /// Sets a property for a IChemObject.
        /// </summary>
        /// <param name="description"> An object description of the property (most likely an unique string)</param>
        /// <param name="property">An object with the property itself</param>
        /// <seealso cref="GetProperty{T}(object)"/>
        /// <seealso cref="RemoveProperty(object)"/>
        public virtual void SetProperty(object description, object property)
        {
#if DEBUG
            if (description != null && !AcceptablePropertyKeyTypes.Contains(description.GetType()))
                throw new Exception();
#endif
            if (this.properties == null)
                InitProperties();
            properties[description] = property;
        }

        /// <summary>
        /// Removes a property for a IChemObject.
        /// </summary>
        /// <param name="description">The object description of the property (most likely an unique string)</param>
        /// <seealso cref="SetProperty(object, object)"/>
        /// <seealso cref="GetProperty{T}(object)"/>
        public virtual void RemoveProperty(object description)
        {
#if DEBUG
            if (!AcceptablePropertyKeyTypes.Contains(description.GetType()))
                throw new Exception();
#endif
            if (this.properties == null)
                return;
            var removed = properties.Remove(description);
        }

        /// <summary>
        /// Returns a property for the IChemObject.
        /// </summary>
        /// <param name="description">An object description of the property (most likely an unique string)</param>
        /// <returns>The object containing the property. Returns null if propert is not set.</returns>
        /// <seealso cref="SetProperty(object, object)"/>
        /// <seealso cref="RemoveProperty(object)"/>
        public virtual T GetProperty<T>(object description)
        {
#if DEBUG
            if (!AcceptablePropertyKeyTypes.Contains(description.GetType()))
                throw new Exception();
#endif
            return GetProperty(description, default(T));
        }

        public virtual T GetProperty<T>(object description, T defaultValue)
        {
#if DEBUG
            if (!AcceptablePropertyKeyTypes.Contains(description.GetType()))
                throw new Exception();
#endif
            if (this.properties == null)
                return defaultValue;
            object property;
            if (properties.TryGetValue(description, out property))
                return (T)property;
            return defaultValue;
        }

        private static readonly IDictionary<object, object> EmptyProperties = new ReadOnlyDictionary<object, object>(new Dictionary<object, object>(0));

        /// <summary>
        /// A <see cref="IDictionary{T, T}"/> with the <see cref="IChemObject"/>'s properties.
        /// </summary>
        /// <returns>The object's properties as an <see cref="IDictionary{T, T}"/></returns>
        /// <seealso cref="AddProperties(IEnumerable{KeyValuePair{object, object}})"/>
        public virtual IDictionary<object, object> GetProperties() 
        {
            if (this.properties == null)
                return EmptyProperties;
            return this.properties;
        }

        public void SetProperties(IEnumerable<KeyValuePair<object, object>> properties)
        {
            this.properties = null;
            if (properties == null)
                return;
            AddProperties(properties);
        }

        /// <summary>
        /// Sets the properties of this object.
        /// </summary>
        /// <param name="properties">a Dictionary specifying the property values</param>
        /// <seealso cref="GetProperties"/>
        public virtual void AddProperties(IEnumerable<KeyValuePair<object, object>> properties)
        {
            if (properties == null)
                return;
            if (this.properties == null)
                InitProperties();
            foreach (var pair in properties)
                this.properties[pair.Key] = pair.Value;
        }

        public virtual object Clone()
        {
            return Clone(new CDKObjectMap());
        }

        public virtual ICDKObject Clone(CDKObjectMap map)
        {
            var clone = (ChemObject)MemberwiseClone();

            // clone the properties - using the Dictionary copy constructor
            // this doesn't deep clone the keys/values but this wasn't happening
            // already
            clone.SetProperties(this.properties);
            // delete all listeners
            clone.listeners = null;
            return clone;
        }

        /// <summary>
        /// Compares a <see cref="IChemObject"/> with this <see cref="IChemObject"/>.
        /// </summary>
        /// <param name="obj">Object of type <see cref="AtomType"/></param>
        /// <returns><see langword="true"/> if the atom types are equal</returns>
        public virtual bool Compare(object obj)
        {
            var o = obj as IChemObject;
            return o == null ? false : Id == o.Id;
        }

        private string id;
        /// <summary>
        /// The identifier (ID) of this object.
        /// </summary>
        public string Id
        {
            get { return id; }
            set
            {
                id = value;
            }
        }
    }
}
