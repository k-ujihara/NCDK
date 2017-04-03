using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.Modeling.Builder3D
{
    class ModelBuilder3D_Example
    {
        void Main()
        {
            IAtomContainer mol = null;
            #region
            ModelBuilder3D mb3d = ModelBuilder3D.GetInstance(Default.ChemObjectBuilder.Instance);
            IAtomContainer molecule = mb3d.Generate3DCoordinates(mol, false);
            #endregion
        }
    }
}
