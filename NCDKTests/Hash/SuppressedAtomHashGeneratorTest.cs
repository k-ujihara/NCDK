using NCDK.Common.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCDK.Hash.Stereo;
using System.Collections;

namespace NCDK.Hash
{
    // @author John May
    // @cdk.module test-hash
    public class SuppressedAtomHashGeneratorTest
    {
        [TestMethod()]
        public void TestGenerate()
        {
            var m_seedMock = new Mock<IAtomHashGenerator>(); var seedMock = m_seedMock.Object;
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;

            IAtomHashGenerator generator = new SuppressedAtomHashGenerator(seedMock, new Xorshift(),
                    AtomSuppression.Unsuppressed, 0);

            m_seedMock.Setup(n => n.Generate(container)).Returns(new long[0]);
            m_container.SetupGet(n => n.Bonds).Returns(new IBond[0]);

            generator.Generate(container);

            m_seedMock.Verify(n => n.Generate(container), Times.Exactly(1));
        }

        [TestMethod()]
        public void TestGenerate_ZeroDepth()
        {
            var m_seedMock = new Mock<IAtomHashGenerator>(); var seedMock = m_seedMock.Object;
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;

            SuppressedAtomHashGenerator generator = new SuppressedAtomHashGenerator(new Mock<IAtomHashGenerator>().Object,
                    new Xorshift(), AtomSuppression.Unsuppressed, 0);

            Assert.IsTrue(Compares.AreDeepEqual(
                new long[] { 1L, 1L, 1L },
                generator.Generate(new long[] { 1L, 1L, 1L }, StereoEncoder.Empty, new int[][] { new int[] { }, new int[] { }, new int[] { } },
                Suppressed.None)));

            BitArray suppressed = new BitArray(3);
            suppressed.Set(0, true);
            suppressed.Set(2, true);

            Assert.IsTrue(Compares.AreDeepEqual(
                new long[] { 0L, 1L, 0L },
                generator.Generate(new long[] { 1L, 1L, 1L }, StereoEncoder.Empty, new int[][] { new int[] { }, new int[] { }, new int[] { } },
                Suppressed.FromBitSet(suppressed))));
        }

        [TestMethod()]
        public void TestGenerate_Disconnected()
        {
            var m_seedMock = new Mock<IAtomHashGenerator>(); var seedMock = m_seedMock.Object;
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;

            SuppressedAtomHashGenerator generator = new SuppressedAtomHashGenerator(new Mock<IAtomHashGenerator>().Object,
                    new Xorshift(), AtomSuppression.Unsuppressed, 2);
            // there are no neighbours, the values should be rotated
            long expected = generator.Distribute(generator.Distribute(1));
            Assert.IsTrue(Compares.AreDeepEqual(
                new long[] { expected, expected, expected },
 generator.Generate(new long[] { 1L, 1L, 1L }, StereoEncoder.Empty, new int[][] { new int[] { }, new int[] { }, new int[] { } },
                        Suppressed.None)));
            BitArray suppressed = new BitArray(3);
            suppressed.Set(1, true);
            Assert.IsTrue(Compares.AreDeepEqual(
                new long[] { expected, 0L, expected },
                            generator.Generate(new long[] { 1L, 1L, 1L }, StereoEncoder.Empty, new int[][] { new int[] { }, new int[] { }, new int[] { } },
                        Suppressed.FromBitSet(suppressed))));
        }

