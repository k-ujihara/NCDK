using NCDK.Default;

namespace NCDK.Config
{
    public class AtomTypeFactory_Example
    {
        public void Main()
        {
            {
                #region 1
                AtomTypeFactory factory = AtomTypeFactory.GetInstance();
                #endregion
            }
            {
                #region 2
                AtomTypeFactory factory = AtomTypeFactory.GetInstance("NCDK.Config.Data.jmol_atomtypes.txt");
                #endregion
            }
        }
    }
}
