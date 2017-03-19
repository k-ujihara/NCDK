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
using NCDK.Tools.Manipulator;

namespace NCDK.Formula
{
    /// <summary>
    /// Checks the functionality of the IsotopePatternGenerator.
    /// </summary>
    // @cdk.module test-formula
    // @author         Miguel Rojas
    // @cdk.created    2007-03-01
    [TestClass()]
    public class IsotopePatternGeneratorTest : CDKTestCase
    {
        private readonly static IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        public IsotopePatternGeneratorTest()
            : base()
        { }

        [TestMethod()]
        public void TestIsotopePatternGenerator()
        {
            IsotopePatternGenerator isotopeGe = new IsotopePatternGenerator();
            Assert.IsNotNull(isotopeGe);
        }

        [TestMethod()]
        public void TestIsotopePatternGenerator_Double()
        {
            IsotopePatternGenerator isotopeGe = new IsotopePatternGenerator();
            Assert.IsNotNull(isotopeGe);
        }

        [TestMethod()]
        public void TestGetIsotopes_IMolecularFormula()
        {
            IMolecularFormula molFor = MolecularFormulaManipulator.GetMajorIsotopeMolecularFormula("C41H79N8O3P1", builder);
            IsotopePatternGenerator isotopeGe = new IsotopePatternGenerator(.1);
            IsotopePattern isos = isotopeGe.GetIsotopes(molFor);
            Assert.AreEqual(2, isos.Isotopes.Count, 0.001);
        }

        [TestMethod()]
        public void TestGetIsotopes_IMolecularFormula_withoutONE()
        {
            IMolecularFormula molFor = MolecularFormulaManipulator.GetMajorIsotopeMolecularFormula("C41H79N8O3P", builder);
            IsotopePatternGenerator isotopeGe = new IsotopePatternGenerator(.01);
            IsotopePattern isos = isotopeGe.GetIsotopes(molFor);
            Assert.AreEqual(6, isos.Isotopes.Count, 0.001);
        }

        /// <summary>
        /// Isotopes of the Bromine.
        /// </summary>
        [TestMethod()]
        public void TestGetIsotopes1()
        {
            IMolecularFormula molFor = new MolecularFormula();
            molFor.Add(builder.CreateIsotope("Br"));
            molFor.Add(builder.CreateIsotope("Br"));

            IsotopePatternGenerator isotopeGe = new IsotopePatternGenerator(.1);
            IsotopePattern isoPattern = isotopeGe.GetIsotopes(molFor);

            Assert.AreEqual(3, isoPattern.Isotopes.Count);

        }

        /// <summary>
        /// Isotopes of the Bromine.
        /// </summary>
        [TestMethod()]
        public void TestCalculateIsotopesAllBromine()
        {
            // RESULTS ACCORDING PAGE: http://www2.sisweb.com/mstools/isotope.htm
            double[] massResults = { 157.836669, 159.834630, 161.832580 };
            double[] abundResults = { .512, 1.00, .487 };

            IMolecularFormula molFor = new MolecularFormula();
            molFor.Add(builder.CreateIsotope("Br"));
            molFor.Add(builder.CreateIsotope("Br"));

            IsotopePatternGenerator isotopeGe = new IsotopePatternGenerator(.1);
            IsotopePattern isoPattern = isotopeGe.GetIsotopes(molFor);

            Assert.AreEqual(3, isoPattern.Isotopes.Count);

            Assert.AreEqual(massResults[0], isoPattern.Isotopes[0].Mass, 0.01);
            Assert.AreEqual(massResults[1], isoPattern.Isotopes[1].Mass, 0.01);
            Assert.AreEqual(massResults[2], isoPattern.Isotopes[2].Mass, 0.01);

            Assert.AreEqual(abundResults[0], isoPattern.Isotopes[0].Intensity, 0.01);
            Assert.AreEqual(abundResults[1], isoPattern.Isotopes[1].Intensity, 0.01);
            Assert.AreEqual(abundResults[2], isoPattern.Isotopes[2].Intensity, 0.01);

        }

        /// <summary>
        /// Isotopes of the Iodemethylidyne.
        /// </summary>
        [TestMethod()]
        public void TestCalculateIsotopesIodemethylidyne()
        {
            // RESULTS ACCORDING PAGE: http://www2.sisweb.com/mstools/isotope.htm
            double[] massResults = { 138.904480, 139.907839 };
            double[] abundResults = { 1.00, .011 };

            IMolecularFormula molFor = new MolecularFormula();
            molFor.Add(builder.CreateIsotope("C"));
            molFor.Add(builder.CreateIsotope("I"));

            Assert.AreEqual(2, molFor.Count);

            IsotopePatternGenerator isotopeGe = new IsotopePatternGenerator(.01);
            IsotopePattern isoPattern = isotopeGe.GetIsotopes(molFor);

            Assert.AreEqual(2, isoPattern.Isotopes.Count);

            Assert.AreEqual(massResults[0], isoPattern.Isotopes[0].Mass, 0.01);
            Assert.AreEqual(massResults[1], isoPattern.Isotopes[1].Mass, 0.01);

            Assert.AreEqual(abundResults[0], isoPattern.Isotopes[0].Intensity, 0.01);
            Assert.AreEqual(abundResults[1], isoPattern.Isotopes[1].Intensity, 0.01);

        }

