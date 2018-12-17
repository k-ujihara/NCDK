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
        private static SmilesParser smipar = CDK.SmilesParser;

        static IAtomContainer Smi(string smi)
        {
            return smipar.ParseSmiles(smi);
        }

        [TestMethod()]
        public void PotassiumCarbonate()
        {
            var mol = Smi("[K+].[O-]C(=O)[O-].[K+]");
            var factory = new Abbreviations { "[K+].[O-]C(=O)[O-].[K+] K2CO3" };
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual("K2CO3", sgroups[0].Subscript);
            Assert.AreEqual(6, sgroups[0].Atoms.Count);
        }

        [TestMethod()]
        public void Phenyl()
        {
            var factory = new Abbreviations { "*c1ccccc1 Ph" };
            var mol = Smi("CCCCCCC(c1ccccc1)(c1ccccc1)c1ccccc1");
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
            var factory = new Abbreviations { "*c1ccccc1 Ph" };
            var mol = Smi("c1ccccc1");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(0, sgroups.Count);
        }

        [TestMethod()]
        public void AvoidOverZealousAbbreviations()
        {
            var factory = new Abbreviations { "*c1ccccc1 Ph" };
            var mol = Smi("Clc1ccccc1");
            Assert.AreEqual(0, factory.Apply(mol));
        }

        [TestMethod()]
        public void PhenylShouldNotMatchC4H6()
        {
            var factory = new Abbreviations { "*c1ccccc1 Ph" };
            var mol = Smi("Oc1ccc(O)cc1");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(0, sgroups.Count);
        }

        [TestMethod()]
        public void PhenylShouldAbbreviateExplicitHydrogens()
        {
            var factory = new Abbreviations { "*c1ccccc1 Ph" };
            var mol = Smi("CCCCc1ccc([H])cc1");
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
            var factory = new Abbreviations { "*c1ccccc1 Ph" };
            var mol = Smi("CCCCC1=CC=CC=C1");
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
            var factory = new Abbreviations
            {
                "*N(=O)(=O) NO2",
                "*[N+]([O-])(=O) NO2"
            };
            var mol = Smi("O=N(=O)CCCC[N+]([O-])=O");
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
            var factory = new Abbreviations
            {
                "*CCC Pr",
                "*CC Et"
            };
            var mol = Smi("c1ccccc1CCC");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual("Pr", sgroups[0].Subscript);
        }

        [TestMethod()]
        public void DontOverwriteExistingSgroups()
        {
            var factory = new Abbreviations { "*CCC Bu" };
            var mol = Smi("c1ccccc1CCC");
            var sgroup = new Sgroup();
            sgroup.Atoms.Add(mol.Atoms[6]);
            sgroup.Atoms.Add(mol.Atoms[7]);
            sgroup.Atoms.Add(mol.Atoms[8]);
            sgroup.Type = SgroupType.CtabAbbreviation;
            sgroup.Subscript = "n-Bu";
            mol.SetCtabSgroups(new[] { sgroup });
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(0, sgroups.Count);
        }
        [TestMethod()]
        public void NHBocFromHeteroCollapse()
        {
            var factory = new Abbreviations { "*C(=O)OC(C)(C)C Boc" };
            var mol = Smi("c1ccccc1NC(=O)OC(C)(C)C");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual("NHBoc", sgroups[0].Subscript);
            Assert.AreEqual(1, sgroups[0].Bonds.Count);
            Assert.AreEqual(8, sgroups[0].Atoms.Count);
        }

        [TestMethod()]
        public void NHBocFromHeteroCollapseExplicitH()
        {
            var factory = new Abbreviations { "*C(=O)OC(C)(C)C Boc" };
            var mol = Smi("c1ccccc1N([H])C(=O)OC(C)(C)C");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual("NHBoc", sgroups[0].Subscript);
            Assert.AreEqual(1, sgroups[0].Bonds.Count);
            Assert.AreEqual(9, sgroups[0].Atoms.Count);
        }

        [TestMethod()]
        public void NBocClFromHeteroCollapseExplicit()
        {
            var factory = new Abbreviations { "*C(=O)OC(C)(C)C Boc" };
            var mol = Smi("c1ccccc1N(Cl)C(=O)OC(C)(C)C");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual("NClBoc", sgroups[0].Subscript);
            Assert.AreEqual(1, sgroups[0].Bonds.Count);
            Assert.AreEqual(9, sgroups[0].Atoms.Count);
        }

        [TestMethod()]
        public void NBoc2FromHeteroCollapse()
        {
            var factory = new Abbreviations { "*C(=O)OC(C)(C)C Boc" };
            var mol = Smi("c1cc2ccccc2cc1N(C(=O)OC(C)(C)C)C(=O)OC(C)(C)C");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual("NBoc2", sgroups[0].Subscript);
            Assert.AreEqual(1, sgroups[0].Bonds.Count);
            Assert.AreEqual(15, sgroups[0].Atoms.Count);
        }

        [TestMethod()]
        public void IPrFromHeteroCollapse()
        {
            var factory = new Abbreviations { "*C(C)C iPr" };
            var mol = Smi("[CH3:27][CH:19]([CH3:28])[C:20]1=[N:26][C:23](=[CH:22][S:21]1)[C:24](=[O:25])O");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual("iPr", sgroups[0].Subscript);
            Assert.AreEqual(1, sgroups[0].Bonds.Count);
            Assert.AreEqual(3, sgroups[0].Atoms.Count);
        }

        [TestMethod()]
        public void NBocFromHeteroCollapseExplicitH()
        {
            var factory = new Abbreviations { "*C(=O)OC(C)(C)C Boc" };
            var mol = Smi("c1cc2ccccc2ccn1C(=O)OC(C)(C)C");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual("NBoc", sgroups[0].Subscript);
            Assert.AreEqual(2, sgroups[0].Bonds.Count);
            Assert.AreEqual(8, sgroups[0].Atoms.Count);
        }

        [TestMethod()]
        public void SO3minusFromHeteroCollapseNone()
        {
            var factory = new Abbreviations { "*S(=O)(=O)[O-] SO3-" };
            var mol = Smi("c1ccccc1N(S(=O)(=O)[O-])S(=O)(=O)[O-]");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(2, sgroups.Count);
            Assert.AreEqual("SO3-", sgroups[0].Subscript);
            Assert.AreEqual("SO3-", sgroups[1].Subscript);
        }

        [TestMethod()]
        public void HclSaltOfEdci()
        {
            var factory = new Abbreviations { "CCN=C=NCCCN(C)C EDCI" };
            var mol = Smi("CCN=C=NCCCN(C)C.Cl");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual("EDCI·HCl", sgroups[0].Subscript);
        }

        [TestMethod()]
        public void SnCl2()
        {
            var factory = new Abbreviations();
            var mol = Smi("Cl[Sn]Cl");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual("SnCl2", sgroups[0].Subscript);
        }

        [TestMethod()]
        public void HOOH()
        {
            var factory = new Abbreviations();
            var mol = Smi("OO");
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual("HOOH", sgroups[0].Subscript);
        }

        [TestMethod()]
        public void MultipleDisconnectedAbbreviations()
        {
            var smi = "ClCCl.Cl[Pd]Cl.[Fe+2].c1ccc(P([c-]2cccc2)c2ccccc2)cc1.c1ccc(P([c-]2cccc2)c2ccccc2)cc1";
            var factory = new Abbreviations
            {
                "ClCCl DCM",
                "Cl[Pd]Cl.[Fe+2].c1ccc(P([c-]2cccc2)c2ccccc2)cc1.c1ccc(P([c-]2cccc2)c2ccccc2)cc1 Pd(dppf)Cl2"
            };
            var mol = Smi(smi);
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual("Pd(dppf)Cl2·DCM", sgroups[0].Subscript);
        }

        [TestMethod()]
        public void MultipleDisconnectedAbbreviations2()
        {
            var smi = "ClCCl.Cl[Pd]Cl.[Fe+2].c1ccc(P([c-]2cccc2)c2ccccc2)cc1.c1ccc(P([c-]2cccc2)c2ccccc2)cc1";
            var factory = new Abbreviations
            {
                "Cl[Pd]Cl.[Fe+2].c1ccc(P([c-]2cccc2)c2ccccc2)cc1.c1ccc(P([c-]2cccc2)c2ccccc2)cc1 Pd(dppf)Cl2",
                "Cl[Pd]Cl PdCl2"
            };
            var mol = Smi(smi);
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual("Pd(dppf)Cl2", sgroups[0].Subscript);
        }

        // Don't generate NiPr
        [TestMethod()]
        public void AvoidAmbiguity()
        {
            var smi = "C1CCCCC1=NC(C)C";
            var factory = new Abbreviations
            {
                "*C(C)C iPr"
            };
            var mol = Smi(smi);
            var sgroups = factory.Generate(mol);
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual("iPr", sgroups[0].Subscript);
        }

        [TestMethod()]
        public void LoadFromFile()
        {
            var factory = new Abbreviations();
            //Assert.AreEqual(27, factory.LoadFromFile("obabel_superatoms.smi"));
            Assert.AreEqual(27, factory.LoadFromFile("NCDK.Depict.obabel_superatoms.smi"));
        }
    }
}
