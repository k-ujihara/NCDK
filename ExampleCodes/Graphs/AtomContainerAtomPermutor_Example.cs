using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Templates;

namespace NCDK.Graphs
{
    [TestClass]
    public class AtomContainerAtomPermutor_Example
    {
        [TestMethod]
        public void Main()
        {
            var container = TestMoleculeFactory.MakeBenzene();
            #region
            AtomContainerAtomPermutor permutor = new AtomContainerAtomPermutor(container);
            while (permutor.MoveNext())
            {
                IAtomContainer permutedContainer = permutor.Current;
                // ...
            }
            #endregion
        }
    }
}
