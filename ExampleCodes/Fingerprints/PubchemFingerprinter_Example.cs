using NCDK.Silent;
using System;

namespace NCDK.Fingerprints
{
        public class PubchemFingerprinter_Example
    {
        public static void Main()
        {
            {
                #region 
                var molecule = new AtomContainer();
                IFingerprinter fingerprinter = new PubchemFingerprinter();
                IBitFingerprint fingerprint = fingerprinter.GetBitFingerprint(molecule);
                Console.WriteLine(fingerprint.Length); // returns 881
                #endregion
            }
        }
    }
}
