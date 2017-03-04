using FaulonSignatures;
using static FaulonSignatures.Chemistry.Molecule;

namespace FaulonSignatures.Chemistry
{
    public class MoleculeBuilder : AbstractGraphBuilder
    {
        private Molecule molecule;

        public MoleculeBuilder()
            : base()
        { }

        public override void MakeEdge(
                int vertexIndex1, int vertexIndex2,
                string symbolA, string symbolB, string edgeLabel)
        {
            if (edgeLabel.Equals(""))
            {
                this.molecule.AddBond(vertexIndex1, vertexIndex2, BondOrder.Single);
            }
            else if (edgeLabel.Equals("="))
            {
                this.molecule.AddBond(vertexIndex1, vertexIndex2, BondOrder.Double);
            }
            else if (edgeLabel.Equals("#"))
            {
                this.molecule.AddBond(vertexIndex1, vertexIndex2, BondOrder.Triple);
            }
        }

        public override void MakeGraph()
        {
            this.molecule = new Molecule();
        }

        public override void MakeVertex(string label)
        {
            this.molecule.AddAtom(label);
        }

        public Molecule FromTree(ColoredTree tree)
        {
            base.MakeFromColoredTree(tree);
            return this.molecule;
        }

        public Molecule Molecule => this.molecule;
    }
}
