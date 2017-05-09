using NCDK.Signatures;

namespace NCDK.SMSD.Labelling
{
    // @cdk.module smsd
    // @cdk.githash
    public class MoleculeSignatureLabellingAdaptor : ICanonicalMoleculeLabeller
    {
        public IAtomContainer GetCanonicalMolecule(IAtomContainer container)
        {
            return AtomContainerAtomPermutor.Permute(GetCanonicalPermutation(container), container);
        }

        public int[] GetCanonicalPermutation(IAtomContainer container)
        {
            MoleculeSignature molSig = new MoleculeSignature(container);
            return molSig.GetCanonicalLabels();
        }
    }
}
