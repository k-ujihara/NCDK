using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;
using NCDK.Stereo;
using System.Linq;
using static NCDK.Geometries.CIP.CIPTool;

namespace NCDK.Geometries.CIP
{
    [TestClass]
    public class CIPTool_Example
    {
        [TestMethod]
        [TestCategory("Example")]
        public void Main()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom centralAtom = new Atom();
            #region
            IAtom[] ligandAtoms = mol.GetConnectedAtoms(centralAtom).ToArray();
            ITetrahedralChirality tetraStereo = new TetrahedralChirality(centralAtom, ligandAtoms, TetrahedralStereo.AntiClockwise);
            CIPChirality cipChirality = CIPTool.GetCIPChirality(mol, tetraStereo);
            #endregion
        }
    }
}
