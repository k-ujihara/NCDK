using System.Collections.Generic;

namespace FaulonSignatures.Chemistry
{
    public class ChiralCenterFinder
    {
        public static List<int> FindTetrahedralChiralCenters(Molecule molecule)
        {
            List<int> chiralCenterIndices = new List<int>();
            MoleculeSignature molSig = new MoleculeSignature(molecule);
            List<string> signatureStrings = molSig.GetVertexSignatureStrings();
            for (int i = 0; i < molecule.GetAtomCount(); i++)
            {
                int[] connected = molecule.GetConnected(i);
                if (connected.Length < 4)
                {
                    continue;
                }
                else
                {
                    string s0 = signatureStrings[connected[0]];
                    string s1 = signatureStrings[connected[1]];
                    string s2 = signatureStrings[connected[2]];
                    string s3 = signatureStrings[connected[3]];
                    if (s0.Equals(s1)
                     || s0.Equals(s2)
                     || s0.Equals(s3)
                     || s1.Equals(s2)
                     || s1.Equals(s3)
                     || s2.Equals(s3))
                    {
                        continue;
                    }
                    else
                    {
                        chiralCenterIndices.Add(i);
                    }
                }
            }

            return chiralCenterIndices;
        }
    }
}
