using NCDK.Isomorphisms.Matchers;
using NCDK.Smiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.Isomorphisms
{
    class UniversalIsomorphismTester_Example
    {
        void Main()
        {
            UniversalIsomorphismTester universalIsomorphismTester = new UniversalIsomorphismTester();
            #region
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer atomContainer = sp.ParseSmiles("CC(=O)OC(=O)C"); // acetic acid anhydride
            IAtomContainer SMILESquery = sp.ParseSmiles("CC"); // ethylene
            IQueryAtomContainer query = QueryAtomContainerCreator.CreateBasicQueryContainer(SMILESquery);
            bool isSubstructure = universalIsomorphismTester.IsSubgraph(atomContainer, query);
            #endregion
        }
    }
}
