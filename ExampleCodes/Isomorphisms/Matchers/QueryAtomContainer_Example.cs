namespace NCDK.Isomorphisms.Matchers
{
    class QueryAtomContainer_Example
    {
        void Main()
        {
            IAtomContainer mol = null;
            #region Create1
            // [nH]1ccc(=O)cc1 => n1:c:c:c(=O):c:c:1
            QueryAtomContainer.Create(mol,
                ExprType.AliphaticElement,
                ExprType.AromaticElement,
                ExprType.SingleOrAromatic,
                ExprType.AliphaticOrder,
                ExprType.Stereochemistry);
            #endregion
            #region Create2
            // [nH]1ccc(=O)cc1 => [nD2]1:[cD2]:[cD2]:[cD2](=[OD1]):[cD2]:[cD2]:1
            QueryAtomContainer.Create(mol,
                ExprType.AliphaticElement,
                ExprType.AromaticElement,
                ExprType.Degree,
                ExprType.SingleOrAromatic,
                ExprType.AliphaticOrder);
            #endregion
            #region Create3
            // [nH]1ccc(=O)cc1 => [nx2+0]1:[cx2+0]:[cx2+0]:[cx2+0](=[O&x0+0]):[cx2+0]:[cx2+0]:1
            // IMPORTANT! use Cycles.MarkRingAtomsAndBonds(mol) to set ring status
            QueryAtomContainer.Create(mol,
                ExprType.AliphaticElement,
                ExprType.AromaticElement,
                ExprType.FormalCharge,
                ExprType.Isotope,
                ExprType.RingBondCount,
                ExprType.SingleOrAromatic,
                ExprType.AliphaticOrder);
            #endregion
            #region Create4
            // [nH]1ccc(=O)cc1 => [0n+0]1:[0c+0]:[0c+0]:[0c+0](=[O+0]):[0c+0]:[0c+0]:1
            QueryAtomContainer.Create(mol,
                ExprType.AliphaticElement,
                ExprType.AromaticElement,
                ExprType.FormalCharge,
                ExprType.Isotope,
                ExprType.RingBondCount,
                ExprType.SingleOrAromatic,
                ExprType.AliphaticOrder);
            #endregion
        }
    }
}
