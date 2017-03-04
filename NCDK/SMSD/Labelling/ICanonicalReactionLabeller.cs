namespace NCDK.SMSD.Labelling
{
    /// <summary>
    // @cdk.module  smsd
    // @cdk.githash
    /// </summary>
    public interface ICanonicalReactionLabeller
    {
        /// <summary>
        /// Convert a reaction into a canonical form by canonizing each of the
        /// structures in the reaction in turn.
        ///
        /// <param name="reaction">the <see cref="IReaction"/> to be processed</param>
        /// <returns>the canonical <see cref="IReaction"/></returns>
        /// </summary>
        IReaction GetCanonicalReaction(IReaction reaction);
    }
}
