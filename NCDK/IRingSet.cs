using System;
using System.Collections.Generic;

namespace NCDK
{
    /// <summary>
    /// Maintains a set of Ring objects.
    /// </summary>
    public interface IRingSet
        : IAtomContainerSet<IRing>
    {
        /// <summary>
        /// all rings that this bond is part of.
        /// </summary>
        /// <param name="bond">The bond to be checked</param>
        /// <returns>all rings that this bond is part of</returns>
        IEnumerable<IRing> GetRings(IBond bond);

        /// <summary>
        /// all rings that this atom is part of.
        /// </summary>
        /// <param name="atom">The atom to be checked</param>
        /// <returns>all rings that this bond is part of</returns>
        IEnumerable<IRing> GetRings(IAtom atom);

        /// <summary>
        /// Returns all the rings in the RingSet that share
        /// one or more atoms with a given ring.
        /// </summary>
        /// <param name="ring">A ring with which all return rings must share one or more atoms</param>
        /// <returns>All the rings that share one or more atoms with a given ring.</returns>
        IEnumerable<IRing> GetConnectedRings(IRing ring);

        /// <summary>
        /// Adds all rings of another RingSet if they are not allready part of this ring set.
        /// If you want to add a single ring to the set use Add(IAtomContainer)
        /// </summary>
        /// <param name="ringSet">the ring set to be united with this one.</param>
        void Add(IRingSet ringSet);

        /// <summary>
        /// True, if at least one of the rings in the ringset contains the given atom.
        /// </summary>
        /// <param name="atom">Atom to check</param>
        /// <returns>true, if the ringset contains the atom</returns>
        bool Contains(IAtom atom);
    }
}
