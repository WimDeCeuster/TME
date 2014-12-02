using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects.Rules;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class RuleSetBuilder
    {
        readonly RuleSet _ruleSet;

        public RuleSetBuilder()
        {
            _ruleSet = new RuleSet();
        }

        public RuleSetBuilder WithAccessoryRules(params EquipmentItemRule[] equipmentItemRules)
        {
            _ruleSet.AccessoryRules = new List<EquipmentItemRule>(equipmentItemRules);

            return this;    
        }

        public RuleSetBuilder WithOptionRules(params EquipmentItemRule[] equipmentItemRules)
        {
            _ruleSet.OptionRules = new List<EquipmentItemRule>(equipmentItemRules);

            return this;
        }
        
        public RuleSetBuilder WithPackRules(params PackRule[] packRules)
        {
            _ruleSet.PackRules = new List<PackRule>(packRules);

            return this;
        }

        public RuleSet Build()
        {
            return _ruleSet;
        }
    }
}