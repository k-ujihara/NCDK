using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NCDK.FaulonSignatures.Simple
{
    [TestClass()]
    public class ReconstructionTest
    {
        // XXX DOES NOT YET WORK - reconstructed graph may be isomorphic, 
        // but not automorphic
        public void Reconstruct(SimpleGraph graph)
        {
            SimpleGraphSignature signature = new SimpleGraphSignature(graph);
            foreach (var symmetryClass in signature.GetSymmetryClasses())
            {
                string signatureString = symmetryClass.GetSignatureString();
                ColoredTree tree = AbstractVertexSignature.Parse(signatureString);
                SimpleGraphBuilder builder = new SimpleGraphBuilder();
                SimpleGraph reconstruction = builder.FromTree(tree);
                Assert.AreEqual(reconstruction.ToString(), graph.ToString());
            }
        }

        //    [TestMethod()]
        //    public void PetersensGraphTest() {
        //        SimpleGraph petersens = SimpleGraphFactory.MakePetersensGraph();
        //        Reconstruct(petersens);
        //    }

        [TestMethod()]
        public void BowtieaneTest()
        {
            SimpleGraph bowtie = SimpleGraphFactory.MakeBowtieane();
            string tmp = new SimpleGraphSignature(bowtie).SignatureStringForVertex(6);
            Console.Out.WriteLine(tmp);
            Console.Out.WriteLine("----------------------------------------");
            string tmp2 = new SimpleGraphSignature(bowtie).SignatureStringForVertex(2);
            Console.Out.WriteLine(tmp2);
        }
    }
}
