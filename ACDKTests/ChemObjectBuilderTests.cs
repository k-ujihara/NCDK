using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACDK
{
    [TestClass()]
    public class ChemObjectBuilderTests
    {
        [TestMethod()]
        public void ChemObjectBuilderTest()
        {
            var a = new W_IChemObjectBuilder(NCDK.Default.ChemObjectBuilder.Instance);
            var b = (IChemObjectBuilder)a;
        }
    }
}
