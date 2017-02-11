using System;

namespace NCDK
{
    /// <summary>
    /// A PDBAtom is a subclass of a Atom which is supposed to store additional informations about the Atom.
    /// </summary>
    public interface IPDBAtom
        : IAtom
    {
        string Record { get; set; }

        /// <summary>
        /// the Temperature factor of this atom
        /// </summary>
        double? TempFactor { get; set; }

        /// <summary>
        /// the Residue name of this atom
        /// </summary>
        string ResName { get; set; }

        /// <summary>
        /// the Code for insertion of residues of this atom
        /// </summary>
        string ICode { get; set; }

        /// <summary>
        /// The Atom name of this atom.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// the Chain identifier of this atom
        /// </summary>
        string ChainID { get; set; }

        /// <summary>
        /// the Alternate location indicator of this atom
        /// </summary>
        string AltLoc { get; set; }

        /// <summary>
        /// the Segment identifier, left-justified of this atom
        /// </summary>
        string SegID { get; set; }

        /// <summary>
        /// the Atom serial number of this atom
        /// </summary>
        int? Serial { get; set; }

        /// <summary>
        /// the Residue sequence number of this atom
        /// </summary>
        string ResSeq { get; set; }

        /// <summary>
        /// true if this atom is a PDB OXT atom.
        /// </summary>
        bool Oxt { get; set; }

        /// <summary>
        /// true if the atom is a heteroatom, otherwise false
        /// </summary>
        bool? HetAtom { get; set; }

        /// <summary>
        /// the Occupancy of this atom
        /// </summary>
        double? Occupancy { get; set; }
    }
}
