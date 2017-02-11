using System.Collections.Generic;

namespace NCDK.Formula
{
    /**
    // This class defines the properties of a deisotoped
    // pattern distribution. A isotope pattern is a set of
    // compounds with different set of isotopes.
     *
    // @author Miguel Rojas Cherto
     *
    // @cdk.module formula
    // @cdk.githash
     */
    public class IsotopePattern
    {
		internal List<IsotopeContainer> isotopes = new List<IsotopeContainer>();
        public IList<IsotopeContainer> Isotopes => isotopes;

        private int monoIsotopePosition;

        /**
        // Constructor of the IsotopePattern object.
         */
        public IsotopePattern()
        {

        }

        /**
        // Set the mono isotope object. Adds the isoContainer to the isotope 
        //                  pattern, if it is not already added. 
         *
        //  @param isoContainer   The IsotopeContainer object
         */
        public void SetMonoIsotope(IsotopeContainer isoContainer)
        {
            if (!Isotopes.Contains(isoContainer))
                Isotopes.Add(isoContainer);
            monoIsotopePosition = Isotopes.IndexOf(isoContainer);
        }

        /**
        // Returns the mono-isotope peak that form this isotope pattern.
         *
        // @return The IsotopeContainer acting as mono-isotope
         */
        public IsotopeContainer GetMonoIsotope()
        {
            return Isotopes[monoIsotopePosition];
        }

        /// <summary>
        /// the charge in this pattern.
        /// </summary>
        public double Charge { get; set; } = 0;

        /**
        // Clones this IsotopePattern object and its content.
         *
        // @return    The cloned object
         */
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
