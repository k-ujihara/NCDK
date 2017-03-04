using System;
using System.Collections.Generic;

namespace NCDK
{
    /// <summary>
    /// A Strand is an AtomContainer which stores additional strand specific informations for a group of Atoms.
    /// </summary>
    public interface IStrand
        : IAtomContainer
    {
        string StrandName { get; set; }
        string StrandType { get; set; }
        
        /// <summary>
        /// Adds the atom oAtom to a specified Monomer.
        /// </summary>
        /// <param name="oAtom">The atom to add</param>
        /// <param name="oMonomer">The monomer the atom belongs to</param>
        void AddAtom(IAtom oAtom, IMonomer oMonomer);

        IReadOnlyDictionary<string, IMonomer> GetMonomerMap();
        IMonomer GetMonomer(string name);
        IEnumerable<string> GetMonomerNames();

        /// <summary>
        /// Removes a particular monomer, specified by its name.
        /// </summary>
        /// <param name="name">The name of the monomer to remove</param>
        void RemoveMonomer(string name);
    }
}
