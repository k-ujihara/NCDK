/*
 *
 * Copyright (C) 2006-2010  Syed Asad Rahman <asad@ebi.ac.uk>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Aromaticities;
using NCDK.Default;
using NCDK.Geometries;
using NCDK.IO;
using NCDK.SMSD.Labelling;
using NCDK.Tools;
using System;
using System.Diagnostics;
using System.IO;

namespace NCDK.SMSD.Tools
{
    /// <summary>
    /// Class that handles molecules for MCS search.
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    /// </summary>
    public class MolHandler
    {
        private IAtomContainer atomContainer = null;
        private bool removeHydrogen = false;
        private ICanonicalMoleculeLabeller canonLabeler = new CanonicalLabellingAdaptor();

        /// <summary>
        /// Creates a new instance of MolHandler
        /// <param name="molFile">atomContainer file name</param>
        /// <param name="cleanMolecule">/// @param removeHydrogen</param>
        ///
        /// </summary>
        public MolHandler(string molFile, bool removeHydrogen, bool cleanMolecule)
        {
            MDLReader molRead = null;
            this.removeHydrogen = removeHydrogen;
            try
            {
                Stream readMolecule = null;

                readMolecule = new FileStream(molFile, FileMode.Open, FileAccess.Read);
                molRead = new MDLReader(new StreamReader(readMolecule));
                this.atomContainer = (IAtomContainer)molRead.Read(new AtomContainer());
                molRead.Close();
                readMolecule.Close();
                /* Remove Hydrogen by Asad */
                if (removeHydrogen)
                {
                    atomContainer = ExtAtomContainerManipulator.RemoveHydrogensExceptSingleAndPreserveAtomID(atomContainer);
                }
                if (cleanMolecule)
                {

                    if (!IsPseudoAtoms())
                    {
                        atomContainer = canonLabeler.GetCanonicalMolecule(atomContainer);
                    }
                    // percieve atoms, set valency etc
                    ExtAtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(atomContainer);
                    //Add implicit Hydrogens
                    CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(atomContainer.Builder);
                    adder.AddImplicitHydrogens(atomContainer);
                    // figure out which atoms are in aromatic rings:
                    Aromaticity.CDKLegacy.Apply(atomContainer);
                    BondTools.MakeUpDownBonds(atomContainer);
                }
            }
            catch (IOException ex)
            {
                Debug.WriteLine(ex);
            }
            catch (CDKException e)
            {
                Console.Error.WriteLine(e);
            }
            finally
            {
                if (molRead != null)
                {
                    try
                    {
                        molRead.Close();
                    }
                    catch (IOException ioe)
                    {
                        Trace.TraceWarning("Couldn't close molReader: ", ioe.Message);
                        Debug.WriteLine(ioe);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new instance of MolHandler
        /// <param name="container">Molecule AtomContainer</param>
        /// <param name="cleanMolecule">/// @param removeHydrogen</param>
        /// </summary>
        public MolHandler(IAtomContainer container, bool removeHydrogen, bool cleanMolecule)
        {
            string molID = container.Id;
            this.removeHydrogen = removeHydrogen;
            this.atomContainer = container;
            if (removeHydrogen)
            {
                try
                {
                    this.atomContainer = ExtAtomContainerManipulator
                            .RemoveHydrogensExceptSingleAndPreserveAtomID(atomContainer);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                }
            }
            else
            {
                this.atomContainer = container.Builder.CreateAtomContainer(atomContainer);
            }

            if (cleanMolecule)
            {
                try
                {
                    if (!IsPseudoAtoms())
                    {
                        atomContainer = canonLabeler.GetCanonicalMolecule(atomContainer);
                    }
                    // percieve atoms, set valency etc
                    ExtAtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(atomContainer);
                    //Add implicit Hydrogens
                    CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(atomContainer.Builder);
                    adder.AddImplicitHydrogens(atomContainer);
                    // figure out which atoms are in aromatic rings:
                    Aromaticity.CDKLegacy.Apply(atomContainer);
                }
                catch (CDKException ex)
                {
                    Trace.TraceError(ex.Message);
                }
            }
            atomContainer.Id = molID;
        }

        /// <summary>
        /// Returns the modified container
        /// <returns>get processed / modified container</returns>
        /// </summary>
        public IAtomContainer Molecule => atomContainer;

        /// <summary>
        /// Returns true if hydrogens were made implicit else return false
        /// <returns>true if remove H else false</returns>
        /// </summary>
        public bool RemoveHydrogenFlag => removeHydrogen;

        private bool IsPseudoAtoms()
        {
            foreach (var atoms in atomContainer.Atoms)
            {
                if (atoms is IPseudoAtom)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
