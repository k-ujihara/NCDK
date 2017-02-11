using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace NCDK.Hash
{
    /**
     * @author John May
     * @cdk.module test-hash
     */
    [TestClass()]
    public class SeedGeneratorTest
    {

        [TestMethod()]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void TestConstruct_Null()
        {
            new SeedGenerator(null);
        }

        [TestMethod()]
        public void TestGenerate()
        {

            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;

            var m_encoder = new Mock<AtomEncoder>(); var encoder = m_encoder.Object;
            SeedGenerator generator = new SeedGenerator(encoder);

            var m_c1 = new Mock<IAtom>(); var c1 = m_c1.Object;
            var m_c2 = new Mock<IAtom>(); var c2 = m_c2.Object;
            var m_c3 = new Mock<IAtom>(); var c3 = m_c3.Object;
            var m_c4 = new Mock<IAtom>(); var c4 = m_c4.Object;
            var m_c5 = new Mock<IAtom>(); var c5 = m_c5.Object;

            m_container.SetupGet(n => n.Atoms.Count).Returns(5);
            m_container.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_container.SetupGet(n => n.Atoms[1]).Returns(c2);
            m_container.SetupGet(n => n.Atoms[2]).Returns(c3);
            m_container.SetupGet(n => n.Atoms[3]).Returns(c4);
            m_container.SetupGet(n => n.Atoms[4]).Returns(c5);

            m_encoder.Setup(n => n.Encode(c1, container)).Returns(42);
            m_encoder.Setup(n => n.Encode(c2, container)).Returns(42);
            m_encoder.Setup(n => n.Encode(c3, container)).Returns(42);
            m_encoder.Setup(n => n.Encode(c4, container)).Returns(42);
            m_encoder.Setup(n => n.Encode(c5, container)).Returns(42);

            generator.Generate(container);

            m_container.Verify(n => n.Atoms.Count, Times.Exactly(1));

            m_container.Verify(n => n.Atoms[It.IsAny<int>()], Times.Exactly(5));

            m_encoder.Verify(n => n.Encode(c1, container), Times.Exactly(1));
            m_encoder.Verify(n => n.Encode(c2, container), Times.Exactly(1));
            m_encoder.Verify(n => n.Encode(c3, container), Times.Exactly(1));
            m_encoder.Verify(n => n.Encode(c4, container), Times.Exactly(1));
            m_encoder.Verify(n => n.Encode(c5, container), Times.Exactly(1));

            //VerifyNoMoreInteractions(c1, c2, c3, c4, c5, container, encoder);
        }

        [TestMethod()]
        public void TestGenerate_SizeSeeding()
        {

            var m_m1 = new Mock<IAtomContainer>(); var m1 = m_m1.Object;
            var m_m2 = new Mock<IAtomContainer>(); var m2 = m_m2.Object;

            var m_encoder = new Mock<AtomEncoder>(); var encoder = m_encoder.Object;
            SeedGenerator generator = new SeedGenerator(encoder);

            var m_c1 = new Mock<IAtom>(); var c1 = m_c1.Object;
            var m_c2 = new Mock<IAtom>(); var c2 = m_c2.Object;
            var m_c3 = new Mock<IAtom>(); var c3 = m_c3.Object;
            var m_c4 = new Mock<IAtom>(); var c4 = m_c4.Object;
            var m_c5 = new Mock<IAtom>(); var c5 = m_c5.Object;
            var m_c6 = new Mock<IAtom>(); var c6 = m_c6.Object;

            m_m1.SetupGet(n => n.Atoms.Count).Returns(5);
            m_m1.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_m1.SetupGet(n => n.Atoms[1]).Returns(c2);
            m_m1.SetupGet(n => n.Atoms[2]).Returns(c3);
            m_m1.SetupGet(n => n.Atoms[3]).Returns(c4);
            m_m1.SetupGet(n => n.Atoms[4]).Returns(c5);

            m_m2.SetupGet(n => n.Atoms.Count).Returns(6);
            m_m2.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_m2.SetupGet(n => n.Atoms[1]).Returns(c2);
            m_m2.SetupGet(n => n.Atoms[2]).Returns(c3);
            m_m2.SetupGet(n => n.Atoms[3]).Returns(c4);
            m_m2.SetupGet(n => n.Atoms[4]).Returns(c5);
            m_m2.SetupGet(n => n.Atoms[5]).Returns(c6);

            m_encoder.Setup(n => n.Encode(c1, m1)).Returns(42);
            m_encoder.Setup(n => n.Encode(c2, m1)).Returns(42);
            m_encoder.Setup(n => n.Encode(c3, m1)).Returns(42);
            m_encoder.Setup(n => n.Encode(c4, m1)).Returns(42);
            m_encoder.Setup(n => n.Encode(c5, m1)).Returns(42);

            m_encoder.Setup(n => n.Encode(c1, m2)).Returns(42);
            m_encoder.Setup(n => n.Encode(c2, m2)).Returns(42);
            m_encoder.Setup(n => n.Encode(c3, m2)).Returns(42);
            m_encoder.Setup(n => n.Encode(c4, m2)).Returns(42);
            m_encoder.Setup(n => n.Encode(c5, m2)).Returns(42);
            m_encoder.Setup(n => n.Encode(c6, m2)).Returns(42);

            long[] v1 = generator.Generate(m1);
            long[] v2 = generator.Generate(m2);

            m_m1.Verify(n => n.Atoms.Count, Times.Exactly(1));
            m_m2.Verify(n => n.Atoms.Count, Times.Exactly(1));

            m_m1.Verify(n => n.Atoms[It.IsAny<int>()], Times.Exactly(5));
            m_m2.Verify(n => n.Atoms[It.IsAny<int>()], Times.Exactly(6));

            m_encoder.Verify(n => n.Encode(c1, m1), Times.Exactly(1));
            m_encoder.Verify(n => n.Encode(c2, m1), Times.Exactly(1));
            m_encoder.Verify(n => n.Encode(c3, m1), Times.Exactly(1));
            m_encoder.Verify(n => n.Encode(c4, m1), Times.Exactly(1));
            m_encoder.Verify(n => n.Encode(c5, m1), Times.Exactly(1));

            m_encoder.Verify(n => n.Encode(c1, m2), Times.Exactly(1));
            m_encoder.Verify(n => n.Encode(c2, m2), Times.Exactly(1));
            m_encoder.Verify(n => n.Encode(c3, m2), Times.Exactly(1));
            m_encoder.Verify(n => n.Encode(c4, m2), Times.Exactly(1));
            m_encoder.Verify(n => n.Encode(c5, m2), Times.Exactly(1));
            m_encoder.Verify(n => n.Encode(c6, m2), Times.Exactly(1));

            // check the value were different (due to molecule size)
            Assert.AreEqual(5, v1.Length);
            Assert.AreEqual(6, v2.Length);
            for (int i = 0; i < v1.Length; i++)
            {
                Assert.AreNotEqual(v1[i], v2[i]);
            }

            //VerifyNoMoreInteractions(m1, m2, c1, c2, c3, c4, c5, c6, encoder);
        }
    }
}
