/*  Copyright (C) 2005-2007  Christian Hoppe <chhoppe@users.sf.net>
 *                     2011  Egon Willighagen <egonw@users.sf.net>
 *                     2014  Mark B Vine (orcid:0000-0002-7794-0426)
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
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Common.Primitives;
using NCDK.Fingerprint;
using NCDK.IO.Iterator;
using NCDK.Isomorphisms;
using NCDK.Isomorphisms.MCSS;
using NCDK.Tools.Manipulator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace NCDK.Modeling.Builder3D
{
    /**
     * Helper class for ModelBuilder3D. Handles templates. This is
     * our layout solution for 3D ring systems
     *
     * @author      cho
     * @author      steinbeck
     * @cdk.created 2004-09-21
     * @cdk.module  builder3d
     * @cdk.githash
     */
    public class TemplateHandler3D
    {
        private static readonly IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;
        IAtomContainerSet<IAtomContainer> templates = null;
        IList<BitArray> fingerprintData = null;
        private bool templatesLoaded = false;

        private static TemplateHandler3D self = null;

        private UniversalIsomorphismTester universalIsomorphismTester = new UniversalIsomorphismTester();

        private TemplateHandler3D()
        {
            templates = builder.CreateAtomContainerSet();
            fingerprintData = new List<BitArray>();
        }

        public static TemplateHandler3D Instance
        {
            get
            {
                if (self == null)
                {
                    self = new TemplateHandler3D();
                }
                return self;
            }
        }

        /**
         * Loads all existing templates into memory.
         * Template file is a mdl file. Creates a Object Set of Molecules
         * @throws CDKException The template file cannot be loaded
         */
        private void LoadTemplates()
        {
            Debug.WriteLine("Loading templates...");
            IteratingSDFReader imdl;
            Stream ins;
            TextReader fin;

            try
            {
                ins = this.GetType().Assembly
                        .GetManifestResourceStream("NCDK.Modeling.Builder3D.Data.ringTemplateStructures.sdf.gz");
                fin = new StreamReader(new GZipStream(ins, CompressionMode.Decompress));
                imdl = new IteratingSDFReader(fin, builder);
            }
            catch (IOException exc1)
            {
                throw new CDKException("Problems loading file ringTemplateStructures.sdf.gz", exc1);
            }
            foreach (var molecule in imdl)
            {
                templates.Add(molecule);
            }
            try
            {
                imdl.Close();
            }
            catch (Exception exc2)
            {
                Console.Out.WriteLine("Could not close Reader due to: " + exc2.Message);
            }
            //Debug.WriteLine("TEMPLATE Finger");
            try
            {
                ins = this.GetType().Assembly.GetManifestResourceStream("NCDK.Modeling.Builder3D.Data.ringTemplateFingerprints.txt.gz");
                fin = new StreamReader(new GZipStream(ins, CompressionMode.Decompress));
            }
            catch (Exception exc3)
            {
                throw new CDKException($"Could not read Fingerprints from FingerprintFile due to: {exc3.Message}", exc3);
            }
            string s = null;
            while (true)
            {
                try
                {
                    s = fin.ReadLine();
                }
                catch (Exception exc4)
                {
                    throw new CDKException($"Error while reading the fingerprints: {exc4.Message}", exc4);
                }

                if (s == null)
                {
                    break;
                }
                try
                {
                    fingerprintData.Add((BitArray)GetBitSetFromFile(Strings.Tokenize(s, '\t', ' ', ';', '{', ',', '}')));
                }
                catch (Exception exception)
                {
                    throw new CDKException($"Error while reading the fingerprints: {exception.Message}", exception);
                }
            }
            //Debug.WriteLine("Fingerprints are read in:"+fingerprintData.Count);
            templatesLoaded = true;
        }

        public static BitArray GetBitSetFromFile(IEnumerable<string> st)
        {
            BitArray bitSet = new BitArray(1024);
            foreach (var s in st)
                bitSet.Set(int.Parse(s), true);
            return bitSet;
        }

        /**
         * Returns the largest (number of atoms) ring set in a molecule.
         *
         *@param  ringSystems  RingSystems of a molecule
         *@return              The largestRingSet
         */
        public IRingSet GetLargestRingSet(List<IRingSet> ringSystems)
        {
            IRingSet largestRingSet = null;
            int atomNumber = 0;
            IAtomContainer container = null;
            for (int i = 0; i < ringSystems.Count; i++)
            {
                container = GetAllInOneContainer(ringSystems[i]);
                if (atomNumber < container.Atoms.Count)
                {
                    atomNumber = container.Atoms.Count;
                    largestRingSet = ringSystems[i];
                }
            }
            return largestRingSet;
        }

        private IAtomContainer GetAllInOneContainer(IRingSet ringSet)
        {
            IAtomContainer resultContainer = ringSet.Builder.CreateAtomContainer();
            var containers = RingSetManipulator.GetAllAtomContainers(ringSet);
            foreach (var container in containers)
                resultContainer.Add(container);
            return resultContainer;
        }

        /**
         * Checks if one of the loaded templates is a substructure in the given
         * Molecule. If so, it assigns the coordinates from the template to the
         * respective atoms in the Molecule.
         *
         * @param ringSystems       AtomContainer from the ring systems.
         * @param numberOfRingAtoms Number of atoms in the specified ring
         * @throws CloneNotSupportedException The atomcontainer cannot be cloned.
         */
        public void MapTemplates(IAtomContainer ringSystems, int numberOfRingAtoms)
        {
            if (!templatesLoaded) self.LoadTemplates();

            //Debug.WriteLine("Map Template...START---Number of Ring Atoms:"+numberOfRingAtoms);
            IAtomContainer ringSystemAnyBondAnyAtom = AtomContainerManipulator.Anonymise(ringSystems);
            BitArray ringSystemFingerprint = new HybridizationFingerprinter().GetBitFingerprint(ringSystemAnyBondAnyAtom).AsBitSet();
            bool flagMaxSubstructure = false;
            bool flagSecondbest = false;
            for (int i = 0; i < fingerprintData.Count; i++)
            {
                IAtomContainer template = templates[i];
                //if the atom count is different, it can't be right anyway
                if (template.Atoms.Count != ringSystems.Atoms.Count)
                {
                    continue;
                }
                //we compare the fingerprint with any atom and any bond
                if (FingerprinterTool.IsSubset(fingerprintData[i], ringSystemFingerprint))
                {
                    IAtomContainer templateAnyBondAnyAtom = AtomContainerManipulator.Anonymise(template);
                    //we do the exact match with any atom and any bond
                    if (universalIsomorphismTester.IsSubgraph(ringSystemAnyBondAnyAtom, templateAnyBondAnyAtom))
                    {
                        //if this is the case, we keep it as a guess, but look if we can do better
                        var list = universalIsomorphismTester.GetSubgraphAtomsMap(ringSystemAnyBondAnyAtom,
                                        templateAnyBondAnyAtom);
                        bool flagwritefromsecondbest = false;
                        if ((numberOfRingAtoms == list.Count)
                                && templateAnyBondAnyAtom.Bonds.Count == ringSystems.Bonds.Count)
                        {
                            //so atom and bond count match, could be it's even an exact match,
                            //we check this with the original ring system
                            if (universalIsomorphismTester.IsSubgraph(ringSystems, template))
                            {
                                flagMaxSubstructure = true;
                                list = universalIsomorphismTester.GetSubgraphAtomsMap(ringSystems, template);
                            }
                            else
                            {
                                //if it isn't we still now it's better than just the isomorphism
                                flagSecondbest = true;
                                flagwritefromsecondbest = true;
                            }
                        }

                        if (!flagSecondbest || flagMaxSubstructure || flagwritefromsecondbest)
                        {
                            for (int j = 0; j < list.Count; j++)
                            {
                                RMap map = list[j];
                                IAtom atom1 = ringSystems.Atoms[map.Id1];
                                IAtom atom2 = template.Atoms[map.Id2];
                                if (atom1.IsInRing)
                                {
                                    atom1.Point3D = atom2.Point3D;
                                }
                            }//for j
                        }

                        if (flagMaxSubstructure)
                        {
                            break;
                        }

                    }//if subgraph
                }//if fingerprint
            }//for i
            if (!flagMaxSubstructure)
            {
                Console.Out.WriteLine("WARNING: Maybe RingTemplateError!");
            }
        }

        /**
         * Gets the templateCount attribute of the TemplateHandler object.
         *
         * @return The templateCount value
         */
        public int TemplateCount => templates.Count;

        /**
         *  Gets the templateAt attribute of the TemplateHandler object.
         *
         *@param  position  Description of the Parameter
         *@return The templateAt value
         */
        public IAtomContainer GetTemplateAt(int position)
        {
            return templates[position];
        }
    }
}
