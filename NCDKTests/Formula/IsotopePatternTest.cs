using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Formula
{
    /// <summary>
    /// Class testing the IsotopePattern class.
    /// </summary>
    // @cdk.module test-formula
    [TestClass()]
    public class IsotopePatternTest : CDKTestCase
    {
        public IsotopePatternTest()
            : base()
        { }

        [TestMethod()]
        public void TestIsotopePattern()
        {
            var isoP = new IsotopePattern();
            Assert.IsNotNull(isoP);
        }

        [TestMethod()]
        public void TestSetMonoIsotopeIsotopeContainer()
        {
            var isoP = new IsotopePattern { MonoIsotope = new IsotopeContainer() };
            Assert.IsNotNull(isoP);
        }

        [TestMethod()]
        public void TestAddIsotopeIsotopeContainer()
        {
            var isoP = new IsotopePattern(new[] { new IsotopeContainer() });
            Assert.IsNotNull(isoP);
        }

        [TestMethod()]
        public void TestGetMonoIsotope()
        {
            var isoP = new IsotopePattern();
            var isoC = new IsotopeContainer();
            isoP.MonoIsotope = isoC;
            Assert.AreEqual(isoC, isoP.MonoIsotope);
        }

        [TestMethod()]
        public void TestGetIsotopes()
        {
            var iso1 = new IsotopeContainer();
            var iso2 = new IsotopeContainer();
            var isoP = new IsotopePattern(new[] { iso1, iso2 }) { MonoIsotope = iso1 };
            Assert.AreEqual(iso1, isoP.Isotopes[0]);
            Assert.AreEqual(iso2, isoP.Isotopes[1]);
        }

        [TestMethod()]
        public void TestGetNumberOfIsotopes()
        {
            var iso1 = new IsotopeContainer();
            var iso2 = new IsotopeContainer();
            var isoP = new IsotopePattern(new[] { iso1, iso2 }) { MonoIsotope = iso1 };
            Assert.AreEqual(2, isoP.Isotopes.Count);
        }

        [TestMethod()]
        public void TestSetChargeDouble()
        {
            var isoP = new IsotopePattern { Charge = 1.0 };
            Assert.AreEqual(1.0, isoP.Charge, 0.000001);
        }

        [TestMethod()]
        public void TestGetCharge()
        {
            var isoP = new IsotopePattern();
            Assert.AreEqual(0, isoP.Charge, 0.000001);
        }

        [TestMethod()]
        public void TestClone()
        {
            var spExp = new IsotopePattern(new[]
            {
                new IsotopeContainer(156.07770, 1),
                new IsotopeContainer(157.07503, 0.0004),
                new IsotopeContainer(157.08059, 0.0003),
                new IsotopeContainer(158.08135, 0.002),
            });
            spExp.MonoIsotope = spExp.Isotopes[0];
            spExp.Charge = 1;

            var clone = (IsotopePattern)spExp.Clone();
            Assert.AreEqual(156.07770, clone.MonoIsotope.Mass, 0.001);
            Assert.AreEqual(156.07770, clone.Isotopes[0].Mass, 0.001);
            Assert.AreEqual(157.07503, clone.Isotopes[1].Mass, 0.001);
            Assert.AreEqual(157.08059, clone.Isotopes[2].Mass, 0.001);
            Assert.AreEqual(158.08135, clone.Isotopes[3].Mass, 0.001);

            Assert.AreEqual(1, clone.MonoIsotope.Intensity, 0.001);
            Assert.AreEqual(1, clone.Isotopes[0].Intensity, 0.001);
            Assert.AreEqual(0.0004, clone.Isotopes[1].Intensity, 0.001);
            Assert.AreEqual(0.0003, clone.Isotopes[2].Intensity, 0.001);
            Assert.AreEqual(0.002, clone.Isotopes[3].Intensity, 0.001);

            Assert.AreEqual(1, clone.Charge, 0.001);
        }
    }
}
