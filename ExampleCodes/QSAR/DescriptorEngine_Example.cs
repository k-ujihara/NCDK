using NCDK.Silent;
using NCDK.Templates;
using System.Linq;

namespace NCDK.QSAR
{
    class DescriptorEngine_Example
    {
        static void Main()
        {
            {
                #region
                IAtomContainer someMolecule = TestMoleculeFactory.MakeAlphaPinene();
                // ...
                DescriptorEngine descriptorEngine = DescriptorEngine.Create<IMolecularDescriptor>(null);
                descriptorEngine.Process(someMolecule);
                #endregion
            }
            {
                IAtomContainer someAtomContainer = null;
                #region ctor
                var classNames = DescriptorEngine.GetDescriptorClassNameByPackage("NCDK.QSRA.Descriptors.Moleculars", null);
                DescriptorEngine engine = new DescriptorEngine(classNames, ChemObjectBuilder.Instance);
                var instances = DescriptorEngine.InstantiateDescriptors(classNames);
                var specs = instances.Select(n => n.Specification).ToList();
                engine.SetDescriptorInstances(instances);
                engine.SetDescriptorSpecifications(specs);

                engine.Process(someAtomContainer);
                #endregion
            }
        }
    }
}
