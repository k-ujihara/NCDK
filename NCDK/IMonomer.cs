using System;

namespace NCDK
{
    public interface IMonomer
        : IAtomContainer
    {
        string MonomerName { get; set; }
        string MonomerType { get; set; }
    }
}
