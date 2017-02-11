namespace NCDK
{
    public interface IMolecularEntity
    {
        bool IsAromatic { get; set; }
        bool IsInRing { get; set; }
        bool IsAliphatic { get; set; }
    }

    public interface IAtomType
        : IIsotope, IMolecularEntity
    {
        string AtomTypeName { get; set; }
        BondOrder MaxBondOrder { get; set; }
        double? BondOrderSum { get; set; }
        int? FormalCharge { get; set; }
        int? FormalNeighbourCount { get; set; }
        Hybridization Hybridization { get; set; }
        double? CovalentRadius { get; set; }
        int? Valency { get; set; }

        bool IsHydrogenBondDonor { get; set; }
        bool IsHydrogenBondAcceptor { get; set; }

        /// <summary>
        /// set if a chemobject has reactive center. It is used for example in reaction.
        /// </summary>
        bool IsReactiveCenter { get; set; }
    }
}
