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
    /**
     * Class that handles execution time of the MCS search.
     * @cdk.module smsd
     * @cdk.githash
     * @author Syed Asad Rahman <asad@ebi.ac.uk>
     */
    public class TimeManager
    {

        private DateTime startTime;
        //private SimpleDateFormat dateFormat;

        /**
         * Constructor for storing execution time
         */
        public TimeManager()
        {

            //dateFormat = new SimpleDateFormat("HH:mm:ss");

            //dateFormat.SetTimeZone(TimeZone.GetTimeZone("GMT"));
            startTime = DateTime.Now;
        }

        /**
         * Returns Elapsed Time In Hours
         * @return Elapsed Time In Hours
         */
        public double GetElapsedTimeInHours()
        {
            var currentTime = DateTime.Now;

            return (currentTime - startTime).Ticks / (60.0 * 60 * 1000 * 10000);

        }

        /**
         * Returns Elapsed Time In Minutes
         * @return Elapsed Time In Minutes
         */
        public double GetElapsedTimeInMinutes()
        {

            //long diffSeconds = diff / 1000;
            //long diffMinutes = diff / (60 * 1000);
            //long diffHours = diff / (60 * 60 * 1000);
            //long diffDays = diff / (24 * 60 * 60 * 1000);

            var currentTime = DateTime.Now;
            return (currentTime - startTime).Ticks / (60.0 * 1000 * 10000);

        }

        /**
         * Return Elapsed Time In Seconds
         * @return Elapsed Time In Seconds
         */
        public double GetElapsedTimeInSeconds()
        {
            var currentTime = DateTime.Now;
            return ((currentTime - startTime).Ticks / (1000.0 * 10000));

        }

        /**
         * Returns Elapsed Time In Mill Seconds
         * @return Elapsed Time In Mill Seconds
         */
        public double GetElapsedTimeInMilliSeconds()
        {
            var currentTime = DateTime.Now;
            return (currentTime - startTime).Ticks / 10000.0;
        }
    }
}
