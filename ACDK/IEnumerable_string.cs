using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace ACDK
{
    [Guid("761E5796-7CC6-4722-B7EF-EC754B25694E")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IEnumerable_string
    {
    }

    [ComVisible(false)]
    public sealed class W_IEnumerable_string
        : IEnumerable_string
    {
        System.Collections.Generic.IEnumerable<string> obj;
        public System.Collections.Generic.IEnumerable<string> Object => obj;
        public W_IEnumerable_string(System.Collections.Generic.IEnumerable<string> obj)
        {
            this.obj = obj;
        }
    }
}
