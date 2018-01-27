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
                var c = ChemicalElement.OfNumber(6);
                // oxygen
                var o = ChemicalElement.OfNumber(8);
                #endregion
            }
            {
                #region OfString
                var a = ChemicalElement.OfString("c");
                var b = ChemicalElement.OfString("C");
                var c = ChemicalElement.OfString("Carbon");
                var d = ChemicalElement.OfString("carbon");
                #endregion
            }
        }
    }
}
