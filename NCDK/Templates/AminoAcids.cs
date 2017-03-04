/* Copyright (C) 2005-2007  Martin Eklund <martin.eklund@farmbio.uu.se>
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
using NCDK.Common.Collections;
using NCDK.Default;
using NCDK.Dict;
using NCDK.IO;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace NCDK.Templates
{
    /// <summary>
    /// Tool that provides templates for the (natural) amino acids.
    ///
    // @author      Martin Eklund <martin.eklund@farmbio.uu.se>
    // @cdk.module  pdb
    // @cdk.githash
    // @cdk.keyword templates
    // @cdk.keyword amino acids, stuctures
    // @cdk.created 2005-02-08
    /// </summary>
    public class AminoAcids
    {
        private static object syncLock = new object();

        /// <summary>
        /// Creates matrix with info about the bonds in the amino acids.
        /// 0 = bond id, 1 = atom1 in bond, 2 = atom2 in bond, 3 = bond order.
        /// <returns>info</returns>
        /// </summary>
        public static int[][] CreateAABondInfo()
        {
            if (aminoAcids == null)
            {
                CreateAAs();
            }

            int[][] info = Arrays.CreateJagged<int>(153, 4);

            int counter = 0;
            int total = 0;
            for (int aa = 0; aa < aminoAcids.Length; aa++)
            {
                AminoAcid acid = aminoAcids[aa];

                total += acid.Bonds.Count;
                Debug.WriteLine("total #bonds: ", total);

                foreach (var bond in acid.Bonds)
                {
                    info[counter][0] = counter;
                    info[counter][1] = acid.Atoms.IndexOf(bond.Atoms[0]);
                    info[counter][2] = acid.Atoms.IndexOf(bond.Atoms[1]);
                    info[counter][3] = bond.Order.Numeric;
                    counter++;
                }
            }

            if (counter > 153)
            {
                Trace.TraceError("Error while creating AA info! Bond count is too large: ", counter);
                return null;
            }

            return info;
        }

        private static AminoAcid[] aminoAcids = null;

        public const string RESIDUE_NAME = "residueName";
        public const string RESIDUE_NAME_SHORT = "residueNameShort";
        public const string NO_ATOMS = "noOfAtoms";
        public const string NO_BONDS = "noOfBonds";
        public const string ID = "id";

        /// <summary>
        /// Creates amino acid AminoAcid objects.
        ///
        /// <returns>aminoAcids, a Dictionary containing the amino acids as AminoAcids.</returns>
        /// </summary>
        public static AminoAcid[] CreateAAs()
        {
            if (aminoAcids != null)
            {
                return aminoAcids;
            }
            lock (syncLock)
            {
                if (aminoAcids != null)
                {
                    return aminoAcids;
                }

                // Create set of AtomContainers
                aminoAcids = new AminoAcid[20];

                IChemFile list = new ChemFile();
                CMLReader reader = new CMLReader(ResourceLoader.GetAsStream("NCDK.Templates.Data.list_aminoacids.cml"));
                try
                {
                    list = (IChemFile)reader.Read(list);
                    var containersList = ChemFileManipulator.GetAllAtomContainers(list);
                    int counter = 0;
                    foreach (var ac in containersList)
                    {
                        Debug.WriteLine("Adding AA: ", ac);
                        // convert into an AminoAcid
                        AminoAcid aminoAcid = new AminoAcid();
                        foreach (var next in ac.GetProperties().Keys)
                        {
                            Debug.WriteLine("Prop: " + next.ToString());
                            if (next is DictRef)
                            {
                                DictRef dictRef = (DictRef)next; 
                                // Debug.WriteLine("DictRef type: " + dictRef.Type);
                                if (dictRef.Type.Equals("pdb:residueName"))
                                {
                                    aminoAcid.SetProperty(RESIDUE_NAME, ac.GetProperty<string>(next).ToUpperInvariant());
                                    aminoAcid.MonomerName = ac.GetProperty<string>(next);
                                }
                                else if (dictRef.Type.Equals("pdb:oneLetterCode"))
                                {
                                    aminoAcid.SetProperty(RESIDUE_NAME_SHORT, ac.GetProperty<string>(next));
                                }
                                else if (dictRef.Type.Equals("pdb:id"))
                                {
                                    aminoAcid.SetProperty(ID, ac.GetProperty<string>(next));
                                    Debug.WriteLine("Set AA ID to: ", ac.GetProperty<string>(next));
                                }
                                else
                                {
                                    Trace.TraceError("Cannot deal with dictRef!");
                                }
                            }
                        }
                        foreach (var atom in ac.Atoms)
                        {
                            string dictRef = atom.GetProperty<string>("org.openscience.cdk.dict");
                            if (dictRef != null && dictRef.Equals("pdb:nTerminus"))
                            {
                                aminoAcid.AddNTerminus(atom);
                            }
                            else if (dictRef != null && dictRef.Equals("pdb:cTerminus"))
                            {
                                aminoAcid.AddCTerminus(atom);
                            }
                            else
                            {
                                aminoAcid.Atoms.Add(atom);
                            }
                        }
                        foreach (var bond in ac.Bonds)
                        {
                            aminoAcid.Bonds.Add(bond);
                        }
                        AminoAcidManipulator.RemoveAcidicOxygen(aminoAcid);
                        aminoAcid.SetProperty(NO_ATOMS, "" + aminoAcid.Atoms.Count);
                        aminoAcid.SetProperty(NO_BONDS, "" + aminoAcid.Bonds.Count);
                        if (counter < aminoAcids.Length)
                        {
                            aminoAcids[counter] = aminoAcid;
                        }
                        else
                        {
                            Trace.TraceError("Could not store AminoAcid! Array too short!");
                        }
                        counter++;
                    }
                    reader.Close();
                }
                catch (Exception exception)
                {
                    if (exception is CDKException | exception is IOException)
                    {
                        Trace.TraceError("Failed reading file: ", exception.Message);
                        Debug.WriteLine(exception);
                    }
                    else
                        throw;
                }

                return aminoAcids;
            }
        }

        /// <summary>
        /// Returns a Dictionary where the key is one of G, A, V, L, I, S, T, C, M, D,
        /// N, E, Q, R, K, H, F, Y, W and P.
        /// </summary>
        public static IDictionary<string, IAminoAcid> GetHashMapBySingleCharCode()
        {
            IAminoAcid[] monomers = CreateAAs();
            Dictionary<string, IAminoAcid> map = new Dictionary<string, IAminoAcid>();
            for (int i = 0; i < monomers.Length; i++)
            {
                map[monomers[i].GetProperty<string>(RESIDUE_NAME_SHORT)] = monomers[i];
            }
            return map;
        }

        /// <summary>
        /// Returns a Dictionary where the key is one of GLY, ALA, VAL, LEU, ILE, SER,
        /// THR, CYS, MET, ASP, ASN, GLU, GLN, ARG, LYS, HIS, PHE, TYR, TRP AND PRO.
        /// </summary>
        public static IDictionary<string, IAminoAcid> GetHashMapByThreeLetterCode()
        {
            AminoAcid[] monomers = CreateAAs();
            IDictionary<string, IAminoAcid> map = new Dictionary<string, IAminoAcid>();
            for (int i = 0; i < monomers.Length; i++)
            {
                map[monomers[i].GetProperty<string>(RESIDUE_NAME)] = monomers[i];
            }
            return map;
        }

        /// <summary>
        /// Returns the one letter code of an amino acid given a three letter code.
        /// For example, it will return "V" when "Val" was passed.
        /// </summary>
        public static string ConvertThreeLetterCodeToOneLetterCode(string threeLetterCode)
        {
            AminoAcid[] monomers = CreateAAs();
            for (int i = 0; i < monomers.Length; i++)
            {
                if (monomers[i].GetProperty<string>(RESIDUE_NAME).Equals(threeLetterCode))
                {
                    return monomers[i].GetProperty<string>(RESIDUE_NAME_SHORT);
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the three letter code of an amino acid given a one letter code.
        /// For example, it will return "Val" when "V" was passed.
        /// </summary>
        public static string ConvertOneLetterCodeToThreeLetterCode(string oneLetterCode)
        {
            AminoAcid[] monomers = CreateAAs();
            for (int i = 0; i < monomers.Length; i++)
            {
                if (monomers[i].GetProperty<string>(RESIDUE_NAME_SHORT).Equals(oneLetterCode))
                {
                    return monomers[i].GetProperty<string>(RESIDUE_NAME);
                }
            }
            return null;
        }
    }
}
