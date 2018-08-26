/* Copyright (C) 2010  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License as published by the Free
 * Software Foundation; either version 2.1 of the License, or (at your option)
 * any later version. All we ask is that proper credit is given for our work,
 * which includes - but is not limited to - adding the above copyright notice to
 * the beginning of your source code files, and to any copyright notice that you
 * may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more
 * details.
 *
 * You should have received rAtomCount copy of the GNU Lesser General Public
 * License along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;
using NCDK.IO;
using NCDK.SMSD.Tools;
using System.IO;

namespace NCDK.SMSD.Algorithms.Singles
{
    /// <summary>
    /// Unit testing for the <see cref="SingleMappingHandler"/> class.
    /// </summary>
    // @author     egonw
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    // @cdk.module test-smsd
    [TestClass()]
    public class SingleMappingHandlerTest : AbstractMCSAlgorithmTest
    {
        protected override AbstractMCSAlgorithm algorithm { get; } = new SingleMappingHandler(true);

        /// <summary>
        /// Test of set method, of class SingleMappingHandler.
        /// </summary>
        [TestMethod()]
        public void TestSet_IAtomContainer_IAtomContainer()
        {
            IAtom atomSource = new Atom("R");
            IAtom atomTarget = new Atom("R");
            IAtomContainer source = new AtomContainer();
            source.Atoms.Add(atomSource);
            IAtomContainer target = new AtomContainer();
            target.Atoms.Add(atomTarget);
            bool removeHydrogen = false;
            SingleMappingHandler instance = new SingleMappingHandler(removeHydrogen);
            MolHandler mol1 = new MolHandler(source, true, true);
            MolHandler mol2 = new MolHandler(target, true, true);
            instance.Set(mol1, mol2);
            Assert.IsNotNull(instance.GetFirstAtomMapping());
        }

        /// <summary>
        /// Test of set method, of class SingleMappingHandler.
        /// </summary>
        [TestMethod()]
        public void TestSet_String_String()
        {
            string molfile = "NCDK.Data.MDL.decalin.mol";
            string queryfile = "NCDK.Data.MDL.decalin.mol";
            IAtomContainer query = new AtomContainer();
            IAtomContainer target = new AtomContainer();

            Stream ins = ResourceLoader.GetAsStream(molfile);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderMode.Strict);
            reader.Read(query);
            ins = ResourceLoader.GetAsStream(queryfile);
            reader = new MDLV2000Reader(ins, ChemObjectReaderMode.Strict);
            reader.Read(target);

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(query, target, true, true);
            smsd1.SetChemFilters(true, true, true);
            double score = 1.0;
            Assert.AreEqual(score, smsd1.GetTanimotoSimilarity(), 0.0001);
        }

        /// <summary>
        /// Test of set method, of class SingleMappingHandler.
        /// </summary>
        [TestMethod()]
        public void TestSet_MolHandler_MolHandler()
        {
            IAtom atomSource = new Atom("R");
            IAtom atomTarget = new Atom("R");
            IAtomContainer source = new AtomContainer();
            source.Atoms.Add(atomSource);
            IAtomContainer target = new AtomContainer();
            target.Atoms.Add(atomTarget);
            MolHandler source1 = new MolHandler(source, true, true);
            MolHandler target1 = new MolHandler(target, true, true);

            bool removeHydrogen = false;
            SingleMappingHandler instance = new SingleMappingHandler(removeHydrogen);
            instance.Set(source1, target1);
            instance.SearchMCS(true);
            Assert.IsNotNull(instance.GetFirstAtomMapping());
        }

        /// <summary>
        /// Test of searchMCS method, of class SingleMappingHandler.
        /// </summary>
        [TestMethod()]
        public override void TestSearchMCS()
        {
            IAtom atomSource = new Atom("R");
            IAtom atomTarget = new Atom("R");
            IAtomContainer source = new AtomContainer();
            source.Atoms.Add(atomSource);
            IAtomContainer target = new AtomContainer();
            target.Atoms.Add(atomTarget);
            bool removeHydrogen = false;
            SingleMappingHandler instance = new SingleMappingHandler(removeHydrogen);
            MolHandler mol1 = new MolHandler(source, true, true);
            MolHandler mol2 = new MolHandler(target, true, true);
            instance.Set(mol1, mol2);
            instance.SearchMCS(true);
            Assert.IsNotNull(instance.GetAllMapping());
            Assert.AreEqual(1, instance.GetAllMapping().Count);
        }

        /// <summary>
        /// Test of getAllMapping method, of class SingleMappingHandler.
        /// </summary>
        [TestMethod()]
        public void TestGetAllMapping()
        {
            IAtom atomSource = new Atom("R");
            IAtom atomTarget = new Atom("R");
            IAtomContainer source = new AtomContainer();
            source.Atoms.Add(atomSource);
            IAtomContainer target = new AtomContainer();
            target.Atoms.Add(atomTarget);
            bool removeHydrogen = false;
            SingleMappingHandler instance = new SingleMappingHandler(removeHydrogen);
            MolHandler mol1 = new MolHandler(source, true, true);
            MolHandler mol2 = new MolHandler(target, true, true);
            instance.Set(mol1, mol2);
            instance.SearchMCS(true);
            Assert.IsNotNull(instance.GetAllMapping());
        }

        /// <summary>
        /// Test of getFirstMapping method, of class SingleMappingHandler.
        /// </summary>
        [TestMethod()]
        public void TestGetFirstMapping()
        {
            IAtom atomSource = new Atom("R");
            IAtom atomTarget = new Atom("R");
            IAtomContainer source = new AtomContainer();
            source.Atoms.Add(atomSource);
            IAtomContainer target = new AtomContainer();
            target.Atoms.Add(atomTarget);
            bool removeHydrogen = false;
            SingleMappingHandler instance = new SingleMappingHandler(removeHydrogen);
            MolHandler mol1 = new MolHandler(source, true, true);
            MolHandler mol2 = new MolHandler(target, true, true);
            instance.Set(mol1, mol2);
            instance.SearchMCS(true);
            Assert.IsNotNull(instance.GetFirstMapping());
        }

        /// <summary>
        /// Test of getAllAtomMapping method, of class SingleMappingHandler.
        /// </summary>
        [TestMethod()]
        public void TestGetAllAtomMapping()
        {
            IAtom atomSource = new Atom("R");
            IAtom atomTarget = new Atom("R");
            IAtomContainer source = new AtomContainer();
            source.Atoms.Add(atomSource);
            IAtomContainer target = new AtomContainer();
            target.Atoms.Add(atomTarget);
            bool removeHydrogen = false;
            SingleMappingHandler instance = new SingleMappingHandler(removeHydrogen);
            MolHandler mol1 = new MolHandler(source, true, true);
            MolHandler mol2 = new MolHandler(target, true, true);
            instance.Set(mol1, mol2);
            instance.SearchMCS(true);
            Assert.IsNotNull(instance.GetAllAtomMapping());
        }

        /// <summary>
        /// Test of getFirstAtomMapping method, of class SingleMappingHandler.
        /// </summary>
        [TestMethod()]
        public void TestGetFirstAtomMapping()
        {
            IAtom atomSource = new Atom("R");
            IAtom atomTarget = new Atom("R");
            IAtomContainer source = new AtomContainer();
            source.Atoms.Add(atomSource);
            IAtomContainer target = new AtomContainer();
            target.Atoms.Add(atomTarget);
            bool removeHydrogen = false;
            SingleMappingHandler instance = new SingleMappingHandler(removeHydrogen);
            MolHandler mol1 = new MolHandler(source, true, true);
            MolHandler mol2 = new MolHandler(target, true, true);
            instance.Set(mol1, mol2);
            instance.SearchMCS(true);
            Assert.IsNotNull(instance.GetFirstAtomMapping());
        }
    }
}
