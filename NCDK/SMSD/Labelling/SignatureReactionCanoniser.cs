namespace NCDK.SMSD.Labelling
{
    // @cdk.module smsd
    // @cdk.githash
    public class SignatureReactionCanoniser : AbstractReactionLabeller, ICanonicalReactionLabeller
    {
        private MoleculeSignatureLabellingAdaptor labeller = new MoleculeSignatureLabellingAdaptor();

        public IReaction GetCanonicalReaction(IReaction reaction)
        {
            return base.LabelReaction(reaction, labeller);
        }
    }
}