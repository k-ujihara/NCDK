using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Tools.Manipulator;

namespace NCDK.Formula
{
    /**
     * Class testing the IsotopePatternSimilarity class.
     *
     * @cdk.module test-formula
     */
    [TestClass()]
    public class IsotopePatternSimilarityTest : CDKTestCase
    {

        private readonly static IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        public IsotopePatternSimilarityTest()
            : base()
        { }

        [TestMethod()]
        public void TestIsotopePatternSimilarity()
        {
            IsotopePatternSimilarity is_ = new IsotopePatternSimilarity();
            Assert.IsNotNull(is_);
        }

        [TestMethod()]
        public void TestSeTolerance_Double()
        {
            IsotopePatternSimilarity is_ = new IsotopePatternSimilarity();
            is_.Tolerance = 0.001;
            Assert.IsNotNull(is_);
        }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestGetTolerance()
        {
            IsotopePatternSimilarity is_ = new IsotopePatternSimilarity();
            is_.Tolerance = 0.001;
            Assert.AreEqual(0.001, is_.Tolerance, 0.000001);
        }

        /**
         * Histidine example
         *
         * @
         */
        [TestMethod()]
        public void TestCompare_IsotopePattern_IsotopePattern()
        {
            var is_ = new IsotopePatternSimilarity();

            IsotopePattern spExp = new IsotopePattern();
            spExp.SetMonoIsotope(new IsotopeContainer(156.07770, 1));
            spExp.Isotopes.Add(new IsotopeContainer(157.07503, 0.0004));
            spExp.Isotopes.Add(new IsotopeContainer(157.08059, 0.0003));
            spExp.Isotopes.Add(new IsotopeContainer(158.08135, 0.002));

            IMolecularFormula formula = MolecularFormulaManipulator.GetMajorIsotopeMolecularFormula("C6H10N3O2", builder);
            IsotopePatternGenerator isotopeGe = new IsotopePatternGenerator(0.1);
            IsotopePattern patternIsoPredicted = isotopeGe.GetIsotopes(formula);
            IsotopePattern patternIsoNormalize = IsotopePatternManipulator.Normalize(patternIsoPredicted);
            double score = is_.Compare(spExp, patternIsoNormalize);
            Assert.AreNotSame(0.0, score);
        }

        /**
         * Histidine example
         *
         * @
         */
        [TestMethod()]
        public void TestSelectingMF()
        {
            var is_ = new IsotopePatternSimilarity();

            IsotopePattern spExp = new IsotopePattern();
            spExp.Charge = 1;
            spExp.SetMonoIsotope(new IsotopeContainer(156.07770, 1));
            spExp.Isotopes.Add(new IsotopeContainer(157.07503, 0.0101));
            spExp.Isotopes.Add(new IsotopeContainer(157.08059, 0.074));
            spExp.Isotopes.Add(new IsotopeContainer(158.08135, 0.0024));

            double score = 0;
            string mfString = "";
            string[] listMF = { "C4H8N6O", "C2H12N4O4", "C3H12N2O5", "C6H10N3O2", "CH10N5O4", "C4H14NO5" };

            for (int i = 0; i < listMF.Length; i++)
            {
                IMolecularFormula formula = MolecularFormulaManipulator.GetMajorIsotopeMolecularFormula(listMF[i], builder);
                IsotopePatternGenerator isotopeGe = new IsotopePatternGenerator(0.01);
                IsotopePattern patternIsoPredicted = isotopeGe.GetIsotopes(formula);

                IsotopePattern patternIsoNormalize = IsotopePatternManipulator.Normalize(patternIsoPredicted);
                double tempScore = is_.Compare(spExp, patternIsoNormalize);
                if (score < tempScore)
                {
                    mfString = MolecularFormulaManipulator.GetString(formula);
                    score = tempScore;
                }
            }
            Assert.AreEqual("C6H10N3O2", mfString);
        }

        /**
         * Real example. Lipid PC
         *
         * @
         */
        [TestMethod()]
        public void TestExperiment()
        {

            IsotopePattern spExp = new IsotopePattern();
            spExp.SetMonoIsotope(new IsotopeContainer(762.6006, 124118304));
            spExp.Isotopes.Add(new IsotopeContainer(763.6033, 57558840));
            spExp.Isotopes.Add(new IsotopeContainer(764.6064, 15432262));
            spExp.Charge = 1.0;

            IMolecularFormula formula = MolecularFormulaManipulator.GetMajorIsotopeMolecularFormula("C42H85NO8P",
                    Silent.ChemObjectBuilder.Instance);

            IsotopePatternGenerator isotopeGe = new IsotopePatternGenerator(0.01);
            IsotopePattern patternIsoPredicted = isotopeGe.GetIsotopes(formula);

            var is_ = new IsotopePatternSimilarity();
            double score = is_.Compare(spExp, patternIsoPredicted);

            Assert.AreEqual(0.97, score, .01);
        }
    }
}
