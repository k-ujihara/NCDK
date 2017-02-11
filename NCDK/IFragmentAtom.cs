using System;
using System.Collections.Generic;

namespace NCDK
{
    /// <summary>
    /// Class to represent an IPseudoAtom which embeds an IAtomContainer. Very much
    /// like the MDL molfile Group concept.
    /// </summary>
    public interface IFragmentAtom
        : IPseudoAtom
    {
        bool IsExpanded { get; set; }
        IAtomContainer Fragment { get; set; }
    }
}
