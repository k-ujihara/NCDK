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
    /// <summary>
    /// <p>Type-safe enumeration of InChI return codes.
    ///
    /// <p>InChI library return values:<br>
    /// <tt>
    /// SKIP     (-2)    Not used in InChI library<br>
    /// EOF      (-1)    No structural data has been provided<br>
    /// OKAY     (0)     Success, no errors or warnings<br>
    /// WARNING  (1)     Success, Warning(s) issued<br>
    /// ERROR    (2)     Error: no InChI has been created<br>
    /// FATAL    (3)     Severe error: no InChI has been created (typically,
    ///                  memory allocation failure)<br>
    /// Unknown  (4)     Unknown program error<br>
    /// BUSY     (5)     Previous call to InChI has not returned yet<br>
    /// </tt>
    /// <p>See <tt>inchi_api.h</tt>.
    // @author Sam Adams
    /// </summary>
    public enum INCHI_RET
    {

        /// <summary>
        /// Not used in InChI library.
        /// </summary>
        SKIP = -2,

        /// <summary>
        /// No structural data has been provided.
        /// </summary>
        EOF = -1,

        /// <summary>
        /// Success; no errors or warnings.
        /// </summary>
        OKAY = 0,

        /// <summary>
        /// Success; Warning(s) issued.
        /// </summary>
        WARNING = 1,

        /// <summary>
        /// Error: no InChI has been created.
        /// </summary>
        ERROR = 2,

        /// <summary>
        /// Severe error: no InChI has been created (typically, memory allocation failure).
        /// </summary>
        FATAL = 3,

        /// <summary>
        /// Unknown program error.
        /// </summary>
        Unknown = 4,

        /// <summary>
        /// Previuos call to InChI has not returned yet.
        /// </summary>
        BUSY = 5
    }
}
