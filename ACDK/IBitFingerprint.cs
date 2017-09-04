using NCDK.Common.Collections;
using System.Runtime.InteropServices;

namespace ACDK
{
    public partial interface IBitFingerprint
    {
        [DispId(0x2001)]
        string ToString();

        [ComVisible(false)]
        System.Collections.BitArray AsBitArray();
    }

    public sealed partial class W_IBitFingerprint
    {
        public override string ToString()
        {
            var bs = Object.AsBitSet();
            return BitArrays.ToString(bs);
        }

        public System.Collections.BitArray AsBitArray()
        {
            return Object.AsBitSet();
        }
    }

    public sealed partial class BitSetFingerprint
    {
        public System.Collections.BitArray AsBitArray()
            => Object.AsBitSet();
    }

    public sealed partial class IntArrayFingerprint
    {
        public System.Collections.BitArray AsBitArray()
            => Object.AsBitSet();
    }
}
