using System;
using System.Collections.Generic;

namespace NCDK
{
    public interface IMapping
        : IChemObject
    {
        IChemObject this[int index] { get; }
        IEnumerable<IChemObject> GetRelatedChemObjects();
    }
}
