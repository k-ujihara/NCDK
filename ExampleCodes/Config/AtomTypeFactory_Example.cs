using NCDK.Default;

namespace NCDK.Config
{
    public class AtomTypeFactory_Example
    {
        public void Main()
        {
            IChemObjectBuilder someChemObjectBuilder = null;
            {
                #region 1
                AtomTypeFactory factory = AtomTypeFactory.GetInstance(someChemObjectBuilder);
                #endregion
            }
            {
                #region 2
                AtomTypeFactory factory = AtomTypeFactory.GetInstance("NCDK.Config.Data.jmol_atomtypes.txt", someChemObjectBuilder);
                #endregion
            }
        }
    }
}
