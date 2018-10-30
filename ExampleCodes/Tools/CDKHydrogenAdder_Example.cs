using NCDK.AtomTypes;
using NCDK.Silent;
using NCDK.Tools.Manipulator;

namespace NCDK.Tools
{
    public class CDKHydrogenAdder_Example
    {
        public void Ctor()
        {
            {
                #region 1
                IAtomContainer methane = new AtomContainer();
                IAtom carbon = new Atom("C");
                methane.Atoms.Add(carbon);
                CDKAtomTypeMatcher matcher = CDKAtomTypeMatcher.GetInstance();
                foreach (var atom in methane.Atoms)
                {
                    IAtomType type = matcher.FindMatchingAtomType(methane, atom);
                    AtomTypeManipulator.Configure(atom, type);
                }
                var adder = CDK.HydrogenAdder;
                adder.AddImplicitHydrogens(methane);
                #endregion
            }
            {
                #region 2
                IAtomContainer ethane = new AtomContainer();
                IAtom carbon1 = new Atom("C");
                IAtom carbon2 = new Atom("C");
                ethane.Atoms.Add(carbon1);
                ethane.Atoms.Add(carbon2);
                CDKAtomTypeMatcher matcher = CDKAtomTypeMatcher.GetInstance();
                IAtomType type = matcher.FindMatchingAtomType(ethane, carbon1);
                AtomTypeManipulator.Configure(carbon1, type);
                var adder = CDK.HydrogenAdder;
                adder.AddImplicitHydrogens(ethane, carbon1);
                #endregion
            }
        }
    }
}
