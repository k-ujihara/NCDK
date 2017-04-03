using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Templates;

namespace NCDK.ForceField.MMFF
{
    [TestClass]
    public class Mmff_Example
    {
        [TestCategory("Example")]
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
