using Microsoft.VisualStudio.TestTools.UnitTesting;
using ACDK;
using System;
using System.Collections;
using System.Web.Script.Serialization;
using NCDK.Common.Collections;

namespace ACDK.Tests
{
    [TestClass()]
    public class IDictionary_string_intTests
    {
        static JavaScriptSerializer Serializer { get; } = new JavaScriptSerializer();

        [TestMethod()]
        public void IDictionary_string_intTest()
        {
            var dic = new System.Collections.Generic.Dictionary<string, int>() { { "a", 1 }, { "b", 2 } };
            IDictionary_string_int o = new W_IDictionary_string_int(dic);
            Assert.AreEqual(dic.Count, o.Count);
            Assert.AreEqual(2, o["b"]);
            Assert.AreEqual(Serializer.Serialize(dic), o.ToString());
        }
    }
}