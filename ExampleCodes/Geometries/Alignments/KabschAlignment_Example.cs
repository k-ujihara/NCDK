using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Numerics;
using NCDK.Silent;
using System;
using System.Linq;

namespace NCDK.Geometries.Alignments
{
    [TestClass()]
    public class KabschAlignment_Example
    {
        [TestMethod]
        [TestCategory("Example")]
        public void Main()
        {
            {
                #region
                AtomContainer ac1 = new AtomContainer();    // molecule 1
                AtomContainer ac2 = new AtomContainer();    // molecule 2
                try
                {
                    KabschAlignment sa = new KabschAlignment(ac1.Atoms, ac2.Atoms);

                    sa.Align();
                    Console.Out.WriteLine(sa.RMSD);
                }
                catch (CDKException) { }
                #endregion
            }
            {
                #region substructure
                AtomContainer ac1 = new AtomContainer();    // whole molecules
                AtomContainer ac2 = new AtomContainer();    // 
                IAtom[] a1 = ac1.Atoms.ToArray();   // some subsets of atoms from the two molecules
                IAtom[] a2 = ac2.Atoms.ToArray();   // 
                try
                {
                    var sa = new KabschAlignment(a1, a2);
                    sa.Align();

                    var cm1 = sa.CenterOfMass;
                    foreach (var a in ac1.Atoms)
                        a.Point3D = a.Point3D.Value - cm1;
                    sa.RotateAtomContainer(ac2);
                    // display the two AtomContainer's
                }
                catch (CDKException) { }
                #endregion
            }
        }
    }
}
