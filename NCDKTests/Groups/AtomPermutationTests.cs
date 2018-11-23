using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Graphs;

namespace NCDK.Groups
{
    // @author maclean
    // @cdk.module test-group
    [TestClass()]
    public class AtomPermutationTests : CDKTestCase
    {
        public static IChemObjectBuilder builder = CDK.Builder;

        /// <summary>
        /// This test is checking all permutations of an atom container to see
        /// if the refiner gives the canonical labelling map (effectively).
        /// </summary>
        /// <param name="atomContainer"></param>
        public void CheckForCanonicalForm(IAtomContainer atomContainer)
        {
            AtomContainerAtomPermutor permutor = new AtomContainerAtomPermutor(atomContainer);
            AtomDiscretePartitionRefiner refiner = new AtomDiscretePartitionRefiner();
            refiner.Refine(atomContainer);
            Permutation best = refiner.GetBest().Invert();
            string cert = AtomContainerPrinter.ToString(atomContainer, best, true);
            while (permutor.MoveNext())
            {
                IAtomContainer permutedContainer = permutor.Current;
                refiner.Refine(permutedContainer);
                best = refiner.GetBest().Invert();
                string permCert = AtomContainerPrinter.ToString(permutedContainer, best, true);
                Assert.AreEqual(cert, permCert);
            }
        }

        [TestMethod()]
        public void TestDisconnectedAtomCarbonCompound()
        {
            string acpString = "C0C1C2 0:2(1)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            CheckForCanonicalForm(ac);
        }

        [TestMethod()]
        public void TestDisconnectedBondsCarbonCompound()
        {
            string acpString = "C0C1C2C3 0:2(1),1:3(2)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            CheckForCanonicalForm(ac);
        }

        [TestMethod()]
        public void TestSimpleCarbonCompound()
        {
            string acpString = "C0C1C2C3 0:1(1),1:2(1),2:3(1)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            CheckForCanonicalForm(ac);
        }

        [TestMethod()]
        public void TestCyclicCarbonCompound()
        {
            string acpString = "C0C1C2C3 0:1(1),0:3(1),1:2(1),2:3(1)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            CheckForCanonicalForm(ac);
        }

        [TestMethod()]
        public void TestDoubleBondCyclicCarbonCompound()
        {
            string acpString = "C0C1C2C3 0:1(1),0:3(2),1:2(2),2:3(1)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            CheckForCanonicalForm(ac);
        }

        [TestMethod()]
        public void TestSimpleCarbonOxygenCompound()
        {
            string acpString = "O0C1C2 0:1(2),1:2(1)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            CheckForCanonicalForm(ac);
        }

        [TestMethod()]
        public void TestCyclicCarbonOxygenCompound()
        {
            string acpString = "O0C1O2C3 0:1(1),0:3(1),1:2(1),2:3(1)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            CheckForCanonicalForm(ac);
        }
    }
}
