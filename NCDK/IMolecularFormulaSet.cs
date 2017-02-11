using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace NCDK
{
    public interface IMolecularFormulaSet
        : ICDKObject, IList<IMolecularFormula>
    {
        void AddRange(IEnumerable<IMolecularFormula> collection);
    }
}
