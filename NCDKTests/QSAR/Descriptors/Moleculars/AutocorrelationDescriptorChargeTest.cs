using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.IO;
using NCDK.QSAR.Results;


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
            string filename = "NCDK.Data.MDL.chlorobenzene.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer container = reader.Read(new AtomContainer());
            DescriptorValue count = Descriptor.Calculate(container);
            Assert.AreEqual(5, count.Value.Length);
            Assert.IsTrue(count.Value is DoubleArrayResult);
            DoubleArrayResult result = (DoubleArrayResult)count.Value;
            for (int i = 0; i < 5; i++)
            {
                Assert.IsFalse(double.IsNaN(result[i]));
                Assert.IsTrue(0.0 != result[i]);
            }
        }
    }
}
