using NCDK.Common.Collections;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.SMSD.Algorithms.VFLib
{
    internal class Mapper
    {
        public static DictionaryEqualityComparer<INode, IAtom> Comparer_INode_IAtom { get; } = new DictionaryEqualityComparer<INode, IAtom>();
    }
}
