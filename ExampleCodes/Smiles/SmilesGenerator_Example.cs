using NCDK.Common.Collections;
using NCDK.Numerics;
using NCDK.Templates;

namespace NCDK.Smiles
{
    class SmilesGenerator_Example
    {
        void Main()
        {
            {
                string smi = null;
                SmilesGenerator sg = null;
                {
                    #region 1
                    IAtomContainer ethanol = TestMoleculeFactory.MakeEthanol();
                    sg = new SmilesGenerator(SmiFlavor.Generic);
                    smi = sg.Create(ethanol); // CCO, C(C)O, C(O)C, or OCC

                    sg = SmilesGenerator.Unique();
                    smi = sg.Create(ethanol); // only CCO
                    #endregion

                    #region 2
                    IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();

                    // 'benzene' molecule has no arom flags, we always get Kekulﾃｩ output
                    sg = new SmilesGenerator(SmiFlavor.Generic);
                    smi = sg.Create(benzene); // C1=CC=CC=C1

                    sg = new SmilesGenerator(SmiFlavor.Generic |
                                                                 SmiFlavor.UseAromaticSymbols);
                    smi = sg.Create(benzene); // C1=CC=CC=C1 flags not set!

                    // Note, in practice we'd use an aromaticity algorithm
                    foreach (IAtom a in benzene.Atoms)
                        a.IsAromatic = true;
                    foreach (IBond b in benzene.Bonds)
                        b.IsAromatic = true;

                    // 'benzene' molecule now has arom flags, we always get aromatic SMILES if we request it
                    sg = new SmilesGenerator(SmiFlavor.Generic);
                    smi = sg.Create(benzene); // C1=CC=CC=C1

                    sg = new SmilesGenerator(SmiFlavor.Generic |
                                                                 SmiFlavor.UseAromaticSymbols);
                    smi = sg.Create(benzene); // c1ccccc1
                    #endregion
                }
            }
            {
                #region 4
                IAtomContainer mol = TestMoleculeFactory.MakeAlphaPinene();
                SmilesGenerator sg = new SmilesGenerator(SmiFlavor.Generic);

                int n = mol.Atoms.Count;
                int[] order = new int[n];

                // the order array is filled up as the SMILES is generated
                string smi = sg.Create(mol, order);

                // load the coordinates array such that they are in the order the atoms
                // are read when parsing the SMILES
                Vector2[] coords = new Vector2[mol.Atoms.Count];
                for (int i = 0; i < coords.Length; i++)
                    coords[order[i]] = mol.Atoms[i].Point2D.Value;

                // SMILES string suffixed by the coordinates
                string smi2d = smi + " " + Arrays.ToJavaString(coords);
                #endregion
            }

            {
                #region WithAtomClasses
                IAtomContainer container = TestMoleculeFactory.MakeAlphaPinene();
                SmilesGenerator smilesGen = SmilesGenerator.Unique().WithAtomClasses();
                smilesGen.CreateSMILES(container); // C[CH2:4]O second atom has class = 4
                #endregion
            }
            {
                #region Create_IAtomContainer_int
                IAtomContainer mol = TestMoleculeFactory.MakeAlphaPinene();
                SmilesGenerator sg = new SmilesGenerator();

                int n = mol.Atoms.Count;
                int[] order = new int[n];

                // the order array is filled up as the SMILES is generated
                string smi = sg.Create(mol, order);

                // load the coordinates array such that they are in the order the atoms
                // are read when parsing the SMILES
                Vector2[] coords = new Vector2[mol.Atoms.Count];
                for (int i = 0; i < coords.Length; i++)
                    coords[order[i]] = mol.Atoms[i].Point2D.Value;

                // SMILES string suffixed by the coordinates
                string smi2d = smi + " " + Arrays.ToJavaString(coords);
                #endregion
            }
            {
                IAtomContainer container = null;
                #region Create_IAtomContainer_int_int
                IAtomContainer mol = TestMoleculeFactory.MakeAlphaPinene();
                SmilesGenerator sg = new SmilesGenerator();

                int n = mol.Atoms.Count;
                int[] order = new int[n];

                // the order array is filled up as the SMILES is generated
                string smi = sg.Create(mol, order);

                // load the coordinates array such that they are in the order the atoms
                // are read when parsing the SMILES
                Vector2[] coords = new Vector2[mol.Atoms.Count];
                for (int i = 0; i < coords.Length; i++)
                    coords[order[i]] = container.Atoms[i].Point2D.Value;

                // SMILES string suffixed by the coordinates
                string smi2d = smi + " " + Arrays.ToJavaString(coords);
                #endregion
            }
        }
    }
}
