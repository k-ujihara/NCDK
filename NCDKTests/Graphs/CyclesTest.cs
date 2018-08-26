using NCDK.Common.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;
using NCDK.IO;
using NCDK.Templates;
using System.Collections.Generic;
using static NCDK.Templates.TestMoleculeFactory;

namespace NCDK.Graphs
{
    /// <summary>
    /// Note - these methods are tested in isolation in their respective classes and
    /// these are mainly to keep the coverage checker happy.
    /// </summary>
    // @author John May
    // @cdk.module test-core
    [TestClass()]
    public class CyclesTest
    {
        [TestMethod()]
        public virtual void All()
        {
            CheckSize(Cycles.FindAll(MakeBiphenyl()), 2);
            CheckSize(Cycles.FindAll(MakeBicycloRings()), 3);
            CheckSize(Cycles.FindAll(MakeNaphthalene()), 3);
            CheckSize(Cycles.FindAll(MakeAnthracene()), 6);
            CheckSize(Cycles.FindAll(MakeCyclophaneLike()), 135);
            CheckSize(Cycles.FindAll(MakeGappedCyclophaneLike()), 135);
        }

        [TestMethod()]
        public virtual void TestMCB()
        {
            CheckSize(Cycles.FindMCB(MakeBiphenyl()), 2);
            CheckSize(Cycles.FindMCB(MakeBicycloRings()), 2);
            CheckSize(Cycles.FindMCB(MakeNaphthalene()), 2);
            CheckSize(Cycles.FindMCB(MakeAnthracene()), 3);
            CheckSize(Cycles.FindMCB(MakeCyclophaneLike()), 8);
            CheckSize(Cycles.FindMCB(MakeGappedCyclophaneLike()), 8);
        }

        [TestMethod()]
        public virtual void TestRelevant()
        {
            CheckSize(Cycles.FindRelevant(MakeBiphenyl()), 2);
            CheckSize(Cycles.FindRelevant(MakeBicycloRings()), 3);
            CheckSize(Cycles.FindRelevant(MakeNaphthalene()), 2);
            CheckSize(Cycles.FindRelevant(MakeAnthracene()), 3);
            CheckSize(Cycles.FindRelevant(MakeCyclophaneLike()), 135);
            CheckSize(Cycles.FindRelevant(MakeGappedCyclophaneLike()), 135);
        }

        [TestMethod()]
        public virtual void TestEssential()
        {
            CheckSize(Cycles.FindEssential(MakeBiphenyl()), 2);
            CheckSize(Cycles.FindEssential(MakeBicycloRings()), 0);
            CheckSize(Cycles.FindEssential(MakeNaphthalene()), 2);
            CheckSize(Cycles.FindEssential(MakeAnthracene()), 3);
            CheckSize(Cycles.FindEssential(MakeCyclophaneLike()), 7);
        }

        [TestMethod()]
        public virtual void TestTripletShort()
        {
            CheckSize(Cycles.FindTripletShort(MakeBiphenyl()), 2);
            CheckSize(Cycles.FindTripletShort(MakeBicycloRings()), 3);
            CheckSize(Cycles.FindTripletShort(MakeNaphthalene()), 3);
            CheckSize(Cycles.FindTripletShort(MakeAnthracene()), 5);
            CheckSize(Cycles.FindTripletShort(MakeCyclophaneLike()), 135);
            CheckSize(Cycles.FindTripletShort(MakeGappedCyclophaneLike()), 135);
        }

        [TestMethod()]
        public virtual void TestEdgeShort()
        {
            CheckSize(Cycles.FindEdgeShort(MakeBiphenyl()), 2);
            CheckSize(Cycles.FindEdgeShort(MakeBicycloRings()), 3);
            CheckSize(Cycles.FindEdgeShort(MakeNaphthalene()), 2);
            CheckSize(Cycles.FindEdgeShort(MakeAnthracene()), 3);
            CheckSize(Cycles.FindEdgeShort(MakeCyclophaneLike()), 7);
            CheckSize(Cycles.FindEdgeShort(MakeGappedCyclophaneLike()), 135);
        }

        [TestMethod()]
        public virtual void TestVertexShort()
        {
            CheckSize(Cycles.FindVertexShort(MakeBiphenyl()), 2);
            CheckSize(Cycles.FindVertexShort(MakeBicycloRings()), 3);
            CheckSize(Cycles.FindVertexShort(MakeNaphthalene()), 2);
            CheckSize(Cycles.FindVertexShort(MakeAnthracene()), 3);
            CheckSize(Cycles.FindVertexShort(MakeCyclophaneLike()), 7);
            CheckSize(Cycles.FindVertexShort(MakeGappedCyclophaneLike()), 7);
        }

