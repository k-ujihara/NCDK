using System;
using System.Collections.Generic;

namespace NCDK.SMSD.Labelling
{
    // @cdk.module smsd
    [Obsolete("SMSD has been deprecated from the CDK with a newer, more recent version of SMSD is available at http://github.com/asad/smsd . ")]
    public interface ICanonicalMoleculeLabeller
    {
        IAtomContainer GetCanonicalMolecule(IAtomContainer container);
        int[] GetCanonicalPermutation(IAtomContainer container);
    }
}
