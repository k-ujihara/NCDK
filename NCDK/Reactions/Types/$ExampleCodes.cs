
namespace NCDK.Reactions.Types
{
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new AdductionProtonLPReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class AdductionProtonLPReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new AdductionProtonPBReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class AdductionProtonPBReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new AdductionSodiumLPReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class AdductionSodiumLPReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new CarbonylEliminationReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class CarbonylEliminationReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new ElectronImpactNBEReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class ElectronImpactNBEReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new ElectronImpactPDBReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class ElectronImpactPDBReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new ElectronImpactSDBReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class ElectronImpactSDBReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new HeterolyticCleavagePBReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class HeterolyticCleavagePBReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new HeterolyticCleavageSBReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class HeterolyticCleavageSBReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new HomolyticCleavageReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class HomolyticCleavageReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new HyperconjugationReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class HyperconjugationReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new PiBondingMovementReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class PiBondingMovementReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new RadicalChargeSiteInitiationHReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class RadicalChargeSiteInitiationHReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new RadicalChargeSiteInitiationReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class RadicalChargeSiteInitiationReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new RadicalSiteHrAlphaReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class RadicalSiteHrAlphaReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new RadicalSiteHrBetaReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class RadicalSiteHrBetaReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new RadicalSiteHrDeltaReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class RadicalSiteHrDeltaReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new RadicalSiteHrGammaReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class RadicalSiteHrGammaReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new RadicalSiteInitiationHReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class RadicalSiteInitiationHReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new RadicalSiteInitiationReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class RadicalSiteInitiationReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new RadicalSiteRrAlphaReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class RadicalSiteRrAlphaReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new RadicalSiteRrBetaReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class RadicalSiteRrBetaReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new RadicalSiteRrDeltaReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class RadicalSiteRrDeltaReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new RadicalSiteRrGammaReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class RadicalSiteRrGammaReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new RearrangementAnionReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class RearrangementAnionReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new RearrangementCationReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class RearrangementCationReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new RearrangementLonePairReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class RearrangementLonePairReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new RearrangementRadicalReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class RearrangementRadicalReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new SharingAnionReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class SharingAnionReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new SharingChargeDBReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class SharingChargeDBReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new SharingChargeSBReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class SharingChargeSBReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new SharingLonePairReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class SharingLonePairReaction {}
    /// <example>
    /// <code>
    /// var setOfReactants = ChemObjectBuilder.Instance.CreateAtomContainerSet&lt;IAtomContainer&gt;();
    /// setOfReactants.Add(molecular);
    /// var type = new TautomerizationReaction();
    /// IParameterReaction param = new SetReactionCenter();
    /// param.IsSetParameter = false;
    /// var paramList = new[] { param };
    /// type.ParameterList = paramList;
    /// IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    public partial class TautomerizationReaction {}
}
