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

using MathNet.Numerics.LinearAlgebra;
using NCDK.Common.Collections;
using NCDK.Config;
using NCDK.Geometries;
using NCDK.QSAR.Results;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NCDK.QSAR.Descriptors.Moleculars
{
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
        private readonly Dictionary<int, double> hashatwt;
        private readonly Dictionary<int, double> hashvdw;
        private readonly Dictionary<int, double> hasheneg;
        private readonly Dictionary<int, double> hashpol;

        public WHIMDescriptor()
        {
            this.type = "unity"; // default weighting scheme

            // set up the values from TOD98

            this.hashatwt = new Dictionary<int, double>
            {
                [NaturalElements.H.AtomicNumber] = 0.084,
                [NaturalElements.B.AtomicNumber] = 0.900,
                [NaturalElements.C.AtomicNumber] = 1.000,
                [NaturalElements.N.AtomicNumber] = 1.166,
                [NaturalElements.O.AtomicNumber] = 1.332,
                [NaturalElements.F.AtomicNumber] = 1.582,
                [NaturalElements.Al.AtomicNumber] = 2.246,
                [NaturalElements.Si.AtomicNumber] = 2.339,
                [NaturalElements.P.AtomicNumber] = 2.579,
                [NaturalElements.S.AtomicNumber] = 2.670,
                [NaturalElements.Cl.AtomicNumber] = 2.952,
                [NaturalElements.Fe.AtomicNumber] = 4.650,
                [NaturalElements.Co.AtomicNumber] = 4.907,
                [NaturalElements.Ni.AtomicNumber] = 4.887,
                [NaturalElements.Cu.AtomicNumber] = 5.291,
                [NaturalElements.Zn.AtomicNumber] = 5.445,
                [NaturalElements.Br.AtomicNumber] = 6.653,
                [NaturalElements.Sn.AtomicNumber] = 9.884,
                [NaturalElements.I.AtomicNumber] = 10.566,
            };

            this.hashvdw = new Dictionary<int, double>
            {
                [NaturalElements.H.AtomicNumber] = 0.299,
                [NaturalElements.B.AtomicNumber] = 0.796,
                [NaturalElements.C.AtomicNumber] = 1.000,
                [NaturalElements.N.AtomicNumber] = 0.695,
                [NaturalElements.O.AtomicNumber] = 0.512,
                [NaturalElements.F.AtomicNumber] = 0.410,
                [NaturalElements.Al.AtomicNumber] = 1.626,
                [NaturalElements.Si.AtomicNumber] = 1.424,
                [NaturalElements.P.AtomicNumber] = 1.181,
                [NaturalElements.S.AtomicNumber] = 1.088,
                [NaturalElements.Cl.AtomicNumber] = 1.035,
                [NaturalElements.Fe.AtomicNumber] = 1.829,
                [NaturalElements.Co.AtomicNumber] = 1.561,
                [NaturalElements.Ni.AtomicNumber] = 0.764,
                [NaturalElements.Cu.AtomicNumber] = 0.512,
                [NaturalElements.Zn.AtomicNumber] = 1.708,
                [NaturalElements.Br.AtomicNumber] = 1.384,
                [NaturalElements.Sn.AtomicNumber] = 2.042,
                [NaturalElements.I.AtomicNumber] = 1.728,
            };

            this.hasheneg = new Dictionary<int, double>
            {
                [NaturalElements.H.AtomicNumber] = 0.944,
                [NaturalElements.B.AtomicNumber] = 0.828,
                [NaturalElements.C.AtomicNumber] = 1.000,
                [NaturalElements.N.AtomicNumber] = 1.163,
                [NaturalElements.O.AtomicNumber] = 1.331,
                [NaturalElements.F.AtomicNumber] = 1.457,
                [NaturalElements.Al.AtomicNumber] = 0.624,
                [NaturalElements.Si.AtomicNumber] = 0.779,
                [NaturalElements.P.AtomicNumber] = 0.916,
                [NaturalElements.S.AtomicNumber] = 1.077,
                [NaturalElements.Cl.AtomicNumber] = 1.265,
                [NaturalElements.Fe.AtomicNumber] = 0.728,
                [NaturalElements.Co.AtomicNumber] = 0.728,
                [NaturalElements.Ni.AtomicNumber] = 0.728,
                [NaturalElements.Cu.AtomicNumber] = 0.740,
                [NaturalElements.Zn.AtomicNumber] = 0.810,
                [NaturalElements.Br.AtomicNumber] = 1.172,
                [NaturalElements.Sn.AtomicNumber] = 0.837,
                [NaturalElements.I.AtomicNumber] = 1.012,
            };

            this.hashpol = new Dictionary<int, double>
            {
                [NaturalElements.H.AtomicNumber] = 0.379,
                [NaturalElements.B.AtomicNumber] = 1.722,
                [NaturalElements.C.AtomicNumber] = 1.000,
                [NaturalElements.N.AtomicNumber] = 0.625,
                [NaturalElements.O.AtomicNumber] = 0.456,
                [NaturalElements.F.AtomicNumber] = 0.316,
                [NaturalElements.Al.AtomicNumber] = 3.864,
                [NaturalElements.Si.AtomicNumber] = 3.057,
                [NaturalElements.P.AtomicNumber] = 2.063,
                [NaturalElements.S.AtomicNumber] = 1.648,
                [NaturalElements.Cl.AtomicNumber] = 1.239,
                [NaturalElements.Fe.AtomicNumber] = 4.773,
                [NaturalElements.Co.AtomicNumber] = 4.261,
                [NaturalElements.Ni.AtomicNumber] = 3.864,
                [NaturalElements.Cu.AtomicNumber] = 3.466,
                [NaturalElements.Zn.AtomicNumber] = 4.034,
                [NaturalElements.Br.AtomicNumber] = 1.733,
                [NaturalElements.Sn.AtomicNumber] = 4.375,
                [NaturalElements.I.AtomicNumber] = 3.040,
            };
        }

        public override IImplementationSpecification Specification => specification;
        private static readonly DescriptorSpecification specification =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#WHIM",
                typeof(WHIMDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <inheritdoc/>
        public override IReadOnlyList<object> Parameters
        {
            set
            {
                if (value.Count != 1)
                {
                    throw new CDKException("WHIMDescriptor requires 1 parameter");
                }
                if (!(value[0] is string))
                {
                    throw new CDKException("Parameters must be of type string");
                }
                this.type = (string)value[0];
                switch (this.type)
                {
                    case "unity":
                    case "mass":
                    case "volume":
                    case "eneg":
                    case "polar":
                        break;
                    default:
                        throw new CDKException("Weighting scheme must be one of those specified in the API");
                }
            }
            get
            {
                return new object[] { this.type };
            }
        }

        public override IReadOnlyList<string> DescriptorNames
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

        /// <inheritdoc/>
        public override IReadOnlyList<string> ParameterNames { get; } = new string[] { "type" };

        /// <inheritdoc/>
        public override object GetParameterType(string name) => "";

        private DescriptorValue<ArrayResult<double>> GetDummyDescriptorValue(Exception e)
        {
            var ndesc = DescriptorNames.Count;
            var results = new ArrayResult<double>(ndesc);
            for (int i = 0; i < ndesc; i++)
                results.Add(double.NaN);
            return new DescriptorValue<ArrayResult<double>>(specification, ParameterNames, Parameters, results, DescriptorNames, e);
        }

        /// <summary>
        /// Calculates 11 directional and 6 non-directional WHIM descriptors for.
        /// the specified weighting scheme
        /// </summary>
        /// <param name="container">Parameter is the atom container.</param>
        /// <returns>An ArrayList containing the descriptors in the order described above.</returns>
        public DescriptorValue<ArrayResult<double>> Calculate(IAtomContainer container)
        {
            if (!GeometryUtil.Has3DCoordinates(container))
                return GetDummyDescriptorValue(new CDKException("Molecule must have 3D coordinates"));

            double sum = 0.0;
            var ac = (IAtomContainer)container.Clone();

            // do aromaticity detecttion for calculating polarizability later on
            //HueckelAromaticityDetector had = new HueckelAromaticityDetector();
            //had.DetectAromaticity(ac);

            // get the coordinate matrix
            var cmat = Arrays.CreateJagged<double>(ac.Atoms.Count, 3);
            for (int i = 0; i < ac.Atoms.Count; i++)
            {
                var coords = ac.Atoms[i].Point3D.Value;
                cmat[i][0] = coords.X;
                cmat[i][1] = coords.Y;
                cmat[i][2] = coords.Z;
            }

            // set up the weight vector
            Dictionary<int, double> hash = null;
            var wt = new double[ac.Atoms.Count];

            if (string.Equals(this.type, "unity", StringComparison.Ordinal))
            {
                for (int i = 0; i < ac.Atoms.Count; i++)
                    wt[i] = 1.0;
            }
            else
            {
                switch (this.type)
                {
                    case "mass":
                        hash = this.hashatwt;
                        break;
                    case "volume":
                        hash = this.hashvdw;
                        break;
                    case "eneg":
                        hash = this.hasheneg;
                        break;
                    case "polar":
                        hash = this.hashpol;
                        break;
                    default:
                        break;
                }
                for (int i = 0; i < ac.Atoms.Count; i++)
                {
                    wt[i] = hash[ac.Atoms[i].AtomicNumber.Value];
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
            var lambda = pcaobject.GetEigenvalues();
            var gamma = new double[3];
            var nu = new double[3];
            var eta = new double[3];

            for (int i = 0; i < 3; i++)
                sum += lambda[i];
            for (int i = 0; i < 3; i++)
                nu[i] = lambda[i] / sum;

            var scores = pcaobject.GetScores();
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
                        if (k == j)
                            continue;
                        if (scores[j][i] == -1 * scores[k][i])
                        {
                            ns++;
                            foundmatch = true;
                            break;
                        }
                    }
                    if (!foundmatch) na++;
                }
                var n = (double)ac.Atoms.Count;
                gamma[i] = -1.0 * ((ns / n) * Math.Log(ns / n) / Math.Log(2.0) + (na / n) * Math.Log(1.0 / n) / Math.Log(2.0));
                gamma[i] = 1.0 / (1.0 + gamma[i]);
                //Debug.WriteLine("ns = "+ns+" na = "+na+"  gamma = "+gamma[i]);
            }
            {
                // non directional WHIMS's
                var t = lambda[0] + lambda[1] + lambda[2];
                var a = lambda[0] * lambda[1] + lambda[0] * lambda[2] + lambda[1] * lambda[2];
                var v = t + a + lambda[0] * lambda[1] * lambda[2];

                double k = 0.0;
                sum = 0.0;
                for (int i = 0; i < 3; i++)
                    sum += lambda[i];
                for (int i = 0; i < 3; i++)
                    k = (lambda[i] / sum) - (1.0 / 3.0);
                k = k / (4.0 / 3.0);

                var g = Math.Pow(gamma[0] * gamma[1] * gamma[2], 1.0 / 3.0);
                var d = eta[0] + eta[1] + eta[2];

                // return all the stuff we calculated
                var retval = new ArrayResult<double>(11 + 6)
                {
                    lambda[0],
                    lambda[1],
                    lambda[2],

                    nu[0],
                    nu[1],

                    gamma[0],
                    gamma[1],
                    gamma[2],

                    eta[0],
                    eta[1],
                    eta[2],

                    t,
                    a,
                    v,
                    k,
                    g,
                    d
                };

                return new DescriptorValue<ArrayResult<double>>(specification, ParameterNames, Parameters, retval, DescriptorNames);
            }
        }

        /// <inheritdoc/>
        public override IDescriptorResult DescriptorResultType { get; } = new ArrayResult<double>(17);

        IDescriptorValue IMolecularDescriptor.Calculate(IAtomContainer container) => Calculate(container);

        class PCA
        {
            private readonly Matrix<double> evec;
            private Matrix<double> t;
            private readonly double[] eval;

            public PCA(double[][] cmat, double[] wt)
            {

                int ncol = 3;
                int nrow = wt.Length;

                if (cmat.Length != wt.Length)
                {
                    throw new CDKException("WHIMDescriptor: number of weights should be equal to number of atoms");
                }

                // make a copy of the coordinate matrix
                var d = Arrays.CreateJagged<double>(nrow, ncol);
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
                var covmat = Arrays.CreateJagged<double>(ncol, ncol);
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
