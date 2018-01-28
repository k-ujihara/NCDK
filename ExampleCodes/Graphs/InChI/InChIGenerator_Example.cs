using NCDK.NInChI;
using System;

namespace NCDK.Graphs.InChI
{
    class InChIGenerator_Example
    {
        void Main()
        {
            IAtomContainer container = null;

            #region 
            // Generate factory -  if native code does not load
            InChIGeneratorFactory factory = new InChIGeneratorFactory();
            // Get InChIGenerator
            InChIGenerator gen = factory.GetInChIGenerator(container);

            InChIReturnCode ret = gen.ReturnStatus;
            if (ret == InChIReturnCode.Warning)
            {
                // InChI generated, but with warning message
                Console.WriteLine($"InChI warning: {gen.Message}");
            }
            else if (ret != InChIReturnCode.Ok)
            {
                // InChI generation failed
                throw new CDKException($"InChI failed: {ret.ToString()} [{gen.Message}]");
            }

            string inchi = gen.InChI;
            string auxinfo = gen.AuxInfo;
            #endregion
        }
    }
}
