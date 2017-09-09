using Microsoft.VisualStudio.TestTools.UnitTesting;
using ACDK;
using System.Linq;

namespace ACDK.Tests
{
    [TestClass()]
    public class IAtomContainerTests
    {
        static NCDK.Smiles.SmilesParser parser = new NCDK.Smiles.SmilesParser(NCDK.Silent.ChemObjectBuilder.Instance);

        [TestMethod()]
        public void IAtomContainerTest()
        {
            var methane = parser.ParseSmiles("C");
            methane.SetProperty("A", "One");
            methane.SetProperty("B", 2);
            IAtomContainer w_methane = new W_IAtomContainer(methane);
            var dic = w_methane.GetProperties();
            Assert.IsTrue(dic.Keys.Contains("A"));
            Assert.IsTrue(dic.Keys.Contains("B"));
            for (int i = 0; i < dic.Count; i++)
            {
                var key = dic.Keys[i];
                var val = dic[key];
                if ((string)key == "A")
                    Assert.AreEqual("One", val);
                if ((string)key == "B")
                    Assert.AreEqual(2, val);
            }
        }
    }
}