using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Templates;

namespace NCDK.Graphs
{
    [TestClass]
    public class AtomContainerBondPermutor_Example
    {
        [TestMethod]
        public void Main()
        {
            var container = TestMoleculeFactory.MakeBenzene();
            #region
            AtomContainerBondPermutor permutor = new AtomContainerBondPermutor(container);
            while (permutor.MoveNext())
            {
                IAtomContainer permutedContainer = permutor.Current;
                // ...
            }
            #endregion
        }
    }
}
