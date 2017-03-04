using System;
using System.Collections.Generic;

namespace NCDK
{
    public static class CDKConstants
    {
        /// <summary>A positive atom parity.</summary>
        public const int STEREO_ATOM_PARITY_PLUS = 1;
        /// <summary>A negative atom parity.</summary>
        public const int STEREO_ATOM_PARITY_MINUS = -1;
        /// <summary>A undefined atom parity.</summary>
        public const int STEREO_ATOM_PARITY_UNDEFINED = 0;
    }

    public static class CDKPropertyName
    {
        /// <summary>
        /// Carbon NMR shift constant for use as a key in the
        /// <see cref="IChemObject"/>.physicalProperties hashtable.
        /// </summary>
        /// <seealso cref="NCDK.Default.ChemObject"/>
        public const string NMRSHIFT_CARBON = "carbon nmr shift";

        /// <summary>
        /// Hydrogen NMR shift constant for use as a key in the
        /// <see cref="IChemObject"/>.physicalProperties hashtable.
        /// </summary>
        /// <seealso cref="NCDK.Default.ChemObject"/>
        public const string NMRSHIFT_HYDROGEN = "hydrogen nmr shift";

        /// <summary>
        /// Nitrogen NMR shift constant for use as a key in the
        /// <see cref="IChemObject"/>.physicalProperties hashtable.
        /// </summary>
        /// <seealso cref="NCDK.Default.ChemObject"/>
        public const string NMRSHIFT_NITROGEN = "nitrogen nmr shift";

        /// <summary> Phosphorus NMR shift constant for use as a key in the
        /// <see cref="IChemObject"/>.physicalProperties hashtable.</summary>
        /// <seealso cref="NCDK.Default.ChemObject"/>
        public const string NMRSHIFT_PHOSPORUS = "phosphorus nmr shift";

        /// <summary> Fluorine NMR shift constant for use as a key in the
        /// <see cref="IChemObject"/>.physicalProperties hashtable.</summary>
        /// <seealso cref="NCDK.Default.ChemObject"/>
        public const string NMRSHIFT_FLUORINE = "fluorine nmr shift";

        /// <summary> Deuterium NMR shift constant for use as a key in the
        /// <see cref="IChemObject"/>.physicalProperties hashtable.</summary>
        /// <seealso cref="NCDK.Default.ChemObject"/>

        public const string NMRSHIFT_DEUTERIUM = "deuterium nmr shift";

        /// <summary> 
        /// Property key to store the CIP descriptor label for an atom / bond. The
        /// label is a string.
        /// </summary>
        public const string CIP_DESCRIPTOR = "cip.label";

        /// <summary>
        /// Flag used for JUnit testing the pointer functionality.
        /// </summary>
        public const int DUMMY_POINTER = 1;

        /// <summary>
        /// Maximum pointers array index.
        /// </summary>
        public const int MAX_POINTER_INDEX = 1;

        // **************************************
        // Some predefined property names for * ChemObjects *
        // **************************************

        /// <summary>The title for a IChemObject.</summary>
        public const string TITLE = "cdk:Title";

        /// <summary>A remark for a IChemObject.</summary>
        public const string REMARK = "cdk:Remark";

        /// <summary>A string comment.</summary>
        public const string COMMENT = "cdk:Comment";

        /// <summary>A List of names.</summary>
        public const string NAMES = "cdk:Names";

        /// <summary>A List of annotation remarks.</summary>
        public const string ANNOTATIONS = "cdk:Annotations";

        /// <summary>A description for a IChemObject.</summary>
        public const string DESCRIPTION = "cdk:Description";

        // **************************************
        // Some predefined property names for * Molecules *
        // **************************************

        /// <summary>The Daylight SMILES.</summary>
        public const string SMILES = "cdk:SMILES";

        /// <summary>The IUPAC International Chemical Identifier.</summary>
        public const string INCHI = "cdk:InChI";

        /// <summary>The Molecular Formula Identifier.</summary>
        public const string FORMULA = "cdk:Formula";

        /// <summary>The IUPAC compatible name generated with AutoNom.</summary>
        public const string AUTONOMNAME = "cdk:AutonomName";

        /// <summary>The Beilstein Registry Number.</summary>
        public const string BEILSTEINRN = "cdk:BeilsteinRN";

        /// <summary>The CAS Registry Number.</summary>
        public const string CASRN = "cdk:CasRN";

        /// <summary>A set of all rings computed for this molecule.</summary>
        public const string ALL_RINGS = "cdk:AllRings";

