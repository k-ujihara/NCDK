using NCDK.Isomorphisms;

namespace NCDK.Smiles.SMARTS
{
    class SmartsPattern_Example
    {
        static void Main()
        {
            IChemObjectSet<IAtomContainer> acs = null;
            {
                #region 1
                Pattern ptrn = SmartsPattern.Create("O[C@?H](C)CC");

                foreach (var ac in acs)
                {
                    if (ptrn.Matches(ac))
                    {
                        // 'ac' contains the pattern
                    }
                }
                #endregion
            }
            {
                int nUniqueHits = 0;
                #region 2
                Pattern ptrn = SmartsPattern.Create("O[C@?H](C)CC");

                foreach (var ac in acs)
                {
                    nUniqueHits += ptrn.MatchAll(ac).CountUnique();
                }
                #endregion
            }
            {
                #region MatchAll
                 Pattern ptrn = SmartsPattern.Create("O[C@?H](C)CC");
                 int nUniqueHits = 0;
                
                 foreach (var ac in acs) {
                   nUniqueHits += ptrn.MatchAll(ac).CountUnique();
                 }
                 #endregion
            }
        }
    }
}
