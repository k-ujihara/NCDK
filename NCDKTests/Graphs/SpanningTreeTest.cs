/* Copyright (C) 2007  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.IO;
using NCDK.Templates;

namespace NCDK.Graphs
{
    // @cdk.module test-core
    [TestClass()]
    public class SpanningTreeTest : CDKTestCase
    {
        private static SpanningTree azulene = null;
        private static SpanningTree ethane = null;

        public SpanningTreeTest()
        {
            if (azulene == null)
            {
                // load azulene
                string filename = "NCDK.Data.MDL.azulene.mol";
                var ins = ResourceLoader.GetAsStream(filename);
                MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
                IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
                IChemSequence seq = chemFile[0];
                IChemModel model = seq[0];
                IAtomContainer azuleneMolecule = model.MoleculeSet[0];
                Assert.AreEqual(10, azuleneMolecule.Atoms.Count);
                Assert.AreEqual(11, azuleneMolecule.Bonds.Count);
                azulene = new SpanningTree(azuleneMolecule);
            }
            if (ethane == null)
            {
                // create ethane
                IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
                IAtomContainer ethaneMolecule = builder.CreateAtomContainer();
                ethaneMolecule.Add(builder.CreateAtom("C"));
                ethaneMolecule.Add(builder.CreateAtom("C"));
                ethaneMolecule.AddBond(ethaneMolecule.Atoms[0], ethaneMolecule.Atoms[1], BondOrder.Single);
                ethane = new SpanningTree(ethaneMolecule);
            }
        }

        [TestMethod()]
        public virtual void TestSpanningTree_IAtomContainer()
        {
            SpanningTree sTree = new SpanningTree(new AtomContainer());
            Assert.IsNotNull(sTree);
        }

        [TestMethod()]
        public virtual void TestGetCyclicFragmentsContainer()
        {
            IAtomContainer ringSystems = azulene.GetCyclicFragmentsContainer();
            Assert.AreEqual(10, ringSystems.Atoms.Count);
            Assert.AreEqual(11, ringSystems.Bonds.Count);
        }

        [TestMethod()]
        public virtual void TestGetBondsCyclicCount()
        {
            Assert.AreEqual(11, azulene.GetBondsCyclicCount());
            Assert.AreEqual(0, ethane.GetBondsCyclicCount());
        }

        [TestMethod()]
        public virtual void TestGetBondsAcyclicCount()
        {
            Assert.AreEqual(0, azulene.GetBondsAcyclicCount());
            Assert.AreEqual(1, ethane.GetBondsAcyclicCount());
        }

        [TestMethod()]
        public virtual void TestGetPath_IAtomContainer_IAtom_IAtom()
        {
            IAtomContainer ethaneMol = ethane.GetSpanningTree();
            IAtomContainer path = ethane.GetPath(ethaneMol, ethaneMol.Atoms[0], ethaneMol.Atoms[1]);
            Assert.AreEqual(2, path.Atoms.Count);
            Assert.AreEqual(1, path.Bonds.Count);

            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer disconnectedStructure = builder.CreateAtomContainer();
            disconnectedStructure.Add(builder.CreateAtom("Na"));
            disconnectedStructure.Atoms[0].FormalCharge = +1;
            disconnectedStructure.Add(builder.CreateAtom("Cl"));
            disconnectedStructure.Atoms[1].FormalCharge = -1;
            path = ethane
                    .GetPath(disconnectedStructure, disconnectedStructure.Atoms[0], disconnectedStructure.Atoms[1]);
            Assert.IsNotNull(path);
            Assert.AreEqual(0, path.Atoms.Count);
            Assert.AreEqual(0, path.Bonds.Count);
        }

        [TestMethod()]
        public virtual void TestIsDisconnected()
        {
            Assert.IsFalse(azulene.IsDisconnected);

            IChemObjectBuilder builder = azulene.GetSpanningTree().Builder;
            IAtomContainer disconnectedStructure = builder.CreateAtomContainer();
            disconnectedStructure.Add(builder.CreateAtom("Na"));
            disconnectedStructure.Atoms[0].FormalCharge = +1;
            disconnectedStructure.Add(builder.CreateAtom("Cl"));
            disconnectedStructure.Atoms[1].FormalCharge = -1;
            SpanningTree stree = new SpanningTree(disconnectedStructure);
            Assert.IsTrue(stree.IsDisconnected);
        }

        [TestMethod()]
        public virtual void TestGetSpanningTree()
        {
            IAtomContainer container = azulene.GetSpanningTree();
            Assert.AreEqual(10, container.Atoms.Count);
            Assert.AreEqual(9, container.Bonds.Count); // two rings to be broken to make a tree

            container = ethane.GetSpanningTree();
            Assert.AreEqual(2, container.Atoms.Count);
            Assert.AreEqual(1, container.Bonds.Count);
        }

        [TestMethod()]
        public virtual void TestGetBasicRings()
        {
            IRingSet ringSet = azulene.GetBasicRings();
            Assert.AreEqual(2, ringSet.Count);

            ringSet = ethane.GetBasicRings();
            Assert.AreEqual(0, ringSet.Count);
        }

        [TestMethod()]
        public virtual void TestGetAllRings()
        {
            IRingSet ringSet = azulene.GetAllRings();
            Assert.AreEqual(3, ringSet.Count);

            ringSet = ethane.GetAllRings();
            Assert.AreEqual(0, ringSet.Count);
        }

        [TestMethod()]
        public virtual void TestGetSpanningTreeSize()
        {
            Assert.AreEqual(9, azulene.GetSpanningTreeSize());
            Assert.AreEqual(1, ethane.GetSpanningTreeSize());
        }

        [TestMethod()]
        public virtual void TestGetSpanningTreeForPyridine()
        {
            IAtomContainer mol = TestMoleculeFactory.MakePyridine();
            SpanningTree spanningTree = new SpanningTree(mol);
            Assert.AreEqual(6, spanningTree.GetBondsCyclicCount());
            Assert.AreEqual(6, spanningTree.GetCyclicFragmentsContainer().Atoms.Count);
            Assert.AreEqual(0, spanningTree.GetBondsAcyclicCount());
        }
    }
}
