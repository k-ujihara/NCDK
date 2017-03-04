using NCDK.Common.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;

namespace NCDK.Graphs
{
    /// <summary>
    // @author maclean
    // @cdk.module test-standard
    /// </summary>
    [TestClass()]
    public class PermutorTest
    {

        private int Factorial(int n)
        {
            if (n <= 1)
            {
                return 1;
            }
            else
            {
                return n * Factorial(n - 1);
            }
        }

        private int[] GetIdentity(int size)
        {
            int[] identity = new int[size];
            for (int index = 0; index < size; index++)
            {
                identity[index] = index;
            }
            return identity;
        }

        private bool ArrayElementsDistinct(int[] array)
        {
            BitArray bitSet = new BitArray(array.Length);
            for (int index = 0; index < array.Length; index++)
            {
                if (bitSet[index])
                {
                    return false;
                }
                else
                {
                    bitSet.Set(index, true);
                }
            }
            return true;
        }

        [TestMethod()]
        public void ConstructorTest()
        {
            int size = 4;
            Permutor permutor = new Permutor(size);
            int[] current = permutor.GetCurrentPermutation();
            Assert.IsTrue(Compares.AreEqual(GetIdentity(size), current));
        }

        [TestMethod()]
        public void HasNextTest()
        {
            int size = 4;
            Permutor permutor = new Permutor(size);
            Assert.IsTrue(permutor.HasNext());
        }

        [TestMethod()]
        public void SetRankTest()
        {
            int size = 4;
            int[] reverse = new int[] { 3, 2, 1, 0 };
            Permutor permutor = new Permutor(size);
            // out of 4! = 24 permutations, numbered 0-23, this is the last
            permutor.Rank = 23;
            Assert.IsTrue(Compares.AreEqual(reverse, permutor.GetCurrentPermutation()));
        }

        [TestMethod()]
        public void GetRankTest()
        {
            int size = 4;
            int rank = 10;
            Permutor permutor = new Permutor(size);
            permutor.Rank = rank;
            Assert.AreEqual(rank, permutor.Rank);
        }

        [TestMethod()]
        public void SetPermutationTest()
        {
            int size = 4;
            int[] target = new int[] { 3, 1, 0, 2 };
            Permutor permutor = new Permutor(size);
            permutor.SetPermutation(target);
            Assert.IsTrue(Compares.AreEqual(target, permutor.GetCurrentPermutation()));
        }

        [TestMethod()]
        public void CountGeneratedPermutations()
        {
            int size = 4;
            Permutor permutor = new Permutor(size);
            int count = 1; // the identity permutation is not generated
            while (permutor.HasNext())
            {
                permutor.GetNextPermutation();
                count++;
            }
            Assert.AreEqual(Factorial(size), count);
        }

        [TestMethod()]
        public void GetCurrentPermutationTest()
        {
            int size = 4;
            Permutor permutor = new Permutor(size);
            bool allOk = true;
            while (permutor.HasNext())
            {
                permutor.GetNextPermutation();
                int[] current = permutor.GetCurrentPermutation();
                if (ArrayElementsDistinct(current))
                {
                    continue;
                }
                else
                {
                    allOk = false;
                    break;
                }
            }
            Assert.IsTrue(allOk);
        }

        [TestMethod()]
        public void MaxRankTest()
        {
            int size = 4;
            Permutor permutor = new Permutor(size);
            Assert.AreEqual(Factorial(size) - 1, permutor.CalculateMaxRank());
        }

        [TestMethod()]
        public void GetRandomNextTest()
        {
            int size = 4;
            Permutor permutor = new Permutor(size);
            int[] random = permutor.GetRandomNextPermutation();
            Assert.IsTrue(ArrayElementsDistinct(random));
        }
    }
}
