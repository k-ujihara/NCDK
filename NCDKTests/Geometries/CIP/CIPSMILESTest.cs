/* Copyright (C) 2010  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Smiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Geometries.CIP
{
    // @cdk.module test-cip
    [TestClass()]
    public class CIPSMILESTest : CDKTestCase
    {
        static readonly SmilesParser smiles = CDK.SmilesParser;
        static readonly IChemObjectBuilder bldr = CDK.Builder;
        static readonly SmilesParser smipar = new SmilesParser(bldr);

        [TestMethod()]
        public void Test()
        {
            var molecule = smiles.ParseSmiles("ClC(Br)(I)[H]");
            LigancyFourChirality chirality = CIPTool.DefineLigancyFourChirality(molecule, 1, 4, 0, 2, 3, TetrahedralStereo.Clockwise);
            Assert.AreEqual(CIPTool.CIPChirality.R, CIPTool.GetCIPChirality(chirality));
        }

        /// <summary>
        /// Test case that tests sequence recursing of the atomic number rule.
        /// </summary>
        /// <seealso cref="Test2methylbutanolS"/>
        // @cdk.inchi InChI=1S/C5H12O/c1-3-5(2)4-6/h5-6H,3-4H2,1-2H3/t5-/m1/s1
        [TestMethod()]
        public void Test2methylbutanolR()
        {
            var molecule = smiles.ParseSmiles("OCC([H])(C)CC");
            LigancyFourChirality chirality = CIPTool.DefineLigancyFourChirality(molecule, 2, 3, 1, 4, 5, TetrahedralStereo.Clockwise);
            Assert.AreEqual(CIPTool.CIPChirality.R, CIPTool.GetCIPChirality(chirality));
        }

        /// <summary>
        /// Test case that tests sequence recursing of the atomic number rule.
        /// </summary>
        /// <seealso cref="Test2methylbutanolR"/>
        // @cdk.inchi InChI=1S/C5H12O/c1-3-5(2)4-6/h5-6H,3-4H2,1-2H3/t5-/m0/s1
        [TestMethod()]
        public void Test2methylbutanolS()
        {
            var molecule = smiles.ParseSmiles("OCC([H])(C)CC");
            LigancyFourChirality chirality = CIPTool.DefineLigancyFourChirality(molecule, 2, 3, 1, 4, 5,
                    TetrahedralStereo.AntiClockwise);
            Assert.AreEqual(CIPTool.CIPChirality.S, CIPTool.GetCIPChirality(chirality));
        }

        [TestMethod()]
        public void TestTwoVersusDoubleBondedOxygenR()
        {
            var molecule = smiles.ParseSmiles("OC(O)C([H])(C)C=O");
            LigancyFourChirality chirality = CIPTool.DefineLigancyFourChirality(molecule, 3, 4, 5, 1, 6, TetrahedralStereo.Clockwise);
            Assert.AreEqual(CIPTool.CIPChirality.R, CIPTool.GetCIPChirality(chirality));
        }

        [TestMethod()]
        public void TestTwoVersusDoubleBondedOxygenS()
        {
            var molecule = smiles.ParseSmiles("OC(O)C([H])(C)C=O");
            LigancyFourChirality chirality = CIPTool.DefineLigancyFourChirality(molecule, 3, 4, 5, 1, 6,
                    TetrahedralStereo.AntiClockwise);
            Assert.AreEqual(CIPTool.CIPChirality.S, CIPTool.GetCIPChirality(chirality));
        }

        [TestMethod()]
        public void TestImplicitHydrogen()
        {
            var molecule = smiles.ParseSmiles("CCC(C)CCC");
            LigancyFourChirality chirality = CIPTool.DefineLigancyFourChirality(molecule, 2, CIPTool.Hydrogen, 3, 1, 4,
                    TetrahedralStereo.AntiClockwise);
            Assert.AreEqual(CIPTool.CIPChirality.S, CIPTool.GetCIPChirality(chirality));
        }

        [TestMethod()]
        [Timeout(5000)]
        // 5 seconds should be enough
        public void TestTermination()
        {
            IAtomContainer mol = smiles
                    .ParseSmiles("[H]O[C@]([H])(C1([H])(C([H])([H])C([H])([H])C1([H])([H])))C2([H])(C([H])([H])C2([H])([H]))");
            var stereoElements = mol.StereoElements;
            var stereo = stereoElements.First();
            Assert.IsNotNull(stereo);
            Assert.IsTrue(stereo is ITetrahedralChirality);
            CIPTool.GetCIPChirality(mol, (ITetrahedralChirality)stereo);
        }

        [TestMethod()]
        [Timeout(5000)]
        // 5 seconds should be enough
        public void TestTermination2()
        {
            var mol = smiles.ParseSmiles("OC1CCC[C@](F)(CC1)Cl");
            var stereoElements = mol.StereoElements;
            var stereo = stereoElements.First();
            Assert.IsNotNull(stereo);
            Assert.IsTrue(stereo is ITetrahedralChirality);
            CIPTool.GetCIPChirality(mol, (ITetrahedralChirality)stereo);
        }

        [TestMethod()]
        public void TestTetraHalogenMethane()
        {
            var molecule = smiles.ParseSmiles("FC(Br)(Cl)I");
            LigancyFourChirality chirality = CIPTool.DefineLigancyFourChirality(molecule, 1, 0, 4, 2, 3,
                    TetrahedralStereo.AntiClockwise);
            Assert.AreEqual(CIPTool.CIPChirality.R, CIPTool.GetCIPChirality(chirality));
        }

        /**
         * @cdk.inchi InChI=1S/C20H20BrN3O3S/c1-23(2)9-10-24(20-22-14-8-7-13(21)11-18(14)28-20)19(25)17-12-26-15-5-3-4-6-16(15)27-17/h3-8,11,17H,9-10,12H2,1-2H3/p+1/t17-/m1/s1
         */
        [TestMethod()]
        public void TestCID42475007R()
        {
            var mol = smiles.ParseSmiles("C[NH+](C)CCN(C1=NC2=C(S1)C=C(C=C2)Br)C(=O)[C@H]3COC4=CC=CC=C4O3");
            var stereoElements = mol.StereoElements;
            var stereo = stereoElements.First();
            Assert.IsNotNull(stereo);
            Assert.IsTrue(stereo is ITetrahedralChirality);
            Assert.AreEqual(CIPTool.CIPChirality.R, CIPTool.GetCIPChirality(mol, (ITetrahedralChirality)stereo));
        }

        // @cdk.inchi InChI=1S/C20H20BrN3O3S/c1-23(2)9-10-24(20-22-14-8-7-13(21)11-18(14)28-20)19(25)17-12-26-15-5-3-4-6-16(15)27-17/h3-8,11,17H,9-10,12H2,1-2H3/p+1/t17+/m1/s1
        [TestMethod()]
        public void TestCID42475007S()
        {
            var mol = smiles.ParseSmiles("C[NH+](C)CCN(C1=NC2=C(S1)C=C(C=C2)Br)C(=O)[C@@H]3COC4=CC=CC=C4O3");
            var stereoElements = mol.StereoElements;
            var stereo = stereoElements.First();
            Assert.IsNotNull(stereo);
            Assert.IsTrue(stereo is ITetrahedralChirality);
            Assert.AreEqual(CIPTool.CIPChirality.S, CIPTool.GetCIPChirality(mol, (ITetrahedralChirality)stereo));
        }

        // @cdk.inchi InChI=1/C4H10OS/c1-3-4-6(2)5/h3-4H2,1-2H3/t6+/s2
        [TestMethod()]
        public void RSulfinyl()
        {
            var mol = smiles.ParseSmiles("CCC[S@@](C)=O");
            var stereoElements = mol.StereoElements;
            var stereo = stereoElements.First();
            Assert.IsNotNull(stereo);
            Assert.IsTrue(stereo is ITetrahedralChirality);
            Assert.AreEqual(CIPTool.CIPChirality.R, CIPTool.GetCIPChirality(mol, (ITetrahedralChirality)stereo));
        }

        // @cdk.inchi InChI=1/C4H10OS/c1-3-4-6(2)5/h3-4H2,1-2H3/t6-/s2
        [TestMethod()]
        public void SSulfinyl()
        {
            var mol = smiles.ParseSmiles("CCC[S@](C)=O");
            var stereoElements = mol.StereoElements;
            var stereo = stereoElements.First();
            Assert.IsNotNull(stereo);
            Assert.IsTrue(stereo is ITetrahedralChirality);
            Assert.AreEqual(CIPTool.CIPChirality.S, CIPTool.GetCIPChirality(mol, (ITetrahedralChirality)stereo));
        }

        [TestMethod()]
        public void EButene()
        {
            Assert.AreEqual(CIPTool.CIPChirality.E, Label("C/C=C/C"));
            Assert.AreEqual(CIPTool.CIPChirality.E, Label("C/C=C/C"));
            Assert.AreEqual(CIPTool.CIPChirality.E, Label("C\\C=C\\C"));
        }

        [TestMethod()]
        public void ZButene()
        {
            Assert.AreEqual(CIPTool.CIPChirality.Z, Label("C/C=C\\C"));
            Assert.AreEqual(CIPTool.CIPChirality.Z, Label("C\\C=C/C"));
        }

        [TestMethod()]
        public void None()
        {
            Assert.AreEqual(CIPTool.CIPChirality.None, Label("C/C=C(/C)C"));
            Assert.AreEqual(CIPTool.CIPChirality.None, Label("C/C(C)=C/C"));
        }

        [TestMethod()]
        public void EDepth2()
        {
            Assert.AreEqual(CIPTool.CIPChirality.E, Label("CC/C(CO)=C(/CC)CO"));
            Assert.AreEqual(CIPTool.CIPChirality.E, Label("OC\\C(CC)=C(/CC)CO"));
        }

        [TestMethod()]
        public void ZDepth2()
        {
            Assert.AreEqual(CIPTool.CIPChirality.Z, Label("CC\\C(CO)=C(/CC)CO"));
            Assert.AreEqual(CIPTool.CIPChirality.Z, Label("OC/C(CC)=C(/CC)CO"));
        }

        [TestMethod()]
        public void OneSizeDepth2()
        {
            Assert.AreEqual(CIPTool.CIPChirality.E, Label("CC\\C(CO)=C(/C)"));
        }

        [TestMethod()]
        public void NoneDepth2()
        {
            Assert.AreEqual(CIPTool.CIPChirality.None, Label("CC/C(CC)=C(/CC)CO"));
        }

        /// <summary>
        /// Get the CIP labelling for a container with a single stereo element.
        /// </summary>
        /// <param name="smi">input smiles</param>
        /// <returns>the labelling</returns>
        CIPTool.CIPChirality Label(string smi)
        {
            return Label(smipar.ParseSmiles(smi));
        }

        /// <summary>
        /// Get the CIP labelling for a container with a single stereo element.
        /// </summary>
        /// <param name="container">input container</param>
        /// <returns>the labelling</returns>
        CIPTool.CIPChirality Label(IAtomContainer container)
        {
            var elements = new List<IStereoElement<IChemObject, IChemObject>>();

            foreach (var element in container.StereoElements)
            {
                elements.Add(element);
            }

            if (elements.Count != 1)
                Assert.Fail($"expected 1 stereo-element, found - {elements.Count}");

            foreach (var element in elements)
            {
                if (element is ITetrahedralChirality)
                {
                    return CIPTool.GetCIPChirality(container, (ITetrahedralChirality)element);
                }
                else if (element is IDoubleBondStereochemistry)
                {
                    return CIPTool.GetCIPChirality(container, (IDoubleBondStereochemistry)element);
                }
            }

            throw new InvalidOperationException();
        }
    }
}

