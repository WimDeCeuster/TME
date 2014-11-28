using System;
using TME.CarConfigurator.Interfaces.Rules;

namespace TME.CarConfigurator.Rules
{
    public class RuleSets : IRuleSets
    {
        private readonly Repository.Objects.Rules.RuleSets _repositoryRuleSets;

        public RuleSets(Repository.Objects.Rules.RuleSets repositoryRuleSets)
        {
            if (repositoryRuleSets == null) throw new ArgumentNullException("repositoryRuleSets");
            _repositoryRuleSets = repositoryRuleSets;
        }

        public IRuleSet Include { get { return new RuleSet(_repositoryRuleSets.Include); } }
        public IRuleSet Exclude { get { return new RuleSet(_repositoryRuleSets.Exclude);} }
    }
}