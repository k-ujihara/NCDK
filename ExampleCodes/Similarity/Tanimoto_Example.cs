using NCDK.Fingerprint;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.Similarity
{
    class Tanimoto_Example
    {
        void Main()
        {
            IAtomContainer molecule1 = null;
            IAtomContainer molecule2 = null;
            Fingerprinter fingerprinter = null;

            #region 1
            BitArray fingerprint1 = fingerprinter.GetBitFingerprint(molecule1).AsBitSet();
            BitArray fingerprint2 = fingerprinter.GetBitFingerprint(molecule2).AsBitSet();
            double tanimoto_coefficient = Tanimoto.Calculate(fingerprint1, fingerprint2);
            #endregion
        }
    }
}
