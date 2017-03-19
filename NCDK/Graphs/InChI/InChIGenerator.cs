/* Copyright (C) 2006-2007  Sam Adams <sea36@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using System;
using NCDK.NInChI;
using System.Collections.Generic;
using NCDK.Config;
using System.Linq;
using NCDK.Stereo;
using System.Diagnostics;

namespace NCDK.Graphs.InChi
{
    /// <summary>
    /// This class generates the IUPAC International Chemical Identifier (InChI) for
    /// a CDK IAtomContainer. It places calls to a JNI wrapper for the InChI C++ library.
    /// </summary>
    /// <remarks><para>If the atom container has 3D coordinates for all of its atoms then they
    /// will be used, otherwise 2D coordinates will be used if available.</para>
    /// <para>Spin multiplicities and some aspects of stereochemistry are not
    /// currently handled completely.</para>
    /// </remarks>
    /// <example>
    /// Example usage
    /// <code>
    /// // Generate factory -  if native code does not load
    /// InChIGeneratorFactory factory = new InChIGeneratorFactory();
    /// // Get InChIGenerator
    /// InChIGenerator gen = factory.GetInChIGenerator(container);
    /// 
    /// INCHI_RET ret = gen.ReturnStatus;
    /// if (ret == INCHI_RET.WARNING) {
    ///   // InChI generated, but with warning message
    ///   Console.Out.WriteLine("InChI warning: " + gen.Message);
    /// } else if (ret != INCHI_RET.OKAY) {
    ///   // InChI generation failed
    ///   throw new CDKException("InChI failed: " + ret.ToString()
    ///     + " [" + gen.Message + "]");
    /// }
    /// 
    /// string inchi = gen.Inchi;
    /// string auxinfo = gen.AuxInfo;
    /// </code>
    /// </example>
    /// TODO: distinguish between singlet and undefined spin multiplicity<br/>
    /// TODO: double bond and allene parities<br/>
    /// TODO: problem recognising bond stereochemistry<br/>
    // @author Sam Adams
    // @cdk.module inchi
    // @cdk.githash
    public class InChIGenerator
    {
        protected NInchiInput input;
        protected NInchiOutput output;
        private readonly bool auxNone;

        /// <summary>
        /// AtomContainer instance refers to.
        /// </summary>
        protected IAtomContainer atomContainer;

        /// <summary>
        /// Constructor. Generates InChI from CDK AtomContainer.
        /// <para>Reads atoms, bonds etc from atom container and converts to format
        /// InChI library requires, then calls the library.</para>
        /// </summary>
        /// <param name="atomContainer">AtomContainer to generate InChI for.</param>
        /// <param name="ignoreAromaticBonds">if aromatic bonds should be treated as bonds of type single and double</param>
        /// <exception cref="CDKException">if there is an error during InChI generation</exception>
        protected internal InChIGenerator(IAtomContainer atomContainer, bool ignoreAromaticBonds)
            : this(atomContainer, new[] { INCHI_OPTION.AuxNone }, ignoreAromaticBonds)
        { }

        /// <summary>
        /// Constructor. Generates InChI from CDK AtomContainer.
        /// <para>Reads atoms, bonds etc from atom container and converts to format
        /// InChI library requires, then calls the library.</para>
        /// </summary>
        /// <param name="atomContainer">AtomContainer to generate InChI for.</param>
        /// <param name="options">Space delimited string of options to pass to InChI library.
        ///     Each option may optionally be preceded by a command line switch (/ or -).</param>
        /// <param name="ignoreAromaticBonds">if aromatic bonds should be treated as bonds of type single and double</param>
        protected internal InChIGenerator(IAtomContainer atomContainer, string options, bool ignoreAromaticBonds)
        {
            try
            {
                input = new NInchiInput(options);
                GenerateInChIFromCDKAtomContainer(atomContainer, ignoreAromaticBonds);
                auxNone = input.Options != null && input.Options.Contains("AuxNone");
            }
            catch (NInchiException jie)
            {
                throw new CDKException("InChI generation failed: " + jie.Message, jie);
            }
        }

        /// <summary>
        /// Constructor. Generates InChI from CDK AtomContainer.
        /// <para>Reads atoms, bonds etc from atom container and converts to format
        /// InChI library requires, then calls the library.</para>
        /// </summary>
        /// <param name="atomContainer">AtomContainer to generate InChI for.</param>
        /// <param name="options">List of INCHI_OPTION.</param>
        /// <param name="ignoreAromaticBonds">if aromatic bonds should be treated as bonds of type single and double</param>
        protected internal InChIGenerator(IAtomContainer atomContainer, IEnumerable<INCHI_OPTION> options, bool ignoreAromaticBonds)
        {
            try
            {
                input = new NInchiInput(new List<INCHI_OPTION>(options));
                GenerateInChIFromCDKAtomContainer(atomContainer, ignoreAromaticBonds);
                auxNone = input.Options != null && input.Options.Contains("AuxNone");
            }
            catch (NInchiException jie)
            {
                throw new CDKException("InChI generation failed: " + jie.Message, jie);
            }
        }

        /// <summary>
        /// Reads atoms, bonds etc from atom container and converts to format
        /// InChI library requires, then places call for the library to generate
        /// the InChI.
        /// </summary>
        /// <param name="atomContainer">AtomContainer to generate InChI for.</param>
        /// <param name="ignore"></param>
        private void GenerateInChIFromCDKAtomContainer(IAtomContainer atomContainer, bool ignore)
        {
            this.atomContainer = atomContainer;

            // Check for 3d coordinates
            bool all3d = true;
            bool all2d = true;
            foreach (var atom in atomContainer.Atoms)
            {
                // fixed CDK's bug
                if (all3d && !atom.Point3D.HasValue)
                    all3d = false;
                if (all2d && !atom.Point2D.HasValue)
                    all2d = false;
            }

            // Process atoms
            IsotopeFactory ifact = null;
            try
            {
                ifact = Isotopes.Instance;
            }
            catch (Exception)
            {
                // Do nothing
            }

            IDictionary<IAtom, NInchiAtom> atomMap = new Dictionary<IAtom, NInchiAtom>();
            foreach (var atom in atomContainer.Atoms)
            {
                // Get coordinates
                // Use 3d if possible, otherwise 2d or none
                double x, y, z;
                if (all3d)
                {
                    var p = atom.Point3D.Value;
                    x = p.X;
                    y = p.Y;
                    z = p.Z;
                }
                else if (all2d)
                {
                    var p = atom.Point2D.Value;
                    x = p.X;
                    y = p.Y;
                    z = 0.0;
                }
                else
                {
                    x = 0.0;
                    y = 0.0;
                    z = 0.0;
                }

                // Chemical element symbol
                string el = atom.Symbol;

                // Generate InChI atom
                NInchiAtom iatom = input.Add(new NInchiAtom(x, y, z, el));
                atomMap[atom] = iatom;

                // Check if charged
                int charge = atom.FormalCharge ?? 0;    // fixed CDK's bug
                if (charge != 0)
                {
                    iatom.Charge = charge;
                }

                // Check whether isotopic
                int? isotopeNumber = atom.MassNumber;
                if (isotopeNumber.HasValue && ifact != null)
                {
                    IAtom isotope = atomContainer.Builder.CreateAtom(el);
                    ifact.Configure(isotope);
                    if (isotope.MassNumber.Value == isotopeNumber.Value)
                    {
                        isotopeNumber = 0;
                    }
                }
                if (isotopeNumber.HasValue)
                {
                    iatom.IsotopicMass = isotopeNumber.Value;
                }

                // Check for implicit hydrogens
                // atom.HydrogenCount returns number of implict hydrogens, not
                // total number
                // Ref: Posting to cdk-devel list by Egon Willighagen 2005-09-17
                int? implicitH = atom.ImplicitHydrogenCount;

                // set implicit hydrogen count, -1 tells the inchi to determine it
                iatom.ImplicitH = implicitH ?? -1;

                // Check if radical
                int count = atomContainer.GetConnectedSingleElectrons(atom).Count();
                if (count == 0)
                {
                    // TODO - how to check whether singlet or undefined multiplicity
                }
                else if (count == 1)
                {
                    iatom.Radical = INCHI_RADICAL.Doublet;
                }
                else if (count == 2)
                {
                    iatom.Radical = INCHI_RADICAL.Triplet;
                }
                else
                {
                    throw new CDKException("Unrecognised radical type");
                }
            }

            // Process bonds
            IDictionary<IBond, NInchiBond> bondMap = new Dictionary<IBond, NInchiBond>();
            foreach (var bond in atomContainer.Bonds)
            {
                // Assumes 2 centre bond
                NInchiAtom at0 = (NInchiAtom)atomMap[bond.Atoms[0]];
                NInchiAtom at1 = (NInchiAtom)atomMap[bond.Atoms[1]];

                // Get bond order
                INCHI_BOND_TYPE order;
                BondOrder bo = bond.Order;
                if (!ignore && bond.IsAromatic)
                {
                    order = INCHI_BOND_TYPE.Altern;
                }
                else if (bo == BondOrder.Single)
                {
                    order = INCHI_BOND_TYPE.Single;
                }
                else if (bo == BondOrder.Double)
                {
                    order = INCHI_BOND_TYPE.Double;
                }
                else if (bo == BondOrder.Triple)
                {
                    order = INCHI_BOND_TYPE.Triple;
                }
                else
                {
                    throw new CDKException("Failed to generate InChI: Unsupported bond type");
                }

                // Create InChI bond
                NInchiBond ibond = new NInchiBond(at0, at1, order);
                bondMap[bond] = ibond;
                input.Add(ibond);

                // Check for bond stereo definitions
                BondStereo stereo = bond.Stereo;
                // No stereo definition
                if (stereo == BondStereo.None)
                {
                    ibond.BondStereo = INCHI_BOND_STEREO.None;
                }
                // Bond ending (fat end of wedge) below the plane
                else if (stereo == BondStereo.Down)
                {
                    ibond.BondStereo = INCHI_BOND_STEREO.Single1Down;
                }
                // Bond ending (fat end of wedge) above the plane
                else if (stereo == BondStereo.Up)
                {
                    ibond.BondStereo = INCHI_BOND_STEREO.Single1Up;
                }
                // Bond starting (pointy end of wedge) below the plane
                else if (stereo == BondStereo.DownInverted)
                {
                    ibond.BondStereo = INCHI_BOND_STEREO.Single2Down;
                }
                // Bond starting (pointy end of wedge) above the plane
                else if (stereo == BondStereo.UpInverted)
                {
                    ibond.BondStereo = INCHI_BOND_STEREO.Single2Up;
                }
                else if (stereo == BondStereo.EOrZ)
                {
                    ibond.BondStereo = INCHI_BOND_STEREO.DoubleEither;
                }
                else if (stereo == BondStereo.UpOrDown)
                {
                    ibond.BondStereo = INCHI_BOND_STEREO.Single1Either;
                }
                else if (stereo == BondStereo.UpOrDownInverted)
                {
                    ibond.BondStereo = INCHI_BOND_STEREO.Single2Either;
                }
                // Bond with undefined stereochemistry
                else if (stereo == BondStereo.None)
                {
                    if (order == INCHI_BOND_TYPE.Single)
                    {
                        ibond.BondStereo = INCHI_BOND_STEREO.Single1Either;
                    }
                    else if (order == INCHI_BOND_TYPE.Double)
                    {
                        ibond.BondStereo = INCHI_BOND_STEREO.DoubleEither;
                    }
                }
            }

            // Process tetrahedral stereo elements
            foreach (var stereoElem in atomContainer.StereoElements)
            {
                if (stereoElem is ITetrahedralChirality)
                {
                    ITetrahedralChirality chirality = (ITetrahedralChirality)stereoElem;
                    var surroundingAtoms = chirality.Ligands;
                    TetrahedralStereo stereoType = chirality.Stereo;

                    NInchiAtom atC = (NInchiAtom)atomMap[chirality.ChiralAtom];
                    NInchiAtom at0 = (NInchiAtom)atomMap[surroundingAtoms[0]];
                    NInchiAtom at1 = (NInchiAtom)atomMap[surroundingAtoms[1]];
                    NInchiAtom at2 = (NInchiAtom)atomMap[surroundingAtoms[2]];
                    NInchiAtom at3 = (NInchiAtom)atomMap[surroundingAtoms[3]];
                    INCHI_PARITY p = INCHI_PARITY.Unknown;
                    if (stereoType == TetrahedralStereo.AntiClockwise)
                    {
                        p = INCHI_PARITY.Odd;
                    }
                    else if (stereoType == TetrahedralStereo.Clockwise)
                    {
                        p = INCHI_PARITY.Even;
                    }
                    else
                    {
                        throw new CDKException("Unknown tetrahedral chirality");
                    }

                    NInchiStereo0D jniStereo = new NInchiStereo0D(atC, at0, at1, at2, at3,
                            INCHI_STEREOTYPE.Tetrahedral, p);
                    input.Stereos.Add(jniStereo);
                }
                else if (stereoElem is IDoubleBondStereochemistry)
                {
                    IDoubleBondStereochemistry dbStereo = (IDoubleBondStereochemistry)stereoElem;
                    var surroundingBonds = dbStereo.Bonds;
                    if (surroundingBonds[0] == null || surroundingBonds[1] == null)
                        throw new CDKException("Cannot generate an InChI with incomplete double bond info");
                    DoubleBondConformation stereoType = dbStereo.Stereo;

                    IBond stereoBond = dbStereo.StereoBond;
                    NInchiAtom at0 = null;
                    NInchiAtom at1 = null;
                    NInchiAtom at2 = null;
                    NInchiAtom at3 = null;
                    // TODO: I should check for two atom bonds... or maybe that should happen when you
                    //    create a double bond stereochemistry
                    if (stereoBond.Contains(surroundingBonds[0].Atoms[0]))
                    {
                        // first atom is A
                        at1 = (NInchiAtom)atomMap[surroundingBonds[0].Atoms[0]];
                        at0 = (NInchiAtom)atomMap[surroundingBonds[0].Atoms[1]];
                    }
                    else
                    {
                        // first atom is X
                        at0 = (NInchiAtom)atomMap[surroundingBonds[0].Atoms[0]];
                        at1 = (NInchiAtom)atomMap[surroundingBonds[0].Atoms[1]];
                    }
                    if (stereoBond.Contains(surroundingBonds[1].Atoms[0]))
                    {
                        // first atom is B
                        at2 = (NInchiAtom)atomMap[surroundingBonds[1].Atoms[0]];
                        at3 = (NInchiAtom)atomMap[surroundingBonds[1].Atoms[1]];
                    }
                    else
                    {
                        // first atom is Y
                        at2 = (NInchiAtom)atomMap[surroundingBonds[1].Atoms[1]];
                        at3 = (NInchiAtom)atomMap[surroundingBonds[1].Atoms[0]];
                    }
                    INCHI_PARITY p = INCHI_PARITY.Unknown;
                    if (stereoType == DoubleBondConformation.Together)
                    {
                        p = INCHI_PARITY.Odd;
                    }
                    else if (stereoType == DoubleBondConformation.Opposite)
                    {
                        p = INCHI_PARITY.Even;
                    }
                    else
                    {
                        throw new CDKException("Unknown double bond stereochemistry");
                    }

                    NInchiStereo0D jniStereo = new NInchiStereo0D(null, at0, at1, at2, at3,
                            INCHI_STEREOTYPE.DoubleBond, p);
                    input.Stereos.Add(jniStereo);
                }
                else if (stereoElem is ExtendedTetrahedral)
                {

                    ExtendedTetrahedral extendedTetrahedral = (ExtendedTetrahedral)stereoElem;
                    TetrahedralStereo winding = extendedTetrahedral.Winding;

                    // The periphals (p<i>) and terminals (t<i>) are refering to
                    // the following atoms. The focus (f) is also shown.
                    //
                    //   p0          p2
                    //    \          /
                    //     t0 = f = t1
                    //    /         \
                    //   p1         p3
                    IAtom[] terminals = extendedTetrahedral.FindTerminalAtoms(atomContainer);
                    IAtom[] peripherals = extendedTetrahedral.Peripherals;

                    // InChI API is particualar about the input, each terminal atom
                    // needs to be present in the list of neighbors and they must
                    // be at index 1 and 2 (i.e. in the middle). This is true even
                    // of explict atoms. For the implicit atoms, the terminals may
                    // be in the peripherals allready and so we correct the winding
                    // and reposition as needed.

                    IList<IBond> t0Bonds = OnlySingleBonded(atomContainer.GetConnectedBonds(terminals[0]));
                    IList<IBond> t1Bonds = OnlySingleBonded(atomContainer.GetConnectedBonds(terminals[1]));

                    // first if there are two explicit atoms we need to replace one
                    // with the terminal atom - the configuration does not change
                    if (t0Bonds.Count == 2)
                    {
                        var orgBond = t0Bonds[0];
                        t0Bonds.RemoveAt(0);
                        IAtom replace = orgBond.GetConnectedAtom(terminals[0]);
                        for (int i = 0; i < peripherals.Length; i++)
                            if (replace == peripherals[i]) peripherals[i] = terminals[0];
                    }

                    if (t1Bonds.Count == 2)
                    {
                        var orgBond = t0Bonds[0];
                        t1Bonds.RemoveAt(0);
                        IAtom replace = orgBond.GetConnectedAtom(terminals[1]);
                        for (int i = 0; i < peripherals.Length; i++)
                            if (replace == peripherals[i]) peripherals[i] = terminals[1];
                    }

                    // the neighbor attached to each terminal atom that we will
                    // define the configuration of
                    IAtom t0Neighbor = t0Bonds[0].GetConnectedAtom(terminals[0]);
                    IAtom t1Neighbor = t1Bonds[0].GetConnectedAtom(terminals[1]);

                    // we now need to move all the atoms into the correct positions
                    // everytime we exchange atoms the configuration inverts
                    for (int i = 0; i < peripherals.Length; i++)
                    {
                        if (i != 0 && t0Neighbor == peripherals[i])
                        {
                            Swap(peripherals, i, 0);
                            winding = winding.Invert();
                        }
                        else if (i != 1 && terminals[0] == peripherals[i])
                        {
                            Swap(peripherals, i, 1);
                            winding = winding.Invert();
                        }
                        else if (i != 2 && terminals[1] == peripherals[i])
                        {
                            Swap(peripherals, i, 2);
                            winding = winding.Invert();
                        }
                        else if (i != 3 && t1Neighbor == peripherals[i])
                        {
                            Swap(peripherals, i, 3);
                            winding = winding.Invert();
                        }
                    }

                    INCHI_PARITY parity = INCHI_PARITY.Unknown;
                    if (winding == TetrahedralStereo.AntiClockwise)
                        parity = INCHI_PARITY.Odd;
                    else if (winding == TetrahedralStereo.Clockwise)
                        parity = INCHI_PARITY.Even;
                    else
                        throw new CDKException("Unknown extended tetrahedral chirality");

                    NInchiStereo0D jniStereo = new NInchiStereo0D(atomMap[extendedTetrahedral.Focus],
                            atomMap[peripherals[0]], atomMap[peripherals[1]], atomMap[peripherals[2]],
                            atomMap[peripherals[3]], INCHI_STEREOTYPE.Allene, parity);
                    input.Stereos.Add(jniStereo);
                }
            }

            try
            {
                output = NInchiWrapper.GetInchi(input);
            }
            catch (NInchiException jie)
            {
                throw new CDKException("Failed to generate InChI: " + jie.Message, jie);
            }
        }

        private static IList<IBond> OnlySingleBonded(IEnumerable<IBond> bonds)
        {
            IList<IBond> filtered = new List<IBond>();
            foreach (var bond in bonds)
            {
                if (bond.Order == BondOrder.Single) filtered.Add(bond);
            }
            return filtered;
        }

        private static void Swap(Object[] objs, int i, int j)
        {
            object tmp = objs[i];
            objs[i] = objs[j];
            objs[j] = tmp;
        }

        /// <summary>
        /// Gets return status from InChI process.  OKAY and WARNING indicate
        /// InChI has been generated, in all other cases InChI generation
        /// has failed.
        /// </summary>
        public INCHI_RET ReturnStatus => output.ReturnStatus;

        /// <summary>
        /// Gets generated InChI string.
        /// </summary>
        public string InChI => output.InChI;

        /// <summary>
        /// Gets generated InChIKey string.
        /// </summary>
        public string GetInChIKey()
        {
            NInchiOutputKey key;
            try
            {
                key = NInchiWrapper.GetInchiKey(output.InChI);
                if (key.ReturnStatus == INCHI_KEY.OK)
                {
                    return key.Key;
                }
                else
                {
                    throw new CDKException("Error while creating InChIKey: " + key.ReturnStatus);
                }
            }
            catch (NInchiException exception)
            {
                throw new CDKException("Error while creating InChIKey: " + exception.Message, exception);
            }
        }

        /// <summary>
        /// Gets auxillary information.
        /// </summary>
        public string AuxInfo
        {
            get
            {
                if (auxNone)
                {
                    Trace.TraceWarning("AuxInfo requested but AuxNone option is set (default).");
                }
                return (output.AuxInfo);
            }
        }

        /// <summary>
        /// Gets generated (error/warning) messages.
        /// </summary>
        public string Message => output.Message;

        /// <summary>
        /// Gets generated log.
        /// </summary>
        public string Log => output.Log;
    }
}
