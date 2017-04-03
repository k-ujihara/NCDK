/*
 * Copyright (c) 2013, European Bioinformatics Institute (EMBL-EBI)
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice, this
 *    list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * Any EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
 * Any DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON Any THEORY OF LIABILITY, WHETHER IN CONTRACT, Strict LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN Any WAY OUT OF THE USE OF THIS
 * SOFTWARE, Even IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 * The views and conclusions contained in the software and documentation are those
 * of the authors and should not be interpreted as representing official policies,
 * either expressed or implied, of the FreeBSD Project.
 */

using System;
using System.Collections.Generic;

namespace NCDK.Beam
{
    /// <summary>
    /// Enumeration of atom-based relative configurations. Each value defines a
    /// configuration of a given topology.
    /// </summary>
    /// <seealso href="http://www.opensmiles.org/opensmiles.html#chirality">Chirality, OpenSMILES</seealso>
    // @author John May
    public sealed class Configuration
    {
        /// <summary>An atoms has Unknown/no configuration. </summary>
        public static readonly Configuration Unknown = new Configuration(Types.None, "");

        /// <summary>Shorthand for TH1, AL1, DB1, TB1 or OH1 configurations. </summary>
        public static readonly Configuration AntiClockwise = new Configuration(Types.Implicit, "@");

        /// <summary>Shorthand for TH2, AL2, DB2, TB2 or OH2 configurations. </summary>
        public static readonly Configuration Clockwise = new Configuration(Types.Implicit, "@@");

        /// <summary>
        /// Tetrahedral, neighbors proceed anti-clockwise looking from the first
        /// atom.
        /// </summary>
        public static readonly Configuration TH1 = new Configuration(Types.Tetrahedral, "@TH1", AntiClockwise);

        /// <summary>Tetrahedral, neighbors proceed clockwise looking from the first atom. </summary>
        public static readonly Configuration TH2 = new Configuration(Types.Tetrahedral, "@TH2", Clockwise);

        /// <summary>
        /// Atom-based double bond configuration, neighbors proceed anti-clockwise in
        /// a plane. <i>Note - this configuration is currently specific to
        /// grins.</i>
        /// </summary>
        public static readonly Configuration DB1 = new Configuration(Types.DoubleBond, "@DB1", AntiClockwise);

        /// <summary>
        /// Atom-based double bond configuration, neighbors proceed clockwise in a
        /// plane.<i>Note - this configuration is currently specific to grins.</i>
        /// </summary>
        public static readonly Configuration DB2 = new Configuration(Types.DoubleBond, "@DB2", Clockwise);

        // extended tetrahedral, allene-like = new Configuration(Sp)
        public static readonly Configuration AL1 = new Configuration(Types.ExtendedTetrahedral, "@AL1", AntiClockwise);
        public static readonly Configuration AL2 = new Configuration(Types.ExtendedTetrahedral, "@AL2", Clockwise);

        // square planar
        public static readonly Configuration SP1 = new Configuration(Types.SquarePlanar, "@SP1");
        public static readonly Configuration SP2 = new Configuration(Types.SquarePlanar, "@SP2");
        public static readonly Configuration SP3 = new Configuration(Types.SquarePlanar, "@SP3");

        // trigonal bipyramidal
        public static readonly Configuration TB1 = new Configuration(Types.TrigonalBipyramidal, "@TB1", AntiClockwise);
        public static readonly Configuration TB2 = new Configuration(Types.TrigonalBipyramidal, "@TB2", Clockwise);
        public static readonly Configuration TB3 = new Configuration(Types.TrigonalBipyramidal, "@TB3");
        public static readonly Configuration TB4 = new Configuration(Types.TrigonalBipyramidal, "@TB4");
        public static readonly Configuration TB5 = new Configuration(Types.TrigonalBipyramidal, "@TB5");
        public static readonly Configuration TB6 = new Configuration(Types.TrigonalBipyramidal, "@TB6");
        public static readonly Configuration TB7 = new Configuration(Types.TrigonalBipyramidal, "@TB7");
        public static readonly Configuration TB8 = new Configuration(Types.TrigonalBipyramidal, "@TB8");
        public static readonly Configuration TB9 = new Configuration(Types.TrigonalBipyramidal, "@TB9");
        public static readonly Configuration TB10 = new Configuration(Types.TrigonalBipyramidal, "@TB10");
        public static readonly Configuration TB11 = new Configuration(Types.TrigonalBipyramidal, "@TB11");
        public static readonly Configuration TB12 = new Configuration(Types.TrigonalBipyramidal, "@TB12");
        public static readonly Configuration TB13 = new Configuration(Types.TrigonalBipyramidal, "@TB13");
        public static readonly Configuration TB14 = new Configuration(Types.TrigonalBipyramidal, "@TB14");
        public static readonly Configuration TB15 = new Configuration(Types.TrigonalBipyramidal, "@TB15");
        public static readonly Configuration TB16 = new Configuration(Types.TrigonalBipyramidal, "@TB16");
        public static readonly Configuration TB17 = new Configuration(Types.TrigonalBipyramidal, "@TB17");
        public static readonly Configuration TB18 = new Configuration(Types.TrigonalBipyramidal, "@TB18");
        public static readonly Configuration TB19 = new Configuration(Types.TrigonalBipyramidal, "@TB19");
        public static readonly Configuration TB20 = new Configuration(Types.TrigonalBipyramidal, "@TB20");

