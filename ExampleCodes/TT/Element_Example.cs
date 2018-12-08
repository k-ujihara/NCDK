using NCDK.Config;
using NCDK.Default;

namespace NCDK.TT
{
    class Element_Example
    {
        public static void Main(string[] args)
        {
            {
                #region
                IsotopeFactory f = XMLIsotopeFactory.Instance;
                ChemicalElement e1 = f.GetElement("C");
                ChemicalElement e2 = f.GetElement(12);
                #endregion
            }
            {
                #region AtomicNumber
                IAtom element = new Atom("C");
                IsotopeFactory f = XMLIsotopeFactory.Instance;
                f.Configure(element);
                #endregion
            }
        }
    }
}
