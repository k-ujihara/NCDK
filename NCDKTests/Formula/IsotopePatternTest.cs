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
            IsotopePattern isoP = new IsotopePattern();
            Assert.IsNotNull(isoP);
        }

        [TestMethod()]
        public void TestSetMonoIsotope_IsotopeContainer()
        {
            IsotopePattern isoP = new IsotopePattern();
            isoP.SetMonoIsotope(new IsotopeContainer());
            Assert.IsNotNull(isoP);
        }

        [TestMethod()]
        public void TestAddIsotope_IsotopeContainer()
        {
            IsotopePattern isoP = new IsotopePattern();
            isoP.Isotopes.Add(new IsotopeContainer());
            Assert.IsNotNull(isoP);
        }

        [TestMethod()]
        public void TestGetMonoIsotope()
        {
            IsotopePattern isoP = new IsotopePattern();
            IsotopeContainer isoC = new IsotopeContainer();
            isoP.SetMonoIsotope(isoC);
            Assert.AreEqual(isoC, isoP.GetMonoIsotope());
        }

        [TestMethod()]
        public void TestGetIsotopes()
        {
            IsotopePattern isoP = new IsotopePattern();
            IsotopeContainer iso1 = new IsotopeContainer();
            isoP.SetMonoIsotope(iso1);
            IsotopeContainer iso2 = new IsotopeContainer();
            isoP.Isotopes.Add(iso2);
            Assert.AreEqual(iso1, isoP.Isotopes[0]);
            Assert.AreEqual(iso2, isoP.Isotopes[1]);
        }

        [TestMethod()]
        public void TestGetIsotope_int()
        {
            IsotopePattern isoP = new IsotopePattern();
            IsotopeContainer iso1 = new IsotopeContainer();
            isoP.SetMonoIsotope(iso1);
            IsotopeContainer iso2 = new IsotopeContainer();
            isoP.Isotopes.Add(iso2);
            Assert.AreEqual(iso1, isoP.Isotopes[0]);
            Assert.AreEqual(iso2, isoP.Isotopes[1]);
        }

        [TestMethod()]
        public void TestGetNumberOfIsotopes()
        {
            IsotopePattern isoP = new IsotopePattern();
            IsotopeContainer iso1 = new IsotopeContainer();
            isoP.SetMonoIsotope(iso1);
            IsotopeContainer iso2 = new IsotopeContainer();
            isoP.Isotopes.Add(iso2);
            Assert.AreEqual(2, isoP.Isotopes.Count);
        }

        [TestMethod()]
        public void TestSetCharge_Double()
        {
            IsotopePattern isoP = new IsotopePattern { Charge = 1.0 };
            Assert.AreEqual(1.0, isoP.Charge, 0.000001);
        }

        [TestMethod()]
        public void TestGetCharge()
        {
            IsotopePattern isoP = new IsotopePattern();
            Assert.AreEqual(0, isoP.Charge, 0.000001);
        }

        [TestMethod()]
        public void TestClone()
        {
            IsotopePattern spExp = new IsotopePattern();
            spExp.SetMonoIsotope(new IsotopeContainer(156.07770, 1));
            spExp.Isotopes.Add(new IsotopeContainer(157.07503, 0.0004));
            spExp.Isotopes.Add(new IsotopeContainer(157.08059, 0.0003));
            spExp.Isotopes.Add(new IsotopeContainer(158.08135, 0.002));
            spExp.Charge = 1;

            IsotopePattern clone = (IsotopePattern)spExp.Clone();
            Assert.AreEqual(156.07770, clone.GetMonoIsotope().Mass, 0.001);
            Assert.AreEqual(156.07770, clone.Isotopes[0].Mass, 0.001);
            Assert.AreEqual(157.07503, clone.Isotopes[1].Mass, 0.001);
            Assert.AreEqual(157.08059, clone.Isotopes[2].Mass, 0.001);
            Assert.AreEqual(158.08135, clone.Isotopes[3].Mass, 0.001);

            Assert.AreEqual(1, clone.GetMonoIsotope().Intensity, 0.001);
            Assert.AreEqual(1, clone.Isotopes[0].Intensity, 0.001);
            Assert.AreEqual(0.0004, clone.Isotopes[1].Intensity, 0.001);
            Assert.AreEqual(0.0003, clone.Isotopes[2].Intensity, 0.001);
            Assert.AreEqual(0.002, clone.Isotopes[3].Intensity, 0.001);

            Assert.AreEqual(1, clone.Charge, 0.001);
        }
    }
}
