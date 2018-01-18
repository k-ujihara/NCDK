
using NCDK.IO.Formats;
using static NCDK.Tools.DataFeatures;

namespace NCDK.Tools
{
    class DataFeatures_Example
    {
        void Main()
        {
            #region 1
            var features = new XYZFormat().SupportedDataFeatures;
            bool has3DCoords = (features & HAS_3D_COORDINATES) == HAS_3D_COORDINATES;
            #endregion
        }
    }
}
