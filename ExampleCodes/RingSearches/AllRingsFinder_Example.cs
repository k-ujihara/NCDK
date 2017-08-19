using static NCDK.RingSearches.AllRingsFinder.Threshold;

namespace NCDK.RingSearches
{
    class AllRingsFinder_Example
    {
        public void Main()
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
                AllRingsFinder arf = AllRingsFinder.UsingThreshold(PubChem_99);
                #endregion
            }
        }
    }
}
