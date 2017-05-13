namespace NCDK.Geometries
{
    public class RDFCalculator_Example
    {
        public void Main()
        {
            #region
            RDFCalculator calculator = new RDFCalculator(0.0, 5.0, 0.1, 0.0,
                delegate(IAtom atom, IAtom atom2) { return atom.Charge.Value * atom2.Charge.Value; });
            #endregion
        }
    }
}
