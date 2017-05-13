using NCDK.Silent;
using System;

namespace NCDK.Fingerprints
{
    public class Fingerprinter_Example
    {
        public void Main()
        {
            {
                #region 
                var molecule = new AtomContainer();
                var fingerprinter = new Fingerprinter();
                var fingerprint = fingerprinter.GetBitFingerprint(molecule);
                Console.WriteLine(fingerprint.Count); // returns 1024 by default
                #endregion
            }
        }
    }
}
