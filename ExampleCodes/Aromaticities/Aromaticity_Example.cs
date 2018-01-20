using NCDK.Graphs;
using NCDK.Templates;
using NCDK.Tools.Manipulator;
using System.Linq;

namespace NCDK.Aromaticities
{
    class Aromaticity_Example
    {
        public static void Main(string[] args)
        {
            {
                var molecules = new Silent.AtomContainerSet();
                #region 
                ElectronDonation model = ElectronDonation.DaylightModel;
                ICycleFinder cycles = Cycles.Or(Cycles.AllFinder, Cycles.GetAllFinder(6));
                Aromaticity aromaticity = new Aromaticity(model, cycles);

                // apply our configured model to each molecule
                foreach (IAtomContainer molecule in molecules)
                {
                    aromaticity.Apply(molecule);
                }
                #endregion
            }
            {
                #region ctor
                // mimics the CDKHuckelAromaticityDetector
                Aromaticity aromaticity_cdk = new Aromaticity(ElectronDonation.CDKModel, Cycles.CDKAromaticSetFinder);
                // mimics the DoubleBondAcceptingAromaticityDetector
                Aromaticity aromaticity_exo = new Aromaticity(ElectronDonation.CDKAllowingExocyclicModel, Cycles.CDKAromaticSetFinder);
                // a good model for writing SMILES
                Aromaticity aromaticity_smi = new Aromaticity(ElectronDonation.DaylightModel, Cycles.AllFinder);
                // a good model for writing MDL/Mol2
                Aromaticity aromaticity_mdl = new Aromaticity(ElectronDonation.PiBondsModel, Cycles.AllFinder);
                #endregion
            }
            {
                #region FindBonds
                Aromaticity aromaticity = new Aromaticity(ElectronDonation.CDKModel, Cycles.AllFinder);
                IAtomContainer container = TestMoleculeFactory.MakeAnthracene();
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(container);
                try
                {
                    var bonds = aromaticity.FindBonds(container);
                    int nAromaticBonds = bonds.Count();
                }
                catch (CDKException)
                {
                    // cycle computation was intractable
                }
                #endregion
            }
            {
                #region Apply
                Aromaticity aromaticity = new Aromaticity(ElectronDonation.CDKModel, Cycles.AllFinder);
                IAtomContainer container = TestMoleculeFactory.MakeAnthracene();
                try
                {
                    if (aromaticity.Apply(container))
                    {
                        //
                    }
                }
                catch (CDKException)
                {
                    // cycle computation was intractable
                }
                #endregion
            }
            {
                #region CDKLegacy_CDKAromaticSetFinder
                new Aromaticity(ElectronDonation.CDKModel, Cycles.CDKAromaticSetFinder);
                #endregion
            }
            {
                #region CDKLegacy_AllFinder_RelevantFinder
                new Aromaticity(ElectronDonation.CDKModel, Cycles.Or(Cycles.AllFinder, Cycles.RelevantFinder));
                #endregion
            }
        }
    }
}
