using System;

namespace NCDK
{
    /// <summary>    
    /// A chemical substance that consists of one or more chemical structures.
    /// Examples uses include that of a racemic mixture, a drug composition, and
    /// a nanomaterial with impurities.
    /// </summary>
    public interface ISubstance 
		: IAtomContainerSet<IAtomContainer>
    {
    }
}
