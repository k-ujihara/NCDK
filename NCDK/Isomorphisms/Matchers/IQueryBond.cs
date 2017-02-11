using System;

namespace NCDK.Isomorphisms.Matchers
{
    public interface IQueryBond
        : IBond
    {
       bool Matches(IBond bond);
    }
}
