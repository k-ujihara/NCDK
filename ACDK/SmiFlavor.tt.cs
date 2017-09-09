
using System;
using System.Runtime.InteropServices;

namespace ACDK
{
    /// <summary>
    /// Flags for customising SMILES generation.
    /// </summary>
    [Guid("23865598-C7BF-4FE9-B48F-6CDDCB51F485")]
    [Flags]
    public enum SmiFlavor
    {
		
        /// <summary>
        /// Output SMILES in a canonical order. The order is not guaranteed to be
        /// equivalent between releases.
        /// </summary>
        Canonical = 0x001,

        /// <summary>
        /// Output SMILES in a canonical order using the InChI labelling algorithm.
        /// </summary>
        /// <seealso cref="UniversalSmiles"/>
        InChILabelling = 0x003,

        /// <summary>
        /// Output atom-atom mapping for reactions and atom classes for molecules. The
        /// map index is set on an atom with property <see cref="CDKPropertyName.AtomAtomMapping"/>
        /// using <see cref="IChemObject.SetProperty(object, object)"/>.
        /// </summary>
        AtomAtomMap = 0x004,

        /// <summary>
        /// Output atomic mass on atoms. For historical reasons the atomic mass is
        /// often set on all atoms in a CDK molecule. Therefore to avoid SMILES like
        /// <code>[12CH3][12CH2][16OH]</code> major isotopes are not generated. If you
        /// wish to generate SMILES with the major isotopes please use the flag
        /// <see cref="AtomicMassStrict"/> this will output all mass numbers and only be
        /// omitted when the mass is unset (null).
        /// </summary>
        AtomicMass = 0x008,

        /// <summary>
        /// Writes aromatic atoms as lower case letters. For portability
        /// this option is not recommended.
        /// </summary>
        UseAromaticSymbols = 0x010,

        // public static final int SuppressHydrogens  = 0x020;

        /// <summary>
        /// Output tetrahedral stereochemistry on atoms as <code>@</code> and <code>@@</code>.
        /// </summary>
        /// <seealso cref="Stereo"/>
        StereoTetrahedral = 0x100,

        /// <summary>
        /// Output <i>cis</i>-<i>trans</i> stereochemistry specified by directional <code>\</code>
        /// of <code>/</code> bonds.
        /// </summary>
        /// <seealso cref="Stereo"/>
        StereoCisTrans = 0x200,

        /// <summary>
        /// Output extended tetrahedral stereochemistry on atoms as <pre>@</pre> and
        /// <pre>@@</pre>. Extended tetrahedral captures rotations around a cumulated
        /// carbon: <pre>CC=[C@]=CC</pre>.
        /// </summary>
        /// <seealso cref="Stereo"/>
        StereoExTetrahedral = 0x400,

        /// <summary>
        /// Generate SMILES with the major isotopes, only omit mass numbers when it
        /// is unset.
        /// </summary>
        AtomicMassStrict = 0x800,

        /// <summary>
        /// Output supported stereochemistry types.
        /// </summary>
        /// <seealso cref="StereoTetrahedral"/>
        /// <seealso cref="StereoCisTrans"/>
        /// <seealso cref="StereoExTetrahedral"/>
        Stereo = StereoTetrahedral | StereoCisTrans | StereoExTetrahedral,

        /// <summary>
        /// Output 2D coordinates.
        /// </summary>
        Cx2dCoordinates = 0x001000,

        /// <summary>
        /// Output 3D coordinates.
        /// </summary>
        Cx3dCoordinates = 0x002000,

        /// <summary>
        /// Output either 2D/3D coordinates.
        /// </summary>
        CxCoordinates = Cx3dCoordinates | Cx2dCoordinates,

        /// <summary>
        /// Output atom labels, atom labels are specified by <see cref="IPseudoAtom.Label"/>.
        /// </summary>
        CxAtomLabel = 0x008000,

        /// <summary>
        /// Output atom values, atom values are specified by <see cref="IPseudoAtom.Label"/>.
        /// </summary>
        CxAtomValue = 0x010000,

        /// <summary>
        /// Output radicals, radicals are specified by <see cref="IAtomContainer.GetConnectedSingleElectrons(IAtom)"/>
        /// </summary>
        CxRadical = 0x020000,

        /// <summary>
        /// Output multi-center bonds, positional variation is specified with <see cref="SGroups.Sgroup"/>s
        /// of the type <see cref="SGroups.SgroupType.ExtMulticenter"/>.
        /// </summary>
        CxMulticenter = 0x040000,

        /// <summary>
        /// Output polymer repeat units is specified with <see cref="SGroups.Sgroup"/>s.
        /// </summary>
        CxPolymer = 0x080000,

        /// <summary>
        /// Output fragment grouping for reactions.
        /// </summary>
        CxFragmentGroup = 0x100000,

        /// <summary>
        /// Output CXSMILES layers.
        /// </summary>
        CxSmiles = CxAtomLabel | CxAtomValue | CxRadical | CxFragmentGroup | CxMulticenter | CxPolymer,

        /// <summary>
        /// Output CXSMILES layers and coordinates.
        /// </summary>
        CxSmilesWithCoords = CxSmiles | CxCoordinates,

        /// <summary>
        /// Output non-canonical SMILES without stereochemistry, atomic masses.
        /// </summary>
        Generic = 0,

        /// <summary>
        /// Output canonical SMILES without stereochemistry, atomic masses.
        /// </summary>
        Unique = Canonical,

        /// <summary>
        /// Output non-canonical SMILES with stereochemistry, atomic masses.
        /// </summary>
        Isomeric = Stereo | AtomicMass,

        /// <summary>
        /// Output canonical SMILES with stereochemistry, atomic masses.
        /// </summary>
        Absolute = Canonical | Isomeric,

        /// <summary>
        /// Default SMILES output write Stereochemistry, Atomic Mass, and CXSMILES layers. The
        /// ordering is not canonical.
        /// </summary>
        Default = Stereo | AtomicMass | CxSmiles,

        /// <summary>
        /// Output canonical SMILES with stereochemistry and atomic masses, This output uses the
        /// InChI labelling algorithm to generate a 'Universal SMILES' <token>cdk-cite-OBoyle12</token>.
        /// </summary>
        /// <remarks>
        /// Unfortunately there are several issues and general use is not recommended:
        /// <ul>
        ///  <li>MAJOR: Molecules with delocalised charges are generally non-canonical, e.g.
        ///             <code>C(\C=C\N1CCCC1)=C/c2[n+](c3c(n2CC)nc4ccccc4n3)CC</code> will generate two different
        ///             SMILES depending on input order</li>
        ///  <li>MINOR: Double bond '/' '\' assignment is different from the original paper (O'Boyle) and
        ///             will not match universal SMILES generated by Open Babel</li>
        ///  <li>MINOR: SMILES with '*' atoms can not be canonicalised by default, to avoid this we use
        ///             the 'Rf' atom as a substitute. Structures with an 'Rf' atom can still be generated
        ///             providing there are no '*' atoms.</li>
        ///  <li>MINOR: The InChI library (v1.03) is not thread safe</li>
        /// </ul>
        /// </remarks>
        UniversalSmiles = InChILabelling | Isomeric,
    
    }
}
