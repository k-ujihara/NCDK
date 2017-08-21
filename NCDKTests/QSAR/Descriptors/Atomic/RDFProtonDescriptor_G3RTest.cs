using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.IO;
using NCDK.QSAR.Results;

namespace NCDK.QSAR.Descriptors.Atomic
{
    // @cdk.module test-qsaratomic
    [TestClass()]
    public class RDFProtonDescriptor_G3RTest : AtomicDescriptorTest
    {
        public RDFProtonDescriptor_G3RTest()
        {
            SetDescriptor(typeof(RDFProtonDescriptor_G3R));
        }

        [TestMethod()]
        public void TestExample1()
        {
            //firstly read file to molecule
            string filename = "NCDK.Data.MDL.hydroxyamino.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            ChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            IChemSequence seq = chemFile[0];
            IChemModel model = seq[0];
            var som = model.MoleculeSet;
            IAtomContainer mol = som[0];

            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                //            Console.Out.WriteLine("Atom: " + mol.Atoms[i].Symbol);
                if (mol.Atoms[i].Symbol.Equals("H"))
                {
                    //secondly perform calculation on it.
                    RDFProtonDescriptor_G3R descriptor = new RDFProtonDescriptor_G3R();
                    DescriptorValue dv = descriptor.Calculate(mol.Atoms[i], mol);
                    IDescriptorResult result = dv.Value;
                    //                Console.Out.WriteLine("array: " + result.ToString());
                    Assert.IsNotNull(result);
                }

            }
        }

        [TestMethod()]
        public void TestReturnsNaNForNonHydrogen()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("O");
            mol.Atoms.Add(atom);
            DescriptorValue dv = descriptor.Calculate(atom, mol);
            IDescriptorResult result = dv.Value;
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ArrayResult<double>);
            ArrayResult<double> dResult = (ArrayResult<double>)result;
            for (int i = 0; i < result.Length; i++)
            {
                Assert.AreEqual(double.NaN, dResult[i]);
            }
        }
    }
}
