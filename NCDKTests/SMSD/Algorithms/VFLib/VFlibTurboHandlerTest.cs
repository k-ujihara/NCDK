/* Copyright (C) 2009-2010 Syed Asad Rahman <asad@ebi.ac.uk>
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
using NCDK.Default;
using NCDK.IO;
using NCDK.Smiles;
using NCDK.SMSD.Tools;
using System.IO;

namespace NCDK.SMSD.Algorithms.VFLib
{
    /**
     * @author Syed Asad Rahman <asad@ebi.ac.uk>
     *
     * @cdk.module test-smsd
     * @cdk.require java1.6+
     */
    [TestClass()]
    public class VFlibTurboHandlerTest : AbstractSubGraphTest
    {

        public VFlibTurboHandlerTest() { }

        /**
         * Test of isSubgraph method, of class VFlibSubStructureHandler.
         * @throws InvalidSmilesException
         */
        [TestMethod()]
        public override void TestIsSubgraph()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");

            VFlibSubStructureHandler smsd1 = new VFlibSubStructureHandler();
            MolHandler mol1 = new MolHandler(queryac, true, true);
            MolHandler mol2 = new MolHandler(target, true, true);
            smsd1.Set(mol1, mol2);
            Assert.IsTrue(smsd1.IsSubgraph(true));
        }

        /**
         * Test of set method, of class VFlibSubStructureHandler.
         * @throws Exception
         */
        [TestMethod()]
        public void TestSet_IAtomContainer_IAtomContainer()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");

            VFlibSubStructureHandler smsd1 = new VFlibSubStructureHandler();
            MolHandler mol1 = new MolHandler(queryac, true, true);
            MolHandler mol2 = new MolHandler(target, true, true);
            smsd1.Set(mol1, mol2);
            Assert.IsTrue(smsd1.IsSubgraph(true));
        }

        /**
         * Test of set method, of class VFlibSubStructureHandler.
         * @throws CDKException
         */
        [TestMethod()]
        public void TestSet_String_String()
        {
            string molfile = "NCDK.Data.MDL.decalin.mol";
            string queryfile = "NCDK.Data.MDL.decalin.mol";
            IAtomContainer query = new AtomContainer();
            IAtomContainer target = new AtomContainer();

            Stream ins = this.GetType().Assembly.GetManifestResourceStream(molfile);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            reader.Read(query);
            ins = this.GetType().Assembly.GetManifestResourceStream(queryfile);
            reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            reader.Read(target);

            VFlibSubStructureHandler smsd1 = new VFlibSubStructureHandler();
            MolHandler mol1 = new MolHandler(query, true, true);
            MolHandler mol2 = new MolHandler(target, true, true);
            smsd1.Set(mol1, mol2);
            Assert.IsTrue(smsd1.IsSubgraph(true));
        }

        /**
         * Test of set method, of class VFlibSubStructureHandler.
         * @throws InvalidSmilesException
         */
        [TestMethod()]
        public void TestSet_MolHandler_MolHandler()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);

            IAtomContainer target1 = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");
            MolHandler source = new MolHandler(queryac, true, true);
            MolHandler target = new MolHandler(target1, true, true);
            VFlibSubStructureHandler instance = new VFlibSubStructureHandler();
            instance.Set(source, target);
            Assert.IsTrue(instance.IsSubgraph(true));
        }

        /**
         * Test of getAllAtomMapping method, of class VFlibSubStructureHandler.
         * @throws InvalidSmilesException
         */
        [TestMethod()]
        public void TestGetAllAtomMapping()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");

            VFlibSubStructureHandler smsd1 = new VFlibSubStructureHandler();
            MolHandler mol1 = new MolHandler(queryac, true, true);
            MolHandler mol2 = new MolHandler(target, true, true);
            smsd1.Set(mol1, mol2);
            smsd1.IsSubgraph(true);

            Assert.AreEqual(4, smsd1.GetAllAtomMapping().Count);
        }

        /**
         * Test of getAllMapping method, of class VFlibSubStructureHandler.
         * @throws InvalidSmilesException
         */
        [TestMethod()]
        public void TestGetAllMapping()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");

            VFlibSubStructureHandler smsd1 = new VFlibSubStructureHandler();
            MolHandler mol1 = new MolHandler(queryac, true, true);
            MolHandler mol2 = new MolHandler(target, true, true);
            smsd1.Set(mol1, mol2);
            smsd1.IsSubgraph(true);

            Assert.AreEqual(4, smsd1.GetAllMapping().Count);
        }

        /**
         * Test of getFirstAtomMapping method, of class VFlibSubStructureHandler.
         * @throws InvalidSmilesException
         */
        [TestMethod()]
        public void TestGetFirstAtomMapping()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");

            VFlibSubStructureHandler smsd1 = new VFlibSubStructureHandler();
            MolHandler mol1 = new MolHandler(queryac, true, true);
            MolHandler mol2 = new MolHandler(target, true, true);
            smsd1.Set(mol1, mol2);
            smsd1.IsSubgraph(true);

            Assert.AreEqual(7, smsd1.GetFirstAtomMapping().Count);
        }

        /**
         * Test of getFirstMapping method, of class VFlibSubStructureHandler.
         * @throws InvalidSmilesException
         */
        [TestMethod()]
        public void TestGetFirstMapping()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");

            VFlibSubStructureHandler smsd1 = new VFlibSubStructureHandler();
            MolHandler mol1 = new MolHandler(queryac, true, true);
            MolHandler mol2 = new MolHandler(target, true, true);
            smsd1.Set(mol1, mol2);
            smsd1.IsSubgraph(true);

            Assert.AreEqual(7, smsd1.GetFirstMapping().Count);
        }
    }
}
