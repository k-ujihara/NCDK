namespace NCDK.Config
{
    public class Elements_Example
    {
        public void Main()
        {
            {
                #region OfNumber
                // carbon
                Elements c = Elements.OfNumber(6);
                // oxygen
                Elements o = Elements.OfNumber(8);
                #endregion
            }
            {
                #region OfString
                 Elements a = Elements.OfString("c");
                 Elements b = Elements.OfString("C");
                 Elements c = Elements.OfString("Carbon");
                 Elements d = Elements.OfString("carbon");
                #endregion
            }
        }
    }
}
