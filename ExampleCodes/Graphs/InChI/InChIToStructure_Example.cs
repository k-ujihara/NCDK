using NCDK.NInChI;
using System;

namespace NCDK.Graphs.InChI
{
    class InChIToStructure_Example
    {
        void Main()
        {
            string inchi = "inchi";
            #region 
            // Generate factory -  if native code does not load
            InChIGeneratorFactory factory = new InChIGeneratorFactory();
            // Get InChIToStructure
            InChIToStructure intostruct = factory.GetInChIToStructure(inchi, Default.ChemObjectBuilder.Instance);

            INCHI_RET ret = intostruct.ReturnStatus;
            if (ret == INCHI_RET.WARNING)
            {
                // Structure generated, but with warning message
                Console.WriteLine($"InChI warning: {intostruct.Message}");
            }
            else if (ret != INCHI_RET.OKAY)
            {
                // Structure generation failed
                throw new CDKException($"Structure generation failed: {ret.ToString()} [{intostruct.Message}]");
            }             
            IAtomContainer container = intostruct.AtomContainer;
            #endregion
        }
    }
}
