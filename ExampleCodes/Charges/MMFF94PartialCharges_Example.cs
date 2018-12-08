using NCDK.Smiles;
using NCDK.Tools.Manipulator;
using System;

namespace NCDK.Charges
{
    public class MMFF94PartialCharges_Example
    {
        public void Ctor()
        {
            #region 
            SmilesParser sp = new SmilesParser();
            IAtomContainer ac = sp.ParseSmiles("CC");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(ac);
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(ac);
            MMFF94PartialCharges mmff = new MMFF94PartialCharges();
            mmff.AssignMMFF94PartialCharges(ac);
            #endregion
            foreach (var atom in ac.Atoms)
                Console.WriteLine(
                #region result
                    atom.GetProperty<double>("MMFF94charge")
                #endregion
                    );
        }
    }
}
