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
using NCDK.Config;
using NCDK.Graphs;
using NCDK.Graphs.Invariant;
using NCDK.SGroups;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NCDK.Smiles
{
    /// <summary>
    /// Generate a SMILES <token>cdk-cite-WEI88</token>; <token>cdk-cite-WEI89</token>provides a compact representation of
    /// chemical structures and reactions.
    /// </summary>
    /// <remarks>
    /// Different <i>flavours</i> of SMILES can be generated and are fully configurable.
    /// The standard flavours of SMILES defined by Daylight are:
    /// <list type="bullet">
    ///     <item><b>Generic</b> - non-canonical SMILES string, different atom ordering
    ///         produces different SMILES. No isotope or stereochemistry encoded.
    ///         </item>
    ///     <item><b>Unique</b> - canonical SMILES string, different atom ordering
    ///         produces the same* SMILES. No isotope or stereochemistry encoded.
    ///         </item>
    ///     <item><b>Isomeric</b> - non-canonical SMILES string, different atom ordering
    ///         produces different SMILES. Isotope and stereochemistry is encoded.
    ///         </item>
    ///     <item><b>Absolute</b> - canonical SMILES string, different atom ordering
    ///         produces the same SMILES. Isotope and stereochemistry is encoded.</item>
    /// </list> 
    /// 
    /// To output a given flavour the flags in <see cref="SmiFlavor"/> are used:
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Smiles.SmilesGenerator_Example.cs+SmiFlavor"]/*' />
    /// <see cref="SmiFlavor"/> provides more fine grained control, for example,
    /// for the following is equivalent to <see cref="SmiFlavor.Isomeric"/>:
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Smiles.SmilesGenerator_Example.cs+SmiFlavor_Isomeric"]/*' />
    /// Bitwise logic can be used such that we can remove options:
    /// <see cref="SmiFlavor.Isomeric"/> <pre>^</pre> <see cref="SmiFlavor.AtomicMass"/>
    /// will generate isomeric SMILES without atomic mass.
    /// </remarks>
    /// <example>
    /// A generator instance is created using one of the static methods, the SMILES
    /// are then created by invoking <see cref="Create(IAtomContainer)"/>.
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Smiles.SmilesGenerator_Example.cs+1"]/*' />
    /// <para>
    /// The isomeric and absolute generator encode tetrahedral and double bond
    /// stereochemistry using <see cref="IStereoElement{TFocus, TCarriers}"/>s
    /// provided on the <see cref="IAtomContainer"/>. If stereochemistry is not being
    /// written it may need to be determined from 2D/3D coordinates using <see cref="Stereo.StereoElementFactory"/>.
    /// </para> 
    /// <para>
    /// By default the generator will not write aromatic SMILES.Kekulé SMILES are
    /// generally preferred for compatibility and aromaticity can easily be
    /// re-perceived by most tool kits whilst kekulisation may fail. If you
    /// really want aromatic SMILES the following code demonstrates
    /// </para>
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Smiles.SmilesGenerator_Example.cs+2"]/*' />
    /// <para>
    /// 
    /// It can be useful to know the output order of SMILES. On input the order of the atoms
    /// reflects the atom index. If we know this order we can refer to atoms by index and
    /// associate data with the SMILES string.
    /// The output order is obtained by parsing in an auxiliary array during creation. The
    /// following snippet demonstrates how we can write coordinates in order.
    /// 
    /// </para>
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Smiles.SmilesGenerator_Example.cs+4"]/*' />
    /// <para>
    /// Using the output order of SMILES forms the basis of
    /// <see href="https://www.chemaxon.com/marvin-archive/latest/help/formats/cxsmiles-doc.html">
    /// ChemAxon Extended SMILES (CXSMILES)</see> which can also be generated. Extended SMILES
    /// allows additional structure data to be serialized including, atom labels/values, fragment
    /// grouping (for salts in reactions), polymer repeats, multi center bonds, and coordinates.
    /// The CXSMILES layer is appended after the SMILES so that parser which don't interpret it
    /// can ignore it.
    /// </para>
    /// <para>
    /// The two aggregate flavours are <see cref="SmiFlavor.CxSmiles"/> and <see cref="SmiFlavor.CxSmilesWithCoords"/>.
    /// As with other flavours, fine grain control is possible <see cref="SmiFlavor"/>.
    /// </para>
    /// <b>*</b> the unique SMILES generation uses a fast equitable labelling procedure
    ///   and as such there are some structures which may not be unique. The number
    ///   of such structures is generally minimal.
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
        private readonly SmiFlavor flavour;

        /// <summary>
        /// Create the SMILES generator, the default output is described by: <see cref="SmiFlavor.Default"/>
        /// but is best to choose/set this flavor.
        /// </summary>
        /// <seealso cref="SmiFlavor.Default"/>
        [Obsolete("Use " + nameof(SmilesGenerator) + "(int) configuring with " + nameof(SmiFlavor) + ".")]
        public SmilesGenerator()
            : this(SmiFlavor.Default)
        {
        }

        /// <summary>
        /// Create a SMILES generator with the specified <see cref="SmiFlavor"/>.
        /// </summary>
        /// <example>
        /// <code>
        /// SmilesGenerator smigen = new SmilesGenerator(SmiFlavor.Stereo |
        ///                                              SmiFlavor.Canonical);
        /// </code>
        /// </example>
        /// <param name="flavour">SMILES flavour flags <see cref="SmiFlavor"/></param>
        public SmilesGenerator(SmiFlavor flavour)
        {
            this.flavour = flavour;
        }

        /// <summary>
        /// Derived a new generator that writes aromatic atoms in lower case.
        /// </summary>
        /// <example>
        /// The preferred way of doing this is now to use the <see cref="SmilesGenerator(SmiFlavor)"/> constructor:
        /// <code>
        /// SmilesGenerator smigen = new SmilesGenerator(SmiFlavor.UseAromaticSymbols);
        /// </code>
        /// </example>
        /// <returns>a generator for aromatic SMILES</returns>
        [Obsolete("Configure with " + nameof(SmilesGenerator))]
        public SmilesGenerator Aromatic()
        {
            return new SmilesGenerator(this.flavour | SmiFlavor.UseAromaticSymbols);
        }

        /// <summary>
        /// Specifies that the generator should write atom classes in SMILES. Atom
        /// classes are provided by the <see cref="CDKPropertyName.AtomAtomMapping"/>
        /// property. This method returns a new SmilesGenerator to use.
        /// </summary>
        /// <example>
        /// <code>
        /// IAtomContainer container = ...;
        /// SmilesGenerator smilesGen = SmilesGenerator.WithAtomClasses();
        /// smilesGen.CreateSMILES(container); // C[CH2:4]O second atom has class = 4
        /// </code>
        /// </example>
        /// <returns>a generator for SMILES with atom classes</returns>
        [Obsolete("Configure with " + nameof(SmilesGenerator))]
        public SmilesGenerator WithAtomClasses()
        {
            return new SmilesGenerator(this.flavour | SmiFlavor.AtomAtomMap);
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
            return new SmilesGenerator(SmiFlavor.Generic);
        }

        /// <summary>
        /// Convenience method for creating an isomeric generator. Isomeric SMILES
        /// are non-unique but contain isotope numbers (e.g. <pre>[13C]</pre>) and
        /// stereo-chemistry.
        /// </summary>
        /// <returns>a new isomeric SMILES generator</returns>
        public static SmilesGenerator Isomeric()
        {
            return new SmilesGenerator(SmiFlavor.Isomeric);
        }

        /// <summary>
        /// Create a unique SMILES generator. Unique SMILES use a fast canonisation
        /// algorithm but does not encode isotope or stereo-chemistry.
        /// </summary>
        /// <returns>a new unique SMILES generator</returns>
        public static SmilesGenerator Unique()
        {
            return new SmilesGenerator(SmiFlavor.Unique);
        }

        /// <summary>
        /// Create a absolute SMILES generator. 
        /// </summary>
        /// <remarks>
        /// Unique SMILES uses the InChI to
        /// canonise SMILES and encodes isotope or stereo-chemistry. The InChI
        /// module is not a dependency of the SMILES module but should be present
        /// on the path when generation absolute SMILES.
        /// </remarks>
        /// <returns>a new absolute SMILES generator</returns>
        public static SmilesGenerator CreateAbsolute()
        {
            return new SmilesGenerator(SmiFlavor.Absolute);
        }

        /// <summary>
        /// Create a SMILES string for the provided molecule.
        /// </summary>
        /// <param name="molecule">the molecule to create the SMILES of</param>
        /// <returns>a SMILES string</returns>
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
                        "SMILES could not be generated, please use the new API method 'create()'"
                                + "to catch the checked exception", e);
            }
        }

        /// <summary>
        /// Create a SMILES string for the provided reaction.
        /// </summary>
        /// <param name="reaction">the reaction to create the SMILES of</param>
        /// <returns>a reaction SMILES string</returns>
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
                        "SMILES could not be generated, please use the new API method 'create()'"
                                + "to catch the checked exception", e);
            }
        }

        /// <summary>
        /// Generate SMILES for the provided <code>molecule</code>.
        /// </summary>
        /// <param name="molecule">The molecule to evaluate</param>
        /// <returns>the SMILES string</returns>
        /// <exception cref="CDKException">SMILES could not be created</exception>
        public string Create(IAtomContainer molecule)
        {
            return Create(molecule, new int[molecule.Atoms.Count]);
        }

        /// <summary>
        /// Creates a SMILES string of the flavour specified in the constructor
        /// and write the output order to the provided array.
        /// </summary>
        /// <remarks>
        /// The output order allows one to arrange auxiliary atom data in the
        /// order that a SMILES string will be read. A simple example is seen below
        /// where 2D coordinates are stored with a SMILES string. This method
        /// forms the basis of CXSMILES.
        /// </remarks>
        /// <example>
        /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Smiles.SmilesGenerator_Example.cs+Create_IAtomContainer_int"]/*' />
        /// </example>
        /// <param name="molecule">the molecule to write</param>
        /// <param name="order">array to store the output order of atoms</param>
        /// <returns>the SMILES string</returns>
        /// <exception cref="CDKException">SMILES could not be created</exception>
        public string Create(IAtomContainer molecule, int[] order)
        {
            return Create(molecule, this.flavour, order);
        }

        /// <summary>
        /// Creates a SMILES string of the flavour specified as a parameter
        /// and write the output order to the provided array.
        /// </summary>
        /// <remarks>
        /// The output order allows one to arrange auxiliary atom data in the
        /// order that a SMILES string will be read. A simple example is seen below
        /// where 2D coordinates are stored with a SMILES string. This method
        /// forms the basis of CXSMILES.
        /// </remarks>
        /// <example>
        /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Smiles.SmilesGenerator_Example.cs+Create_IAtomContainer_int_int"]/*' />
        /// </example>
        /// <param name="molecule">the molecule to write</param>
        /// <param name="order">array to store the output order of atoms</param>
        /// <returns>the SMILES string</returns>
        /// <exception cref="CDKException">a valid SMILES could not be created</exception>
        public static string Create(IAtomContainer molecule, SmiFlavor flavour, int[] order)
        {
            try
            {
                if (order.Length != molecule.Atoms.Count)
                    throw new ArgumentException("the array for storing output order should be"
                            + "the same length as the number of atoms");

                Graph g = CDKToBeam.ToBeamGraph(molecule, flavour);

                // apply the canonical labelling
                if (SmiFlavors.IsSet(flavour, SmiFlavor.Canonical))
                {
                    // determine the output order
                    int[] labels = Labels(flavour, molecule);

                    g = g.Permute(labels).Resonate();

                    if (SmiFlavors.IsSet(flavour, SmiFlavor.StereoCisTrans))
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
                    System.Array.Copy(canorder, 0, order, 0, order.Length);

                    if (SmiFlavors.IsSet(flavour, SmiFlavor.CxSmilesWithCoords))
                    {
                        smiles += CxSmilesGenerator.Generate(GetCxSmilesState(flavour, molecule),
                                                             flavour, null, order);
                    }

                    return smiles;
                }
                else
                {
                    string smiles = g.ToSmiles(order);

                    if (SmiFlavors.IsSet(flavour, SmiFlavor.CxSmilesWithCoords))
                    {
                        smiles += CxSmilesGenerator.Generate(GetCxSmilesState(flavour, molecule), flavour, null, order);
                    }

                    return smiles;
                }
            }
            catch (IOException e)
            {
                throw new CDKException(e.Message);
            }
        }

        /// <summary>
        /// Create a SMILES for a reaction.
        /// </summary>
        /// <param name="reaction">CDK reaction instance</param>
        /// <returns>reaction SMILES</returns>
        /// <exception cref="CDKException">a valid SMILES could not be created</exception>
        [Obsolete("Use " + nameof(Create) + "(" + nameof(IAtomContainer) + ")")]
        public string CreateReactionSMILES(IReaction reaction)
        {
            return Create(reaction);
        }

        /// <summary>
        /// Create a SMILES for a reaction of the flavour specified in the constructor.
        /// </summary>
        /// <param name="reaction">CDK reaction instance</param>
        /// <returns>reaction SMILES</returns>
        public string Create(IReaction reaction)
        {
            return Create(reaction, new int[ReactionManipulator.GetAtomCount(reaction)]);
        }

        // utility method that safely collects the Sgroup from a molecule
        private void SafeAddSgroups(List<SGroup> sgroups, IAtomContainer mol)
        {
            IList<SGroup> molSgroups = mol.GetProperty<IList<SGroup>>(CDKPropertyName.CtabSgroups);
            if (molSgroups != null)
                foreach (var g in molSgroups)
                    sgroups.Add(g);
        }

        /// <summary>
        /// Create a SMILES for a reaction of the flavour specified in the constructor and
        /// write the output order to the provided array.
        /// </summary>
        /// <param name="reaction">CDK reaction instance</param>
        /// <returns>reaction SMILES</returns>
        public string Create(IReaction reaction, int[] ordering)
        {
            var reactants = reaction.Reactants;
            var agents = reaction.Agents;
            var products = reaction.Products;

            IAtomContainer reactantPart = reaction.Builder.NewAtomContainer();
            IAtomContainer agentPart = reaction.Builder.NewAtomContainer();
            IAtomContainer productPart = reaction.Builder.NewAtomContainer();

            List<SGroup> sgroups = new List<SGroup>();

            foreach (IAtomContainer reactant in reactants)
            {
                reactantPart.Add(reactant);
                SafeAddSgroups(sgroups, reactant);
            }
            foreach (IAtomContainer agent in agents)
            {
                agentPart.Add(agent);
                SafeAddSgroups(sgroups, agent);
            }
            foreach (IAtomContainer product in products)
            {
                productPart.Add(product);
                SafeAddSgroups(sgroups, product);
            }

            int[] reactantOrder = new int[reactantPart.Atoms.Count];
            int[] agentOrder = new int[agentPart.Atoms.Count];
            int[] productOrder = new int[productPart.Atoms.Count];

            int expectedSize = reactantOrder.Length + agentOrder.Length + productOrder.Length;
            if (expectedSize != ordering.Length)
            {
                throw new CDKException("Output ordering array does not have correct amount of space: " + ordering.Length +
                                       " expected: " + expectedSize);
            }

            // we need to make sure we generate without the CXSMILES layers
            string smi = Create(reactantPart, flavour & ~SmiFlavor.CxSmilesWithCoords, reactantOrder) + ">" +
                         Create(agentPart, flavour & ~SmiFlavor.CxSmilesWithCoords, agentOrder) + ">" +
                         Create(productPart, flavour & ~SmiFlavor.CxSmilesWithCoords, productOrder);

            // copy ordering back to unified array and adjust values
            int agentBeg = reactantOrder.Length;
            int agentEnd = reactantOrder.Length + agentOrder.Length;
            int prodEnd = reactantOrder.Length + agentOrder.Length + productOrder.Length;
            System.Array.Copy(reactantOrder, 0, ordering, 0, agentBeg);
            System.Array.Copy(agentOrder, 0, ordering, agentBeg, agentEnd - agentBeg);
            System.Array.Copy(productOrder, 0, ordering, agentEnd, prodEnd - agentEnd);
            for (int i = agentBeg; i < agentEnd; i++)
                ordering[i] += agentBeg;
            for (int i = agentEnd; i < prodEnd; i++)
                ordering[i] += agentEnd;

            if (SmiFlavors.IsSet(flavour, SmiFlavor.CxSmilesWithCoords))
            {
                IAtomContainer unified = reaction.Builder.NewAtomContainer();
                unified.Add(reactantPart);
                unified.Add(agentPart);
                unified.Add(productPart);
                unified.SetProperty(CDKPropertyName.CtabSgroups, sgroups);

                // base CXSMILES state information
                CxSmilesState cxstate = GetCxSmilesState(flavour, unified);

                int[] components = null;

                // extra state info on fragment grouping, specific to reactions
                if (SmiFlavors.IsSet(flavour, SmiFlavor.CxFragmentGroup))
                {

                    cxstate.fragGroups = new List<List<int>>();

                    // calculate the connected components
                    components = new ConnectedComponents(GraphUtil.ToAdjList(unified)).Components();

                    // AtomContainerSet is ordered so this is safe, it was actually a set we
                    // would need some extra data structures
                    var tmp = new HashSet<int>();
                    int beg = 0, end = 0;
                    foreach (IAtomContainer mol in reactants)
                    {
                        end = end + mol.Atoms.Count;
                        tmp.Clear();
                        for (int i = beg; i < end; i++)
                            tmp.Add(components[i]);
                        if (tmp.Count > 1)
                            cxstate.fragGroups.Add(new List<int>(tmp));
                        beg = end;
                    }
                    foreach (IAtomContainer mol in agents)
                    {
                        end = end + mol.Atoms.Count;
                        tmp.Clear();
                        for (int i = beg; i < end; i++)
                            tmp.Add(components[i]);
                        if (tmp.Count > 1)
                            cxstate.fragGroups.Add(new List<int>(tmp));
                        beg = end;
                    }
                    foreach (IAtomContainer mol in products)
                    {
                        end = end + mol.Atoms.Count;
                        tmp.Clear();
                        for (int i = beg; i < end; i++)
                            tmp.Add(components[i]);
                        if (tmp.Count > 1)
                            cxstate.fragGroups.Add(new List<int>(tmp));
                        beg = end;
                    }

                }


                smi += CxSmilesGenerator.Generate(cxstate, flavour, components, ordering);
            }

            return smi;
        }

        /// <summary>
        /// Indicates whether output should be an aromatic SMILES.
        /// </summary>
        /// <param name="useAromaticityFlag">if false only SP2-hybridized atoms will be lower case (default), true=SP2 or aromaticity trigger lower case</param>
        [Obsolete("Since 1.5.6, use " + nameof(Aromatic) + "()  - invoking this method does nothing")]
        public void SetUseAromaticityFlag(bool useAromaticityFlag)
        {
        }

        /// <summary>
        /// Given a molecule (possibly disconnected) compute the labels which
        /// would order the atoms by increasing canonical labelling. If the SMILES
        /// are isomeric (i.e. stereo and isotope specific) the InChI numbers are
        /// used. These numbers are loaded via reflection and the 'cdk-inchi' module
        /// should be present on the path.
        /// </summary>
        /// <param name="molecule">the molecule to</param>
        /// <returns>the permutation</returns>
        /// <seealso cref="Canon"/>
        private static int[] Labels(SmiFlavor flavour, IAtomContainer molecule)
        {
            // FIXME: use SmiOpt.InChiLabelling
            long[] labels = SmiFlavors.IsSet(flavour, SmiFlavor.Isomeric) ? InchiNumbers(molecule)
                                                                         : Canon.Label(molecule, GraphUtil.ToAdjList(molecule));
            int[] cpy = new int[labels.Length];
            for (int i = 0; i < labels.Length; i++)
                cpy[i] = (int)labels[i] - 1;
            return cpy;
        }

        /// <summary>
        /// Obtain the InChI numbering for canonising SMILES. The cdk-smiles module
        /// does not and should not depend on cdk-inchi and so the numbers are loaded
        /// via reflection. If the class cannot be found on the path an
        /// exception is thrown.
        /// </summary>
        /// <param name="container">a structure</param>
        /// <returns>the inchi numbers</returns>
        /// <exception cref="CDKException">the inchi numbers could not be obtained</exception>
        private static long[] InchiNumbers(IAtomContainer container)
        {
            // TODO: create an interface so we don't have to dynamically load the
            // class each time
            string cname = "NCDK.Graphs.Invariant.InChINumbersTools";
            string mname = "GetUSmilesNumbers";

            var rgrps = GetRgrps(container, Elements.Rutherfordium);
            foreach (IAtom rgrp in rgrps)
            {
                rgrp.AtomicNumber = Elements.Rutherfordium.AtomicNumber;
                rgrp.Symbol = Elements.Rutherfordium.Symbol;
            }

            try
            {
                var c = Type.GetType(cname, true);
                var method = c.GetMethod("GetUSmilesNumbers", BindingFlags.Public | BindingFlags.Static, null, new[] { container.GetType() }, null);
                return (long[])method.Invoke(c, new[] { container });
            }
            catch (FileNotFoundException)
            {
                throw new CDKException("The cdk-inchi module is not loaded,"
                        + " this module is need when generating absolute SMILES.");
            }
            catch (TypeLoadException e)
            {
                throw new CDKException("The method " + mname + " was not found", e);
            }
            catch (TargetInvocationException e)
            {
                throw new CDKException("An InChI could not be generated and used to canonise SMILES: " + e.Message, e);
            }
            finally
            {
                foreach (IAtom rgrp in rgrps)
                {
                    rgrp.AtomicNumber = Elements.Unknown.AtomicNumber;
                    rgrp.Symbol = "*";
                }
            }
        }

        private static IList<IAtom> GetRgrps(IAtomContainer container, Elements reversed)
        {
            List<IAtom> res = new List<IAtom>();
            foreach (IAtom atom in container.Atoms)
            {
                if (atom.AtomicNumber == 0)
                {
                    res.Add(atom);
                }
                else if (atom.AtomicNumber == reversed.AtomicNumber)
                {
                    return Array.Empty<IAtom>();
                }
            }
            return res;
        }

        // utility safety check to guard against invalid state
        private static int EnsureNotNull(int? x)
        {
            if (x == null)
                throw new InvalidOperationException("Inconsistent CXSMILES state! Check the SGroups.");
            return x.Value;
        }

        // utility method maps the atoms to their indices using the provided map.
        private static List<int> ToAtomIdxs(ICollection<IAtom> atoms, IDictionary<IAtom, int> atomidx)
        {
            List<int> idxs = new List<int>(atoms.Count);
            foreach (IAtom atom in atoms)
                idxs.Add(EnsureNotNull(atomidx[atom]));
            return idxs;
        }

        private class Comp<T> : IEqualityComparer<T>
        {
            public bool Equals(T x, T y) => object.ReferenceEquals(x, y);
            public int GetHashCode(T obj) => 0;
        }

        // Creates a CxSmilesState from a molecule with atom labels, repeat units, multi-center bonds etc
        private static CxSmilesState GetCxSmilesState(SmiFlavor flavour, IAtomContainer mol)
        {
            CxSmilesState state = new CxSmilesState
            {
                AtomCoords = new List<double[]>(),
                coordFlag = false
            };

            // set the atom labels, values, and coordinates,
            // and build the atom->idx map required by other parts
            IDictionary<IAtom, int> atomidx = new Dictionary<IAtom, int>(new Comp<IAtom>());
            for (int idx = 0; idx < mol.Atoms.Count; idx++)
            {
                IAtom atom = mol.Atoms[idx];
                if (atom is IPseudoAtom)
                {

                    if (state.atomLabels == null)
                        state.atomLabels = new Dictionary<int, string>();

                    IPseudoAtom pseudo = (IPseudoAtom)atom;
                    if (pseudo.AttachPointNum > 0)
                    {
                        state.atomLabels[idx] = "_AP" + pseudo.AttachPointNum;
                    }
                    else
                    {
                        if (!"*".Equals(pseudo.Label))
                            state.atomLabels[idx] = pseudo.Label;
                    }
                }
                object comment = atom.GetProperty<object>(CDKPropertyName.Comment);
                if (comment != null)
                {
                    if (state.atomValues == null)
                        state.atomValues = new Dictionary<int, string>();
                    state.atomValues[idx] = comment.ToString();
                }
                atomidx[atom] = idx;

                var p2 = atom.Point2D;
                var p3 = atom.Point3D;

                if (SmiFlavors.IsSet(flavour, SmiFlavor.Cx2dCoordinates) && p2 != null)
                {
                    state.AtomCoords.Add(new double[] { p2.Value.X, p2.Value.Y, 0 });
                    state.coordFlag = true;
                }
                else if (SmiFlavors.IsSet(flavour, SmiFlavor.Cx3dCoordinates) && p3 != null)
                {
                    state.AtomCoords.Add(new double[] { p3.Value.X, p3.Value.Y, p3.Value.Z });
                    state.coordFlag = true;
                }
                else if (SmiFlavors.IsSet(flavour, SmiFlavor.CxCoordinates))
                {
                    state.AtomCoords.Add(new double[3]);
                }
            }

            if (!state.coordFlag)
                state.AtomCoords = null;

            // radicals
            if (mol.SingleElectrons.Count > 0)
            {
                state.atomRads = new Dictionary<int, CxSmilesState.Radicals>();
                foreach (ISingleElectron radical in mol.SingleElectrons)
                {
                    // 0->1, 1->2, 2->3
                    if (!state.atomRads.TryGetValue(EnsureNotNull(atomidx[radical.Atom]), out CxSmilesState.Radicals val))
                        val = CxSmilesState.Radicals.Monovalent;
                    else if (val == CxSmilesState.Radicals.Monovalent)
                        val = CxSmilesState.Radicals.Divalent;
                    else if (val == CxSmilesState.Radicals.Divalent)
                        val = CxSmilesState.Radicals.Trivalent;
                    else if (val == CxSmilesState.Radicals.Trivalent)
                        throw new ArgumentException("Invalid radical state, can not be more than trivalent");

                    state.atomRads[atomidx[radical.Atom]] = val;
                }
            }

            IList<SGroup> sgroups = mol.GetProperty<IList<SGroup>>(CDKPropertyName.CtabSgroups);
            if (sgroups != null)
            {
                state.sgroups = new List<CxSmilesState.PolymerSgroup>();
                state.positionVar = new Dictionary<int, IList<int>>();
                foreach (SGroup sgroup in sgroups)
                {
                    switch (sgroup.Type)
                    {
                        // polymer SRU
                        case SGroupTypes.CtabStructureRepeatUnit:
                        case SGroupTypes.CtabMonomer:
                        case SGroupTypes.CtabMer:
                        case SGroupTypes.CtabCopolymer:
                        case SGroupTypes.CtabCrossLink:
                        case SGroupTypes.CtabModified:
                        case SGroupTypes.CtabMixture:
                        case SGroupTypes.CtabFormulation:
                        case SGroupTypes.CtabAnyPolymer:
                        case SGroupTypes.CtabGeneric:
                        case SGroupTypes.CtabComponent:
                        case SGroupTypes.CtabGraft:
                            string supscript = (string)sgroup.GetValue(SGroupKeys.CtabConnectivity);
                            state.sgroups.Add(new CxSmilesState.PolymerSgroup(GetSgroupPolymerKey(sgroup),
                                                                              ToAtomIdxs(sgroup.Atoms, atomidx),
                                                                              sgroup.Subscript,
                                                                              supscript));
                            break;
                        case SGroupTypes.ExtMulticenter:
                            IAtom beg = null;
                            List<IAtom> ends = new List<IAtom>();
                            ISet<IBond> bonds = sgroup.Bonds;
                            if (bonds.Count != 1)
                                throw new ArgumentException("Multicenter Sgroup in inconsistent state!");
                            IBond bond = bonds.First();
                            foreach (IAtom atom in sgroup.Atoms)
                            {
                                if (bond.Contains(atom))
                                {
                                    if (beg != null)
                                        throw new ArgumentException("Multicenter Sgroup in inconsistent state!");
                                    beg = atom;
                                }
                                else
                                {
                                    ends.Add(atom);
                                }
                            }
                            state.positionVar[EnsureNotNull(atomidx[beg])] =
                                                  ToAtomIdxs(ends, atomidx);
                            break;
                        case SGroupTypes.CtabAbbreviation:
                        case SGroupTypes.CtabMultipleGroup:
                            // display shortcuts are not output
                            break;
                        case SGroupTypes.CtabData:
                            // can be generated but currently ignored
                            break;
                        default:
                            throw new NotSupportedException("Unsupported Sgroup Polymer");
                    }
                }
            }

            return state;
        }

        private static string GetSgroupPolymerKey(SGroup sgroup)
        {
            switch (sgroup.Type)
            {
                case SGroupTypes.CtabStructureRepeatUnit:
                    return "n";
                case SGroupTypes.CtabMonomer:
                    return "mon";
                case SGroupTypes.CtabMer:
                    return "mer";
                case SGroupTypes.CtabCopolymer:
                    string subtype = (string)sgroup.GetValue(SGroupKeys.CtabSubType);
                    if (subtype == null)
                        return "co";
                    switch (subtype)
                    {
                        case "RAN":
                            return "ran";
                        case "ALT":
                            return "alt";
                        case "BLO":
                            return "blk";
                    }
                    goto case SGroupTypes.CtabCrossLink;
                case SGroupTypes.CtabCrossLink:
                    return "xl";
                case SGroupTypes.CtabModified:
                    return "mod";
                case SGroupTypes.CtabMixture:
                    return "mix";
                case SGroupTypes.CtabFormulation:
                    return "f";
                case SGroupTypes.CtabAnyPolymer:
                    return "any";
                case SGroupTypes.CtabGeneric:
                    return "gen";
                case SGroupTypes.CtabComponent:
                    return "c";
                case SGroupTypes.CtabGraft:
                    return "grf";
                default:
                    throw new ArgumentException();
            }
        }
    }
}
