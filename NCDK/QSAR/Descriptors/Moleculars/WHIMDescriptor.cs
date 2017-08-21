/* Copyright (C) 2004-2007  Rajarshi Guha <rajarshi@users.sourceforge.net>
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
using NCDK.Common.Collections;
using MathNet.Numerics.LinearAlgebra;
using NCDK.Geometries;
using NCDK.QSAR.Results;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NCDK.QSAR.Descriptors.Moleculars {
    /// <summary>
    /// Holistic descriptors described by Todeschini et al <token>cdk-cite-TOD98</token>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The descriptors are based on a number of atom weightings. There are 6 different
    /// possible weightings:
    /// <list type="bullet"> 
    /// <item>unit weights</item>
    /// <item>atomic masses</item>
    /// <item>van der Waals volumes</item>
    /// <item>Mulliken atomic electronegativites</item>
    /// <item>atomic polarizabilities</item>
    /// <item>E-state values described by Kier &amp; Hall</item>
    /// </list>
    /// Currently weighting schemes 1,2,3,4 &amp; 5 are implemented. The weight values
    /// are taken from <token>cdk-cite-TOD98</token> and as a result 19 elements are considered.
    /// </para>
    /// <para>For each weighting scheme we can obtain
    /// <list type="bullet"> 
    /// <item>11 directional WHIM descriptors (É…<sub>1 .. 3</sub>, ÉÀ<sub>1 .. 2</sub>, É¡<sub>1 .. 3</sub>, É≈<sub>1 .. 3</sub>)</item>
    /// <item>6 non-directional WHIM descriptors (T, A, V, K, G, D)</item>
    /// </list>
    /// </para>
    /// <para>Though <token>cdk-cite-TOD98</token> mentions that for planar molecules only 8 directional WHIM
    /// descriptors are required the current code will return all 11.
    /// </para>
    /// <para>
    /// The descriptor returns 17 values for a given weighting scheme, named as follows:
    /// <list type="bullet"> 
    /// <item>Wlambda1</item>
    /// <item>Wlambda2</item>
    /// <item>wlambda3</item>
    /// <item>Wnu1</item>
    /// <item>Wnu2</item>
    /// <item>Wgamma1</item>
    /// <item>Wgamma2</item>
    /// <item>Wgamma3</item>
    /// <item>Weta1</item>
    /// <item>Weta2</item>
    /// <item>Weta3</item>
    /// <item>WT</item>
    /// <item>WA</item>
    /// <item>WV</item>
    /// <item>WK</item>
    /// <item>WG</item>
    /// <item>WD</item>
    /// </list>
    /// </para>
    /// <para>
    /// Each name will have a suffix of the form <i>.X</i> where <i>X</i> indicates
    /// the weighting scheme used. Possible values of <i>X</i> are
    /// <list type="bullet"> 
    /// <item>unity</item>
    /// <item>mass</item>
    /// <item>volume</item>
    /// <item>eneg</item>
    /// <item>polar</item>
    /// </list>
    /// </para>
    /// <para>This descriptor uses these parameters:
    /// <list type="table">
    /// <item>
    /// <term>Name</term>
    /// <term>Default</term>
    /// <term>Description</term>
    /// </item>
    /// <item>
    /// <term>type</term>
    /// <term>unity</term>
    /// <term>Type of weighting as described above</term>
    /// </item>
    /// </list>
    /// </para>
    /// </remarks>
    // @author Rajarshi Guha
    // @cdk.created 2004-12-1
    // @cdk.module qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:WHIM
    // @cdk.keyword WHIM
    // @cdk.keyword descriptor
    public class WHIMDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        string type = "";
        IDictionary<string, double> hashatwt, hashvdw, hasheneg, hashpol;

        public WHIMDescriptor()
        {
            this.type = "unity"; // default weighting scheme

            // set up the values from TOD98

            this.hashatwt = new Dictionary<string, double>();
            this.hashvdw = new Dictionary<string, double>();
            this.hasheneg = new Dictionary<string, double>();
            this.hashpol = new Dictionary<string, double>();

            this.hashatwt["H"] = 0.084;
            this.hashatwt["B"] = 0.900;
            this.hashatwt["C"] = 1.000;
            this.hashatwt["N"] = 1.166;
            this.hashatwt["O"] = 1.332;
            this.hashatwt["F"] = 1.582;
            this.hashatwt["Al"] = 2.246;
            this.hashatwt["Si"] = 2.339;
            this.hashatwt["P"] = 2.579;
            this.hashatwt["S"] = 2.670;
            this.hashatwt["Cl"] = 2.952;
            this.hashatwt["Fe"] = 4.650;
            this.hashatwt["Co"] = 4.907;
            this.hashatwt["Ni"] = 4.887;
            this.hashatwt["Cu"] = 5.291;
            this.hashatwt["Zn"] = 5.445;
            this.hashatwt["Br"] = 6.653;
            this.hashatwt["Sn"] = 9.884;
            this.hashatwt["I"] = 10.566;

            this.hashvdw["H"] = 0.299;
            this.hashvdw["B"] = 0.796;
            this.hashvdw["C"] = 1.000;
            this.hashvdw["N"] = 0.695;
            this.hashvdw["O"] = 0.512;
            this.hashvdw["F"] = 0.410;
            this.hashvdw["Al"] = 1.626;
            this.hashvdw["Si"] = 1.424;
            this.hashvdw["P"] = 1.181;
            this.hashvdw["S"] = 1.088;
            this.hashvdw["Cl"] = 1.035;
            this.hashvdw["Fe"] = 1.829;
            this.hashvdw["Co"] = 1.561;
            this.hashvdw["Ni"] = 0.764;
            this.hashvdw["Cu"] = 0.512;
            this.hashvdw["Zn"] = 1.708;
            this.hashvdw["Br"] = 1.384;
            this.hashvdw["Sn"] = 2.042;
            this.hashvdw["I"] = 1.728;

            this.hasheneg["H"] = 0.944;
            this.hasheneg["B"] = 0.828;
            this.hasheneg["C"] = 1.000;
            this.hasheneg["N"] = 1.163;
            this.hasheneg["O"] = 1.331;
            this.hasheneg["F"] = 1.457;
            this.hasheneg["Al"] = 0.624;
            this.hasheneg["Si"] = 0.779;
            this.hasheneg["P"] = 0.916;
            this.hasheneg["S"] = 1.077;
            this.hasheneg["Cl"] = 1.265;
            this.hasheneg["Fe"] = 0.728;
            this.hasheneg["Co"] = 0.728;
            this.hasheneg["Ni"] = 0.728;
            this.hasheneg["Cu"] = 0.740;
            this.hasheneg["Zn"] = 0.810;
            this.hasheneg["Br"] = 1.172;
            this.hasheneg["Sn"] = 0.837;
            this.hasheneg["I"] = 1.012;

            this.hashpol["H"] = 0.379;
            this.hashpol["B"] = 1.722;
            this.hashpol["C"] = 1.000;
            this.hashpol["N"] = 0.625;
            this.hashpol["O"] = 0.456;
            this.hashpol["F"] = 0.316;
            this.hashpol["Al"] = 3.864;
            this.hashpol["Si"] = 3.057;
            this.hashpol["P"] = 2.063;
            this.hashpol["S"] = 1.648;
            this.hashpol["Cl"] = 1.239;
            this.hashpol["Fe"] = 4.773;
            this.hashpol["Co"] = 4.261;
            this.hashpol["Ni"] = 3.864;
            this.hashpol["Cu"] = 3.466;
            this.hashpol["Zn"] = 4.034;
            this.hashpol["Br"] = 1.733;
            this.hashpol["Sn"] = 4.375;
            this.hashpol["I"] = 3.040;
        }

        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#WHIM",
                typeof(WHIMDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the WHIMDescriptor object.
        /// <para>The new parameter values. The Object array should have a single element
        ///               which should be a string. The possible values of this string are: unity,
        ///               mass, volume, eneg, polar</para>
        /// </summary>
        /// <exception cref="CDKException">if the parameters are of the wrong type</exception>
        public override object[] Parameters
        {
            set
            {
                if (value.Length != 1)
                {
                    throw new CDKException("WHIMDescriptor requires 1 parameter");
                }
                if (!(value[0] is string))
                {
                    throw new CDKException("Parameters must be of type string");
                }
                this.type = (string)value[0];
                if (!this.type.Equals("unity") && !this.type.Equals("mass") && !this.type.Equals("volume")
                        && !this.type.Equals("eneg") && !this.type.Equals("polar"))
                    throw new CDKException("Weighting scheme must be one of those specified in the API");
            }
            get
            {
                return new object[] { this.type };
            }
        }

        public override string[] DescriptorNames
        {
            get
            {
                string[] names = {"Wlambda1", "Wlambda2", "Wlambda3", "Wnu1", "Wnu2", "Wgamma1", "Wgamma2", "Wgamma3", "Weta1",
                "Weta2", "Weta3", "WT", "WA", "WV", "WK", "WG", "WD"};
                for (int i = 0; i < names.Length; i++)
                    names[i] += "." + type;
                return names;
            }
        }

        /// <summary>
        /// The parameterNames attribute of the WHIMDescriptor object.
        /// </summary>
        public override string[] ParameterNames { get; } = new string[] { "type" };

        /// <summary>
        /// Gets the parameterType attribute of the WHIMDescriptor object.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public override object GetParameterType(string name) => "";

        private DescriptorValue GetDummyDescriptorValue(Exception e)
        {
            int ndesc = DescriptorNames.Length;
            DoubleArrayResult results = new DoubleArrayResult(ndesc);
            for (int i = 0; i < ndesc; i++)
                results.Add(double.NaN);
            return new DescriptorValue(_Specification, ParameterNames, Parameters, results,
                    DescriptorNames, e);
        }

        /// <summary>
        /// Calculates 11 directional and 6 non-directional WHIM descriptors for.
        /// the specified weighting scheme
        /// </summary>
        /// <param name="container">Parameter is the atom container.</param>
        /// <returns>An ArrayList containing the descriptors in the order described above.</returns>
        public override DescriptorValue Calculate(IAtomContainer container)
        {
            if (!GeometryUtil.Has3DCoordinates(container))
                return GetDummyDescriptorValue(new CDKException("Molecule must have 3D coordinates"));

            double sum = 0.0;
            IAtomContainer ac = (IAtomContainer)container.Clone();

            // do aromaticity detecttion for calculating polarizability later on
            //HueckelAromaticityDetector had = new HueckelAromaticityDetector();
            //had.DetectAromaticity(ac);

            // get the coordinate matrix
            double[][] cmat = Arrays.CreateJagged<double>(ac.Atoms.Count, 3);
            for (int i = 0; i < ac.Atoms.Count; i++)
            {
                var coords = ac.Atoms[i].Point3D.Value;
                cmat[i][0] = coords.X;
                cmat[i][1] = coords.Y;
                cmat[i][2] = coords.Z;
            }

            // set up the weight vector
            IDictionary<string, double> hash = null;
            double[] wt = new double[ac.Atoms.Count];

            if (this.type.Equals("unity"))
            {
                for (int i = 0; i < ac.Atoms.Count; i++)
                    wt[i] = 1.0;
            }
            else
            {
                if (this.type.Equals("mass"))
                {
                    hash = this.hashatwt;
                }
                else if (this.type.Equals("volume"))
                {
                    hash = this.hashvdw;
                }
                else if (this.type.Equals("eneg"))
                {
                    hash = this.hasheneg;
                }
                else if (this.type.Equals("polar"))
                {
                    hash = this.hashpol;
                }
                for (int i = 0; i < ac.Atoms.Count; i++)
                {
                    string sym = ac.Atoms[i].Symbol;
                    wt[i] = (Double)hash[sym];
                }
            }

            PCA pcaobject = null;
            try
            {
                pcaobject = new PCA(cmat, wt);
            }
            catch (CDKException cdke)
            {
                Debug.WriteLine(cdke);
            }

            // directional WHIM's
            double[] lambda = pcaobject.GetEigenvalues();
            double[] gamma = new double[3];
            double[] nu = new double[3];
            double[] eta = new double[3];

            for (int i = 0; i < 3; i++)
                sum += lambda[i];
            for (int i = 0; i < 3; i++)
                nu[i] = lambda[i] / sum;

            double[][] scores = pcaobject.GetScores();
            for (int i = 0; i < 3; i++)
            {
                sum = 0.0;
                for (int j = 0; j < ac.Atoms.Count; j++)
                    sum += scores[j][i] * scores[j][i] * scores[j][i] * scores[j][i];
                sum = sum / (lambda[i] * lambda[i] * ac.Atoms.Count);
                eta[i] = 1.0 / sum;
            }

            // look for symmetric & asymmetric atoms for the gamma descriptor
            for (int i = 0; i < 3; i++)
            {
                double ns = 0.0;
                double na = 0.0;
                for (int j = 0; j < ac.Atoms.Count; j++)
                {
                    bool foundmatch = false;
                    for (int k = 0; k < ac.Atoms.Count; k++)
                    {
                        if (k == j) continue;
                        if (scores[j][i] == -1 * scores[k][i])
                        {
                            ns++;
                            foundmatch = true;
                            break;
                        }
                    }
                    if (!foundmatch) na++;
                }
                double n = (double)ac.Atoms.Count;
                gamma[i] = -1.0 * ((ns / n) * Math.Log(ns / n) / Math.Log(2.0) + (na / n) * Math.Log(1.0 / n) / Math.Log(2.0));
                gamma[i] = 1.0 / (1.0 + gamma[i]);
                //Debug.WriteLine("ns = "+ns+" na = "+na+"  gamma = "+gamma[i]);
            }
            {
                // non directional WHIMS's
                double t = lambda[0] + lambda[1] + lambda[2];
                double a = lambda[0] * lambda[1] + lambda[0] * lambda[2] + lambda[1] * lambda[2];
                double v = t + a + lambda[0] * lambda[1] * lambda[2];

                double k = 0.0;
                sum = 0.0;
                for (int i = 0; i < 3; i++)
                    sum += lambda[i];
                for (int i = 0; i < 3; i++)
                    k = (lambda[i] / sum) - (1.0 / 3.0);
                k = k / (4.0 / 3.0);

                double g = Math.Pow(gamma[0] * gamma[1] * gamma[2], 1.0 / 3.0);
                double d = eta[0] + eta[1] + eta[2];

                // return all the stuff we calculated
                DoubleArrayResult retval = new DoubleArrayResult(11 + 6);
                retval.Add(lambda[0]);
                retval.Add(lambda[1]);
                retval.Add(lambda[2]);

                retval.Add(nu[0]);
                retval.Add(nu[1]);

                retval.Add(gamma[0]);
                retval.Add(gamma[1]);
                retval.Add(gamma[2]);

                retval.Add(eta[0]);
                retval.Add(eta[1]);
                retval.Add(eta[2]);

                retval.Add(t);
                retval.Add(a);
                retval.Add(v);
                retval.Add(k);
                retval.Add(g);
                retval.Add(d);

                return new DescriptorValue(_Specification, ParameterNames, Parameters, retval, DescriptorNames);
            }
        }

        /// <inheritdoc/>
        public override IDescriptorResult DescriptorResultType { get; } = new DoubleArrayResultType(17);

        class PCA
        {
            Matrix<double> evec;
            Matrix<double> t;
            double[] eval;

            public PCA(double[][] cmat, double[] wt)
            {

                int ncol = 3;
                int nrow = wt.Length;

                if (cmat.Length != wt.Length)
                {
                    throw new CDKException("WHIMDescriptor: number of weights should be equal to number of atoms");
                }

                // make a copy of the coordinate matrix
                double[][] d = Arrays.CreateJagged<double>(nrow, ncol);
                for (int i = 0; i < nrow; i++)
                {
                    for (int j = 0; j < ncol; j++)
                        d[i][j] = cmat[i][j];
                }

                // do mean centering - though the first paper used
                // barymetric centering
                for (int i = 0; i < ncol; i++)
                {
                    double mean = 0.0;
                    for (int j = 0; j < nrow; j++)
                        mean += d[j][i];
                    mean = mean / (double)nrow;
                    for (int j = 0; j < nrow; j++)
                        d[j][i] = (d[j][i] - mean);
                }

                // get the covariance matrix
                double[][] covmat = Arrays.CreateJagged<double>(ncol, ncol);
                double sumwt = 0;
                for (int i = 0; i < nrow; i++)
                    sumwt += wt[i];
                for (int i = 0; i < ncol; i++)
                {
                    double meanx = 0;
                    for (int k = 0; k < nrow; k++)
                        meanx += d[k][i];
                    meanx = meanx / (double)nrow;
                    for (int j = 0; j < ncol; j++)
                    {
                        double meany = 0.0;
                        for (int k = 0; k < nrow; k++)
                            meany += d[k][j];
                        meany = meany / (double)nrow;

                        double sum = 0;
                        for (int k = 0; k < nrow; k++)
                        {
                            //double dd =  wt[k] * (d[k][i] - meanx) * (d[k][j] - meany);
                            //Debug.WriteLine("("+i+","+j+") "+wts[k] + " * " + d[k][i] + "-" + meanx + " * " + d[k][j] + "-" + meany + "==" + dd);
                            sum += wt[k] * (d[k][i] - meanx) * (d[k][j] - meany);
                        }
                        //Debug.WriteLine(sum+" / "+sumwt+"="+sum/sumwt);
                        covmat[i][j] = sum / sumwt;
                    }
                }

                // get the loadings (ie eigenvector matrix)
                var m = Matrix<double>.Build.DenseOfColumnArrays(covmat);
                var ed = m.Evd();
                this.eval = ed.EigenValues.Select(n => n.Real).ToArray();
                this.evec = ed.EigenVectors;
                var x = Matrix<double>.Build.DenseOfColumnArrays(d);
                this.t = this.evec * x;
            }

            public double[] GetEigenvalues()
            {
                return (this.eval);
            }

            public double[][] GetScores()
            {
                return t.ToColumnArrays();
            }
        }
    }
}
