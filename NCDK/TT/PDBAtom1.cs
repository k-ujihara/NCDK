















// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2015-2016  Kazuya Ujihara

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
    /**
	 * Represents the idea of an atom as used in PDB files. It contains extra fields
	 * normally associated with atoms in such files.
	 *
	 * @cdk.module data
	 * @cdk.githash
	 *
	 * @see  Atom
	 */
    public class PDBAtom : Atom, ICloneable, IPDBAtom
    {
        /**
		 * Constructs an IPDBAtom from a Element.
		 *
		 * @param element IElement to copy information from
		 */
        public PDBAtom(IElement element)
            : base(element)
        {
            InitValues();
        }

        /**
		 * Constructs an {@link IPDBAtom} from a string containing an element symbol.
		 *
		 * @param symbol  The string describing the element for the PDBAtom
		 */
        public PDBAtom(string symbol)
            : base(symbol)
        {
            InitValues();
        }

        /**
		 * Constructs an {@link IPDBAtom} from an Element and a Vector3.
		 *
		 * @param  symbol     The symbol of the atom
		 * @param  coordinate The 3D coordinates of the atom
		 */
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

        /**
		 * one entire line from the PDB entry file which describe the IPDBAtom.
		 * It consists of 80 columns.
		 *
		 * @return a string with all information
		 */
        public virtual string Record { get; set; }

        /**
		 * The Temperature factor of this atom.
		 */
        public double? TempFactor { get; set; }

        /**
		 * The Residue name of this atom.
		 */
        public string ResName { get; set; }

        /**
		 * The Code for insertion of residues of this atom.
		 */
        public string ICode { get; set; }

        /**
		 * The Atom name of this atom.
		 */

        public string Name { get; set; }

        /**
		 * The Chain identifier of this atom.
		 */
        public string ChainID { get; set; }

        /**
		 * The Alternate location indicator of this atom.
		 */
        public string AltLoc { get; set; }

        /**
		 * The Segment identifier, left-justified of this atom.
		 */
        public string SegID { get; set; }

        /**
		 * The Atom serial number of this atom.
		 */
        public int? Serial { get; set; }

        /**
		 * The Residue sequence number of this atom.
		 */
        public string ResSeq { get; set; }

        public bool Oxt { get; set; }

        public bool? HetAtom { get; set; }

        /**
		 * The Occupancy of this atom.
		 */

        public double? Occupancy { get; set; }

        /**
		 * Returns a one line string representation of this Atom.
		 * Methods is conform RFC #9.
		 *
		 * @return  The string representation of this Atom
		 */
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
    /**
	 * Represents the idea of an atom as used in PDB files. It contains extra fields
	 * normally associated with atoms in such files.
	 *
	 * @cdk.module data
	 * @cdk.githash
	 *
	 * @see  Atom
	 */
    public class PDBAtom : Atom, ICloneable, IPDBAtom
    {
        /**
		 * Constructs an IPDBAtom from a Element.
		 *
		 * @param element IElement to copy information from
		 */
        public PDBAtom(IElement element)
            : base(element)
        {
            InitValues();
        }

        /**
		 * Constructs an {@link IPDBAtom} from a string containing an element symbol.
		 *
		 * @param symbol  The string describing the element for the PDBAtom
		 */
        public PDBAtom(string symbol)
            : base(symbol)
        {
            InitValues();
        }

        /**
		 * Constructs an {@link IPDBAtom} from an Element and a Vector3.
		 *
		 * @param  symbol     The symbol of the atom
		 * @param  coordinate The 3D coordinates of the atom
		 */
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

        /**
		 * one entire line from the PDB entry file which describe the IPDBAtom.
		 * It consists of 80 columns.
		 *
		 * @return a string with all information
		 */
        public virtual string Record { get; set; }

        /**
		 * The Temperature factor of this atom.
		 */
        public double? TempFactor { get; set; }

        /**
		 * The Residue name of this atom.
		 */
        public string ResName { get; set; }

        /**
		 * The Code for insertion of residues of this atom.
		 */
        public string ICode { get; set; }

        /**
		 * The Atom name of this atom.
		 */

        public string Name { get; set; }

        /**
		 * The Chain identifier of this atom.
		 */
        public string ChainID { get; set; }

        /**
		 * The Alternate location indicator of this atom.
		 */
        public string AltLoc { get; set; }

        /**
		 * The Segment identifier, left-justified of this atom.
		 */
        public string SegID { get; set; }

        /**
		 * The Atom serial number of this atom.
		 */
        public int? Serial { get; set; }

        /**
		 * The Residue sequence number of this atom.
		 */
        public string ResSeq { get; set; }

        public bool Oxt { get; set; }

        public bool? HetAtom { get; set; }

        /**
		 * The Occupancy of this atom.
		 */

        public double? Occupancy { get; set; }

        /**
		 * Returns a one line string representation of this Atom.
		 * Methods is conform RFC #9.
		 *
		 * @return  The string representation of this Atom
		 */
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
