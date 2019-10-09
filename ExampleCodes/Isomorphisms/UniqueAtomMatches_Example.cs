namespace NCDK.Isomorphisms
{
    class UniqueAtomMatches_Example
    {
        void Main()
        {
            IAtomContainer query = null;
            IAtomContainer target = null;
            IChemObjectSet<IAtomContainer> ms = null;
            {
                #region
                Pattern pattern = Ullmann.CreateSubstructureFinder(query);
                pattern.MatchAll(target).Filter(new UniqueAtomMatches().Apply);
                #endregion
            }
        }
    }
}
