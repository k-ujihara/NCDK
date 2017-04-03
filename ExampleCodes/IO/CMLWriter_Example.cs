using System.IO;

namespace NCDK.IO
{
    class CMLWriter_Example
    {
        void Main()
        {
            {
                IAtomContainer molecule = null;
                #region 
                using (var output = new FileStream("molecule.cml", FileMode.Create))
                using (CMLWriter cmlwriter = new CMLWriter(output))
                {
                    cmlwriter.Write(molecule);
                }
                #endregion
            }
        }
    }
}
