using System.Collections.Generic;

namespace TME.CarConfigurator.Repository.Objects.Rules
{
    public class RuleSet
    {
        public RuleSet()
        {
            PackRules = new List<PackRule>();
            OptionRules = new List<EquipmentItemRule>();
            AccessoryRules = new List<EquipmentItemRule>();
        }

        public IReadOnlyList<PackRule> PackRules { get; set; }
        public IReadOnlyList<EquipmentItemRule> OptionRules { get; set; }
        public IReadOnlyList<EquipmentItemRule> AccessoryRules { get; set; }
    }
}