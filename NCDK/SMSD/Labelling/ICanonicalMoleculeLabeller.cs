namespace NCDK.SMSD.Labelling
{
    /// <summary>
    // @cdk.module smsd
    // @cdk.githash
    /// </summary>
    public interface ICanonicalMoleculeLabeller
    {
        IAtomContainer GetCanonicalMolecule(IAtomContainer container);
        int[] GetCanonicalPermutation(IAtomContainer container);
    }
}
