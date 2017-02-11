using System;
using NCDK.Numerics;

namespace NCDK
{
    /// <summary>
    /// Represents the idea of an chemical atom.
    /// </summary>
    public interface IAtom
        : IAtomType
    {
        double? Charge { get; set; }
        int? ImplicitHydrogenCount { get; set; }
        Vector2? Point2D { get; set; }
        Vector3? Point3D { get; set; }
        /// <summary>
        /// A point specifying the location of this atom in a Crystal unit cell.
        /// </summary>
        Vector3? FractionalPoint3D { get; set; }
        int? StereoParity { get; set; }

        /**
         * Flag used for marking uncertainty of the bond order.
         * If used on an
         * <ul>
         *  <li><see cref="IAtomContainer"/> it means that one or several of the bonds have
         * 		this flag raised (which may indicate aromaticity).</li>
         *  <li>{@link IBond} it means that it's unclear whether the bond is a single or
         * 		double bond.</li>
         *  <li>{@link IAtom} it is a way for the Smiles parser to indicate that this atom was
         * 		written with a lower case letter, e.g. 'c' rather than 'C'</li>
         * </ul>
         */
        bool IsSingleOrDouble { get; set; }
    }
}
