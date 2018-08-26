using NCDK.Silent;

namespace NCDK.Config
{
    public static class XMLIsotopeFactory_Example
    {
        public static void Main()
        {
            {
                #region 1
                IsotopeFactory ifac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
                #endregion
            }
            {
                #region example
                IsotopeFactory factory = XMLIsotopeFactory.GetInstance(ChemObjectBuilder.Instance);
                IIsotope major = factory.GetMajorIsotope("H");
                #endregion
            }
        }
    }
}
