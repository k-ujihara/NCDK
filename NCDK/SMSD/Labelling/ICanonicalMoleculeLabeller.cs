using System;
using System.ComponentModel;

namespace NCDK.SMSD.Labelling
{
    // @cdk.module smsd
    // @cdk.githash
    [Category("Legacy")]
    [Obsolete("SMSD has been deprecated from the CDK with a newer, more recent version of SMSD is available at http://github.com/asad/smsd . ")]
    public interface ICanonicalMoleculeLabeller
    {
        IAtomContainer GetCanonicalMolecule(IAtomContainer container);
        int[] GetCanonicalPermutation(IAtomContainer container);
    }
}
