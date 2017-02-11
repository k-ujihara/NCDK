using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Smiles;

namespace NCDK.Isomorphisms.Matchers.SMARTS
{
    /**
     * Sets the computation of SMARTSAtomInvariants using the Daylight ring
     * values.
     *
     * @author John May
     * @cdk.module test-smarts
     */
     [TestClass()]
    public class DaylightSMARTSAtomInvariantsTest
    {

        [TestMethod()]
        public void targetTest()
        {
            IAtomContainer container = sp.ParseSmiles("CCC");
            SMARTSAtomInvariants.ConfigureDaylightWithRingInfo(container);
            foreach (var atom in container.Atoms) {
                Assert.AreEqual(container, (atom.GetProperty<SMARTSAtomInvariants>(SMARTSAtomInvariants.Key)).Target);
            }
        }

        [TestMethod()]
        public void ValenceTest() {
            Assert.AreEqual(4, InvariantOfFirstAtom("C").Valence);
            Assert.AreEqual(3, InvariantOfFirstAtom("N").Valence);
            Assert.AreEqual(2, InvariantOfFirstAtom("O").Valence);
            Assert.AreEqual(3, InvariantOfFirstAtom("P").Valence);
            Assert.AreEqual(2, InvariantOfFirstAtom("S").Valence);
            Assert.AreEqual(0, InvariantOfFirstAtom("[H]").Valence);
            Assert.AreEqual(5, InvariantOfFirstAtom("[NH5]").Valence);
            Assert.AreEqual(5, InvariantOfFirstAtom("N(=O)=C").Valence);
        }

        [TestMethod()]
        public void ConnectivityTest()
        {
            Assert.AreEqual(4, InvariantOfFirstAtom("C").Connectivity);
            Assert.AreEqual(3, InvariantOfFirstAtom("N").Connectivity);
            Assert.AreEqual(2, InvariantOfFirstAtom("O").Connectivity);
            Assert.AreEqual(3, InvariantOfFirstAtom("P").Connectivity);
            Assert.AreEqual(2, InvariantOfFirstAtom("S").Connectivity);
            Assert.AreEqual(0, InvariantOfFirstAtom("[H]").Connectivity);
            Assert.AreEqual(1, InvariantOfFirstAtom("[H][H]").Connectivity);
            Assert.AreEqual(5, InvariantOfFirstAtom("[NH5]").Connectivity);
            Assert.AreEqual(3, InvariantOfFirstAtom("N(=O)=C").Connectivity);
        }

        [TestMethod()]
        public void DegreeTest()
        {
            Assert.AreEqual(0, InvariantOfFirstAtom("C").Degree);
            Assert.AreEqual(0, InvariantOfFirstAtom("N").Degree);
            Assert.AreEqual(0, InvariantOfFirstAtom("O").Degree);
            Assert.AreEqual(0, InvariantOfFirstAtom("P").Degree);
            Assert.AreEqual(0, InvariantOfFirstAtom("S").Degree);
            Assert.AreEqual(0, InvariantOfFirstAtom("[H]").Degree);
            Assert.AreEqual(1, InvariantOfFirstAtom("[H][H]").Degree);
            Assert.AreEqual(0, InvariantOfFirstAtom("[NH5]").Degree);
            Assert.AreEqual(2, InvariantOfFirstAtom("N(=O)=C").Degree);
        }

        [TestMethod()]
        public void TotalHydrogenCountTest()
        {
            Assert.AreEqual(4, InvariantOfFirstAtom("C").TotalHydrogenCount);
            Assert.AreEqual(4, InvariantOfFirstAtom("[CH4]").TotalHydrogenCount);
            Assert.AreEqual(4, InvariantOfFirstAtom("C[H]").TotalHydrogenCount);
            Assert.AreEqual(3, InvariantOfFirstAtom("[CH2][H]").TotalHydrogenCount);
            Assert.AreEqual(4, InvariantOfFirstAtom("[CH2]([H])[H]").TotalHydrogenCount);
        }

        [TestMethod()]
        public void RingConnectivityTest()
        {
            Assert.AreEqual(0, InvariantOfFirstAtom("C").RingConnectivity);

            // 2,3,4 ring bonds
            Assert.AreEqual(2, InvariantOfFirstAtom("C1CCC1").RingConnectivity);
            Assert.AreEqual(3, InvariantOfFirstAtom("C12CCC1CC2").RingConnectivity);
            Assert.AreEqual(4, InvariantOfFirstAtom("C12(CCC2)CCC1").RingConnectivity);

            // note 2 ring bonds but 3 ring atoms
            Assert.AreEqual(2, InvariantOfFirstAtom("C1(CCC1)C1CCC1").RingConnectivity);
        }

