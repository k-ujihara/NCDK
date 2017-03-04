using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Beam
{
    // @author John May 
    [TestClass()]
    public class NormaliseDirectionalLabelsTest
    {
        [TestMethod()]
        public void Simple()
        {
            Transform("F\\C=C\\F",
                      "F/C=C/F");
        }

        [TestMethod()]
        public void Ordering2()
        {
            Transform("C(\\F)(/C)=C\\F",
                      "C(/F)(\\C)=C/F");
        }

        [TestMethod()]
        public void Simple2()
        {
            Transform("C(\\F)=C\\F",
                      "C(/F)=C/F");
        }

        [TestMethod()]
        public void Partial()
        {
            Transform("FC=C(\\F)/C=C/F",
                      "FC=C(\\F)/C=C/F");
        }

        [TestMethod()]
        public void Partial2()
        {
            Transform("FC=C(F)C=C(F)\\C=C\\F",
                      "FC=C(F)C=C(F)/C=C/F");
        }

        [TestMethod()]
        public void Conjugated()
        {
            Transform("F\\C=C(\\F)/C(/F)=C\\F",
                      "F/C=C(/F)\\C(\\F)=C/F");
        }

        [TestMethod()]
        public void CyclicTest()
        {
            Transform("C/C=C\\1/C\\C(=C\\C)\\C1",
                      "C/C=C\\1/C/C(=C/C)/C1");
            Transform("C/C=C\\1/C/C(=C/C)/C1",
                      "C/C=C\\1/C/C(=C/C)/C1");
        }

        // invalid structure
        public void Chebi15617()
        {
            Transform("C/C=C\\1/[C@@H](C)C(=O)N/C1=C\\C/2=N/C(=C\\C3=C(CCC(=O)O)C(=C(\\C=C/4\\C(=C(CC)C(=O)N4)C)N3)C)/C(=C2C)CCC(=O)O",
                      "C/C=C\\1/[C@@H](C)C(=O)N/C1=C\\C/2=N/C(=C\\C3=C(CCC(=O)O)C(=C(/C=C\\4/C(=C(CC)C(=O)N4)C)N3)C)/C(=C2C)CCC(=O)O");
            Transform("C/C=C\\1/[C@@H](C)C(=O)N/C1=C\\C/2=N/C(=C\\C3=C(CCC(=O)O)C(=C(/C=C\\4/C(=C(CC)C(=O)N4)C)N3)C)/C(=C2C)CCC(=O)O",
                      "C/C=C\\1/[C@@H](C)C(=O)N/C1=C\\C/2=N/C(=C\\C3=C(CCC(=O)O)C(=C(/C=C\\4/C(=C(CC)C(=O)N4)C)N3)C)/C(=C2C)CCC(=O)O");
        }


        static void Transform(string smi, string exp)
        {
            Assert.AreEqual(exp, Generator.Generate(new NormaliseDirectionalLabels()
                                                         .Apply(Parser.Parse(smi))));
        }
    }
}
