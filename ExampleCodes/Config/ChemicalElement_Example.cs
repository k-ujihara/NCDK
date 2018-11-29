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
                var c = ChemicalElement.Of(6);
                // oxygen
                var o = ChemicalElement.Of(8);
                #endregion
            }
            {
                #region ToAtomicNumber
                var a = ChemicalElement.OfSymbol("c").AtomicNumber;
                var b = ChemicalElement.OfSymbol("C").AtomicNumber;
                var c = ChemicalElement.OfSymbol("Carbon").AtomicNumber;
                var d = ChemicalElement.OfSymbol("carbon").AtomicNumber;
                #endregion
            }
        }
    }
}
