namespace NCDK
{
    /// <summary>
    /// Base class for entities containing electrons, like bonds, orbitals, lone-pairs.
    /// </summary>
    public interface IElectronContainer
        : IChemObject
    {
        int? ElectronCount { get; set; }
    }
}
