using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NCDK.FaulonSignatures
{
    [TestClass()]
    public class ParsingTest
    {
        [TestMethod()]
        public void BasicParseTest()
        {
            string sig = "[A]([B])";
            ColoredTree tree = AbstractVertexSignature.Parse(sig);
            Assert.AreEqual(sig, tree.ToString());
        }

        [TestMethod()]
        public void MultipleChildrenParseTest()
        {
            string sig = "[A]([B1][B2][B3])";
            ColoredTree tree = AbstractVertexSignature.Parse(sig);
            Assert.AreEqual(sig, tree.ToString());
        }

        [TestMethod()]
        public void MultipleLevelsParseTest()
        {
            string sig = "[A]([B1]([C])[B2])";
            ColoredTree tree = AbstractVertexSignature.Parse(sig);
            Assert.AreEqual(sig, tree.ToString());
        }

        [TestMethod()]
        public void EdgeLabelParseTest()
        {
            string sig = "[A](=[B])";
            ColoredTree tree = AbstractVertexSignature.Parse(sig);
            Console.Out.WriteLine(tree.ToString());
            Assert.AreEqual(sig, tree.ToString());
        }

        [TestMethod()]
        public void EdgeLabelMultipleChildrenParseTest()
        {
            string sig = "[A](=[B1]=[B2])";
            ColoredTree tree = AbstractVertexSignature.Parse(sig);
            Console.Out.WriteLine(tree.ToString());
            Assert.AreEqual(sig, tree.ToString());
        }

        [TestMethod()]
        public void EdgeLabelMultipleLevelsParseTest()
        {
            string sig = "[A](=[B1]([C])=[B2])";
            ColoredTree tree = AbstractVertexSignature.Parse(sig);
            Console.Out.WriteLine(tree.ToString());
            Assert.AreEqual(sig, tree.ToString());
        }
    }
}
