using System;
using System.Collections.Generic;

namespace NCDK
{
    /// <summary>
    /// Class representing a ring structure in a molecule.
    /// A ring is a linear sequence of
    /// N atoms interconnected to each other by covalent bonds,
    /// such that atom i(1 &gt; i &gt; N ) is bonded to
    /// atom i-1 and atom i+1 and atom 1 is bonded to atom N and atom 2.
    /// </summary>
    public interface IRing
        : IAtomContainer
    {
        int RingSize { get; }

        /// <summary>
        /// Returns the next bond in order, relative to a given bond and atom.
        /// Example: Let the ring be composed of 0-1, 1-2, 2-3 and 3-0.
        /// A request GetNextBond(1-2, 2) will return Bond 2-3.
        /// </summary>
        /// <param name="bond">A bond for which an atom from a consecutive bond is sought</param>
        /// <param name="atom">A atom from the bond above to assign a search direction</param>
        /// <returns>The next bond in the order given by the above assignment</returns>
        IBond GetNextBond(IBond bond, IAtom atom);

        /// <summary>
        /// The sum of all bond orders in the ring.
        /// </summary>
        int BondOrderSum { get; }
    }
}
