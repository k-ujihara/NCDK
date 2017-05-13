using NCDK.Templates;

namespace NCDK.ForceField.MMFF
{
    public class Mmff_Example
    {
        public void Main()
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
