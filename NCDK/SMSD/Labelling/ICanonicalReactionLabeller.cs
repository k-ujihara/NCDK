namespace NCDK.SMSD.Labelling
{
    /**
     * @cdk.module  smsd
     * @cdk.githash
     */
    public interface ICanonicalReactionLabeller
    {
        /**
         * Convert a reaction into a canonical form by canonizing each of the
         * structures in the reaction in turn.
         *
         * @param  reaction the {@link IReaction} to be processed
         * @return          the canonical {@link IReaction}
         */
        IReaction GetCanonicalReaction(IReaction reaction);
    }
}
