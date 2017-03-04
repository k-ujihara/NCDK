
















// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2015-2017  Kazuya Ujihara

using System;

namespace NCDK.Default
{
    [Serializable]
    public class Substance
        : AtomContainerSet<IAtomContainer>, ISubstance, ICloneable
    {
        public Substance()
            : base()
        {
        }
    }
}
namespace NCDK.Silent
{
    [Serializable]
    public class Substance
        : AtomContainerSet<IAtomContainer>, ISubstance, ICloneable
    {
        public Substance()
            : base()
        {
        }
    }
}
