namespace NCDK.Depict
{
    class Abbreviations_Example
    {
        public void Main()
        {
            IAtomContainer mol = null;
            #region
            Abbreviations abrv = new Abbreviations
            {
                // add some abbreviations, when overlapping (e.g. Me,Et,tBu) first one wins
                "[Na+].[H-] NaH",
                "*c1ccccc1 Ph",
                "*C(C)(C)C tBu",
                "*CC Et",
                "*C Me"
            };
            // maybe we don't want 'Me' in the depiction
            abrv.SetEnabled("Me", false);
            // assign abbreviations with some filters
            int numAdded = abrv.Apply(mol);
            // generate all but don't assign, need to be added manually
            // set/update the CDKPropertyName.CtabSgroups property of mol
            var sgroups = abrv.Generate(mol);
            #endregion

            #region 1
            // https://www.github.com/openbabel/superatoms
            abrv.LoadFromFile("obabel_superatoms.smi");
            #endregion
        }
    }
}
