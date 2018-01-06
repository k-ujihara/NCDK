/*
 * Copyright (c) 2015 John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Sgroups;
using NCDK.Smiles;

namespace NCDK.Depict
{
    [TestClass()]
    public class AbbreviationsTest
    {
        private static SmilesParser smipar = new SmilesParser(Silent.ChemObjectBuilder.Instance);

        static IAtomContainer Smi(string smi)
        {
            return smipar.ParseSmiles(smi);
        }

        [TestMethod()]
        public void PotassiumCarbonate()
        {
            IAtomContainer mol = Smi("[K+].[O-]C(=O)[O-].[K+]");
            Abbreviations factory = new Abbreviations();
            factory.Add("[K+].[O-]C(=O)[O-].[K+] K2CO3");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual("K2CO3", sgroups[0].Subscript);
            Assert.AreEqual(6, sgroups[0].Atoms.Count);
        }

        [TestMethod()]
        public void Phenyl()
        {
            Abbreviations factory = new Abbreviations();
            IAtomContainer mol = Smi("CCCCCCC(c1ccccc1)(c1ccccc1)c1ccccc1");
            factory.Add("*c1ccccc1 Ph");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(3, sgroups.Count);
            Assert.AreEqual("Ph", sgroups[0].Subscript);
            Assert.AreEqual(6, sgroups[0].Atoms.Count);
            Assert.AreEqual(1, sgroups[0].Bonds.Count);
            Assert.AreEqual("Ph", sgroups[1].Subscript);
            Assert.AreEqual(6, sgroups[1].Atoms.Count);
            Assert.AreEqual(1, sgroups[1].Bonds.Count);
            Assert.AreEqual("Ph", sgroups[2].Subscript);
            Assert.AreEqual(6, sgroups[2].Atoms.Count);
            Assert.AreEqual(1, sgroups[2].Bonds.Count);
        }

        [TestMethod()]
        public void PhenylShouldNotMatchBenzene()
        {
            Abbreviations factory = new Abbreviations();
            IAtomContainer mol = Smi("c1ccccc1");
            factory.Add("*c1ccccc1 Ph");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(0, sgroups.Count);
        }

        [TestMethod()]
        public void AvoidOverZealousAbbreviations()
        {
            Abbreviations factory = new Abbreviations();
            IAtomContainer mol = Smi("Clc1ccccc1");
            factory.Add("*c1ccccc1 Ph");
            Assert.AreEqual(0, factory.Apply(mol));
        }

        [TestMethod()]
        public void PhenylShouldNotMatchC4H6()
        {
            Abbreviations factory = new Abbreviations();
            IAtomContainer mol = Smi("Oc1ccc(O)cc1");
            factory.Add("*c1ccccc1 Ph");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(0, sgroups.Count);
        }

        [TestMethod()]
        public void PhenylShouldAbbreviateExplicitHydrogens()
        {
            Abbreviations factory = new Abbreviations();
            IAtomContainer mol = Smi("CCCCc1ccc([H])cc1");
            factory.Add("*c1ccccc1 Ph");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual("Ph", sgroups[0].Subscript);
            Assert.AreEqual(7, sgroups[0].Atoms.Count);
            Assert.AreEqual(1, sgroups[0].Bonds.Count);
        }

        // some SMARTS foo here :-)
        [TestMethod()]
        public void PhenylShouldMatchKekuleForm()
        {
            Abbreviations factory = new Abbreviations();
            IAtomContainer mol = Smi("CCCCC1=CC=CC=C1");
            factory.Add("*c1ccccc1 Ph");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual("Ph", sgroups[0].Subscript);
            Assert.AreEqual(6, sgroups[0].Atoms.Count);
            Assert.AreEqual(1, sgroups[0].Bonds.Count);
        }

        // SMARTS foo not that good
        [TestMethod()]
        public void NitroGroups()
        {
            Abbreviations factory = new Abbreviations();
            IAtomContainer mol = Smi("O=N(=O)CCCC[N+]([O-])=O");
            factory.Add("*N(=O)(=O) NO2");
            factory.Add("*[N+]([O-])(=O) NO2");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(2, sgroups.Count);
            Assert.AreEqual("NO2", sgroups[0].Subscript);
            Assert.AreEqual(3, sgroups[0].Atoms.Count);
            Assert.AreEqual(1, sgroups[0].Bonds.Count);
            Assert.AreEqual("NO2", sgroups[1].Subscript);
            Assert.AreEqual(3, sgroups[1].Atoms.Count);
            Assert.AreEqual(1, sgroups[1].Bonds.Count);
        }

        [TestMethod()]
        public void AbbreviationsHavePriority()
        {
            Abbreviations factory = new Abbreviations();
            IAtomContainer mol = Smi("c1ccccc1CCC");
            factory.Add("*CCC Pr");
            factory.Add("*CC Et");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual("Pr", sgroups[0].Subscript);
        }

        [TestMethod()]
        public void DontOverwriteExistingSgroups()
        {
            Abbreviations factory = new Abbreviations();
            factory.Add("*CCC Bu");
            IAtomContainer mol = Smi("c1ccccc1CCC");
            Sgroup sgroup = new Sgroup();
            sgroup.Atoms.Add(mol.Atoms[6]);
            sgroup.Atoms.Add(mol.Atoms[7]);
            sgroup.Atoms.Add(mol.Atoms[8]);
            sgroup.Type = SgroupTypes.CtabAbbreviation;
            sgroup.Subscript = "n-Bu";
            mol.SetProperty(CDKPropertyName.CtabSgroups, new[] { sgroup });
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(0, sgroups.Count);
        }
        [TestMethod()]
        public void NHBocFromHeteroCollapse()
        {
            Abbreviations factory = new Abbreviations();
            factory.Add("*C(=O)OC(C)(C)C Boc");
            IAtomContainer mol = Smi("c1ccccc1NC(=O)OC(C)(C)C");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual("NHBoc", sgroups[0].Subscript);
            Assert.AreEqual(1, sgroups[0].Bonds.Count);
            Assert.AreEqual(8, sgroups[0].Atoms.Count);
        }

        [TestMethod()]
        public void NHBocFromHeteroCollapseExplicitH()
        {
            Abbreviations factory = new Abbreviations();
            factory.Add("*C(=O)OC(C)(C)C Boc");
            IAtomContainer mol = Smi("c1ccccc1N([H])C(=O)OC(C)(C)C");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual("NHBoc", sgroups[0].Subscript);
            Assert.AreEqual(1, sgroups[0].Bonds.Count);
            Assert.AreEqual(9, sgroups[0].Atoms.Count);
        }

        [TestMethod()]
        public void NBocClFromHeteroCollapseExplicit()
        {
            Abbreviations factory = new Abbreviations();
            factory.Add("*C(=O)OC(C)(C)C Boc");
            IAtomContainer mol = Smi("c1ccccc1N(Cl)C(=O)OC(C)(C)C");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual("NClBoc", sgroups[0].Subscript);
            Assert.AreEqual(1, sgroups[0].Bonds.Count);
            Assert.AreEqual(9, sgroups[0].Atoms.Count);
        }

        [TestMethod()]
        public void NBoc2FromHeteroCollapse()
        {
            Abbreviations factory = new Abbreviations();
            factory.Add("*C(=O)OC(C)(C)C Boc");
            IAtomContainer mol = Smi("c1cc2ccccc2cc1N(C(=O)OC(C)(C)C)C(=O)OC(C)(C)C");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual("NBoc2", sgroups[0].Subscript);
            Assert.AreEqual(1, sgroups[0].Bonds.Count);
            Assert.AreEqual(15, sgroups[0].Atoms.Count);
        }

        [TestMethod()]
        public void IPrFromHeteroCollapse()
        {
            Abbreviations factory = new Abbreviations();
            factory.Add("*C(C)C iPr");
            IAtomContainer mol = Smi("[CH3:27][CH:19]([CH3:28])[C:20]1=[N:26][C:23](=[CH:22][S:21]1)[C:24](=[O:25])O");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual("iPr", sgroups[0].Subscript);
            Assert.AreEqual(1, sgroups[0].Bonds.Count);
            Assert.AreEqual(3, sgroups[0].Atoms.Count);
        }

        [TestMethod()]
        public void NBocFromHeteroCollapseExplicitH()
        {
            Abbreviations factory = new Abbreviations();
            factory.Add("*C(=O)OC(C)(C)C Boc");
            IAtomContainer mol = Smi("c1cc2ccccc2ccn1C(=O)OC(C)(C)C");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual("NBoc", sgroups[0].Subscript);
            Assert.AreEqual(2, sgroups[0].Bonds.Count);
            Assert.AreEqual(8, sgroups[0].Atoms.Count);
        }

        [TestMethod()]
        public void SO3minusFromHeteroCollapseNone()
        {
            Abbreviations factory = new Abbreviations();
            factory.Add("*S(=O)(=O)[O-] SO3-");
            IAtomContainer mol = Smi("c1ccccc1N(S(=O)(=O)[O-])S(=O)(=O)[O-]");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(2, sgroups.Count);
            Assert.AreEqual("SO3-", sgroups[0].Subscript);
            Assert.AreEqual("SO3-", sgroups[1].Subscript);
        }

        [TestMethod()]
        public void HclSaltOfEdci()
        {
            Abbreviations factory = new Abbreviations();
            factory.Add("CCN=C=NCCCN(C)C EDCI");
            IAtomContainer mol = Smi("CCN=C=NCCCN(C)C.Cl");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual("EDCI·HCl", sgroups[0].Subscript);
        }

        [TestMethod()]
        public void SnCl2()
        {
            Abbreviations factory = new Abbreviations();
            IAtomContainer mol = Smi("Cl[Sn]Cl");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual("SnCl2", sgroups[0].Subscript);
        }

        [TestMethod()]
        public void HOOH()
        {
            Abbreviations factory = new Abbreviations();
            IAtomContainer mol = Smi("OO");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual("HOOH", sgroups[0].Subscript);
        }

        [TestMethod()]
        public void LoadFromFile()
        {
            Abbreviations factory = new Abbreviations();
            //Assert.AreEqual(27, factory.LoadFromFile("obabel_superatoms.smi"));
            Assert.AreEqual(27, factory.LoadFromFile("NCDK.Depict.obabel_superatoms.smi"));
        }
    }
}
