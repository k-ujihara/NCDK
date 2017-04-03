
using NCDK.Smiles;
using NCDK.Tools.Manipulator;

namespace NCDK.Tools
{
    class AtomContainerManipulator_Example
    {
        void Main()
        {
            {
                IAtomContainer container = null;
                IAtom atom1 = null;
                IAtom atom2 = null;
                #region 1
                AtomContainerManipulator.ReplaceAtomByAtom(container, atom1, atom2);
                #endregion
            }
            #region SetSingleOrDoubleFlags
            SmilesParser parser = new SmilesParser(Default.ChemObjectBuilder.Instance);
            parser.IsPreservingAromaticity = true;

            IAtomContainer biphenyl = parser.ParseSmiles("c1cccc(c1)c1ccccc1");

            AtomContainerManipulator.SetSingleOrDoubleFlags(biphenyl);
            #endregion
        }
    }
}
