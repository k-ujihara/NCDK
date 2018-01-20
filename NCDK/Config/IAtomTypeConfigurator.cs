using System.Collections.Generic;
using System.IO;

namespace NCDK.Config
{
    public interface IAtomTypeConfigurator
    {
        Stream Stream { set; }
        IEnumerable<IAtomType> ReadAtomTypes(IChemObjectBuilder builder);
    }
}
