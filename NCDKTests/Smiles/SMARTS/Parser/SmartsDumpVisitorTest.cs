using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace NCDK.Smiles.SMARTS.Parser
{
    /// <summary>
    /// Junit testing routine for SmartsDumpVisitor
    /// </summary>
    // @author Dazhi Jiao
    // @cdk.created 2007-05-10
    // @cdk.module test-smarts
    // @cdk.keyword SMARTS
    [TestClass()]
    public class SmartsDumpVisitorTest : CDKTestCase
    {
        public void Dump(string smarts)
        {
            SMARTSParser parser = new SMARTSParser(new StringReader(smarts));
            ASTStart start = parser.Start();
            SmartsDumpVisitor visitor = new SmartsDumpVisitor();
            visitor.Visit(start, null);
        }

        [TestMethod()]
        public void TestRing()
        {
            Dump("(C=1CCC1).(CCC).(C1CC1CCC=12CCCC2)");
        }
    }
}
