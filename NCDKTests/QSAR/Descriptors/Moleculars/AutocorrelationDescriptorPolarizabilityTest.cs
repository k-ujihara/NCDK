using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.IO;

using System;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class AutocorrelationDescriptorPolarizabilityTest : MolecularDescriptorTest
    {
        public AutocorrelationDescriptorPolarizabilityTest()
                : base()
        {
            SetDescriptor(typeof(AutocorrelationDescriptorPolarizability));
        }

        public void IgnoreCalculate_IAtomContainer()
        {
            string filename = "NCDK.Data.MDL.chlorobenzene.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer container = reader.Read(new AtomContainer());
            var count = Descriptor.Calculate(container);
            Console.Out.WriteLine(count.Value);

            Assert.Fail("Not validated yet");
        }
    }
}
