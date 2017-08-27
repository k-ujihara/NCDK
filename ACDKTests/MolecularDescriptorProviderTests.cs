using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACDK.Tests
{
    [TestClass()]
    public class MolecularDescriptorProviderTests
    {
        static ObjectFactory factory = new ObjectFactory();
        static SmilesParser parser = factory.NewSmilesParser(factory.SilentChemObjectBuilder);

        [TestMethod()]
        public void GetDescriptorTest()
        {
            var provider = new MolecularDescriptorProvider();
            var calculator = provider.GetDescriptor("FragmentComplexityDescriptor");
            var ethane = parser.ParseSmiles("CC");
            var value = calculator.Calculate(ethane);
            var valueAsString = value.ValueAsString();
        }
    }
}
