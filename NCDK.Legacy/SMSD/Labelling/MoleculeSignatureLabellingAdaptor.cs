using NCDK.Signatures;
using System;

namespace NCDK.SMSD.Labelling
{
    // @cdk.module smsd
    [Obsolete("SMSD has been deprecated from the CDK with a newer, more recent version of SMSD is available at http://github.com/asad/smsd . ")]
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
