using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NCDK
{
    /// <summary>
    /// An object containing multiple MoleculeSet and
    /// the other lower level concepts like rings, sequences,
    /// fragments, etc.
    /// </summary>
    public interface IChemModel
        : IChemObject
    {
        IAtomContainerSet<IAtomContainer> MoleculeSet { get; set; }
        IRingSet RingSet { get; set; }
        ICrystal Crystal { get; set; }
        IReactionSet ReactionSet { get; set; }
        bool IsEmpty { get; }
    }
}