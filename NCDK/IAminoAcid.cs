using System;
using System.Collections.Generic;

namespace NCDK
{
    public interface IAminoAcid
        : IMonomer
    {
        IAtom NTerminus { get; }
        IAtom CTerminus { get; }

        void AddNTerminus(IAtom atom);
        void AddCTerminus(IAtom atom);
    }
}
