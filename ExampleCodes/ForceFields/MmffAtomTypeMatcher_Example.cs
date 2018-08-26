namespace NCDK.ForceFields
{
    static class MmffAtomTypeMatcher_Example
    {
        static void Main()
        {
            IChemObjectSet<IAtomContainer> containers = null;
            #region
            MmffAtomTypeMatcher mmffAtomTypes = new MmffAtomTypeMatcher();
            foreach (var container in containers)
            {
                string[] symbs = mmffAtomTypes.SymbolicTypes(container);
            }
            #endregion
        }
    }
}
