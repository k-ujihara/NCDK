using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Templates;

namespace NCDK.Graphs
{
    [TestClass]
    public class ConnectivityChecker_Example
    {
        [TestMethod]
        public void Main()
        {
            var container = TestMoleculeFactory.MakeBenzene();
            #region 1
            bool isConnected = ConnectivityChecker.IsConnected(container);
            #endregion

            var disconnectedContainer = TestMoleculeFactory.MakeBenzene();
            #region 2
            var fragments = ConnectivityChecker.PartitionIntoMolecules(disconnectedContainer);
            int fragmentCount = fragments.Count;
            #endregion
        }
    }
}
