using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCDK.Hash.Stereo;

namespace NCDK.Hash.Stereo
{
    /// <summary>
    // @author John May
    // @cdk.module test-hash
    /// </summary>
    [TestClass()]
    public class CombinedPermutationParityTest
    {
        [TestMethod()]
        public void TestParity()
        {
            var m_left = new Mock<PermutationParity>(); var left = m_left.Object;
            var m_right = new Mock<PermutationParity>(); var right = m_right.Object;
            PermutationParity parity = new CombinedPermutationParity(left, right);

            long[] dummy = new long[5];

            m_left.Setup(n => n.Parity(dummy)).Returns(-1);
            m_right.Setup(n => n.Parity(dummy)).Returns(-1);
            Assert.AreEqual(1, parity.Parity(dummy));

            m_left.Verify(n => n.Parity(dummy), Times.Exactly(1));
            m_right.Verify(n => n.Parity(dummy), Times.Exactly(1));

            m_left.Setup(n => n.Parity(dummy)).Returns(-1);
            m_right.Setup(n => n.Parity(dummy)).Returns(1);
            Assert.AreEqual(-1, parity.Parity(dummy));

            m_left.Setup(n => n.Parity(dummy)).Returns(1);
            m_right.Setup(n => n.Parity(dummy)).Returns(-1);
            Assert.AreEqual(-1, parity.Parity(dummy));

            m_left.Setup(n => n.Parity(dummy)).Returns(1);
            m_right.Setup(n => n.Parity(dummy)).Returns(1);
            Assert.AreEqual(1, parity.Parity(dummy));

            m_left.Setup(n => n.Parity(dummy)).Returns(0);
            m_right.Setup(n => n.Parity(dummy)).Returns(1);
            Assert.AreEqual(0, parity.Parity(dummy));

            m_left.Setup(n => n.Parity(dummy)).Returns(1);
            m_right.Setup(n => n.Parity(dummy)).Returns(0);
            Assert.AreEqual(0, parity.Parity(dummy));

            m_left.Setup(n => n.Parity(dummy)).Returns(0);
            m_right.Setup(n => n.Parity(dummy)).Returns(-1);
            Assert.AreEqual(0, parity.Parity(dummy));

            m_left.Setup(n => n.Parity(dummy)).Returns(-1);
            m_right.Setup(n => n.Parity(dummy)).Returns(0);
            Assert.AreEqual(0, parity.Parity(dummy));
        }
    }
}
