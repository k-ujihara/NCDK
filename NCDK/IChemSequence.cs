using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NCDK
{
    /// <summary>
    /// A sequence of ChemModels, which can, for example, be used to
    /// store the course of a reaction.Each state of the reaction would be
    /// stored in one ChemModel.
    /// </summary>
    public interface IChemSequence
        : IChemObject, IList<IChemModel>
    {
    }
}
