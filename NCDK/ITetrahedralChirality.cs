using System;
using System.Collections.Generic;

namespace NCDK
{
    public interface ITetrahedralChirality
        : IStereoElement
    {
        IList<IAtom> Ligands { get; }
        IAtom ChiralAtom { get; }
        TetrahedralStereo Stereo { get; set; }
    }
}
