using NCDK.Isomorphisms.Matchers;
using NCDK.Smiles;

namespace NCDK.SMSD.Algorithms.RGraph
{
    class CDKMCS_Example
    {
        void Main()
        {
            #region
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer atomContainer = sp.ParseSmiles("CC(=O)OC(=O)C"); // acetic acid anhydride
            IAtomContainer SMILESquery = sp.ParseSmiles("CC"); // acetic acid anhydride
            IQueryAtomContainer query = QueryAtomContainerCreator.CreateBasicQueryContainer(SMILESquery);
            bool isSubstructure = CDKMCS.IsSubgraph(atomContainer, query, true);
            #endregion
        }
    }
}
