using NCDK.Silent;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace NCDK.SMSD.Labelling
{
    // @cdk.module smsd
    [Obsolete]
    public class AbstractReactionLabeller
    {
        /// <summary>
        /// A nasty hack necessary to get around a bug in the CDK
        /// </summary>
        private readonly bool fixAtomMappingCastType = false;

        private static void FixAtomMapping(IAtomContainer canonicalForm)
        {
            foreach (var a in canonicalForm.Atoms)
            {
                string v = a.GetProperty<string>(CDKPropertyName.AtomAtomMapping);
                if (v != null)
                {
                    a.SetProperty(CDKPropertyName.AtomAtomMapping, int.Parse(v, NumberFormatInfo.InvariantInfo));
                }
            }
        }

        private static IReadOnlyDictionary<IAtom, IAtom> AtomAtomMap(IReaction reaction, IReaction clone, IReadOnlyDictionary<IAtomContainer, int[]> permutationMap)
        {
            // create a Map of corresponding atoms for molecules
            // (key: original Atom, value: clone Atom)
            var atomAtom = new Dictionary<IAtom, IAtom>();
            var reactants = reaction.Reactants;
            var clonedReactants = clone.Reactants;
            for (int i = 0; i < reactants.Count; ++i)
            {
                var mol = reactants[i];
                var mol2 = clonedReactants[i];
                var permutation = permutationMap[mol2];
                for (int j = 0; j < mol.Atoms.Count; ++j)
                {
                    atomAtom[mol.Atoms[j]] = mol2.Atoms[permutation[j]];
                }
            }
            var products = reaction.Products;
            var clonedProducts = clone.Products;
            for (int i = 0; i < products.Count; ++i)
            {
                var mol = products[i];
                var mol2 = clonedProducts[i];
                var permutation = permutationMap[mol2];
                for (int j = 0; j < mol.Atoms.Count; ++j)
                {
                    atomAtom[mol.Atoms[j]] = mol2.Atoms[permutation[j]];
                }
            }

            foreach (var key in atomAtom.Keys)
            {
                var keyAC = ReactionManipulator.GetRelevantAtomContainer(reaction, key);
                var keyIndex = keyAC.Atoms.IndexOf(key);
                var value = atomAtom[key];
                var valueAC = ReactionManipulator.GetRelevantAtomContainer(clone, value);
                var valueIndex = valueAC.Atoms.IndexOf(value);
                Console.Out.WriteLine("key " + keyIndex + key.Symbol + " mapped to " + valueIndex + value.Symbol);
            }

            return atomAtom;
        }

        private static List<IMapping> CloneMappings(IReaction reaction, IReadOnlyDictionary<IAtom, IAtom> atomAtomMap)
        {
            // clone the mappings
            var numberOfMappings = reaction.Mappings.Count;
            var map = new List<IMapping>();
            for (int mappingIndex = 0; mappingIndex < numberOfMappings; mappingIndex++)
            {
                var mapping = reaction.Mappings[mappingIndex];
                var keyChemObj0 = mapping[0];
                var keyChemObj1 = mapping[1];
                var co0 = (IChemObject)atomAtomMap[(IAtom)keyChemObj0];
                var co1 = (IChemObject)atomAtomMap[(IAtom)keyChemObj1];
                map.Add(new Mapping(co0, co1));
            }
            return map;
        }

        class MappingSorter : IComparer<IMapping>
        {
            private readonly IReadOnlyDictionary<IChemObject, int> indexMap;

            public MappingSorter(IReadOnlyDictionary<IChemObject, int> indexMap)
            {
                this.indexMap = indexMap;
            }

            public int Compare(IMapping o1, IMapping o2)
            {
                var o10 = o1[0];
                var o20 = o2[0];
                return indexMap[o10].CompareTo(indexMap[o20]);
            }
        }

        /// <summary>
        /// Clone and Sort the mappings based on the order of the first object
        /// in the mapping (which is assumed to be the reactant).
        /// </summary>
        private static void CloneAndSortMappings(IReaction reaction, IReaction copyOfReaction,
            IReadOnlyDictionary<IAtomContainer, int[]> permutationMap)
        {
            // make a lookup for the indices of the atoms in the copy
            var indexMap = new Dictionary<IChemObject, int>();
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

            var atomAtomMap = AtomAtomMap(reaction, copyOfReaction, permutationMap);
            var map = CloneMappings(reaction, atomAtomMap);

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
            var canonReaction = new Reaction();

            var permutationMap = new Dictionary<IAtomContainer, int[]>();

            var canonicalProducts = ChemObjectBuilder.Instance.NewChemObjectSet<IAtomContainer>();
            foreach (var product in reaction.Products)
            {
                var canonicalForm = labeller.GetCanonicalMolecule(product);
                if (fixAtomMappingCastType)
                {
                    FixAtomMapping(canonicalForm);
                }
                var canonicalMolecule = canonicalForm.Builder.NewAtomContainer(canonicalForm);
                permutationMap[canonicalMolecule] = labeller.GetCanonicalPermutation(product);
                canonicalProducts.Add(canonicalMolecule);
            }
            var canonicalReactants = ChemObjectBuilder.Instance.NewChemObjectSet<IAtomContainer>();
            foreach (var reactant in reaction.Reactants)
            {
                var canonicalForm = labeller.GetCanonicalMolecule(reactant);
                if (fixAtomMappingCastType)
                {
                    FixAtomMapping(canonicalForm);
                }
                var canonicalMolecule = canonicalForm.Builder.NewAtomContainer(canonicalForm);
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
