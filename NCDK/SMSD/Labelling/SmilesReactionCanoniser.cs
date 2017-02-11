namespace NCDK.SMSD.Labelling
{
    /**
     * @cdk.module smsd
     * @cdk.githash
     */
    public class SmilesReactionCanoniser : AbstractReactionLabeller, ICanonicalReactionLabeller
    {
        private CanonicalLabellingAdaptor labeller = new CanonicalLabellingAdaptor();

        public IReaction GetCanonicalReaction(IReaction reaction)
        {
            return base.LabelReaction(reaction, labeller);
        }
    }
}
