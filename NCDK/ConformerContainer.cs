/* Copyright (C) 2004-2008  Rajarshi Guha <rajarshi.guha@gmail.com>
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Numerics;
using System;
using System.Collections.Generic;
using System.Collections;

namespace NCDK
{
    /**
    // A memory-efficient data structure to store conformers for a single molecule.
    // <p/>
    // Since all the conformers for a given molecule only differ in their 3D coordinates
    // this data structure stores a single <see cref="IAtomContainer"/> containing the atom and bond
    // details and a List of 3D coordinate sets, each element being the set of 3D coordinates
    // for a given conformer.
    // <p/>
    // The class behaves in many ways as a List<IAtomContainer> object, though a few methods are not
    // implemented. Though it is possible to add conformers by hand, this data structure is
    // probably best used in combination with {@link org.openscience.cdk.io.iterator.IteratingMDLConformerReader} as
    // <pre>
    // IteratingMDLConformerReader reader = new IteratingMDLConformerReader(
    //          new FileReader(new File(filename)),
    //          Default.ChemObjectBuilder.Instance);
    // while (reader.HasNext()) {
    //     ConformerContainer cc = (ConformerContainer) reader.Next();
    //     foreach (var conformer in cc) {
    //         // do something with each conformer
    //     }
    // }
    // </pre>
     *
    // @cdk.module data
    // @cdk.githash
    // @author Rajarshi Guha
    // @see org.openscience.cdk.io.iterator.IteratingMDLConformerReader
     */
    public class ConformerContainer : IList<IAtomContainer>
    {

        private IAtomContainer atomContainer = null;
        private string title = null;
        private IList<Vector3[]> coordinates;

        private Vector3[] GetCoordinateList(IAtomContainer atomContainer)
        {
            Vector3[] tmp = new Vector3[atomContainer.Atoms.Count];
            for (int i = 0; i < atomContainer.Atoms.Count; i++)
            {
                IAtom atom = atomContainer.Atoms[i];
                if (atom.Point3D == null) throw new ArgumentException("Molecule must have 3D coordinates");
                tmp[i] = atom.Point3D.Value;
            }
            return tmp;
        }

        public ConformerContainer()
        {
            coordinates = new List<Vector3[]>();
        }

        /**
        // Create a ConformerContainer object from a single molecule object.
        // <p/>
        // Using this constructor, the resultant conformer container will
        // contain a single conformer. More conformers can be added using the
        // {@link #add} method.
        // <p/>
        // Note that the constructor will use the title of the input molecule
        // when adding new molecules as conformers. That is, the title of any molecule
        // to be added as a conformer should match the title of the input molecule.
         *
        // @param atomContainer The base molecule (or first conformer).
         */
        public ConformerContainer(IAtomContainer atomContainer)
        {
            this.atomContainer = atomContainer;
            title = atomContainer.GetProperty<string>(CDKPropertyName.TITLE);
            coordinates = new List<Vector3[]>();
            coordinates.Add(GetCoordinateList(atomContainer));
        }

        /**
        // Create a ConformerContainer from an array of molecules.
        // <p/>
        // This constructor can be used when you have an array of conformers of a given
        // molecule. Note that this constructor will assume that all molecules in the
        // input array will have the same title.
         *
        // @param atomContainers The array of conformers
         */
        public ConformerContainer(IAtomContainer[] atomContainers)
        {
            if (atomContainers.Length == 0) throw new ArgumentException("Can't use a zero-length molecule array");

            // lets check that the titles match
            title = atomContainers[0].GetProperty<string>(CDKPropertyName.TITLE);
            foreach (var atomContainer in atomContainers)
            {
                string nextTitle = atomContainer.GetProperty<string>(CDKPropertyName.TITLE);
                if (title != null && !nextTitle.Equals(title))
                    throw new ArgumentException("Titles of all molecules must match");
            }

            this.atomContainer = atomContainers[0];
            coordinates = new List<Vector3[]>();
            foreach (var container in atomContainers)
            {
                coordinates.Add(GetCoordinateList(container));
            }
        }

        /**
        // Get the title of the conformers.
        // <p/>
        // Note that all conformers for a given molecule will have the same
        // title.
         *
        // @return The title for the conformers
         */
        public string Title => title;

        /**
        // Get the number of conformers stored.
         *
        // @return The number of conformers
         */
        public int Count => coordinates.Count;

        /**
        // Checks whether any conformers are stored or not.
         *
        // @return true if there is at least one conformer, otherwise false
         */
        public bool IsEmpty => coordinates.Count == 0;

        /**
        // Checks to see whether the specified conformer is currently stored.
        // <p/>
        // This method first checks whether the title of the supplied molecule
        // matches the stored title. If not, it returns false. If the title matches
        // it then checks all the coordinates to see whether they match. If all
        // coordinates match it returns true else false.
         *
        // @param o The IAtomContainer to check for
        // @return true if it is present, false otherwise
         */
        public bool Contains(IAtomContainer o)
        {
            return IndexOf(o) != -1;
        }

        /**
        // Returns the conformers in the form of an array of IAtomContainers.
        // <p/>
        // Beware that if you have a large number of conformers you may run out
        // memory during construction of the array since IAtomContainer's are not
        // light weight objects!
         *
        // @return The conformers as an array of individual IAtomContainers.
         */
        public IAtomContainer[] ToArray()
        {
            IAtomContainer[] ret = new IAtomContainer[coordinates.Count];
            int index = 0;
            foreach (var coords in coordinates)
            {
                IAtomContainer conf = (IAtomContainer)atomContainer.Clone();
                for (int i = 0; i < coords.Length; i++)
                {
                    IAtom atom = conf.Atoms[i];
                    atom.Point3D = coords[i];
                }
                ret[index++] = conf;
            }
            return ret;
        }

        /**
        // Add a conformer to the end of the list.
        // <p/>
        // This method allows you to add a IAtomContainer object as another conformer.
        // Before adding it ensures that the title of specific object matches the
        // stored title for these conformers. It will also check that the number of
        // atoms in the specified molecule match the number of atoms in the current set
        // of conformers.
        // <p/>
        // This method will not check for duplicate conformers.
         *
        // @param atomContainer The new conformer to add.
        // @return true
         */
        public void Add(IAtomContainer atomContainer)
        {
            if (this.atomContainer == null)
            {
                this.atomContainer = atomContainer;
                title = atomContainer.GetProperty<string>(CDKPropertyName.TITLE);
            }
            if (title == null)
            {
                throw new ArgumentException("At least one of the input molecules does not have a title");
            }
            if (!title.Equals(atomContainer.GetProperty<string>(CDKPropertyName.TITLE)))
                throw new ArgumentException("The input molecules does not have the same title ('" + title
                        + "') as the other conformers ('" + atomContainer.GetProperty<string>(CDKPropertyName.TITLE) + "')");

            if (atomContainer.Atoms.Count != this.atomContainer.Atoms.Count)
                throw new ArgumentException("Doesn't have the same number of atoms as the rest of the conformers");

            coordinates.Add(GetCoordinateList(atomContainer));
        }

        /**
        // Remove the specified conformer.
         *
        // @param o The conformer to remove (should be castable to IAtomContainer)
        // @return true if the specified conformer was present and removed, false if not found
         */
        public bool Remove(IAtomContainer atomContainer)
        {
            // we should never have a null conformer
            if (atomContainer == null) return false;

            int index = IndexOf(atomContainer);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }

        /**
        // Get rid of all the conformers but keeps atom and bond information.
         */
        public void Clear()
        {
            coordinates.Clear();
        }

        /**
        // Get the conformer at a specified position.
         *
        // @param i The position of the requested conformer
        // @return The conformer
         */
        public IAtomContainer this[int i]
        {
            get
            {
                Vector3[] tmp = coordinates[i];
                for (int j = 0; j < atomContainer.Atoms.Count; j++)
                {
                    IAtom atom = atomContainer.Atoms[j];
                    atom.Point3D = tmp[j];
                }
                return atomContainer;
            }
            set
            {
                Set(i, value);
            }
        }

        public IAtomContainer Set(int i, IAtomContainer atomContainer)
        {
            if (!title.Equals(atomContainer.GetProperty<string>(CDKPropertyName.TITLE)))
                throw new ArgumentException(
                        "The input molecules does not have the same title as the other conformers");
            Vector3[] tmp = GetCoordinateList(atomContainer);
            IAtomContainer oldAtomContainer = this[i];
            coordinates[i] = tmp;
            return oldAtomContainer;
        }

        public void Insert(int i, IAtomContainer atomContainer)
        {
            if (this.atomContainer == null)
            {
                this.atomContainer = atomContainer;
                title = (string)atomContainer.GetProperty<string>(CDKPropertyName.TITLE);
            }

            if (!title.Equals(atomContainer.GetProperty<string>(CDKPropertyName.TITLE)))
                throw new ArgumentException(
                        "The input molecules does not have the same title as the other conformers");

            if (atomContainer.Atoms.Count != this.atomContainer.Atoms.Count)
                throw new ArgumentException("Doesn't have the same number of atoms as the rest of the conformers");

            Vector3[] tmp = GetCoordinateList(atomContainer);
            coordinates.Insert(i, tmp);
        }

        /**
        // Removes the conformer at the specified position.
         *
        // @param i The position in the list to remove
        // @return The conformer that was at the specified position
         */
        public void RemoveAt(int i)
        {
            coordinates.RemoveAt(i);
        }

        /**
        // Returns the lowest index at which the specific IAtomContainer appears in the list or -1 if is not found.
        // <p/>
        // A given IAtomContainer will occur in the list if the title matches the stored title for
        // the conformers in this container and if the coordinates for each atom in the specified molecule
        // are equal to the coordinates of the corresponding atoms in a conformer.
         *
        // @param o The IAtomContainer whose presence is being tested
        // @return The index where o was found
         */
        public int IndexOf(IAtomContainer atomContainer)
        {
            if (!atomContainer.GetProperty<string>(CDKPropertyName.TITLE).Equals(title)) return -1;

            if (atomContainer.Atoms.Count != this.atomContainer.Atoms.Count) return -1;

            bool coordsMatch;
            int index = 0;
            foreach (var coords in coordinates)
            {
                coordsMatch = true;
                for (int i = 0; i < atomContainer.Atoms.Count; i++)
                {
                    Vector3 p = atomContainer.Atoms[i].Point3D.Value;
                    if (!(p.X == coords[i].X && p.Y == coords[i].Y && p.Z == coords[i].Z))
                    {
                        coordsMatch = false;
                        break;
                    }
                }
                if (coordsMatch) return index;
                index++;
            }
            return -1;
        }

        /**
        // Returns the highest index at which the specific IAtomContainer appears in the list or -1 if is not found.
        // <p/>
        // A given IAtomContainer will occur in the list if the title matches the stored title for
        // the conformers in this container and if the coordinates for each atom in the specified molecule
        // are equal to the coordinates of the corresponding atoms in a conformer.
         *
        // @param o The IAtomContainer whose presence is being tested
        // @return The index where o was found
         */
        public int LastIndexOf(IAtomContainer atomContainer)
        {
            if (!atomContainer.GetProperty<string>(CDKPropertyName.TITLE).Equals(title)) return -1;

            if (atomContainer.Atoms.Count != coordinates[0].Length) return -1;

            bool coordsMatch;
            for (int j = coordinates.Count - 1; j >= 0; j--)
            {
                Vector3[] coords = coordinates[j];
                coordsMatch = true;
                for (int i = 0; i < atomContainer.Atoms.Count; i++)
                {
                    Vector3 p = atomContainer.Atoms[i].Point3D.Value;
                    if (!(p.X == coords[i].X && p.Y == coords[i].Y && p.Z == coords[i].Z))
                    {
                        coordsMatch = false;
                        break;
                    }
                }
                if (coordsMatch) return j;
            }
            return -1;
        }

        public bool IsReadOnly => false;

        public void CopyTo(IAtomContainer[] array, int arrayIndex)
        {
            throw new InvalidOperationException();
        }

        public IEnumerator<IAtomContainer> GetEnumerator()
        {
            foreach (var tmp in coordinates)
            {
                for (int j = 0; j < tmp.Length; j++)
                    atomContainer.Atoms[j].Point3D = tmp[j];
                yield return atomContainer;
            }
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
