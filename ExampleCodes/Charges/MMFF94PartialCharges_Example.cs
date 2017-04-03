using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Smiles;
using NCDK.Tools.Manipulator;
using System;

namespace NCDK.Charges
{
    [TestClass()]
    public class MMFF94PartialCharges_Example
    {
        [TestMethod]
        public void Ctor()
        {
            #region 
            SmilesParser sp = new SmilesParser(Silent.ChemObjectBuilder.Instance);
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
