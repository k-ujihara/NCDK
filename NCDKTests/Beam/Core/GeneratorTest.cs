using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace NCDK.Beam
{
   /// <summary> <author>John May </author></summary>
    [TestClass()]
    public class GeneratorTest
    {
        [TestMethod()]
        public void PermuteTH_3_nonRing()
        {
            string input = "C[C@H](N)O";
            Graph g = Parser.Parse(input);
            Assert.AreEqual(Generator.Generate(g), input);
        }

        [TestMethod()]
        public void PermuteTH_4_nonRing()
        {
            string input = "C[C@]([H])(N)O";
            Graph g = Parser.Parse(input);
            Assert.AreEqual(Generator.Generate(g), input);
        }

        [TestMethod()]
        public void PermuteTH_4_ring()
        {
            string input = "C[C@]12CCCC[C@@]1(C)OCCC2";
            Graph g = Parser.Parse(input);
            Assert.AreEqual(Generator.Generate(g), input);
        }

        public void Test()
        {
            Console.Out.WriteLine(RandomPermutations("[C@]([H])(N)(C)C(=O)O", 50));
        }

        public void Test2()
        {
            Console.Out.WriteLine(RandomPermutations("[C@H](N)(C)C(=O)O", 50));
        }

        public void Test3()
        {
            Console.Out.WriteLine(RandomPermutations("[C@H]12CCCC[C@@]1(C)OCCC2", 50));
        }

        [TestMethod()]
        public void ImplicitHCentre()
        {

            RoundTrip("[C@@H](N)(O)C");

            // permutations
            RoundTrip("[C@@H](N)(O)C", new int[] { 0, 1, 2, 3 }, "[C@@H](N)(O)C");
            RoundTrip("[C@@H](N)(O)C", new int[] { 0, 1, 3, 2 }, "[C@H](N)(C)O");
            RoundTrip("[C@@H](N)(O)C", new int[] { 0, 2, 1, 3 }, "[C@H](O)(N)C");
            RoundTrip("[C@@H](N)(O)C", new int[] { 0, 2, 3, 1 }, "[C@@H](C)(N)O");
            RoundTrip("[C@@H](N)(O)C", new int[] { 0, 3, 1, 2 }, "[C@@H](O)(C)N");
            RoundTrip("[C@@H](N)(O)C", new int[] { 0, 3, 2, 1 }, "[C@H](C)(O)N");

            RoundTrip("[C@@H](N)(O)C", new int[] { 1, 0, 2, 3 }, "N[C@H](O)C");
            RoundTrip("[C@@H](N)(O)C", new int[] { 1, 0, 3, 2 }, "N[C@@H](C)O");

            RoundTrip("[C@@H](N)(O)C", new int[] { 1, 2, 0, 3 }, "O[C@@H](N)C");
            RoundTrip("[C@@H](N)(O)C", new int[] { 1, 3, 0, 2 }, "O[C@H](C)N");

            RoundTrip("[C@@H](N)(O)C", new int[] { 1, 2, 3, 0 }, "C[C@H](N)O");
            RoundTrip("[C@@H](N)(O)C", new int[] { 1, 3, 2, 0 }, "C[C@@H](O)N");

            RoundTrip("[C@H](N)(C)O");

            RoundTrip("[C@H](N)(C)O", new int[] { 0, 1, 2, 3 }, "[C@H](N)(C)O");
            RoundTrip("[C@H](N)(C)O", new int[] { 0, 1, 3, 2 }, "[C@@H](N)(O)C");
            RoundTrip("[C@H](N)(C)O", new int[] { 0, 2, 1, 3 }, "[C@@H](C)(N)O");
            RoundTrip("[C@H](N)(C)O", new int[] { 0, 2, 3, 1 }, "[C@H](O)(N)C");
            RoundTrip("[C@H](N)(C)O", new int[] { 0, 3, 1, 2 }, "[C@H](C)(O)N");
            RoundTrip("[C@H](N)(C)O", new int[] { 0, 3, 2, 1 }, "[C@@H](O)(C)N");

            RoundTrip("[C@H](N)(C)O", new int[] { 1, 0, 2, 3 }, "N[C@@H](C)O");
            RoundTrip("[C@H](N)(C)O", new int[] { 1, 0, 3, 2 }, "N[C@H](O)C");

            RoundTrip("[C@H](N)(C)O", new int[] { 1, 2, 0, 3 }, "C[C@H](N)O");
            RoundTrip("[C@H](N)(C)O", new int[] { 1, 3, 0, 2 }, "C[C@@H](O)N");

            RoundTrip("[C@H](N)(C)O", new int[] { 1, 2, 3, 0 }, "O[C@@H](N)C");
            RoundTrip("[C@H](N)(C)O", new int[] { 1, 3, 2, 0 }, "O[C@H](C)N");

            RoundTrip("N[C@@H](C)O");
            RoundTrip("N[C@@H](C)O");
            RoundTrip("N[C@H](O)C");
            RoundTrip("O[C@@H](N)C");
            RoundTrip("O[C@H](C)N");
            RoundTrip("C[C@@H](O)N");
            RoundTrip("C[C@H](N)O");
        }

        [TestMethod()]
        public void Ring_closures1()
        {
            RoundTrip("C1=CN=CC2=NC=N[C@@H]21");
        }

        [TestMethod()]
        public void Ring_closures2()
        {
            RoundTrip("C1=CN=CC2=NC=N[C@H]21");
        }

        [TestMethod()]
        public void Ring_closures3()
        {
            RoundTrip("C1=CC(=CC2=NC(=N[C@@H]21)C(F)(F)F)N");
        }

        [TestMethod()]
        public void Ring_closures4()
        {
            RoundTrip("C1=CC(=CC2=NC(=N[C@H]21)C(F)(F)F)N");
        }


        [TestMethod()]
        public void LowRingNumberOrder()
        {
            RoundTrip("C1=CC2=CC=CC=C2C=C1");
        }

        [TestMethod()]
        public void MultipleRingNumberOrder()
        {
            RoundTrip("C1=CC2=C3C4=C5C(C=CC6=C5C7=C(C=C6)C=CC(C=C2)=C37)=CC=C14");
        }

        [TestMethod()]
        public void HighRingNumberOrder()
        {
            RoundTrip("C1CC2CCC3=C4C2=C5C1CCC6=C5C7=C8C(C=C9CCC%10CCC%11CCC%12=CC(=C3)C(C%13=C8C9=C%10C%11=C%12%13)=C47)=C6");
        }

        [TestMethod()]
        public void BondTypeOnFirstAtom1()
        {
            string smi = "C1C=CC=CC=1";
            string exp = "C=1C=CC=CC1";
            Assert.AreEqual(Generator.Generate(Parser.Parse(smi)), exp);
        }

        [TestMethod()]
        public void BondTypeOnFirstAtom2()
        {
            string smi = "C=1C=CC=CC1";
            string exp = "C=1C=CC=CC1";
            Assert.AreEqual(Generator.Generate(Parser.Parse(smi)), exp);
        }

        [TestMethod()]
        public void BondTypeOnFirstAtom3()
        {
            string smi = "C=1C=CC=CC=1";
            string exp = "C=1C=CC=CC1";
            Assert.AreEqual(Generator.Generate(Parser.Parse(smi)), exp);
        }

        [TestMethod()]
        public void DirectionalBondTypeOnFirstAtom1()
        {

            string smi = "C1CCCCCCCCCCC\\C=C/1";
            string exp = "C\\1CCCCCCCCCCC\\C=C1";
            Assert.AreEqual(Generator.Generate(Parser.Parse(smi)), exp);
        }

        [TestMethod()]
        public void DirectionalBondTypeOnFirstAtom2()
        {
            string smi = "C\\1CCCCCCCCCCC\\C=C1";
            string exp = "C\\1CCCCCCCCCCC\\C=C1";
            Assert.AreEqual(Generator.Generate(Parser.Parse(smi)), exp);
        }

        [TestMethod()]
        public void DirectionalBondTypeOnFirstAtom3()
        {
            string smi = "C\\1CCCCCCCCCCC\\C=C/1";
            string exp = "C\\1CCCCCCCCCCC\\C=C1";
            Assert.AreEqual(Generator.Generate(Parser.Parse(smi)), exp);
        }

        [TestMethod()]
        public void ReuseNumbering()
        {
            Generator generator = new Generator(Graph.FromSmiles("c1cc1c2ccc2"),
                                                new Generator.ReuseRingNumbering(1));
            Assert.AreEqual(generator.GetString(), "c1cc1c1ccc1");
        }

        [TestMethod()]
        public void SodiumChloride()
        {
            RoundTrip("[Na+].[Cl-]");
        }

        [TestMethod()]
        public void Disconnected()
        {
            RoundTrip("CCCC.OOOO.C[CH]C.CNO");
        }

        [TestMethod()]
        public void ExtendedTetrhedral_al1()
        {
            Graph g = Graph.FromSmiles("CC=[C]=CC");
            g.AddTopology(Topology.CreateExtendedTetrahedral(2, new int[] { 0, 1, 3, 4 }, Configuration.AL1));
            g.SetFlags(Graph.HAS_EXT_STRO);
            Assert.AreEqual(g.ToSmiles(), "CC=[C@]=CC");
        }

        [TestMethod()]
        public void ExtendedTetrhedral_al2()
        {
            Graph g = Graph.FromSmiles("CC=[C]=CC");
            g.AddTopology(Topology.CreateExtendedTetrahedral(2, new int[] { 0, 1, 3, 4 }, Configuration.AL2));
            g.SetFlags(Graph.HAS_EXT_STRO);
            Assert.AreEqual(g.ToSmiles(), "CC=[C@@]=CC");
        }

        [TestMethod()]
        public void ExtendedTetrhedral_al1_permute_1()
        {
            Graph g = Graph.FromSmiles("CC=[C]=CC");
            g.AddTopology(Topology.CreateExtendedTetrahedral(2, new int[] { 0, 1, 3, 4 }, Configuration.AL1));
            g.SetFlags(Graph.HAS_EXT_STRO);
            g = g.Permute(new int[] { 1, 0, 2, 3, 4 });
            Assert.AreEqual(g.ToSmiles(), "C(C)=[C@@]=CC");
        }

        [TestMethod()]
        public void ExtendedTetrhedral_al1_inv_permute_1()
        {
            Graph g = Graph.FromSmiles("C(C)=[C]=CC");
            g.AddTopology(Topology.CreateExtendedTetrahedral(2, new int[] { 0, 1, 3, 4 }, Configuration.AL1));
            g.SetFlags(Graph.HAS_EXT_STRO);
            g = g.Permute(new int[] { 1, 0, 2, 3, 4 });
            Assert.AreEqual(g.ToSmiles(), "CC=[C@@]=CC");
        }

        [TestMethod()]
        public void ExtendedTetrhedral_al1_permute_2()
        {
            Graph g = Graph.FromSmiles("CC=[C]=CC");
            g.AddTopology(Topology.CreateExtendedTetrahedral(2, new int[] { 0, 1, 3, 4 }, Configuration.AL1));
            g.SetFlags(Graph.HAS_EXT_STRO);
            g = g.Permute(new int[] { 4, 3, 2, 1, 0 });
            Assert.AreEqual(g.ToSmiles(), "CC=[C@]=CC");
        }

        [TestMethod()]
        public void ExtendedTetrhedral_al1_permute_3()
        {
            Graph g = Graph.FromSmiles("CC=[C]=CC");
            g.AddTopology(Topology.CreateExtendedTetrahedral(2, new int[] { 0, 1, 3, 4 }, Configuration.AL1));
            g.SetFlags(Graph.HAS_EXT_STRO);
            g = g.Permute(new int[] { 4, 3, 2, 0, 1 });
            Assert.AreEqual(g.ToSmiles(), "C(C)=[C@@]=CC");
        }

        [TestMethod()]
        public void ResetRingNumbersBetweenComponents1()
        {
            Graph g = Graph.FromSmiles("C1CC1.C1CC1");
            Assert.AreEqual("C1CC1.C1CC1",
                new Generator(g, new Generator.ReuseRingNumbering(1)).GetString());
        }

        [TestMethod()]
        public void ResetRingNumbersBetweenComponents2()
        {
            Graph g = Graph.FromSmiles("C1CC1.C1CC1");
            Assert.AreEqual("C1CC1.C1CC1",
                new Generator(g, new Generator.IterativeRingNumbering(1)).GetString());
        }


        [TestMethod()]
        public void ReusingNumbering()
        {
            Generator.RingNumbering rnums = new Generator.ReuseRingNumbering(0);
            for (int i = 0; i < 50; i++)
            {
                int rnum = rnums.Next();
                Assert.AreEqual(rnum, i);
                rnums.Use(rnum);
            }
            rnums.Free(40);
            rnums.Free(20);
            rnums.Free(4);
            Assert.AreEqual(rnums.Next(), 4);
            rnums.Use(4);
            Assert.AreEqual(rnums.Next(), 20);
            rnums.Use(20);
            Assert.AreEqual(rnums.Next(), 40);
            rnums.Use(40);
            for (int i = 50; i < 100; i++)
            {
                int rnum = rnums.Next();
                Assert.AreEqual(rnum, i);
                rnums.Use(rnum);
            }
        }

        [TestMethod()]
        public void IterativeNumbering()
        {
            Generator.RingNumbering rnums = new Generator.IterativeRingNumbering(0);
            for (int i = 0; i < 50; i++)
            {
                int rnum = rnums.Next();
                Assert.AreEqual(rnum, i);
                rnums.Use(rnum);
            }
            rnums.Free(40);
            rnums.Free(25);
            Assert.AreEqual(rnums.Next(), 50);
            rnums.Use(50);
            Assert.AreEqual(rnums.Next(), 51);
            rnums.Use(51);
            Assert.AreEqual(rnums.Next(), 52);
            rnums.Use(52);
            for (int i = 53; i < 100; i++)
            {
                int rnum = rnums.Next();
                Assert.AreEqual(rnum, i);
                rnums.Use(rnum);
            }
            rnums.Free(20);
            rnums.Free(5);
            Assert.AreEqual(rnums.Next(), 5);
            rnums.Use(5);
            Assert.AreEqual(rnums.Next(), 20);
            rnums.Use(20);
            Assert.AreEqual(rnums.Next(), 25);
            rnums.Use(25);
            Assert.AreEqual(rnums.Next(), 40);
            rnums.Use(40);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void MaxRingNumbers()
        {
            Generator.RingNumbering rnums = new Generator.IterativeRingNumbering(0);
            for (int i = 0; i < 101; i++)
            {
                int rnum = rnums.Next();
                rnums.Use(rnum);
            }
        }

        internal static void RoundTrip(string smi)
        {
            Assert.AreEqual(Generator.Generate(Parser.Parse(smi)), smi);
        }

        internal static void RoundTrip(string smi, int[] p, string res)
        {
            Assert.AreEqual(Generator.Generate(Parser.Parse(smi).Permute(p)), res);
        }

        /// <summary>
        ///Generate random permutations of the molecule.
        /// </summary>
        /// <param name="input">input SMILES</param>
        /// <param name="n">number of generations (how many molecules to produce)</param>
        /// <returns>a single SMILES string of disconnected molecules (input) randomly</returns>
        ///        permuted
        ///@ the input SMILES was invalid
        private static string RandomPermutations(string input, int n)
        {
            Graph g = Parser.Parse(input);
            StringBuilder sb = new StringBuilder();
            sb.Append(Generator.Generate(g));
            for (int i = 0; i < n; i++)
            {
                sb.Append('.');
                int[] p = Random(g.Order);
                string smi = Generator.Generate(g.Permute(p));
                g = Parser.Parse(smi);
                sb.Append(smi);
            }
            return sb.ToString();
        }

        static int[] Ident(int n)
        {
            int[] p = new int[n];
            for (int i = 0; i < n; i++)
                p[i] = i;
            return p;
        }

        static int[] Random(int n)
        {
            int[] p = Ident(n);
            Random rnd = new Random();
            for (int i = n; i > 1; i--)
                Swap(p, i - 1, rnd.Next(i));
            return p;
        }

        static int[] Inv(int[] p)
        {
            int[] q = (int[])p.Clone();
            for (int i = 0; i < p.Length; i++)
                q[p[i]] = i;
            return q;
        }

        static void Swap(int[] p, int i, int j)
        {
            int tmp = p[i];
            p[i] = p[j];
            p[j] = tmp;
        }
    }
}
