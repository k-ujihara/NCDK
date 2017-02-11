using System;
using System.Collections.Generic;

namespace NCDK.Hash
{
    public interface EnsembleHashGenerator
    {
        long Generate(ICollection<IAtomContainer> ensemble);
    }
}
