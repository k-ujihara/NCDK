using System;

namespace NCDK
{
    /// <summary>
    /// Represents the idea of an chemical structure.

    public interface IPDBStructure
            : ICDKObject
    {
        /// <summary>
        /// the ending Chain identifier of this structure
        /// </summary>
        char? EndChainID { get; set; }

        /// <summary>
        /// the ending Code for insertion of residues of this structure
        /// </summary>
        char? EndInsertionCode { get; set; }

        /// <summary>
        /// the ending sequence number of this structure
        /// </summary>
        int? EndSequenceNumber { get; set; }

        /// <summary>
        /// the start Chain identifier of this structure
        /// </summary>
        char? StartChainID { get; set; }

        /// <summary>
        /// the start Code for insertion of residues of this structure
        /// </summary>
        char? StartInsertionCode { get; set; }

        /// <summary>
        /// the start sequence number of this structure
        /// </summary>
        int? StartSequenceNumber { get; set; }

        /// <summary>
        /// the Structure Type of this structure
        /// </summary>
        string StructureType { get; set; }
    }
}

