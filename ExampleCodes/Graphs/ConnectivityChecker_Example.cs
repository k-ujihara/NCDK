using NCDK.Templates;

namespace NCDK.Graphs
{
    public class ConnectivityChecker_Example
    {
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
