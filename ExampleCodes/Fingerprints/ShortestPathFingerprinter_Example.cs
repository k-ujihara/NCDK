using NCDK.Silent;
using NCDK.Tools.Manipulator;
using System;

namespace NCDK.Fingerprints
{
    public class ShortestPathFingerprinter_Example
    {
        public void Main()
        {
            {
                #region 
                var molecule = new AtomContainer();
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
                var fingerprinter = new ShortestPathFingerprinter();
                var fingerprint = fingerprinter.GetBitFingerprint(molecule);
                Console.WriteLine(fingerprint.Length); // returns 1024 by default
                #endregion
            }
        }
    }
}
