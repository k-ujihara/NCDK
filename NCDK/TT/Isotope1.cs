

// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2016-2017  Kazuya Ujihara <uzzy@users.sourceforge.net>

/* Copyright (C) 1997-2007  Christoph Steinbeck <steinbeck@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All I ask is that proper credit is given for my work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
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
 using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace NCDK.Default
{
	/// <summary>
	/// Used to store and retrieve data of a particular isotope.
	/// </summary>
	/// <example>
	/// For example, an carbon 13 isotope can be created with:
	/// <code>
	///   Isotope carbon = new Isotope("C", 13);
	/// </code>
	/// 
	/// A full specification can be constructed with:
	/// <code>
	///   // make deuterium
	///   Isotope carbon = new Isotope(1, "H", 2, 2.01410179, 100.0);
	/// </code>
	/// 
	/// Once instantiated all field not filled by passing parameters
	/// to the constructor are null. Isotopes can be configured by using
	/// the IsotopeFactory.Configure() method:
	/// <code>
	///   Isotope isotope = new Isotope("C", 13);
	///   IsotopeFactory f = IsotopeFactory.GetInstance(isotope.Builder);
	///   f.Configure(isotope);
	/// </code>
	/// </example>
	// @cdk.githash 
	// @author     steinbeck
	// @cdk.created    2001-08-21 
	// @cdk.keyword     isotope 
    [Serializable]
    public class Isotope
            : Element, IIsotope, ICloneable
    {
        private double? naturalAbundance;
        private double? exactMass;
        private int? massNumber;

		/// <summary>
		///  Constructor for the Isotope object.
		/// </summary>
		/// <param name="elementSymbol">The element symbol, "O" for Oxygen, etc.</param>
        public Isotope(string elementSymbol)
            : base(elementSymbol)
        {
        }

		/// <summary>
		///  Constructor for the Isotope object.
		/// </summary>
		/// <param name="atomicNumber">The atomic number of the isotope</param>
		/// <param name="elementSymbol">The element symbol, "O" for Oxygen, etc.</param>
		/// <param name="massNumber">The atomic mass of the isotope, 16 for Oxygen, e.g.</param>
		/// <param name="exactMass">The exact mass of the isotope, be a little more explicit here :-)</param>
		/// <param name="abundance">The natural abundance of the isotope</param>
		public Isotope(int atomicNumber, string elementSymbol, int massNumber, double exactMass, double abundance)
            : base(elementSymbol, atomicNumber)
        {
            this.massNumber = massNumber;
            this.exactMass = exactMass;
            this.naturalAbundance = abundance;
        }

		/// <summary>
		///  Constructor for the Isotope object.
		/// </summary>
		/// <param name="atomicNumber">The atomic number of the isotope, 8 for Oxygen</param>
		/// <param name="elementSymbol">The element symbol, "O" for Oxygen, etc.</param>
		/// <param name="exactMass">The exact mass of the isotope, be a little more explicit here :-)</param>
		/// <param name="abundance">The natural abundance of the isotope</param>
		public Isotope(int atomicNumber, string elementSymbol, double exactMass, double abundance)
            : this(elementSymbol, atomicNumber)
        {
            this.exactMass = exactMass;
            this.naturalAbundance = abundance;
        }

		/// <summary>
		/// Constructor for the Isotope object.
		/// </summary>
		/// <param name="elementSymbol">The element symbol, "O" for Oxygen, etc.</param>
		/// <param name="massNumber">The atomic mass of the isotope, 16 for Oxygen, e.g.</param>
        public Isotope(string elementSymbol, int massNumber)
            : base(elementSymbol)
        {
            this.massNumber = massNumber;
        }

		/// <summary>
		/// Constructs an empty by copying the symbol, atomic number,
		/// flags, and identifier from the given IElement. It does
		/// not copy the listeners and properties. If the element is
		/// an instanceof IIsotope, then the exact mass, natural
		/// abundance and mass number are copied too.
		/// </summary>
		/// <param name="element">IElement to copy information from</param>
        public Isotope(IElement element)
            : base(element)
        {
            var isotope = element as IIsotope;
            if (isotope != null)
            {
                this.exactMass = isotope.ExactMass;
                this.naturalAbundance = isotope.NaturalAbundance;
                this.massNumber = isotope.MassNumber;
            }
        }

		/// <summary>
		/// The NaturalAbundance attribute of the Isotope object.
		/// </summary>
		/// <example>
		/// Once instantiated all field not filled by passing parameters
		/// to the constructor are null. Isotopes can be configured by using
		/// the IsotopeFactory.configure() method:
		/// <code>
		///   Isotope isotope = new Isotope("C", 13);
		///   IsotopeFactory f = IsotopeFactory.GetInstance(isotope.Builder);
		///   f.Configure(isotope);
		/// </code>
		/// </example>
        public virtual double? NaturalAbundance
        {
            get { return naturalAbundance; }
            set
            {
                naturalAbundance = value;
                NotifyChanged();
            }
        }

		/// <summary>
		/// The ExactMass attribute of the Isotope object.
		/// </summary>
		/// <example>
		/// Once instantiated all field not filled by passing parameters
		/// to the constructor are null. Isotopes can be configured by using
		/// the IsotopeFactory.configure() method:
		/// <code>
		///   Isotope isotope = new Isotope("C", 13);
		///   IsotopeFactory f = IsotopeFactory.GetInstance(isotope.Builder);
		///   f.Configure(isotope);
		/// </code>
		/// </example>
        public virtual double? ExactMass
        {
            get { return exactMass; }
            set
            {
                exactMass = value;
                NotifyChanged();
            }
        }

		/// <summary>
		/// The atomic mass of this element.
		/// </summary>
		/// <example>
		/// Once instantiated all field not filled by passing parameters
		/// to the constructor are null. Isotopes can be configured by using
		/// the IsotopeFactory.configure() method:
		/// <code>
		///   Isotope isotope = new Isotope("C", 13);
		///   IsotopeFactory f = IsotopeFactory.GetInstance(isotope.Builder);
		///   f.Configure(isotope);
		/// </code>
		/// </example>
        public virtual int? MassNumber
        {
            get { return massNumber; }
            set
            {
                massNumber = value;
                NotifyChanged();
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Isotope(").Append(GetHashCode());
            if (MassNumber != null)
                sb.Append(", MN:").Append(MassNumber);
            if (ExactMass != null)
                sb.Append(", EM:").Append(ExactMass);
            if (NaturalAbundance != null)
                sb.Append(", AB:").Append(NaturalAbundance);
            sb.Append(", ").Append(base.ToString());
            sb.Append(')');
            return sb.ToString();
        }

		/// <summary>
		/// Compares an isotope with this isotope.
		/// </summary>
		/// <param name="obj">Object of type Isotope</param>
		/// <returns>true if the isotopes are equal</returns>
	    public override bool Compare(object obj)
        {
            var isotope = obj as Isotope;
            return isotope != null && base.Compare(obj)
                && isotope.MassNumber == MassNumber
                && NearlyEquals(isotope.ExactMass, ExactMass)
                && NearlyEquals(isotope.NaturalAbundance, NaturalAbundance);
        }

        private static bool NearlyEquals(double? a, double? b)
        {
            if (a.HasValue != b.HasValue)
                return false;
            if (a.HasValue && b.HasValue)
                if (Math.Abs(a.Value - b.Value) > 0.0000001)
                    return false;
            return true;
        }
    }
}
namespace NCDK.Silent
{
	/// <summary>
	/// Used to store and retrieve data of a particular isotope.
	/// </summary>
	/// <example>
	/// For example, an carbon 13 isotope can be created with:
	/// <code>
	///   Isotope carbon = new Isotope("C", 13);
	/// </code>
	/// 
	/// A full specification can be constructed with:
	/// <code>
	///   // make deuterium
	///   Isotope carbon = new Isotope(1, "H", 2, 2.01410179, 100.0);
	/// </code>
	/// 
	/// Once instantiated all field not filled by passing parameters
	/// to the constructor are null. Isotopes can be configured by using
	/// the IsotopeFactory.Configure() method:
	/// <code>
	///   Isotope isotope = new Isotope("C", 13);
	///   IsotopeFactory f = IsotopeFactory.GetInstance(isotope.Builder);
	///   f.Configure(isotope);
	/// </code>
	/// </example>
	// @cdk.githash 
	// @author     steinbeck
	// @cdk.created    2001-08-21 
	// @cdk.keyword     isotope 
    [Serializable]
    public class Isotope
            : Element, IIsotope, ICloneable
    {
        private double? naturalAbundance;
        private double? exactMass;
        private int? massNumber;

		/// <summary>
		///  Constructor for the Isotope object.
		/// </summary>
		/// <param name="elementSymbol">The element symbol, "O" for Oxygen, etc.</param>
        public Isotope(string elementSymbol)
            : base(elementSymbol)
        {
        }

		/// <summary>
		///  Constructor for the Isotope object.
		/// </summary>
		/// <param name="atomicNumber">The atomic number of the isotope</param>
		/// <param name="elementSymbol">The element symbol, "O" for Oxygen, etc.</param>
		/// <param name="massNumber">The atomic mass of the isotope, 16 for Oxygen, e.g.</param>
		/// <param name="exactMass">The exact mass of the isotope, be a little more explicit here :-)</param>
		/// <param name="abundance">The natural abundance of the isotope</param>
		public Isotope(int atomicNumber, string elementSymbol, int massNumber, double exactMass, double abundance)
            : base(elementSymbol, atomicNumber)
        {
            this.massNumber = massNumber;
            this.exactMass = exactMass;
            this.naturalAbundance = abundance;
        }

		/// <summary>
		///  Constructor for the Isotope object.
		/// </summary>
		/// <param name="atomicNumber">The atomic number of the isotope, 8 for Oxygen</param>
		/// <param name="elementSymbol">The element symbol, "O" for Oxygen, etc.</param>
		/// <param name="exactMass">The exact mass of the isotope, be a little more explicit here :-)</param>
		/// <param name="abundance">The natural abundance of the isotope</param>
		public Isotope(int atomicNumber, string elementSymbol, double exactMass, double abundance)
            : this(elementSymbol, atomicNumber)
        {
            this.exactMass = exactMass;
            this.naturalAbundance = abundance;
        }

		/// <summary>
		/// Constructor for the Isotope object.
		/// </summary>
		/// <param name="elementSymbol">The element symbol, "O" for Oxygen, etc.</param>
		/// <param name="massNumber">The atomic mass of the isotope, 16 for Oxygen, e.g.</param>
        public Isotope(string elementSymbol, int massNumber)
            : base(elementSymbol)
        {
            this.massNumber = massNumber;
        }

		/// <summary>
		/// Constructs an empty by copying the symbol, atomic number,
		/// flags, and identifier from the given IElement. It does
		/// not copy the listeners and properties. If the element is
		/// an instanceof IIsotope, then the exact mass, natural
		/// abundance and mass number are copied too.
		/// </summary>
		/// <param name="element">IElement to copy information from</param>
        public Isotope(IElement element)
            : base(element)
        {
            var isotope = element as IIsotope;
            if (isotope != null)
            {
                this.exactMass = isotope.ExactMass;
                this.naturalAbundance = isotope.NaturalAbundance;
                this.massNumber = isotope.MassNumber;
            }
        }

		/// <summary>
		/// The NaturalAbundance attribute of the Isotope object.
		/// </summary>
		/// <example>
		/// Once instantiated all field not filled by passing parameters
		/// to the constructor are null. Isotopes can be configured by using
		/// the IsotopeFactory.configure() method:
		/// <code>
		///   Isotope isotope = new Isotope("C", 13);
		///   IsotopeFactory f = IsotopeFactory.GetInstance(isotope.Builder);
		///   f.Configure(isotope);
		/// </code>
		/// </example>
        public virtual double? NaturalAbundance
        {
            get { return naturalAbundance; }
            set
            {
                naturalAbundance = value;
            }
        }

		/// <summary>
		/// The ExactMass attribute of the Isotope object.
		/// </summary>
		/// <example>
		/// Once instantiated all field not filled by passing parameters
		/// to the constructor are null. Isotopes can be configured by using
		/// the IsotopeFactory.configure() method:
		/// <code>
		///   Isotope isotope = new Isotope("C", 13);
		///   IsotopeFactory f = IsotopeFactory.GetInstance(isotope.Builder);
		///   f.Configure(isotope);
		/// </code>
		/// </example>
        public virtual double? ExactMass
        {
            get { return exactMass; }
            set
            {
                exactMass = value;
            }
        }

		/// <summary>
		/// The atomic mass of this element.
		/// </summary>
		/// <example>
		/// Once instantiated all field not filled by passing parameters
		/// to the constructor are null. Isotopes can be configured by using
		/// the IsotopeFactory.configure() method:
		/// <code>
		///   Isotope isotope = new Isotope("C", 13);
		///   IsotopeFactory f = IsotopeFactory.GetInstance(isotope.Builder);
		///   f.Configure(isotope);
		/// </code>
		/// </example>
        public virtual int? MassNumber
        {
            get { return massNumber; }
            set
            {
                massNumber = value;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Isotope(").Append(GetHashCode());
            if (MassNumber != null)
                sb.Append(", MN:").Append(MassNumber);
            if (ExactMass != null)
                sb.Append(", EM:").Append(ExactMass);
            if (NaturalAbundance != null)
                sb.Append(", AB:").Append(NaturalAbundance);
            sb.Append(", ").Append(base.ToString());
            sb.Append(')');
            return sb.ToString();
        }

		/// <summary>
		/// Compares an isotope with this isotope.
		/// </summary>
		/// <param name="obj">Object of type Isotope</param>
		/// <returns>true if the isotopes are equal</returns>
	    public override bool Compare(object obj)
        {
            var isotope = obj as Isotope;
            return isotope != null && base.Compare(obj)
                && isotope.MassNumber == MassNumber
                && NearlyEquals(isotope.ExactMass, ExactMass)
                && NearlyEquals(isotope.NaturalAbundance, NaturalAbundance);
        }

        private static bool NearlyEquals(double? a, double? b)
        {
            if (a.HasValue != b.HasValue)
                return false;
            if (a.HasValue && b.HasValue)
                if (Math.Abs(a.Value - b.Value) > 0.0000001)
                    return false;
            return true;
        }
    }
}
