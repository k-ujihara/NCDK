
namespace NCDK.Tools
{
    class StructureResonanceGenerator_Example
    {
        void Main()
        {
            IAtomContainer molecule = null;
            #region 1
            StructureResonanceGenerator sRG = new StructureResonanceGenerator();
            IChemObjectSet<IAtomContainer> setOf = sRG.GetContainers(molecule);
            #endregion

            #region 2
            molecule.Atoms[0].IsReactiveCenter = true;
            #endregion
        }
    }
}
