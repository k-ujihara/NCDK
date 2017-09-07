using System.Runtime.InteropServices;

namespace ACDK
{
    [Guid("6735287C-A8F7-417A-AC45-848EBAEED4C0")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface ICircularFingerprinterProvider
    {
        [DispId(0x2001)] ICircularFingerprinter CreateECFP0();
        [DispId(0x2002)] ICircularFingerprinter CreateECFP2();
        [DispId(0x2003)] ICircularFingerprinter CreateECFP4();
        [DispId(0x2004)] ICircularFingerprinter CreateECFP6();
        [DispId(0x2005)] ICircularFingerprinter CreateFCFP0();
        [DispId(0x2006)] ICircularFingerprinter CreateFCFP2();
        [DispId(0x2007)] ICircularFingerprinter CreateFCFP4();
        [DispId(0x2008)] ICircularFingerprinter CreateFCFP6();
    }

    [Guid("C91CF814-2A57-40E5-B9AB-098B48EAF1F3")]
    [ComDefaultInterface(typeof(ICircularFingerprinterProvider))]
    public sealed partial class CircularFingerprinterProvider
        : ICircularFingerprinterProvider
    {
        [DispId(0x2001)] public ICircularFingerprinter CreateECFP0() => new CircularFingerprinter(new NCDK.Fingerprints.CircularFingerprinter(NCDK.Fingerprints.CircularFingerprinter.CLASS_ECFP0));
        [DispId(0x2002)] public ICircularFingerprinter CreateECFP2() => new CircularFingerprinter(new NCDK.Fingerprints.CircularFingerprinter(NCDK.Fingerprints.CircularFingerprinter.CLASS_ECFP2));
        [DispId(0x2003)] public ICircularFingerprinter CreateECFP4() => new CircularFingerprinter(new NCDK.Fingerprints.CircularFingerprinter(NCDK.Fingerprints.CircularFingerprinter.CLASS_ECFP4));
        [DispId(0x2004)] public ICircularFingerprinter CreateECFP6() => new CircularFingerprinter(new NCDK.Fingerprints.CircularFingerprinter(NCDK.Fingerprints.CircularFingerprinter.CLASS_ECFP6));
        [DispId(0x2005)] public ICircularFingerprinter CreateFCFP0() => new CircularFingerprinter(new NCDK.Fingerprints.CircularFingerprinter(NCDK.Fingerprints.CircularFingerprinter.CLASS_FCFP0));
        [DispId(0x2006)] public ICircularFingerprinter CreateFCFP2() => new CircularFingerprinter(new NCDK.Fingerprints.CircularFingerprinter(NCDK.Fingerprints.CircularFingerprinter.CLASS_FCFP2));
        [DispId(0x2007)] public ICircularFingerprinter CreateFCFP4() => new CircularFingerprinter(new NCDK.Fingerprints.CircularFingerprinter(NCDK.Fingerprints.CircularFingerprinter.CLASS_FCFP4));
        [DispId(0x2008)] public ICircularFingerprinter CreateFCFP6() => new CircularFingerprinter(new NCDK.Fingerprints.CircularFingerprinter(NCDK.Fingerprints.CircularFingerprinter.CLASS_FCFP6));
    }
}
