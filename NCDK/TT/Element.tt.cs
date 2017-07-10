

// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2016-2017  Kazuya Ujihara <ujihara.kazuya@gmail.com>

/* Copyright (C) 1997-2007  Christoph Steinbeck <steinbeck@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */
using NCDK.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace NCDK.Default
{
    /// <summary>
    /// Implements the idea of an element in the periodic table.
    /// </summary>
    /// <example>
    /// Use the IsotopeFactory to get a ready-to-use elements
    /// by symbol or atomic number:
    /// <code>
    ///   IsotopeFactory f = IsotopeFactory.getInstance(new Element().Builder);
    ///   Element e1 = f.GetElement("C");
    ///   Element e2 = f.GetElement(12);
    /// </code></example>
    // @cdk.githash 
    // @cdk.keyword element 
    [Serializable]
    public class Element
        : ChemObject, IElement, ICloneable
    {
        /// <summary>The element symbol for this element as listed in the periodic table.</summary>
        internal string symbol;

        /// <summary>The atomic number for this element giving their position in the periodic table.</summary>
        internal int? atomicNumber;

        /// <summary>
        /// Constructs an empty Element.
        /// </summary>
        public Element()
            : this(null, null)
        {
        }

        /// <summary>
        /// Constructs an empty by copying the symbol, atomic number,
        /// flags, and identifier from the given IElement. It does
        /// not copy the listeners and properties.
        /// </summary>
        /// <param name="element">IElement to copy information from</param>
        public Element(IElement element)
            : this(element.Symbol, element.AtomicNumber)
        {
        }

        /// <summary>
        /// Constructs an Element with a given
        /// element symbol.
        /// </summary>
        /// <param name="symbol">The element symbol that this element should have.</param>
        public Element(string symbol)
            : this(symbol, Elements.OfString(symbol).AtomicNumber)
        {
        }

        /// <summary>
        /// Constructs an Element with a given element symbol,
        /// atomic number and atomic mass.
        /// </summary>
        /// <param name="symbol">The element symbol of this element.</param>
        /// <param name="atomicNumber">The atomicNumber of this element.</param>
        public Element(string symbol, int? atomicNumber)
            : base()
        {
            this.symbol = symbol;
            this.atomicNumber = atomicNumber;
        }

        /// <summary>
        /// The atomic number of this element.
        /// </summary>
        /// <example>
        /// Once instantiated all field not filled by passing parameters
        /// to the constructor are null. Elements can be configured by using
        /// the <see cref="IsotopeFactory.Configure(IAtom)"/> method:
        /// <code>
        ///   Element element = new Element("C");
        ///   IsotopeFactory f = IsotopeFactory.GetInstance(element.Builder);
        ///   f.Configure(element);
        /// </code>
        /// </example>
        public virtual int? AtomicNumber
        {
            get { return atomicNumber; }

            set
            {
                atomicNumber = value;
                NotifyChanged();
            }
        }

        /// <summary>
        /// The element symbol of this element.
        /// </summary>
        /// <returns>The element symbol of this element. <see langword="null"/> if unset.</returns>
        public virtual string Symbol
        {
            get { return symbol; }

            set
            {
                symbol = value;
                NotifyChanged();
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Element(").Append(GetHashCode());
            if (Symbol != null)
                sb.Append(", S:").Append(Symbol);
            if (Id != null)
                sb.Append(", ID:").Append(Id);
            if (AtomicNumber != null)
                sb.Append(", AN:").Append(AtomicNumber);
            sb.Append(')');
            return sb.ToString();
        }

        /// <summary>
        /// Compares an Element with this Element.
        /// </summary>
        /// <param name="obj">Object of type AtomType</param>
        /// <returns>true if the atom types are equal</returns>
        public override bool Compare(object obj)
        {
            var elem = obj as Element;
            if (elem == null)
                return false;
            if (!base.Compare(obj))
                return false;
            return AtomicNumber == elem.AtomicNumber && Symbol == elem.Symbol;
        }
    }
}
namespace NCDK.Silent
{
    /// <summary>
    /// Implements the idea of an element in the periodic table.
    /// </summary>
    /// <example>
    /// Use the IsotopeFactory to get a ready-to-use elements
    /// by symbol or atomic number:
    /// <code>
    ///   IsotopeFactory f = IsotopeFactory.getInstance(new Element().Builder);
    ///   Element e1 = f.GetElement("C");
    ///   Element e2 = f.GetElement(12);
    /// </code></example>
    // @cdk.githash 
    // @cdk.keyword element 
    [Serializable]
    public class Element
        : ChemObject, IElement, ICloneable
    {
        /// <summary>The element symbol for this element as listed in the periodic table.</summary>
        internal string symbol;

        /// <summary>The atomic number for this element giving their position in the periodic table.</summary>
        internal int? atomicNumber;

        /// <summary>
        /// Constructs an empty Element.
        /// </summary>
        public Element()
            : this(null, null)
        {
        }

        /// <summary>
        /// Constructs an empty by copying the symbol, atomic number,
        /// flags, and identifier from the given IElement. It does
        /// not copy the listeners and properties.
        /// </summary>
        /// <param name="element">IElement to copy information from</param>
        public Element(IElement element)
            : this(element.Symbol, element.AtomicNumber)
        {
        }

        /// <summary>
        /// Constructs an Element with a given
        /// element symbol.
        /// </summary>
        /// <param name="symbol">The element symbol that this element should have.</param>
        public Element(string symbol)
            : this(symbol, Elements.OfString(symbol).AtomicNumber)
        {
        }

        /// <summary>
        /// Constructs an Element with a given element symbol,
        /// atomic number and atomic mass.
        /// </summary>
        /// <param name="symbol">The element symbol of this element.</param>
        /// <param name="atomicNumber">The atomicNumber of this element.</param>
        public Element(string symbol, int? atomicNumber)
            : base()
        {
            this.symbol = symbol;
            this.atomicNumber = atomicNumber;
        }

        /// <summary>
        /// The atomic number of this element.
        /// </summary>
        /// <example>
        /// Once instantiated all field not filled by passing parameters
        /// to the constructor are null. Elements can be configured by using
        /// the <see cref="IsotopeFactory.Configure(IAtom)"/> method:
        /// <code>
        ///   Element element = new Element("C");
        ///   IsotopeFactory f = IsotopeFactory.GetInstance(element.Builder);
        ///   f.Configure(element);
        /// </code>
        /// </example>
        public virtual int? AtomicNumber
        {
            get { return atomicNumber; }

            set
            {
                atomicNumber = value;
            }
        }

        /// <summary>
        /// The element symbol of this element.
        /// </summary>
        /// <returns>The element symbol of this element. <see langword="null"/> if unset.</returns>
        public virtual string Symbol
        {
            get { return symbol; }

            set
            {
                symbol = value;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Element(").Append(GetHashCode());
            if (Symbol != null)
                sb.Append(", S:").Append(Symbol);
            if (Id != null)
                sb.Append(", ID:").Append(Id);
            if (AtomicNumber != null)
                sb.Append(", AN:").Append(AtomicNumber);
            sb.Append(')');
            return sb.ToString();
        }

        /// <summary>
        /// Compares an Element with this Element.
        /// </summary>
        /// <param name="obj">Object of type AtomType</param>
        /// <returns>true if the atom types are equal</returns>
        public override bool Compare(object obj)
        {
            var elem = obj as Element;
            if (elem == null)
                return false;
            if (!base.Compare(obj))
                return false;
            return AtomicNumber == elem.AtomicNumber && Symbol == elem.Symbol;
        }
    }
}
