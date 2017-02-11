/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 *  This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Common.Util;
using NCDK.Numerics;
using NCDK.IO.Formats;
using System;
using System.IO;

namespace NCDK.IO
{
    /**
     * A reader for GAMESS log file.
     *
     * <p><b>Expected behaviour</b>:
     * <br>The "GamessReader" object is able to read GAMESS output log file format.
     *
     * <p><b>Limitations</b>: <br>This reader was developed from a small set of
     * example log files, and therefore, is not guaranteed to properly read all
     * GAMESS output. If you have problems, please contact the author of this code,
     * not the developers of GAMESS.
     *
     * <!-- <p><b>State information</b>: <br> [] -->
     * <!-- <p><b>Dependencies</b>: <br> [all OS/Software/Hardware dependencies] -->
     *
     * <p><b>Implementation</b>
     * <br>Available Feature(s):
     * <ul>
     * 	<li><b>Molecular coordinates</b>: Each set of coordinates is added to the ChemFile in the order they are found.</li>
     * </ul>
     * Unavailable Feature(s):
     * <ul>
     * <!--	<li><b>GAMESS version number</b>: The version number can be retrieved.</li> -->
     * <!--	<li><b>Point group symmetry information</b>: The point group is associated with the set of molecules.</li> -->
     * <!--	<li><b>MOPAC charges</b>: The point group is associated with the set of molecules.</li> -->
     * 	<li><b>Energies</b>: They are associated with the previously read set of coordinates.</li>
     * 	<li><b>Normal coordinates of vibrations</b>: They are associated with the previously read set of coordinates.</li>
     * </ul>
     *
     * <!-- <p><b>Security:</b> -->
     *
     * <p><b>References</b>:
     * <br><a href="http://www.msg.ameslab.gov/GAMESS/GAMESS.html">GAMESS</a> is a
     * quantum chemistry program by Gordon research group atIowa State University.
     *
     * @cdk.module  extra
     * @cdk.githash
     * @cdk.keyword Gamess
     * @cdk.keyword file format
     * @cdk.keyword output
     * @cdk.keyword log file
     * @cdk.iooptions
     *
     * @author Bradley A. Smith
     *
     * <!-- @see #GamessWriter(Reader) -->
     */
    //TODO Update class comments with appropriate information.
    //TODO Update "see" tag with reference to GamessWriter when it will be implemented.
    //TODO Update "author" tag with appropriate information.
    public class GamessReader : DefaultChemObjectReader
    {
        /**
         * bool constant used to specify that the coordinates are given in Bohr units.
         */
        public const bool BOHR_UNIT = true;

        /**
         * Double constant that contains the conversion factor from Bohr unit to
         * &Aring;ngstrom unit.
         */
        //TODO Check the accuracy of this comment.
        public const double BOHR_TO_ANGSTROM = 0.529177249;

        /**
         * bool constant used to specify that the coordinates are given in &Aring;ngstrom units.
         */
        public const bool ANGSTROM_UNIT = false;

        /**
         * The "TextReader" object used to read data from the "file system" file.
         *
         * @see	org.openscience.cdk.io.GamessReader#GamessReader(Reader)
         */
        //TODO Improve field comment.
        //TODO Answer the question : When is it opened and when is it closed?
        private TextReader input;

        /**
         * Constructs a new "GamessReader" object given a "Reader" object as input.
         *
         * <p>The "Reader" object may be an instantiable object from the "Reader"
         * hierarchy.
         * <br>For more detail about the "Reader" objects that are really accepted
         * by this "GamessReader" see <code>Accepts(IChemObject)</code> method
         * documentation.
         *
         * @param	inputReader		The "Reader" object given as input parameter.
         *
         * @see #Accepts(Class)
         * @see	java.io.Reader
         *
         */
        public GamessReader(TextReader inputReader)
        {
            this.input = inputReader;
        }

        public GamessReader(Stream input)
            : this(new StreamReader(input))
        { }

        public GamessReader()
                : this(new StringReader(""))
        { }

        /*
         * (non-Javadoc) (Javadoc is automaticly inherited from the link below)
         * @see
         * org.openscience.cdk.io.ChemObjectIO#Accepts(org.openscience.cdk.ChemObject
         * )
         */
        //TODO Update comment with appropriate information to comply Constructor's documentation.

        public override IResourceFormat Format => GamessFormat.Instance;

        public override void SetReader(TextReader reader)
        {
            this.input = reader; // fixed CDK's bug
        }

        public override void SetReader(Stream input)
        {
            SetReader(new StreamReader(input));
        }

