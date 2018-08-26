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
using NCDK.Smiles;
using NCDK.SMSD.Tools;
using System.Diagnostics;
using System.IO;

namespace NCDK.SMSD.Algorithms.VFLib
{
    /// <summary>
    /// Unit testing for the <see cref="VFlibMCSHandler"/> class.
    /// </summary>
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    // @cdk.module test-smsd
    [TestClass()]
    public class VFlibMCSHandlerTest : AbstractMCSAlgorithmTest
    {
        protected override AbstractMCSAlgorithm algorithm { get; } = new VFlibMCSHandler();

        public VFlibMCSHandlerTest() { }

        /// <summary>
        /// Test of searchMCS method, of class VFlibMCSHandler.
        /// </summary>
        [TestMethod()]
        public override void TestSearchMCS()
        {
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer target = null;
            try
            {
                target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            }
            catch (InvalidSmilesException ex)
            {
                Trace.TraceError(ex.Message);
            }
            IAtomContainer queryac = null;
            try
            {
                queryac = sp.ParseSmiles("Nc1ccccc1");
            }
            catch (InvalidSmilesException ex)
            {
                Trace.TraceError(ex.Message);
            }

            VFlibMCSHandler smsd1 = new VFlibMCSHandler();
            MolHandler mol1 = new MolHandler(queryac, true, true);
            MolHandler mol2 = new MolHandler(target, true, true);
            smsd1.Set(mol1, mol2);
            smsd1.SearchMCS(true);
            Assert.IsNotNull(smsd1.GetFirstMapping());
        }

        /// <summary>
        /// Test of set method, of class VFlibMCSHandler.
        /// </summary>
        [TestMethod()]
        public void TestSet_IAtomContainer_IAtomContainer()
        {
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");

            VFlibMCSHandler smsd1 = new VFlibMCSHandler();
            MolHandler mol1 = new MolHandler(queryac, true, true);
            MolHandler mol2 = new MolHandler(target, true, true);
            smsd1.Set(mol1, mol2);
            smsd1.SearchMCS(true);
            Assert.IsNotNull(smsd1.GetFirstMapping());
        }

        /// <summary>
        /// Test of set method, of class VFlibMCSHandler.
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

            VFlibMCSHandler smsd1 = new VFlibMCSHandler();
            MolHandler mol1 = new MolHandler(query, true, true);
            MolHandler mol2 = new MolHandler(target, true, true);
            smsd1.Set(mol1, mol2);
            smsd1.SearchMCS(true);

            Assert.IsNotNull(smsd1.GetFirstMapping());
        }

        /// <summary>
        /// Test of set method, of class VFlibMCSHandler.
        /// </summary>
        [TestMethod()]
        public void TestSet_MolHandler_MolHandler()
        {
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);

            IAtomContainer target1 = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");
            MolHandler source = new MolHandler(queryac, true, true);
            MolHandler target = new MolHandler(target1, true, true);
            VFlibMCSHandler instance = new VFlibMCSHandler();
            instance.Set(source, target);
            instance.SearchMCS(true);
            Assert.IsNotNull(instance.GetFirstMapping());
        }

        /// <summary>
        /// Test of getAllAtomMapping method, of class VFlibMCSHandler.
        /// </summary>
        [TestMethod()]
        public void TestGetAllAtomMapping()
        {
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");

            VFlibMCSHandler smsd1 = new VFlibMCSHandler();
            MolHandler mol1 = new MolHandler(queryac, true, true);
            MolHandler mol2 = new MolHandler(target, true, true);
            smsd1.Set(mol1, mol2);
            smsd1.SearchMCS(true);
            Assert.IsNotNull(smsd1.GetFirstMapping());

            Assert.AreEqual(4, smsd1.GetAllAtomMapping().Count);
        }

        /// <summary>
        /// Test of getAllMapping method, of class VFlibMCSHandler.
        /// </summary>
        [TestMethod()]
        public void TestGetAllMapping()
        {
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");

            VFlibMCSHandler smsd1 = new VFlibMCSHandler();
            MolHandler mol1 = new MolHandler(queryac, true, true);
            MolHandler mol2 = new MolHandler(target, true, true);
            smsd1.Set(mol1, mol2);
            smsd1.SearchMCS(true);
            Assert.IsNotNull(smsd1.GetFirstMapping());

            Assert.AreEqual(4, smsd1.GetAllMapping().Count);
        }

        /// <summary>
        /// Test of getFirstAtomMapping method, of class VFlibMCSHandler.
        /// </summary>
        [TestMethod()]
        public void TestGetFirstAtomMapping()
        {
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");

            VFlibMCSHandler smsd1 = new VFlibMCSHandler();
            MolHandler mol1 = new MolHandler(queryac, true, true);
            MolHandler mol2 = new MolHandler(target, true, true);
            smsd1.Set(mol1, mol2);
            smsd1.SearchMCS(true);
            Assert.IsNotNull(smsd1.GetFirstMapping());

            Assert.AreEqual(7, smsd1.GetFirstAtomMapping().Count);
        }

        /// <summary>
        /// Test of getFirstMapping method, of class VFlibMCSHandler.
        /// </summary>
        [TestMethod()]
        public void TestGetFirstMapping()
        {
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");

            VFlibMCSHandler smsd1 = new VFlibMCSHandler();
            MolHandler mol1 = new MolHandler(queryac, true, true);
            MolHandler mol2 = new MolHandler(target, true, true);
            smsd1.Set(mol1, mol2);
            smsd1.SearchMCS(true);
            Assert.IsNotNull(smsd1.GetFirstMapping());

            Assert.AreEqual(7, smsd1.GetFirstMapping().Count);
        }
    }
}
