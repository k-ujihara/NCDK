using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NCDK.Beam
{
    // @author John May 
    public class BondBasedConfigurationTest
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void NonDoubleBond()
        {
            Graph g = Graph.FromSmiles("CCCC");
            BondBasedConfiguration.ConfigurationOf(g, 0, 1, 2, 3);
        }

        [TestMethod()]
        public void Opposite1()
        {
            Graph g = Graph.FromSmiles("F/C=C/F");
            Assert.AreEqual(
                Configuration.DoubleBonds.Opposite,
                BondBasedConfiguration.ConfigurationOf(g, 0, 1, 2, 3));
        }

        [TestMethod()]
        public void Opposite2()
        {
            Graph g = Graph.FromSmiles("F\\C=C\\F");
            Assert.AreEqual(
                Configuration.DoubleBonds.Opposite,
                BondBasedConfiguration.ConfigurationOf(g, 0, 1, 2, 3));
        }

        [TestMethod()]
        public void Together1()
        {
            Graph g = Graph.FromSmiles("F/C=C\\F");
            Assert.AreEqual(
                Configuration.DoubleBonds.Together,
                BondBasedConfiguration.ConfigurationOf(g, 0, 1, 2, 3));
        }

        [TestMethod()]
        public void Together2()
        {
            Graph g = Graph.FromSmiles("F\\C=C/F");
            Assert.AreEqual(
                Configuration.DoubleBonds.Together,
                BondBasedConfiguration.ConfigurationOf(g, 0, 1, 2, 3));
        }
    }
}