        public override bool Accepts(Type type)
        {
            if (typeof(IChemFile).IsAssignableFrom(type)) return true;
            return false;
        }

        /*
         * (non-Javadoc) (Javadoc is automaticly inherited from the link below)
         * @see
         * org.openscience.cdk.io.ChemObjectReader#Read(org.openscience.cdk.ChemObject
         * )
         */
        public override T Read<T>(T obj)
        {
            if (obj is IChemFile)
            {
                try
                {
                    return (T)ReadChemFile((IChemFile)obj);
                }
                catch (IOException)
                {
                    return default(T);
                }
            }
            else
            {
                throw new CDKException("Only supported is reading of ChemFile objects.");
            }
        }

        /**
         * Reads data from the "file system" file through the use of the "input"
         * field, parses data and feeds the ChemFile object with the extracted data.
         *
         * @return A ChemFile containing the data parsed from input.
         *
         * @throws	IOException	may be thrown buy the <code>this.input.ReadLine()</code> instruction.
         *
         * @see org.openscience.cdk.io.GamessReader#input
         */
        //TODO Answer the question : Is this method's name appropriate (given the fact that it do not read a ChemFile object, but return it)?
        private IChemFile ReadChemFile(IChemFile file)
        {
            IChemSequence sequence = file.Builder.CreateChemSequence(); // TODO Answer the question : Is this line needed ?
            IChemModel model = file.Builder.CreateChemModel(); // TODO Answer the question : Is this line needed ?
            var moleculeSet = file.Builder.CreateAtomContainerSet();

            model.MoleculeSet = moleculeSet; //TODO Answer the question : Should I do this?
            sequence.Add(model); //TODO Answer the question : Should I do this?
            file.Add(sequence); //TODO Answer the question : Should I do this?

            string currentReadLine = this.input.ReadLine();
            while (currentReadLine != null)
            {
                /*
                 * There are 2 types of coordinate sets: - bohr coordinates sets (if
                 * statement) - angstr???m coordinates sets (else statement)
                 */
                if (currentReadLine.IndexOf("COORDINATES (BOHR)") >= 0)
                {

                    /*
                     * The following line do no contain data, so it is ignored.
                     */
                    this.input.ReadLine();
                    moleculeSet.Add(this.ReadCoordinates(file.Builder.CreateAtomContainer(),
                            GamessReader.BOHR_UNIT));
                    //break; //<- stops when the first set of coordinates is found.
                }
                else if (currentReadLine.IndexOf(" COORDINATES OF ALL ATOMS ARE (ANGS)") >= 0)
                {

                    /*
                     * The following 2 lines do no contain data, so it are ignored.
                     */
                    this.input.ReadLine();
                    this.input.ReadLine();

                    moleculeSet.Add(this.ReadCoordinates(file.Builder.CreateAtomContainer(),
                            GamessReader.ANGSTROM_UNIT));
                    //break; //<- stops when the first set of coordinates is found.
                }
                currentReadLine = this.input.ReadLine();
            }
            return file;
        }

        /**
         * Reads a set of coordinates from the "file system" file through the use of
         * the "input" field, scales coordinate to angstr???m unit, builds each atom with
         * the right associated coordinates, builds a new molecule with these atoms
         * and returns the complete molecule.
         *
         * <p><b>Implementation</b>:
         * <br>Dummy atoms are ignored.
         *
         * @param	coordinatesUnits	The unit in which coordinates are given.
         *
         * @throws	IOException	may be thrown by the "input" object.
         *
         * @see org.openscience.cdk.io.GamessReader#input
         */
        //TODO Update method comments with appropriate information.
        private IAtomContainer ReadCoordinates(IAtomContainer molecule, bool coordinatesUnits)
        {

            /*
             * Coordinates must all be given in angstr???ms.
             */
            double unitScaling = GamessReader.scalesCoordinatesUnits(coordinatesUnits);

            string retrievedLineFromFile;

            while (true)
            {
                retrievedLineFromFile = this.input.ReadLine();
                /*
                 * A coordinate set is followed by an empty line, so when this line
                 * is reached, there are no more coordinates to add to the current
                 * set.
                 */
                if ((retrievedLineFromFile == null) || (retrievedLineFromFile.Trim().Length == 0))
                {
                    break;
                }

                int atomicNumber;
                string atomicSymbol;

                //StringReader sr = new StringReader(retrievedLineFromFile);
                StreamTokenizer token = new StreamTokenizer(new StringReader(retrievedLineFromFile));

                /*
                 * The first token is ignored. It contains the atomic symbol and may
                 * be concatenated with a number.
                 */
                token.NextToken();

                if (token.NextToken() == StreamTokenizer.TT_NUMBER)
                {
                    atomicNumber = (int)token.NumberValue;
                    atomicSymbol = this.IdentifyAtomicSymbol(atomicNumber);
                    /*
                     * Dummy atoms are assumed to be given with an atomic number set
                     * to zero. We will do not add them to the molecule.
                     */
                    if (atomicNumber == 0)
                    {
                        continue;
                    }
                }
                else
                {
                    throw new IOException("Error reading coordinates");
                }

                /*
                 * Atom's coordinates are stored in an array.
                 */
                double[] coordinates = new double[3];
                for (int i = 0; i < coordinates.Length; i++)
                {
                    if (token.NextToken() == StreamTokenizer.TT_NUMBER)
                    {
                        coordinates[i] = token.NumberValue * unitScaling;
                    }
                    else
                    {
                        throw new IOException("Error reading coordinates");
                    }
                }
                IAtom atom = molecule.Builder.CreateAtom(atomicSymbol,
                        new Vector3(coordinates[0], coordinates[1], coordinates[2]));
                molecule.Atoms.Add(atom);
            }
            return molecule;
        }

