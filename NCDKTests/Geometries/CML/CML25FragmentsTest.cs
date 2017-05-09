/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *  */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;
using System.IO;
using System.Text;

namespace NCDK.IO.CML
{
    /// <summary>
    /// Atomic tests for reading CML documents. All tested CML strings are valid CML 2.5,
    /// as can be determined in "NCDK.IO.CML.cml25TestFramework.xml".
    /// </summary>
    // @cdk.module test-io
    // @author Egon Willighagen <egonw@sci.kun.nl>
    [TestClass()]
    public class CML25FragmentsTest : CDKTestCase
    {
        //@Ignore("Functionality not yet implemented")
        public void TestIsotopeRef()
        {
            string cmlString = "<cml>" + "  <isotopeList>" + "    <isotope id='H1' number='1' elementType='H'>"
                    + "      <scalar dictRef='bo:relativeAbundance'>99.9885</scalar>"
                    + "      <scalar dictRef='bo:exactMass' errorValue='0.0001E-6'>1.007825032</scalar>" + "    </isotope>"
                    + "  </isotopeList>" + "  <molecule>" + "    <atomArray>"
                    + "      <atom id='a1' elementType='H' isotopeRef='H1'/>" + "    </atomArray>" + "  </molecule>"
                    + "</cml>";

            IChemFile chemFile = ParseCMLString(cmlString);
            IAtomContainer mol = CheckForSingleMoleculeFile(chemFile);

            Assert.AreEqual(1, mol.Atoms.Count);
            IAtom atom = mol.Atoms[0];
            Assert.AreEqual("a1", atom.Id);
            Assert.IsNotNull(atom.NaturalAbundance);
            Assert.AreEqual(99.9885, atom.NaturalAbundance.Value, 0.0001);
            Assert.IsNotNull(atom.ExactMass);
            Assert.AreEqual(1.007825032, atom.ExactMass.Value, 0.0000001);
        }

        private IChemFile ParseCMLString(string cmlString)
        {
            IChemFile chemFile = null;
            CMLReader reader = new CMLReader(new MemoryStream(Encoding.UTF8.GetBytes(cmlString)));
            chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();
            return chemFile;
        }

        /// <summary>
        /// Tests whether the file is indeed a single molecule file
        /// </summary>
        private IAtomContainer CheckForSingleMoleculeFile(IChemFile chemFile)
        {
            return CheckForXMoleculeFile(chemFile, 1);
        }

        private IAtomContainer CheckForXMoleculeFile(IChemFile chemFile, int numberOfMolecules)
        {
            Assert.IsNotNull(chemFile);

            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);

            Assert.AreEqual(seq.Count, 1);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);

            var moleculeSet = model.MoleculeSet;
            Assert.IsNotNull(moleculeSet);

            Assert.AreEqual(moleculeSet.Count, numberOfMolecules);
            IAtomContainer mol = null;
            for (int i = 0; i < numberOfMolecules; i++)
            {
                mol = moleculeSet[i];
                Assert.IsNotNull(mol);
            }
            return mol;
        }
    }
}
