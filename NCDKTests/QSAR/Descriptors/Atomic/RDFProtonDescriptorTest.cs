using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.IO;
using System.Linq;

namespace NCDK.QSAR.Descriptors.Atomic
{
    // @cdk.module test-qsaratomic
    public abstract class RDFProtonDescriptorTest<T> : AtomicDescriptorTest<T> where T: IAtomicDescriptor
    {
        [TestMethod()]
        public void TestExample1()
        {
            //firstly read file to molecule
            var filename = "NCDK.Data.MDL.hydroxyamino.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new MDLV2000Reader(ins, ChemObjectReaderMode.Strict);
            var chemFile = reader.Read(CDK.Builder.NewChemFile());
            var seq = chemFile[0];
            var model = seq[0];
            var som = model.MoleculeSet;
            var mol = som[0];

            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                if (mol.Atoms[i].Symbol.Equals("H"))
                {
                    //secondly perform calculation on it.
                    var result = CreateDescriptor(mol).Calculate(mol.Atoms[i]);
                    Assert.IsNotNull(result);
                }
            }
        }

        [TestMethod()]
        public void TestReturnsNaNForNonHydrogen()
        {
            var mol = CDK.Builder.NewAtomContainer();
            var atom = CDK.Builder.NewAtom("O");
            mol.Atoms.Add(atom);
            var result = CreateDescriptor(mol).Calculate(atom);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Exception);
        }
    }

    [TestClass()]
    public class RDFProtonDescriptorG3RTest : RDFProtonDescriptorTest<RDFProtonDescriptorG3R>
    {
    }

    [TestClass()]
    public class RDFProtonDescriptorGDRTest : RDFProtonDescriptorTest<RDFProtonDescriptorGDR>
    {
    }

    [TestClass()]
    public class RDFProtonDescriptorGHRTopologyTest : RDFProtonDescriptorTest<RDFProtonDescriptorGHRTopology>
    {
    }

    [TestClass()]
    public class RDFProtonDescriptorGHRTest : RDFProtonDescriptorTest<RDFProtonDescriptorGHR>
    {
    }

    [TestClass()]
    public class RDFProtonDescriptorGSRTest : RDFProtonDescriptorTest<RDFProtonDescriptorGSR>
    {
    }
}
