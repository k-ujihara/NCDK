using NCDK.IO.Iterator;
using NCDK.Silent;
using System.IO;

namespace NCDK
{
    public static class ConformerContainer_Example
    {
        public static void Ctor()
        {
            string filename = null;
            #region
            var reader = new IEnumerableMDLConformerReader(
                new FileStream(filename, FileMode.Open),
                ChemObjectBuilder.Instance);
            foreach (ConformerContainer cc in reader)
            {
                foreach (var conformer in cc)
                {
                    // do something with each conformer
                }
            }
            #endregion
        }
    }
}
