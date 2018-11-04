



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
 */

using NCDK.Common.Serialization;
using NCDK.Config;
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace NCDK.Default
{
    /// <summary>
    /// Implements the idea of an element in the periodic table.
    /// </summary>
    /// <example>
    /// Use the IsotopeFactory to get a ready-to-use elements
    /// by symbol or atomic number:
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.TT.Element_Example.cs"]/*' />
    /// </example>
    // @cdk.keyword element
    public class Element
        : ChemObject, IElement, ICloneable, ISerializable
    {
        /// <summary>The atomic number for this element giving their position in the periodic table.</summary>
        internal int? atomicNumber;

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddNullableValue(nameof(atomicNumber), atomicNumber);
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected Element(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            atomicNumber = info.GetNullable<int>(nameof(atomicNumber));
        }

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
            : this(symbol, SymbolToAtomicNumber(symbol))
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
            this.atomicNumber = atomicNumber;
        }

        /// <summary>
        /// The atomic number of this element.
        /// </summary>
        /// <example>
        /// Once instantiated all field not filled by passing parameters
        /// to the constructor are null. Elements can be configured by using
        /// the <see cref="IsotopeFactory.Configure(IAtom)"/> method:
        /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.TT.Element_Example.cs+AtomicNumber"]/*' />
        /// </example>
        public virtual int? AtomicNumber
        {
            get => atomicNumber;

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
            get
            {
                if (atomicNumber == null)
                    return null;
                if (atomicNumber.Value == 0)
                    return "R";
                return NaturalElement.OfNumber(atomicNumber.Value).Symbol;
            }

            set
            {
                AtomicNumber = SymbolToAtomicNumber(value);
                NotifyChanged();
            }
        }

        private static int? SymbolToAtomicNumber(string symbol)
        {
            if (symbol == null)
                return null;
            else
                return NaturalElement.OfString(symbol).AtomicNumber;
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
            if (!(obj is Element elem))
                return false;
            if (!base.Compare(obj))
                return false;
            return object.Equals(AtomicNumber, elem.AtomicNumber);
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
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.TT.Element_Example.cs"]/*' />
    /// </example>
    // @cdk.keyword element
    public class Element
        : ChemObject, IElement, ICloneable, ISerializable
    {
        /// <summary>The atomic number for this element giving their position in the periodic table.</summary>
        internal int? atomicNumber;

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddNullableValue(nameof(atomicNumber), atomicNumber);
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected Element(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            atomicNumber = info.GetNullable<int>(nameof(atomicNumber));
        }

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
            : this(symbol, SymbolToAtomicNumber(symbol))
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
            this.atomicNumber = atomicNumber;
        }

        /// <summary>
        /// The atomic number of this element.
        /// </summary>
        /// <example>
        /// Once instantiated all field not filled by passing parameters
        /// to the constructor are null. Elements can be configured by using
        /// the <see cref="IsotopeFactory.Configure(IAtom)"/> method:
        /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.TT.Element_Example.cs+AtomicNumber"]/*' />
        /// </example>
        public virtual int? AtomicNumber
        {
            get => atomicNumber;

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
            get
            {
                if (atomicNumber == null)
                    return null;
                if (atomicNumber.Value == 0)
                    return "R";
                return NaturalElement.OfNumber(atomicNumber.Value).Symbol;
            }

            set
            {
                AtomicNumber = SymbolToAtomicNumber(value);
            }
        }

        private static int? SymbolToAtomicNumber(string symbol)
        {
            if (symbol == null)
                return null;
            else
                return NaturalElement.OfString(symbol).AtomicNumber;
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
            if (!(obj is Element elem))
                return false;
            if (!base.Compare(obj))
                return false;
            return object.Equals(AtomicNumber, elem.AtomicNumber);
        }
    }
}
