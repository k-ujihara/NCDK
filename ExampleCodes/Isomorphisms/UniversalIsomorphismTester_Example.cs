using NCDK.Isomorphisms.Matchers;
using NCDK.Silent;
using NCDK.Smiles;

namespace NCDK.Isomorphisms
{
    static class UniversalIsomorphismTester_Example
    {
        static void Main()
        {
            UniversalIsomorphismTester universalIsomorphismTester = new UniversalIsomorphismTester();
            #region
            SmilesParser sp = new SmilesParser();
            IAtomContainer atomContainer = sp.ParseSmiles("CC(=O)OC(=O)C"); // acetic acid anhydride
            IAtomContainer SMILESquery = sp.ParseSmiles("CC"); // ethylene
            IQueryAtomContainer query = QueryAtomContainerCreator.CreateBasicQueryContainer(SMILESquery);
            bool isSubstructure = universalIsomorphismTester.IsSubgraph(atomContainer, query);
            #endregion
        }
    }
}
