using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.Config;
using System;
using System.Linq;

namespace NCDK.Config
{
    [TestClass()]
    public class CDKBasedAtomTypeConfiguratorTest
    {
        [TestMethod()]
        public virtual void TestCDKBasedAtomTypeConfigurator()
        {
            CDKBasedAtomTypeConfigurator configurator = new CDKBasedAtomTypeConfigurator();
            Assert.IsNotNull(configurator);
        }

        [TestMethod()]
        public virtual void TestReadAtomTypes_IChemObjectBuilder()
        {
            var configFile = "NCDK.Config.Data.structgen_atomtypes.xml";
            var ins = typeof(CDKBasedAtomTypeConfigurator).Assembly.GetManifestResourceStream(configFile);
            var configurator = new CDKBasedAtomTypeConfigurator();
            configurator.Stream = ins;
            var atomTypes = configurator.ReadAtomTypes(new ChemObject().Builder);
            Assert.AreNotSame(0, atomTypes.Count());
        }

        [TestMethod()]
        public void TestSetInputStream_InputStream()
        {
            TestReadAtomTypes_IChemObjectBuilder();
        }
    }
}