using NCDK.Templates;
using System.Collections.Generic;

namespace NCDK.Signatures
{
    class MoleculeSignature_Example
    {
        void Main()
        {
            #region
             IAtomContainer diamantane = TestMoleculeFactory.MakeBenzene();
             MoleculeSignature moleculeSignature = new MoleculeSignature(diamantane);
             string canonicalSignature = moleculeSignature.ToCanonicalString();
            
             // to get the orbits of this molecule
            IList<Orbit> orbits = moleculeSignature.CalculateOrbits();
            
             // and to get the height-2 signature string of just atom 5:
             string hSignatureForAtom5 = moleculeSignature.SignatureStringForVertex(5, 2);
            
           #endregion
        }
    }
}
