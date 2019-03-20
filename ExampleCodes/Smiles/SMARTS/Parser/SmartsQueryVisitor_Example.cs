using NCDK.Isomorphisms.Matchers;
using System.IO;

namespace NCDK.Smiles.SMARTS.Parser
{
    class SmartsQueryVisitor_Example
    {
        static void Main()
        {
            #region 1
            SMARTSParser parser = new SMARTSParser(new StringReader("C*C"));
            ASTStart ast = parser.Start();
            SmartsQueryVisitor visitor = new SmartsQueryVisitor();
            QueryAtomContainer query = (QueryAtomContainer)visitor.Visit(ast, null);
            #endregion
        }
    }
}