        /// <summary>
        /// Isotopes of the n-Carbone.
        /// </summary>
        [TestMethod()]
        public void TestCalculateIsotopesnCarbono()
        {
            // RESULTS ACCORDING PAGE: http://www2.sisweb.com/mstools/isotope.htm
            double[] massResults = { 120.000000, 121.003360, 122.006709 };
            double[] abundResults = { 1.00, .108, 0.005 };

            IMolecularFormula molFor = new MolecularFormula();
            molFor.Add(builder.CreateIsotope("C"), 10);

            IsotopePatternGenerator isotopeGe = new IsotopePatternGenerator(0.0010);
            IsotopePattern isoPattern = isotopeGe.GetIsotopes(molFor);

            Assert.AreEqual(3, isoPattern.Isotopes.Count);

            Assert.AreEqual(massResults[0], isoPattern.Isotopes[0].Mass, 0.01);
            Assert.AreEqual(massResults[1], isoPattern.Isotopes[1].Mass, 0.01);
            Assert.AreEqual(massResults[2], isoPattern.Isotopes[2].Mass, 0.01);

            Assert.AreEqual(abundResults[0], isoPattern.Isotopes[0].Intensity, 0.01);
            Assert.AreEqual(abundResults[1], isoPattern.Isotopes[1].Intensity, 0.01);
            Assert.AreEqual(abundResults[2], isoPattern.Isotopes[2].Intensity, 0.01);

        }

        [TestMethod()]
        public void TestCalculateIsotopesOrthinine()
        {
            // RESULTS ACCORDING PAGE: http://www2.sisweb.com/mstools/isotope.htm
            double[] massResults = { 133.097720, 134.094750, 134.101079, 134.103990, 135.101959, 135.104430 };
            double[] abundResults = { 1.00, .006, .054, 0.002, 0.004, 0.001 };

            IMolecularFormula molFor = new MolecularFormula();
            molFor.Add(builder.CreateIsotope("C"), 5);
            molFor.Add(builder.CreateIsotope("H"), 13);
            molFor.Add(builder.CreateIsotope("N"), 2);
            molFor.Add(builder.CreateIsotope("O"), 2);

            IsotopePatternGenerator isotopeGe = new IsotopePatternGenerator(0.0010);
            IsotopePattern isoPattern = isotopeGe.GetIsotopes(molFor);

            Assert.AreEqual(6, isoPattern.Isotopes.Count);

            Assert.AreEqual(massResults[0], isoPattern.Isotopes[0].Mass, 0.01);
            Assert.AreEqual(massResults[1], isoPattern.Isotopes[1].Mass, 0.01);
            Assert.AreEqual(massResults[2], isoPattern.Isotopes[2].Mass, 0.01);
            Assert.AreEqual(massResults[3], isoPattern.Isotopes[3].Mass, 0.01);
            Assert.AreEqual(massResults[4], isoPattern.Isotopes[4].Mass, 0.01);
            Assert.AreEqual(massResults[5], isoPattern.Isotopes[5].Mass, 0.01);

            Assert.AreEqual(abundResults[0], isoPattern.Isotopes[0].Intensity, 0.01);
            Assert.AreEqual(abundResults[1], isoPattern.Isotopes[1].Intensity, 0.01);
            Assert.AreEqual(abundResults[2], isoPattern.Isotopes[2].Intensity, 0.01);
            Assert.AreEqual(abundResults[3], isoPattern.Isotopes[3].Intensity, 0.01);
            Assert.AreEqual(abundResults[4], isoPattern.Isotopes[4].Intensity, 0.01);
            Assert.AreEqual(abundResults[5], isoPattern.Isotopes[5].Intensity, 0.01);

        }

        /// <summary>
        // @cdk.bug 3273205
        /// </summary>
        [TestMethod()]
        public void TestCalculateIsotopesMn()
        {
            IMolecularFormula molFor = new MolecularFormula();
            molFor.Add(builder.CreateIsotope("Mn"), 1);

            IsotopePatternGenerator isotopeGe = new IsotopePatternGenerator(0.001);
            IsotopePattern isoPattern = isotopeGe.GetIsotopes(molFor);

            Assert.AreEqual(1, isoPattern.Isotopes.Count);
        }

        /// <summary>
        /// Calculate isotopes for C10000.
        /// </summary>
        /// <remarks>failed in CDK 1.5.12.</remarks>
        [TestMethod()]
        public void TestCalculateIsotopesC10000()
        {
            IMolecularFormula molFor = MolecularFormulaManipulator.GetMajorIsotopeMolecularFormula("C10000", builder);
            IsotopePatternGenerator isotopeGe = new IsotopePatternGenerator(.1);
            IsotopePattern isos = isotopeGe.GetIsotopes(molFor);
            Assert.AreEqual(44, isos.Isotopes.Count);
            for (int i = 0; i < isos.Isotopes.Count; i++)
                Assert.IsTrue(isos.Isotopes[i].Mass > 120085);
        }

        /// <summary>
        /// Calculate isotopes for C20H30Fe2P2S4Cl4 (in CDK 1.5.12, this call 
        /// sometimes returns 34 and sometimes 35 isotopes, non-deterministically).
        /// </summary>
        [TestMethod()]
        public void TestCalculateIsotopesC20H30Fe2P2S4Cl4()
        {
            IMolecularFormula molFor = MolecularFormulaManipulator.GetMajorIsotopeMolecularFormula("C20H30Fe2P2S4Cl4", builder);
            IsotopePatternGenerator isotopeGe = new IsotopePatternGenerator(.01);
            IsotopePattern isos = isotopeGe.GetIsotopes(molFor);
            Assert.IsTrue(isos.Isotopes.Count == 34 || isos.Isotopes.Count == 35);
        }
    }
}
