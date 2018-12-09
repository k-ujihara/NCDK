using NCDK.Default;

namespace NCDK
{
    class ISingleElectron_Example
    {
        public void Main()
        {
            {
                #region 
                AtomContainer radical = new AtomContainer();
                Atom carbon = new Atom("C");
                carbon.ImplicitHydrogenCount = 3;
                radical.Add(new SingleElectron(carbon));
                #endregion
            }
        }
    }
}
