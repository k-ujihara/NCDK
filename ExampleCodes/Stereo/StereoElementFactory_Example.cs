namespace NCDK.Stereo
{
    class StereoElementFactory_Example
    {
        void Main()
        {
            IAtomContainer someMolecule = null;
            StereoElementFactory someFactory = null;
            {
                #region
                IAtomContainer container = someMolecule;
                StereoElementFactory stereo = StereoElementFactory.Using2DCoordinates(container).InterpretProjections(Projection.Haworth);

                // set the elements replacing any existing elements (recommended)
                container.SetStereoElements(stereo.CreateAll());

                // adding elements individually is no recommended as the AtomContainer
                // does not check for duplicate or contradicting elements
                foreach (var element in stereo.CreateAll())
                    container.StereoElements.Add(element); // bad, there may already be elements
                #endregion
            }
            {
                #region CreateTetrahedral_int
                StereoElementFactory factory = someFactory; // 2D/3D
                IAtomContainer container = someMolecule; // container

                for (int v = 0; v < container.Atoms.Count; v++)
                {
                    // ... verify v is a stereo atom ...
                    ITetrahedralChirality element = factory.CreateTetrahedral(v, null);
                    if (element != null)
                        container.StereoElements.Add(element);
                }
                #endregion
            }
            {
                #region CreateTetrahedral_IAtom
                StereoElementFactory factory = someFactory; // 2D/3D
                IAtomContainer container = someMolecule; // container

                foreach (var atom in container.Atoms)
                {
                    // ... verify atom is a stereo atom ...
                    ITetrahedralChirality element = factory.CreateTetrahedral(atom, null);
                    if (element != null)
                        container.StereoElements.Add(element);
                }
                #endregion
            }
            {
                #region CreateGeometric_IBond
                StereoElementFactory factory = someFactory; // 2D/3D
                IAtomContainer container = someMolecule; // container

                foreach (var bond in container.Bonds)
                {
                    if (bond.Order != BondOrder.Double)
                        continue;
                    // ... verify bond is a stereo bond...
                    IDoubleBondStereochemistry element = factory.CreateGeometric(bond, null);
                    if (element != null)
                        container.StereoElements.Add(element);
                }
                #endregion
            }
            {
                #region CreateExtendedTetrahedral
                StereoElementFactory factory = someFactory; // 2D/3D
                IAtomContainer container = someMolecule; // container

                for (int v = 0; v < container.Atoms.Count; v++)
                {
                    // ... verify v is a stereo atom ...
                    ExtendedTetrahedral element = factory.CreateExtendedTetrahedral(v, null);
                    if (element != null)
                        container.StereoElements.Add(element);
                }
                #endregion
            }
            {
                IAtomContainer container = someMolecule; // container
                #region InterpretProjections
                StereoElementFactory factory =
                    StereoElementFactory.Using2DCoordinates(container)
                        .InterpretProjections(Projection.Fischer, Projection.Haworth);
                #endregion
            }
        }
    }
}
