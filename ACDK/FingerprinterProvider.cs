using NCDK.Common.Collections;
using NCDK.Fingerprints;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ACDK
{
    [Guid("C716184D-D385-4056-811B-6F447A0C4178")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IFingerprinterProvider
    {
        [DispId(0x2001)]
        IFingerprinter GetFingerprinter(string name);

        [DispId(0x2002)]
        IBitFingerprint BitFingerprintFromString(string @string);
        [DispId(0x2003)]
        string BitFingerprintToString(IBitFingerprint fingerprint);

        [DispId(0x2004)]
        IBitFingerprint BitFingerprintFromBitString(string @string);
        [DispId(0x2005)]
        string BitFingerprintToBitString(IBitFingerprint fingerprint);
    }

    [Guid("2F703FC5-CEB0-4F1D-A083-457A7EB3D976")]
    [ComDefaultInterface(typeof(IFingerprinterProvider))]
    public class FingerprinterProvider
       : IFingerprinterProvider
    {
        [DispId(0x2001)]
        public IFingerprinter GetFingerprinter(string name)
        {
            string className = "ACDK." + name;
            var type = Assembly.GetExecutingAssembly().GetType(className);
            var ctor = type.GetConstructor(Type.EmptyTypes);
            var obj = ctor.Invoke(new object[0]);
            return (IFingerprinter)obj;
        }
        
        [DispId(0x2002)]
        public IBitFingerprint BitFingerprintFromString(string @string)
        {
            return new BitSetFingerprint(new NCDK.Fingerprints.BitSetFingerprint(BitArrays.FromString(@string)));
        }

        [DispId(0x2003)]
        public string BitFingerprintToString(IBitFingerprint fingerprint)
        {
            return fingerprint.ToString();
        }

        [DispId(0x2004)]
        public IBitFingerprint BitFingerprintFromBitString(string @string)
        {
            return new BitSetFingerprint(new NCDK.Fingerprints.BitSetFingerprint(BitArrays.FromString(@string)));
        }

        [DispId(0x2005)]
        public string BitFingerprintToBitString(IBitFingerprint fingerprint)
        {
            return BitArrays.AsBitString(fingerprint.AsBitArray());
        }
    }
}
