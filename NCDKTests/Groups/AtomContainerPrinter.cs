using NCDK.Common.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace NCDK.Groups
{
    // @author maclean
    // @cdk.module test-group
    public class AtomContainerPrinter
    {
        public static void Print(IAtomContainer atomContainer)
        {
            Console.Out.WriteLine(AtomContainerPrinter.ToString(atomContainer));
        }

        public static string ToString(IAtomContainer atomContainer)
        {
            return AtomContainerPrinter.ToString(atomContainer, new Permutation(atomContainer.Atoms.Count));
        }

        public static string ToString(IAtomContainer atomContainer, bool sortEdges)
        {
            Permutation identity = new Permutation(atomContainer.Atoms.Count);
            return ToString(atomContainer, identity, sortEdges);
        }

        public static string ToString(IAtomContainer atomContainer, Permutation permutation)
        {
            return ToString(atomContainer, permutation, false); // don't sort by default?
        }

        public static string ToString(IAtomContainer atomContainer, Permutation permutation, bool sortEdges)
        {
            var sb = new StringBuilder();
            int atomCount = atomContainer.Atoms.Count;
            IAtom[] pAtoms = new IAtom[atomCount];
            {
                for (int i = 0; i < atomCount; i++)
                {
                    pAtoms[permutation[i]] = atomContainer.Atoms[i];
                }
            }
            {
                for (int i = 0; i < atomCount; i++)
                {
                    sb.Append(pAtoms[i].Symbol).Append(i);
                }
            }
            sb.Append(" ");
            List<string> edgeStrings = null;
            {
                int i = 0;
                if (sortEdges)
                {
                    edgeStrings = new List<string>();
                }
                foreach (var bond in atomContainer.Bonds)
                {
                    int a0 = atomContainer.Atoms.IndexOf(bond.Begin);
                    int a1 = atomContainer.Atoms.IndexOf(bond.End);
                    int pA0 = permutation[a0];
                    int pA1 = permutation[a1];
                    char o = BondOrderToChar(bond.Order);
                    if (sortEdges)
                    {
                        string edgeString;
                        if (pA0 < pA1)
                        {
                            edgeString = pA0 + ":" + pA1 + "(" + o + ")";
                        }
                        else
                        {
                            edgeString = pA1 + ":" + pA0 + "(" + o + ")";
                        }
                        edgeStrings.Add(edgeString);
                    }
                    else
                    {
                        if (pA0 < pA1)
                        {
                            sb.Append(pA0 + ":" + pA1 + "(" + o + ")");
                        }
                        else
                        {
                            sb.Append(pA1 + ":" + pA0 + "(" + o + ")");
                        }
                    }
                    if (!sortEdges && i < atomContainer.Bonds.Count - 1)
                    {
                        sb.Append(',');
                    }
                    i++;
                }
            }
            if (sortEdges)
            {
                edgeStrings.Sort();
                int i = 0;
                foreach (var edgeString in edgeStrings)
                {
                    sb.Append(edgeString);
                    if (i < atomContainer.Bonds.Count - 1)
                    {
                        sb.Append(",");
                    }
                }
            }
            return sb.ToString();
        }

        private static char BondOrderToChar(BondOrder order)
        {
            switch (order)
            {
                case BondOrder.Single:
                    return '1';
                case BondOrder.Double:
                    return '2';
                case BondOrder.Triple:
                    return '3';
                case BondOrder.Quadruple:
                    return '4';
                case BondOrder.Unset:
                    return '?';
                default:
                    return '?';
            }
        }

        private static BondOrder charToBondOrder(char orderChar)
        {
            switch (orderChar)
            {
                case '1':
                    return BondOrder.Single;
                case '2':
                    return BondOrder.Double;
                case '3':
                    return BondOrder.Triple;
                case '4':
                    return BondOrder.Quadruple;
                case '?':
                    return BondOrder.Unset;
                default:
                    return BondOrder.Unset;
            }
        }

        public static IAtomContainer FromString(string acpString, IChemObjectBuilder builder)
        {
            int gapIndex = acpString.IndexOf(' ');
            if (gapIndex == -1)
            {
                gapIndex = acpString.Length;
            }

            IAtomContainer atomContainer = builder.NewAtomContainer();
            string elementString = acpString.Substring(0, gapIndex);
            // skip the atom number, as this is just a visual convenience
            for (int index = 0; index < elementString.Length; index += 2)
            {
                string elementSymbol = elementString[index].ToString();
                atomContainer.Atoms.Add(builder.NewAtom(elementSymbol));
            }

            // no bonds
            if (gapIndex >= acpString.Length - 1)
            {
                return atomContainer;
            }

            string bondString = acpString.Substring(gapIndex + 1);
            foreach (var bondPart in Strings.Tokenize(bondString, ','))
            {
                int colonIndex = bondPart.IndexOf(':');
                int openBracketIndex = bondPart.IndexOf('(');
                int closeBracketIndex = bondPart.IndexOf(')');
                int a0 = int.Parse(bondPart.Substring(0, colonIndex));
                int a1 = int.Parse(bondPart.Substring(colonIndex + 1, openBracketIndex - (colonIndex + 1)));
                char o = bondPart.Substring(openBracketIndex + 1, closeBracketIndex - (openBracketIndex + 1))[0];
                atomContainer.AddBond(atomContainer.Atoms[a0], atomContainer.Atoms[a1], charToBondOrder(o));
            }
            return atomContainer;
        }
    }
}
