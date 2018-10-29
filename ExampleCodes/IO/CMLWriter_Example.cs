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
                using (var cmlwriter = new CMLWriter(new FileStream("molecule.cml", FileMode.Create)))
                {
                    cmlwriter.Write(molecule);
                }
                #endregion
            }
        }
    }
}
