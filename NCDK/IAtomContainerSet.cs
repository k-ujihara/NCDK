using System;
using System.Collections.Generic;

namespace NCDK
{
    public interface IAtomContainerSet : IChemObject
    {
        void SetMultiplier(int position, double? multiplier);
        IReadOnlyList<double?> GetMultipliers();
        bool SetMultipliers(IEnumerable<double?> multipliers);
        double? GetMultiplier(int number);
        bool IsEmpty();
    }

    public interface IAtomContainerSet<T>
        : IAtomContainerSet, IList<T>
        where T : IAtomContainer
    {
        void SetMultiplier(T container, double? multiplier);
        void Add(T atomContainer, double? multiplier);
        void AddRange(IEnumerable<T> atomContainerSet);
        double? GetMultiplier(T container);
        void Sort(IComparer<T> comparator);
    }
}