        [TestMethod()]
        public virtual void TestCDKAromaticSet()
        {
            CheckSize(Cycles.CDKAromaticSetFinder.Find(MakeBiphenyl()), 2);
            CheckSize(Cycles.CDKAromaticSetFinder.Find(MakeBicycloRings()), 3);
            CheckSize(Cycles.CDKAromaticSetFinder.Find(MakeNaphthalene()), 3);
            CheckSize(Cycles.CDKAromaticSetFinder.Find(MakeAnthracene()), 6);
            CheckSize(Cycles.CDKAromaticSetFinder.Find(MakeCyclophaneLike()), 8);
            CheckSize(Cycles.CDKAromaticSetFinder.Find(MakeGappedCyclophaneLike()), 8);
        }

        [TestMethod()]
        public virtual void FindAllOrVertexShort()
        {
            CheckSize(Cycles.AllOrVertexShortFinder.Find(MakeBiphenyl()), 2);
            CheckSize(Cycles.AllOrVertexShortFinder.Find(MakeBicycloRings()), 3);
            CheckSize(Cycles.AllOrVertexShortFinder.Find(MakeNaphthalene()), 3);
            CheckSize(Cycles.AllOrVertexShortFinder.Find(MakeAnthracene()), 6);
            CheckSize(Cycles.AllOrVertexShortFinder.Find(MakeCyclophaneLike()), 135);
            CheckSize(Cycles.AllOrVertexShortFinder.Find(MakeGappedCyclophaneLike()), 135);
            CheckSize(Cycles.AllOrVertexShortFinder.Find(GetFullerene()), 120);
        }

        [TestMethod()]
        public virtual void CDKAromaticSet_withGraph()
        {
            CheckSize(Cycles.CDKAromaticSetFinder.Find(MakeBiphenyl(), GraphUtil.ToAdjList(MakeBiphenyl()), int.MaxValue),
                    2);
            CheckSize(
                    Cycles.CDKAromaticSetFinder.Find(MakeBicycloRings(), GraphUtil.ToAdjList(MakeBicycloRings()),
                            int.MaxValue), 3);
            CheckSize(
                    Cycles.CDKAromaticSetFinder.Find(MakeNaphthalene(), GraphUtil.ToAdjList(MakeNaphthalene()),
                            int.MaxValue), 3);
            CheckSize(
                    Cycles.CDKAromaticSetFinder
                            .Find(MakeAnthracene(), GraphUtil.ToAdjList(MakeAnthracene()), int.MaxValue), 6);
            CheckSize(
                    Cycles.CDKAromaticSetFinder.Find(MakeCyclophaneLike(), GraphUtil.ToAdjList(MakeCyclophaneLike()),
                            int.MaxValue), 8);
            CheckSize(
                    Cycles.CDKAromaticSetFinder.Find(MakeGappedCyclophaneLike(),
                            GraphUtil.ToAdjList(MakeGappedCyclophaneLike()), int.MaxValue), 8);
        }

        [TestMethod()]
        public virtual void AllOrVertexShort_withGraph()
        {
            CheckSize(Cycles.AllOrVertexShortFinder.Find(MakeBiphenyl(), GraphUtil.ToAdjList(MakeBiphenyl()), int.MaxValue), 2);
            CheckSize(Cycles.AllOrVertexShortFinder.Find(MakeBicycloRings(), GraphUtil.ToAdjList(MakeBicycloRings()), int.MaxValue), 3);
            CheckSize(Cycles.AllOrVertexShortFinder.Find(MakeNaphthalene(), GraphUtil.ToAdjList(MakeNaphthalene()), int.MaxValue), 3);
            CheckSize(Cycles.AllOrVertexShortFinder.Find(MakeAnthracene(), GraphUtil.ToAdjList(MakeAnthracene()), int.MaxValue), 6);
            CheckSize(Cycles.AllOrVertexShortFinder.Find(MakeCyclophaneLike(), GraphUtil.ToAdjList(MakeCyclophaneLike()), int.MaxValue), 135);
            CheckSize(Cycles.AllOrVertexShortFinder.Find(MakeGappedCyclophaneLike(), GraphUtil.ToAdjList(MakeGappedCyclophaneLike()), int.MaxValue), 135);
            CheckSize(Cycles.AllOrVertexShortFinder.Find(GetFullerene(), GraphUtil.ToAdjList(GetFullerene()), int.MaxValue), 120);
        }

        [TestMethod()]
        public virtual void AllUpToLength()
        {
            CheckSize(Cycles.GetAllFinder(6).Find(MakeBiphenyl(), GraphUtil.ToAdjList(MakeBiphenyl()), int.MaxValue), 2);
            CheckSize(Cycles.GetAllFinder(6).Find(MakeBicycloRings(), GraphUtil.ToAdjList(MakeBicycloRings()), int.MaxValue), 3);
            CheckSize(Cycles.GetAllFinder(6).Find(MakeNaphthalene(), GraphUtil.ToAdjList(MakeNaphthalene()), int.MaxValue), 2);
            CheckSize(Cycles.GetAllFinder(6).Find(MakeAnthracene(), GraphUtil.ToAdjList(MakeAnthracene()), int.MaxValue), 3);
        }

