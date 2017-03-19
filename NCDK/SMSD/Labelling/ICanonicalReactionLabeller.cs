namespace NCDK.SMSD.Labelling
{
    // @cdk.module  smsd
    // @cdk.githash
    public interface ICanonicalReactionLabeller
    {
        /// <summary>
        /// Convert a reaction into a canonical form by canonizing each of the
        /// structures in the reaction in turn.
        /// </summary>
        /// <param name="reaction">the <see cref="IReaction"/> to be processed</param>
        /// <returns>the canonical <see cref="IReaction"/></returns>
        IReaction GetCanonicalReaction(IReaction reaction);
    }
}
