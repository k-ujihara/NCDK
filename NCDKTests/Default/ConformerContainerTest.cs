using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using NCDK.Numerics;
using NCDK.Common.Mathematics;

namespace NCDK.Default
{
    // @cdk.module test-data
    [TestClass()]
    public class ConformerContainerTest : CDKTestCase
    {
        private IAtomContainer base_;
        private IAtomContainer[] confs;

        private static int natom = 10;
        private static int nconfs = 20;

        private static Random rnd = new Random();

        private static IAtomContainer GetBaseAtomContainer(int natom, string title)
        {
            IAtomContainer container = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            container.SetProperty(CDKPropertyName.TITLE, title);
            for (int i = 0; i < natom; i++)
            {
                Vector3 coord = new Vector3();
                coord.X = rnd.NextDouble();
                coord.Y = rnd.NextDouble();
                coord.Z = rnd.NextDouble();

                IAtom atom = Default.ChemObjectBuilder.Instance.CreateAtom("C", coord);
                container.Atoms.Add(atom);
            }

            for (int i = 0; i < natom - 1; i++)
            {
                IAtom atom1 = container.Atoms[i];
                IAtom atom2 = container.Atoms[i + 1];
                IBond bond = Default.ChemObjectBuilder.Instance.CreateBond(atom1, atom2,
                        BondOrder.Single);
                container.Bonds.Add(bond);
            }
            return container;
        }

        private static IAtomContainer[] GetConformers(IAtomContainer base_, int nconf)
        {
            IAtomContainer[] ret = new IAtomContainer[nconf];
            for (int i = 0; i < nconf; i++)
            {
                for (int j = 0; j < base_.Atoms.Count; j++)
                {
                    Vector3 p = base_.Atoms[j].Point3D.Value;
                    p.X = rnd.NextDouble();
                    p.Y = rnd.NextDouble();
                    p.Z = rnd.NextDouble();
                    base_.Atoms[j].Point3D = p;
                }
                ret[i] = (IAtomContainer)base_.Clone();
            }
            return ret;
        }

        [TestInitialize()]
        public virtual void SetUp()
        {
            base_ = GetBaseAtomContainer(natom, "myMolecule");
            confs = GetConformers(base_, nconfs);
        }

        [TestMethod()]
        public virtual void TestConformerContainer()
        {
            ConformerContainer container = new ConformerContainer();
            Assert.IsNotNull(container);
            base_.SetProperty(CDKPropertyName.TITLE, "myMolecule");
            container.Add(base_);
            Assert.AreEqual(1, container.Count());

            foreach (var conf in confs)
                container.Add(conf);
            Assert.AreEqual(nconfs + 1, container.Count());
        }

        [TestMethod()]
        public virtual void TestConformerContainer_arrayIAtomContainer()
        {
            ConformerContainer container = new ConformerContainer(confs);
            Assert.IsNotNull(container);
            Assert.AreEqual(nconfs, container.Count());
        }

        [TestMethod()]
        public virtual void TestGetTitle()
        {
            ConformerContainer container = new ConformerContainer(confs);
            Assert.AreEqual("myMolecule", container.Title);
        }

        [TestMethod()]
        public virtual void TestIsEmpty()
        {
            ConformerContainer container = new ConformerContainer(confs);
            Assert.IsTrue(!container.IsEmpty);
        }

        [TestMethod()]
        public virtual void TestContains()
        {
            ConformerContainer container = new ConformerContainer(confs);
            IAtomContainer o = container[0];
            Assert.IsTrue(container.Contains(o));
        }

        [TestMethod()]
        public virtual void TestToArray()
        {
            ConformerContainer container = new ConformerContainer(confs);
            IAtomContainer[] array = (IAtomContainer[])container.ToArray();
            Assert.AreEqual(nconfs, array.Length);
        }

        [TestMethod()]
        public virtual void TestIterator()
        {
            ConformerContainer container = new ConformerContainer(confs);
            int nmol = 0;
            foreach (var atomContainer in container)
            {
                nmol++;
            }
            Assert.AreEqual(nconfs, nmol);
        }

        [TestMethod()]
        public virtual void TestIterator2()
        {
            ConformerContainer container = new ConformerContainer(confs);
            int nmol = 0;
            foreach (var conf in container)
            {
                nmol++;
            }
            Assert.AreEqual(nconfs, nmol);
        }

        [TestMethod()]
        public virtual void TestRemove_int()
        {
            ConformerContainer container = new ConformerContainer(confs);
            container.Clear();
            Assert.AreEqual(0, container.Count());

            for (int i = 0; i < nconfs; i++)
                container.Add(confs[i]);
            Assert.AreEqual(nconfs, container.Count());

            container.RemoveAt(0);
            Assert.AreEqual(nconfs - 1, container.Count());
        }

