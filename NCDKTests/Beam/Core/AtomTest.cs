using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NCDK.Beam
{
    /// <summary> <author>John May </author></summary>
    [TestClass()]
    public class AtomTest
    {
        [TestMethod()]
        public void AliphaticSubsetFromElement()
        {
            foreach (var a in AtomImpl.AliphaticSubset.Values)
            {
                Assert.AreEqual(AtomImpl.AliphaticSubset.OfElement(a.Element), a);
            }
        }

        [TestMethod()]
        public void AliphaticSubsetInvalidElement()
        {
            try
            {
                AtomImpl.AliphaticSubset.OfElement(Element.Californium);
                Assert.Fail();
            }
            catch (ArgumentException)
            { }
        }

        [TestMethod()]
        public void IsAromaticSubsetFromElement()
        {
            foreach (var a in AtomImpl.AromaticSubset.Values)
            {
                Assert.AreEqual(AtomImpl.AromaticSubset.OfElement(a.Element), a);
            }
        }

        [TestMethod()]
        public void IsAromaticSubsetInvalidElement()
        {
            try
            {
                AtomImpl.AromaticSubset.OfElement(Element.Unknown);
                Assert.Fail();
            }
            catch (ArgumentException)
            { }
        }
    }
}