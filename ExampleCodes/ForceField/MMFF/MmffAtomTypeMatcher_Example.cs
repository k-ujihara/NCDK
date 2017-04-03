namespace NCDK.ForceField.MMFF
{
    class MmffAtomTypeMatcher_Example
    {
        void Main()
        {
            IAtomContainerSet<IAtomContainer> containers = null;
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