        [TestMethod()]
        public void TestGenerate_Simple()
        {
            var m_seedMock = new Mock<IAtomHashGenerator>(); var seedMock = m_seedMock.Object;
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;

            SuppressedAtomHashGenerator generator = new SuppressedAtomHashGenerator(new Mock<IAtomHashGenerator>().Object,
                    new Xorshift(), AtomSuppression.Unsuppressed, 2);

            // no suppression
            {
                // first iteration, values are distributed and then neighbours xor'd
                // in. when two neighbours have the same value the second should be
                // rotated
                long[] first = new long[]{generator.Distribute(1) ^ 2L,
                    generator.Distribute(2L) ^ 1L ^ generator.Rotate(1L), generator.Distribute(1) ^ 2L};

                long[] second = new long[]{generator.Distribute(first[0]) ^ first[1],
                    generator.Distribute(first[1]) ^ first[0] ^ generator.Rotate(first[2]),
                    generator.Distribute(first[2]) ^ first[1]};

                Assert.IsTrue(Compares.AreDeepEqual(
                    second,
                    generator.Generate(new long[] { 1L, 2L, 1L }, StereoEncoder.Empty, new int[][] { new[] { 1 }, new[] { 0, 2 }, new[] { 1 } },
                    Suppressed.None)));
            }
            // vertex '2' supressed
            BitArray suppressed = new BitArray(3);
            suppressed.Set(2, true);
            {
                long[] first = new long[]{generator.Distribute(1) ^ 2L, generator.Distribute(2L) ^ 1L, // generator.Rotate(1L) not included is '[2]' is suppressed
                    0L,};

                long[] second = new long[]{generator.Distribute(first[0]) ^ first[1], // generator.Rotate(first[2]) not included is '[2]' is suppressed
                    generator.Distribute(first[1]) ^ first[0], 0L};

                Assert.IsTrue(Compares.AreDeepEqual(
                    second,
                    generator.Generate(new long[] { 1L, 2L, 1L }, StereoEncoder.Empty, new int[][] { new[] { 1 }, new[] { 0, 2 }, new[] { 1 } },
                    Suppressed.FromBitSet(suppressed))));
            }
        }

        [TestMethod()]
        public void TestRotation()
        {

            var m_seedMock = new Mock<IAtomHashGenerator>(); var seedMock = m_seedMock.Object;
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;

            SuppressedAtomHashGenerator generator = new SuppressedAtomHashGenerator(new Mock<IAtomHashGenerator>().Object,
                    new Xorshift(), AtomSuppression.Unsuppressed, 2);

            int[][] graph = new int[][] { new[] { 1, 2, 3 }, new[] { 0 }, new[] { 0 }, new[] { 0 } };

            // simulate 3 identical neighbors
            long[] invs = new long[] { 21, 31, 31, 31 };
            long[] unique = new long[4];
            long[] rotated = new long[4];

            // non-suppressed
            {
                long value = generator.Next(graph, 0, invs, unique, rotated, Suppressed.None);

                Assert.IsTrue(Compares.AreDeepEqual(new long[] { 31, 0, 0, 0 }, unique));
                Assert.IsTrue(Compares.AreDeepEqual(new long[] { generator.Rotate(31, 2), 0, 0, 0 }, rotated));
                Assert.IsTrue(Compares.AreDeepEqual(
                    value,
                    generator.Distribute(21) ^ 31 ^ generator.Rotate(31) ^ generator.Rotate(31, 2)));
            }

            // okay now suppress vertices 1
            {
                BitArray suppressed = new BitArray(3);
                suppressed.Set(1, true);

                long value = generator.Next(graph, 0, invs, unique, rotated, Suppressed.FromBitSet(suppressed));

                Assert.IsTrue(Compares.AreDeepEqual(new long[] { 31, 0, 0, 0 }, unique));
                Assert.IsTrue(Compares.AreDeepEqual(new long[] { generator.Rotate(31, 1), 0, 0, 0 }, rotated)); // 31 only encountered twice
                Assert.IsTrue(Compares.AreDeepEqual(value, generator.Distribute(21) ^ 31 ^ generator.Rotate(31)));
            }

            // okay now suppress vertices 1 and 3
            {
                BitArray suppressed = new BitArray(4);
                suppressed.Set(1, true);
                suppressed.Set(3, true);

                long value = generator.Next(graph, 0, invs, unique, rotated, Suppressed.FromBitSet(suppressed));

                Assert.IsTrue(Compares.AreDeepEqual(new long[] { 31, 0, 0, 0 }, unique));
                Assert.IsTrue(Compares.AreDeepEqual(new long[] { 31, 0, 0, 0 }, rotated)); // 31 only encountered once and is not rotated
                Assert.IsTrue(Compares.AreDeepEqual(value, generator.Distribute(21) ^ 31)); // only encountered once
            }
        }
    }
}
