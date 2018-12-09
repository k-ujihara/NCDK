using NCDK.Silent;

namespace NCDK.Graphs.Invariant
{
    class ConjugatedPiSystemsDetector_Example
    {
        public static void Main(string[] args)
        {
            {
                var mol = new AtomContainer();
                #region Detect
                Atom a0 = new Atom("C"); mol.Atoms.Add(a0);
                Atom a1 = new Atom("C"); mol.Atoms.Add(a1);
                Atom a2 = new Atom("C"); mol.Atoms.Add(a2);
                Atom h1 = new Atom("H"); mol.Atoms.Add(h1);
                Atom h2 = new Atom("H"); mol.Atoms.Add(h2);
                Atom h3 = new Atom("H"); mol.Atoms.Add(h3);
                Atom h4 = new Atom("H"); mol.Atoms.Add(h4);
                Atom h5 = new Atom("H"); mol.Atoms.Add(h5);
                mol.AddBond(a0, a1, BondOrder.Double);
                mol.AddBond(a1, a2, BondOrder.Single);
                mol.AddBond(a0, h1, BondOrder.Single);
                mol.AddBond(a0, h2, BondOrder.Single);
                mol.AddBond(a1, h3, BondOrder.Single);
                mol.AddBond(a2, h4, BondOrder.Single);
                mol.AddBond(a2, h5, BondOrder.Single);
                SingleElectron se = new SingleElectron(a2);
                mol.Add(se);

                var pi_systems = ConjugatedPiSystemsDetector.Detect(mol);
                #endregion
            }
        }
    }
}
