/* Copyright (C) 2007  Miguel Rojasch <miguelrojasch@users.sf.net>
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
using NCDK.Config;
using NCDK.Formula;
using NCDK.IO;
using NCDK.Templates;
using System;
using System.IO;
using System.Linq;

namespace NCDK.Tools.Manipulator
{
    /// <summary>
    /// Checks the functionality of the MolecularFormulaManipulator.
    /// </summary>
    // @cdk.module test-formula
    [TestClass()]
    public class MolecularFormulaManipulatorTest 
        : CDKTestCase
    {
        private readonly static IChemObjectBuilder builder = CDK.Builder;
        private IsotopeFactory ifac;

        /// <summary>
        ///  Constructor for the MolecularFormulaManipulatorTest object.
        /// </summary>
        public MolecularFormulaManipulatorTest()
            : base()
        {
            try
            {
                ifac = BODRIsotopeFactory.Instance;
            }
            catch (IOException e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }
        }

        /// <summary>Test atom and isotope count for methyl-group.</summary>
        [TestMethod()]
        public void TestGetAtomCount_IMolecularFormula()
        {
            var formula = new MolecularFormula();
            formula.Add(builder.NewIsotope("C"));
            formula.Add(builder.NewIsotope("H"), 3);
            Assert.AreEqual(2, formula.Isotopes.Count());
            Assert.AreEqual(4, MolecularFormulaManipulator.GetAtomCount(formula));
        }

        /// <summary>
        /// Test molecular formula's generated from IIsotopes, including hydrogen/deuterium handling.
        /// </summary>
        [TestMethod()]
        public void TestGetElementCount_IMolecularFormula_IElement()
        {
            var formula = new MolecularFormula();
            var carb = builder.NewIsotope("C");
            var flu = builder.NewIsotope("F");
            var h1 = builder.NewIsotope("H");
            var h2 = builder.NewIsotope("H");
            h2.ExactMass = 2.014101778;
            formula.Add(carb, 2);
            formula.Add(flu);
            formula.Add(h1, 3);
            formula.Add(h2, 4);

            Assert.AreEqual(10, MolecularFormulaManipulator.GetAtomCount(formula));
            Assert.AreEqual(4, formula.Isotopes.Count());
            Assert.AreEqual(3, formula.GetCount(h1));
            Assert.AreEqual(4, formula.GetCount(h2));

            Assert.AreEqual(2, MolecularFormulaManipulator.GetElementCount(formula, carb.Element));
            Assert.AreEqual(1, MolecularFormulaManipulator.GetElementCount(formula, flu.Element));
            Assert.AreEqual(7, MolecularFormulaManipulator.GetElementCount(formula, h1.Element));
        }

        /// <summary>
        /// Test.Isotopes for hydrogen/deuterium.
        /// </summary>
        [TestMethod()]
        public void TestGetIsotopes_IMolecularFormula_IElement()
        {
            var formula = new MolecularFormula();
            var carb = builder.NewIsotope("C");
            var flu = builder.NewIsotope("F");
            var h1 = builder.NewIsotope("H");
            var h2 = builder.NewIsotope("H");
            h2.ExactMass = 2.014101778;
            formula.Add(carb, 1);
            formula.Add(flu);
            formula.Add(h1, 1);
            formula.Add(h2, 2);

            var isotopes = MolecularFormulaManipulator.GetIsotopes(formula, ChemicalElement.H);
            Assert.AreEqual(2, isotopes.Count());
        }

        [TestMethod()]
        public void TestContainsElement_IMolecularFormula_IElement()
        {
            var formula = new MolecularFormula();
            var carb = builder.NewIsotope("C");
            var flu = builder.NewIsotope("F");
            var h1 = builder.NewIsotope("H");
            var h2 = builder.NewIsotope("H");
            h2.ExactMass = 2.014101778;
            formula.Add(carb, 1);
            formula.Add(flu);
            formula.Add(h1, 1);
            formula.Add(h2, 2);

            Assert.IsTrue(MolecularFormulaManipulator.ContainsElement(formula, ChemicalElement.C));
            Assert.IsTrue(MolecularFormulaManipulator.ContainsElement(formula, ChemicalElement.H));
            Assert.IsTrue(MolecularFormulaManipulator.ContainsElement(formula, ChemicalElement.F));
        }

        [TestMethod()]
        public void TestGetString_IMolecularFormula_Empty()
        {
            var stringMF = MolecularFormulaManipulator.GetString(new MolecularFormula());
            Assert.IsNotNull(stringMF);
            Assert.AreEqual("", stringMF);
        }

        /// <summary>Test if formula re-ordering to a user-specified element order works</summary>
        [TestMethod()]
        public void TestGetString_IMolecularFormula_arrayString_boolean()
        {
            var formula = new MolecularFormula();
            formula.Add(builder.NewIsotope("C"), 2);
            formula.Add(builder.NewIsotope("H"), 2);
            Assert.AreEqual("C2H2", MolecularFormulaManipulator.GetString(formula));

            var newOrder = new string[2];
            newOrder[0] = "H";
            newOrder[1] = "C";

            Assert.AreEqual("H2C2", MolecularFormulaManipulator.GetString(formula, newOrder, true));
        }

        /// <summary>Test if isotope-list re-ordering to a user-specified element order works</summary>
        [TestMethod()]
        public void TestPutInOrder_arrayString_IMolecularFormula()
        {
            var formula = new MolecularFormula();
            formula.Add(builder.NewIsotope("C"), 2);
            formula.Add(builder.NewIsotope("H"), 2);

            var newOrder = new string[2];
            newOrder[0] = "H";
            newOrder[1] = "C";

            var list = MolecularFormulaManipulator.PutInOrder(newOrder, formula);
            Assert.AreEqual("H", list[0].Symbol);
            Assert.AreEqual("C", list[1].Symbol);

            newOrder = new string[2];
            newOrder[0] = "C";
            newOrder[1] = "H";

            list = MolecularFormulaManipulator.PutInOrder(newOrder, formula);
            Assert.AreEqual("C", list[0].Symbol);
            Assert.AreEqual("H", list[1].Symbol);
        }

        [TestMethod()]
        public void TestGetString__String_IMolecularFormula()
        {
            Assert.IsNotNull(MolecularFormulaManipulator.GetMolecularFormula("C10H16", new MolecularFormula()));
            Assert.IsNotNull(MolecularFormulaManipulator.GetMolecularFormula("C10H16", builder));
        }

        /// <summary>Test if formula-order is independent of isotope-insertion order</summary>
        [TestMethod()]
        public void TestGetString_IMolecularFormula()
        {
            var mf1 = new MolecularFormula();
            mf1.Add(builder.NewIsotope("C"), 10);
            mf1.Add(builder.NewIsotope("H"), 16);

            Assert.AreEqual("C10H16", MolecularFormulaManipulator.GetString(mf1));

            var mf2 = new MolecularFormula();
            mf2.Add(builder.NewAtom("H"), 16);
            mf2.Add(builder.NewAtom("C"), 10);

            Assert.AreEqual("C10H16", MolecularFormulaManipulator.GetString(mf2));

            Assert.AreEqual(MolecularFormulaManipulator.GetString(mf2), MolecularFormulaManipulator.GetString(mf1));
        }

        /// <summary>
        // @cdk.bug 2276507
        /// </summary>
        [TestMethod()]
        public void TestBug2276507()
        {
            var mf1 = new MolecularFormula();
            mf1.Add(builder.NewIsotope("C"), 1);
            mf1.Add(builder.NewIsotope("H"), 4);

            Assert.AreEqual("CH4", MolecularFormulaManipulator.GetString(mf1));
        }

        /// <summary>
        /// Test setOne parameter for <see cref="MolecularFormulaManipulator.GetString(IMolecularFormula, bool)"/>
        /// </summary>
        [TestMethod()]
        public void TestGetString_IMolecularFormula_boolean()
        {
            var mf1 = new MolecularFormula();
            mf1.Add(builder.NewIsotope("C"), 1);
            mf1.Add(builder.NewIsotope("H"), 4);

            Assert.AreEqual("C1H4", MolecularFormulaManipulator.GetString(mf1, true));
        }

        /// <summary>Test if formulae group elements when not inserted simultaneously</summary>
        [TestMethod()]
        public void TestGetString_Isotopes()
        {
            var mf1 = new MolecularFormula();
            mf1.Add(builder.NewIsotope("C", 12), 9);
            mf1.Add(builder.NewIsotope("C", 13), 1);
            mf1.Add(builder.NewIsotope("H"), 16);

            Assert.AreEqual("C10H16", MolecularFormulaManipulator.GetString(mf1));
        }

        [TestMethod()]
        public void TestGetMolecularFormula_String_IChemObjectBuilder()
        {
            var molecularFormula = MolecularFormulaManipulator.GetMolecularFormula("C10H16", builder);

            Assert.AreEqual(26, MolecularFormulaManipulator.GetAtomCount(molecularFormula));
            Assert.AreEqual(2, molecularFormula.Isotopes.Count());
        }

        /// <summary>Test formula summing</summary>
        [TestMethod()]
        public void TestGetMolecularFormula_String_IMolecularFormula()
        {
            var mf1 = new MolecularFormula();
            mf1.Add(builder.NewIsotope("C"), 10);
            mf1.Add(builder.NewIsotope("H"), 16);

            Assert.AreEqual(26, MolecularFormulaManipulator.GetAtomCount(mf1));
            Assert.AreEqual(2, mf1.Isotopes.Count());

            var mf2 = MolecularFormulaManipulator.GetMolecularFormula("C11H17", mf1);

            Assert.AreEqual(54, MolecularFormulaManipulator.GetAtomCount(mf2));
            Assert.AreEqual(2, mf2.Isotopes.Count());
        }

        /// <summary>Test formula mass calculation</summary>
        [TestMethod()]
        public void TestGetMajorIsotopeMolecularFormula_String_IChemObjectBuilder()
        {
            var mf2 = MolecularFormulaManipulator.GetMajorIsotopeMolecularFormula("C11H17", builder);

            Assert.AreEqual(28, MolecularFormulaManipulator.GetAtomCount(mf2));
            Assert.AreEqual(2, mf2.Isotopes.Count());
            var carbon = BODRIsotopeFactory.Instance.GetMajorIsotope("C");
            var hydrogen = BODRIsotopeFactory.Instance.GetMajorIsotope("H");
            double totalMass = carbon.ExactMass.Value * 11;
            totalMass += hydrogen.ExactMass.Value * 17;
            Assert.AreEqual(totalMass, MolecularFormulaManipulator.GetTotalExactMass(mf2), 0.0000001);
        }

        /// <summary>test <see cref="MolecularFormulaManipulator.RemoveElement(IMolecularFormula, IElement)"/></summary>
        [TestMethod()]
        public void TestRemoveElement_IMolecularFormula_IElement()
        {
            IMolecularFormula formula;

            formula = new MolecularFormula();
            formula.Add(builder.NewIsotope("C"), 1);
            var fl = builder.NewIsotope("F");
            var hy2 = builder.NewIsotope("H");
            var hy1 = builder.NewIsotope("H");
            hy2.ExactMass = 2.014101778;
            formula.Add(fl, 1);
            formula.Add(hy1, 2);
            formula.Add(hy2, 1);

            Assert.AreEqual(4, formula.Isotopes.Count());

            formula = MolecularFormulaManipulator.RemoveElement(formula, ChemicalElement.F);

            Assert.AreEqual(3, formula.Isotopes.Count());
            Assert.AreEqual(4, MolecularFormulaManipulator.GetAtomCount(formula));

            formula = MolecularFormulaManipulator.RemoveElement(formula, ChemicalElement.H);

            Assert.AreEqual(1, MolecularFormulaManipulator.GetAtomCount(formula));
            Assert.AreEqual(1, formula.Isotopes.Count());
        }

        /// <summary>
        /// Test total Exact Mass.
        /// </summary>
        [TestMethod()]
        public void TestGetTotalExactMass_IMolecularFormula()
        {
            var formula = new MolecularFormula();
            IIsotope carb = builder.NewIsotope("C");
            carb.ExactMass = 12.00;
            IIsotope cl = builder.NewIsotope("Cl");
            cl.ExactMass = 34.96885268;

            formula.Add(carb);
            formula.Add(cl);

            double totalExactMass = MolecularFormulaManipulator.GetTotalExactMass(formula);

            Assert.AreEqual(46.96885268, totalExactMass, 0.000001);
        }

        /// <summary>
        /// Test total Exact Mass.
        /// </summary>
        [TestMethod()]
        public void TestGetTotalExactMassWithCharge_IMolecularFormula()
        {
            var formula = MolecularFormulaManipulator.GetMajorIsotopeMolecularFormula("CH5O", builder);

            double totalExactMass = MolecularFormulaManipulator.GetTotalExactMass(formula);
            Assert.AreEqual(33.034040, totalExactMass, 0.0001);

            formula.Charge = 1;
            double totalExactMass2 = MolecularFormulaManipulator.GetTotalExactMass(formula);
            Assert.AreEqual(33.03349, totalExactMass2, 0.0001);
        }

        /// <summary>
        /// Test total Exact Mass.
        /// </summary>
        [TestMethod()]
        public void TestGetTotalExactMassWithChargeNeg_IMolecularFormula()
        {
            var formula = MolecularFormulaManipulator.GetMajorIsotopeMolecularFormula("H2PO4", builder);
            formula.Charge = -1;
            double totalExactMass2 = MolecularFormulaManipulator.GetTotalExactMass(formula);
            Assert.AreEqual(96.96961875390926, totalExactMass2, 0.0001);
        }

        [TestMethod()]
        public void TestGetNaturalExactMass_IMolecularFormula()
        {
            var formula = new MolecularFormula();
            formula.Add(builder.NewIsotope("C"));
            formula.Add(builder.NewIsotope("Cl"));

            double expectedMass = 0.0;
            expectedMass += BODRIsotopeFactory.Instance.GetNaturalMass(ChemicalElement.C);
            expectedMass += BODRIsotopeFactory.Instance.GetNaturalMass(ChemicalElement.Cl);

            double totalExactMass = MolecularFormulaManipulator.GetNaturalExactMass(formula);
            Assert.AreEqual(expectedMass, totalExactMass, 0.000001);
        }

        [TestMethod()]
        public void TestGetTotalMassNumber_IMolecularFormula()
        {
            var formula = new MolecularFormula();
            formula.Add(builder.NewIsotope("C"));
            formula.Add(builder.NewIsotope("O"));

            double totalExactMass = MolecularFormulaManipulator.GetTotalMassNumber(formula);
            Assert.AreEqual(28, totalExactMass, 0.000001);
        }

        [TestMethod()]
        public void TestGetMajorIsotopeMass_IMolecularFormula()
        {
            var formula = new MolecularFormula();
            formula.Add(builder.NewIsotope("C"));
            formula.Add(builder.NewIsotope("H"), 4);

            double expectedMass = 0.0;
            expectedMass += BODRIsotopeFactory.Instance.GetMajorIsotope("C").ExactMass.Value;
            expectedMass += 4.0 * BODRIsotopeFactory.Instance.GetMajorIsotope("H").ExactMass.Value;

            double totalExactMass = MolecularFormulaManipulator.GetMajorIsotopeMass(formula);
            Assert.AreEqual(expectedMass, totalExactMass, 0.000001);
        }

        /// <summary>
        /// Test total Exact Mass. It is necessary to have added the
        /// corresponding isotope before to calculate the exact mass.
        /// </summary>
        [TestMethod()]
        public void TestBug_1944604()
        {
            var formula = new MolecularFormula();
            IIsotope carb = builder.NewIsotope("C");

            formula.Add(carb);

            Assert.AreEqual("C1", MolecularFormulaManipulator.GetString(formula, true));

            double totalExactMass = MolecularFormulaManipulator.GetTotalExactMass(formula);

            Assert.AreEqual(12.0, totalExactMass, 0.000001);
        }

        /// <summary>
        /// Test total natural abundance.
        /// </summary>
        [TestMethod()]
        public void TestGetTotalNaturalAbundance_IMolecularFormula()
        {
            var formula = new MolecularFormula();
            IIsotope carb = builder.NewIsotope("C");
            carb.NaturalAbundance = 98.93;
            IIsotope cl = builder.NewIsotope("Cl");
            cl.NaturalAbundance = 75.78;
            formula.Add(carb);
            formula.Add(cl);

            double totalAbudance = MolecularFormulaManipulator.GetTotalNaturalAbundance(formula);

            Assert.AreEqual(0.74969154, totalAbudance, 0.000001);
        }

        /// <summary>
        /// Test total natural abundance.
        /// </summary>
        [TestMethod()]
        public void TestGetTotalNaturalAbundance_IMolecularFormula2()
        {
            IMolecularFormula formula1 = new MolecularFormula();
            IIsotope br1 = builder.NewIsotope("Br");
            br1.NaturalAbundance = 49.31;
            IIsotope br2 = builder.NewIsotope("Br");
            br2.NaturalAbundance = 50.69;
            formula1.Add(br1);
            formula1.Add(br2);

            Assert.AreEqual(2, formula1.Isotopes.Count(), 0.000001);
            double totalAbudance = MolecularFormulaManipulator.GetTotalNaturalAbundance(formula1);
            Assert.AreEqual(0.24995235, totalAbudance, 0.000001);
        }

        [TestMethod()]
        public void TestGetTotalNaturalAbundance_IMolecularFormula3()
        {
            IMolecularFormula formula2 = new MolecularFormula();
            IIsotope br1 = builder.NewIsotope("Br");
            br1.NaturalAbundance = 50.69;
            IIsotope br2 = builder.NewIsotope("Br");
            br2.NaturalAbundance = 50.69;
            formula2.Add(br1);
            formula2.Add(br2);

            Assert.AreEqual(1, formula2.Isotopes.Count(), 0.000001);
            double totalAbudance = MolecularFormulaManipulator.GetTotalNaturalAbundance(formula2);

            Assert.AreEqual(0.25694761, totalAbudance, 0.000001);
        }

        [TestMethod()]
        public void TestGetTotalNaturalAbundance_IMolecularFormula4()
        {
            IMolecularFormula formula2 = new MolecularFormula();
            IIsotope br1 = builder.NewIsotope("Br");
            br1.NaturalAbundance = 50.69;
            formula2.Add(br1);
            formula2.Add(br1);

            Assert.AreEqual(1, formula2.Isotopes.Count());
            double totalAbudance = MolecularFormulaManipulator.GetTotalNaturalAbundance(formula2);

            Assert.AreEqual(0.25694761, totalAbudance, 0.000001);
        }

        /// <summary>Test Double-Bond-Equivalent (DBE) calculation</summary>
        [TestMethod()]
        public void TestGetDBE_IMolecularFormula()
        {
            var formula = new MolecularFormula();
            formula.Add(builder.NewIsotope("C"), 10);
            formula.Add(builder.NewIsotope("H"), 22);

            Assert.AreEqual(0.0, MolecularFormulaManipulator.GetDBE(formula), 0.01);

            formula = new MolecularFormula();
            formula.Add(builder.NewIsotope("C"), 10);
            formula.Add(builder.NewIsotope("H"), 16);

            Assert.AreEqual(3.0, MolecularFormulaManipulator.GetDBE(formula), 0.01);

            formula = new MolecularFormula();
            formula.Add(builder.NewIsotope("C"), 10);
            formula.Add(builder.NewIsotope("H"), 16);
            formula.Add(builder.NewIsotope("O"));

            Assert.AreEqual(3.0, MolecularFormulaManipulator.GetDBE(formula), 0.01);

            formula = new MolecularFormula();
            formula.Add(builder.NewIsotope("C"), 10);
            formula.Add(builder.NewIsotope("H"), 19);
            formula.Add(builder.NewIsotope("N"));

            Assert.AreEqual(2.0, MolecularFormulaManipulator.GetDBE(formula), 0.01);
        }

        [TestMethod()]
        public void TestGetHTML_IMolecularFormula()
        {
            MolecularFormula formula = new MolecularFormula();
            formula.Add(builder.NewIsotope("C"), 8);
            formula.Add(builder.NewIsotope("H"), 10);
            formula.Add(builder.NewIsotope("Cl"), 2);
            formula.Add(builder.NewIsotope("O"), 2);

            Assert.AreEqual("C<sub>8</sub>H<sub>10</sub>Cl<sub>2</sub>O<sub>2</sub>",
                    MolecularFormulaManipulator.GetHTML(formula));
        }

        [TestMethod()]
        public void HtmlFormulaDoesNotAddSubscriptForSingleElements()
        {
            MolecularFormula formula = new MolecularFormula();
            formula.Add(builder.NewIsotope("C"), 1);
            formula.Add(builder.NewIsotope("H"), 4);

            Assert.AreEqual("CH<sub>4</sub>", MolecularFormulaManipulator.GetHTML(formula));
        }

        [TestMethod()]
        public void TestGetHTML_IMolecularFormula_boolean_boolean()
        {
            MolecularFormula formula = new MolecularFormula();
            formula.Add(builder.NewIsotope("C"), 10);

            Assert.AreEqual("C<sub>10</sub>", MolecularFormulaManipulator.GetHTML(formula, true, false));
            formula.Charge = 1;
            Assert.AreEqual("C<sub>10</sub><sup>+</sup>", MolecularFormulaManipulator.GetHTML(formula, true, false));
            formula.Charge = formula.Charge - 2;
            Assert.AreEqual("C<sub>10</sub><sup>–</sup>", MolecularFormulaManipulator.GetHTML(formula, true, false));
        }

        [TestMethod()]
        public void NullIsotopeLabels()
        {
            MolecularFormula formula = new MolecularFormula();
            formula.Add(builder.NewIsotope("C"), 10);

            Assert.AreEqual("C<sub>10</sub>", MolecularFormulaManipulator.GetHTML(formula, true, false));
            formula.Charge = 1;
            Assert.AreEqual("C<sub>10</sub><sup>+</sup>", MolecularFormulaManipulator.GetHTML(formula, true, true));
            formula.Charge = formula.Charge - 2;
            Assert.AreEqual("C<sub>10</sub><sup>–</sup>", MolecularFormulaManipulator.GetHTML(formula, true, true));
        }

        [TestMethod()]
        public void TestGetHTML_IMolecularFormula_arrayString_boolean_boolean()
        {
            var formula = new MolecularFormula();
            formula.Add(builder.NewIsotope("C"), 2);
            formula.Add(builder.NewIsotope("H"), 2);

            string[] newOrder = new string[2];
            newOrder[0] = "H";
            newOrder[1] = "C";

            Assert.AreEqual("H<sub>2</sub>C<sub>2</sub>",
                    MolecularFormulaManipulator.GetHTML(formula, newOrder, false, false));
        }

        [TestMethod()]
        public void TestGetHTML_IMolecularFormulaWithIsotope()
        {
            MolecularFormula formula = new MolecularFormula();
            formula.Add(ifac.GetMajorIsotope("C"), 2);
            formula.Add(ifac.GetMajorIsotope("H"), 6);
            Assert.AreEqual("<sup>12</sup>C<sub>2</sub><sup>1</sup>H<sub>6</sub>",
                    MolecularFormulaManipulator.GetHTML(formula, false, true));
        }

        [TestMethod()]
        public void TestGetHTML_IMolecularFormulaWithIsotopeAndCharge()
        {
            MolecularFormula formula = new MolecularFormula();
            formula.Add(ifac.GetMajorIsotope("C"), 2);
            formula.Add(ifac.GetMajorIsotope("H"), 6);
            formula.Charge = 1;
            Assert.AreEqual("<sup>12</sup>C<sub>2</sub><sup>1</sup>H<sub>6</sub><sup>+</sup>",
                    MolecularFormulaManipulator.GetHTML(formula, true, true));
        }

        [TestMethod()]
        public void TestGetMolecularFormula_IAtomContainer()
        {
            IAtomContainer ac = builder.NewAtomContainer();
            ac.Atoms.Add(builder.NewAtom("C"));
            ac.Atoms.Add(builder.NewAtom("C"));
            ac.Atoms.Add(builder.NewAtom("H"));
            ac.Atoms.Add(builder.NewAtom("H"));
            ac.Atoms.Add(builder.NewAtom("H"));
            ac.Atoms.Add(builder.NewAtom("H"));

            var mf1 = MolecularFormulaManipulator.GetMolecularFormula(ac);

            var mf2 = new MolecularFormula();
            mf2.Add(builder.NewIsotope("C"), 2);
            mf2.Add(builder.NewIsotope("H"), 4);

            Assert.AreEqual(MolecularFormulaManipulator.GetAtomCount(mf2),
                    MolecularFormulaManipulator.GetAtomCount(mf1));
            Assert.AreEqual(mf2.Isotopes.Count(), mf1.Isotopes.Count());
            var elemC = ChemicalElement.C;
            var elemH = ChemicalElement.H;
            Assert.AreEqual(mf2.GetCount(builder.NewIsotope(elemC)),
                    mf1.GetCount(builder.NewIsotope(elemC)));
            Assert.AreEqual(mf2.GetCount(builder.NewIsotope(elemH)),
                    mf1.GetCount(builder.NewIsotope(elemH)));
            Assert.AreEqual(MolecularFormulaManipulator.GetElementCount(mf2, elemC),
                    MolecularFormulaManipulator.GetElementCount(mf1, elemC));
            Assert.AreEqual(MolecularFormulaManipulator.GetElementCount(mf2, elemH),
                    MolecularFormulaManipulator.GetElementCount(mf1, elemH));
        }

        [TestMethod()]
        public void TestGetMolecularFormula_IAtomNullCharge()
        {
            IAtomContainer ac = builder.NewAtomContainer();
            ac.Atoms.Add(builder.NewAtom("C"));
            ac.Atoms[0].FormalCharge = null;
            ac.Atoms.Add(builder.NewAtom("C"));
            ac.Atoms.Add(builder.NewAtom("H"));
            ac.Atoms.Add(builder.NewAtom("H"));
            ac.Atoms.Add(builder.NewAtom("H"));
            ac.Atoms.Add(builder.NewAtom("H"));

            var mf1 = MolecularFormulaManipulator.GetMolecularFormula(ac);
            Assert.IsNotNull(mf1);
        }

        [TestMethod()]
        public void TestGetMolecularFormula_IAtomContainer_withCharge()
        {
            IAtomContainer ac = builder.NewAtomContainer();
            ac.Atoms.Add(builder.NewAtom("C"));
            ac.Atoms[0].FormalCharge = 1;
            ac.Atoms.Add(builder.NewAtom("C"));
            ac.Atoms.Add(builder.NewAtom("H"));
            ac.Atoms.Add(builder.NewAtom("H"));
            ac.Atoms.Add(builder.NewAtom("H"));

            var mf1 = MolecularFormulaManipulator.GetMolecularFormula(ac);

            Assert.AreEqual(1, mf1.Charge.Value, 0.000);
        }

        [TestMethod()]
        public void TestGetMolecularFormula_IAtomContainer_IMolecularFormula()
        {
            IAtomContainer ac = builder.NewAtomContainer();
            ac.Atoms.Add(builder.NewAtom("C"));
            ac.Atoms.Add(builder.NewAtom("C"));
            ac.Atoms.Add(builder.NewAtom("H"));
            ac.Atoms.Add(builder.NewAtom("H"));
            ac.Atoms.Add(builder.NewAtom("H"));
            ac.Atoms.Add(builder.NewAtom("H"));

            var mf1 = MolecularFormulaManipulator.GetMolecularFormula(ac, new MolecularFormula());

            var mf2 = new MolecularFormula();
            mf2.Add(builder.NewIsotope("C"), 2);
            mf2.Add(builder.NewIsotope("H"), 4);

            Assert.AreEqual(MolecularFormulaManipulator.GetAtomCount(mf2),
                    MolecularFormulaManipulator.GetAtomCount(mf1));
            Assert.AreEqual(mf2.Isotopes.Count(), mf1.Isotopes.Count());
            var elemC = ChemicalElement.C;
            var elemH = ChemicalElement.H;
            Assert.AreEqual(mf2.GetCount(builder.NewIsotope(elemC)),
                    mf1.GetCount(builder.NewIsotope(elemC)));
            Assert.AreEqual(mf2.GetCount(builder.NewIsotope(elemH)),
                    mf1.GetCount(builder.NewIsotope(elemH)));
            Assert.AreEqual(MolecularFormulaManipulator.GetElementCount(mf2, elemC),
                    MolecularFormulaManipulator.GetElementCount(mf1, elemC));
            Assert.AreEqual(MolecularFormulaManipulator.GetElementCount(mf2, elemH),
                    MolecularFormulaManipulator.GetElementCount(mf1, elemH));
        }

        [TestMethod()]
        public void TestGetMolecularFormula_IAtomContainerIMolecularFormula_2()
        {
            var ac = builder.NewAtomContainer();
            ac.Atoms.Add(builder.NewAtom("C"));
            ac.Atoms.Add(builder.NewAtom("C"));
            ac.Atoms.Add(builder.NewAtom("H"));
            ac.Atoms.Add(builder.NewAtom("H"));
            ac.Atoms.Add(builder.NewAtom("H"));
            ac.Atoms.Add(builder.NewAtom("H"));

            var mf0 = new MolecularFormula();
            mf0.Add(builder.NewIsotope("C"), 2);
            mf0.Add(builder.NewIsotope("H"), 5);

            var mf1 = MolecularFormulaManipulator.GetMolecularFormula(ac, mf0);

            var mf2 = new MolecularFormula();
            mf2.Add(builder.NewIsotope("C"), 4);
            mf2.Add(builder.NewIsotope("H"), 9);

            Assert.AreEqual(MolecularFormulaManipulator.GetAtomCount(mf2),
                    MolecularFormulaManipulator.GetAtomCount(mf1));
            Assert.AreEqual(mf2.Isotopes.Count(), mf1.Isotopes.Count());
            var elemC = ChemicalElement.C;
            var elemH = ChemicalElement.H;
            Assert.AreEqual(mf2.GetCount(builder.NewIsotope(elemC)),
                    mf1.GetCount(builder.NewIsotope(elemC)));
            Assert.AreEqual(mf2.GetCount(builder.NewIsotope(elemH)),
                    mf1.GetCount(builder.NewIsotope(elemH)));
            Assert.AreEqual(MolecularFormulaManipulator.GetElementCount(mf2, elemC),
                    MolecularFormulaManipulator.GetElementCount(mf1, elemC));
            Assert.AreEqual(MolecularFormulaManipulator.GetElementCount(mf2, elemH),
                    MolecularFormulaManipulator.GetElementCount(mf1, elemH));
        }

        [TestMethod()]
        public void TestGetAtomContainer_IMolecularFormula()
        {
            var mf2 = new MolecularFormula();
            mf2.Add(builder.NewIsotope("C"), 2);
            mf2.Add(builder.NewIsotope("H"), 4);

            var ac = MolecularFormulaManipulator.GetAtomContainer(mf2);

            Assert.AreEqual(6, ac.Atoms.Count);
        }

        [TestMethod()]
        public void TestGetAtomContainer_IMolecularFormula_IAtomContainer()
        {
            var mf2 = new MolecularFormula();
            mf2.Add(builder.NewIsotope("C"), 2);
            mf2.Add(builder.NewIsotope("H"), 4);

            var ac = MolecularFormulaManipulator.GetAtomContainer(mf2, builder.NewAtomContainer());

            Assert.AreEqual(6, ac.Atoms.Count);
        }

        [TestMethod()]
        public void TestGetAtomContainer_String_IChemObjectBuilder()
        {
            var mf = "C2H4";
            var atomContainer = MolecularFormulaManipulator.GetAtomContainer(mf, builder);
            Assert.AreEqual(6, atomContainer.Atoms.Count);
        }

        // @cdk.bug 1296
        [TestMethod()]
        public void TestGetAtomContainer_AddsAtomicNumbers()
        {
            var mf2 = new MolecularFormula();
            mf2.Add(builder.NewIsotope("C"), 2);
            mf2.Add(builder.NewIsotope("H"), 4);
            var ac = MolecularFormulaManipulator.GetAtomContainer(mf2, builder.NewAtomContainer());
            Assert.AreEqual(6, ac.Atoms.Count);
            Assert.IsNotNull(ac.Atoms[0].AtomicNumber);
            foreach (var atom in ac.Atoms)
            {
                switch (atom.AtomicNumber)
                {
                    case AtomicNumbers.C:
                        Assert.AreEqual(6, atom.AtomicNumber);
                        break;
                    case AtomicNumbers.H:
                        Assert.AreEqual(1, atom.AtomicNumber);
                        break;
                    default:
                        Assert.Fail("Unexpected element: " + atom.Symbol);
                        break;
                }
            }
        }

        [TestMethod()]
        public void TestMolecularFormulaIAtomContainer_to_IAtomContainer2()
        {
            var ac = builder.NewAtomContainer();
            ac.Atoms.Add(builder.NewAtom("C"));
            ac.Atoms.Add(builder.NewAtom("C"));
            ac.Atoms.Add(builder.NewAtom("H"));
            ac.Atoms.Add(builder.NewAtom("H"));
            ac.Atoms.Add(builder.NewAtom("H"));
            ac.Atoms.Add(builder.NewAtom("H"));

            var mf2 = new MolecularFormula();
            mf2.Add(builder.NewIsotope("C"), 2);
            mf2.Add(builder.NewIsotope("H"), 4);

            var ac2 = MolecularFormulaManipulator.GetAtomContainer(mf2, builder.NewAtomContainer());

            Assert.AreEqual(ac2.Atoms.Count, ac2.Atoms.Count);
            Assert.AreEqual(ac2.Atoms[0].Symbol, ac2.Atoms[0].Symbol);
            Assert.AreEqual(ac2.Atoms[5].Symbol, ac2.Atoms[5].Symbol);
        }

        [TestMethod()]
        public void TestElements_IMolecularFormula()
        {
            var formula = new MolecularFormula();
            formula.Add(builder.NewIsotope("C"), 1);
            formula.Add(builder.NewIsotope("H"), 2);

            var br1 = builder.NewIsotope("Br");
            br1.NaturalAbundance = 50.69;
            formula.Add(br1);
            var br2 = builder.NewIsotope("Br");
            br2.NaturalAbundance = 50.69;
            formula.Add(br2);

            var elements = MolecularFormulaManipulator.Elements(formula);

            Assert.AreEqual(5, MolecularFormulaManipulator.GetAtomCount(formula));
            Assert.AreEqual(3, elements.Count());
        }

        [TestMethod()]
        public void TestCompare_Charge()
        {
            var formula1 = new MolecularFormula();
            formula1.Add(builder.NewIsotope("C"), 1);
            formula1.Add(builder.NewIsotope("H"), 2);

            var formula2 = new MolecularFormula();
            formula2.Add(builder.NewIsotope("C"), 1);
            formula2.Add(builder.NewIsotope("H"), 2);

            var formula3 = new MolecularFormula();
            formula3.Add(builder.NewIsotope("C"), 1);
            formula3.Add(builder.NewIsotope("H"), 2);
            formula3.Charge = 0;

            Assert.IsTrue(MolecularFormulaManipulator.Compare(formula1, formula2));
            Assert.IsFalse(MolecularFormulaManipulator.Compare(formula1, formula3));
        }

        [TestMethod()]
        public void TestCompare_NumberIsotope()
        {
            var formula1 = new MolecularFormula();
            formula1.Add(builder.NewIsotope("C"), 1);
            formula1.Add(builder.NewIsotope("H"), 2);

            var formula2 = new MolecularFormula();
            formula2.Add(builder.NewIsotope("C"), 1);
            formula2.Add(builder.NewIsotope("H"), 2);

            var formula3 = new MolecularFormula();
            formula3.Add(builder.NewIsotope("C"), 1);
            formula3.Add(builder.NewIsotope("H"), 3);

            Assert.IsTrue(MolecularFormulaManipulator.Compare(formula1, formula2));
            Assert.IsFalse(MolecularFormulaManipulator.Compare(formula1, formula3));
        }

        [TestMethod()]
        public void TestCompare_IMolecularFormula_IMolecularFormula()
        {
            var formula1 = new MolecularFormula();
            formula1.Add(builder.NewIsotope("C"), 1);
            formula1.Add(builder.NewIsotope("H"), 2);

            var formula2 = new MolecularFormula();
            formula2.Add(builder.NewIsotope("C"), 1);
            formula2.Add(builder.NewIsotope("H"), 2);

            var formula3 = new MolecularFormula();
            formula3.Add(builder.NewIsotope("C"), 1);
            var hyd = builder.NewIsotope("H");
            hyd.ExactMass = 2.002334234;
            formula3.Add(hyd, 2);

            Assert.IsTrue(MolecularFormulaManipulator.Compare(formula1, formula2));
            Assert.IsFalse(MolecularFormulaManipulator.Compare(formula1, formula3));
        }

        [TestMethod()]
        public void TestGetHeavyElements_IMolecularFormula()
        {
            var formula = new MolecularFormula();
            formula.Add(builder.NewIsotope("C"), 10);
            formula.Add(builder.NewIsotope("H"), 16);
            Assert.AreEqual(1, MolecularFormulaManipulator.GetHeavyElements(formula).Count());
        }

        [TestMethod()]
        public void TestGetHeavyElements_IMolecularFormula_2()
        {
            var formula = MolecularFormulaManipulator.GetMolecularFormula("CH3OH", builder);
            Assert.AreEqual(2, MolecularFormulaManipulator.GetHeavyElements(formula).Count());
        }

        /// <summary>
        /// Test if the elements-ordered-by-probability are in the expected order.
        /// </summary>
        [TestMethod()]
        public void TestGenerateOrderEle()
        {
            string[] listElements = new string[]{
                // Elements of life
                "C", "H", "O", "N", "Si", "P", "S", "F", "Cl",

                "Br", "I", "Sn", "B", "Pb", "Tl", "Ba", "In", "Pd", "Pt", "Os", "Ag", "Zr", "Se", "Zn", "Cu", "Ni",
                "Co", "Fe", "Cr", "Ti", "Ca", "K", "Al", "Mg", "Na", "Ce", "Hg", "Au", "Ir", "Re", "W", "Ta", "Hf",
                "Lu", "Yb", "Tm", "Er", "Ho", "Dy", "Tb", "Gd", "Eu", "Sm", "Pm", "Nd", "Pr", "La", "Cs", "Xe", "Te",
                "Sb", "Cd", "Rh", "Ru", "Tc", "Mo", "Nb", "Y", "Sr", "Rb", "Kr", "As", "Ge", "Ga", "Mn", "V", "Sc",
                "Ar", "Ne", "He", "Be", "Li",

                // rest of periodic table, in atom-number order.
                "Bi", "Po", "At", "Rn",
                // row-7 elements (including f-block)
                "Fr", "Ra", "Ac", "Th", "Pa", "U", "Np", "Pu", "Am", "Cm", "Bk", "Cf", "Es", "Fm", "Md", "No", "Lr",
                "Rf", "Db", "Sg", "Bh", "Hs", "Mt", "Ds", "Rg", "Cn",

                // The "odd one out": an unspecified R-group
                "R"};

            var arrayGenerated = MolecularFormulaManipulator.OrderEle;
            var listGenerated = arrayGenerated.ToReadOnlyList();
            Assert.AreEqual(113, listGenerated.Count());

            for (int i = 0; i < listElements.Length; i++)
            {
                string element = listElements[i];
                Assert.IsTrue(listGenerated.Contains(element), "Element missing from generateOrderEle: " + element);
            }
        }

        /// <summary>
        /// TODO: REACT: Introduce method
        /// </summary>
        // @cdk.bug 2672696
        [TestMethod()]
        public void TestGetHillString_IMolecularFormula()
        {
            IMolecularFormula formula;
            string listGenerated;

            formula = MolecularFormulaManipulator.GetMolecularFormula("CH3OH", builder);
            listGenerated = MolecularFormulaManipulator.GetHillString(formula);
            Assert.AreEqual("CH4O", listGenerated);

            formula = MolecularFormulaManipulator.GetMolecularFormula("CH3CH2Br", builder);
            listGenerated = MolecularFormulaManipulator.GetHillString(formula);
            Assert.AreEqual("C2H5Br", listGenerated);

            formula = MolecularFormulaManipulator.GetMolecularFormula("HCl", builder);
            listGenerated = MolecularFormulaManipulator.GetHillString(formula);
            Assert.AreEqual("ClH", listGenerated);

            formula = MolecularFormulaManipulator.GetMolecularFormula("HBr", builder);
            listGenerated = MolecularFormulaManipulator.GetHillString(formula);
            Assert.AreEqual("BrH", listGenerated);
        }

        /// <summary>
        /// Tests that an atom which has not be configured with isotope information,
        /// provides the correct exact mass.
        /// </summary>
        // @cdk.bug 1944604
        [TestMethod()]
        public void TestSingleAtomFromSmiles()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C"));

            // previously performed inside SmilesParser
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            CDK.HydrogenAdder.AddImplicitHydrogens(mol);

            var mf = MolecularFormulaManipulator.GetMolecularFormula(mol);
            var exactMass = MolecularFormulaManipulator.GetTotalExactMass(mf);
            Assert.AreEqual(16.0313, exactMass, 0.0001);
        }

        [TestMethod()]
        public void TestSingleAtom()
        {
            var formula = "CH4";
            var mf = MolecularFormulaManipulator.GetMolecularFormula(formula, builder);
            Assert.AreEqual(1, MolecularFormulaManipulator.GetIsotopes(mf, ChemicalElement.C).Count());
        }

        [TestMethod()]
        public void TestSimplifyMolecularFormula_String()
        {
            var formula = "C1H41.H2O";
            var simplifyMF = MolecularFormulaManipulator.SimplifyMolecularFormula(formula);
            Assert.AreEqual("C1H43O", simplifyMF);
        }

        [TestMethod()]
        public void TestSimplifyMolecularFormula_String2()
        {
            var formula = "CH41.H2O";
            var simplifyMF = MolecularFormulaManipulator.SimplifyMolecularFormula(formula);
            Assert.AreEqual("CH43O", simplifyMF);
        }

        [TestMethod()]
        public void TestSimplifygetMF()
        {
            var formula = "CH4.H2O";
            var formula1 = new MolecularFormula();
            formula1.Add(builder.NewIsotope("C"), 1);
            formula1.Add(builder.NewIsotope("H"), 6);
            formula1.Add(builder.NewIsotope("O"), 1);
            var ff = MolecularFormulaManipulator.GetMolecularFormula(formula, builder);
            Assert.IsTrue(MolecularFormulaManipulator.Compare(formula1, MolecularFormulaManipulator.GetMolecularFormula(formula, builder)));
            Assert.AreEqual("CH6O", MolecularFormulaManipulator.GetString(ff));
        }

        [TestMethod()]
        public void TestSpace()
        {
            var formula = "C17H21NO. C7H6O3";
            var simplifyMF = MolecularFormulaManipulator.SimplifyMolecularFormula(formula);
            Assert.AreEqual("C24H27NO4", simplifyMF);
        }

        /// <summary>Test molecule simplification</summary>
        [TestMethod()]
        public void Test0()
        {
            var formula = "Fe.(C6H11O7)3";
            var simplifyMF = MolecularFormulaManipulator.SimplifyMolecularFormula(formula);
            Assert.AreEqual("FeC18H33O21", simplifyMF);
        }

        /// <summary>Test molecule simplification</summary>
        [TestMethod()]
        public void Test1()
        {
            var formula = "(C6H11O7)3.Fe";
            var simplifyMF = MolecularFormulaManipulator.SimplifyMolecularFormula(formula);
            Assert.AreEqual("C18H33O21Fe", simplifyMF);
        }

        /// <summary>Test molecule simplification</summary>
        [TestMethod()]
        public void Test2()
        {
            var formula = "C14H14N2.2HCl";
            var simplifyMF = MolecularFormulaManipulator.SimplifyMolecularFormula(formula);
            Assert.AreEqual("C14H16N2Cl2", simplifyMF);
        }

        /// <summary>Test molecule simplification</summary>
        [TestMethod()]
        public void Test3()
        {
            var formula = "(C27H33N3O8)2.2HNO3.3H2O";
            var simplifyMF = MolecularFormulaManipulator.SimplifyMolecularFormula(formula);
            Assert.AreEqual("C54H74N8O25", simplifyMF);
        }

        /// <summary>Test if formula-comparison is simplify-independant</summary>
        [TestMethod()]
        public void Test4()
        {
            var formula = "(C27H33N3O8)2.2HNO3.3H2O";
            var formula1 = new MolecularFormula();
            formula1.Add(builder.NewIsotope("C"), 54);
            formula1.Add(builder.NewIsotope("H"), 74);
            formula1.Add(builder.NewIsotope("O"), 25);
            formula1.Add(builder.NewIsotope("N"), 8);
            IMolecularFormula ff = MolecularFormulaManipulator.GetMolecularFormula(formula, builder);
            Assert.IsTrue(MolecularFormulaManipulator.Compare(formula1, MolecularFormulaManipulator.GetMolecularFormula(formula, builder)));
            Assert.AreEqual("C54H74N8O25", MolecularFormulaManipulator.GetString(ff));
        }

        /// <summary>Test if formula-comparison is simplify-independant</summary>
        [TestMethod()]
        public void Test5()
        {
            var formula = "[SO3]2-";
            var formula1 = new MolecularFormula();
            formula1.Add(builder.NewIsotope("S"), 1);
            formula1.Add(builder.NewIsotope("O"), 3);
            formula1.Charge = -2;
            var ff = MolecularFormulaManipulator.GetMolecularFormula(formula, builder);
            Assert.IsTrue(MolecularFormulaManipulator.Compare(formula1, MolecularFormulaManipulator.GetMolecularFormula(formula, builder)));
            Assert.AreEqual("[O3S]2-", MolecularFormulaManipulator.GetString(ff));
            Assert.AreEqual(-2, ff.Charge.Value, 0.00001);
        }

        /// <summary>Test if formula-comparison is simplify-independant</summary>
        [TestMethod()]
        public void Test6()
        {
            var formula = "(CH3)2";
            var formula1 = new MolecularFormula();
            formula1.Add(builder.NewIsotope("C"), 2);
            formula1.Add(builder.NewIsotope("H"), 6);
            var ff = MolecularFormulaManipulator.GetMolecularFormula(formula, builder);
            Assert.IsTrue(MolecularFormulaManipulator.Compare(formula1, MolecularFormulaManipulator.GetMolecularFormula(formula, builder)));
            Assert.AreEqual("C2H6", MolecularFormulaManipulator.GetString(ff));
        }

        /// <summary>Test if formula-comparison is simplify-independant</summary>
        [TestMethod()]
        public void TestWithH_Initial()
        {
            var formula = "HC5H11NO2H";
            var formula1 = new MolecularFormula();
            formula1.Add(builder.NewIsotope("C"), 5);
            formula1.Add(builder.NewIsotope("H"), 13);
            formula1.Add(builder.NewIsotope("N"), 1);
            formula1.Add(builder.NewIsotope("O"), 2);
            var ff = MolecularFormulaManipulator.GetMolecularFormula(formula, builder);
            Assert.IsTrue(MolecularFormulaManipulator.Compare(formula1, MolecularFormulaManipulator.GetMolecularFormula(formula, builder)));
            Assert.AreEqual("C5H13NO2", MolecularFormulaManipulator.GetString(ff));
        }

        // @cdk.bug 3071473
        [TestMethod()]
        public void TestFromMol()
        {
            var filename = "NCDK.Data.MDL.formulatest.mol";
            IChemFile chemFile;
            using (var reader = new MDLV2000Reader(ResourceLoader.GetAsStream(filename)))
            {
                chemFile = reader.Read(builder.NewChemFile());
            }
            Assert.IsNotNull(chemFile);
            var mols = ChemFileManipulator.GetAllAtomContainers(chemFile).ToReadOnlyList();
            var mol = mols[0];

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            var ha = CDK.HydrogenAdder;
            ha.AddImplicitHydrogens(mol);
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(mol);

            var molecularFormula = MolecularFormulaManipulator.GetMolecularFormula(mol);
            var formula2 = MolecularFormulaManipulator.GetString(molecularFormula);
            Assert.IsTrue(formula2.Equals("C35H64N3O21P3S"));
        }

        // @cdk.bug 3340660
        [TestMethod()]
        public void TestHelium()
        {
            var helium = builder.NewAtomContainer();
            helium.Atoms.Add(builder.NewAtom("He"));
            var formula = MolecularFormulaManipulator.GetMolecularFormula(helium);
            Assert.IsNotNull(formula);
            Assert.AreEqual("He", MolecularFormulaManipulator.GetString(formula));
        }

        // @cdk.bug 3340660
        [TestMethod()]
        public void TestAmericum()
        {
            var helium = builder.NewAtomContainer();
            helium.Atoms.Add(builder.NewAtom("Am"));
            var formula = MolecularFormulaManipulator.GetMolecularFormula(helium);
            Assert.IsNotNull(formula);
            Assert.AreEqual("Am", MolecularFormulaManipulator.GetString(formula));
        }

        // @cdk.bug 2983334
        [TestMethod()]
        public void TestImplicitH()
        {
            var adder = CDK.HydrogenAdder;
            var mol = TestMoleculeFactory.MakeBenzene();
            var f = MolecularFormulaManipulator.GetMolecularFormula(mol);
            Assert.AreEqual("C6", MolecularFormulaManipulator.GetString(f));
            Assert.AreEqual(6, mol.Atoms.Count);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            adder.AddImplicitHydrogens(mol);
            Assert.AreEqual(6, mol.Atoms.Count);
            f = MolecularFormulaManipulator.GetMolecularFormula(mol);
            Assert.AreEqual("C6H6", MolecularFormulaManipulator.GetString(f));
        }

        [TestMethod()]
        public void NoNullPointerExceptionForExactMassOfRGroups()
        {
            var formula = new MolecularFormula();
            formula.Add(builder.NewIsotope("C"));
            formula.Add(builder.NewIsotope("H"), 3);
            formula.Add(builder.NewIsotope("R"));
            Assert.AreEqual(15.0234, MolecularFormulaManipulator.GetTotalExactMass(formula), 0.01);
        }

        [TestMethod()]
        public void NoNullPointerExceptionForMassOfRGroups()
        {
            var formula = new MolecularFormula();
            formula.Add(builder.NewIsotope("C"));
            formula.Add(builder.NewIsotope("H"), 3);
            formula.Add(builder.NewIsotope("R"));
            Assert.AreEqual(15.0, MolecularFormulaManipulator.GetTotalMassNumber(formula), 0.01);
        }

        [TestMethod()]
        public void NoNullPointerExceptionForMajorMassOfRGroups()
        {
            var formula = new MolecularFormula();
            formula.Add(builder.NewIsotope("C"));
            formula.Add(builder.NewIsotope("H"), 3);
            formula.Add(builder.NewIsotope("R"));
            Assert.AreEqual(15.0234, MolecularFormulaManipulator.GetMajorIsotopeMass(formula), 0.01);
        }

        [TestMethod()]
        public void NoNullPointerForStaticIsotopes()
        {
            var isotopes = BODRIsotopeFactory.Instance;
            var carbon = isotopes.GetMajorIsotope("C");
            var mf = new MolecularFormula();
            mf.Add(carbon, 10);
            MolecularFormulaManipulator.GetNaturalExactMass(mf);
        }

        [TestMethod()]
        public void AcceptMinusAsInput()
        {
            var bldr = CDK.Builder;
            var mf = MolecularFormulaManipulator.GetMolecularFormula("[PO4]3–", bldr);
            Assert.AreEqual(-3, mf.Charge);
        }

        [TestMethod()]
        public void DeprotonatePhenol()
        {
            var bldr = CDK.Builder;
            var mf = MolecularFormulaManipulator.GetMolecularFormula("C6H6O", bldr);
            Assert.IsTrue(MolecularFormulaManipulator.AdjustProtonation(mf, -1));
            Assert.AreEqual("[C6H5O]-", MolecularFormulaManipulator.GetString(mf));
            Assert.AreEqual(-1, mf.Charge);
        }

        [TestMethod()]
        public void ProtonatePhenolate()
        {
            var bldr = CDK.Builder;
            var mf = MolecularFormulaManipulator.GetMolecularFormula("[C6H5O]-", bldr);
            Assert.IsTrue(MolecularFormulaManipulator.AdjustProtonation(mf, +1));
            Assert.AreEqual("C6H6O", MolecularFormulaManipulator.GetString(mf));
            Assert.AreEqual(0, mf.Charge);
            Assert.AreEqual(3, mf.Isotopes.Count());
        }

        [TestMethod()]
        public void ProtonatePhenolateMajorIsotopes()
        {
            var bldr = CDK.Builder;
            var mf = MolecularFormulaManipulator.GetMajorIsotopeMolecularFormula("[C6H5O]-", bldr);
            Assert.IsTrue(MolecularFormulaManipulator.AdjustProtonation(mf, +1));
            Assert.AreEqual("C6H6O", MolecularFormulaManipulator.GetString(mf));
            Assert.AreEqual(0, mf.Charge);
            Assert.AreEqual(3, mf.Isotopes.Count());
        }

        [TestMethod()]
        public void DeprontateHCl()
        {
            var bldr = CDK.Builder;
            var mf = MolecularFormulaManipulator.GetMolecularFormula("HCl", bldr);
            Assert.IsTrue(MolecularFormulaManipulator.AdjustProtonation(mf, -1));
            Assert.AreEqual("[Cl]-", MolecularFormulaManipulator.GetString(mf));
            Assert.AreEqual(-1, mf.Charge);
            Assert.AreEqual(1, mf.Isotopes.Count());
        }

        [TestMethod()]
        public void ProntateChloride()
        {
            var bldr = CDK.Builder;
            var mf = MolecularFormulaManipulator.GetMolecularFormula("[Cl]-", bldr);
            Assert.IsTrue(MolecularFormulaManipulator.AdjustProtonation(mf, +1));
            Assert.AreEqual("ClH", MolecularFormulaManipulator.GetString(mf));
            Assert.AreEqual(0, mf.Charge);
            Assert.AreEqual(2, mf.Isotopes.Count());
        }

        [TestMethod()]
        public void DeprontateChloride()
        {
            var bldr = CDK.Builder;
            var mf = MolecularFormulaManipulator.GetMolecularFormula("[Cl]-", bldr);
            Assert.IsFalse(MolecularFormulaManipulator.AdjustProtonation(mf, -1));
        }

        [TestMethod()]
        public void ProtonateDeuteratedPhenolate()
        {
            var bldr = CDK.Builder;
            var mf = bldr.NewMolecularFormula();
            // [C6DH4O]- (parser not good enough ATM so need to create like this)
            var deuterium = BODRIsotopeFactory.Instance.GetIsotope("H", 2);
            var hydrogen = BODRIsotopeFactory.Instance.GetMajorIsotope(1);
            mf.Add(deuterium, 1);
            mf.Add(hydrogen, 4);
            mf.Add(BODRIsotopeFactory.Instance.GetMajorIsotope(6), 6);
            mf.Add(BODRIsotopeFactory.Instance.GetMajorIsotope(8), 1);
            mf.Charge = -1;
            Assert.IsTrue(MolecularFormulaManipulator.AdjustProtonation(mf, +1));
            Assert.AreEqual("C6H6O", MolecularFormulaManipulator.GetString(mf));
            Assert.AreEqual(0, mf.Charge);
            Assert.AreEqual(4, mf.Isotopes.Count());
            Assert.AreEqual(1, mf.GetCount(deuterium));
            Assert.AreEqual(5, mf.GetCount(hydrogen));
        }

        [TestMethod()]
        public void TestMassNumberDisplay()
        {
            var ifac = BODRIsotopeFactory.Instance;
            var bldr = CDK.Builder;
            var mf = bldr.NewMolecularFormula();
            mf.Add(builder.NewAtom("C"), 7);
            mf.Add(builder.NewAtom("O"), 3);
            mf.Add(builder.NewAtom("H"), 3);
            mf.Add(builder.NewAtom("Br"), 1);
            mf.Add(ifac.GetIsotope("Br", 81), 1);
            Assert.AreEqual("C7H3Br2O3", MolecularFormulaManipulator.GetString(mf, false, false));
            Assert.AreEqual("C7H3Br[81]BrO3", MolecularFormulaManipulator.GetString(mf, false, true));
        }

        [TestMethod()]
        public void TestMassNumberDisplayWithDefinedIsotopes()
        {
            var ifac = BODRIsotopeFactory.Instance;
            var bldr = CDK.Builder;
            var mf = bldr.NewMolecularFormula();
            mf.Add(ifac.GetMajorIsotope("C"), 7);
            mf.Add(ifac.GetMajorIsotope("O"), 3);
            mf.Add(ifac.GetMajorIsotope("H"), 3);
            mf.Add(ifac.GetMajorIsotope("Br"), 1);
            mf.Add(ifac.GetIsotope("Br", 81), 1);
            BODRIsotopeFactory.ClearMajorIsotopes(mf);
            Assert.AreEqual("C7H3Br2O3", MolecularFormulaManipulator.GetString(mf, false, false));
            Assert.AreEqual("C7H3Br[81]BrO3", MolecularFormulaManipulator.GetString(mf, false, true));
        }

        [TestMethod()]
        public void ParseMFMass()
        {
            var str = "C7H3[81]BrBrO3";
            var builder = CDK.Builder;
            var mf = MolecularFormulaManipulator.GetMolecularFormula(str, builder);
            Assert.AreEqual("C7H3Br[81]BrO3", MolecularFormulaManipulator.GetString(mf, false, true));
        }

        [TestMethod()]
        public void TestRoundTripCharge()
        {
            var f = "[C3H7]+";
            var m = MolecularFormulaManipulator.GetMolecularFormula(f, CDK.Builder);
            Assert.AreEqual("[C3H7]+", MolecularFormulaManipulator.GetString(m));
        }
    }
}
