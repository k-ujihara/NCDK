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
                #region OfString
                var a = NaturalElement.OfString("c");
                var b = NaturalElement.OfString("C");
                var c = NaturalElement.OfString("Carbon");
                var d = NaturalElement.OfString("carbon");
                #endregion
            }
        }
    }
}
