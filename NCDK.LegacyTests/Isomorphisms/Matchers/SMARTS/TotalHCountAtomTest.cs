using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace NCDK.Isomorphisms.Matchers.SMARTS
{
    // @author John May
    // @cdk.module test-smarts
    [TestClass()]
    public class TotalHCountAtomTest
    {
        [TestMethod()]
        public void Matches()
        {
            var matcher = new TotalHCountAtom(4);
            var mock_atom = new Mock<IAtom>();
            var atom = mock_atom.Object;
            mock_atom.Setup(n => n.GetProperty<SMARTSAtomInvariants>(SMARTSAtomInvariants.Key)).Returns(
                        new SMARTSAtomInvariants(new Mock<IAtomContainer>().Object, 0, 0,
                        Array.Empty<int>(), 0,
                                0, 0, 4));
            Assert.IsTrue(matcher.Matches(atom));
        }

        [TestMethod()]
        public void TestToString()
        {
            var total = new TotalHCountAtom(4);
            Assert.AreEqual("H4", total.ToString());
        }
    }
}