        /// <summary>A smallest set of smallest rings computed for this molecule.</summary>
        public const string SMALLEST_RINGS = "cdk:SmallestRings";

        /// <summary>The essential rings computed for this molecule.
        ///  The concept of Essential Rings is defined in
        ///  SSSRFinder
        /// </summary>
        public const string ESSENTIAL_RINGS = "cdk:EssentialRings";

        /// <summary>The relevant rings computed for this molecule.
        ///  The concept of relevant Rings is defined in
        ///  SSSRFinder
        /// </summary>
        public const string RELEVANT_RINGS = "cdk:RelevantRings";

        // **************************************
        // Some predefined property names for * Atoms *
        // **************************************

        /// <summary>
        /// This property will contain an List of Integers. Each
        /// element of the list indicates the size of the ring the given
        /// atom belongs to (if it is a ring atom at all).
        /// </summary>
        public const string RING_SIZES = "cdk:RingSizes";

        /// <summary>
        /// This property indicates how many ring bonds are connected to
        /// the given atom.
        /// </summary>
        public const string RING_CONNECTIONS = "cdk:RingConnections";

        /// <summary>
        /// This property indicate how many bond are present on the atom.
        /// </summary>
        public const string TOTAL_CONNECTIONS = "cdk:TotalConnections";

        /// <summary>
        /// Hydrogen count
        /// </summary>
        public const string TOTAL_H_COUNT = "cdk:TotalHydrogenCount";

        /// <summary>The Isotropic Shielding, usually calculated by
         /// a quantum chemistry program like Gaussian.
         /// This is a property used for calculating NMR chemical
         /// shifts by subtracting the value from the
         /// isotropic shielding value of a standard (e.g. TMS).
         /// </summary>
        public const string ISOTROPIC_SHIELDING = "cdk:IsotropicShielding";

        /// <summary>
        /// A property to indicate IsRestH being true or false. IsRestH is a term
        /// used in RGroup queries: "if this property is applied ('on'), sites labelled
        /// with Rgroup rrr may only be substituted with a member of the Rgroup or with H"
        /// </summary>
        public const string REST_H = "cdk:IsRestH";

        public const string ATOM_ATOM_MAPPING = "cdk:AtomAtomMapping";

        /// <summary>
        /// Atom number/label that can be applied using the Manual Numbering 
        /// Tool in ACD/ChemSketch.
        /// </summary>
        public const string ACDLABS_LABEL = "cdk:ACDLabsAtomLabel";

        /// <summary>
        /// Key to store/fetch CTab Sgroups from Molfiles. Important! - Use at your own risk,
        /// property is transitive and may be removed in future with a more specific accessor.
        /// </summary>
        public const string CTAB_SGROUPS = "cdk:CtabSgroups";


        /// <summary>
        /// Property for reaction objects where the conditions of reactions can be placed.
        /// </summary>
        public const string REACTION_CONDITIONS = "cdk:ReactionConditions";

        // **************************************
        // Some predefined property names for * AtomTypes *
        // **************************************

        /// <summary>Used as property key for indicating the ring size of a certain atom type.</summary>
        public const string PART_OF_RING_OF_SIZE = "cdk:Part of ring of size";

        /// <summary>Used as property key for indicating the chemical group of a certain atom type.</summary>
        public const string CHEMICAL_GROUP_CONSTANT = "cdk:Chemical Group";

        /// <summary>Used as property key for indicating the HOSE code for a certain atom type.</summary>
        public const string SPHERICAL_MATCHER = "cdk:HOSE code spherical matcher";

        /// <summary>Used as property key for indicating the HOSE code for a certain atom type.</summary>
        public const string PI_BOND_COUNT = "cdk:Pi Bond Count";

        /// <summary>Used as property key for indicating the HOSE code for a certain atom type.</summary>
        public const string LONE_PAIR_COUNT = "cdk:Lone Pair Count";

        /// <summary>Used as property key for indicating the number of single electrons on the atom type.</summary>
        public const string SINGLE_ELECTRON_COUNT = "cdk:Lone Pair Count";

        /// <summary>
        /// pack the RGB color space components into a single int.
        /// </summary>
        public const string COLOR = "org.openscience.cdk.renderer." + "color";

        internal static int RGB2Int(int r, int g, int b) => (((r << 16) & 0xff0000) | ((g << 8) & 0x00ff00) | (b & 0x0000ff));
    }
}
