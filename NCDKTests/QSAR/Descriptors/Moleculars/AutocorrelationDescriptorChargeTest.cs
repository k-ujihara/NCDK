using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.IO;
using NCDK.QSAR.Result;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class AutocorrelationDescriptorChargeTest : MolecularDescriptorTest
    {
        public AutocorrelationDescriptorChargeTest()
                : base()
        {
            SetDescriptor(typeof(AutocorrelationDescriptorCharge));
        }

        [TestMethod()]
        public void Test1()
        {
            string filename = "NCDK.Data.MDL.clorobenzene.mol";
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer container = reader.Read(new AtomContainer());
            DescriptorValue count = Descriptor.Calculate(container);
            Assert.AreEqual(5, count.GetValue().Length);
            Assert.IsTrue(count.GetValue() is DoubleArrayResult);
            DoubleArrayResult result = (DoubleArrayResult)count.GetValue();
            for (int i = 0; i < 5; i++)
            {
                Assert.IsFalse(double.IsNaN(result[i]));
                Assert.IsTrue(0.0 != result[i]);
            }
        }
    }
}
