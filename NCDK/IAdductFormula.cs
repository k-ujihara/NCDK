using System;
using System.Collections.Generic;

namespace NCDK
{
    /// <summary>
    /// Class defining an adduct object in a MolecularFormula. 
    /// It maintains a list of list IMolecularFormula.
    /// Examples: [C2H4O2+Na]+
    /// </summary>
    public interface IAdductFormula
        : IMolecularFormulaSet
    {
        /// <summary>
        ///  Checks a set of Nodes for the occurrence of the isotope in the
        ///  adduct formula from a particular isotope. It returns 0 if the does not exist.
        ///
        /// <param name="isotope">The IIsotope to look for</param>
        /// <returns>The occurrence of this isotope in this adduct</returns>
        /// @see                      #GetCount()
        /// </summary>
        int GetCount(IIsotope isotope);

        /// <summary>
        ///  Checks a set of Nodes for the number of different isotopes in the
        ///  adduct formula.
        ///
        /// <returns>The the number of different isotopes in this adduct formula</returns>
        /// @see           #GetCount(IIsotope)
        /// </summary>
        int IsotopeCount { get; }

        /// <summary>
        ///  An IEnumerator for looping over all isotopes in this adduct formula.
        /// </summary>
        IEnumerable<IIsotope> GetIsotopes();

        /// <summary>
        ///  The partial charge of this Adduct. If the charge
        ///  has not been set the return value is double.NaN.
        /// </summary>
        int? Charge { get; set; }

        /// <summary>
        ///  True, if the AdductFormula contains the given IIsotope object.
        ///
        /// <param name="isotope">The IIsotope this AdductFormula is searched for</param>
        /// <returns>True, if the AdductFormula contains the given isotope object</returns>
        /// </summary>
        bool Contains(IIsotope isotope);
    }
}
