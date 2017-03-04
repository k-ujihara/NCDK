using System;
using System.Collections.Generic;

namespace NCDK
{
    /// <summary>
    /// A PDBPolymer is a subclass of a BioPolymer which is supposed to store
    /// additional informations about the BioPolymer which are connected to BioPolymers.
    /// </summary>
    public interface IPDBPolymer
        : IBioPolymer
    {
        /// <summary>
        /// Adds the atom oAtom without specifying a Monomer or a Strand. Therefore the
        /// atom to this AtomContainer, but not to a certain Strand or Monomer (intended
        /// e.g. for HETATMs).
        /// </summary>
        /// <param name="oAtom">The atom to add</param>
        void Add(IPDBAtom oAtom);

        /// <summary>
        /// Adds the atom to a specified Strand and a specified Monomer.
        /// </summary>
        /// <param name="oAtom">The atom to add</param>
        /// <param name="oMonomer">The monomer the atom belongs to</param>
        /// <param name="oStrand">The strand the atom belongs to</param>
        void AddAtom(IPDBAtom oAtom, IMonomer oMonomer, IStrand oStrand);

        /// <summary>
        /// Adds the PDBStructure structure a this PDBPolymer.
        /// </summary>
        /// <param name="structure">The PDBStructure to add</param>
        void Add(IPDBStructure structure);

        /// <summary>
        /// Returns a Collection containing the PDBStructure in the PDBPolymer.
        /// </summary>
        /// <returns>Collection containing the PDBStructure in the PDBPolymer</returns>
        IEnumerable<IPDBStructure> GetStructures();
    }
}
