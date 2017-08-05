using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Common.Collections;
using System;
using System.Collections.Generic;

namespace NCDK.FaulonSignatures.Simple
{
    [TestClass()]
    public class CanonicalLabellingTest
    {
        public int[] GetLabels(SimpleGraph graph)
        {
            SimpleGraphSignature signature = new SimpleGraphSignature(graph);
            return signature.GetCanonicalLabels();
        }

        public void PermuteTest(SimpleGraph graph)
        {
            Console.Out.WriteLine(Arrays.ToJavaString(GetLabels(graph)) + " " + graph);
            SimpleGraphPermutor permutor = new SimpleGraphPermutor(graph);
            var relabelledStrings = new HashSet<string>();

            // permute, and relabel
            foreach (var permutation in permutor)
            {
                int[] labels = GetLabels(permutation);
                SimpleGraph relabelled = new SimpleGraph(permutation, labels);
                bool isIdentity = IsIdentity(labels);
                Console.Out.WriteLine(
                    Arrays.ToJavaString(labels) + " " + permutation + " " + relabelled
                    + " " + isIdentity);
                relabelledStrings.Add(relabelled.ToString());
            }
            Assert.AreEqual(1, relabelledStrings.Count);

            // list the number of unique strings
            var values = relabelledStrings.GetEnumerator();
            for (int i = 0; i < relabelledStrings.Count; i++)
            {
                values.MoveNext();
                Console.Out.WriteLine(i + " " + values.Current);
            }
        }

        public bool IsIdentity(int[] permutation)
        {
            if (permutation.Length < 1) return true;
            int prev = permutation[0];
            for (int i = 1; i < permutation.Length; i++)
            {
                if (permutation[i] < prev) return false;
                prev = permutation[i];
            }
            return true;
        }

        [TestMethod()]
        public void TestSimpleGraphLabelling()
        {
            string graphString = "0:1,1:2";
            SimpleGraph graph = new SimpleGraph(graphString);
            SimpleGraphSignature signature = new SimpleGraphSignature(graph);
            int[] labels = signature.GetCanonicalLabels();
            Assert.IsTrue(IsIdentity(labels));
        }

        [TestMethod()]
        public void FourCycleTest()
        {
            PermuteTest(new SimpleGraph("0:1,0:3,1:2,2:3"));
        }

        [TestMethod()]
        public void FiveCycleTest()
        {
            PermuteTest(new SimpleGraph("0:1,0:4,1:2,2:3,3:4"));
        }

        [TestMethod()]
        public void ThreeFourFusedCycle()
        {
            PermuteTest(new SimpleGraph("0:1,0:2,1:2,1:3,2:4,3:4"));
        }

        [TestMethod()]
        public void ThreeThreeFusedCycle()
        {
            PermuteTest(new SimpleGraph("0:1,0:2,0:3,1:4,2:4,3:4"));
        }

        [TestCategory("SlowTest")]
        [TestMethod()]
        public void LargePermuteTestA()
        {
            PermuteTest(new SimpleGraph("5:7,6:7,0:6,1:6,2:5,3:5,0:4,1:4,2:4,3:4,0:3,2:3,0:1,1:2"));
        }

        [TestCategory("SlowTest")]
        [TestMethod()]
        public void LargePermuteTestB()
        {
            PermuteTest(new SimpleGraph("5:7,6:7,0:6,2:6,1:5,3:5,0:4,1:4,2:4,3:4,0:3,1:3,0:2,1:2"));
        }

        [TestMethod()]
        public void TmpOrbitsTest()
        {
            SimpleGraph a = new SimpleGraph("5:7,6:7,0:6,1:6,2:5,3:5,0:4,1:4,2:4,3:4,0:3,2:3,0:1,1:2");
            //        SimpleGraph b = new SimpleGraph("5:7,6:7,0:6,2:6,1:5,3:5,0:4,1:4,2:4,3:4,0:3,1:3,0:2,1:2");
            SimpleGraphSignature sigA = new SimpleGraphSignature(a);
            foreach (var cls in sigA.GetSymmetryClasses())
            {
                Console.Out.WriteLine(cls);
            }
        }

        //[TestMethod()]
        public void IsomorphicPair()
        {
            SimpleGraph a = new SimpleGraph("5:7,6:7,0:6,1:6,2:5,3:5,0:4,1:4,2:4,3:4,0:3,2:3,0:1,1:2");
            SimpleGraph b = new SimpleGraph("5:7,6:7,0:6,2:6,1:5,3:5,0:4,1:4,2:4,3:4,0:3,1:3,0:2,1:2");

            SimpleGraphSignature sigA = new SimpleGraphSignature(a);
            SimpleGraphSignature sigB = new SimpleGraphSignature(b);
            Assert.AreEqual(sigA.ToCanonicalString(), sigB.ToCanonicalString());
        }
    }
}
