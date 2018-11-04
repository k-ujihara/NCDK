/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.Common.Collections;
using NCDK.IO;
using NCDK.Silent;
using NCDK.Smiles;
using NCDK.Templates;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Graphs
{
    [TestClass()]
    public class PathToolsTest : CDKTestCase
    {
        private static IAtomContainer molecule = TestMoleculeFactory.MakeAlphaPinene();
        private static SmilesParser sp = CDK.SmilesParser;

        [TestMethod()]
        public virtual void TestBreadthFirstTargetSearch_IAtomContainer_List_IAtom_int_int()
        {
            IAtom atom1 = molecule.Atoms[0];
            IAtom atom2 = molecule.Atoms[8];
            List<IAtom> sphere = new List<IAtom>
            {
                atom1
            };
            int length = PathTools.BreadthFirstTargetSearch(molecule, sphere, atom2, 0, 3);
            Assert.AreEqual(3, length);
        }

        [TestMethod()]
        public virtual void TestReSetFlags_IAtomContainer()
        {
            IAtomContainer atomContainer = new AtomContainer();
            IAtom atom1 = new Atom("C")
            {
                IsVisited = true
            };
            IAtom atom2 = new Atom("C")
            {
                IsVisited = true
            };
            IBond bond1 = new Bond(atom1, atom2, BondOrder.Single);
            atomContainer.Atoms.Add(atom1);
            atomContainer.Atoms.Add(atom2);
            atomContainer.Bonds.Add(bond1);

            PathTools.ResetFlags(atomContainer);

            // now assume that no VISITED is set
            IEnumerator<IAtom> atoms = atomContainer.Atoms.GetEnumerator();
            while (atoms.MoveNext())
            {
                Assert.IsFalse(atoms.Current.IsVisited);
            }
            IEnumerator<IBond> bonds = atomContainer.Bonds.GetEnumerator();
            while (bonds.MoveNext())
            {
                Assert.IsFalse(bonds.Current.IsVisited);
            }
        }

        [TestMethod()]
        public virtual void TestGetShortestPath_IAtomContainer_IAtom_IAtom()
        {
            IAtomContainer atomContainer = null;
            IAtom start = null;
            IAtom end = null;
            var sp = CDK.SmilesParser;
            atomContainer = sp.ParseSmiles("CCCC");
            start = atomContainer.Atoms[0];
            end = atomContainer.Atoms[3];
            var path = PathTools.GetShortestPath(atomContainer, start, end);
            Assert.AreEqual(4, path.Count);

            atomContainer = sp.ParseSmiles("CC(N)CC");
            start = atomContainer.Atoms[0];
            end = atomContainer.Atoms[2];
            path = PathTools.GetShortestPath(atomContainer, start, end);
            Assert.AreEqual(3, path.Count);

            atomContainer = sp.ParseSmiles("C1C(N)CC1");
            start = atomContainer.Atoms[0];
            end = atomContainer.Atoms[2];
            path = PathTools.GetShortestPath(atomContainer, start, end);
            Assert.AreEqual(3, path.Count);
        }

        [TestMethod()]
        public virtual void TestGetShortestPath_Middle()
        {
            string filename = "NCDK.Data.MDL.shortest_path_test.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer testMolecule = new AtomContainer();
            reader.Read(testMolecule);

            var path = PathTools.GetShortestPath(testMolecule, testMolecule.Atoms[0], testMolecule.Atoms[9]);
            Assert.AreEqual(10, path.Count);

            path = PathTools.GetShortestPath(testMolecule, testMolecule.Atoms[1], testMolecule.Atoms[9]);
            Assert.AreEqual(9, path.Count);

            path = PathTools.GetShortestPath(testMolecule, testMolecule.Atoms[9], testMolecule.Atoms[0]);
            Assert.AreEqual(10, path.Count);
        }

        [TestMethod()]
        public virtual void TestGetPathsOfLength_IAtomContainer_IAtom_int()
        {
            IAtomContainer atomContainer = null;
            IAtom start = null;
            IEnumerable<IList<IAtom>> paths = null;
            atomContainer = sp.ParseSmiles("c1cc2ccccc2cc1");
            start = atomContainer.Atoms[0];
            paths = PathTools.GetPathsOfLength(atomContainer, start, 1);
            Assert.AreEqual(2, paths.Count());

            atomContainer = sp.ParseSmiles("Cc1cc2ccccc2cc1");
            start = atomContainer.Atoms[0];
            paths = PathTools.GetPathsOfLength(atomContainer, start, 1);
            Assert.AreEqual(1, paths.Count());
        }

        [TestMethod()]
        public virtual void TestGetAllPaths_IAtomContainer_IAtom_IAtom()
        {
            var atomContainer = sp.ParseSmiles("c12ccccc1cccc2");

            IAtom start = atomContainer.Atoms[0];
            IAtom end = atomContainer.Atoms[2];
            IEnumerable<IList<IAtom>> paths = PathTools.GetAllPaths(atomContainer, start, end);

            Assert.AreEqual(3, paths.Count());

            IList<IAtom> path1 = paths.ElementAt(0);
            IList<IAtom> path2 = paths.ElementAt(1);
            IList<IAtom> path3 = paths.ElementAt(2);

            Assert.AreEqual(start, path1[0]);
            Assert.AreEqual(atomContainer.Atoms[1], path1[1]);
            Assert.AreEqual(end, path1[2]);

            Assert.AreEqual(start, path2[0]);
            Assert.AreEqual(atomContainer.Atoms[5], path2[1]);
            Assert.AreEqual(atomContainer.Atoms[4], path2[2]);
            Assert.AreEqual(atomContainer.Atoms[3], path2[3]);
            Assert.AreEqual(end, path2[4]);
            Assert.IsNotNull(path3);
        }

        [TestMethod()]
        public virtual void TestGetVertexCountAtDistance_IAtomContainer_int()
        {
            var atomContainer = sp.ParseSmiles("c12ccccc1cccc2");
            Assert.AreEqual(11, PathTools.GetVertexCountAtDistance(atomContainer, 1));
            Assert.AreEqual(14, PathTools.GetVertexCountAtDistance(atomContainer, 2));
        }

        [TestMethod()]
        public virtual void TestGetInt2DColumnSum_arrayintint()
        {
            int[][] start = Arrays.CreateJagged<int>(2, 2);
            start[0][0] = 5;
            start[0][1] = 3;
            start[1][0] = 1;
            start[1][1] = 2;

            Assert.AreEqual(8, PathTools.GetInt2DColumnSum(start)[0]);
            Assert.AreEqual(3, PathTools.GetInt2DColumnSum(start)[1]);
        }

        [TestMethod()]
        public virtual void TestGetMolecularGraphRadius_IAtomContainer()
        {
            var atomContainer = sp.ParseSmiles("CCCC");
            Assert.AreEqual(2, PathTools.GetMolecularGraphRadius(atomContainer));
            atomContainer = sp.ParseSmiles("C1C(N)CC1");
            Assert.AreEqual(2, PathTools.GetMolecularGraphRadius(atomContainer));
            atomContainer = sp.ParseSmiles("c12ccccc1cccc2");
            Assert.AreEqual(3, PathTools.GetMolecularGraphRadius(atomContainer));
        }

        [TestMethod()]
        public virtual void TestGetMolecularGraphDiameter_IAtomContainer()
        {
            var atomContainer = sp.ParseSmiles("CCCC");
            Assert.AreEqual(3, PathTools.GetMolecularGraphDiameter(atomContainer));
            atomContainer = sp.ParseSmiles("C1C(N)CC1");
            Assert.AreEqual(3, PathTools.GetMolecularGraphDiameter(atomContainer));
            atomContainer = sp.ParseSmiles("c12ccccc1cccc2");
            Assert.AreEqual(5, PathTools.GetMolecularGraphDiameter(atomContainer));
        }

        [TestMethod()]
        public virtual void TestComputeFloydAPSP_arrayintint()
        {
            int[][] start = Arrays.CreateJagged<int>(5, 5); // default to all zeros
            start[0][1] = 1;
            start[1][2] = 1;
            start[1][4] = 1;
            start[3][4] = 1;
            start[1][0] = 1;
            start[2][1] = 1;
            start[4][1] = 1;
            start[4][3] = 1;

            int[][] floydAPSP = PathTools.ComputeFloydAPSP(start);
            Assert.AreEqual(5, floydAPSP.Length);
            Assert.AreEqual(5, floydAPSP[0].Length);

            Assert.AreEqual(1, floydAPSP[0][1]);
            Assert.AreEqual(2, floydAPSP[0][2]);
            Assert.AreEqual(3, floydAPSP[0][3]);
            Assert.AreEqual(2, floydAPSP[0][4]);
            Assert.AreEqual(1, floydAPSP[1][2]);
            Assert.AreEqual(2, floydAPSP[1][3]);
            Assert.AreEqual(1, floydAPSP[1][4]);
            Assert.AreEqual(3, floydAPSP[2][3]);
            Assert.AreEqual(2, floydAPSP[2][4]);
            Assert.AreEqual(1, floydAPSP[3][4]);
        }

        [TestMethod()]
        public virtual void TestComputeFloydAPSP_arrayDoubledouble()
        {
            double[][] start = Arrays.CreateJagged<double>(5, 5); // default to all zeros
            start[0][1] = 1.0;
            start[1][2] = 1.0;
            start[1][4] = 2.0;
            start[3][4] = 1.0;
            start[1][0] = 1.0;
            start[2][1] = 1.0;
            start[4][1] = 2.0;
            start[4][3] = 1.0;

            int[][] floydAPSP = PathTools.ComputeFloydAPSP(start);
            Assert.AreEqual(5, floydAPSP.Length);
            Assert.AreEqual(5, floydAPSP[0].Length);

            Assert.AreEqual(1, floydAPSP[0][1]);
            Assert.AreEqual(2, floydAPSP[0][2]);
            Assert.AreEqual(3, floydAPSP[0][3]);
            Assert.AreEqual(2, floydAPSP[0][4]);
            Assert.AreEqual(1, floydAPSP[1][2]);
            Assert.AreEqual(2, floydAPSP[1][3]);
            Assert.AreEqual(1, floydAPSP[1][4]);
            Assert.AreEqual(3, floydAPSP[2][3]);
            Assert.AreEqual(2, floydAPSP[2][4]);
            Assert.AreEqual(1, floydAPSP[3][4]);
        }

        [TestMethod()]
        public virtual void TestDepthFirstTargetSearch_IAtomContainer_IAtom_IAtom_IAtomContainer()
        {
            var molecule = sp.ParseSmiles("C(COF)(Br)NC");
            foreach (var atom in molecule.Atoms)
                atom.IsVisited = false;

            IAtomContainer paths = ChemObjectBuilder.Instance.NewAtomContainer();
            IAtom root = molecule.Atoms[0];
            IAtom target = null;

            foreach (var atom in molecule.Atoms)
            {
                if (atom.Symbol.Equals("F"))
                {
                    target = atom;
                }
            }

            bool status = PathTools.DepthFirstTargetSearch(molecule, root, target, paths);
            Assert.IsTrue(status);
            Assert.AreEqual(3, paths.Atoms.Count);
            Assert.AreEqual(target, paths.Atoms[2]);
        }

        [TestMethod()]
        public virtual void TestBreadthFirstSearch_IAtomContainer_List_IAtomContainer()
        {
            IAtomContainer atomContainer;
            IAtom start;
            atomContainer = sp.ParseSmiles("CCCC");
            PathTools.ResetFlags(atomContainer);
            start = atomContainer.Atoms[0];
            List<IAtom> sphere = new List<IAtom>
            {
                start
            };
            IAtomContainer result = atomContainer.Builder.NewAtomContainer();
            PathTools.BreadthFirstSearch(atomContainer, sphere, result);
            Assert.AreEqual(4, result.Atoms.Count);
        }

        [TestMethod()]
        public virtual void TestBreadthFirstSearch_IAtomContainer_List_IAtomContainer_int()
        {
            IAtomContainer atomContainer;
            IAtom start;
            atomContainer = sp.ParseSmiles("CCCC");
            PathTools.ResetFlags(atomContainer);
            start = atomContainer.Atoms[0];
            List<IAtom> sphere = new List<IAtom>
            {
                start
            };
            IAtomContainer result = atomContainer.Builder.NewAtomContainer();
            PathTools.BreadthFirstSearch(atomContainer, sphere, result, 1);
            Assert.AreEqual(2, result.Atoms.Count);

            result = atomContainer.Builder.NewAtomContainer();
            PathTools.ResetFlags(atomContainer);
            PathTools.BreadthFirstSearch(atomContainer, sphere, result, 2);
            Assert.AreEqual(3, result.Atoms.Count);

            result = atomContainer.Builder.NewAtomContainer();
            PathTools.ResetFlags(atomContainer);
            PathTools.BreadthFirstSearch(atomContainer, sphere, result, 3);
            Assert.AreEqual(4, result.Atoms.Count);
        }

        [TestMethod()]
        public virtual void TestFindClosestByBond()
        {
            var container = sp.ParseSmiles("CCN(CSCP)CCCOF");
            IAtom queryAtom = null;
            foreach (var atom in container.Atoms)
            {
                if (atom.Symbol.Equals("N"))
                {
                    queryAtom = atom;
                    break;
                }
            }
            var closestAtoms = PathTools.FindClosestByBond(container, queryAtom, 2);
            foreach (var atom in closestAtoms)
            {
                Assert.AreEqual("C", atom.Symbol);
            }
        }

        [TestMethod()]
        public virtual void TestGetPathsOfLengthUpto()
        {
            var container = sp.ParseSmiles("CCCC");
            IEnumerable<IList<IAtom>> paths = PathTools.GetPathsOfLengthUpto(container, container.Atoms[0], 2);
            Assert.AreEqual(3, paths.Count());

            container = sp.ParseSmiles("C(C)CCC");
            paths = PathTools.GetPathsOfLengthUpto(container, container.Atoms[0], 2);
            Assert.AreEqual(4, paths.Count());
        }

        [TestMethod()]
        public virtual void TestGetLimitedPathsOfLengthUpto()
        {
            var container = sp.ParseSmiles("CCCC");
            IEnumerable<IList<IAtom>> paths = PathTools.GetPathsOfLengthUpto(container, container.Atoms[0], 2);
            Assert.AreEqual(3, paths.Count());

            container = sp.ParseSmiles("C(C)CCC");
            paths = PathTools.GetPathsOfLengthUpto(container, container.Atoms[0], 2);
            Assert.AreEqual(4, paths.Count());
        }

        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        public virtual void TestGetLimitedPathsOfLengthUpto_Exception()
        {
            IAtomContainer container = sp
                    .ParseSmiles("[B]1234[B]567[B]89%10[B]%11%12%13[B]%14%15%16[B]11([B]%17%18%19[B]%20%21%22[B]22%23[B]%24%25%26[B]%27%28%29[B]55([B]%30%31%32[B]88%33[B]%34%35%36[B]%37%38%39[B]%11%11([B]%40%41%42[B]%14%14%43[B]%44%45%46[B]%17%17([B]%47%48%49[B]%50%51%52[B]%20%20([B]%53%54%55[B]%24%24([B]%56%57%58[B]%27%27%59[B]%60%61%62[B]%30%30([B]%63%64%65[B]%34%34([B]%66%67%68[B]%37%37%69[B]%70%71%72[B]%40%40([B]%73%74%75[B]%44%44([B]%47%47%76[B]%77%78%79[B]%80%81%82[B]%50%50([B]%53%53%83[B]%84%85%86[B]%56%56([B]%87%88%89[B]%60%60([B]%63%63%90[B]%91%92%93[B]%66%66([B]%94%95%96[B]%70%70([B]%73%73%97[B]%77%77([B]%98%99%100[B]%80%80%101[B]%84%84([B]%87%87%102[B]%91%91([B]%94%98([B]%95%70%73%77%99)[B]%100%80%84%87%91)[B]%88%60%63%92%102)[B]%81%50%53%85%101)[B]%74%44%47%78%97)[B]%67%37%71%66%96)[B]%64%34%68%90%93)[B]%57%27%61%56%89)[B]%54%24%58%83%86)[B]%48%51%76%79%82)[B]%41%14%45%40%75)[B]%38%11%42%69%72)[B]%318%35%30%65)[B]%285%32%59%62)[B]%212%25%20%55)[B]%18%22%17%49%52)[B]%151%19%43%46)[B]9%12%33%36%39)[B]36%23%26%29)[B]47%10%13%16");
            PathTools.GetLimitedPathsOfLengthUpto(container, container.Atoms[0], 8, 150);
        }
    }
}
