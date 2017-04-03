using NCDK.IO.Listener;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.IO
{
    class MDLV2000Writer_Example
    {
        void Main()
        {
            {
                IAtomContainer molecule = null;
                #region
                using (var srm = new FileStream("output.mol", FileMode.Create))
                using (MDLV2000Writer writer = new MDLV2000Writer(srm))
                {
                    writer.Write((IAtomContainer)molecule);
                }
                #endregion
            }
            {
                MDLV2000Writer writer = null;
                #region listener
                var customSettings = new NameValueCollection();
                customSettings["ForceWriteAs2DCoordinates"] = "true";
                PropertiesListener listener = new PropertiesListener(customSettings);
                writer.Listeners.Add(listener);
                #endregion
            }
        }
    }
}
