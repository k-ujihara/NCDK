using System;
using NCDK.Numerics;

namespace NCDK
{
    /// <summary>
    /// Class representing a molecular crystal.
    /// The crystal is described with molecules in fractional
    /// coordinates and three cell axes: a,b and c.
    /// The crystal is designed to store only the asymetric atoms.
    /// Though this is not enforced, it is assumed by all methods.
    /// </summary>
    public interface ICrystal
        : IAtomContainer
    {
        /// <summary>
        /// the A unit cell axes in carthesian coordinates in a eucledian space.
        /// </summary>
        Vector3 A { get; set; }
        Vector3 B { get; set; }
        Vector3 C { get; set; }

        /// <summary>
        /// the space group of this crystal.
        /// </summary>
        string SpaceGroup { get; set; }

        /// <summary>
        /// the number of asymmetric parts in the unit cell.
        /// </summary>
        int? Z { get; set; }
    }
}