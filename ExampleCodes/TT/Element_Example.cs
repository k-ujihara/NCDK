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
                IsotopeFactory f = XMLIsotopeFactory.GetInstance(new Element().Builder);
                IElement e1 = f.GetElement("C");
                IElement e2 = f.GetElement(12);
                #endregion
            }
            {
                #region AtomicNumber
                IAtom element = new Atom("C");
                IsotopeFactory f = XMLIsotopeFactory.GetInstance(element.Builder);
                f.Configure(element);
                #endregion
            }
        }
    }
}
