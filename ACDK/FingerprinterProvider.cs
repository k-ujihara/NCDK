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
        string BitFingerprintToString(IBitFingerprint fingerprint);
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
        public string BitFingerprintToString(IBitFingerprint fingerprint)
        {
            var fp = ((W_IBitFingerprint)fingerprint).Object;
            var bs = fp.AsBitSet();
            return BitArrays.ToString(bs);
        }
    }
}
