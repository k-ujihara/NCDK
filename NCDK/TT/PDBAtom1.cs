
















// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2015-2017  Kazuya Ujihara

/* Copyright (C) 2005-2015  Egon Willighagen <egonw@users.sf.net>
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
 *
 */

using System;
using NCDK.Numerics;
using System.Text;

namespace NCDK.Default
{
    /// <summary>
    /// Represents the idea of an atom as used in PDB files. It contains extra fields
    /// normally associated with atoms in such files.
    ///
    // @cdk.module data
    // @cdk.githash
    ///
    // @see  Atom
    /// </summary>
    public class PDBAtom : Atom, ICloneable, IPDBAtom
    {
        /// <summary>
        /// Constructs an IPDBAtom from a Element.
        ///
        /// <param name="element">IElement to copy information from</param>
        /// </summary>
        public PDBAtom(IElement element)
            : base(element)
        {
            InitValues();
        }

        /// <summary>
        /// Constructs an <see cref="IPDBAtom"/> from a string containing an element symbol.
        ///
        /// <param name="symbol">The string describing the element for the PDBAtom</param>
        /// </summary>
        public PDBAtom(string symbol)
            : base(symbol)
        {
            InitValues();
        }

        /// <summary>
        /// Constructs an <see cref="IPDBAtom"/> from an Element and a Vector3.
        ///
        /// <param name="symbol">The symbol of the atom</param>
        /// <param name="coordinate">The 3D coordinates of the atom</param>
        /// </summary>
        public PDBAtom(string symbol, Vector3 coordinate)
            : base(symbol, coordinate)
        {
            InitValues();
        }

        private void InitValues()
        {
            Record = null;
            TempFactor = -1.0;
            ResName = null;
            ICode = null;
            Occupancy = -1.0;
            Name = null;
            ChainID = null;
            AltLoc = null;
            SegID = null;
            Serial = 0;
            ResSeq = null;

            Oxt = false;
            HetAtom = false;

            base.Charge = 0.0;
            base.FormalCharge = 0;
        }

        /// <summary>
        /// one entire line from the PDB entry file which describe the IPDBAtom.
        /// It consists of 80 columns.
        ///
        /// <returns>a string with all information</returns>
        /// </summary>
        public virtual string Record { get; set; }

        /// <summary>
        /// The Temperature factor of this atom.
        /// </summary>
        public double? TempFactor { get; set; }

        /// <summary>
        /// The Residue name of this atom.
        /// </summary>
        public string ResName { get; set; }

        /// <summary>
        /// The Code for insertion of residues of this atom.
        /// </summary>
        public string ICode { get; set; }

        /// <summary>
        /// The Atom name of this atom.
        /// </summary>

        public string Name { get; set; }

        /// <summary>
        /// The Chain identifier of this atom.
        /// </summary>
        public string ChainID { get; set; }

        /// <summary>
        /// The Alternate location indicator of this atom.
        /// </summary>
        public string AltLoc { get; set; }

        /// <summary>
        /// The Segment identifier, left-justified of this atom.
        /// </summary>
        public string SegID { get; set; }

        /// <summary>
        /// The Atom serial number of this atom.
        /// </summary>
        public int? Serial { get; set; }

        /// <summary>
        /// The Residue sequence number of this atom.
        /// </summary>
        public string ResSeq { get; set; }

        public bool Oxt { get; set; }

        public bool? HetAtom { get; set; }

        /// <summary>
        /// The Occupancy of this atom.
        /// </summary>

        public double? Occupancy { get; set; }

        /// <summary>
        /// Returns a one line string representation of this Atom.
        /// Methods is conform RFC #9.
        ///
        /// <returns>The string representation of this Atom</returns>
        /// </summary>
        public override string ToString()
        {
            StringBuilder description = new StringBuilder(150);
            description.Append("PDBAtom(");
            description.Append(this.GetHashCode());
            description.Append(", altLoc=").Append(AltLoc);
            description.Append(", chainID=").Append(ChainID);
            description.Append(", iCode=").Append(ICode);
            description.Append(", name=").Append(Name);
            description.Append(", resName=").Append(ResName);
            description.Append(", resSeq=").Append(ResSeq);
            description.Append(", segID=").Append(SegID);
            description.Append(", serial=").Append(Serial);
            description.Append(", tempFactor=").Append(TempFactor);
            description.Append(", oxt=").Append(Oxt);
            description.Append(", hetatm=").Append(HetAtom).Append(", ");
            description.Append(base.ToString());
            description.Append(')');
            return description.ToString();
        }
    }
}
namespace NCDK.Silent
{
    /// <summary>
    /// Represents the idea of an atom as used in PDB files. It contains extra fields
    /// normally associated with atoms in such files.
    ///
    // @cdk.module data
    // @cdk.githash
    ///
    // @see  Atom
    /// </summary>
    public class PDBAtom : Atom, ICloneable, IPDBAtom
    {
        /// <summary>
        /// Constructs an IPDBAtom from a Element.
        ///
        /// <param name="element">IElement to copy information from</param>
        /// </summary>
        public PDBAtom(IElement element)
            : base(element)
        {
            InitValues();
        }

