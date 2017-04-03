using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Geometries
{
    [TestClass]
    public class RDFCalculator_Example
    {
        [TestMethod]
        [TestCategory("Example")]
        public void Main()
        {
            #region
            RDFCalculator calculator = new RDFCalculator(0.0, 5.0, 0.1, 0.0,
                delegate(IAtom atom, IAtom atom2) { return atom.Charge.Value * atom2.Charge.Value; });
            #endregion
        }
    }
}
