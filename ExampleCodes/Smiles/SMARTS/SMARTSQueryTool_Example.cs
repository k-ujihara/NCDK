using NCDK.Aromaticities;
using NCDK.Graphs;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.Smiles.SMARTS
{
    class SMARTSQueryTool_Example
    {
        void Main()
        {
            {
                #region
                SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
                IAtomContainer atomContainer = sp.ParseSmiles("CC(=O)OC(=O)C");
                SMARTSQueryTool querytool = new SMARTSQueryTool("O=CO", Silent.ChemObjectBuilder.Instance);
                bool status = querytool.Matches(atomContainer);
                if (status)
                {
                    int nmatch = querytool.MatchesCount;
                    IList<IList<int>> mappings = querytool.GetMatchingAtoms();
                    for (int i = 0; i < nmatch; i++)
                    {
                        IList<int> atomIndices = mappings[i];
                    }
                }
                #endregion
            }
            {
                string someSmartsPattern = null;
                IChemObjectSet<IAtomContainer> molecules = null;
                #region SetAromaticity
                SMARTSQueryTool sqt = new SMARTSQueryTool(someSmartsPattern, Default.ChemObjectBuilder.Instance);
                sqt.SetAromaticity(new Aromaticity(ElectronDonation.CDKModel, Cycles.CDKAromaticSetFinder));
                foreach (var molecule in molecules)
                {
                    // CDK Aromatic model needs atom types
                    AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
                    sqt.Matches(molecule);
                }
                #endregion
            }
        }
    }
}