        /// <summary>
        /// Constructs an <see cref="IPDBAtom"/> from a string containing an element symbol.
        ///
        /// <param name="symbol">The string describing the element for the PDBAtom</param>
        /// </summary>
        public PDBAtom(string symbol)
            : base(symbol)
        {
            InitValues();
        }

        /// <summary>
        /// Constructs an <see cref="IPDBAtom"/> from an Element and a Vector3.
        ///
        /// <param name="symbol">The symbol of the atom</param>
        /// <param name="coordinate">The 3D coordinates of the atom</param>
        /// </summary>
        public PDBAtom(string symbol, Vector3 coordinate)
            : base(symbol, coordinate)
        {
            InitValues();
        }

        private void InitValues()
        {
            Record = null;
            TempFactor = -1.0;
            ResName = null;
            ICode = null;
            Occupancy = -1.0;
            Name = null;
            ChainID = null;
            AltLoc = null;
            SegID = null;
            Serial = 0;
            ResSeq = null;

            Oxt = false;
            HetAtom = false;

            base.Charge = 0.0;
            base.FormalCharge = 0;
        }

        /// <summary>
        /// one entire line from the PDB entry file which describe the IPDBAtom.
        /// It consists of 80 columns.
        ///
        /// <returns>a string with all information</returns>
        /// </summary>
        public virtual string Record { get; set; }

        /// <summary>
        /// The Temperature factor of this atom.
        /// </summary>
        public double? TempFactor { get; set; }

        /// <summary>
        /// The Residue name of this atom.
        /// </summary>
        public string ResName { get; set; }

        /// <summary>
        /// The Code for insertion of residues of this atom.
        /// </summary>
        public string ICode { get; set; }

        /// <summary>
        /// The Atom name of this atom.
        /// </summary>

        public string Name { get; set; }

        /// <summary>
        /// The Chain identifier of this atom.
        /// </summary>
        public string ChainID { get; set; }

        /// <summary>
        /// The Alternate location indicator of this atom.
        /// </summary>
        public string AltLoc { get; set; }

        /// <summary>
        /// The Segment identifier, left-justified of this atom.
        /// </summary>
        public string SegID { get; set; }

        /// <summary>
        /// The Atom serial number of this atom.
        /// </summary>
        public int? Serial { get; set; }

        /// <summary>
        /// The Residue sequence number of this atom.
        /// </summary>
        public string ResSeq { get; set; }

        public bool Oxt { get; set; }

        public bool? HetAtom { get; set; }

        /// <summary>
        /// The Occupancy of this atom.
        /// </summary>

        public double? Occupancy { get; set; }

        /// <summary>
        /// Returns a one line string representation of this Atom.
        /// Methods is conform RFC #9.
        ///
        /// <returns>The string representation of this Atom</returns>
        /// </summary>
        public override string ToString()
        {
            StringBuilder description = new StringBuilder(150);
            description.Append("PDBAtom(");
            description.Append(this.GetHashCode());
            description.Append(", altLoc=").Append(AltLoc);
            description.Append(", chainID=").Append(ChainID);
            description.Append(", iCode=").Append(ICode);
            description.Append(", name=").Append(Name);
            description.Append(", resName=").Append(ResName);
            description.Append(", resSeq=").Append(ResSeq);
            description.Append(", segID=").Append(SegID);
            description.Append(", serial=").Append(Serial);
            description.Append(", tempFactor=").Append(TempFactor);
            description.Append(", oxt=").Append(Oxt);
            description.Append(", hetatm=").Append(HetAtom).Append(", ");
            description.Append(base.ToString());
            description.Append(')');
            return description.ToString();
        }
    }
}
