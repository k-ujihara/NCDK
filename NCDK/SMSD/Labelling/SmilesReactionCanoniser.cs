using System;

namespace NCDK.SMSD.Labelling
{
    // @cdk.module smsd
    // @cdk.githash
    [Obsolete("This class is part of SMSD and either duplicates functionality elsewhere in the CDK or provides public access to internal implementation details. SMSD has been deprecated from the CDK and a newer, more recent version of SMSD is available at http://github.com/asad/smsd .")]
    public class SmilesReactionCanoniser : AbstractReactionLabeller, ICanonicalReactionLabeller
    {
        private CanonicalLabellingAdaptor labeller = new CanonicalLabellingAdaptor();

        public IReaction GetCanonicalReaction(IReaction reaction)
        {
            return base.LabelReaction(reaction, labeller);
        }
    }
}
