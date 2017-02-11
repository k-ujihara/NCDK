/* Copyright (C) 2012-2013  Egon Willighagen <egonw@users.sf.net>
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Linq;
using NCDK.Tools;
using System.Collections.ObjectModel;

namespace NCDK.Config
{
    /**
    /// A read-only class used by {@link Isotopes} for the natural elements. This class is not to
    /// be used than by only {@link Isotopes}.
     *
    /// @author      egonw
    /// @cdk.module  core
    /// @cdk.githash
     */
#if TEST
    public
#endif
    sealed class BODRIsotope
        : IIsotope
    {
        private string element;
        private int? atomicNumber;
        private double? naturalAbundance;
        private double? exactMass;
        private int? massNumber;

        public BODRIsotope(string element, int? atomicNumber, int? massNumber, double? exactMass,
                double? naturalAbundance)
        {
            this.element = element;
            this.atomicNumber = atomicNumber;
            this.massNumber = massNumber;
            this.naturalAbundance = naturalAbundance;
            this.exactMass = exactMass;
        }

        public bool Compare(object obj) => this == obj;

        // ignored event
        public ICollection<IChemObjectListener> Listeners { get; } = new ReadOnlyCollection<IChemObjectListener>(new List<IChemObjectListener>());
        public bool Notification { get { return false; } set { } }
        public void NotifyChanged() { }

        // unsupported methods
        public void SetProperty(object description, object property) { }
        public void SetProperties(IEnumerable<KeyValuePair<object, object>> properties) { }
        public void AddProperties(IEnumerable<KeyValuePair<object, object>> properties) { }
        public void RemoveProperty(object description) { }
        public T GetProperty<T>(object description) => default(T);
        public T GetProperty<T>(object description, T defaultValue) => default(T);
        public IDictionary<object, object> GetProperties() => null;

        public bool IsPlaced { get { return false; } set { } }
        public bool IsVisited { get { return false; } set { } }

        public string Id
        {
            get { return null; }
            set { }
        }

        public IChemObjectBuilder Builder => null;

        public int? AtomicNumber
        {
            get { return atomicNumber; }
            set { }
        }

        public string Symbol
        {
            get { return element; }
            set { }
        }

        public double? NaturalAbundance
        {
            get { return naturalAbundance; }
            set { }
        }

        public double? ExactMass
        {
            get { return exactMass; }
            set { }
        }

        public int? MassNumber
        {
            get { return massNumber; }
            set { }
        }

        public object Clone() => this;
        public ICDKObject Clone(CDKObjectMap map)  => this;
    }
}

