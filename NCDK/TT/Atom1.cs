

// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2015-2017  Kazuya Ujihara

/* Copyright (C) 2000-2007  Christoph Steinbeck <steinbeck@users.sf.net>
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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Config;
using System;
using NCDK.Numerics;
using System.Text;

namespace NCDK.Default
{
    /// <summary>
    /// Represents the idea of an chemical atom.
    /// </summary>
    /// <example>
    /// An Atom class is instantiated with at least the atom symbol:
    /// <code>
    ///   Atom a = new Atom("C");
    /// </code>
    ///
    /// Once instantiated all field not filled by passing parameters
    /// to the constructor are null. Atoms can be configured by using
    /// the IsotopeFactory.configure() method:
    /// <code>
    ///   IsotopeFactory if = IsotopeFactory.getInstance(a.getNewBuilder());
    ///   if.configure(a);
    /// </code>
    ///
    /// More examples about using this class can be found in the
    /// Junit test for this class.
    /// </example>
    /// <seealso cref="NCDK.Config.XMLIsotopeFactory.GetInstance(IChemObjectBuilder)"/>
    // @cdk.module  silent
    // @cdk.githash
    // @author     steinbeck
    // @cdk.created    2000-10-02
    // @cdk.keyword    atom
	[Serializable]
    public class Atom
        : AtomType, IAtom
    {
		// Let's keep this exact specification of what kind of point2d we're talking
		// of here, since there are so many around in the java standard api

        internal double? charge;
        internal int? implicitHydrogenCount;
        internal Vector2? point2D;
        internal Vector3? point3D;
        internal Vector3? fractionalPoint3D;
        internal int? stereoParity;
        internal bool isSingleOrDouble;

        /// <summary>
        /// Constructs an completely unset Atom.
        /// </summary>
        public Atom()
            : this((string)null)
        { }

        /// <summary>
        /// Constructs an Atom from a String containing an element symbol.
        /// </summary>
        /// <param name="elementSymbol">The String describing the element for the Atom</param>
        public Atom(string elementSymbol)
            : this(new Element(elementSymbol, Elements.OfString(elementSymbol).AtomicNumber))
        {
            FormalCharge = 0;
        }

        /// <summary>
        /// Constructs an <see cref="IAtom"/> from an element name and a <see cref="Vector2"/>.
        /// </summary>
        /// <param name="elementSymbol">The Element</param>
        /// <param name="point2d">The Point</param>
        public Atom(string elementSymbol, Vector2 point2d)
            : base(elementSymbol)
        {
            this.point2D = point2d;
        }

        /// <summary>
        /// Constructs an <see cref="IAtom"/> from an element name and a <see cref="Vector3"/>.
        /// </summary>
        /// <param name="elementSymbol">The Element</param>
        /// <param name="point3d">The Point</param>
        public Atom(string elementSymbol, Vector3 point3d)
            : base(elementSymbol)
        {
            this.point3D = point3d;
        }

        /// <summary>    
        /// Constructs an isotope by copying the symbol, atomic number,
        /// flags, identifier, exact mass, natural abundance, mass
        /// number, maximum bond order, bond order sum, van der Waals
        /// and covalent radii, formal charge, hybridization, electron
        /// valency, formal neighbour count and atom type name from the
        /// given IAtomType. It does not copy the listeners and
        /// properties. If the element is an instanceof
        /// IAtom, then the 2D, 3D and fractional coordinates, partial
        /// atomic charge, hydrogen count and stereo parity are copied
        /// too.
        /// </summary>
        /// <param name="element"><see cref="IAtomType"/> to copy information from</param>
        public Atom(IElement element)
            : base(element)
        {
            IAtom a = element as IAtom;
            if (a != null)
            {
                this.point2D = a.Point2D;
                this.point3D = a.Point3D;
                this.fractionalPoint3D = a.FractionalPoint3D;
                this.implicitHydrogenCount = a.ImplicitHydrogenCount;
                this.charge = a.Charge;
                this.stereoParity = a.StereoParity;
            }
        }

        /// <summary>
        /// The partial charge of this atom.
        /// </summary>
        public virtual double? Charge
        {
            get { return charge; }
            set 
            {
                 charge = value;  
                NotifyChanged();
            }
        }

        /// <summary>
        /// The number of implicit hydrogen count of this atom.
        /// </summary>
        public virtual int? ImplicitHydrogenCount
        {
            get { return implicitHydrogenCount; }
            set 
            {
                implicitHydrogenCount = value;  
                NotifyChanged();
            }
        }

        /// <summary>
        /// A point specifying the location of this atom in a 2D space.
        /// </summary>
        public virtual Vector2? Point2D
        {
            get { return point2D; }
            set 
            {
                point2D = value;  
                NotifyChanged();
            }
        }

        /// <summary>
        /// A point specifying the location of this atom in a 3D space.
        /// </summary>
        public virtual Vector3? Point3D
        {
            get { return point3D; }
            set 
            {
                point3D = value;  
                NotifyChanged();
            }
        }

        /// <summary>
        /// A point specifying the location of this atom in a <see cref="Crystal"/> unit cell.
        /// </summary>
        public virtual Vector3? FractionalPoint3D
        {
            get { return fractionalPoint3D; }
            set 
            {
                fractionalPoint3D = value;  
                NotifyChanged();
            }
        }

        /// <summary>
        /// The stereo parity for this atom.
        /// </summary>
        public virtual int? StereoParity
        {
            get { return stereoParity; }
            set 
            {
                stereoParity = value;  
                NotifyChanged();
            }
        }

        /// <inheritdoc/>
        public bool IsSingleOrDouble
        {
            get { return isSingleOrDouble; }
            set 
            {
                isSingleOrDouble = value;  
                NotifyChanged();
            }
        }

        /// <inheritdoc/>
        public override bool Compare(object obj)
        {
            var aa = obj as IAtom;
            return aa != null && base.Compare(obj)
                && Point2D == aa.Point2D
                && Point3D == aa.Point3D
                && ImplicitHydrogenCount == aa.ImplicitHydrogenCount
                && StereoParity == aa.StereoParity
                && Charge == aa.Charge;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Atom(").Append(GetHashCode());
            if (Symbol != null)
                sb.Append(", S:").Append(Symbol);
            if (ImplicitHydrogenCount != null)
                sb.Append(", H:").Append(ImplicitHydrogenCount);
            if (StereoParity != null)
                sb.Append(", SP:").Append(StereoParity);
            if (Point2D != null)
                sb.Append(", 2D:[").Append(Point2D).Append(']');
            if (Point3D != null)
                sb.Append(", 3D:[").Append(Point3D).Append(']');
            if (FractionalPoint3D != null)
                sb.Append(", F3D:[").Append(FractionalPoint3D);
            if (Charge != null)
                sb.Append(", C:").Append(Charge);
            sb.Append(", ").Append(base.ToString());
            sb.Append(')');
            return sb.ToString();
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            if (map == null)
                throw new ArgumentNullException(nameof(map));
            IAtom clone;
            if (map.AtomMap.TryGetValue(this, out clone))
                return clone;
            clone = (Atom)base.Clone(map);
            map.AtomMap.Add(this, clone);
            return clone;
        }
    }
}
namespace NCDK.Silent
{
    /// <summary>
    /// Represents the idea of an chemical atom.
    /// </summary>
    /// <example>
    /// An Atom class is instantiated with at least the atom symbol:
    /// <code>
    ///   Atom a = new Atom("C");
    /// </code>
    ///
    /// Once instantiated all field not filled by passing parameters
    /// to the constructor are null. Atoms can be configured by using
    /// the IsotopeFactory.configure() method:
    /// <code>
    ///   IsotopeFactory if = IsotopeFactory.getInstance(a.getNewBuilder());
    ///   if.configure(a);
    /// </code>
    ///
    /// More examples about using this class can be found in the
    /// Junit test for this class.
    /// </example>
    /// <seealso cref="NCDK.Config.XMLIsotopeFactory.GetInstance(IChemObjectBuilder)"/>
    // @cdk.module  silent
    // @cdk.githash
    // @author     steinbeck
    // @cdk.created    2000-10-02
    // @cdk.keyword    atom
	[Serializable]
    public class Atom
        : AtomType, IAtom
    {
		// Let's keep this exact specification of what kind of point2d we're talking
		// of here, since there are so many around in the java standard api

        internal double? charge;
        internal int? implicitHydrogenCount;
        internal Vector2? point2D;
        internal Vector3? point3D;
        internal Vector3? fractionalPoint3D;
        internal int? stereoParity;
        internal bool isSingleOrDouble;

        /// <summary>
        /// Constructs an completely unset Atom.
        /// </summary>
        public Atom()
            : this((string)null)
        { }

        /// <summary>
        /// Constructs an Atom from a String containing an element symbol.
        /// </summary>
        /// <param name="elementSymbol">The String describing the element for the Atom</param>
        public Atom(string elementSymbol)
            : this(new Element(elementSymbol, Elements.OfString(elementSymbol).AtomicNumber))
        {
            FormalCharge = 0;
        }

        /// <summary>
        /// Constructs an <see cref="IAtom"/> from an element name and a <see cref="Vector2"/>.
        /// </summary>
        /// <param name="elementSymbol">The Element</param>
        /// <param name="point2d">The Point</param>
        public Atom(string elementSymbol, Vector2 point2d)
            : base(elementSymbol)
        {
            this.point2D = point2d;
        }

        /// <summary>
        /// Constructs an <see cref="IAtom"/> from an element name and a <see cref="Vector3"/>.
        /// </summary>
        /// <param name="elementSymbol">The Element</param>
        /// <param name="point3d">The Point</param>
        public Atom(string elementSymbol, Vector3 point3d)
            : base(elementSymbol)
        {
            this.point3D = point3d;
        }

        /// <summary>    
        /// Constructs an isotope by copying the symbol, atomic number,
        /// flags, identifier, exact mass, natural abundance, mass
        /// number, maximum bond order, bond order sum, van der Waals
        /// and covalent radii, formal charge, hybridization, electron
        /// valency, formal neighbour count and atom type name from the
        /// given IAtomType. It does not copy the listeners and
        /// properties. If the element is an instanceof
        /// IAtom, then the 2D, 3D and fractional coordinates, partial
        /// atomic charge, hydrogen count and stereo parity are copied
        /// too.
        /// </summary>
        /// <param name="element"><see cref="IAtomType"/> to copy information from</param>
        public Atom(IElement element)
            : base(element)
        {
            IAtom a = element as IAtom;
            if (a != null)
            {
                this.point2D = a.Point2D;
                this.point3D = a.Point3D;
                this.fractionalPoint3D = a.FractionalPoint3D;
                this.implicitHydrogenCount = a.ImplicitHydrogenCount;
                this.charge = a.Charge;
                this.stereoParity = a.StereoParity;
            }
        }

        /// <summary>
        /// The partial charge of this atom.
        /// </summary>
        public virtual double? Charge
        {
            get { return charge; }
            set 
            {
                 charge = value;  
            }
        }

        /// <summary>
        /// The number of implicit hydrogen count of this atom.
        /// </summary>
        public virtual int? ImplicitHydrogenCount
        {
            get { return implicitHydrogenCount; }
            set 
            {
                implicitHydrogenCount = value;  
            }
        }

        /// <summary>
        /// A point specifying the location of this atom in a 2D space.
        /// </summary>
        public virtual Vector2? Point2D
        {
            get { return point2D; }
            set 
            {
                point2D = value;  
            }
        }

        /// <summary>
        /// A point specifying the location of this atom in a 3D space.
        /// </summary>
        public virtual Vector3? Point3D
        {
            get { return point3D; }
            set 
            {
                point3D = value;  
            }
        }

        /// <summary>
        /// A point specifying the location of this atom in a <see cref="Crystal"/> unit cell.
        /// </summary>
        public virtual Vector3? FractionalPoint3D
        {
            get { return fractionalPoint3D; }
            set 
            {
                fractionalPoint3D = value;  
            }
        }

        /// <summary>
        /// The stereo parity for this atom.
        /// </summary>
        public virtual int? StereoParity
        {
            get { return stereoParity; }
            set 
            {
                stereoParity = value;  
            }
        }

        /// <inheritdoc/>
        public bool IsSingleOrDouble
        {
            get { return isSingleOrDouble; }
            set 
            {
                isSingleOrDouble = value;  
            }
        }

        /// <inheritdoc/>
        public override bool Compare(object obj)
        {
            var aa = obj as IAtom;
            return aa != null && base.Compare(obj)
                && Point2D == aa.Point2D
                && Point3D == aa.Point3D
                && ImplicitHydrogenCount == aa.ImplicitHydrogenCount
                && StereoParity == aa.StereoParity
                && Charge == aa.Charge;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Atom(").Append(GetHashCode());
            if (Symbol != null)
                sb.Append(", S:").Append(Symbol);
            if (ImplicitHydrogenCount != null)
                sb.Append(", H:").Append(ImplicitHydrogenCount);
            if (StereoParity != null)
                sb.Append(", SP:").Append(StereoParity);
            if (Point2D != null)
                sb.Append(", 2D:[").Append(Point2D).Append(']');
            if (Point3D != null)
                sb.Append(", 3D:[").Append(Point3D).Append(']');
            if (FractionalPoint3D != null)
                sb.Append(", F3D:[").Append(FractionalPoint3D);
            if (Charge != null)
                sb.Append(", C:").Append(Charge);
            sb.Append(", ").Append(base.ToString());
            sb.Append(')');
            return sb.ToString();
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            if (map == null)
                throw new ArgumentNullException(nameof(map));
            IAtom clone;
            if (map.AtomMap.TryGetValue(this, out clone))
                return clone;
            clone = (Atom)base.Clone(map);
            map.AtomMap.Add(this, clone);
            return clone;
        }
    }
}
