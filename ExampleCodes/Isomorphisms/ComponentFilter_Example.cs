using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.Isomorphisms
{
    class ComponentFilter_Example
    {
        void Main()
        {
            IAtomContainer query = null;
            IAtomContainer someTarget = null;
            Pattern somePattern = null;
            int[] grouping = null;
            #region
            // grouping is actually set by SMARTS parser but this shows how it's stored
            query.SetProperty(ComponentFilter.Key, grouping);

            IAtomContainer target = someTarget;
            Pattern pattern = somePattern; // create pattern for query

            // filter for mappings which respect component grouping in the query
            var filter = new ComponentFilter(query, target);
            pattern.MatchAll(target).Where(n => filter.Apply(n));
            #endregion
        }
    }
}
