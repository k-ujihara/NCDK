using NCDK.Isomorphisms.Matchers;

namespace NCDK.Isomorphisms
{
    class DfState_Example
    {
        void Main()
        {
            IQueryAtomContainer query = null;
            IAtomContainer mol = null;
            #region
            var state = new DfState(query);
            state.SetMol(mol);
            int count = 0;
            foreach (int[] amap in state)
            {
                // amap is permutation of query to molecule
                ++count;
            }
            #endregion
        }
    }
}
