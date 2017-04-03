/* Copyright (C) 2002-2007  Oliver Horlacher
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *  All we ask is that proper credit is given for our work, which includes
 *  - but is not limited to - adding the above copyright notice to the beginning
 *  of your source code files, and to any copyright notice that you may distribute
 *  with programs based on this work.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT Any WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Beam;
using NCDK.Graphs;
using NCDK.Graphs.Invariant;
using System;
using System.IO;
using System.Text;

namespace NCDK.Smiles
{
    /// <summary>
    /// Generate a SMILES <token>cdk-cite-WEI88</token>; <token>cdk-cite-WEI89</token> string for a provided structure.
    /// The generator can produce several <i>flavour</i> of SMILES.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///     <item>generic - non-canonical SMILES string, different atom ordering
    ///         produces different SMILES. No isotope or stereochemistry encoded.
    ///         </item>
    ///     <item>unique - canonical SMILES string, different atom ordering
    ///         produces the same* SMILES. No isotope or stereochemistry encoded.
    ///         </item>
    ///     <item>isomeric - non-canonical SMILES string, different atom ordering
    ///         produces different SMILES. Isotope and stereochemistry is encoded.
    ///         </item>
    ///     <item>absolute - canonical SMILES string, different atom ordering
    ///         produces the same SMILES. Isotope and stereochemistry is encoded.</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// A generator instance is created using one of the static methods, the SMILES
    /// are then created by invoking <see cref="Create(IAtomContainer)"/>.
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Smiles.SmilesGenerator_Example.cs+1"]/*' />
    /// <para>
    /// The isomeric and absolute generator encode tetrahedral and double bond
    /// stereochemistry using <see cref="IStereoElement"/>s
    /// provided on the <see cref="IAtomContainer"/>. If stereochemistry is not being
    /// written it may need to be determined from 2D/3D coordinates using <see cref="Stereo.StereoElementFactory"/>.
    /// </para> 
    /// <para>
    /// By default the generator will not write aromatic SMILES. Kekulé SMILES are
    /// generally preferred for compatibility and aromaticity can easily be
    /// reperceived. Modifying a generator to produce <see cref="Aromatic()"/> SMILES
    /// will use the <see cref="IMolecularEntity.IsAromatic"/> flags.
    /// These flags can be set manually or with the
    /// <see cref="Aromaticities.Aromaticity"/> utility.
    /// </para>
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Smiles.SmilesGenerator_Example.cs+2"]/*' />
    /// <para>
    /// By default atom classes are not written. Atom classes can be written but
    /// creating a generator <see cref="WithAtomClasses()"/>.</para>
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Smiles.SmilesGenerator_Example.cs+3"]/*' />
    /// <para>
    /// Auxiliary data can be stored with SMILES by knowing the output order of
    /// atoms. The following example demonstrates the storage of 2D coordinates.
    /// </para>
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Smiles.SmilesGenerator_Example.cs+4"]/*' />
    /// <note type="note">
    ///   the unique SMILES generation uses a fast equitable labelling procedure
    ///   and as such there are some structures which may not be unique. The number
    ///   of such structures is generally minimal.
    ///   </note>
    /// </example>
    /// <seealso cref="Aromaticities.Aromaticity"/> 
    /// <seealso cref="Stereo.StereoElementFactory"/>
    /// <seealso cref="ITetrahedralChirality"/>
    /// <seealso cref="IDoubleBondStereochemistry"/>
    /// <seealso cref="IMolecularEntity.IsAromatic"/> 
    /// <seealso cref="SmilesParser"/>
    // @author         Oliver Horlacher
    // @author         Stefan Kuhn (chiral smiles)
    // @author         John May
    // @cdk.keyword    SMILES, generator
    // @cdk.module     smiles
    // @cdk.githash
    public sealed class SmilesGenerator
    {
        private readonly bool isomeric, canonical, aromatic, classes;
        private readonly CDKToBeam converter;

        /// <summary>
        /// Create the generic SMILES generator.
        /// <seealso cref="Generic"/>
        /// </summary>
        public SmilesGenerator()
            : this(false, false, false, false)
        {
        }

        /// <summary>
        /// Create the SMILES generator.
        /// </summary>
        /// <param name="isomeric">include isotope and stereo configurations in produced SMILES</param>
        private SmilesGenerator(bool isomeric, bool canonical, bool aromatic, bool classes)
        {
            this.isomeric = isomeric;
            this.canonical = canonical;
            this.aromatic = aromatic;
            this.classes = classes;
            this.converter = new CDKToBeam(isomeric, aromatic, classes);
        }

        /// <summary>
        /// The generator should write aromatic (lower-case) SMILES. This option is
        /// not recommended as different parsers can interpret where bonds should be
        /// placed.
        /// </summary>
        /// <example>
        /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Smiles.SmilesGenerator_Example.cs+Aromatic"]/*' />
        /// </example>
        /// <returns>a generator for aromatic SMILES</returns>
        public SmilesGenerator Aromatic()
        {
            return new SmilesGenerator(isomeric, canonical, true, classes);
        }

        /// <summary>
        /// Specifies that the generator should write atom classes in SMILES. Atom
        /// classes are provided by the <see cref="CDKPropertyName.AtomAtomMapping"/> 
        /// property. This method returns a new SmilesGenerator to use.
        /// </summary>
        /// <example>
        /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Smiles.SmilesGenerator_Example.cs+WithAtomClasses"]/*' />
        /// </example>
        /// <returns>a generator for SMILES with atom classes</returns>
        public SmilesGenerator WithAtomClasses()
        {
            return new SmilesGenerator(isomeric, canonical, aromatic, true);
        }

        /// <summary>
        /// Create a generator for generic SMILES. Generic SMILES are
        /// non-canonical and useful for storing information when it is not used
        /// as an index (i.e. unique keys). The generated SMILES is dependant on
        /// the input order of the atoms.
        /// </summary>
        /// <returns>a new arbitrary SMILES generator</returns>
        public static SmilesGenerator Generic()
        {
            return new SmilesGenerator(false, false, false, false);
        }

        /// <summary>
        /// Convenience method for creating an isomeric generator. Isomeric SMILES
        /// are non-unique but contain isotope numbers (e.g. <c>[13C]</c>) and
        /// stereo-chemistry.
        /// </summary>
        /// <returns>a new isomeric SMILES generator</returns>
        public static SmilesGenerator Isomeric()
        {
            return new SmilesGenerator(true, false, false, false);
        }

        /// <summary>
        /// Create a unique SMILES generator. Unique SMILES use a fast canonisation
        /// algorithm but does not encode isotope or stereo-chemistry.
        /// </summary>
        /// <returns>a new unique SMILES generator</returns>
        public static SmilesGenerator Unique()
        {
            return new SmilesGenerator(false, true, false, false);
        }

        /// <summary>
        /// Create a absolute SMILES generator. Unique SMILES uses the InChI to
        /// canonise SMILES and encodes isotope or stereo-chemistry. The InChI
        /// module is not a dependency of the SMILES module but should be present
        /// on the classpath when generation absolute SMILES.
        /// </summary>
        /// <returns>a new absolute SMILES generator</returns>
        public static SmilesGenerator CreateAbsolute()
        {
            return new SmilesGenerator(true, true, false, false);
        }

        /// <summary>
        /// Create a SMILES string for the provided molecule.
        /// </summary>
        /// <param name="molecule">the molecule to create the SMILES of</param>
        /// <returns>a SMILES string</returns> 
        /// <exception cref="ArgumentException">SMILES could not be generated</exception>
        [Obsolete("Use " + nameof(Create))]
        public string CreateSMILES(IAtomContainer molecule)
        {
            try
            {
                return Create(molecule);
            }
            catch (CDKException e)
            {
                throw new ArgumentException(
                        "SMILES could not be generated, please use the new API method 'Create()'"
                                + "to catch the checked exception", e);
            }
        }

        /// <summary>
        /// Create a SMILES string for the provided reaction.
        /// </summary>
        /// <param name="reaction">the reaction to create the SMILES of</param>
        /// <returns>a reaction SMILES string</returns>
        /// <exception cref="ArgumentException">SMILES could not be generated</exception>
        [Obsolete("Use " + nameof(CreateReactionSMILES))]
        public string CreateSMILES(IReaction reaction)
        {
            try
            {
                return CreateReactionSMILES(reaction);
            }
            catch (CDKException e)
            {
                throw new ArgumentException(
                        "SMILES could not be generated, please use the new API method 'Create()'"
                                + "to catch the checked exception", e);
            }
        }

        /// <summary>
        /// Generate SMILES for the provided <paramref name="molecule"/>.
        /// </summary>
        /// <param name="molecule">The molecule to evaluate</param>
        /// <returns>the SMILES string</returns>
        /// <exception cref="ArgumentException">SMILES could not be generated</exception>
        public string Create(IAtomContainer molecule)
        {
            return Create(molecule, new int[molecule.Atoms.Count]);
        }

        /// <summary>
        /// Create a SMILES string and obtain the order which the atoms were
        /// written. The output order allows one to arrange auxiliary atom data in the
        /// order that a SMILES string will be read. A simple example is seen below
        /// where 2D coordinates are stored with a SMILES string. In reality a more
        /// compact binary encoding would be used instead of printing the coordinates
        /// as a string.
        /// </summary>
        /// <example>
        /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Smiles.SmilesGenerator_Example.cs+Create"]/*' />
        /// </example>
        /// <param name="molecule">the molecule to write</param>
        /// <param name="order">array to store the output order of atoms</param>
        /// <returns>the SMILES string</returns>
        /// <exception cref="ArgumentException">SMILES could not be generated</exception>
        public string Create(IAtomContainer molecule, int[] order)
        {
            try
            {
                if (order.Length != molecule.Atoms.Count)
                    throw new ArgumentException("the array for storing output order should be"
                            + "the same length as the number of atoms");

                Graph g = converter.ToBeamGraph(molecule);

                // apply the canonical labelling
                if (canonical)
                {
                    // determine the output order
                    int[] labels = Labels(molecule);

                    g = g.Permute(labels).Resonate();

                    if (isomeric)
                    {
                        // FIXME: required to ensure canonical double bond labelling
                        g.Sort(new Graph.VisitHighOrderFirst());

                        // canonical double-bond stereo, output be C/C=C/C or C\C=C\C
                        // and we need to normalise to one
                        g = Functions.NormaliseDirectionalLabels(g);

                        // visit double bonds first, prefer C1=CC=C1 to C=1C=CC1
                        // visit hydrogens first
                        g.Sort(new Graph.VisitHighOrderFirst()).Sort(new Graph.VisitHydrogenFirst());
                    }

                    string smiles = g.ToSmiles(order);

                    // the SMILES has been generated on a reordered molecule, transform
                    // the ordering
                    int[] canorder = new int[order.Length];
                    for (int i = 0; i < labels.Length; i++)
                        canorder[i] = order[labels[i]];
                    Array.Copy(canorder, 0, order, 0, order.Length);

                    return smiles;
                }
                else
                {
                    return g.ToSmiles(order);
                }
            }
            catch (IOException e)
            {
                throw new CDKException(e.Message);
            }
        }

        /// <summary>
        /// Generate a SMILES for the given <paramref name="reaction"/>.
        /// </summary>
        /// <param name="reaction">the reaction in question</param>
        /// <returns>the SMILES representation of the reaction</returns>
        /// <exception cref="CDKException">if there is an error during SMILES generation</exception>
        public string CreateReactionSMILES(IReaction reaction)
        {
            StringBuilder reactionSMILES = new StringBuilder();
            IAtomContainerSet<IAtomContainer> reactants = reaction.Reactants;
            for (int i = 0; i < reactants.Count; i++)
            {
                reactionSMILES.Append(Create(reactants[i]));
                if (i + 1 < reactants.Count)
                {
                    reactionSMILES.Append('.');
                }
            }
            reactionSMILES.Append('>');
            IAtomContainerSet<IAtomContainer> agents = reaction.Agents;
            for (int i = 0; i < agents.Count; i++)
            {
                reactionSMILES.Append(Create(agents[i]));
                if (i + 1 < agents.Count)
                {
                    reactionSMILES.Append('.');
                }
            }
            reactionSMILES.Append('>');
            IAtomContainerSet<IAtomContainer> products = reaction.Products;
            for (int i = 0; i < products.Count; i++)
            {
                reactionSMILES.Append(Create(products[i]));
                if (i + 1 < products.Count)
                {
                    reactionSMILES.Append('.');
                }
            }
            return reactionSMILES.ToString();
        }

        /// <summary>
        /// Indicates whether output should be an aromatic SMILES.
        /// </summary>
        /// <param name="useAromaticityFlag">if false only SP2-hybridized atoms will be lower case (default),
        /// true=SP2 or aromaticity trigger lower case</param>
        [Obsolete("since 1.5.6, use " + nameof(Aromatic) + " - invoking this method does nothing")]
        public void SetUseAromaticityFlag(bool useAromaticityFlag)
        {
        }

        /// <summary>
        /// Given a molecule (possibly disconnected) compute the labels which
        /// would order the atoms by increasing canonical labelling. If the SMILES
        /// are isomeric (i.e. stereo and isotope specific) the InChI numbers are
        /// used. These numbers are loaded via reflection and the 'cdk-inchi' module
        /// should be present on the classpath.
        /// </summary>
        /// <param name="molecule">the molecule to</param>
        /// <returns>the permutation</returns>
        /// <seealso cref="Canon"/>
        private int[] Labels(IAtomContainer molecule)
        {
            long[] labels = isomeric ? InChINumbers(molecule) : Canon.Label(molecule, GraphUtil.ToAdjList(molecule));
            int[] cpy = new int[labels.Length];
            for (int i = 0; i < labels.Length; i++)
                cpy[i] = (int)labels[i] - 1;
            return cpy;
        }

        /// <summary>
        /// Obtain the InChI numbering for canonising SMILES. The cdk-smiles module
        /// does not and should not depend on cdk-inchi and so the numbers are loaded
        /// via reflection. If the class cannot be found on the classpath an
        /// exception is thrown.
        /// </summary>
        /// <param name="container">a structure</param>
        /// <returns>the inchi numbers</returns>
        /// <exception cref="CDKException">the inchi numbers could not be obtained</exception>
        private long[] InChINumbers(IAtomContainer container)
        {
            // TODO: create an interface so we don't have to dynamically load the
            // class each time

            return InChINumbersTools.GetUSmilesNumbers(container);
        }
    }
}
