using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Formula
{
    /// <summary>
    /// Class testing the IsotopePatternManipulator class.
    /// </summary>
    // @cdk.module test-formula
    [TestClass()]
    public class IsotopePatternManipulatorTest : CDKTestCase
    {
        public IsotopePatternManipulatorTest()
            : base()
        { }

        [TestMethod()]
        public void TestNormalizeIsotopePattern()
        {
            var spExp = new IsotopePattern(new[]
                {
                    new IsotopeContainer(156.07770, 2),
                    new IsotopeContainer(157.08059, 0.0006),
                    new IsotopeContainer(157.07503, 0.0002),
                    new IsotopeContainer(158.08135, 0.004),
                });
            spExp.MonoIsotope = spExp.Isotopes[0];
            spExp.Charge = 1;

            var isoNorma = IsotopePatternManipulator.Normalize(spExp);
            var listISO = isoNorma.Isotopes;
            Assert.AreEqual(1, isoNorma.MonoIsotope.Intensity, 0.00001);
            Assert.AreEqual(1, listISO[0].Intensity, 0.00001);
            Assert.AreEqual(0.0003, listISO[1].Intensity, 0.00001);
            Assert.AreEqual(0.0001, listISO[2].Intensity, 0.00001);
            Assert.AreEqual(0.002, listISO[3].Intensity, 0.00001);

            Assert.AreEqual(156.07770, isoNorma.MonoIsotope.Mass, 0.00001);
            Assert.AreEqual(156.07770, listISO[0].Mass, 0.00001);
            Assert.AreEqual(157.08059, listISO[1].Mass, 0.00001);
            Assert.AreEqual(157.07503, listISO[2].Mass, 0.00001);
            Assert.AreEqual(158.08135, listISO[3].Mass, 0.00001);

            Assert.AreEqual(1, isoNorma.Charge, 0.00001);
        }

        [TestMethod()]
        public void TestSortByIntensityIsotopePattern()
        {
            var spExp = new IsotopePattern(new[]
                {
                    new IsotopeContainer(157.07503, 0.0001),
                    new IsotopeContainer(156.07770, 1),
                    new IsotopeContainer(157.08059, 0.0003),
                    new IsotopeContainer(158.08135, 0.002),
                });
            spExp.MonoIsotope = spExp.Isotopes[0];
            spExp.Charge = 1;

            var isoNorma = IsotopePatternManipulator.SortByIntensity(spExp);
            var listISO = isoNorma.Isotopes;
            Assert.AreEqual(156.07770, isoNorma.MonoIsotope.Mass, 0.00001);
            Assert.AreEqual(156.07770, listISO[0].Mass, 0.00001);
            Assert.AreEqual(158.08135, listISO[1].Mass, 0.00001);
            Assert.AreEqual(157.08059, listISO[2].Mass, 0.00001);
            Assert.AreEqual(157.07503, listISO[3].Mass, 0.00001);

            Assert.AreEqual(1, isoNorma.MonoIsotope.Intensity, 0.00001);
            Assert.AreEqual(1, listISO[0].Intensity, 0.00001);
            Assert.AreEqual(0.002, listISO[1].Intensity, 0.00001);
            Assert.AreEqual(0.0003, listISO[2].Intensity, 0.001);
            Assert.AreEqual(0.0001, listISO[3].Intensity, 0.00001);

            Assert.AreEqual(1, isoNorma.Charge, 0.00001);
        }

        [TestMethod()]
        public void TestSortAndNormalizedByIntensityIsotopePattern()
        {
            var spExp = new IsotopePattern(new[]
                {
                    new IsotopeContainer(157.07503, 0.0002),
                    new IsotopeContainer(156.07770, 2),
                    new IsotopeContainer(158.08135, 0.004),
                    new IsotopeContainer(157.08059, 0.0006),
                });
            spExp.MonoIsotope = spExp.Isotopes[1];
            spExp.Charge = 1;

            var isoNorma = IsotopePatternManipulator.SortAndNormalizedByIntensity(spExp);
            var listISO = isoNorma.Isotopes;
            Assert.AreEqual(156.07770, isoNorma.MonoIsotope.Mass, 0.00001);
            Assert.AreEqual(156.07770, listISO[0].Mass, 0.00001);
            Assert.AreEqual(158.08135, listISO[1].Mass, 0.00001);
            Assert.AreEqual(157.08059, listISO[2].Mass, 0.00001);
            Assert.AreEqual(157.07503, listISO[3].Mass, 0.00001);

            Assert.AreEqual(1, isoNorma.MonoIsotope.Intensity, 0.00001);
            Assert.AreEqual(1, listISO[0].Intensity, 0.00001);
            Assert.AreEqual(0.002, listISO[1].Intensity, 0.00001);
            Assert.AreEqual(0.0003, listISO[2].Intensity, 0.00001);
            Assert.AreEqual(0.0001, listISO[3].Intensity, 0.00001);

            Assert.AreEqual(1, isoNorma.Charge, 0.001);
        }

        [TestMethod()]
        public void TestSortByMassIsotopePattern()
        {
            var spExp = new IsotopePattern(new[]
                {
                    new IsotopeContainer(157.07503, 0.0002),
                    new IsotopeContainer(156.07770, 2),
                    new IsotopeContainer(158.08135, 0.004),
                    new IsotopeContainer(157.08059, 0.0006),
                });
            spExp.MonoIsotope = spExp.Isotopes[1];
            spExp.Charge = 1;

            var isoNorma = IsotopePatternManipulator.SortByMass(spExp);
            var listISO = isoNorma.Isotopes;
            Assert.AreEqual(156.07770, isoNorma.MonoIsotope.Mass, 0.001);
            Assert.AreEqual(156.07770, listISO[0].Mass, 0.00001);
            Assert.AreEqual(157.07503, listISO[1].Mass, 0.00001);
            Assert.AreEqual(157.08059, listISO[2].Mass, 0.00001);
            Assert.AreEqual(158.08135, listISO[3].Mass, 0.00001);

            Assert.AreEqual(2, isoNorma.MonoIsotope.Intensity, 0.001);
            Assert.AreEqual(2, listISO[0].Intensity, 0.001);
            Assert.AreEqual(0.0002, listISO[1].Intensity, 0.00001);
            Assert.AreEqual(0.0006, listISO[2].Intensity, 0.00001);
            Assert.AreEqual(0.004, listISO[3].Intensity, 0.00001);

            Assert.AreEqual(1, isoNorma.Charge, 0.001);
        }
    }
}
