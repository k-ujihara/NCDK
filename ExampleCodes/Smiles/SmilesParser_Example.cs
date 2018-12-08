namespace NCDK.Smiles
{
    class SmilesParser_Example
    {
        void Main()
        {
            {
                #region 1
                SmilesParser sp = new SmilesParser();
                IAtomContainer m = sp.ParseSmiles("c1[cH:5]cccc1");
                var c1 = m.Atoms[1].GetProperty<int>(CDKPropertyName.AtomAtomMapping); // 5
                var c2 = m.Atoms[2].GetProperty<int>(CDKPropertyName.AtomAtomMapping); // null
                #endregion
            }
            {
                #region 2
                SmilesParser sp = new SmilesParser();
                IAtomContainer m = sp.ParseSmiles("c1[cH:5]cccc1");
                var c1 = m.Atoms[1].GetProperty<int>(CDKPropertyName.AtomAtomMapping); // 5
                var c2 = m.Atoms[2].GetProperty<int>(CDKPropertyName.AtomAtomMapping); // null
                #endregion
            }
        }
    }
}
