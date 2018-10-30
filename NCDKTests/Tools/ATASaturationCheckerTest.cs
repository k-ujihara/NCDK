/*  Copyright (C) 2012  Klas Jönsson <klas.joensson@gmail.com>
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT Any WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Config;
using NCDK.Silent;
using NCDK.Smiles;
using NCDK.Tools.Manipulator;

namespace NCDK.Tools
{
    /// <summary>
    /// A test class for the AtomTypeAwereSaturationChecker-class
    /// </summary>
    // @author Klas Jönsson
    // @cdk.created 2012-04-18
    // @cdk.module  test-valencycheck
    [TestClass()]
    public class ATASaturationCheckerTest : CDKTestCase
    {
        private static readonly ISaturationChecker satcheck = CDK.SaturationChecker;

        private static SmilesParser sp = CDK.SilentSmilesParser;
        private AtomTypeAwareSaturationChecker atasc = new AtomTypeAwareSaturationChecker();

        /// <summary>
        /// A test of DecideBondOrder(IAtomContainer) with a molecule we created
        /// from scratch.
        /// </summary>
        [TestMethod()]
        public void TestASimpleCarbonRing()
        {
            // First we create a simple carbon ring to play with...
            var mol = new AtomContainer();
            var carbon = new AtomType(ChemicalElements.Carbon.ToIElement());

            var a0 = new Atom("C")
            {
                Hybridization = Hybridization.SP2,
                ImplicitHydrogenCount = 1
            };
            AtomTypeManipulator.ConfigureUnsetProperties(a0, carbon);
            var a1 = new Atom("C")
            {
                Hybridization = Hybridization.SP2,
                ImplicitHydrogenCount = 1
            };
            AtomTypeManipulator.ConfigureUnsetProperties(a1, carbon);
            var a2 = new Atom("C")
            {
                Hybridization = Hybridization.SP2,
                ImplicitHydrogenCount = 1
            };
            AtomTypeManipulator.ConfigureUnsetProperties(a2, carbon);
            var a3 = new Atom("C")
            {
                Hybridization = Hybridization.SP2,
                ImplicitHydrogenCount = 1
            };
            AtomTypeManipulator.ConfigureUnsetProperties(a3, carbon);
            var a4 = new Atom("C")
            {
                Hybridization = Hybridization.SP2,
                ImplicitHydrogenCount = 1
            };
            AtomTypeManipulator.ConfigureUnsetProperties(a4, carbon);
            var a5 = new Atom("C")
            {
                Hybridization = Hybridization.SP2,
                ImplicitHydrogenCount = 1
            };
            AtomTypeManipulator.ConfigureUnsetProperties(a5, carbon);

            mol.Atoms.Add(a0);
            mol.Atoms.Add(a1);
            mol.Atoms.Add(a2);
            mol.Atoms.Add(a3);
            mol.Atoms.Add(a4);
            mol.Atoms.Add(a5);

            var b0 = new Bond(a0, a1) { IsSingleOrDouble = true };
            mol.Bonds.Add(b0);
            var b1 = new Bond(a1, a2) { IsSingleOrDouble = true };
            mol.Bonds.Add(b1);
            var b2 = new Bond(a2, a3) { IsSingleOrDouble = true };
            mol.Bonds.Add(b2);
            var b3 = new Bond(a3, a4) { IsSingleOrDouble = true };
            mol.Bonds.Add(b3);
            var b4 = new Bond(a4, a5) { IsSingleOrDouble = true };
            mol.Bonds.Add(b4);
            var b5 = new Bond(a5, a0) { IsSingleOrDouble = true };
            mol.Bonds.Add(b5);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);

            // ...then we send it to the method we want to test...
            atasc.DecideBondOrder(mol, false);

            Assert.AreEqual(BondOrder.Double, b0.Order);
            Assert.AreEqual(BondOrder.Single, b1.Order);
            Assert.AreEqual(BondOrder.Double, b2.Order);
            Assert.AreEqual(BondOrder.Single, b3.Order);
            Assert.AreEqual(BondOrder.Double, b4.Order);
            Assert.AreEqual(BondOrder.Single, b5.Order);

            Assert.IsTrue(satcheck.IsSaturated(a0, mol));
        }

        /// <summary>
        /// A test of DecideBondOrder(IAtomContainer) with a molecule we created
        /// from a SMILES.
        /// </summary>
        [TestMethod()]
        public void TestQuinone()
        {
            var mol = sp.ParseSmiles("O=c1ccc(=O)cc1");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);

            atasc.DecideBondOrder(mol, true);

            Assert.IsTrue(mol.Atoms[1].Hybridization == Hybridization.SP2);

            Assert.IsTrue(mol.Bonds[0].End.Symbol.Equals("C"));
            Assert.IsTrue(mol.Bonds[0].Begin.Symbol.Equals("O"));
            Assert.AreEqual(mol.Bonds[0].Order, BondOrder.Double);

            Assert.IsTrue(mol.Bonds[1].Begin.Symbol.Equals("C"));
            Assert.IsTrue(mol.Bonds[1].End.Symbol.Equals("C"));
            Assert.AreEqual(mol.Bonds[1].Order, BondOrder.Single);

            Assert.IsTrue(mol.Bonds[2].Begin.Symbol.Equals("C"));
            Assert.IsTrue(mol.Bonds[2].End.Symbol.Equals("C"));
            Assert.AreEqual(mol.Bonds[2].Order, BondOrder.Double);

            Assert.IsTrue(mol.Bonds[3].Begin.Symbol.Equals("C"));
            Assert.IsTrue(mol.Bonds[3].End.Symbol.Equals("C"));
            Assert.AreEqual(mol.Bonds[3].Order, BondOrder.Single);

            Assert.IsTrue(mol.Bonds[4].End.Symbol.Equals("O"));
            Assert.IsTrue(mol.Bonds[4].Begin.Symbol.Equals("C"));
            Assert.AreEqual(mol.Bonds[4].Order, BondOrder.Double);

            Assert.IsTrue(mol.Bonds[5].Begin.Symbol.Equals("C"));
            Assert.IsTrue(mol.Bonds[5].End.Symbol.Equals("C"));
            Assert.IsTrue(mol.Bonds[5].Order == BondOrder.Single);

            Assert.IsTrue(mol.Bonds[6].Begin.Symbol.Equals("C"));
            Assert.IsTrue(mol.Bonds[6].End.Symbol.Equals("C"));
            Assert.AreEqual(mol.Bonds[6].Order, BondOrder.Double);

            Assert.IsTrue(mol.Bonds[7].Begin.Symbol.Equals("C"));
            Assert.IsTrue(mol.Bonds[7].End.Symbol.Equals("C"));
            Assert.AreEqual(mol.Bonds[7].Order, BondOrder.Single);

            Assert.AreEqual(mol.Bonds[0].End, mol.Bonds[7].Begin);
        }

        /// <summary>
        /// A test of DecideBondOrder(IAtomContainer) with a simple carbon ring we
        /// created from a SMILES.
        /// </summary>
        [TestMethod()]
        public void TestASimpleCarbonRing2()
        {
            var mol = sp.ParseSmiles("c1ccccc1");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);

            atasc.DecideBondOrder(mol, true);

            Assert.AreEqual(mol.Atoms[1].Hybridization, Hybridization.SP2);
            Assert.AreEqual(mol.Bonds[0].Order, BondOrder.Double);
            Assert.AreEqual(mol.Bonds[1].Order, BondOrder.Single);
            Assert.AreEqual(mol.Bonds[2].Order, BondOrder.Double);
            Assert.AreEqual(mol.Bonds[3].Order, BondOrder.Single);
            Assert.AreEqual(mol.Bonds[4].Order, BondOrder.Double);
            Assert.AreEqual(mol.Bonds[5].Order, BondOrder.Single);
        }

        /// <summary>
        /// This method tests the AtomTypeAwareSaturnationChecker with a large ring
        /// system.
        /// </summary>
        [TestMethod()]
        public void TestALargeRingSystem()
        {
            // Should have 13 double bonds.
            var smiles = "O=C1Oc6ccccc6(C(O)C1C5c2ccccc2CC(c3ccc(cc3)c4ccccc4)C5)";
            var mol = sp.ParseSmiles(smiles);

            atasc.DecideBondOrder(mol, true);

            int doubleBondCount = 0;
            foreach (var bond in mol.Bonds)
            {
                if (bond.Order.Equals(BondOrder.Double)) doubleBondCount++;
            }
            Assert.AreEqual(13, doubleBondCount);
        }

        /// <summary>
        /// This do the same as the method above, but with five other large ring
        /// systems.
        /// </summary>
        [TestMethod()]
        public void TestLargeRingSystem1()
        {
            // Should have 6 double bonds
            var smiles = "c1ccc2c(c1)CC4NCCc3cccc2c34";
            var mol = sp.ParseSmiles(smiles);
            atasc.DecideBondOrder(mol, true);

            int doubleBondCount = 0;
            foreach (var bond in mol.Bonds)
            {
                if (bond.Order.Equals(BondOrder.Double))
                    doubleBondCount++;
            }
            Assert.AreEqual(6, doubleBondCount);
        }

        [TestMethod()]
        public void TestLargeRingSystem2()
        {
            // Should have 8 double bonds
            var smiles = "Oc1ccc(cc1)c1coc2c(c1=O)c(O)cc(c2)O";
            var mol = sp.ParseSmiles(smiles);
            atasc.DecideBondOrder(mol, true);

            int doubleBondCount = 0;
            foreach (var bond in mol.Bonds)
            {
                if (bond.Order.Equals(BondOrder.Double)) doubleBondCount++;
            }
            Assert.AreEqual(8, doubleBondCount);
        }

        [TestMethod()]
        public void TestADoubleRingWithANitrogenAtom()
        {
            // Should have 4 double bonds and all three bonds to/from the nitrogen
            // should be single
            string smiles = "c1ccn2cccc2c1";
            var mol = sp.ParseSmiles(smiles);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureUnsetProperties(mol);

            IAtom nitrogen = mol.Atoms[3];

            atasc.DecideBondOrder(mol, true);

            int doubleBondCount = 0, singleBondCount = 0;
            foreach (var bond in mol.Bonds)
            {
                if (bond.Order.Equals(BondOrder.Double)) doubleBondCount++;
                if (bond.Contains(nitrogen)) if (bond.Order.Equals(BondOrder.Single)) singleBondCount++;
            }
            Assert.AreEqual(4, doubleBondCount);
            Assert.AreEqual(3, singleBondCount);
        }

        [TestMethod()]
        public void TestLargeRingSystem3()
        {
            // Should have 17 double bonds
            string smiles = "O=C5C=C(O)C(N=Nc1ccc(cc1)Nc2ccccc2)=CC5(=NNc3ccc(cc3)Nc4ccccc4)";
            var mol = sp.ParseSmiles(smiles);
            atasc.DecideBondOrder(mol, true);

            int doubleBondCount = 0;
            foreach (var bond in mol.Bonds)
            {
                if (bond.Order.Equals(BondOrder.Double)) doubleBondCount++;
            }
            Assert.AreEqual(17, doubleBondCount);
        }

        [TestMethod()]
        public void TestLargeRingSystem4()
        {
            // Should have 18 double bonds
            string smiles = "c1ccc(cc1)[Sn](c2ccccc2)(c3ccccc3)S[Sn](c4ccccc4)(c5ccccc5)c6ccccc6";
            var mol = sp.ParseSmiles(smiles);
            atasc.DecideBondOrder(mol, true);

            int doubleBondCount = 0;
            foreach (var bond in mol.Bonds)
            {
                if (bond.Order.Equals(BondOrder.Double)) doubleBondCount++;
            }
            Assert.AreEqual(18, doubleBondCount);
        }

        [TestMethod()]
        public void TestLargeRingSystem5()
        {
            // Should have 24 double bonds
            string smiles = "O=C1c2ccccc2C(=O)c3c1ccc4c3[nH]c5c6C(=O)c7ccccc7C(=O)c6c8[nH]c9c%10C(=O)c%11ccccc%11C(=O)c%10ccc9c8c45";

            var mol = sp.ParseSmiles(smiles);
            atasc.DecideBondOrder(mol, true);

            int doubleBondCount = 0;
            foreach (var bond in mol.Bonds)
            {
                if (bond.Order.Equals(BondOrder.Double)) doubleBondCount++;
            }
            Assert.AreEqual(24, doubleBondCount);
        }

        /// <summary>
        /// From DeduceBondSystemToolTest
        /// </summary>
        [TestMethod()]
        public void TestLargeBioclipseUseCase()
        {
            // Should have 14 double bonds
            string smiles = "COc1ccc2[C@@H]3[C@H](COc2c1)C(C)(C)OC4=C3C(=O)C(=O)C5=C4OC(C)(C)[C@@H]6COc7cc(OC)ccc7[C@H]56";
            var molecule = sp.ParseSmiles(smiles);

            atasc.DecideBondOrder(molecule, true);

            int doubleBondCount = 0;
            for (int i = 0; i < molecule.Bonds.Count; i++)
            {
                IBond bond = molecule.Bonds[i];
                if (bond.Order == BondOrder.Double) doubleBondCount++;
            }
            Assert.AreEqual(10, doubleBondCount);
        }

        [TestMethod()]
        public void TestCyclobutadiene()
        {
            var mol = sp.ParseSmiles("c1ccc1");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);

            atasc.DecideBondOrder(mol, true);
            Assert.AreEqual(mol.Atoms[1].Hybridization, Hybridization.SP2);
            Assert.AreEqual(BondOrder.Double, mol.Bonds[0].Order);
            Assert.AreEqual(BondOrder.Single, mol.Bonds[1].Order);
            Assert.AreEqual(BondOrder.Double, mol.Bonds[2].Order);
            Assert.AreEqual(BondOrder.Single, mol.Bonds[3].Order);
        }

        [TestMethod()]
        public void TestPyrrole()
        {
            var mol = sp.ParseSmiles("c1c[nH]cc1");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);

            atasc.DecideBondOrder(mol, true);
            Assert.AreEqual(mol.Atoms[1].Hybridization, Hybridization.SP2);
            Assert.AreEqual(BondOrder.Double, mol.Bonds[0].Order);
            Assert.AreEqual(BondOrder.Single, mol.Bonds[1].Order);
            Assert.AreEqual(BondOrder.Single, mol.Bonds[2].Order);
            Assert.AreEqual(BondOrder.Double, mol.Bonds[3].Order);
            Assert.AreEqual(BondOrder.Single, mol.Bonds[4].Order);
        }

        [TestMethod()]
        public void TestFurane()
        {
            var mol = sp.ParseSmiles("c1cocc1");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);

            atasc.DecideBondOrder(mol, true);
            Assert.AreEqual(mol.Atoms[1].Hybridization, Hybridization.SP2);
            Assert.AreEqual(BondOrder.Double, mol.Bonds[0].Order);
            Assert.AreEqual(BondOrder.Single, mol.Bonds[1].Order);
            //bond to oxygen
            Assert.AreEqual(BondOrder.Single, mol.Bonds[2].Order);
            //bond to oxygen
            Assert.AreEqual(BondOrder.Double, mol.Bonds[3].Order);
            Assert.AreEqual(BondOrder.Single, mol.Bonds[4].Order);
        }

        [TestMethod()]
        public void TestAnOtherDoubleRing()
        {
            var mol = sp.ParseSmiles("c1cccc2cccc2c1");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);

            atasc.DecideBondOrder(mol, true);
            Assert.AreEqual(mol.Atoms[1].Hybridization, Hybridization.SP2);
            int doubleBondCount = 0;
            foreach (var bond in mol.Bonds)
            {
                if (bond.Order == BondOrder.Double) doubleBondCount++;
            }
            Assert.AreEqual(5, doubleBondCount);
        }

        [TestMethod()]
        public void TestAnOtherRingSystem()
        {
            // Should have 6 double bonds
            var mol = sp.ParseSmiles("o2c1ccccc1c3c2cccc3");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            atasc.DecideBondOrder(mol, true);
            Assert.AreEqual(mol.Atoms[1].Hybridization, Hybridization.SP2);
            int doubleBondCount = 0;
            foreach (var bond in mol.Bonds)
            {
                if (bond.Order == BondOrder.Double)
                    doubleBondCount++;
            }
            Assert.AreEqual(6, doubleBondCount);
        }

        [TestMethod()]
        public void TestAnOtherRingSystem2()
        {
            // Should have 7 double bonds
            var mol = sp.ParseSmiles("O=c2c1ccccc1c3ccccc23");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            atasc.DecideBondOrder(mol, true);
            Assert.AreEqual(mol.Atoms[1].Hybridization, Hybridization.SP2);
            int doubleBondCount = 0;
            foreach (var bond in mol.Bonds)
            {
                if (bond.Order == BondOrder.Double)
                    doubleBondCount++;
            }
            Assert.AreEqual(7, doubleBondCount);
        }

        [TestMethod()]
        public void TestAzulene()
        {
            var mol = sp.ParseSmiles("c12c(ccccc2)ccc1");
            atasc.DecideBondOrder(mol, true);

            int doubleBondCount = 0;
            foreach (var bond in mol.Bonds)
            {
                if (bond.Order == BondOrder.Double) doubleBondCount++;
            }
            Assert.AreEqual(5, doubleBondCount);

            Assert.AreEqual(BondOrder.Single, mol.Bonds[0].Order);
        }

        [TestMethod()]
        public void MailCase1a()
        {
            var mol = sp.ParseSmiles("o1cccc1");
            atasc.DecideBondOrder(mol, true);

            int doubleBondCount = 0;
            foreach (var bond in mol.Bonds)
            {
                if (bond.Order == BondOrder.Double) doubleBondCount++;
            }
            Assert.AreEqual(2, doubleBondCount);
        }

        [TestMethod()]
        public void MailCase1b()
        {
            var mol = sp.ParseSmiles("O1cccc1");
            atasc.DecideBondOrder(mol, true);

            int doubleBondCount = 0;
            foreach (var bond in mol.Bonds)
            {
                if (bond.Order == BondOrder.Double) doubleBondCount++;
            }
            Assert.AreEqual(2, doubleBondCount);
        }

        [TestMethod()]
        public void MailCase3b()
        {
            var mol = sp.ParseSmiles("c1ccccc1Oc1cOcc1");
            atasc.DecideBondOrder(mol, true);

            int doubleBondCount = 0;
            foreach (var bond in mol.Bonds)
            {
                if (bond.Order == BondOrder.Double) doubleBondCount++;
            }
            Assert.AreEqual(5, doubleBondCount);
        }

        [TestMethod()]
        public void MailCase4()
        {
            var mol = sp.ParseSmiles("o2c1ccccc1c3c2cccc3");
            atasc.DecideBondOrder(mol, true);

            int doubleBondCount = 0;
            foreach (var bond in mol.Bonds)
            {
                if (bond.Order == BondOrder.Double) doubleBondCount++;
            }
            Assert.AreEqual(6, doubleBondCount);
        }

        [TestMethod()]
        public void MailCase5a()
        {
            var mol = sp.ParseSmiles("c5cc2ccc1ccccc1c2c6c4c3ccccc3ccc4ccc56");
            atasc.DecideBondOrder(mol, true);

            int doubleBondCount = 0;
            foreach (var bond in mol.Bonds)
            {
                if (bond.Order == BondOrder.Double) doubleBondCount++;
            }
            Assert.AreEqual(13, doubleBondCount);
        }

        [TestMethod()]
        public void MailCase5b()
        {
            var mol = sp.ParseSmiles("c1cc2ccc3ccc4ccc5ccc6ccc1c7c2c3c4c5c67");

            atasc.DecideBondOrder(mol, true);

            int doubleBondCount = 0;
            foreach (var bond in mol.Bonds)
            {
                if (bond.Order == BondOrder.Double) doubleBondCount++;
            }

            Assert.AreEqual(12, doubleBondCount);
        }

        [TestMethod()]
        public void MailCase6()
        {
            var mol = sp.ParseSmiles("c1ccc2c(c1)cc-3c4c2cccc4-c5c3cccc5");
            atasc.DecideBondOrder(mol, true);

            int doubleBondCount = 0;
            foreach (var bond in mol.Bonds)
            {
                if (bond.Order == BondOrder.Double) doubleBondCount++;
            }
            Assert.AreEqual(10, doubleBondCount);
        }

        [TestMethod()]
        public void TestNPolycyclicCompounds()
        {
            var mol = sp.ParseSmiles("n12cncc1cccc2");
            atasc.DecideBondOrder(mol, true);

            int doubleBondCount = 0;
            foreach (var bond in mol.Bonds)
            {
                if (bond.Order == BondOrder.Double) doubleBondCount++;
            }
            Assert.AreEqual(4, doubleBondCount);
        }

        [TestMethod()]
        public void TestIndoles2()
        {
            var mol = sp.ParseSmiles("Cl.Cl.Oc1ccc2CC3[C@](Cc4c(-c5ccccc5)c(C)[nH0](Cc5ccccc5)c4[C@@H]([C@](CCN3CC3CC3)(c2c1O1)2)1)2(O)");
            atasc.DecideBondOrder(mol, true);

            int doubleBondCount = 0;
            foreach (var bond in mol.Bonds)
            {
                if (bond.Order == BondOrder.Double) doubleBondCount++;
            }
            Assert.AreEqual(11, doubleBondCount);
        }

        [TestMethod()]
        public void SomeOtherWieredDoubleRing()
        {
            var mol = sp.ParseSmiles("CCc2c3ccccc3[nH]c2");
            atasc.DecideBondOrder(mol, true);

            int doubleBondCount = 0;
            foreach (var bond in mol.Bonds)
                if (bond.Order == BondOrder.Double) doubleBondCount++;

            Assert.AreEqual(4, doubleBondCount);
        }

        [TestMethod()]
        public void TestButadieneSmile()
        {
            var mol = sp.ParseSmiles("cccc");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            // The SMILES-parser does not seams raise the SingleOrDouble-flag if a
            // molecule don't have any rings
            foreach (var bond in mol.Bonds)
                bond.IsSingleOrDouble = true;
            
            atasc.DecideBondOrder(mol, false);

            int doubleBondCount = 0;
            foreach (var bond in mol.Bonds)
            {
                if (bond.Order == BondOrder.Double) doubleBondCount++;
            }
            Assert.AreEqual(2, doubleBondCount);
        }

        [TestMethod()]
        public void TestButadiene()
        {
            var mol = new AtomContainer();
            var carbon = new AtomType(ChemicalElements.Carbon.ToIElement());

            var a0 = new Atom("C")
            {
                Hybridization = Hybridization.SP2,
                ImplicitHydrogenCount = 2
            };
            AtomTypeManipulator.ConfigureUnsetProperties(a0, carbon);
            var a1 = new Atom("C")
            {
                Hybridization = Hybridization.SP2,
                ImplicitHydrogenCount = 1
            };
            AtomTypeManipulator.ConfigureUnsetProperties(a1, carbon);
            var a2 = new Atom("C")
            {
                Hybridization = Hybridization.SP2,
                ImplicitHydrogenCount = 1
            };
            AtomTypeManipulator.ConfigureUnsetProperties(a2, carbon);
            var a3 = new Atom("C")
            {
                Hybridization = Hybridization.SP2,
                ImplicitHydrogenCount = 2
            };
            AtomTypeManipulator.ConfigureUnsetProperties(a3, carbon);

            mol.Atoms.Add(a0);
            mol.Atoms.Add(a1);
            mol.Atoms.Add(a2);
            mol.Atoms.Add(a3);

            var b0 = new Bond(a0, a1) { IsSingleOrDouble = true };
            mol.Bonds.Add(b0);
            var b1 = new Bond(a1, a2) { IsSingleOrDouble = true };
            mol.Bonds.Add(b1);
            IBond b2 = new Bond(a2, a3) { IsSingleOrDouble = true };
            mol.Bonds.Add(b2);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);

            atasc.DecideBondOrder(mol, true);

            Assert.AreEqual(BondOrder.Double, mol.Bonds[0].Order);
            Assert.AreEqual(BondOrder.Single, mol.Bonds[1].Order);
            Assert.AreEqual(BondOrder.Double, mol.Bonds[2].Order);
        }

        [TestMethod()]
        public void TestMolFromSdf()
        {
            var mol = sp.ParseSmiles("OC(COc1ccccc1CC=C)CNC(C)C");
            atasc.DecideBondOrder(mol, true);

            int doubleBondCount = 0;
            foreach (var bond in mol.Bonds)
            {
                if (bond.Order == BondOrder.Double)
                    doubleBondCount++;
            }
            Assert.AreEqual(4, doubleBondCount);
        }

        [TestMethod()]
        public void TestOnlyOneAtom()
        {
            // If all bonds in the molecule are implicit, then it was noticed that the SatChecker failed
            var mol = sp.ParseSmiles("C");

            int preBondCount = mol.Bonds.Count;
            atasc.DecideBondOrder(mol);

            Assert.AreEqual(preBondCount, mol.Bonds.Count);
        }

        [TestMethod()]
        public void TestBug3394()
        {
            IAtomContainer mol;
            try
            {
                mol = sp.ParseSmiles("OCC1OC(O)C(O)C(Op2(OC3C(O)C(O)OC(CO)C3O)np(OC4C(O)C(O)OC(CO)C4O)(OC5C(O)C(O)OC(CO)C5O)np(OC6C(O)C(O)OC(CO)C6O)(OC7C(O)C(O)OC(CO)C7O)n2)C1O");
                atasc.DecideBondOrder(mol);
            }
            catch (InvalidSmilesException)
            {
                Assert.Fail("SMILES failed");
            }
            catch (CDKException)
            {
                Assert.Fail("ATASatChecer failed");
            }
        }
    }
}
