/* Copyright (C) 2009-2010 maclean {gilleain.torrance@gmail.com}
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace NCDK.Signature
{
    // @cdk.module test-signature
    // @author maclean
    [TestClass()]
    public class AbstractSignatureTest
    {
        public static IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        public static void Print(IAtomContainer mol)
        {
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                IAtom a = mol.Atoms[i];
                Console.Out.Write(a.Symbol + " " + i + " ");
            }
            Console.Out.WriteLine();
            foreach (var bond in mol.Bonds)
            {
                IAtom aa = bond.Atoms[0];
                IAtom ab = bond.Atoms[1];
                int o = bond.Order.Numeric;
                int x = mol.Atoms.IndexOf(aa);
                int y = mol.Atoms.IndexOf(ab);
                if (x < y)
                {
                    Console.Out.Write(x + "-" + y + "(" + o + "),");
                }
                else
                {
                    Console.Out.Write(y + "-" + x + "(" + o + "),");
                }
            }
        }

        public static void AddHydrogens(IAtomContainer mol, int carbonIndex, int count)
        {
            for (int i = 0; i < count; i++)
            {
                mol.Atoms.Add(builder.CreateAtom("H"));
                int hydrogenIndex = mol.Atoms.Count - 1;
                mol.AddBond(mol.Atoms[carbonIndex], mol.Atoms[hydrogenIndex], BondOrder.Single);
            }
        }

        public static void AddCarbons(IAtomContainer mol, int count)
        {
            for (int i = 0; i < count; i++)
            {
                mol.Atoms.Add(builder.CreateAtom("C"));
            }
        }

        public static void AddRing(int atomToAttachTo, int ringSize, IAtomContainer mol)
        {
            int numberOfAtoms = mol.Atoms.Count;
            int previous = atomToAttachTo;
            for (int i = 0; i < ringSize; i++)
            {
                mol.Atoms.Add(builder.CreateAtom("C"));
                int current = numberOfAtoms + i;
                mol.AddBond(mol.Atoms[previous], mol.Atoms[current], BondOrder.Single);
                previous = current;
            }
            mol.AddBond(mol.Atoms[numberOfAtoms], mol.Atoms[numberOfAtoms + (ringSize - 1)], BondOrder.Single);
        }

        public static IAtomContainer MakeRhLikeStructure(int pCount, int ringCount)
        {
            IAtomContainer ttpr = builder.CreateAtomContainer();
            ttpr.Atoms.Add(builder.CreateAtom("Rh"));
            for (int i = 1; i <= pCount; i++)
            {
                ttpr.Atoms.Add(builder.CreateAtom("P"));
                ttpr.AddBond(ttpr.Atoms[0], ttpr.Atoms[i], BondOrder.Single);
            }

            for (int j = 1; j <= pCount; j++)
            {
                for (int k = 0; k < ringCount; k++)
                {
                    AbstractSignatureTest.AddRing(j, 6, ttpr);
                }
            }

            return ttpr;
        }

        public static IAtomContainer MakeCycleWheel(int ringSize, int ringCount)
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            mol.Atoms.Add(builder.CreateAtom("C"));
            for (int r = 0; r < ringCount; r++)
            {
                AbstractSignatureTest.AddRing(0, ringSize, mol);
            }
            return mol;
        }

        public static IAtomContainer MakeSandwich(int ringSize, bool hasMethyl)
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            AbstractSignatureTest.AddCarbons(mol, (ringSize * 2));
            mol.Atoms.Add(builder.CreateAtom("Fe"));
            int center = ringSize * 2;
            // face A
            for (int i = 0; i < ringSize - 1; i++)
            {
                mol.AddBond(mol.Atoms[i], mol.Atoms[i + 1], BondOrder.Single);
                mol.AddBond(mol.Atoms[i], mol.Atoms[center], BondOrder.Single);
            }
            mol.AddBond(mol.Atoms[ringSize - 1], mol.Atoms[0], BondOrder.Single);
            mol.AddBond(mol.Atoms[ringSize - 1], mol.Atoms[center], BondOrder.Single);

            //        // face B
            for (int i = 0; i < ringSize - 1; i++)
            {
                mol.AddBond(mol.Atoms[ringSize + 1], mol.Atoms[i + ringSize + 1], BondOrder.Single);
                mol.AddBond(mol.Atoms[ringSize + 1], mol.Atoms[center], BondOrder.Single);
            }
            mol.AddBond(mol.Atoms[(2 * ringSize) - 1], mol.Atoms[ringSize], BondOrder.Single);
            mol.AddBond(mol.Atoms[(2 * ringSize) - 1], mol.Atoms[center], BondOrder.Single);

            if (hasMethyl)
            {
                mol.Atoms.Add(builder.CreateAtom("C"));
                mol.AddBond(mol.Atoms[0], mol.Atoms.Last(), BondOrder.Single);
            }

            return mol;
        }

        public static IAtomContainer MakeC7H16A()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            AbstractSignatureTest.AddCarbons(mol, 7);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[5], BondOrder.Single);
            mol.AddBond(mol.Atoms[5], mol.Atoms[6], BondOrder.Single);
            AbstractSignatureTest.AddHydrogens(mol, 0, 3);
            AbstractSignatureTest.AddHydrogens(mol, 1, 2);
            AbstractSignatureTest.AddHydrogens(mol, 2, 2);
            AbstractSignatureTest.AddHydrogens(mol, 3, 1);
            AbstractSignatureTest.AddHydrogens(mol, 4, 3);
            AbstractSignatureTest.AddHydrogens(mol, 5, 2);
            AbstractSignatureTest.AddHydrogens(mol, 6, 3);
            return mol;
        }

        public static IAtomContainer MakeC7H16B()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            AbstractSignatureTest.AddCarbons(mol, 7);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[5], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[5], mol.Atoms[6], BondOrder.Single);
            AbstractSignatureTest.AddHydrogens(mol, 0, 3);
            AbstractSignatureTest.AddHydrogens(mol, 1, 2);
            AbstractSignatureTest.AddHydrogens(mol, 2, 1);
            AbstractSignatureTest.AddHydrogens(mol, 3, 2);
            AbstractSignatureTest.AddHydrogens(mol, 4, 3);
            AbstractSignatureTest.AddHydrogens(mol, 5, 2);
            AbstractSignatureTest.AddHydrogens(mol, 6, 3);
            return mol;
        }

        public static IAtomContainer MakeC7H16C()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            AbstractSignatureTest.AddCarbons(mol, 7);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Single);
            mol.AddBond(mol.Atoms[5], mol.Atoms[6], BondOrder.Single);
            AbstractSignatureTest.AddHydrogens(mol, 0, 3);
            AbstractSignatureTest.AddHydrogens(mol, 1, 3);
            AbstractSignatureTest.AddHydrogens(mol, 2, 1);
            AbstractSignatureTest.AddHydrogens(mol, 3, 2);
            AbstractSignatureTest.AddHydrogens(mol, 4, 2);
            AbstractSignatureTest.AddHydrogens(mol, 5, 2);
            AbstractSignatureTest.AddHydrogens(mol, 6, 3);
            return mol;
        }

        public static IAtomContainer MakeDodecahedrane()
        {
            IAtomContainer dodec = builder.CreateAtomContainer();
            for (int i = 0; i < 20; i++)
            {
                dodec.Atoms.Add(builder.CreateAtom("C"));
            }
            dodec.AddBond(dodec.Atoms[0], dodec.Atoms[1], BondOrder.Single);
            dodec.AddBond(dodec.Atoms[0], dodec.Atoms[4], BondOrder.Single);
            dodec.AddBond(dodec.Atoms[0], dodec.Atoms[5], BondOrder.Single);
            dodec.AddBond(dodec.Atoms[1], dodec.Atoms[2], BondOrder.Single);
            dodec.AddBond(dodec.Atoms[1], dodec.Atoms[6], BondOrder.Single);
            dodec.AddBond(dodec.Atoms[2], dodec.Atoms[3], BondOrder.Single);
            dodec.AddBond(dodec.Atoms[2], dodec.Atoms[7], BondOrder.Single);
            dodec.AddBond(dodec.Atoms[3], dodec.Atoms[4], BondOrder.Single);
            dodec.AddBond(dodec.Atoms[3], dodec.Atoms[8], BondOrder.Single);
            dodec.AddBond(dodec.Atoms[4], dodec.Atoms[9], BondOrder.Single);
            dodec.AddBond(dodec.Atoms[5], dodec.Atoms[10], BondOrder.Single);
            dodec.AddBond(dodec.Atoms[5], dodec.Atoms[14], BondOrder.Single);
            dodec.AddBond(dodec.Atoms[6], dodec.Atoms[10], BondOrder.Single);
            dodec.AddBond(dodec.Atoms[6], dodec.Atoms[11], BondOrder.Single);
            dodec.AddBond(dodec.Atoms[7], dodec.Atoms[11], BondOrder.Single);
            dodec.AddBond(dodec.Atoms[7], dodec.Atoms[12], BondOrder.Single);
            dodec.AddBond(dodec.Atoms[8], dodec.Atoms[12], BondOrder.Single);
            dodec.AddBond(dodec.Atoms[8], dodec.Atoms[13], BondOrder.Single);
            dodec.AddBond(dodec.Atoms[9], dodec.Atoms[13], BondOrder.Single);
            dodec.AddBond(dodec.Atoms[9], dodec.Atoms[14], BondOrder.Single);
            dodec.AddBond(dodec.Atoms[10], dodec.Atoms[16], BondOrder.Single);
            dodec.AddBond(dodec.Atoms[11], dodec.Atoms[17], BondOrder.Single);
            dodec.AddBond(dodec.Atoms[12], dodec.Atoms[18], BondOrder.Single);
            dodec.AddBond(dodec.Atoms[13], dodec.Atoms[19], BondOrder.Single);
            dodec.AddBond(dodec.Atoms[14], dodec.Atoms[15], BondOrder.Single);
            dodec.AddBond(dodec.Atoms[15], dodec.Atoms[16], BondOrder.Single);
            dodec.AddBond(dodec.Atoms[15], dodec.Atoms[19], BondOrder.Single);
            dodec.AddBond(dodec.Atoms[16], dodec.Atoms[17], BondOrder.Single);
            dodec.AddBond(dodec.Atoms[17], dodec.Atoms[18], BondOrder.Single);
            dodec.AddBond(dodec.Atoms[18], dodec.Atoms[19], BondOrder.Single);

            return dodec;
        }

        public static IAtomContainer MakeCage()
        {
            /*
             * This 'molecule' is the example used to illustrate the algorithm
             * outlined in the 2004 Faulon &ct. paper
             */
            IAtomContainer cage = builder.CreateAtomContainer();
            for (int i = 0; i < 16; i++)
            {
                cage.Atoms.Add(builder.CreateAtom("C"));
            }
            cage.AddBond(cage.Atoms[0], cage.Atoms[1], BondOrder.Single);
            cage.AddBond(cage.Atoms[0], cage.Atoms[3], BondOrder.Single);
            cage.AddBond(cage.Atoms[0], cage.Atoms[4], BondOrder.Single);
            cage.AddBond(cage.Atoms[1], cage.Atoms[2], BondOrder.Single);
            cage.AddBond(cage.Atoms[1], cage.Atoms[6], BondOrder.Single);
            cage.AddBond(cage.Atoms[2], cage.Atoms[3], BondOrder.Single);
            cage.AddBond(cage.Atoms[2], cage.Atoms[8], BondOrder.Single);
            cage.AddBond(cage.Atoms[3], cage.Atoms[10], BondOrder.Single);
            cage.AddBond(cage.Atoms[4], cage.Atoms[5], BondOrder.Single);
            cage.AddBond(cage.Atoms[4], cage.Atoms[11], BondOrder.Single);
            cage.AddBond(cage.Atoms[5], cage.Atoms[6], BondOrder.Single);
            cage.AddBond(cage.Atoms[5], cage.Atoms[12], BondOrder.Single);
            cage.AddBond(cage.Atoms[6], cage.Atoms[7], BondOrder.Single);
            cage.AddBond(cage.Atoms[7], cage.Atoms[8], BondOrder.Single);
            cage.AddBond(cage.Atoms[7], cage.Atoms[13], BondOrder.Single);
            cage.AddBond(cage.Atoms[8], cage.Atoms[9], BondOrder.Single);
            cage.AddBond(cage.Atoms[9], cage.Atoms[10], BondOrder.Single);
            cage.AddBond(cage.Atoms[9], cage.Atoms[14], BondOrder.Single);
            cage.AddBond(cage.Atoms[10], cage.Atoms[11], BondOrder.Single);
            cage.AddBond(cage.Atoms[11], cage.Atoms[15], BondOrder.Single);
            cage.AddBond(cage.Atoms[12], cage.Atoms[13], BondOrder.Single);
            cage.AddBond(cage.Atoms[12], cage.Atoms[15], BondOrder.Single);
            cage.AddBond(cage.Atoms[13], cage.Atoms[14], BondOrder.Single);
            cage.AddBond(cage.Atoms[14], cage.Atoms[15], BondOrder.Single);
            return cage;
        }

        /**
         * Strictly speaking, this is more like a cube than cubane, as it has no
         * hydrogens.
         *
         * @return
         */
        public static IAtomContainer MakeCubane()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            AddCarbons(mol, 8);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[7], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[6], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[5], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Single);
            mol.AddBond(mol.Atoms[4], mol.Atoms[7], BondOrder.Single);
            mol.AddBond(mol.Atoms[5], mol.Atoms[6], BondOrder.Single);
            mol.AddBond(mol.Atoms[6], mol.Atoms[7], BondOrder.Single);
            return mol;
        }

        public static IAtomContainer MakeCuneane()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            AddCarbons(mol, 8);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[5], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[7], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[7], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Single);
            mol.AddBond(mol.Atoms[4], mol.Atoms[6], BondOrder.Single);
            mol.AddBond(mol.Atoms[5], mol.Atoms[6], BondOrder.Single);
            mol.AddBond(mol.Atoms[6], mol.Atoms[7], BondOrder.Single);
            return mol;
        }

        public static IAtomContainer MakeCyclobutane()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            AddCarbons(mol, 4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            return mol;
        }

        public static IAtomContainer MakeBridgedCyclobutane()
        {
            IAtomContainer mol = AbstractSignatureTest.MakeCyclobutane();
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            return mol;
        }

        public static IAtomContainer MakeNapthalene()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            AddCarbons(mol, 10);
            foreach (var atom in mol.Atoms)
            {
                atom.IsAromatic = true;
            }
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[7], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Single);
            mol.AddBond(mol.Atoms[5], mol.Atoms[6], BondOrder.Single);
            mol.AddBond(mol.Atoms[6], mol.Atoms[7], BondOrder.Single);
            mol.AddBond(mol.Atoms[7], mol.Atoms[8], BondOrder.Single);
            mol.AddBond(mol.Atoms[8], mol.Atoms[9], BondOrder.Single);
            mol.AddBond(mol.Atoms[9], mol.Atoms[0], BondOrder.Single);
            foreach (var bond in mol.Bonds)
            {
                bond.IsAromatic = true;
            }
            return mol;
        }

        public static IAtomContainer MakeHexane()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            AddCarbons(mol, 6);

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Single);

            return mol;
        }

        public static IAtomContainer MakeTwistane()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            AddCarbons(mol, 10);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[5], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[7], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[8], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[9], BondOrder.Single);
            mol.AddBond(mol.Atoms[4], mol.Atoms[6], BondOrder.Single);
            mol.AddBond(mol.Atoms[4], mol.Atoms[9], BondOrder.Single);
            mol.AddBond(mol.Atoms[5], mol.Atoms[6], BondOrder.Single);
            mol.AddBond(mol.Atoms[7], mol.Atoms[8], BondOrder.Single);
            return mol;
        }

        public static IAtomContainer MakeBenzene()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            AddCarbons(mol, 6);
            foreach (var atom in mol.Atoms)
            {
                atom.IsAromatic = true;
            }

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Single);
            mol.AddBond(mol.Atoms[5], mol.Atoms[0], BondOrder.Single);
            foreach (var bond in mol.Bonds)
            {
                bond.IsAromatic = true;
            }
            return mol;
        }

        /**
         * This may not be a real molecule, but it is a good, simple test.
         * It is something like cyclobutane with a single carbon bridge across it,
         * or propellane without one of its bonds (see makePropellane).
         *
         * @return
         */
        public static IAtomContainer MakePseudoPropellane()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            AddCarbons(mol, 5);

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);

            return mol;
        }

        public static IAtomContainer MakePropellane()
        {
            IAtomContainer mol = AbstractSignatureTest.MakePseudoPropellane();
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single);
            return mol;
        }
    }
}
