/* Copyright (C) 2012  Egon Willighagen <egonw@users.sf.net>
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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NCDK.Config
{
    /// <summary>
    /// A read-only class used by <see cref="Elements"/> for the natural elements. This class is not to be used than by only <see cref="Elements"/>.
    /// </summary>
    // @author      egonw
    // @cdk.module  core
    // @cdk.githash
    internal sealed class NaturalElement 
        : IElement
    {
        private readonly string  symbol;
        private readonly int? atomicNumber;

        public NaturalElement(string element, int? atomicNumber)
        {
            this.symbol = element;
            this.atomicNumber = atomicNumber;
        }

        public ICollection<IChemObjectListener> Listeners { get; } = new ReadOnlyCollection<IChemObjectListener>(new List<IChemObjectListener>());
        public bool Notification { get { return false; } set { } }
        public void NotifyChanged() { }

        public bool IsPlaced { get { return false; } set { } }
        public bool IsVisited { get { return false; } set { } }

        public int? AtomicNumber { get { return atomicNumber; } set { } }
        public string Symbol { get { return symbol; } set { } }

        public string Id { get { return null; } set { } }
        public IChemObjectBuilder Builder { get { return null; } }

        public void RemoveProperty(object description) { }
        public T GetProperty<T>(object description) => default(T);
        public T GetProperty<T>(object description, T defaultValue) => default(T);
        public void SetProperty(object key, object value) { }
        public void SetProperties(IEnumerable<KeyValuePair<object, object>> properties) { }
        public void AddProperties(IEnumerable<KeyValuePair<object, object>> properties) { }
        private static readonly IDictionary<object, object> emptyMap = new ReadOnlyDictionary<object, object>(new Dictionary<object, object>());
        public IDictionary<object, object> GetProperties() => emptyMap;

        public bool Compare(object obj) => this == obj;

        public object Clone() => this;
        public ICDKObject Clone(CDKObjectMap map) => this;
    }
}        
