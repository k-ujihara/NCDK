using NCDK.Tools.Manipulator;

namespace NCDK.Formula
{
    /// <summary>
    /// This class defines a isotope container. It contains in principle a
    /// IMolecularFormula, a mass and intensity/abundance value.
    /// </summary>
    // @author Miguel Rojas Cherto
    // @cdk.module  formula
    // @cdk.githash
    public class IsotopeContainer
    {
        public IsotopeContainer()
        {
        }

        /// <summary>
        /// Constructor of the <see cref="IsotopeContainer"/> object setting a <see cref="IMolecularFormula"/> object and intensity value.
        /// </summary>
        /// <param name="formula">The formula of this container</param>
        /// <param name="intensity">The intensity of this container</param>
        public IsotopeContainer(IMolecularFormula formula, double intensity)
        {
            Formula = formula;
            if (formula != null) Mass = MolecularFormulaManipulator.GetTotalExactMass(formula);
            Intensity = intensity;
        }

        /// <summary>
        /// Constructor of the IsotopeContainer object setting a mass and intensity value.
        /// </summary>
        /// <param name="mass">The mass of this container</param>
        /// <param name="intensity">The intensity of this container</param>
        public IsotopeContainer(double mass, double intensity)
        {
            Mass = mass;
            Intensity = intensity;
        }

        /// <summary>
        /// the IMolecularFormula object of this container.
        /// </summary>
        public IMolecularFormula Formula { get; set; }

        /// <summary>
        /// the mass value of this container.
        /// </summary>
        public double Mass { get; set; }

        /// <summary>
        /// the intensity value of this container.
        /// </summary>
        public double Intensity { get; set; }

        /// <summary>
        /// Clones this IsotopeContainer object and its content.
        /// </summary>
        /// <returns>The cloned object</returns>
        public object Clone()
        {
            IsotopeContainer isoClone = new IsotopeContainer();
            isoClone.Formula = Formula;
            isoClone.Intensity = Intensity;
            isoClone.Mass = Mass;
            return isoClone;
        }
    }
}
