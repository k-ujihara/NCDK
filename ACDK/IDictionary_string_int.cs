using System;
using System.Runtime.InteropServices;

namespace ACDK
{
    [Guid("62013D6F-5ADD-41F9-9482-875C762F8B51")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IDictionary_string_int
    {
    }

    [ComVisible(false)]
    public sealed partial class W_IDictionary_string_int
    : IDictionary_string_int
    {
        System.Collections.Generic.IDictionary<string, int> obj;
        public System.Collections.Generic.IDictionary<string, int> Object => obj;
        public W_IDictionary_string_int(System.Collections.Generic.IDictionary<string, int> obj)
        {
            this.obj = obj;
        }
    }
}
