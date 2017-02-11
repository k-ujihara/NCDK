using System;

namespace NCDK
{
    /// <summary>
    /// Implements the idea of an element in the periodic table.
    /// </summary>
    public interface IElement
        : IChemObject
    {
        /// <summary>
        /// Returns the atomic number of this element.
        /// </summary>
        int? AtomicNumber { get; set; }

        /// <summary>
        /// Returns the element symbol of this element.
        /// </summary>
        string Symbol { get; set; }
    }
}
