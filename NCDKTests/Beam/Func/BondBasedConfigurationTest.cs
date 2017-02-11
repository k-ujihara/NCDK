using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NCDK.Beam
{
    /// <author>John May </author>
    public class BondBasedConfigurationTest
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void nonDoubleBond()
        {
            Graph g = Graph.FromSmiles("CCCC");
            BondBasedConfiguration.ConfigurationOf(g, 0, 1, 2, 3);
        }

        [TestMethod()]
        public void opposite1()
        {
            Graph g = Graph.FromSmiles("F/C=C/F");
            Assert.AreEqual(
                Configuration.DoubleBond.Opposite,
                BondBasedConfiguration.ConfigurationOf(g, 0, 1, 2, 3));
        }

        [TestMethod()]
        public void opposite2()
        {
            Graph g = Graph.FromSmiles("F\\C=C\\F");
            Assert.AreEqual(
                Configuration.DoubleBond.Opposite,
                BondBasedConfiguration.ConfigurationOf(g, 0, 1, 2, 3));
        }

        [TestMethod()]
        public void together1()
        {
            Graph g = Graph.FromSmiles("F/C=C\\F");
            Assert.AreEqual(
                Configuration.DoubleBond.Together,
                BondBasedConfiguration.ConfigurationOf(g, 0, 1, 2, 3));
        }

        [TestMethod()]
        public void together2()
        {
            Graph g = Graph.FromSmiles("F\\C=C/F");
            Assert.AreEqual(
                Configuration.DoubleBond.Together,
                BondBasedConfiguration.ConfigurationOf(g, 0, 1, 2, 3));
        }
    }
}