        // octahedral
        public static readonly Configuration OH1 = new Configuration(Types.Octahedral, "@OH1", AntiClockwise);
        public static readonly Configuration OH2 = new Configuration(Types.Octahedral, "@OH2", Clockwise);
        public static readonly Configuration OH3 = new Configuration(Types.Octahedral, "@OH3");
        public static readonly Configuration OH4 = new Configuration(Types.Octahedral, "@OH4");
        public static readonly Configuration OH5 = new Configuration(Types.Octahedral, "@OH5");
        public static readonly Configuration OH6 = new Configuration(Types.Octahedral, "@OH6");
        public static readonly Configuration OH7 = new Configuration(Types.Octahedral, "@OH7");
        public static readonly Configuration OH8 = new Configuration(Types.Octahedral, "@OH8");
        public static readonly Configuration OH9 = new Configuration(Types.Octahedral, "@OH9");
        public static readonly Configuration OH10 = new Configuration(Types.Octahedral, "@OH10");
        public static readonly Configuration OH11 = new Configuration(Types.Octahedral, "@OH11");
        public static readonly Configuration OH12 = new Configuration(Types.Octahedral, "@OH12");
        public static readonly Configuration OH13 = new Configuration(Types.Octahedral, "@OH13");
        public static readonly Configuration OH14 = new Configuration(Types.Octahedral, "@OH14");
        public static readonly Configuration OH15 = new Configuration(Types.Octahedral, "@OH15");
        public static readonly Configuration OH16 = new Configuration(Types.Octahedral, "@OH16");
        public static readonly Configuration OH17 = new Configuration(Types.Octahedral, "@OH17");
        public static readonly Configuration OH18 = new Configuration(Types.Octahedral, "@OH18");
        public static readonly Configuration OH19 = new Configuration(Types.Octahedral, "@OH19");
        public static readonly Configuration OH20 = new Configuration(Types.Octahedral, "@OH20");
        public static readonly Configuration OH21 = new Configuration(Types.Octahedral, "@OH21");
        public static readonly Configuration OH22 = new Configuration(Types.Octahedral, "@OH22");
        public static readonly Configuration OH23 = new Configuration(Types.Octahedral, "@OH23");
        public static readonly Configuration OH24 = new Configuration(Types.Octahedral, "@OH24");
        public static readonly Configuration OH25 = new Configuration(Types.Octahedral, "@OH25");
        public static readonly Configuration OH26 = new Configuration(Types.Octahedral, "@OH26");
        public static readonly Configuration OH27 = new Configuration(Types.Octahedral, "@OH27");
        public static readonly Configuration OH28 = new Configuration(Types.Octahedral, "@OH28");
        public static readonly Configuration OH29 = new Configuration(Types.Octahedral, "@OH29");
        public static readonly Configuration OH30 = new Configuration(Types.Octahedral, "@OH30");

        public static readonly IEnumerable<Configuration> Values = new[] { Unknown }; // TODO:

        /// <summary>Type of configuration. </summary>
        private readonly Types type;

        /// <summary>Symbol used to represent configuration </summary>
        private readonly string symbol;

        /// <summary>Shorthand - often converted to this in output </summary>
        private readonly Configuration shorthand;

        /// <summary>Lookup tables for trigonal bipyramidal and octahedral </summary>
        private static readonly Configuration[] tbs = new Configuration[21];
        private static readonly Configuration[] ohs = new Configuration[31];

        // initialise trigonal lookup
        static Configuration()
        {
            {
                int i = 1;
                foreach (var config in Values)
                {
                    if (config.Type.Equals(Types.TrigonalBipyramidal))
                        tbs[i++] = config;
                }
            }

            // initialise octahedral lookup
            {
                int i = 1;
                foreach (var config in Values)
                {
                    if (config.Type.Equals(Types.Octahedral))
                        ohs[i++] = config;
                }
            }
        }

        private Configuration(Types type, string symbol, Configuration shorthand)
        {
            this.type = type;
            this.symbol = symbol;
            this.shorthand = shorthand;
        }

        private Configuration(Types type, string symbol)
        {
            this.type = type;
            this.symbol = symbol;
            this.shorthand = this;
        }

