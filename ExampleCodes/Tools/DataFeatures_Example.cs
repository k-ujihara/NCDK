using NCDK.IO.Formats;

namespace NCDK.Tools
{
    static class DataFeatures_Example
    {
        static void Main()
        {
            #region 1
            var features = new XYZFormat().SupportedDataFeatures;
            bool has3DCoords = (features & DataFeatures.Has3DCoordinates) == DataFeatures.Has3DCoordinates;
            #endregion
        }
    }
}
