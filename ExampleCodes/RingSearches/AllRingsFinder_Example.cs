namespace NCDK.RingSearches
{
    class AllRingsFinder_Example
    {
        public static void Main()
        {
            {
                IChemObjectSet<IAtomContainer> ms = null;
                #region
                AllRingsFinder arf = new AllRingsFinder();
                foreach (var m in ms)
                {
                    try
                    {
                        IRingSet rs = arf.FindAllRings(m);
                    }
                    catch (CDKException)
                    {
                        // molecule was too complex, handle error
                    }
                }
                #endregion
            }
            {
                #region UsingThreshold
                // using static NCDK.RingSearches.AllRingsFinder.Threshold;
                AllRingsFinder arf = AllRingsFinder.UsingThreshold(Threshold.PubChem99);
                #endregion
            }
        }
    }
}
