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
                SmilesGenerator sg = null;
                string smi = null;
                {
                    #region 1
                    IAtomContainer ethanol = TestMoleculeFactory.MakeEthanol();

                    sg = SmilesGenerator.Generic();
                    smi = sg.Create(ethanol); // CCO or OCC

                    sg = SmilesGenerator.Unique();
                    smi = sg.Create(ethanol); // only CCO
                    #endregion

                    #region 2
                    IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();

                    // with no flags set the output is always kekule
                    sg = SmilesGenerator.Generic();
                    smi = sg.Create(benzene); // C1=CC=CC=C1

                    sg = SmilesGenerator.Generic().Aromatic();
                    smi = sg.Create(ethanol); // C1=CC=CC=C1

                    foreach (var a in benzene.Atoms)
                        a.IsAromatic = true;
                    foreach (var b in benzene.Bonds)
                        b.IsAromatic = true;

                    // with flags set, the aromatic generator encodes this information
                    sg = SmilesGenerator.Generic();
                    smi = sg.Create(benzene); // C1=CC=CC=C1

                    sg = SmilesGenerator.Generic()
                                                             .Aromatic();
                    smi = sg.Create(ethanol); // c1ccccc1
                    #endregion

                    #region 3
                    // see CDKConstants for property key
                    benzene.Atoms[3].SetProperty(CDKPropertyName.AtomAtomMapping, 42);

                    sg = SmilesGenerator.Generic();
                    smi = sg.Create(benzene); // C1=CC=CC=C1

                    sg = SmilesGenerator.Generic().WithAtomClasses();
                    smi = sg.Create(ethanol); // C1=CC=[CH:42]C=C1
                    #endregion
                }
            }
            {
                #region 4
                IAtomContainer mol = TestMoleculeFactory.MakeAlphaPinene();
                SmilesGenerator sg = SmilesGenerator.Generic();

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
                #region Aromatic
                IAtomContainer container = TestMoleculeFactory.MakeAlphaPinene();
                SmilesGenerator smilesGen = SmilesGenerator.Unique().Aromatic();
                smilesGen.CreateSMILES(container);
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
                #region
                 IAtomContainer  mol = TestMoleculeFactory.MakeAlphaPinene();
                SmilesGenerator sg  = SmilesGenerator.Generic();
                
                 int   n     = mol.Atoms.Count;
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
        }
    }
}
