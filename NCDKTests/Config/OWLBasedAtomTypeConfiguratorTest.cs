using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Config;
using NCDK.Default;
using System;
using System.Linq;

namespace NCDK.Config
{
    [TestClass()]
    public class OWLBasedAtomTypeConfiguratorTest
    {
        [TestMethod()]
        public virtual void TestCDKBasedAtomTypeConfigurator()
        {
            OWLBasedAtomTypeConfigurator configurator = new OWLBasedAtomTypeConfigurator();
            Assert.IsNotNull(configurator);
        }

        [TestMethod()]
        public virtual void TestReadAtomTypes_IChemObjectBuilder()
        {
            var configFile = "NCDK.Dict.Data.cdk-atom-types.owl";
            var ins = typeof(OWLBasedAtomTypeConfigurator).Assembly.GetManifestResourceStream(configFile);
            var configurator = new OWLBasedAtomTypeConfigurator();
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