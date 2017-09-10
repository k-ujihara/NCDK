/*
 * Copyright (C) 2017  Kazuya Ujihara <ujihara.kazuya@gmail.com>
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */



using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace ACDK
{
    [Guid("F65DEF09-DB98-49EA-BB85-0E80C469621F")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IDictionary_object_object
    {
        [DispId(0x2001)]
        object this[object key] { get; }

        [DispId(0x2002)]
        int Count { get; }

        object[] Keys
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
    public sealed partial class W_IDictionary_object_object
        : IDictionary_object_object
    {
        System.Collections.Generic.IDictionary<object, object> obj;
        public System.Collections.Generic.IDictionary<object, object> Object => obj;
        public W_IDictionary_object_object(System.Collections.Generic.IDictionary<object, object> obj)
        {
            this.obj = obj;
        }

        public object this[object key] => Object[key];

        public int Count => Object.Count;

        public object[] Keys
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
            this.obj = ser.Deserialize<System.Collections.Generic.Dictionary<object, object>>(json);
        }
    }
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

