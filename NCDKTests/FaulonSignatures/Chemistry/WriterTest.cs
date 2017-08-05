using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.FaulonSignatures.Chemistry
{
    [TestClass()]
    public class WriterTest
    {
        [TestMethod()]
        public void MinimalTest()
        {
            Molecule mol = new Molecule();
            mol.AddAtom("C");
            mol.AddAtom("O");
            mol.AddAtom("N");
            mol.AddSingleBond(0, 1);
            mol.AddBond(0, 2, Molecule.BondOrder.Double);

            // TODO : test this somehow...
            //       MoleculeWriter.WriteToStream(System.out, mol);
        }
    }
}
