using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.Stereo
{
    static class Stereocenters_Example
    {
        static void Main()
        {
            IAtomContainer someContainer = null;
            #region Of
            IAtomContainer container = someContainer;
            Stereocenters centers = Stereocenters.Of(container);
            for (int i = 0; i < container.Atoms.Count; i++)
            {
                if (centers.IsStereocenter(i))
                {

                }
            }
            #endregion
        }
    }
}
