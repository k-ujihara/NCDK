namespace NCDK.Isomorphisms
{
    class VentoFoggia_Example
    {
        void Main()
        {
            IAtomContainer queryStructure = null;
            IChemObjectSet<IAtomContainer> ms = null;
            {
                #region 1
                IAtomContainer query = queryStructure;
                Pattern pattern = VentoFoggia.CreateSubstructureFinder(query);

                int hits = 0;
                foreach (var m in ms)
                    if (pattern.Matches(m))
                        hits++;
                #endregion
            }
            {
                #region 2
                IAtomContainer query = queryStructure;
                Pattern pattern = VentoFoggia.CreateSubstructureFinder(query);

                int hits = 0;
                foreach (var m in ms)
                {
                    int[] match = pattern.Match(m);
                    if (match.Length > 0)
                        hits++;
                }
                #endregion
            }
        }
    }
}
