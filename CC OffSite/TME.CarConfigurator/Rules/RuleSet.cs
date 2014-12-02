using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces.Rules;

namespace TME.CarConfigurator.Rules
{
    public class RuleSet : IRuleSet
    {
        private readonly Repository.Objects.Rules.RuleSet _ruleSet;

        public RuleSet(Repository.Objects.Rules.RuleSet ruleSet)
        {
            if (ruleSet == null) throw new ArgumentNullException("ruleSet");

            _ruleSet = ruleSet;
        }

        public IReadOnlyList<IEquipmentRule> Options { get { return _ruleSet.OptionRules.Select(a => new EquipmentRule(a)).ToList(); } }
        public IReadOnlyList<IEquipmentRule> Accessories { get { return _ruleSet.AccessoryRules.Select(o => new EquipmentRule(o)).ToList(); } } 
        public IReadOnlyList<IPackRule> Packs { get { return _ruleSet.PackRules.Select(p => new PackRule(p)).ToList(); }}
    }
}