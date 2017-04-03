using System.IO;

namespace NCDK.IO.Iterator
{
    class IteratingSDFReader_Example
    {
        void Main()
        {
            {
                #region 
                using (var srm = new FileStream("../zinc-structures/ZINC_subset3_3D_charged_wH_maxmin1000.sdf", FileMode.Open))
                {
                    IteratingSDFReader reader = new IteratingSDFReader(srm, Default.ChemObjectBuilder.Instance);
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
