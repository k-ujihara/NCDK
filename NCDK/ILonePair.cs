using System;
using System.Collections.Generic;

namespace NCDK
{
    /// <summary>
    /// A LonePair is an orbital primarily located with one Atom, containing two electrons.
    /// </summary>
    public interface ILonePair
        : IElectronContainer
    {
        IAtom Atom { get; set; }
        bool Contains(IAtom atom);
    }
}
