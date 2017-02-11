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
using System.Collections.Generic;

namespace NCDK.NInChI
{
    /**
     * Encapsulates structure input for InChI generation.
     * @author Sam Adams
     */
    public class NInchiInput : NInchiStructure
    {
        /**
         * Options string,
         */
        public string Options {
            get;
#if !TEST
            protected 
#endif
            set;
        }

        /**
         * Constructor.
         * @
         */
        public NInchiInput()
        {
            Options = "";
        }

        /**
         * Constructor.
         * @param opts    Options string.
         * @
         */
        public NInchiInput(string opts)
        {
            Options = opts == null ? "" : NInchiWrapper.CheckOptions(opts);
        }

        /**
         * Constructor.
         * @param opts    List of options.
         * @
         */
        public NInchiInput(IList<INCHI_OPTION> opts)
        {
            Options = NInchiWrapper.CheckOptions(opts);
        }

        /**
         * Constructor.
         * @
         */
        public NInchiInput(NInchiStructure struct_)
            : this()
        {
            SetStructure(struct_);
        }

        /**
         * Constructor.
         * @
         */
        public NInchiInput(NInchiStructure struct_, string opts)
            : this(opts)
        {
            SetStructure(struct_);
        }
    }
}
