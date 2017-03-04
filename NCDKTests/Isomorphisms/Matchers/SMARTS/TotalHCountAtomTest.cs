using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace NCDK.Isomorphisms.Matchers.SMARTS
{
    /// <summary>
    // @author John May
    // @cdk.module test-smarts
    /// </summary>
    [TestClass()]
    public class TotalHCountAtomTest
    {
        [TestMethod()]
        public void Matches()
        {
            TotalHCountAtom matcher = new TotalHCountAtom(4, new Mock<IChemObjectBuilder>().Object);
            var mock_atom = new Mock<IAtom>();
            IAtom atom = mock_atom.Object;
            mock_atom.Setup(n => n.GetProperty<SMARTSAtomInvariants>(SMARTSAtomInvariants.Key)).Returns(
                        new SMARTSAtomInvariants(new Mock<IAtomContainer>().Object, 0, 0,
                        new int[0], 0,
                                0, 0, 4));
            Assert.IsTrue(matcher.Matches(atom));
        }

        [TestMethod()]
        public void TestToString()
        {
            TotalHCountAtom total = new TotalHCountAtom(4, new Mock<IChemObjectBuilder>().Object);
            Assert.AreEqual("H4", total.ToString());
        }
    }
}
