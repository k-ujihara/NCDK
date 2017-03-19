namespace NCDK.SMSD.Labelling
{
    // @cdk.module smsd
    // @cdk.githash
    public interface ICanonicalMoleculeLabeller
    {
        IAtomContainer GetCanonicalMolecule(IAtomContainer container);
        int[] GetCanonicalPermutation(IAtomContainer container);
    }
}
