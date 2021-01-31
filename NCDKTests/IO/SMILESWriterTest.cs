/* Copyright (C) 2009  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@slists.sourceforge.net
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
using NCDK.Aromaticities;
using NCDK.IO.Listener;
using NCDK.Silent;
using NCDK.Templates;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Collections.Generic;
using NCDK.Smiles;
using NCDK.Common.Primitives;
using System.Linq;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the writer for SMILES files.
    /// </summary>
    /// <seealso cref="SMILESWriter"/>
    // @cdk.module test-smiles
    [TestClass()]
    public class SMILESWriterTest : ChemObjectIOTest
    {
        protected override Type ChemObjectIOToTestType => typeof(SMILESWriter);

        [TestMethod()]
        public void TestAccepts()
        {
            SMILESWriter reader = new SMILESWriter(new StringWriter());
            Assert.IsTrue(reader.Accepts(typeof(AtomContainer)));
            Assert.IsTrue(reader.Accepts(typeof(ChemObjectSet<IAtomContainer>)));
        }

        [TestMethod()]
        public void TestWriteSMILESFile()
        {
            StringWriter stringWriter = new StringWriter();
            IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();
            AddImplicitHydrogens(benzene);
            SMILESWriter smilesWriter = new SMILESWriter(stringWriter);
            smilesWriter.Write(benzene);
            smilesWriter.Close();
            Assert.IsTrue(stringWriter.ToString().Contains("C=C"));
        }

        [TestMethod()]
        public void TestWriteAromatic()
        {
            StringWriter stringWriter = new StringWriter();
            IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();
            AddImplicitHydrogens(benzene);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(benzene);
            Aromaticity.CDKLegacy.Apply(benzene);
            SMILESWriter smilesWriter = new SMILESWriter(stringWriter);
            var prop = new NameValueCollection
            {
                ["UseAromaticity"] = "true"
            };
            PropertiesListener listener = new PropertiesListener(prop);
            smilesWriter.Listeners.Add(listener);
            smilesWriter.CustomizeJob();
            smilesWriter.Write(benzene);
            smilesWriter.Close();
            Assert.IsFalse(stringWriter.ToString().Contains("C=C"));
            Assert.IsTrue(stringWriter.ToString().Contains("ccc"));
        }

        [TestMethod()]
        public void TestWriteNonCanon()
        {
            var mol1 = CDK.SmilesParser.ParseSmiles("CCO");
            var mol2 = CDK.SmilesParser.ParseSmiles("OCC");
            using var wtr = new StringWriter();
            using (var smigen = new SMILESWriter(wtr))
            {
                smigen.Write(mol1);
                smigen.Write(mol2);
            }
            var lines = wtr.ToString().Split("\n").Where(n => !string.IsNullOrEmpty(n)).ToList();
            Assert.AreEqual(2, lines.Count);
            Assert.AreEqual(2, new HashSet<string>(lines).Count);
        }

        [TestMethod()]
        public void TestWriteCanon()
        {
            var mol1 = CDK.SmilesParser.ParseSmiles("CCO");
            var mol2 = CDK.SmilesParser.ParseSmiles("OCC");
            using var wtr = new StringWriter();
            using (var smigen = new SMILESWriter(wtr))
            {
                smigen.Flavor = SmiFlavors.Canonical;
                smigen.Write(mol1);
                smigen.Write(mol2);
            }
            var lines = wtr.ToString().Split("\n").Where(n => !string.IsNullOrEmpty(n)).ToList();
            Assert.AreEqual(2, lines.Count);
            Assert.AreEqual(1, new HashSet<string>(lines).Count);
        }

        [TestMethod()]
        public void TestWriteWithTitle()
        {
            var mol1 = CDK.SmilesParser.ParseSmiles("CCO mol 1");
            var mol2 = CDK.SmilesParser.ParseSmiles("OCC mol 2");
            using var wtr = new StringWriter();
            using (var smigen = new SMILESWriter(wtr))
            {
                smigen.Flavor = SmiFlavors.Canonical;
                smigen.WriteTitle = true;
                smigen.Write(mol1);
                smigen.Write(mol2);
            }
            Assert.IsTrue(wtr.ToString().ContainsOrdinal("mol 1"));
            Assert.IsTrue(wtr.ToString().ContainsOrdinal("mol 2"));
        }

        [TestMethod()]
        public void TestWriteWithoutTitle()
        {
            var mol1 = CDK.SmilesParser.ParseSmiles("CCO mol 1");
            var mol2 = CDK.SmilesParser.ParseSmiles("OCC mol 2");
            using var wtr = new StringWriter();
            using (var smigen = new SMILESWriter(wtr))
            {
                smigen.Flavor = SmiFlavors.Canonical;
                smigen.WriteTitle = false;
                smigen.Write(mol1);
                smigen.Write(mol2);
            }
            Assert.IsFalse(wtr.ToString().ContainsOrdinal("mol 1"));
            Assert.IsFalse(wtr.ToString().ContainsOrdinal("mol 2"));
        }

        [TestMethod()]
        public void TestWriteSmiFlavor()
        {
            var mol = CDK.SmilesParser.ParseSmiles("c1ccccc1");
            using var wtr = new StringWriter();
            using (var smigen = new SMILESWriter(wtr))
            {
                smigen.Flavor = SmiFlavors.InChILabelling;
                smigen.Write(mol);
            }
            Assert.IsTrue(wtr.ToString().ContainsOrdinal("C=1C=CC=CC1"));
        }
    }
}
