using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace NCDK.Smiles.SMARTS.Parser
{
    /**
     * Junit testing routine for SmartsDumpVisitor
     *
     * @author Dazhi Jiao
     * @cdk.created 2007-05-10
     * @cdk.module test-smarts
     * @cdk.keyword SMARTS
     */
	 [TestClass()]
    public class SmartsDumpVisitorTest : CDKTestCase
    {
        public void dump(string smarts)
        {
            SMARTSParser parser = new SMARTSParser(new StringReader(smarts));
            ASTStart start = parser.Start();
            SmartsDumpVisitor visitor = new SmartsDumpVisitor();
            visitor.Visit(start, null);
        }

        public void TestRing()
        {
            dump("(C=1CCC1).(CCC).(C1CC1CCC=12CCCC2)");
        }
    }
}
