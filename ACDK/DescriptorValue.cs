using System;
using System.Runtime.InteropServices;

namespace ACDK
{
    public partial interface DescriptorValue
    {
        [DispId(0x2001)]
        string ValueAsString();
    }

    public sealed partial class W_DescriptorValue
        : DescriptorValue
    {
        public string ValueAsString()
        {
            return this.Object.Value.ToString();
        }
    }
}
