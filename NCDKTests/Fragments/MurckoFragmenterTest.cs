/*
 * Copyright (C) 2010 Rajarshi Guha <rajarshi.guha@gmail.com>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
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
using NCDK.Aromaticities;
using NCDK.Smiles;
using NCDK.Templates;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System.Linq;

namespace NCDK.Fragments
{
    /// <summary>
    /// Test Murcko fragmenter.
    /// </summary>
    // @cdk.module test-fragment
    [TestClass()]
    public class MurckoFragmenterTest : CDKTestCase
    {
        static MurckoFragmenter fragmenter = new MurckoFragmenter();
        static SmilesParser smilesParser = CDK.SilentSmilesParser;

        [TestMethod()]
        public void TestNoFramework()
        {
            var mol = smilesParser.ParseSmiles("CCO[C@@H](C)C(=O)C(O)O");
            fragmenter.GenerateFragments(mol);
            var frameworks = fragmenter.GetFrameworks();
            Assert.AreEqual(0, frameworks.Count());
        }

        [TestMethod()]
        public void TestOnlyRingSystem()
        {
            var mol = smilesParser.ParseSmiles("c1ccccc1CCCCC");
            fragmenter.GenerateFragments(mol);
            var frameworks = fragmenter.GetFrameworks();
            Assert.AreEqual(0, frameworks.Count());
            var rings = fragmenter.GetRingSystems();
            Assert.AreEqual(1, rings.Count());
        }

        [TestMethod()]
        public void TestMF3()
        {
            var mol = smilesParser.ParseSmiles("C(CC1=C2C=CC=CC2=CC2=C1C=CC=C2)C1CCCCC1");
            fragmenter.GenerateFragments(mol);
            var frameworks = fragmenter.GetFrameworks();
            Assert.AreEqual(1, frameworks.Count());
        }

        [TestMethod()]
        public void TestMF3_Container()
        {
            var mol = smilesParser.ParseSmiles("C(CC1=C2C=CC=CC2=CC2=C1C=CC=C2)C1CCCCC1");
            fragmenter.GenerateFragments(mol);
            var frameworks = fragmenter.GetFrameworksAsContainers();
            Assert.AreEqual(1, frameworks.Count());
        }

        [TestMethod()]
        public void TestMF1()
        {
            var mol = smilesParser.ParseSmiles("c1ccccc1PP(B)c1cccc(N(N)N)c1SC1CCC1");
            var fragmenter = new MurckoFragmenter(false, 2);

            fragmenter.GenerateFragments(mol);
            var frameworks = fragmenter.GetFrameworks();
            Assert.AreEqual(3, frameworks.Count());

            var rings = fragmenter.GetRingSystems();
            Assert.AreEqual(2, rings.Count());
        }

        [TestMethod()]
        public void TestMF1_Container()
        {
            var mol = smilesParser.ParseSmiles("c1ccccc1PP(B)c1cccc(N(N)N)c1SC1CCC1");
            var fragmenter = new MurckoFragmenter(false, 2);

            fragmenter.GenerateFragments(mol);
            var frameworks = fragmenter.GetFrameworksAsContainers();
            Assert.AreEqual(3, frameworks.Count());

            var rings = fragmenter.GetRingSystemsAsContainers();
            Assert.AreEqual(2, rings.Count());
        }

        [TestMethod()]
        public void TestMF2()
        {
            var mol = smilesParser.ParseSmiles("C1(c2ccccc2)(CC(CC1)CCc1ccccc1)CC1C=CC=C1");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            fragmenter.GenerateFragments(mol);

            var rings = fragmenter.GetRingSystems();
            Assert.AreEqual(3, rings.Count());

            var frameworks = fragmenter.GetFrameworks().ToReadOnlyList();
            Assert.AreEqual(7, frameworks.Count);

            var ar = new[] { "c1ccc(cc1)CCC2CCC(CC3C=CC=C3)C2", "c1ccc(cc1)CCC2CCCC2", "C=1C=CC(C1)CC2CCCC2",
                        "c1ccc(cc1)C2(CCCC2)CC3C=CC=C3", "c1ccc(cc1)CCC2CCC(c3ccccc3)(CC4C=CC=C4)C2",
                        "c1ccc(cc1)C2CCCC2", "c1ccc(cc1)CCC2CCC(c3ccccc3)C2" };
            foreach (var s in ar)
                Assert.IsTrue(frameworks.Contains(s));
        }

        [TestMethod()]
        public void TestSingleFramework()
        {
            var mol = smilesParser.ParseSmiles("C1(c2ccccc2)(CC(CC1)CCc1ccccc1)CC1C=CC=C1");
            var fragmenter = new MurckoFragmenter(true, 6);
            fragmenter.GenerateFragments(mol);

            var frameworks = fragmenter.GetFrameworks();
            Assert.AreEqual(1, frameworks.Count());
        }

        [TestMethod()]
        public void TestMF4()
        {
            var mol = smilesParser.ParseSmiles("c1ccc(cc1)c2c(oc(n2)N(CCO)CCO)c3ccccc3");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            fragmenter.GenerateFragments(mol);

            var frameworks = fragmenter.GetFrameworks().ToReadOnlyList();
            Assert.AreEqual(3, frameworks.Count);
            var ar = new[] { "n1coc(c1)-c2ccccc2", "n1coc(-c2ccccc2)c1-c3ccccc3", "n1cocc1-c2ccccc2" };
            foreach (var s in ar)
                Assert.IsTrue(frameworks.Contains(s));
        }

        [TestMethod()]
        public void TestMF5()
        {
            var mol = smilesParser.ParseSmiles("c1cc(ccc1C(=O)Nc2ccc3c(c2)nc(o3)c4ccncc4)F");
            fragmenter.GenerateFragments(mol);
            var frameworks = fragmenter.GetFrameworks();
            Assert.AreEqual(3, frameworks.Count());

            var rings = fragmenter.GetRingSystems();
            Assert.AreEqual(3, rings.Count());
        }

        [TestMethod()]
        public void TestMF6()
        {
            var mol = smilesParser.ParseSmiles("COc1ccc(cc1OCc2ccccc2)C(=S)N3CCOCC3");
            fragmenter.GenerateFragments(mol);

            var frameworks = fragmenter.GetFrameworks();
            Assert.AreEqual(3, frameworks.Count());

            var rings = fragmenter.GetRingSystems();
            Assert.AreEqual(2, rings.Count());
        }

        [TestMethod()]
        public void TestMF7()
        {
            var mol = smilesParser.ParseSmiles("Cc1nnc(s1)N[C@H](C(=O)c2ccccc2)NC(=O)c3ccco3");
            fragmenter.GenerateFragments(mol);

            var frameworks = fragmenter.GetFrameworks();
            Assert.AreEqual(4, frameworks.Count());

            var rings = fragmenter.GetRingSystems();
            Assert.AreEqual(3, rings.Count());
        }

        // @cdk.bug 1848591
        [TestMethod()]
        public void TestBug1848591()
        {
            var mol = smilesParser.ParseSmiles("c1(ccc(cc1C)CCC(C(CCC)C2C(C2)CC)C3C=C(C=C3)CC)C");
            var fragmenter = new MurckoFragmenter(true, 6);
            fragmenter.GenerateFragments(mol);

            var frameworks = fragmenter.GetFrameworks();
            Assert.AreEqual(1, frameworks.Count());
            Assert.AreEqual("c1ccc(cc1)CCC(CC2CC2)C3C=CC=C3", frameworks.ElementAt(0));
        }

        // @cdk.bug 3088164
        [TestMethod()]
        public void TestCarbinoxamine_Bug3088164()
        {
            var mol = smilesParser.ParseSmiles("CN(C)CCOC(C1=CC=C(Cl)C=C1)C1=CC=CC=N1");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            var fragmenter = new MurckoFragmenter(true, 6);
            fragmenter.GenerateFragments(mol);

            var f = fragmenter.GetFrameworks().ToReadOnlyList();
            var fc = fragmenter.GetFrameworksAsContainers().ToReadOnlyList();
            Assert.AreEqual(1, f.Count);
            Assert.AreEqual(f.Count, fc.Count);
            Assert.AreEqual("n1ccccc1Cc2ccccc2", f[0]);

            var sg = SmilesGenerator.Unique().Aromatic();
            for (int i = 0; i < f.Count; i++)
            {
                Aromaticity.CDKLegacy.Apply(fc[i]);
                string newsmiles = sg.Create(fc[i]);
                Assert.IsTrue(f[i].Equals(newsmiles), f[i] + " did not match the container, " + newsmiles);
            }
        }

        // @cdk.bug 3088164
        [TestMethod()]
        public void TestPirenperone_Bug3088164()
        {
            var sg = SmilesGenerator.Unique().Aromatic();

            var mol = smilesParser.ParseSmiles("Fc1ccc(cc1)C(=O)C4CCN(CCC\\3=C(\\N=C2\\C=C/C=C\\N2C/3=O)C)CC4");
            AtomContainerManipulator.ClearAtomConfigurations(mol);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            var fragmenter = new MurckoFragmenter(true, 6);
            fragmenter.GenerateFragments(mol);

            var f = fragmenter.GetFrameworks().ToReadOnlyList();
            var fc = fragmenter.GetFrameworksAsContainers().ToReadOnlyList();

            Assert.AreEqual(1, f.Count);
            Assert.AreEqual(f.Count, fc.Count);

            AtomContainerManipulator.ClearAtomConfigurations(mol);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            CDK.HydrogenAdder.AddImplicitHydrogens(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            Assert.AreEqual("N=1C=C(CN2C=CC=CC12)CCN3CCC(Cc4ccccc4)CC3", f[0]);

            for (int i = 0; i < f.Count; i++)
            {
                string newsmiles = sg.Create(fc[i]);
                Assert.IsTrue(f[i].Equals(newsmiles), f[i] + " did not match the container, " + newsmiles);
            }
        }

        // @cdk.bug 3088164
        [TestMethod()]
        public void TestIsomoltane_Bug3088164()
        {
            var sg = SmilesGenerator.Unique().Aromatic();

            var mol = smilesParser.ParseSmiles("CC(C)NCC(O)COC1=C(C=CC=C1)N1C=CC=C1");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            var fragmenter = new MurckoFragmenter(true, 6);
            fragmenter.GenerateFragments(mol);

            var f = fragmenter.GetFrameworks().ToReadOnlyList();
            var fc = fragmenter.GetFrameworksAsContainers().ToReadOnlyList();
            Assert.AreEqual(1, f.Count);
            Assert.AreEqual(f.Count, fc.Count);
            Assert.AreEqual("c1ccc(cc1)-n2cccc2", f[0]);

            for (int i = 0; i < f.Count; i++)
            {
                Aromaticity.CDKLegacy.Apply(fc[i]);
                string newsmiles = sg.Create(fc[i]);
                Assert.IsTrue(f[i].Equals(newsmiles), f[i] + " did not match the container, " + newsmiles);
            }
        }

        [TestMethod()]
        public void TestGetFragmentsAsContainers()
        {
            var biphenyl = TestMoleculeFactory.MakeBiphenyl();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(biphenyl);
            Aromaticity.CDKLegacy.Apply(biphenyl);

            var fragmenter = new MurckoFragmenter(true, 6);
            fragmenter.GenerateFragments(biphenyl);
            var fragments = fragmenter.GetFragmentsAsContainers().ToReadOnlyList();

            Assert.AreEqual(2, fragments.Count);
            Assert.AreEqual(12, fragments[0].Atoms.Count);
            Assert.AreEqual(6, fragments[1].Atoms.Count);
        }

        /// <summary>
        /// Test for large branched, symmetric molecule.
        /// </summary>
        // @cdk.inchi InChI=1S/C76H52O46/c77-32-1-22(2-33(78)53(32)92)67(103)113-47-16-27(11-42(87)58(47)97)66(102)112-21-52-63(119-72(108)28-12-43(88)59(98)48(17-28)114-68(104)23-3-34(79)54(93)35(80)4-23)64(120-73(109)29-13-44(89)60(99)49(18-29)115-69(105)24-5-36(81)55(94)37(82)6-24)65(121-74(110)30-14-45(90)61(100)50(19-30)116-70(106)25-7-38(83)56(95)39(84)8-25)76(118-52)122-75(111)31-15-46(91)62(101)51(20-31)117-71(107)26-9-40(85)57(96)41(86)10-26/h1-20,52,63-65,76-101H,21H2
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestMacrocycle()
        {
            var mol = smilesParser.ParseSmiles("C1=C(C=C(C(=C1O)O)O)C(=O)OC2=CC(=CC(=C2O)O)C(=O)OCC3C(C(C(C(O3)OC(=O)C4=CC(=C(C(=C4)OC(=O)C5=CC(=C(C(=C5)O)O)O)O)O)OC(=O)C6=CC(=C(C(=C6)OC(=O)C7=CC(=C(C(=C7)O)O)O)O)O)OC(=O)C8=CC(=C(C(=C8)OC(=O)C9=CC(=C(C(=C9)O)O)O)O)O)OC(=O)C1=CC(=C(C(=C1)OC(=O)C1=CC(=C(C(=C1)O)O)O)O)O");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            var fragmenter = new MurckoFragmenter(true, 6);
            fragmenter.GenerateFragments(mol);

            var f = fragmenter.GetFrameworks();
            Assert.AreEqual(1, f.Count());
            var rs = fragmenter.GetRingSystems();
            Assert.AreEqual(2, rs.Count());
            var fs = fragmenter.GetFragments();
            Assert.AreEqual(3, fs.Count());
        }

        /// <see href="https://github.com/cdk/cdk/issues/263">GitHub Issue #263</see>
        [TestMethod()]
        public void TestCHEMBL529226()
        {
            var smiles = "CC1=CN([C@@H]2O[C@@]3(COP(=S)(O)O[C@H]4[C@H]5OC[C@]4(COP(=S)(O)O[C@H]6C[C@@H](O[C@@H]6COP(=S)(O)O[C@H]7[C@@H](O)[C@@H](O[C@@H]7COP(=S)(O)O[C@H]8[C@@H](O)[C@@H](O[C@@H]8COP(=S)(O)O[C@H]9[C@@H](O)[C@@H](O[C@@H]9COP(=S)(O)O[C@H]%10[C@@H](O)[C@@H](O[C@@H]%10COP(=S)(O)O[C@H]%11[C@@H](O)[C@@H](O[C@@H]%11COP(=S)(O)O[C@H]%12[C@@H](O)[C@@H](O[C@@H]%12COP(=S)(O)O[C@H]%13[C@@H](O)[C@@H](O[C@@H]%13COP(=S)(O)O[C@H]%14[C@@H](O)[C@@H](O[C@@H]%14COP(=S)(O)O[C@H]%15[C@@H](O)[C@@H](O[C@@H]%15COP(=S)(O)O[C@H]%16[C@H]%17OC[C@]%16(COP(=S)(O)O[C@H]%18[C@H]%19OC[C@]%18(CO)O[C@H]%19N%20C=C(C)C(=O)NC%20=O)O[C@H]%17N%21C=C(C)C(=NC%21=O)N)N%22C=CC(=NC%22=O)N)n%23cnc%24C(=O)NC(=Nc%23%24)N)n%25cnc%26C(=O)NC(=Nc%25%26)N)N%27C=C(C)C(=O)NC%27=O)N%28C=CC(=NC%28=O)N)n%29cnc%30c(N)ncnc%29%30)N%31C=CC(=NC%31=O)N)n%32cnc%33C(=O)NC(=Nc%32%33)N)n%34cnc%35C(N)NC=Nc%34%35)N%36C=C(C)C(=O)NC%36=O)O[C@H]5N%37C=C(C)C(=O)NC%37=O)CO[C@@H]2[C@@H]3O)C(=O)N=C1N";
            var smipar = CDK.SilentSmilesParser;
            var mol = smipar.ParseSmiles(smiles);
            var fragmenter = new MurckoFragmenter(true, 6);
            fragmenter.GenerateFragments(mol);
            Assert.AreEqual(1, fragmenter.GetFrameworks().Count());
        }
    }
}
