using NCDK.Default;

namespace NCDK.Config
{
    public class ChemicalElement_Example
    {
        public void Main()
        {
            {
                #region OfNumber
                // carbon
                var c = NaturalElement.OfNumber(6);
                // oxygen
                var o = NaturalElement.OfNumber(8);
                #endregion
            }
            {
                #region ToAtomicNumber
                var a = NaturalElement.ToAtomicNumber("c");
                var b = NaturalElement.ToAtomicNumber("C");
                var c = NaturalElement.ToAtomicNumber("Carbon");
                var d = NaturalElement.ToAtomicNumber("carbon");
                #endregion
            }
        }
    }
}
