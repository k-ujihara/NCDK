using NCDK.Templates;

namespace NCDK.Graphs
{
    public class AtomContainerAtomPermutor_Example
    { 
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
