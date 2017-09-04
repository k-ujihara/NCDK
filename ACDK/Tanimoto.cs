using NCDK.Common.Collections;
using System;
using System.Runtime.InteropServices;

namespace ACDK
{
    [Guid("998657A6-C510-49E6-8145-BFE5A24D007E")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ITanimoto
    {
        [DispId(0x1001)]
        double Calculate_IBitFingerprint(IBitFingerprint fingerprint1, IBitFingerprint fingerprint2);

        [DispId(0x1002)]
        double Calculate_String(string fingerprint1, string fingerprint2);

        [DispId(0x1003)]
        double Calculate_Dictionary(IDictionary_string_int fingerprint1, IDictionary_string_int fingerprint2);
    }

    [Guid("5CBE3488-10B3-4462-85D7-0A01CDAA2995")]
    [ComDefaultInterface(typeof(ITanimoto))]
    public sealed class Tanimoto
        : ITanimoto
    {
        [DispId(0x1001)]
        public double Calculate_IBitFingerprint(IBitFingerprint fingerprint1, IBitFingerprint fingerprint2)
        {
            return NCDK.Similarity.Tanimoto.Calculate(fingerprint1.AsBitArray(), fingerprint2.AsBitArray());
        }

        [DispId(0x1002)]
        public double Calculate_String(string fingerprint1, string fingerprint2)
        {
            var ba1 = BitArrays.FromString(fingerprint1);
            var ba2 = BitArrays.FromString(fingerprint2);
            return NCDK.Similarity.Tanimoto.Calculate(ba1, ba2);
        }

        [DispId(0x1003)]
        public double Calculate_Dictionary(IDictionary_string_int fingerprint1, IDictionary_string_int fingerprint2)
        {
            var d1 = ((W_IDictionary_string_int)fingerprint1).Object;
            var d2 = ((W_IDictionary_string_int)fingerprint2).Object;
            return NCDK.Similarity.Tanimoto.Calculate(d1, d2);
        }
    }
}
