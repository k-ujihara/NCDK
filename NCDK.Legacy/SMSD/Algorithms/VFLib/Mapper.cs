using NCDK.Common.Collections;

namespace NCDK.SMSD.Algorithms.VFLib
{
    [System.Obsolete]
    internal class Mapper
    {
        public static DictionaryEqualityComparer<INode, IAtom> Comparer_INode_IAtom { get; } = new DictionaryEqualityComparer<INode, IAtom>();
    }
}
