using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace NCDK.Beam
{
   /// <summary> <author>John May </author></summary>
    [TestClass()]
    public class ValenceTest
    {
        [TestMethod()]
        public void Hydrogen()
        {
            Normal("[H]");
            Normal("[H](C)");
            Abnormal("[H](C)(C)");

            Normal("[H+]");
            Normal("[H-]");
            Abnormal("[H++]");
            Abnormal("[H--]");

            Abnormal("[H+](C)");
            Normal("[H+](C)(C)");
            Abnormal("[H-](C)");
        }

        [TestMethod()]
        public void Helium()
        {
            Normal("[He]");
            Abnormal("[He+]");
            Abnormal("[He-]");
            Abnormal("[HeH]");
            Abnormal("[HeH2]");
            Abnormal("[HeH3]");
            Abnormal("[HeH4]");
        }

        [TestMethod()]
        public void Lithium()
        {
            Normal("[Li]");

            Normal("[Li+]");
            Abnormal("[Li-]");
            Abnormal("[Li++]");
            Abnormal("[Li--]");

            Abnormal("[Li+](C)(C)");
        }

        [TestMethod()]
        public void Boron()
        {
            Normal("[B]");
            Abnormal("[B+]");
            Abnormal("[B-]");

            Abnormal("[BH]");
            Abnormal("[BH2]");
            Normal("[BH3]");
            Abnormal("[BH4]");
            Abnormal("[BH5]");
        }

        [TestMethod()]
        public void Carbon_neutral()
        {
            Normal("[C]");
            Abnormal("[CH1]");
            Normal("[CH2]");
            Abnormal("[CH3]");
            Normal("[CH4]");
            Abnormal("[CH5]");
            Abnormal("[CH6]");
        }

        [TestMethod()]
        public void Carbon_anion()
        {
            Normal("[CH-]");
            Abnormal("[CH2-]");
            Normal("[CH3-]");
            Abnormal("[CH4-]");
            Abnormal("[CH5-]");
        }

        [TestMethod()]
        public void Carbon_cation()
        {
            Abnormal("[CH+]");
            Abnormal("[CH2+]");
            Normal("[CH3+]");
            Abnormal("[CH4+]");
            Normal("[CH5+]");
        }

        [TestMethod()]
        public void Carbon_dianion()
        {
            Abnormal("[CH--]");
            Abnormal("[CH2--]");
            Abnormal("[CH3--]");
            Abnormal("[CH4--]");
        }

        [TestMethod()]
        public void Carbon_dication()
        {
            Abnormal("[CH++]");
            Abnormal("[CH2++]");
            Abnormal("[CH3++]");
            Abnormal("[CH4++]");
        }

        [TestMethod()]
        public void Nitrogen_neutral()
        {
            Normal("[N]");
            Abnormal("[NH]");
            Abnormal("[NH2]");
            Normal("[NH3]");
            Abnormal("[NH4]");
            Normal("[NH5]");
            Abnormal("[NH6]");
            Abnormal("[NH7]");
        }

        [TestMethod()]
        public void Nitrogen_cation()
        {
            Abnormal("[N+]");
            Abnormal("[NH+]");
            Abnormal("[NH2+]");
            Abnormal("[NH3+]");
            Normal("[NH4+]");
            Normal("[NH5+]");
            Abnormal("[NH6+]");
            Abnormal("[NH7+]");
        }

        [TestMethod()]
        public void Nitrogen_anion()
        {
            Abnormal("[N-]");
            Abnormal("[NH-]");
            Normal("[NH2-]");
            Abnormal("[NH3-]");
            Abnormal("[NH4-]");
            Normal("[NH5-]");
            Abnormal("[NH6-]");
            Abnormal("[NH7-]");
        }

        [TestMethod()]
        public void Nitrogen_dication()
        {
            Abnormal("[N++]");
            Abnormal("[NH++]");
            Abnormal("[NH2++]");
            Abnormal("[NH3++]");
            Abnormal("[NH4++]");
            Abnormal("[NH5++]");
            Abnormal("[NH6+]");
            Abnormal("[NH7+]");
        }

        [TestMethod()]
        public void Nitrogen_dianion()
        {
            Abnormal("[N--]");
            Normal("[NH--]");
            Abnormal("[NH2--]");
            Abnormal("[NH3--]");
            Abnormal("[NH4--]");
            Normal("[NH5--]");
            Abnormal("[NH6--]");
            Abnormal("[NH7--]");
        }

        [TestMethod()]
        public void Nitrogen_trianion()
        {
            Normal("[N---]");
            Abnormal("[NH---]");
            Abnormal("[NH2---]");
            Abnormal("[NH3---]");
            Abnormal("[NH4---]");
            Normal("[NH5---]");
            Abnormal("[NH6---]");
            Abnormal("[NH7---]");
        }

        [TestMethod()]
        public void Oxygen_neutral()
        {
            Normal("[O]");
            Abnormal("[OH1]");
            Normal("[OH2]");
            Abnormal("[OH3]");
            Abnormal("[OH4]");
            Abnormal("[OH5]");
        }

        [TestMethod()]
        public void Oxygen_cation()
        {
            Abnormal("[O+]");
            Abnormal("[OH1+]");
            Abnormal("[OH2+]");
            Normal("[OH3+]");
            Abnormal("[OH4+]");
            Abnormal("[OH5+]");
        }

        [TestMethod()]
        public void Oxygen_anion()
        {
            Abnormal("[O-]");
            Normal("[OH1-]");
            Abnormal("[OH2-]");
            Abnormal("[OH3-]");
            Abnormal("[OH4-]");
            Abnormal("[OH5-]");
        }

        [TestMethod()]
        public void Oxygen_dication()
        {
            Abnormal("[O++]");
            Abnormal("[OH1++]");
            Abnormal("[OH2++]");
            Abnormal("[OH3++]");
            Abnormal("[OH4++]");
            Abnormal("[OH5++]");
        }

        [TestMethod()]
        public void Oxygen_dianion()
        {
            Normal("[O--]");
            Abnormal("[OH1--]");
            Abnormal("[OH2--]");
            Abnormal("[OH3--]");
            Abnormal("[OH4--]");
            Abnormal("[OH5--]");
        }

        [TestMethod()]
        public void Fluorine()
        {
            Normal("[F]");
            Abnormal("[F+]");
            Normal("[F-]");
            Abnormal("[F++]");
            Abnormal("[F--]");

            Normal("[FH1]");
            Abnormal("[FH2]");
            Abnormal("[FH3]");
            Abnormal("[FH4]");
            Abnormal("[FH5]");
            Abnormal("[FH6]");
            Abnormal("[FH7]");

            Normal("[FH1-]");
            Abnormal("[FH2-]");
            Abnormal("[FH3-]");
            Abnormal("[FH4-]");
            Abnormal("[FH5-]");
            Abnormal("[FH6-]");
            Abnormal("[FH7-]");
        }

        [TestMethod()]
        public void Neon()
        {
            Normal("[Ne]");
            Abnormal("[Ne+]");
            Abnormal("[Ne-]");
            Abnormal("[Ne++]");
            Abnormal("[Ne--]");

            Abnormal("[NeH]");
        }

        [TestMethod()]
        public void Sodium()
        {
            Normal("[Na]");
            Normal("[Na+]");
            Abnormal("[Na-]");
            Abnormal("[Na++]");
            Abnormal("[Na--]");

            Normal("[Na](C)");
            Abnormal("[Na+](C)");
            Abnormal("[Na+](C)(C)");
        }

        [TestMethod()]
        public void Magnesium()
        {
            Normal("[Mg]");
            Normal("[Mg+](C)");
            Normal("[Mg++]");

            Abnormal("[Mg](C)");
            Normal("[Mg](C)(C)");

            Normal("[Mg+](C)");
            Abnormal("[Mg+](C)(C)");
            Abnormal("[Mg+](C)(C)(C)");

            Abnormal("[Mg++](C)");
            Abnormal("[Mg++](C)(C)");
            Abnormal("[Mg++](C)(C)(C)");
        }

        [TestMethod()]
        public void Phosphorus_neutral()
        {
            Normal("[P]");
            Abnormal("[PH1]");
            Abnormal("[PH2]");
            Normal("[PH3]");
            Abnormal("[PH4]");
            Normal("[PH5]");
            Abnormal("[PH6]");
            Abnormal("[PH7]");
            Abnormal("[PH8]");
        }

        [TestMethod()]
        public void Phosphorus_cation()
        {
            Abnormal("[P+]");
            Abnormal("[PH1+]");
            Abnormal("[PH2+]");
            Abnormal("[PH3+]");
            Normal("[PH4+]");
            Abnormal("[PH5+]");
            Normal("[PH6+]");
            Abnormal("[PH7+]");
            Abnormal("[PH8+]");
        }

        [TestMethod()]
        public void Phosphorus_anion()
        {
            Abnormal("[P-]");
            Abnormal("[PH1-]");
            Normal("[PH2-]");
            Abnormal("[PH3-]");
            Normal("[PH4-]");
            Abnormal("[PH5-]");
            Abnormal("[PH6-]");
            Abnormal("[PH7-]");
            Abnormal("[PH8-]");
        }

        [TestMethod()]
        public void Phosphorus_dication()
        {
            Abnormal("[P++]");
            Abnormal("[PH1++]");
            Abnormal("[PH2++]");
            Abnormal("[PH3++]");
            Abnormal("[PH4++]");
            Abnormal("[PH5++]");
            Abnormal("[PH6++]");
            Abnormal("[PH7++]");
            Abnormal("[PH8++]");
        }

        [TestMethod()]
        public void Phosphorus_dianion()
        {
            Abnormal("[P--]");
            Normal("[PH1--]");
            Abnormal("[PH2--]");
            Normal("[PH3--]");
            Abnormal("[PH4--]");
            Abnormal("[PH5--]");
            Abnormal("[PH6--]");
            Abnormal("[PH7--]");
            Abnormal("[PH8--]");
        }

        [TestMethod()]
        public void Phosphorus_trianion()
        {
            Normal("[P---]");
            Abnormal("[PH1---]");
            Normal("[PH2---]");
            Abnormal("[PH3---]");
            Abnormal("[PH4---]");
            Abnormal("[PH5---]");
            Abnormal("[PH6---]");
            Abnormal("[PH7---]");
            Abnormal("[PH8---]");
        }

        [TestMethod()]
        public void Sulphur()
        {
            Normal("[S]");
            Abnormal("[SH1]");
            Normal("[SH2]");
            Abnormal("[SH3]");
            Normal("[SH4]");
            Abnormal("[SH5]");
            Normal("[SH6]");
            Abnormal("[SH7]");
            Abnormal("[SH8]");
        }

        [TestMethod()]
        public void Sulphur_cation()
        {
            Abnormal("[S+]");
            Abnormal("[SH1+]");
            Abnormal("[SH2+]");
            Normal("[SH3+]");
            Abnormal("[SH4+]");
            Normal("[SH5+]");
            Abnormal("[SH6+]");
            Normal("[SH7+]");
            Abnormal("[SH8+]");
        }

        [TestMethod()]
        public void Sulphur_anion()
        {
            Abnormal("[S-]");
            Normal("[SH1-]");
            Abnormal("[SH2-]");
            Normal("[SH3-]");
            Abnormal("[SH4-]");
            Normal("[SH5-]");
            Abnormal("[SH6-]");
            Abnormal("[SH7-]");
            Abnormal("[SH8-]");
        }

        [TestMethod()]
        public void Sulphur_dication()
        {
            Abnormal("[S++]");
            Abnormal("[SH1++]");
            Abnormal("[SH2++]");
            Abnormal("[SH3++]");
            Normal("[SH4++]");
            Abnormal("[SH5++]");
            Normal("[SH6++]");
            Abnormal("[SH7++]");
            Normal("[SH8++]");
            Abnormal("[SH9++]");
        }

        [TestMethod()]
        public void Sulphur_trication()
        {
            Abnormal("[S+++]");
            Abnormal("[SH1+++]");
            Abnormal("[SH2+++]");
            Abnormal("[SH3+++]");
            Abnormal("[SH4+++]");
            Normal("[SH5+++]");
            Abnormal("[SH6+++]");
            Normal("[SH7+++]");
            Abnormal("[SH8+++]");
            Normal("[SH9+++]");
            Abnormal("[SH10+++]");
        }

        [TestMethod()]
        public void Chlorine()
        {
            Normal("[Cl]");
            Abnormal("[Cl+]");
            Normal("[Cl-]");

            Abnormal("[ClH1+]");
            Normal("[ClH2+]");
            Abnormal("[ClH3+]");
            Normal("[ClH4+]");
            Abnormal("[ClH5+]");
            Normal("[ClH6+]");
            Abnormal("[ClH7+]");
            Normal("[ClH8+]");

            Abnormal("[ClH1-]");
            Normal("[ClH2-]");
            Abnormal("[ClH3-]");
            Normal("[ClH4-]");
            Abnormal("[ClH5-]");
            Normal("[ClH6-]");

            Abnormal("[ClH1++]");
            Abnormal("[ClH2++]");
            Normal("[ClH3++]");
            Abnormal("[ClH4++]");
            Normal("[ClH5++]");
            Abnormal("[ClH6++]");
            Normal("[ClH7++]");
            Abnormal("[ClH8++]");
            Normal("[ClH9++]");
            Abnormal("[ClH10++]");

            Normal("[ClH1--]");
            Abnormal("[ClH2--]");
            Normal("[ClH3--]");
            Abnormal("[ClH4--]");
            Normal("[ClH5--]");
            Abnormal("[ClH6--]");
            Abnormal("[ClH7--]");
        }

        [TestMethod()]
        public void Argon()
        {
            Normal("[Ar]");
            Abnormal("[Ar+]");
            Abnormal("[Ar-]");
            Abnormal("[ArH]");
            Abnormal("[ArH]");
        }

        [TestMethod()]
        public void Potassium()
        {
            Normal("[K]");
            Normal("[K+]");
            Abnormal("[K-]");
            Normal("[K](C)");
            Abnormal("[K+](C)");
        }

        [TestMethod()]
        public void Calcium()
        {
            Normal("[Ca]");
            Normal("[Ca](C)(C)");
            Abnormal("[Ca+]");
            Normal("[Ca+](C)");
            Abnormal("[Ca+](C)(C)");
            Normal("[Ca++]");
            Abnormal("[Ca++](C)");
            Abnormal("[Ca++](C)(C)");

        }

        [TestMethod()]
        public void Arsenic_neutral()
        {
            Normal("[As]");
            Abnormal("[AsH1]");
            Abnormal("[AsH2]");
            Normal("[AsH3]");
            Abnormal("[AsH4]");
            Normal("[AsH5]");
            Abnormal("[AsH6]");
        }

        [TestMethod()]
        public void Arsenic_cation()
        {
            Abnormal("[As+]");
            Abnormal("[AsH1+]");
            Abnormal("[AsH2+]");
            Abnormal("[AsH3+]");
            Normal("[AsH4+]");
            Abnormal("[AsH5+]");
            Normal("[AsH6+]");
            Abnormal("[AsH7+]");
        }

        [TestMethod()]
        public void Arsenic_dication()
        {
            Abnormal("[As++]");
            Abnormal("[AsH1++]");
            Abnormal("[AsH2++]");
            Abnormal("[AsH3++]");
            Abnormal("[AsH4++]");
            Abnormal("[AsH5++]");
            Abnormal("[AsH6++]");
            Abnormal("[AsH7++]");
        }

        [TestMethod()]
        public void Arsenic_anion()
        {
            Abnormal("[As-]");
            Abnormal("[AsH1-]");
            Normal("[AsH2-]");
            Abnormal("[AsH3-]");
            Normal("[AsH4-]");
            Abnormal("[AsH5-]");
            Abnormal("[AsH6-]");
            Abnormal("[AsH7-]");
        }

        [TestMethod()]
        public void Arsenic_dianion()
        {
            Abnormal("[As--]");
            Normal("[AsH1--]");
            Abnormal("[AsH2--]");
            Normal("[AsH3--]");
            Abnormal("[AsH4--]");
            Abnormal("[AsH5--]");
            Abnormal("[AsH6--]");
            Abnormal("[AsH7--]");
        }

        [TestMethod()]
        public void Arsenic_trianion()
        {
            Normal("[As---]");
            Abnormal("[AsH1---]");
            Normal("[AsH2---]");
            Abnormal("[AsH3---]");
            Abnormal("[AsH4---]");
            Abnormal("[AsH5---]");
            Abnormal("[AsH6---]");
            Abnormal("[AsH7---]");
        }

        [TestMethod()]
        public void Selenium_neutral()
        {
            Normal("[Se]");
            Abnormal("[SeH1]");
            Normal("[SeH2]");
            Abnormal("[SeH3]");
            Normal("[SeH4]");
            Abnormal("[SeH5]");
            Normal("[SeH6]");
            Abnormal("[SeH7]");
            Abnormal("[SeH8]");
        }

        [TestMethod()]
        public void Selenium_cation()
        {
            Abnormal("[Se+]");
            Abnormal("[SeH1+]");
            Abnormal("[SeH2+]");
            Normal("[SeH3+]");
            Abnormal("[SeH4+]");
            Normal("[SeH5+]");
            Abnormal("[SeH6+]");
            Normal("[SeH7+]");
            Abnormal("[SeH8+]");
        }

        [TestMethod()]
        public void Selenium_anion()
        {
            Abnormal("[Se-]");
            Normal("[SeH1-]");
            Abnormal("[SeH2-]");
            Normal("[SeH3-]");
            Abnormal("[SeH4-]");
            Normal("[SeH5-]");
            Abnormal("[SeH6-]");
            Abnormal("[SeH7-]");
            Abnormal("[SeH8-]");
        }

        // selenium charge is unrestricted
        [TestMethod()]
        public void Selenium_n_cation()
        {
            Abnormal("[Se+6]");
            Abnormal("[SeH1+6]");
            Abnormal("[SeH2+6]");
            Abnormal("[SeH3+6]");
            Abnormal("[SeH4+6]");
            Abnormal("[SeH5+6]");
            Abnormal("[SeH6+6]");
            Abnormal("[SeH7+6]");
            Normal("[SeH8+6]");
            Abnormal("[SeH9+6]");
            Normal("[SeH10+6]");
            Abnormal("[SeH11+6]");
            Normal("[SeH12+6]");
        }

        [TestMethod()]
        public void Bromine()
        {
            Normal("[Br]");
            Abnormal("[Br+]");
            Normal("[Br-]");

            Abnormal("[BrH1+]");
            Normal("[BrH2+]");
            Abnormal("[BrH3+]");
            Normal("[BrH4+]");
            Abnormal("[BrH5+]");
            Normal("[BrH6+]");
            Abnormal("[BrH7+]");
            Normal("[BrH8+]");

            Abnormal("[BrH1-]");
            Normal("[BrH2-]");
            Abnormal("[BrH3-]");
            Normal("[BrH4-]");
            Abnormal("[BrH5-]");
            Normal("[BrH6-]");

            Abnormal("[BrH1++]");
            Abnormal("[BrH2++]");
            Normal("[BrH3++]");
            Abnormal("[BrH4++]");
            Normal("[BrH5++]");
            Abnormal("[BrH6++]");
            Normal("[BrH7++]");
            Abnormal("[BrH8++]");
            Normal("[BrH9++]");
            Abnormal("[BrH10++]");

            Normal("[BrH1--]");
            Abnormal("[BrH2--]");
            Normal("[BrH3--]");
            Abnormal("[BrH4--]");
            Normal("[BrH5--]");
            Abnormal("[BrH6--]");
            Abnormal("[BrH7--]");
        }

        [TestMethod()]
        public void Krypton()
        {
            Normal("[Kr]");
            Abnormal("[Kr+]");
            Abnormal("[Kr-]");
            Abnormal("[KrH]");
        }

        [TestMethod()]
        public void Strontium()
        {
            Normal("[Sr]");
            Abnormal("[Sr+]");
            Normal("[Sr++]");
            Normal("[Sr+](C)");
            Normal("[Sr](C)(C)");
        }

        [TestMethod()]
        public void Tellurium_neutral()
        {
            Normal("[Te]");
            Abnormal("[TeH1]");
            Normal("[TeH2]");
            Abnormal("[TeH3]");
            Normal("[TeH4]");
            Abnormal("[TeH5]");
            Normal("[TeH6]");
            Abnormal("[TeH7]");
            Abnormal("[TeH8]");
        }

        [TestMethod()]
        public void Tellurium_cation()
        {
            Abnormal("[Te+]");
            Abnormal("[TeH1+]");
            Abnormal("[TeH2+]");
            Normal("[TeH3+]");
            Abnormal("[TeH4+]");
            Normal("[TeH5+]");
            Abnormal("[TeH6+]");
            Normal("[TeH7+]");
            Abnormal("[TeH8+]");
        }

        [TestMethod()]
        public void Tellurium_anion()
        {
            Abnormal("[Te-]");
            Normal("[TeH1-]");
            Abnormal("[TeH2-]");
            Normal("[TeH3-]");
            Abnormal("[TeH4-]");
            Normal("[TeH5-]");
            Abnormal("[TeH6-]");
            Abnormal("[TeH7-]");
            Abnormal("[TeH8-]");
        }

        // tellurium charge is unrestricted
        [TestMethod()]
        public void Tellurium_n_cation()
        {
            Abnormal("[Te+6]");
            Abnormal("[TeH1+6]");
            Abnormal("[TeH2+6]");
            Abnormal("[TeH3+6]");
            Abnormal("[TeH4+6]");
            Abnormal("[TeH5+6]");
            Abnormal("[TeH6+6]");
            Abnormal("[TeH7+6]");
            Normal("[TeH8+6]");
            Abnormal("[TeH9+6]");
            Normal("[TeH10+6]");
            Abnormal("[TeH11+6]");
            Normal("[TeH12+6]");
        }

        [TestMethod()]
        public void Iodine()
        {
            Normal("[I]");
            Abnormal("[I+]");
            Normal("[I-]");

            Abnormal("[IH1+]");
            Normal("[IH2+]");
            Abnormal("[IH3+]");
            Normal("[IH4+]");
            Abnormal("[IH5+]");
            Normal("[IH6+]");
            Abnormal("[IH7+]");
            Normal("[IH8+]");

            Abnormal("[IH1-]");
            Normal("[IH2-]");
            Abnormal("[IH3-]");
            Normal("[IH4-]");
            Abnormal("[IH5-]");
            Normal("[IH6-]");

            Abnormal("[IH1++]");
            Abnormal("[IH2++]");
            Normal("[IH3++]");
            Abnormal("[IH4++]");
            Normal("[IH5++]");
            Abnormal("[IH6++]");
            Normal("[IH7++]");
            Abnormal("[IH8++]");
            Normal("[IH9++]");
            Abnormal("[IH10++]");

            Normal("[IH1--]");
            Abnormal("[IH2--]");
            Normal("[IH3--]");
            Abnormal("[IH4--]");
            Normal("[IH5--]");
            Abnormal("[IH6--]");
            Abnormal("[IH7--]");
        }

        [TestMethod()]
        public void astatine()
        {
            Normal("[At]");
            Abnormal("[At+]");
            Normal("[At-]");

            Abnormal("[AtH1+]");
            Normal("[AtH2+]");
            Abnormal("[AtH3+]");
            Normal("[AtH4+]");
            Abnormal("[AtH5+]");
            Normal("[AtH6+]");
            Abnormal("[AtH7+]");
            Normal("[AtH8+]");

            Abnormal("[AtH1-]");
            Normal("[AtH2-]");
            Abnormal("[AtH3-]");
            Normal("[AtH4-]");
            Abnormal("[AtH5-]");
            Normal("[AtH6-]");

            Abnormal("[AtH1++]");
            Abnormal("[AtH2++]");
            Normal("[AtH3++]");
            Abnormal("[AtH4++]");
            Normal("[AtH5++]");
            Abnormal("[AtH6++]");
            Normal("[AtH7++]");
            Abnormal("[AtH8++]");
            Normal("[AtH9++]");
            Abnormal("[AtH10++]");

            Normal("[AtH1--]");
            Abnormal("[AtH2--]");
            Normal("[AtH3--]");
            Abnormal("[AtH4--]");
            Normal("[AtH5--]");
            Abnormal("[AtH6--]");
            Abnormal("[AtH7--]");
        }

        [TestMethod()]
        public void Barium()
        {
            Normal("[Ba]");
            Abnormal("[Ba+]");
            Normal("[Ba++]");
            Normal("[Ba+](C)");
            Normal("[Ba](C)(C)");
        }

        [TestMethod()]
        public void Radium()
        {
            Normal("[Ra]");
            Abnormal("[Ra+]");
            Normal("[Ra++]");
            Normal("[Ra+](C)");
            Normal("[Ra](C)(C)");
            Abnormal("[Ra++](C)(C)");
        }

        static void Normal(string str)
        {
            Normal(str, 0);
        }

        static void Abnormal(string str)
        {
            Abnormal(str, 0);
        }

        static void Normal(string str, int v)
        {
            try
            {
                Graph g = Graph.FromSmiles(str);
                int sum = 0;
                foreach (var e in g.GetEdges(v))
                {
                    sum += e.Bond.Order;
                }
                Assert.IsTrue(
                           g.GetAtom(v).Element.Verify(sum + g.ImplHCount(v),
                                                      g.GetAtom(v).Charge),
                           str + " should be normal but was abnormal");
            }
            catch (IOException e)
            {
                Assert.Fail("parse error:" + e);
            }
        }

        static void Abnormal(string str, int v)
        {
            try
            {
                Graph g = Graph.FromSmiles(str);
                int sum = 0;
                foreach (var e in g.GetEdges(v))
                {
                    sum += e.Bond.Order;
                }
                Assert.IsFalse(
                    g.GetAtom(v).Element.Verify(sum + g.ImplHCount(v), g.GetAtom(v).Charge),
                    str + " should be abnormal but was normal");
            }
            catch (IOException e)
            {
                Assert.Fail("parse error:" + e);
            }
        }
    }
}
