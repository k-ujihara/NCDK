/* Copyright (C) 1997-2007,2014  Egon Willighagen <egonw@users.sf.net>
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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using System;
using System.Text;

namespace NCDK.IO.CML
{
    /**
     * Low weight alternative to Sun's Stack class.
     *
     * @cdk.module io
     * @cdk.githash
     *
     * @cdk.keyword stack
     */
    public class CMLModuleStack
    {
        ICMLModule[] stack = new ICMLModule[64];
        int sp = 0;

        /**
         * Adds an entry to the stack.
         */
        public void Push(ICMLModule item)
        {
            if (sp == stack.Length)
            {
                ICMLModule[] temp = new ICMLModule[2 * sp];
                Array.Copy(stack, 0, temp, 0, sp);
                stack = temp;
            }
            stack[sp++] = item;
        }

        public int Length => sp;

        /**
         * Retrieves and deletes to last added entry.
         *
         * @see #Current
         */
        public ICMLModule Pop()
        {
            return stack[--sp];
        }

        /**
         * Returns the last added entry.
         *
         * @see #Pop()
         */
        public ICMLModule Current
        {
            get
            {
                if (sp > 0)
                {
                    return stack[sp - 1];
                }
                else
                {
                    return null;
                }
            }
        }

        /**
         * Returns a string representation of the stack.
         */
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('/');
            for (int i = 0; i < sp; ++i)
            {
                sb.Append(stack[i].GetType().Name);
                sb.Append('/');
            }
            return sb.ToString();
        }

        /**
         * Convenience method to check the last added elements.
         */
        public bool EndsWith(ICMLModule lastElement)
        {
            return stack[sp - 1].Equals(lastElement);
        }

        /**
         * Convenience method to check the last two added elements.
         */
        public bool EndsWith(ICMLModule oneButLast, ICMLModule lastElement)
        {
            return EndsWith(lastElement) && stack[sp - 2].Equals(oneButLast);
        }

        /**
         * Convenience method to check the last three added elements.
         */
        public bool EndsWith(ICMLModule twoButLast, ICMLModule oneButLast, ICMLModule lastElement)
        {
            return EndsWith(oneButLast, lastElement) && stack[sp - 3].Equals(twoButLast);
        }
    }
}
