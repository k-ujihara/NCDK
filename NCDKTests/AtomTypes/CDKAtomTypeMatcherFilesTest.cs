/* Copyright (C) 2008  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.IO;
using NCDK.Silent;
using NCDK.Tools.Manipulator;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.AtomTypes
{
    /// <summary>
    /// This class tests the matching of atom types defined in the
    /// CDK atom type list.
    /// </summary>
    // @cdk.module test-core
    [TestClass()]
    public class CDKAtomTypeMatcherFilesTest : AbstractCDKAtomTypeTest
    {
        private static readonly Dictionary<string, int> testedAtomTypes = new Dictionary<string, int>();

        [TestMethod()]
        public void TestFile3()
        {
            string filename = "NCDK.Data.CML.3.cml";
            var ins = ResourceLoader.GetAsStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            IAtomContainer mol = ChemFileManipulator.GetAllAtomContainers(chemFile).First();

            string[] expectedTypes = {"C.sp2", "N.sp2", "C.sp2", "N.sp3", "C.sp2", "N.sp2", "O.sp3", "C.sp2", "C.sp2",
                "C.sp2"};
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        /// <summary>
        // @cdk.bug 3141611
        /// </summary>
        [TestMethod()]
        public void TestBug3141611()
        {
            string filename = "NCDK.Data.MDL.error.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            IAtomContainer mol = ChemFileManipulator.GetAllAtomContainers(chemFile).First();

            string[] expectedTypes = {"C.sp3", "C.sp2", "O.sp2", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "P.ate", "O.sp2",
                "O.minus"};
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestOla28()
        {
            string filename = "NCDK.Data.CML.mol28.cml";
            var ins = ResourceLoader.GetAsStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            IAtomContainer mol = ChemFileManipulator.GetAllAtomContainers(chemFile).First();

            string[] expectedTypes = {"C.sp2", "C.sp2", "C.sp2", "C.sp2", "F", "C.sp2", "C.sp2", "C.sp2", "O.sp2", "C.sp3",
                "C.sp3", "C.sp3", "N.plus", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp2", "O.sp3", "C.sp2",
                "C.sp2", "C.sp2", "C.sp2", "C.sp2", "Cl"};
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestSmilesFiles()
        {
            CDKAtomTypeMatcher atomTypeMatcher = CDKAtomTypeMatcher.GetInstance(Silent.ChemObjectBuilder.Instance);

            // Read the first file
            string filename = "NCDK.Data.CML.smiles1.cml";
            var ins = ResourceLoader.GetAsStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            Assert.IsNotNull(chemFile);
            IAtomContainer mol1 = ChemFileManipulator.GetAllAtomContainers(chemFile).First();

            // Read the second file
            filename = "NCDK.Data.CML.smiles2.cml";
            ins = ResourceLoader.GetAsStream(filename);
            reader = new CMLReader(ins);
            chemFile = (IChemFile)reader.Read(new ChemFile());
            Assert.IsNotNull(chemFile);
            IAtomContainer mol2 = ChemFileManipulator.GetAllAtomContainers(chemFile).First();

            var types1 = atomTypeMatcher.FindMatchingAtomTypes(mol1).ToList();
            var types2 = atomTypeMatcher.FindMatchingAtomTypes(mol2).ToList();
            for (int i = 0; i < mol1.Atoms.Count; i++)
            {
                Assert.IsNotNull(types1[i], "Atom typing in mol1 failed for atom " + (i + 1));
                Assert.IsNotNull(types2[i], "Atom typing in mol2 failed for atom " + (i + 1));
                Assert.AreEqual(types1[i].AtomTypeName, types2[i].AtomTypeName,
                    "Atom type mismatch for the " + (i + 1) + " atom");
            }
        }
    }
}