        [TestMethod()]
        public virtual void PathsAreCopy()
        {
            Cycles cs = Cycles.FindAll(MakeAnthracene());
            int[][] org = cs.GetPaths();
            org[0][0] = -203; // modify
            Assert.IsFalse(Compares.AreEqual(org, cs.GetPaths())); // internal is unchanged
        }

        [TestMethod()]
        public virtual void ToRingSet()
        {
            IAtomContainer biphenyl = MakeBiphenyl();
            IRingSet rs = Cycles.FindVertexShort(biphenyl).ToRingSet();
            IEnumerator<IAtomContainer> it = rs.GetEnumerator();
            Assert.IsTrue(it.MoveNext());
            IAtomContainer r1 = it.Current;

            Assert.AreEqual(r1.Atoms[0], biphenyl.Atoms[0]);
            Assert.AreEqual(r1.Atoms[1], biphenyl.Atoms[1]);
            Assert.AreEqual(r1.Atoms[2], biphenyl.Atoms[2]);
            Assert.AreEqual(r1.Atoms[3], biphenyl.Atoms[3]);
            Assert.AreEqual(r1.Atoms[4], biphenyl.Atoms[4]);
            Assert.AreEqual(r1.Atoms[5], biphenyl.Atoms[5]);

            Assert.AreEqual(r1.Bonds[0], biphenyl.Bonds[0]);
            Assert.AreEqual(r1.Bonds[1], biphenyl.Bonds[1]);
            Assert.AreEqual(r1.Bonds[2], biphenyl.Bonds[2]);
            Assert.AreEqual(r1.Bonds[3], biphenyl.Bonds[3]);
            Assert.AreEqual(r1.Bonds[4], biphenyl.Bonds[4]);
            Assert.AreEqual(r1.Bonds[5], biphenyl.Bonds[5]);

            Assert.IsTrue(it.MoveNext());
            IAtomContainer r2 = it.Current;

            Assert.AreEqual(r2.Atoms[0], biphenyl.Atoms[6]);
            Assert.AreEqual(r2.Atoms[1], biphenyl.Atoms[7]);
            Assert.AreEqual(r2.Atoms[2], biphenyl.Atoms[8]);
            Assert.AreEqual(r2.Atoms[3], biphenyl.Atoms[9]);
            Assert.AreEqual(r2.Atoms[4], biphenyl.Atoms[10]);
            Assert.AreEqual(r2.Atoms[5], biphenyl.Atoms[11]);

            Assert.AreEqual(r2.Bonds[0], biphenyl.Bonds[7]);
            Assert.AreEqual(r2.Bonds[1], biphenyl.Bonds[8]);
            Assert.AreEqual(r2.Bonds[2], biphenyl.Bonds[9]);
            Assert.AreEqual(r2.Bonds[3], biphenyl.Bonds[10]);
            Assert.AreEqual(r2.Bonds[4], biphenyl.Bonds[11]);
            Assert.AreEqual(r2.Bonds[5], biphenyl.Bonds[12]);
        }

        [TestMethod()]
        public virtual void MarkAtomsAndBonds()
        {
            IAtomContainer biphenyl = MakeBiphenyl();
            Cycles.MarkRingAtomsAndBonds(biphenyl);
            int cyclicAtoms = 0;
            int cyclicBonds = 0;
            foreach (var atom in biphenyl.Atoms)
            {
                if (atom.IsInRing)
                    cyclicAtoms++;
            }
            foreach (var bond in biphenyl.Bonds)
            {
                if (bond.IsInRing)
                    cyclicBonds++;
            }
            Assert.AreEqual(cyclicAtoms, biphenyl.Atoms.Count);
            Assert.AreEqual(cyclicBonds, biphenyl.Bonds.Count - 1);
        }

        [TestMethod()]
        public virtual void Or()
        {
            ICycleFinder cf = Cycles.Or(Cycles.AllSimpleFinder, Cycles.GetAllFinder(3));
            IAtomContainer fullerene = GetFullerene();
            CheckSize(cf.Find(fullerene, fullerene.Atoms.Count), 120);
        }

        [TestMethod()]
        public virtual void Unchorded()
        {
            IAtomContainer container = TestMoleculeFactory.MakeAnthracene();
            CheckSize(Cycles.GetUnchorded(Cycles.AllSimpleFinder).Find(container), 3);
        }

        // load a boron fullerene
        private IAtomContainer GetFullerene()
        {
            string path = "NCDK.Data.MDL.boronBuckyBall.mol";
            MDLV2000Reader mdl = new MDLV2000Reader(ResourceLoader.GetAsStream(path));
            try
            {
                return mdl.Read(new AtomContainer());
            }
            finally
            {
                mdl.Close();
            }
        }

        static void CheckSize(Cycles cs, int nCycles)
        {
            Assert.AreEqual(nCycles, cs.GetNumberOfCycles());
        }
    }
}
