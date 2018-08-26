using NCDK.Tools.Manipulator;
using System;

namespace NCDK.Formula
{
    /// <summary>
    /// This class defines a isotope container. It contains in principle a
    /// <see cref="IMolecularFormula"/>, a mass and intensity/abundance value.
    /// </summary>
    // @author Miguel Rojas Cherto
    // @cdk.module  formula
    // @cdk.githash
    public class IsotopeContainer
    {
        private IMolecularFormula formula;
        private double mass;
        private double intensity;

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
            if (formula != null)
                Mass = MolecularFormulaManipulator.GetTotalExactMass(formula);
            Intensity = intensity;
        }

        /// <summary>
        /// Constructor of the <see cref="IsotopeContainer"/> object setting a mass and intensity value.
        /// </summary>
        /// <param name="mass">The mass of this container</param>
        /// <param name="intensity">The intensity of this container</param>
        public IsotopeContainer(double mass, double intensity)
        {
            Mass = mass;
            Intensity = intensity;
        }

        /// <summary>
        /// The <see cref="IMolecularFormula"/> object of this container.
        /// </summary>
        public IMolecularFormula Formula
        {
            get => formula;
            set => formula = value;
        }

        /// <summary>
        /// the mass value of this container.
        /// </summary>
        public double Mass
        {
            get => mass;
            set => mass = value;
        }

        /// <summary>
        /// the intensity value of this container.
        /// </summary>
        public double Intensity
        {
            get => intensity;
            set => intensity = value;
        }

        /// <summary>
        /// Clones this <see cref="IsotopeContainer"/> object and its content.
        /// </summary>
        /// <returns>The cloned object</returns>
        public object Clone()
        {
            var isoClone = new IsotopeContainer
            {
                formula = formula,
                intensity = intensity,
                mass = mass
            };
            return isoClone;
        }
    }
}
