namespace NCDK.Isomorphisms
{
    class UniqueBondMatches_Example
    {
        void Main()
        {
            IAtomContainer query = null;
            IAtomContainer target = null;
            int[][] queryGraph = null;
            {
                #region
                Pattern pattern = Ullmann.CreateSubstructureFinder(query);
                var unique = pattern.MatchAll(target).Filter(new UniqueBondMatches(queryGraph).Apply);
                #endregion
            }
        }
    }
}
