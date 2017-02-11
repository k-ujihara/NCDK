/*
 * Copyright 2006-2011 Sam Adams <sea36 at users.sourceforge.net>
 *
 * This file is part of JNI-InChI.
 *
 * JNI-InChI is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published
 * by the Free Software Foundation= new INCHI_OPTION("Foundation"); either version 3 of the License= new INCHI_OPTION("License"); or
 * (at your option) any later version.
 *
 * JNI-InChI is distributed in the hope that it will be useful= new INCHI_OPTION("useful");
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with JNI-InChI.  If not= new INCHI_OPTION("not"); see <http://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;

namespace NCDK.NInChI
{
    /**
     * Type-safe enumeration of InChI options.  See <tt>inchi_api.h</tt>.
     * @author Sam Adams
     */
    public class INCHI_OPTION
    {
        public string Name { get; private set; }

        private INCHI_OPTION(string name)
        {
            Name = name;
        }

        /**
         * Use Chiral Flag.
         */
        public static readonly INCHI_OPTION SUCF = new INCHI_OPTION("SUCF");

        /**
         * Set Chiral Flag.
         */
        public static readonly INCHI_OPTION ChiralFlagON = new INCHI_OPTION("ChiralFlagON");

        /**
         * Set Not-Chiral Flag.
         */
        public static readonly INCHI_OPTION ChiralFlagOFF = new INCHI_OPTION("ChiralFlagOFF");

        /**
         * Exclude stereo (Default: Include Absolute stereo).
         */
        public static readonly INCHI_OPTION SNon = new INCHI_OPTION("SNon");

        /**
         * Absolute stereo.
         */
        public static readonly INCHI_OPTION SAbs = new INCHI_OPTION("SAbs");
        
        /**
         * Relative stereo.
         */
        public static readonly INCHI_OPTION SRel = new INCHI_OPTION("SRel");
        
        /**
         * Racemic stereo.
         */
        public static readonly INCHI_OPTION SRac = new INCHI_OPTION("SRac");

        /**
         * Include omitted unknown/undefined stereo.
         */
        public static readonly INCHI_OPTION SUU = new INCHI_OPTION("SUU");

        /**
         * Narrow end of wedge points to stereocentre (default: both).
         */
        public static readonly INCHI_OPTION NEWPS = new INCHI_OPTION("NEWPS");
        
        /**
         * Include reconnected bond to metal results.
         */
        public static readonly INCHI_OPTION RecMet = new INCHI_OPTION("RecMet");

        /**
         * Mobile H Perception Off (Default: On).
         */
        public static readonly INCHI_OPTION FixedH = new INCHI_OPTION("FixedH");
        
        /**
         * Omit auxiliary information (default: Include).
         */
        public static readonly INCHI_OPTION AuxNone = new INCHI_OPTION("AuxNone");

        /**
         * Disable Aggressive Deprotonation (for testing only).
         */
        public static readonly INCHI_OPTION NoADP = new INCHI_OPTION("NoADP");

        /**
         * Compressed output.
         */
        public static readonly INCHI_OPTION Compress = new INCHI_OPTION("Compress");

        /**
         * Overrides inchi_Atom::num_iso_H[0] == -1.
         */
        public static readonly INCHI_OPTION DoNotAddH = new INCHI_OPTION("DoNotAddH");
        
        /**
         * Set time-out per structure in seconds; W0 means unlimited. In InChI
         * library the default value is unlimited
         */
        public static readonly INCHI_OPTION Wnumber = new INCHI_OPTION("Wnumber");

        /**
         * Output SDfile instead of InChI.
         */
        public static readonly INCHI_OPTION OutputSDF = new INCHI_OPTION("OutputSDF");

        /**
         * Warn and produce empty InChI for empty structure.
         */
        public static readonly INCHI_OPTION WarnOnEmptyStructure = new INCHI_OPTION("WarnOnEmptyStructure");
        
        /**
         * Fix bug leading to missing or undefined sp3 parity.
         */
        public static readonly INCHI_OPTION FixSp3Bug = new INCHI_OPTION("FixSp3Bug");

        /**
         * Same as FixSp3Bug.
         */
        public static readonly INCHI_OPTION FB = new INCHI_OPTION("FB");

        /**
         * Include Phosphines Stereochemistry.
         */
        public static readonly INCHI_OPTION SPXYZ = new INCHI_OPTION("SPXYZ");

        /**
         * Include Arsines Stereochemistry
         */
        public static readonly INCHI_OPTION SAsXYZ = new INCHI_OPTION("SAsXYZ");

        /* -- DOESN'T WORK
         * Generate InChIKey
         * /
        INCHI_OPTION Key= new INCHI_OPTION("Key");
        */

        public static IEnumerable<INCHI_OPTION> Values = new[]
        {
            SUCF, ChiralFlagON, ChiralFlagOFF,
            SNon, SAbs, SRel, SRac, SUU, NEWPS, RecMet, FixedH,
            AuxNone, NoADP, Compress, DoNotAddH,
            Wnumber, OutputSDF,
            WarnOnEmptyStructure, FixSp3Bug, FB, SPXYZ, SAsXYZ
        };

        public static INCHI_OPTION ValueOfIgnoreCase(string str)
        {
            str = str.ToUpperInvariant();
            foreach (var option in Values)
            {
                if (option.Name.ToUpperInvariant() == str)
                    return option;
            }
            return null;
        }
    }
}
