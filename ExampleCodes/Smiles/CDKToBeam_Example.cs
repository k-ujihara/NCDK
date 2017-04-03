using NCDK.Templates;

namespace NCDK.Smiles
{
    class CDKToBeam_Example
    {
        void Main()
        {
            IAtomContainer m = TestMoleculeFactory.MakeBenzene();

            // converter is thread-safe and can be used by multiple threads
            CDKToBeam c2g = new CDKToBeam();
            Beam.Graph g = c2g.ToBeamGraph(m);

            // get the SMILES notation from the Beam graph
            string smi = g.ToSmiles();
        }
    }
}
