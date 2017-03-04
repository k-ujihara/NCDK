using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Formula
{
    /// <summary>
    /// Class testing the IsotopePatternManipulator class.
    ///
    // @cdk.module test-formula
    /// </summary>
    [TestClass()]
    public class IsotopePatternManipulatorTest : CDKTestCase
    {
        public IsotopePatternManipulatorTest()
            : base()
        { }

        [TestMethod()]
        public void TestNormalize_IsotopePattern()
        {
            IsotopePattern spExp = new IsotopePattern();
            spExp.SetMonoIsotope(new IsotopeContainer(156.07770, 2));
            spExp.Isotopes.Add(new IsotopeContainer(157.08059, 0.0006));
            spExp.Isotopes.Add(new IsotopeContainer(157.07503, 0.0002));
            spExp.Isotopes.Add(new IsotopeContainer(158.08135, 0.004));
            spExp.Charge = 1;

            IsotopePattern isoNorma = IsotopePatternManipulator.Normalize(spExp);
            var listISO = isoNorma.Isotopes;
            Assert.AreEqual(1, isoNorma.GetMonoIsotope().Intensity, 0.00001);
            Assert.AreEqual(1, listISO[0].Intensity, 0.00001);
            Assert.AreEqual(0.0003, listISO[1].Intensity, 0.00001);
            Assert.AreEqual(0.0001, listISO[2].Intensity, 0.00001);
            Assert.AreEqual(0.002, listISO[3].Intensity, 0.00001);

            Assert.AreEqual(156.07770, isoNorma.GetMonoIsotope().Mass, 0.00001);
            Assert.AreEqual(156.07770, listISO[0].Mass, 0.00001);
            Assert.AreEqual(157.08059, listISO[1].Mass, 0.00001);
            Assert.AreEqual(157.07503, listISO[2].Mass, 0.00001);
            Assert.AreEqual(158.08135, listISO[3].Mass, 0.00001);

            Assert.AreEqual(1, isoNorma.Charge, 0.00001);

        }

        /// <summary>
        /// Junit test
        ///
        // @
        /// </summary>
        [TestMethod()]
        public void TestSortByIntensity_IsotopePattern()
        {
            IsotopePattern spExp = new IsotopePattern();
            spExp.SetMonoIsotope(new IsotopeContainer(157.07503, 0.0001));
            spExp.Isotopes.Add(new IsotopeContainer(156.07770, 1));
            spExp.Isotopes.Add(new IsotopeContainer(157.08059, 0.0003));
            spExp.Isotopes.Add(new IsotopeContainer(158.08135, 0.002));
            spExp.Charge = 1;

            IsotopePattern isoNorma = IsotopePatternManipulator.SortByIntensity(spExp);
            var listISO = isoNorma.Isotopes;
            Assert.AreEqual(156.07770, isoNorma.GetMonoIsotope().Mass, 0.00001);
            Assert.AreEqual(156.07770, listISO[0].Mass, 0.00001);
            Assert.AreEqual(158.08135, listISO[1].Mass, 0.00001);
            Assert.AreEqual(157.08059, listISO[2].Mass, 0.00001);
            Assert.AreEqual(157.07503, listISO[3].Mass, 0.00001);

            Assert.AreEqual(1, isoNorma.GetMonoIsotope().Intensity, 0.00001);
            Assert.AreEqual(1, listISO[0].Intensity, 0.00001);
            Assert.AreEqual(0.002, listISO[1].Intensity, 0.00001);
            Assert.AreEqual(0.0003, listISO[2].Intensity, 0.001);
            Assert.AreEqual(0.0001, listISO[3].Intensity, 0.00001);

            Assert.AreEqual(1, isoNorma.Charge, 0.00001);
        }

        /// <summary>
        /// Junit test
        ///
        // @
        /// </summary>
        [TestMethod()]
        public void TestSortAndNormalizedByIntensity_IsotopePattern()
        {
            IsotopePattern spExp = new IsotopePattern();
            spExp.Isotopes.Add(new IsotopeContainer(157.07503, 0.0002));
            spExp.SetMonoIsotope(new IsotopeContainer(156.07770, 2));
            spExp.Isotopes.Add(new IsotopeContainer(158.08135, 0.004));
            spExp.Isotopes.Add(new IsotopeContainer(157.08059, 0.0006));
            spExp.Charge = 1;

            IsotopePattern isoNorma = IsotopePatternManipulator.SortAndNormalizedByIntensity(spExp);
            var listISO = isoNorma.Isotopes;
            Assert.AreEqual(156.07770, isoNorma.GetMonoIsotope().Mass, 0.00001);
            Assert.AreEqual(156.07770, listISO[0].Mass, 0.00001);
            Assert.AreEqual(158.08135, listISO[1].Mass, 0.00001);
            Assert.AreEqual(157.08059, listISO[2].Mass, 0.00001);
            Assert.AreEqual(157.07503, listISO[3].Mass, 0.00001);

            Assert.AreEqual(1, isoNorma.GetMonoIsotope().Intensity, 0.00001);
            Assert.AreEqual(1, listISO[0].Intensity, 0.00001);
            Assert.AreEqual(0.002, listISO[1].Intensity, 0.00001);
            Assert.AreEqual(0.0003, listISO[2].Intensity, 0.00001);
            Assert.AreEqual(0.0001, listISO[3].Intensity, 0.00001);

            Assert.AreEqual(1, isoNorma.Charge, 0.001);
        }

        /// <summary>
        /// Junit test
        ///
        // @
        /// </summary>
        [TestMethod()]
        public void TestSortByMass_IsotopePattern()
        {
            IsotopePattern spExp = new IsotopePattern();
            spExp.Isotopes.Add(new IsotopeContainer(157.07503, 0.0002));
            spExp.SetMonoIsotope(new IsotopeContainer(156.07770, 2));
            spExp.Isotopes.Add(new IsotopeContainer(158.08135, 0.004));
            spExp.Isotopes.Add(new IsotopeContainer(157.08059, 0.0006));
            spExp.Charge = 1;

            IsotopePattern isoNorma = IsotopePatternManipulator.SortByMass(spExp);
            var listISO = isoNorma.Isotopes;
            Assert.AreEqual(156.07770, isoNorma.GetMonoIsotope().Mass, 0.001);
            Assert.AreEqual(156.07770, listISO[0].Mass, 0.00001);
            Assert.AreEqual(157.07503, listISO[1].Mass, 0.00001);
            Assert.AreEqual(157.08059, listISO[2].Mass, 0.00001);
            Assert.AreEqual(158.08135, listISO[3].Mass, 0.00001);

            Assert.AreEqual(2, isoNorma.GetMonoIsotope().Intensity, 0.001);
            Assert.AreEqual(2, listISO[0].Intensity, 0.001);
            Assert.AreEqual(0.0002, listISO[1].Intensity, 0.00001);
            Assert.AreEqual(0.0006, listISO[2].Intensity, 0.00001);
            Assert.AreEqual(0.004, listISO[3].Intensity, 0.00001);

            Assert.AreEqual(1, isoNorma.Charge, 0.001);
        }
    }
}
