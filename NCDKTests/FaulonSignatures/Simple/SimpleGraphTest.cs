using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NCDK.FaulonSignatures.Simple
{
    [TestClass()]
    public class SimpleGraphTest
    {
        public SimpleGraphSignature SignatureFromString(string str)
        {
            SimpleGraph graph = new SimpleGraph(str);
            return new SimpleGraphSignature(graph);
        }

        [TestMethod()]
        public void TestChain()
        {
            string chain = "0:1,1:2,2:3,3:4";
            SimpleGraphSignature signature = SignatureFromString(chain);

            string uncanonizedString = signature.ToCanonicalString();
            // TODO : FIXME - maximal / minimal problem
            //        string maxSignature = signature.GetGraphSignature();
            string maxSignature = signature.GetMaximalSignature();

            Assert.AreEqual(uncanonizedString, maxSignature);
        }

        [TestMethod()]
        public void TestColoredTreeRoundtrip()
        {
            string signatureString = "[.]([.]([.,0])[.]([.,0]))";
            ColoredTree tree = SimpleVertexSignature.Parse(signatureString);
            Assert.AreEqual(signatureString, tree.ToString());

            SimpleGraphBuilder builder = new SimpleGraphBuilder();
            SimpleGraph graph = builder.FromTree(tree);
            SimpleGraphSignature graphSignature = new SimpleGraphSignature(graph);
            string canonicalString = graphSignature.ToCanonicalString();
            Assert.AreEqual(signatureString, canonicalString);
        }

        [TestMethod()]
        public void TestVertexCount()
        {

        }

        [TestMethod()]
        public void SignatureHeightTest()
        {
            SimpleGraph g = SimpleGraphFactory.MakeCuneane();
            SimpleGraphSignature signature = new SimpleGraphSignature(g);
            for (int h = 1; h < g.GetVertexCount(); h++)
            {
                for (int i = 0; i < g.GetVertexCount(); i++)
                {
                    string sig = signature.SignatureStringForVertex(i, h);
                    Console.Out.WriteLine(h + "\t" + i + "\t" + sig);
                }
            }
        }
    }
}
