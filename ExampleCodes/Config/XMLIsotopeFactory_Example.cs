using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;

namespace NCDK.Config
{
    [TestClass]
    public class XMLIsotopeFactory_Example
    {
        [TestMethod]
        [TestCategory("Example")]
        public void Main()
        {
            {
                #region 1
                IsotopeFactory ifac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
                #endregion
            }
            {
                #region example
                IsotopeFactory factory = XMLIsotopeFactory.GetInstance(Default.ChemObjectBuilder.Instance);
                IIsotope major = factory.GetMajorIsotope("H");
                #endregion
            }
        }
    }
}
