using NCDK.Templates;

namespace NCDK.ForceFields
{
    public static class Mmff_Example
    {
        public static void Main()
        {
            {
                #region 
                IAtomContainer mol = TestMoleculeFactory.MakeAlphaPinene();
                Mmff mmff = new Mmff();
                mmff.AssignAtomTypes(mol);
                mmff.PartialCharges(mol);
                mmff.ClearProps(mol); // optional
                #endregion
            }
        }
    }
}
