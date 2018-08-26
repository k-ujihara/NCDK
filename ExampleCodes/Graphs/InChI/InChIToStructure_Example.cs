using NCDK.Silent;
using System;

namespace NCDK.Graphs.InChI
{
    class InChIToStructure_Example
    {
        static void Main()
        {
            string inchi = "inchi";
            #region 
            // Get InChIToStructure
            InChIToStructure intostruct = InChIToStructure.FromInChI(inchi, ChemObjectBuilder.Instance);

            InChIReturnCode ret = intostruct.ReturnStatus;
            if (ret == InChIReturnCode.Warning)
            {
                // Structure generated, but with warning message
                Console.WriteLine($"InChI warning: {intostruct.Message}");
            }
            else if (ret != InChIReturnCode.Ok)
            {
                // Structure generation failed
                throw new CDKException($"Structure generation failed: {ret.ToString()} [{intostruct.Message}]");
            }             
            IAtomContainer container = intostruct.AtomContainer;
            #endregion
        }
    }
}
