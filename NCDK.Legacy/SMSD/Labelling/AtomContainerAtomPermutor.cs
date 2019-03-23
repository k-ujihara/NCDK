using System;
using System.Collections;
using System.Collections.Generic;

namespace NCDK.SMSD.Labelling
{
    // @cdk.module smsd
    [Obsolete]
    public class AtomContainerAtomPermutor : Permutor, IEnumerable<IAtomContainer>
    {
        private readonly IAtomContainer original;

        public AtomContainerAtomPermutor(IAtomContainer atomContainer)
            : base(atomContainer.Atoms.Count)
        {
            original = atomContainer;
        }

        public IEnumerator<IAtomContainer> GetEnumerator()
        {
            while (base.HasNext())
            {
                int[] p = base.GetNextPermutation();
                yield return AtomContainerAtomPermutor.Permute(p, original);
            }
            yield break;
        }

        public static IAtomContainer Permute(int[] p, IAtomContainer atomContainer)
        {
            bool useA = false;
            if (useA)
            {
                return PermuteA(p, atomContainer);
            }
            else
            {
                return PermuteB(p, atomContainer);
            }
        }

        private static IAtomContainer PermuteA(int[] p, IAtomContainer atomContainer)
        {
            IAtomContainer permutedContainer = null;
            permutedContainer = atomContainer.Builder.NewAtomContainer();
            foreach (var pp in p)
            {
                IAtom atom = atomContainer.Atoms[pp];
                permutedContainer.Atoms.Add((IAtom)atom.Clone());
            }
            foreach (var bond in atomContainer.Bonds)
            {
                IBond clonedBond = (IBond)bond.Clone();
                clonedBond.SetAtoms(new IAtom[clonedBond.Atoms.Count]);
                int i = 0;
                foreach (var atom in bond.Atoms)
                {
                    int index = atomContainer.Atoms.IndexOf(atom);
                    IAtom permutedAtom = permutedContainer.Atoms[p[index]];
                    clonedBond.Atoms[i++] = permutedAtom;
                }
                permutedContainer.Bonds.Add(clonedBond);
            }
            return permutedContainer;
        }

        private static IAtomContainer PermuteB(int[] p, IAtomContainer atomContainer)
        {
            IAtomContainer permutedContainer = null;
            permutedContainer = (IAtomContainer)atomContainer.Clone();
            int n = atomContainer.Atoms.Count;
            IAtom[] permutedAtoms = new IAtom[n];
            for (int originalIndex = 0; originalIndex < n; originalIndex++)
            {
                // get the newly cloned atom
                IAtom atom = permutedContainer.Atoms[originalIndex];

                // permute the index
                int newIndex = p[originalIndex];

                // put the atom in the new place
                permutedAtoms[newIndex] = atom;
            }
            permutedContainer.SetAtoms(permutedAtoms);
            return permutedContainer;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
