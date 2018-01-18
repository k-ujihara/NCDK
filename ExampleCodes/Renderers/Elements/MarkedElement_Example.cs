namespace NCDK.Renderers.Elements
{
    class MarkedElement_Example
    {
        void Main()
        {
            IAtom atom = null;
            #region
            atom.SetProperty(MarkedElement.IdKey, "my_atm_id");
            atom.SetProperty(MarkedElement.ClassKey, "h_donor");
            atom.SetProperty(MarkedElement.ClassKey, "h_acceptor");
            #endregion
        }
    }
}
