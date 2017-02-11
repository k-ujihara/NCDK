using System;
using System.Collections.Generic;

namespace NCDK
{
    /// <summary>
    /// Represents the idea of a non-chemical atom-like entity, like Me, R, X, Phe, His, etc.
    /// </summary>
    public interface IPseudoAtom
        : IAtom
    {
        string Label { get; set; }
        int AttachPointNum { get; set; }
    }
}
