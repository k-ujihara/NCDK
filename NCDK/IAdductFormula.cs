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
        /**
         *  Checks a set of Nodes for the occurrence of the isotope in the
         *  adduct formula from a particular isotope. It returns 0 if the does not exist.
         *
         * @param   isotope          The IIsotope to look for
         * @return                   The occurrence of this isotope in this adduct
         * @see                      #GetCount()
         */
        int GetCount(IIsotope isotope);

        /**
         *  Checks a set of Nodes for the number of different isotopes in the
         *  adduct formula.
         *
         * @return        The the number of different isotopes in this adduct formula
         * @see           #GetCount(IIsotope)
         */
        int IsotopeCount { get; }

        /**
         *  An IEnumerator for looping over all isotopes in this adduct formula.
         */
        IEnumerable<IIsotope> Isotopes { get; }

        /**
         *  The partial charge of this Adduct. If the charge
         *  has not been set the return value is double.NaN.
         */
        int? Charge { get; set; }

        /**
         *  True, if the AdductFormula contains the given IIsotope object.
         *
         * @param  isotope  The IIsotope this AdductFormula is searched for
         * @return          True, if the AdductFormula contains the given isotope object
         */
        bool Contains(IIsotope isotope);
    }
}
