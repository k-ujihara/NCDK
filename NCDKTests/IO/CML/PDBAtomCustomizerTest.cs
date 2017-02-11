/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
 *                    2013  Egon Willighagen <egonw@users.sf.net>
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
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.LibIO.CML;
using System;
using System.IO;
using System.Text;

namespace NCDK.IO.CML
{
    /// <summary>
    /// TestCase for the <see cref="PDBAtomCustomizer"/> class.
    /// </summary>
    // @cdk.module test-pdbcml
    public class PDBAtomCustomizerTest : CDKTestCase
    {
        /// <summary>
        /// A roundtripping test to see of PDB atom customization works.
        /// </summary>
        // @cdk.bug 1085912
        [TestMethod()]
        public void TestSFBug1085912_1()
        {
            string filename_pdb = "NCDK.Data.PDB.1CKV.pdb";
            Stream ins1 = this.GetType().Assembly.GetManifestResourceStream(filename_pdb);

            ISimpleChemObjectReader reader = new PDBReader(ins1);
            IChemFile chemFile1 = (IChemFile)reader.Read(new ChemFile());
            reader.Close();
            IChemSequence seq1 = chemFile1[0];
            IChemModel model1 = seq1[0];
            IAtomContainer container = model1.MoleculeSet[0];
            IBioPolymer polymer1 = (IBioPolymer)container;
            int countchemFile1 = chemFile1.Count;
            int countmodel1 = model1.MoleculeSet.Count;
            int countpolymer1 = polymer1.Atoms.Count;

            StringWriter writer = new StringWriter();
            CMLWriter cmlWriter = new CMLWriter(writer);
            cmlWriter.RegisterCustomizer(new PDBAtomCustomizer());
            cmlWriter.Write(polymer1);
            cmlWriter.Close();
            string cmlContent1 = writer.ToString();
            Console.Out.WriteLine(cmlContent1.Substring(0, 500));

            CMLReader reader2 = new CMLReader(new MemoryStream(Encoding.UTF8.GetBytes(cmlContent1)));
            IChemFile chemFil2 = (IChemFile)reader2.Read(new ChemFile());
            reader2.Close();
            IChemSequence seq2 = chemFil2[0];
            IChemModel model2 = seq2[0];
            PDBPolymer polymer2 = (PDBPolymer)model2.MoleculeSet[0];

            int countchemFile2 = chemFil2.Count;
            int countmodel2 = model2.MoleculeSet.Count;
            int countpolymer2 = polymer2.Atoms.Count;

            Assert.AreEqual(countchemFile1, countchemFile2);
            Assert.AreEqual(countmodel1, countmodel2);
            Assert.AreEqual(countpolymer1, countpolymer2);

            writer = new StringWriter();
            cmlWriter = new CMLWriter(writer);
            cmlWriter.RegisterCustomizer(new PDBAtomCustomizer());
            cmlWriter.Write(polymer2);
            cmlWriter.Close();
            string cmlContent2 = writer.ToString();
            Console.Out.WriteLine(cmlContent2.Substring(0, 500));

            string conte1 = cmlContent1.Substring(0, 1000);
            string conte2 = cmlContent2.Substring(0, 1000);
            Assert.AreEqual(conte1, conte2);
        }
    }
}
