using System;

namespace NCDK.SMSD.Labelling
{
    // @cdk.module smsd
    [Obsolete("This class is part of SMSD and either duplicates functionality elsewhere in the CDK or provides public access to internal implementation details. SMSD has been deprecated from the CDK and a newer, more recent version of SMSD is available at http://github.com/asad/smsd .")]
    public class SignatureReactionCanoniser : AbstractReactionLabeller, ICanonicalReactionLabeller
    {
        private MoleculeSignatureLabellingAdaptor labeller = new MoleculeSignatureLabellingAdaptor();

        public IReaction GetCanonicalReaction(IReaction reaction)
        {
            return base.LabelReaction(reaction, labeller);
        }
    }
}