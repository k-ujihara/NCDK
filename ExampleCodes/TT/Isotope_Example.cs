using NCDK.Config;
using NCDK.Default;

namespace NCDK.TT
{
    class Isotope_Example
    {
        public static void Main(string[] args)
        {
            {
                #region 1
                Isotope carbon = new Isotope("C", 13);
                #endregion
            }
            {
                #region 2
                // make deuterium
                Isotope carbon = new Isotope(1, "H", 2, 2.01410179, 100.0);
                #endregion
            }
            {
                #region NaturalAbundance
                IAtom atom = new Atom();
                Isotope isotope = new Isotope("C", 13);
                XMLIsotopeFactory f = XMLIsotopeFactory.GetInstance(isotope.Builder);
                f.Configure(atom, isotope);
                #endregion
            }
        }
    }
}
