using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Formula
{
    /**
     * Class testing the IsotopeContainer class.
     *
     * @cdk.module test-formula
     */
    [TestClass()]
    public class IsotopeContainerTest : CDKTestCase
    {

        private static IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        /**
         *  Constructor for the IsotopeContainerTest object.
         *
         */
        public IsotopeContainerTest()
                : base()
        { }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestIsotopeContainer()
        {
            IsotopeContainer isoC = new IsotopeContainer();
            Assert.IsNotNull(isoC);
        }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestIsotopeContainer_IMolecularFormula_Double()
        {
            IMolecularFormula formula = builder.CreateMolecularFormula();
            double intensity = 130.00;
            IsotopeContainer isoC = new IsotopeContainer(formula, intensity);

            Assert.IsNotNull(isoC);
            Assert.AreEqual(formula, isoC.Formula);
            Assert.AreEqual(intensity, isoC.Intensity, 0.001);
        }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestIsotopeContainer_Double_double()
        {
            double mass = 130.00;
            double intensity = 500000.00;
            IsotopeContainer isoC = new IsotopeContainer(mass, intensity);

            Assert.IsNotNull(isoC);
            Assert.AreEqual(mass, isoC.Mass, 0.001);
            Assert.AreEqual(intensity, isoC.Intensity, 0.001);
        }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestSetFormula_IMolecularFormula()
        {
            IsotopeContainer isoC = new IsotopeContainer();
            IMolecularFormula formula = builder.CreateMolecularFormula();
            isoC.Formula = formula;
            Assert.IsNotNull(isoC);
        }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestSetMass_Double()
        {
            IsotopeContainer isoC = new IsotopeContainer();
            isoC.Mass = 130.00;
            Assert.IsNotNull(isoC);
        }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestSetIntensity_Double()
        {
            IsotopeContainer isoC = new IsotopeContainer();
            isoC.Intensity = 5000000.0;
            Assert.IsNotNull(isoC);
        }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestGetFormula()
        {
            IsotopeContainer isoC = new IsotopeContainer();
            IMolecularFormula formula = builder.CreateMolecularFormula();
            isoC.Formula = formula;
            Assert.AreEqual(formula, isoC.Formula);
        }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestGetMass()
        {
            IsotopeContainer isoC = new IsotopeContainer();
            double mass = 130.00;
            isoC.Mass = mass;
            Assert.AreEqual(mass, isoC.Mass, 0.001);

        }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestGetIntensity()
        {
            IsotopeContainer isoC = new IsotopeContainer();
            double intensity = 130.00;
            isoC.Intensity = intensity;
            Assert.AreEqual(intensity, isoC.Intensity, 0.001);
        }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestClone()
        {
            IsotopeContainer isoC = new IsotopeContainer();
            IMolecularFormula formula = builder.CreateMolecularFormula();
            isoC.Formula = formula;
            double mass = 130.00;
            isoC.Mass = mass;
            double intensity = 130.00;
            isoC.Intensity = intensity;

            IsotopeContainer clone = (IsotopeContainer)isoC.Clone();
            Assert.AreEqual(mass, clone.Mass, 0.001);
            Assert.AreEqual(intensity, clone.Intensity, 0.001);
            Assert.AreEqual(formula, clone.Formula);
        }
    }
}
