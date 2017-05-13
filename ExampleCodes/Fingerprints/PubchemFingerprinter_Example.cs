using NCDK.Silent;
using System;

namespace NCDK.Fingerprints
{
        public class PubchemFingerprinter_Example
    {
        public void Main()
        {
            {
                #region 
                var molecule = new AtomContainer();
                IFingerprinter fingerprinter = new PubchemFingerprinter(Silent.ChemObjectBuilder.Instance);
                IBitFingerprint fingerprint = fingerprinter.GetBitFingerprint(molecule);
                Console.WriteLine(fingerprint.Count); // returns 881
                #endregion
            }
        }
    }
}
