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
using NCDK.QSAR.Results;
using NCDK.SMARTS;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// A fragment count descriptor that uses e-state fragments.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Traditionally the e-state descriptors identify the relevant fragments and
    /// then evaluate the actual e-state value. However it has been
    /// <see href="http://www.mdpi.org/molecules/papers/91201004.pdf">shown</see> in <token>cdk-cite-BUTINA2004</token>
    /// that simply using the <i>counts</i> of the e-state fragments can lead to QSAR models
    /// that exhibit similar performance to those built using the actual e-state indices.
    /// </para>
    /// <para>
    /// Atom typing and aromaticity perception should be performed prior to calling this
    /// descriptor. The atom type definitions are taken from <token>cdk-cite-HALL1995</token>.
    /// The SMARTS definitions were obtained from <see href="http://www.rdkit.org">RDKit</see>.
    /// </para>
    /// <para>
    /// The descriptor returns an integer array result of 79 values with the
    /// following names (see <see href="http://www.edusoft-lc.com/molconn/manuals/350/appV.html">here</see>
    /// for the corresponding chemical groups).
    /// </para>
    /// <list type="table">
    /// <listheader>
    /// <term>Serial</term>
    /// <term>Name</term>
    /// <term>Pattern</term>
    /// </listheader>
    /// <item>
    /// <term>0</term><term>khs.sLi</term><term>[LiD1]-*</term>
    /// </item>
    /// <item>
    /// <term>1</term><term>khs.ssBe</term><term>[BeD2](-*)-*</term>
    /// </item>
    /// <item>
    /// <term>2</term><term>khs.ssssBe</term><term>[BeD4](-*)(-*)(-*)-*</term>
    /// </item>
    /// <item>
    /// <term>3</term><term>khs.ssBH</term><term>[BD2H](-*)-*</term>
    /// </item>
    /// <item>
    /// <term>4</term><term>khs.sssB</term><term>[BD3](-*)(-*)-*</term>
    /// </item>
    /// <item>
    /// <term>5</term><term>khs.ssssB</term><term>[BD4](-*)(-*)(-*)-*</term>
    /// </item>
    /// <item>
    /// <term>6</term><term>khs.sCH3</term><term>[CD1H3]-*</term>
    /// </item>
    /// <item>
    /// <term>7</term><term>khs.dCH2</term><term>[CD1H2]=*</term>
    /// </item>
    /// <item>
    /// <term>8</term><term>khs.ssCH2</term><term>[CD2H2](-*)-*</term>
    /// </item>
    /// <item>
    /// <term>9</term><term>khs.tCH</term><term>[CD1H]#*</term>
    /// </item>
    /// <item>
    /// <term>10</term><term>khs.dsCH</term><term>[CD2H](=*)-*</term>
    /// </item>
    /// <item>
    /// <term>11</term><term>khs.aaCH</term><term>[C,c;D2H](:*):*</term>
    /// </item>
    /// <item>
    /// <term>12</term><term>khs.sssCH</term><term>[CD3H](-*)(-*)-*</term>
    /// </item>
    /// <item>
    /// <term>13</term><term>khs.ddC</term><term>[CD2H0](=*)=*</term>
    /// </item>
    /// <item>
    /// <term>14</term><term>khs.tsC</term><term>[CD2H0](#*)-*</term>
    /// </item>
    /// <item>
    /// <term>15</term><term>khs.dssC</term><term>[CD3H0](=*)(-*)-*</term>
    /// </item>
    /// <item>
    /// <term>16</term><term>khs.aasC</term><term>[C,c;D3H0](:*)(:*)-*</term>
    /// </item>
    /// <item>
    /// <term>17</term><term>khs.aaaC</term><term>[C,c;D3H0](:*)(:*):*</term>
    /// </item>
    /// <item>
    /// <term>18</term><term>khs.ssssC</term><term>[CD4H0](-*)(-*)(-*)-*</term>
    /// </item>
    /// <item>
    /// <term>19</term><term>khs.sNH3</term><term>[ND1H3]-*</term>
    /// </item>
    /// <item>
    /// <term>20</term><term>khs.sNH2</term><term>[ND1H2]-*</term>
    /// </item>
    /// <item>
    /// <term>21</term><term>khs.ssNH2</term><term>[ND2H2](-*)-*</term>
    /// </item>
    /// <item>
    /// <term>22</term><term>khs.dNH</term><term>[ND1H]=*</term>
    /// </item>
    /// <item>
    /// <term>23</term><term>khs.ssNH</term><term>[ND2H](-*)-*</term>
    /// </item>
    /// <item>
    /// <term>24</term><term>khs.aaNH</term><term>[N,nD2H](:*):*</term>
    /// </item>
    /// <item>
    /// <term>25</term><term>khs.tN</term><term>[ND1H0]#*</term>
    /// </item>
    /// <item>
    /// <term>26</term><term>khs.sssNH</term><term>[ND3H](-*)(-*)-*</term>
    /// </item>
    /// <item>
    /// <term>27</term><term>khs.dsN</term><term>[ND2H0](=*)-*</term>
    /// </item>
    /// <item>
    /// <term>28</term><term>khs.aaN</term><term>[N,nD2H0](:*):*</term>
    /// </item>
    /// <item>
    /// <term>29</term><term>khs.sssN</term><term>[ND3H0](-*)(-*)-*</term>
    /// </item>
    /// <item>
    /// <term>30</term><term>khs.ddsN</term><term>[ND3H0](~[OD1H0])(~[OD1H0])-,:*</term>
    /// </item>
    /// <item>
    /// <term>31</term><term>khs.aasN</term><term>[N,nD3H0](:*)(:*)-,:*</term>
    /// </item>
    /// <item>
    /// <term>32</term><term>khs.ssssN</term><term>[ND4H0](-*)(-*)(-*)-*</term>
    /// </item>
    /// <item>
    /// <term>33</term><term>khs.sOH</term><term>[OD1H]-*</term>
    /// </item>
    /// <item>
    /// <term>34</term><term>khs.dO</term><term>[OD1H0]=*</term>
    /// </item>
    /// <item>
    /// <term>35</term><term>khs.ssO</term><term>[OD2H0](-*)-*</term>
    /// </item>
    /// <item>
    /// <term>36</term><term>khs.aaO</term><term>[O,oD2H0](:*):*</term>
    /// </item>
    /// <item>
    /// <term>37</term><term>khs.sF</term><term>[FD1]-*</term>
    /// </item>
    /// <item>
    /// <term>38</term><term>khs.sSiH3</term><term>[SiD1H3]-*</term>
    /// </item>
    /// <item>
    /// <term>39</term><term>khs.ssSiH2</term><term>[SiD2H2](-*)-*</term>
    /// </item>
    /// <item>
    /// <term>40</term><term>khs.sssSiH</term><term>[SiD3H1](-*)(-*)-*</term>
    /// </item>
    /// <item>
    /// <term>41</term><term>khs.ssssSi</term><term>[SiD4H0](-*)(-*)(-*)-*</term>
    /// </item>
    /// <item>
    /// <term>42</term><term>khs.sPH2</term><term>[PD1H2]-*</term>
    /// </item>
    /// <item>
    /// <term>43</term><term>khs.ssPH</term><term>[PD2H1](-*)-*</term>
    /// </item>
    /// <item>
    /// <term>44</term><term>khs.sssP</term><term>[PD3H0](-*)(-*)-*</term>
    /// </item>
    /// <item>
    /// <term>45</term><term>khs.dsssP</term><term>[PD4H0](=*)(-*)(-*)-*</term>
    /// </item>
    /// <item>
    /// <term>46</term><term>khs.sssssP</term><term>[PD5H0](-*)(-*)(-*)(-*)-*</term>
    /// </item>
    /// <item>
    /// <term>47</term><term>khs.sSH</term><term>[SD1H1]-*</term>
    /// </item>
    /// <item>
    /// <term>48</term><term>khs.dS</term><term>[SD1H0]=*</term>
    /// </item>
    /// <item>
    /// <term>49</term><term>khs.ssS</term><term>[SD2H0](-*)-*</term>
    /// </item>
    /// <item>
    /// <term>50</term><term>khs.aaS</term><term>[S,sD2H0](:*):*</term>
    /// </item>
    /// <item>
    /// <term>51</term><term>khs.dssS</term><term>[SD3H0](=*)(-*)-*</term>
    /// </item>
    /// <item>
    /// <term>52</term><term>khs.ddssS</term><term>[SD4H0](~[OD1H0])(~[OD1H0])(-*)-*</term>
    /// </item>
    /// <item>
    /// <term>53</term><term>khs.sCl</term><term>[ClD1]-*</term>
    /// </item>
    /// <item>
    /// <term>54</term><term>khs.sGeH3</term><term>[GeD1H3](-*)</term>
    /// </item>
    /// <item>
    /// <term>55</term><term>khs.ssGeH2</term><term>[GeD2H2](-*)-*</term>
    /// </item>
    /// <item>
    /// <term>56</term><term>khs.sssGeH</term><term>[GeD3H1](-*)(-*)-*</term>
    /// </item>
    /// <item>
    /// <term>57</term><term>khs.ssssGe</term><term>[GeD4H0](-*)(-*)(-*)-*</term>
    /// </item>
    /// <item>
    /// <term>58</term><term>khs.sAsH2</term><term>[AsD1H2]-*</term>
    /// </item>
    /// <item>
    /// <term>59</term><term>khs.ssAsH</term><term>[AsD2H1](-*)-*</term>
    /// </item>
    /// <item>
    /// <term>60</term><term>khs.sssAs</term><term>[AsD3H0](-*)(-*)-*</term>
    /// </item>
    /// <item>
    /// <term>61</term><term>khs.sssdAs</term><term>[AsD4H0](=*)(-*)(-*)-*</term>
    /// </item>
    /// <item>
    /// <term>62</term><term>khs.sssssAs</term><term>[AsD5H0](-*)(-*)(-*)(-*)-*</term>
    /// </item>
    /// <item>
    /// <term>63</term><term>khs.sSeH</term><term>[SeD1H1]-*</term>
    /// </item>
    /// <item>
    /// <term>64</term><term>khs.dSe</term><term>[SeD1H0]=*</term>
    /// </item>
    /// <item>
    /// <term>65</term><term>khs.ssSe</term><term>[SeD2H0](-*)-*</term>
    /// </item>
    /// <item>
    /// <term>66</term><term>khs.aaSe</term><term>[SeD2H0](:*):*</term>
    /// </item>
    /// <item>
    /// <term>67</term><term>khs.dssSe</term><term>[SeD3H0](=*)(-*)-*</term>
    /// </item>
    /// <item>
    /// <term>68</term><term>khs.ddssSe</term><term>[SeD4H0](=*)(=*)(-*)-*</term>
    /// </item>
    /// <item>
    /// <term>69</term><term>khs.sBr</term><term>[BrD1]-*</term>
    /// </item>
    /// <item>
    /// <term>70</term><term>khs.sSnH3</term><term>[SnD1H3]-*</term>
    /// </item>
    /// <item>
    /// <term>71</term><term>khs.ssSnH2</term><term>[SnD2H2](-*)-*</term>
    /// </item>
    /// <item>
    /// <term>72</term><term>khs.sssSnH</term><term>[SnD3H1](-*)(-*)-*</term>
    /// </item>
    /// <item>
    /// <term>73</term><term>khs.ssssSn</term><term>[SnD4H0](-*)(-*)(-*)-*</term>
    /// </item>
    /// <item>
    /// <term>74</term><term>khs.sI</term><term>[ID1]-*</term>
    /// </item>
    /// <item>
    /// <term>75</term><term>khs.sPbH3</term><term>[PbD1H3]-*</term>
    /// </item>
    /// <item>
    /// <term>76</term><term>khs.ssPbH2</term><term>[PbD2H2](-*)-*</term>
    /// </item>
    /// <item>
    /// <term>77</term><term>khs.sssPbH</term><term>[PbD3H1](-*)(-*)-*</term>
    /// </item>
    /// <item>
    /// <term>78</term><term>khs.ssssPb</term><term>[PbD4H0](-*)(-*)(-*)-*</term>
    /// </item>
    /// </list>
    /// </remarks>
    // @author Rajarshi Guha
    // @cdk.module qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:kierHallSmarts
    public class KierHallSmartsDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private static string[] names;
        private static readonly IReadOnlyList<SmartsPattern> SMARTS = EStateFragments.Patterns;

        public KierHallSmartsDescriptor()
        {
            var tmp = EStateFragments.Names;
            names = new string[tmp.Count];
            for (int i = 0; i < tmp.Count; i++)
                names[i] = "khs." + tmp[i];
        }

        /// <inheritdoc/>
        public override IImplementationSpecification Specification => specification;
        private static readonly DescriptorSpecification specification =
         new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#kierHallSmarts",
                typeof(KierHallSmartsDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <inheritdoc/>
        public override IReadOnlyList<object> Parameters
        {
            get => null;
            set
            {
                if (value != null)
                    throw new CDKException("Must not supply any parameters");
            }
        }

        public override IReadOnlyList<string> DescriptorNames => names;

        private DescriptorValue<ArrayResult<int>> GetDummyDescriptorValue(Exception e)
        {
            ArrayResult<int> result = new ArrayResult<int>();
            foreach (var smart in SMARTS)
                result.Add(0);
            return new DescriptorValue<ArrayResult<int>>(specification, ParameterNames, Parameters, result, DescriptorNames, e);
        }

        /// <summary>
        /// This method calculates occurrences of the Kier &amp; Hall E-state fragments.
        /// </summary>
        /// <param name="container">The molecule for which this descriptor is to be calculated</param>
        /// <returns>Counts of the fragments</returns>
        public DescriptorValue<ArrayResult<int>> Calculate(IAtomContainer container)
        {
            if (container == null || container.Atoms.Count == 0)
            {
                return GetDummyDescriptorValue(new CDKException("Container was null or else had no atoms"));
            }

            var atomContainer = (IAtomContainer)container.Clone();
            atomContainer = AtomContainerManipulator.RemoveHydrogens(atomContainer);

            var counts = new int[SMARTS.Count];
            SmartsPattern.Prepare(atomContainer);
            for (int i = 0; i < SMARTS.Count; i++)
            {
                counts[i] = SMARTS[i].MatchAll(atomContainer).CountUnique();
            }

            var result = new ArrayResult<int>();
            foreach (var i in counts)
                result.Add(i);

            return new DescriptorValue<ArrayResult<int>>(specification, ParameterNames, Parameters, result, DescriptorNames);
        }

        /// <inheritdoc/>
        public override IDescriptorResult DescriptorResultType { get; } = new ArrayResult<int>(SMARTS.Count);

        /// <inheritdoc/>
        public override IReadOnlyList<string> ParameterNames => Array.Empty<string>();

        /// <inheritdoc/>
        public override object GetParameterType(string name) => null;

        IDescriptorValue IMolecularDescriptor.Calculate(IAtomContainer container) => Calculate(container);
    }
}
