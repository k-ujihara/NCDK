using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACDK
{
    [TestClass()]
    public class ObjectFactoryTests
    {
        [TestMethod()]
        public void ObjectFactoryTest()
        {
            IObjectFactory o = new ObjectFactory();
            IChemObjectBuilder builder;
            builder = o.DefaultChemObjectBuilder;
            Assert.IsNotNull(((W_IChemObjectBuilder)builder).Object);
            builder = o.SilentChemObjectBuilder;
            Assert.IsNotNull(((W_IChemObjectBuilder)builder).Object);
        }
    }
}