        /**
         * Identifies the atomic symbol of an atom given its default atomic number.
         *
         * <p><b>Implementation</b>:
         * <br>This is not a definitive method. It will probably be replaced with a
         * more appropriate one. Be advised that as it is not a definitive version,
         * it only recognise atoms from Hydrogen (1) to Argon (18).
         *
         * @param	atomicNumber	The atomic number of an atom.
         *
         * @return	The Symbol corresponding to the atom or "null" is the atom was not recognised.
         */
        //TODO Update method comments with appropriate information.
        private string IdentifyAtomicSymbol(int atomicNumber)
        {
            string symbol;
            switch (atomicNumber)
            {
                case 1:
                    symbol = "H";
                    break;
                case 2:
                    symbol = "He";
                    break;
                case 3:
                    symbol = "Li";
                    break;
                case 4:
                    symbol = "Be";
                    break;
                case 5:
                    symbol = "B";
                    break;
                case 6:
                    symbol = "C";
                    break;
                case 7:
                    symbol = "N";
                    break;
                case 8:
                    symbol = "O";
                    break;
                case 9:
                    symbol = "F";
                    break;
                case 10:
                    symbol = "Ne";
                    break;
                case 11:
                    symbol = "Na";
                    break;
                case 12:
                    symbol = "Mg";
                    break;
                case 13:
                    symbol = "Al";
                    break;
                case 14:
                    symbol = "Si";
                    break;
                case 15:
                    symbol = "P";
                    break;
                case 16:
                    symbol = "S";
                    break;
                case 17:
                    symbol = "Cl";
                    break;
                case 18:
                    symbol = "Ar";
                    break;
                default:
                    symbol = null;
                    break;
            }
            return symbol;
        }

        /**
         * Scales coordinates to &Aring;ngstr&ouml;m unit if they are given in Bohr unit.
         * If coordinates are already given in &Aring;ngstr&ouml;m unit, then no modifications
         * are performed.
         *
         * @param	coordinatesUnits	<code>BOHR_UNIT</code> if coordinates are given in Bohr unit and <code>ANGSTROM_UNIT</code>
         *                              if they are given in &Aring;ngstr&ouml;m unit.
         *
         * @return	The scaling conversion factor: 1 if no scaling is needed and <code>BOHR_TO_ANGSTROM</code> if scaling has to be performed.
         *
         * @see org.openscience.cdk.PhysicalConstants#BOHR_TO_ANGSTROM
         * @see org.openscience.cdk.io.GamessReader#BOHR_UNIT
         * @see org.openscience.cdk.io.GamessReader#ANGSTROM_UNIT
         */
        //TODO Update method comments with appropriate information.
        private static double scalesCoordinatesUnits(bool coordinatesUnits)
        {
            if (coordinatesUnits == GamessReader.BOHR_UNIT)
            {
                return PhysicalConstants.BOHR_TO_ANGSTROM;
            }
            else
            { //condition is: (coordinatesUnits == GamessReader.ANGTROM_UNIT)
                return (double)1;
            }
        }

        /*
         * (non-Javadoc) (Javadoc is automaticly inherited from the link below)
         * @see org.openscience.cdk.io.ChemObjectIO#Close()
         */
        //TODO Answer the question : What are all concerned ressources ?
        public override void Close()
        {
            /*
             * Closes the TextReader used to read the file content.
             */
            input.Close();
        }

        public override void Dispose()
        {
            Close();
        }
    }
}
