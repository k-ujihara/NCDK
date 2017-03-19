using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace NCDK.IO.Iterator
{
    /// <summary>
    /// Iterate over conformers of a collection of molecules stored in SDF format.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class is analogous to the {@link org.openscience.cdk.io.iterator.IteratingSDFReader} except that
    /// rather than return a single <see cref="IAtomContainer"/> at each iteration this
    /// class will return all the conformers for a given molecule at each iteration.
    /// </para>
    /// <para>
    /// The class assumes that the molecules are stored in SDF format and that all conformers for a given
    /// molecule are in sequential order.
    /// </para>
    /// <para>
    /// Currently, the code uses the title of each molecule in the SD file to perform te conformer check
    /// and so it is important that all conformers for a given molecule have the same title field, but
    /// different from the title fields of conformers of other molecules. In
    /// the future the class will allow the user to perform the check using either the title or a more
    /// rigorous (but more time-consuming) graph isomorphism check.
    /// </para>
    /// </remarks>
    /// <example>
    /// Example usage is
    /// <code>
    /// string filename = "/Users/rguha/conf2.sdf";
    /// IteratingMDLConformerReader2 reader = new IteratingMDLConformerReader2(
    ///         new FileReader(new File(filename)), Default.ChemObjectBuilder.Instance);
    /// while (reader.HasNext()) {
    ///      ConformerContainer2 cc = (ConformerContainer2) reader.Next();
    /// }
    /// // do something with this set of conformers
    /// </code>
    /// </example>
    // @cdk.module extra
    // @cdk.githash
    // @author Rajarshi Guha
    // @see org.openscience.cdk.ConformerContainer
    // @cdk.keyword file format SDF
    // @cdk.keyword conformer conformation
    public class IteratingMDLConformerReader : IEnumerable<ConformerContainer>
    {
        private IteratingSDFReader imdlr;

        public IteratingMDLConformerReader(TextReader ins, IChemObjectBuilder builder)
        {
            imdlr = new IteratingSDFReader(ins, builder);
        }

        public IteratingMDLConformerReader(Stream ins, IChemObjectBuilder builder)
        {
            imdlr = new IteratingSDFReader(ins, builder);
        }

        public IEnumerator<ConformerContainer> GetEnumerator()
        {
            ConformerContainer container = null;
            foreach (var mol in imdlr)
            {
                if (container == null)
                    container = new ConformerContainer(mol);
                else
                {
                    if (container.Title.Equals(mol.GetProperty<string>(CDKPropertyName.Title)))
                        container.Add(mol);
                    else
                    {
                        yield return container;
                        container = new ConformerContainer(mol);
                    }
                }
            }
            if (container != null)
                yield return container;
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
