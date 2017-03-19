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
using NCDK.Default;
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
    ///
    // @cdk.module test-formula
    /// </summary>
    [TestClass()]
    public class MolecularFormulaManipulatorTest : CDKTestCase
    {

        private readonly static IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;
        private IsotopeFactory ifac;

        /// <summary>
        ///  Constructor for the MolecularFormulaManipulatorTest object.
        /// </summary>
        public MolecularFormulaManipulatorTest()
            : base()
        {
            try
            {
                ifac = Isotopes.Instance;
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

            IMolecularFormula formula = new MolecularFormula();
            formula.Add(builder.CreateIsotope("C"));
            formula.Add(builder.CreateIsotope("H"), 3);

            Assert.AreEqual(2, formula.Isotopes.Count());

            Assert.AreEqual(4, MolecularFormulaManipulator.GetAtomCount(formula));
        }

        /// <summary>
        /// Test molecular formula's generated from IIsotopes, including hydrogen/deuterium handling.
        /// </summary>
        [TestMethod()]
        public void TestGetElementCount_IMolecularFormula_IElement()
        {
            IMolecularFormula formula = new MolecularFormula();
            IIsotope carb = builder.CreateIsotope("C");
            IIsotope flu = builder.CreateIsotope("F");
            IIsotope h1 = builder.CreateIsotope("H");
            IIsotope h2 = builder.CreateIsotope("H");
            h2.ExactMass = 2.014101778;
            formula.Add(carb, 2);
            formula.Add(flu);
            formula.Add(h1, 3);
            formula.Add(h2, 4);

            Assert.AreEqual(10, MolecularFormulaManipulator.GetAtomCount(formula));
            Assert.AreEqual(4, formula.Isotopes.Count());
            Assert.AreEqual(3, formula.GetCount(h1));
            Assert.AreEqual(4, formula.GetCount(h2));

            Assert.AreEqual(2,
                MolecularFormulaManipulator.GetElementCount(formula, builder.CreateElement(carb)));
            Assert.AreEqual(1,
                    MolecularFormulaManipulator.GetElementCount(formula, builder.CreateElement(flu)));
            Assert.AreEqual(7,
                    MolecularFormulaManipulator.GetElementCount(formula, builder.CreateElement(h1)));
        }

        /// <summary>
        /// Test.Isotopes for hydrogen/deuterium.
        /// </summary>
        [TestMethod()]
        public void TestGetIsotopes_IMolecularFormula_IElement()
        {
            IMolecularFormula formula = new MolecularFormula();
            IIsotope carb = builder.CreateIsotope("C");
            IIsotope flu = builder.CreateIsotope("F");
            IIsotope h1 = builder.CreateIsotope("H");
            IIsotope h2 = builder.CreateIsotope("H");
            h2.ExactMass = 2.014101778;
            formula.Add(carb, 1);
            formula.Add(flu);
            formula.Add(h1, 1);
            formula.Add(h2, 2);

            var isotopes = MolecularFormulaManipulator.GetIsotopes(formula,
                            builder.CreateElement("H"));
            Assert.AreEqual(2, isotopes.Count());
        }

        [TestMethod()]
        public void TestContainsElement_IMolecularFormula_IElement()
        {
            IMolecularFormula formula = new MolecularFormula();
            IIsotope carb = builder.CreateIsotope("C");
            IIsotope flu = builder.CreateIsotope("F");
            IIsotope h1 = builder.CreateIsotope("H");
            IIsotope h2 = builder.CreateIsotope("H");
            h2.ExactMass = 2.014101778;
            formula.Add(carb, 1);
            formula.Add(flu);
            formula.Add(h1, 1);
            formula.Add(h2, 2);

            Assert.IsTrue(MolecularFormulaManipulator.ContainsElement(formula, builder.CreateElement("C")));
            Assert.IsTrue(MolecularFormulaManipulator.ContainsElement(formula, builder.CreateElement("H")));
            Assert.IsTrue(MolecularFormulaManipulator.ContainsElement(formula, builder.CreateElement("F")));
        }

        [TestMethod()]
        public void TestGetString_IMolecularFormula_Empty()
        {
            string stringMF = MolecularFormulaManipulator.GetString(new MolecularFormula());
            Assert.IsNotNull(stringMF);
            Assert.AreEqual("", stringMF);
        }

        /// <summary>Test if formula re-ordering to a user-specified element order works</summary>
        [TestMethod()]
        public void TestGetString_IMolecularFormula_arrayString_boolean()
        {
            IMolecularFormula formula = new MolecularFormula();
            formula.Add(builder.CreateIsotope("C"), 2);
            formula.Add(builder.CreateIsotope("H"), 2);
            Assert.AreEqual("C2H2", MolecularFormulaManipulator.GetString(formula));

            string[] newOrder = new string[2];
            newOrder[0] = "H";
            newOrder[1] = "C";

            Assert.AreEqual("H2C2", MolecularFormulaManipulator.GetString(formula, newOrder, true));

        }

        /// <summary>Test if isotope-list re-ordering to a user-specified element order works</summary>
        [TestMethod()]
        public void TestPutInOrder_arrayString_IMolecularFormula()
        {
            IMolecularFormula formula = new MolecularFormula();
            formula.Add(builder.CreateIsotope("C"), 2);
            formula.Add(builder.CreateIsotope("H"), 2);

            string[] newOrder = new string[2];
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
            IMolecularFormula mf1 = new MolecularFormula();
            mf1.Add(builder.CreateIsotope("C"), 10);
            mf1.Add(builder.CreateIsotope("H"), 16);

            Assert.AreEqual("C10H16", MolecularFormulaManipulator.GetString(mf1));

            IMolecularFormula mf2 = new MolecularFormula();
            mf2.Add(builder.CreateAtom("H"), 16);
            mf2.Add(builder.CreateAtom("C"), 10);

            Assert.AreEqual("C10H16", MolecularFormulaManipulator.GetString(mf2));

            Assert.AreEqual(MolecularFormulaManipulator.GetString(mf2), MolecularFormulaManipulator.GetString(mf1));

        }

        /// <summary>
        // @cdk.bug 2276507
        /// </summary>
        [TestMethod()]
        public void TestBug2276507()
        {
            IMolecularFormula mf1 = new MolecularFormula();
            mf1.Add(builder.CreateIsotope("C"), 1);
            mf1.Add(builder.CreateIsotope("H"), 4);

            Assert.AreEqual("CH4", MolecularFormulaManipulator.GetString(mf1));
        }

        /// <summary>
        /// Test setOne parameter for {@link MolecularFormulaManipulator#GetString(IMolecularFormula, bool)}
        /// </summary>
        [TestMethod()]
        public void TestGetString_IMolecularFormula_boolean()
        {
            IMolecularFormula mf1 = new MolecularFormula();
            mf1.Add(builder.CreateIsotope("C"), 1);
            mf1.Add(builder.CreateIsotope("H"), 4);

            Assert.AreEqual("C1H4", MolecularFormulaManipulator.GetString(mf1, true));
        }

        /// <summary>Test if formulae group elements when not inserted simultaneously</summary>
        [TestMethod()]
        public void TestGetString_Isotopes()
        {
            IMolecularFormula mf1 = new MolecularFormula();
            mf1.Add(builder.CreateIsotope("C", 12), 9);
            mf1.Add(builder.CreateIsotope("C", 13), 1);
            mf1.Add(builder.CreateIsotope("H"), 16);

            Assert.AreEqual("C10H16", MolecularFormulaManipulator.GetString(mf1));
        }

        [TestMethod()]
        public void TestGetMolecularFormula_String_IChemObjectBuilder()
        {
            IMolecularFormula molecularFormula = MolecularFormulaManipulator.GetMolecularFormula("C10H16", builder);

            Assert.AreEqual(26, MolecularFormulaManipulator.GetAtomCount(molecularFormula));
            Assert.AreEqual(2, molecularFormula.Isotopes.Count());

        }

        /// <summary>Test formula summing</summary>
        [TestMethod()]
        public void TestGetMolecularFormula_String_IMolecularFormula()
        {

            IMolecularFormula mf1 = new MolecularFormula();
            mf1.Add(builder.CreateIsotope("C"), 10);
            mf1.Add(builder.CreateIsotope("H"), 16);

            Assert.AreEqual(26, MolecularFormulaManipulator.GetAtomCount(mf1));
            Assert.AreEqual(2, mf1.Isotopes.Count());

            IMolecularFormula mf2 = MolecularFormulaManipulator.GetMolecularFormula("C11H17", mf1);

            Assert.AreEqual(54, MolecularFormulaManipulator.GetAtomCount(mf2));
            Assert.AreEqual(2, mf2.Isotopes.Count());
        }

        /// <summary>Test formula mass calculation</summary>
        [TestMethod()]
        public void TestGetMajorIsotopeMolecularFormula_String_IChemObjectBuilder()
        {
            IMolecularFormula mf2 = MolecularFormulaManipulator.GetMajorIsotopeMolecularFormula("C11H17", builder);

            Assert.AreEqual(28, MolecularFormulaManipulator.GetAtomCount(mf2));
            Assert.AreEqual(2, mf2.Isotopes.Count());
            IIsotope carbon = Isotopes.Instance.GetMajorIsotope("C");
            IIsotope hydrogen = Isotopes.Instance.GetMajorIsotope("H");
            double totalMass = carbon.ExactMass.Value * 11;
            totalMass += hydrogen.ExactMass.Value * 17;
            Assert.AreEqual(totalMass, MolecularFormulaManipulator.GetTotalExactMass(mf2), 0.0000001);
        }

        /// <summary>test @link {@link MolecularFormulaManipulator#RemoveElement(IMolecularFormula, IElement)}</summary>
        [TestMethod()]
        public void TestRemoveElement_IMolecularFormula_IElement()
        {
            IMolecularFormula formula = new MolecularFormula();
            formula.Add(builder.CreateIsotope("C"), 1);
            IIsotope fl = builder.CreateIsotope("F");
            IIsotope hy2 = builder.CreateIsotope("H");
            IIsotope hy1 = builder.CreateIsotope("H");
            hy2.ExactMass = 2.014101778;
            formula.Add(fl, 1);
            formula.Add(hy1, 2);
            formula.Add(hy2, 1);

            Assert.AreEqual(4, formula.Isotopes.Count());

            formula = MolecularFormulaManipulator.RemoveElement(formula, builder.CreateElement("F"));

            Assert.AreEqual(3, formula.Isotopes.Count());
            Assert.AreEqual(4, MolecularFormulaManipulator.GetAtomCount(formula));

            formula = MolecularFormulaManipulator.RemoveElement(formula, builder.CreateElement("H"));

            Assert.AreEqual(1, MolecularFormulaManipulator.GetAtomCount(formula));
            Assert.AreEqual(1, formula.Isotopes.Count());

        }

        /// <summary>
        /// Test total Exact Mass.
        /// </summary>
        [TestMethod()]
        public void TestGetTotalExactMass_IMolecularFormula()
        {

            IMolecularFormula formula = new MolecularFormula();
            IIsotope carb = builder.CreateIsotope("C");
            carb.ExactMass = 12.00;
            IIsotope cl = builder.CreateIsotope("Cl");
            cl.ExactMass = 34.96885268;

            formula.Add(carb);
            formula.Add(cl);

            double totalExactMass = MolecularFormulaManipulator.GetTotalExactMass(formula);

            Assert.AreEqual(46.96885268, totalExactMass, 0.000001);
        }

        /// <summary>
        /// Test total Exact Mass.
        ///
        // @throws IOException
        // @throws ClassNotFoundException
        /// <exception cref="CDKException"></exception>
        /// </summary>
        [TestMethod()]
        public void TestGetTotalExactMassWithCharge_IMolecularFormula()
        {

            IMolecularFormula formula = MolecularFormulaManipulator.GetMajorIsotopeMolecularFormula("CH5O", builder);

            double totalExactMass = MolecularFormulaManipulator.GetTotalExactMass(formula);
            Assert.AreEqual(33.034040, totalExactMass, 0.0001);

            formula.Charge = 1;
            double totalExactMass2 = MolecularFormulaManipulator.GetTotalExactMass(formula);
            Assert.AreEqual(33.03349, totalExactMass2, 0.0001);
        }

        /// <summary>
        /// Test total Exact Mass.
        ///
        // @throws IOException
        // @throws ClassNotFoundException
        /// <exception cref="CDKException"></exception>
        /// </summary>
        [TestMethod()]
        public void TestGetTotalExactMassWithChargeNeg_IMolecularFormula()
        {

            IMolecularFormula formula = MolecularFormulaManipulator.GetMajorIsotopeMolecularFormula("H2PO4", builder);
            formula.Charge = -1;
            double totalExactMass2 = MolecularFormulaManipulator.GetTotalExactMass(formula);
            Assert.AreEqual(96.96961875390926, totalExactMass2, 0.0001);
        }

        [TestMethod()]
        public void TestGetNaturalExactMass_IMolecularFormula()
        {
            IMolecularFormula formula = new MolecularFormula();
            formula.Add(builder.CreateIsotope("C"));
            formula.Add(builder.CreateIsotope("Cl"));

            double expectedMass = 0.0;
            expectedMass += Isotopes.Instance.GetNaturalMass(builder.CreateElement("C"));
            expectedMass += Isotopes.Instance.GetNaturalMass(builder.CreateElement("Cl"));

            double totalExactMass = MolecularFormulaManipulator.GetNaturalExactMass(formula);
            Assert.AreEqual(expectedMass, totalExactMass, 0.000001);
        }

        [TestMethod()]
        public void TestGetTotalMassNumber_IMolecularFormula()
        {
            IMolecularFormula formula = new MolecularFormula();
            formula.Add(builder.CreateIsotope("C"));
            formula.Add(builder.CreateIsotope("O"));

            double totalExactMass = MolecularFormulaManipulator.GetTotalMassNumber(formula);
            Assert.AreEqual(28, totalExactMass, 0.000001);
        }

        [TestMethod()]
        public void TestGetMajorIsotopeMass_IMolecularFormula()
        {
            IMolecularFormula formula = new MolecularFormula();
            formula.Add(builder.CreateIsotope("C"));
            formula.Add(builder.CreateIsotope("H"), 4);

            double expectedMass = 0.0;
            expectedMass += Isotopes.Instance.GetMajorIsotope("C").ExactMass.Value;
            expectedMass += 4.0 * Isotopes.Instance.GetMajorIsotope("H").ExactMass.Value;

            double totalExactMass = MolecularFormulaManipulator.GetMajorIsotopeMass(formula);
            Assert.AreEqual(expectedMass, totalExactMass, 0.000001);
        }

        /// <summary>
        /// Test total Exact Mass. It is necessary to have added the
        /// corresponding isotope before to calculate the exact mass.
        ///
        /// </summary>
        [TestMethod()]
        public void TestBug_1944604()
        {

            IMolecularFormula formula = new MolecularFormula();
            IIsotope carb = builder.CreateIsotope("C");

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

            IMolecularFormula formula = new MolecularFormula();
            IIsotope carb = builder.CreateIsotope("C");
            carb.NaturalAbundance = 98.93;
            IIsotope cl = builder.CreateIsotope("Cl");
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
            IIsotope br1 = builder.CreateIsotope("Br");
            br1.NaturalAbundance = 49.31;
            IIsotope br2 = builder.CreateIsotope("Br");
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
            IIsotope br1 = builder.CreateIsotope("Br");
            br1.NaturalAbundance = 50.69;
            IIsotope br2 = builder.CreateIsotope("Br");
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
            IIsotope br1 = builder.CreateIsotope("Br");
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
            IMolecularFormula formula = new MolecularFormula();
            formula.Add(builder.CreateIsotope("C"), 10);
            formula.Add(builder.CreateIsotope("H"), 22);

            Assert.AreEqual(0.0, MolecularFormulaManipulator.GetDBE(formula), 0.01);

            formula = new MolecularFormula();
            formula.Add(builder.CreateIsotope("C"), 10);
            formula.Add(builder.CreateIsotope("H"), 16);

            Assert.AreEqual(3.0, MolecularFormulaManipulator.GetDBE(formula), 0.01);

            formula = new MolecularFormula();
            formula.Add(builder.CreateIsotope("C"), 10);
            formula.Add(builder.CreateIsotope("H"), 16);
            formula.Add(builder.CreateIsotope("O"));

            Assert.AreEqual(3.0, MolecularFormulaManipulator.GetDBE(formula), 0.01);

            formula = new MolecularFormula();
            formula.Add(builder.CreateIsotope("C"), 10);
            formula.Add(builder.CreateIsotope("H"), 19);
            formula.Add(builder.CreateIsotope("N"));

            Assert.AreEqual(2.0, MolecularFormulaManipulator.GetDBE(formula), 0.01);

        }

        [TestMethod()]
        public void TestGetHTML_IMolecularFormula()
        {
            MolecularFormula formula = new MolecularFormula();
            formula.Add(builder.CreateIsotope("C"), 8);
            formula.Add(builder.CreateIsotope("H"), 10);
            formula.Add(builder.CreateIsotope("Cl"), 2);
            formula.Add(builder.CreateIsotope("O"), 2);

            Assert.AreEqual("C<sub>8</sub>H<sub>10</sub>Cl<sub>2</sub>O<sub>2</sub>",
                    MolecularFormulaManipulator.GetHTML(formula));
        }

        [TestMethod()]
        public void HtmlFormulaDoesNotAddSubscriptForSingleElements()
        {
            MolecularFormula formula = new MolecularFormula();
            formula.Add(builder.CreateIsotope("C"), 1);
            formula.Add(builder.CreateIsotope("H"), 4);

            Assert.AreEqual("CH<sub>4</sub>", MolecularFormulaManipulator.GetHTML(formula));
        }

        [TestMethod()]
        public void TestGetHTML_IMolecularFormula_boolean_boolean()
        {
            MolecularFormula formula = new MolecularFormula();
            formula.Add(builder.CreateIsotope("C"), 10);

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
            formula.Add(builder.CreateIsotope("C"), 10);

            Assert.AreEqual("C<sub>10</sub>", MolecularFormulaManipulator.GetHTML(formula, true, false));
            formula.Charge = 1;
            Assert.AreEqual("C<sub>10</sub><sup>+</sup>", MolecularFormulaManipulator.GetHTML(formula, true, true));
            formula.Charge = formula.Charge - 2;
            Assert.AreEqual("C<sub>10</sub><sup>–</sup>", MolecularFormulaManipulator.GetHTML(formula, true, true));
        }

        [TestMethod()]
        public void TestGetHTML_IMolecularFormula_arrayString_boolean_boolean()
        {
            IMolecularFormula formula = new MolecularFormula();
            formula.Add(builder.CreateIsotope("C"), 2);
            formula.Add(builder.CreateIsotope("H"), 2);

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
            IAtomContainer ac = builder.CreateAtomContainer();
            ac.Atoms.Add(builder.CreateAtom("C"));
            ac.Atoms.Add(builder.CreateAtom("C"));
            ac.Atoms.Add(builder.CreateAtom("H"));
            ac.Atoms.Add(builder.CreateAtom("H"));
            ac.Atoms.Add(builder.CreateAtom("H"));
            ac.Atoms.Add(builder.CreateAtom("H"));

            IMolecularFormula mf1 = MolecularFormulaManipulator.GetMolecularFormula(ac);

            IMolecularFormula mf2 = new MolecularFormula();
            mf2.Add(builder.CreateIsotope("C"), 2);
            mf2.Add(builder.CreateIsotope("H"), 4);

            Assert.AreEqual(MolecularFormulaManipulator.GetAtomCount(mf2),
                    MolecularFormulaManipulator.GetAtomCount(mf1));
            Assert.AreEqual(mf2.Isotopes.Count(), mf1.Isotopes.Count());
            IElement elemC = builder.CreateElement("C");
            IElement elemH = builder.CreateElement("H");
            Assert.AreEqual(mf2.GetCount(builder.CreateIsotope(elemC)),
                    mf1.GetCount(builder.CreateIsotope(elemC)));
            Assert.AreEqual(mf2.GetCount(builder.CreateIsotope(elemH)),
                    mf1.GetCount(builder.CreateIsotope(elemH)));
            Assert.AreEqual(MolecularFormulaManipulator.GetElementCount(mf2, elemC),
                    MolecularFormulaManipulator.GetElementCount(mf1, elemC));
            Assert.AreEqual(MolecularFormulaManipulator.GetElementCount(mf2, elemH),
                    MolecularFormulaManipulator.GetElementCount(mf1, elemH));

        }

        [TestMethod()]
        public void TestGetMolecularFormula_IAtomNullCharge()
        {
            IAtomContainer ac = builder.CreateAtomContainer();
            ac.Atoms.Add(builder.CreateAtom("C"));
            ac.Atoms[0].FormalCharge = null;
            ac.Atoms.Add(builder.CreateAtom("C"));
            ac.Atoms.Add(builder.CreateAtom("H"));
            ac.Atoms.Add(builder.CreateAtom("H"));
            ac.Atoms.Add(builder.CreateAtom("H"));
            ac.Atoms.Add(builder.CreateAtom("H"));

            IMolecularFormula mf1 = MolecularFormulaManipulator.GetMolecularFormula(ac);
            Assert.IsNotNull(mf1);
        }

        [TestMethod()]
        public void TestGetMolecularFormula_IAtomContainer_withCharge()
        {
            IAtomContainer ac = builder.CreateAtomContainer();
            ac.Atoms.Add(builder.CreateAtom("C"));
            ac.Atoms[0].FormalCharge = 1;
            ac.Atoms.Add(builder.CreateAtom("C"));
            ac.Atoms.Add(builder.CreateAtom("H"));
            ac.Atoms.Add(builder.CreateAtom("H"));
            ac.Atoms.Add(builder.CreateAtom("H"));

            IMolecularFormula mf1 = MolecularFormulaManipulator.GetMolecularFormula(ac);

            Assert.AreEqual(1, mf1.Charge.Value, 0.000);
        }

        [TestMethod()]
        public void TestGetMolecularFormula_IAtomContainer_IMolecularFormula()
        {
            IAtomContainer ac = builder.CreateAtomContainer();
            ac.Atoms.Add(builder.CreateAtom("C"));
            ac.Atoms.Add(builder.CreateAtom("C"));
            ac.Atoms.Add(builder.CreateAtom("H"));
            ac.Atoms.Add(builder.CreateAtom("H"));
            ac.Atoms.Add(builder.CreateAtom("H"));
            ac.Atoms.Add(builder.CreateAtom("H"));

            IMolecularFormula mf1 = MolecularFormulaManipulator.GetMolecularFormula(ac, new MolecularFormula());

            IMolecularFormula mf2 = new MolecularFormula();
            mf2.Add(builder.CreateIsotope("C"), 2);
            mf2.Add(builder.CreateIsotope("H"), 4);

            Assert.AreEqual(MolecularFormulaManipulator.GetAtomCount(mf2),
                    MolecularFormulaManipulator.GetAtomCount(mf1));
            Assert.AreEqual(mf2.Isotopes.Count(), mf1.Isotopes.Count());
            IElement elemC = builder.CreateElement("C");
            IElement elemH = builder.CreateElement("H");
            Assert.AreEqual(mf2.GetCount(builder.CreateIsotope(elemC)),
                    mf1.GetCount(builder.CreateIsotope(elemC)));
            Assert.AreEqual(mf2.GetCount(builder.CreateIsotope(elemH)),
                    mf1.GetCount(builder.CreateIsotope(elemH)));
            Assert.AreEqual(MolecularFormulaManipulator.GetElementCount(mf2, elemC),
                    MolecularFormulaManipulator.GetElementCount(mf1, elemC));
            Assert.AreEqual(MolecularFormulaManipulator.GetElementCount(mf2, elemH),
                    MolecularFormulaManipulator.GetElementCount(mf1, elemH));

        }

        [TestMethod()]
        public void TestGetMolecularFormula_IAtomContainerIMolecularFormula_2()
        {
            IAtomContainer ac = builder.CreateAtomContainer();
            ac.Atoms.Add(builder.CreateAtom("C"));
            ac.Atoms.Add(builder.CreateAtom("C"));
            ac.Atoms.Add(builder.CreateAtom("H"));
            ac.Atoms.Add(builder.CreateAtom("H"));
            ac.Atoms.Add(builder.CreateAtom("H"));
            ac.Atoms.Add(builder.CreateAtom("H"));

            IMolecularFormula mf0 = new MolecularFormula();
            mf0.Add(builder.CreateIsotope("C"), 2);
            mf0.Add(builder.CreateIsotope("H"), 5);

            IMolecularFormula mf1 = MolecularFormulaManipulator.GetMolecularFormula(ac, mf0);

            IMolecularFormula mf2 = new MolecularFormula();
            mf2.Add(builder.CreateIsotope("C"), 4);
            mf2.Add(builder.CreateIsotope("H"), 9);

            Assert.AreEqual(MolecularFormulaManipulator.GetAtomCount(mf2),
                    MolecularFormulaManipulator.GetAtomCount(mf1));
            Assert.AreEqual(mf2.Isotopes.Count(), mf1.Isotopes.Count());
            IElement elemC = builder.CreateElement("C");
            IElement elemH = builder.CreateElement("H");
            Assert.AreEqual(mf2.GetCount(builder.CreateIsotope(elemC)),
                    mf1.GetCount(builder.CreateIsotope(elemC)));
            Assert.AreEqual(mf2.GetCount(builder.CreateIsotope(elemH)),
                    mf1.GetCount(builder.CreateIsotope(elemH)));
            Assert.AreEqual(MolecularFormulaManipulator.GetElementCount(mf2, elemC),
                    MolecularFormulaManipulator.GetElementCount(mf1, elemC));
            Assert.AreEqual(MolecularFormulaManipulator.GetElementCount(mf2, elemH),
                    MolecularFormulaManipulator.GetElementCount(mf1, elemH));

        }

        [TestMethod()]
        public void TestGetAtomContainer_IMolecularFormula()
        {

            IMolecularFormula mf2 = new MolecularFormula();
            mf2.Add(builder.CreateIsotope("C"), 2);
            mf2.Add(builder.CreateIsotope("H"), 4);

            IAtomContainer ac = MolecularFormulaManipulator.GetAtomContainer(mf2);

            Assert.AreEqual(6, ac.Atoms.Count);

        }

        [TestMethod()]
        public void TestGetAtomContainer_IMolecularFormula_IAtomContainer()
        {

            IMolecularFormula mf2 = new MolecularFormula();
            mf2.Add(builder.CreateIsotope("C"), 2);
            mf2.Add(builder.CreateIsotope("H"), 4);

            IAtomContainer ac = MolecularFormulaManipulator
                    .GetAtomContainer(mf2, builder.CreateAtomContainer());

            Assert.AreEqual(6, ac.Atoms.Count);

        }

        [TestMethod()]
        public void TestGetAtomContainer_String_IChemObjectBuilder()
        {
            string mf = "C2H4";
            IAtomContainer atomContainer = MolecularFormulaManipulator.GetAtomContainer(mf,
                    Default.ChemObjectBuilder.Instance);
            Assert.AreEqual(6, atomContainer.Atoms.Count);
        }

        /// <summary>
        // @cdk.bug 1296
        /// </summary>
        [TestMethod()]
        public void TestGetAtomContainer_AddsAtomicNumbers()
        {
            IMolecularFormula mf2 = new MolecularFormula();
            mf2.Add(builder.CreateIsotope("C"), 2);
            mf2.Add(builder.CreateIsotope("H"), 4);
            IAtomContainer ac = MolecularFormulaManipulator
                    .GetAtomContainer(mf2, builder.CreateAtomContainer());
            Assert.AreEqual(6, ac.Atoms.Count);
            Assert.IsNotNull(ac.Atoms[0].AtomicNumber);
            foreach (var atom in ac.Atoms)
            {
                if ("C".Equals(atom.Symbol))
                    Assert.AreEqual(6, atom.AtomicNumber.Value);
                else if ("H".Equals(atom.Symbol))
                    Assert.AreEqual(1, atom.AtomicNumber.Value);
                else
                    Assert.Fail("Unexcepted element: " + atom.Symbol);
            }
        }

        [TestMethod()]
        public void TestMolecularFormulaIAtomContainer_to_IAtomContainer2()
        {
            IAtomContainer ac = builder.CreateAtomContainer();
            ac.Atoms.Add(builder.CreateAtom("C"));
            ac.Atoms.Add(builder.CreateAtom("C"));
            ac.Atoms.Add(builder.CreateAtom("H"));
            ac.Atoms.Add(builder.CreateAtom("H"));
            ac.Atoms.Add(builder.CreateAtom("H"));
            ac.Atoms.Add(builder.CreateAtom("H"));

            IMolecularFormula mf2 = new MolecularFormula();
            mf2.Add(builder.CreateIsotope("C"), 2);
            mf2.Add(builder.CreateIsotope("H"), 4);

            IAtomContainer ac2 = MolecularFormulaManipulator.GetAtomContainer(mf2,
                    builder.CreateAtomContainer());

            Assert.AreEqual(ac2.Atoms.Count, ac2.Atoms.Count);
            Assert.AreEqual(ac2.Atoms[0].Symbol, ac2.Atoms[0].Symbol);
            Assert.AreEqual(ac2.Atoms[5].Symbol, ac2.Atoms[5].Symbol);

        }

        [TestMethod()]
        public void TestElements_IMolecularFormula()
        {

            IMolecularFormula formula = new MolecularFormula();
            formula.Add(builder.CreateIsotope("C"), 1);
            formula.Add(builder.CreateIsotope("H"), 2);

            IIsotope br1 = builder.CreateIsotope("Br");
            br1.NaturalAbundance = 50.69;
            formula.Add(br1);
            IIsotope br2 = builder.CreateIsotope("Br");
            br2.NaturalAbundance = 50.69;
            formula.Add(br2);

            var elements = MolecularFormulaManipulator.Elements(formula);

            Assert.AreEqual(5, MolecularFormulaManipulator.GetAtomCount(formula));
            Assert.AreEqual(3, elements.Count());
        }

        [TestMethod()]
        public void TestCompare_Charge()
        {

            IMolecularFormula formula1 = new MolecularFormula();
            formula1.Add(builder.CreateIsotope("C"), 1);
            formula1.Add(builder.CreateIsotope("H"), 2);

            IMolecularFormula formula2 = new MolecularFormula();
            formula2.Add(builder.CreateIsotope("C"), 1);
            formula2.Add(builder.CreateIsotope("H"), 2);

            IMolecularFormula formula3 = new MolecularFormula();
            formula3.Add(builder.CreateIsotope("C"), 1);
            formula3.Add(builder.CreateIsotope("H"), 2);
            formula3.Charge = 0;

            Assert.IsTrue(MolecularFormulaManipulator.Compare(formula1, formula2));
            Assert.IsFalse(MolecularFormulaManipulator.Compare(formula1, formula3));

        }

        [TestMethod()]
        public void TestCompare_NumberIsotope()
        {

            IMolecularFormula formula1 = new MolecularFormula();
            formula1.Add(builder.CreateIsotope("C"), 1);
            formula1.Add(builder.CreateIsotope("H"), 2);

            IMolecularFormula formula2 = new MolecularFormula();
            formula2.Add(builder.CreateIsotope("C"), 1);
            formula2.Add(builder.CreateIsotope("H"), 2);

            IMolecularFormula formula3 = new MolecularFormula();
            formula3.Add(builder.CreateIsotope("C"), 1);
            formula3.Add(builder.CreateIsotope("H"), 3);

            Assert.IsTrue(MolecularFormulaManipulator.Compare(formula1, formula2));
            Assert.IsFalse(MolecularFormulaManipulator.Compare(formula1, formula3));

        }

        [TestMethod()]
        public void TestCompare_IMolecularFormula_IMolecularFormula()
        {

            IMolecularFormula formula1 = new MolecularFormula();
            formula1.Add(builder.CreateIsotope("C"), 1);
            formula1.Add(builder.CreateIsotope("H"), 2);

            IMolecularFormula formula2 = new MolecularFormula();
            formula2.Add(builder.CreateIsotope("C"), 1);
            formula2.Add(builder.CreateIsotope("H"), 2);

            IMolecularFormula formula3 = new MolecularFormula();
            formula3.Add(builder.CreateIsotope("C"), 1);
            IIsotope hyd = builder.CreateIsotope("H");
            hyd.ExactMass = 2.002334234;
            formula3.Add(hyd, 2);

            Assert.IsTrue(MolecularFormulaManipulator.Compare(formula1, formula2));
            Assert.IsFalse(MolecularFormulaManipulator.Compare(formula1, formula3));

        }

        [TestMethod()]
        public void TestGetHeavyElements_IMolecularFormula()
        {
            IMolecularFormula formula = new MolecularFormula();
            formula.Add(builder.CreateIsotope("C"), 10);
            formula.Add(builder.CreateIsotope("H"), 16);
            Assert.AreEqual(1, MolecularFormulaManipulator.GetHeavyElements(formula).Count());
        }

        [TestMethod()]
        public void TestGetHeavyElements_IMolecularFormula_2()
        {
            IMolecularFormula formula = MolecularFormulaManipulator.GetMolecularFormula("CH3OH", builder);
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

            string[] arrayGenerated = MolecularFormulaManipulator.OrderEle;
            var listGenerated = arrayGenerated.ToList();
            Assert.AreEqual(113, listGenerated.Count());

            for (int i = 0; i < listElements.Length; i++)
            {
                string element = listElements[i];
                Assert.IsTrue(listGenerated.Contains(element), "Element missing from generateOrderEle: " + element);
            }
        }

        /// <summary>
        /// TODO: REACT: Introduce method
        ///
        // @cdk.bug 2672696
        /// </summary>
        [TestMethod()]
        public void TestGetHillString_IMolecularFormula()
        {
            IMolecularFormula formula = MolecularFormulaManipulator.GetMolecularFormula("CH3OH", builder);
            string listGenerated = MolecularFormulaManipulator.GetHillString(formula);
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
        // @cdk.bug 1944604
        /// </summary>
        [TestMethod()]
        public void TestSingleAtomFromSmiles()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("C"));

            // previously performed inside SmilesParser
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            CDKHydrogenAdder.GetInstance(Default.ChemObjectBuilder.Instance).AddImplicitHydrogens(mol);

            IMolecularFormula mf = MolecularFormulaManipulator.GetMolecularFormula(mol);
            double exactMass = MolecularFormulaManipulator.GetTotalExactMass(mf);
            Assert.AreEqual(16.0313, exactMass, 0.0001);
        }

        [TestMethod()]
        public void TestSingleAtom()
        {
            string formula = "CH4";
            IMolecularFormula mf = MolecularFormulaManipulator.GetMolecularFormula(formula, builder);
            Assert.AreEqual(1,
                    MolecularFormulaManipulator.GetIsotopes(mf, mf.Builder.CreateElement("C")).Count());
        }

        [TestMethod()]
        public void TestSimplifyMolecularFormula_String()
        {
            string formula = "C1H41.H2O";
            string simplifyMF = MolecularFormulaManipulator.SimplifyMolecularFormula(formula);
            Assert.AreEqual("C1H43O", simplifyMF);
        }

        [TestMethod()]
        public void TestSimplifyMolecularFormula_String2()
        {
            string formula = "CH41.H2O";
            string simplifyMF = MolecularFormulaManipulator.SimplifyMolecularFormula(formula);
            Assert.AreEqual("CH43O", simplifyMF);
        }

        [TestMethod()]
        public void TestSimplifygetMF()
        {
            string formula = "CH4.H2O";
            IMolecularFormula formula1 = new MolecularFormula();
            formula1.Add(builder.CreateIsotope("C"), 1);
            formula1.Add(builder.CreateIsotope("H"), 6);
            formula1.Add(builder.CreateIsotope("O"), 1);
            IMolecularFormula ff = MolecularFormulaManipulator.GetMolecularFormula(formula, builder);
            Assert.IsTrue(MolecularFormulaManipulator.Compare(formula1,
                    MolecularFormulaManipulator.GetMolecularFormula(formula, builder)));
            Assert.AreEqual("CH6O", MolecularFormulaManipulator.GetString(ff));
        }

        [TestMethod()]
        public void TestSpace()
        {
            string formula = "C17H21NO. C7H6O3";
            string simplifyMF = MolecularFormulaManipulator.SimplifyMolecularFormula(formula);
            Assert.AreEqual("C24H27NO4", simplifyMF);
        }

        /// <summary>Test molecule simplification</summary>
        [TestMethod()]
        public void Test0()
        {
            string formula = "Fe.(C6H11O7)3";
            string simplifyMF = MolecularFormulaManipulator.SimplifyMolecularFormula(formula);
            Assert.AreEqual("FeC18H33O21", simplifyMF);
        }

        /// <summary>Test molecule simplification</summary>
        [TestMethod()]
        public void Test1()
        {
            string formula = "(C6H11O7)3.Fe";
            string simplifyMF = MolecularFormulaManipulator.SimplifyMolecularFormula(formula);
            Assert.AreEqual("C18H33O21Fe", simplifyMF);
        }

        /// <summary>Test molecule simplification</summary>
        [TestMethod()]
        public void Test2()
        {
            string formula = "C14H14N2.2HCl";
            string simplifyMF = MolecularFormulaManipulator.SimplifyMolecularFormula(formula);
            Assert.AreEqual("C14H16N2Cl2", simplifyMF);
        }

        /// <summary>Test molecule simplification</summary>
        [TestMethod()]
        public void Test3()
        {
            string formula = "(C27H33N3O8)2.2HNO3.3H2O";
            string simplifyMF = MolecularFormulaManipulator.SimplifyMolecularFormula(formula);
            Assert.AreEqual("C54H74N8O25", simplifyMF);
        }

        /// <summary>Test if formula-comparison is simplify-independant</summary>
        [TestMethod()]
        public void Test4()
        {
            string formula = "(C27H33N3O8)2.2HNO3.3H2O";
            IMolecularFormula formula1 = new MolecularFormula();
            formula1.Add(builder.CreateIsotope("C"), 54);
            formula1.Add(builder.CreateIsotope("H"), 74);
            formula1.Add(builder.CreateIsotope("O"), 25);
            formula1.Add(builder.CreateIsotope("N"), 8);
            IMolecularFormula ff = MolecularFormulaManipulator.GetMolecularFormula(formula, builder);
            Assert.IsTrue(MolecularFormulaManipulator.Compare(formula1,
                    MolecularFormulaManipulator.GetMolecularFormula(formula, builder)));
            Assert.AreEqual("C54H74N8O25", MolecularFormulaManipulator.GetString(ff));
        }

        /// <summary>Test if formula-comparison is simplify-independant</summary>
        [TestMethod()]
        public void Test5()
        {
            string formula = "[SO3]2-";
            IMolecularFormula formula1 = new MolecularFormula();
            formula1.Add(builder.CreateIsotope("S"), 1);
            formula1.Add(builder.CreateIsotope("O"), 3);
            formula1.Charge = -2;
            IMolecularFormula ff = MolecularFormulaManipulator.GetMolecularFormula(formula, builder);
            Assert.IsTrue(MolecularFormulaManipulator.Compare(formula1,
                    MolecularFormulaManipulator.GetMolecularFormula(formula, builder)));
            Assert.AreEqual("O3S", MolecularFormulaManipulator.GetString(ff));
            Assert.AreEqual(-2, ff.Charge.Value, 0.00001);
        }

        /// <summary>Test if formula-comparison is simplify-independant</summary>
        [TestMethod()]
        public void Test6()
        {
            string formula = "(CH3)2";
            IMolecularFormula formula1 = new MolecularFormula();
            formula1.Add(builder.CreateIsotope("C"), 2);
            formula1.Add(builder.CreateIsotope("H"), 6);
            IMolecularFormula ff = MolecularFormulaManipulator.GetMolecularFormula(formula, builder);
            Assert.IsTrue(MolecularFormulaManipulator.Compare(formula1,
                    MolecularFormulaManipulator.GetMolecularFormula(formula, builder)));
            Assert.AreEqual("C2H6", MolecularFormulaManipulator.GetString(ff));
        }

        /// <summary>Test if formula-comparison is simplify-independant</summary>
        [TestMethod()]
        public void TestWithH_Initial()
        {
            string formula = "HC5H11NO2H";
            IMolecularFormula formula1 = new MolecularFormula();
            formula1.Add(builder.CreateIsotope("C"), 5);
            formula1.Add(builder.CreateIsotope("H"), 13);
            formula1.Add(builder.CreateIsotope("N"), 1);
            formula1.Add(builder.CreateIsotope("O"), 2);
            IMolecularFormula ff = MolecularFormulaManipulator.GetMolecularFormula(formula, builder);
            Assert.IsTrue(MolecularFormulaManipulator.Compare(formula1,
                    MolecularFormulaManipulator.GetMolecularFormula(formula, builder)));
            Assert.AreEqual("C5H13NO2", MolecularFormulaManipulator.GetString(ff));
        }

        /// <summary>
        // @cdk.bug 3071473
        /// </summary>
        [TestMethod()]
        public void TestFromMol()
        {
            string filename = "NCDK.Data.MDL.formulatest.mol";
            Stream ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile chemFile = reader.Read(new ChemFile());
            reader.Close();
            Assert.IsNotNull(chemFile);
            var mols = ChemFileManipulator.GetAllAtomContainers(chemFile).ToList();
            IAtomContainer mol = mols[0];

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            CDKHydrogenAdder ha = CDKHydrogenAdder.GetInstance(Default.ChemObjectBuilder.Instance);
            ha.AddImplicitHydrogens(mol);
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(mol);

            IMolecularFormula molecularFormula = MolecularFormulaManipulator.GetMolecularFormula(mol);
            string formula2 = MolecularFormulaManipulator.GetString(molecularFormula);
            Assert.IsTrue(formula2.Equals("C35H64N3O21P3S"));
        }

        /// <summary>
        // @cdk.bug 3340660
        /// </summary>
        [TestMethod()]
        public void TestHelium()
        {
            IAtomContainer helium = new AtomContainer();
            helium.Atoms.Add(new Atom("He"));

            IMolecularFormula formula = MolecularFormulaManipulator.GetMolecularFormula(helium);
            Assert.IsNotNull(formula);
            Assert.AreEqual("He", MolecularFormulaManipulator.GetString(formula));
        }

        /// <summary>
        // @cdk.bug 3340660
        /// </summary>
        [TestMethod()]
        public void TestAmericum()
        {
            IAtomContainer helium = new AtomContainer();
            helium.Atoms.Add(new Atom("Am"));

            IMolecularFormula formula = MolecularFormulaManipulator.GetMolecularFormula(helium);
            Assert.IsNotNull(formula);
            Assert.AreEqual("Am", MolecularFormulaManipulator.GetString(formula));
        }

        /// <summary>
        // @cdk.bug 2983334
        /// </summary>
        [TestMethod()]
        public void TestImplicitH()
        {

            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(Silent.ChemObjectBuilder.Instance);

            IAtomContainer mol = TestMoleculeFactory.MakeBenzene();

            IMolecularFormula f = MolecularFormulaManipulator.GetMolecularFormula(mol);
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
            IMolecularFormula formula = new MolecularFormula();
            formula.Add(new Isotope("C"));
            formula.Add(new Isotope("H"), 3);
            formula.Add(new Isotope("R"));
            Assert.AreEqual(15.0234, MolecularFormulaManipulator.GetTotalExactMass(formula), 0.01);
        }

        [TestMethod()]
        public void NoNullPointerExceptionForMassOfRGroups()
        {
            IMolecularFormula formula = new MolecularFormula();
            formula.Add(new Isotope("C"));
            formula.Add(new Isotope("H"), 3);
            formula.Add(new Isotope("R"));
            Assert.AreEqual(15.0, MolecularFormulaManipulator.GetTotalMassNumber(formula), 0.01);
        }

        [TestMethod()]
        public void NoNullPointerExceptionForMajorMassOfRGroups()
        {
            IMolecularFormula formula = new MolecularFormula();
            formula.Add(new Isotope("C"));
            formula.Add(new Isotope("H"), 3);
            formula.Add(new Isotope("R"));
            Assert.AreEqual(15.0234, MolecularFormulaManipulator.GetMajorIsotopeMass(formula), 0.01);
        }

        [TestMethod()]
        public void NoNullPointerForStaticIsotopes()
        {
            Isotopes isotopes = Isotopes.Instance;
            IIsotope carbon = isotopes.GetMajorIsotope("C");
            MolecularFormula mf = new MolecularFormula();
            mf.Add(carbon, 10);
            MolecularFormulaManipulator.GetNaturalExactMass(mf);
        }

        [TestMethod()]
        public void AcceptMinusAsInput()
        {
            IChemObjectBuilder bldr = Silent.ChemObjectBuilder.Instance;
            IMolecularFormula mf = MolecularFormulaManipulator.GetMolecularFormula("[PO4]3–",
                                                                                   bldr);
            Assert.AreEqual(-3, mf.Charge);
        }
    }
}
