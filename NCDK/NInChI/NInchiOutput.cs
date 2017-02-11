/*
 * Copyright 2006-2011 Sam Adams <sea36 at users.sourceforge.net>
 *
 * This file is part of JNI-InChI.
 *
 * JNI-InChI is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * JNI-InChI is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with JNI-InChI.  If not, see <http://www.gnu.org/licenses/>.
 */
namespace NCDK.NInChI
{
    /**
     * Encapsulates output from InChI generation.
     * @author Sam Adams
     */
    public class NInchiOutput
    {
        public INCHI_RET ReturnStatus { get; protected internal set; }

        /**
         * InChI ASCIIZ string
         */
        public string Inchi { get; protected internal set; }

        /**
         * Aux info ASCIIZ string
         */
        public string AuxInfo { get; protected internal set; }

        /**
         * Error/warning ASCIIZ message
         */
        public string Message { get; protected internal set; }

        /**
         * log-file ASCIIZ string, contains a human-readable list of recognized
         * options and possibly an Error/warning message
         */
        public string Log { get; protected internal set; }


        public NInchiOutput(int ret, string inchi, string auxInfo, string message, string log)
            : this((INCHI_RET)ret, inchi, auxInfo, message, log)
        {
        }

        public NInchiOutput(INCHI_RET ret, string inchi, string auxInfo, string message, string log)
        {
            ReturnStatus = ret;
            Inchi = inchi;
            AuxInfo = auxInfo;
            Message = message;
            Log = log;
        }
      
        public override string ToString()
        {
            return "InChI_Output: " + ReturnStatus + "/" + Inchi + "/" + AuxInfo + "/" + Message + "/" + Log;
        }
    }
}
