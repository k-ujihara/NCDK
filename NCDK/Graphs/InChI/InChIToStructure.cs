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
using NCDK.Config;
using NCDK.Stereo;
using NCDK.Tools;
using NCDK.NInChI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace NCDK.Graphs.InChI
{
    /// <summary>
    /// This class generates a CDK IAtomContainer from an InChI string.  It places
    /// calls to a JNI wrapper for the InChI C++ library.
    /// <para>
    /// The generated IAtomContainer will have all 2D and 3D coordinates set to 0.0,
    /// but may have atom parities set.  Double bond and allene stereochemistry are
    /// not currently recorded.
    /// </para>
    /// </summary>
    /// <example>
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Graphs.InChI.InChIToStructure_Example.cs"]/*' />
    /// </example>
    // @author Sam Adams
    // @cdk.module inchi
    // @cdk.githash
    public class InChIToStructure
    {
        protected NInchiInputInchi input;
        protected NInchiOutputStructure output;
        protected IAtomContainer molecule;

        // magic number - indicates isotope mass is relative
        private const int ISOTOPIC_SHIFT_FLAG = 10000;

        /// <summary>
        /// Constructor. Generates CDK AtomContainer from InChI.
        /// </summary>
        /// <param name="inchi"></param>
        /// <param name="builder"></param>
        internal protected InChIToStructure(string inchi, IChemObjectBuilder builder)
        {
            try
            {
                input = new NInchiInputInchi(inchi, "");
            }
            catch (NInchiException jie)
            {
                throw new CDKException("Failed to convert InChI to molecule: " + jie.Message, jie);
            }
            GenerateAtomContainerFromInChI(builder);
        }

        /// <summary>
        /// Constructor. Generates CMLMolecule from InChI.
        /// </summary>
        /// <param name="inchi"></param>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        protected internal InChIToStructure(string inchi, IChemObjectBuilder builder, string options)
        {
            try
            {
                input = new NInchiInputInchi(inchi, options);
            }
            catch (NInchiException jie)
            {
                throw new CDKException("Failed to convert InChI to molecule: " + jie.Message, jie);
            }
            GenerateAtomContainerFromInChI(builder);
        }

        /// <summary>
        /// Constructor. Generates CMLMolecule from InChI.
        /// </summary>
        /// <param name="inchi"></param>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        protected internal InChIToStructure(string inchi, IChemObjectBuilder builder, IList<string> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            try
            {
                input = new NInchiInputInchi(inchi, options.Select(n => INCHI_OPTION.ValueOfIgnoreCase(n)).ToList());
            }
            catch (NInchiException jie)
            {
                throw new CDKException("Failed to convert InChI to molecule: " + jie.Message);
            }
            GenerateAtomContainerFromInChI(builder);
        }

        /// <summary>
        /// Gets structure from InChI, and converts InChI library data structure into an IAtomContainer.
        /// </summary>
        /// <param name="builder"></param>
        protected internal void GenerateAtomContainerFromInChI(IChemObjectBuilder builder)
        {
            try
            {
                output = NInchiWrapper.GetStructureFromInchi(input);
            }
            catch (NInchiException jie)
            {
                throw new CDKException("Failed to convert InChI to molecule: " + jie.Message, jie);
            }

            //molecule = new AtomContainer();
            molecule = builder.NewAtomContainer();

            IDictionary<NInchiAtom, IAtom> inchiCdkAtomMap = new Dictionary<NInchiAtom, IAtom>();

            for (int i = 0; i < output.Atoms.Count; i++)
            {
                NInchiAtom iAt = output.Atoms[i];
                IAtom cAt = builder.NewAtom();

                inchiCdkAtomMap[iAt] = cAt;

                cAt.Id = "a" + i;
                cAt.Symbol = iAt.ElementType;
                cAt.AtomicNumber = PeriodicTable.GetAtomicNumber(cAt.Symbol);

                // Ignore coordinates - all zero - unless aux info was given... but
                // the CDK doesn't have an API to provide that

                // InChI does not have unset properties so we set charge,
                // hydrogen count (implicit) and isotopic mass
                cAt.FormalCharge = iAt.Charge;
                cAt.ImplicitHydrogenCount = iAt.ImplicitH;

                int isotopicMass = iAt.IsotopicMass;

                if (isotopicMass != 0)
                {
                    if (ISOTOPIC_SHIFT_FLAG == (isotopicMass & ISOTOPIC_SHIFT_FLAG))
                    {
                        try
                        {
                            int massNumber = Isotopes.Instance.GetMajorIsotope(cAt.AtomicNumber.Value).MassNumber.Value;
                            cAt.MassNumber = massNumber + (isotopicMass - ISOTOPIC_SHIFT_FLAG);
                        }
                        catch (IOException e)
                        {
                            throw new CDKException("Could not load Isotopes data", e);
                        }
                    }
                    else
                    {
                        cAt.MassNumber = isotopicMass;
                    }
                }

                molecule.Atoms.Add(cAt);
            }

            for (int i = 0; i < output.Bonds.Count; i++)
            {
                NInchiBond iBo = output.Bonds[i];

                IAtom atO = inchiCdkAtomMap[iBo.OriginAtom];
                IAtom atT = inchiCdkAtomMap[iBo.TargetAtom];
                IBond cBo = builder.NewBond(atO, atT);

                INCHI_BOND_TYPE type = iBo.BondType;
                if (type == INCHI_BOND_TYPE.Single)
                {
                    cBo.Order = BondOrder.Single;
                }
                else if (type == INCHI_BOND_TYPE.Double)
                {
                    cBo.Order = BondOrder.Double;
                }
                else if (type == INCHI_BOND_TYPE.Triple)
                {
                    cBo.Order = BondOrder.Triple;
                }
                else if (type == INCHI_BOND_TYPE.Altern)
                {
                    cBo.IsAromatic = true;
                }
                else
                {
                    throw new CDKException("Unknown bond type: " + type);
                }

                INCHI_BOND_STEREO stereo = iBo.BondStereo;

                // No stereo definition
                if (stereo == INCHI_BOND_STEREO.None)
                {
                    cBo.Stereo = BondStereo.None;
                }
                // Bond ending (fat end of wedge) below the plane
                else if (stereo == INCHI_BOND_STEREO.Single1Down)
                {
                    cBo.Stereo = BondStereo.Down;
                }
                // Bond ending (fat end of wedge) above the plane
                else if (stereo == INCHI_BOND_STEREO.Single1Up)
                {
                    cBo.Stereo = BondStereo.Up;
                }
                // Bond starting (pointy end of wedge) below the plane
                else if (stereo == INCHI_BOND_STEREO.Single2Down)
                {
                    cBo.Stereo = BondStereo.DownInverted;
                }
                // Bond starting (pointy end of wedge) above the plane
                else if (stereo == INCHI_BOND_STEREO.Single2Up)
                {
                    cBo.Stereo = BondStereo.UpInverted;
                }
                // Bond with undefined stereochemistry
                else if (stereo == INCHI_BOND_STEREO.Single1Either || stereo == INCHI_BOND_STEREO.DoubleEither)
                {
                    cBo.Stereo = BondStereo.None;
                }

                molecule.Bonds.Add(cBo);
            }

            for (int i = 0; i < output.Stereos.Count; i++)
            {
                NInchiStereo0D stereo0d = output.Stereos[i];
                if (stereo0d.StereoType == INCHI_STEREOTYPE.Tetrahedral
                        || stereo0d.StereoType == INCHI_STEREOTYPE.Allene)
                {
                    NInchiAtom central = stereo0d.CentralAtom;
                    NInchiAtom[] neighbours = stereo0d.Neighbors;

                    IAtom focus = inchiCdkAtomMap[central];
                    IAtom[] neighbors = new IAtom[]{inchiCdkAtomMap[neighbours[0]], inchiCdkAtomMap[neighbours[1]],
                            inchiCdkAtomMap[neighbours[2]], inchiCdkAtomMap[neighbours[3]]};
                    TetrahedralStereo stereo;

                    // as per JNI InChI doc even is clockwise and odd is
                    // anti-clockwise
                    if (stereo0d.Parity == INCHI_PARITY.Odd)
                    {
                        stereo = TetrahedralStereo.AntiClockwise;
                    }
                    else if (stereo0d.Parity == INCHI_PARITY.Even)
                    {
                        stereo = TetrahedralStereo.Clockwise;
                    }
                    else
                    {
                        // CDK Only supports parities of + or -
                        continue;
                    }

                    IReadOnlyStereoElement<IChemObject, IChemObject> stereoElement = null;

                    if (stereo0d.StereoType == INCHI_STEREOTYPE.Tetrahedral)
                    {
                        stereoElement = builder.NewTetrahedralChirality(focus, neighbors, stereo);
                    }
                    else if (stereo0d.StereoType == INCHI_STEREOTYPE.Allene)
                    {

                        // The periphals (p<i>) and terminals (t<i>) are refering to
                        // the following atoms. The focus (f) is also shown.
                        //
                        //   p0          p2
                        //    \          /
                        //     t0 = f = t1
                        //    /         \
                        //   p1         p3
                        IAtom[] peripherals = neighbors;
                        IAtom[] terminals = ExtendedTetrahedral.FindTerminalAtoms(molecule, focus);

                        // InChI always provides the terminal atoms t0 and t1 as
                        // periphals, here we find where they are and then add in
                        // the other explicit atom. As the InChI create hydrogens
                        // for stereo elements, there will always we an explicit
                        // atom that can be found - it may be optionally suppressed
                        // later.

                        // not much documentation on this (at all) but they appear
                        // to always be the middle two atoms (index 1, 2) we therefore
                        // test these first - but handle the other indices just in
                        // case
                        foreach (var terminal in terminals)
                        {
                            if (peripherals[1] == terminal)
                            {
                                peripherals[1] = FindOtherSinglyBonded(molecule, terminal, peripherals[0]);
                            }
                            else if (peripherals[2] == terminal)
                            {
                                peripherals[2] = FindOtherSinglyBonded(molecule, terminal, peripherals[3]);
                            }
                            else if (peripherals[0] == terminal)
                            {
                                peripherals[0] = FindOtherSinglyBonded(molecule, terminal, peripherals[1]);
                            }
                            else if (peripherals[3] == terminal)
                            {
                                peripherals[3] = FindOtherSinglyBonded(molecule, terminal, peripherals[2]);
                            }
                        }

                        stereoElement = new ExtendedTetrahedral(focus, peripherals, stereo);
                    }

                    Trace.Assert(stereoElement != null);
                    molecule.StereoElements.Add(stereoElement);
                }
                else if (stereo0d.StereoType == INCHI_STEREOTYPE.DoubleBond)
                {
                    NInchiAtom[] neighbors = stereo0d.Neighbors;

                    // from JNI InChI doc
                    //                            neighbor[4]  : {#X,#A,#B,#Y} in this order
                    // X                          central_atom : NO_ATOM
                    //  \            X        Y   type         : INCHI_StereoType_DoubleBond
                    //   A == B       \      /
                    //         \       A == B
                    //          Y
                    IAtom x = inchiCdkAtomMap[neighbors[0]];
                    IAtom a = inchiCdkAtomMap[neighbors[1]];
                    IAtom b = inchiCdkAtomMap[neighbors[2]];
                    IAtom y = inchiCdkAtomMap[neighbors[3]];

                    // XXX: AtomContainer is doing slow lookup
                    IBond stereoBond = molecule.GetBond(a, b);
                    stereoBond.SetAtoms(new[] { a, b }); // ensure a is first atom

                    DoubleBondConformation conformation = DoubleBondConformation.Unset;

                    switch (stereo0d.Parity)
                    {
                        case INCHI_PARITY.Odd:
                            conformation = DoubleBondConformation.Together;
                            break;
                        case INCHI_PARITY.Even:
                            conformation = DoubleBondConformation.Opposite;
                            break;
                    }

                    // unspecified not stored
                    if (conformation.IsUnset()) continue;

                    molecule.StereoElements.Add(new DoubleBondStereochemistry(stereoBond, new IBond[]{molecule.GetBond(x, a),
                            molecule.GetBond(b, y)}, conformation));
                }
                else
                {
                    // TODO - other types of atom parity - double bond, etc
                }
            }
        }
        
        /// <summary>
        /// Finds a neighbor attached to 'atom' that is singley bonded and isn't 'exclude'. If no such atom exists, the 'atom' is returned.
        /// </summary>
        /// <param name="container">a molecule container</param>
        /// <param name="atom">the atom to find the neighbor or</param>
        /// <param name="exclude">don't find this atom</param>
        /// <returns>the other atom (or 'atom')</returns>
        private static IAtom FindOtherSinglyBonded(IAtomContainer container, IAtom atom, IAtom exclude)
        {
            foreach (var bond in container.GetConnectedBonds(atom))
            {
                if (!BondOrder.Single.Equals(bond.Order) || bond.Contains(exclude)) continue;
                return bond.GetOther(atom);
            }
            return atom;
        }

        /// <summary>
        /// Generated molecule.
        /// </summary>
        public IAtomContainer AtomContainer => molecule;

        /// <summary>
        /// Return status from InChI process.  OKAY and WARNING indicate
        /// InChI has been generated, in all other cases InChI generation has failed.
        /// </summary>
        public INCHI_RET ReturnStatus => output.ReturnStatus;

        /// <summary>
        /// Generated (error/warning) messages.
        /// </summary>
        public string Message => output.Message;
        
        /// <summary>
        /// Generated log.
        /// </summary>
        public string Log => output.Log;

        /// <summary>
        /// Returns warning flags, see INCHIDIFF in inchicmp.h.
        /// 
        /// [x][y]:
        /// <list type="bullet">
        /// <item>x=0 =&gt; Reconnected if present in InChI otherwise Disconnected/Normal</item>
        /// <item>x=1 =&gt; Disconnected layer if Reconnected layer is present</item>
        /// <item>y=1 =&gt; Main layer or Mobile-H</item>
        /// <item>y=0 =&gt; Fixed-H layer</item>
        /// </list>
        /// </summary>
        public ulong[,] WarningFlags => output.WarningFlags;
    }
}
