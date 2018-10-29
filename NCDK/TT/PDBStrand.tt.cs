



// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2016-2017  Kazuya Ujihara <ujihara.kazuya@gmail.com>

/* Copyright (C) 2001-2007  Egon Willighagen <egonw@users.sf.net>
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

using System.Collections.Generic;
using System.Text;

namespace NCDK.Default
{
    /// <summary>
    /// An entry in the PDB database. It is not just a regular protein, but the
    /// regular PDB mix of protein or protein complexes, ligands, water molecules
    /// and other species.
    /// </summary>
    // @cdk.module  pdb
    // @cdk.githash
    // @author      Egon Willighagen
    // @cdk.created 2006-04-19
    // @cdk.keyword polymer
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Ignored")]
    public class PDBStrand : Strand
    {
        List<string> sequentialListOfMonomers;

        /// <summary>
        /// Constructs a new Polymer to store the Monomers.
        /// </summary>
        public PDBStrand()
            : base()
        {
            sequentialListOfMonomers = new List<string>();
        }

        /// <summary>
        /// Adds the atom oAtom to a specified Monomer. Additionally, it keeps
        /// record of the iCode.
        ///
        /// <param name="oAtom">The atom to add</param>
        /// <param name="oMonomer">The monomer the atom belongs to</param>
        /// </summary>
        public override void AddAtom(IAtom oAtom, IMonomer oMonomer)
        {
            base.AddAtom(oAtom, oMonomer);
            if (!sequentialListOfMonomers.Contains(oMonomer.MonomerName))
                sequentialListOfMonomers.Add(oMonomer.MonomerName);
        }

        /// <summary>
        /// Returns the monomer names in the order in which they were added.
        /// </summary>
        /// <seealso cref="IPolymer.GetMonomerNames()"/>
        public ICollection<string> GetMonomerNamesInSequentialOrder()
        {
            // don't return the original
            return new List<string>(sequentialListOfMonomers);
        }

        public override string ToString()
        {
            var stringContent = new StringBuilder();
            stringContent.Append("PDBPolymer(");
            stringContent.Append(this.GetHashCode()).Append(", ");
            stringContent.Append(base.ToString());
            stringContent.Append(')');
            return stringContent.ToString();
        }
    }
}
namespace NCDK.Silent
{
    /// <summary>
    /// An entry in the PDB database. It is not just a regular protein, but the
    /// regular PDB mix of protein or protein complexes, ligands, water molecules
    /// and other species.
    /// </summary>
    // @cdk.module  pdb
    // @cdk.githash
    // @author      Egon Willighagen
    // @cdk.created 2006-04-19
    // @cdk.keyword polymer
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Ignored")]
    public class PDBStrand : Strand
    {
        List<string> sequentialListOfMonomers;

        /// <summary>
        /// Constructs a new Polymer to store the Monomers.
        /// </summary>
        public PDBStrand()
            : base()
        {
            sequentialListOfMonomers = new List<string>();
        }

        /// <summary>
        /// Adds the atom oAtom to a specified Monomer. Additionally, it keeps
        /// record of the iCode.
        ///
        /// <param name="oAtom">The atom to add</param>
        /// <param name="oMonomer">The monomer the atom belongs to</param>
        /// </summary>
        public override void AddAtom(IAtom oAtom, IMonomer oMonomer)
        {
            base.AddAtom(oAtom, oMonomer);
            if (!sequentialListOfMonomers.Contains(oMonomer.MonomerName))
                sequentialListOfMonomers.Add(oMonomer.MonomerName);
        }

        /// <summary>
        /// Returns the monomer names in the order in which they were added.
        /// </summary>
        /// <seealso cref="IPolymer.GetMonomerNames()"/>
        public ICollection<string> GetMonomerNamesInSequentialOrder()
        {
            // don't return the original
            return new List<string>(sequentialListOfMonomers);
        }

        public override string ToString()
        {
            var stringContent = new StringBuilder();
            stringContent.Append("PDBPolymer(");
            stringContent.Append(this.GetHashCode()).Append(", ");
            stringContent.Append(base.ToString());
            stringContent.Append(')');
            return stringContent.ToString();
        }
    }
}