        [TestMethod()]
        public virtual void TestIndexOf_IAtomContainer()
        {
            ConformerContainer container = new ConformerContainer(confs);
            IAtomContainer ac = container[2];
            int index = container.IndexOf(ac);
            Assert.AreEqual(2, index);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public virtual void TestAdd_IAtomContainer()
        {
            ConformerContainer container = new ConformerContainer(confs);
            base_.SetProperty(CDKPropertyName.TITLE, "junk");
            container.Add(base_);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public virtual void TestGet_int()
        {
            ConformerContainer container = new ConformerContainer(confs);
            var dummy = container[100];
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public virtual void TestGet2()
        {
            ConformerContainer container = new ConformerContainer(confs);
            for (var i = 0; i < container.Count + 1; i++)
            {
                var dummy = container[i];
            }
        }

        [TestMethod()]
        public virtual void TestAdd_int_IAtomContainer()
        {
            ConformerContainer container = new ConformerContainer(confs);
            container.Insert(5, confs[5]);
        }

        [TestMethod()]
        public virtual void TestAdd_int_Object()
        {
            ConformerContainer container = new ConformerContainer(confs);
            container.Insert(5, confs[5]);
        }

        [TestMethod()]
        public virtual void TestAdd_Object()
        {
            ConformerContainer container = new ConformerContainer();
            Assert.IsNotNull(container);
            foreach (var conf in confs)
                container.Add(conf);
            Assert.AreEqual(nconfs, container.Count());
        }

        [TestMethod()]
        public virtual void TestIndexOf_Object()
        {
            ConformerContainer container = new ConformerContainer(confs);
            Assert.IsNotNull(container);

            int counter = 0;
            foreach (var conf in confs)
            {
                Assert.AreEqual(counter, container.IndexOf(conf));
                counter++;
            }
        }

        [TestMethod()]
        public virtual void TestClear()
        {
            ConformerContainer container = new ConformerContainer(confs);
            Assert.AreEqual(nconfs, container.Count());
            container.Clear();
            Assert.AreEqual(0, container.Count());
        }

        [TestMethod()]
        public virtual void TestSize()
        {
            ConformerContainer container = new ConformerContainer(confs);
            Assert.AreEqual(nconfs, container.Count());
        }

        [TestMethod()]
        public virtual void TestLastIndexOf_Object()
        {
            ConformerContainer container = new ConformerContainer(confs);
            Assert.AreEqual(nconfs, container.Count());
            int x = container.LastIndexOf(container[3]);
            Assert.AreEqual(3, container.LastIndexOf(container[3]));
        }

        [TestMethod()]
        public virtual void TestContains_Object()
        {
            ConformerContainer container = new ConformerContainer(confs);
            Assert.AreEqual(nconfs, container.Count());
            Assert.IsTrue(container.Contains(container[3]));
        }

        [TestMethod()]
        public virtual void TestAddAll_Collection()
        {
            ConformerContainer container = new ConformerContainer(confs);
            Assert.AreEqual(nconfs, container.Count());
            Assert.IsTrue(container.Contains(container[3]));
        }

        //[TestMethod()][ExpectedException(typeof(InvalidOperationException))]
        //public virtual void TestAddAll_int_Collection() {
        //    ConformerContainer container = new ConformerContainer(confs);
        //    container.AddAll(5, null);
        //}

        //[TestMethod()]
        //[ExpectedException(typeof(InvalidOperationException))]
        //public virtual void TestToArray_arrayObject()
        //{
        //    ConformerContainer container = new ConformerContainer(confs);
        //    container.ToArray();
        //}

        [TestMethod()]
        public virtual void TestRemove_Object()
        {
            ConformerContainer cContainer = new ConformerContainer(confs);
            Assert.AreEqual(nconfs, cContainer.Count());
            IAtomContainer container = cContainer[3];
            Assert.IsTrue(cContainer.Contains(container));
            cContainer.Remove(container);
            Assert.AreEqual(nconfs - 1, cContainer.Count());
            Assert.IsFalse(cContainer.Contains(container));
        }

        [TestMethod()]
        public virtual void TestSet_int_IAtomContainer()
        {
            ConformerContainer container = new ConformerContainer(confs);
            int location = 5;
            container.Set(location, container[location + 1]);
            Assert.AreEqual(location, container.IndexOf(container[location + 1]));
        }

        /// <summary>
        /// note that AreNotSame checks whether object A *refers* to the same as
        /// object B since a ConformerContainer.get always returns the sme
        /// IAtomContainer object (just changing the 3D coords for a given conformer)
        /// the following will always fail
        /// Assert.AreNotSame(container[location+1], container[location]);
        /// Sinmilarly: Assert.AreEqual(container[location+1],
        /// container[location]); since this will check whether the references of
        /// X & Y are the same. Since Get() returns the same atom container (with the
        /// coords chanegd), there is no difference in the reference returned. Thus
        /// this test is always the same. Better to check set position X to X+1 and
        /// then find the first occurence of the object originally at X+1 - it should
        /// now be X
        /// </summary>
        [TestMethod()]
        public virtual void TestSet_int_Object()
        {
            ConformerContainer container = new ConformerContainer(confs);
            int location = 5;
            container.Set(location, container[location + 1]);
            Assert.AreEqual(location, container.IndexOf(container[location + 1]));
        }

        [TestMethod()]
        public virtual void TestContainsAll_Collection()
        {
            ConformerContainer container = new ConformerContainer(confs);
            Assert.IsNotNull(container);
            Assert.AreEqual(nconfs, container.Count());
            foreach (var e in container)
                Assert.IsTrue(container.Contains(e));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public virtual void TestRemoveAll_Collection()
        {
            ConformerContainer container = new ConformerContainer(confs);
            Assert.IsNotNull(container);
            Assert.AreEqual(nconfs, container.Count());

            foreach (var e in container)
                container.Remove(e);
            Assert.AreEqual(0, container.Count());
        }

        [TestMethod()]
        public virtual void TestConformerContainer_IAtomContainer()
        {
            ConformerContainer container = new ConformerContainer(base_);
            Assert.IsNotNull(container);
            Assert.AreEqual(1, container.Count());
        }
    }
}
