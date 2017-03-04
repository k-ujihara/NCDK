using System;

namespace NCDK
{
    /// <summary>
    /// Represents the idea of an protein monomer as found in PDB files.
    /// </summary>
    public interface IPDBMonomer
        : IMonomer
    {
        /// <summary>
        /// the I code of this monomer
        /// </summary>
        string ICode { get; set; }

        /// <summary>
        /// the Chain ID of this monomer
        /// </summary>
        string ChainID { get; set; }

        /// <summary>
        /// the sequence identifier of this monomer
        /// </summary>
        string ResSeq { get; set; }
    }
}
