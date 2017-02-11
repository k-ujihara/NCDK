using System;
using System.Collections.Generic;

namespace NCDK
{
    /// <summary>
    /// A set of reactions, for example those taking part in a reaction.
    /// </summary>
    public interface IReactionSet
        : IChemObject, IList<IReaction>
    {
        bool IsEmpty { get; }
    }
}
