using System;

namespace NCDK.Isomorphisms.Matchers
{
    /// <summary>
    /// Defines the ability to be matched against {@link IAtom}'s. Most prominent application
    /// is in isomorphism and substructure matching in the {@link org.openscience.cdk.isomorphism.UniversalIsomorphismTester}.
    /// </summary>
    public interface IQueryAtom : IAtom
    {
        /// <summary>
        /// Returns true of the given <code>atom</code> matches this IQueryAtom.
        /// </summary>
        /// <param name="atom">IAtom to match against</param>
        /// <returns>true, if this IQueryAtom matches the given IAtom</returns>
        bool Matches(IAtom atom);
    }
}
