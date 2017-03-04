/* Copyright (C) 2008 Rajarshi Guha  <rajarshi@users.sourceforge.net>
 *
 *  Contact: rajarshi@users.sourceforge.net
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

using NCDK.Config.Fragments;
using NCDK.QSAR.Result;
using NCDK.Smiles.SMARTS;
using NCDK.Tools.Manipulator;
using System;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// A fragment count descriptor that uses e-state fragments.
    /// <p/>
    /// Traditionally the e-state descriptors identify the relevant fragments and
    /// then evaluate the actual e-state value. However it has been
    /// <a href="http://www.mdpi.org/molecules/papers/91201004.pdf">shown</a> in {@cdk.cite BUTINA2004}
    /// that simply using the <i>counts</i> of the e-state fragments can lead to QSAR models
    /// that exhibit similar performance to those built using the actual e-state indices.
    /// <p/>
    /// Atom typing and aromaticity perception should be performed prior to calling this
    /// descriptor. The atom type definitions are taken from {@cdk.cite HALL1995}.
    /// The SMARTS definitions were obtained from <a href="http://www.rdkit.org">RDKit</a>.
    /// <p/>
    /// The descriptor returns an integer array result of 79 values with the
    /// following names (see <a href="http://www.edusoft-lc.com/molconn/manuals/350/appV.html">
    /// here</a> for the corresponding chemical groups).
    /// <p/>
    /// <p/>
    /// <table border=1 cellpadding=5>
    /// <thead>
    /// <tr>
    /// <th>Serial</th>
    /// <th>Name</th>
    /// <th>Pattern</th>
    /// </tr>
    /// <tbody>
    /// <tr>
    /// <td>0</td><td>khs.sLi</td><td>[LiD1]-*</td>
    /// </tr>
    /// <tr>
    /// <td>1</td><td>khs.ssBe</td><td>[BeD2](-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>2</td><td>khs.ssssBe</td><td>[BeD4](-*)(-*)(-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>3</td><td>khs.ssBH</td><td>[BD2H](-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>4</td><td>khs.sssB</td><td>[BD3](-*)(-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>5</td><td>khs.ssssB</td><td>[BD4](-*)(-*)(-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>6</td><td>khs.sCH3</td><td>[CD1H3]-*</td>
    /// </tr>
    /// <tr>
    /// <td>7</td><td>khs.dCH2</td><td>[CD1H2]=*</td>
    /// </tr>
    /// <tr>
    /// <td>8</td><td>khs.ssCH2</td><td>[CD2H2](-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>9</td><td>khs.tCH</td><td>[CD1H]#*</td>
    /// </tr>
    /// <tr>
    /// <td>10</td><td>khs.dsCH</td><td>[CD2H](=*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>11</td><td>khs.aaCH</td><td>[C,c;D2H](:*):*</td>
    /// </tr>
    /// <tr>
    /// <td>12</td><td>khs.sssCH</td><td>[CD3H](-*)(-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>13</td><td>khs.ddC</td><td>[CD2H0](=*)=*</td>
    /// </tr>
    /// <tr>
    /// <td>14</td><td>khs.tsC</td><td>[CD2H0](#*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>15</td><td>khs.dssC</td><td>[CD3H0](=*)(-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>16</td><td>khs.aasC</td><td>[C,c;D3H0](:*)(:*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>17</td><td>khs.aaaC</td><td>[C,c;D3H0](:*)(:*):*</td>
    /// </tr>
    /// <tr>
    /// <td>18</td><td>khs.ssssC</td><td>[CD4H0](-*)(-*)(-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>19</td><td>khs.sNH3</td><td>[ND1H3]-*</td>
    /// </tr>
    /// <tr>
    /// <td>20</td><td>khs.sNH2</td><td>[ND1H2]-*</td>
    /// </tr>
    /// <tr>
    /// <td>21</td><td>khs.ssNH2</td><td>[ND2H2](-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>22</td><td>khs.dNH</td><td>[ND1H]=*</td>
    /// </tr>
    /// <tr>
    /// <td>23</td><td>khs.ssNH</td><td>[ND2H](-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>24</td><td>khs.aaNH</td><td>[N,nD2H](:*):*</td>
    /// </tr>
    /// <tr>
    /// <td>25</td><td>khs.tN</td><td>[ND1H0]#*</td>
    /// </tr>
    /// <tr>
    /// <td>26</td><td>khs.sssNH</td><td>[ND3H](-*)(-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>27</td><td>khs.dsN</td><td>[ND2H0](=*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>28</td><td>khs.aaN</td><td>[N,nD2H0](:*):*</td>
    /// </tr>
    /// <tr>
    /// <td>29</td><td>khs.sssN</td><td>[ND3H0](-*)(-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>30</td><td>khs.ddsN</td><td>[ND3H0](~[OD1H0])(~[OD1H0])-,:*</td>
    /// </tr>
    /// <tr>
    /// <td>31</td><td>khs.aasN</td><td>[N,nD3H0](:*)(:*)-,:*</td>
    /// </tr>
    /// <tr>
    /// <td>32</td><td>khs.ssssN</td><td>[ND4H0](-*)(-*)(-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>33</td><td>khs.sOH</td><td>[OD1H]-*</td>
    /// </tr>
    /// <tr>
    /// <td>34</td><td>khs.dO</td><td>[OD1H0]=*</td>
    /// </tr>
    /// <tr>
    /// <td>35</td><td>khs.ssO</td><td>[OD2H0](-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>36</td><td>khs.aaO</td><td>[O,oD2H0](:*):*</td>
    /// </tr>
    /// <tr>
    /// <td>37</td><td>khs.sF</td><td>[FD1]-*</td>
    /// </tr>
    /// <tr>
    /// <td>38</td><td>khs.sSiH3</td><td>[SiD1H3]-*</td>
    /// </tr>
    /// <tr>
    /// <td>39</td><td>khs.ssSiH2</td><td>[SiD2H2](-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>40</td><td>khs.sssSiH</td><td>[SiD3H1](-*)(-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>41</td><td>khs.ssssSi</td><td>[SiD4H0](-*)(-*)(-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>42</td><td>khs.sPH2</td><td>[PD1H2]-*</td>
    /// </tr>
    /// <tr>
    /// <td>43</td><td>khs.ssPH</td><td>[PD2H1](-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>44</td><td>khs.sssP</td><td>[PD3H0](-*)(-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>45</td><td>khs.dsssP</td><td>[PD4H0](=*)(-*)(-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>46</td><td>khs.sssssP</td><td>[PD5H0](-*)(-*)(-*)(-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>47</td><td>khs.sSH</td><td>[SD1H1]-*</td>
    /// </tr>
    /// <tr>
    /// <td>48</td><td>khs.dS</td><td>[SD1H0]=*</td>
    /// </tr>
    /// <tr>
    /// <td>49</td><td>khs.ssS</td><td>[SD2H0](-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>50</td><td>khs.aaS</td><td>[S,sD2H0](:*):*</td>
    /// </tr>
    /// <tr>
    /// <td>51</td><td>khs.dssS</td><td>[SD3H0](=*)(-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>52</td><td>khs.ddssS</td><td>[SD4H0](~[OD1H0])(~[OD1H0])(-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>53</td><td>khs.sCl</td><td>[ClD1]-*</td>
    /// </tr>
    /// <tr>
    /// <td>54</td><td>khs.sGeH3</td><td>[GeD1H3](-*)</td>
    /// </tr>
    /// <tr>
    /// <td>55</td><td>khs.ssGeH2</td><td>[GeD2H2](-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>56</td><td>khs.sssGeH</td><td>[GeD3H1](-*)(-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>57</td><td>khs.ssssGe</td><td>[GeD4H0](-*)(-*)(-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>58</td><td>khs.sAsH2</td><td>[AsD1H2]-*</td>
    /// </tr>
    /// <tr>
    /// <td>59</td><td>khs.ssAsH</td><td>[AsD2H1](-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>60</td><td>khs.sssAs</td><td>[AsD3H0](-*)(-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>61</td><td>khs.sssdAs</td><td>[AsD4H0](=*)(-*)(-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>62</td><td>khs.sssssAs</td><td>[AsD5H0](-*)(-*)(-*)(-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>63</td><td>khs.sSeH</td><td>[SeD1H1]-*</td>
    /// </tr>
    /// <tr>
    /// <td>64</td><td>khs.dSe</td><td>[SeD1H0]=*</td>
    /// </tr>
    /// <tr>
    /// <td>65</td><td>khs.ssSe</td><td>[SeD2H0](-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>66</td><td>khs.aaSe</td><td>[SeD2H0](:*):*</td>
    /// </tr>
    /// <tr>
    /// <td>67</td><td>khs.dssSe</td><td>[SeD3H0](=*)(-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>68</td><td>khs.ddssSe</td><td>[SeD4H0](=*)(=*)(-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>69</td><td>khs.sBr</td><td>[BrD1]-*</td>
    /// </tr>
    /// <tr>
    /// <td>70</td><td>khs.sSnH3</td><td>[SnD1H3]-*</td>
    /// </tr>
    /// <tr>
    /// <td>71</td><td>khs.ssSnH2</td><td>[SnD2H2](-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>72</td><td>khs.sssSnH</td><td>[SnD3H1](-*)(-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>73</td><td>khs.ssssSn</td><td>[SnD4H0](-*)(-*)(-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>74</td><td>khs.sI</td><td>[ID1]-*</td>
    /// </tr>
    /// <tr>
    /// <td>75</td><td>khs.sPbH3</td><td>[PbD1H3]-*</td>
    /// </tr>
    /// <tr>
    /// <td>76</td><td>khs.ssPbH2</td><td>[PbD2H2](-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>77</td><td>khs.sssPbH</td><td>[PbD3H1](-*)(-*)-*</td>
    /// </tr>
    /// <tr>
    /// <td>78</td><td>khs.ssssPb</td><td>[PbD4H0](-*)(-*)(-*)-*</td>
    /// </tr>
    /// </tbody></table>
    ///
    // @author Rajarshi Guha
    // @cdk.module qsarmolecular
    // @cdk.githash
    // @cdk.set qsar-descriptors
    // @cdk.dictref qsar-descriptors:kierHallSmarts
    /// </summary>
    public class KierHallSmartsDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private static string[] names;
        private static readonly string[] SMARTS = EStateFragments.Smarts;

        public KierHallSmartsDescriptor()
        {
            string[] tmp = EStateFragments.Names;
            names = new string[tmp.Length];
            for (int i = 0; i < tmp.Length; i++)
                names[i] = "khs." + tmp[i];
        }

        /// <summary>
        /// A map which specifies which descriptor
        /// is implemented by this class.
        /// <para>
        /// These fields are used in the map:
        /// <ul>
        /// <li>Specification-Reference: refers to an entry in a unique dictionary</li>
        /// <li>Implementation-Title: anything</li>
        /// <li>Implementation-Identifier: a unique identifier for this version of
        /// this class</li>
        /// <li>Implementation-Vendor: CDK, JOELib, or anything else</li>
        /// </ul></para>
        /// </summary>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
         new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#kierHallSmarts",
                typeof(KierHallSmartsDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the descriptor.
        /// </summary>
        /// <exception cref="CDKException">if any parameters are specified</exception>
        public override object[] Parameters
        {
            get
            {
                return null;
            }
            set
            {
                if (value != null) throw new CDKException("Must not supply any parameters");
            }
        }

        public override string[] DescriptorNames => names;

        private DescriptorValue GetDummyDescriptorValue(Exception e)
        {
            IntegerArrayResult result = new IntegerArrayResult();
            foreach (var smart in SMARTS)
                result.Add(0);
            return new DescriptorValue(_Specification, ParameterNames, Parameters, result, DescriptorNames, e);
        }

        /// <summary>
        /// This method calculates occurrences of the Kier &amp; Hall E-state fragments.
        /// </summary>
        /// <param name="container">The molecule for which this descriptor is to be calculated</param>
        /// <returns>Counts of the fragments</returns>
        public override DescriptorValue Calculate(IAtomContainer container)
        {
            if (container == null || container.Atoms.Count == 0)
            {
                return GetDummyDescriptorValue(new CDKException("Container was null or else had no atoms"));
            }

            IAtomContainer atomContainer = (IAtomContainer)container.Clone();
            atomContainer = AtomContainerManipulator.RemoveHydrogens(atomContainer);

            int[] counts = new int[SMARTS.Length];
            try
            {
                SMARTSQueryTool sqt = new SMARTSQueryTool("C", container.Builder);
                for (int i = 0; i < SMARTS.Length; i++)
                {
                    sqt.Smarts = SMARTS[i];
                    bool status = sqt.Matches(atomContainer);
                    if (status)
                    {
                        counts[i] = sqt.GetUniqueMatchingAtoms().Count;
                    }
                    else
                        counts[i] = 0;
                }
            }
            catch (CDKException e)
            {
                return GetDummyDescriptorValue(e);
            }

            IntegerArrayResult result = new IntegerArrayResult();
            foreach (var i in counts)
                result.Add(i);

            return new DescriptorValue(_Specification, ParameterNames, Parameters, result, DescriptorNames);
        }

        /// <summary>
        /// Returns the specific type of the DescriptorResult object.
        /// <para>
        /// The return value from this method really indicates what type of result will
        /// be obtained from the <see cref="DescriptorValue"/> object. Note that the same result
        /// can be achieved by interrogating the <see cref="DescriptorValue"/> object; this method
        /// allows you to do the same thing, without actually calculating the descriptor.</para>
        /// </summary>
        public override IDescriptorResult DescriptorResultType { get; } = new IntegerArrayResult(SMARTS.Length);

        /// <summary>
        /// Gets the parameterNames attribute of the descriptor.
        /// </summary>
        /// <returns>The parameterNames value</returns>
        public override string[] ParameterNames => null;

        /// <summary>
        /// Gets the parameterType attribute of the descriptor.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>An Object whose class is that of the parameter requested</returns>
        public override object GetParameterType(string name) => null;
    }
}
