using NCDK.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.Layout
{
    class HydrogenPlacer_Example
    {
        void Main()
        {
            #region
            IAtomContainer container = TestMoleculeFactory.MakeAlphaPinene();
            HydrogenPlacer hydrogenPlacer = new HydrogenPlacer();
            hydrogenPlacer.PlaceHydrogens2D(container, 1.5);
            #endregion
        }
    }
}
