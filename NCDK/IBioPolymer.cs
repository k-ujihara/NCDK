using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NCDK
{
    public interface IBioPolymer
        : IPolymer
    {
        /// <summary>
        /// Adds the atom oAtom to a specified Strand, whereas the Monomer is unspecified. Hence
        /// the atom will be added to a Monomer of type Unknown in the specified Strand.
        /// </summary>
        /// <param name="oAtom">The atom to add</param>
        /// <param name="oStrand">The strand the atom belongs to</param>
        void AddAtom(IAtom oAtom, IStrand oStrand);

        /// <summary>
        /// Adds the atom to a specified Strand and a specified Monomer.
        /// </summary>
        /// <param name="oAtom">The atom to add</param>
        /// <param name="oMonomer">The monomer the atom belongs to</param>
        /// <param name="oStrand">The strand the atom belongs to</param>
        void AddAtom(IAtom oAtom, IMonomer oMonomer, IStrand oStrand);

        /// <summary>
        /// Retrieve a <code>Monomer</code> object by specifying its name.
        /// You have to specify the strand to enable
        /// monomers with the same name in different strands. There is at least one such case: every
        /// strand contains a monomer called "".
        /// </summary>
        /// <param name="monName">The name of the monomer to look for</param>
        /// <param name="strandName">The name of the strand to look for</param>
        /// <returns>The Monomer object which was asked for</returns>
        IMonomer GetMonomer(string monName, string strandName);

        IDictionary<string, IStrand> GetStrandMap();
        IStrand GetStrand(string cName);
        IEnumerable<string> GetStrandNames();
        void RemoveStrand(string name);
    }
}
