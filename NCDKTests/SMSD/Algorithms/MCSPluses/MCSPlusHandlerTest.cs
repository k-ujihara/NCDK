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
using NCDK.Default;

using NCDK.IO;
using NCDK.Smiles;
using NCDK.SMSD.Tools;
using NCDK.Tools.Manipulator;
using System.Diagnostics;
using System.IO;
using System;

namespace NCDK.SMSD.Algorithms.MCSPluses
{
    /// <summary>
    /// Unit testing for the {@link MCSPlusHandler} class.
    // @author     Syed Asad Rahman
    // @author     egonw
    // @cdk.module test-smsd
    /// </summary>
     [TestClass()]
    public class MCSPlusHandlerTest : AbstractMCSAlgorithmTest
    {
        protected override AbstractMCSAlgorithm algorithm { get; } = new MCSPlusHandler();

        /// <summary>
        /// Test of searchMCS method, of class MCSPlusHandler.
        /// </summary>
        [TestMethod()]
        public override void TestSearchMCS()
        {
            try
            {
                SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
                IAtomContainer target = null;
                target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
                IAtomContainer queryac = null;
                queryac = sp.ParseSmiles("Nc1ccccc1");
                MCSPlusHandler smsd1 = new MCSPlusHandler();
                MolHandler mol1 = new MolHandler(queryac, true, true);
                MolHandler mol2 = new MolHandler(target, true, true);
                smsd1.Set(mol1, mol2);
                smsd1.SearchMCS(true);
                Assert.IsNotNull(smsd1.GetFirstMapping());
            }
            catch (InvalidSmilesException ex)
            {
                Trace.TraceError(ex.Message);
            }
        }

        /// <summary>
        /// Test of set method, of class MCSPlusHandler.
        // @throws Exception
        /// </summary>
        [TestMethod()]
        public void TestSet_IAtomContainer_IAtomContainer()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");

            MCSPlusHandler smsd1 = new MCSPlusHandler();
            MolHandler mol1 = new MolHandler(queryac, true, true);
            MolHandler mol2 = new MolHandler(target, true, true);
            smsd1.Set(mol1, mol2);
            smsd1.SearchMCS(true);
            Assert.IsNotNull(smsd1.GetFirstMapping());
        }

        /// <summary>
        /// Test of set method, of class MCSPlusHandler.
        // @throws CDKException
        // @throws IOException
        /// </summary>
        [TestMethod()]
        public void TestSet_String_String()
        {
            string molfile = "NCDK.Data.MDL.decalin.mol";
            string queryfile = "NCDK.Data.MDL.decalin.mol";
            IAtomContainer query = new AtomContainer();
            IAtomContainer target = new AtomContainer();

            Stream ins = ResourceLoader.GetAsStream(molfile);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            reader.Read(query);
            ins = ResourceLoader.GetAsStream(queryfile);
            reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            reader.Read(target);

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(query, target, true, true);
            smsd1.SetChemFilters(true, true, true);
            double score = 1.0;
            Assert.AreEqual(score, smsd1.GetTanimotoSimilarity(), 0.0001);
        }

        /// <summary>
        /// Test of set method, of class MCSPlusHandler.
        // @throws InvalidSmilesException
        /// </summary>
        [TestMethod()]
        public void TestSet_MolHandler_MolHandler()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);

            IAtomContainer target1 = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");
            MolHandler source = new MolHandler(queryac, true, true);
            MolHandler target = new MolHandler(target1, true, true);
            MCSPlusHandler instance = new MCSPlusHandler();
            instance.Set(source, target);
            instance.SearchMCS(true);
            Assert.IsNotNull(instance.GetFirstMapping());
        }

        /// <summary>
        /// Test of getAllAtomMapping method, of class MCSPlusHandler.
        // @throws InvalidSmilesException
        /// </summary>
        //[TestMethod()]
        //@Ignore("Failing but not going to be fixed")
        public void TestGetAllAtomMapping()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            sp.Kekulise(false);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(target);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(queryac);

            MCSPlusHandler smsd1 = new MCSPlusHandler();
            MolHandler mol1 = new MolHandler(queryac, true, true);
            MolHandler mol2 = new MolHandler(target, true, true);
            smsd1.Set(mol1, mol2);
            smsd1.SearchMCS(true);
            Assert.IsNotNull(smsd1.GetFirstMapping());

            Assert.AreEqual(4, smsd1.GetAllAtomMapping().Count);
        }

        /// <summary>
        /// Test of getAllMapping method, of class MCSPlusHandler.
        // @throws InvalidSmilesException
        /// </summary>
        //[TestMethod()]
        //@Ignore("Failing but not going to be fixed")
        public void TestGetAllMapping()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            sp.Kekulise(false);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(target);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(queryac);

            MCSPlusHandler smsd1 = new MCSPlusHandler();
            MolHandler mol1 = new MolHandler(queryac, true, true);
            MolHandler mol2 = new MolHandler(target, true, true);
            smsd1.Set(mol1, mol2);
            smsd1.SearchMCS(true);
            Assert.IsNotNull(smsd1.GetFirstMapping());

            Assert.AreEqual(4, smsd1.GetAllMapping().Count);
        }

        /// <summary>
        /// Test of getFirstAtomMapping method, of class MCSPlusHandler.
        // @throws InvalidSmilesException
        /// </summary>
        [TestMethod()]
        public void TestGetFirstAtomMapping()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");

            MCSPlusHandler smsd1 = new MCSPlusHandler();
            MolHandler mol1 = new MolHandler(queryac, true, true);
            MolHandler mol2 = new MolHandler(target, true, true);
            smsd1.Set(mol1, mol2);
            smsd1.SearchMCS(true);
            Assert.IsNotNull(smsd1.GetFirstMapping());

            Assert.AreEqual(7, smsd1.GetFirstAtomMapping().Count);
        }

        /// <summary>
        /// Test of getFirstMapping method, of class MCSPlusHandler.
        // @throws InvalidSmilesException
        /// </summary>
        [TestMethod()]
        public void TestGetFirstMapping()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");

            MCSPlusHandler smsd1 = new MCSPlusHandler();
            MolHandler mol1 = new MolHandler(queryac, true, true);
            MolHandler mol2 = new MolHandler(target, true, true);
            smsd1.Set(mol1, mol2);
            smsd1.SearchMCS(true);
            Assert.IsNotNull(smsd1.GetFirstMapping());

            Assert.AreEqual(7, smsd1.GetFirstMapping().Count);
        }
    }
}
