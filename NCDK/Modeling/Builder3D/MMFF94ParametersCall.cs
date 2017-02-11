using System.Collections;
using System.Collections.Generic;

namespace NCDK.Modeling.Builder3D
{
    /**
     * Set the right atoms order to get the parameters.
     *
     * @author         chhoppe
     * @cdk.created    2004-10-8
     * @cdk.module     forcefield
     * @cdk.githash
     */
    public class MMFF94ParametersCall
    {
        private IDictionary<string, IList> pSet = null;

        //private final static double DEFAULT_BOND_LENGTH = 1.5;
        //private final static double DEFAULT_ANGLE = 90;			// Only to test
        //private final static double DEFAULT_TORSION_ANGLE = 90;

        public MMFF94ParametersCall() { }

        /**
         * Initialize the AtomOrder class.
         *
         * @param  parameterSet  Force Field parameter as Map
         */
        public void Initialize(IDictionary<string, IList> parameterSet)
        {
            pSet = parameterSet;
        }

        /**
         *  Gets the bond parameter set.
         *
         * @param  id1            atom1 id
         * @param  id2            atom2 id
         * @return                The distance value from the force field parameter set
         * @exception  Exception  Description of the Exception
         */
        public IList GetBondData(string code, string id1, string id2)
        {
            string dkey = "";
            if (pSet.ContainsKey(("bond" + code + ";" + id1 + ";" + id2)))
            {
                dkey = "bond" + code + ";" + id1 + ";" + id2;
            }
            else if (pSet.ContainsKey(("bond" + code + ";" + id2 + ";" + id1)))
            {
                dkey = "bond" + code + ";" + id2 + ";" + id1;
            } /*
           * else { Console.Out.WriteLine("KEYError:Unknown distance key in pSet: "
           * + code + ";" + id2 + " ;" + id1+" take default bon length:" +
           * DEFAULT_BOND_LENGTH); return DEFAULT_BOND_LENGTH; }
           */
              //Debug.WriteLine("dkey = " + dkey);
            return (IList)pSet[dkey];
        }

        /**
         *  Gets the angle parameter set.
         *
         * @param  id1            ID from Atom 1.
         * @param  id2            ID from Atom 2.
         * @param  id3            ID from Atom 3.
         * @return                The angle data from the force field parameter set
         * @exception  Exception  Description of the Exception
         */
        public IList GetAngleData(string angleType, string id1, string id2, string id3)
        {
            string akey = "";
            if (pSet.ContainsKey(("angle" + angleType + ";" + id1 + ";" + id2 + ";" + id3)))
            {
                akey = "angle" + angleType + ";" + id1 + ";" + id2 + ";" + id3;
            }
            else if (pSet.ContainsKey(("angle" + angleType + ";" + id3 + ";" + id2 + ";" + id1)))
            {
                akey = "angle" + angleType + ";" + id3 + ";" + id2 + ";" + id1;
            } /*
           * else {
           * Console.Out.WriteLine("KEYErrorAngle:Unknown angle key in pSet: " +
           * angleType + ";" + id1 + " ; " + id2 + " ; " + id3
           * +" take default angle:" + DEFAULT_ANGLE); return
           * (Vector)[DEFAULT_ANGLE,0,0]; }
           */
              //Debug.WriteLine("angle key : " + akey);
            return (IList)pSet[akey];
        }

