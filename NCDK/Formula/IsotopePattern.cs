using System.Collections.Generic;

namespace NCDK.Formula
{
    /// <summary>
    /// This class defines the properties of a deisotoped
    /// pattern distribution. A isotope pattern is a set of
    /// compounds with different set of isotopes.
    /// </summary>
    // @author Miguel Rojas Cherto
    // @cdk.module formula
    // @cdk.githash
    public class IsotopePattern
    {
        internal List<IsotopeContainer> isotopes = new List<IsotopeContainer>();
        public IList<IsotopeContainer> Isotopes => isotopes;

        private int monoIsotopePosition;

        /// <summary>
        /// Constructor of the IsotopePattern object.
        /// </summary>
        public IsotopePattern()
        { }

        /// <summary>
        /// Set the mono isotope object. Adds the isoContainer to the isotope 
        ///                 pattern, if it is not already added. 
        /// </summary>
        /// <param name="isoContainer">The IsotopeContainer object</param>
        public void SetMonoIsotope(IsotopeContainer isoContainer)
        {
            if (!Isotopes.Contains(isoContainer))
                Isotopes.Add(isoContainer);
            monoIsotopePosition = Isotopes.IndexOf(isoContainer);
        }

        /// <summary>
        /// Returns the mono-isotope peak that form this isotope pattern.
        /// </summary>
        /// <returns>The IsotopeContainer acting as mono-isotope</returns>
        public IsotopeContainer GetMonoIsotope()
        {
            return Isotopes[monoIsotopePosition];
        }

        /// <summary>
        /// the charge in this pattern.
        /// </summary>
        public double Charge { get; set; } = 0;

        /// <summary>
        /// Clones this IsotopePattern object and its content.
        /// </summary>
        /// <returns>The cloned object</returns>
        public object Clone()
        {
            IsotopePattern isoClone = new IsotopePattern();
            IsotopeContainer isoHighest = GetMonoIsotope();
            foreach (var isoContainer in Isotopes)
            {
                if (isoHighest.Equals(isoContainer))
                    isoClone.SetMonoIsotope((IsotopeContainer)isoContainer.Clone());
                else
                    isoClone.Isotopes.Add((IsotopeContainer)isoContainer.Clone());
            }
            isoClone.Charge = Charge;
            return isoClone;
        }
    }
}
