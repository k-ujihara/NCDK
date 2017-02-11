















// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2015-2016  Kazuya Ujihara

using System;
using System.Collections.Generic;

namespace NCDK.Default
{
    [Serializable]
    public class Mapping
        : ChemObject, ICloneable, IMapping
    {
        private IChemObject[] relation = new IChemObject[2];

        public Mapping(IChemObject objectOne, IChemObject objectTwo)
        {
            relation[0] = objectOne;
            relation[1] = objectTwo;
        }

		public IChemObject this[int index] => relation[index];

        public IEnumerable<IChemObject> GetRelatedChemObjects()
        {
            return relation;
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone = (Mapping)base.Clone(map);
            clone.relation = new IAtom[relation.Length];
            for (var i = 0; i < relation.Length; i++)
                clone.relation[i] = (IAtom)relation[i].Clone(map);
            return clone;
        }
    }
}
namespace NCDK.Silent
{
    [Serializable]
    public class Mapping
        : ChemObject, ICloneable, IMapping
    {
        private IChemObject[] relation = new IChemObject[2];

        public Mapping(IChemObject objectOne, IChemObject objectTwo)
        {
            relation[0] = objectOne;
            relation[1] = objectTwo;
        }

		public IChemObject this[int index] => relation[index];

        public IEnumerable<IChemObject> GetRelatedChemObjects()
        {
            return relation;
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone = (Mapping)base.Clone(map);
            clone.relation = new IAtom[relation.Length];
            for (var i = 0; i < relation.Length; i++)
                clone.relation[i] = (IAtom)relation[i].Clone(map);
            return clone;
        }
    }
}
