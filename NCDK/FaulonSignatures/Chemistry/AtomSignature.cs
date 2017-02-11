using static FaulonSignatures.Chemistry.Molecule;

namespace FaulonSignatures.Chemistry
{
    public class AtomSignature : AbstractVertexSignature
    {
        private Molecule molecule;

        public AtomSignature(Molecule molecule, int atomNumber)
            : base()
        {
            this.molecule = molecule;
            this.CreateMaximumHeight(atomNumber, molecule.GetAtomCount());
        }

        public AtomSignature(Molecule molecule, int atomNumber, int height)
            : base()
        {
            this.molecule = molecule;
            this.Create(atomNumber, molecule.GetAtomCount(), height);
        }

        public AtomSignature(Molecule molecule, int atomNumber,
                int height, AbstractVertexSignature.InvariantType invariantType)
            : base(invariantType)
        {
            this.molecule = molecule;
            this.Create(atomNumber, molecule.GetAtomCount(), height);
        }

        public override int GetIntLabel(int vertexIndex)
        {
            string symbol = GetVertexSymbol(vertexIndex);

            // not exactly comprehensive...
            if (symbol.Equals("H"))
            {
                return 1;
            }
            else if (symbol.Equals("C"))
            {
                return 12;
            }
            else if (symbol.Equals("O"))
            {
                return 16;
            }
            else
            {
                return -1;
            }
        }

        public override int[] GetConnected(int vertexIndex)
        {
            return this.molecule.GetConnected(vertexIndex);
        }

        public override string GetEdgeLabel(int vertexIndex, int otherVertexIndex)
        {
            BondOrder bondOrder =
                molecule.GetBondOrder(vertexIndex, otherVertexIndex);
            switch (bondOrder)
            {
                case BondOrder.Single: return "";
                case BondOrder.Double: return "=";
                case BondOrder.Triple: return "#";
                case BondOrder.Aromatic: return "p";
                default: return "";
            }
        }

        public override string GetVertexSymbol(int vertexIndex)
        {
            return this.molecule.GetSymbolFor(vertexIndex);
        }

        public override int ConvertEdgeLabelToColor(string label)
        {
            if (label.Equals("-"))
            {
                return 1;
            }
            else if (label.Equals("="))
            {
                return 2;
            }
            else if (label.Equals("#"))
            {
                return 3;
            }
            return 1;
        }
    }
}
