namespace NCDK.Isomorphisms
{
    class StereoMatch_Example
    {
        void Main()
        {
            IAtomContainer query = null;
            IAtomContainer target = null;
            Mappings something = null;
            {
                #region
                StereoMatch f = new StereoMatch(query, target);
                Mappings mappings = something; // from subgraph isomorphism etc.
                Mappings stereoMappings = mappings.Filter(f.Apply);
                #endregion
            }
        }
    }
}
