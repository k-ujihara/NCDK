/* Copyright (C) 2010  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@slists.sourceforge.net
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
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
 * details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;

namespace NCDK.IO
{
    // @cdk.module test-smiles
    [TestClass()]
    public class MoSSOutputReaderTest : SimpleChemObjectReaderTest
    {
        protected override string testFile => "NCDK.Data.MoSS.TKO.mossoutput";
        static readonly MoSSOutputReader simpleReader = new MoSSOutputReader();
        protected override IChemObjectIO ChemObjectIOToTest => simpleReader;

        [TestMethod()]
        public void TestAccepts()
        {
            MoSSOutputReader reader = new MoSSOutputReader();
            Assert.IsTrue(reader.Accepts(typeof(AtomContainerSet<IAtomContainer>)));
        }

        [TestMethod()]
        public void TestExampleFile_MolReading()
        {
            string filename = "NCDK.Data.MoSS.TKO.mossoutput";
            var ins = ResourceLoader.GetAsStream(filename);
            MoSSOutputReader reader = new MoSSOutputReader(ins);
            var moleculeSet = new AtomContainerSet<IAtomContainer>();
            moleculeSet = reader.Read(moleculeSet);
            Assert.AreEqual(19, moleculeSet.Count);
            foreach (var mol in moleculeSet)
            {
                Assert.AreEqual(int.Parse(mol.GetProperty<string>("atomCount").ToString()), mol.Atoms.Count);
                Assert.AreEqual(int.Parse(mol.GetProperty<string>("bondCount").ToString()), mol.Bonds.Count);
            }
        }

        [TestMethod()]
        public void TestExampleFile_SupportColumns()
        {
            string filename = "NCDK.Data.MoSS.TKO.mossoutput";
            var ins = ResourceLoader.GetAsStream(filename);
            MoSSOutputReader reader = new MoSSOutputReader(ins);
            var moleculeSet = new AtomContainerSet<IAtomContainer>();
            moleculeSet = reader.Read(moleculeSet);
            Assert.AreEqual(5.06, double.Parse(moleculeSet[0].GetProperty<string>("focusSupport").ToString()), 0.01);
            Assert.AreEqual(1.74, double.Parse(moleculeSet[0].GetProperty<string>("complementSupport").ToString()), 0.01);
        }
    }
}
