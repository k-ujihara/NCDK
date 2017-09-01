using Microsoft.VisualStudio.TestTools.UnitTesting;
using ACDK;
using System;

namespace ACDK.Tests
{
    [TestClass()]
    public class IDictionary_string_intTests
    {
        [TestMethod()]
        public void IDictionary_string_intTest()
        {
            var dic = new System.Collections.Generic.Dictionary<string, int>() { { "a", 1 }, { "b", 2 } };
            IDictionary_string_int o = new W_IDictionary_string_int(dic);
            Assert.AreEqual(dic.Count, o.Count);
            Assert.AreEqual(2, o["b"]);
        }
    }
}