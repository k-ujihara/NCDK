using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Tools
{
    [TestClass()]
    public class PeriodicTableTest
    {
        [TestMethod()]
        public virtual void TestPeriodicTable()
        {
            Assert.AreEqual(null, PeriodicTable.GetVdwRadius("Co"));
            Assert.AreEqual(1.7, PeriodicTable.GetVdwRadius("C").Value, 0.001);
            Assert.AreEqual(39, PeriodicTable.GetAtomicNumber("Y"), 0.001);
            Assert.AreEqual(2.55, PeriodicTable.GetPaulingElectronegativity("C").Value, 0.001);
            Assert.AreEqual(null, PeriodicTable.GetPaulingElectronegativity("He"));
            Assert.AreEqual(null, PeriodicTable.GetCovalentRadius("Pu"));
            Assert.AreEqual(0.32, PeriodicTable.GetCovalentRadius("He").Value, 0.001);
            Assert.AreEqual(14, PeriodicTable.GetGroup("C"), 0.01);

            Assert.AreEqual("H", PeriodicTable.GetSymbol(1));
            Assert.AreEqual("C", PeriodicTable.GetSymbol(6));
        }
    }
}
