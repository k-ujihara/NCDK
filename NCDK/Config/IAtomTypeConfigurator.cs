using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace NCDK.Config
{
    public interface IAtomTypeConfigurator
    {
        Stream Stream { set; }
        IEnumerable<IAtomType> ReadAtomTypes(IChemObjectBuilder builder);
    }
}