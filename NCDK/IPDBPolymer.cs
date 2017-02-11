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
        /**
         * Adds the atom oAtom without specifying a Monomer or a Strand. Therefore the
         * atom to this AtomContainer, but not to a certain Strand or Monomer (intended
         * e.g. for HETATMs).
         *
         * @param oAtom  The atom to add
         */
        void Add(IPDBAtom oAtom);

        /**
         * Adds the atom to a specified Strand and a specified Monomer.
         *
         * @param oAtom    The atom to add
         * @param oMonomer The monomer the atom belongs to
         * @param oStrand  The strand the atom belongs to
         */
        void AddAtom(IPDBAtom oAtom, IMonomer oMonomer, IStrand oStrand);

        /**
         * Adds the PDBStructure structure a this PDBPolymer.
         *
         * @param structure  The PDBStructure to add
         */
        void Add(IPDBStructure structure);

        /**
         * Returns a Collection containing the PDBStructure in the PDBPolymer.
         *
         * @return Collection containing the PDBStructure in the PDBPolymer
         */
        IEnumerable<IPDBStructure> GetStructures();
    }
}
