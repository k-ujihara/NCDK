namespace NCDK
{
    class IAtomContainer_Example
    {
        void Main()
        {
            IAtomContainer atomContainer = null;
            #region
            foreach (var bond in atomContainer.Bonds)
            {
                // do something
            }
            #endregion
        }
    }
}
