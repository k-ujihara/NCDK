using System;
using System.Collections.Generic;

namespace NCDK
{
    /// <summary>
    /// Represents the concept of a stereo element in the molecule. Stereo elements can be
    /// that of quadrivalent atoms, cis/trans isomerism around double bonds, but also include
    /// axial and helical stereochemistry.
    /// </summary>
    public interface IStereoElement
        : ICDKObject
    {
        /// <summary>
        /// Does the stereo element contain the provided atom.
        /// </summary>
        /// <param name="atom">an atom to test membership</param>
        /// <returns>whether the atom is present</returns>
        bool Contains(IAtom atom);
    }
}
