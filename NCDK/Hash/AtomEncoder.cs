using System;

namespace NCDK.Hash
{
    public interface AtomEncoder
    {
        int Encode(IAtom atom, IAtomContainer container);
    }
}
