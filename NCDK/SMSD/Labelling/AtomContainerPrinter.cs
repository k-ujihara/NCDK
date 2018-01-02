using System;
using System.Collections.Generic;
using System.Text;

namespace NCDK.SMSD.Labelling
{
    // @cdk.module smsd
    // @cdk.githash
    public class AtomContainerPrinter
    {
        private class Edge : IComparable<Edge>
        {
            public string firstString;
            public string lastString;
            public int first;
            public int last;
            public int order;

            public Edge(int first, int last, int order, string firstString, string lastString)
            {
                this.first = first;
                this.last = last;
                this.order = order;
                this.firstString = firstString;
                this.lastString = lastString;
            }

            public int CompareTo(Edge o)
            {
                if (first < o.first || (first == o.first && last < o.last))
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }

            public override string ToString()
            {
                return firstString + first + ":" + lastString + last + "(" + order + ")";
            }
        }

        public string ToString(IAtomContainer atomContainer)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var atom in atomContainer.Atoms)
            {
                sb.Append(atom.Symbol);
            }
            sb.Append(' ');
            List<Edge> edges = new List<Edge>();
            foreach (var bond in atomContainer.Bonds)
            {
                IAtom a0 = bond.Atoms[0];
                IAtom a1 = bond.Atoms[1];
                int a0N = atomContainer.Atoms.IndexOf(a0);
                int a1N = atomContainer.Atoms.IndexOf(a1);
                string a0S = a0.Symbol;
                string a1S = a1.Symbol;
                int o = bond.Order.Numeric();
                if (a0N < a1N)
                {
                    edges.Add(new Edge(a0N, a1N, o, a0S, a1S));
                }
                else
                {
                    edges.Add(new Edge(a1N, a0N, o, a1S, a0S));
                }
            }
            edges.Sort();
            sb.Append(edges.ToString());
            return sb.ToString();
        }
    }
}