        /// <summary>
        /// Access the shorthand for the configuration, if no shorthand is defined
        /// <see cref="Unknown"/> is returned.
        /// </summary>
        /// <value>the shorthand '@' or '@@'</value>
        public Configuration Shorthand => shorthand;

        /// <summary>
        /// Symbol of the chiral configuration.
        /// </summary>
        public string Symbol => symbol;

        /// <summary>
        /// The general type of relative configuration this represents.
        /// </summary>
        /// <returns>type of the configuration</returns>
        /// <seealso cref="Type"/>
        public Types Type => type;

        /// <summary>
        /// Read a chiral configuration from a character buffer and progress the
        /// buffer. If there is no configuration then <see cref="Unknown"/>
        /// is returned. Encountering an invalid permutation designator (e.g.
        /// &#64;TB21) or incomplete class (e.g. &#64;T) will throw an invalid smiles
        /// exception.
        /// </summary>
        /// <param name="buffer">a character buffer</param>
        /// <returns>the configuration</returns>
        internal static Configuration Read(CharBuffer buffer)
        {
            if (buffer.GetIf('@'))
            {
                if (buffer.GetIf('@'))
                {
                    return Configuration.Clockwise;
                }
                else if (buffer.GetIf('T'))
                {
                    // TH (tetrahedral) or TB (trigonal bipyramidal)
                    if (buffer.GetIf('H'))
                    {
                        if (buffer.GetIf('1'))
                            return Configuration.TH1;
                        else if (buffer.GetIf('2'))
                            return Configuration.TH2;
                        else
                            throw new InvalidSmilesException("invalid permutation designator for @TH, valid values are @TH1 or @TH2:",
                                                             buffer);
                    }
                    else if (buffer.GetIf('B'))
                    {
                        int num = buffer.GetNumber();
                        if (num < 1 || num > 20)
                            throw new InvalidSmilesException("invalid permutation designator for @TB, valid values are '@TB1, @TB2, ... @TB20:'",
                                                             buffer);
                        return tbs[num];
                    }
                    throw new InvalidSmilesException("'@T' is not a valid chiral specification:", buffer);
                }
                else if (buffer.GetIf('D'))
                {
                    // DB (double bond)
                    if (buffer.GetIf('B'))
                    {
                        if (buffer.GetIf('1'))
                            return Configuration.DB1;
                        else if (buffer.GetIf('2'))
                            return Configuration.DB2;
                        else
                            throw new InvalidSmilesException("invalid permutation designator for @DB, valid values are @DB1 or @DB2:",
                                                             buffer);
                    }
                    throw new InvalidSmilesException("'@D' is not a valid chiral specification:", buffer);
                }
                else if (buffer.GetIf('A'))
                {
                    // allene (extended tetrahedral)
                    if (buffer.GetIf('L'))
                    {
                        if (buffer.GetIf('1'))
                            return Configuration.AL1;
                        else if (buffer.GetIf('2'))
                            return Configuration.AL2;
                        else
                            throw new InvalidSmilesException("invalid permutation designator for @AL, valid values are '@AL1 or @AL2':", buffer);
                    }
                    else
                    {
                        throw new InvalidSmilesException("'@A' is not a valid chiral specification:", buffer);
                    }
                }
                else if (buffer.GetIf('S'))
                {
                    // square planar
                    if (buffer.GetIf('P'))
                    {
                        if (buffer.GetIf('1'))
                            return Configuration.SP1;
                        else if (buffer.GetIf('2'))
                            return Configuration.SP2;
                        else if (buffer.GetIf('3'))
                            return Configuration.SP3;
                        else
                            throw new InvalidSmilesException("invalid permutation designator for @SP, valid values are '@SP1, @SP2 or @SP3':",
                                                             buffer);
                    }
                    else
                    {
                        throw new InvalidSmilesException("'@S' is not a valid chiral specification:", buffer);
                    }
                }
                else if (buffer.GetIf('O'))
                {
                    if (buffer.GetIf('H'))
                    {
                        // octahedral
                        int num = buffer.GetNumber();
                        if (num < 1 || num > 30)
                            throw new InvalidSmilesException("invalid permutation designator for @OH, valud values are '@OH1, @OH2, ... @OH30':", buffer);
                        return ohs[num];
                    }
                    else
                    {
                        throw new InvalidSmilesException("'@O' is not a valid chiral specification:", buffer);
                    }
                }
                else
                {
                    return Configuration.AntiClockwise;
                }
            }
            return Unknown;
        }

        /// <summary>Types of configuration.</summary>
        public enum Types
        {
            None,
            Implicit,
            Tetrahedral,
            DoubleBond,
            ExtendedTetrahedral,
            SquarePlanar,
            TrigonalBipyramidal,
            Octahedral
        }

        /// <summary>Configurations for double-bond bond-based specification. </summary>
        public enum DoubleBond
        {
            Unspecified,
            Together,
            Opposite
        }
    }
}
