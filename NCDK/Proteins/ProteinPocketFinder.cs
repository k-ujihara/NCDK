/* Copyright (C) 2005-2007  Christian Hoppe <chhoppe@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
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
using NCDK.Config;
using NCDK.Default;
using NCDK.IO;
using NCDK.Numerics;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace NCDK.Proteins
{
    /**
     * The detection of pocket and cavities in a bioPolymer is done similar to the program
     * LIGSITE {@cdk.cite MH1997}.
     *
     * <p>TODO: Optimisation of the cubic grid placement
     *
     * @author      cho
     * @cdk.created 2005-09-30
     * @cdk.module     extra
     * @cdk.githash
     * @cdk.keyword    protein
     * @cdk.keyword    pocket
     */
    public class ProteinPocketFinder
    {
        public int SolvantValue { get; set; } = 0;
        public int ProteinInterior { get; set; } = -1;
        public int PocketSize { get; set; } = 100;				// # datapoints needed to form a pocket
        public double RAtom { get; set; } = 1.5;				// default atom radius
        public double RSolvent { get; set; } = 1.4;         // default solvant radius
        public double LatticeConstant { get; set; } = 0.5;
        public int MinPSPocket { get; set; } = 2;
        public int MinPSCluster { get; set; } = 2;
        public double LinkageRadius { get; set; } = 1;
        public double AtomCheckRadius { get; set; } = 0;	// variable to reduce the atom radius search points
        public IBioPolymer Protein { get; set; } = null;
        public string VanDerWaalsFile { get; set; } = "NCDK.Config.Data.pdb_atomtypes.xml";
        public double[][][] Grid { get; set; } = null;
        GridGenerator gridGenerator = new GridGenerator();
        IDictionary<string, int> visited = new Dictionary<string, int>();
        public IList<IList<Vector3>> Pockets { get; set; } = new List<IList<Vector3>>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="biopolymerFile">The file name containing the protein</param>
        /// <param name="cubicGrid">if true generate the grid</param>
        public ProteinPocketFinder(string biopolymerFile, bool cubicGrid)
        {
            ReadBioPolymer(biopolymerFile);
            if (cubicGrid)
            {
                createCubicGrid();
            }
        }

        public ProteinPocketFinder(string biopolymerFile, double latticeConstant, bool cubicGrid)
        {
            ReadBioPolymer(biopolymerFile);
            this.LatticeConstant = latticeConstant;
            gridGenerator.LatticeConstant = this.LatticeConstant;
            if (cubicGrid)
            {
                createCubicGrid();
            }
            else
            {

            }
        }

        public ProteinPocketFinder(string biopolymerFile, double[][][] grid)
        {
            this.Grid = grid;
            gridGenerator.Grid = grid;
            ReadBioPolymer(biopolymerFile);
        }

        public ProteinPocketFinder(IBioPolymer protein, double[][][] grid)
        {
            this.Protein = protein;
            this.Grid = grid;
            gridGenerator.Grid = grid;
        }

        /**
         * Creates from a PDB File a BioPolymer.
         */
        private void ReadBioPolymer(string biopolymerFile)
        {
            try
            {
                // Read PDB file
                var fileReader = new StreamReader(biopolymerFile);
                ISimpleChemObjectReader reader = new ReaderFactory().CreateReader(fileReader);
                IChemFile chemFile = (IChemFile)reader.Read((IChemObject)new ChemFile());
                // Get molecule from ChemFile
                IChemSequence chemSequence = chemFile[0];
                IChemModel chemModel = chemSequence[0];
                var setOfMolecules = chemModel.MoleculeSet;
                Protein = (IBioPolymer)setOfMolecules[0];
            }
            catch (Exception exc)
            {
                if (exc is IOException || exc is CDKException)
                {
                    Trace.TraceError("Could not read BioPolymer from file>" + biopolymerFile + " due to: " + exc.Message);
                    Debug.WriteLine(exc);
                }
                else
                    throw;
            }
        }

        /**
         * Method determines the minimum and maximum values of a coordinate space
         * up to 3D space.
         *
         * @return double[] stores min,max,min,max,min,max
         */
        public double[] FindGridBoundaries()
        {
            IAtom[] atoms = AtomContainerManipulator.GetAtomArray(Protein);
            double[] minMax = new double[6];
            minMax[0] = atoms[0].Point3D.Value.X;
            minMax[1] = atoms[0].Point3D.Value.X;
            minMax[2] = atoms[0].Point3D.Value.Y;
            minMax[3] = atoms[0].Point3D.Value.Y;
            minMax[4] = atoms[0].Point3D.Value.Z;
            minMax[5] = atoms[0].Point3D.Value.Z;
            for (int i = 0; i < atoms.Length; i++)
            {
                if (atoms[i].Point3D.Value.X > minMax[1])
                {
                    minMax[1] = atoms[i].Point3D.Value.X;
                }
                else if (atoms[i].Point3D.Value.Y > minMax[3])
                {
                    minMax[3] = atoms[i].Point3D.Value.Y;
                }
                else if (atoms[i].Point3D.Value.Z > minMax[5])
                {
                    minMax[5] = atoms[i].Point3D.Value.Z;
                }
                else if (atoms[i].Point3D.Value.X < minMax[0])
                {
                    minMax[0] = atoms[i].Point3D.Value.X;
                }
                else if (atoms[i].Point3D.Value.Y < minMax[2])
                {
                    minMax[2] = atoms[i].Point3D.Value.Y;
                }
                else if (atoms[i].Point3D.Value.Z < minMax[4])
                {
                    minMax[4] = atoms[i].Point3D.Value.Z;
                }
            }
            return minMax;
        }

        /**
         * Method creates a cubic grid with the grid generator class.
         */
        public void createCubicGrid()
        {
            //		Debug.WriteLine("	CREATE CUBIC GRID");
            gridGenerator.SetDimension(FindGridBoundaries(), true);
            gridGenerator.GenerateGrid();
            this.Grid = gridGenerator.Grid;
        }

        /**
         * Method assigns the atoms of a biopolymer to the grid. For every atom
         * the corresponding grid point is identified and set to the value
         * of the proteinInterior variable.
         * The atom radius and solvent radius is accounted for with the variables:
         * double rAtom, and double rSolvent.
         *
         * @throws Exception
         */
        public void assignProteinToGrid()
        {
            //		logger.debug.print("	ASSIGN PROTEIN TO GRID");
            // 1. Step: Set all grid points to solvent accessible
            this.Grid = gridGenerator.InitializeGrid(this.Grid, 0);
            // 2. Step Grid points inaccessible to solvent are assigend a value of -1
            // set grid points around (r_atom+r_solv) to -1
            IAtom[] atoms = AtomContainerManipulator.GetAtomArray(Protein);
            Vector3 gridPoint;
            int checkGridPoints = 0;
            double vdWRadius = 0;
            int[] dim = gridGenerator.Dim;
            //int proteinAtomCount = 0;//Debugging
            int[] minMax = { 0, 0, 0, 0, 0, 0 };

            for (int i = 0; i < atoms.Length; i++)
            {
                if (((PDBAtom)atoms[i]).HetAtom.Value)
                {
                    continue;
                }
                gridPoint = gridGenerator.GetGridPointFrom3dCoordinates(atoms[i].Point3D.Value);
                this.Grid[(int)gridPoint.X][(int)gridPoint.Y][(int)gridPoint.Z] = -1;
                vdWRadius = PeriodicTable.GetVdwRadius(atoms[i].Symbol).Value;
                if (vdWRadius == 0)
                {
                    vdWRadius = RAtom;
                }
                checkGridPoints = (int)(((vdWRadius + RSolvent) / gridGenerator.LatticeConstant) - AtomCheckRadius);
                if (checkGridPoints < 0)
                {
                    checkGridPoints = 0;
                }
                minMax[0] = (int)gridPoint.X - checkGridPoints;
                minMax[1] = (int)gridPoint.X + checkGridPoints;
                minMax[2] = (int)gridPoint.Y - checkGridPoints;
                minMax[3] = (int)gridPoint.Y + checkGridPoints;
                minMax[4] = (int)gridPoint.Z - checkGridPoints;
                minMax[5] = (int)gridPoint.Z + checkGridPoints;
                minMax = CheckBoundaries(minMax, dim);
                for (int x = minMax[0]; x <= minMax[1]; x++)
                {
                    for (int y = minMax[2]; y <= minMax[3]; y++)
                    {
                        for (int z = minMax[4]; z <= minMax[5]; z++)
                        {
                            this.Grid[x][y][z] = this.Grid[x][y][z] - 1;
                            //proteinAtomCount++;//Debugging
                        }
                    }

                }
            }// for atoms.Length

            //		Debug.WriteLine("- checkGridPoints>" + checkGridPoints
            //				+ " ProteinGridPoints>" + proteinAtomCount);
        }

        public void debuggCheckPSPEvent()
        {
            Debug.WriteLine("	debugg_checkPSPEvent");
            int[] dim = gridGenerator.Dim;
            // int pspMin=0;
            int[] pspEvents = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int proteinGrid = 0;
            for (int x = 0; x <= dim[0]; x++)
            {
                for (int y = 0; y <= dim[1]; y++)
                {
                    for (int z = 0; z <= dim[2]; z++)
                    {

                        if (this.Grid[x][y][z] == 0)
                        {
                            pspEvents[0]++;
                        }
                        else if (this.Grid[x][y][z] == 1)
                        {
                            pspEvents[1]++;
                        }
                        else if (this.Grid[x][y][z] == 2)
                        {
                            pspEvents[2]++;
                        }
                        else if (this.Grid[x][y][z] == 3)
                        {
                            pspEvents[3]++;
                        }
                        else if (this.Grid[x][y][z] == 4)
                        {
                            pspEvents[4]++;
                        }
                        else if (this.Grid[x][y][z] == 5)
                        {
                            pspEvents[5]++;
                        }
                        else if (this.Grid[x][y][z] == 6)
                        {
                            pspEvents[6]++;
                        }
                        else if (this.Grid[x][y][z] == 7)
                        {
                            pspEvents[7]++;
                        }
                        else if (this.Grid[x][y][z] >= 7)
                        {
                            pspEvents[8]++;
                        }

                        if (this.Grid[x][y][z] < 0)
                        {
                            proteinGrid++;
                        }
                    }
                }
            }
            Console.Out.Write($"  minPSPocket:{MinPSPocket} proteinGridPoints:{proteinGrid}");
            int sum = 0;
            for (int i = 0; i < pspEvents.Length; i++)
            {
                if (i >= MinPSPocket)
                {
                    sum = sum + pspEvents[i];
                }
                Debug.WriteLine(" " + i + ":" + pspEvents[i]);
            }
            Debug.WriteLine(" pspAll>" + sum);
            // Debug.WriteLine(" PSPAll:"+pspAll+" minPSP:"+minPSP+"
            // #pspMin:"+pspMin+" psp7:"+psp7+" proteinGridPoints:"+proteinGrid
            // +" solventGridPoints:"+solventGrid);
        }

        /**
         * Main method which calls the methods: assignProteinToGrid,
         * GridScan, and FindPockets.
         *
         */
        public void SiteFinder()
        {
            //Debug.WriteLine("SITEFINDER");
            try
            {
                assignProteinToGrid();
            }
            catch (Exception ex1)
            {
                Trace.TraceError($"Problems with assignProteinToGrid due to:{ex1.ToString()}");
            }
            // 3. Step scan allong x,y,z axis and the diagonals, if PSP event add +1
            // to grid cell
            int[] dim = gridGenerator.Dim;
            //		Debug.WriteLine("	SITEFINDER-SCAN - dim:" + dim[0] + " grid:"
            //				+ this.grid[0].Length + " grid point sum:" + this.grid.Length
            //				* this.grid[0].Length * this.grid[0][0].Length);
            axisScanX(dim[2], dim[1], dim[0]);// x-Axis
            axisScanY(dim[2], dim[0], dim[1]);// y-Axis
            axisScanZ(dim[0], dim[1], dim[2]);// z-Axis

            DiagonalAxisScanXZY(dim[0], dim[2], dim[1]);// diagonal1-Axis
            DiagonalAxisScanYZX(dim[1], dim[2], dim[0]);// diagonal2-Axis
            DiagonalAxisScanYXZ(dim[1], dim[0], dim[2]);// diagonal3-Axis
            DiagonalAxisScanXYZ(dim[0], dim[1], dim[2]);// diagonal4-Axis

            //debuggCheckPSPEvent();

            FindPockets();

            SortPockets();
        }

        /**
         * Method sorts the pockets due to its size. The biggest pocket is the first.
         *
         */
        private void SortPockets()
        {
            //		Debug.WriteLine("	SORT POCKETS Start#:" + pockets.Count);
            var hashPockets = new Dictionary<int, IList<int>>();
            IList<Vector3> pocket;
            var sortPockets = new List<IList<Vector3>>(Pockets.Count);
            for (int i = 0; i < Pockets.Count; i++)
            {
                pocket = Pockets[i];
                if (hashPockets.ContainsKey(pocket.Count))
                {
                    var tmp = hashPockets[pocket.Count];
                    tmp.Add(i);
                    hashPockets[pocket.Count] = tmp;
                }
                else
                {
                    List<int> value = new List<int>();
                    value.Add(i);
                    hashPockets[pocket.Count] = value;
                }
            }

            var keys = new List<int>(hashPockets.Keys);
            keys.Sort();
            for (int i = keys.Count - 1; i >= 0; i--)
            {
                var value = hashPockets[keys[i]];
                //			Debug.WriteLine("key:" + i + " Value" + keys[i]
                //					+ " #Pockets:" + value.Count);
                for (int j = 0; j < value.Count; j++)
                {
                    sortPockets.Add(Pockets[value[j]]);
                }
            }
            //		Debug.WriteLine("	SORT POCKETS End#:" + sortPockets.Count);
            Pockets = sortPockets;
        }

        /**
         * Method which finds the pocket, with a simple nearest neighbour clustering. The points
         * which should be clustered or form a pocket can be determined with:
         * 	minPSPocket, minPSCluster, linkageRadius, and pocketSize.
         */
        private void FindPockets()
        {
            int[] dim = gridGenerator.Dim;
            //		Debug.WriteLine("	FIND POCKETS>dimx:" + dim[0] + " dimy:" + dim[1]
            //				+ " dimz:" + dim[2] + " linkageRadius>" + linkageRadius
            //				+ " latticeConstant>" + latticeConstant + " pocketSize:"
            //				+ pocketSize + " minPSPocket:" + minPSPocket + " minPSCluster:"
            //				+ minPSCluster);
            //int pointsVisited = 0;//Debugging
            //int significantPointsVisited = 0;//Debugging
            for (int x = 0; x < dim[0]; x++)
            {
                for (int y = 0; y < dim[1]; y++)
                {
                    for (int z = 0; z < dim[2]; z++)
                    {
                        // logger.debug.print(" x:"+x+" y:"+y+" z:"+z);
                        Vector3 start = new Vector3(x, y, z);
                        //pointsVisited++;
                        if (this.Grid[x][y][z] >= MinPSPocket & !visited.ContainsKey(x + "." + y + "." + z))
                        {
                            List<Vector3> subPocket = new List<Vector3>();
                            // logger.debug.print("new Point: "+grid[x][y][z]);
                            //significantPointsVisited++;
                            // Debug.WriteLine("visited:"+pointsVisited);
                            subPocket = this.clusterPSPPocket(start, subPocket, dim);
                            if (subPocket != null && subPocket.Count >= PocketSize)
                            {
                                Pockets.Add(subPocket);
                            }
                            // Debug.WriteLine(" Points visited:"+pointsVisited+"
                            // subPocketSize:"+subPocket.Count+"
                            // pocketsSize:"+pockets.Count
                            // +" hashtable:"+visited.Count);

                        }
                    }
                }

            }
            //		try {
            //			Debug.WriteLine("	->>>> #pockets:" + pockets.Count
            //					+ " significantPointsVisited:" + significantPointsVisited
            //					+ " keys:" + visited.Count + " PointsVisited:"
            //					+ pointsVisited);
            //		} catch (Exception ex1) {
            //			logger.debug
            //					.println("Problem in System.out due to " + ex1.ToString());
            //		}

        }

        /**
         * Method performs the clustering, is called by FindPockets().
         */
        public List<Vector3> clusterPSPPocket(Vector3 root, List<Vector3> subPocket, int[] dim)
        {
            // Debug.WriteLine(" ****** New Root ******:"+root.X+" "+root.Y+"
            // "+root.Z);
            visited[(int)root.X + "." + (int)root.Y + "." + (int)root.Z] = 1;
            int[] minMax = { 0, 0, 0, 0, 0, 0 };
            minMax[0] = (int)(root.X - LinkageRadius);
            minMax[1] = (int)(root.X + LinkageRadius);
            minMax[2] = (int)(root.Y - LinkageRadius);
            minMax[3] = (int)(root.Y + LinkageRadius);
            minMax[4] = (int)(root.Z - LinkageRadius);
            minMax[5] = (int)(root.Z + LinkageRadius);
            minMax = CheckBoundaries(minMax, dim);
            // Debug.WriteLine("cluster:"+minMax[0]+" "+minMax[1]+" "+minMax[2]+"
            // "+minMax[3]+" "+minMax[4]+" "+minMax[5]+" ");
            for (int k = minMax[0]; k <= minMax[1]; k++)
            {
                for (int m = minMax[2]; m <= minMax[3]; m++)
                {
                    for (int l = minMax[4]; l <= minMax[5]; l++)
                    {
                        Vector3 node = new Vector3(k, m, l);
                        // Debug.WriteLine(" clusterPSPPocket:"+root.X+"
                        // "+root.Y+" "+root.Z+" ->"+k+" "+m+" "+l+"
                        // #>"+this.grid[k][m][l]+" key:"+visited.ContainsKey(new
                        // string(k+"."+m+"."+l)));
                        if (this.Grid[k][m][l] >= MinPSCluster && !visited.ContainsKey(k + "." + m + "." + l))
                        {
                            // Debug.WriteLine(" ---->FOUND");
                            subPocket.Add(node);
                            this.clusterPSPPocket(node, subPocket, dim);
                        }
                    }
                }
            }
            subPocket.Add(root);
            return subPocket;
        }

        /**
         * Method checks boundaries.
         *
         * @param minMax with minMax values
         * @param dim    dimension
         * @return new minMax values between 0 and dim
         */
        private int[] CheckBoundaries(int[] minMax, int[] dim)
        {
            if (minMax[0] < 0)
            {
                minMax[0] = 0;
            }
            if (minMax[1] > dim[0])
            {
                minMax[1] = dim[0];
            }
            if (minMax[2] < 0)
            {
                minMax[2] = 0;
            }
            if (minMax[3] > dim[1])
            {
                minMax[3] = dim[1];
            }
            if (minMax[4] < 0)
            {
                minMax[4] = 0;
            }
            if (minMax[5] > dim[2])
            {
                minMax[5] = dim[2];
            }
            return minMax;
        }

        /**
         * Method which assigns upon a PSP event +1 to these grid points.
         */
        private void firePSPEvent(List<Vector3> line)
        {
            for (int i = 0; i < line.Count; i++)
            {
                this.Grid[(int)line[i].X][(int)line[i].Y][(int)line[i].Z] = this.Grid[(int)line[i].X][(int)line
                        [i].Y][(int)line[i].Z] + 1;
            }

        }

        /**
         * Method performs a scan; works only for cubic grids!
         *
         * @param dimK first dimension
         * @param dimL second dimension
         * @param dimM third dimension
         */
        public void DiagonalAxisScanXZY(int dimK, int dimL, int dimM)
        {
            // x min ->x max;left upper corner z+y max->min//1
            //Debug.WriteLine("	diagonalAxisScanXZY");
            if (dimM < dimL)
            {
                dimL = dimM;
            }
            //int gridPoints = 0;//Debugging
            List<Vector3> line = new List<Vector3>();
            int pspEvent = 0;
            int m = 0;
            for (int j = dimM; j >= 1; j--)
            {// z
                line.Clear();
                pspEvent = 0;
                for (int k = 0; k <= dimK; k++)
                {// min -> max; x
                    m = dimM;// m==y
                    line.Clear();
                    pspEvent = 0;
                    for (int l = dimL; l >= 0; l--)
                    {// z
                     //gridPoints++;
                        if (Grid[k][m][l] < 0)
                        {
                            if (pspEvent < 2)
                            {
                                line.Clear();
                                pspEvent = 1;
                            }
                            else if (pspEvent == 2)
                            {
                                firePSPEvent(line);
                                line.Clear();
                                pspEvent = 1;
                            }
                        }
                        else
                        {
                            if (pspEvent == 1 | pspEvent == 2)
                            {
                                line.Add(new Vector3(k, m, l));
                                pspEvent = 2;
                            }
                        }
                        m--;
                    }// for l
                }
                dimL = j;
            }
            //Debug.WriteLine(" #gridPoints>" + gridPoints);
        }

        /**
         * Method performs a scan; works only for cubic grids!
         *
         * @param dimK first dimension
         * @param dimL second dimension
         * @param dimM third dimension
         */
        public void DiagonalAxisScanYZX(int dimK, int dimL, int dimM)
        {
            // y min -> y max; right lower corner zmax->zmin, xmax ->min//4
            // logger.debug.print(" diagonalAxisScanYZX");
            //int gridPoints = 0;//Debugging
            if (dimM < dimL)
            {
                dimL = dimM;
            }
            List<Vector3> line = new List<Vector3>();
            int pspEvent = 0;
            int m = 0;
            for (int j = dimM; j >= 1; j--)
            {// z
                line.Clear();
                pspEvent = 0;
                for (int k = 0; k <= dimK; k++)
                {// min -> max; y
                    m = dimM;// m==x
                    line.Clear();
                    pspEvent = 0;
                    for (int l = dimL; l >= 0; l--)
                    {// z
                     //gridPoints++;
                        if (Grid[m][k][l] < 0)
                        {
                            if (pspEvent < 2)
                            {
                                line.Clear();
                                pspEvent = 1;
                            }
                            else if (pspEvent == 2)
                            {
                                firePSPEvent(line);
                                line.Clear();
                                pspEvent = 1;
                            }
                        }
                        else
                        {
                            if (pspEvent == 1 | pspEvent == 2)
                            {
                                line.Add(new Vector3(m, k, l));
                                pspEvent = 2;
                            }
                        }
                        m--;
                    }// for l
                }
                dimL = j;
            }
            // Debug.WriteLine(" #gridPoints>"+gridPoints);
        }

        /**
         * Method performs a scan; works only for cubic grids!
         *
         * @param dimK first dimension
         * @param dimL second dimension
         * @param dimM third dimension
         */
        public void DiagonalAxisScanYXZ(int dimK, int dimL, int dimM)
        {
            // y min -> y max; left lower corner z max->min, x min->max//2
            // logger.debug.print(" diagonalAxisScanYXZ");
            //int gridPoints = 0;//Debugging
            if (dimM < dimL)
            {
                dimL = dimM;
            }
            else
            {
                dimM = dimL;
            }
            List<Vector3> line = new List<Vector3>();
            int pspEvent = 0;
            int l = 0;
            for (int j = dimL; j >= 1; j--)
            {// z
                line.Clear();
                pspEvent = 0;
                for (int k = 0; k <= dimK; k++)
                {// min -> max; y
                    line.Clear();
                    pspEvent = 0;
                    l = 0;// x
                    for (int m = dimM; m >= 0; m--)
                    {// z
                     //gridPoints++;
                        if (Grid[l][k][m] < 0)
                        {
                            if (pspEvent < 2)
                            {
                                line.Clear();
                                pspEvent = 1;
                            }
                            else if (pspEvent == 2)
                            {
                                firePSPEvent(line);
                                line.Clear();
                                pspEvent = 1;
                            }
                        }
                        else
                        {
                            if (pspEvent == 1 | pspEvent == 2)
                            {
                                line.Add(new Vector3(l, k, m));
                                pspEvent = 2;
                            }
                        }
                        l++;
                    }// for m;z
                }// for k;y
                dimM = j;
            }
            // Debug.WriteLine(" #gridPoints>"+gridPoints);
        }

        /**
         * Method performs a scan; works only for cubic grids!
         *
         * @param dimK first dimension
         * @param dimL second dimension
         * @param dimM third dimension
         */
        public void DiagonalAxisScanXYZ(int dimK, int dimL, int dimM)
        {
            // x min -> xmax;left lower corner z max->min, y min->max//3
            // logger.debug.print(" diagonalAxisScanXYZ");
            //int gridPoints = 0;//Debugging
            if (dimM < dimL)
            {
                dimL = dimM;
            }
            else
            {
                dimM = dimL;
            }
            List<Vector3> line = new List<Vector3>();
            int pspEvent = 0;
            int l = 0;
            for (int j = dimL; j >= 1; j--)
            {// z
                line.Clear();
                pspEvent = 0;
                for (int k = 0; k <= dimK; k++)
                {// min -> max;x
                    line.Clear();
                    pspEvent = 0;
                    l = 0;// y
                    for (int m = dimM; m >= 0; m--)
                    {// z
                     //gridPoints++;
                        if (Grid[k][l][m] < 0)
                        {
                            if (pspEvent < 2)
                            {
                                line.Clear();
                                pspEvent = 1;
                            }
                            else if (pspEvent == 2)
                            {
                                firePSPEvent(line);
                                line.Clear();
                                pspEvent = 1;
                            }
                        }
                        else
                        {
                            if (pspEvent == 1 | pspEvent == 2)
                            {
                                line.Add(new Vector3(k, l, m));
                                pspEvent = 2;
                            }
                        }
                        l++;
                    }// for m;z
                }// for k;x
                dimM = j;
            }
            // Debug.WriteLine(" #gridPoints>"+gridPoints);
        }

        /**
         * Method performs a scan; works only for cubic grids!
         *
         * @param dimK first dimension
         * @param dimL second dimension
         * @param dimM third dimension
         */
        public void axisScanX(int dimK, int dimL, int dimM)
        {
            // z,y,x
            //		logger.debug.print("	diagonalAxisScanX");
            //int gridPoints = 0;//Debugging
            List<Vector3> line = new List<Vector3>();
            int pspEvent = 0;
            for (int k = 0; k <= dimK; k++)
            {
                line.Clear();
                pspEvent = 0;
                for (int l = 0; l <= dimL; l++)
                {
                    line.Clear();
                    pspEvent = 0;
                    for (int m = 0; m <= dimM; m++)
                    {
                        //gridPoints++;
                        if (Grid[m][l][k] < 0)
                        {
                            if (pspEvent < 2)
                            {
                                pspEvent = 1;
                                line.Clear();
                            }
                            else if (pspEvent == 2)
                            {
                                firePSPEvent(line);
                                line.Clear();
                                pspEvent = 1;
                            }
                        }
                        else
                        {
                            if (pspEvent == 1 | pspEvent == 2)
                            {
                                line.Add(new Vector3(m, l, k));
                                pspEvent = 2;
                            }
                        }
                    }
                }
            }
            //		Debug.WriteLine(" #gridPoints>" + gridPoints);
        }

        /**
         * Method performs a scan; works only for cubic grids!
         *
         * @param dimK first dimension
         * @param dimL second dimension
         * @param dimM third dimension
         */
        public void axisScanY(int dimK, int dimL, int dimM)
        {
            // z,x,y
            List<Vector3> line = new List<Vector3>();
            int pspEvent = 0;
            for (int k = 0; k <= dimK; k++)
            {
                line.Clear();
                pspEvent = 0;
                for (int l = 0; l <= dimL; l++)
                {
                    line.Clear();
                    pspEvent = 0;
                    for (int m = 0; m <= dimM; m++)
                    {
                        if (Grid[l][m][k] < 0)
                        {
                            if (pspEvent < 2)
                            {
                                pspEvent = 1;
                                line.Clear();
                            }
                            else if (pspEvent == 2)
                            {
                                // if (line.Count>2){
                                firePSPEvent(line);
                                // }
                                line.Clear();
                                pspEvent = 1;
                            }
                        }
                        else
                        {
                            if (pspEvent > 0)
                            {
                                line.Add(new Vector3(l, m, k));
                                pspEvent = 2;
                            }
                        }
                    }
                }
            }
        }

        /**
         * Method performs a scan; works only for cubic grids!
         *
         * @param dimK first dimension
         * @param dimL second dimension
         * @param dimM third dimension
         */
        public void axisScanZ(int dimK, int dimL, int dimM)
        {
            // x,y,z
            List<Vector3> line = new List<Vector3>();
            int pspEvent = 0;
            for (int k = 0; k <= dimK; k++)
            {
                line.Clear();
                pspEvent = 0;
                for (int l = 0; l <= dimL; l++)
                {
                    line.Clear();
                    pspEvent = 0;
                    for (int m = 0; m <= dimM; m++)
                    {
                        if (Grid[k][l][m] < 0)
                        {
                            if (pspEvent < 2)
                            {
                                pspEvent = 1;
                                line.Clear();
                            }
                            else if (pspEvent == 2)
                            {
                                firePSPEvent(line);
                                line.Clear();
                                pspEvent = 1;
                            }
                        }
                        else
                        {
                            if (pspEvent > 0)
                            {
                                line.Add(new Vector3(k, l, m));
                                pspEvent = 2;
                            }
                        }
                    }
                }
            }
        }

        /**
         * Method which assigns van der Waals radii to the biopolymer
         * default org/openscience/cdk/config/data/pdb_atomtypes.xml
         * stored in the variable string vanDerWaalsFile.
         */
        public void assignVdWRadiiToProtein()
        {
            AtomTypeFactory atf = null;
            IAtom[] atoms = AtomContainerManipulator.GetAtomArray(Protein);
            try
            {
                atf = AtomTypeFactory.GetInstance(VanDerWaalsFile, atoms[0].Builder);
            }
            catch (Exception ex1)
            {
                Console.Out.WriteLine($"Problem with AtomTypeFactory due to:{ex1.ToString()}");
            }
            for (int i = 0; i < atoms.Length; i++)
            {
                try
                {
                    atf.Configure(atoms[i]);
                }
                catch (Exception ex2)
                {
                    Trace.TraceError($"Problem with atf.configure due to:{ex2.ToString()}");
                }
            }

        }

        /**
         * Method writes the grid to pmesh format.
         */
        public void gridToPmesh(string outPutFileName)
        {
            try
            {
                gridGenerator.WriteGridInPmeshFormat(outPutFileName);
            }
            catch (IOException e)
            {
                Debug.WriteLine(e);
            }
        }

        /**
         * Method writes the PSP points (>=minPSPocket) to pmesh format.
         */
        public void pspGridToPmesh(string outPutFileName)
        {
            try
            {
                gridGenerator.WriteGridInPmeshFormat(outPutFileName, MinPSPocket);
            }
            catch (IOException e)
            {
                Debug.WriteLine(e);
            }
        }

        /**
         * Method writes the protein grid points to pmesh format.
         */
        public void proteinGridToPmesh(string outPutFileName)
        {
            try
            {
                gridGenerator.WriteGridInPmeshFormat(outPutFileName, -1);
            }
            catch (IOException e)
            {
                Debug.WriteLine(e);
            }
        }

        /**
         * Method writes the pockets to pmesh format.
         */
        public void WritePocketsToPMesh(string outPutFileName)
        {

            try
            {
                for (int i = 0; i < Pockets.Count; i++)
                {// go through every
                 // pocket
                    var writer = new StreamWriter(outPutFileName + "-" + i + ".pmesh");
                    var pocket = Pockets[i];
                    writer.Write(pocket.Count + "\n");
                    for (int j = 0; j < pocket.Count; j++)
                    {// go through every
                     // grid point of the
                     // actual pocket
                        Vector3 actualGridPoint = (Vector3)pocket[j];
                        Vector3 coords = gridGenerator.GetCoordinatesFromGridPoint(actualGridPoint);
                        writer.Write(coords.X + "\t" + coords.Y + "\t" + coords.Z + "\n");
                    }
                    writer.Close();
                }
            }
            catch (IOException e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}
