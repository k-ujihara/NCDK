using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace ACDK
{
    [Guid("62013D6F-5ADD-41F9-9482-875C762F8B51")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IDictionary_string_int
    {
        [DispId(0x2001)]
        int this[string key] { get; }

        [DispId(0x2002)]
        int Count { get; }

        string[] Keys
        {
            [DispId(0x2003)]
            [return: MarshalAs(UnmanagedType.SafeArray)]
            get;
        }

        [DispId(0x2004)]
        string ToString();

        [DispId(0x2005)]
        void Load(string json);
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

        public int this[string key] => Object[key];

        public int Count => Object.Count;

        public string[] Keys
        {
            [return: MarshalAs(UnmanagedType.SafeArray)]
            get
            {
                return Object.Keys.ToArray();
            }
        }

        static System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();

        public override string ToString()
        {
            return ser.Serialize(Object);
        }

        public void Load(string json)
        {
            this.obj = ser.Deserialize<System.Collections.Generic.Dictionary<string, int>>(json);
        }
    }
}
