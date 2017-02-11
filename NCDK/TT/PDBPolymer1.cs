















// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2015-2016  Kazuya Ujihara

/* Copyright (C) 2001-2008  Egon Willighagen <egonw@users.sf.net>
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
 *  */
using System;
using System.Collections.Generic;
using System.Linq;
using NCDK.Default;
using System.Text;

namespace NCDK.Default
{
    /**
	 * An entry in the PDB database. It is not just a regular protein, but the
	 * regular PDB mix of protein or protein complexes, ligands, water molecules
	 * and other species.
	 *
	 * @cdk.module  data
	 * @cdk.githash
	 *
	 * @author      Egon Willighagen
	 * @cdk.created 2006-04-19
	 * @cdk.keyword polymer
	 */
    public class PDBPolymer : BioPolymer, ICloneable, IPDBPolymer
    {
        IList<string> sequentialListOfMonomers;
        IList<IPDBStructure> secondaryStructures;

        /**
		 * Constructs a new Polymer to store the {@link IMonomer}s.
		 */
        public PDBPolymer()
            : base()
        {
            sequentialListOfMonomers = new List<string>();
            secondaryStructures = new List<IPDBStructure>();
        }

        public void Add(IPDBStructure structure)
        {
            secondaryStructures.Add(structure);
        }


        public IEnumerable<IPDBStructure> GetStructures()
        {
            //		don't return the original
            return new List<IPDBStructure>(secondaryStructures);
        }

        /**
		 * Adds the atom oAtom without specifying a {@link IMonomer} or a Strand. Therefore the
		 * atom to this AtomContainer, but not to a certain Strand or {@link IMonomer} (intended
		 * e.g. for HETATMs).
		 *
		 * @param oAtom  The {@link IPDBAtom} to add
		 */

        public void Add(IPDBAtom oAtom)
        {
            base.Add(oAtom);
        }

        /**
		 * Adds the atom oAtom to a specified Monomer. Additionally, it keeps
		 * record of the iCode.
		 *
		 * @param oAtom  The IPDBAtom to add
		 * @param oMonomer  The monomer the atom belongs to
		 */
        public void AddAtom(IPDBAtom oAtom, IMonomer oMonomer)
        {
            base.AddAtom(oAtom, oMonomer);
            if (!sequentialListOfMonomers.Contains(oMonomer.MonomerName))
                sequentialListOfMonomers.Add(oMonomer.MonomerName);
        }

        /**
		 * Adds the IPDBAtom oAtom to a specified Monomer of a specified Strand.
		 * Additionally, it keeps record of the iCode.
		 *
		 * @param oAtom  The IPDBAtom to add
		 * @param oMonomer  The monomer the atom belongs to
		 */

        public void AddAtom(IPDBAtom oAtom, IMonomer oMonomer, IStrand oStrand)
        {
            base.AddAtom(oAtom, oMonomer, oStrand);
            if (!sequentialListOfMonomers.Contains(oMonomer.MonomerName))
                sequentialListOfMonomers.Add(oMonomer.MonomerName);
        }

        /**
		 * Returns the monomer names in the order in which they were added.
		 *
		 * @see IPolymer#GetMonomerNames()
		 */
        public IEnumerable<string> GetMonomerNamesInSequentialOrder()
        {
            // don't return the original
            return new List<string>(sequentialListOfMonomers);
        }

        /**
		 * @inheritDoc
		 */
        public override object Clone()
        {
            return (IPDBPolymer)base.Clone();
        }

        public override string ToString()
        {
            StringBuilder stringContent = new StringBuilder();
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
    /**
	 * An entry in the PDB database. It is not just a regular protein, but the
	 * regular PDB mix of protein or protein complexes, ligands, water molecules
	 * and other species.
	 *
	 * @cdk.module  data
	 * @cdk.githash
	 *
	 * @author      Egon Willighagen
	 * @cdk.created 2006-04-19
	 * @cdk.keyword polymer
	 */
    public class PDBPolymer : BioPolymer, ICloneable, IPDBPolymer
    {
        IList<string> sequentialListOfMonomers;
        IList<IPDBStructure> secondaryStructures;

        /**
		 * Constructs a new Polymer to store the {@link IMonomer}s.
		 */
        public PDBPolymer()
            : base()
        {
            sequentialListOfMonomers = new List<string>();
            secondaryStructures = new List<IPDBStructure>();
        }

        public void Add(IPDBStructure structure)
        {
            secondaryStructures.Add(structure);
        }


        public IEnumerable<IPDBStructure> GetStructures()
        {
            //		don't return the original
            return new List<IPDBStructure>(secondaryStructures);
        }

        /**
		 * Adds the atom oAtom without specifying a {@link IMonomer} or a Strand. Therefore the
		 * atom to this AtomContainer, but not to a certain Strand or {@link IMonomer} (intended
		 * e.g. for HETATMs).
		 *
		 * @param oAtom  The {@link IPDBAtom} to add
		 */

        public void Add(IPDBAtom oAtom)
        {
            base.Add(oAtom);
        }

        /**
		 * Adds the atom oAtom to a specified Monomer. Additionally, it keeps
		 * record of the iCode.
		 *
		 * @param oAtom  The IPDBAtom to add
		 * @param oMonomer  The monomer the atom belongs to
		 */
        public void AddAtom(IPDBAtom oAtom, IMonomer oMonomer)
        {
            base.AddAtom(oAtom, oMonomer);
            if (!sequentialListOfMonomers.Contains(oMonomer.MonomerName))
                sequentialListOfMonomers.Add(oMonomer.MonomerName);
        }

        /**
		 * Adds the IPDBAtom oAtom to a specified Monomer of a specified Strand.
		 * Additionally, it keeps record of the iCode.
		 *
		 * @param oAtom  The IPDBAtom to add
		 * @param oMonomer  The monomer the atom belongs to
		 */

        public void AddAtom(IPDBAtom oAtom, IMonomer oMonomer, IStrand oStrand)
        {
            base.AddAtom(oAtom, oMonomer, oStrand);
            if (!sequentialListOfMonomers.Contains(oMonomer.MonomerName))
                sequentialListOfMonomers.Add(oMonomer.MonomerName);
        }

        /**
		 * Returns the monomer names in the order in which they were added.
		 *
		 * @see IPolymer#GetMonomerNames()
		 */
        public IEnumerable<string> GetMonomerNamesInSequentialOrder()
        {
            // don't return the original
            return new List<string>(sequentialListOfMonomers);
        }

        /**
		 * @inheritDoc
		 */
        public override object Clone()
        {
            return (IPDBPolymer)base.Clone();
        }

        public override string ToString()
        {
            StringBuilder stringContent = new StringBuilder();
            stringContent.Append("PDBPolymer(");
            stringContent.Append(this.GetHashCode()).Append(", ");
            stringContent.Append(base.ToString());
            stringContent.Append(')');
            return stringContent.ToString();
        }
    }
}
