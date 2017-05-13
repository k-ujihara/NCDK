using NCDK.Templates;

namespace NCDK.Graphs
{
    public class AtomContainerBondPermutor_Example
    {
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
