using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using NCDK.Config.AtomType;

namespace NCDK.Config
{
    public class CDKBasedAtomTypeConfigurator
        : IAtomTypeConfigurator
    {
        private const string configFile = "NCDK.Config.Data.structgen_atomtypes.xml";

        public Stream Stream { get; set; }

        public CDKBasedAtomTypeConfigurator() { }

        public IEnumerable<IAtomType> ReadAtomTypes(IChemObjectBuilder builder)
        {
            if (Stream == null)
                Stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(configFile);

            return new AtomTypeReader(Stream).ReadAtomTypes(builder);
        }
    }
}
