using System.Collections.Generic;
using System.Text;

namespace NCDK.Isomorphisms
{
    class Mappings_Example
    {
        void Main()
        {
            IAtomContainer queryStructure = null;
            IAtomContainer targetStructure = null;
            {
                #region 1
                IAtomContainer query = queryStructure;
                IAtomContainer target = targetStructure;

                Mappings mappings = Pattern.CreateSubstructureFinder(query).MatchAll(target);
                #endregion

                #region enum_mappings
                foreach (int[] p in mappings)
                {
                    for (int i = 0; i < p.Length; i++)
                    {
                        // query.Atoms[i] is mapped to target.Atoms[p[i]];
                    }
                }
                #endregion

                #region stereochemistry
                foreach (int[] p in mappings.GetStereochemistry())
                {
                    // ...
                }
                #endregion

                #region unique_matches
                foreach (int[] p in mappings.GetUniqueAtoms())
                {
                    // ...
                }

                foreach (int[] p in mappings.GetUniqueBonds())
                {
                    // ...
                }
                #endregion

                {
                    #region toarray
                    int[][] ps = mappings.ToArray();
                    foreach (int[] p in ps)
                    {
                        // ...
                    }
                    #endregion
                }

                {
                    #region limit_matches
                    // first ten matches
                    foreach (int[] p in mappings.Limit(10))
                    {
                        // ...
                    }

                    // first 10 unique matches
                    foreach (int[] p in mappings.GetUniqueAtoms().Limit(10))
                    {
                        // ...
                    }

                    // ensure we don't waste memory and only 'fix' up to 100 unique matches
                    int[][] ps = mappings.GetUniqueAtoms().Limit(100).ToArray();
                    #endregion
                }

                #region all
                // first 100 unique matches
                Mappings m1 = mappings.GetUniqueAtoms().Limit(100);

                // unique matches in the first 100 matches
                Mappings m2 = mappings.Limit(100).GetUniqueAtoms();

                // first 10 unique matches in the first 100 matches
                Mappings m3 = mappings.Limit(100).GetUniqueAtoms().Limit(10);

                // number of unique atom matches
                int n1 = mappings.CountUnique();

                // number of unique atom matches with correct stereochemistry
                int n2 = mappings.GetStereochemistry().CountUnique();
                #endregion
            }

            {
                #region Filter
                IAtomContainer query = queryStructure;
                IAtomContainer target = targetStructure;

                // obtain only the mappings where the first atom in the query is
                // mapped to the first atom in the target
                Mappings mappings = Pattern.CreateSubstructureFinder(query)
                    .MatchAll(target)
                    .Filter(input => input[0] == 0);
                #endregion
            }

            {
                #region GetMapping
                IAtomContainer query = queryStructure;
                IAtomContainer target = targetStructure;

                Mappings mappings = Pattern.CreateSubstructureFinder(query).MatchAll(target);
                // a string that indicates the mapping of atom elements and numbers
                IEnumerable<string> strs = mappings.GetMapping(
                    input =>
                    {
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < input.Length; i++)
                        {
                            if (i > 0) sb.Append(", ");
                            sb.Append(query.Atoms[i])
                               .Append(i + 1)
                               .Append(" -> ")
                               .Append(target.Atoms[input[i]])
                               .Append(input[i] + 1);
                        }
                        return sb.ToString();
                    });
                #endregion
            }

            {
                #region ToArray1
                IAtomContainer query = queryStructure;
                IAtomContainer target = targetStructure;

                Pattern pat = Pattern.CreateSubstructureFinder(query);

                // lazily iterator
                foreach (int[] mapping in pat.MatchAll(target))
                {
                    // logic...
                }

                int[][] mappings = pat.MatchAll(target).ToArray();

                // same as lazy iterator but we now can refer to and parse 'mappings'
                // to other methods without regenerating the graph match
                foreach (int[] mapping in mappings)
                {
                    // logic...
                }
                #endregion
            }

            {
                #region ToArray2
                IAtomContainer query = queryStructure;
                IAtomContainer target = targetStructure;

                Pattern pat = Pattern.CreateSubstructureFinder(query);

                // array of the first 5 unique atom mappings
                int[][] mappings = pat.MatchAll(target)
                                      .GetUniqueAtoms()
                                      .Limit(5)
                                      .ToArray();
                #endregion
            }

            {
                Mappings mappings = null;
                #region ToAtomMap
                foreach (var map in mappings.ToAtomMaps())
                {
                    foreach (var e in map)
                    {
                        IAtom queryAtom = e.Key;
                        IAtom targetAtom = e.Value;
                    }
                }
                #endregion
            }

            {
                Mappings mappings = null;
                #region ToBondMap
                foreach (var map in mappings.ToBondMaps())
                {
                    foreach (var e in map)
                    {
                        IBond queryBond = e.Key;
                        IBond targetBond = e.Value;
                    }
                }
                #endregion
            }

            {
                IAtomContainer query = queryStructure;
                Mappings mappings = null;
                int i = 0;
                #region ToAtomBondMap
                foreach (var map in mappings.ToAtomBondMaps())
                {
                    foreach (var e in map)
                    {
                        IChemObject queryObj = e.Key;
                        IChemObject targetObj = e.Value;
                    }
                    IAtom matchedAtom = (IAtom)map[query.Atoms[i]];
                    IBond matchedBond = (IBond)map[query.Bonds[i]];
                }
                #endregion
            }

            {
                Mappings mappings = null;
                #region ToChemObjects
                foreach (var obj in mappings.ToChemObjects())
                {
                    if (obj is IAtom)
                    {
                        // this atom was 'hit' by the pattern
                    }
                }
                #endregion
            }

            {
                Mappings someMappings = null;
                #region ToSubstructures
                IAtomContainer target = targetStructure;
                Mappings mappings = someMappings;
                foreach (var mol in mappings.ToSubstructures())
                {
                    foreach (var atom in mol.Atoms)
                        target.Contains(atom); // always true
                    foreach (var atom in target.Atoms)
                        mol.Contains(atom); // not always true
                }
                #endregion
            }

            {
                Mappings someMappings = null;
                #region AtLeast
                Mappings mappings = someMappings;

                if (mappings.AtLeast(5))
                {
                    // set bit flag etc.
                }

                // are the at least 5 unique matches?
                if (mappings.GetUniqueAtoms().AtLeast(5))
                {
                    // set bit etc.
                }
                #endregion
            }
        }
    }
}
