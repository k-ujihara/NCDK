using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.Isomorphisms
{
    class Pattern_Example
    {
        void Main()
        {
            Pattern createPattern = null;
            IChemObjectSet<IAtomContainer> ms = null;
            {
                #region Match
                Pattern pattern = createPattern; // create pattern
                foreach (var m in ms)
                {
                    int[] mapping = pattern.Match(m);
                    if (mapping.Length > 0)
                    {
                        // found mapping!
                    }
                }
                #endregion
            }
            {
                #region Matches
                Pattern pattern = createPattern; // create pattern
                foreach (var m in ms)
                {
                    if (pattern.Matches(m))
                    {
                        // found mapping!
                    }
                }
                #endregion
            }
            IAtomContainer target = null;
            IAtomContainer query = null;
            {
                #region MatchAll1
                Pattern pattern = Pattern.CreateSubstructureFinder(query);
                foreach (var m in ms)
                {
                    foreach (int[] mapping in pattern.MatchAll(m))
                    {
                        // found mapping
                    }
                }
                #endregion
            }
            {
                #region MatchAll2
                // find only the first 5 mappings and store them in an array
                Pattern pattern = Pattern.CreateSubstructureFinder(query);
                int[][] mappings = pattern.MatchAll(target)
                                          .Limit(5)
                                          .ToArray();
                #endregion
            }
        }
    }
}
