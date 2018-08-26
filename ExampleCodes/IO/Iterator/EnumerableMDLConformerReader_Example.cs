using NCDK.Silent;
using System.IO;

namespace NCDK.IO.Iterator
{
    static class EnumerableMDLConformerReader_Example
    {
        static void Main()
        {
            {
                #region 
                string filename = "/Users/rguha/conf2.sdf";
                using (var srm = new FileStream(filename, FileMode.Open))
                {
                    IEnumerableMDLConformerReader reader = new IEnumerableMDLConformerReader(srm, ChemObjectBuilder.Instance);
                    foreach (var cc in reader)
                    {
                        // do something 
                    }
                }
                // do something with this set of conformers
                #endregion
            }
        }
    }
}
