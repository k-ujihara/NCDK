using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.IO.Iterator;
using System.IO;

namespace NCDK
{
    [TestClass]
    public class ConformerContainer_Example
    {
        [TestCategory("Example")]
        public void Ctor()
        {
            string filename = null;
            #region
            var reader = new IteratingMDLConformerReader(
                new FileStream(filename, FileMode.Open),
                Default.ChemObjectBuilder.Instance);
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
