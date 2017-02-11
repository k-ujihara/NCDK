using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using NCDK.Default;
using NCDK.Config;

namespace NCDK.Config
{
    [TestClass()]
    public class TXTBasedAtomTypeConfiguratorTest
    {
        [TestMethod()]
        public virtual void TestTXTBasedAtomTypeConfigurator()
        {
            TXTBasedAtomTypeConfigurator configurator = new TXTBasedAtomTypeConfigurator();
            Assert.IsNotNull(configurator);
        }

        [TestMethod()]
        public virtual void TestReadAtomTypes_IChemObjectBuilder()
        {
            var configFile = "NCDK.Config.Data.jmol_atomtypes.txt";
            var ins = typeof(TXTBasedAtomTypeConfigurator).Assembly.GetManifestResourceStream(configFile);
            var configurator = new TXTBasedAtomTypeConfigurator();
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