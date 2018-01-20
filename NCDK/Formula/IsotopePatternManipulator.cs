namespace NCDK.Formula
{
    /// <summary>
    /// Class to manipulate IsotopePattern objects.
    /// </summary>
    // @author Miguel Rojas Cherto
    // @cdk.module  formula
    // @cdk.githash
    public class IsotopePatternManipulator
    {
        /// <summary>
        /// Return the isotope pattern normalized to the highest abundance.
        /// </summary>
        /// <param name="isotopeP">The IsotopePattern object to normalize</param>
        /// <returns>The IsotopePattern normalized</returns>
        public static IsotopePattern Normalize(IsotopePattern isotopeP)
        {
            IsotopeContainer isoHighest = null;

            double biggestAbundance = 0;
            /* Extraction of the isoContainer with the highest abundance */
            foreach (var isoContainer in isotopeP.Isotopes)
            {
                double abundance = isoContainer.Intensity;
                if (biggestAbundance < abundance)
                {
                    biggestAbundance = abundance;
                    isoHighest = isoContainer;
                }
            }
            /* Normalize */
            IsotopePattern isoNormalized = new IsotopePattern();
            foreach (var isoContainer in isotopeP.Isotopes)
            {
                double inten = isoContainer.Intensity / isoHighest.Intensity;
                IsotopeContainer icClone;
                icClone = (IsotopeContainer)isoContainer.Clone();
                icClone.Intensity = inten;
                if (isoHighest.Equals(isoContainer))
                    isoNormalized.SetMonoIsotope(icClone);
                else
                    isoNormalized.Isotopes.Add(icClone);
            }
            isoNormalized.Charge = isotopeP.Charge;
            return isoNormalized;
        }

        /// <summary>
        /// Return the isotope pattern sorted and normalized by intensity
        /// to the highest abundance.
        /// </summary>
        /// <param name="isotopeP">The IsotopePattern object to sort</param>
        /// <returns>The IsotopePattern sorted</returns>
        public static IsotopePattern SortAndNormalizedByIntensity(IsotopePattern isotopeP)
        {
            IsotopePattern isoNorma = Normalize(isotopeP);
            return SortByIntensity(isoNorma);
        }

        /// <summary>
        /// Return the isotope pattern sorted by intensity to the highest abundance.
        /// </summary>
        /// <param name="isotopeP">The IsotopePattern object to sort</param>
        /// <returns>The IsotopePattern sorted</returns>
        public static IsotopePattern SortByIntensity(IsotopePattern isotopeP)
        {
            IsotopePattern isoSort = (IsotopePattern)isotopeP.Clone();

            // Do nothing for empty isotope pattern
            if (isoSort.Isotopes.Count == 0)
                return isoSort;

            // Sort the isotopes
            var listISO = isoSort.Isotopes;
            isoSort.isotopes.Sort(
                delegate (IsotopeContainer o1, IsotopeContainer o2)
                {
                    return o2.Intensity.CompareTo(o1.Intensity);
                });
            // Set the monoisotopic peak to the one with highest intensity
            isoSort.SetMonoIsotope(listISO[0]);

            return isoSort;
        }

        /// <summary>
        /// Return the isotope pattern sorted by mass to the highest abundance.
        /// </summary>
        /// <param name="isotopeP">The IsotopePattern object to sort</param>
        /// <returns>The IsotopePattern sorted</returns>
        public static IsotopePattern SortByMass(IsotopePattern isotopeP)
        {
            IsotopePattern isoSort = (IsotopePattern)isotopeP.Clone();

            // Do nothing for empty isotope pattern
            if (isoSort.Isotopes.Count == 0)
                return isoSort;

            // Sort the isotopes
            var listISO = isoSort.Isotopes;
            isoSort.isotopes.Sort(delegate (IsotopeContainer o1, IsotopeContainer o2)
            {
                return o1.Mass.CompareTo(o2.Mass);
            });

            // Set the monoisotopic peak to the one with lowest mass
            isoSort.SetMonoIsotope(listISO[0]);

            return isoSort;
        }
    }
}
