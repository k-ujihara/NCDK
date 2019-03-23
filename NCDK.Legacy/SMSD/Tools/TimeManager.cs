/* Copyright (C) 2006-2010  Syed Asad Rahman <asad@ebi.ac.uk>
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

using System;

namespace NCDK.SMSD.Tools
{
    /// <summary>
    /// Class that handles execution time of the MCS search.
    /// </summary>
    // @cdk.module smsd
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    [Obsolete("SMSD has been deprecated from the CDK with a newer, more recent version of SMSD is available at http://github.com/asad/smsd .")]
    public class TimeManager
    {
        private readonly DateTime startTime;
        //private SimpleDateFormat dateFormat;

        /// <summary>
        /// Constructor for storing execution time
        /// </summary>
        public TimeManager()
        {
            //dateFormat = new SimpleDateFormat("HH:mm:ss");

            //dateFormat.SetTimeZone(TimeZone.GetTimeZone("GMT"));
            startTime = DateTime.Now;
        }

        /// <summary>
        /// Returns Elapsed Time In Hours
        /// <returns>Elapsed Time In Hours</returns>
        /// </summary>
        public double GetElapsedTimeInHours()
        {
            var currentTime = DateTime.Now;

            return (currentTime - startTime).Ticks / (60.0 * 60 * 1000 * 10000);
        }

        /// <summary>
        /// Returns Elapsed Time In Minutes
        /// <returns>Elapsed Time In Minutes</returns>
        /// </summary>
        public double GetElapsedTimeInMinutes()
        {
            //long diffSeconds = diff / 1000;
            //long diffMinutes = diff / (60 * 1000);
            //long diffHours = diff / (60 * 60 * 1000);
            //long diffDays = diff / (24 * 60 * 60 * 1000);

            var currentTime = DateTime.Now;
            return (currentTime - startTime).Ticks / (60.0 * 1000 * 10000);
        }

        /// <summary>
        /// Return Elapsed Time In Seconds
        /// <returns>Elapsed Time In Seconds</returns>
        /// </summary>
        public double GetElapsedTimeInSeconds()
        {
            var currentTime = DateTime.Now;
            return ((currentTime - startTime).Ticks / (1000.0 * 10000));
        }

        /// <summary>
        /// Returns Elapsed Time In Mill Seconds
        /// <returns>Elapsed Time In Mill Seconds</returns>
        /// </summary>
        public double GetElapsedTimeInMilliSeconds()
        {
            var currentTime = DateTime.Now;
            return (currentTime - startTime).Ticks / 10000.0;
        }
    }
}