        [TestMethod()]
        public void RingNumberTest()
        {
            Assert.AreEqual(0, InvariantOfFirstAtom("C").RingNumber);

            Assert.AreEqual(1, InvariantOfFirstAtom("C1CCC1").RingNumber);
            Assert.AreEqual(2, InvariantOfFirstAtom("C12CCC1CC2").RingNumber);
            Assert.AreEqual(2, InvariantOfFirstAtom("C12(CCC2)CCC1").RingNumber);
        }

        /**
         * Demonstates a problems with the SSSR but we match what Daylight depict
         * match says. We always have 4 atoms atoms in 3 rings but the other atoms
         * are either in 1 ring or 2 rings. Which atoms depends on the order of
         * atoms in the input.
         */
        [TestMethod()]
        public void RingNumber_cyclophane() {
            IAtomContainer container = sp.ParseSmiles("C1CC23CCC11CCC4(CC1)CCC(CC2)(CC3)CC4");
            SMARTSAtomInvariants.ConfigureDaylightWithRingInfo(container);
            int R1 = 0, R2 = 0, R3 = 0;
            foreach (var atom in container.Atoms) {
                SMARTSAtomInvariants inv = atom.GetProperty<SMARTSAtomInvariants>(SMARTSAtomInvariants.Key);
                switch (inv.RingNumber) {
                    case 1:
                        R1++;
                        break;
                    case 2:
                        R2++;
                        break;
                    case 3:
                        R3++;
                        break;
                }
            }
            Assert.AreEqual(8, R1);
            Assert.AreEqual(8, R2);
            Assert.AreEqual(4, R3);
        }

        [TestMethod()]
        public void RingSizeTest()
        {
            Assert.IsTrue(InvariantOfFirstAtom("C").RingSize.Count == 0);
            Assert.IsTrue(InvariantOfFirstAtom("C1CC1").RingSize.Contains(3));
            Assert.IsTrue(InvariantOfFirstAtom("C1CCC1").RingSize.Contains(4));
            Assert.IsTrue(InvariantOfFirstAtom("C1CCCC1").RingSize.Contains(5));
        }

        /**
         * Shows that the store ring sizes are only the smallest. There is one ring
         * of size six and one ring of size 5. When we count the ring sizes (can be
         * verities on depict match) there are only 4 atoms in a 6 member ring. This
         * is because 2 atoms are shared with the smalled 5 member ring.
         *
         * @
         */
        [TestMethod()]
        public void RingSize_imidazole() {

            IAtomContainer container = sp.ParseSmiles("N1C=NC2=CC=CC=C12");
            SMARTSAtomInvariants.ConfigureDaylightWithRingInfo(container);
            int ringSize5 = 0, ringSize6 = 0;
            foreach (var atom in container.Atoms) {
                SMARTSAtomInvariants inv = atom.GetProperty<SMARTSAtomInvariants>(SMARTSAtomInvariants.Key);
                if (inv.RingSize.Contains(5)) ringSize5++;
                if (inv.RingSize.Contains(6)) ringSize6++;
            }

            Assert.AreEqual(5, ringSize5);
            Assert.AreEqual(4, ringSize6);
        }

        /**
         * Shows that the exterior ring of the SSSR (size 12) is not
         * @
         */
        [TestMethod()]
        public void RingSize_cyclophane()
        {

            IAtomContainer container = sp.ParseSmiles("C1CC23CCC11CCC4(CC1)CCC(CC2)(CC3)CC4");
            SMARTSAtomInvariants.ConfigureDaylightWithRingInfo(container);
            foreach (var atom in container.Atoms)
            {
                SMARTSAtomInvariants inv = atom.GetProperty<SMARTSAtomInvariants>(SMARTSAtomInvariants.Key);
                Assert.IsTrue(inv.RingSize.Contains(6));
                Assert.IsFalse(inv.RingSize.Contains(12));
            }
        }

        [TestMethod()]
        public void noRingInfo() {
            IAtomContainer container = sp.ParseSmiles("C1CC23CCC11CCC4(CC1)CCC(CC2)(CC3)CC4");
            SMARTSAtomInvariants.ConfigureDaylightWithoutRingInfo(container);
            foreach (var atom in container.Atoms) {
                SMARTSAtomInvariants inv = atom.GetProperty<SMARTSAtomInvariants>(SMARTSAtomInvariants.Key);
                Assert.IsTrue(inv.RingSize.Count == 0);
                Assert.AreEqual(0, inv.RingNumber);
            }
        }

        static readonly SmilesParser sp = new SmilesParser(Silent.ChemObjectBuilder.Instance);

        // compute the invariants for the first atom in a SMILES string
        static SMARTSAtomInvariants InvariantOfFirstAtom(string smiles) {
            IAtomContainer container = sp.ParseSmiles(smiles);
            SMARTSAtomInvariants.ConfigureDaylightWithRingInfo(container);
            return container.Atoms[0].GetProperty<SMARTSAtomInvariants>(SMARTSAtomInvariants.Key);
        }
    }
}
