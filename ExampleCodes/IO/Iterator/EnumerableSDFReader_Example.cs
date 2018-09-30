using NCDK.Silent;
using System.IO;

namespace NCDK.IO.Iterator
{
    static class EnumerableSDFReader_Example
    {
        static void Main()
        {
            {
                #region 
                using (var srm = new FileStream("../zinc-structures/ZINC_subset3_3D_charged_wH_maxmin1000.sdf", FileMode.Open))
                {
                    EnumerableSDFReader reader = new EnumerableSDFReader(srm, ChemObjectBuilder.Instance);
                    foreach (var molecule in reader)
                    {
                        // do something
                    }
                }
                #endregion
            }
        }
    }
}
