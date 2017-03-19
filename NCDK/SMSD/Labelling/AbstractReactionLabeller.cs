using NCDK.Default;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;

namespace NCDK.SMSD.Labelling
{
    // @cdk.module smsd
    // @cdk.githash
    public class AbstractReactionLabeller
    {
        /// <summary>
        /// A nasty hack necessary to get around a bug in the CDK
        /// </summary>
        private bool fixAtomMappingCastType = false;

        private void FixAtomMapping(IAtomContainer canonicalForm)
        {
            foreach (var a in canonicalForm.Atoms)
            {
                string v = a.GetProperty<string>(CDKPropertyName.AtomAtomMapping);
                if (v != null)
                {
                    a.SetProperty(CDKPropertyName.AtomAtomMapping, int.Parse(v));
                }
            }
        }

        private IDictionary<IAtom, IAtom> AtomAtomMap(IReaction reaction, IReaction clone, IDictionary<IAtomContainer, int[]> permutationMap)
        {
            // create a Map of corresponding atoms for molecules
            // (key: original Atom, value: clone Atom)
            IDictionary<IAtom, IAtom> atomAtom = new Dictionary<IAtom, IAtom>();
            var reactants = reaction.Reactants;
            var clonedReactants = clone.Reactants;
            for (int i = 0; i < reactants.Count; ++i)
            {
                IAtomContainer mol = reactants[i];
                IAtomContainer mol2 = clonedReactants[i];
                int[] permutation = permutationMap[mol2];
                for (int j = 0; j < mol.Atoms.Count; ++j)
                {
                    atomAtom[mol.Atoms[j]] = mol2.Atoms[permutation[j]];
                }
            }
            var products = reaction.Products;
            var clonedProducts = clone.Products;
            for (int i = 0; i < products.Count; ++i)
            {
                IAtomContainer mol = products[i];
                IAtomContainer mol2 = clonedProducts[i];
                int[] permutation = permutationMap[mol2];
                for (int j = 0; j < mol.Atoms.Count; ++j)
                {
                    atomAtom[mol.Atoms[j]] = mol2.Atoms[permutation[j]];
                }
            }

            foreach (var key in atomAtom.Keys)
            {
                IAtomContainer keyAC = ReactionManipulator.GetRelevantAtomContainer(reaction, key);
                int keyIndex = keyAC.Atoms.IndexOf(key);
                IAtom value = atomAtom[key];
                IAtomContainer valueAC = ReactionManipulator.GetRelevantAtomContainer(clone, value);
                int valueIndex = valueAC.Atoms.IndexOf(value);
                Console.Out.WriteLine("key " + keyIndex + key.Symbol + " mapped to " + valueIndex + value.Symbol);
            }

            return atomAtom;
        }

        private List<IMapping> CloneMappings(IReaction reaction, IDictionary<IAtom, IAtom> atomAtomMap)
        {
            // clone the mappings
            int numberOfMappings = reaction.Mappings.Count;
            List<IMapping> map = new List<IMapping>();
            for (int mappingIndex = 0; mappingIndex < numberOfMappings; mappingIndex++)
            {
                IMapping mapping = reaction.Mappings[mappingIndex];
                IChemObject keyChemObj0 = mapping[0];
                IChemObject keyChemObj1 = mapping[1];
                IChemObject co0 = (IChemObject)atomAtomMap[(IAtom)keyChemObj0];
                IChemObject co1 = (IChemObject)atomAtomMap[(IAtom)keyChemObj1];
                map.Add(new Mapping(co0, co1));
            }
            return map;
        }

        class MappingSorter : IComparer<IMapping>
        {
            IDictionary<IChemObject, int> indexMap;

            public MappingSorter(IDictionary<IChemObject, int> indexMap)
            {
                this.indexMap = indexMap;
            }

            public int Compare(IMapping o1, IMapping o2)
            {
                IChemObject o10 = o1[0];
                IChemObject o20 = o2[0];
                return indexMap[o10].CompareTo(indexMap[o20]);
            }
        }

        /// <summary>
        /// Clone and Sort the mappings based on the order of the first object
        /// in the mapping (which is assumed to be the reactant).
        /// </summary>
        /// <param name="reaction"></param>
        private void CloneAndSortMappings(IReaction reaction, IReaction copyOfReaction,
            IDictionary<IAtomContainer, int[]> permutationMap)
        {

            // make a lookup for the indices of the atoms in the copy
            IDictionary<IChemObject, int> indexMap = new Dictionary<IChemObject, int>();
            var all = ReactionManipulator.GetAllAtomContainers(copyOfReaction);
            int globalIndex = 0;
            foreach (var ac in all)
            {
                foreach (var atom in ac.Atoms)
                {
                    indexMap[atom] = globalIndex;
                    globalIndex++;
                }
            }

            IDictionary<IAtom, IAtom> atomAtomMap = AtomAtomMap(reaction, copyOfReaction, permutationMap);
            List<IMapping> map = CloneMappings(reaction, atomAtomMap);

            var mappingSorter = new MappingSorter(indexMap);
            map.Sort(mappingSorter);
            int mappingIndex = 0;
            foreach (var mapping in map)
            {
                mapping[0].SetProperty(CDKPropertyName.AtomAtomMapping, mappingIndex);
                mapping[1].SetProperty(CDKPropertyName.AtomAtomMapping, mappingIndex);
                copyOfReaction.Mappings.Add(mapping);
                mappingIndex++;
            }
        }

        public IReaction LabelReaction(IReaction reaction, ICanonicalMoleculeLabeller labeller)
        {
            Console.Out.WriteLine("labelling");
            IReaction canonReaction = new Reaction();

            IDictionary<IAtomContainer, int[]> permutationMap = new Dictionary<IAtomContainer, int[]>();

            IAtomContainerSet<IAtomContainer> canonicalProducts = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet<IAtomContainer>();
            foreach (var product in reaction.Products)
            {
                IAtomContainer canonicalForm = labeller.GetCanonicalMolecule(product);
                if (fixAtomMappingCastType)
                {
                    FixAtomMapping(canonicalForm);
                }
                IAtomContainer canonicalMolecule = canonicalForm.Builder.CreateAtomContainer(canonicalForm);
                permutationMap[canonicalMolecule] = labeller.GetCanonicalPermutation(product);
                canonicalProducts.Add(canonicalMolecule);
            }
            IAtomContainerSet<IAtomContainer> canonicalReactants = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet<IAtomContainer>();
            foreach (var reactant in reaction.Reactants)
            {
                IAtomContainer canonicalForm = labeller.GetCanonicalMolecule(reactant);
                if (fixAtomMappingCastType)
                {
                    FixAtomMapping(canonicalForm);
                }
                IAtomContainer canonicalMolecule = canonicalForm.Builder.CreateAtomContainer(canonicalForm);
                permutationMap[canonicalMolecule] = labeller.GetCanonicalPermutation(reactant);
                canonicalReactants.Add(canonicalMolecule);
            }
            canonReaction.Products.AddRange(canonicalProducts);
            canonReaction.Reactants.AddRange(canonicalReactants);
            CloneAndSortMappings(reaction, canonReaction, permutationMap);
            return canonReaction;
        }
    }
}
