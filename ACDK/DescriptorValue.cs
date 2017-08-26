using System;
using System.Runtime.InteropServices;

namespace ACDK
{
    public partial interface DescriptorValue
    {
        [DispId(0x2001)]
        string ToString();
    }

    public sealed partial class W_DescriptorValue
        : DescriptorValue
    {
        public override string ToString()
        {
            return this.Object.Value.ToString();
        }
    }
}
