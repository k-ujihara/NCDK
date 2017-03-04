using NCDK.Common.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCDK.Hash.Stereo;

namespace NCDK.Hash
{
    /// <summary>
    // @author John May
    // @cdk.module test-hash
    /// </summary>
    public class BasicAtomHashGeneratorTest
    {

        [TestMethod()]
        public void TestGenerate()
        {

            var m_seedMock = new Mock<AtomHashGenerator>(); var seedMock = m_seedMock.Object;
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;

            AtomHashGenerator generator = new BasicAtomHashGenerator(seedMock, new Xorshift(), 0);

            m_seedMock.Setup(n => n.Generate(container)).Returns(new long[0]);
            m_container.SetupGet(n => n.Bonds).Returns(new IBond[0]);

            generator.Generate(container);

            m_seedMock.Verify(n => n.Generate(container), Times.Exactly(1));
        }

        [TestMethod()]
        public void TestGenerate_ZeroDepth()
        {

            var m_seedMock = new Mock<AtomHashGenerator>(); var seedMock = m_seedMock.Object;
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;

            BasicAtomHashGenerator generator = new BasicAtomHashGenerator(new Mock<AtomHashGenerator>().Object, new Xorshift(), 0);

            Assert.IsTrue(Compares.AreDeepEqual(
                new long[] { 1L, 1L, 1L },
                generator.Generate(new long[] { 1L, 1L, 1L }, StereoEncoder.EMPTY, new int[][] { new int[] { }, new int[] { }, new int[] { } },
                    Suppressed.None)));
        }

        [TestMethod()]
        public void TestGenerate_Disconnected()
        {
            var m_seedMock = new Mock<AtomHashGenerator>(); var seedMock = m_seedMock.Object;
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;

            BasicAtomHashGenerator generator = new BasicAtomHashGenerator(new Mock<AtomHashGenerator>().Object, new Xorshift(), 2);
            // there are no neighbours, the values should be rotated
            long expected = generator.Distribute(generator.Distribute(1));
            Assert.IsTrue(Compares.AreDeepEqual(
                new long[] { expected, expected, expected },
                generator.Generate(new long[] { 1L, 1L, 1L }, StereoEncoder.EMPTY, new int[][] { new int[] { }, new int[] { }, new int[] { } },
                Suppressed.None)));
        }

        [TestMethod()]
        public void TestGenerate_Simple()
        {
            var m_seedMock = new Mock<AtomHashGenerator>(); var seedMock = m_seedMock.Object;
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;

            BasicAtomHashGenerator generator = new BasicAtomHashGenerator(new Mock<AtomHashGenerator>().Object, new Xorshift(), 2);

            // first iteration, values are distributed and then neighbours xor'd
            // in. when two neighbout have the same value the second should be
            // rotated
            long[] first = new long[]{generator.Distribute(1) ^ 2L, generator.Distribute(2L) ^ 1L ^ generator.Rotate(1L),
                generator.Distribute(1) ^ 2L};

            long[] second = new long[]{generator.Distribute(first[0]) ^ first[1],
                generator.Distribute(first[1]) ^ first[0] ^ generator.Rotate(first[2]),
                generator.Distribute(first[2]) ^ first[1]};

            Assert.IsTrue(Compares.AreDeepEqual(
                second,
                generator.Generate(new long[] { 1L, 2L, 1L }, StereoEncoder.EMPTY, new int[][] { new int[] { 1 }, new int[] { 0, 2 }, new int[] { 1 } },
                    Suppressed.None)));
        }

        [TestMethod()]
        public void TestRotation()
        {
            var m_seedMock = new Mock<AtomHashGenerator>(); var seedMock = m_seedMock.Object;
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;

            BasicAtomHashGenerator generator = new BasicAtomHashGenerator(new Mock<AtomHashGenerator>().Object, new Xorshift(), 2);

            int[][] graph = new int[][] { new int[] { 1, 2, 3 }, new int[] { 0 }, new int[] { 0 }, new int[] { 0 } };

            // simulate 3 identical neighbors
            long[] invs = new long[] { 21, 31, 31, 31 };
            long[] unique = new long[4];
            long[] rotated = new long[4];

            long value = generator.Next(graph, 0, invs, unique, rotated);

            Assert.IsTrue(Compares.AreDeepEqual(new long[] { 31, 0, 0, 0 }, unique));
            Assert.IsTrue(Compares.AreDeepEqual(new long[] { generator.Rotate(31, 2), 0, 0, 0 }, rotated));
            Assert.AreEqual(generator.Distribute(21) ^ 31 ^ generator.Rotate(31) ^ generator.Rotate(31, 2), value);
        }
    }
}
