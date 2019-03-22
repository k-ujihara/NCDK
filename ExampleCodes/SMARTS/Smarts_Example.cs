using NCDK.Isomorphisms.Matchers;
using NCDK.Smiles;

namespace NCDK.SMARTS
{
    class Smarts_Example
    {
        static void Main()
        {
            IAtomContainer mol = null;
            #region 1
            if (Smarts.Parse(mol, "[aD3]a-a([aD3])[aD3]"))
            {
                var smarts = Smarts.Generate(mol);
            }
            #endregion

            {
                #region GenerateAtom
                var expr = new Expr(ExprType.Degree, 4).And(
                           new Expr(ExprType.IsAromatic));
                var aExpr = Smarts.GenerateAtom(expr);
                // aExpr = "[D4a]"
                #endregion
            }

            {
                #region GenerateBond
                var expr = new Expr(ExprType.True);
                var bExpr = Smarts.GenerateBond(expr);
                // // bExpr='~'
                #endregion
            }

            {
                #region Generate
                var qatom1 = new QueryAtom();
                var qatom2 = new QueryAtom();
                var qbond = new QueryBond();
                qatom1.Expression = new Expr(ExprType.IsAromatic);
                qatom2.Expression = new Expr(ExprType.IsAromatic);
                qbond.Expression = new Expr(ExprType.IsAliphatic);
                qbond.SetAtoms(new IAtom[] { qatom1, qatom2 });
                mol.Atoms.Add(qatom1);
                mol.Atoms.Add(qatom2);
                mol.Bonds.Add(qbond);
                var smartsStr = Smarts.Generate(mol);
                // smartsStr = 'a!:a'
                #endregion
            }
        }
    }
}
