using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.Beam
{
    /// <summary> <author>John May </author>*/
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
        [ExpectedException(typeof(ArgumentException))]
        public void AliphaticSubsetInvalidElement()
        {
            AtomImpl.AliphaticSubset.OfElement(Element.Californium);
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
        [ExpectedException(typeof(ArgumentException))]
        public void IsAromaticSubsetInvalidElement()
        {
            AtomImpl.AromaticSubset.OfElement(Element.Unknown);
        }
    }
}