using System;
using System.Collections.Generic;

namespace NCDK
{
    public interface IChemFile
        : IChemObject, IList<IChemSequence>
    {
    }
}
