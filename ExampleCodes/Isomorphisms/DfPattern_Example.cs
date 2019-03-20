using NCDK.Isomorphisms.Matchers;

namespace NCDK.Isomorphisms
{
    class DfPattern_Example
    {
        void Main()
        {
            IQueryAtomContainer query = null;
            IAtomContainer mol = null;
            #region
            var pattern = DfPattern.FindSubstructure(query);
            // has match?
            if (pattern.Matches(mol))
            {
            }
            // get lazy mapping iterator
            foreach (var atom in mol.Atoms)
            {
                if (pattern.MatchesRoot(atom))
                {
                }
            }
            #endregion
        }
    }
}