        /**
         *  Gets the bond-angle interaction parameter set.
         *
         * @param  id1            ID from Atom 1.
         * @param  id2            ID from Atom 2.
         * @param  id3            ID from Atom 3.
         * @return                The bond-angle interaction data from the force field parameter set
         * @exception  Exception  Description of the Exception
         */
        public IList GetBondAngleInteractionData(string strbndType, string id1, string id2, string id3)
        {
            string akey = "";
            if (pSet.ContainsKey(("strbnd" + strbndType + ";" + id1 + ";" + id2 + ";" + id3)))
            {
                akey = "strbnd" + strbndType + ";" + id1 + ";" + id2 + ";" + id3;
            }
            else if (pSet.ContainsKey(("strbnd" + strbndType + ";" + id1 + ";" + id3 + ";" + id2)))
            {
                akey = "strbnd" + strbndType + ";" + id1 + ";" + id3 + ";" + id2;
            }
            else if (pSet.ContainsKey(("strbnd" + strbndType + ";" + id2 + ";" + id1 + ";" + id3)))
            {
                akey = "strbnd" + strbndType + ";" + id2 + ";" + id1 + ";" + id3;
            }
            else if (pSet.ContainsKey(("strbnd" + strbndType + ";" + id2 + ";" + id3 + ";" + id1)))
            {
                akey = "strbnd" + strbndType + ";" + id2 + ";" + id3 + ";" + id1;
            }
            else if (pSet.ContainsKey(("strbnd" + strbndType + ";" + id3 + ";" + id1 + ";" + id2)))
            {
                akey = "strbnd" + strbndType + ";" + id3 + ";" + id1 + ";" + id2;
            }
            else if (pSet.ContainsKey(("strbnd" + strbndType + ";" + id3 + ";" + id2 + ";" + id1)))
            {
                akey = "strbnd" + strbndType + ";" + id3 + ";" + id2 + ";" + id1;
            } /*
           * else {
           * Console.Out.WriteLine("KEYErrorAngle:Unknown angle key in pSet: " +id1
           * + " ; " + id2 + " ; " + id3+" take default angle:" +
           * DEFAULT_ANGLE); return (Vector)[DEFAULT_ANGLE,0,0]; }
           */
              //Debug.WriteLine("akey : " + akey);
            return (IList)pSet[akey];
        }

        /**
         * Gets the bond-angle interaction parameter set.
         *
         * @param  iR             ID from Atom 1.
         * @param  jR             ID from Atom 2.
         * @param  kR             ID from Atom 3.
         * @return                The bond-angle interaction data from the force field parameter set
         * @exception  Exception  Description of the Exception
         */
        public IList GetDefaultStretchBendData(int iR, int jR, int kR)
        {
            string dfsbkey = "";
            if (pSet.ContainsKey(("DFSB" + iR + ";" + jR + ";" + kR)))
            {
                dfsbkey = "DFSB" + iR + ";" + jR + ";" + kR;
            } /*
           * else { Console.Out.WriteLine(
           * "KEYErrorDefaultStretchBend:Unknown default stretch-bend key in pSet: "
           * + iR + " ; " + jR + " ; " + kR); }
           */
              //Debug.WriteLine("dfsbkey : " + dfsbkey);
            return (IList)pSet[dfsbkey];
        }

        /**
         *  Gets the bond parameter set.
         *
         * @param  id1            atom1 id
         * @param  id2            atom2 id
         * @return                The distance value from the force field parameter set
         * @exception  Exception  Description of the Exception
         */
        public IList GetTorsionData(string code, string id1, string id2, string id3, string id4)
        {
            string dkey = "";
            if (pSet.ContainsKey(("torsion" + code + ";" + id1 + ";" + id2 + ";" + id3 + ";" + id4)))
            {
                dkey = "torsion" + code + ";" + id1 + ";" + id2 + ";" + id3 + ";" + id4;
            }
            else if (pSet.ContainsKey(("torsion" + code + ";" + id4 + ";" + id3 + ";" + id2 + ";" + id1)))
            {
                dkey = "torsion" + code + ";" + id4 + ";" + id3 + ";" + id2 + ";" + id1;
            }
            else if (pSet.ContainsKey(("torsion" + code + ";*;" + id2 + ";" + id3 + ";*")))
            {
                dkey = "torsion" + code + ";*;" + id2 + ";" + id3 + ";*";
            }
            else if (pSet.ContainsKey(("torsion" + code + ";*;" + id3 + ";" + id2 + ";*")))
            {
                dkey = "torsion" + code + ";*;" + id3 + ";" + id2 + ";*";
            }
            else if (pSet.ContainsKey(("torsion" + 0 + ";*;" + id2 + ";" + id3 + ";*")))
            {
                dkey = "torsion" + 0 + ";*;" + id2 + ";" + id3 + ";*";
            }
            else if (pSet.ContainsKey(("torsion" + 0 + ";*;" + id3 + ";" + id2 + ";*")))
            {
                dkey = "torsion" + 0 + ";*;" + id3 + ";" + id2 + ";*";
            } /*
           * else {
           * Console.Out.WriteLine("KEYError:Unknown distance key in pSet: torsion"
           * + code + ";" + id1 + ";" + id2 + ";" + id3 + ";" + id4 +
           * " take default torsion angle:" + DEFAULT_TORSION_ANGLES); return
           * DEFAULT_TORSION_ANGLE; }
           */
              //Debug.WriteLine("dkey = " + dkey);
            return (IList)pSet[dkey];
        }
    }
}
