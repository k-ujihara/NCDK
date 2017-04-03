using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.Layout
{
    class StructureDiagramGenerator_Example
    {
        void Main()
        {
            IAtomContainer someMolecule = null;
            #region
            StructureDiagramGenerator sdg = new StructureDiagramGenerator();
             sdg.Molecule = someMolecule;
             sdg.GenerateCoordinates();
            IAtomContainer layedOutMol = sdg.Molecule;
            #endregion
        }
    }
}
