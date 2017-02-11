using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace NCDK.Beam
{
    /// <summary> <author>John May </author>*/
	[TestClass()]
    public class ValenceTest
    {
        [TestMethod()]
        public void Hydrogen()
        {
            normal("[H]");
            normal("[H](C)");
            abnormal("[H](C)(C)");

            normal("[H+]");
            normal("[H-]");
            abnormal("[H++]");
            abnormal("[H--]");

            abnormal("[H+](C)");
            normal("[H+](C)(C)");
            abnormal("[H-](C)");
        }

        [TestMethod()]
        public void helium()
        {
            normal("[He]");
            abnormal("[He+]");
            abnormal("[He-]");
            abnormal("[HeH]");
            abnormal("[HeH2]");
            abnormal("[HeH3]");
            abnormal("[HeH4]");
        }

        [TestMethod()]
        public void lithium()
        {
            normal("[Li]");

            normal("[Li+]");
            abnormal("[Li-]");
            abnormal("[Li++]");
            abnormal("[Li--]");

            abnormal("[Li+](C)(C)");
        }

        [TestMethod()]
        public void boron()
        {
            normal("[B]");
            abnormal("[B+]");
            abnormal("[B-]");

            abnormal("[BH]");
            abnormal("[BH2]");
            normal("[BH3]");
            abnormal("[BH4]");
            abnormal("[BH5]");
        }

        [TestMethod()]
        public void Carbon_neutral()
        {
            normal("[C]");
            abnormal("[CH1]");
            normal("[CH2]");
            abnormal("[CH3]");
            normal("[CH4]");
            abnormal("[CH5]");
            abnormal("[CH6]");
        }

        [TestMethod()]
        public void Carbon_anion()
        {
            normal("[CH-]");
            abnormal("[CH2-]");
            normal("[CH3-]");
            abnormal("[CH4-]");
            abnormal("[CH5-]");
        }

        [TestMethod()]
        public void Carbon_cation()
        {
            abnormal("[CH+]");
            abnormal("[CH2+]");
            normal("[CH3+]");
            abnormal("[CH4+]");
            normal("[CH5+]");
        }

        [TestMethod()]
        public void Carbon_dianion()
        {
            abnormal("[CH--]");
            abnormal("[CH2--]");
            abnormal("[CH3--]");
            abnormal("[CH4--]");
        }

        [TestMethod()]
        public void Carbon_dication()
        {
            abnormal("[CH++]");
            abnormal("[CH2++]");
            abnormal("[CH3++]");
            abnormal("[CH4++]");
        }

        [TestMethod()]
        public void nitrogen_neutral()
        {
            normal("[N]");
            abnormal("[NH]");
            abnormal("[NH2]");
            normal("[NH3]");
            abnormal("[NH4]");
            normal("[NH5]");
            abnormal("[NH6]");
            abnormal("[NH7]");
        }

        [TestMethod()]
        public void nitrogen_cation()
        {
            abnormal("[N+]");
            abnormal("[NH+]");
            abnormal("[NH2+]");
            abnormal("[NH3+]");
            normal("[NH4+]");
            normal("[NH5+]");
            abnormal("[NH6+]");
            abnormal("[NH7+]");
        }

        [TestMethod()]
        public void nitrogen_anion()
        {
            abnormal("[N-]");
            abnormal("[NH-]");
            normal("[NH2-]");
            abnormal("[NH3-]");
            abnormal("[NH4-]");
            normal("[NH5-]");
            abnormal("[NH6-]");
            abnormal("[NH7-]");
        }

        [TestMethod()]
        public void nitrogen_dication()
        {
            abnormal("[N++]");
            abnormal("[NH++]");
            abnormal("[NH2++]");
            abnormal("[NH3++]");
            abnormal("[NH4++]");
            abnormal("[NH5++]");
            abnormal("[NH6+]");
            abnormal("[NH7+]");
        }

        [TestMethod()]
        public void nitrogen_dianion()
        {
            abnormal("[N--]");
            normal("[NH--]");
            abnormal("[NH2--]");
            abnormal("[NH3--]");
            abnormal("[NH4--]");
            normal("[NH5--]");
            abnormal("[NH6--]");
            abnormal("[NH7--]");
        }

        [TestMethod()]
        public void nitrogen_trianion()
        {
            normal("[N---]");
            abnormal("[NH---]");
            abnormal("[NH2---]");
            abnormal("[NH3---]");
            abnormal("[NH4---]");
            normal("[NH5---]");
            abnormal("[NH6---]");
            abnormal("[NH7---]");
        }

        [TestMethod()]
        public void oxygen_neutral()
        {
            normal("[O]");
            abnormal("[OH1]");
            normal("[OH2]");
            abnormal("[OH3]");
            abnormal("[OH4]");
            abnormal("[OH5]");
        }

        [TestMethod()]
        public void oxygen_cation()
        {
            abnormal("[O+]");
            abnormal("[OH1+]");
            abnormal("[OH2+]");
            normal("[OH3+]");
            abnormal("[OH4+]");
            abnormal("[OH5+]");
        }

        [TestMethod()]
        public void oxygen_anion()
        {
            abnormal("[O-]");
            normal("[OH1-]");
            abnormal("[OH2-]");
            abnormal("[OH3-]");
            abnormal("[OH4-]");
            abnormal("[OH5-]");
        }

        [TestMethod()]
        public void oxygen_dication()
        {
            abnormal("[O++]");
            abnormal("[OH1++]");
            abnormal("[OH2++]");
            abnormal("[OH3++]");
            abnormal("[OH4++]");
            abnormal("[OH5++]");
        }

        [TestMethod()]
        public void oxygen_dianion()
        {
            normal("[O--]");
            abnormal("[OH1--]");
            abnormal("[OH2--]");
            abnormal("[OH3--]");
            abnormal("[OH4--]");
            abnormal("[OH5--]");
        }

        [TestMethod()]
        public void Fluorine()
        {
            normal("[F]");
            abnormal("[F+]");
            normal("[F-]");
            abnormal("[F++]");
            abnormal("[F--]");

            normal("[FH1]");
            abnormal("[FH2]");
            abnormal("[FH3]");
            abnormal("[FH4]");
            abnormal("[FH5]");
            abnormal("[FH6]");
            abnormal("[FH7]");

            normal("[FH1-]");
            abnormal("[FH2-]");
            abnormal("[FH3-]");
            abnormal("[FH4-]");
            abnormal("[FH5-]");
            abnormal("[FH6-]");
            abnormal("[FH7-]");
        }

        [TestMethod()]
        public void Neon()
        {
            normal("[Ne]");
            abnormal("[Ne+]");
            abnormal("[Ne-]");
            abnormal("[Ne++]");
            abnormal("[Ne--]");

            abnormal("[NeH]");
        }

        [TestMethod()]
        public void Sodium()
        {
            normal("[Na]");
            normal("[Na+]");
            abnormal("[Na-]");
            abnormal("[Na++]");
            abnormal("[Na--]");

            normal("[Na](C)");
            abnormal("[Na+](C)");
            abnormal("[Na+](C)(C)");
        }

        [TestMethod()]
        public void magnesium()
        {
            normal("[Mg]");
            normal("[Mg+](C)");
            normal("[Mg++]");

            abnormal("[Mg](C)");
            normal("[Mg](C)(C)");

            normal("[Mg+](C)");
            abnormal("[Mg+](C)(C)");
            abnormal("[Mg+](C)(C)(C)");

            abnormal("[Mg++](C)");
            abnormal("[Mg++](C)(C)");
            abnormal("[Mg++](C)(C)(C)");
        }

        [TestMethod()]
        public void Phosphorus_neutral()
        {
            normal("[P]");
            abnormal("[PH1]");
            abnormal("[PH2]");
            normal("[PH3]");
            abnormal("[PH4]");
            normal("[PH5]");
            abnormal("[PH6]");
            abnormal("[PH7]");
            abnormal("[PH8]");
        }

        [TestMethod()]
        public void Phosphorus_cation()
        {
            abnormal("[P+]");
            abnormal("[PH1+]");
            abnormal("[PH2+]");
            abnormal("[PH3+]");
            normal("[PH4+]");
            abnormal("[PH5+]");
            normal("[PH6+]");
            abnormal("[PH7+]");
            abnormal("[PH8+]");
        }

        [TestMethod()]
        public void Phosphorus_anion()
        {
            abnormal("[P-]");
            abnormal("[PH1-]");
            normal("[PH2-]");
            abnormal("[PH3-]");
            normal("[PH4-]");
            abnormal("[PH5-]");
            abnormal("[PH6-]");
            abnormal("[PH7-]");
            abnormal("[PH8-]");
        }

        [TestMethod()]
        public void Phosphorus_dication()
        {
            abnormal("[P++]");
            abnormal("[PH1++]");
            abnormal("[PH2++]");
            abnormal("[PH3++]");
            abnormal("[PH4++]");
            abnormal("[PH5++]");
            abnormal("[PH6++]");
            abnormal("[PH7++]");
            abnormal("[PH8++]");
        }

        [TestMethod()]
        public void Phosphorus_dianion()
        {
            abnormal("[P--]");
            normal("[PH1--]");
            abnormal("[PH2--]");
            normal("[PH3--]");
            abnormal("[PH4--]");
            abnormal("[PH5--]");
            abnormal("[PH6--]");
            abnormal("[PH7--]");
            abnormal("[PH8--]");
        }

        [TestMethod()]
        public void Phosphorus_trianion()
        {
            normal("[P---]");
            abnormal("[PH1---]");
            normal("[PH2---]");
            abnormal("[PH3---]");
            abnormal("[PH4---]");
            abnormal("[PH5---]");
            abnormal("[PH6---]");
            abnormal("[PH7---]");
            abnormal("[PH8---]");
        }

        [TestMethod()]
        public void sulphur()
        {
            normal("[S]");
            abnormal("[SH1]");
            normal("[SH2]");
            abnormal("[SH3]");
            normal("[SH4]");
            abnormal("[SH5]");
            normal("[SH6]");
            abnormal("[SH7]");
            abnormal("[SH8]");
        }

        [TestMethod()]
        public void sulphur_cation()
        {
            abnormal("[S+]");
            abnormal("[SH1+]");
            abnormal("[SH2+]");
            normal("[SH3+]");
            abnormal("[SH4+]");
            normal("[SH5+]");
            abnormal("[SH6+]");
            normal("[SH7+]");
            abnormal("[SH8+]");
        }

        [TestMethod()]
        public void sulphur_anion()
        {
            abnormal("[S-]");
            normal("[SH1-]");
            abnormal("[SH2-]");
            normal("[SH3-]");
            abnormal("[SH4-]");
            normal("[SH5-]");
            abnormal("[SH6-]");
            abnormal("[SH7-]");
            abnormal("[SH8-]");
        }

        [TestMethod()]
        public void sulphur_dication()
        {
            abnormal("[S++]");
            abnormal("[SH1++]");
            abnormal("[SH2++]");
            abnormal("[SH3++]");
            normal("[SH4++]");
            abnormal("[SH5++]");
            normal("[SH6++]");
            abnormal("[SH7++]");
            normal("[SH8++]");
            abnormal("[SH9++]");
        }

        [TestMethod()]
        public void sulphur_trication()
        {
            abnormal("[S+++]");
            abnormal("[SH1+++]");
            abnormal("[SH2+++]");
            abnormal("[SH3+++]");
            abnormal("[SH4+++]");
            normal("[SH5+++]");
            abnormal("[SH6+++]");
            normal("[SH7+++]");
            abnormal("[SH8+++]");
            normal("[SH9+++]");
            abnormal("[SH10+++]");
        }

        [TestMethod()]
        public void chlorine()
        {
            normal("[Cl]");
            abnormal("[Cl+]");
            normal("[Cl-]");

            abnormal("[ClH1+]");
            normal("[ClH2+]");
            abnormal("[ClH3+]");
            normal("[ClH4+]");
            abnormal("[ClH5+]");
            normal("[ClH6+]");
            abnormal("[ClH7+]");
            normal("[ClH8+]");

            abnormal("[ClH1-]");
            normal("[ClH2-]");
            abnormal("[ClH3-]");
            normal("[ClH4-]");
            abnormal("[ClH5-]");
            normal("[ClH6-]");

            abnormal("[ClH1++]");
            abnormal("[ClH2++]");
            normal("[ClH3++]");
            abnormal("[ClH4++]");
            normal("[ClH5++]");
            abnormal("[ClH6++]");
            normal("[ClH7++]");
            abnormal("[ClH8++]");
            normal("[ClH9++]");
            abnormal("[ClH10++]");

            normal("[ClH1--]");
            abnormal("[ClH2--]");
            normal("[ClH3--]");
            abnormal("[ClH4--]");
            normal("[ClH5--]");
            abnormal("[ClH6--]");
            abnormal("[ClH7--]");
        }

        [TestMethod()]
        public void Argon()
        {
            normal("[Ar]");
            abnormal("[Ar+]");
            abnormal("[Ar-]");
            abnormal("[ArH]");
            abnormal("[ArH]");
        }

        [TestMethod()]
        public void Potassium()
        {
            normal("[K]");
            normal("[K+]");
            abnormal("[K-]");
            normal("[K](C)");
            abnormal("[K+](C)");
        }

        [TestMethod()]
        public void Calcium()
        {
            normal("[Ca]");
            normal("[Ca](C)(C)");
            abnormal("[Ca+]");
            normal("[Ca+](C)");
            abnormal("[Ca+](C)(C)");
            normal("[Ca++]");
            abnormal("[Ca++](C)");
            abnormal("[Ca++](C)(C)");

        }

        [TestMethod()]
        public void Arsenic_neutral()
        {
            normal("[As]");
            abnormal("[AsH1]");
            abnormal("[AsH2]");
            normal("[AsH3]");
            abnormal("[AsH4]");
            normal("[AsH5]");
            abnormal("[AsH6]");
        }

        [TestMethod()]
        public void Arsenic_cation()
        {
            abnormal("[As+]");
            abnormal("[AsH1+]");
            abnormal("[AsH2+]");
            abnormal("[AsH3+]");
            normal("[AsH4+]");
            abnormal("[AsH5+]");
            normal("[AsH6+]");
            abnormal("[AsH7+]");
        }

        [TestMethod()]
        public void Arsenic_dication()
        {
            abnormal("[As++]");
            abnormal("[AsH1++]");
            abnormal("[AsH2++]");
            abnormal("[AsH3++]");
            abnormal("[AsH4++]");
            abnormal("[AsH5++]");
            abnormal("[AsH6++]");
            abnormal("[AsH7++]");
        }

        [TestMethod()]
        public void Arsenic_anion()
        {
            abnormal("[As-]");
            abnormal("[AsH1-]");
            normal("[AsH2-]");
            abnormal("[AsH3-]");
            normal("[AsH4-]");
            abnormal("[AsH5-]");
            abnormal("[AsH6-]");
            abnormal("[AsH7-]");
        }

        [TestMethod()]
        public void Arsenic_dianion()
        {
            abnormal("[As--]");
            normal("[AsH1--]");
            abnormal("[AsH2--]");
            normal("[AsH3--]");
            abnormal("[AsH4--]");
            abnormal("[AsH5--]");
            abnormal("[AsH6--]");
            abnormal("[AsH7--]");
        }

        [TestMethod()]
        public void Arsenic_trianion()
        {
            normal("[As---]");
            abnormal("[AsH1---]");
            normal("[AsH2---]");
            abnormal("[AsH3---]");
            abnormal("[AsH4---]");
            abnormal("[AsH5---]");
            abnormal("[AsH6---]");
            abnormal("[AsH7---]");
        }

        [TestMethod()]
        public void selenium_neutral()
        {
            normal("[Se]");
            abnormal("[SeH1]");
            normal("[SeH2]");
            abnormal("[SeH3]");
            normal("[SeH4]");
            abnormal("[SeH5]");
            normal("[SeH6]");
            abnormal("[SeH7]");
            abnormal("[SeH8]");
        }

        [TestMethod()]
        public void selenium_cation()
        {
            abnormal("[Se+]");
            abnormal("[SeH1+]");
            abnormal("[SeH2+]");
            normal("[SeH3+]");
            abnormal("[SeH4+]");
            normal("[SeH5+]");
            abnormal("[SeH6+]");
            normal("[SeH7+]");
            abnormal("[SeH8+]");
        }

        [TestMethod()]
        public void selenium_anion()
        {
            abnormal("[Se-]");
            normal("[SeH1-]");
            abnormal("[SeH2-]");
            normal("[SeH3-]");
            abnormal("[SeH4-]");
            normal("[SeH5-]");
            abnormal("[SeH6-]");
            abnormal("[SeH7-]");
            abnormal("[SeH8-]");
        }

        // selenium charge is unrestricted
        [TestMethod()]
        public void selenium_n_cation()
        {
            abnormal("[Se+6]");
            abnormal("[SeH1+6]");
            abnormal("[SeH2+6]");
            abnormal("[SeH3+6]");
            abnormal("[SeH4+6]");
            abnormal("[SeH5+6]");
            abnormal("[SeH6+6]");
            abnormal("[SeH7+6]");
            normal("[SeH8+6]");
            abnormal("[SeH9+6]");
            normal("[SeH10+6]");
            abnormal("[SeH11+6]");
            normal("[SeH12+6]");
        }

        [TestMethod()]
        public void bromine()
        {
            normal("[Br]");
            abnormal("[Br+]");
            normal("[Br-]");

            abnormal("[BrH1+]");
            normal("[BrH2+]");
            abnormal("[BrH3+]");
            normal("[BrH4+]");
            abnormal("[BrH5+]");
            normal("[BrH6+]");
            abnormal("[BrH7+]");
            normal("[BrH8+]");

            abnormal("[BrH1-]");
            normal("[BrH2-]");
            abnormal("[BrH3-]");
            normal("[BrH4-]");
            abnormal("[BrH5-]");
            normal("[BrH6-]");

            abnormal("[BrH1++]");
            abnormal("[BrH2++]");
            normal("[BrH3++]");
            abnormal("[BrH4++]");
            normal("[BrH5++]");
            abnormal("[BrH6++]");
            normal("[BrH7++]");
            abnormal("[BrH8++]");
            normal("[BrH9++]");
            abnormal("[BrH10++]");

            normal("[BrH1--]");
            abnormal("[BrH2--]");
            normal("[BrH3--]");
            abnormal("[BrH4--]");
            normal("[BrH5--]");
            abnormal("[BrH6--]");
            abnormal("[BrH7--]");
        }

        [TestMethod()]
        public void Krypton()
        {
            normal("[Kr]");
            abnormal("[Kr+]");
            abnormal("[Kr-]");
            abnormal("[KrH]");
        }

        [TestMethod()]
        public void Strontium()
        {
            normal("[Sr]");
            abnormal("[Sr+]");
            normal("[Sr++]");
            normal("[Sr+](C)");
            normal("[Sr](C)(C)");
        }

        [TestMethod()]
        public void tellurium_neutral()
        {
            normal("[Te]");
            abnormal("[TeH1]");
            normal("[TeH2]");
            abnormal("[TeH3]");
            normal("[TeH4]");
            abnormal("[TeH5]");
            normal("[TeH6]");
            abnormal("[TeH7]");
            abnormal("[TeH8]");
        }

        [TestMethod()]
        public void tellurium_cation()
        {
            abnormal("[Te+]");
            abnormal("[TeH1+]");
            abnormal("[TeH2+]");
            normal("[TeH3+]");
            abnormal("[TeH4+]");
            normal("[TeH5+]");
            abnormal("[TeH6+]");
            normal("[TeH7+]");
            abnormal("[TeH8+]");
        }

        [TestMethod()]
        public void tellurium_anion()
        {
            abnormal("[Te-]");
            normal("[TeH1-]");
            abnormal("[TeH2-]");
            normal("[TeH3-]");
            abnormal("[TeH4-]");
            normal("[TeH5-]");
            abnormal("[TeH6-]");
            abnormal("[TeH7-]");
            abnormal("[TeH8-]");
        }

        // tellurium charge is unrestricted
        [TestMethod()]
        public void tellurium_n_cation()
        {
            abnormal("[Te+6]");
            abnormal("[TeH1+6]");
            abnormal("[TeH2+6]");
            abnormal("[TeH3+6]");
            abnormal("[TeH4+6]");
            abnormal("[TeH5+6]");
            abnormal("[TeH6+6]");
            abnormal("[TeH7+6]");
            normal("[TeH8+6]");
            abnormal("[TeH9+6]");
            normal("[TeH10+6]");
            abnormal("[TeH11+6]");
            normal("[TeH12+6]");
        }

        [TestMethod()]
        public void iodine()
        {
            normal("[I]");
            abnormal("[I+]");
            normal("[I-]");

            abnormal("[IH1+]");
            normal("[IH2+]");
            abnormal("[IH3+]");
            normal("[IH4+]");
            abnormal("[IH5+]");
            normal("[IH6+]");
            abnormal("[IH7+]");
            normal("[IH8+]");

            abnormal("[IH1-]");
            normal("[IH2-]");
            abnormal("[IH3-]");
            normal("[IH4-]");
            abnormal("[IH5-]");
            normal("[IH6-]");

            abnormal("[IH1++]");
            abnormal("[IH2++]");
            normal("[IH3++]");
            abnormal("[IH4++]");
            normal("[IH5++]");
            abnormal("[IH6++]");
            normal("[IH7++]");
            abnormal("[IH8++]");
            normal("[IH9++]");
            abnormal("[IH10++]");

            normal("[IH1--]");
            abnormal("[IH2--]");
            normal("[IH3--]");
            abnormal("[IH4--]");
            normal("[IH5--]");
            abnormal("[IH6--]");
            abnormal("[IH7--]");
        }

        [TestMethod()]
        public void astatine()
        {
            normal("[At]");
            abnormal("[At+]");
            normal("[At-]");

            abnormal("[AtH1+]");
            normal("[AtH2+]");
            abnormal("[AtH3+]");
            normal("[AtH4+]");
            abnormal("[AtH5+]");
            normal("[AtH6+]");
            abnormal("[AtH7+]");
            normal("[AtH8+]");

            abnormal("[AtH1-]");
            normal("[AtH2-]");
            abnormal("[AtH3-]");
            normal("[AtH4-]");
            abnormal("[AtH5-]");
            normal("[AtH6-]");

            abnormal("[AtH1++]");
            abnormal("[AtH2++]");
            normal("[AtH3++]");
            abnormal("[AtH4++]");
            normal("[AtH5++]");
            abnormal("[AtH6++]");
            normal("[AtH7++]");
            abnormal("[AtH8++]");
            normal("[AtH9++]");
            abnormal("[AtH10++]");

            normal("[AtH1--]");
            abnormal("[AtH2--]");
            normal("[AtH3--]");
            abnormal("[AtH4--]");
            normal("[AtH5--]");
            abnormal("[AtH6--]");
            abnormal("[AtH7--]");
        }

        [TestMethod()]
        public void barium()
        {
            normal("[Ba]");
            abnormal("[Ba+]");
            normal("[Ba++]");
            normal("[Ba+](C)");
            normal("[Ba](C)(C)");
        }

        [TestMethod()]
        public void radium()
        {
            normal("[Ra]");
            abnormal("[Ra+]");
            normal("[Ra++]");
            normal("[Ra+](C)");
            normal("[Ra](C)(C)");
            abnormal("[Ra++](C)(C)");
        }

        static void normal(string str)
        {
            normal(str, 0);
        }

        static void abnormal(string str)
        {
            abnormal(str, 0);
        }

        static void normal(string str, int v)
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

        static void abnormal(string str, int v)
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
