using NCDK.Silent;

namespace NCDK.Modelings.Builder3D
{
    class ModelBuilder3D_Example
    {
        void Main()
        {
            IAtomContainer mol = null;
            #region
            ModelBuilder3D mb3d = ModelBuilder3D.GetInstance();
            IAtomContainer molecule = mb3d.Generate3DCoordinates(mol, false);
            #endregion
        }
    }
}
