using NCDK.IO.Listener;
using System.Collections.Specialized;
using System.IO;

namespace NCDK.IO
{
    class MDLV2000Writer_Example
    {
        static void Main()
        {
            {
                IAtomContainer molecule = null;
                #region
                using (var writer = new MDLV2000Writer(new FileStream("output.mol", FileMode.Create)))
                {
                    writer.Write((IAtomContainer)molecule);
                }
                #endregion
            }
            {
                MDLV2000Writer writer = null;
                #region listener
                var customSettings = new NameValueCollection
                {
                    ["ForceWriteAs2DCoordinates"] = "true"
                };
                var listener = new PropertiesListener(customSettings);
                writer.Listeners.Add(listener);
                #endregion
            }
        }
    }
}
