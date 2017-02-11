using System;
using System.Collections.Generic;

namespace NCDK.Hash
{
    public interface MoleculeHashGenerator
    {
        long Generate(IAtomContainer container);
    }
}
