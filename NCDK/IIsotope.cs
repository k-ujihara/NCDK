using System;

namespace NCDK
{
    public interface IIsotope
        : IElement
    {
        double? NaturalAbundance { get; set; }
        double? ExactMass { get; set; }
        int? MassNumber { get; set; }
    }
}
