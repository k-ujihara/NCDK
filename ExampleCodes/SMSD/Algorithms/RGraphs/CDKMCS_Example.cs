using NCDK.Isomorphisms.Matchers;
using NCDK.Silent;
using NCDK.Smiles;

namespace NCDK.SMSD.Algorithms.RGraphs
{
    static class CDKMCS_Example
    {
        static void Main()
        {
            #region
            SmilesParser sp = new SmilesParser();
            IAtomContainer atomContainer = sp.ParseSmiles("CC(=O)OC(=O)C"); // acetic acid anhydride
            IAtomContainer SMILESquery = sp.ParseSmiles("CC"); // acetic acid anhydride
            IQueryAtomContainer query = QueryAtomContainerCreator.CreateBasicQueryContainer(SMILESquery);
            bool isSubstructure = CDKMCS.IsSubgraph(atomContainer, query, true);
            #endregion
        }
    }
}
