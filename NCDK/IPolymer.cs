using System;
using System.Collections.Generic;

namespace NCDK
{
    public interface IPolymer
        : IAtomContainer
    {
        /// <summary>
        /// Adds the atom oAtom to a specified Monomer.
        /// </summary>
        /// <param name="oAtom">The atom to add</param>
        /// <param name="oMonomer">The monomer the atom belongs to</param>
        void AddAtom(IAtom oAtom, IMonomer oMonomer);

        IEnumerable<KeyValuePair<string, IMonomer>> GetMonomerMap();
        IMonomer GetMonomer(string name);
        IEnumerable<string> GetMonomerNames();
        void RemoveMonomer(string name);
    }
}