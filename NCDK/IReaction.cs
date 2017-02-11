using System;
using System.Collections.Generic;

namespace NCDK
{
    /// <summary>
    /// Represents the idea of a chemical reaction. The reaction consists of
    /// a set of reactants and a set of products.
    /// 
    /// <para>The class mostly represents abstract reactions, such as 2D diagrams,
    /// and is not intended to represent reaction trajectories.Such can better
    /// be represented with a ChemSequence.</para>
    /// </summary>
    public interface IReaction
        : IChemObject
    {
        IAtomContainerSet<IAtomContainer> Reactants { get; }
        IAtomContainerSet<IAtomContainer> Products { get; }
        IAtomContainerSet<IAtomContainer> Agents { get; }
        ReactionDirection Direction { get; set; }
        IList<IMapping> Mappings { get; }
    }
}
