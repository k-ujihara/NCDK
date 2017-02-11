using System;
using System.Collections.Generic;

namespace NCDK
{
    public interface ISingleElectron 
        : IElectronContainer
    {
        IAtom Atom { get; set; }
        bool Contains(IAtom atom);
    }
}
