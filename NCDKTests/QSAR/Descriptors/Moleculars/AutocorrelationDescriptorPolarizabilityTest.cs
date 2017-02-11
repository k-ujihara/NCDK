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
            string filename = "NCDK.Data.MDL.clorobenzene.mol";
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer container = reader.Read(new AtomContainer());
            DescriptorValue count = Descriptor.Calculate(container);
            Console.Out.WriteLine(count.GetValue());

            Assert.Fail("Not validated yet");
        }
    }
}
