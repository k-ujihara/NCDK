using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.Groups
{
    class PartitionRefinement_Example
    {
        void Main()
        {
            IAtomContainer someMolecule = null;
            {
                #region
                IAtomContainerDiscretePartitionRefiner refiner = PartitionRefinement.ForAtoms().Create();
                #endregion
            }
        }
    }
}
