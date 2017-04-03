using NCDK.Isomorphisms;
using NCDK.Isomorphisms.Matchers;
using System.IO;

namespace NCDK.Smiles.SMARTS.Parser
{
    class SMARTSParser_Example
    {
        void Main()
        {
            UniversalIsomorphismTester universalIsomorphismTester = null;
            #region 1
            SmilesParser sp = new SmilesParser(Silent.ChemObjectBuilder.Instance);
             IAtomContainer atomContainer = sp.ParseSmiles("CC(=O)OC(=O)C");
             QueryAtomContainer query = SMARTSParser.Parse("C*C", Silent.ChemObjectBuilder.Instance);
             bool queryMatch = universalIsomorphismTester.IsSubgraph(atomContainer, query);
            #endregion
            #region 2
             SMARTSParser parser = new SMARTSParser(new StringReader("C*C"));
             ASTStart start = parser.Start();
            #endregion
        }
    }
}
