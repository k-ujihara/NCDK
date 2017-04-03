using NCDK.Templates;

namespace NCDK.QSAR
{
    class DescriptorEngine_Example
    {
        void Main()
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
                DescriptorEngine engine = new DescriptorEngine(classNames, Default.ChemObjectBuilder.Instance);
                var instances = engine.InstantiateDescriptors(classNames);
                var specs = engine.InitializeSpecifications(instances);
                engine.SetDescriptorInstances(instances);
                engine.SetDescriptorSpecifications(specs);

                engine.Process(someAtomContainer);
                #endregion
            }
        }
    }
}
