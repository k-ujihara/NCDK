using System;
using System.Collections.Generic;

namespace NCDK
{
    /// <summary>
    /// Stereochemistry specification for double bond stereochemistry. The data model defines the double
    /// atoms and two ligands attached to those two atoms, linearly connected with the double bond in the
    /// middle. The IBonds that define the stereo element are defined as an array where the bonds
    /// are sorted according to the linear connectivity.Thus, the first and third bonds are the two
    /// bonds attached on either side of the double bond, and the second bond is the double bond.
    /// The stereo annotation then indicates if the ligand atoms are in the cis position
    /// (Conformation.Together) or in the trans position (Conformation.Opposite), matching the
    /// orientation of the methyls in but-2-ene respectively as <i>Z</i> and <i>E</i>.
    /// </summary>
    public interface IDoubleBondStereochemistry
        : IStereoElement
    {
        IReadOnlyList<IBond> Bonds { get; }
        IBond StereoBond { get; }
        DoubleBondConformation Stereo { get; }
    }
}
